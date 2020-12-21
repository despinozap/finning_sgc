<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GraficoInvolucradoAnual.aspx.cs" Inherits="NCCSAN.Analisis.GraficoInvolucradoAnual" %>

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
            Involucrado en el año</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label1" runat="server" Text="Involucrado"></asp:Label>
                            <asp:TextBox ID="txtNombreInvolucrado" runat="server" Width="300px" 
                                Enabled="False" ReadOnly="True"></asp:TextBox>
                            <asp:Button ID="btBuscarInvolucrado" runat="server" Text="Seleccionar" 
                                onclick="btBuscarInvolucrado_Click"></asp:Button>
                            <asp:HiddenField ID="hfRutInvolucrado" runat="server"></asp:HiddenField>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" colspan="2" align="center">
                            <asp:Button ID="btGenerar" runat="server" Text="Generar" OnClick="btGenerar_Click"
                                    class="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div align="center">
                <asp:Chart ID="ChartInvolucradoAnual" runat="server" Width="1024px" Height="400px" >
                </asp:Chart>
            </div>
            <!-- Panel Buscar Persona -->
            <asp:HiddenField ID="hfBuscarPersona" runat="server" />
            <asp:ModalPopupExtender ID="hfBuscarPersona_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upBuscarPersona" TargetControlID="hfBuscarPersona">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upBuscarPersona" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnBuscarPersona" runat="server" CssClass="Popup_frontground">
                        <table class="tableControls">
                            <tr>
                                <td colspan="2" class="tdTitle">
                                    <h1>
                                        Buscar persona</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label22" runat="server" Text="Apellido"></asp:Label>
                                    <asp:TextBox ID="txtBuscarPersonaApellido" runat="server" AutoPostBack="True"></asp:TextBox>
                                    <asp:Button ID="btBuscarPersonaBuscar" runat="server" Text="Buscar" />
                                    <asp:Button ID="btBuscarPersonaLimpiar" runat="server" Text="Limpiar" OnClick="btBuscarPersonaLimpiar_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:GridView ID="gvBuscarPersonas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                        DataKeyNames="rut" DataSourceID="SDSBuscarPersonas" HorizontalAlign="Center"
                                        OnRowCommand="gvBuscarPersonas_RowCommand" Width="1010px">
                                        <AlternatingRowStyle BackColor="#EDEDED" />
                                        <Columns>
                                            <asp:ButtonField HeaderText="Seleccionar" ButtonType="Image" ImageUrl="~/Images/Icon/add_user.png"
                                                CommandName="SetInvolucrado" ItemStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Middle">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:ButtonField>
                                            <asp:BoundField DataField="rut" HeaderText="RUT" ReadOnly="True" SortExpression="rut" />
                                            <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                                            <asp:BoundField DataField="edad" HeaderText="Edad" ReadOnly="True" SortExpression="edad">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="cargo" HeaderText="Cargo" SortExpression="cargo">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="antiguedad" HeaderText="Antigüedad" ReadOnly="True" SortExpression="antiguedad">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle BackColor="#666666" ForeColor="White" />
                                        <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                            PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                            NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                        <PagerStyle HorizontalAlign="Center" />
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="SDSBuscarPersonas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                        SelectCommand="SELECT p.rut, p.nombre, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p WHERE (p.nombre LIKE '%' + @nombre + '%') AND (p.id_centro=@centro) AND (p.fecha_retiro IS NULL) AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') ORDER BY p.nombre ASC"
                                        OnSelected="SDSBuscarPersonas_Selected">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="txtBuscarPersonaApellido" Name="nombre" PropertyName="Text" />
                                            <asp:SessionParameter Name="centro" SessionField="id_centro" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                    <div align="left" style="text-align: center">
                                        <asp:Literal ID="ltBuscarPersonaSummary" runat="server"></asp:Literal>
                                    </div>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btBuscarPersonaCancelar" runat="server" Text="Cancelar" OnClick="btBuscarPersonaCancelar_Click"
                                        CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Buscar Persona !-->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
