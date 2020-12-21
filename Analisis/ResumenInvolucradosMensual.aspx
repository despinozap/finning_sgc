<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ResumenInvolucradosMensual.aspx.cs" Inherits="NCCSAN.Analisis.BonificacionMesual" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Resumen de Involucrados</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique" align="right">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px">
                            </asp:DropDownList>
                            <asp:Button ID="btDescargar" runat="server" Text="Descargar" OnClick="btDescargar_Click"
                                class="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btDescargar" />
        </Triggers>
    </asp:UpdatePanel>
    </asp:UpdatePanel>
</asp:Content>
