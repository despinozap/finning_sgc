<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteHallazgos.aspx.cs" Inherits="NCCSAN.Reportes.ReporteHallazgos" %>

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
    <script type="text/javascript" language="javascript">

        $(document).ready(function () {
            setDatepickerStyle();
        });


        function setDatepickerStyle() {

            $("#<%= txtFecha.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'mm-dd-yy',
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

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

            function EndRequestHandler(sender, args) {
                $("#<%= txtFecha.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'mm-dd-yy',
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

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Reportes Hallazgos</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Label ID="Label1" runat="server" Text="W/O"></asp:Label>
                            <asp:TextBox ID="txtWO" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Label ID="Label3" runat="server" Text="Fecha"></asp:Label>
                            <asp:TextBox ID="txtFecha" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Button ID="btDescargar" runat="server" Text="Descargar" OnClick="btDescargar_Click" class="submitPanelButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:SqlDataSource ID="SDSHallazgos" runat="server" 
        ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>" SelectCommand="
SELECT
		e.codigo,
		e.work_order,
		e.fecha_ingreso,
		e.nombre_area,
		e.nombre_subarea,
		e.nombre_tipoequipo,
		e.modelo_equipo,
		e.serie_equipo,
		e.nombre_clasificacion,
		e.nombre_subclasificacion,
		e.nombre_sistema,
		e.nombre_subsistema,
		e.nombre_componente,
		e.serie_componente,
		e.agente_corrector,
		e.probabilidad,
		e.consecuencia,
		e.irc,
		e.criticidad,
		e.detalle,
		a.contenido
FROM
		Evento e,
		EventoArchivo ea,
		Archivo a
WHERE
		(e.id_centro = @id_centro) AND
		(UPPER(e.work_order) = UPPER(@work_order)) AND
		(CONVERT(DATE, e.fecha_ingreso) = (CONVERT(DATE, @fecha_ingreso))) AND
		(e.irc &lt; 10) AND
		(e.nombre_fuente &lt;&gt; 'Reclamo de Cliente') AND
		(ea.codigo_evento = e.codigo) AND
		(a.id = ea.id_archivo) AND
		(
			(UPPER(nombre) LIKE '%.JPEG')
		)
UNION
SELECT
		e.codigo,
		e.work_order,
		e.fecha_ingreso,
		e.nombre_area,
		e.nombre_subarea,
		e.nombre_tipoequipo,
		e.modelo_equipo,
		e.serie_equipo,
		e.nombre_clasificacion,
		e.nombre_subclasificacion,
		e.nombre_sistema,
		e.nombre_subsistema,
		e.nombre_componente,
		e.serie_componente,
		e.agente_corrector,
		e.probabilidad,
		e.consecuencia,
		e.irc,
		e.criticidad,
		e.detalle,
		NULL
FROM
		Evento e
WHERE
		(e.id_centro = @id_centro) AND
		(UPPER(e.work_order) = UPPER(@work_order)) AND
		(CONVERT(DATE, e.fecha_ingreso) = (CONVERT(DATE, @fecha_ingreso))) AND
		(e.irc &lt; 10) AND
		(e.nombre_fuente &lt;&gt; 'Reclamo de Cliente') AND
		(e.codigo NOT IN (
				SELECT 
					ea.codigo_evento 
				FROM 
					Evento e,
					EventoArchivo ea,
					Archivo a
				WHERE
					(e.id_centro=@id_centro) AND
					(UPPER(e.work_order) = UPPER(@work_order)) AND
					(CONVERT(DATE, e.fecha_ingreso) = (CONVERT(DATE, @fecha_ingreso))) AND
					(e.irc &lt; 10) AND
					(e.nombre_fuente &lt;&gt; 'Reclamo de Cliente') AND
					(ea.codigo_evento=e.codigo) AND
					(a.id = ea.id_archivo) AND
					(
						(UPPER(nombre) LIKE '%.JPEG')
					)
				)
		)">
                <SelectParameters>
                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                    <asp:ControlParameter ControlID="txtWO" Name="work_order" PropertyName="Text" />
                    <asp:ControlParameter ControlID="txtFecha" Name="fecha_ingreso" 
                        PropertyName="Text" />
                </SelectParameters>
            </asp:SqlDataSource>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btDescargar" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
