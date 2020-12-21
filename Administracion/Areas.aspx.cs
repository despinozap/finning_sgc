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
    public partial class Areas : System.Web.UI.Page
    {
        private static readonly int MODO_REGISTRO_AREA_CREAR = 1;
        private static readonly int MODO_REGISTRO_AREA_ACTUALIZAR = 2;

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
                Session.Remove("ModoRegistroArea");
            }
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


        private void setModeRegistroArea(int mode)
        {
            Session["ModoRegistroArea"] = mode;
        }


        private int getModeRegistroArea()
        {
            if (Session["ModoRegistroArea"] == null)
                return -1;
            else
                return (int)Session["ModoRegistroArea"];
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



        protected void btRegistrarArea_Click(object sender, EventArgs e)
        {
            if (txtNombreArea.Text.Length < 1)
            {
                showMessageError("No se ha indicado el nombre de Área");

                uPanel.Update();
                return;
            }

            if (Session["id_centro"] == null)
            {
                showMessageError("No se puede recuperar la información del Centro");

                uPanel.Update();
                return;
            }

            string id_centro = (string)Session["id_centro"];


            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar información del Usuario");

                uPanel.Update();
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];


            string rutJefe = hfRutJefeArea.Value;
            if (rutJefe.Length < 1)
            {
                showMessageError("No se ha seleccionado el Jefe de Área");

                uPanel.Update();
                return;
            }


            int modeRegistroArea = getModeRegistroArea();
            if (modeRegistroArea == 1)
            {
                //INSERT CentroArea
                string status = LogicController.registerCentroArea(txtNombreArea.Text, rutJefe, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    hfAgregarArea_ModalPopupExtender.Hide();
                    gvAreas.DataBind();
                    uPanel.Update();

                    Session.Remove("ModoRegistroArea");
                    showMessageSuccess("Se ha agregado el Área con éxito");
                }
                else
                {
                    showMessageError(status);

                    uPanel.Update();
                }
            }
            else if (modeRegistroArea == 2)
            {
                //UPDATE CentroArea
                int index = getIndexRow();
                if ((index >= 0) && (index < gvAreas.Rows.Count))
                {
                    string status = LogicController.updateCentroArea(((Label)gvAreas.Rows[index].Cells[0].FindControl("lbNombre")).Text, txtNombreArea.Text, rutJefe, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                    if (status == null)
                    {
                        hfAgregarArea_ModalPopupExtender.Hide();
                        gvAreas.DataBind();
                        uPanel.Update();

                        Session.Remove("ModoRegistroArea");
                        showMessageSuccess("Se ha actualizado el Área con éxito");
                    }
                    else
                    {
                        showMessageError(status);

                        uPanel.Update();
                    }
                }
                else
                {
                    showMessageError("No se puede recuperar el nombre del Área");

                    uPanel.Update();
                }
            }

        }


        protected void btCancelarArea_Click(object sender, EventArgs e)
        {
            txtNombreJefeArea.Text = "";
            hfRutJefeArea.Value = "";

            hfAgregarArea_ModalPopupExtender.Hide();
            uPanel.Update();
        }



        protected void btBuscarJefeArea_Click(object sender, EventArgs e)
        {
            txtNombreJefeArea.Text = "";
            hfRutJefeArea.Value = "";

            hfAgregarArea_ModalPopupExtender.Hide();
            hfBuscarPersona_ModalPopupExtender.Show();
            uPanel.Update();
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

            if (e.CommandName.Equals("SetJefeArea"))
            {
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                PersonaInfo jefe = LogicController.getPersonaInfo(rut);

                if (jefe == null)
                {
                    txtNombreJefeArea.Text = "";
                    hfRutJefeArea.Value = "";

                    showBuscarPersonaMessageError("Error al recuperar la información de \"" + nombre + "\"");
                }
                else
                {
                    txtNombreJefeArea.Text = jefe.getNombre();
                    hfRutJefeArea.Value = jefe.getRut();

                    txtBuscarPersonaApellido.Text = "";
                    ltBuscarPersonaSummary.Text = "";

                    hfBuscarPersona_ModalPopupExtender.Hide();
                    hfAgregarArea_ModalPopupExtender.Show();
                    uPanel.Update();
                }
            }
        }



        protected void btBuscarPersonaCancelar_Click(object sender, EventArgs e)
        {
            hfBuscarPersona_ModalPopupExtender.Hide();
            hfAgregarArea_ModalPopupExtender.Show();
            uPanel.Update();
        }


        protected void gvAreas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            setIndexRow(-1);

            if (e.CommandName.Equals("AgregarArea"))
            {
                setModeRegistroArea(MODO_REGISTRO_AREA_CREAR);

                txtNombreArea.Text = "";
                txtNombreJefeArea.Text = "";
                hfRutJefeArea.Value = "";

                hfAgregarArea_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("GestionarSubareas"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvAreas.Rows.Count))
                {
                    setIndexRow(index);
                    Session["nombre_area"] = ((Label)gvAreas.Rows[index].Cells[0].FindControl("lbNombre")).Text;

                    Response.Redirect("~/Administracion/Subareas.aspx", true);
                }
            }
            else if (e.CommandName.Equals("EliminarArea"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvAreas.Rows.Count))
                {
                    setIndexRow(index);
                    lbMessageEliminarArea.Text = "Si se elimina el Área \"" + ((Label)gvAreas.Rows[index].Cells[0].FindControl("lbNombre")).Text + "\" se eliminará también los datos de sub-áreas y supervisores. ¿Desea continuar?";
                    hfEliminarArea_ModalPopupExtender.Show();
                }
            }
            else if (e.CommandName.Equals("EditarArea"))
            {

                setModeRegistroArea(MODO_REGISTRO_AREA_ACTUALIZAR);

                if (Session["id_centro"] == null)
                {
                    showMessageError("No se puede recuperar la información del Centro");

                    uPanel.Update();
                    return;
                }


                string id_centro = (string)Session["id_centro"];
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvAreas.Rows.Count))
                {
                    setIndexRow(index);

                    Label lbNombre = (Label)gvAreas.Rows[index].Cells[0].FindControl("lbNombre");

                    PersonaInfo jefe = LogicController.getCentroAreaJefe(lbNombre.Text, id_centro);
                    if (jefe == null)
                    {
                        showMessageError("No se puede recuperar la información del Jefe de Área");

                        uPanel.Update();
                        return;
                    }


                    txtNombreArea.Text = lbNombre.Text;
                    txtNombreJefeArea.Text = jefe.getNombre();
                    hfRutJefeArea.Value = jefe.getRut();

                    hfAgregarArea_ModalPopupExtender.Show();

                }
            }
        }


        protected void btEliminarArea_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                showMessageError("No se puede recuperar la información del Centro");

                uPanel.Update();
                return;
            }

            string id_centro = (string)Session["id_centro"];


            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar información del Usuario");

                uPanel.Update();
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];


            int index = getIndexRow();
            if ((index >= 0) && (index < gvAreas.Rows.Count))
            {
                string status = LogicController.removeCentroArea(((Label)gvAreas.Rows[index].Cells[0].FindControl("lbNombre")).Text, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    hfAgregarArea_ModalPopupExtender.Hide();
                    SDSAreas.DataBind();
                    gvAreas.DataBind();

                    showMessageSuccess("Se ha eliminado el Área con éxito");
                    uPanel.Update();
                }
                else
                {
                    showMessageError(status);

                    uPanel.Update();
                }
            }
            else
            {
                showMessageError("No se puede recuperar el nombre del Cliente");

                uPanel.Update();
            }

        }

        protected void btCancelarEliminarArea_Click(object sender, EventArgs e)
        {
            hfEliminarArea_ModalPopupExtender.Hide();
            uPanel.Update();
        }
    }
}