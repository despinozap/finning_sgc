using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using System.Data;
using NCCSAN.Source.Logic;
using System.IO;

namespace NCCSAN.Registros
{
    public partial class RegistroAccionInmediata : System.Web.UI.Page
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

                List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();
                saveListInvolucrados(listInvolucrados);
                updateGVInvolucrados();

                updateGVArchivos();

                if ((Session["CodigoEventoSeleccionado"] != null) && (Session["PreviousPageAccionInmediata"] != null))
                {
                    string codigo = (string)Session["CodigoEventoSeleccionado"];
                    string previuosPage = (string)Session["PreviousPageAccionInmediata"];

                    if ((codigo != null) && (previuosPage != null))
                    {
                        hfCodigoEvento.Value = codigo;

                        hfPreviousPage.Value = previuosPage;

                        Session.Remove("Evaluacion");

                        int evaluacion_exists = LogicController.evaluacionExists(codigo);
                        if (evaluacion_exists < 0)
                        {
                            string msg = "Error al recuperar información del Evento";
                            Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                        }
                        else if (evaluacion_exists > 0)
                        {
                            pnCausas.Visible = false;
                            pnInvolucrados.Visible = false;

                            if (!loadEvaluacion(codigo))
                            {
                                string msg = "Error al recuperar información de la Evaluación";
                                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                            }

                        }
                        else if (evaluacion_exists == 0)
                        {
                            pnCausasEvaluacion.Visible = false;
                            pnInvolucradosEvaluacion.Visible = false;


                            if (!loadResponsablesEvento(codigo))
                            {
                                string msg = "Error al recuperar los posibles responsables del Evento";
                                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                            }

                            resetCausas();
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


        private void resetCausas()
        {
            ddlTipoCausaInmediata.Items.Clear();
            ddlTipoCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));

            ddlTipoCausaBasica.Items.Clear();
            ddlTipoCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlOrigenFalla_DataBound(object sender, EventArgs e)
        {
            ddlOrigenFalla.Items.Insert(0, new ListItem("Seleccione..", ""));
        }



        protected void ddlTipoCausaInmediata_DataBound(object sender, EventArgs e)
        {
            ddlTipoCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlTipoCausaInmediata_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCausaInmediata.Items.Clear();
            SDSCausaInmediata.DataBind();
            ddlCausaInmediata.DataBind();

            ddlSubcausaInmediata.Items.Clear();
            SDSSubcausaInmediata.DataBind();
            ddlSubcausaInmediata.DataBind();
        }



        protected void ddlCausaInmediata_DataBound(object sender, EventArgs e)
        {
            ddlCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlSubcausaInmediata_DataBound(object sender, EventArgs e)
        {
            ddlSubcausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlOrigenFalla_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlOrigenFalla.SelectedIndex < 1)
            {
                resetCausas();
            }
            else if (ddlOrigenFalla.SelectedValue.Equals("Finning"))
            {
                SDSTipoCausaInmediata.DataBind();
                ddlTipoCausaInmediata.DataSource = SDSTipoCausaInmediata;
                ddlTipoCausaInmediata.DataBind();

                SDSTipoCausaBasica.DataBind();
                ddlTipoCausaBasica.DataSource = SDSTipoCausaBasica;
                ddlTipoCausaBasica.DataBind();
            }
            else
            {
                ddlTipoCausaInmediata.Items.Clear();
                ddlTipoCausaInmediata.Items.Add(new ListItem("N/A", "N/A"));
                ddlTipoCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));

                ddlTipoCausaBasica.Items.Clear();
                ddlTipoCausaBasica.Items.Add(new ListItem("N/A", "N/A"));
                ddlTipoCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
            }

            ddlCausaInmediata.Items.Clear();
            SDSCausaInmediata.DataBind();
            ddlCausaInmediata.DataBind();

            ddlCausaBasica.Items.Clear();
            SDSCausaBasica.DataBind();
            ddlCausaBasica.DataBind();

            ddlSubcausaInmediata.Items.Clear();
            SDSSubcausaInmediata.DataBind();
            ddlSubcausaInmediata.DataBind();

            ddlSubcausaBasica.Items.Clear();
            SDSSubcausaBasica.DataBind();
            ddlSubcausaBasica.DataBind();
        }


        protected void ddlCausaInmediata_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubcausaInmediata.Items.Clear();
            SDSSubcausaInmediata.DataBind();
            ddlSubcausaInmediata.DataBind();
        }



        protected void ddlTipoCausaBasica_DataBound(object sender, EventArgs e)
        {
            ddlTipoCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlTipoCausaBasica_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCausaBasica.Items.Clear();
            SDSCausaBasica.DataBind();
            ddlCausaBasica.DataBind();

            ddlSubcausaBasica.Items.Clear();
            SDSSubcausaBasica.DataBind();
            ddlSubcausaBasica.DataBind();
        }


        protected void ddlCausaBasica_DataBound(object sender, EventArgs e)
        {
            ddlCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlCausaBasica_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubcausaBasica.Items.Clear();
            SDSSubcausaBasica.DataBind();
            ddlSubcausaBasica.DataBind();
        }

        protected void ddlSubcausaBasica_DataBound(object sender, EventArgs e)
        {
            ddlSubcausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }




        private bool loadEvaluacion(string codigo_evento)
        {
            Evaluacion ev = LogicController.getEvaluacion(codigo_evento);
            if (ev == null)
            {
                return false;
            }


            PersonaInfo responsable_evento = LogicController.getResponsableEvento(codigo_evento);
            if (responsable_evento == null)
            {
                return false;
            }

            lbOrigenFalla.Text = ev.getNombreOrigen();
            lbTipoCausaInmediata.Text = ev.getTipoCausaInmediata();
            lbCausaInmediata.Text = ev.getNombreCausaInmediata();
            lbSubcausaInmediata.Text = ev.getNombreSubcausaInmediata();
            lbTipoCausaBasica.Text = ev.getTipoCausaBasica();
            lbCausaBasica.Text = ev.getNombreCausaBasica();
            lbSubcausaBasica.Text = ev.getNombreSubcausaBasica();
            lbNombreResponsableEvento.Text = responsable_evento.getNombre();


            if (!loadInvolucradosEvaluacion(codigo_evento))
            {
                return false;
            }

            Session["Evaluacion"] = ev;

            return true;
        }


        private bool loadResponsablesEvento(string codigo_evento)
        {
            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return false;
            }

            List<PersonaInfo> listSupervisoes = LogicController.getListSupervisoresCentroAreaSubarea(evento.getIDCentro(), evento.getNombreArea(), evento.getNombreSubarea());
            if (listSupervisoes == null)
            {
                return false;
            }

            ddlResponsableEvento.Items.Clear();
            foreach (PersonaInfo supervisor in listSupervisoes)
            {
                ddlResponsableEvento.Items.Add(new ListItem("[Supervisor] " + supervisor.getNombre(), supervisor.getRut()));
            }

            PersonaInfo jefe = LogicController.getCentroAreaJefe(evento.getNombreArea(), evento.getIDCentro());
            if (jefe == null)
            {
                return false;
            }
            else
            {
                ddlResponsableEvento.Items.Add(new ListItem("[Jefe área] " + jefe.getNombre(), jefe.getRut()));
            }

            ddlResponsableEvento.Items.Insert(0, new ListItem("Seleccione..", ""));

            return true;
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


        private void showSummary(List<string> errors)
        {
            showMessageError("Para continuar debes completar los campos requeridos en el formulario");
            string summary = "<b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            ltSummary.Text = summary;
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

        protected void gvArchivos_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (false)
            {

            }
            else if (e.CommandName.Equals("RemoveArchivo"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index >= 0)
                {
                    List<Archivo> listArchivos = getListaArchivos();
                    if (index < listArchivos.Count)
                    {
                        string filename = listArchivos[index].getNombre();
                        listArchivos.RemoveAt(index);
                        setListaArchivos(listArchivos);

                        showMessageInfo("Se ha quitado el archivo \"" + filename + "\" del Evento");
                        updateGVArchivos();
                    }
                }
                else // index = -1
                {
                    return;
                }
            }
        }


        private List<Archivo> getListaArchivos()
        {
            List<Archivo> listArchivos;

            if (Session["listArchivos"] != null)
                listArchivos = (List<Archivo>)Session["listArchivos"];
            else
            {
                listArchivos = new List<Archivo>();
                setListaArchivos(listArchivos);
            }

            return listArchivos;
        }


        private void setListaArchivos(List<Archivo> listArchivo)
        {
            Session["listArchivos"] = listArchivo;
        }

        protected void ibAddArchivo_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";

            addFileToList();
        }


        private void addFileToList()
        {

            if (fuArchivo.HasFile)
            {
                List<Archivo> listArchivos = getListaArchivos();
                try
                {
                    byte[] contentFile;
                    BinaryReader binaryReader;
                    HttpFileCollection hfc = Request.Files;
                    HttpPostedFile hpf;
                    string fileType;
                    string contentType;
                    string fileName;
                    string valid_filename;
                    string ext;
                    int max_file_size;
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = hfc[i];

                        fileName = Path.GetFileName(hpf.FileName);
                        valid_filename = Utils.validateFilename(fileName);
                        if (valid_filename != null)
                        {
                            showMessageError(valid_filename);
                            return;
                        }
                        ext = Utils.getFileExtension(fileName);
                        contentType = Utils.getContentType(ext);
                        if (contentType == null)
                        {
                            showMessageError("Extensión de archivo \"" + ext + "\" no permitida");
                            return;
                        }

                        max_file_size = LogicController.getMaxFileSizeByExtension(ext);
                        if (max_file_size < 0)
                        {
                            showMessageError("No se puede comprobar el tamaño máximo permitido para el tipo de archivo");

                            return;
                        }


                        if ((hpf.ContentLength) > max_file_size)
                        {
                            showMessageError("El tipo de archivo no permite un tamaño superior a " + Convert.ToString(max_file_size / 1000000) + " Mb");

                            return;
                        }
                        

                        fileType = Utils.getFileType(ext);

                        contentFile = new byte[hpf.ContentLength];
                        binaryReader = new BinaryReader(hpf.InputStream);
                        binaryReader.Read(contentFile, 0, hpf.ContentLength);
                        Archivo archivo = new Archivo(Utils.getUniqueCode(), fileName, contentFile, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }
                catch (Exception ex)
                {
                    showMessageError("Se ha producido un error al cargar los archivos");
                    return;
                }

                setListaArchivos(listArchivos);

                updateGVArchivos();
            }
            else
            {
                showMessageWarning("No se ha seleccionado el archivo");
            }
        }



        private void updateGVArchivos()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Tamaño");

            DataRow dr;

            List<Archivo> listArchivos = getListaArchivos();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();

                dt.Rows.Add(dr);

            }

            dt.AcceptChanges();
            gvArchivos.DataSource = dt;
            gvArchivos.DataBind();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                ((Label)gvArchivos.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivos[i].getNombre();
                ((Label)gvArchivos.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivos[i].getTipoArchivo();
                ((Label)gvArchivos.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivos[i].getSize()) + " bytes";
            }
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


        protected void fuArchivo_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            showMessageSuccess("Se ha cargado el archivo \"" + fuArchivo.FileName + "\" al Evento");
        }

        protected void fuArchivo_UploadedFileError(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            showMessageError("Falló la carga del archivo \"" + fuArchivo.FileName + "\".Inténtelo nuevamente");
        }


        protected void ibRegistrarAccionInmediata_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";
            List<string> errors = validatePanelAccionInmediata();
            if (errors.Count == 0)
            {
                if (rblEfectividad.SelectedValue.Equals("Si"))
                {
                    lbMessageConfirmAccionInmediata.Text = "Se va a registrar la Acción Inmediata como <b>Efectiva</b>. ¿Desea continuar?";
                }
                else
                {
                    lbMessageConfirmAccionInmediata.Text = "Se va a registrar la Acción Inmediata como <b>No efectiva</b>. ¿Desea continuar?";
                }


                hfConfirmAccionInmediata_ModalPopupExtender.Show();

            }
            else
            {
                showSummary(errors);
            }
        }


        private List<string> validatePanelAccionInmediata()
        {
            List<string> errors = new List<string>();

            if (Session["Evaluacion"] == null)
            {
                if (ddlOrigenFalla.SelectedIndex < 1)
                {
                    errors.Add("No se ha seleccionado el origen de la falla");
                }

                if (ddlCausaInmediata.SelectedIndex < 1)
                {
                    errors.Add("No se ha seleccionado la causa inmediata");
                }

                if (ddlSubcausaInmediata.SelectedIndex < 1)
                {
                    errors.Add("No se ha seleccionado la sub-causa inmediata");
                }

                if (ddlCausaBasica.SelectedIndex < 1)
                {
                    errors.Add("No se ha seleccionado la causa básica");
                }

                if (ddlSubcausaBasica.SelectedIndex < 1)
                {
                    errors.Add("No se ha seleccionado la sub-causa básica");
                }

                if (ddlResponsableEvento.SelectedIndex < 1)
                {
                    errors.Add("No se ha seleccionado el responsable del Evento");
                }
            }


            if (txtAccionInmediata.Text.Length < 1)
            {
                errors.Add("No se ha indicado la acción inmediata");
            }

            if (txtFechaAccionInmediata.Text.Length < 1)
            {
                errors.Add("No se ha indicado la fecha de la acción inmediata");
            }

            if (rblEfectividad.SelectedIndex < 0)
            {
                errors.Add("Se debe seleccionar la efectividad de la Acción");
            }

            return errors;
        }



        private string registrarNuevaAccionInmediata()
        {
            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];

            List<Archivo> listArchivos = getListaArchivos();
            List<PersonaInfo> listInvolucrados = getListInvolucrados();


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }

            Usuario usuario = (Usuario)Session["usuario"];

            string rut_responsable;
            string origen;
            string causa_inmediata;
            string subcausa_inmediata;
            string causa_basica;
            string subcausa_basica;

            if (Session["Evaluacion"] != null)
            {
                Evaluacion ev = (Evaluacion)Session["Evaluacion"];

                origen = ev.getNombreOrigen();
                causa_inmediata = ev.getNombreCausaInmediata();
                subcausa_inmediata = ev.getNombreSubcausaInmediata();
                causa_basica = ev.getNombreCausaBasica();
                subcausa_basica = ev.getNombreSubcausaBasica();
                rut_responsable = null;
            }
            else
            {
                origen = ddlOrigenFalla.SelectedValue;
                causa_inmediata = ddlCausaInmediata.SelectedValue;
                subcausa_inmediata = ddlSubcausaInmediata.SelectedValue;
                causa_basica = ddlCausaBasica.SelectedValue;
                subcausa_basica = ddlSubcausaBasica.SelectedValue;
                rut_responsable = ddlResponsableEvento.SelectedValue;
            }


            string registrado = LogicController.registerAccionInmediata
                                                                    (
                                                                        hfCodigoEvento.Value,
                                                                        origen,
                                                                        causa_inmediata,
                                                                        subcausa_inmediata,
                                                                        causa_basica,
                                                                        subcausa_basica,
                                                                        rut_responsable,
                                                                        txtAccionInmediata.Text,
                                                                        txtFechaAccionInmediata.Text,
                                                                        rblEfectividad.SelectedValue,
                                                                        txtObservacion.Text,
                                                                        listArchivos,
                                                                        listInvolucrados,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );
            if (registrado == null)
            {
                Session.Remove("Evaluacion");

                return null;
            }
            else
            {
                return "Error al registrar la Acción Inmediata: " + registrado;
            }
        }



        protected void fvEvaluarEvento_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetallePersona"))
            {
                if (e.CommandArgument != null)
                {
                    string rut = Convert.ToString(e.CommandArgument);
                    Session["RutDetalle"] = rut;
                    Session["PreviousPagePersona"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Personas/DetallePersona.aspx", true);
                }
            }
        }


        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            hfConfirmVolver_ModalPopupExtender.Show();
            uPanel.Update();
        }

        protected void btMessageAceptar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Registros/ListarAccionesInmediatasPendientes.aspx", true);
        }


        private void showBuscarPersonaMessage(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span>" + msg + "</span>";
        }


        private void showBuscarPersonaMessageSuccess(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #339933;\">" + msg + "</span>";
        }


        private void showBuscarPersonaMessageError(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #FF0000;\">" + msg + "</span>";
        }


        protected void btBuscarPersonaLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarPersonaApellido.Text = "";
        }



        protected void SDSBuscarPersonas_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                showBuscarPersonaMessage(Convert.ToString(e.AffectedRows) + " registros coinciden con la búsqueda");
            else
                showBuscarPersonaMessage("No se han encontrado coincidencias con la búsqueda");
        }



        private static DataTable getDTPersonasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("RUT");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Centro");
            dt.Columns.Add("Cargo");
            dt.Columns.Add("Antiguedad");
            dt.Columns.Add("Opciones");

            dt.AcceptChanges();

            return dt;
        }



        protected void gvBuscarPersonas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";

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

            if (e.CommandName.Equals("AddInvolucrado"))
            {
                List<PersonaInfo> listInvolucrados = getListInvolucrados();
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                if (isPersonaInList(listInvolucrados, rut))
                {
                    showBuscarPersonaMessageError("La Persona \"" + nombre + "\" ya existe en la lista de Involucrados");
                    return;
                }

                string nombre_centro = gvBuscarPersonas.Rows[index].Cells[4].Text;
                string cargo = gvBuscarPersonas.Rows[index].Cells[5].Text;
                int antiguedad = Convert.ToInt32(gvBuscarPersonas.Rows[index].Cells[6].Text);

                PersonaInfo p = new PersonaInfo(rut, nombre, nombre_centro, cargo, antiguedad);
                listInvolucrados.Add(p);

                saveListInvolucrados(listInvolucrados);
                showBuscarPersonaMessageSuccess("Se ha ingresado a \"" + nombre + "\" a la lista de Involucrados");

                updateGVInvolucrados();
                upInvolucrados.Update();
            }


            txtBuscarPersonaApellido.Text = "";
            //ltBuscarPersonaSummary.Text = "";

            //hfBuscarPersona_ModalPopupExtender.Hide();
        }


        protected void btBuscarPersonaCerrar_Click(object sender, EventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";
            hfBuscarPersona_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        private void saveListInvolucrados(List<PersonaInfo> listInvolucrados)
        {
            Session["listInvolucrados"] = listInvolucrados;
        }



        private bool loadInvolucradosEvaluacion(string codigo_evento)
        {
            
            List<PersonaInfo> listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
            if (listInvolucrados == null)
            {
                return false;
            }

            DataTable dt = getDTPersonasFormat();

            DataRow dr;

            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Button();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvInvolucradosEvaluacion.DataSource = dt;
            gvInvolucradosEvaluacion.DataBind();


            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                ((Label)gvInvolucradosEvaluacion.Rows[i].FindControl("lbRUT")).Text = listInvolucrados[i].getRut();
                ((Label)gvInvolucradosEvaluacion.Rows[i].FindControl("lbNombre")).Text = listInvolucrados[i].getNombre();
                ((Label)gvInvolucradosEvaluacion.Rows[i].FindControl("lbNombreCentro")).Text = listInvolucrados[i].getNombreCentro();
                ((Label)gvInvolucradosEvaluacion.Rows[i].FindControl("lbCargo")).Text = listInvolucrados[i].getCargo();
                ((Label)gvInvolucradosEvaluacion.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(listInvolucrados[i].getAntiguedad());
            }

            return true;
        }


        private void updateGVInvolucrados()
        {
            DataTable dt = getDTPersonasFormat();
            List<PersonaInfo> listInvolucrados = getListInvolucrados();

            DataRow dr;

            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Button();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvInvolucrados.DataSource = dt;
            gvInvolucrados.DataBind();


            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                ((Label)gvInvolucrados.Rows[i].FindControl("lbRUT")).Text = listInvolucrados[i].getRut();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombre")).Text = listInvolucrados[i].getNombre();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombreCentro")).Text = listInvolucrados[i].getNombreCentro();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbCargo")).Text = listInvolucrados[i].getCargo();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(listInvolucrados[i].getAntiguedad());
            }
        }


        protected void gvInvolucrados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("BuscarInvolucrado"))
            {
                ((ButtonField)gvBuscarPersonas.Columns[0]).CommandName = "AddInvolucrado";
                hfBuscarPersona_ModalPopupExtender.Show();
                return;

            }

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

            List<PersonaInfo> listInvolucrados = getListInvolucrados();
            if (index < listInvolucrados.Count)
            {
                listInvolucrados.RemoveAt(index);

                saveListInvolucrados(listInvolucrados);
                updateGVInvolucrados();
            }
        }


        private List<PersonaInfo> getListInvolucrados()
        {
            List<PersonaInfo> listInvolucrados;

            if (Session["listInvolucrados"] == null)
                listInvolucrados = new List<PersonaInfo>();
            else
                listInvolucrados = (List<PersonaInfo>)Session["listInvolucrados"];

            return listInvolucrados;
        }


        private bool isPersonaInList(List<PersonaInfo> listPersona, string rut)
        {
            foreach (PersonaInfo p in listPersona)
            {
                if (p.getRut().Equals(rut))
                    return true;
            }

            return false;
        }



        protected void btConfirmAccionInmediataNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void btConfirmAccionInmediataSi_Click(object sender, EventArgs e)
        {
            string status = registrarNuevaAccionInmediata();
            if (status == null)
            {
                Session.Remove("listArchivos");

                lbMessage.Text = "La Acción Inmediata se registró satisfactoriamente para el Evento con código <b>" + hfCodigoEvento.Value + "</b>";

                hfConfirmAccionInmediata_ModalPopupExtender.Hide();
                hfMessage_ModalPopupExtender.Show();
                uPanel.Update();
            }
            else
            {
                showMessageError(status);
            }
        }



        protected void btConfirmVolverSi_Click(object sender, EventArgs e)
        {
            Session.Remove("p");
            Session.Remove("RutDetalle");
            Session.Remove("listArchivos");
            Session.Remove("listInvolucrados");
            Session.Remove("Evaluacion");

            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }

        protected void btConfirmVolverNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


    }

}