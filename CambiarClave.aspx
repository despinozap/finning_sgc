<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CambiarClave.aspx.cs" Inherits="NCCSAN.Account.CambiarClave" %>

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
            Cambiar clave de acceso</h1>
    </div>
    <br />
    <br />
    <asp:UpdatePanel ID="uPanel" runat="server" ViewStateMode="Inherit" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="tableControls" align="center">
                <tr>
                    <td class="tdUnique">
                        <asp:Label ID="Label1" runat="server" Text="Clave actual"></asp:Label>
                    </td>
                    <td class="tdUnique">
                        <asp:TextBox ID="txtClave" runat="server" MaxLength="30" TextMode="Password" ViewStateMode="Enabled"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique">
                        <asp:Label ID="Label2" runat="server" Text="Nueva clave"></asp:Label>
                    </td>
                    <td class="tdUnique">
                        <asp:TextBox ID="txtNuevaClave" runat="server" TextMode="Password" MaxLength="30"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique">
                        <asp:Label ID="Label3" runat="server" Text="Repetir nueva clave"></asp:Label>
                    </td>
                    <td class="tdUnique">
                        <asp:TextBox ID="txtRepeticionClave" runat="server" MaxLength="30" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <asp:Panel ID="Panel1" runat="server">
                <table align="center">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibCambiarClave" runat="server" CssClass="submitPanelButton"
                                ImageUrl="~/Images/Button/save.png" OnClick="ibCambiarClave_Click" />
                        </td>
                    </tr>
                </table>
                <div align="left" style="color: #FF0000; text-align: center">
                    <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
                </div>
            </asp:Panel>
            <!-- Panel Message -->
            <asp:HiddenField ID="hfMessage" runat="server" />
            <asp:ModalPopupExtender ID="hfMessage_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upMessage" TargetControlID="hfMessage">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upMessage" runat="server" UpdateMode="Always" style="width: 100%">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnMessage" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <asp:Label ID="lbMessage" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <asp:Label ID="Label6" runat="server" Text="Se ha cambiado la clave de acceso exitosamente"
                            CssClass="messageBoxMessage"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btMessageOk" runat="server" Text="Aceptar" OnClick="btMessageOk_Click"
                                        Width="70px" CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btMessageOk" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Panel Message !-->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
