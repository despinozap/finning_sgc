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
    public partial class Clientes : System.Web.UI.Page
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


        protected void gvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            setIndexRow(-1);

            if (e.CommandName.Equals("AgregarCliente"))
            {
                txtAgregarClienteNombre.Text = "";
                ltSummary.Text = "";

                hfAgregarCliente_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("GestionarCuenta"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvClientes.Rows.Count))
                {
                    setIndexRow(index);
                    Label lbNombre = (Label)gvClientes.Rows[index].Cells[0].FindControl("lbNombre");

                    if (lbNombre.Text.Length < 1)
                    {
                        uPanel.Update();

                        showMessageError("No se puede recuperar la información del Cliente");

                        return;
                    }

                    ltSummary.Text = "";

                    Session["cliente"] = lbNombre.Text;

                    Response.Redirect("~/Configuracion/ClienteCuenta.aspx", true);

                }
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
            if (txtAgregarClienteNombre.Text.Length < 1)
            {
                ltSummary.Text = "No se ha indicado el nombre del Cliente";

                return;
            }

            if (Session["usuario"] == null)
            {
                ltSummary.Text = "Error al recuperar información del Usuario";

                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];

            //INSERT CentroCliente
            string status = LogicController.registerCliente(txtAgregarClienteNombre.Text, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);

            if (status == null)
            {
                hfAgregarCliente_ModalPopupExtender.Hide();
                SDSClientes.DataBind();
                gvClientes.DataBind();
                uPanel.Update();

                showMessageSuccess("Se ha agregado el Cliente con éxito");
            }
            else
            {
                ltSummary.Text = "Error: " + status;
            }
        }


        protected void btEliminarCliente_Click(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar información del Usuario");

                uPanel.Update();
                return;
            }

            Usuario usuario = (Usuario)Session["usuario"];


            int index = getIndexRow();
            if ((index >= 0) && (index < gvClientes.Rows.Count))
            {
                //Eliminar Cliente
                string status = LogicController.removeCliente(((Label)gvClientes.Rows[index].Cells[0].FindControl("lbNombre")).Text, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);

                if (status == null)
                {
                    hfAgregarCliente_ModalPopupExtender.Hide();
                    SDSClientes.DataBind();
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
    }
}