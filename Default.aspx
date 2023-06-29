<%--<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Gff3_tools._Default" %>--%>

<%@ Page
    Title="Gff3 Tools"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Gff3_tools._Default"
    ValidateRequest="false"
    ViewStateEncryptionMode="Never" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" CssClass="container-lg">
    <%--<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" integrity="sha384-HSMxcRTRxnN+Bdg0JdbxYKrThecOKuH5zCYotlSAcp1+c8xmyTe9GYg1l9a69psu" crossorigin="anonymous">--%>
    <%--    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">--%>

    <script
        src="https://code.jquery.com/jquery-3.6.0.min.js"
        integrity="sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4="
        crossorigin="anonymous"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" integrity="sha512-9usAa10IRO0HhonpyAIVpjrylPvoDwiPUiKdWk5t3PyolY1cOd4DSE0Ga+ri4AuTroPR5aQvXU9xC6qOPnzFeg==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        .container {
            margin-top: 20px;
        }

        td {
            min-width: 70px;
        }

        p {
            font-style: italic;
            font-weight: lighter;
        }

        .animated {
            -webkit-transition: height 0.2s;
            -moz-transition: height 0.2s;
            transition: height 0.2s;
        }

        .textbox {
            width: 300px;
        }

        .stato {
            text-align: center;
            padding: 15px;
        }

        .ele-contiene-list .list-group-item, .ele-noncontiene-list .list-group-item {
            clear: both;
            padding: 0;
        }

            .ele-contiene-list .list-group-item span, .ele-noncontiene-list .list-group-item span {
                display: inline-block;
                padding: 8px;
            }

        .ele-contiene-list button, .ele-noncontiene-list button {
            float: right;
            margin: 2px;
        }

        .ele-contiene .input-group input[type="text"], .ele-noncontiene .input-group input[type="text"] {
            width: 1%;
            padding: 0.375rem 0.75rem;
            margin: 0;
            border-right: 0;
        }

        .ele-contiene .input-group button, .ele-noncontiene .input-group button {
            margin: 0;
            padding: 0.375rem 0.75rem;
        }

        .StickyHeader th {
            position: sticky;
            top: 60px;
            background-color: #14989d;
        }

        .Footer {
            background-color: #14989d;
        }

        #btnCarica {
            margin-top: 10px;
        }

        /* .table a {
            color: white;
        }*/

        .btn-operazioni {
            max-width: 120px;
            display: flex;
        }

        .card {
            -webkit-box-shadow: 0px 0px 6px 0px #000000;
            box-shadow: 0px 0px 6px 0px #000000;
        }

        #btn-back-to-top {
            position: fixed;
            bottom: 20px;
            right: 20px;
            display: none;
        }

        .popover {
            max-width: 75%; /* Max Width of the popover (depending on the container!) */
        }

        #gdvBiocoso a {
            color: white;
        }
    </style>

    <div class="container">
        <asp:Literal ID="messaggio" runat="server"></asp:Literal>
        <div>
            <h1>Gff3 Tools</h1>
            <h2>Strumenti per lo studio di file in formato .gff3</h2>
            <div class="accordion mb-3" id="accordionDettagli">
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingThree">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                            Funzionalità disponibili
                        </button>
                    </h2>
                    <div id="collapseThree" class="accordion-collapse collapse" aria-labelledby="headingThree" data-bs-parent="#accordionDettagli">
                        <div class="accordion-body">
                            <div style="font-size: small;">È possibile effettuare le seguenti operazioni: </div>
                            <ul style="font-size: small;">
                                <li>caricare e visualizzare uno o più file in formato .gff3
                                </li>
                                <li>Filtrare la lista impostando parole da ricercare/escludere
                                </li>
                                <li>Mostrare/Nascondere le colonne della tabella
                                </li>
                                <li>Esportare la tabella in formato excel
                                </li>
                                <li>Ordinare la tabella in ordine crescente/decrescente (cliccare sul nome della colonna)
                                </li>
                                <li>Impostare il numero di risultati da visualizzare
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="panRicerca" runat="server" DefaultButton="btnRicerca">
                <!-- Card caricamento files -->
                <div class="card p-3 mb-3">
                    <h5 class="card-title">File</h5>
                    <p class="card-text">Seleziona uno o più file in formato .gff3 e clicca sul pulsante carica. I risultati verranno mostrati in un'unica tabella.</p>
                    <div class="card-body">
                        <h6>Carica i file GFF3</h6>
                        <div class="row">
                            <div class="col-md-5">
                                <asp:FileUpload ID="fileCaricato" runat="server" CssClass="form-control" AllowMultiple="true" accept=".gff3" />
                                <asp:Button
                                    ID="btnCarica"
                                    runat="server"
                                    ClientIDMode="Static"
                                    Text="Carica file"
                                    CssClass="btn btn-secondary"
                                    OnClick="ImportCSV"
                                    OnClientClick="showLoading();" />
                            </div>
                            <div class="col-md-7">
                                <asp:Literal ID="litFileCaricato" runat="server" Visible="false"></asp:Literal>
                                <asp:Repeater ID="repBtnModalFile" runat="server">
                                    <ItemTemplate>
                                        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#samplemodal-<%# Eval("sample_id") %>">
                                            <%# Eval("sample_description") %> (<%# Eval("sample_name") %>)
                                        </button>
                                        <div class="modal fade" id="samplemodal-<%# Eval("sample_id") %>" tabindex="-1" aria-labelledby="labelModal-<%# Eval("sample_id") %>" aria-hidden="true">
                                            <div class="modal-dialog modal-xl">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <h5 class="modal-title" id="labelModal-<%# Eval("sample_id") %>">
                                                            <%# Eval("sample_description") %>
                                                            <span class="badge rounded-pill bg-warning text-dark">
                                                                <%# Eval("sample_type") %>
                                                            </span>
                                                        </h5>
                                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                    </div>
                                                    <div class="modal-body">
                                                        <div class="row">
                                                            <div class="col-md-4 fw-bold">
                                                                Campione
                                                            </div>
                                                            <div class="col-md-8">
                                                                <%# Eval("sample_name") %>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-4 fw-bold">
                                                                Ricercatori
                                                            </div>
                                                            <div class="col-md-8">
                                                                <asp:Repeater runat="server" DataSource='<%# Eval("investigators") %>'>
                                                                    <ItemTemplate>
                                                                        <%# Eval("investigator_name") %>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </div>
                                                        </div>

                                                        <div class="accordion mt-5" id="accordion-<%# Eval("sample_id") %>">
                                                            <!-- files -->
                                                            <asp:Repeater runat="server" DataSource='<%# Eval("sample_files") %>'>
                                                                <HeaderTemplate>
                                                                    <div class="accordion-item">
                                                                        <h2 class="accordion-header" id="headingFiles">
                                                                            <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseFiles" aria-expanded="true" aria-controls="headingCollapseFiles">
                                                                                Files
                                                                            </button>
                                                                        </h2>
                                                                        <div id="collapseFiles" class="accordion-collapse collapse" aria-labelledby="headingCollapseFiles"
                                                                            data-bs-parent="#accordion-<%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, "sample_id")%>">
                                                                            <div class="accordion-body">
                                                                                <table class="table">
                                                                                    <thead>
                                                                                        <tr>
                                                                                            <th scope="col">Nome</th>
                                                                                            <th scope="col">Tipo</th>
                                                                                            <th scope="col"></th>
                                                                                        </tr>
                                                                                    </thead>
                                                                                    <tbody>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%# GetFileName(Eval("file")) %></td>
                                                                        <td><%# Eval("sample_file_type.type") %></td>
                                                                        <td><a target="_blank" class="btn btn-outline-secondary" href="http://datacommons.cyverse.org/browse/<%# Eval("file") %>">Download</a></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    </tbody>
                                                                    </table>
                                                                    </div>
                                                                        </div>
                                                                    </div>
                                                                </FooterTemplate>
                                                            </asp:Repeater>

                                                            <!-- Attributes -->
                                                            <asp:Repeater runat="server" DataSource='<%# Eval("sample_attrs") %>'>
                                                                <HeaderTemplate>
                                                                    <div class="accordion-item">
                                                                        <h2 class="accordion-header" id="headingAttributes">
                                                                            <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseAttributes" aria-expanded="true" aria-controls="headingCollapseAttributes">
                                                                                Attributi
                                                                            </button>
                                                                        </h2>
                                                                        <div id="collapseAttributes" class="accordion-collapse collapse" aria-labelledby="headingCollapseAttributes"
                                                                            data-bs-parent="#accordion-<%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, "sample_id")%>">
                                                                            <div class="accordion-body">
                                                                                <table class="table" style="table-layout: fixed;">
                                                                                    <thead>
                                                                                        <tr>
                                                                                            <th scope="col">Tipo</th>
                                                                                            <th scope="col">Valore</th>
                                                                                        </tr>
                                                                                    </thead>
                                                                                    <tbody>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%# Eval("sample_attr_type.sample_attr_type_category.category") %></td>
                                                                        <td>
                                                                            <div style="word-wrap: break-word; overflow-wrap: break-word; width: 100%;">
                                                                                <%# Eval("attr_value") %>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    </tbody>
                                                                    </table>
                                                                    </div>
                                                                        </div>
                                                                    </div>
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                        </div>
                                                    </div>
                                                    <div class="modal-footer">
                                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Chiudi</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                        <div id="divFileAggiuntivi" runat="server">
                            <h6 class="mt-3">Carica i file FASTA</h6>
                            <p>
                                <span class="text-danger">Attenzione:</span>
                                caricare molti file FASTA aumenterà l'elaborazione dei dati. Per evitare lunghe attese si suggerisce di filtrare i dati
                            in base alle proprie necessità e successivamente caricare i file per aggregarli nella tabella.
                            </p>
                            <div class="row">
                                <div class="col-md-5">
                                    <asp:FileUpload ID="fileCaricatoFasta" runat="server" CssClass="form-control" AllowMultiple="true" accept=".fasta" />
                                    <asp:Button
                                        ID="btnCaricaFasta"
                                        runat="server"
                                        ClientIDMode="Static"
                                        Text="Carica file"
                                        CssClass="btn btn-secondary mt-2"
                                        OnClick="ImportFasta"
                                        OnClientClick="showLoading();" />
                                </div>
                                <div class="col-md-7">
                                    <asp:Literal ID="Literal1" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>

                            <h6 class="mt-3">Carica i file CDS</h6>
                            <p>
                                <span class="text-danger">Attenzione:</span>
                                caricare molti file cds aumenterà l'elaborazione dei dati. Per evitare lunghe attese si suggerisce di filtrare i dati
                            in base alle proprie necessità e successivamente caricare i file per aggregarli nella tabella.
                            </p>
                            <div class="row">
                                <div class="col-md-5">
                                    <asp:FileUpload ID="fileCaricatoCDS" runat="server" CssClass="form-control" AllowMultiple="true" accept=".cds.fa" />
                                    <asp:Button
                                        ID="btnCaricaCDS"
                                        runat="server"
                                        ClientIDMode="Static"
                                        Text="Carica file"
                                        CssClass="btn btn-secondary mt-2"
                                        OnClick="ImportCDS"
                                        OnClientClick="showLoading();" />
                                </div>
                                <div class="col-md-7">
                                    <asp:Literal ID="Literal2" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Card Filtri -->
                <div id="CardFiltri" runat="server" class="card p-3">
                    <h5 class="card-title">Filtri</h5>
                    <p class="card-text">
                        Filtrare i risultati per parole che devono/non devono essere presenti con la possibilità di selezionare su quale colonna/colonne
                        effettuare la ricerca.
                    </p>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-3">
                                <asp:Label ID="lblFiltro" runat="server"><strong>Contiene:</strong>
                                    <span 
                                        class="info-color"
                                        data-bs-toggle="tooltip" 
                                        data-bs-placement="top" 
                                        title="è possibile impostare 1 o più parole da ricercare in modo da filtrare i dati che le contengono.">
                                      <i class="fa-solid fa-circle-info"></i>
                                    </span>
                                </asp:Label>
                            </div>
                            <div class="col-md-9">
                                <div class="ele-contiene" style="">
                                    <div class="ele-contiene-list list-group w-75"></div>
                                    <div class="input-group mb-3 w-75">
                                        <input type="text"
                                            class="ele-contiene-edit form-control"
                                            placeholder="Parola da ricercare"
                                            aria-label="Parola da ricercare"
                                            aria-describedby="basic-addon2">
                                        <div class="input-group-append">
                                            <button class="ele-contiene-add btn btn-success"
                                                role="button"
                                                type="button">
                                                <i class="fa fa-plus" aria-hidden="true"></i>
                                                Aggiungi
                                            </button>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="valoriContiene" runat="server" ClientIDMode="Static" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <asp:Label ID="lblNonContiene" runat="server">
                                    <strong>Non contiene:</strong>
                                    <span 
                                        class="info-color"
                                        data-bs-toggle="tooltip" 
                                        data-bs-placement="top" 
                                        title="è possibile impostare 1 o più parole da ricercare in modo da filtrare i dati che NON le contengono.">
                                      <i class="fa-solid fa-circle-info"></i>
                                    </span>
                                </asp:Label>
                            </div>
                            <div class="col-md-9">
                                <div class="ele-noncontiene">
                                    <div class="ele-noncontiene-list list-group w-75"></div>
                                    <div class="input-group mb-3 w-75">
                                        <input type="text"
                                            class="ele-noncontiene-edit form-control"
                                            placeholder="Parola da rimuovere"
                                            aria-label="Parola da rimuovere"
                                            aria-describedby="basic-addon2">
                                        <div class="input-group-append">
                                            <button class="ele-noncontiene-add btn btn-success"
                                                role="button"
                                                type="button">
                                                <i class="fa fa-plus" aria-hidden="true"></i>
                                                Aggiungi
                                            </button>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="valoriNonContiene" runat="server" ClientIDMode="Static" />
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-md-2">
                                <strong>Ricerca in:</strong>
                                <span
                                    class="info-color"
                                    data-bs-toggle="tooltip"
                                    data-bs-placement="top"
                                    title="è possibile impostare le colonne in cui ricercare le parole da filtrare o escludere dalla tabella.">
                                    <i class="fa-solid fa-circle-info"></i>
                                </span>
                            </div>
                            <div class="col-md-10">
                                <asp:CheckBoxList ID="cklColonne" runat="server" ClientIDMode="Static" AutoPostBack="false" RepeatLayout="Table" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="File" Selected="True">File</asp:ListItem>
                                    <asp:ListItem Value="Sequid" Selected="True">Sequid</asp:ListItem>
                                    <asp:ListItem Value="Source" Selected="True">Source</asp:ListItem>
                                    <asp:ListItem Value="Type" Selected="True">Type</asp:ListItem>
                                    <asp:ListItem Value="Start" Selected="True">Start</asp:ListItem>
                                    <asp:ListItem Value="End" Selected="True">End</asp:ListItem>
                                    <asp:ListItem Value="Score" Selected="True">Score</asp:ListItem>
                                    <asp:ListItem Value="Strand" Selected="True">Strand</asp:ListItem>
                                    <asp:ListItem Value="Phase" Selected="True">Phase</asp:ListItem>
                                    <asp:ListItem Value="Attributes" Selected="True">Attributes</asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <strong>Sottolinea parole ricercate:</strong>
                                <span
                                    class="info-color"
                                    data-bs-toggle="tooltip"
                                    data-bs-placement="top"
                                    title="Permette di evidenziare in giallo nella tabella le parole ricercate in modo da avere una visione immediata del dato.">
                                    <i class="fa-solid fa-circle-info"></i>
                                </span>
                            </div>
                            <div class="col-md-9">
                                <asp:CheckBox ID="chkSottolineaParole" runat="server" ClientIDMode="Static" />
                            </div>
                        </div>
                        <p>
                            <strong>Esempio: </strong>
                            Devo filtrare i risultati in modo da avere soltanto quelli che contengono la parola "mitochondrial" nella colonna "Attributes".
                            In questo caso dovrò prima impostare la parola su "Contiene" e cliccare su "Aggiungi" per confermare, successivamente,
                            in "Ricerca in" rimuovo la spunta da tutte le checkbox lasciando attiva soltanto quella di "Attributes".
                            Infine clicco sul pulsante "Applica i filtri" per avviare la ricerca dei risultati.
                        </p>

                        <div style="display: inline-flex;">
                            <asp:Button
                                ID="btnRicerca"
                                runat="server"
                                CssClass="btn btn-primary btn-operazioni me-2"
                                Text="Applica i filtri"
                                OnClick="btnSearch_Click"
                                OnClientClick="showLoading();" />
                            <asp:Button
                                ID="btnEliminaFiltri"
                                runat="server"
                                CssClass="btn btn-danger btn-operazioni"
                                Text="Elimina filtri"
                                OnClick="btnEliminaFiltri_Click" />
                            <%--<asp:Button
                                ID="btnEsportaExcel"
                                runat="server"
                                CssClass="btn btn-secondary btn-operazioni"
                                Text="Esporta excel"
                                OnClick="btnEsportaExcel_Click" />--%>
                        </div>
                    </div>
                </div>

                <!-- Card esportazione -->
                <div id="CardEsportazione" runat="server" class="card p-3">
                    <h5 class="card-title">Esporta</h5>
                    <p class="card-text">
                        Esportazione dei dati nel formato richiesto.
                        La funzionalità permette di esportare dei dati custom come ad esempio combinazioni di file gff3, specifiche righe, dati aggregati come fasta e cds.
                        Inoltre i file esportati saranno filtrati e ordinati in base ai dati presenti nella tabella.
                    </p>
                    <div class="card-body">
                        <div class="d-inline-flex">
                            <asp:Button
                                ID="btnEsportaGff3"
                                runat="server"
                                CssClass="btn btn-secondary btn-operazioni me-2"
                                Text="Esporta GFF3"
                                OnClick="btnEsportaGFF3_Click" />
                            <asp:Button
                                ID="btnEsportaCds"
                                runat="server"
                                CssClass="btn btn-secondary btn-operazioni me-2"
                                Text="Esporta CDS"
                                OnClick="btnEsportaCDS_Click" />
                            <asp:Button
                                ID="btnEsportaExcel"
                                runat="server"
                                CssClass="btn btn-secondary btn-operazioni"
                                Text="Esporta Excel"
                                OnClick="ExportToNewExcel_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>

        <!-- stato -->
        <div class="stato">
            <asp:Label ID="stato" runat="server"></asp:Label>
        </div>

        <!-- pulsanti tabella -->
        <div id="pulsantiColonne" runat="server" visible="false">
            <div class="row">
                <div class="col-md-2">
                    <asp:Label ID="lblPulsantiColonne" runat="server"><strong>Mostra/Nascondi colonne:</strong></asp:Label>
                </div>
                <div class="col-md-8">
                    <asp:Button ID="colonna0" runat="server" CommandName="1" OnClick="mostraNascondiColonne" Text="File" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna1" runat="server" CommandName="4" OnClick="mostraNascondiColonne" Text="Sequid" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna2" runat="server" CommandName="5" OnClick="mostraNascondiColonne" Text="Source" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna3" runat="server" CommandName="6" OnClick="mostraNascondiColonne" Text="Type" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna4" runat="server" CommandName="7" OnClick="mostraNascondiColonne" Text="Start" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna5" runat="server" CommandName="8" OnClick="mostraNascondiColonne" Text="End" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna6" runat="server" CommandName="9" OnClick="mostraNascondiColonne" Text="Score" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna7" runat="server" CommandName="10" OnClick="mostraNascondiColonne" Text="Strand" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna8" runat="server" CommandName="11" OnClick="mostraNascondiColonne" Text="Phase" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                    <asp:Button ID="colonna9" runat="server" CommandName="12" OnClick="mostraNascondiColonne" Text="Attributes" CssClass="btn btn-danger" OnClientClick="showLoading();" />
                </div>
                <div class="col-md-2">
                    Risultati:
                    <asp:DropDownList
                        ID="ddlNumeroPagine"
                        runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlNumeroRisultati_SelectedIndexChanged">
                        <asp:ListItem Value="100">100</asp:ListItem>
                        <asp:ListItem Value="500" Selected="True">500</asp:ListItem>
                        <asp:ListItem Value="1000">1000</asp:ListItem>
                        <asp:ListItem Value="2000">2000</asp:ListItem>
                        <asp:ListItem Value="4000">4000</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- Tabella -->
        <asp:GridView
            ID="gdvBiocoso"
            ClientIDMode="Static"
            runat="server"
            OnRowDataBound="gdvBiocoso_RowDataBound"
            OnPageIndexChanging="gdvBiocoso_PageIndexChanging" AllowPaging="true" PageSize="2000"
            OnSorting="gdvBiocoso_Sorting" AllowSorting="true" CurrentSortDir="ASC" CurrentSortField="sequid"
            AlternatingRowStyle-CssClass="alt"
            AutoGenerateColumns="false"
            Width="100%" border="1" CellPadding="3" CssClass="table table-striped table-bordered table-hover"
            Style="border: 1px solid #E5E5E5; word-break: break-all; word-wrap: break-word">
            <HeaderStyle CssClass="StickyHeader" />
            <PagerStyle CssClass="Footer" />
            <Columns>
                <asp:TemplateField HeaderText="" ItemStyle-CssClass="short tdSelezione" HeaderStyle-CssClass="short">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelezione" runat="server" HeaderText="Seleziona" />
                        <%--                        <input class="form-check-input" type="checkbox" id="chkSelezione" value="" aria-label="seleziona">--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="File" HeaderText="File" SortExpression="file" ItemStyle-CssClass="tdFile" />
                <asp:TemplateField HeaderText="Fasta" ItemStyle-CssClass="short tdFasta" HeaderStyle-CssClass="short" SortExpression="fasta">
                    <ItemTemplate>
                        <button
                            type="button"
                            class="btn btn-outline-secondary"
                            style='<%#(Eval("Fasta") == null || Eval("Fasta") == "" ? "display: none;": "")%>'
                            data-bs-toggle="popover"
                            title="Stringa del file Fasta"
                            data-bs-content='<%# Eval("Fasta") %>'>
                            <i class="fa-solid fa-eye"></i>
                        </button>

                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CDS" ItemStyle-CssClass="short tdCDS" HeaderStyle-CssClass="short" SortExpression="CDS">
                    <ItemTemplate>
                        <button
                            type="button"
                            class="btn btn-outline-secondary"
                            data-bs-toggle="popover"
                            style='<%#(Eval("CDS") == null || Eval("CDS") == "" ? "display: none;": "")%>'
                            title="Stringa del file CDS"
                            data-bs-content='<%# Eval("CDS") %>'>
                            <i class="fa-solid fa-eye"></i>
                        </button>

                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Sequid" HeaderText="Sequid" ItemStyle-CssClass="short tdSequid" HeaderStyle-CssClass="short" SortExpression="sequid" />
                <asp:BoundField DataField="Source" HeaderText="Source" ItemStyle-CssClass="short tdSource" HeaderStyle-CssClass="short" SortExpression="source" />
                <asp:BoundField DataField="Type" HeaderText="Type" ItemStyle-CssClass="short tdType" HeaderStyle-CssClass="short" SortExpression="type" />
                <asp:BoundField DataField="Start" HeaderText="Start" ItemStyle-CssClass="short tdStart" HeaderStyle-CssClass="short" SortExpression="start" />
                <asp:BoundField DataField="End" HeaderText="End" ItemStyle-CssClass="short tdEnd" HeaderStyle-CssClass="short" SortExpression="end" />
                <asp:BoundField DataField="Score" HeaderText="Score" ItemStyle-CssClass="short tdScore" HeaderStyle-CssClass="short" SortExpression="score" />
                <asp:BoundField DataField="Strand" HeaderText="Strand" ItemStyle-CssClass="short tdStrand" HeaderStyle-CssClass="short" SortExpression="strand" />
                <asp:BoundField DataField="Phase" HeaderText="Phase" ItemStyle-CssClass="short tdPhase" HeaderStyle-CssClass="short" SortExpression="phase" />
                <asp:TemplateField HeaderText="Attributes" ItemStyle-CssClass="short tdAttributes" HeaderStyle-CssClass="short" SortExpression="attributes">
                    <ItemTemplate>
                        <asp:Repeater runat="server" DataSource='<%# Eval("Attributes") %>'>
                            <ItemTemplate>
                                <%# Container.DataItem.ToString() ?? string.Empty%><br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <button
            type="button"
            class="btn btn-danger btn-floating btn-lg"
            id="btn-back-to-top">
            <i class="fas fa-arrow-up"></i>
        </button>
    </div>

    <!-- Tipo campo Elenco -->
    <script>
        function removeItemContiene(el) {
            let hid = $('.ele-contiene input[type=hidden]')
            let valoreStringa = el.parent('.list-group-item').attr('valoreStringa')
            if (hid.length > 0 && hid.val() != null) {
                // ottengo un array degli elementi della lista
                listaDati = hid.val().split(';')
                console.log(listaDati)
                // rimuovo elemento da array
                for (var i = listaDati.length - 1; i >= 0; i--) {
                    if (listaDati[i] === valoreStringa) {
                        listaDati.splice(i, 1)
                        break
                    }
                }
                console.log(listaDati)
                // aggiorno la input hidden
                hid.val(listaDati.join(';'))

                // eliminazione elemento dalla lista
                $('.ele-contiene-list .list-group-item').each(function () {
                    let value = $(this).children('span').text()
                    if (value == valoreStringa) {
                        // elimino la riga
                        $(this).remove()
                    }
                })
            }
        }

        function removeItemContieneEdit(el) {
            let valoreStringa = el.parent('.list-group-item').attr('valoreStringa')
            let hid = $('#valoriContiene')
            if (hid.length > 0 && hid.val() != null) {
                // ottengo un array degli elementi della lista
                listaDati = hid.val().split(';')
                // rimuovo elemento da array
                for (var i = listaDati.length - 1; i >= 0; i--) {
                    if (listaDati[i] === valoreStringa) {
                        listaDati.splice(i, 1)
                        break
                    }
                }

                // aggiorno la input hidden
                hid.val(listaDati.join(';'))

                // eliminazione elemento dalla lista
                $('.ele-contiene-list .list-group-item').each(function () {
                    let value = $(this).children('span').text()
                    if (value == valoreStringa) {
                        // elimino la riga
                        $(this).remove()
                    }
                })
            }
        }

        function loadListContieneInEdit() {
            // carico hidden con gli elementi dell'elenco
            let loadedHid = $('#valoriContiene')

            if (loadedHid && loadedHid.val()) {
                // ottengo un array degli elementi della lista
                let listaDati = loadedHid.val().split(';')

                // visualizzo la lista
                listaDati.forEach(element => {
                    $('.ele-contiene-list').append(
                        '<div class="list-group-item" valoreStringa="' + escapeHtml(element) + '"><span>' + element + '</span>' +
                        '<button class="ele-contiene-remove btn btn-danger btn-sm" onclick="removeItemContieneEdit(' +
                        '$(this)' + ')" role="button" type="button">' +
                        '<i class="fa fa-trash" aria-hidden="true"></i>' +
                        '</button>' +
                        '</div>'
                    )
                })
            }
        }

        $(document).ready(function () {
            // carico la lista in modifica
            loadListContieneInEdit()

            $('.ele-contiene-add').click(function () {
                let edit = $(this).parent().siblings('.ele-contiene-edit')
                if (edit.length > 0 && edit.val() != '') {
                    // valore nuovo elemento
                    let oldElement = $.trim(edit.val())
                    let newElement = escapeHtml(oldElement)
                    // pulisco la textbox di input
                    edit.val('')

                    // inserisco elemento nel campo hidden
                    let hid = $(this).parent().parent().siblings('input[type=hidden]')
                    let hidText = hid.val() == '' ? oldElement : ';' + oldElement
                    hid.val(hid.val() + hidText)
                    // inserisco elemento nella lista
                    let list = $(this).parent().parent().siblings('.ele-contiene-list')
                    list.append(
                        '<div class="list-group-item" valoreStringa="' + newElement + '"><span>' + newElement + '</span>' +
                        '<button class="ele-contiene-remove btn btn-danger btn-sm" onclick="removeItemContiene(' +
                        '$(this)' + ')" role="button" type="button">' +
                        '<i class="fa fa-trash" aria-hidden="true"></i>' +
                        '</button>' +
                        '</div>'
                    )
                }
            })
        })
    </script>

    <script>
        function removeItem(el) {
            let hid = $('.ele-noncontiene input[type=hidden]')
            let valoreStringa = el.parent('.list-group-item').attr('valoreStringa')
            if (hid.length > 0 && hid.val() != null) {
                // ottengo un array degli elementi della lista
                listaDati = hid.val().split(';')

                // rimuovo elemento da array
                for (var i = listaDati.length - 1; i >= 0; i--) {
                    if (listaDati[i] === valoreStringa) {
                        listaDati.splice(i, 1)
                        break
                    }
                }

                // aggiorno la input hidden
                hid.val(listaDati.join(';'))

                // eliminazione elemento dalla lista
                $('.ele-noncontiene-list .list-group-item').each(function () {
                    let value = $(this).children('span').text()
                    if (value == valoreStringa) {
                        // elimino la riga
                        $(this).remove()
                    }
                })
            }
        }

        function removeItemEdit(el) {
            let valoreStringa = el.parent('.list-group-item').attr('valoreStringa')
            let hid = $('#valoriNonContiene')
            if (hid.length > 0 && hid.val() != null) {
                // ottengo un array degli elementi della lista
                listaDati = hid.val().split(';')
                // rimuovo elemento da array
                for (var i = listaDati.length - 1; i >= 0; i--) {
                    if (listaDati[i] === valoreStringa) {
                        listaDati.splice(i, 1)
                        break
                    }
                }

                // aggiorno la input hidden
                hid.val(listaDati.join(';'))

                // eliminazione elemento dalla lista
                $('.ele-noncontiene-list .list-group-item').each(function () {
                    let value = $(this).children('span').text()
                    if (value == valoreStringa) {
                        // elimino la riga
                        $(this).remove()
                    }
                })
            }
        }

        function loadListInEdit() {
            // carico hidden con gli elementi dell'elenco
            let loadedHid = $('#valoriNonContiene')

            if (loadedHid && loadedHid.val()) {
                // ottengo un array degli elementi della lista
                let listaDati = loadedHid.val().split(';')

                // visualizzo la lista
                listaDati.forEach(element => {
                    $('.ele-noncontiene-list').append(
                        '<div class="list-group-item" valoreStringa="' + escapeHtml(element) + '"><span>' + element + '</span>' +
                        '<button class="ele-noncontiene-remove btn btn-danger btn-sm" onclick="removeItemEdit(' +
                        '$(this)' + ')" role="button" type="button">' +
                        '<i class="fa fa-trash" aria-hidden="true"></i>' +
                        '</button>' +
                        '</div>'
                    )
                })
            }
        }

        $(document).ready(function () {
            // carico la lista in modifica
            loadListInEdit()

            $('.ele-noncontiene-add').click(function () {
                let edit = $(this).parent().siblings('.ele-noncontiene-edit')
                if (edit.length > 0 && edit.val() != '') {
                    // valore nuovo elemento
                    let oldElement = $.trim(edit.val())
                    let newElement = escapeHtml(oldElement)
                    // pulisco la textbox di input
                    edit.val('')

                    // inserisco elemento nel campo hidden
                    let hid = $(this).parent().parent().siblings('input[type=hidden]')
                    let hidText = hid.val() == '' ? oldElement : ';' + oldElement
                    hid.val(hid.val() + hidText)
                    // inserisco elemento nella lista
                    let list = $(this).parent().parent().siblings('.ele-noncontiene-list')
                    list.append(
                        '<div class="list-group-item" valoreStringa="' + newElement + '"><span>' + newElement + '</span>' +
                        '<button class="ele-noncontiene-remove btn btn-danger btn-sm" onclick="removeItem(' +
                        '$(this)' + ')" role="button" type="button">' +
                        '<i class="fa fa-trash" aria-hidden="true"></i>' +
                        '</button>' +
                        '</div>'
                    )
                }
            })
        })
    </script>

    <script>
        var entityMap = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;',
            '/': '&#x2F;',
            '`': '&#x60;',
            '=': '&#x3D;'
        };

        function escapeHtml(string) {
            return String(string).replace(/[&<>"'`=\/]/g, function (s) {
                return entityMap[s];
            });
        }
    </script>

    <!-- evidenzia parole -->
    <script>
        $(document).ready(function () {
            // se è attiva l'opzione
            if ($('#chkSottolineaParole').is(":checked")) {
                // prendo tutti gli input delle colonne dove ricercare le parole
                var inputs = $("#cklColonne :input")

                inputs.each(function (index) {
                    // verifico se è selezionata la colonna per la ricerca
                    if ($(this).is(":checked")) {
                        // prendo la classe delle celle dove effettuare la ricerca
                        var classe = getColonnaRiferimento($(this))

                        // ciclo le celle
                        $(classe).each(function () {
                            // sottolineo il testo ricercato dall'utente e lo imposto nella cella
                            var testo = sottolineaParoleRicercate($(this).html())
                            $(this).html(testo)
                        });
                    }
                });
            }
        });

        // funzione per estrapolare la classe delle celle desiderate
        function getColonnaRiferimento(el) {
            if (el.val() == "File") {
                return ".tdFile"
            }
            else if (el.val() == "Sequid") {
                return ".tdSequid"
            }
            else if (el.val() == "Source") {
                return ".tdSource"
            }
            else if (el.val() == "Type") {
                return ".tdType"
            }
            else if (el.val() == "Start") {
                return ".tdStart"
            }
            else if (el.val() == "End") {
                return ".tdEnd"
            }
            else if (el.val() == "Score") {
                return ".tdScore"
            }
            else if (el.val() == "Strand") {
                return ".tdStrand"
            }
            else if (el.val() == "Phase") {
                return ".tdPhase"
            }
            else if (el.val() == "Attributes") {
                return ".tdAttributes"
            }
        }

        // funzione per sottolineare le parole ricercate
        function sottolineaParoleRicercate(testo) {
            if ($('#valoriContiene').val() != null) {
                var split = $('#valoriContiene').val().split(';')

                var testoFinale = testo
                split.forEach(function (item) {
                    if (testo.indexOf(item) >= 0) {
                        testoFinale = testoFinale.replace(item, '<span style="background-color: yellow;">' + item + '</span>')
                    }
                });

                return testoFinale
            }
        }
    </script>

    <script runat="server">
        protected string GetFileName(object url)
        {
            if (url != null)
            {
                string urlString = url.ToString();
                string nomeFile = System.IO.Path.GetFileName(urlString);
                return nomeFile;
            }
            return string.Empty;
        }
    </script>

    <!-- Abilita popper -->
    <script>
        $(document).ready(function () {
            var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
            var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
                return new bootstrap.Popover(popoverTriggerEl)
            })
        });
    </script>

    <!-- imposto la classe corretta alle checkbox -->
    <script>
        $(document).ready(function () {
            $("input[type=checkbox]").addClass("form-check-input");
        });
    </script>
</asp:Content>
