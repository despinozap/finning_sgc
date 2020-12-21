<%@ Page Title="Sistema GEC - Finning Chile S.A 2014" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="NCCSAN._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="divTituloForm">
        <h1>Dirección de Soporte a la Minería</h1>
    </div>
    <h2>
         Sistema de Gestión de Calidad - Finning
    </h2>
    <p>
        El SGC está diseñado para administrar los incidentes de calidad reportados bajo normativas internacionales que permiten ejercer control y efectiva gestión de los eventos para mejorar los productos y servicios entregados
    </p>
    <br />
    <table align="center">
        <tr>
            <td align="center">
                <asp:Image ID="imgEquipos" runat="server" ImageAlign="Middle" 
                    ImageUrl="~/Images/machines.png" Width="100%" />
            </td>
        </tr>
    </table>
</asp:Content>
