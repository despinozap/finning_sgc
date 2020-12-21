using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using System.Data;
using NCCSAN.Source.Entity;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;

namespace NCCSAN.Analisis
{
    public partial class BonificacionMesual : System.Web.UI.Page
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
                ddlMes.Items.Add(new ListItem("Seleccione..", "0"));
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
                ddlAnio.Items.Add(new ListItem("Seleccione..", "0"));
                int currentYear = DateTime.Now.Year;
                for (int year = 2014; year <= currentYear; year++)
                {
                    ddlAnio.Items.Add(new ListItem(Convert.ToString(year), Convert.ToString(year)));
                }
            }

        }



        protected void btDescargar_Click(object sender, EventArgs e)
        {
            string valid_params = validateParameters();
            if (valid_params != null)
            {
                showMessageWarning(valid_params);

                return;
            }

            string id_centro;
            if (Session["id_centro"] != null)
            {
                id_centro = (string)Session["id_centro"];
            }
            else
            {
                id_centro = null;
            }

            string nombre_centro = LogicController.getNombreCentro(id_centro);
            if (nombre_centro == null)
            {
                showMessageError("No se puede recuperar información del Centro");

                return;
            }

            int anio = Convert.ToInt32(ddlAnio.SelectedItem.Value);
            int mes = Convert.ToInt32(ddlMes.SelectedItem.Value);


            DataTable dt = DataExporter.getDataTableResumenInvolucradosMensual(id_centro, anio, mes);

            if (dt == null)
            {
                showMessageError("No se puede recuperar la información solicitada");
            }
            else if (dt.Rows.Count > 1)
            {
                ExportDataToExcel(dt, "ResumenInvolucradosMensual", "ResumenInvolucradosMensual", nombre_centro);
            }
            else
            {
                showMessageWarning("No se han encontrado involucrados en el mes seleccionado");
            }
        }



        private void ExportDataToExcel(DataTable dtData, string title_document, string title_file, string nombre_centro)
        {
            List<Control> controls = new List<Control>();

            DataTable dtFiltro = new DataTable();
            dtFiltro.Columns.Add("Filtros");
            dtFiltro.Columns.Add("Valor");
            DataRow dr;

            dr = dtFiltro.NewRow();
            dr[0] = "Centro";
            dr[1] = nombre_centro;
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

            for (int i = 0; i < gvFiltro.Rows.Count; i++)
            {
                gvFiltro.Rows[i].Cells[1].HorizontalAlign = HorizontalAlign.Center;
            }

            controls.Add(gvFiltro);


            GridView gvData = new GridView();
            gvData.DataSource = dtData;
            gvData.DataBind();


            gvData.ShowHeader = true;
            gvData.HeaderStyle.Font.Bold = true;

            for (int i = 0; i < gvData.HeaderRow.Cells.Count; i++)
            {
                gvData.HeaderRow.Cells[i].BackColor = System.Drawing.ColorTranslator.FromHtml("#666666");
                gvData.HeaderRow.Cells[i].ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

                gvData.Rows[gvData.Rows.Count - 1].Cells[i].Font.Bold = true;
            }

            for (int i = 0; i < gvData.Rows.Count; i++)
            {
                gvData.Rows[i].Cells[gvData.HeaderRow.Cells.Count - 1].Font.Bold = true;
            }



            /*
            gvData.RowStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
            gvData.RowStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");
            */

            Label lbTitle = new Label();
            lbTitle.Text = HttpUtility.HtmlEncode(title_document);
            lbTitle.Font.Size = 24;
            //controls.Add(lbTitle);
            controls.Add(gvData);

            ExportToExcel("DataExport_" + title_file, controls);
        }



        private void ExportToExcel(string prefixFilename, List<Control> controls)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Page pagina = new Page();
            Page pagina2 = new Page();
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

            pagina2.EnableEventValidation = false;
            pagina2.DesignerInitialize();
            pagina2.Controls.Add(form);

            foreach (Control control in controls)
                form.Controls.Add(control);

            pagina.RenderControl(htw);
            pagina2.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }



        private string validateParameters()
        {
            if (Session["id_centro"] == null)
            {
                return "No se puede recuperar la información del Centro";
            }


            if (ddlAnio.SelectedIndex < 1)
            {
                return "No se ha seleccionado el año";
            }


            if (!Utils.validateNumber(ddlAnio.SelectedItem.Value))
            {
                return "El año seleccionado es inválido";
            }

            if (ddlMes.SelectedIndex < 1)
            {
                return "No se ha seleccionado el mes";
            }

            if (!Utils.validateNumber(ddlMes.SelectedItem.Value))
            {
                return "El mes seleccionado es inválido";
            }

            return null;
        }


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }


        private void showMessageWarning(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxWarning", "<script type=\"text/javascript\">showMessageWarning('" + message + "');</script>", false);
        }
    }
}