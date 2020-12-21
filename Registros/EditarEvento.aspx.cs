using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;
using System.IO;

namespace NCCSAN.Registros
{
    public partial class EditarEvento : System.Web.UI.Page
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

                    string id_centro = u.getIDCentro();
                    if ((id_centro.Equals("CSAN")) || (id_centro.Equals("CMSFM")) || (id_centro.Equals("CRCAR")))
                    {
                        lbAsteriskSerieEquipo.Visible = false;
                        txtAgenteCorrector.Enabled = false;
                    }
                    else if (id_centro.Equals("CSAR"))
                    {
                        lbAsteriskSerieEquipo.Visible = true;
                        txtAgenteCorrector.Enabled = true;
                    }
                }

                Session.Remove("listArchivos");

                if ((Session["CodigoEventoSeleccionado"] != null) && (Session["PreviousPage"] != null))
                {
                    string codigo = (string)Session["CodigoEventoSeleccionado"];
                    string previuosPage = (string)Session["PreviousPage"];

                    if ((codigo != null) && (previuosPage != null))
                    {
                        hfCodigoEvento.Value = codigo;
                        hfPreviousPage.Value = previuosPage;

                        Evento evento = LogicController.getEvento(codigo);
                        if (evento != null)
                        {
                            if (!loadProbabilidadConsecuencia())
                            {
                                string msg = "Error al recuperar información del Usuario";
                                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                            }

                            Session["Evento"] = evento;
                            loadEvento();
                        }
                        else
                        {
                            string msg = "Error al recuperar el Evento";
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


        private Evento getEvento()
        {
            if (Session["Evento"] != null)
            {
                Evento evento = (Evento)Session["Evento"];

                return evento;
            }
            else
            {
                return null;
            }
        }


        private void loadEvento()
        {
            Evento evento = getEvento();
            if (evento == null)
                return;

            txtCodigo.Text = evento.getCodigo();
            txtWO.Text = evento.getWO();
            txtFecha.Text = evento.getFecha().ToShortDateString().Replace("/", "-");
            ddlCliente.Text = evento.getNombreCliente();
            ddlTipo.Text = evento.getTipoEquipo();
            SDSModelo.DataBind();
            txtSerieEquipo.Text = evento.getSerieEquipo();
            ddlSistema.Text = evento.getNombreSistema();
            if (evento.getSerieComponente() != null)
            {
                txtSerieComponente.Text = evento.getSerieComponente();
            }

            if (evento.getParte() != null)
            {
                txtParte.Text = evento.getParte();
            }

            if (evento.getNumeroParte() != null)
            {
                txtNumeroParte.Text = evento.getNumeroParte();
            }

            if (evento.getHoras() >= 0)
            {
                txtHoras.Text = Convert.ToString(evento.getHoras());
            }

            ddlFuente.Text = evento.getNombreFuente();

            if (evento.getNombreArea() != null)
            {
                ddlArea.Text = evento.getNombreArea();
                SDSSubarea.DataBind();
            }

            ddlClasificacion.Text = evento.getNombreClasificacion();
            SDSSubclasificacion.DataBind();

            if (evento.getCriticidad() != null)
            {
                ddlProbabilidad.SelectedIndex = findProbabilidadIndex(evento.getProbabilidad());
                ddlConsecuencia.SelectedIndex = findConsecuenciaIndex(evento.getConsecuencia());

                txtIRC.Text = Convert.ToString(evento.getIRC());
                txtCriticidad.Text = evento.getCriticidad();
            }

            txtDetalle.Text = evento.getDetalle();

            List<Archivo> listArchivos = LogicController.getArchivosEvento(evento.getCodigo());
            setListArchivos(listArchivos);
            updateGVArchivos();
        }


        private int findProbabilidadIndex(string probabilidad)
        {
            for (int i = 0; i < ddlProbabilidad.Items.Count; i++)
                if (ddlProbabilidad.Items[i].Text.Equals(probabilidad))
                    return i;

            return 0;
        }


        private int findConsecuenciaIndex(string consecuencia)
        {
            for (int i = 0; i < ddlConsecuencia.Items.Count; i++)
                if (ddlConsecuencia.Items[i].Text.Equals(consecuencia))
                    return i;

            return 0;
        }



        private bool loadProbabilidadConsecuencia()
        {
            if (Session["usuario"] == null)
            {
                return false;
            }

            Usuario usuario = (Usuario)Session["usuario"];

            ddlProbabilidad.Items.Add(new ListItem("Seleccione..", ""));
            ddlProbabilidad.Items.Add(new ListItem("1 - Insignificante", "1"));
            ddlProbabilidad.Items.Add(new ListItem("2 - Baja", "2"));
            ddlProbabilidad.Items.Add(new ListItem("3 - Media", "3"));
            ddlProbabilidad.Items.Add(new ListItem("4 - Alta", "4"));


            ddlConsecuencia.Items.Add(new ListItem("Seleccione..", ""));
            ddlConsecuencia.Items.Add(new ListItem("1 - Menor", "1"));
            ddlConsecuencia.Items.Add(new ListItem("2 - Serio", "2"));
            ddlConsecuencia.Items.Add(new ListItem("10 - Mayor", "10"));
            ddlConsecuencia.Items.Add(new ListItem("100 - Grave", "100"));
            ddlConsecuencia.Items.Add(new ListItem("1000 - Catastrófico", "1000"));


            return true;
        }



        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            if (ddlCliente.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlCliente.Items.Insert(0, li);
            }
            else
            {
                ddlCliente.Items.Clear();
            }
        }


        protected void ddlTipo_DataBound(object sender, EventArgs e)
        {
            if (ddlTipo.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlTipo.Items.Insert(0, li);
            }
            else
            {
                ddlTipo.Items.Clear();
            }
        }


        protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlModelo.Items.Clear();
            ddlSistema.Items.Clear();
            ddlSubsistema.Items.Clear();
            ddlComponente.Items.Clear();
        }


        protected void ddlModelo_DataBound(object sender, EventArgs e)
        {
            if (ddlModelo.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlModelo.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlModelo.Text = evento.getModeloEquipo();
                        SDSSistema.DataBind();
                    }
                }
            }
            else
            {
                ddlModelo.Items.Clear();
            }
        }


        protected void ddlSistema_DataBound(object sender, EventArgs e)
        {
            if (ddlSistema.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlSistema.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlSistema.Text = evento.getNombreSistema();
                        SDSSubsistema.DataBind();
                    }
                }
            }
            else
            {
                ddlSistema.Items.Clear();
            }
        }

        protected void ddlSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubsistema.Items.Clear();
            ddlComponente.Items.Clear();
        }

        protected void ddlSubsistema_DataBound(object sender, EventArgs e)
        {
            if (ddlSubsistema.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlSubsistema.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlSubsistema.Text = evento.getNombreSubsistema();
                        SDSComponente.DataBind();
                    }
                }
            }
            else
            {
                ddlSubsistema.Items.Clear();
            }
        }


        protected void ddlSubsistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlComponente.Items.Clear();
        }


        protected void ddlComponente_DataBound(object sender, EventArgs e)
        {
            if (ddlComponente.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlComponente.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlComponente.Text = evento.getNombreComponente();
                    }
                }
            }
            else
            {
                ddlComponente.Items.Clear();
            }
        }


        protected void ddlFuente_DataBound(object sender, EventArgs e)
        {
            ddlFuente.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlSubarea_DataBound(object sender, EventArgs e)
        {
            ddlSubarea.Items.Insert(0, new ListItem("Seleccione..", ""));

            if (!IsPostBack)
            {
                Evento evento = getEvento();
                if (evento != null)
                {
                    ddlSubarea.Text = evento.getNombreSubarea();
                }
            }
        }

        protected void ddlClasificacion_DataBound(object sender, EventArgs e)
        {
            ddlClasificacion.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlSubclasificacion_DataBound(object sender, EventArgs e)
        {
            ddlSubclasificacion.Items.Insert(0, new ListItem("Seleccione..", ""));

            if (!IsPostBack)
            {
                Evento evento = getEvento();
                if (evento != null)
                {
                    ddlSubclasificacion.Text = evento.getNombreSubclasificacion();
                }
            }
        }

        protected void ddlProbabilidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarCriticidad();
        }

        protected void ddlConsecuencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarCriticidad();
        }


        private double getIRC()
        {
            if ((ddlProbabilidad.SelectedIndex > 0) && (ddlConsecuencia.SelectedIndex > 0))
            {
                int probabilidad = Convert.ToInt32(ddlProbabilidad.SelectedItem.Value);
                int consecuencia = Convert.ToInt32(ddlConsecuencia.SelectedItem.Value);

                double irc = probabilidad * consecuencia;

                if (ddlFuente.SelectedValue.ToUpper().Equals("RECLAMO DE CLIENTE"))
                {
                    irc = irc * (1.2);
                }

                return irc;
            }
            else
            {
                return -1;
            }
        }


        private string getCriticidad(double irc)
        {
            if (irc < 1)
            {
                return null;
            }
            else if (irc <= 4)
            {
                return "No crítico";
            }
            else if (irc < 10)
            {
                return "Medianamente crítico";
            }
            else if (irc <= 300)
            {
                return "Altamente crítico";
            }
            else
            {
                return "Super crítico";
            }
        }


        private void mostrarCriticidad()
        {
            double irc = getIRC();
            if (irc > 0)
            {
                txtIRC.Text = Convert.ToString(irc);

                string criticidad = getCriticidad(irc);

                if (criticidad != null)
                {
                    txtCriticidad.Text = criticidad;
                }
                else
                {
                    txtCriticidad.Text = "--";
                }

            }
            else
            {
                txtIRC.Text = "--";
                txtCriticidad.Text = "--";
            }
        }


        private List<string> validatePanelEvento()
        {
            List<string> errors = new List<string>();

            if (txtWO.Text.Length < 1)
            {
                errors.Add("No se indicó la Orden de Trabajo (W/O)");
            }

            if (ddlCliente.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Cliente");
            }

            if (ddlTipo.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el tipo de Equipo");
            }

            if (ddlModelo.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el modelo del Equipo");
            }


            if (Session["id_centro"] == null)
            {
                errors.Add("No se puede recuperar el Centro");
            }
            else
            {
                string id_centro = (string)Session["id_centro"];

                if (id_centro.Equals("CSAR"))
                {
                    if (txtSerieEquipo.Text.Length < 1)
                    {
                        errors.Add("Se debe ingresar la serie del Equipo");
                    }
                }
            }


            if (ddlSistema.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el sistema del Componente");
            }

            if (ddlSubsistema.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el subsistema del Componente");
            }

            if (ddlComponente.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Componente");
            }


            if (txtHoras.Text.Length > 0)
            {
                if (!Utils.validateNumber(txtHoras.Text))
                {
                    errors.Add("La cantidad de horas de trabajo debe ser un valor numérico");
                }
            }

            if (!Utils.validateFecha(txtFecha.Text))
            {
                errors.Add("No se indicó la fecha en que ocurrió el Evento");
            }

            if (ddlFuente.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la fuente de detección");
            }

            if (ddlArea.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Área");
            }

            if (ddlSubarea.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Sub-área");
            }

            if (ddlClasificacion.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la clasificación para el evento");
            }

            if (ddlSubclasificacion.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar una subclasificación para el Evento");
            }

            if (txtDetalle.Text.Length < 1)
            {
                errors.Add("No se detalló lo ocurrido");
            }


            if (ddlProbabilidad.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la probabilidad de ocurrencia");
            }


            if (ddlConsecuencia.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la consecuencia del Evento");
            }

            double irc = getIRC();
            if ((irc >= 0) && (irc < 10) && (!ddlFuente.SelectedValue.ToUpper().Equals("RECLAMO DE CLIENTE")))
            {
                if (ddlClasificacion.SelectedValue.Equals("*Sin especificar"))
                {
                    errors.Add("El tipo de Evento no permite una Clasificación \"*Sin especificar\"");
                }

                if (ddlSubclasificacion.SelectedValue.Equals("*Sin especificar"))
                {
                    errors.Add("El tipo de Evento no permite una Sub-clasificación \"*Sin especificar\"");
                }
            }

            return errors;
        }


        protected void ibMatrizConsecuencia_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                showMessageError("Error al recuperar el Centro de servicios");

                return;
            }

            string centro = (string)Session["id_centro"];
            imgMatrizConsecuencia.ImageUrl = ResolveClientUrl("~/Images/MatrizConsecuencia/" + centro + ".png");
            hfMatrizConsecuencia_ModalPopupExtender.Show();
        }

        protected void btCerrarMatrizConsecuencia_Click(object sender, EventArgs e)
        {
            hfMatrizConsecuencia_ModalPopupExtender.Hide();

            uPanel.Update();
        }

        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            hfConfirmVolver_ModalPopupExtender.Show();
        }



        protected void ibUpdateEvento_Click(object sender, ImageClickEventArgs e)
        {
            Evento evento = getEvento();

            if (evento == null)
            {
                showMessageError("El Evento que se está intentando editar no existe");

                return;
            }

            List<string> errors = validatePanelEvento();
            if (errors.Count == 0)
            {
                int tipo_evento = Utils.getEventoType(evento.getNombreFuente(), evento.getIRC());
                if (tipo_evento < 0)
                {
                    showMessageError("No se puede comprobar el tipo de Evento");

                    return;
                }


                int nuevo_tipo_evento = Utils.getEventoType(ddlFuente.SelectedValue, getIRC());
                if (nuevo_tipo_evento < 0)
                {
                    showMessageError("No se puede comprobar el tipo de Evento");

                    return;
                }



                if (tipo_evento == 1) //1: No conformidad
                {
                    if (nuevo_tipo_evento == 1)
                    {
                        //Sigue siendo no conformidad
                        lbMessageConfirmEdicion.Text = "Se modificarán los datos editados del Evento. ¿Desea guardar los cambios?";
                    }
                    else if (nuevo_tipo_evento == 0)
                    {
                        //Se eliminará la investigación y tendra estado "Acción inmediata pendiente"
                        int investigacion_exists = LogicController.investigacionExists(evento.getCodigo());
                        if (investigacion_exists < 0)
                        {
                            showMessageError("No se pueden comprobar los documentos asociados al Evento");

                            return;
                        }

                        if (investigacion_exists == 1)
                        {
                            //Se confirma la eliminación de la Investigación para registrarlo como Hallazgo
                            lbMessageConfirmEdicion.Text = "El Evento tiene una Investigación asociada y se eliminará ya que ahora clasifica como Hallazgo. ¿Desea continuar?";
                        }
                        else if (investigacion_exists == 0)
                        {
                            //No tiene Investigación, por tanto sólo se cambia el estado a "Acción inmediata pendiente"
                            lbMessageConfirmEdicion.Text = "Se modificarán los datos editados del Evento. ¿Desea guardar los cambios?";
                        }
                    }

                }
                else if (tipo_evento == 0) //0: Hallazgo
                {
                    if (nuevo_tipo_evento == 1)
                    {
                        //Ahora es No conformidad y queda en estado "Investigación pendiente"
                        lbMessageConfirmEdicion.Text = "Se modificarán los datos editados del Evento. ¿Desea guardar los cambios?";
                    }
                    else if (nuevo_tipo_evento == 0)
                    {
                        //Sigue siendo hallazgo
                        lbMessageConfirmEdicion.Text = "Se modificarán los datos editados del Evento. ¿Desea guardar los cambios?";
                    }
                }


                hfConfirmEdicion_ModalPopupExtender.Show();
            }
            else
            {
                showSummary(errors);
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



        protected void btConfirmEdicionNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void btConfirmEdicionSi_Click(object sender, EventArgs e)
        {
            string status = updateEvento();
            if (status == null)
            {
                Session.Remove("listArchivos");
                Session.Remove("Evento");

                lbMessage.Text = "El Evento se modifió satisfactoriamente";
                hfConfirmEdicion_ModalPopupExtender.Hide();
                hfMessage_ModalPopupExtender.Show();

                uPanel.Update();
            }
            else
            {
                showMessageError(status);
            }
        }


        protected void gvArchivos_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (e.CommandName.Equals("DescargarArchivo"))
            {
                index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivos.Rows.Count))
                {
                    List<Archivo> listArchivos = getListArchivos();
                    if (listArchivos == null)
                    {
                        showMessageError("Se ha producido un error al recuperar la lista de archivos");

                        return;
                    }

                    if (index >= listArchivos.Count)
                    {
                        showMessageError("Se ha producido un error al recuperar el archivo");

                        return;
                    }


                    Archivo archivo = listArchivos[index];
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
                    List<Archivo> listArchivos = getListArchivos();
                    if (index < listArchivos.Count)
                    {
                        string filename = listArchivos[index].getNombre();
                        listArchivos.RemoveAt(index);
                        setListArchivos(listArchivos);

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


        private List<Archivo> getListArchivos()
        {
            List<Archivo> listArchivos;

            if (Session["listArchivos"] != null)
                listArchivos = (List<Archivo>)Session["listArchivos"];
            else
            {
                listArchivos = new List<Archivo>();
                setListArchivos(listArchivos);
            }

            return listArchivos;
        }


        private void setListArchivos(List<Archivo> listArchivo)
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
                List<Archivo> listArchivos = getListArchivos();
                try
                {
                    byte[] contentFile;
                    BinaryReader binaryReader;
                    HttpFileCollection hfc = Request.Files;
                    HttpPostedFile hpf;
                    string fileType;
                    string contentType;
                    string fileName;
                    string ext;
                    int max_file_size;
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = hfc[i];

                        fileName = Path.GetFileName(hpf.FileName);
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

                setListArchivos(listArchivos);

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

            List<Archivo> listArchivos = getListArchivos();

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



        private string updateEvento()
        {
            Evento evento = getEvento();
            if (evento == null)
            {
                return "Error al recuperar el Evento";
            }


            int tipo_evento;
            int nuevo_tipo_evento;

            if(evento.getCriticidad() != null)
            {
                tipo_evento = Utils.getEventoType(evento.getNombreFuente(), evento.getIRC());
                nuevo_tipo_evento = Utils.getEventoType(ddlFuente.SelectedValue, getIRC());

                if (tipo_evento < 0)
                {
                    return "Error comprobar el tipo de Evento";
                }

                if (nuevo_tipo_evento < 0)
                {
                    return "Error comprobar el nuevo tipo de Evento";
                }
            }
            else
            {
                tipo_evento = 2;
                nuevo_tipo_evento = 2;
            }


            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];


            int horas;
            if (txtHoras.Text.Length > 0)
            {
                if (!Utils.validateNumber(txtHoras.Text))
                {
                    return "La cantidad de horas de trabajo debe ser numérica";
                }

                horas = Convert.ToInt32(txtHoras.Text);
                if (horas < 0)
                {
                    return "La cantidad de horas de trabajo debe ser un número positivo";
                }
            }
            else
            {
                horas = -1;
            }


            double irc = getIRC();
            if (irc < 1)
            {
                return "Error al calcular el IRC";
            }

            string criticidad = getCriticidad(irc);
            if (criticidad == null)
            {
                return "Error al calcular la criticidad";
            }

            List<Archivo> listArchivos = getListArchivos();
            if (listArchivos == null)
            {
                return "Error al recuperar la lista de archivos adjuntos";
            }


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }


            Usuario usuario = (Usuario)Session["usuario"];


            string status = LogicController.updateEvento
                                (
                                    evento.getCodigo(),
                                    txtWO.Text,
                                    txtFecha.Text,
                                    ddlCliente.Text,
                                    id_centro,
                                    ddlArea.Text,
                                    ddlSubarea.Text,
                                    ddlFuente.Text,
                                    ddlModelo.Text,
                                    ddlTipo.Text,
                                    txtSerieEquipo.Text,
                                    ddlSistema.Text,
                                    ddlSubsistema.Text,
                                    ddlComponente.Text,
                                    txtSerieComponente.Text,
                                    txtParte.Text,
                                    txtNumeroParte.Text,
                                    horas,
                                    ddlClasificacion.Text,
                                    ddlSubclasificacion.Text,
                                    txtAgenteCorrector.Text,
                                    txtDetalle.Text,
                                    ddlProbabilidad.SelectedItem.Text,
                                    ddlConsecuencia.SelectedItem.Text,
                                    getIRC(),
                                    txtCriticidad.Text,
                                    getListArchivos(),
                                    tipo_evento,
                                    nuevo_tipo_evento,
                                    usuario.getUsuario(),
                                    Request.ServerVariables["REMOTE_ADDR"]
                                );

            return status;
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

        protected void btMessageAceptar_Click(object sender, EventArgs e)
        {
            Session.Remove("listArchivos");
            Session.Remove("Evento");

            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }


        protected void btConfirmVolverSi_Click(object sender, EventArgs e)
        {
            Session.Remove("listArchivos");
            Session.Remove("Evento");

            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }

        protected void btConfirmVolverNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }

        protected void ddlFuente_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarCriticidad();
        }


    }
}