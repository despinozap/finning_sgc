<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="GraficoEventosMensual.aspx.cs" Inherits="NCCSAN.Graficos.EventosMensual" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Eventos en el mes</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <contenttemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique" align="right">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                            <asp:Button ID="btGenerar" runat="server" Text="Generar" OnClick="btGenerar_Click"
                                class="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="center">
                <asp:Chart ID="ChartEventosMensual" runat="server" Width="1024px" Height="400px" >
                </asp:Chart>
            </div>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>
