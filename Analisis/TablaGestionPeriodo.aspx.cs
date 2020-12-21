using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using System.Data;
using NCCSAN.Source.Entity;

namespace NCCSAN.Analisis
{
    public partial class TablaGestionPeriodo : System.Web.UI.Page
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


            if (txtDiasPlanAccion.Text.Length < 1)
            {
                return "No se ha indicado la cantidad de días";
            }


            if (!Utils.validateNumber(txtDiasPlanAccion.Text))
            {
                return "La cantidad de días es inválida";
            }


            return null;
        }




        private string loadIndicadoresAnual(GridView gv, string id_centro, int anio)
        {
            if (gv == null)
            {
                return "Tabla inválida";
            }


            if (id_centro == null)
            {
                return "No se puede recuperar información del centro";
            }


            if (anio < 1)
            {
                return "El año seleccionado es inválido";
            }


            Dictionary<string, int> indicadores = IndicatorBuilder.getGestionPeriodoIndicadoresAnual(id_centro, anio, 1, 12);
            if (indicadores != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("NombreIndicador");
                dt.Columns.Add("ValorIndicador");

                DataRow dr;

                dr = dt.NewRow();

                dt.Rows.Add(dr);

                dt.AcceptChanges();
                gv.DataSource = dt;
                gv.DataBind();



                {//DiasPromedioRespuestaCliente
                    int value = indicadores["DiasPromedioRespuestaCliente"];

                    ((Label)gv.Rows[0].FindControl("lbNombreIndicador")).Text = "Tiempo promedio de respuesta a Cliente";
                    ((Label)gv.Rows[0].FindControl("lbValorIndicador")).Text = Convert.ToString(value) + " días";
                }


                return null;
            }
            else
            {
                return "Error al recuperar la información de indicadores";
            }
        }


        private string loadIndicadoresMensual(GridView gv, string id_centro, int anio, int mes_desde, int mes_hasta, int planaccion_mes, int planaccion_dias)
        {
            if (gv == null)
            {
                return "Tabla inválida";
            }


            if (id_centro == null)
            {
                return "No se puede recuperar información del centro";
            }


            if (anio < 1)
            {
                return "El año seleccionado es inválido";
            }


            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return "El mes de inicio seleccionado es inválido";
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return "El mes de término seleccionado es inválido";
            }

            if (mes_desde > mes_hasta)
            {
                return "El mes de término debe ser igual o posterior al mes de inicio";
            }


            if ((planaccion_mes < 1) || (planaccion_mes > 12))
            {
                return "El mes para planes de acción abiertos es inválido";
            }


            if (planaccion_dias < 0)
            {
                return "La cantidad de dias para planes de acción abiertos debe ser positiva";
            }

            Dictionary<string, int> indicadores = IndicatorBuilder.getGestionPeriodoIndicadoresMensual(id_centro, anio, mes_desde, mes_hasta, planaccion_mes, planaccion_dias);
            if (indicadores != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("NombreIndicador");
                dt.Columns.Add("ValorIndicador");

                DataRow dr;
                for (int i = 0; i < 2; i++)
                {
                    dr = dt.NewRow();

                    dt.Rows.Add(dr);
                }

                dt.AcceptChanges();
                gv.DataSource = dt;
                gv.DataBind();

                {//PlanesAccionAbiertosMasXDias
                    int value = indicadores["PlanesAccionAbiertosMasXDias"];

                    ((Label)gv.Rows[0].FindControl("lbNombreIndicador")).Text = "Planes de acción > " + Convert.ToString(planaccion_dias) + " días abiertos";
                    ((Label)gv.Rows[0].FindControl("lbValorIndicador")).Text = Convert.ToString(value);
                }


                {//DiasPromedioRespuestaCliente
                    int value = indicadores["DiasPromedioRespuestaCliente"];

                    ((Label)gv.Rows[1].FindControl("lbNombreIndicador")).Text = "Tiempo promedio de respuesta a Cliente";
                    ((Label)gv.Rows[1].FindControl("lbValorIndicador")).Text = Convert.ToString(value) + " días";
                }


                return null;
            }
            else
            {
                return "Error al recuperar la información de indicadores";
            }
        }



        private string loadResumen(GridView gv, string id_centro, int anio, int mes_desde, int mes_hasta)
        {

            if (gv == null)
            {
                return "Tabla inválida";
            }


            if (id_centro == null)
            {
                return "No se puede recuperar información del centro";
            }


            if (anio < 1)
            {
                return "El año seleccionado es inválido";
            }


            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return "El mes de inicio seleccionado es inválido";
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return "El mes de término seleccionado es inválido";
            }

            if (mes_desde > mes_hasta)
            {
                return "El mes de término debe ser igual o posterior al mes de inicio";
            }


            Dictionary<string, int[]> resumen = IndicatorBuilder.getGestionPeriodoResumen(id_centro, anio, mes_desde, mes_hasta);
            if (resumen != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Elemento");
                dt.Columns.Add("Abierto");
                dt.Columns.Add("Cerrado");
                dt.Columns.Add("Total");

                DataRow dr;
                for (int i = 0; i < 4; i++)
                {
                    dr = dt.NewRow();

                    dt.Rows.Add(dr);
                }

                dt.AcceptChanges();
                gv.DataSource = dt;
                gv.DataBind();

                {//Evento
                    int[] values = resumen["Evento"];

                    ((Label)gv.Rows[0].FindControl("lbNombreElemento")).Text = "Total Eventos";
                    ((Label)gv.Rows[0].FindControl("lbCantidadAbierto")).Text = Convert.ToString(values[0]);
                    ((Label)gv.Rows[0].FindControl("lbCantidadCerrado")).Text = Convert.ToString(values[1]);
                    ((Label)gv.Rows[0].FindControl("lbCantidadTotal")).Text = Convert.ToString(values[2]);
                }


                {//NoConformidad
                    int[] values = resumen["NoConformidad"];

                    ((Label)gv.Rows[1].FindControl("lbNombreElemento")).Text = "No conformidades";
                    ((Label)gv.Rows[1].FindControl("lbCantidadAbierto")).Text = Convert.ToString(values[0]);
                    ((Label)gv.Rows[1].FindControl("lbCantidadCerrado")).Text = Convert.ToString(values[1]);
                    ((Label)gv.Rows[1].FindControl("lbCantidadTotal")).Text = Convert.ToString(values[2]);
                }


                {//Hallazgo
                    int[] values = resumen["Hallazgo"];

                    ((Label)gv.Rows[2].FindControl("lbNombreElemento")).Text = "Hallazgos";
                    ((Label)gv.Rows[2].FindControl("lbCantidadAbierto")).Text = Convert.ToString(values[0]);
                    ((Label)gv.Rows[2].FindControl("lbCantidadCerrado")).Text = Convert.ToString(values[1]);
                    ((Label)gv.Rows[2].FindControl("lbCantidadTotal")).Text = Convert.ToString(values[2]);
                }


                {//PlanAccion
                    int[] values = resumen["PlanAccion"];

                    ((Label)gv.Rows[3].FindControl("lbNombreElemento")).Text = "Planes de acción";
                    ((Label)gv.Rows[3].FindControl("lbCantidadAbierto")).Text = Convert.ToString(values[0]);
                    ((Label)gv.Rows[3].FindControl("lbCantidadCerrado")).Text = Convert.ToString(values[1]);
                    ((Label)gv.Rows[3].FindControl("lbCantidadTotal")).Text = Convert.ToString(values[2]);
                }

                return null;
            }
            else
            {
                return "Error al recuperar la información de resumen";
            }
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


        protected void btGenerar_Click(object sender, EventArgs e)
        {
            string id_centro;
            if (Session["id_centro"] != null)
            {
                id_centro = (string)Session["id_centro"];
            }
            else
            {
                showMessageError("No se puede recuperar información del centro");

                return;
            }



            if (ddlAnio.SelectedIndex < 1)
            {
                showMessageError("Se debe seleccionar el año");

                return;
            }
            else if (!Utils.validateNumber(ddlAnio.SelectedValue))
            {
                showMessageError("El año seleccionado es inválido");

                return;
            }


            if (ddlMes.SelectedIndex < 1)
            {
                showMessageError("Se debe seleccionar el mes");

                return;
            }
            else if (!Utils.validateNumber(ddlMes.SelectedValue))
            {
                showMessageError("El mes seleccionado es inválido");

                return;
            }

            if (txtDiasPlanAccion.Text.Length < 1)
            {
                showMessageError("Días para planes de acción abiertos inválido");
            }
            else if (!Utils.validateNumber(txtDiasPlanAccion.Text))
            {
                showMessageError("Días para planes de acción abiertos inválido");
            }


            int anio = Convert.ToInt32(ddlAnio.SelectedValue);
            int mes = Convert.ToInt32(ddlMes.SelectedValue);
            int dias = Convert.ToInt32(txtDiasPlanAccion.Text);

            string loaded;

            loaded = loadResumen(gvGestionPeriodoAnualResumen, id_centro, anio, 1, 12);
            if (loaded == null)
            {
                gvGestionPeriodoAnualResumen.HeaderRow.Cells[0].Text = "Año " + ddlAnio.SelectedItem.Text;
            }
            else
            {
                showMessageError(loaded);

                return;
            }

            loaded = loadIndicadoresAnual(gvGestionPeriodoAnualIndicadores, id_centro, anio);
            if (loaded == null)
            {

            }
            else
            {
                showMessageError(loaded);

                return;
            }



            loaded = loadResumen(gvGestionPeriodoMensualResumen, id_centro, anio, mes, mes);
            if (loaded == null)
            {
                gvGestionPeriodoMensualResumen.HeaderRow.Cells[0].Text = ddlMes.SelectedItem.Text + " " + ddlAnio.SelectedItem.Text;
            }
            else
            {
                showMessageError(loaded);

                return;
            }

            loaded = loadIndicadoresMensual(gvGestionPeriodoMensualIndicadores, id_centro, anio, mes, mes, mes, dias);
            if (loaded == null)
            {

            }
            else
            {
                showMessageError(loaded);

                return;
            }
        }


    }
}