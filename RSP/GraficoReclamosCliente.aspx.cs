using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using NCCSAN.Source.Logic;
using System.Drawing;
using NCCSAN.Source.Entity;

namespace NCCSAN.RSP
{
    public partial class GraficoReclamosCliente : System.Web.UI.Page
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


                ddlAnio.Items.Clear();
                ddlAnio.Items.Add(new ListItem("Seleccione..", "0"));
                int currentYear = DateTime.Now.Year;
                for (int year = 2014; year <= currentYear; year++)
                {
                    ddlAnio.Items.Add(new ListItem(Convert.ToString(year), Convert.ToString(year)));
                }
            }

            ChartReclamosCliente.BackColor = ColorTranslator.FromHtml("#EEEEEE");
        }


        private void dumpChart(Chart chartFrom, Chart chartTo)
        {
            chartTo.Titles.Clear();
            chartTo.Series.Clear();
            chartTo.ChartAreas.Clear();
            chartTo.Legends.Clear();

            foreach (Title title in chartFrom.Titles)
            {
                title.BackColor = ColorTranslator.FromHtml("#EEEEEE");
                chartTo.Titles.Add(title);
            }

            foreach (ChartArea area in chartFrom.ChartAreas)
            {
                area.BackColor = ColorTranslator.FromHtml("#EEEEEE");
                chartTo.ChartAreas.Add(area);
            }

            foreach (Series serie in chartFrom.Series)
            {
                chartTo.Series.Add(serie);
            }

            foreach (Legend legend in chartFrom.Legends)
            {
                legend.BackColor = ColorTranslator.FromHtml("#EEEEEE");
                chartTo.Legends.Add(legend);
            }
        }


        protected void btGenerar_Click(object sender, EventArgs e)
        {
            string valid_params = validateParameters();
            if (valid_params != null)
            {
                showMessageWarning(valid_params);

                return;
            }

            string rut_creador = null;
            if (Session["usuario"] != null)
            {
                Usuario u = (Usuario)Session["usuario"];
                rut_creador = u.getRutPersona();
            }


            int anio = Convert.ToInt32(ddlAnio.SelectedItem.Value);


            Chart chart = null;

            if (ddlCentro.SelectedIndex > 0)
            {
                chart = ChartBuilder.getChartReclamosClienteCreadorCentro(rut_creador, ddlCentro.SelectedItem.Value, anio);
            }
            else
            {
                chart = ChartBuilder.getChartReclamosClienteCreador(rut_creador, anio);
            }

            if (chart != null)
            {
                dumpChart(chart, ChartReclamosCliente);
            }
            else
            {
                showMessageError("Se ha producido un error al generar el gráfico");
            }
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

        protected void ddlCentro_DataBound(object sender, EventArgs e)
        {
            ddlCentro.Items.Insert(0, new ListItem("[Todos]", ""));
        }
    }
}