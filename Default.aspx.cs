﻿using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gff3_tools
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Imposta il contesto di licenza su NonCommercial

                List<BioTizio> dati = Session["data"] as List<BioTizio>;
                if (dati != null)
                {
                    if (Session["contiene"] != null)
                    {
                        valoriContiene.Value = Session["contiene"].ToString();
                    }
                    if (Session["noncontiene"] != null)
                    {
                        valoriNonContiene.Value = Session["noncontiene"].ToString();
                    }

                    aggiornaTabella(dati);
                }
                visualizzaFileCaricato(false);
            }
        }

        protected void ImportCSV(object sender, EventArgs e)
        {
            //Create a DataTable.
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[12] {
            new DataColumn("File", typeof(string)),
            new DataColumn("Fasta", typeof(string)),
            new DataColumn("CDS", typeof(string)),
            new DataColumn("Sequid", typeof(string)),
            new DataColumn("Source", typeof(string)),
            new DataColumn("Type", typeof(string)),
            new DataColumn("Start", typeof(string)),
            new DataColumn("End", typeof(string)),
            new DataColumn("Score", typeof(string)),
            new DataColumn("Strand", typeof(string)),
            new DataColumn("Phase", typeof(string)),
            new DataColumn("Attributes",typeof(string)) });

            var listaTizi = new List<BioTizio>();

            //Read the contents of CSV file.

            //string csvPath;
            //using (StreamReader inputStreamReader = new StreamReader(fileCaricato.PostedFile.InputStream))
            //{
            //    csvPath = inputStreamReader.ReadToEnd();
            //}

            var listaFile = new List<string>();

            if (fileCaricato.HasFiles)
            {
                foreach (HttpPostedFile uploadedFile in fileCaricato.PostedFiles)
                {
                    listaFile.Add(uploadedFile.FileName);

                    StreamReader inputStreamReader = new StreamReader(uploadedFile.InputStream);
                    string streamFile = inputStreamReader.ReadToEnd();

                    //Execute a loop over the rows.
                    foreach (string row in streamFile.Split('\n'))
                    {
                        // Se la riga non è vuota e non è un commento la aggiungo
                        if (!string.IsNullOrEmpty(row) && !row.StartsWith("#"))
                        {
                            var tizio = new BioTizio();

                            tizio.File = uploadedFile.FileName;
                            tizio.Fasta = "";

                            int i = 0;
                            //Execute a loop over the columns.
                            foreach (string cell in row.Split('\t'))
                            {
                                if (i == 0)
                                {
                                    tizio.Sequid = cell;
                                }
                                else if (i == 1)
                                {
                                    tizio.Source = cell;
                                }
                                else if (i == 2)
                                {
                                    tizio.Type = cell;
                                }
                                else if (i == 3)
                                {
                                    tizio.Start = cell;
                                }
                                else if (i == 4)
                                {
                                    tizio.End = cell;
                                }
                                else if (i == 5)
                                {
                                    tizio.Score = cell;
                                }
                                else if (i == 6)
                                {
                                    tizio.Strand = cell;
                                }
                                else if (i == 7)
                                {
                                    tizio.Phase = cell;
                                }
                                else if (i == 8)
                                {
                                    tizio.Attributes = cell.Split(';');
                                }
                                i++;
                            }

                            listaTizi.Add(tizio);
                        }
                    }
                }
            }

            Session["nomeFile"] = listaFile;

            Session["data"] = listaTizi;

            //Bind the DataTable.
            aggiornaTabella(listaTizi);
            visualizzaFileCaricato(true);
        }

        public List<SearchResult> SearchSamplesIMicrobe(string valoreDaRicercare)
        {
            try
            {
                return APISearchSamplesIMicrobe(valoreDaRicercare);
            }
            catch
            {
                return APISearchSamplesIMicrobe(valoreDaRicercare);
            }
        }

        public List<SearchResult> APISearchSamplesIMicrobe(string valoreDaRicercare)
        {
            string api = $"https://www.imicrobe.us/api/v1/search/{valoreDaRicercare}";
            var request = (HttpWebRequest)WebRequest.Create(api);
            try
            {
                request.Timeout = 5000;
                request.ContentType = "application/json";
                request.Method = "GET";

                WebResponse response = request.GetResponse() as HttpWebResponse;
                var stream = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream);
                // Leggo la risposta.
                string stringaRisultato = reader2.ReadToEnd();

                if (String.IsNullOrWhiteSpace(stringaRisultato))
                {
                    return null;
                }
                else
                {
                    var risultati = JsonConvert.DeserializeObject<List<SearchResult>>(stringaRisultato);
                    return risultati != null ? risultati.Where(x => x.table_name == "sample").ToList() : null;
                }
            }
            catch
            {
                request.Abort();

                throw;
            }
        }

        public RootSample GetSampleInfoIMicrobe(int id)
        {
            try
            {
                return APISampleInfoIMicrobe(id);
            }
            catch
            {
                return APISampleInfoIMicrobe(id);
            }
        }

        public RootSample APISampleInfoIMicrobe(int id)
        {
            try
            {
                string api = $"https://www.imicrobe.us/api/v1/samples/{id}";
                var request = (HttpWebRequest)WebRequest.Create(api);
                request.ContentType = "application/json";
                request.Method = "GET";

                WebResponse response = request.GetResponse() as HttpWebResponse;
                var stream = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream);
                // Leggo la risposta.
                string stringaRisultato = reader2.ReadToEnd();

                if (String.IsNullOrWhiteSpace(stringaRisultato))
                {
                    return null;
                }
                else
                {
                    var sample = JsonConvert.DeserializeObject<RootSample>(stringaRisultato);
                    return sample;
                }
            }
            catch
            {
                throw;
            }
        }

        protected void ImportFasta(object sender, EventArgs e)
        {
            List<BioTizio> dati = Session["data"] as List<BioTizio>;

            var listaFile = new List<string>();

            if (fileCaricatoFasta.HasFiles)
            {
                foreach (HttpPostedFile uploadedFile in fileCaricatoFasta.PostedFiles)
                {
                    listaFile.Add(uploadedFile.FileName);

                    StreamReader inputStreamReader = new StreamReader(uploadedFile.InputStream);
                    string streamFile = inputStreamReader.ReadToEnd();

                    //Execute a loop over the rows.
                    foreach (string row in streamFile.Split('>'))
                    {
                        // Se la riga non è vuota e non è un commento la aggiungo
                        if (!string.IsNullOrEmpty(row) && !row.StartsWith("#"))
                        {
                            // splitto i valori per prendere la prima riga dove è presente il sequid
                            var split = row.Split('\n');

                            var split2 = split[0].Split('|');
                            //dati = dati.Where(x => x.Sequid.Contains($"{split2[2]}|{split2[3]}")).Select(x => { x.Fasta = row; return x; }).ToList();
                            dati.Where(x => x.Sequid.Split('_')[0] == $"{split2[2]}|{split2[3]}").ToList().ForEach(x => x.Fasta = row);

                        }
                    }
                }
            }

            Session["data"] = dati;

            //Bind the DataTable.
            aggiornaTabella(dati);
            visualizzaFileCaricato(false);
        }

        protected void ImportCDS(object sender, EventArgs e)
        {
            List<BioTizio> dati = Session["data"] as List<BioTizio>;

            var listaFile = new List<string>();

            if (fileCaricatoCDS.HasFiles)
            {
                foreach (HttpPostedFile uploadedFile in fileCaricatoCDS.PostedFiles)
                {
                    listaFile.Add(uploadedFile.FileName);

                    StreamReader inputStreamReader = new StreamReader(uploadedFile.InputStream);
                    string streamFile = inputStreamReader.ReadToEnd();

                    //Execute a loop over the rows.
                    foreach (string row in streamFile.Split('>'))
                    {
                        // Se la riga non è vuota e non è un commento la aggiungo
                        if (!string.IsNullOrEmpty(row) && !row.StartsWith("#"))
                        {
                            // splitto i valori per prendere la prima riga dove è presente il sequid
                            var split = row.Split('\n');

                            var split2 = split[0].Split(' ');
                            //dati = dati.Where(x => x.Sequid.Contains($"{split2[2]}|{split2[3]}")).Select(x => { x.Fasta = row; return x; }).ToList();
                            dati.Where(x => x.Sequid == split2[0]).ToList().ForEach(x => x.CDS = row);

                        }
                    }
                }
            }

            Session["data"] = dati;

            //Bind the DataTable.
            aggiornaTabella(dati);
            visualizzaFileCaricato(false);
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            var dati = impostaFiltri();
            aggiornaTabella(dati);
        }

        public List<BioTizio> impostaFiltri()
        {
            List<BioTizio> dati = Session["data"] as List<BioTizio>;

            if (dati != null)
            {
                var contiene = valoriContiene.Value;
                var noncontiene = valoriNonContiene.Value;

                var colAttive = impostaColonneRicerca();

                IEnumerable<BioTizio> query = dati;
                //(prova as DataTable).DefaultView.RowFilter = string.Format("Attributes = '{0}'", txtRicerca.Text);
                if (contiene != "")
                {
                    Session["contiene"] = contiene;
                    var listaParole = contiene.Split(';');
                    int conteggioParole = listaParole.Count();
                    foreach (var parola in listaParole)
                    {
                        query =
                            query.Where(x =>
                                    colAttive.inFile && x.File.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inAttributes && x.Attributes.Any(y => y.ToLower().Contains(parola.ToLower())) ||
                                    colAttive.inSequid && x.Sequid.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inSource && x.Source.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inType && x.Type.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inStart && x.Start.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inEnd && x.End.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inScore && x.Score.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inStrand && x.Strand.ToLower().Contains(parola.ToLower()) ||
                                    colAttive.inPhase && x.Phase.ToLower().Contains(parola.ToLower())
                                );
                    }
                }
                if (noncontiene.Trim() != "")
                {
                    Session["noncontiene"] = noncontiene;

                    var listaParole = noncontiene.Split(';');
                    foreach (var parola in listaParole)
                    {
                        query =
                            query.Where(x =>
                                    (!colAttive.inFile || !x.File.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inAttributes || !x.Attributes.Any(y => y.ToLower().Contains(parola.ToLower()))) &&
                                    (!colAttive.inSequid || !x.Sequid.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inSource || !x.Source.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inType || !x.Type.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inStart || !x.Start.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inEnd || !x.End.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inScore || !x.Score.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inStrand || !x.Strand.ToLower().Contains(parola.ToLower())) &&
                                    (!colAttive.inPhase || !x.Phase.ToLower().Contains(parola.ToLower()))
                                );
                    }
                }

                dati = query.ToList();
            }

            return dati;
        }

        public InColonne impostaColonneRicerca()
        {
            var colAttive = new InColonne();

            foreach (ListItem item in cklColonne.Items)
            {
                if (item.Value == "File")
                {
                    if (item.Selected) { colAttive.inFile = true; } else { colAttive.inFile = false; }
                }
                else if (item.Value == "Sequid")
                {
                    if (item.Selected) { colAttive.inSequid = true; } else { colAttive.inSequid = false; }
                }
                else if (item.Value == "Source")
                {
                    if (item.Selected) { colAttive.inSource = true; } else { colAttive.inSource = false; }
                }
                else if (item.Value == "Type")
                {
                    if (item.Selected) { colAttive.inType = true; } else { colAttive.inType = false; }
                }
                else if (item.Value == "Start")
                {
                    if (item.Selected) { colAttive.inStart = true; } else { colAttive.inStart = false; }
                }
                else if (item.Value == "End")
                {
                    if (item.Selected) { colAttive.inEnd = true; } else { colAttive.inEnd = false; }
                }
                else if (item.Value == "Score")
                {
                    if (item.Selected) { colAttive.inScore = true; } else { colAttive.inScore = false; }
                }
                else if (item.Value == "Strand")
                {
                    if (item.Selected) { colAttive.inStrand = true; } else { colAttive.inStrand = false; }
                }
                else if (item.Value == "Phase")
                {
                    if (item.Selected) { colAttive.inPhase = true; } else { colAttive.inPhase = false; }
                }
                else if (item.Value == "Attributes")
                {
                    if (item.Selected) { colAttive.inAttributes = true; } else { colAttive.inAttributes = false; }
                }
            }

            return colAttive;
        }

        protected void gdvBiocoso_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        public void aggiornaTabella(List<BioTizio> lista)
        {
            if (lista != null)
            {
                gdvBiocoso.DataSource = lista;
                gdvBiocoso.DataBind();

                stato.Text = "Ci sono <strong>" + lista.Count + "</strong> risultati.";

                messaggio.Text = "";
                pulsantiColonne.Visible = true;

                GridView grid = gdvBiocoso as GridView;
                if (lista.Any(x => !String.IsNullOrWhiteSpace(x.Fasta)))
                {
                    grid.Columns[2].Visible = true;
                }
                else
                {
                    grid.Columns[2].Visible = false;
                }

                if (lista.Any(x => !String.IsNullOrWhiteSpace(x.CDS)))
                {
                    grid.Columns[3].Visible = true;
                }
                else
                {
                    grid.Columns[3].Visible = false;
                }
            }
            else
            {
                pulsantiColonne.Visible = false;
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }

        public void visualizzaFileCaricato(bool loadInfo)
        {
            if (Session["nomeFile"] != null)
            {
                var files = Session["nomeFile"] as List<string>;
                litFileCaricato.Visible = true;

                litFileCaricato.Text = "";

                if (loadInfo)
                {
                    var listaSamples = new List<RootSample>();
                    foreach (var file in files)
                    {
                        litFileCaricato.Text += "<strong>File attivo:</strong> " + file + "<br>";

                        var risultati = SearchSamplesIMicrobe(Path.ChangeExtension(file, null));

                        var sample = risultati != null ? GetSampleInfoIMicrobe(risultati.FirstOrDefault().id) : null;

                        if (sample != null)
                        {
                            listaSamples.Add(sample);
                        }
                    }

                    repBtnModalFile.DataSource = listaSamples;
                    repBtnModalFile.DataBind();
                }
            }
            else
            {
                litFileCaricato.Text = "";
                litFileCaricato.Visible = false;
            }
        }

        protected void btnEsportaExcel_Click(object sender, EventArgs e)
        {
            ExportGridToExcel();
        }

        protected void btnEsportaGFF3_Click(object sender, EventArgs e)
        {
            ExportToGFF3(gdvBiocoso, "prova.xls");
        }

        private void ExportGridToExcel()
        {
            gdvBiocoso.AllowPaging = false;
            var dati = impostaFiltri();
            aggiornaTabella(dati);

            gdvBiocoso.BorderStyle = BorderStyle.Solid;
            gdvBiocoso.BorderWidth = 1;
            gdvBiocoso.BackColor = Color.WhiteSmoke;
            gdvBiocoso.GridLines = GridLines.Both;
            gdvBiocoso.Font.Name = "Verdana";
            gdvBiocoso.Font.Size = FontUnit.XXSmall;
            gdvBiocoso.HeaderStyle.BackColor = Color.DimGray;
            gdvBiocoso.HeaderStyle.ForeColor = Color.White;
            gdvBiocoso.RowStyle.HorizontalAlign = HorizontalAlign.Left;
            gdvBiocoso.RowStyle.VerticalAlign = VerticalAlign.Top;

            string FileName = "Gff3ToExcel_" + DateTime.Now + ".xls";
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";
            response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);

            using (var sw = new StringWriter())
            {
                using (var htw = new HtmlTextWriter(sw))
                {
                    gdvBiocoso.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();

                    gdvBiocoso.AllowPaging = true;
                    var datiNuovi = impostaFiltri();
                    aggiornaTabella(datiNuovi);
                }
            }
        }

        protected void ExportToNewExcel_Click(object sender, EventArgs e)
        {
            var arrayRigheSelezionate = GetRigheSelezionate();
            var dati = impostaFiltri();
            var datiOrdinati = OrdinaTabella(dati);
            var listaFinale = arrayRigheSelezionate.Length == 0 ? datiOrdinati : FiltraListaPerSelezionati(datiOrdinati, arrayRigheSelezionate);
            if (listaFinale != null)
            {
                // Crea un nuovo pacchetto Excel in memoria
                using (ExcelPackage package = new ExcelPackage())
                {
                    // Crea un foglio di lavoro
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Dati");

                    // Riga di intestazione
                    int rowIndex = 1;
                    PropertyInfo[] properties = typeof(BioTizio).GetProperties();
                    for (int columnIndex = 1; columnIndex <= properties.Length; columnIndex++)
                    {
                        PropertyInfo property = properties[columnIndex - 1];
                        worksheet.Cells[rowIndex, columnIndex].Value = property.Name;
                    }

                    // Riempimento dei dati
                    rowIndex = 2;
                    foreach (BioTizio bioTizio in listaFinale)
                    {
                        for (int columnIndex = 1; columnIndex <= properties.Length; columnIndex++)
                        {
                            PropertyInfo property = properties[columnIndex - 1];
                            object value = property.GetValue(bioTizio);

                            if (property.Name == "Attributes")
                            {
                                // Gestisci la colonna "Attributes"
                                string attributesString = string.Join(", ", (string[])value);
                                worksheet.Cells[rowIndex, columnIndex].Value = attributesString;
                            }
                            else
                            {
                                worksheet.Cells[rowIndex, columnIndex].Value = value != null ? value.ToString() : string.Empty;
                            }
                        }
                        rowIndex++;
                    }

                    // Auto-dimensiona le colonne
                    worksheet.Cells.AutoFitColumns();

                    // Converti il pacchetto Excel in un array di byte
                    byte[] excelBytes = package.GetAsByteArray();

                    // Invia il file come download al client
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=Dati.xlsx");
                    HttpContext.Current.Response.BinaryWrite(excelBytes);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }

        protected int[] GetRigheSelezionate()
        {
            List<int> selectedRowIndexes = new List<int>();

            foreach (GridViewRow row in gdvBiocoso.Rows)
            {
                CheckBox checkBox = row.FindControl("chkSelezione") as CheckBox;
                var boh = row.FindControl("chkSelezione");
                if (checkBox != null && checkBox.Checked)
                {
                    int rowIndex = row.RowIndex;
                    selectedRowIndexes.Add(rowIndex);
                }
            }

            // Salva gli indici delle colonne selezionate in un array o in un'altra struttura dati
            int[] selectedRowsArray = selectedRowIndexes.ToArray();

            // Esegui altre operazioni con l'array degli indici delle colonne selezionate
            return selectedRowsArray;
        }

        protected List<BioTizio> FiltraListaPerSelezionati(List<BioTizio> originalList, int[] rowIndexes)
        {
            List<BioTizio> filteredList = new List<BioTizio>();

            for (int i = 0; i < originalList.Count; i++)
            {
                if (rowIndexes.Contains(i))
                {
                    filteredList.Add(originalList[i]);
                }
            }

            return filteredList;
        }

        public void ExportToGFF3(GridView gridView, string fileName)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set header row style
                using (var headerRange = worksheet.Cells[1, 1, 1, gridView.Columns.Count])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Fill header row with column names
                for (int i = 0; i < gridView.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = gridView.Columns[i].HeaderText;
                }

                // Fill data rows
                for (int row = 0; row < gridView.Rows.Count; row++)
                {
                    for (int col = 0; col < gridView.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = gridView.Rows[row].Cells[col].Text;
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    package.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(HttpContext.Current.Response.OutputStream);
                }
            }

            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public void mostraNascondiColonne(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            GridView grid = gdvBiocoso as GridView;

            if (button.CommandName == "1")
            {
                grid.Columns[0].Visible = grid.Columns[0].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[0].Visible);
            }
            else if (button.CommandName == "2")
            {
                grid.Columns[1].Visible = grid.Columns[1].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[1].Visible);
            }
            else if (button.CommandName == "3")
            {
                grid.Columns[2].Visible = grid.Columns[2].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[2].Visible);
            }
            else if (button.CommandName == "4")
            {
                grid.Columns[3].Visible = grid.Columns[3].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[3].Visible);
            }
            else if (button.CommandName == "5")
            {
                grid.Columns[4].Visible = grid.Columns[4].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[4].Visible);
            }
            else if (button.CommandName == "6")
            {
                grid.Columns[5].Visible = grid.Columns[5].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[5].Visible);
            }
            else if (button.CommandName == "7")
            {
                grid.Columns[6].Visible = grid.Columns[6].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[6].Visible);
            }
            else if (button.CommandName == "8")
            {
                grid.Columns[7].Visible = grid.Columns[7].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[7].Visible);
            }
            else if (button.CommandName == "9")
            {
                grid.Columns[8].Visible = grid.Columns[8].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[8].Visible);
            }
            else if (button.CommandName == "10")
            {
                grid.Columns[9].Visible = grid.Columns[9].Visible == true ? false : true;
                aggiornaBottoni(button, grid.Columns[9].Visible);
            }

            //Bind the DataTable.
            var dati = impostaFiltri();
            aggiornaTabella(dati);
        }

        public void aggiornaBottoni(Button button, bool attivo)
        {
            button.CssClass = attivo ? "btn btn-danger" : "btn btn-success";
        }

        protected void gdvBiocoso_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvBiocoso.PageIndex = e.NewPageIndex;
            impostaFiltri(); // this is whatever method you call to bind your data.
        }

        protected void ddlNumeroRisultati_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlNumeroPagine.SelectedValue == "all")
            {
                List<BioTizio> dati = Session["data"] as List<BioTizio>;
                gdvBiocoso.PageSize = dati.Count;
                var datiFiltrati = impostaFiltri();
                aggiornaTabella(datiFiltrati);
            }
            else
            {
                List<BioTizio> dati = Session["data"] as List<BioTizio>;
                gdvBiocoso.PageSize = Convert.ToInt32(ddlNumeroPagine.SelectedValue);
                gdvBiocoso.DataSource = dati;
                var datiFiltrati = impostaFiltri();
                aggiornaTabella(datiFiltrati);
            }
        }

        private void GridViewSortDirection(GridView g, GridViewSortEventArgs e, out SortDirection d, out string f)
        {
            f = e.SortExpression;
            d = e.SortDirection;

            //Check if GridView control has required Attributes
            if (g.Attributes["CurrentSortField"] != null && g.Attributes["CurrentSortDir"] != null)
            {
                if (f == g.Attributes["CurrentSortField"])
                {
                    d = SortDirection.Descending;
                    if (g.Attributes["CurrentSortDir"] == "ASC")
                    {
                        d = SortDirection.Ascending;
                    }
                }

                g.Attributes["CurrentSortField"] = f;
                g.Attributes["CurrentSortDir"] = (d == SortDirection.Ascending ? "DESC" : "ASC");

                var tab = OrdinaTabella();
                aggiornaTabella(tab);
            }

        }

        public List<BioTizio> OrdinaTabella(List<BioTizio> tabella = null)
        {
            var ordine = gdvBiocoso.Attributes["CurrentSortField"];
            var direzione =
                String.IsNullOrEmpty(gdvBiocoso.Attributes["CurrentSortDir"])
                ? SortDirection.Ascending
                : gdvBiocoso.Attributes["CurrentSortDir"] == "ASC"
                ? SortDirection.Ascending
                : SortDirection.Descending;
            tabella = tabella == null ? Session["data"] as List<BioTizio> : tabella;

            if (tabella == null)
            {
                return null;
            }
            else
            {
                IOrderedEnumerable<BioTizio> tab = null;
                if (ordine == "file")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.File) : tabella.OrderByDescending(x => x.File);
                }
                else if (ordine == "sequid")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Sequid) : tabella.OrderByDescending(x => x.Sequid);
                }
                else if (ordine == "source")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Source) : tabella.OrderByDescending(x => x.Source);
                }
                else if (ordine == "type")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Type) : tabella.OrderByDescending(x => x.Type);
                }
                else if (ordine == "start")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Start) : tabella.OrderByDescending(x => x.Start);
                }
                else if (ordine == "end")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.End) : tabella.OrderByDescending(x => x.End);
                }
                else if (ordine == "score")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Score) : tabella.OrderByDescending(x => x.Score);
                }
                else if (ordine == "strand")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Strand) : tabella.OrderByDescending(x => x.Strand);
                }
                else if (ordine == "phase")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Phase) : tabella.OrderByDescending(x => x.Phase);
                }
                else if (ordine == "attributes")
                {
                    tab = direzione == SortDirection.Ascending ? tabella.OrderBy(x => x.Attributes) : tabella.OrderByDescending(x => x.Attributes);
                }

                return tab.ToList();
            }
        }

        protected void gdvBiocoso_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortDirection sortDirection = SortDirection.Ascending;
            string sortField = string.Empty;
            GridViewSortDirection(gdvBiocoso, e, out sortDirection, out sortField);
        }

        protected void btnEliminaFiltri_Click(object sender, EventArgs e)
        {
            // Rimuovo i filtri per parola
            Session["contiene"] = null;
            Session["noncontiene"] = null;
            valoriContiene.Value = null;
            valoriNonContiene.Value = null;

            // Ripristino la visione delle colonne
            GridView grid = gdvBiocoso as GridView;
            for (int i = 0; i < 10; i++)
            {
                grid.Columns[i].Visible = true;
            }

            // Ripristino le colonne sui filtri
            foreach (ListItem item in cklColonne.Items)
            {
                item.Selected = true;
            }

            // imposto i pulsanti del mostra/nascondi colonne come attivi
            colonna1.CssClass = "btn btn-danger";
            colonna2.CssClass = "btn btn-danger";
            colonna3.CssClass = "btn btn-danger";
            colonna4.CssClass = "btn btn-danger";
            colonna5.CssClass = "btn btn-danger";
            colonna6.CssClass = "btn btn-danger";
            colonna7.CssClass = "btn btn-danger";
            colonna8.CssClass = "btn btn-danger";
            colonna9.CssClass = "btn btn-danger";

            var datiFiltrati = impostaFiltri();
            aggiornaTabella(datiFiltrati);
        }
    }

    [Serializable]
    public class BioTizio
    {
        public string File { get; set; }
        public string Fasta { get; set; }
        public string CDS { get; set; }
        public string Sequid { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Score { get; set; }
        public string Strand { get; set; }
        public string Phase { get; set; }
        public string[] Attributes { get; set; }
    }

    public class InColonne
    {
        public bool inFile { get; set; }
        public bool inSequid { get; set; }
        public bool inSource { get; set; }
        public bool inType { get; set; }
        public bool inStart { get; set; }
        public bool inEnd { get; set; }
        public bool inScore { get; set; }
        public bool inStrand { get; set; }
        public bool inPhase { get; set; }
        public bool inAttributes { get; set; }
    }

    // IMicrobe Classes

    public class SearchResult
    {
        public string table_name { get; set; }
        public int id { get; set; }
        public string object_name { get; set; }
    }

    public class Assembly
    {
        public int assembly_id { get; set; }
        public int project_id { get; set; }
        public string assembly_code { get; set; }
        public string assembly_name { get; set; }
        public string organism { get; set; }
        public string pep_file { get; set; }
        public string nt_file { get; set; }
        public string cds_file { get; set; }
        public object description { get; set; }
        public int sample_id { get; set; }
        public string url { get; set; }
    }

    public class AvailableFileType
    {
        public int sample_file_type_id { get; set; }
        public string type { get; set; }
    }

    public class CombinedAssembly
    {
        public int combined_assembly_id { get; set; }
        public int project_id { get; set; }
        public string assembly_name { get; set; }
        public string phylum { get; set; }
        public string @class { get; set; }
        public string family { get; set; }
        public string genus { get; set; }
        public string species { get; set; }
        public string strain { get; set; }
        public string pcr_amp { get; set; }
        public string annotations_file { get; set; }
        public string peptides_file { get; set; }
        public string nucleotides_file { get; set; }
        public string cds_file { get; set; }
        public CombinedAssemblyToSample combined_assembly_to_sample { get; set; }
    }

    public class CombinedAssemblyToSample
    {
        public int combined_assembly_to_sample_id { get; set; }
        public int combined_assembly_id { get; set; }
        public int sample_id { get; set; }
    }

    public class Investigator
    {
        public int investigator_id { get; set; }
        public string investigator_name { get; set; }
    }

    public class Project
    {
        public int project_id { get; set; }
        public string project_code { get; set; }
        public string project_name { get; set; }
        public string project_type { get; set; }
        public string description { get; set; }
        public int @private { get; set; }
        public object ebi_status { get; set; }
        public List<object> project_groups { get; set; }
        public List<object> users { get; set; }
    }

    public class RootSample
    {
        public int sample_id { get; set; }
        public int project_id { get; set; }
        public string sample_acc { get; set; }
        public string sample_name { get; set; }
        public string sample_type { get; set; }
        public string sample_description { get; set; }
        public string url { get; set; }
        public DateTime creation_date { get; set; }
        public Project project { get; set; }
        public List<Investigator> investigators { get; set; }
        public List<SampleFile> sample_files { get; set; }
        public List<Assembly> assemblies { get; set; }
        public List<CombinedAssembly> combined_assemblies { get; set; }
        public List<SampleAttr> sample_attrs { get; set; }
        public List<string> available_types { get; set; }
        public List<AvailableFileType> available_file_types { get; set; }
        public int protein_count { get; set; }
        public int centrifuge_count { get; set; }
    }

    public class SampleAttr
    {
        public int sample_attr_id { get; set; }
        public int sample_attr_type_id { get; set; }
        public int sample_id { get; set; }
        public string attr_value { get; set; }
        public SampleAttrType sample_attr_type { get; set; }
    }

    public class SampleAttrType
    {
        public int sample_attr_type_id { get; set; }
        public int sample_attr_type_category_id { get; set; }
        public string type { get; set; }
        public string url_template { get; set; }
        public string description { get; set; }
        public string units { get; set; }
        public SampleAttrTypeCategory sample_attr_type_category { get; set; }
    }

    public class SampleAttrTypeCategory
    {
        public int sample_attr_type_category_id { get; set; }
        public string category { get; set; }
    }

    public class SampleFile
    {
        public int sample_file_id { get; set; }
        public int sample_id { get; set; }
        public int sample_file_type_id { get; set; }
        public string file { get; set; }
        public string comments { get; set; }
        public SampleFileType sample_file_type { get; set; }
    }

    public class SampleFileType
    {
        public int sample_file_type_id { get; set; }
        public string type { get; set; }
    }


}