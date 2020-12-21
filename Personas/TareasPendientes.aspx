<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TareasPendientes.aspx.cs" Inherits="NCCSAN.Personas.TareasPendientes" %>

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
            Tareas pendientes</h1>
    </div>
    <br />
    <br />
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnTareasAsignadas" runat="server" Visible="False">
                <table class="tableControls" align="center" width="60%">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Tareas que me asignaron</h1>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView ID="gvTareasAsignadas" runat="server" AutoGenerateColumns="False"
                                HorizontalAlign="Center" OnRowCommand="gvTareasAsignadas_RowCommand" 
                                Width="100%">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Image ID="imgEstadoIcono" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código Evento">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbCodigoEvento" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbTarea" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha límite">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbFechaLimite" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Detalle">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ibDetalle" runat="server" ImageUrl="~/Images/Icon/search.png" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Registrar ejecución">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ibRegistrarEjecucion" runat="server" ImageUrl="~/Images/Icon/action.png" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <RowStyle CssClass="rowData" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
            <asp:Panel ID="pnAccionesInmediatas" runat="server" Visible="False">
                <table class="tableControls" align="center" width="80%">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Inspecciones</h1>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView ID="gvAccionesInmediatasPendientes" runat="server" 
                                AutoGenerateColumns="False" HorizontalAlign="Center" 
                                OnRowCommand="gvAccionesInmediatasPendientes_RowCommand" Width="100%">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Image ID="imgEstadoIcono" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Código Evento">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbCodigoEvento" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbTarea" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha límite">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label ID="lbFechaLimite" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Detalle">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ibDetalle" runat="server" ImageUrl="~/Images/Icon/search.png" CommandName="DetalleEvento" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Registrar">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ibRegistrar" runat="server" ImageUrl="~/Images/Icon/action.png" CommandName="RegistrarAccionInmediata" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <RowStyle CssClass="rowData" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnTareasOk" runat="server" Visible="False">
                <table align="center">
                    <tr>
                        <td>
                            <asp:Image ID="imgTareasOk" runat="server" Height="100px" ImageUrl="~/Images/tareas_ok.png" />
                        </td>
                        <td>
                            <asp:Label ID="lbTareasOk" runat="server" Text="Usted no tiene tareas pendientes"
                                Font-Bold="True" Font-Size="20px"></asp:Label>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!-- Panel Clave por Defecto -->
            <asp:HiddenField ID="hfDefaultPassword" runat="server" />
            <asp:ModalPopupExtender ID="hfDefaultPassword_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="pnDefaultPassword"
                TargetControlID="hfDefaultPassword">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upDefaultPassword" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnDefaultPassword" runat="server" HorizontalAlign="Center" CssClass="Popup_frontground"
                        Width="100%">
                        <asp:Label ID="Label24" runat="server" Text="Estás utilizando la clave por defecto para los usuarios del SGC. Por razones de seguridad es muy importante que la cambies lo antes posible."></asp:Label>
                        <br />
                        <br />
                        <table align="center" width="100%">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btDefaultPasswordCambiarClave" runat="server" Text="Cambiar clave"
                                        Width="120px" CssClass="submitButton" OnClick="btDefaultPasswordCambiarClave_Click" />
                                    <asp:Button ID="btDefaultPasswordCerrar" runat="server" Text="Cerrar" Width="70px"
                                        CssClass="submitButton" OnClick="btDefaultPasswordCerrar_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Clave por Defecto !-->
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Detalle Evento -->
    <asp:HiddenField ID="hfDetalleEvento" runat="server" />
    <asp:ModalPopupExtender ID="hfDetalleEvento_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upDetalleEvento" TargetControlID="hfDetalleEvento">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upDetalleEvento" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnDetalleEvento" runat="server" HorizontalAlign="Center" CssClass="Popup_frontground"
                ScrollBars="Auto" Height="500">
                <table class="tableControls" align="center">
                    <tr>
                        <td colspan="5" class="tdTitle">
                            <h1>
                                Detalle del Evento</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label1" runat="server" Text="Código" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label2" runat="server" Text="W/O" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label3" runat="server" Text="Fecha" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label4" runat="server" Text="Cliente" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label5" runat="server" Text="Equipo" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoCodigo" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoWO" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoFecha" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoCliente" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoEquipo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label6" runat="server" Text="Fuente" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label7" runat="server" Text="Centro" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label8" runat="server" Text="Área" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label9" runat="server" Text="Sub-área" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label10" runat="server" Text="Componente" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoFuente" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoCentro" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoArea" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoSubarea" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoComponente" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label11" runat="server" Text="Parte" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label12" runat="server" Text="Serie" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label13" runat="server" Text="Ítem" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label14" runat="server" Text="Clasificación" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label15" runat="server" Text="Criticidad" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoParte" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoSerie" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoItem" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoClasificacion" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoCriticidad" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label16" runat="server" Text="Probabilidad" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label17" runat="server" Text="Consecuencia" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdFirst" align="center">
                            <asp:Label ID="Label18" runat="server" Text="IRC" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="2" class="tdFirst" align="center">
                            <asp:Label ID="Label27" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoProbabilidad" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoConsecuencia" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="lbDetalleEventoIRC" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="Label26" runat="server"></asp:Label>
                        </td>
                        <td class="tdLast" align="center">
                            <asp:Label ID="Label32" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label19" runat="server" Text="Detalle" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4" class="tdUnique" align="left">
                            <asp:Label ID="lbDetalleEventoDetalle" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="left">
                            <asp:Label ID="Label22" runat="server" Text="Creado por" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4" class="tdUnique" align="left">
                            <asp:LinkButton ID="lbtDetalleEventoDetalleCreador" runat="server" OnClick="lbtDetalleEventoDetalleCreador_Click"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table class="tableControls" align="center">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Archivos adjuntos</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:GridView ID="gvArchivosEvento" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center"
                                Width="863px" OnRowCommand="gvArchivosEvento_RowCommand" ShowHeaderWhenEmpty="True">
                                <AlternatingRowStyle BackColor="#E4E4E4" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Nombre">
                                        <ItemTemplate>
                                            <asp:Label ID="lbNombre" runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo">
                                        <ItemTemplate>
                                            <asp:Label ID="lbTipo" runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tamaño">
                                        <ItemTemplate>
                                            <asp:Label ID="lbSize" runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" CommandName="DescargarArchivo" HeaderText="Descargar"
                                        ImageUrl="~/Images/Icon/download.png" Text="Descargar" AccessibleHeaderText="btBle">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:ButtonField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <EmptyDataTemplate>
                                    <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                    <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                        Font-Size="14px"></asp:Label>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td>
                            <asp:Button ID="btDetalleEventoCerrar" runat="server" Text="Cerrar" Width="70px"
                                CssClass="submitButton" OnClick="btDetalleEventoCerrar_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="gvArchivosEvento" />
        </Triggers>
    </asp:UpdatePanel>
    <!-- Panel Detalle Evento !-->
    <!-- Panel Detalle AccionCorrectiva -->
    <asp:HiddenField ID="hfDetalleAccionCorrectiva" runat="server" />
    <asp:ModalPopupExtender ID="hfDetalleAccionCorrectiva_ModalPopupExtender" runat="server"
        BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="upDetalleAccionCorrectiva"
        TargetControlID="hfDetalleAccionCorrectiva">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upDetalleAccionCorrectiva" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnDetalleAccionCorrectiva" runat="server" HorizontalAlign="Center"
                CssClass="Popup_frontground" ScrollBars="Auto" Height="500">
                <table class="tableControls" align="center">
                    <tr>
                        <td colspan="5" class="tdTitle">
                            <h1>
                                Detalle de la Acción Correctiva</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label20" runat="server" Text="Plan de Acción" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbDetalleAccionCorrectivaCodigoPlanAccion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label21" runat="server" Text="ID Acción Correctiva" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbDetalleAccionCorrectivaIDAccionCorrectiva" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label28" runat="server" Text="Descripción" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4" class="tdUnique" align="left">
                            <asp:Label ID="lbDetalleAccionCorrectivaDescripcion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label23" runat="server" Text="Fecha límite" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="4" class="tdUnique" align="left">
                            <asp:Label ID="lbDetalleAccionCorrectivaFechaLimite" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td>
                            <asp:Button ID="btDetalleAccionCorrectivaCerrar" runat="server" Text="Cerrar" Width="70px"
                                CssClass="submitButton" OnClick="btDetalleAccionCorrectivaCerrar_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Detalle AccionCorrectiva !-->
</asp:Content>
