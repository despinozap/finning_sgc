<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CuentaCorreoSistema.aspx.cs" Inherits="NCCSAN.Configuracion.CuentaCorreoSistema" %>

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
            Cuenta de Correo SGC</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnMaxFileSize" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Credencial de acceso</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbCredential" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Dato" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Valor" BackColor="#666666" ForeColor="White" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Right">
                                        <asp:Label ID="Label2" runat="server" Text="Usuario"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:TextBox ID="txtUser" runat="server" Width="200px"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Right">
                                        <asp:Label ID="Label1" runat="server" Text="Contraseña"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:TextBox ID="txtPassword" runat="server" Width="200px"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Right">
                                        <asp:Label ID="Label3" runat="server" Text="Correo"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:TextBox ID="txtEmail" runat="server" Width="300px"></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Right">
                                        <asp:Label ID="Label4" runat="server" Text="Activado" Width="200px"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:CheckBox ID="chbActive" runat="server"></asp:CheckBox>
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
