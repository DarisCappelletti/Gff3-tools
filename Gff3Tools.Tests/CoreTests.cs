using System.Text;
using Gff3Tools.Models;
using Gff3Tools.Services;

namespace Gff3Tools.Tests;

public class Gff3ParserTest
{
    private const string Gff3DiEsempio =
        "##gff-version 3\n" +
        "##sequence-region ctgA 1 1000\n" +
        "ctgA\tGeneMark\tgene\t100\t200\t.\t+\t.\tID=gene1;Name=foo\n" +
        "# una riga di commento\n" +
        "ctgA\tPredictor\tCDS\t120\t180\t0.98\t+\t0\tParent=gene1\n";

    private static List<BioTizio> ParseEsempio()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Gff3DiEsempio));
        return Gff3Parser.Parse(stream, "esempio.gff3");
    }

    [Fact]
    public void SaltaCommentiEVuote()
    {
        var righe = ParseEsempio();
        Assert.Equal(2, righe.Count);
    }

    [Fact]
    public void LeggeICampiDellaRiga()
    {
        var riga = ParseEsempio()[0];
        Assert.Equal("esempio.gff3", riga.File);
        Assert.Equal("ctgA", riga.Sequid);
        Assert.Equal("GeneMark", riga.Source);
        Assert.Equal("gene", riga.Type);
        Assert.Equal("100", riga.Start);
        Assert.Equal("200", riga.End);
        Assert.Equal(".", riga.Score);
        Assert.Equal("+", riga.Strand);
        Assert.Equal(".", riga.Phase);
        Assert.Equal(["ID=gene1", "Name=foo"], riga.Attributes);
    }

    [Fact]
    public void SaltaLeRigheMalformate()
    {
        var testo = "ctgA\tsolo\ttre\tcampi\nctgA\tGeneMark\tgene\t1\t2\t.\t+\t.\tID=x\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(testo));
        var righe = Gff3Parser.Parse(stream, "x.gff3");
        var riga = Assert.Single(righe);
        Assert.Equal("ID=x", riga.Attributes[0]);
    }
}

public class FiltriGff3Test
{
    private static readonly List<BioTizio> Dati =
    [
        new() { File = "a.gff3", Sequid = "ctgA", Source = "GeneMark", Type = "gene", Start = "100", End = "200", Score = ".", Strand = "+", Phase = ".", Attributes = ["ID=gene1", "Name=foo"] },
        new() { File = "b.gff3", Sequid = "ctgB", Source = "Predictor", Type = "CDS", Start = "120", End = "180", Score = "0.98", Strand = "-", Phase = "0", Attributes = ["Parent=gene1"] },
    ];

    [Fact]
    public void ContieneCercaInTutteLeColonne()
    {
        var risultato = FiltriGff3.Applica(Dati, "genemark", "", new ColonneRicerca());
        Assert.Single(risultato);
        Assert.Equal("ctgA", risultato[0].Sequid);
    }

    [Fact]
    public void ContieneConPiuParoleEFraLoro()
    {
        var risultato = FiltriGff3.Applica(Dati, "ctgA;gene", "", new ColonneRicerca());
        Assert.Single(risultato);
    }

    [Fact]
    public void NonContieneEsclude()
    {
        var risultato = FiltriGff3.Applica(Dati, "", "predictor", new ColonneRicerca());
        Assert.Single(risultato);
        Assert.Equal("ctgA", risultato[0].Sequid);
    }

    [Fact]
    public void ColonneDisattivateNonVengonoCercate()
    {
        var colonne = new ColonneRicerca { Source = false, Type = false, Attributes = false };
        var risultato = FiltriGff3.Applica(Dati, "gene", "", colonne);
        Assert.Empty(risultato); // "gene" appare solo in Source/Type/Attributes, disattivate
    }
}

public class SequidMatcherTest
{
    private static List<BioTizio> Dati() =>
    [
        new() { Sequid = "MMETSP0013|k141_1" },
        new() { Sequid = "MMETSP0014|k141_2" },
    ];

    private static Stream StreamDi(string contenuto) => new MemoryStream(Encoding.UTF8.GetBytes(contenuto));

    [Fact]
    public void CollegaFastaPerPrefissoSequid()
    {
        var dati = Dati();
        var fasta = ">trinity|gnl|MMETSP0013|k141_1\nATGCGT\n>altro|XX|YY|nonpresente\nTTTT\n";

        SequidMatcher.CollegaFasta(dati, [StreamDi(fasta)]);

        Assert.Contains("ATGCGT", dati[0].Fasta);
        Assert.Equal("", dati[1].Fasta);
    }

    [Fact]
    public void CollegaCdsPerSequidEsatto()
    {
        var dati = Dati();
        var cds = ">MMETSP0014|k141_2 gene xyz\nATTTTT\nAAAA\n>inesistente\nCCC\n";

        SequidMatcher.CollegaCds(dati, [StreamDi(cds)]);

        Assert.Contains("ATTTTT", dati[1].CDS);
        Assert.EndsWith("\nAAAA\n", dati[1].CDS);
        Assert.Equal("", dati[0].CDS);
    }
}

public class EsportatoreGff3Test
{
    private static readonly List<BioTizio> Dati =
    [
        new() { Sequid = "ctgA", Source = "GeneMark", Type = "gene", Start = "100", End = "200", Score = ".", Strand = "+", Phase = ".", Attributes = ["ID=gene1"] },
    ];

    [Fact]
    public void IlGff3EsportatoHaIntestazioneERighe()
    {
        var testo = EsportatoreGff3.ToGff3Text(Dati);
        Assert.StartsWith("##gff-version 3\n", testo);
        Assert.Contains("ctgA\tGeneMark\tgene\t100\t200\t.\t+\t.\tID=gene1\n", testo);
    }

    [Fact]
    public void IlCdsEsportatoUnisceSoloLeRigheConSequenza()
    {
        var conCds = Dati.ToList();
        conCds[0] = conCds[0] with { CDS = ">seq\nATG\n" };
        var testo = EsportatoreGff3.ToCdsText(conCds);
        Assert.Equal(">seq\nATG\n", testo);
    }

    [Fact]
    public void LesportazioneExcelProduceUnFoglioLeggibile()
    {
        var bytes = EsportatoreGff3.ToExcel(Dati);
        Assert.True(bytes.Length > 0);

        using var package = new OfficeOpenXml.ExcelPackage(new MemoryStream(bytes));
        var foglio = package.Workbook.Worksheets["Dati"];
        Assert.Equal("Sequid", foglio.Cells[1, 4].Value);
        Assert.Equal("ctgA", foglio.Cells[2, 4].Value);
    }
}

public class OrdinamentoGff3Test
{
    private static readonly List<BioTizio> Dati =
    [
        new() { Start = "200" },
        new() { Start = "50" },
        new() { Start = "100" },
    ];

    [Fact]
    public void OrdinaPerStart()
    {
        // Come nell'originale, Start è una stringa: l'ordine è lessicale, non numerico
        var crescente = OrdinamentoGff3.Ordina(Dati, "start", discendente: false);
        Assert.Equal(["100", "200", "50"], crescente.Select(r => r.Start));

        var decrescente = OrdinamentoGff3.Ordina(Dati, "start", discendente: true);
        Assert.Equal(["50", "200", "100"], decrescente.Select(r => r.Start));
    }
}