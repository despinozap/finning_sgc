using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;

namespace NCCSAN.Graficos
{
    public partial class GestionAcumulada : System.Web.UI.Page
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

            ChartGestionAcumulada.BackColor = ColorTranslator.FromHtml("#EEEEEE");
        }



        private string loadResponsablesArea(string nombre_area)
        {
            if (Session["id_centro"] == null)
            {
                return "Error al recuperar información del Centro";
            }

            string id_centro = (string)Session["id_centro"];

            List<PersonaInfo> listSupervisores = LogicController.getListSupervisoresCentroArea(id_centro, nombre_area);
            if (listSupervisores == null)
            {
                return "Error al recuperar información de los Supervisores";
            }

            ddlResponsable.Items.Clear();
            foreach (PersonaInfo supervisor in listSupervisores)
            {
                ddlResponsable.Items.Add(new ListItem("[Supervisor] " + supervisor.getNombre(), supervisor.getRut()));
            }

            PersonaInfo jefe = LogicController.getCentroAreaJefe(nombre_area, id_centro);
            if (jefe == null)
            {
                return "Error al recuperar información del Jefe de Área";
            }
            else
            {
                ddlResponsable.Items.Add(new ListItem("[Jefe área] " + jefe.getNombre(), jefe.getRut()));
            }

            ddlResponsable.Items.Insert(0, new ListItem("[Todos]", ""));

            return null;
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

            string id_centro;
            if (Session["id_centro"] != null)
            {
                id_centro = (string)Session["id_centro"];
            }
            else
            {
                id_centro = null;
            }

            int anio = Convert.ToInt32(ddlAnio.SelectedItem.Value);


            Chart chart = ChartBuilder.getChartGestionAcumulada(id_centro, ddlArea.SelectedValue, ddlResponsable.SelectedValue, anio, chbEventoAbierto.Checked, chbEventoCerrado.Checked, chbPlanAccionAbierto.Checked, chbPlanAccionCerrado.Checked);

            if (chart != null)
            {
                dumpChart(chart, ChartGestionAcumulada);
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


        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("[Todas]", ""));

            ddlResponsable.Items.Clear();
            ddlResponsable.Items.Insert(0, new ListItem("[Todos]", ""));
        }



        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlArea.SelectedIndex > 0)
            {
                string msg = loadResponsablesArea(ddlArea.SelectedValue);
                if (msg != null)
                {
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }
            else
            {
                ddlResponsable.Items.Clear();
                ddlResponsable.Items.Insert(0, new ListItem("[Todos]", ""));
            }
        }


        protected void ddlResponsable_DataBound(object sender, EventArgs e)
        {
            ddlResponsable.Items.Insert(0, new ListItem("[Todos]", ""));
        }
    }
}