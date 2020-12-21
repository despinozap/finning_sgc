<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AccesoDenegado.aspx.cs" Inherits="NCCSAN.AccesoDenegado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <div class="divTituloForm">
            <h1>Acceso Denegado</h1>
        </div>
    </div>
    <table align="center">
        <tr>
            <td align="center">
                <asp:Image ID="imgAccessDenied" runat="server" ImageUrl="~/Images/access_denied.png" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" 
                    Text="El tipo de cuenta que estás utilizando no tiene los permisos suficientes para acceder al contenido de la página solicitada" 
                    Font-Size="Medium"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
