using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;

namespace NCCSAN.Personas
{
    public partial class TareasPendientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                {//Acceso del Usuario a la Página
                    if (Session["usuario"] == null)
                    {
                        string msg = "Error al recuperar tu información de Usuario";
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }

                    Usuario u = (Usuario)Session["usuario"];
                    string pagename = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);
                    string prefix = ResolveUrl("~");
                    if (LogicController.isPageAllowed(u.getNombreRol(), pagename, prefix) < 1)
                    {
                        Response.Redirect("~/AccesoDenegado.aspx", true);
                    }
                }

                if (Session["DefaultPassword"] != null)
                {
                    Session.Remove("DefaultPassword");
                    hfDefaultPassword_ModalPopupExtender.Show();
                }

                loadTaskTable();
            }
        }


        private void loadTaskTable()
        {
            if (Session["usuario"] == null)
            {
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];
            if (usuario == null)
            {
                return;
            }

            List<Investigacion> listInvestigaciones = LogicController.getListInvestigacionesPersona(usuario.getRutPersona());
            if (listInvestigaciones == null)
            {
                return;
            }

            List<AccionCorrectiva> listAccionesCorrectivas = LogicController.getListAccionesCorrectivasPersona(usuario.getRutPersona());
            if (listAccionesCorrectivas == null)
            {
                return;
            }

            List<Evento> listAccionesInmediatasPendientesInspector = null;
            if (usuario.getNombreRol().Equals("Inspector"))
            {
                string id_centro = usuario.getIDCentro();
                string rut_persona = usuario.getRutPersona();

                listAccionesInmediatasPendientesInspector = LogicController.getListEventosAccionInmediataPendienteInspector(id_centro, rut_persona);

                if (listAccionesInmediatasPendientesInspector == null)
                {
                    return;
                }
            }


            updateGVTareasAsignadas(listInvestigaciones.Count + listAccionesCorrectivas.Count);


            ConfigEmailSender ces; 
            ces = LogicController.getConfigEmailSender("Investigación en curso", "Usuario", usuario.getIDCentro());
            if (ces == null)
            {
                return;
            }


            int index_tareas_asignadas = 0;
            Image imgEstadoIcono;
            int dias_en_curso;
            ImageButton ibDetalle;
            ImageButton ibRegistrarEjecucion;
            DateTime fecha_limite;
            TimeSpan ts;
            foreach (Investigacion investigacion in listInvestigaciones)
            {
                ((Label)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("lbCodigoEvento")).Text = investigacion.getCodigoEvento();
                ((Label)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("lbTarea")).Text = "Investigación";

                fecha_limite = Convert.ToDateTime(investigacion.getFechaInicio()).AddDays(ces.getDiasLimite());
                ((Label)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("lbFechaLimite")).Text = fecha_limite.ToShortDateString();
                ibDetalle = (ImageButton)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("ibDetalle");
                ibDetalle.CommandName = "DetalleInvestigacion";
                ibDetalle.CommandArgument = investigacion.getCodigoEvento();
                ibRegistrarEjecucion = (ImageButton)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("ibRegistrarEjecucion");
                ibRegistrarEjecucion.Visible = false;

                //Icono
                {
                    ts = Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaInicio());
                    dias_en_curso = ts.Days;

                    imgEstadoIcono = (Image)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("imgEstadoIcono");
                    if (dias_en_curso > ces.getDiasLimite())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                        imgEstadoIcono.ToolTip = "Investigación vencida";
                    }
                    else if (dias_en_curso == ces.getDiasLimite())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                        imgEstadoIcono.ToolTip = "La Investigación vence hoy";
                    }
                    else if (dias_en_curso >= ces.getDiasAlerta())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";

                        if ((ces.getDiasLimite() - dias_en_curso) == 1)
                        {
                            imgEstadoIcono.ToolTip = "La Investigación vence en 1 día";
                        }
                        else
                        {
                            imgEstadoIcono.ToolTip = "La Investigación vence en " + Convert.ToString(ces.getDiasLimite() - dias_en_curso) + " días";
                        }
                    }
                }


                index_tareas_asignadas++;
            }


            ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Usuario", usuario.getIDCentro());
            if (ces == null)
            {
                return;
            }


            string codigo_planaccion;
            int dias_diferencia;
            foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivas)
            {
                codigo_planaccion = LogicController.getCodigoPlanAccionAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                if (codigo_planaccion == null)
                {
                    codigo_planaccion = "--";
                }

                ((Label)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("lbCodigoEvento")).Text = codigo_planaccion;
                ((Label)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("lbTarea")).Text = "Acción Correctiva";
                ((Label)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("lbFechaLimite")).Text = accion_correctiva.getFechaLimite();
                ibDetalle = (ImageButton)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("ibDetalle");
                ibDetalle.CommandName = "DetalleAccionCorrectiva";
                ibDetalle.CommandArgument = accion_correctiva.getIdAccionCorrectiva();
                ibRegistrarEjecucion = (ImageButton)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("ibRegistrarEjecucion");
                ibRegistrarEjecucion.Visible = true;
                ibRegistrarEjecucion.CommandName = "IngresarAccionCorrectiva";
                ibRegistrarEjecucion.CommandArgument = accion_correctiva.getIdAccionCorrectiva();

                //Icono
                {
                    ts = Convert.ToDateTime(accion_correctiva.getFechaLimite()) - Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    dias_diferencia = ts.Days;
                    imgEstadoIcono = (Image)gvTareasAsignadas.Rows[index_tareas_asignadas].FindControl("imgEstadoIcono");
                    if (dias_diferencia < 0)
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                        imgEstadoIcono.ToolTip = "Acción Correctiva vencida";
                    }
                    else if (dias_diferencia == 0)
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                        imgEstadoIcono.ToolTip = "La Acción Correctiva vence hoy";
                    }
                    else if (dias_diferencia <= ces.getDiasAlerta())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";

                        if (dias_diferencia == 1)
                        {
                            imgEstadoIcono.ToolTip = "La Acción Correctiva vence en 1 día";
                        }
                        else
                        {
                            imgEstadoIcono.ToolTip = "La Acción Correctiva vence en " + Convert.ToString(dias_diferencia) + " días";
                        }
                    }
                }

                index_tareas_asignadas++;
            }


            ces = LogicController.getConfigEmailSender("Acción inmediata pendiente", "Inspector", usuario.getIDCentro());
            if (ces == null)
            {
                return;
            }


            int index_acciones_inmediatas_pendientes = 0;
            if (listAccionesInmediatasPendientesInspector != null)
            {
                updateGVAccionesInmediatasPendientes(listAccionesInmediatasPendientesInspector.Count);

                ImageButton ibRegistrar;
                foreach (Evento evento in listAccionesInmediatasPendientesInspector)
                {
                    ((Label)gvAccionesInmediatasPendientes.Rows[index_acciones_inmediatas_pendientes].FindControl("lbCodigoEvento")).Text = evento.getCodigo();
                    ((Label)gvAccionesInmediatasPendientes.Rows[index_acciones_inmediatas_pendientes].FindControl("lbTarea")).Text = "Acción Inmediata";

                    fecha_limite = Convert.ToDateTime(evento.getFechaIngreso()).AddDays(ces.getDiasLimite());
                    ((Label)gvAccionesInmediatasPendientes.Rows[index_acciones_inmediatas_pendientes].FindControl("lbFechaLimite")).Text = fecha_limite.ToShortDateString();
                    ibDetalle = (ImageButton)gvAccionesInmediatasPendientes.Rows[index_acciones_inmediatas_pendientes].FindControl("ibDetalle");
                    ibDetalle.CommandArgument = evento.getCodigo();
                    ibRegistrar = (ImageButton)gvAccionesInmediatasPendientes.Rows[index_acciones_inmediatas_pendientes].FindControl("ibRegistrar");
                    ibRegistrar.CommandArgument = evento.getCodigo();

                    //Icono
                    {
                        ts = fecha_limite - Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        dias_diferencia = ts.Days;
                        imgEstadoIcono = (Image)gvAccionesInmediatasPendientes.Rows[index_acciones_inmediatas_pendientes].FindControl("imgEstadoIcono");
                        if (dias_diferencia < 0)
                        {
                            imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                            imgEstadoIcono.ToolTip = "Acción Inmediata vencida";
                        }
                        else if (dias_diferencia == 0)
                        {
                            imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                            imgEstadoIcono.ToolTip = "La Acción Inmediata vence hoy";
                        }
                        else if (dias_diferencia <= ces.getDiasLimite())
                        {
                            imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";

                            if (dias_diferencia == 1)
                            {
                                imgEstadoIcono.ToolTip = "La Acción Inmediata vence en 1 día";
                            }
                            else
                            {
                                imgEstadoIcono.ToolTip = "La Acción Inmediata vence en " + Convert.ToString(dias_diferencia) + " días";
                            }
                        }
                    }


                    index_acciones_inmediatas_pendientes++;
                }
            }


            if ((index_tareas_asignadas == 0) && (index_acciones_inmediatas_pendientes == 0))
            {
                pnTareasAsignadas.Visible = false;
                pnAccionesInmediatas.Visible = false;
                pnTareasOk.Visible = true;
            }
            else
            {
                pnTareasOk.Visible = false;

                if (index_tareas_asignadas > 0)
                {
                    pnTareasAsignadas.Visible = true;
                }

                if (index_acciones_inmediatas_pendientes > 0)
                {
                    pnAccionesInmediatas.Visible = true;
                }
            }
        }


        private bool loadDetalleEvento(string codigo_evento)
        {
            try
            {
                Evento evento = LogicController.getEvento(codigo_evento);
                if (evento == null)
                {
                    return false;
                }

                PersonaInfo creador = LogicController.getCreadorEvento(codigo_evento);
                if (creador == null)
                {
                    return false;
                }

                lbDetalleEventoCodigo.Text = evento.getCodigo();
                lbDetalleEventoWO.Text = evento.getWO();
                lbDetalleEventoFecha.Text = evento.getFecha().ToShortDateString();
                lbDetalleEventoCliente.Text = evento.getNombreCliente();
                lbDetalleEventoEquipo.Text = evento.getModeloEquipo();
                lbDetalleEventoFuente.Text = evento.getNombreFuente();
                lbDetalleEventoCentro.Text = evento.getIDCentro();
                lbDetalleEventoArea.Text = evento.getNombreArea();
                lbDetalleEventoSubarea.Text = evento.getNombreSubarea();
                lbDetalleEventoComponente.Text = evento.getNombreComponente();
                lbDetalleEventoParte.Text = evento.getParte();
                lbDetalleEventoSerie.Text = evento.getSerieComponente();
                lbDetalleEventoItem.Text = evento.getNombreClasificacion();
                lbDetalleEventoClasificacion.Text = evento.getNombreSubclasificacion();
                lbDetalleEventoCriticidad.Text = evento.getCriticidad();
                lbDetalleEventoProbabilidad.Text = evento.getProbabilidad();
                lbDetalleEventoConsecuencia.Text = evento.getConsecuencia();
                lbDetalleEventoIRC.Text = Convert.ToString(evento.getIRC());
                lbDetalleEventoDetalle.Text = evento.getDetalle();
                lbtDetalleEventoDetalleCreador.CommandName = "DetallePersona";

                lbtDetalleEventoDetalleCreador.CommandArgument = creador.getRut();
                lbtDetalleEventoDetalleCreador.Text = creador.getNombre();

                if (loadArchivosEvento(codigo_evento))
                {
                    return true;
                }
                else
                {
                    showMessageError("Error al cargar los archivos asociados al Evento");

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private bool loadDetalleAccionCorrectiva(string id_accion_correctiva)
        {
            try
            {
                AccionCorrectiva accion_correctiva = LogicController.getAccionCorrectiva(id_accion_correctiva);
                if (accion_correctiva == null)
                {
                    return false;
                }

                string codigo_plan_accion = LogicController.getCodigoPlanAccionAccionCorrectiva(id_accion_correctiva);
                if (codigo_plan_accion == null)
                {
                    return false;
                }

                lbDetalleAccionCorrectivaCodigoPlanAccion.Text = codigo_plan_accion;
                lbDetalleAccionCorrectivaIDAccionCorrectiva.Text = accion_correctiva.getIdAccionCorrectiva();
                lbDetalleAccionCorrectivaDescripcion.Text = accion_correctiva.getDescripcion();
                lbDetalleAccionCorrectivaFechaLimite.Text = accion_correctiva.getFechaLimite();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void setListIDArchivosEvento(List<string> listIDArchivosEvento)
        {
            Session["listIDArchivosEvento"] = listIDArchivosEvento;
        }


        private List<string> getListIDArchivosEvento()
        {
            if (Session["listIDArchivosEvento"] != null)
            {
                return (List<string>)Session["listIDArchivosEvento"];
            }
            else
            {
                return null;
            }
        }


        private bool loadArchivosEvento(string codigo_evento)
        {
            try
            {
                List<Archivo> listArchivosEvento = LogicController.getArchivosEvento(codigo_evento);
                if (listArchivosEvento == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosEvento = new List<string>();

                foreach (Archivo archivo in listArchivosEvento)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosEvento.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosEvento.DataSource = dt;
                gvArchivosEvento.DataBind();

                setListIDArchivosEvento(listIDArchivosEvento);

                for (int i = 0; i < listArchivosEvento.Count; i++)
                {
                    ((Label)gvArchivosEvento.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosEvento[i].getNombre();
                    ((Label)gvArchivosEvento.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosEvento[i].getTipoArchivo();
                    ((Label)gvArchivosEvento.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosEvento[i].getSize()) + " bytes";

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        protected void gvArchivosEvento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosEvento.Rows.Count))
                {
                    List<string> listIDArchivosEvento = getListIDArchivosEvento();
                    if (listIDArchivosEvento == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados al Evento");
                        return;
                    }

                    string id_archivo = listIDArchivosEvento[index];
                    Archivo archivo = LogicController.downloadFile(id_archivo);
                    if (archivo != null)
                    {
                        //Limpiamos la salida
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.Clear();

                        //Añadimos al encabezado la información de nuestro archivo
                        Response.AddHeader("Content-type", archivo.getTipoContenido());
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + archivo.getNombre());

                        //Finalmente escribimos los bytes en la respuesta de la página web
                        Response.BinaryWrite(archivo.getContenido());
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        showMessageError("Se ha producido un error al recuperar el archivo para descargar");
                    }
                }
            }
        }


        private void updateGVTareasAsignadas(int total_tasks)
        {
            DataTable dt = getDTTareasAsignadas();
            DataRow dr;

            for (int i = 0; i < total_tasks; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Image();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvTareasAsignadas.DataSource = dt;
            gvTareasAsignadas.DataBind();
        }


        private void updateGVAccionesInmediatasPendientes(int total_tasks)
        {
            DataTable dt = getDTTareasAsignadas();
            DataRow dr;

            for (int i = 0; i < total_tasks; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Image();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvAccionesInmediatasPendientes.DataSource = dt;
            gvAccionesInmediatasPendientes.DataBind();
        }


        private DataTable getDTTareasAsignadas()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Codigo");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Fecha límite");
            dt.Columns.Add("Detalle");
            dt.Columns.Add("Registrar ejecución");
            dt.AcceptChanges();

            return dt;
        }


        private DataTable getDTAccionesInmediatasPendientes()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Codigo");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Fecha límite");
            dt.Columns.Add("Detalle");
            dt.Columns.Add("Registrar");
            dt.AcceptChanges();

            return dt;
        }


        protected void gvTareasAsignadas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetalleInvestigacion"))
            {
                string codigo_evento = (string)e.CommandArgument;
                if (loadDetalleEvento(codigo_evento))
                {
                    upDetalleEvento.Update();
                    hfDetalleEvento_ModalPopupExtender.Show();
                }
                else
                {
                    showMessageError("Error al cargar los detalles del Evento");
                }
            }
            else if (e.CommandName.Equals("DetalleAccionCorrectiva"))
            {
                string id_accioncorrectiva = (string)e.CommandArgument;
                if (loadDetalleAccionCorrectiva(id_accioncorrectiva))
                {
                    upDetalleAccionCorrectiva.Update();
                    hfDetalleAccionCorrectiva_ModalPopupExtender.Show();
                }
                else
                {
                    showMessageError("Error al cargar los detalles de la Acción Correctiva");
                }

            }
            else if (e.CommandName.Equals("IngresarAccionCorrectiva"))
            {
                Session.Remove("CodigoAccionCorrectivaSeleccionada");

                string id_accion_correctiva = null;
                try
                {
                    id_accion_correctiva = (string)e.CommandArgument;
                }
                catch (Exception ex)
                {
                    showMessageError("Error al recuperar la información de la Acción Correctiva");
                    return;
                }

                if (id_accion_correctiva == null)
                {
                    showMessageError("Error al recuperar la información de la Acción Correctiva");
                    return;
                }

                AccionCorrectiva accion_correctiva = LogicController.getAccionCorrectiva(id_accion_correctiva);
                if (accion_correctiva == null)
                {
                    showMessageError("Error al recuperar la información de la Acción Correctiva");
                    return;
                }

                string codigo_planaccion = LogicController.getCodigoPlanAccionAccionCorrectiva(id_accion_correctiva);
                if (codigo_planaccion == null)
                {
                    showMessageError("Error al recuperar la información del Plan de Acción");
                    return;
                }

                PlanAccion plan_accion = LogicController.getPlanAccion(codigo_planaccion);
                if (plan_accion == null)
                {
                    showMessageError("Error al recuperar la información del Plan de Acción");
                    return;
                }


                if (Session["usuario"] == null)
                {
                    showMessageError("Error al recuperar la información del Usuario");
                    return;
                }

                Usuario u = (Usuario)Session["usuario"];
                if (!accion_correctiva.getResponsable().getRut().Equals(u.getRutPersona()))
                {
                    showMessageError("Sólo puedes registrar las acciones correctivas que te fueron asignadas");
                    return;
                }


                Session["PreviousPageAccionCorrectiva"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);
                Session["PlanAccion"] = plan_accion;
                Session["CodigoAccionCorrectivaSeleccionada"] = id_accion_correctiva;
                Response.Redirect("~/PlanesAccion/RegistrarAccionCorrectiva.aspx");

            }
        }


        protected void btDetalleEventoCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleEvento_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void btDetalleAccionCorrectivaCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleAccionCorrectiva_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void lbtDetalleEventoDetalleCreador_Click(object sender, EventArgs e)
        {
            if (lbtDetalleEventoDetalleCreador.CommandName.Equals("DetallePersona"))
            {
                if (lbtDetalleEventoDetalleCreador.CommandArgument != null)
                {
                    string rut = Convert.ToString(lbtDetalleEventoDetalleCreador.CommandArgument);
                    Session["RutDetalle"] = rut;
                    Session["PreviousPage"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Personas/DetallePersona.aspx", true);
                }
            }
            uPanel.Update();
        }


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }

        protected void btDefaultPasswordCerrar_Click(object sender, EventArgs e)
        {
            hfDefaultPassword_ModalPopupExtender.Hide();

            uPanel.Update();
        }

        protected void btDefaultPasswordCambiarClave_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/CambiarClave.aspx", true);
        }

        protected void gvAccionesInmediatasPendientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetalleEvento"))
            {
                string codigo_evento = (string)e.CommandArgument;
                if (loadDetalleEvento(codigo_evento))
                {
                    upDetalleEvento.Update();
                    hfDetalleEvento_ModalPopupExtender.Show();
                }
                else
                {
                    showMessageError("Error al cargar los detalles del Evento");
                }
            }
            else if (e.CommandName.Equals("RegistrarAccionInmediata"))
            {
                string codigo_evento = (string)e.CommandArgument;

                Session["CodigoEventoSeleccionado"] = codigo_evento;
                Session["PreviousPageAccionInmediata"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                Response.Redirect("~/Registros/RegistrarAccionInmediata.aspx");
            }
        }

    }
}