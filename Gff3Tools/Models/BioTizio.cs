namespace Gff3Tools.Models;

/// <summary>Una riga di un file GFF3: i 9 campi del formato, il file di provenienza
/// e le sequenze FASTA/CDS eventualmente collegate in un secondo momento.</summary>
public record BioTizio
{
    public string File { get; init; } = "";
    public string Fasta { get; set; } = "";
    public string CDS { get; set; } = "";
    public string Sequid { get; init; } = "";
    public string Source { get; init; } = "";
    public string Type { get; init; } = "";
    public string Start { get; init; } = "";
    public string End { get; init; } = "";
    public string Score { get; init; } = "";
    public string Strand { get; init; } = "";
    public string Phase { get; init; } = "";
    public string[] Attributes { get; init; } = [];
}

/// <summary>Colonne su cui applicare i filtri di ricerca (tutte attive per default).</summary>
public class ColonneRicerca
{
    public bool File { get; set; } = true;
    public bool Sequid { get; set; } = true;
    public bool Source { get; set; } = true;
    public bool Type { get; set; } = true;
    public bool Start { get; set; } = true;
    public bool End { get; set; } = true;
    public bool Score { get; set; } = true;
    public bool Strand { get; set; } = true;
    public bool Phase { get; set; } = true;
    public bool Attributes { get; set; } = true;
}