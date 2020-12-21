using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using System.Data;
using NCCSAN.Source.Entity;

namespace NCCSAN.RSP
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

            if (Session["usuario"] == null)
            {
                return "No se puede recuperar la información del Usuario";
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



        private string loadResumenCreador(GridView gv, string rut_creador, int anio, int mes_desde, int mes_hasta)
        {

            if (gv == null)
            {
                return "Tabla inválida";
            }


            if (rut_creador == null)
            {
                return "No se puede recuperar información del creador";
            }

            PersonaInfo persona = LogicController.getPersonaInfo(rut_creador);
            if (persona == null)
            {
                return "No se puede recuperar información del creador";
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


            Dictionary<string, int[]> resumen = null;

            if (ddlCentro.SelectedIndex > 0)
            {
                if (ddlCliente.SelectedIndex > 0)
                {
                    resumen = IndicatorBuilder.getGestionPeriodoCreadorResumenCentroCliente(rut_creador, ddlCentro.SelectedItem.Value, ddlCliente.SelectedItem.Value, anio, mes_desde, mes_hasta);
                }
                else
                {
                    resumen = IndicatorBuilder.getGestionPeriodoCreadorResumenCentro(rut_creador, ddlCentro.SelectedItem.Value, anio, mes_desde, mes_hasta);
                }
            }
            else
            {
                resumen = IndicatorBuilder.getGestionPeriodoCreadorResumen(rut_creador, anio, mes_desde, mes_hasta);
            }


            if (resumen != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Elemento");
                dt.Columns.Add("Abierto");
                dt.Columns.Add("Cerrado");
                dt.Columns.Add("Total");

                DataRow dr;
                for (int i = 0; i < 1; i++)
                {
                    dr = dt.NewRow();

                    dt.Rows.Add(dr);
                }

                dt.AcceptChanges();
                gv.DataSource = dt;
                gv.DataBind();

                {//Evento
                    int[] values = resumen["Evento"];

                    ((Label)gv.Rows[0].FindControl("lbNombreElemento")).Text = "Evento registrado";
                    ((Label)gv.Rows[0].FindControl("lbCantidadAbierto")).Text = Convert.ToString(values[0]);
                    ((Label)gv.Rows[0].FindControl("lbCantidadCerrado")).Text = Convert.ToString(values[1]);
                    ((Label)gv.Rows[0].FindControl("lbCantidadTotal")).Text = Convert.ToString(values[2]);
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
            string rut_creador;

            if (Session["usuario"] != null)
            {
                Usuario u = (Usuario)Session["usuario"];

                rut_creador = u.getRutPersona();
            }
            else
            {
                showMessageError("No se puede recuperar la información del Usuario");

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


            int anio = Convert.ToInt32(ddlAnio.SelectedValue);
            int mes = Convert.ToInt32(ddlMes.SelectedValue);

            string loaded;

            loaded = loadResumenCreador(gvGestionPeriodoAnualResumen, rut_creador, anio, 1, 12);
            if (loaded == null)
            {
                gvGestionPeriodoAnualResumen.HeaderRow.Cells[0].Text = "Año " + ddlAnio.SelectedItem.Text;
            }
            else
            {
                showMessageError(loaded);

                return;
            }



            loaded = loadResumenCreador(gvGestionPeriodoMensualResumen, rut_creador, anio, mes, mes);
            if (loaded == null)
            {
                gvGestionPeriodoMensualResumen.HeaderRow.Cells[0].Text = ddlMes.SelectedItem.Text + " " + ddlAnio.SelectedItem.Text;
            }
            else
            {
                showMessageError(loaded);

                return;
            }
        }

        protected void ddlCentro_DataBound(object sender, EventArgs e)
        {
            ddlCentro.Items.Insert(0, new ListItem("[Todos]", ""));
        }

        protected void ddlCentro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCentro.SelectedIndex < 0)
            {
                ddlCliente.Items.Clear();
                SDSCliente.DataBind();
                ddlCliente.DataBind();
            }
        }

        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            ddlCliente.Items.Insert(0, new ListItem("[Todos]", ""));
        }
    }
}