<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TablaGestionPeriodo.aspx.cs" Inherits="NCCSAN.RSP.TablaGestionPeriodo" %>

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
           Gestión en periodo</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdUnique" align="right">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label1" runat="server" Text="Centro"></asp:Label>
                            <asp:DropDownList ID="ddlCentro" runat="server" Width="150px" 
                                AutoPostBack="True" DataSourceID="SDSCentro" DataTextField="nombre" 
                                DataValueField="id" ondatabound="ddlCentro_DataBound" 
                                onselectedindexchanged="ddlCentro_SelectedIndexChanged" >
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCentro" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT c.id, c.nombre FROM Centro c WHERE (c.id&lt;&gt;@id_ext) AND (c.id&lt;&gt;@id_rsp)">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="EXT" Name="id_ext" />
                                    <asp:Parameter DefaultValue="RSP" Name="id_rsp" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label3" runat="server" Text="Cliente"></asp:Label>
                            <asp:DropDownList ID="ddlCliente" runat="server" Width="150px" 
                                DataSourceID="SDSCliente" DataTextField="nombre_cliente" 
                                DataValueField="nombre_cliente" ondatabound="ddlCliente_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCliente" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT [nombre_cliente] FROM [CentroCliente] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlCentro" Name="id_centro" 
                                        PropertyName="SelectedValue" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique" align="left">
                            <asp:Button ID="btGenerar" runat="server" Text="Generar"
                                class="submitPanelButton" onclick="btGenerar_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tableControls" align="center" style="width: 100%">
                <tr>
                    <td colspan="4" class="tdTitle">
                        <h1>
                            Resumen del Año</h1>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique" align="center">
                        <asp:GridView ID="gvGestionPeriodoAnualResumen" runat="server" AutoGenerateColumns="False">
                            <HeaderStyle BackColor="#000000" ForeColor="#FFFFFF" />
                            <Columns>
                                <asp:TemplateField HeaderText="Elemento">
                                    <ItemTemplate>
                                        <asp:Label ID="lbNombreElemento" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Abierto">
                                    <ItemTemplate>
                                        <asp:Label ID="lbCantidadAbierto" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cerrado">
                                    <ItemTemplate>
                                        <asp:Label ID="lbCantidadCerrado" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total">
                                    <ItemTemplate>
                                        <asp:Label ID="lbCantidadTotal" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="tdTitle">
                        <h1>
                            Resumen del Mes</h1>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique" align="center">
                        <asp:GridView ID="gvGestionPeriodoMensualResumen" runat="server" AutoGenerateColumns="False">
                            <HeaderStyle BackColor="#000000" ForeColor="#FFFFFF" />
                            <Columns>
                                <asp:TemplateField HeaderText="Elemento">
                                    <ItemTemplate>
                                        <asp:Label ID="lbNombreElemento" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Abierto">
                                    <ItemTemplate>
                                        <asp:Label ID="lbCantidadAbierto" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cerrado">
                                    <ItemTemplate>
                                        <asp:Label ID="lbCantidadCerrado" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total">
                                    <ItemTemplate>
                                        <asp:Label ID="lbCantidadTotal" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
