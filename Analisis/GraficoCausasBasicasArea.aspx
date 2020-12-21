<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GraficoCausasBasicasArea.aspx.cs" Inherits="NCCSAN.Analisis.GraficoCausasBasicasArea" %>

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
            Causas básicas por Área</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <contenttemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique" align="center">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique" align="center">
                            <asp:Label ID="Label1" runat="server" Text="Área"></asp:Label>
                            <asp:DropDownList ID="ddlArea" runat="server" Width="150px" AutoPostBack="True" 
                                DataSourceID="SDSArea" DataTextField="nombre_area" DataValueField="nombre_area" 
                                ondatabound="ddlArea_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSArea" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT DISTINCT [nombre_area] FROM [CentroArea] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <asp:Button ID="btGenerar" runat="server" Text="Generar" OnClick="btGenerar_Click"
                                class="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="center">
                <asp:Chart ID="ChartCausasBasicasInterno" runat="server" Width="1024px" Height="400px" >
                </asp:Chart>
            </div>
            <br />
            <div align="center">
                <asp:Chart ID="ChartCausasBasicasExterno" runat="server" Width="1024px" Height="400px" >
                </asp:Chart>
            </div>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>
