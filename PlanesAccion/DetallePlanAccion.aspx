﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DetallePlanAccion.aspx.cs" Inherits="NCCSAN.PlanesAccion.DetallePlanAccion" %>

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
            Detalle Plan Acción</h1>
    </div>
    <asp:HiddenField ID="hfCodigoEvento" runat="server" />
    <asp:HiddenField ID="hfPreviousPage" runat="server" />
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <br />
            <table align="center">
                <tr>
                    <td>
                        <asp:ImageButton ID="ibVerDetalleEvento" runat="server" CssClass="submitPanelButton"
                            ImageUrl="~/Images/Button/view_evento.png" OnClick="ibVerDetalleEvento_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel ID="pnPlanAccion" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Panel Detalle</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label22" runat="server" Text="Corrección inmediata" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbDetalleCorreccion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label23" runat="server" Text="Fecha corrección" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbFechaCorreccion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Progreso" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <table width="200px">
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
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center" class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Panel Plan de Acción</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdFirst">
                            <asp:GridView ID="gvAccionesCorrectivas" runat="server" AutoGenerateColumns="False"
                                HorizontalAlign="Center" OnRowCommand="gvAccionesCorrectivas_RowCommand" ShowHeaderWhenEmpty="True">
                                <AlternatingRowStyle BackColor="#E4E4E4" />
                                <Columns>
                                    <asp:TemplateField HeaderText="">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Image ID="imgEstadoIcono" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Descripción">
                                        <ItemTemplate>
                                            <asp:Label ID="lbDescripcion" runat="server" Text=""></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha límite">
                                        <ItemTemplate>
                                            <asp:Label ID="lbFechaLimite" runat="server" Text=""></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fecha ejecución">
                                        <ItemTemplate>
                                            <asp:Label ID="lbFechaRealizado" runat="server" Text=""></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Responsable">
                                        <ItemTemplate>
                                            <asp:Label ID="lbNombreResponsable" runat="server" Text=""></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Registrar ejecución" Text="Registrar ejecución"
                                        ImageUrl="~/Images/Icon/action.png" CommandName="IngresarAccionCorrectiva">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" CommandName="DetalleAccionCorrectiva" HeaderText="Detalle"
                                        ImageUrl="~/Images/Icon/search.png" Text="Detalle">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                </Columns>
                                <HeaderStyle BackColor="#333333" ForeColor="White" />
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
                            <asp:ImageButton ID="ibVolver" runat="server" ImageUrl="~/Images/Button/back.png"
                                ImageAlign="AbsMiddle" OnClick="ibVolver_Click" CssClass="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
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
                                    <asp:Label ID="Label52" runat="server" Text="Código" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label53" runat="server" Text="W/O" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label54" runat="server" Text="Cliente" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label55" runat="server" Text="Fuente" Font-Bold="True"></asp:Label>
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
                                    <asp:Label ID="lbDetalleEventoNombreCliente" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreFuente" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label56" runat="server" Text="Fecha identificación" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label57" runat="server" Text="Tipo" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label58" runat="server" Text="Modelo" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label59" runat="server" Text="Serie equipo" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoFecha" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoTipoEquipo" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoModeloEquipo" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoSerieEquipo" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label60" runat="server" Text="Sistema" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label61" runat="server" Text="Sub-sistema" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label62" runat="server" Text="Componente" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label63" runat="server" Text="Serie componente" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSistema" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSubsistema" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreComponente" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoSerieComponente" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label64" runat="server" Text="Parte o Pieza" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label65" runat="server" Text="Número parte" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label66" runat="server" Text="Horas" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label67" runat="server" Text="" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoParte" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNumeroParte" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoHoras" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEvento" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label68" runat="server" Text="Área" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label69" runat="server" Text="Sub-área" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label70" runat="server" Text="Clasificación" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label71" runat="server" Text="Sub-clasificación" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreArea" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSubarea" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreClasificacion" runat="server"></asp:Label>
                                </td>
                                <td class="tdLast" align="center">
                                    <asp:Label ID="lbDetalleEventoNombreSubclasificacion" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label72" runat="server" Text="Probabilidad" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label73" runat="server" Text="Consecuencia" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label74" runat="server" Text="IRC" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdFirst" align="center">
                                    <asp:Label ID="Label75" runat="server" Text="Criticidad" Font-Bold="True"></asp:Label>
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
                                    <asp:Label ID="lbDetalleEventoCriticidad" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label76" runat="server" Text="Detalle" Font-Bold="True"></asp:Label>
                                </td>
                                <td colspan="4" class="tdUnique" align="left">
                                    <asp:Label ID="lbDetalleEventoDetalle" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique" align="left">
                                    <asp:Label ID="Label28" runat="server" Text="Creado por" Font-Bold="True"></asp:Label>
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
            </asp:UpdatePanel>
            <!-- Panel Detalle Evento !-->
            <!-- Detalle Acción Correctiva -->
            <asp:HiddenField ID="hfDetalleAccionCorrectiva" runat="server" />
            <asp:ModalPopupExtender ID="hfDetalleAccionCorrectiva_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="upDetalleAccionCorrectiva"
                TargetControlID="hfDetalleAccionCorrectiva">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upDetalleAccionCorrectiva" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnDetalleAccionCorrectiva" runat="server" CssClass="Popup_frontground"
                        ScrollBars="Auto" Height="500">
                        <table align="center" class="tableControls">
                            <tr>
                                <td colspan="2" class="tdTitle">
                                    <h1>
                                        Acción Correctiva</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label2" runat="server" Text="Descripción" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbDescripcionAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label3" runat="server" Text="Responsable" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbNombreResponsableAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label4" runat="server" Text="Fecha límite" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbFechaLimiteAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label6" runat="server" Text="Fecha de ejecución" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbFechaRealizadoAccionCorrectiva" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique">
                                    <asp:Label ID="Label7" runat="server" Text="Observación" Font-Bold="True"></asp:Label>
                                </td>
                                <td class="tdUnique">
                                    <asp:Label ID="lbObservacionAccionCorrectiva" runat="server"></asp:Label>
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
                                    <asp:GridView ID="gvArchivosAccionCorrectiva" runat="server" AutoGenerateColumns="False"
                                        HorizontalAlign="Center" Width="863px" OnRowCommand="gvArchivosAccionCorrectiva_RowCommand"
                                        ShowHeaderWhenEmpty="True">
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
                        <table class="tableControls" align="center">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btDetalleAccionCorrectivaCerrar" runat="server" Text="Cerrar" CssClass="submitButton"
                                        OnClick="btDetalleAccionCorrectivaCerrar_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="gvArchivosEvento" />
                    <asp:PostBackTrigger ControlID="gvArchivosAccionCorrectiva" />
                </Triggers>
            </asp:UpdatePanel>
            <!-- Detalle Acción Correctiva !-->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
