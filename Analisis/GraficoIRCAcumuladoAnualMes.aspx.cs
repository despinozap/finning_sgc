﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;
using System.Drawing;

namespace NCCSAN.Graficos
{
    public partial class IRCAcumuladoAnualMes : System.Web.UI.Page
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

            ChartIRCAcumulado.BackColor = ColorTranslator.FromHtml("#EEEEEE");
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
            int mes = Convert.ToInt32(ddlMes.SelectedItem.Value);
            int periodo = Convert.ToInt32(txtPeriodo.Text);

            int a1 = Convert.ToInt32(txtA1.Text);
            int b1 = Convert.ToInt32(txtB1.Text);
            int a2 = Convert.ToInt32(txtA2.Text);
            int b2 = Convert.ToInt32(txtB2.Text);
            int a3 = Convert.ToInt32(txtA3.Text);
            int b3 = Convert.ToInt32(txtB3.Text);

            Chart chart = ChartBuilder.getChartIRCAcumuladoAnualMes(id_centro, anio, mes, periodo, a1, a2, a3, b1, b2, b3);
            if (chart != null)
            {
                dumpChart(chart, ChartIRCAcumulado);
            }
            else
            {
                showMessageError("Se ha producido un error al generar el gráfico");
            }
        }


        private string validateParameters()
        {
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
                return "No se ha seleccionado el mes a destacar";
            }

            if (txtPeriodo.Text.Length < 1)
            {
                return "No se ha indicado el periodo en cantidad de meses";
            }

            if (!Utils.validateNumber(txtPeriodo.Text))
            {
                return "El periodo debe ser un número en cantidad de meses";
            }

            int periodo = Convert.ToInt32(txtPeriodo.Text);
            if (periodo < 1)
            {
                return "El periodo debe ser igual o mayor a 1";
            }
            else if (periodo > 12)
            {
                return "El periodo debe ser menor o igual a 12";
            }

            if (!Utils.validateNumber(txtA1.Text))
            {
                return "El valor A1 debe ser numérico";
            }

            if (!Utils.validateNumber(txtB1.Text))
            {
                return "El valor B1 debe ser numérico";
            }

            if (!Utils.validateNumber(txtA2.Text))
            {
                return "El valor A2 debe ser numérico";
            }

            if (!Utils.validateNumber(txtB2.Text))
            {
                return "El valor B2 debe ser numérico";
            }

            if (!Utils.validateNumber(txtA3.Text))
            {
                return "El valor A3 debe ser numérico";
            }

            if (!Utils.validateNumber(txtB3.Text))
            {
                return "El valor B3 debe ser numérico";
            }


            int a1 = Convert.ToInt32(txtA1.Text);
            int b1 = Convert.ToInt32(txtB1.Text);
            int a2 = Convert.ToInt32(txtA2.Text);
            int b2 = Convert.ToInt32(txtB2.Text);
            int a3 = Convert.ToInt32(txtA3.Text);
            int b3 = Convert.ToInt32(txtB3.Text);

            if (a1 < 1)
            {
                return "El valor de A1 debe ser mayor o igual a 1";
            }

            if (a2 < 1)
            {
                return "El valor de A2 debe ser mayor o igual a 1";
            }

            if (a3 < 1)
            {
                return "El valor de A3 debe ser mayor o igual a 1";
            }

            if (b1 < 1)
            {
                return "El valor de B1 debe ser mayor o igual a 1";
            }

            if (b2 < 1)
            {
                return "El valor de B2 debe ser mayor o igual a 1";
            }

            if (b3 < 1)
            {
                return "El valor de B3 debe ser mayor o igual a 1";
            }

            if (a1 >= a2)
            {
                return "El valor A1 debe ser menor a A2";
            }

            if (a2 >= a3)
            {
                return "El valor A2 debe ser menor a A3";
            }


            if (b1 >= b2)
            {
                return "El valor B1 debe ser menor a B2";
            }

            if (b2 >= b3)
            {
                return "El valor B2 debe ser menor a B3";
            }

            if (a1 < b1)
            {
                return "El valor A1 debe ser mayor igual a B1";
            }

            if (a2 < b2)
            {
                return "El valor A2 debe ser mayor igual a B2";
            }

            if (a3 < b3)
            {
                return "El valor A3 debe ser mayor igual a B3";
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