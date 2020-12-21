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
    public partial class CuentaCorreoSistema : System.Web.UI.Page
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


                string loaded = loadTables();
                if (loaded != null)
                {
                    string msg = loaded;
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }
        }


        private string loadTables()
        {

            if (!loadTableEmail())
            {
                return "No se puede cargar los datos de la credencial de acceso";
            }


            return null;
        }


        private bool loadTableEmail()
        {
            Dictionary<string, string> listEmailCredential = LogicController.getEmailCredential();
            if (listEmailCredential == null)
            {
                return false;
            }


            txtUser.Text = Convert.ToString(listEmailCredential["User"]);
            txtPassword.Text = Convert.ToString(listEmailCredential["Password"]);
            txtEmail.Text = Convert.ToString(listEmailCredential["Email"]);
            string active = Convert.ToString(listEmailCredential["Active"]);
            if (active.Equals("Si"))
            {
                chbActive.Checked = true;
            }
            else
            {
                chbActive.Checked = false;
            }

            return true;
        }




        private List<string> validatePanels()
        {
            List<string> errors = new List<string>();
            errors.AddRange(validatePanelEmail());

            return errors;
        }


        private List<string> validatePanelEmail()
        {
            List<string> errors = new List<string>();

            if (txtUser.Text.Length < 1)
            {
                errors.Add("No se indicó el nombre de usuario de la red");
            }

            if (txtPassword.Text.Length < 1)
            {
                errors.Add("No se indicó la clave para el usuario de la red");
            }

            if (txtEmail.Text.Length < 1)
            {
                errors.Add("No se indicó el correo del usuario de la red");
            }
            else if (!Utils.validateEmail(txtEmail.Text))
            {
                errors.Add("El email indicado es inválido");
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

            Dictionary<string, string> listEmailCredential = LogicController.getEmailCredential();
            if (listEmailCredential == null)
            {
                return "Error al recuperar la credencial de correo actual";
            }

            listEmailCredential["User"] = txtUser.Text;
            listEmailCredential["Password"] = txtPassword.Text;
            listEmailCredential["Email"] = txtEmail.Text;
            if (chbActive.Checked)
            {
                listEmailCredential["Active"] = "Si";
            }
            else
            {
                listEmailCredential["Active"] = "No";
            }


            string status = LogicController.updateConfiguracionEmailCredential(
                                                                            listEmailCredential,
                                                                            id_centro,
                                                                            usuario.getUsuario(),
                                                                            Request.ServerVariables["REMOTE_ADDR"],
                                                                            owner
                                                                        );
            if (status == null)
            {
                EmailSender.listEmailCredential = listEmailCredential;
                EmailSender.active = chbActive.Checked;

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
                    showMessageSuccess("Se ha actualizado exitosamente la credencial de acceso");
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