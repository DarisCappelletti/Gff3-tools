using System.Text;
using Gff3Tools.Models;
using OfficeOpenXml;

namespace Gff3Tools.Services;

/// <summary>Esportazioni della tabella: GFF3, CDS e Excel.</summary>
public static class EsportatoreGff3
{
    public static string ToGff3Text(List<BioTizio> righe)
    {
        var sb = new StringBuilder("##gff-version 3\n");

        foreach (var riga in righe)
        {
            sb.Append($"{riga.Sequid}\t{riga.Source}\t{riga.Type}\t{riga.Start}\t{riga.End}\t{riga.Score}\t{riga.Strand}\t{riga.Phase}\t{string.Join(";", riga.Attributes)}\n");
        }

        return sb.ToString();
    }

    public static string ToCdsText(List<BioTizio> righe)
    {
        var blocchi = righe
            .Select(r => r.CDS)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct();

        return string.Join("", blocchi);
    }

    public static byte[] ToExcel(List<BioTizio> righe)
    {
        // Licenza NonCommercial personale di EPPlus 8 (lo stesso contesto del tool originale)
        ExcelPackage.License.SetNonCommercialPersonal("Daris Cappelletti");

        using var package = new ExcelPackage();
        var foglio = package.Workbook.Worksheets.Add("Dati");

        var colonne = new (string Titolo, Func<BioTizio, string> Valore)[]
        {
            ("File", r => r.File),
            ("Fasta", r => r.Fasta),
            ("CDS", r => r.CDS),
            ("Sequid", r => r.Sequid),
            ("Source", r => r.Source),
            ("Type", r => r.Type),
            ("Start", r => r.Start),
            ("End", r => r.End),
            ("Score", r => r.Score),
            ("Strand", r => r.Strand),
            ("Phase", r => r.Phase),
            ("Attributes", r => string.Join(", ", r.Attributes)),
        };

        for (var i = 0; i < colonne.Length; i++)
        {
            foglio.Cells[1, i + 1].Value = colonne[i].Titolo;
        }

        for (var riga = 0; riga < righe.Count; riga++)
        {
            for (var colonna = 0; colonna < colonne.Length; colonna++)
            {
                foglio.Cells[riga + 2, colonna + 1].Value = colonne[colonna].Valore(righe[riga]);
            }
        }

        foglio.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }
}