<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListarReportesPlanesAccion.aspx.cs" Inherits="NCCSAN.Reportes.ListarNoConformidades" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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
            Reportes Plan de Acción</h1>
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
                            DataSourceID="SDSPlanesAccion" HorizontalAlign="Center" Width="100%" AllowPaging="True"
                            PageSize="20" OnRowCommand="gvEventos_RowCommand" ShowHeaderWhenEmpty="True">
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
                                <asp:BoundField DataField="codigo" HeaderText="Código" ReadOnly="True" SortExpression="codigo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="work_order" HeaderText="W/O" SortExpression="work_order">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha_ingreso" HeaderText="Fecha ingreso" SortExpression="fecha_ingreso">
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
                                <asp:BoundField DataField="fecha_cierre" HeaderText="Fecha de Cierre" SortExpression="fecha_cierre">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:ButtonField ButtonType="Image" CommandName="ExportarPDF" HeaderText="Exportar a PDF"
                                    ImageUrl="~/Images/Icon/export_pdf.png" Text="Exportar a PDF">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Image" CommandName="ExportarExcel" HeaderText="Exportar a Excel"
                                    ImageUrl="~/Images/Icon/export_excel.png" Text="Exportar a Excel">
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
                        <asp:SqlDataSource ID="SDSPlanesAccion" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            SelectCommand="SELECT e.codigo, e.work_order, CONVERT(VARCHAR(10), e.fecha_ingreso, 105)  AS fecha_ingreso, e.nombre_cliente, e.nombre_fuente, e.irc, e.nombre_componente, CASE WHEN (pa.fecha_cierre IS NOT NULL) THEN CONVERT(VARCHAR(10), pa.fecha_cierre, 105) ELSE '--' END AS fecha_cierre FROM Evento e, PlanAccion pa WHERE (e.id_centro=@id_centro) AND (pa.codigo_evento=e.codigo) AND (e.nombre_cliente LIKE '%' + @nombre_cliente + '%') AND (e.nombre_area LIKE '%' + @nombre_area + '%') AND (CONVERT(VARCHAR(10), e.fecha_ingreso, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%')) ORDER BY e.fecha_ingreso DESC">
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
            </table>
            <asp:HiddenField ID="hfCodigoEvento" runat="server"/>
            <asp:SqlDataSource ID="SDSEvento" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                SelectCommand="SELECT
		e.codigo,
		e.work_order,
		e.fecha AS fecha_deteccion,
		e.fecha_ingreso,
		e.nombre_cliente,
		e.nombre_fuente,
		e.nombre_area,
		e.detalle
FROM
		Evento e
WHERE
		(e.codigo=@codigo_evento)">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hfCodigoEvento" DefaultValue="" Name="codigo_evento"
                        PropertyName="Value" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SDSEvaluacion" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                SelectCommand="SELECT
		pre.nombre AS nombre_responsable_evento,
		pri.nombre AS nombre_responsable_investigacion,
		preva.nombre AS nombre_responsable_evaluacion,
		ci.tipo AS tipo_causainmediata,
		eva.nombre_causainmediata,
		eva.nombre_subcausainmediata,
		cb.tipo AS tipo_causabasica,
		eva.nombre_causabasica,
		eva.nombre_subcausabasica
FROM
		EventoPersona ep,
		Evaluacion eva,
		CausaInmediata ci,
		CausaBasica cb,
		Investigacion i,
		Persona pre,
		Persona pri,
		Persona preva
WHERE
		(i.codigo_evento=@codigo_evento) AND
		(eva.codigo_evento=i.codigo_evento)
		AND (ci.nombre=eva.nombre_causainmediata)
		AND (cb.nombre=eva.nombre_causabasica)
		AND (pre.rut=ep.rut_persona)
		AND (ep.codigo_evento=eva.codigo_evento)
		AND (ep.tipo=@tipo)
		AND (pri.rut=i.rut_responsable)
		AND (preva.rut=eva.rut_responsable)">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hfCodigoEvento" Name="codigo_evento" 
                        PropertyName="Value" />
                    <asp:Parameter DefaultValue="Responsable" Name="tipo" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SDSPlanAccion" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                SelectCommand="SELECT
		pa.detalle_correccion,
		pa.fecha_correccion,
		CONVERT(VARCHAR(4), (CAST(pa.progreso AS VARCHAR) + CAST('%' AS VARCHAR))) AS progreso,
		(
			CASE 
					WHEN (pa.fecha_cierre IS NULL)
					THEN 'El Plan de Acción está abierto'
					
					WHEN (pa.fecha_cierre IS NOT NULL)
					THEN CONVERT(VARCHAR(10), pa.fecha_cierre, 105)
			END
		) AS fecha_cierre
FROM
		PlanAccion pa
WHERE
		(pa.codigo_evento=@codigo_evento)">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hfCodigoEvento" Name="codigo_evento" PropertyName="Value" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SDSAccionCorrectiva" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                SelectCommand="SELECT
		ac.descripcion AS descripcion_accion_correctiva,
		ac.fecha_limite,
		ac.fecha_realizado,
		(
			CASE
				WHEN (ac.fecha_realizado IS NOT NULL)
				THEN 'Realizado'
				ELSE
					CASE
						WHEN (ac.fecha_limite &lt; SYSDATETIME())
						THEN 'Vencido'
						ELSE 'Pendiente'
					END
			END
		) AS estado,
		prac.nombre
FROM
		AccionCorrectiva ac,
		Persona prac
WHERE
		(prac.rut=ac.rut_responsable)
		AND (ac.codigo_planaccion=@codigo_evento)
ORDER BY
		(ac.fecha_limite) ASC">
                <SelectParameters>
                    <asp:ControlParameter ControlID="hfCodigoEvento" Name="codigo_evento" PropertyName="Value" />
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:SqlDataSource ID="SDSVerificacion" runat="server" 
        ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" SelectCommand="SELECT
		v.efectivo,
		CONVERT(VARCHAR(10), v.fecha, 105) AS fecha_verificacion,
		v.observacion,
		prv.nombre AS nombre_responsable_verificacion
FROM
		Verificacion v,
		Persona prv
WHERE
		(v.codigo_planaccion=@codigo_evento)
		AND (prv.rut=v.rut_responsable)">
        <SelectParameters>
            <asp:ControlParameter ControlID="hfCodigoEvento" Name="codigo_evento" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="gvEventos" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
