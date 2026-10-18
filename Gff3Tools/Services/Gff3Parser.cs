using Gff3Tools.Models;

namespace Gff3Tools.Services;

/// <summary>Lettura dei file GFF3: ogni riga di dato (non commento, non vuota) diventa una <see cref="BioTizio"/>.</summary>
public static class Gff3Parser
{
    public static List<BioTizio> Parse(Stream stream, string nomeFile)
    {
        var righe = new List<BioTizio>();

        using var reader = new StreamReader(stream);
        string? riga;
        while ((riga = reader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(riga) || riga.StartsWith("#"))
            {
                continue;
            }

            var campi = riga.Split('\t');
            if (campi.Length < 9)
            {
                continue; // riga malformata: la si salta, come nel tool originale
            }

            righe.Add(new BioTizio
            {
                File = nomeFile,
                Sequid = campi[0],
                Source = campi[1],
                Type = campi[2],
                Start = campi[3],
                End = campi[4],
                Score = campi[5],
                Strand = campi[6],
                Phase = campi[7],
                Attributes = campi[8].Split(';'),
            });
        }

        return righe;
    }
}