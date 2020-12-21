<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MatrizConsecuencia.aspx.cs" Inherits="NCCSAN.Administracion.MatrizConsecuencia" %>

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
            Matriz de Consecuencia</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnMatrizConsecuencia" runat="server" HorizontalAlign="Center">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Actualizar Matriz de Consecuencia</h1>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:FileUpload ID="fuArchivo" runat="server" Width="500" multiple="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" align="center">
                            <asp:ImageButton ID="ibActualizarMatrizConsecuencia" runat="server" ImageAlign="AbsMiddle"
                                ImageUrl="~/Images/Button/save.png" CssClass="submitPanelButton" 
                                onclick="ibActualizarMatrizConsecuencia_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibActualizarMatrizConsecuencia" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
