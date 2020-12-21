<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListarPlanesAccionEnCurso.aspx.cs" Inherits="NCCSAN.PlanesAccion.ListarPlanesAccionEnCurso" %>

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
            Lista de Planes de Acción en Curso</h1>
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
                            DataSourceID="SDSPlanesAccionEnCurso" HorizontalAlign="Center" Width="100%" OnRowCommand="gvEventos_RowCommand"
                            AllowPaging="True" OnDataBound="gvEventos_DataBound" PageSize="20" ShowHeaderWhenEmpty="True">
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
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
                                <asp:TemplateField HeaderText="Progreso">
                                    <ItemTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td align="center" width="5%">
                                                    <asp:Label ID="lbProgreso" runat="server" Text='<%# Eval("progreso") %>' Font-Bold="True"></asp:Label>
                                                </td>
                                                <td align="left" width="95%">
                                                    <asp:Panel ID="pnProgreso" runat="server" Height="15" Width="100%" BackColor="#CCCCCC">
                                                        <asp:Panel ID="pnEjecutado" runat="server" Height="15" Width="30%">
                                                        </asp:Panel>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:ButtonField ButtonType="Image" HeaderText="Editar" ImageUrl="~/Images/Icon/edit.png"
                                    Text="Editar" CommandName="EditarPlanAccion">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:ButtonField>
                                <asp:TemplateField HeaderText="Cerrar">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibCerrarPlanAccion" runat="server" CommandName="CerrarPlanAccion"
                                            ImageAlign="Middle" ImageUrl="~/Images/Icon/shutdown.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:ButtonField ButtonType="Image" CommandName="DetallePlanAccion" HeaderText="Detalle"
                                    ImageUrl="~/Images/Icon/search.png" Text="Detalle">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:TemplateField HeaderText="Eliminar">
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibEliminarPlanAccion" runat="server" CommandName="EliminarPlanAccion"
                                            ImageAlign="Middle" ImageUrl="~/Images/Icon/remove_file.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
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
                        <asp:SqlDataSource ID="SDSPlanesAccionEnCurso" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            SelectCommand="SELECT e.codigo, e.work_order, e.nombre_cliente, e.nombre_fuente, e.irc, e.nombre_componente, pa.progreso FROM Evento e, PlanAccion pa WHERE (e.id_centro=@id_centro) AND (pa.codigo_evento=e.codigo) AND (e.estado='Plan de acción en curso') AND (pa.fecha_cierre IS NULL) AND (e.nombre_cliente LIKE '%' + @nombre_cliente + '%') AND (e.nombre_area LIKE '%' + @nombre_area + '%') AND (CONVERT(VARCHAR(10), e.fecha_ingreso, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%'))">
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
            <!-- Panel Confirmar Cerrar Plan Acción -->
            <asp:HiddenField ID="hfCodigoPlanAccionCerrar" runat="server" />
            <asp:HiddenField ID="hfConfirmCerrarPlanAccion" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmCerrarPlanAccion_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmCerrarPlanAccion"
                PopupControlID="pnConfirmCerrarPlanAccion">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmCerrarPlanAccion" runat="server" style="width: 100%"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmCerrarPlanAccion" runat="server" CssClass="Popup_frontground"
                        HorizontalAlign="Center" Width="100%">
                        <asp:Label ID="lbConfirmCerrarPlanAccion" runat="server" CssClass="messageBoxTitle"
                            Text="Antes de cerrar el Plan de Acción revise que la ejecución de todas las Acciones Correctivas esté conforme. ¿Desea continuar?"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmCerrarPlanAccionSi" runat="server" Text="Si" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmCerrarPlanAccionSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmCerrarPlanAccionNo" runat="server" Text="No" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmCerrarPlanAccionNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Cerrar Plan Acción !-->
            <!-- Panel Confirmar Eliminar Plan Acción -->
            <asp:HiddenField ID="hfCodigoPlanAccionEliminar" runat="server" />
            <asp:HiddenField ID="hfConfirmEliminarPlanAccion" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmEliminarPlanAccion_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmEliminarPlanAccion"
                PopupControlID="pnConfirmEliminarPlanAccion">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmEliminarPlanAccion" runat="server" style="width: 100%"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmEliminarPlanAccion" runat="server" CssClass="Popup_frontground"
                        HorizontalAlign="Center" Width="100%">
                        <asp:Label ID="lbConfirmEliminarPlanAccion" runat="server" CssClass="messageBoxTitle"
                            Text="Se va a eliminar el Plan de Acción junto a todas las Acciones Correctivas y el progreso de ellas. ¿Desea continuar?"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmEliminarPlanAccionSi" runat="server" Text="Si" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmEliminarPlanAccionSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmEliminarPlanAccionNo" runat="server" Text="No" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmEliminarPlanAccionNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Eliminar Plan Acción !-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
