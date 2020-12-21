<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListarInvestigacionesEnCurso.aspx.cs" Inherits="NCCSAN.Investigaciones.ListarInvestigacionesEnCurso" %>

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

        function pageLoad() {

            $("#<%= txtFechaInvestigacionRealizada.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'dd-mm-yy',
                        // Primer dia de la semana El lunes
                        firstDay: 1,
                        // Dias Largo en castellano
                        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                        // Dias cortos en castellano
                        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                        // Nombres largos de los meses en castellano
                        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                        // Nombres de los meses en formato corto 
                        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec"],
                        firstDay: 1
                    }
                );

            loadDatepickerStyle();
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Lista de Investigaciones en Curso</h1>
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
                            DataSourceID="SDSInvestigacionesEnCurso" HorizontalAlign="Center" Width="100%"
                            OnRowCommand="gvEventos_RowCommand" AllowPaging="True" OnDataBound="gvEventos_DataBound"
                            PageSize="20" ShowHeaderWhenEmpty="True">
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
                                <asp:BoundField DataField="fecha_inicio" HeaderText="Inicio de Investigación" ReadOnly="True"
                                    SortExpression="fecha_inicio">
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
                                <asp:BoundField DataField="dias" HeaderText="Días en curso" SortExpression="dias">
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre" HeaderText="Responsable" SortExpression="nombre" />
                                <asp:ButtonField ButtonType="Image" HeaderText="Cerrar" ImageUrl="~/Images/Icon/shutdown.png"
                                    Text="Cerrar" CommandName="CerrarInvestigacion">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Image" CommandName="EliminarInicioInvestigacion" HeaderText="Retroceder"
                                    ImageUrl="~/Images/Icon/undo.png" Text="Retroceder">
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
                        <asp:SqlDataSource ID="SDSInvestigacionesEnCurso" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            SelectCommand="SELECT e.codigo, e.work_order, CONVERT(VARCHAR(10), i.fecha_inicio, 105) AS fecha_inicio, e.nombre_cliente, e.nombre_fuente, e.irc, e.nombre_componente, DATEDIFF(DAY, i.fecha_inicio, SYSDATETIME()) AS dias, p.nombre FROM Evento e, Investigacion i, Persona p WHERE (e.id_centro=@id_centro) AND (e.estado='Investigación en curso') AND (i.codigo_evento=e.codigo) AND (i.rut_responsable=p.rut) AND (e.nombre_cliente LIKE '%' + @nombre_cliente + '%') AND (e.nombre_area LIKE '%' + @nombre_area + '%') AND (CONVERT(VARCHAR(10), e.fecha, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%'))">
                            <SelectParameters>
                                <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                <asp:ControlParameter ControlID="ddlCliente" DefaultValue="" Name="nombre_cliente"
                                    PropertyName="SelectedValue" ConvertEmptyStringToNull="False" Type="String" />
                                <asp:ControlParameter ControlID="ddlArea" ConvertEmptyStringToNull="False" 
                                    Name="nombre_area" PropertyName="SelectedValue" />
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
            <asp:HiddenField ID="hfCalendar" runat="server" />
            <asp:ModalPopupExtender ID="hfCalendar_ModalPopupExtender" runat="server" DynamicServicePath=""
                Enabled="True" TargetControlID="hfCalendar" PopupControlID="pnCalendar" BackgroundCssClass="Popup_background">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upCalendar" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnCalendar" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <table class="tableControls" align="center">
                            <tr>
                                <td class="tdTitle">
                                    <h1>
                                        Cierre de la Investigación
                                    </h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdUnique" align="center">
                                    <asp:Label ID="Label5" runat="server" Text="Fecha"></asp:Label>
                                    &nbsp;<asp:Label ID="Label6" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                                    &nbsp;&nbsp;<asp:TextBox ID="txtFechaInvestigacionRealizada" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                        <table align="center" class="tableControls">
                            <tr>
                                <td class="tdTitle">
                                    <h1>
                                        Panel de Archivos</h1>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdFirst">
                                    <asp:GridView ID="gvArchivosEvaluacion" runat="server" AutoGenerateColumns="False"
                                        Width="963px" HorizontalAlign="Center" OnRowCommand="gvArchivosEvaluacion_RowCommand"
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
                                            <asp:ButtonField ButtonType="Image" CommandName="RemoveArchivo" HeaderText="Eliminar"
                                                ImageUrl="~/Images/Icon/remove_file.png">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:ButtonField>
                                        </Columns>
                                        <HeaderStyle BackColor="#666666" ForeColor="White" />
                                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <EmptyDataTemplate>
                                            <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                            <asp:Label ID="lbEmptyList" runat="server" Text="  No se han cargado archivos" Font-Size="14px"></asp:Label>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:FileUpload ID="fuArchivo" runat="server" Width="500" multiple="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdLast" align="center">
                                    <asp:ImageButton ID="ibAddArchivo" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Button/attachment.png"
                                        CssClass="submitAttachFile" OnClick="ibAddArchivo_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btRegistrarInvestigacionRealizada" runat="server" Text="Registrar"
                                        OnClick="btRegistrarInvestigacionRealizada_Click" CssClass="submitButton" />
                                    <asp:Button ID="btCancelarInvestigacionRealizada" runat="server" Text="Cancelar"
                                        OnClick="btCancelarInvestigacionRealizada_Click" CssClass="submitButton" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Eliminar Investigación -->
            <asp:HiddenField ID="hfEliminarInicioInvestigacion" runat="server" />
            <asp:ModalPopupExtender ID="hfEliminarInicioInvestigacion_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="pnEliminarInicioInvestigacion"
                TargetControlID="hfEliminarInicioInvestigacion">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upEliminarInicioInvestigacion" runat="server" style="width: 100%"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnEliminarInicioInvestigacion" runat="server" CssClass="Popup_frontground"
                        Width="100%">
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Label ID="lbMessageEliminarInicioInvestigacion" runat="server" CssClass="messageBoxMessage"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btEliminarInicioInvestigacion" runat="server" Text="Aceptar" CssClass="submitButton"
                                        OnClick="btEliminarInicioInvestigacion_Click" />
                                    <asp:Button ID="btCancelarEliminarInicioInvestigacion" runat="server" Text="Cancelar"
                                        CssClass="submitButton" OnClick="btCancelarEliminarInicioInvestigacion_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Eliminar Investigación !-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
            <asp:PostBackTrigger ControlID="ibAddArchivo" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
