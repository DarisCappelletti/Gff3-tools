using Gff3Tools.Models;

namespace Gff3Tools.Services;

/// <summary>Filtri "contiene / non contiene" del tool, con parole separate da ';' e ricerca per colonna.</summary>
public static class FiltriGff3
{
    /// <summary>Applica i filtri: ogni parola di "contiene" deve trovarsi in almeno una colonna attiva
    /// (le parole sono in AND tra loro), ogni parola di "non contiene" deve essere assente da tutte le colonne attive.</summary>
    public static List<BioTizio> Applica(IEnumerable<BioTizio> dati, string contiene, string nonContiene, ColonneRicerca colonne)
    {
        IEnumerable<BioTizio> query = dati;

        if (!string.IsNullOrWhiteSpace(contiene))
        {
            foreach (var parola in contiene.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                query = query.Where(riga => ContieneParola(riga, parola, colonne));
            }
        }

        if (!string.IsNullOrWhiteSpace(nonContiene))
        {
            foreach (var parola in nonContiene.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                query = query.Where(riga => !ContieneParola(riga, parola, colonne));
            }
        }

        return query.ToList();
    }

    private static bool ContieneParola(BioTizio riga, string parola, ColonneRicerca colonne)
    {
        var p = parola.ToLowerInvariant();

        return (colonne.File && riga.File.ToLowerInvariant().Contains(p))
            || (colonne.Attributes && riga.Attributes.Any(a => a.ToLowerInvariant().Contains(p)))
            || (colonne.Sequid && riga.Sequid.ToLowerInvariant().Contains(p))
            || (colonne.Source && riga.Source.ToLowerInvariant().Contains(p))
            || (colonne.Type && riga.Type.ToLowerInvariant().Contains(p))
            || (colonne.Start && riga.Start.ToLowerInvariant().Contains(p))
            || (colonne.End && riga.End.ToLowerInvariant().Contains(p))
            || (colonne.Score && riga.Score.ToLowerInvariant().Contains(p))
            || (colonne.Strand && riga.Strand.ToLowerInvariant().Contains(p))
            || (colonne.Phase && riga.Phase.ToLowerInvariant().Contains(p));
    }
}

/// <summary>Ordinamento della tabella per campo, crescente o decrescente.</summary>
public static class OrdinamentoGff3
{
    public static List<BioTizio> Ordina(IEnumerable<BioTizio> righe, string? campo, bool discendente)
    {
        var dati = righe.ToList();
        if (dati.Count == 0 || string.IsNullOrEmpty(campo))
        {
            return dati;
        }

        Func<BioTizio, string> chiave = campo.ToLowerInvariant() switch
        {
            "file" => r => r.File,
            "sequid" => r => r.Sequid,
            "source" => r => r.Source,
            "type" => r => r.Type,
            "start" => r => r.Start,
            "end" => r => r.End,
            "score" => r => r.Score,
            "strand" => r => r.Strand,
            "phase" => r => r.Phase,
            "attributes" => r => r.Attributes.Length > 0 ? r.Attributes[0] : "",
            _ => r => "",
        };

        return discendente
            ? dati.OrderByDescending(chiave).ToList()
            : dati.OrderBy(chiave).ToList();
    }
}