using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using Microsoft.Reporting.WebForms;
using System.Data;

namespace NCCSAN.Reportes
{
    public partial class ListarNoConformidades : System.Web.UI.Page
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


        protected void gvEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if ((index >= 0) && (index < gvEventos.Rows.Count))
            {
                string codigo_evento = gvEventos.Rows[index].Cells[0].Text;

                if (e.CommandName.Equals("ExportarPDF"))
                {
                    if (!downloadReport(codigo_evento, REPORT_TYPE_PDF))
                    {
                        showMessageError("Se ha producido un error al generar el reporte en PDF");
                    }
                }
                else if (e.CommandName.Equals("ExportarExcel"))
                {
                    if (!downloadReport(codigo_evento, REPORT_TYPE_EXCEL))
                    {
                        showMessageError("Se ha producido un error al generar el reporte en Excel");
                    }
                }
            }
            else
            {
                showMessageError("El Plan de Acción seleccionado no existe");
            }
        }


        private bool downloadReport(string codigo_evento, int report_type)
        {
            if (codigo_evento == null)
            {
                return false;
            }
            else if (codigo_evento.Length < 1)
            {
                return false;
            }

            if (report_type < 0)
            {
                return false;
            }



            hfCodigoEvento.Value = codigo_evento;
            SDSEvento.DataBind();
            SDSEvaluacion.DataBind();
            SDSPlanAccion.DataBind();
            SDSAccionCorrectiva.DataBind();

            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view;

            view = (DataView)SDSEvento.Select(args);
            DataTable dtEvento = view.ToTable();

            view = (DataView)SDSEvaluacion.Select(args);
            DataTable dtEvaluacion = view.ToTable();

            view = (DataView)SDSPlanAccion.Select(args);
            DataTable dtPlanAccion = view.ToTable();

            view = (DataView)SDSAccionCorrectiva.Select(args);
            DataTable dtAccionCorrectiva = view.ToTable();

            view = (DataView)SDSVerificacion.Select(args);
            DataTable dtVerificacion = view.ToTable();

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = "Reportes/ReportPlanAccion.rdlc";
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DTEvento", dtEvento));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DTEvaluacion", dtEvaluacion));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DTPlanAccion", dtPlanAccion));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DTAccionCorrectiva", dtAccionCorrectiva));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DTVerificacion", dtVerificacion));
            viewer.LocalReport.Refresh();

            switch (report_type)
            {
                case 0:
                    {
                        exportReportToPDF(viewer, codigo_evento);
                        break;
                    }

                case 1:
                    {
                        exportReportToExcel(viewer, codigo_evento);
                        break;
                    }

                default:
                    {

                        return false;
                    }
            }

            return true;
        }



        private void exportReportToPDF(ReportViewer viewer, string codigo_evento)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = "pdf";
            string filename = "ReportPlanAccion_" + codigo_evento;

            byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + "." + extension);
            Response.BinaryWrite(bytes); // create the file
            Response.Flush(); // send it to the client to download
        }


        private void exportReportToExcel(ReportViewer viewer, string codigo_evento)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = "xls";
            string filename = "ReportPlanAccion_" + codigo_evento;

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

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("[Todos]", ""));
        }
    }
}