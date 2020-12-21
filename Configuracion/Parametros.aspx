<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Parametros.aspx.cs" Inherits="NCCSAN.Configuracion.Prametros" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Popup_background
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        
        .Popup_frontground
        {
            background-color: White;
            padding: 30px; /*border-style: dotted;
            border-width: 2px;*/
            box-shadow: 10px 10px 5px #555555;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Parámetros de Sistema</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnMaxFileSize" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Tamaño de Archivos</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbMaxFileSize" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Tipo" BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                    <asp:TableHeaderCell Text="Extensiones" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Tamaño máximo (en MegaBytes)" BackColor="#666666" ForeColor="White" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label1" runat="server" Text="Imagen"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label2" runat="server" Text="*.bmp, *.gif, *.jpeg, *.jpg, *.png, *.tif, *.tiff"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtMaxSizeImagen" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                        <asp:Label ID="Label13" runat="server" Text="MB"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label3" runat="server" Text="Documento"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label4" runat="server" Text="*.doc, *.docx, *.pdf, *.xlsx, *.xls, *.xlsm, *.msg, *.csv, *.xml, *.txt"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtMaxSizeDocumento" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                        <asp:Label ID="Label14" runat="server" Text="MB"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label9" runat="server" Text="Video"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label10" runat="server" Text="*.avi, *.mp4, *.wmv, *.mpeg, *.mpg, *.swf, *.qt"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtMaxSizeVideo" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                        <asp:Label ID="Label18" runat="server" Text="MB"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label7" runat="server" Text="Audio"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label8" runat="server" Text="*.mp3, *.wma, *.wav, *.ogg"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtMaxSizeAudio" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                        <asp:Label ID="Label15" runat="server" Text="MB"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label5" runat="server" Text="Diapositivas"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label6" runat="server" Text="*.ppt, *.pptx"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtMaxSizeDiapositivas" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                        <asp:Label ID="Label16" runat="server" Text="MB"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label11" runat="server" Text="Comprimido"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label12" runat="server" Text="*.zip, *.rar"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtMaxSizeComprimido" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                        <asp:Label ID="Label17" runat="server" Text="MB"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
            <table align="center">
                <tr>
                    <td>
                        <asp:ImageButton ID="ibGuardarConfiguracionParametros" runat="server" CssClass="submitPanelButton"
                            ImageUrl="~/Images/Button/save.png" 
                            onclick="ibGuardarConfiguracionParametros_Click1" />
                    </td>
                </tr>
            </table>
            <div align="left" style="color: #FF0000; text-align: center">
                <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
            </div>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
