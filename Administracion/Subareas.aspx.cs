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
    public partial class Subareas : System.Web.UI.Page
    {
        private static readonly int MODO_REGISTRO_SUBAREA_CREAR = 1;
        private static readonly int MODO_REGISTRO_SUBAREA_ACTUALIZAR = 2;


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

                if ((Session["nombre_area"] != null) && (Session["id_centro"] != null))
                {
                    string nombre_area = (string)Session["nombre_area"];
                    string id_centro = (string)Session["id_centro"];

                    if ((nombre_area != null) && (id_centro != null))
                    {
                        lbNombreArea.Text = nombre_area;
                        updateGVSupervisores();
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


        private void setModeRegistroSubarea(int mode)
        {
            Session["ModoRegistroSubarea"] = mode;
        }


        private int getModeRegistroSubarea()
        {
            if (Session["ModoRegistroSubarea"] == null)
                return -1;
            else
                return (int)Session["ModoRegistroSubarea"];
        }



        private void updateGVSupervisores()
        {
            if (ddlSubareas.SelectedIndex < 1)
            {
                gvSupervisores.DataSource = null;
                gvSupervisores.DataBind();
                gvSupervisores.Visible = false;

                return;
            }
            else
            {
                gvSupervisores.Visible = true;
            }


            DataTable dt = getDTPersonasFormat();

            if(Session["id_centro"] == null)
            {
                gvSupervisores.DataSource = dt;
                gvSupervisores.DataBind();

                return;
            }

            string id_centro = (string)Session["id_centro"];
            if(id_centro == null)
            {
                gvSupervisores.DataSource = dt;
                gvSupervisores.DataBind();

                return;
            }


            if(Session["nombre_area"] == null)
            {
                gvSupervisores.DataSource = dt;
                gvSupervisores.DataBind();

                return;
            }


            string nombre_area = (string)Session["nombre_area"];
            if(nombre_area == null)
            {
                gvSupervisores.DataSource = dt;
                gvSupervisores.DataBind();

                return;
            }


            List<PersonaInfo> listSupervisores = LogicController.getListSupervisoresCentroAreaSubarea(id_centro, nombre_area, ddlSubareas.SelectedValue);
            if (listSupervisores == null)
            {
                gvSupervisores.DataSource = dt;
                gvSupervisores.DataBind();

                return;
            }


            DataRow dr;

            for (int i = 0; i < listSupervisores.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Button();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvSupervisores.DataSource = dt;
            gvSupervisores.DataBind();


            for (int i = 0; i < listSupervisores.Count; i++)
            {
                ((Label)gvSupervisores.Rows[i].FindControl("lbRUT")).Text = listSupervisores[i].getRut();
                ((Label)gvSupervisores.Rows[i].FindControl("lbNombre")).Text = listSupervisores[i].getNombre();
                ((Label)gvSupervisores.Rows[i].FindControl("lbNombreCentro")).Text = listSupervisores[i].getNombreCentro();
                ((Label)gvSupervisores.Rows[i].FindControl("lbCargo")).Text = listSupervisores[i].getCargo();
                ((Label)gvSupervisores.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(listSupervisores[i].getAntiguedad());
            }
        }



        private static DataTable getDTPersonasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("RUT");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Centro");
            dt.Columns.Add("Cargo");
            dt.Columns.Add("Antiguedad");
            dt.Columns.Add("Opciones");

            dt.AcceptChanges();

            return dt;
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

            if (Session["nombre_area"] == null)
            {
                showMessageError("No se puede recuperar la información del Área");

                uPanel.Update();
                return;
            }

            string nombre_area = (string)Session["nombre_area"];


            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar información del Usuario");

                uPanel.Update();
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];


            int index = getIndexRow();
            if ((index >= 0) && (index < gvSubareas.Rows.Count))
            {
                string status = LogicController.removeCentroAreaSubarea(nombre_area, ((Label)gvSubareas.Rows[index].Cells[0].FindControl("lbNombre")).Text, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    hfAgregarSubarea_ModalPopupExtender.Hide();
                    SDSSubareas.DataBind();
                    gvSubareas.DataBind();
                    ddlSubareas.DataBind();

                    showMessageSuccess("Se ha eliminado el Sub-área con éxito");
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


        protected void btRegistrarSubarea_Click(object sender, EventArgs e)
        {
            if (txtNombreSubarea.Text.Length < 1)
            {
                showMessageError("No se ha indicado el nombre de Sub-área");

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


            if (Session["nombre_area"] == null)
            {
                showMessageError("No se puede recuperar la información del Área");

                uPanel.Update();
                return;
            }


            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar información del Usuario");

                uPanel.Update();
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];

            string nombre_area = (string)Session["nombre_area"];


            int modeRegistroSubarea = getModeRegistroSubarea();
            if (modeRegistroSubarea == 1)
            {
                //INSERT CentroAreaSubarea
                string status = LogicController.registerCentroAreaSubarea(nombre_area, txtNombreSubarea.Text, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    hfAgregarSubarea_ModalPopupExtender.Hide();
                    SDSSubareas.DataBind();
                    gvSubareas.DataBind();
                    ddlSubareas.DataBind();
                    uPanel.Update();

                    Session.Remove("ModoRegistroSubarea");
                    showMessageSuccess("Se ha agregado el Sub-área con éxito");
                }
                else
                {
                    showMessageError(status);

                    uPanel.Update();
                }
            }
            else if (modeRegistroSubarea == 2)
            {
                //UPDATE CentroAreaSubarea
                int index = getIndexRow();
                if ((index >= 0) && (index < gvSubareas.Rows.Count))
                {
                    string status = LogicController.updateCentroAreaSubarea(((Label)gvSubareas.Rows[index].Cells[0].FindControl("lbNombre")).Text, txtNombreSubarea.Text, nombre_area, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                    if (status == null)
                    {
                        hfAgregarSubarea_ModalPopupExtender.Hide();
                        SDSSubareas.DataBind();
                        gvSubareas.DataBind();
                        ddlSubareas.DataBind();
                        uPanel.Update();

                        Session.Remove("ModoRegistroSubarea");
                        showMessageSuccess("Se ha actualizado el Sub-área con éxito");
                    }
                    else
                    {
                        showMessageError(status);

                        uPanel.Update();
                    }
                }
                else
                {
                    showMessageError("No se puede recuperar el nombre del Sub-área");

                    uPanel.Update();
                }
            }
        }


        protected void btCancelarSubarea_Click(object sender, EventArgs e)
        {
            txtNombreSubarea.Text = "";

            hfAgregarSubarea_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void btCancelarEliminarSubarea_Click(object sender, EventArgs e)
        {
            hfEliminarSubarea_ModalPopupExtender.Hide();
            uPanel.Update();
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

        protected void gvSubareas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            setIndexRow(-1);

            if (e.CommandName.Equals("AgregarSubarea"))
            {
                setModeRegistroSubarea(MODO_REGISTRO_SUBAREA_CREAR);

                txtNombreSubarea.Text = "";

                hfAgregarSubarea_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EliminarSubarea"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvSubareas.Rows.Count))
                {
                    setIndexRow(index);
                    lbMessageEliminarSubarea.Text = "Si se elimina el Sub-área \"" + ((Label)gvSubareas.Rows[index].Cells[0].FindControl("lbNombre")).Text + "\" se eliminará también los datos de supervisores asociados. ¿Desea continuar?";
                    hfEliminarSubarea_ModalPopupExtender.Show();
                }
            }
            else if (e.CommandName.Equals("EditarSubarea"))
            {

                setModeRegistroSubarea(MODO_REGISTRO_SUBAREA_ACTUALIZAR);

                if (Session["id_centro"] == null)
                {
                    showMessageError("No se puede recuperar la información del Centro");

                    uPanel.Update();
                    return;
                }


                string id_centro = (string)Session["id_centro"];
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvSubareas.Rows.Count))
                {
                    setIndexRow(index);

                    Label lbNombre = (Label)gvSubareas.Rows[index].Cells[0].FindControl("lbNombre");

                    txtNombreSubarea.Text = lbNombre.Text;


                    hfAgregarSubarea_ModalPopupExtender.Show();

                }
            }
        }


        protected void ddlSubareas_DataBound(object sender, EventArgs e)
        {
            ddlSubareas.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlSubareas_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateGVSupervisores();
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

        private void showBuscarPersonaMessage(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span>" + msg + "</span>";
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


            if (Session["id_centro"] == null)
            {
                showMessageError("No se puede recuperar la información del Centro");

                uPanel.Update();
                return;
            }

            string id_centro = (string)Session["id_centro"];


            if (Session["nombre_area"] == null)
            {
                showMessageError("No se puede recuperar la información del Área");

                uPanel.Update();
                return;
            }

            string nombre_area = (string)Session["nombre_area"];


            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar información del Usuario");

                uPanel.Update();
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];


            if (e.CommandName.Equals("AddSupervisor"))
            {
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;

                string status = LogicController.addSupervisorCentroAreaSubarea(rut, id_centro, nombre_area, ddlSubareas.SelectedValue, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    updateGVSupervisores();
                    upSupervisores.Update();

                    showMessageSuccess("Se ha agregado el Supervisor con éxito");

                    uPanel.Update();
                    return;
                }
                else
                {
                    showMessageError(status);

                    uPanel.Update();
                    return;
                }
            }


            txtBuscarPersonaApellido.Text = "";
            ltBuscarPersonaSummary.Text = "";

            hfBuscarPersona_ModalPopupExtender.Hide();
        }


        protected void btBuscarPersonaCerrar_Click(object sender, EventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";
            hfBuscarPersona_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void gvSupervisores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("BuscarSupervisor"))
            {
                ((ButtonField)gvBuscarPersonas.Columns[0]).CommandName = "AddSupervisor";
                ltBuscarPersonaSummary.Text = "";
                txtBuscarPersonaApellido.Text = "";
                SDSBuscarPersonas.DataBind();
                gvBuscarPersonas.DataBind();

                hfBuscarPersona_ModalPopupExtender.Show();

                uPanel.Update();
                return;
            }
            else if (e.CommandName.Equals("DelSupervisor"))
            {

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


                if (Session["id_centro"] == null)
                {
                    showMessageError("No se puede recuperar la información del Centro");

                    uPanel.Update();
                    return;
                }

                string id_centro = (string)Session["id_centro"];


                if (Session["nombre_area"] == null)
                {
                    showMessageError("No se puede recuperar la información del Área");

                    uPanel.Update();
                    return;
                }

                string nombre_area = (string)Session["nombre_area"];


                if (Session["usuario"] == null)
                {
                    showMessageError("Error al recuperar información del Usuario");

                    uPanel.Update();
                    return;
                }

                Usuario usuario = (Usuario)Session["usuario"];


                string rut = ((Label)gvSupervisores.Rows[index].Cells[0].FindControl("lbRUT")).Text;

                string status = LogicController.removeSupervisorCentroAreaSubarea(rut, id_centro, nombre_area, ddlSubareas.SelectedValue, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    updateGVSupervisores();
                    upSupervisores.Update();

                    showMessageSuccess("Se ha eliminado el Supervisor del Sub-área");

                    uPanel.Update();
                    return;
                }
                else
                {
                    showMessageError(status);

                    uPanel.Update();
                    return;
                }
            }
        }

        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/Administracion/Areas.aspx", true);
        }
    }
}