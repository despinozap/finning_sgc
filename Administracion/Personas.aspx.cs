using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.Administracion
{
    public partial class Personas : System.Web.UI.Page
    {

        private static readonly int MODO_REGISTRO_PERSONA_CREAR = 1;
        private static readonly int MODO_REGISTRO_PERSONA_ACTUALIZAR = 2;

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
                Session.Remove("ModoRegistroPersona");

                ddlSexo.Items.Clear();
                ddlSexo.Items.Add(new ListItem("Seleccione..", ""));
                ddlSexo.Items.Add(new ListItem("Masculino", "M"));
                ddlSexo.Items.Add(new ListItem("Femenino", "F"));


            }
        }


        private void clearPanelPersona()
        {
            txtRUT.Text = "";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            txtFechaNacimiento.Text = "";
            ddlSexo.SelectedIndex = 0;
            txtIDEmpleado.Text = "";
            ddlCentro.SelectedIndex = 0;
            ddlStore.SelectedIndex = 0;
            txtLob.Text = "";
            txtCargo.Text = "";
            ddlClasificacionPersona.SelectedIndex = 0;
            txtNombreSupervisor.Text = "";
            hfRutSupervisor.Value = "";
            txtEmail.Text = "";
            txtFechaIngreso.Text = "";
            txtFechaRetiro.Text = "";

            ltSummary.Text = "";
        }


        private bool loadPersona(string rut)
        {
            Persona p = LogicController.getPersona(rut);
            if (p == null)
                return false;

            txtRUT.Text = p.getRut();
            string[] spNombre = p.getNombre().Split(',');
            if (spNombre.Length != 2)
                return false;

            string nombre = spNombre[1];
            if (nombre.Length < 2)
                return false;

            txtNombres.Text = nombre.Substring(1, nombre.Length - 1);
            txtApellidos.Text = spNombre[0];
            txtFechaNacimiento.Text = p.getFechaNacimiento();
            ddlSexo.SelectedValue = p.getSexo();
            txtIDEmpleado.Text = p.getIDEmpleado();
            ddlClasificacionPersona.SelectedValue = p.getClasificacion();

            disableRSPFields(p.getClasificacion());

            if (!p.getClasificacion().ToUpper().Equals("RSP"))
            {
                ddlCentro.SelectedValue = p.getIDCentro();
                ddlStore.SelectedValue = p.getIDStore();
                txtLob.Text = p.getLob();
            }

            txtCargo.Text = p.getCargo();

            if (p.getRutSupervisor() != null)
            {
                PersonaInfo supervisor = LogicController.getPersonaInfo(p.getRutSupervisor());
                if (supervisor != null)
                {
                    txtNombreSupervisor.Text = supervisor.getNombre();
                    hfRutSupervisor.Value = supervisor.getRut();
                }
            }

            if (p.getEmail() != null)
            {
                txtEmail.Text = p.getEmail();
            }

            txtFechaIngreso.Text = p.getFechaIngreso();

            if (p.getFechaRetiro() != null)
            {
                txtFechaRetiro.Text = p.getFechaRetiro();
            }

            return true;
        }



        private void setModeRegistroPersona(int mode)
        {
            Session["ModoRegistroPersona"] = mode;
        }


        private int getModeRegistroPersona()
        {
            if (Session["ModoRegistroPersona"] == null)
                return -1;
            else
                return (int)Session["ModoRegistroPersona"];
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

        private void showPersonasCount(string msg)
        {
            if (msg != null)
                lbPersonasCount.Text = msg;
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


        protected void btPersonaLimpiar_Click(object sender, EventArgs e)
        {
            txtPersonaRut.Text = "";
            txtPersonaNombre.Text = "";

            SDSPersonas.DataBind();
            gvPersonas.DataBind();
            uPanel.Update();
        }



        protected void SDSPersonas_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                showPersonasCount(Convert.ToString(e.AffectedRows) + " registros coinciden con la búsqueda");
            else
                showPersonasCount("No se han encontrado coincidencias con la búsqueda");
        }


        protected void gvPersonas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";

            int index = -1;

            if (e.CommandName.Equals("AgregarPersona"))
            {
                setModeRegistroPersona(MODO_REGISTRO_PERSONA_CREAR);
                clearPanelPersona();
                txtRUT.Enabled = true;
                txtRUT.ReadOnly = false;
                hfAgregarPersona_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EditarPersona"))
            {
                setModeRegistroPersona(MODO_REGISTRO_PERSONA_ACTUALIZAR);

                index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvPersonas.Rows.Count))
                {
                    setIndexRow(index);
                    Label lbRUT = (Label)gvPersonas.Rows[index].Cells[0].FindControl("lbRUT");

                    clearPanelPersona();
                    txtRUT.Enabled = false;
                    txtRUT.ReadOnly = true;

                    if (loadPersona(lbRUT.Text))
                    {
                        hfAgregarPersona_ModalPopupExtender.Show();
                    }
                    else
                    {
                        showMessageError("No se puede recuperar la información de la Persona para editar");
                    }

                }
            }
            else if (e.CommandName.Equals("EliminarPersona"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }


                if ((index >= 0) && (index < gvPersonas.Rows.Count))
                {
                    setIndexRow(index);
                    lbMessageEliminarPersona.Text = "Se eliminará la información de \"" + ((Label)gvPersonas.Rows[index].FindControl("lbNombre")).Text + "\". ¿Desea continuar?";
                    upEliminarPersona.Update();
                    hfEliminarPersona_ModalPopupExtender.Show();
                }
            }

        }

        protected void btCancelarRegistroPersona_Click(object sender, EventArgs e)
        {
            hfAgregarPersona_ModalPopupExtender.Hide();
        }

        protected void ddlStore_DataBound(object sender, EventArgs e)
        {
            ddlStore.Items.Insert(0, new ListItem("Seleccione..", ""));
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

            if (e.CommandName.Equals("SetSupervisor"))
            {
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                PersonaInfo supervisor = LogicController.getPersonaInfo(rut);

                if (supervisor == null)
                {
                    txtNombreSupervisor.Text = "";

                    showBuscarPersonaMessageError("Error al recuperar la información de \"" + nombre + "\"");
                }
                else
                {
                    txtNombreSupervisor.Text = supervisor.getNombre();
                    hfRutSupervisor.Value = supervisor.getRut();

                    txtBuscarPersonaApellido.Text = "";
                    ltBuscarPersonaSummary.Text = "";

                    hfBuscarPersona_ModalPopupExtender.Hide();
                    hfAgregarPersona_ModalPopupExtender.Show();
                }
            }
        }



        protected void btBuscarPersonaCancelar_Click(object sender, EventArgs e)
        {
            hfBuscarPersona_ModalPopupExtender.Hide();
            hfAgregarPersona_ModalPopupExtender.Show();
        }

        protected void btPersonaBuscar_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }

        protected void btSeleccionarSupervisor_Click(object sender, EventArgs e)
        {
            if (ddlCentro.SelectedIndex > 0)
            {
                hfAgregarPersona_ModalPopupExtender.Hide();
                hfBuscarPersona_ModalPopupExtender.Show();
            }
            else
            {
                List<string> errors = new List<string>();
                errors.Add("Para seleccionar el Supervisor primero se debe seleccionar el Centro");

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


        private List<string> validatePanelPersona()
        {
            List<string> errors = new List<string>();

            if (txtRUT.Text.Length < 1)
            {
                errors.Add("Debes ingresar el RUT de la Persona");
            }

            if (txtNombres.Text.Length < 1)
            {
                errors.Add("Debes ingresar los nombres de la Persona");
            }

            if (txtApellidos.Text.Length < 1)
            {
                errors.Add("Debes ingresar los apellidos de la Persona");
            }

            if (txtFechaNacimiento.Text.Length < 1)
            {
                errors.Add("Debes seleccionar la fecha de nacimiento de la Persona");
            }
            else if (!Utils.validateFecha(txtFechaNacimiento.Text))
            {
                errors.Add("La fecha de nacimiento es inválida");
            }

            if (ddlSexo.SelectedIndex < 1)
            {
                errors.Add("Debes seleccionar el sexo de la Persona");
            }


            if (ddlClasificacionPersona.SelectedIndex < 1)
            {
                errors.Add("Debes seleccionar la clasificación de la Persona");
            }

            if (txtIDEmpleado.Text.Length < 1)
            {
                errors.Add("Debes ingresar el ID de Empleado de la Persona");
            }


            if (!ddlClasificacionPersona.SelectedItem.Value.ToUpper().Equals("RSP"))
            {
                //Si no es RSP

                if (ddlCentro.SelectedIndex < 1)
                {
                    errors.Add("Debes seleccionar el Centro al que pertenece la Persona");
                }


                if (ddlStore.SelectedIndex < 1)
                {
                    errors.Add("Debes seleccionar el Store al que pertenece la Persona");
                }

                if (txtLob.Text.Length < 1)
                {
                    errors.Add("Debes ingresar el Lob al que pertenece la Persona");
                }

            }


            if (txtCargo.Text.Length < 1)
            {
                errors.Add("Debes ingresar el cargo de la Persona");
            }

            if (txtEmail.Text.Length < 1)
            {
                errors.Add("Debes ingresar el email corporativo de la Persona");
            }


            if (txtFechaIngreso.Text.Length < 1)
            {
                errors.Add("Debes seleccionar la fecha de ingreso a la empresa");
            }
            else if (!Utils.validateFecha(txtFechaIngreso.Text))
            {
                errors.Add("La fecha de ingreso a la empresa es inválida");
            }


            if (txtFechaRetiro.Text.Length > 0)
            {
                if (!Utils.validateFecha(txtFechaRetiro.Text))
                {
                    errors.Add("La fecha de retiro de la Persona es inválida");
                }
            }


            return errors;

        }



        protected void btAceptarRegistroPersona_Click(object sender, EventArgs e)
        {
            List<string> errors = validatePanelPersona();
            if (errors.Count == 0)
            {
                if (Session["id_centro"] == null)
                {
                    showMessageError("Error al recuperar el Centro de servicios");

                    return;
                }

                string id_centro = (string)Session["id_centro"];

                if ((!ddlCentro.SelectedValue.Equals(id_centro)) && (!ddlCentro.SelectedValue.ToUpper().Equals("RSP")))
                {
                    hfAgregarPersona_ModalPopupExtender.Hide();
                    hfConfirmTransferirPersona_ModalPopupExtender.Show();

                    uPanel.Update();

                    return;
                }


                int modeRegistroPersona = getModeRegistroPersona();
                if (modeRegistroPersona == 1)
                {
                    //INSERT Persona

                    string status = registrarPersona();
                    if (status == null)
                    {
                        clearPanelPersona();
                        hfAgregarPersona_ModalPopupExtender.Hide();
                        SDSPersonas.DataBind();
                        gvPersonas.DataBind();
                        uPanel.Update();

                        showMessageSuccess("Se ha registrado exitosamente a la Persona");
                    }
                    else
                    {
                        errors = new List<string>();
                        errors.Add(status);

                        showSummary(errors);
                    }

                }
                else if (modeRegistroPersona == 2)
                {
                    //UPDATE Persona
                    string status = actualizarPersona();
                    if (status == null)
                    {
                        clearPanelPersona();
                        hfAgregarPersona_ModalPopupExtender.Hide();
                        SDSPersonas.DataBind();
                        gvPersonas.DataBind();
                        uPanel.Update();

                        showMessageSuccess("Se ha actualizado exitosamente a la Persona");
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



        private string actualizarPersona()
        {

            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];

            string fecha_retiro;
            if (txtFechaRetiro.Text.Length > 0)
            {
                fecha_retiro = txtFechaRetiro.Text;
            }
            else
            {
                fecha_retiro = null;
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

            string registrado = LogicController.updatePersona(
                                                                        txtRUT.Text,
                                                                        txtNombres.Text,
                                                                        txtApellidos.Text,
                                                                        txtFechaNacimiento.Text,
                                                                        ddlSexo.SelectedValue,
                                                                        txtIDEmpleado.Text,
                                                                        ddlCentro.SelectedValue,
                                                                        ddlStore.SelectedValue,
                                                                        txtLob.Text,
                                                                        txtCargo.Text,
                                                                        ddlClasificacionPersona.SelectedValue,
                                                                        hfRutSupervisor.Value,
                                                                        txtEmail.Text,
                                                                        txtFechaIngreso.Text,
                                                                        fecha_retiro,
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



        protected void btConfirmTransferirPersonaNo_Click(object sender, EventArgs e)
        {
            hfConfirmTransferirPersona_ModalPopupExtender.Hide();
            hfAgregarPersona_ModalPopupExtender.Show();

            uPanel.Update();
        }


        protected void btConfirmTransferirPersonaSi_Click(object sender, EventArgs e)
        {

            int modeRegistroPersona = getModeRegistroPersona();
            if (modeRegistroPersona == 1)
            {
                //INSERT Persona

                string status = registrarPersona();
                if (status == null)
                {
                    hfConfirmTransferirPersona_ModalPopupExtender.Hide();
                    clearPanelPersona();
                    hfAgregarPersona_ModalPopupExtender.Hide();
                    SDSPersonas.DataBind();
                    gvPersonas.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha registrado exitosamente a la Persona");
                }
                else
                {
                    List<string> errors = new List<string>();
                    errors.Add(status);

                    showSummary(errors);

                    hfConfirmTransferirPersona_ModalPopupExtender.Hide();
                    hfAgregarPersona_ModalPopupExtender.Show();

                    uPanel.Update();
                }

            }
            else if (modeRegistroPersona == 2)
            {
                //UPDATE Persona
                string status = actualizarPersona();
                if (status == null)
                {
                    hfConfirmTransferirPersona_ModalPopupExtender.Hide();
                    clearPanelPersona();
                    hfAgregarPersona_ModalPopupExtender.Hide();
                    SDSPersonas.DataBind();
                    gvPersonas.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha actualizado exitosamente a la Persona");
                }
                else
                {
                    List<string> errors = new List<string>();
                    errors.Add(status);

                    showSummary(errors);

                    hfConfirmTransferirPersona_ModalPopupExtender.Hide();
                    hfAgregarPersona_ModalPopupExtender.Show();

                    uPanel.Update();
                }
            }
        }



        private string eliminarPersona(string rut)
        {
            if (rut == null)
            {
                return "No se ha seleccionado una Persona";
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

            string status = LogicController.removePersona(rut, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"], owner);
            if (status == null)
            {
                return null;
            }
            else
            {
                return status;
            }
        }


        private string registrarPersona()
        {

            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];

            string fecha_retiro;
            if (txtFechaRetiro.Text.Length > 0)
            {
                fecha_retiro = txtFechaRetiro.Text;
            }
            else
            {
                fecha_retiro = null;
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

            string registrado = LogicController.registerPersona(
                                                                        txtRUT.Text,
                                                                        txtNombres.Text,
                                                                        txtApellidos.Text,
                                                                        txtFechaNacimiento.Text,
                                                                        ddlSexo.SelectedValue,
                                                                        txtIDEmpleado.Text,
                                                                        ddlCentro.SelectedValue,
                                                                        ddlStore.SelectedValue,
                                                                        txtLob.Text,
                                                                        txtCargo.Text,
                                                                        ddlClasificacionPersona.SelectedValue,
                                                                        hfRutSupervisor.Value,
                                                                        txtEmail.Text,
                                                                        txtFechaIngreso.Text,
                                                                        fecha_retiro,
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

        protected void btCancelarEliminarPersona_Click(object sender, EventArgs e)
        {
            hfEliminarPersona_ModalPopupExtender.Hide();
            uPanel.Update();
        }

        protected void btEliminarPersona_Click(object sender, EventArgs e)
        {

            int index = getIndexRow();
            if ((index >= 0) && (index < gvPersonas.Rows.Count))
            {
                string status = eliminarPersona(((Label)gvPersonas.Rows[index].Cells[0].FindControl("lbRUT")).Text);
                if (status == null)
                {
                    hfEliminarPersona_ModalPopupExtender.Hide();
                    SDSPersonas.DataBind();
                    gvPersonas.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha eliminado la Persona con éxito");
                }
                else
                {
                    showMessageError(status);
                }
            }
            else
            {
                showMessageError("No se puede eliminar la información de la Persona");
            }
        }

        protected void ddlCentro_DataBound(object sender, EventArgs e)
        {
            ddlCentro.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlCentro_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfRutSupervisor.Value = "";
            txtNombreSupervisor.Text = "";
        }

        protected void ddlClasificacionPersona_DataBound(object sender, EventArgs e)
        {
            ddlClasificacionPersona.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlClasificacionPersonaBuscar_DataBound(object sender, EventArgs e)
        {
            ddlClasificacionPersonaBuscar.Items.Insert(0, new ListItem("[TODAS]", ""));
        }


        protected void ddlClasificacionPersona_SelectedIndexChanged(object sender, EventArgs e)
        {
            disableRSPFields(ddlClasificacionPersona.SelectedItem.Value);
        }



        private void disableRSPFields(string nombre_clasificacionpersona)
        {
            if (nombre_clasificacionpersona.ToUpper().Equals("RSP"))
            {
                lbAsteriskCentro.Visible = false;
                ddlCentro.Items.Clear();
                ddlCentro.Items.Add(new ListItem("RSP", "RSP"));
                ddlCentro.SelectedIndex = 0;
                ddlCentro.Enabled = false;

                lbAsteriskStore.Visible = false;
                ddlStore.Items.Clear();
                ddlStore.Items.Add(new ListItem("N/A", "N/A"));
                ddlStore.SelectedIndex = 0;
                ddlStore.Enabled = false;

                lbAsteriskLob.Visible = false;
                txtLob.Text = "";
                txtLob.Enabled = false;

                hfRutSupervisor.Value = "";
                txtNombreSupervisor.Text = "";
                btSeleccionarSupervisor.Enabled = false;
            }
            else
            {
                lbAsteriskCentro.Visible = true;
                SDSCentros.DataBind();
                ddlCentro.DataBind();
                ddlCentro.Enabled = true;

                lbAsteriskStore.Visible = true;
                SDSStores.DataBind();
                ddlStore.DataBind();
                ddlStore.Enabled = true;

                lbAsteriskLob.Visible = true;
                txtLob.Enabled = true;

                btSeleccionarSupervisor.Enabled = true;
            }
        }

    }
}