using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;

namespace NCCSAN.Investigaciones
{
    public partial class DetalleEvaluacion : System.Web.UI.Page
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

                if ((Session["CodigoEventoSeleccionado"] != null) && (Session["PreviousPageEvaluacion"] != null))
                {
                    string codigo = (string)Session["CodigoEventoSeleccionado"];
                    string previuosPage = (string)Session["PreviousPageEvaluacion"];

                    if ((codigo != null) && (previuosPage != null))
                    {
                        hfCodigoEvento.Value = codigo;
                        hfPreviousPage.Value = previuosPage;

                        string msg = loadDetalleInvestigacion(codigo);
                        if (msg != null)
                        {
                            Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                        }

                        msg = loadDetalleEvaluacion(codigo);
                        if (msg != null)
                        {
                            Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                        }

                    }
                    else
                    {
                        hfCodigoEvento.Value = "Error al pasar el parámetro";
                        Response.Redirect("~/Investigaciones/ListarEvaluacionesRealizadas.aspx", true);
                    }
                }
                else
                {
                    hfCodigoEvento.Value = "No se ha pasado el parámetro";
                    Response.Redirect("~/Investigaciones/ListarEvaluacionesRealizadas.aspx", true);
                }
            }

        }


        private string loadDetalleInvestigacion(string codigo_evento)
        {
            try
            {
                Investigacion investigacion = LogicController.getInvestigacion(codigo_evento);
                if (investigacion == null)
                {
                    return "Error al recuperar datos de la Investigación";
                }

                lbDetalleInvestigacionNombreResponsable.Text = investigacion.getResponsable().getNombre();
                lbDetalleInvestigacionAntiguedadResponsable.Text = Convert.ToString(investigacion.getAntiguedadResponsable());
                lbDetalleInvestigacionFechaInicio.Text = investigacion.getFechaInicio();
                if (investigacion.getFechaCierre() != null)
                {
                    lbDetalleInvestigacionFechaCierre.Text = investigacion.getFechaCierre();
                }
                else
                {
                    lbDetalleInvestigacionFechaCierre.Text = "--";
                }

                TimeSpan ts_dias_investigacion = Convert.ToDateTime(investigacion.getFechaCierre()) - Convert.ToDateTime(investigacion.getFechaInicio());
                lbDetalleInvestigacionDiasDeInvestigacion.Text = Convert.ToString(ts_dias_investigacion.Days);

                TimeSpan ts_dias_curso = DateTime.Now - Convert.ToDateTime(investigacion.getFechaInicio());
                lbDetalleInvestigacionDiasEnCurso.Text = Convert.ToString(ts_dias_curso.Days);

                if (!loadArchivosInvestigacion(codigo_evento))
                {
                    return "Error al cargar los archivos asociados a la Investigación";
                }


                return null;
            }
            catch (Exception ex)
            {
                return "Se ha producido un error al cargar la Investigación";
            }
        }



        private bool loadArchivosInvestigacion(string codigo_evento)
        {
            try
            {
                List<Archivo> listArchivosInvestigacion = LogicController.getArchivosInvestigacion(codigo_evento);
                if (listArchivosInvestigacion == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosInvestigacion = new List<string>();

                foreach (Archivo archivo in listArchivosInvestigacion)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosInvestigacion.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosInvestigacion.DataSource = dt;
                gvArchivosInvestigacion.DataBind();

                setListIDArchivosInvestigacion(listIDArchivosInvestigacion);

                for (int i = 0; i < listArchivosInvestigacion.Count; i++)
                {
                    ((Label)gvArchivosInvestigacion.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosInvestigacion[i].getNombre();
                    ((Label)gvArchivosInvestigacion.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosInvestigacion[i].getTipoArchivo();
                    ((Label)gvArchivosInvestigacion.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosInvestigacion[i].getSize()) + " bytes";

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        private void setListIDArchivosInvestigacion(List<string> listIDArchivosInvestigacion)
        {
            Session["listIDArchivosInvestigacion"] = listIDArchivosInvestigacion;
        }


        private List<string> getListIDArchivosInvestigacion()
        {
            if (Session["listIDArchivosInvestigacion"] != null)
            {
                return (List<string>)Session["listIDArchivosInvestigacion"];
            }
            else
            {
                return null;
            }
        }


        protected void gvArchivosInvestigacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosInvestigacion.Rows.Count))
                {
                    List<string> listIDArchivosInvestigacion = getListIDArchivosInvestigacion();
                    if (listIDArchivosInvestigacion == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados a la Investigación");
                        return;
                    }

                    string id_archivo = listIDArchivosInvestigacion[index];
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



        private string loadDetalleEvaluacion(string codigo_evento)
        {
            try
            {
                Evaluacion evaluacion = LogicController.getEvaluacion(codigo_evento);
                if (evaluacion == null)
                {
                    return "Error al recuperar datos de la Evaluación";
                }


                PersonaInfo evaluador = LogicController.getPersonaInfo(evaluacion.getRutResponsable());
                if (evaluador == null)
                {
                    return "Error al recuperar datos de la Evaluación";
                }


                PersonaInfo responsable_evento = LogicController.getResponsableEvento(codigo_evento);
                if (responsable_evento == null)
                {
                    return "Error al recuperar datos de la Evaluación";
                }

                lbDetalleEvaluacionOrigenFalla.Text = evaluacion.getNombreOrigen();
                lbDetalleEvaluacionTipoCausaInmediata.Text = evaluacion.getTipoCausaInmediata();
                lbDetalleEvaluacionCausaInmediata.Text = evaluacion.getNombreCausaInmediata();
                lbDetalleEvaluacionSubcausaInmediata.Text = evaluacion.getNombreSubcausaInmediata();
                lbDetalleEvaluacionTipoCausaBasica.Text = evaluacion.getTipoCausaBasica();
                lbDetalleEvaluacionCausaBasica.Text = evaluacion.getNombreCausaBasica();
                lbDetalleEvaluacionSubcausaBasica.Text = evaluacion.getNombreSubcausaBasica();
                lbNombreResponsableEvento.Text = responsable_evento.getNombre();
                lbDetalleEvaluacionAceptado.Text = evaluacion.getAceptado();
                if (evaluacion.getObservacion() != null)
                {
                    lbDetalleEvaluacionObservacion.Text = evaluacion.getObservacion();
                }
                lbDetalleEvaluacionFecha.Text = evaluacion.getFecha();
                lbtDetalleEvaluacionDetalleEvaluador.CommandName = "DetallePersona";

                lbtDetalleEvaluacionDetalleEvaluador.CommandArgument = evaluador.getRut();
                lbtDetalleEvaluacionDetalleEvaluador.Text = evaluador.getNombre();


                if (!loadArchivosEvaluacion(codigo_evento))
                {
                    return "Error al cargar los archivos asociados a la Evaluación";
                }


                if (!loadInvolucradosEvaluacion(codigo_evento))
                {
                    return "Error al cargar la lista de involucrados en el Evento";
                }

                return null;

            }
            catch (Exception ex)
            {
                return "Se ha producido un error al cargar la Evaluación";
            }
        }



        private bool loadArchivosEvaluacion(string codigo_evento)
        {
            try
            {
                List<Archivo> listArchivosEvaluacion = LogicController.getArchivosEvaluacion(codigo_evento);
                if (listArchivosEvaluacion == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosEvaluacion = new List<string>();

                foreach (Archivo archivo in listArchivosEvaluacion)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosEvaluacion.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosEvaluacion.DataSource = dt;
                gvArchivosEvaluacion.DataBind();

                setListIDArchivosEvaluacion(listIDArchivosEvaluacion);

                for (int i = 0; i < listArchivosEvaluacion.Count; i++)
                {
                    ((Label)gvArchivosEvaluacion.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosEvaluacion[i].getNombre();
                    ((Label)gvArchivosEvaluacion.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosEvaluacion[i].getTipoArchivo();
                    ((Label)gvArchivosEvaluacion.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosEvaluacion[i].getSize()) + " bytes";

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        private void setListIDArchivosEvaluacion(List<string> listIDArchivosEvaluacion)
        {
            Session["listIDArchivosEvaluacion"] = listIDArchivosEvaluacion;
        }


        private List<string> getListIDArchivosEvaluacion()
        {
            if (Session["listIDArchivosEvaluacion"] != null)
            {
                return (List<string>)Session["listIDArchivosEvaluacion"];
            }
            else
            {
                return null;
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



        protected void lbtDetalleEvaluacionDetalleEvaluador_Click(object sender, EventArgs e)
        {
            if (lbtDetalleEvaluacionDetalleEvaluador.CommandName.Equals("DetallePersona"))
            {
                if (lbtDetalleEvaluacionDetalleEvaluador.CommandArgument != null)
                {
                    string rut = Convert.ToString(lbtDetalleEvaluacionDetalleEvaluador.CommandArgument);
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


        protected void gvArchivosEvaluacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosEvaluacion.Rows.Count))
                {
                    List<string> listIDArchivosEvaluacion = getListIDArchivosEvaluacion();
                    if (listIDArchivosEvaluacion == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados a la Evaluación");
                        return;
                    }

                    string id_archivo = listIDArchivosEvaluacion[index];
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


        private static DataTable getDTPersonasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("RUT");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Centro");
            dt.Columns.Add("Cargo");
            dt.Columns.Add("Antiguedad");

            dt.AcceptChanges();

            return dt;
        }




        private bool loadInvolucradosEvaluacion(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return false;
            }

            Evaluacion evaluacion = LogicController.getEvaluacion(codigo_evento);
            if (evaluacion == null)
            {
                return false;
            }

            DataTable dt = getDTPersonasFormat();

            DataRow dr;

            for (int i = 0; i < evaluacion.getListInvolucrados().Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvInvolucrados.DataSource = dt;
            gvInvolucrados.DataBind();


            for (int i = 0; i < evaluacion.getListInvolucrados().Count; i++)
            {
                ((Label)gvInvolucrados.Rows[i].FindControl("lbRUT")).Text = evaluacion.getListInvolucrados()[i].getRut();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombre")).Text = evaluacion.getListInvolucrados()[i].getNombre();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombreCentro")).Text = evaluacion.getListInvolucrados()[i].getNombreCentro();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbCargo")).Text = evaluacion.getListInvolucrados()[i].getCargo();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(evaluacion.getListInvolucrados()[i].getAntiguedad());
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
            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }
    }
}