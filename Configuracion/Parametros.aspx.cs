using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;

namespace NCCSAN.Configuracion
{
    public partial class Prametros : System.Web.UI.Page
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

                if (Session["id_centro"] != null)
                {
                    string id_centro = (string)Session["id_centro"];

                    string loaded = loadTables(id_centro);
                    if (loaded != null)
                    {
                        string msg = loaded;
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }
                }
                else
                {

                }
            }
        }


        private string loadTables(string id_centro)
        {

            if (!loadTableArchivos())
            {
                return "No se puede cargar la tabla de Tamaño para Archivos";
            }


            return null;
        }


        private bool loadTableArchivos()
        {
            Dictionary<string, int> listMaxFileSize = LogicController.getListMaxFileSize();
            if (listMaxFileSize == null)
            {
                return false;
            }

            txtMaxSizeImagen.Text = Convert.ToString(listMaxFileSize["Imagen"] / 1000000);
            txtMaxSizeDocumento.Text = Convert.ToString(listMaxFileSize["Documento"] / 1000000);
            txtMaxSizeVideo.Text = Convert.ToString(listMaxFileSize["Video"] / 1000000);
            txtMaxSizeAudio.Text = Convert.ToString(listMaxFileSize["Audio"] / 1000000);
            txtMaxSizeDiapositivas.Text = Convert.ToString(listMaxFileSize["Diapositivas"] / 1000000);
            txtMaxSizeComprimido.Text = Convert.ToString(listMaxFileSize["Comprimido"] / 1000000);

            return true;
        }




        private List<string> validatePanels()
        {
            List<string> errors = new List<string>();
            errors.AddRange(validatePanelMaxFileSize());

            return errors;
        }


        private List<string> validatePanelMaxFileSize()
        {
            List<string> errors = new List<string>();

            int max_file_size;

            {//Imagen
                if (txtMaxSizeImagen.Text.Length < 1)
                {
                    errors.Add("Se debe indicar el tamaño máximo para Imágenes");
                }
                else
                {

                    if (!Utils.validateNumber(txtMaxSizeImagen.Text))
                    {
                        errors.Add("El tamaño máximo para Imágenes debe ser un número");
                    }

                    max_file_size = Convert.ToInt32(txtMaxSizeImagen.Text);
                    if (max_file_size < 1)
                    {
                        errors.Add("El tamaño máximo para Imágenes debe ser mayor que 0 MegaBytes");
                    }

                    if (max_file_size > 99)
                    {
                        errors.Add("El tamaño máximo para Imágenes no puede ser mayor que 99 MegaBytes");
                    }
                }
            }


            {//Documento
                if (txtMaxSizeDocumento.Text.Length < 1)
                {
                    errors.Add("Se debe indicar el tamaño máximo para Documentos");
                }
                else
                {

                    if (!Utils.validateNumber(txtMaxSizeDocumento.Text))
                    {
                        errors.Add("El tamaño máximo para Documento debe ser un número");
                    }

                    max_file_size = Convert.ToInt32(txtMaxSizeDocumento.Text);
                    if (max_file_size < 1)
                    {
                        errors.Add("El tamaño máximo para Documentos debe ser mayor que 0 MegaBytes");
                    }

                    if (max_file_size > 99)
                    {
                        errors.Add("El tamaño máximo para Documentos no puede ser mayor que 99 MegaBytes");
                    }
                }
            }


            {//Video
                if (txtMaxSizeVideo.Text.Length < 1)
                {
                    errors.Add("Se debe indicar el tamaño máximo para Videos");
                }
                else
                {

                    if (!Utils.validateNumber(txtMaxSizeVideo.Text))
                    {
                        errors.Add("El tamaño máximo para Video debe ser un número");
                    }

                    max_file_size = Convert.ToInt32(txtMaxSizeVideo.Text);
                    if (max_file_size < 1)
                    {
                        errors.Add("El tamaño máximo para Videos debe ser mayor que 0 MegaBytes");
                    }

                    if (max_file_size > 99)
                    {
                        errors.Add("El tamaño máximo para Videos no puede ser mayor que 99 MegaBytes");
                    }
                }
            }


            {//Audio
                if (txtMaxSizeAudio.Text.Length < 1)
                {
                    errors.Add("Se debe indicar el tamaño máximo para Audios");
                }
                else
                {

                    if (!Utils.validateNumber(txtMaxSizeAudio.Text))
                    {
                        errors.Add("El tamaño máximo para Audio debe ser un número");
                    }

                    max_file_size = Convert.ToInt32(txtMaxSizeAudio.Text);
                    if (max_file_size < 1)
                    {
                        errors.Add("El tamaño máximo para Audios debe ser mayor que 0 MegaBytes");
                    }

                    if (max_file_size > 99)
                    {
                        errors.Add("El tamaño máximo para Audios no puede ser mayor que 99 MegaBytes");
                    }
                }
            }


            {//Diapositivas
                if (txtMaxSizeDiapositivas.Text.Length < 1)
                {
                    errors.Add("Se debe indicar el tamaño máximo para Diapositivas");
                }
                else
                {

                    if (!Utils.validateNumber(txtMaxSizeDiapositivas.Text))
                    {
                        errors.Add("El tamaño máximo para Diapositivas debe ser un número");
                    }

                    max_file_size = Convert.ToInt32(txtMaxSizeDiapositivas.Text);
                    if (max_file_size < 1)
                    {
                        errors.Add("El tamaño máximo para Diapositivas debe ser mayor que 0 MegaBytes");
                    }

                    if (max_file_size > 99)
                    {
                        errors.Add("El tamaño máximo para Diapositivas no puede ser mayor que 99 MegaBytes");
                    }
                }
            }


            {//Comprimido
                if (txtMaxSizeComprimido.Text.Length < 1)
                {
                    errors.Add("Se debe indicar el tamaño máximo para Comprimidos");
                }
                else
                {

                    if (!Utils.validateNumber(txtMaxSizeComprimido.Text))
                    {
                        errors.Add("El tamaño máximo para Comprimido debe ser un número");
                    }

                    max_file_size = Convert.ToInt32(txtMaxSizeComprimido.Text);
                    if (max_file_size < 1)
                    {
                        errors.Add("El tamaño máximo para Comprimidos debe ser mayor que 0 MegaBytes");
                    }

                    if (max_file_size > 99)
                    {
                        errors.Add("El tamaño máximo para Comprimidos no puede ser mayor que 99 MegaBytes");
                    }
                }
            }


            return errors;
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



        private string saveConfig()
        {
            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];


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

            Dictionary<string, int> listMaxFileSize = new Dictionary<string, int>();
            listMaxFileSize.Add("Imagen", Convert.ToInt32(txtMaxSizeImagen.Text));
            listMaxFileSize.Add("Documento", Convert.ToInt32(txtMaxSizeDocumento.Text));
            listMaxFileSize.Add("Video", Convert.ToInt32(txtMaxSizeVideo.Text));
            listMaxFileSize.Add("Audio", Convert.ToInt32(txtMaxSizeAudio.Text));
            listMaxFileSize.Add("Diapositivas", Convert.ToInt32(txtMaxSizeDiapositivas.Text));
            listMaxFileSize.Add("Comprimido", Convert.ToInt32(txtMaxSizeComprimido.Text));

            string status = LogicController.updateConfiguracionMaxFileSize(
                                                                            listMaxFileSize,
                                                                            id_centro,
                                                                            usuario.getUsuario(),
                                                                            Request.ServerVariables["REMOTE_ADDR"],
                                                                            owner
                                                                        );
            if (status == null)
            {
                return null;
            }
            else
            {
                return status;
            }
        }


        protected void ibGuardarConfiguracionParametros_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";

            List<string> errors = validatePanels();

            if (errors.Count == 0)
            {
                string status = saveConfig();
                if (status == null)
                {
                    showMessageSuccess("Se ha actualizado exitosamente la configuración para el envío de Emails");
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


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }


        private void showMessageSuccess(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxSuccess", "<script type=\"text/javascript\">showMessageSuccess('" + message + "');</script>", false);
        }

        protected void ibGuardarConfiguracionParametros_Click1(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";

            List<string> errors = validatePanels();

            if (errors.Count == 0)
            {
                string status = saveConfig();
                if (status == null)
                {
                    showMessageSuccess("Se ha actualizado exitosamente la configuración Parámetros");
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
    }
}