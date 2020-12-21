<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListarPlanesAccionPendientes.aspx.cs" Inherits="NCCSAN.PlanesAccion.ListarPlanesAccionPendientes" %>

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
            Lista de Planes de Acción Pendientes</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Código o W/O"></asp:Label>
                            <asp:TextBox ID="txtCodigoWO" runat="server" Width="120px"></asp:TextBox>
                            <asp:ImageButton ID="ibSearch" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/search.png"
                                AlternateText="Buscar" ToolTip="Buscar" />
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Cliente"></asp:Label>
                            <asp:DropDownList ID="ddlCliente" runat="server" DataSourceID="SDSCliente" DataTextField="nombre_cliente"
                                DataValueField="nombre_cliente" OnDataBound="ddlCliente_DataBound" AutoPostBack="True"
                                Width="300">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCliente" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT [nombre_cliente] FROM [CentroCliente] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label9" runat="server" Text="Área"></asp:Label>
                            <asp:DropDownList ID="ddlArea" runat="server" Width="150px" AutoPostBack="True" 
                                DataSourceID="SDSArea" DataTextField="nombre_area" DataValueField="nombre_area" 
                                ondatabound="ddlArea_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSArea" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" 
                                SelectCommand="SELECT DISTINCT [nombre_area] FROM [CentroArea] WHERE ([id_centro] = @id_centro)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlAnio_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlMes_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfFecha" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tableControls" align="center" style="width: 100%">
                <tr>
                    <td class="tdUnique">
                        <asp:GridView ID="gvEventos" runat="server" AutoGenerateColumns="False" DataKeyNames="codigo"
                            DataSourceID="SDSPlanesAccionPendientes" HorizontalAlign="Center" Width="100%"
                            OnRowCommand="gvEventos_RowCommand" AllowPaging="True" OnDataBound="gvEventos_DataBound"
                            PageSize="20" ShowHeaderWhenEmpty="True" >
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Image ID="imgEstadoIcono" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="codigo" HeaderText="Código" ReadOnly="True" SortExpression="codigo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="work_order" HeaderText="W/O" SortExpression="work_order">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_cliente" HeaderText="Cliente" SortExpression="nombre_cliente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="irc" HeaderText="IRC" SortExpression="irc">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_componente" HeaderText="Componente" SortExpression="nombre_componente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="dias_espera" HeaderText="Días en espera" 
                                    SortExpression="dias_espera">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:ButtonField ButtonType="Image" HeaderText="Ingresar Plan de Acción" ImageUrl="~/Images/Icon/list.png"
                                    Text="Ingresar Plan de Acción" CommandName="IngresarPlanAccion">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:ButtonField>
                            </Columns>
                            <HeaderStyle BackColor="#666666" ForeColor="White" />
                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                            <PagerStyle HorizontalAlign="Center" />
                            <RowStyle CssClass="rowData" />
                            <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            <EmptyDataTemplate>
                                <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                    Font-Size="14px"></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SDSPlanesAccionPendientes" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            
                            SelectCommand="SELECT e.codigo, e.work_order, e.nombre_cliente, e.nombre_fuente, e.irc, e.nombre_componente, DATEDIFF(DAY, ev.fecha, SYSDATETIME()) AS dias_espera FROM Evento e, Evaluacion ev WHERE (e.id_centro=@id_centro) AND (e.estado='Plan de acción pendiente') AND (ev.codigo_evento=e.codigo) AND (e.nombre_cliente LIKE '%' + @nombre_cliente + '%') AND (e.nombre_area LIKE '%' + @nombre_area + '%') AND (CONVERT(VARCHAR(10), e.fecha_ingreso, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%'))">
                            <SelectParameters>
                                <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                <asp:ControlParameter ControlID="ddlCliente" ConvertEmptyStringToNull="False" Name="nombre_cliente"
                                    PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="ddlArea" ConvertEmptyStringToNull="False" 
                                    Name="nombre_area" PropertyName="SelectedValue" />
                                <asp:ControlParameter ControlID="hfFecha" ConvertEmptyStringToNull="False" Name="fecha"
                                    PropertyName="Value" Type="String" />
                                <asp:ControlParameter ControlID="txtCodigoWO" ConvertEmptyStringToNull="False" Name="codigowo"
                                    PropertyName="Text" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique" align="left">
                        <asp:Panel ID="pnOpcionesLista" runat="server" Visible="False">
                            <asp:ImageButton ID="ibExportToExcel" runat="server" ImageUrl="~/Images/Icon/ms_excel.png"
                                OnClick="ibExportToExcel_Click" ToolTip="Descargar como planilla Excel" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
