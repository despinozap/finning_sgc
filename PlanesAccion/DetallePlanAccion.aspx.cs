using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;

namespace NCCSAN.PlanesAccion
{
    public partial class DetallePlanAccion : System.Web.UI.Page
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

                Session.Remove("PlanAccion");
                Session.Remove("AccionCorrectiva");
                Session.Remove("CodigoAccionCorrectivaSeleccionada");
                Session.Remove("listIDArchivosAccionCorrectiva");

                if ((Session["CodigoPlanAccionSeleccionado"] != null) && (Session["PreviousPagePlanAccion"] != null))
                {
                    string codigo = (string)Session["CodigoPlanAccionSeleccionado"];
                    string previuosPage = (string)Session["PreviousPagePlanAccion"];

                    if ((codigo != null) && (previuosPage != null))
                    {
                        hfCodigoEvento.Value = codigo;
                        hfPreviousPage.Value = previuosPage;

                        PlanAccion plan_acccion = LogicController.getPlanAccion(codigo);
                        if (plan_acccion != null)
                        {
                            Session["PlanAccion"] = plan_acccion;
                            loadPlanAccion();
                        }
                        else
                        {
                            string msg = "Error al recuperar el Plan de Acción";
                            Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                        }
                    }
                    else
                    {
                        string msg = "Error al recuperar la información del Plan de Acción";
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }
                }
                else
                {
                    string msg = "No se ha pasado el parámetro";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }


                if (Session["usuario"] != null)
                {
                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();

                    if ((rol.Equals("Jefe Calidad")) || (rol.Equals("Coordinador")))
                    {
                        gvAccionesCorrectivas.Columns[5].Visible = true;
                    }
                    else
                    {
                        gvAccionesCorrectivas.Columns[5].Visible = false;
                    }
                }
            }
        }


        private PlanAccion getPlanAccion()
        {
            if (Session["PlanAccion"] != null)
            {
                PlanAccion plan_accion = (PlanAccion)Session["PlanAccion"];

                return plan_accion;
            }
            else
            {
                return null;
            }
        }


        private void loadPlanAccion()
        {
            PlanAccion plan_accion = getPlanAccion();
            if (plan_accion == null)
                return;


            lbDetalleCorreccion.Text = plan_accion.getDetalleCorreccion();
            lbFechaCorreccion.Text = plan_accion.getFechaCorreccion().ToShortDateString();
            int progreso = plan_accion.getProgreso();

            lbProgreso.Text = Convert.ToString(progreso) + "%";
            pnEjecutado.Width = System.Web.UI.WebControls.Unit.Parse(Convert.ToString(progreso) + "%");

            if (progreso >= 75)
            {
                pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#00CC00");
            }
            else if (progreso >= 50)
            {
                pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#F9ED60");
            }
            else if (progreso >= 25)
            {
                pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9933");
            }
            else
            {
                pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF3300");
            }

            updateGVAccionesCorrectivas();
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

                lbDetalleEventoNombreCliente.Text = evento.getNombreCliente();
                lbDetalleEventoNombreFuente.Text = evento.getNombreFuente();
                lbDetalleEventoFecha.Text = evento.getFecha().ToShortDateString();
                lbDetalleEventoTipoEquipo.Text = evento.getTipoEquipo();
                lbDetalleEventoModeloEquipo.Text = evento.getModeloEquipo();

                if (evento.getSerieEquipo() != null)
                {
                    lbDetalleEventoSerieEquipo.Text = evento.getSerieEquipo();
                }

                lbDetalleEventoNombreSistema.Text = evento.getNombreSistema();
                lbDetalleEventoNombreSubsistema.Text = evento.getNombreSubsistema();
                lbDetalleEventoNombreComponente.Text = evento.getNombreComponente();

                if (evento.getSerieComponente() != null)
                {
                    lbDetalleEventoSerieComponente.Text = evento.getSerieComponente();
                }

                if (evento.getParte() != null)
                {
                    lbDetalleEventoParte.Text = evento.getParte();
                }

                if (evento.getNumeroParte() != null)
                {
                    lbDetalleEventoNumeroParte.Text = evento.getNumeroParte();
                }

                if (evento.getHoras() >= 0)
                {
                    lbDetalleEventoHoras.Text = Convert.ToString(evento.getHoras());
                }
                else
                {
                    lbDetalleEventoHoras.Text = "--";
                }

                lbDetalleEventoNombreArea.Text = evento.getNombreArea();
                lbDetalleEventoNombreSubarea.Text = evento.getNombreSubarea();
                lbDetalleEventoNombreClasificacion.Text = evento.getNombreClasificacion();
                lbDetalleEventoNombreSubclasificacion.Text = evento.getNombreSubclasificacion();
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


        protected void lbtDetalleEventoDetalleCreador_Click(object sender, EventArgs e)
        {
            if (lbtDetalleEventoDetalleCreador.CommandName.Equals("DetallePersona"))
            {
                if (lbtDetalleEventoDetalleCreador.CommandArgument != null)
                {
                    string rut = Convert.ToString(lbtDetalleEventoDetalleCreador.CommandArgument);
                    Session["RutDetalle"] = rut;
                    Session["PreviousPagePersona"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Personas/DetallePersona.aspx", true);
                }
            }
            uPanel.Update();
        }


        protected void btDetalleEventoCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleEvento_ModalPopupExtender.Hide();
            uPanel.Update();
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


        protected void ibVerDetalleEvento_Click(object sender, ImageClickEventArgs e)
        {
            if (loadDetalleEvento(hfCodigoEvento.Value))
            {
                hfDetalleEvento_ModalPopupExtender.Show();
            }
            else
            {
                showMessageError("Error al cargar los detalles del Evento");
            }
        }


        private void setListIDArchivosAccionCorrectiva(List<string> listIDArchivosAccionCorrectiva)
        {
            Session["listIDArchivosAccionCorrectiva"] = listIDArchivosAccionCorrectiva;
        }


        private List<string> getListIDArchivosAccionCorrectiva()
        {
            if (Session["listIDArchivosAccionCorrectiva"] != null)
            {
                return (List<string>)Session["listIDArchivosAccionCorrectiva"];
            }
            else
            {
                return null;
            }
        }


        private bool loadDetalleAccionCorrectiva(AccionCorrectiva accion_correctiva)
        {
            if (accion_correctiva == null)
            {
                return false;
            }

            //Datos
            {
                lbDescripcionAccionCorrectiva.Text = accion_correctiva.getDescripcion();
                lbNombreResponsableAccionCorrectiva.Text = accion_correctiva.getResponsable().getNombre();
                lbFechaLimiteAccionCorrectiva.Text = accion_correctiva.getFechaLimite();
                lbFechaRealizadoAccionCorrectiva.Text = accion_correctiva.getFechaRealizado();
                lbObservacionAccionCorrectiva.Text = accion_correctiva.getObservacion();
            }


            //Archivos
            {
                List<Archivo> listArchivosAccionCorrectiva = LogicController.getArchivosAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                if (listArchivosAccionCorrectiva == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosAccionCorrectiva = new List<string>();

                foreach (Archivo archivo in listArchivosAccionCorrectiva)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosAccionCorrectiva.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosAccionCorrectiva.DataSource = dt;
                gvArchivosAccionCorrectiva.DataBind();

                setListIDArchivosAccionCorrectiva(listIDArchivosAccionCorrectiva);

                for (int i = 0; i < listArchivosAccionCorrectiva.Count; i++)
                {
                    ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosAccionCorrectiva[i].getNombre();
                    ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosAccionCorrectiva[i].getTipoArchivo();
                    ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosAccionCorrectiva[i].getSize()) + " bytes";
                }

            }

            return true;
        }



        private void showMessageInfo(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxInfo", "<script type=\"text/javascript\">showMessageInfo('" + message + "');</script>", false);
        }

        private void showMessageWarning(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxWarning", "<script type=\"text/javascript\">showMessageWarning('" + message + "');</script>", false);
        }

        private void showMessageSuccess(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxSuccess", "<script type=\"text/javascript\">showMessageSuccess('" + message + "');</script>", false);
        }


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }


        protected void gvAccionesCorrectivas_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName.Equals("IngresarAccionCorrectiva"))
            {
                Session.Remove("CodigoAccionCorrectivaSeleccionada");

                int index = -1;
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index < 0)
                    return;


                if (Session["usuario"] == null)
                {
                    return;
                }

                Usuario usuario = (Usuario)Session["usuario"];
                string rol = usuario.getNombreRol();

                if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                {
                    showMessageError("No tienes privilegios para registrar Acciones Correctivas");
                    return;
                }


                PlanAccion plan_accion = getPlanAccion();

                if (plan_accion == null)
                {
                    showMessageError("Error al recuperar información del Plan de Acción");
                    return;
                }

                if (index < gvAccionesCorrectivas.Rows.Count)
                {
                    Session["CodigoAccionCorrectivaSeleccionada"] = plan_accion.getListAccionesCorrectivas()[index].getIdAccionCorrectiva();
                    Session["PreviousPageAccionCorrectiva"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);
                    Response.Redirect("~/PlanesAccion/RegistrarAccionCorrectiva.aspx");
                }

            }
            else if (e.CommandName.Equals("DetalleAccionCorrectiva"))
            {

                int index = -1;
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index < 0)
                    return;

                PlanAccion plan_accion = getPlanAccion();

                if (plan_accion == null)
                {
                    showMessageError("Error al recuperar información del Plan de Acción");
                    return;
                }

                AccionCorrectiva accion_correctiva = null;
                if (index < gvAccionesCorrectivas.Rows.Count)
                {
                    accion_correctiva = plan_accion.getListAccionesCorrectivas()[index];
                }


                if (accion_correctiva == null)
                {
                    showMessageError("Error al recuperar información de la Acción Correctiva");

                    return;
                }


                if (loadDetalleAccionCorrectiva(accion_correctiva))
                {
                    hfDetalleAccionCorrectiva_ModalPopupExtender.Show();
                }
                else
                {
                    showMessageError("Error al cargar información de la Acción Correctiva");
                }
            }
        }


        private void setShowHeaderOnEmpty(GridView gv)
        {
            DataTable dt = getDTAccionesCorrectivasFormat();
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            gv.DataSource = dt;
            gv.DataBind();
        }


        private DataTable getDTAccionesCorrectivasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("");
            dt.Columns.Add("Descripción");
            dt.Columns.Add("Fecha límite");
            dt.Columns.Add("Fecha ejecución");
            dt.Columns.Add("Responsable");
            dt.Columns.Add("Registrar");
            dt.Columns.Add("Detalle");

            dt.AcceptChanges();

            return dt;
        }


        private void updateGVAccionesCorrectivas()
        {
            PlanAccion plan_accion = getPlanAccion();
            if (plan_accion == null)
                return;

            DataTable dt = getDTAccionesCorrectivasFormat();
            List<AccionCorrectiva> listAccionesCorrectivas = plan_accion.getListAccionesCorrectivas();

            if (listAccionesCorrectivas.Count < 1)
            {
                setShowHeaderOnEmpty(gvAccionesCorrectivas);
                gvAccionesCorrectivas.Columns[4].Visible = false;

                return;
            }

            gvAccionesCorrectivas.Columns[4].Visible = true;

            DataRow dr;

            for (int i = 0; i < listAccionesCorrectivas.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Label();
                dr[5] = new Button();
                dr[6] = new Button();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvAccionesCorrectivas.DataSource = dt;
            gvAccionesCorrectivas.DataBind();

            Image imgEstadoIcono;
            for (int i = 0; i < listAccionesCorrectivas.Count; i++)
            {
                imgEstadoIcono = (Image)gvAccionesCorrectivas.Rows[i].FindControl("imgEstadoIcono");

                if (listAccionesCorrectivas[i].getFechaRealizado() != null)
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_green.png";
                    gvAccionesCorrectivas.Rows[i].Cells[5].Controls[0].Visible = false;
                }
                else if (Convert.ToDateTime(listAccionesCorrectivas[i].getFechaLimite()) < DateTime.Now)
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                    gvAccionesCorrectivas.Rows[i].Cells[6].Controls[0].Visible = false;
                }
                else
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";
                    gvAccionesCorrectivas.Rows[i].Cells[6].Controls[0].Visible = false;
                }

                ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbDescripcion")).Text = listAccionesCorrectivas[i].getDescripcion();
                ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbFechaLimite")).Text = listAccionesCorrectivas[i].getFechaLimite();
                if (listAccionesCorrectivas[i].getFechaRealizado() != null)
                {
                    ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbFechaRealizado")).Text = listAccionesCorrectivas[i].getFechaRealizado();
                }
                ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbNombreResponsable")).Text = listAccionesCorrectivas[i].getResponsable().getNombre();
            }
        }


        protected void btDetalleAccionCorrectivaCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleAccionCorrectiva_ModalPopupExtender.Hide();
        }

        protected void gvArchivosAccionCorrectiva_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosAccionCorrectiva.Rows.Count))
                {
                    List<string> listIDArchivosAccionCorrectiva = getListIDArchivosAccionCorrectiva();
                    if (listIDArchivosAccionCorrectiva == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados a la Acción Correctiva");
                        return;
                    }

                    string id_archivo = listIDArchivosAccionCorrectiva[index];
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


        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }
    }
}