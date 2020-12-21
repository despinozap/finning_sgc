using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Data;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.PlanesAccion
{
    public partial class ListarPlanesAccionEnCurso : System.Web.UI.Page
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


                if (Session["usuario"] != null)
                {
                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();

                    if ((rol.Equals("Jefe Calidad")) || (rol.Equals("Coordinador")))
                    {
                        gvEventos.Columns[7].Visible = true;
                    }
                    else
                    {
                        gvEventos.Columns[7].Visible = false;
                    }
                }

                Session.Remove("CodigoPlanAccionSeleccionado");

                {//Filtrar búsqueda por parámetro
                    if (Request.QueryString["ce"] != null)
                    {
                        string codigo_evento = Request.QueryString["ce"];
                        txtCodigoWO.Text = codigo_evento;

                        SDSPlanesAccionEnCurso.DataBind();
                        gvEventos.DataBind();
                    }
                }
            }
        }

        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            ddlCliente.Items.Insert(0, new ListItem("[Todos]", ""));
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


        protected void gvEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetallePlanAccion"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    Session["CodigoPlanAccionSeleccionado"] = gvEventos.Rows[index].Cells[0].Text;
                    Session["PreviousPagePlanAccion"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/PlanesAccion/DetallePlanAccion.aspx");
                }
            }
            else if (e.CommandName.Equals("EditarPlanAccion"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    if (Session["usuario"] == null)
                    {
                        return;
                    }

                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();

                    if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                    {
                        showMessageError("No tienes privilegios para editar Planes de Acción");
                        return;
                    }


                    Session["CodigoPlanAccionSeleccionado"] = gvEventos.Rows[index].Cells[0].Text;
                    Session["PreviousPagePlanAccion"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/PlanesAccion/EditarPlanAccion.aspx");
                }
            }
            else if (e.CommandName.Equals("CerrarPlanAccion"))
            {

                if (Session["usuario"] == null)
                {
                    return;
                }

                Usuario usuario = (Usuario)Session["usuario"];
                string rol = usuario.getNombreRol();

                if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                {
                    showMessageError("No tienes privilegios para cerrar Planes de Acción");
                    return;
                }

                string codigo = (string)e.CommandArgument;
                hfCodigoPlanAccionCerrar.Value = codigo;

                hfConfirmCerrarPlanAccion_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EliminarPlanAccion"))
            {

                if (Session["usuario"] == null)
                {
                    return;
                }

                Usuario usuario = (Usuario)Session["usuario"];
                string rol = usuario.getNombreRol();

                if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                {
                    showMessageError("No tienes privilegios para eliminar Planes de Acción");
                    return;
                }

                string codigo = (string)e.CommandArgument;
                hfCodigoPlanAccionEliminar.Value = codigo;

                hfConfirmEliminarPlanAccion_ModalPopupExtender.Show();
            }
        }


        protected void gvEventos_DataBound(object sender, EventArgs e)
        {
            if (gvEventos.Rows.Count > 0)
            {
                pnOpcionesLista.Visible = true;

                Label lbProgreso;
                Panel pnEjecutado;
                int progreso;
                ImageButton ibCerrarPlanAccion;
                ImageButton ibEliminarPlanAccion;
                for (int i = 0; i < gvEventos.Rows.Count; i++)
                {
                    lbProgreso = (Label)gvEventos.Rows[i].FindControl("lbProgreso");
                    progreso = Convert.ToInt32(lbProgreso.Text);

                    lbProgreso.Text = Convert.ToString(progreso) + "%";

                    pnEjecutado = (Panel)gvEventos.Rows[i].FindControl("pnEjecutado");
                    pnEjecutado.Width = System.Web.UI.WebControls.Unit.Parse(Convert.ToString(progreso) + "%");

                    ibCerrarPlanAccion = (ImageButton)gvEventos.Rows[i].FindControl("ibCerrarPlanAccion");
                    ibCerrarPlanAccion.CommandArgument = gvEventos.Rows[i].Cells[0].Text;

                    ibEliminarPlanAccion = (ImageButton)gvEventos.Rows[i].FindControl("ibEliminarPlanAccion");
                    ibEliminarPlanAccion.CommandArgument = gvEventos.Rows[i].Cells[0].Text;

                    ibCerrarPlanAccion.Visible = false;


                    if (progreso >= 75)
                    {
                        pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#00CC00");

                        if (progreso == 100)
                        {
                            ibCerrarPlanAccion.Visible = true;
                        }

                    }
                    else if (progreso >= 50)
                    {
                        pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#F9ED60");
                    }
                    else if (progreso >= 25)
                    {
                        pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9933");
                    }
                    else
                    {
                        pnEjecutado.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF3300");
                    }

                }
            }
            else
            {
                pnOpcionesLista.Visible = false;
            }

        }


        protected void ibExportToExcel_Click(object sender, ImageClickEventArgs e)
        {
            gvEventos.GridLines = GridLines.Both;
            gvEventos.HeaderStyle.Font.Bold = true;
            gvEventos.EnableViewState = false;
            gvEventos.AllowPaging = false;
            gvEventos.DataBind();

            int column_count = gvEventos.Columns.Count;
            for (int i = 6; i < column_count; i++)
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
            dr[0] = "Área";
            dr[1] = ddlArea.SelectedItem.Text;
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
            ExportToExcel("PlanesAccionEnCurso", controls);

            gvEventos.AllowPaging = true;
            gvEventos.DataBind();

            for (int i = 6; i < column_count; i++)
            {
                gvEventos.Columns[i].Visible = true;
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

        protected void btConfirmCerrarPlanAccionSi_Click(object sender, EventArgs e)
        {
            string codigo = hfCodigoPlanAccionCerrar.Value;
            if (LogicController.planAccionExists(codigo) < 1)
            {
                showMessageError("El Plan de Acción seleccionado no existe");

                return;
            }

            string status = cerrarPlanAccion(codigo);
            if (status == null)
            {
                showMessageSuccess("Se cerró exitosamente el Plan de Acción");
            }
            else
            {
                showMessageError(status);
            }

            hfConfirmCerrarPlanAccion_ModalPopupExtender.Hide();
            SDSPlanesAccionEnCurso.DataBind();
            gvEventos.DataBind();

        }


        private string cerrarPlanAccion(string codigo_evento)
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

            string status = LogicController.closePlanAccion(codigo_evento, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
            if (status == null)
            {
                return null;
            }
            else
            {
                return status;
            }
        }



        protected void btConfirmCerrarPlanAccionNo_Click(object sender, EventArgs e)
        {
            hfConfirmCerrarPlanAccion_ModalPopupExtender.Hide();
        }


        protected void btConfirmEliminarPlanAccionSi_Click(object sender, EventArgs e)
        {
            string codigo = hfCodigoPlanAccionEliminar.Value;
            if (LogicController.planAccionExists(codigo) < 1)
            {
                showMessageError("El Plan de Acción seleccionado no existe");

                return;
            }

            string status = eliminarPlanAccion(codigo);
            if (status == null)
            {
                showMessageSuccess("Se eliminó exitosamente el Plan de Acción");
            }
            else
            {
                showMessageError(status);
            }

            hfConfirmEliminarPlanAccion_ModalPopupExtender.Hide();
            SDSPlanesAccionEnCurso.DataBind();
            gvEventos.DataBind();
        }


        private string eliminarPlanAccion(string codigo_evento)
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

            string status = LogicController.removePlanAccion(codigo_evento, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"]);
            if (status == null)
            {
                return null;
            }
            else
            {
                return status;
            }
        }



        protected void btConfirmEliminarPlanAccionNo_Click(object sender, EventArgs e)
        {
            hfConfirmEliminarPlanAccion_ModalPopupExtender.Hide();
        }

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("[Todas]", ""));
        }
    }
}