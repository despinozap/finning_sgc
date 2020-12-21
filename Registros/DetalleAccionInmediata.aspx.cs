using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;

namespace NCCSAN.Registros
{
    public partial class DetalleAccionInmediata : System.Web.UI.Page
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

                Session.Remove("listArchivos");

                if ((Session["CodigoEventoSeleccionado"] != null) && (Session["PreviousPageAccionInmediata"] != null))
                {
                    string codigo = (string)Session["CodigoEventoSeleccionado"];
                    string previuosPage = (string)Session["PreviousPageAccionInmediata"];

                    if ((codigo != null) && (previuosPage != null))
                    {
                        hfCodigoEvento.Value = codigo;
                        hfPreviousPage.Value = previuosPage;

                        if (!loadDetalleAccionInmediata(codigo))
                        {
                            string msg = "Error al cargar la Acción Inmediata";
                            Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                        }
                    }
                    else
                    {
                        string msg = "Error al pasar el parámetro";
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


        private bool loadDetalleAccionInmediata(string codigo_evento)
        {
            try
            {
                AccionInmediata accion_inmediata = LogicController.getAccionInmediata(codigo_evento);
                if (accion_inmediata == null)
                {
                    return false;
                }


                PersonaInfo responsable_evento = LogicController.getResponsableEvento(codigo_evento);
                if (responsable_evento == null)
                {
                    return false;
                }

                lbOrigenFalla.Text = accion_inmediata.getNombreOrigen();
                lbTipoCausaInmediata.Text = accion_inmediata.getTipoCausaInmediata();
                lbCausaInmediata.Text = accion_inmediata.getNombreCausaInmediata();
                lbSubcausaInmediata.Text = accion_inmediata.getNombreSubcausaInmediata();
                lbTipoCausaBasica.Text = accion_inmediata.getTipoCausaBasica();
                lbCausaBasica.Text = accion_inmediata.getNombreCausaBasica();
                lbSubcausaBasica.Text = accion_inmediata.getNombreSubcausaBasica();
                lbNombreResponsableEvento.Text = responsable_evento.getNombre();

                lbAccionInmediata.Text = accion_inmediata.getAccionInmediata();
                lbFechaAccion.Text = accion_inmediata.getFechaAccion();
                lbEfectividad.Text = accion_inmediata.getEfectividad();
                if (accion_inmediata.getObservacion() != null)
                {
                    lbObservacion.Text = accion_inmediata.getObservacion();
                }


                if (!loadArchivosAccionInmediata(codigo_evento))
                {
                    showMessageError("Error al cargar los archivos asociados a la Acción Inmediata");

                    return false;
                }

                if (!loadInvolucradosAccionInmediata(codigo_evento))
                {
                    showMessageError("Error al cargar la lista de involucrados en el Evento");

                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private bool loadArchivosAccionInmediata(string codigo_evento)
        {
            try
            {
                List<Archivo> listArchivosAccionInmediata = LogicController.getArchivosAccionInmediata(codigo_evento);
                if (listArchivosAccionInmediata == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosAccionInmediata = new List<string>();

                foreach (Archivo archivo in listArchivosAccionInmediata)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosAccionInmediata.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosAccionInmediata.DataSource = dt;
                gvArchivosAccionInmediata.DataBind();

                setListIDArchivosAccionInmediata(listIDArchivosAccionInmediata);

                for (int i = 0; i < listArchivosAccionInmediata.Count; i++)
                {
                    ((Label)gvArchivosAccionInmediata.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosAccionInmediata[i].getNombre();
                    ((Label)gvArchivosAccionInmediata.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosAccionInmediata[i].getTipoArchivo();
                    ((Label)gvArchivosAccionInmediata.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosAccionInmediata[i].getSize()) + " bytes";

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        private void setListIDArchivosAccionInmediata(List<string> listIDArchivosAccionInmediata)
        {
            Session["listIDArchivosAccionInmediata"] = listIDArchivosAccionInmediata;
        }


        private List<string> getListIDArchivosAccionInmediata()
        {
            if (Session["listIDArchivosAccionInmediata"] != null)
            {
                return (List<string>)Session["listIDArchivosAccionInmediata"];
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


        protected void btDetalleEventoCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleEvento_ModalPopupExtender.Hide();
            uPanel.Update();
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


        protected void gvArchivosAccionInmediata_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosAccionInmediata.Rows.Count))
                {
                    List<string> listIDArchivosAccionInmediata = getListIDArchivosAccionInmediata();
                    if (listIDArchivosAccionInmediata == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados a la Acción Inmediata");
                        return;
                    }

                    string id_archivo = listIDArchivosAccionInmediata[index];
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




        private bool loadInvolucradosAccionInmediata(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return false;
            }

            AccionInmediata accion_inmediata = LogicController.getAccionInmediata(codigo_evento);
            if (accion_inmediata == null)
            {
                return false;
            }

            DataTable dt = getDTPersonasFormat();

            DataRow dr;

            for (int i = 0; i < accion_inmediata.getListInvolucrados().Count; i++)
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


            for (int i = 0; i < accion_inmediata.getListInvolucrados().Count; i++)
            {
                ((Label)gvInvolucrados.Rows[i].FindControl("lbRUT")).Text = accion_inmediata.getListInvolucrados()[i].getRut();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombre")).Text = accion_inmediata.getListInvolucrados()[i].getNombre();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombreCentro")).Text = accion_inmediata.getListInvolucrados()[i].getNombreCentro();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbCargo")).Text = accion_inmediata.getListInvolucrados()[i].getCargo();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(accion_inmediata.getListInvolucrados()[i].getAntiguedad());
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
            if (Session["PreviousPageAccionInmediata"] != null)
            {
                string previuosPage = (string)Session["PreviousPageAccionInmediata"];
                Response.Redirect(previuosPage, true);
            }
            else
            {
                Response.Redirect("~/Default.aspx", true);
            }
        }


    }
}