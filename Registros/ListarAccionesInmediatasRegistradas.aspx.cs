using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;

namespace NCCSAN.Registros
{
    public partial class ListarAccionesInmediatasIngresadas : System.Web.UI.Page
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

                ddlMes.Items.Clear();
                ddlMes.Items.Add(new ListItem("[Todos]", ""));
                ddlMes.Items.Add(new ListItem("Enero", "01"));
                ddlMes.Items.Add(new ListItem("Febrero", "02"));
                ddlMes.Items.Add(new ListItem("Marzo", "03"));
                ddlMes.Items.Add(new ListItem("Abril", "04"));
                ddlMes.Items.Add(new ListItem("Mayo", "05"));
                ddlMes.Items.Add(new ListItem("Junio", "06"));
                ddlMes.Items.Add(new ListItem("Julio", "07"));
                ddlMes.Items.Add(new ListItem("Agosto", "08"));
                ddlMes.Items.Add(new ListItem("Septiembre", "09"));
                ddlMes.Items.Add(new ListItem("Octubre", "10"));
                ddlMes.Items.Add(new ListItem("Noviembre", "11"));
                ddlMes.Items.Add(new ListItem("Diciembre", "12"));

                ddlAnio.Items.Clear();
                ddlAnio.Items.Add(new ListItem("[Todos]", ""));
                int currentYear = DateTime.Now.Year;
                for (int year = 2014; year <= currentYear; year++)
                {
                    ddlAnio.Items.Add(new ListItem(Convert.ToString(year), Convert.ToString(year)));
                }

                Session.Remove("CodigoEventoSeleccionado");
            }
        }

        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            ddlCliente.Items.Insert(0, new ListItem("[Todos]", ""));
        }


        protected void ibClearCodigoWO_Click(object sender, ImageClickEventArgs e)
        {
            txtCodigoWO.Text = "";
        }


        protected void ddlAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            setFechaValue();
        }


        protected void ddlMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            setFechaValue();
        }


        private void setFechaValue()
        {
            if (ddlAnio.SelectedIndex > 0)
            {
                if (ddlMes.SelectedIndex > 0)
                {
                    hfFecha.Value = "-" + ddlMes.SelectedValue + "-" + ddlAnio.SelectedValue;
                }
                else
                {
                    hfFecha.Value = "-" + ddlAnio.SelectedValue;
                }
            }
            else
            {
                if (ddlMes.SelectedIndex > 0)
                {
                    hfFecha.Value = "-" + ddlMes.SelectedValue + "-";
                }
                else
                {
                    hfFecha.Value = "";
                }
            }
        }


        protected void gvEventos_DataBound(object sender, EventArgs e)
        {
            if (gvEventos.Rows.Count > 0)
            {
                pnOpcionesLista.Visible = true;
            }
            else
            {
                pnOpcionesLista.Visible = false;
            }
        }


        private void ExportToExcel(string prefixFilename, List<Control> controls)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Page pagina = new Page();
            HtmlForm form = new HtmlForm();

            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string FileName = prefixFilename + "(" + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToLongTimeString() + ")" + ".xls";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            pagina.EnableEventValidation = false;
            pagina.DesignerInitialize();
            pagina.Controls.Add(form);

            foreach (Control control in controls)
                form.Controls.Add(control);

            pagina.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }


        protected void ibExportToExcel_Click(object sender, ImageClickEventArgs e)
        {
            gvEventos.GridLines = GridLines.Both;
            gvEventos.HeaderStyle.Font.Bold = true;
            gvEventos.EnableViewState = false;
            gvEventos.AllowPaging = false;
            gvEventos.DataBind();

            int column_count = gvEventos.Columns.Count;
            for (int i = 9; i < column_count; i++)
            {
                gvEventos.Columns[i].Visible = false;
            }

            List<Control> controls = new List<Control>();
            DataTable dtFiltro = new DataTable();
            dtFiltro.Columns.Add("Filtros");
            dtFiltro.Columns.Add("Valor");
            DataRow dr;

            dr = dtFiltro.NewRow();
            dr[0] = "Código o W/O";
            dr[1] = txtCodigoWO.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Cliente";
            dr[1] = ddlCliente.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Año";
            dr[1] = ddlAnio.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Mes";
            dr[1] = ddlMes.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            dtFiltro.AcceptChanges();
            GridView gvFiltro = new GridView();
            gvFiltro.DataSource = dtFiltro;
            gvFiltro.DataBind();

            gvFiltro.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#000000");
            gvFiltro.HeaderStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
            gvFiltro.RowStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
            gvFiltro.RowStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");


            controls.Add(gvFiltro);

            controls.Add(gvEventos);
            ExportToExcel("AccionesInmediatasRegistradas", controls);

            gvEventos.AllowPaging = true;
            gvEventos.DataBind();

            for (int i = 9; i < column_count; i++)
            {
                gvEventos.Columns[i].Visible = true;
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


        protected void gvEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("EditarAccionInmediata"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    if (Session["usuario"] == null)
                    {
                        showMessageError("No se puede recuperar la información del Usuario");

                        return;
                    }

                    //Filtro de eliminación
                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();
                    if (!rol.Equals("Jefe Calidad"))
                    {
                        showMessageError("No tienes privilegios para editar la Acción Inmediata");

                        return;
                    }

                    Session["CodigoEventoSeleccionado"] = gvEventos.Rows[index].Cells[0].Text;
                    Response.Redirect("~/Registros/EditarAccionInmediata.aspx", true);
                }
            }
            else if (e.CommandName.Equals("EliminarAccionInmediata"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    if (Session["usuario"] == null)
                    {
                        showMessageError("No se puede recuperar la información del Usuario");

                        return;
                    }

                    //Filtro de eliminación
                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();
                    if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")) && (!rol.Equals("Inspector")))
                    {
                        showMessageError("No tienes privilegios para eliminar la Acción Inmediata");

                        return;
                    }

                    Session["CodigoEventoSeleccionado"] = gvEventos.Rows[index].Cells[0].Text;
                    hfConfirmEliminar_ModalPopupExtender.Show();
                }
            }
            else if (e.CommandName.Equals("DetalleAccionInmediata"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    Session["CodigoEventoSeleccionado"] = gvEventos.Rows[index].Cells[0].Text;
                    Session["PreviousPageAccionInmediata"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Registros/DetalleAccionInmediata.aspx", true);
                }
            }
        }

        protected void btConfirmEliminarNo_Click(object sender, EventArgs e)
        {
            hfConfirmEliminar_ModalPopupExtender.Hide();

            uPanel.Update();
        }


        private string eliminarAccionInmediata(string codigo_evento)
        {
            if(codigo_evento == null)
            {
                return "Error al recuperar la información del Evento";
            }


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


            string registrado = LogicController.removeAccionInmediata
                                                                    (
                                                                        codigo_evento,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );
            if (registrado == null)
            {
                return null;
            }
            else
            {
                return "Error al eliminar la Acción Inmediata: " + registrado;
            }
        }


        protected void btConfirmEliminarSi_Click(object sender, EventArgs e)
        {
            if (Session["CodigoEventoSeleccionado"] == null)
            {
                showMessageError("No se ha seleccionado una Acción Inmediata");

                return;
            }

            string codigo_evento = (string)Session["CodigoEventoSeleccionado"];
            string status = eliminarAccionInmediata(codigo_evento);
            if (status == null)
            {
                showMessageSuccess("Acción Inmediata eliminada exitosamente");

                SDSEventos.DataBind();
                gvEventos.DataBind();
            }
            else
            {
                showMessageError(status);
            }

            uPanel.Update();
        }
    }
}