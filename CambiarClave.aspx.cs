using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.Account
{
    public partial class CambiarClave : System.Web.UI.Page
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
            }
        }


        private void clearPanelClave()
        {
            txtClave.Text = "";
            txtNuevaClave.Text = "";
            txtRepeticionClave.Text = "";

            ltSummary.Text = "";
        }

        private List<string> validatePanelClave()
        {
            List<string> errors = new List<string>();

            if (txtClave.Text.Length < 1)
            {
                errors.Add("No se ha ingresado la clave actual");
            }

            if (txtNuevaClave.Text.Length < 1)
            {
                errors.Add("No se ha ingresado la nueva clave");
            }
            else if (txtNuevaClave.Text.Length < 8)
            {
                errors.Add("La nueva clave debe tener una longitud mínima de 8 caracteres");
            }
            else if (!Utils.isFullMatched(txtNuevaClave.Text, "([A-z]+[0-9]+)+"))
            {
                errors.Add("La nueva clave debe contener letras, números y comenzar con una letra");
            }
            else if (txtRepeticionClave.Text.Length < 1)
            {
                errors.Add("Debe repetir la nueva clave para validación");
            }
            else if (!txtNuevaClave.Text.Equals(txtRepeticionClave.Text))
            {
                errors.Add("La clave nueva no coincide con la repetición");
            }

            return errors;
        }



        private string cambiarClave()
        {
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

            string registrado = LogicController.changePassword
                                                                    (
                                                                        usuario.getUsuario(),
                                                                        txtClave.Text,
                                                                        txtNuevaClave.Text,
                                                                        Request.ServerVariables["REMOTE_ADDR"],
                                                                        owner
                                                                    );
            if (registrado == null)
            {
                return null;
            }
            else
            {
                return registrado;
            }
        }


        protected void ibCambiarClave_Click(object sender, ImageClickEventArgs e)
        {
            List<string> errors = validatePanelClave();
            if (errors.Count == 0)
            {

                string status = cambiarClave();
                if (status == null)
                {
                    hfMessage_ModalPopupExtender.Show();
                    uPanel.Update();
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

        protected void btMessageOk_Click(object sender, EventArgs e)
        {
            hfMessage_ModalPopupExtender.Hide();

            Response.Redirect("~/Default.aspx", true);
        }
    }
}