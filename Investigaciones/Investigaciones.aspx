<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Investigaciones.aspx.cs" Inherits="NCCSAN.Investigaciones.Investigaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Resumen de Investigaciones</h1>
    </div>
    <br />
    <br />
    <asp:GridView ID="gvResumenInvestigaciones" runat="server" AutoGenerateColumns="False" Width="50%"
        HorizontalAlign="Center">
        <AlternatingRowStyle CssClass="rowDataAlt" />
        <Columns>
            <asp:TemplateField HeaderText="Icono">
                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                <ItemTemplate>
                    <asp:Image ID="imgIcono" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <Columns>
            <asp:TemplateField HeaderText="Estado">
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:Label ID="lbEstado" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <Columns>
            <asp:TemplateField HeaderText="Cantidad">
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:Label ID="lbCantidad" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle BackColor="#666666" ForeColor="White" />
        <RowStyle CssClass="rowData" />
    </asp:GridView>
</asp:Content>
