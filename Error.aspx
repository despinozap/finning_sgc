<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Error.aspx.cs" Inherits="NCCSAN.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>Error</h1>
    </div>
    
    <h1>Se ha recibido el siguiente error:</h1>
    <asp:Label ID="lbMensaje" runat="server" Text="Label" Font-Size="X-Large" ForeColor="#FF3300"></asp:Label>
    <br />
    <br />
    <asp:Label ID="Label1" runat="server" Text="Si el problema persiste, comuníquese con el personal del área "></asp:Label><asp:Label
        ID="Label2" runat="server" Text="Aseguramiento de Calidad y Mejoramiento continuo" Font-Bold="True"></asp:Label>
        <br />
    <asp:Label ID="lbErrorSaved" runat="server" 
        Text="El error ha sido registrado en el sistema para una posterior revisión" Visible="False"></asp:Label>
</asp:Content>
