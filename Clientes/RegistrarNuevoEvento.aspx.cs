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

namespace NCCSAN.Clientes
{
    public partial class RegistrarNuevoEvento : System.Web.UI.Page
    {

        private static readonly int PANEL_COMPONENTE = 0;
        private static readonly int PANEL_DESVIACION = 1;
        private static readonly int PANEL_ARCHIVOS = 2;


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

                showPanel(PANEL_COMPONENTE);
                Session.Remove("listArchivos");
                updateGVArchivos();


                WOInfo woinfo = getWOInfo();
                if (woinfo != null)
                {
                    txtWO.Text = woinfo.getCodigoWO();
                    if (woinfo.getSerieEquipo() != null)
                    {
                        txtSerieEquipo.Text = woinfo.getSerieEquipo();
                    }

                    if (woinfo.getSerieComponente() != null)
                    {
                        txtSerieComponente.Text = woinfo.getSerieComponente();
                    }
                }
            }
            else
            {
                //AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "setControlsStyle", "<script type=\"text/javascript\">setControlsStyle();</script>", false);
            }

        }


        private void removeWOInfo()
        {
            Session.Remove("woinfo");
        }

        private void setWOInfo(WOInfo woInfo)
        {
            Session["woinfo"] = woInfo;
        }


        private WOInfo getWOInfo()
        {
            if (Session["woinfo"] != null)
            {
                return (WOInfo)Session["woinfo"];
            }
            else
            {
                return null;
            }
        }


        private void loadWOInfo()
        {
            WOInfo woInfo = getWOInfo();
            if (woInfo != null)
            {
                txtWO.Text = woInfo.getCodigoWO();
                if (ddlCentro.Items.Contains(new ListItem(woInfo.getNombreCentro(), woInfo.getIDCentro())))
                {
                    ddlCentro.SelectedValue = woInfo.getIDCentro();
                }

                if (ddlTipo.Items.Contains(new ListItem(woInfo.getTipoEquipo(), woInfo.getTipoEquipo())))
                {
                    ddlTipo.SelectedValue = woInfo.getTipoEquipo();
                }

                txtSerieEquipo.Text = woInfo.getSerieEquipo();
                txtSerieComponente.Text = woInfo.getSerieComponente();
            }
        }


        private void clearForm()
        {
            ddlCentro.SelectedIndex = 0;
            ddlTipo.SelectedIndex = 0;
            ddlModelo.Items.Clear();
            ddlModelo.DataBind();
            txtSerieEquipo.Text = "";
            ddlSistema.Items.Clear();
            ddlSistema.DataBind();
            ddlSubsistema.Items.Clear();
            ddlComponente.Items.Clear();
            txtSerieComponente.Text = "";
        }

        private void showPanel(int indexPanel)
        {
            ltSummary.Text = "";

            pnComponente.Visible = false;
            pnDesviacion.Visible = false;
            pnArchivos.Visible = false;

            switch (indexPanel)
            {
                case 0:
                    {
                        pnComponente.Visible = true;
                        break;
                    }

                case 1:
                    {
                        pnDesviacion.Visible = true;
                        break;
                    }

                case 2:
                    {
                        pnArchivos.Visible = true;
                        uPanel.Update();
                        break;
                    }
            }

        }


        protected void ddlTipo_DataBound(object sender, EventArgs e)
        {
            if (ddlTipo.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlTipo.Items.Insert(0, li);


                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlTipo.Items.Contains(new ListItem(woInfo.getTipoEquipo(), woInfo.getTipoEquipo())))
                    {
                        ddlTipo.SelectedValue = woInfo.getTipoEquipo();
                    }
                }
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

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlModelo.Items.Contains(new ListItem(woInfo.getModeloEquipo(), woInfo.getModeloEquipo())))
                    {
                        ddlModelo.SelectedValue = woInfo.getModeloEquipo();
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

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlSistema.Items.Contains(new ListItem(woInfo.getNombreSistema(), woInfo.getNombreSistema())))
                    {
                        ddlSistema.SelectedValue = woInfo.getNombreSistema();
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

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlSubsistema.Items.Contains(new ListItem(woInfo.getNombreSubsistema(), woInfo.getNombreSubsistema())))
                    {
                        ddlSubsistema.SelectedValue = woInfo.getNombreSubsistema();
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
            ddlComponente.DataBind();
        }


        protected void ddlComponente_DataBound(object sender, EventArgs e)
        {
            if (ddlComponente.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlComponente.Items.Insert(0, li);

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlComponente.Items.Contains(new ListItem(woInfo.getNombreComponente(), woInfo.getNombreComponente())))
                    {
                        hfNombreComponente.Value = woInfo.getNombreComponente();

                        removeWOInfo();
                    }
                }
                else if (hfNombreComponente.Value.Length > 0)
                {
                    if (ddlComponente.Items.Contains(new ListItem(hfNombreComponente.Value, hfNombreComponente.Value)))
                    {
                        ddlComponente.SelectedValue = hfNombreComponente.Value;
                    }
                }
            }
            else
            {
                ddlComponente.Items.Clear();
            }
        }


        protected void ibNextComponente_Click(object sender, ImageClickEventArgs e)
        {

            List<string> errors = validatePanelComponente();
            if (errors.Count == 0)
            {
                showPanel(PANEL_DESVIACION);
            }
            else
            {
                showSummary(errors);
            }

        }


        protected void ibPreviousDesviacion_Click(object sender, ImageClickEventArgs e)
        {
            showPanel(PANEL_COMPONENTE);
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


        private List<string> validatePanelComponente()
        {
            List<string> errors = new List<string>();

            if (ddlCentro.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Centro");
            }

            if (ddlTipo.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el tipo de Equipo");
            }

            if (ddlModelo.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el modelo del Equipo");
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

            if (Session["id_centro"] == null)
            {
                errors.Add("No se puede recuperar el Centro");
            }


            return errors;
        }


        private List<string> validatePanelDesviacion()
        {
            List<string> errors = new List<string>();

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
                errors.Add("Se debe seleccionar una clasificación para el evento");
            }

            if (ddlSubclasificacion.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar una Sub-clasificación para el Evento");
            }

            if (txtDetalle.Text.Length < 1)
            {
                errors.Add("No se detalló lo ocurrido");
            }


            return errors;
        }


        private List<string> validatePanelArchivos()
        {
            List<string> errors = new List<string>();

            /*ACEPTA REGISTRAR EVENTOS SIN ARCHIVOS COMO EVIDENCIA OBLIGATORIA
            if (getListaArchivos().Count < 1)
            {
                errors.Add("No se han seleccionado los archivos adjuntos");
            }
            */

            return errors;
        }


        protected void ddlClasificacion_DataBound(object sender, EventArgs e)
        {
            ddlClasificacion.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlSubclasificacion_DataBound(object sender, EventArgs e)
        {
            ddlSubclasificacion.Items.Insert(0, new ListItem("Seleccione..", ""));
        }



        protected void ibNextDesviacion_Click(object sender, ImageClickEventArgs e)
        {
            List<string> errors = validatePanelDesviacion();
            if (errors.Count == 0)
            {
                showPanel(PANEL_ARCHIVOS);
            }
            else
            {
                showSummary(errors);
            }

        }


        private void clearPanelComponente()
        {
            txtWO.Text = "";

            ddlCentro.Items.Clear();
            ddlCentro.DataBind();

            ddlTipo.Items.Clear();
            ddlTipo.DataBind();

            ddlModelo.Items.Clear();
            ddlModelo.DataBind();

            txtSerieEquipo.Text = "";

            ddlSistema.Items.Clear();
            ddlSistema.DataBind();

            ddlSubsistema.Items.Clear();
            ddlSubsistema.DataBind();

            ddlComponente.Items.Clear();
            ddlComponente.DataBind();

            txtSerieComponente.Text = "";
        }


        private void clearPanelDesviacion()
        {
            txtParte.Text = "";
            txtNumeroParte.Text = "";
            txtHoras.Text = "";
            txtFecha.Text = "";

            ddlClasificacion.Items.Clear();
            ddlClasificacion.DataBind();

            ddlSubclasificacion.Items.Clear();
            ddlSubclasificacion.DataBind();

            txtDetalle.Text = "";


            ltSummary.Text = "";
        }



        private string registrarEvento()
        {
            string error = validatePanelsNuevoRegistro();
            if (error != null)
            {
                return error;
            }

            if (Session["nombre_cliente"] == null)
            {
                return "Error al recuperar la información de cliente";
            }

            string nombre_cliente = (string)Session["nombre_cliente"];

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


            List<Archivo> listArchivos = getListaArchivos();
            if (listArchivos == null)
            {
                return "Error al recuperar la lista de archivos adjuntos";
            }


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }

            Usuario usuario = (Usuario)Session["usuario"];
            PersonaInfo owner = LogicController.getPersonaInfo(usuario.getRutPersona());
            if (owner == null)
            {
                return "Error al recuperar tu información";
            }


            string registrado = LogicController.registerEvento_Cliente
                                                                        (
                                                                            txtWO.Text,
                                                                            txtFecha.Text,
                                                                            nombre_cliente,
                                                                            ddlCentro.SelectedItem.Value,
                                                                            ddlModelo.SelectedItem.Text,
                                                                            ddlTipo.SelectedItem.Text,
                                                                            txtSerieEquipo.Text,
                                                                            ddlSistema.SelectedItem.Text,
                                                                            ddlSubsistema.SelectedItem.Text,
                                                                            ddlComponente.SelectedItem.Text,
                                                                            txtSerieComponente.Text,
                                                                            txtParte.Text,
                                                                            txtNumeroParte.Text,
                                                                            horas,
                                                                            ddlClasificacion.SelectedItem.Text,
                                                                            ddlSubclasificacion.SelectedItem.Text,
                                                                            txtDetalle.Text,
                                                                            listArchivos,
                                                                            usuario.getUsuario(),
                                                                            Request.ServerVariables["REMOTE_ADDR"],
                                                                            owner
                                                                        );
            if (registrado.ToUpper().StartsWith("CODIGO:"))
            {
                Session.Remove("listArchivos");

                string codigo = registrado.ToUpper().Replace("CODIGO:", "");
                lbMessage.Text = "El Evento se registró satisfactoriamente con código <b>" + codigo + "</b>";

                return null;
            }
            else
            {
                return "Error al registrar el Evento: " + registrado;
            }
        }


        protected void btMessageSi_Click(object sender, EventArgs e)
        {

            Session.Remove("listArchivos");
            updateGVArchivos();

            clearPanelDesviacion();
            showPanel(PANEL_DESVIACION);
            hfMessage_ModalPopupExtender.Hide();
        }


        protected void btMessageNo_Click(object sender, EventArgs e)
        {
            hfMessage_ModalPopupExtender.Hide();
            uPanel.Update();
            Response.Redirect("~/Clientes/EventosRegistrados.aspx", true);
        }


        private string validatePanelsNuevoRegistro()
        {
            List<string> errors = validatePanelComponente();
            if (errors.Count > 0)
            {
                return "Existen campos sin completar en el panel de Componente";
            }

            errors = validatePanelDesviacion();
            if (errors.Count > 0)
            {
                return "Existen campos sin completar en el panel de Desviación";
            }

            return null;
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


        protected void ibPreviousArchivos_Click(object sender, ImageClickEventArgs e)
        {
            showPanel(PANEL_DESVIACION);
        }


        protected void ibRegisterEvento_Click(object sender, ImageClickEventArgs e)
        {
            List<string> errors = validatePanelArchivos();
            if (errors.Count == 0)
            {

                string status = registrarEvento();
                if (status == null)
                {
                    Session.Remove("listArchivos");
                    hfMessage_ModalPopupExtender.Show();
                }
                else
                {
                    showMessageError(status);
                }
            }
            else
            {
                showSummary(errors);
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


        protected void txtWO_TextChanged(object sender, EventArgs e)
        {
            if (txtWO.Text.Length < 1)
            {
                return;
            }

            if (ddlCentro.SelectedIndex < 1)
            {
                return;
            }

            string id_centro = ddlCentro.SelectedItem.Value;
            WOInfo woInfo = LogicController.getWOInfo(txtWO.Text, id_centro);
            if (woInfo != null)
            {
                setWOInfo(woInfo);

                lbMessageLoadWO.Text = "Se ha encontrado el componente asociado a la <b>W/O " + woInfo.getCodigoWO() + "</b>. ¿Desea cargar los datos?";
                hfLoadWO_ModalPopupExtender.Show();
            }
            else
            {
                Session.Remove("woinfo");
            }
        }


        protected void btLoadWOSi_Click(object sender, EventArgs e)
        {
            loadWOInfo();
        }


        protected void btLoadWONo_Click(object sender, EventArgs e)
        {
            Session.Remove("woinfo");
            hfLoadWO_ModalPopupExtender.Hide();
        }



        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlCentro_DataBound(object sender, EventArgs e)
        {
            ddlCentro.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

     
    }
}