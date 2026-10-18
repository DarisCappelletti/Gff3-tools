using Gff3Tools.Models;

namespace Gff3Tools.Services;

/// <summary>Collega le sequenze di file FASTA e CDS alle righe GFF3, cercando il corrispondente sequid nell'intestazione ">".</summary>
public static class SequidMatcher
{
    /// <summary>FASTA: l'intestazione è tipo ">a|b|c|d": il sequid è "c|d" e viene abbinato
    /// alle righe il cui Sequid lo contiene (come nel tool originale).</summary>
    public static void CollegaFasta(List<BioTizio> dati, IEnumerable<Stream> fileFasta)
    {
        foreach (var stream in fileFasta)
        {
            using var reader = new StreamReader(stream);
            string? riga;
            var dentroSequenza = false;
            var sequidCorrente = "";
            var blocco = "";

            while ((riga = reader.ReadLine()) != null)
            {
                if (riga.StartsWith(">"))
                {
                    ChiudiBlocco(dati, dentroSequenza, sequidCorrente, blocco, (d, b) => d.Fasta = b);
                    dentroSequenza = false;
                    blocco = "";

                    sequidCorrente = EstraiSequidFasta(riga);
                    if (sequidCorrente.Length > 0 && dati.Any(d => d.Sequid.Contains(sequidCorrente)))
                    {
                        dentroSequenza = true;
                        blocco = riga;
                    }
                }
                else if (dentroSequenza)
                {
                    blocco += riga;
                }
            }

            ChiudiBlocco(dati, dentroSequenza, sequidCorrente, blocco, (d, b) => d.Fasta = b);
        }
    }

    /// <summary>CDS: il sequid è il primo token dell'intestazione (fino allo spazio) e deve
    /// coincidere esattamente con lo Sequid della riga.</summary>
    public static void CollegaCds(List<BioTizio> dati, IEnumerable<Stream> fileCds)
    {
        foreach (var stream in fileCds)
        {
            using var reader = new StreamReader(stream);
            string? riga;
            var dentroSequenza = false;
            var sequidCorrente = "";
            var blocco = "";

            while ((riga = reader.ReadLine()) != null)
            {
                if (riga.StartsWith(">"))
                {
                    ChiudiBlocco(dati, dentroSequenza, sequidCorrente, blocco, (d, b) => d.CDS = b);
                    dentroSequenza = false;
                    blocco = "";

                    var intestazione = riga.TrimStart('>');
                    var sequid = intestazione.Split(' ')[0];

                    if (dati.Any(d => d.Sequid == sequid))
                    {
                        dentroSequenza = true;
                        sequidCorrente = sequid;
                        blocco = riga + "\n";
                    }
                }
                else if (dentroSequenza)
                {
                    blocco += riga + "\n";
                }
            }

            ChiudiBlocco(dati, dentroSequenza, sequidCorrente, blocco, (d, b) => d.CDS = b);
        }
    }

    private static string EstraiSequidFasta(string intestazione)
    {
        var parti = intestazione.Split('|');
        if (parti.Length < 4)
        {
            return "";
        }

        return $"{parti[2]}|{parti[3]}";
    }

    private static void ChiudiBlocco(List<BioTizio> dati, bool dentroSequenza, string sequid, string blocco, Action<BioTizio, string> assegna)
    {
        if (!dentroSequenza)
        {
            return;
        }

        foreach (var dato in dati)
        {
            if (dato.Sequid.Contains(sequid))
            {
                assegna(dato, blocco);
            }
        }
    }
}