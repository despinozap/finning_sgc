using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.Registros
{
    public partial class DetalleEvento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                {//Acceso del Usuario a la Página
                    if (Session["usuario"] == null)
                    {
                        Session["LinkedPage"] = Request.Url.ToString();

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

                string codigo = null;
                if (Session["CodigoEventoSeleccionado"] != null)
                {
                    codigo = (string)Session["CodigoEventoSeleccionado"];
                }
                else if (Request.QueryString["ce"] != null)
                {
                    codigo = Request.QueryString["ce"];
                }
                else
                {
                    string msg = "No se ha pasado el parámetro";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }


                if (codigo != null)
                {

                    hfCodigoEvento.Value = codigo;

                    if (!loadDetalleEvento(codigo))
                    {
                        string msg = "El Evento que intenta ver no existe";
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }

                }
                else
                {
                    string msg = "No se ha pasado el parámetro";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
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

                lbDetalleEventoNombreCliente.Text = evento.getNombreCliente();
                lbDetalleEventoNombreFuente.Text = evento.getNombreFuente();
                lbDetalleEventoFecha.Text = evento.getFecha().ToShortDateString();
                lbDetalleEventoTipoEquipo.Text = evento.getTipoEquipo();
                lbDetalleEventoModeloEquipo.Text = evento.getModeloEquipo();

                if (evento.getSerieEquipo() != null)
                {
                    lbDetalleEventoSerieEquipo.Text = evento.getSerieEquipo();
                }
                else
                {
                    lbDetalleEventoSerieEquipo.Text = "--";
                }

                lbDetalleEventoNombreSistema.Text = evento.getNombreSistema();
                lbDetalleEventoNombreSubsistema.Text = evento.getNombreSubsistema();
                lbDetalleEventoNombreComponente.Text = evento.getNombreComponente();

                if (evento.getSerieComponente() != null)
                {
                    lbDetalleEventoSerieComponente.Text = evento.getSerieComponente();
                }
                else
                {
                    lbDetalleEventoSerieComponente.Text = "--";
                }

                if (evento.getParte() != null)
                {
                    lbDetalleEventoParte.Text = evento.getParte();
                }
                else
                {
                    lbDetalleEventoParte.Text = "--";
                }

                if (evento.getNumeroParte() != null)
                {
                    lbDetalleEventoNumeroParte.Text = evento.getNumeroParte();
                }
                else
                {
                    lbDetalleEventoNumeroParte.Text = "--";
                }

                if (evento.getHoras() >= 0)
                {
                    lbDetalleEventoHoras.Text = Convert.ToString(evento.getHoras());
                }
                else
                {
                    lbDetalleEventoHoras.Text = "--";
                }

                if (evento.getAgenteCorrector() != null)
                {
                    lbDetalleEventoAgenteCorrector.Text = evento.getAgenteCorrector();
                }
                else
                {
                    lbDetalleEventoAgenteCorrector.Text = "--";
                }

                if (evento.getNombreArea() != null)
                {
                    lbDetalleEventoNombreArea.Text = evento.getNombreArea();
                    lbDetalleEventoNombreSubarea.Text = evento.getNombreSubarea();
                }
                else
                {
                    lbDetalleEventoNombreArea.Text = "--";
                    lbDetalleEventoNombreSubarea.Text = "--";
                }

                lbDetalleEventoNombreClasificacion.Text = evento.getNombreClasificacion();
                lbDetalleEventoNombreSubclasificacion.Text = evento.getNombreSubclasificacion();

                if (evento.getCriticidad() != null)
                {
                    lbDetalleEventoCriticidad.Text = evento.getCriticidad();
                    lbDetalleEventoProbabilidad.Text = evento.getProbabilidad();
                    lbDetalleEventoConsecuencia.Text = evento.getConsecuencia();
                    lbDetalleEventoIRC.Text = Convert.ToString(evento.getIRC());
                }
                else
                {
                    lbDetalleEventoCriticidad.Text = "--";
                    lbDetalleEventoProbabilidad.Text = "--";
                    lbDetalleEventoConsecuencia.Text = "--";
                    lbDetalleEventoIRC.Text = "--";
                }


                lbDetalleEventoDetalle.Text = evento.getDetalle();
                lbtDetalleEventoDetalleCreador.CommandName = "DetallePersona";

                lbtDetalleEventoDetalleCreador.CommandArgument = creador.getRut();
                lbtDetalleEventoDetalleCreador.Text = creador.getNombre();

                int involucrados_loaded = loadInvolucradosEvento(codigo_evento);
                if (involucrados_loaded < 0)
                {
                    return false;
                }
                else if (involucrados_loaded > 0)
                {
                    pnInvolucrados.Visible = true;
                }
                else
                {
                    pnInvolucrados.Visible = false;
                }


                if (!loadArchivosEvento(codigo_evento))
                {
                    showMessageError("Error al cargar los archivos asociados al Evento");

                    return false;
                }

                if (!loadDocumentosRelacionados(codigo_evento))
                {
                    showMessageError("Error al cargar los documentos relacionados al Evento");

                    return false;
                }

                return true;

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


        private void setShowHeaderOnEmpty(GridView gv)
        {
            DataTable dt = getDTPersonasFormat();
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            gv.DataSource = dt;
            gv.DataBind();
        }


        private static DataTable getDTPersonasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("RUT");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Cargo");
            dt.Columns.Add("Antiguedad");

            dt.AcceptChanges();

            return dt;
        }




        private int loadInvolucradosEvento(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return -1;
            }

            if (LogicController.eventoExists(codigo_evento) < 1)
            {
                return -1;
            }

            if ((LogicController.accionInmediataExists(codigo_evento) < 1) && (LogicController.evaluacionExists(codigo_evento) < 1))
            {
                return 0;
            }

            DataTable dt = getDTPersonasFormat();

            List<PersonaInfo> listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
            if (listInvolucrados == null)
            {
                return -1;
            }

            if (listInvolucrados.Count < 1)
            {
                setShowHeaderOnEmpty(gvInvolucrados);

                return 1;
            }

            DataRow dr;

            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvInvolucrados.DataSource = dt;
            gvInvolucrados.DataBind();


            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                ((Label)gvInvolucrados.Rows[i].FindControl("lbRUT")).Text = listInvolucrados[i].getRut();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombre")).Text = listInvolucrados[i].getNombre();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbCargo")).Text = listInvolucrados[i].getCargo();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(listInvolucrados[i].getAntiguedad());
            }

            return 1;
        }



        private bool loadDocumentosRelacionados(string codigo_evento)
        {
            int accion_inmediata_exist = LogicController.accionInmediataExists(codigo_evento);
            int evaluacion_exist = LogicController.evaluacionExists(codigo_evento);
            int plan_accion_exist = LogicController.planAccionExists(codigo_evento);
            int verificacion_exist = LogicController.verificacionExists(codigo_evento);

            if ((accion_inmediata_exist < 0) || (evaluacion_exist < 0) || (plan_accion_exist < 0) || (verificacion_exist < 0))
            {
                return false;
            }

            DataTable dt = new DataTable();

            dt.Columns.Add("Acción Inmediata");
            dt.Columns.Add("Evaluación");
            dt.Columns.Add("Plan de Acción");
            dt.Columns.Add("Verificación");

            dt.AcceptChanges();

            if ((accion_inmediata_exist == 1) || (evaluacion_exist == 1) || (plan_accion_exist == 1) || (verificacion_exist == 1))
            {
                DataRow dr;

                dr = dt.NewRow();

                dr[0] = new ImageButton();
                dr[1] = new ImageButton();
                dr[2] = new ImageButton();
                dr[3] = new ImageButton();

                dt.Rows.Add(dr);

                dt.AcceptChanges();
                gvDocumentosRelacionados.DataSource = dt;
                gvDocumentosRelacionados.DataBind();

                if (accion_inmediata_exist < 1)
                {
                    gvDocumentosRelacionados.Rows[0].Cells[0].FindControl("ibDetalleAccionInmediata").Visible = false;
                }

                if (evaluacion_exist < 1)
                {
                    gvDocumentosRelacionados.Rows[0].Cells[1].FindControl("ibDetalleEvaluacion").Visible = false;
                }

                if (plan_accion_exist < 1)
                {
                    gvDocumentosRelacionados.Rows[0].Cells[2].FindControl("ibDetallePlanAccion").Visible = false;
                }

                if (verificacion_exist < 1)
                {
                    gvDocumentosRelacionados.Rows[0].Cells[3].FindControl("ibDetalleVerificacion").Visible = false;
                }
            }
            else
            {
                dt.AcceptChanges();
                gvDocumentosRelacionados.DataSource = dt;
                gvDocumentosRelacionados.DataBind();
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


        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["PreviousPageEvento"] != null)
            {
                string previuosPage = (string)Session["PreviousPageEvento"];
                Response.Redirect(previuosPage, true);
            }
            else
            {
                Response.Redirect("~/Default.aspx", true);
            }
        }

        protected void gvDocumentosRelacionados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetalleAccionInmediata"))
            {
                Session["CodigoEventoSeleccionado"] = hfCodigoEvento.Value;
                Session["PreviousPageAccionInmediata"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                Response.Redirect("~/Registros/DetalleAccionInmediata.aspx");
            }
            else if (e.CommandName.Equals("DetalleEvaluacion"))
            {
                Session["CodigoEventoSeleccionado"] = hfCodigoEvento.Value;
                Session["PreviousPageEvaluacion"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                Response.Redirect("~/Investigaciones/DetalleEvaluacion.aspx");
            }
            else if (e.CommandName.Equals("DetallePlanAccion"))
            {
                Session["CodigoPlanAccionSeleccionado"] = hfCodigoEvento.Value;
                Session["PreviousPagePlanAccion"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                Response.Redirect("~/PlanesAccion/DetallePlanAccion.aspx");
            }
            else if (e.CommandName.Equals("DetalleVerificacion"))
            {
                Session["CodigoPlanAccionSeleccionado"] = hfCodigoEvento.Value;
                Session["PreviousPageVerificacion"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                Response.Redirect("~/PlanesAccion/DetalleVerificacion.aspx");
            }
        }

    }
}