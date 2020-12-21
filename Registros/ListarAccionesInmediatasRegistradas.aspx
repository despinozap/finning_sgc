<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListarAccionesInmediatasRegistradas.aspx.cs" Inherits="NCCSAN.Registros.ListarAccionesInmediatasIngresadas" %>

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
            Acciones Inmediatas Registradas</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Código o W/O"></asp:Label>
                            <asp:TextBox ID="txtCodigoWO" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ibSearch" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/search.png"
                                AlternateText="Buscar" ToolTip="Buscar" />
                            <asp:ImageButton ID="ibClearCodigoWO" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/clear.png"
                                OnClick="ibClearCodigoWO_Click" AlternateText="Limpiar" ToolTip="Limpiar" />
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
                            DataSourceID="SDSEventos" HorizontalAlign="Center" Width="100%" AllowPaging="True"
                            OnDataBound="gvEventos_DataBound" PageSize="20" OnRowCommand="gvEventos_RowCommand" ShowHeaderWhenEmpty="True">
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
                                <asp:BoundField DataField="codigo" HeaderText="Código" ReadOnly="True" SortExpression="codigo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="work_order" HeaderText="W/O" SortExpression="work_order">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha_ingreso" HeaderText="Fecha" SortExpression="fecha_ingreso">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_cliente" HeaderText="Cliente" SortExpression="nombre_cliente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_fuente" HeaderText="Fuente" SortExpression="nombre_fuente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="irc" HeaderText="IRC" SortExpression="irc">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_componente" HeaderText="Componente" SortExpression="nombre_componente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha_ejecucion" HeaderText="Fecha ejecución" SortExpression="fecha_ejecucion">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_origen" HeaderText="Origen falla" 
                                    SortExpression="nombre_origen">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:ButtonField ButtonType="Image" CommandName="EditarAccionInmediata" 
                                    HeaderText="Editar" ImageUrl="~/Images/Icon/edit.png" Text="Editar">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Image" CommandName="EliminarAccionInmediata" HeaderText="Eliminar"
                                    ImageUrl="~/Images/Icon/remove_file.png" Text="Eliminar">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Image" CommandName="DetalleAccionInmediata" HeaderText="Detalle"
                                    ImageUrl="~/Images/Icon/search.png" Text="Detalle">
                                    <ItemStyle HorizontalAlign="Center" />
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
                        <asp:SqlDataSource ID="SDSEventos" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            
                            SelectCommand="SELECT e.codigo, e.work_order, CONVERT(VARCHAR(10), e.fecha_ingreso, 105) AS fecha_ingreso, e.nombre_cliente, e.nombre_fuente, e.irc, e.nombre_componente, CONVERT(VARCHAR(10), ai.fecha_accion, 105) AS fecha_ejecucion, ai.nombre_origen FROM Evento e, AccionInmediata ai WHERE (e.id_centro=@id_centro) AND (ai.codigo_evento=e.codigo) AND (e.nombre_cliente LIKE '%' + @nombre_cliente + '%') AND (CONVERT(VARCHAR(10), e.fecha_ingreso, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%')) ORDER BY e.fecha_ingreso DESC">
                            <SelectParameters>
                                <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                <asp:ControlParameter ControlID="ddlCliente" DefaultValue="" Name="nombre_cliente"
                                    PropertyName="SelectedValue" ConvertEmptyStringToNull="False" Type="String" />
                                <asp:ControlParameter ControlID="hfFecha" DefaultValue="" Name="fecha" PropertyName="Value"
                                    ConvertEmptyStringToNull="False" Type="String" />
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
            <!-- Panel Confirmar Eliminar Acción Inmediata -->
            <asp:HiddenField ID="hfConfirmEliminar" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmEliminar_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmEliminar"
                PopupControlID="pnConfirmEliminar">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmEliminar" runat="server" style="width: 100%" UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmEliminar" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center"
                        Width="100%">
                        <asp:Label ID="lbConfirmEliminar" runat="server" CssClass="messageBoxTitle" Text="Se eliminará la Acción Inmediata y todos sus datos asociados. ¿Desea continuar?"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmEliminarSi" runat="server" Text="Si" Width="70px" CssClass="submitButton"
                                        OnClick="btConfirmEliminarSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmEliminarNo" runat="server" Text="No" Width="70px" CssClass="submitButton"
                                        OnClick="btConfirmEliminarNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Eliminar Acción Inmediata !-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
