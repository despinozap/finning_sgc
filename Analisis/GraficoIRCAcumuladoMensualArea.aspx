<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="GraficoIRCAcumuladoMensualArea.aspx.cs" Inherits="NCCSAN.Graficos.IRCAcumuladoMensualArea" %>

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
            IRC acumulado por área</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <contenttemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label7" runat="server" Text="Año"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label8" runat="server" Text="Mes"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label9" runat="server" Text="a1"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label10" runat="server" Text="b1"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label12" runat="server" Text="a2"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label14" runat="server" Text="b2"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label15" runat="server" Text="a3"></asp:Label>
                        </td>
                        <td class="tdFirst" align="left">
                            <asp:Label ID="Label16" runat="server" Text="b3"></asp:Label>
                        </td>
                        <td class="tdUnique" rowspan="2" align="center">
                            <asp:Button ID="Button1" runat="server" Text="Generar" OnClick="btGenerar_Click"
                                class="submitPanelButton" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" align="right">
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdLast" align="left">
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdLast" align="right">
                            <asp:TextBox ID="txtA1" runat="server" MaxLength="5" TextMode="Number" 
                                Width="70px" BorderColor="Lime">200</asp:TextBox>
                        </td>
                        <td class="tdLast" align="left">
                            <asp:TextBox ID="txtB1" runat="server" MaxLength="5" TextMode="Number" 
                                Width="70px" BorderColor="Lime">10</asp:TextBox>
                        </td>
                        <td class="tdLast" align="right">
                            <asp:TextBox ID="txtA2" runat="server" MaxLength="5" TextMode="Number" 
                                Width="70px" BorderColor="#FFCC00">400</asp:TextBox>
                        </td>
                        <td class="tdLast" align="left">
                            <asp:TextBox ID="txtB2" runat="server" MaxLength="5" TextMode="Number" 
                                Width="70px" BorderColor="#FFCC00">20</asp:TextBox>
                        </td>
                        <td class="tdLast" align="right">
                            <asp:TextBox ID="txtA3" runat="server" MaxLength="5" TextMode="Number" 
                                Width="70px" BorderColor="Red">800</asp:TextBox>
                        </td>
                        <td class="tdLast" align="left">
                            <asp:TextBox ID="txtB3" runat="server" MaxLength="5" TextMode="Number" 
                                Width="70px" BorderColor="Red">40</asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="center">
                <asp:Chart ID="ChartIRCAcumulado" runat="server" Width="1024px" Height="400px">
                </asp:Chart>
            </div>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>
