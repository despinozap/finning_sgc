using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace NCCSAN.Reportes
{
    public partial class ReporteHallazgos : System.Web.UI.Page
    {

        private static readonly int REPORT_TYPE_PDF = 0;
        private static readonly int REPORT_TYPE_EXCEL = 1;

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



        private bool downloadReport(int report_type)
        {
            if (report_type < 0)
            {
                return false;
            }


            SDSHallazgos.DataBind();

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view;

            view = (DataView)SDSHallazgos.Select(args);
            DataTable dtEvento = view.ToTable();

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = "Reportes/ReportHallazgos.rdlc";
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DTEvento", dtEvento));
            viewer.LocalReport.Refresh();

            switch (report_type)
            {
                case 0:
                    {
                        exportReportToPDF(viewer, txtWO.Text, txtFecha.Text);
                        break;
                    }

                case 1:
                    {
                        exportReportToExcel(viewer, txtWO.Text, txtFecha.Text);
                        break;
                    }

                default:
                    {

                        return false;
                    }
            }

            return true;
        }


        private void exportReportToPDF(ReportViewer viewer, string work_order, string fecha)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = "pdf";
            string filename = "ReportHallazgos_" + work_order + "_" + fecha;

            byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + "." + extension);
            Response.BinaryWrite(bytes); // create the file
            Response.Flush(); // send it to the client to download
        }


        private void exportReportToExcel(ReportViewer viewer, string work_order, string fecha)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = "xls";
            string filename = "ReportHallazgos_" + work_order + "_" + fecha;

            byte[] bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + "." + extension);
            Response.BinaryWrite(bytes); // create the file
            Response.Flush(); // send it to the client to download
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


        private void showMessageWarning(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxWarning", "<script type=\"text/javascript\">showMessageWarning('" + message + "');</script>", false);
        }


        protected void btDescargar_Click(object sender, EventArgs e)
        {

            if (txtWO.Text.Length < 1)
            {
                showMessageWarning("No se ha seleccionado la W/O a reportar");

                return;
            }

            if (txtFecha.Text.Length < 1)
            {
                showMessageWarning("No se ha seleccionado la fecha de reporte");

                return;
            }


            if (!downloadReport(REPORT_TYPE_PDF))
            {
                showMessageError("Se ha producido un error al generar el reporte en PDF");
            }
        }
    }
}