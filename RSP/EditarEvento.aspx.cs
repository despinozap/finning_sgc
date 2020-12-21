using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.IO;
using System.Data;

namespace NCCSAN.RSP
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

                    /*
                    string id_centro = u.getIDCentro();
                    if ((id_centro.Equals("CSAN")) || (id_centro.Equals("CMSFM")) || (id_centro.Equals("CRCAR")))
                    {
                        lbAsteriskSerieEquipo.Visible = false;
                        lbAsteriskSerieComponente.Visible = true;
                    }
                    else if (id_centro.Equals("CSAR"))
                    {
                        lbAsteriskSerieEquipo.Visible = true;
                        lbAsteriskSerieComponente.Visible = false;
                    }
                    */
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

            txtWO.Text = evento.getWO();
            txtFecha.Text = evento.getFecha().ToShortDateString().Replace("/", "-");
            ddlCentro.SelectedValue = evento.getIDCentro();
            SDSCliente.DataBind();
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


            ddlClasificacion.Text = evento.getNombreClasificacion();
            SDSSubclasificacion.DataBind();

            txtDetalle.Text = evento.getDetalle();

            List<Archivo> listArchivos = LogicController.getArchivosEvento(evento.getCodigo());
            setListArchivos(listArchivos);
            updateGVArchivos();
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



        protected void ddlClasificacion_DataBound(object sender, EventArgs e)
        {
            if (ddlClasificacion.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlClasificacion.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlClasificacion.Text = evento.getNombreClasificacion();
                        SDSSubclasificacion.DataBind();
                    }
                }
            }
            else
            {
                ddlClasificacion.Items.Clear();
            }
        }


        protected void ddlSubclasificacion_DataBound(object sender, EventArgs e)
        {
            if (ddlSubclasificacion.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlSubclasificacion.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlSubclasificacion.Text = evento.getNombreSubclasificacion();
                    }
                }
            }
            else
            {
                ddlSubclasificacion.Items.Clear();
            }
        }


        private List<string> validatePanelEvento()
        {
            List<string> errors = new List<string>();

            if (ddlCentro.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Centro");
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

            if (txtSerieComponente.Text.Length < 1)
            {
                errors.Add("Se debe indicar la serie del Componente");
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


            return errors;
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
                //Sigue siendo no conformidad
                lbMessageConfirmEdicion.Text = "Se modificarán los datos editados del Evento. ¿Desea guardar los cambios?";


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
            }
            else
            {
                hfConfirmEdicion_ModalPopupExtender.Hide();

                showMessageError(status);
            }

            uPanel.Update();
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


            int tipo_evento = Utils.getEventoType(evento.getNombreFuente(), evento.getIRC());
            if (tipo_evento < 0)
            {
                return "Error comprobar el tipo de Evento";
            }


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

            string status = LogicController.updateEvento_Cliente
                                                                (
                                                                    evento.getCodigo(),
                                                                    txtWO.Text,
                                                                    txtFecha.Text,
                                                                    ddlCliente.SelectedItem.Value,
                                                                    ddlCentro.SelectedItem.Value,
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
                                                                    txtDetalle.Text,
                                                                    getListArchivos(),
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

        protected void ddlCentro_DataBound(object sender, EventArgs e)
        {
            ddlCentro.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlCentro_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCliente.Items.Clear();
            SDSCliente.DataBind();
            ddlCliente.DataBind();

            ddlClasificacion.Items.Clear();
            SDSClasificacion.DataBind();
            ddlClasificacion.DataBind();

            ddlSubclasificacion.Items.Clear();
        }

        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            if (ddlCliente.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlCliente.Items.Insert(0, li);

                if (!IsPostBack)
                {
                    Evento evento = getEvento();
                    if (evento != null)
                    {
                        ddlCliente.Text = evento.getNombreCliente();
                    }
                }
            }
            else
            {
                ddlCliente.Items.Clear();
            }
        }
    }
}