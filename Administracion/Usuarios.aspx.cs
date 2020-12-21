using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;

namespace NCCSAN.Administracion
{
    public partial class Usuarios : System.Web.UI.Page
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

                setIndexRow(-1);
            }
        }


        private void clearPanelUsuarioAgregar()
        {

            txtNombrePersonaAgregar.Text = "";
            hfRutPersonaAgregar.Value = "";
            txtUsuarioAgregar.Text = "";
            ddlRolAgregar.SelectedIndex = 0;
            txtClaveAgregar.Text = "";
            txtEmailAgregar.Text = "";

            ltSummaryAgregar.Text = "";

        }


        private void clearPanelUsuarioEditar()
        {
            txtNombrePersonaEditar.Text = "";
            hfRutPersonaEditar.Value = "";
            txtUsuarioEditar.Text = "";
            ddlRolEditar.SelectedIndex = 0;
            txtEmailEditar.Text = "";

            ltSummaryEditar.Text = "";

        }

        private bool loadUsuario(string nombre_usuario)
        {

            Usuario u = LogicController.getUsuario(nombre_usuario);
            if (u == null)
                return false;

            Persona p = LogicController.getPersona(u.getRutPersona());
            if (p == null)
                return false;

            hfRutPersonaEditar.Value = p.getRut();
            txtNombrePersonaEditar.Text = p.getNombre();
            txtUsuarioEditar.Text = u.getUsuario();

            if (p.getClasificacion().ToUpper().Equals("RSP"))
            {
                ddlRolEditar.Items.Clear();
                ddlRolEditar.Items.Add(new ListItem("RSP", "RSP"));
                ddlRolEditar.SelectedIndex = 0;
                ddlRolEditar.Enabled = false;
            }
            else
            {
                SDSRol.DataBind();
                ddlRolEditar.DataBind();
                ddlRolEditar.Enabled = true;
            }

            if (!p.getClasificacion().ToUpper().Equals("RSP"))
            {
                try
                {
                    ddlRolEditar.SelectedValue = u.getNombreRol();
                }
                catch (Exception ex)
                {
                    ddlRolEditar.SelectedIndex = 0;
                }
            }


            if (p.getEmail() != null)
            {
                txtEmailEditar.Text = p.getEmail();
            }


            return true;
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



        protected void btPersonaLimpiar_Click(object sender, EventArgs e)
        {
            txtPersonaRut.Text = "";
            txtUsuarioNombre.Text = "";
            txtPersonaNombre.Text = "";

            SDSUsuarios.DataBind();
            gvUsuarios.DataBind();
            uPanel.Update();
        }

        protected void btPersonaBuscar_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (e.CommandName.Equals("AgregarUsuario"))
            {
                clearPanelUsuarioAgregar();

                hfAgregarUsuario_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EditarUsuario"))
            {


                index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvUsuarios.Rows.Count))
                {
                    setIndexRow(index);
                    Label lbUsuario = (Label)gvUsuarios.Rows[index].Cells[0].FindControl("lbUsuario");

                    clearPanelUsuarioEditar();

                    if (loadUsuario(lbUsuario.Text))
                    {
                        hfEditarUsuario_ModalPopupExtender.Show();
                    }
                    else
                    {
                        showMessageError("No se puede recuperar la información del Usuario para editar");
                    }

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


        protected void btAceptarEdicionUsuario_Click(object sender, EventArgs e)
        {

            List<string> errors = validatePanelUsuarioEditar();
            if (errors.Count == 0)
            {

                //UPDATE Usuario
                string status = actualizarUsuario();
                if (status == null)
                {
                    clearPanelUsuarioEditar();
                    hfEditarUsuario_ModalPopupExtender.Hide();
                    SDSUsuarios.DataBind();
                    gvUsuarios.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha actualizado exitosamente el Usuario");
                }
                else
                {
                    errors = new List<string>();
                    errors.Add(status);

                    showSummaryEditar(errors);
                }
            }
            else
            {
                showSummaryEditar(errors);
            }

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



        private List<string> validatePanelUsuarioEditar()
        {
            List<string> errors = new List<string>();

            Persona persona = LogicController.getPersona(hfRutPersonaEditar.Value);
            if (persona == null)
            {
                errors.Add("No se puede recuperar información de la Persona");
            }


            if (txtUsuarioEditar.Text.Length < 1)
            {
                errors.Add("Debes ingresar el nombre de Usuario");
            }


            if (!persona.getClasificacion().ToUpper().Equals("RSP"))
            {
                if (ddlRolEditar.SelectedIndex < 1)
                {
                    errors.Add("Debes seleccionar el Rol del Usuario");
                }
            }


            if (txtEmailEditar.Text.Length < 1)
            {
                errors.Add("Debes ingresar el email corporativo de la Persona");
            }


            return errors;

        }



        private List<string> validatePanelUsuarioAgregar()
        {
            List<string> errors = new List<string>();

            Persona persona = null;

            if ((txtNombrePersonaAgregar.Text.Length < 1) || (hfRutPersonaAgregar.Value.Length < 1))
            {
                errors.Add("Debes seleccionar la Persona");
            }
            else
            {
                persona = LogicController.getPersona(hfRutPersonaAgregar.Value);
                if (persona == null)
                {
                    errors.Add("No se puede recuperar información de la Persona");
                }
            }


            if (txtUsuarioAgregar.Text.Length < 1)
            {
                errors.Add("Debes ingresar el nombre de Usuario");
            }


            if (persona != null)
            {
                if (!persona.getClasificacion().ToUpper().Equals("RSP"))
                {
                    if (ddlRolAgregar.SelectedIndex < 1)
                    {
                        errors.Add("Debes seleccionar el Rol del Usuario");
                    }
                }
            }
            else
            {
                if (ddlRolAgregar.SelectedIndex < 1)
                {
                    errors.Add("Debes seleccionar el Rol del Usuario");
                }
            }


            if (chbDefaultPassword.Checked == false)
            {
                if (txtClaveAgregar.Text.Length < 1)
                {
                    errors.Add("Debes ingresar la clave de acceso");
                }
            }

            if (hfRutPersonaAgregar.Value.Length > 0)
            {
                if (LogicController.getPersonaInfo(hfRutPersonaAgregar.Value) == null)
                {
                    errors.Add("La Persona seleccionada es inválida");
                }
            }

            if (txtEmailAgregar.Text.Length < 1)
            {
                errors.Add("Debes ingresar el email corporativo de la Persona");
            }


            return errors;

        }


        private string registrarUsuario()
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


            string registrado = LogicController.registerUsuario(
                                                                        txtUsuarioAgregar.Text,
                                                                        clave,
                                                                        ddlRolAgregar.SelectedValue,
                                                                        hfRutPersonaAgregar.Value,
                                                                        txtEmailAgregar.Text,
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


        private string actualizarUsuario()
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


            string registrado = LogicController.updateUsuario(
                                                                        txtUsuarioEditar.Text,
                                                                        ddlRolEditar.SelectedValue,
                                                                        txtEmailEditar.Text,
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



        protected void btCancelarRegistroUsuario_Click(object sender, EventArgs e)
        {
            hfAgregarUsuario_ModalPopupExtender.Hide();
        }

        protected void btCancelarEdicionUsuario_Click(object sender, EventArgs e)
        {
            hfEditarUsuario_ModalPopupExtender.Hide();
        }

        protected void ddlRolAgregar_DataBound(object sender, EventArgs e)
        {
            ddlRolAgregar.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlRolEditar_DataBound(object sender, EventArgs e)
        {
            ddlRolEditar.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void btSeleccionarPersona_Click(object sender, EventArgs e)
        {
            hfAgregarUsuario_ModalPopupExtender.Hide();
            hfBuscarPersona_ModalPopupExtender.Show();
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

            if (e.CommandName.Equals("SetPersona"))
            {
                txtUsuarioAgregar.Text = "";

                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                Persona persona = LogicController.getPersona(rut);

                if (persona == null)
                {
                    txtNombrePersonaAgregar.Text = "";

                    showBuscarPersonaMessageError("Error al recuperar la información de \"" + nombre + "\"");
                }
                else
                {
                    txtNombrePersonaAgregar.Text = persona.getNombre();
                    hfRutPersonaAgregar.Value = persona.getRut();

                    txtBuscarPersonaApellido.Text = "";
                    ltBuscarPersonaSummary.Text = "";

                    hfBuscarPersona_ModalPopupExtender.Hide();
                    hfAgregarUsuario_ModalPopupExtender.Show();

                    if (persona.getClasificacion().ToUpper().Equals("RSP"))
                    {
                        ddlRolAgregar.Items.Clear();
                        ddlRolAgregar.Items.Add(new ListItem("RSP", "RSP"));
                        ddlRolAgregar.SelectedIndex = 0;
                        ddlRolAgregar.Enabled = false;
                    }
                    else
                    {
                        SDSRol.DataBind();
                        ddlRolAgregar.DataBind();
                        ddlRolAgregar.Enabled = true;
                    }
                }
            }
        }



        protected void btBuscarPersonaLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarPersonaApellido.Text = "";
        }


        protected void btBuscarPersonaCancelar_Click(object sender, EventArgs e)
        {
            hfBuscarPersona_ModalPopupExtender.Hide();
            hfAgregarUsuario_ModalPopupExtender.Show();
        }


        private void showBuscarPersonaMessageError(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #FF0000;\">" + msg + "</span>";
        }

        protected void btGenerarUsuario_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            if (txtNombrePersonaAgregar.Text.Length < 1)
            {
                errors.Add("Para generar el Usuario debes seleccionar la Persona");

                showSummaryAgregar(errors);
                return;
            }

            try
            {
                string[] spNombre = txtNombrePersonaAgregar.Text.Split(',');
                if (spNombre.Length < 2)
                {
                    errors.Add("Error al procesar el nombre. Ingréselo manualmente");

                    showSummaryAgregar(errors);
                    return;
                }

                string[] spApellidos = spNombre[0].Split(' ');
                if (spApellidos.Length != 2)
                {
                    errors.Add("Error al procesar el apellido. Ingréselo manualmente");

                    showSummaryAgregar(errors);
                    return;
                }

                string usuario = spNombre[1].Substring(1, 1) + spApellidos[0] + spApellidos[1].Substring(0, 1);
                usuario = usuario.ToLower();
                txtUsuarioAgregar.Text = usuario;
            }
            catch (Exception ex)
            {
                errors.Add("Error al generar el usuario. Ingréselo manualmente");

                showSummaryAgregar(errors);
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


        private void showSummaryEditar(List<string> errors)
        {
            string summary = "<span style=\"color:red;\"><b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            summary += "</span>";

            ltSummaryEditar.Text = summary;
        }

        protected void btCancelarEliminarUsuario_Click(object sender, EventArgs e)
        {
            hfEliminarUsuario_ModalPopupExtender.Hide();
            uPanel.Update();
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
    }
}