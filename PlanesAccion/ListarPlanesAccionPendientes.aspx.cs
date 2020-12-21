using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Data;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.PlanesAccion
{
    public partial class ListarPlanesAccionPendientes : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                {//Acceso del Usuario a la Página
                    if (Session["usuario"] == null)
                    {
                        Session["LinkedPage"] = Request.Url.ToString();

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
                        gvEventos.Columns[6].Visible = true;
                    }
                    else
                    {
                        gvEventos.Columns[6].Visible = false;
                    }
                }

                Session.Remove("CodigoEventoSeleccionado");

                {//Filtrar búsqueda por parámetro
                    if (Request.QueryString["ce"] != null)
                    {
                        string codigo_evento = Request.QueryString["ce"];
                        txtCodigoWO.Text = codigo_evento;

                        SDSPlanesAccionPendientes.DataBind();
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
            if (e.CommandName.Equals("IngresarPlanAccion"))
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

                    if((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                    {
                        showMessageError("No tienes privilegios para ingresar Planes de Acción");
                        return;
                    }


                    Session["CodigoEventoSeleccionado"] = gvEventos.Rows[index].Cells[1].Text;
                    Session["PreviousPage"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/PlanesAccion/RegistrarPlanAccion.aspx");
                }
            }
        }



        protected void gvEventos_DataBound(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                string msg = "Error al recuperar tu información de Usuario";
                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
            }

            Usuario usuario = (Usuario)Session["usuario"];


            if (gvEventos.Rows.Count > 0)
            {
                pnOpcionesLista.Visible = true;
            }
            else
            {
                pnOpcionesLista.Visible = false;
            }


            ConfigEmailSender ces = LogicController.getConfigEmailSender("Plan de acción pendiente", usuario.getNombreRol(), usuario.getIDCentro());
            if (ces == null)
            {
                return;
            }


            Image imgEstadoIcono;
            string dias_en_cursoS;
            int dias_en_curso;
            for (int i = 0; i < gvEventos.Rows.Count; i++)
            {
                imgEstadoIcono = (Image)gvEventos.Rows[i].Cells[0].FindControl("imgEstadoIcono");
                dias_en_cursoS = HttpUtility.HtmlDecode(gvEventos.Rows[i].Cells[6].Text);

                if (Utils.validateNumber(dias_en_cursoS))
                {
                    dias_en_curso = Convert.ToInt32(dias_en_cursoS);

                    if (dias_en_curso > ces.getDiasLimite())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                        imgEstadoIcono.ToolTip = "Registro de Plan de Acción vencido";
                    }
                    else if (dias_en_curso == ces.getDiasLimite())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                        imgEstadoIcono.ToolTip = "El registro de Plan de Acción vence hoy";
                    }
                    else if (dias_en_curso >= ces.getDiasAlerta())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";

                        if ((ces.getDiasLimite() - dias_en_curso) == 1)
                        {
                            imgEstadoIcono.ToolTip = "El registro de Plan de Acción vence en 1 día";
                        }
                        else
                        {
                            imgEstadoIcono.ToolTip = "El registro de Plan de Acción vence en " + Convert.ToString(ces.getDiasLimite() - dias_en_curso) + " días";
                        }
                    }
                }
            }
        }


        protected void ibExportToExcel_Click(object sender, ImageClickEventArgs e)
        {
            gvEventos.GridLines = GridLines.Both;
            gvEventos.HeaderStyle.Font.Bold = true;
            gvEventos.EnableViewState = false;
            gvEventos.AllowPaging = false;
            gvEventos.DataBind();

            gvEventos.Columns[0].Visible = false;
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
            ExportToExcel("PlanesAccionPendientes", controls);

            gvEventos.AllowPaging = true;
            gvEventos.DataBind();

            gvEventos.Columns[0].Visible = true;
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

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("[Todas]", ""));
        }
    }
}