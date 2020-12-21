<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BaseDatos.aspx.cs" Inherits="NCCSAN.Analisis.BaseDatos" %>

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
            Base de Datos</h1>
    </div>
    <asp:Panel ID="pnResumenes" runat="server">
        <table align="center" class="tableControls">
            <tr>
                <td class="tdTitle">
                    <h1>
                        Exportar registros a Excel</h1>
                </td>
            </tr>
            <tr>
                <td class="tdUnique" align="center">
                    <asp:Table ID="tbResumenes" runat="server" Width="100%">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell Text="Eventos" BackColor="#666666" ForeColor="White" />
                            <asp:TableHeaderCell Text="Acciones Inmediatas" BackColor="#666666" ForeColor="White" />
                            <asp:TableHeaderCell Text="Evaluaciones" BackColor="#666666" ForeColor="White" />
                            <asp:TableHeaderCell Text="Planes de Acción" BackColor="#666666" ForeColor="White" />
                            <asp:TableHeaderCell Text="Acciones Correctivas" BackColor="#666666" ForeColor="White" />
                            <asp:TableHeaderCell Text="Full" BackColor="#666666" ForeColor="White" />
                        </asp:TableHeaderRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Center" BorderColor="#666666" BorderWidth="1" BorderStyle="Solid">
                                <asp:ImageButton ID="ibExportEventosToExcel" runat="server" CommandName="ExportEventos" ImageAlign="Middle" ImageUrl="~/Images/Icon/download.png" OnClick="ibExportEventosToExcel_Click" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center" BorderColor="#666666" BorderWidth="1" BorderStyle="Solid">
                                <asp:ImageButton ID="ibExportAccionesInmediatasToExcel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Icon/download.png" OnClick="ibExportAccionesInmediatasToExcel_Click" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center" BorderColor="#666666" BorderWidth="1" BorderStyle="Solid">
                                <asp:ImageButton ID="ibExportEvaluacionesToExcel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Icon/download.png" OnClick="ibExportEvaluacionesToExcel_Click" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center" BorderColor="#666666" BorderWidth="1" BorderStyle="Solid">
                                <asp:ImageButton ID="ibExportPlanesAccionToExcel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Icon/download.png" OnClick="ibExportPlanesAccionToExcel_Click" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center" BorderColor="#666666" BorderWidth="1" BorderStyle="Solid">
                                <asp:ImageButton ID="ibExportAccionesCorrectivasToExcel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Icon/download.png" OnClick="ibExportAccionesCorrectivasToExcel_Click" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Center" BorderColor="#666666" BorderWidth="1" BorderStyle="Solid">
                                <asp:ImageButton ID="ibExportFullToExcel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Icon/download.png" OnClick="ibExportFullToExcel_Click" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

