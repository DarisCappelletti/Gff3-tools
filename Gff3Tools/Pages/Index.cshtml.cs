using System.Text.Json;
using Gff3Tools.Models;
using Gff3Tools.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gff3Tools.Pages;

/// <summary>Stato della tabella che sopravvive ai postback (filtri, colonne, ordinamento, paginazione).</summary>
public class StatoTabella
{
    public static readonly string[] TutteLeColonne = ["File", "Sequid", "Source", "Type", "Start", "End", "Score", "Strand", "Phase", "Attributes"];

    public string Contiene { get; set; } = "";
    public string NonContiene { get; set; } = "";
    public string[] ColonneRicerca { get; set; } = [.. TutteLeColonne];
    public string[] ColonneVisibili { get; set; } = [.. TutteLeColonne];
    public string? Ordina { get; set; }
    public bool Discendente { get; set; }
    public int Pagina { get; set; } = 1;
    public int PerPagina { get; set; } = 100;

    public ColonneRicerca ComeColonneRicerca()
    {
        return new ColonneRicerca
        {
            File = ColonneRicerca.Contains("File"),
            Sequid = ColonneRicerca.Contains("Sequid"),
            Source = ColonneRicerca.Contains("Source"),
            Type = ColonneRicerca.Contains("Type"),
            Start = ColonneRicerca.Contains("Start"),
            End = ColonneRicerca.Contains("End"),
            Score = ColonneRicerca.Contains("Score"),
            Strand = ColonneRicerca.Contains("Strand"),
            Phase = ColonneRicerca.Contains("Phase"),
            Attributes = ColonneRicerca.Contains("Attributes"),
        };
    }
}

public class IndexModel : PageModel
{
    private const string ChiaveDati = "Gff3.Dati";
    private const string ChiaveNomiFile = "Gff3.NomiFile";
    private const string ChiaveStato = "Gff3.Stato";

    public List<BioTizio> Dati { get; private set; } = [];
    public List<string> NomiFile { get; private set; } = [];
    public StatoTabella Stato { get; private set; } = new();

    public List<BioTizio> RigheFiltrate { get; private set; } = [];
    public List<BioTizio> RighePagina { get; private set; } = [];

    public int TotalePagine => RigheFiltrate.Count == 0 ? 0 : (int)Math.Ceiling(RigheFiltrate.Count / (double)Stato.PerPagina);

    [BindProperty] public List<IFormFile>? FileGff3 { get; set; }
    [BindProperty] public List<IFormFile>? FileFasta { get; set; }
    [BindProperty] public List<IFormFile>? FileCds { get; set; }

    public void OnGet(int pagina = 1, string? ordina = null, string? dir = null)
    {
        CaricaStato();

        if (ordina is not null)
        {
            if (Stato.Ordina == ordina)
            {
                Stato.Discendente = !Stato.Discendente;
            }
            else
            {
                Stato.Ordina = ordina;
                Stato.Discendente = dir == "desc";
            }
            Stato.Pagina = 1;
        }

        Stato.Pagina = Math.Max(1, pagina);
        SalvaStato();
        Elabora();
    }

    public IActionResult OnGetColonna(string nome)
    {
        CaricaStato();

        // Mostra/nascondi colonna della tabella
        if (Stato.ColonneVisibili.Contains(nome))
        {
            Stato.ColonneVisibili = [.. Stato.ColonneVisibili.Where(c => c != nome)];
        }
        else
        {
            Stato.ColonneVisibili = [.. Stato.ColonneVisibili, nome];
        }

        SalvaStato();
        return RedirectToPage("Index");
    }

    public IActionResult OnGetAzzera()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("Index");
    }

    public IActionResult OnGetPerPagina(int perPagina)
    {
        CaricaStato();
        Stato.PerPagina = Math.Clamp(perPagina, 10, 1000);
        Stato.Pagina = 1;
        SalvaStato();
        return RedirectToPage("Index");
    }

    public IActionResult OnGetAzzeraFiltri()
    {
        CaricaStato();
        Stato.Contiene = "";
        Stato.NonContiene = "";
        Stato.ColonneRicerca = [.. StatoTabella.TutteLeColonne];
        Stato.Pagina = 1;
        SalvaStato();
        return RedirectToPage("Index");
    }

    public IActionResult OnPostFiltri(string contiene, string nonContiene, string[]? colonne)
    {
        CaricaStato();
        Stato.Contiene = contiene ?? "";
        Stato.NonContiene = nonContiene ?? "";
        Stato.ColonneRicerca = colonne is null || colonne.Length == 0 ? [.. StatoTabella.TutteLeColonne] : colonne;
        Stato.Pagina = 1;
        SalvaStato();
        return RedirectToPage("Index");
    }

    public IActionResult OnPostCaricaGff3()
    {
        if (FileGff3 is { Count: > 0 })
        {
            CaricaStato();

            foreach (var file in FileGff3)
            {
                using var stream = file.OpenReadStream();
                Dati.AddRange(Gff3Parser.Parse(stream, file.FileName));
                NomiFile.Add(Path.GetFileNameWithoutExtension(file.FileName));
            }

            Stato.Pagina = 1;
            SalvaDati();
            SalvaStato();
        }

        return RedirectToPage("Index");
    }

    public IActionResult OnPostCaricaFasta()
    {
        if (FileFasta is { Count: > 0 })
        {
            CaricaDati();
            SequidMatcher.CollegaFasta(Dati, FileFasta.Select(ApriStream));
            SalvaDati();
        }

        return RedirectToPage("Index");
    }

    public IActionResult OnPostCaricaCds()
    {
        if (FileCds is { Count: > 0 })
        {
            CaricaDati();
            SequidMatcher.CollegaCds(Dati, FileCds.Select(ApriStream));
            SalvaDati();
        }

        return RedirectToPage("Index");
    }

    public IActionResult OnPostEsporta(string formato, List<int>? righe)
    {
        CaricaStato();

        var listaFinale = RigheElaborate();
        if (righe is { Count: > 0 })
        {
            // Selezione per indice nella lista elaborata (filtrata e ordinata)
            var indici = righe.ToHashSet();
            listaFinale = listaFinale.Where((_, i) => indici.Contains(i)).ToList();
        }

        if (listaFinale.Count == 0)
        {
            Elabora();
            return RedirectToPage("Index");
        }

        var data = DateTime.Now.ToString("yyyy-MM-dd");
        return formato switch
        {
            "gff3" => File(System.Text.Encoding.UTF8.GetBytes(EsportatoreGff3.ToGff3Text(listaFinale)), "text/plain", $"gff3-tools-{data}.gff3"),
            "cds" => File(System.Text.Encoding.UTF8.GetBytes(EsportatoreGff3.ToCdsText(listaFinale)), "text/plain", $"gff3-tools-{data}.cds.fa"),
            "excel" => File(EsportatoreGff3.ToExcel(listaFinale), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"gff3-tools-{data}.xlsx"),
            _ => RedirectToPage("Index"),
        };
    }

    private static Stream ApriStream(IFormFile file) => file.OpenReadStream();

    /// <summary>Lista dopo filtri e ordinamento: è quella che si vede in pagina e quella che si esporta.</summary>
    private List<BioTizio> RigheElaborate()
    {
        var filtrate = FiltriGff3.Applica(Dati, Stato.Contiene, Stato.NonContiene, Stato.ComeColonneRicerca());
        return OrdinamentoGff3.Ordina(filtrate, Stato.Ordina, Stato.Discendente);
    }

    private void Elabora()
    {
        RigheFiltrate = RigheElaborate();
        var salta = (Stato.Pagina - 1) * Stato.PerPagina;
        RighePagina = RigheFiltrate.Skip(salta).Take(Stato.PerPagina).ToList();
    }

    private void CaricaDati()
    {
        Dati = HttpContext.Session.GetString(ChiaveDati) is { } json
            ? JsonSerializer.Deserialize<List<BioTizio>>(json) ?? []
            : [];
        NomiFile = HttpContext.Session.GetString(ChiaveNomiFile) is { } nomiJson
            ? JsonSerializer.Deserialize<List<string>>(nomiJson) ?? []
            : [];
    }

    private void SalvaDati()
    {
        HttpContext.Session.SetString(ChiaveDati, JsonSerializer.Serialize(Dati));
        HttpContext.Session.SetString(ChiaveNomiFile, JsonSerializer.Serialize(NomiFile));
    }

    private void CaricaStato()
    {
        CaricaDati();
        Stato = HttpContext.Session.GetString(ChiaveStato) is { } json
            ? JsonSerializer.Deserialize<StatoTabella>(json) ?? new()
            : new();
    }

    private void SalvaStato()
    {
        HttpContext.Session.SetString(ChiaveStato, JsonSerializer.Serialize(Stato));
    }
}