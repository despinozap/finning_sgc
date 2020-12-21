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
    public partial class CentroClientes : System.Web.UI.Page
    {

        private static readonly int MODO_REGISTRO_CLIENTE_CREAR = 1;
        private static readonly int MODO_REGISTRO_CLIENTE_ACTUALIZAR = 2;

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
                Session.Remove("ModoRegistroCliente");
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


        private void setModeRegistroCliente(int mode)
        {
            Session["ModoRegistroCliente"] = mode;
        }


        private int getModeRegistroCliente()
        {
            if (Session["ModoRegistroCliente"] == null)
                return -1;
            else
                return (int)Session["ModoRegistroCliente"];
        }


        protected void gvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            setIndexRow(-1);

            if (e.CommandName.Equals("AgregarCliente"))
            {
                setModeRegistroCliente(MODO_REGISTRO_CLIENTE_CREAR);

                lbNombreCliente.Visible = false;
                ddlNombreCliente.Visible = true;
                SDSNoCentroClientes.DataBind();
                ddlNombreCliente.DataBind();
                txtAgregarClienteEmail.Text = "";
                ltSummary.Text = "";

                hfAgregarCliente_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EliminarCliente"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvClientes.Rows.Count))
                {
                    setIndexRow(index);
                    lbMessageEliminarCliente.Text = "Se eliminará el Cliente \"" + ((Label)gvClientes.Rows[index].Cells[0].FindControl("lbNombre")).Text + "\". ¿Desea continuar?";
                    hfEliminarCliente_ModalPopupExtender.Show();
                }
            }
            else if (e.CommandName.Equals("EditarCliente"))
            {
                setModeRegistroCliente(MODO_REGISTRO_CLIENTE_ACTUALIZAR);

                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvClientes.Rows.Count))
                {
                    setIndexRow(index);
                    Label lbNombre = (Label)gvClientes.Rows[index].Cells[0].FindControl("lbNombre");
                    Label lbEmail = (Label)gvClientes.Rows[index].Cells[1].FindControl("lbEmail");

                    ddlNombreCliente.Visible = false;
                    lbNombreCliente.Visible = true;
                    lbNombreCliente.Text = lbNombre.Text;
                    if (lbEmail.Text.Length > 0)
                    {
                        txtAgregarClienteEmail.Text = lbEmail.Text;
                    }
                    else
                    {
                        txtAgregarClienteEmail.Text = "";
                    }

                    ltSummary.Text = "";

                    hfAgregarCliente_ModalPopupExtender.Show();

                }
            }
        }


        protected void btAgregarClienteCancelar_Click(object sender, EventArgs e)
        {
            hfAgregarCliente_ModalPopupExtender.Hide();
            uPanel.Update();
        }

        protected void btCancelarEliminarCliente_Click(object sender, EventArgs e)
        {
            hfEliminarCliente_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void btAgregarClienteAceptar_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                ltSummary.Text = "No se puede recuperar la información del Centro";

                return;
            }

            string id_centro = (string)Session["id_centro"];


            string email;
            if (txtAgregarClienteEmail.Text.Length < 1)
            {
                email = null;
            }
            else
            {
                email = txtAgregarClienteEmail.Text;
            }

            if (Session["usuario"] == null)
            {
                ltSummary.Text = "Error al recuperar información del Usuario";

                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];

            int modeRegistroCliente = getModeRegistroCliente();
            if (modeRegistroCliente == 1)
            {
                //INSERT CentroCliente

                if (ddlNombreCliente.SelectedIndex < 1)
                {
                    ltSummary.Text = "No se ha seleccionado el Cliente";

                    return;
                }


                string status = LogicController.registerCentroCliente(ddlNombreCliente.SelectedValue, email, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    hfAgregarCliente_ModalPopupExtender.Hide();
                    SDSCentroClientes.DataBind();
                    gvClientes.DataBind();
                    SDSNoCentroClientes.DataBind();
                    ddlNombreCliente.DataBind();
                    uPanel.Update();
                    Session.Remove("ModoRegistroCliente");

                    showMessageSuccess("Se ha agregado el Cliente con éxito");
                }
                else
                {
                    ltSummary.Text = "Error: " + status;
                }
            }
            else if (modeRegistroCliente == 2)
            {
                //UPDATE CentroCliente

                int index = getIndexRow();
                if ((index >= 0) && (index < gvClientes.Rows.Count))
                {
                    string status = LogicController.updateCentroCliente(((Label)gvClientes.Rows[index].Cells[0].FindControl("lbNombre")).Text, email, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                    if (status == null)
                    {
                        hfAgregarCliente_ModalPopupExtender.Hide();
                        SDSCentroClientes.DataBind();
                        gvClientes.DataBind();
                        uPanel.Update();

                        Session.Remove("ModoRegistroCliente");
                        showMessageSuccess("Se ha actualizado el Cliente con éxito");
                    }
                    else
                    {
                        ltSummary.Text = "Error: " + status;
                    }
                }
                else
                {
                    ltSummary.Text = "Error: No se puede recuperar el nombre del Cliente";
                }
            }
        }


        protected void btEliminarCliente_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                showMessageError("No se puede recuperar la información del Centro");

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

            string id_centro = (string)Session["id_centro"];

            int index = getIndexRow();
            if ((index >= 0) && (index < gvClientes.Rows.Count))
            {
                string status = LogicController.removeCentroCliente(((Label)gvClientes.Rows[index].Cells[0].FindControl("lbNombre")).Text, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
                if (status == null)
                {
                    hfAgregarCliente_ModalPopupExtender.Hide();
                    SDSCentroClientes.DataBind();
                    gvClientes.DataBind();
                    uPanel.Update();

                    showMessageSuccess("Se ha eliminado el Cliente con éxito");
                }
                else
                {
                    uPanel.Update();

                    showMessageError(status);
                }
            }
            else
            {
                uPanel.Update();

                showMessageError("No se puede recuperar el nombre del Cliente");
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

        protected void ddlNombreCliente_DataBound(object sender, EventArgs e)
        {
            ddlNombreCliente.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

    }
}