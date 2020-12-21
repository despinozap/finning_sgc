using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Data;

namespace NCCSAN.Analisis
{
    public partial class BaseDatos : System.Web.UI.Page
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


        private void ExportDataToExcel(DataTable dtData, string title_document, string title_file)
        {
            List<Control> controls = new List<Control>();
            GridView gvData = new GridView();
            gvData.DataSource = dtData;
            gvData.DataBind();

            gvData.ShowHeader = true;
            gvData.HeaderStyle.Font.Bold = true;

            for (int i = 0; i < gvData.HeaderRow.Cells.Count; i++)
            {
                gvData.HeaderRow.Cells[i].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
                gvData.HeaderRow.Cells[i].ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");
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


        private void ExportFullDataToExcel(List<DataTable> listDtData, List<string> listTitleDocument)
        {
            List<Control> controls = new List<Control>();
            GridView gvData;
            Label lbTitle;
            Label lbSeparator;

            for (int i = 0; i < listDtData.Count; i++)
            {
                gvData = new GridView();
                gvData.DataSource = listDtData[i];
                gvData.DataBind();

                gvData.HeaderStyle.Font.Bold = true;

                /*
                gvData.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#000000");
                gvData.HeaderStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
                gvData.RowStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
                gvData.RowStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");
                */

                lbTitle = new Label();
                lbTitle.Text = HttpUtility.HtmlEncode(listTitleDocument[i]);
                lbTitle.Font.Size = 24;
                //controls.Add(lbTitle);
                controls.Add(gvData);
                lbSeparator = new Label();
                lbSeparator.Text = "<br /><br />";
                lbSeparator.Font.Size = 24;
                //controls.Add(lbSeparator);
            }

            ExportToExcel("DataExport_FullDB", controls);
        }


        protected void ibExportPlanesAccionToExcel_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];

            DataTable dt = DataExporter.getDataTableXPlanAccion(id_centro);
            if (dt != null)
            {
                ExportDataToExcel(dt, "Planes de Acción", "PlanAccion");
            }
        }

        protected void ibExportEvaluacionesToExcel_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];

            DataTable dt = DataExporter.getDataTableXEvaluacion(id_centro);
            if (dt != null)
            {
                ExportDataToExcel(dt, "Evaluaciones", "Evaluacion");
            }
        }

        protected void ibExportEventosToExcel_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];

            DataTable dt = DataExporter.getDataTableXEvento(id_centro);
            if (dt != null)
            {
                ExportDataToExcel(dt, "Eventos", "Evento");
            }
        }



        protected void ibExportAccionesInmediatasToExcel_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];

            DataTable dt = DataExporter.getDataTableXAccionInmediata(id_centro);
            if (dt != null)
            {
                ExportDataToExcel(dt, "Acciones Inmediatas", "AccionInmediata");
            }
        }

        protected void ibExportAccionesCorrectivasToExcel_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];

            DataTable dt = DataExporter.getDataTableXAccionCorrectiva(id_centro);
            if (dt != null)
            {
                ExportDataToExcel(dt, "Acciones Correctivas", "AccionCorrectiva");
            }
        }


        protected void ibExportFullToExcel_Click(object sender, EventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];

            List<DataTable> listDtData = new List<DataTable>();
            List<string> listTitleDocument = new List<string>();

            DataTable dt;


            dt = DataExporter.getDataTableXEvento(id_centro);
            if (dt != null)
            {
                listDtData.Add(dt);
                listTitleDocument.Add("Eventos");
            }


            dt = DataExporter.getDataTableXEvaluacion(id_centro);
            if (dt != null)
            {
                listDtData.Add(dt);
                listTitleDocument.Add("Evaluaciones");
            }


            dt = DataExporter.getDataTableXAccionInmediata(id_centro);
            if (dt != null)
            {
                listDtData.Add(dt);
                listTitleDocument.Add("Acciones Inmediatas");
            }


            dt = DataExporter.getDataTableXPlanAccion(id_centro);
            if (dt != null)
            {
                listDtData.Add(dt);
                listTitleDocument.Add("Planes de Acción");
            }


            dt = DataExporter.getDataTableXAccionCorrectiva(id_centro);
            if (dt != null)
            {
                listDtData.Add(dt);
                listTitleDocument.Add("Acciones Correctivas");
            }


            ExportFullDataToExcel(listDtData, listTitleDocument);
        }
    }
}