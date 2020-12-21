using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;

namespace NCCSAN.Administracion
{
    public partial class CentroClienteUsuarios : System.Web.UI.Page
    {
        private static readonly int MODO_REGISTRO_CUENTA_CREAR = 1;
        private static readonly int MODO_REGISTRO_CUENTA_ACTUALIZAR = 2;

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

                if ((Session["cliente"] != null) && (Session["id_centro"] != null))
                {
                    string nombre_cliente = (string)Session["cliente"];
                    string id_centro = (string)Session["id_centro"];

                    if ((nombre_cliente != null) && (id_centro != null))
                    {
                        hfNombreCliente.Value = nombre_cliente;
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


        private void setModeRegistroCuenta(int mode)
        {
            Session["ModoRegistroCuenta"] = mode;
        }


        private int getModeRegistroCuenta()
        {
            if (Session["ModoRegistroCuenta"] == null)
                return -1;
            else
                return (int)Session["ModoRegistroCuenta"];
        }


        private void setIndexRow(int index)
        {
            Session["IndexRow"] = index;
        }


        private int getIndexRow()
        {
            if (Session["IndexRow"] == null)
                return -1;
            else
                return (int)Session["IndexRow"];
        }


        private void clearPanelPersona()
        {
            txtEmail.Text = "";

            ltSummary.Text = "";
        }


        private bool loadPersona(string nombre_cliente)
        {
            Persona p = LogicController.getClientePersona(nombre_cliente);
            if (p == null)
                return false;

            if (p.getEmail() != null)
            {
                txtEmail.Text = p.getEmail();
            }

            return true;
        }



        protected void gvCuentas_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (e.CommandName.Equals("ConfigurarCuenta"))
            {
                setModeRegistroCuenta(MODO_REGISTRO_CUENTA_CREAR);
                clearPanelPersona();
                hfAgregarCuenta_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EditarCuenta"))
            {
                setModeRegistroCuenta(MODO_REGISTRO_CUENTA_ACTUALIZAR);

                index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvCuentas.Rows.Count))
                {
                    setIndexRow(index);

                    if (hfNombreCliente.Value == null)
                    {
                        showMessageError("Error al recuperar información del Cliente");

                        return;
                    }
                    else if (hfNombreCliente.Value.Length < 1)
                    {
                        showMessageError("Error al recuperar información del Cliente");

                        return;
                    }

                    string nombre_cliente = hfNombreCliente.Value;


                    clearPanelPersona();


                    if (loadPersona(nombre_cliente))
                    {
                        hfAgregarCuenta_ModalPopupExtender.Show();
                    }
                    else
                    {
                        showMessageError("No se puede recuperar la información de la Cuenta para editar");
                    }

                }
            }
            else if (e.CommandName.Equals("EliminarCuenta"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }


                if ((index >= 0) && (index < gvCuentas.Rows.Count))
                {
                    setIndexRow(index);
                    lbMessageEliminarCuenta.Text = "Se eliminará la información de \"" + gvCuentas.Rows[index].Cells[0].Text + "\". ¿Desea continuar?";
                    upEliminarCuenta.Update();
                    hfEliminarCuenta_ModalPopupExtender.Show();
                }
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



        private void showSummary(List<string> errors)
        {
            string summary = "<span style=\"color:red;\"><b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            summary += "</span>";

            ltSummary.Text = summary;
        }



        private List<string> validatePanelCuenta()
        {
            List<string> errors = new List<string>();

            if (txtEmail.Text.Length < 1)
            {
                errors.Add("Debes ingresar el email del Cliente");
            }


            return errors;

        }


        protected void btAceptarRegistroCuenta_Click(object sender, EventArgs e)
        {
            List<string> errors = validatePanelCuenta();
            if (errors.Count == 0)
            {

                int modeRegistroCuenta = getModeRegistroCuenta();
                if (modeRegistroCuenta == 1)
                {
                    //INSERT Persona

                    string status = registrarCuenta();
                    if (status == null)
                    {
                        clearPanelPersona();
                        hfAgregarCuenta_ModalPopupExtender.Hide();
                        SDSCuentas.DataBind();
                        gvCuentas.DataBind();
                        uPanel.Update();

                        showMessageSuccess("Se ha registrado exitosamente la Cuenta");
                    }
                    else
                    {
                        errors = new List<string>();
                        errors.Add(status);

                        showSummary(errors);
                    }

                }
                else if (modeRegistroCuenta == 2)
                {
                    //UPDATE Persona
                    string status = actualizarCuenta();
                    if (status == null)
                    {
                        clearPanelPersona();
                        hfAgregarCuenta_ModalPopupExtender.Hide();
                        SDSCuentas.DataBind();
                        gvCuentas.DataBind();
                        uPanel.Update();

                        showMessageSuccess("Se ha actualizado exitosamente la Cuenta");
                    }
                    else
                    {
                        errors = new List<string>();
                        errors.Add(status);

                        showSummary(errors);
                    }
                }

            }
            else
            {
                showSummary(errors);
            }
        }



        private string registrarCuenta()
        {

            if (hfNombreCliente.Value == null)
            {
                return "Error al recuperar información del Cliente";
            }
            else if (hfNombreCliente.Value.Length < 1)
            {
                return "Error al recuperar información del Cliente";
            }

            string nombre_cliente = hfNombreCliente.Value;


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }

            Usuario usuario = (Usuario)Session["usuario"];


            string registrado = LogicController.registerClientePersona(
                                                                        nombre_cliente,
                                                                        txtEmail.Text,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );

            if (registrado == null)
            {
                return null;
            }
            else
            {
                return "Error: " + registrado;
            }
        }



        private string actualizarCuenta()
        {

            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];

            if (hfNombreCliente.Value == null)
            {
                return "Error al recuperar información del Cliente";
            }
            else if (hfNombreCliente.Value.Length < 1)
            {
                return "Error al recuperar información del Cliente";
            }

            string nombre_cliente = hfNombreCliente.Value;

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

            string registrado = LogicController.updateClientePersona(
                                                                        nombre_cliente,
                                                                        txtEmail.Text,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );

            if (registrado == null)
            {
                return null;
            }
            else
            {
                return "Error: " + registrado;
            }
        }




        private string eliminarCuenta()
        {
            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];

            if (hfNombreCliente.Value == null)
            {
                return "Error al recuperar información del Cliente";
            }
            else if (hfNombreCliente.Value.Length < 1)
            {
                return "Error al recuperar información del Cliente";
            }

            string nombre_cliente = hfNombreCliente.Value;


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



            string status = LogicController.removeClientePersona(
                                                                        nombre_cliente,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
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


        protected void btCancelarRegistroCuenta_Click(object sender, EventArgs e)
        {
            hfAgregarCuenta_ModalPopupExtender.Hide();
        }


        protected void btCancelarEliminarCuenta_Click(object sender, EventArgs e)
        {
            hfEliminarCuenta_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void btEliminarCuenta_Click(object sender, EventArgs e)
        {

            int index = getIndexRow();
            if ((index >= 0) && (index < gvCuentas.Rows.Count))
            {
                string status = eliminarCuenta();
                if (status == null)
                {
                    hfEliminarCuenta_ModalPopupExtender.Hide();
                    SDSCuentas.DataBind();
                    gvCuentas.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha eliminado la Cuenta con éxito");
                }
                else
                {
                    showMessageError(status);
                }
            }
            else
            {
                showMessageError("No se puede eliminar la información de la Cuenta");
            }
        }



        private void clearPanelUsuarioAgregar()
        {
            txtUsuarioAgregar.Text = "";
            txtClaveAgregar.Text = "";

            ltSummaryAgregar.Text = "";
        }



        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (e.CommandName.Equals("AgregarUsuario"))
            {
                if (Session["id_centro"] == null)
                {
                    showMessageError("Error al recuperar el Centro de servicios");

                    return;
                }

                string id_centro = (string)Session["id_centro"];

                if (hfNombreCliente.Value == null)
                {
                    showMessageError("Error al recuperar información del Cliente");

                    return;
                }
                else if (hfNombreCliente.Value.Length < 1)
                {
                    showMessageError("Error al recuperar información del Cliente");

                    return;
                }

                string nombre_cliente = hfNombreCliente.Value;


                int persona_exists = LogicController.clientePersonaExists(hfNombreCliente.Value);
                if (persona_exists < 0)
                {
                    showMessageError("Error al recuperar información del Cliente");

                    return;
                }
                else if (persona_exists == 0)
                {
                    showMessageError("Antes de agregar usuarios se debe configurar la cuenta del Cliente");

                    return;
                }


                clearPanelUsuarioAgregar();

                hfAgregarUsuario_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("ResetPassword"))
            {
                index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvUsuarios.Rows.Count))
                {
                    setIndexRow(index);

                    hfConfirmResetPassword_ModalPopupExtender.Show();
                }
            }
            else if (e.CommandName.Equals("EliminarUsuario"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }


                if ((index >= 0) && (index < gvUsuarios.Rows.Count))
                {
                    setIndexRow(index);
                    lbMessageEliminarUsuario.Text = "Se eliminará el usuario \"" + ((Label)gvUsuarios.Rows[index].FindControl("lbUsuario")).Text + "\". ¿Desea continuar?";
                    upEliminarUsuario.Update();
                    hfEliminarUsuario_ModalPopupExtender.Show();
                }
            }

        }



        protected void chbDefaultPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chbDefaultPassword.Checked)
            {
                txtClaveAgregar.Text = "";
                txtClaveAgregar.Enabled = false;
            }
            else
            {
                txtClaveAgregar.Enabled = true;
            }
        }



        private List<string> validatePanelUsuarioAgregar()
        {
            List<string> errors = new List<string>();

            if (txtUsuarioAgregar.Text.Length < 1)
            {
                errors.Add("Debes ingresar el nombre de Usuario");
            }


            if (chbDefaultPassword.Checked == false)
            {
                if (txtClaveAgregar.Text.Length < 1)
                {
                    errors.Add("Debes ingresar la clave de acceso");
                }
            }


            return errors;

        }



        protected void btAceptarRegistroUsuario_Click(object sender, EventArgs e)
        {

            List<string> errors = validatePanelUsuarioAgregar();
            if (errors.Count == 0)
            {

                //INSERT Usuario
                string status = registrarUsuario();
                if (status == null)
                {
                    clearPanelUsuarioAgregar();
                    hfAgregarUsuario_ModalPopupExtender.Hide();
                    SDSUsuarios.DataBind();
                    gvUsuarios.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha registrado exitosamente el Usuario");
                }
                else
                {
                    errors = new List<string>();
                    errors.Add(status);

                    showSummaryAgregar(errors);
                }
            }
            else
            {
                showSummaryAgregar(errors);
            }

        }



        private string registrarUsuario()
        {

            int persona_exists = LogicController.clientePersonaExists(hfNombreCliente.Value);
            if (persona_exists < 0)
            {
                return "Error al recuperar información del Cliente";
            }
            else if (persona_exists == 0)
            {
                return "Antes de agregar usuarios se debe configurar la cuenta del Cliente";
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

            string clave;
            if (chbDefaultPassword.Checked)
            {
                clave = "0000";
            }
            else
            {
                if (txtClaveAgregar.Text.Length < 8)
                {
                    return "La nueva clave debe tener una longitud mínima de 8 caracteres";
                }

                if (!Utils.isFullMatched(txtClaveAgregar.Text, "([A-z]+[0-9]+)+"))
                {
                    return "La nueva clave debe contener letras, números y comenzar con una letra";
                }

                clave = txtClaveAgregar.Text;
            }


            string registrado = LogicController.registerUsuarioClientePersona(
                                                                        hfNombreCliente.Value,
                                                                        txtUsuarioAgregar.Text,
                                                                        clave,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"],
                                                                        owner
                                                                    );

            if (registrado == null)
            {
                return null;
            }
            else
            {
                return "Error: " + registrado;
            }
        }



        private void showSummaryAgregar(List<string> errors)
        {
            string summary = "<span style=\"color:red;\"><b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            summary += "</span>";

            ltSummaryAgregar.Text = summary;
        }



        protected void btCancelarRegistroUsuario_Click(object sender, EventArgs e)
        {
            hfAgregarUsuario_ModalPopupExtender.Hide();
        }


        protected void btAceptarResetPassword_Click(object sender, EventArgs e)
        {
            int index = getIndexRow();
            if ((index >= 0) && (index < gvUsuarios.Rows.Count))
            {
                string nombre_usuario = ((Label)gvUsuarios.Rows[index].Cells[0].FindControl("lbUsuario")).Text;
                string status = restablecerPassword(nombre_usuario);
                if (status == null)
                {
                    SDSUsuarios.DataBind();
                    gvUsuarios.DataBind();
                    hfConfirmResetPassword_ModalPopupExtender.Hide();
                    uPanel.Update();

                    showMessageSuccess("Se ha restablecido la clave de acceso por defecto para \"" + nombre_usuario + "\"");
                }
                else
                {

                    hfConfirmResetPassword_ModalPopupExtender.Hide();
                    uPanel.Update();
                    showMessageError(status);
                }
            }
            else
            {

                hfConfirmResetPassword_ModalPopupExtender.Hide();
                uPanel.Update();
                showMessageError("No se puede restablecer la clave de acceso");
            }
        }



        private string restablecerPassword(string nombre_usuario)
        {

            int persona_exists = LogicController.clientePersonaExists(hfNombreCliente.Value);
            if (persona_exists < 0)
            {
                return "Error al recuperar información del Cliente";
            }
            else if (persona_exists == 0)
            {
                return "Antes de agregar usuarios se debe configurar la cuenta del Cliente";
            }


            Persona persona_cuenta = LogicController.getClientePersona(hfNombreCliente.Value);
            if (persona_cuenta == null)
            {
                return "Error al recuperar información de la cuenta";
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


            string registrado = LogicController.resetDefaultPassword(
                                                                        nombre_usuario,
                                                                        Request.ServerVariables["REMOTE_ADDR"],
                                                                        owner
                                                                    );
            if (registrado == null)
            {
                return null;
            }
            else
            {
                return "Error: " + registrado;
            }
        }



        protected void btCancelarResetPassword_Click(object sender, EventArgs e)
        {
            hfConfirmResetPassword_ModalPopupExtender.Hide();
        }



        protected void btEliminarUsuario_Click(object sender, EventArgs e)
        {
            int index = getIndexRow();
            if ((index >= 0) && (index < gvUsuarios.Rows.Count))
            {
                string status = eliminarUsuario(((Label)gvUsuarios.Rows[index].Cells[0].FindControl("lbUsuario")).Text);
                if (status == null)
                {
                    SDSUsuarios.DataBind();
                    gvUsuarios.DataBind();
                    hfEliminarUsuario_ModalPopupExtender.Hide();
                    uPanel.Update();

                    showMessageSuccess("Se ha eliminado el Usuario con éxito");
                }
                else
                {

                    hfEliminarUsuario_ModalPopupExtender.Hide();
                    uPanel.Update();
                    showMessageError(status);
                }
            }
            else
            {

                hfEliminarUsuario_ModalPopupExtender.Hide();
                uPanel.Update();
                showMessageError("No se puede eliminar el Usuario");
            }
        }



        private string eliminarUsuario(string nombre_usuario)
        {
            if (nombre_usuario == null)
            {
                return "No se ha seleccionado un Usuario";
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

            string status = LogicController.removeUsuario(nombre_usuario, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"], owner);
            if (status == null)
            {
                return null;
            }
            else
            {
                return status;
            }
        }


        protected void btCancelarEliminarUsuario_Click(object sender, EventArgs e)
        {
            hfEliminarUsuario_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            Session.Remove("cliente");

            Response.Redirect("~/Configuracion/Clientes.aspx", true);
        }
    }
}