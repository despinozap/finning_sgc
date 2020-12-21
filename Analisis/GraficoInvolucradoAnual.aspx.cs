using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using NCCSAN.Source.Entity;

namespace NCCSAN.Analisis
{
    public partial class GraficoInvolucradoAnual : System.Web.UI.Page
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

            ChartInvolucradoAnual.BackColor = ColorTranslator.FromHtml("#EEEEEE");
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

            string rut_involucrado = hfRutInvolucrado.Value;


            int anio = Convert.ToInt32(ddlAnio.SelectedItem.Value);


            Chart chart = ChartBuilder.getChartInvolucradoAnual(rut_involucrado, anio);
            if (chart != null)
            {
                dumpChart(chart, ChartInvolucradoAnual);
            }
            else
            {
                showMessageError("Se ha producido un error al generar el gráfico");
            }
        }


        private string validateParameters()
        {
            if ((txtNombreInvolucrado.Text.Length < 1) || (hfRutInvolucrado.Value.Length < 1))
            {
                return "No se ha seleccionado el involucrado";
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

        protected void btBuscarInvolucrado_Click(object sender, EventArgs e)
        {
            hfBuscarPersona_ModalPopupExtender.Show();
            uPanel.Update();
        }


        private void showBuscarPersonaMessage(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span>" + msg + "</span>";
        }


        private void showBuscarPersonaMessageSuccess(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #339933;\">" + msg + "</span>";
        }


        private void showBuscarPersonaMessageError(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #FF0000;\">" + msg + "</span>";
        }


        protected void btBuscarPersonaLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarPersonaApellido.Text = "";
        }



        protected void SDSBuscarPersonas_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                showBuscarPersonaMessage(Convert.ToString(e.AffectedRows) + " registros coinciden con la búsqueda");
            else
                showBuscarPersonaMessage("No se han encontrado coincidencias con la búsqueda");
        }


        protected void gvBuscarPersonas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";

            int index = -1;
            try
            {
                index = Convert.ToInt32(e.CommandArgument);
            }
            catch (Exception ex)
            {
                return;
            }

            if (index < 0)
                return;

            if (e.CommandName.Equals("SetInvolucrado"))
            {
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                PersonaInfo responsable = LogicController.getPersonaInfo(rut);

                if (responsable == null)
                {
                    txtNombreInvolucrado.Text = "";
                    hfRutInvolucrado.Value = "";

                    showBuscarPersonaMessageError("Error al recuperar la información de \"" + nombre + "\"");
                }
                else
                {
                    txtNombreInvolucrado.Text = responsable.getNombre();
                    hfRutInvolucrado.Value = responsable.getRut();

                    txtBuscarPersonaApellido.Text = "";
                    ltBuscarPersonaSummary.Text = "";

                    hfBuscarPersona_ModalPopupExtender.Hide();
                    uPanel.Update();
                }
            }
        }



        protected void btBuscarPersonaCancelar_Click(object sender, EventArgs e)
        {
            hfBuscarPersona_ModalPopupExtender.Hide();
            upBuscarPersona.Update();
            uPanel.Update();
        }
    }
}