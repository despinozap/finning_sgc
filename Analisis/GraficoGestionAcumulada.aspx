<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="GraficoGestionAcumulada.aspx.cs" Inherits="NCCSAN.Graficos.GestionAcumulada" %>

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
            Gestión acumulada</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <contenttemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Área"></asp:Label>
                            <asp:DropDownList ID="ddlArea" runat="server" DataSourceID="SDSAreas" DataTextField="nombre_area"
                                DataValueField="nombre_area" OnDataBound="ddlArea_DataBound" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlArea_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSAreas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT ca.nombre_area FROM CentroArea ca WHERE (ca.id_centro=@id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter DefaultValue="" Name="id_centro" 
                                        SessionField="id_centro" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Responsable"></asp:Label>
                            <asp:DropDownList ID="ddlResponsable" runat="server" OnDataBound="ddlResponsable_DataBound">
                            </asp:DropDownList>
                            <asp:Button ID="btGenerar" runat="server" Text="Generar" OnClick="btGenerar_Click"
                                class="submitPanelButton" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" colspan="3">
                            <asp:CheckBox ID="chbEventoAbierto" runat="server" Text="Eventos abiertos" Checked="True" />
                            <asp:CheckBox ID="chbEventoCerrado" runat="server" Text="Eventos cerrados" Checked="True" />
                            <asp:CheckBox ID="chbPlanAccionAbierto" runat="server" Text="Planes de acción abiertos"
                                Checked="True" />
                            <asp:CheckBox ID="chbPlanAccionCerrado" runat="server" Text="Planes de acción cerrados"
                                Checked="True" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="center">
                <asp:Chart ID="ChartGestionAcumulada" runat="server" Width="1024px" Height="400px">
                </asp:Chart>
            </div>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>
