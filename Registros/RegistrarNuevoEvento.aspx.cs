using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;
using System.Data;
using System.IO;

namespace NCCSAN
{
    public partial class RegistrarNuevoEvento : System.Web.UI.Page
    {
        private static readonly int PANEL_COMPONENTE = 0;
        private static readonly int PANEL_DESVIACION = 1;
        private static readonly int PANEL_ARCHIVOS = 2;
        private static readonly int PANEL_EVENTO_ACCION_INMEDIATA = 3;

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

                    string id_centro = u.getIDCentro();
                    if ((id_centro.Equals("CSAN")) || (id_centro.Equals("CMSFM")) || (id_centro.Equals("CRCAR")))
                    {
                        lbAsteriskSerieEquipo.Visible = false;
                        txtAgenteCorrector.Enabled = false;
                    }
                    else if (id_centro.Equals("CSAR"))
                    {
                        lbAsteriskSerieEquipo.Visible = true;
                        txtAgenteCorrector.Enabled = true;
                    }
                }

                resetCausas();

                showPanel(PANEL_COMPONENTE);
                Session.Remove("listArchivos");
                updateGVArchivos();
                Session.Remove("listArchivosAccionInmediata");
                updateGVArchivosAccionInmediata();

                List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();
                saveListInvolucrados(listInvolucrados);
                updateGVInvolucrados();

                if (!loadProbabilidadConsecuencia())
                {
                    string msg = "Error al recuperar información del Usuario";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }

                WOInfo woinfo = getWOInfo();
                if (woinfo != null)
                {
                    txtWO.Text = woinfo.getCodigoWO();
                    if (woinfo.getSerieEquipo() != null)
                    {
                        txtSerieEquipo.Text = woinfo.getSerieEquipo();
                    }

                    if (woinfo.getSerieComponente() != null)
                    {
                        txtSerieComponente.Text = woinfo.getSerieComponente();
                    }
                }
            }
            else
            {
                //AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "setControlsStyle", "<script type=\"text/javascript\">setControlsStyle();</script>", false);
            }
        }



        private void resetCausas()
        {
            ddlTipoCausaInmediata.Items.Clear();
            ddlTipoCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));

            ddlTipoCausaBasica.Items.Clear();
            ddlTipoCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlOrigenFalla_DataBound(object sender, EventArgs e)
        {
            ddlOrigenFalla.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlTipoCausaInmediata_DataBound(object sender, EventArgs e)
        {
            ddlTipoCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlOrigenFalla_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlOrigenFalla.SelectedIndex < 1)
            {
                resetCausas();
            }
            else if (ddlOrigenFalla.SelectedValue.Equals("Finning"))
            {
                SDSTipoCausaInmediata.DataBind();
                ddlTipoCausaInmediata.DataSource = SDSTipoCausaInmediata;
                ddlTipoCausaInmediata.DataBind();

                SDSTipoCausaBasica.DataBind();
                ddlTipoCausaBasica.DataSource = SDSTipoCausaBasica;
                ddlTipoCausaBasica.DataBind();
            }
            else
            {
                ddlTipoCausaInmediata.Items.Clear();
                ddlTipoCausaInmediata.Items.Add(new ListItem("N/A", "N/A"));
                ddlTipoCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));

                ddlTipoCausaBasica.Items.Clear();
                ddlTipoCausaBasica.Items.Add(new ListItem("N/A", "N/A"));
                ddlTipoCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
            }

            ddlCausaInmediata.Items.Clear();
            SDSCausaInmediata.DataBind();
            ddlCausaInmediata.DataBind();

            ddlCausaBasica.Items.Clear();
            SDSCausaBasica.DataBind();
            ddlCausaBasica.DataBind();

            ddlSubcausaInmediata.Items.Clear();
            SDSSubcausaInmediata.DataBind();
            ddlSubcausaInmediata.DataBind();

            ddlSubcausaBasica.Items.Clear();
            SDSSubcausaBasica.DataBind();
            ddlSubcausaBasica.DataBind();
        }


        protected void ddlTipoCausaInmediata_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCausaInmediata.Items.Clear();
            SDSCausaInmediata.DataBind();
            ddlCausaInmediata.DataBind();

            ddlSubcausaInmediata.Items.Clear();
            SDSSubcausaInmediata.DataBind();
            ddlSubcausaInmediata.DataBind();
        }


        protected void ddlCausaInmediata_DataBound(object sender, EventArgs e)
        {
            ddlCausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlSubcausaInmediata_DataBound(object sender, EventArgs e)
        {
            ddlSubcausaInmediata.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlCausaInmediata_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubcausaInmediata.Items.Clear();
            SDSSubcausaInmediata.DataBind();
            ddlSubcausaInmediata.DataBind();
        }


        protected void ddlTipoCausaBasica_DataBound(object sender, EventArgs e)
        {
            ddlTipoCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlTipoCausaBasica_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCausaBasica.Items.Clear();
            SDSCausaBasica.DataBind();
            ddlCausaBasica.DataBind();

            ddlSubcausaBasica.Items.Clear();
            SDSSubcausaBasica.DataBind();
            ddlSubcausaBasica.DataBind();
        }


        protected void ddlCausaBasica_DataBound(object sender, EventArgs e)
        {
            ddlCausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlCausaBasica_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubcausaBasica.Items.Clear();
            SDSSubcausaBasica.DataBind();
            ddlSubcausaBasica.DataBind();
        }

        protected void ddlSubcausaBasica_DataBound(object sender, EventArgs e)
        {
            ddlSubcausaBasica.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        private bool loadProbabilidadConsecuencia()
        {
            if (Session["usuario"] == null)
            {
                return false;
            }

            Usuario usuario = (Usuario)Session["usuario"];

            ddlProbabilidad.Items.Add(new ListItem("Seleccione..", ""));
            ddlProbabilidad.Items.Add(new ListItem("1 - Insignificante", "1"));
            ddlProbabilidad.Items.Add(new ListItem("2 - Baja", "2"));
            ddlProbabilidad.Items.Add(new ListItem("3 - Media", "3"));
            ddlProbabilidad.Items.Add(new ListItem("4 - Alta", "4"));


            ddlConsecuencia.Items.Add(new ListItem("Seleccione..", ""));
            ddlConsecuencia.Items.Add(new ListItem("1 - Menor", "1"));
            ddlConsecuencia.Items.Add(new ListItem("2 - Serio", "2"));
            ddlConsecuencia.Items.Add(new ListItem("10 - Mayor", "10"));
            ddlConsecuencia.Items.Add(new ListItem("100 - Grave", "100"));
            ddlConsecuencia.Items.Add(new ListItem("1000 - Catastrófico", "1000"));

            return true;
        }



        private string loadResponsablesEvento()
        {
            if ((ddlArea.SelectedIndex < 1) || (ddlSubarea.SelectedIndex < 1))
            {
                return null;
            }

            if (Session["id_centro"] == null)
            {
                return "No se puede recuperar información del Centro";
            }
            string id_centro = (string)Session["id_centro"];


            List<PersonaInfo> listSupervisoes = LogicController.getListSupervisoresCentroAreaSubarea(id_centro, ddlArea.SelectedValue, ddlSubarea.SelectedValue);
            if (listSupervisoes == null)
            {
                return "No se puede cargar la lista de posibles responsables del Evento";
            }

            ddlResponsableEvento.Items.Clear();
            foreach (PersonaInfo supervisor in listSupervisoes)
            {
                ddlResponsableEvento.Items.Add(new ListItem("[Supervisor] " + supervisor.getNombre(), supervisor.getRut()));
            }

            PersonaInfo jefe = LogicController.getCentroAreaJefe(ddlArea.SelectedValue, id_centro);
            if (jefe == null)
            {
                return "Error al recuperar información del Jefe de Área";
            }
            else
            {
                ddlResponsableEvento.Items.Add(new ListItem("[Jefe área] " + jefe.getNombre(), jefe.getRut()));
            }

            ddlResponsableEvento.Items.Insert(0, new ListItem("Seleccione..", ""));

            return null;
        }



        private void removeWOInfo()
        {
            Session.Remove("woinfo");
        }

        private void setWOInfo(WOInfo woInfo)
        {
            Session["woinfo"] = woInfo;
        }


        private WOInfo getWOInfo()
        {
            if (Session["woinfo"] != null)
            {
                return (WOInfo)Session["woinfo"];
            }
            else
            {
                return null;
            }
        }


        private void loadWOInfo()
        {
            WOInfo woInfo = getWOInfo();
            if (woInfo != null)
            {
                txtWO.Text = woInfo.getCodigoWO();
                if (ddlCliente.Items.Contains(new ListItem(woInfo.getNombreCliente(), woInfo.getNombreCliente())))
                {
                    ddlCliente.SelectedValue = woInfo.getNombreCliente();
                }

                if (ddlTipo.Items.Contains(new ListItem(woInfo.getTipoEquipo(), woInfo.getTipoEquipo())))
                {
                    ddlTipo.SelectedValue = woInfo.getTipoEquipo();
                }

                txtSerieEquipo.Text = woInfo.getSerieEquipo();
                txtSerieComponente.Text = woInfo.getSerieComponente();
            }
        }


        private void clearForm()
        {
            ddlCliente.SelectedIndex = 0;
            ddlTipo.SelectedIndex = 0;
            ddlModelo.Items.Clear();
            ddlModelo.DataBind();
            txtSerieEquipo.Text = "";
            ddlSistema.Items.Clear();
            ddlSistema.DataBind();
            ddlSubsistema.Items.Clear();
            ddlComponente.Items.Clear();
            txtSerieComponente.Text = "";
        }

        private void showPanel(int indexPanel)
        {
            ltSummary.Text = "";

            pnComponente.Visible = false;
            pnDesviacion.Visible = false;
            pnArchivos.Visible = false;
            pnEventoAccionInmediata.Visible = false;

            switch (indexPanel)
            {
                case 0:
                    {
                        pnComponente.Visible = true;
                        break;
                    }

                case 1:
                    {
                        pnDesviacion.Visible = true;
                        break;
                    }

                case 2:
                    {
                        pnArchivos.Visible = true;
                        uPanel.Update();
                        break;
                    }

                case 3:
                    {
                        pnEventoAccionInmediata.Visible = true;
                        uPanel.Update();
                        break;
                    }
            }

        }


        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            if (ddlCliente.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlCliente.Items.Insert(0, li);

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlCliente.Items.Contains(new ListItem(woInfo.getNombreCliente(), woInfo.getNombreCliente())))
                    {
                        ddlCliente.SelectedValue = woInfo.getNombreCliente();
                    }
                }
            }
            else
            {
                ddlCliente.Items.Clear();
            }
        }

        protected void ddlTipo_DataBound(object sender, EventArgs e)
        {
            if (ddlTipo.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlTipo.Items.Insert(0, li);


                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlTipo.Items.Contains(new ListItem(woInfo.getTipoEquipo(), woInfo.getTipoEquipo())))
                    {
                        ddlTipo.SelectedValue = woInfo.getTipoEquipo();
                    }
                }
            }
            else
            {
                ddlTipo.Items.Clear();
            }
        }

        protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlModelo.Items.Clear();
            ddlSistema.Items.Clear();
            ddlSubsistema.Items.Clear();
            ddlComponente.Items.Clear();
        }


        protected void ddlModelo_DataBound(object sender, EventArgs e)
        {
            if (ddlModelo.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlModelo.Items.Insert(0, li);

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlModelo.Items.Contains(new ListItem(woInfo.getModeloEquipo(), woInfo.getModeloEquipo())))
                    {
                        ddlModelo.SelectedValue = woInfo.getModeloEquipo();
                    }
                }
            }
            else
            {
                ddlModelo.Items.Clear();
            }
        }


        protected void ddlSistema_DataBound(object sender, EventArgs e)
        {
            if (ddlSistema.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlSistema.Items.Insert(0, li);

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlSistema.Items.Contains(new ListItem(woInfo.getNombreSistema(), woInfo.getNombreSistema())))
                    {
                        ddlSistema.SelectedValue = woInfo.getNombreSistema();
                    }
                }
            }
            else
            {
                ddlSistema.Items.Clear();
            }
        }

        protected void ddlSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubsistema.Items.Clear();
            ddlComponente.Items.Clear();
        }


        protected void ddlSubsistema_DataBound(object sender, EventArgs e)
        {
            if (ddlSubsistema.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlSubsistema.Items.Insert(0, li);

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlSubsistema.Items.Contains(new ListItem(woInfo.getNombreSubsistema(), woInfo.getNombreSubsistema())))
                    {
                        ddlSubsistema.SelectedValue = woInfo.getNombreSubsistema();
                    }
                }
            }
            else
            {
                ddlSubsistema.Items.Clear();
            }
        }


        protected void ddlSubsistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlComponente.Items.Clear();
            ddlComponente.DataBind();
        }


        protected void ddlComponente_DataBound(object sender, EventArgs e)
        {
            if (ddlComponente.Items.Count > 0)
            {
                ListItem li = new ListItem("Seleccione..", "");
                ddlComponente.Items.Insert(0, li);

                WOInfo woInfo = getWOInfo();
                if (woInfo != null)
                {
                    if (ddlComponente.Items.Contains(new ListItem(woInfo.getNombreComponente(), woInfo.getNombreComponente())))
                    {
                        hfNombreComponente.Value = woInfo.getNombreComponente();

                        removeWOInfo();
                    }
                }
                else if (hfNombreComponente.Value.Length > 0)
                {
                    if (ddlComponente.Items.Contains(new ListItem(hfNombreComponente.Value, hfNombreComponente.Value)))
                    {
                        ddlComponente.SelectedValue = hfNombreComponente.Value;
                    }
                }
            }
            else
            {
                ddlComponente.Items.Clear();
            }
        }


        protected void ibNextComponente_Click(object sender, ImageClickEventArgs e)
        {

            List<string> errors = validatePanelComponente();
            if (errors.Count == 0)
            {
                showPanel(PANEL_DESVIACION);
            }
            else
            {
                showSummary(errors);
            }

        }


        protected void ibPreviousDesviacion_Click(object sender, ImageClickEventArgs e)
        {
            showPanel(PANEL_COMPONENTE);
        }


        private void showSummary(List<string> errors)
        {
            showMessageError("Para continuar debes completar los campos requeridos en el formulario");
            string summary = "<b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            ltSummary.Text = summary;

        }


        private List<string> validatePanelComponente()
        {
            List<string> errors = new List<string>();
            if (txtWO.Text.Length < 1)
            {
                errors.Add("No se indicó la Orden de Trabajo (W/O)");
            }

            if (ddlCliente.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Cliente");
            }

            if (ddlTipo.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el tipo de Equipo");
            }

            if (ddlModelo.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el modelo del Equipo");
            }

            if (Session["id_centro"] == null)
            {
                errors.Add("No se puede recuperar el Centro");
            }
            else
            {
                string id_centro = (string)Session["id_centro"];

                if (id_centro.Equals("CSAR"))
                {
                    if (txtSerieEquipo.Text.Length < 1)
                    {
                        errors.Add("Se debe ingresar la serie del Equipo");
                    }
                }
            }


            if (ddlSistema.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el sistema del Componente");
            }

            if (ddlSubsistema.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el subsistema del Componente");
            }

            if (ddlComponente.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Componente");
            }

            if (Session["id_centro"] == null)
            {
                errors.Add("No se puede recuperar el Centro");
            }




            return errors;
        }


        private List<string> validatePanelDesviacion()
        {
            List<string> errors = new List<string>();

            if (txtHoras.Text.Length > 0)
            {
                if (!Utils.validateNumber(txtHoras.Text))
                {
                    errors.Add("La cantidad de horas de trabajo debe ser un valor numérico");
                }
            }

            if (!Utils.validateFecha(txtFecha.Text))
            {
                errors.Add("No se indicó la fecha en que ocurrió el Evento");
            }

            if (ddlFuente.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la fuente de detección");
            }

            if (ddlArea.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Área");
            }

            if (ddlSubarea.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar el Sub-área");
            }

            if (ddlClasificacion.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar una clasificación para el evento");
            }

            if (ddlSubclasificacion.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar una Sub-clasificación para el Evento");
            }

            if (txtDetalle.Text.Length < 1)
            {
                errors.Add("No se detalló lo ocurrido");
            }


            if (ddlProbabilidad.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la probabilidad de ocurrencia");
            }


            if (ddlConsecuencia.SelectedIndex < 1)
            {
                errors.Add("Se debe seleccionar la consecuencia del Evento");
            }


            double irc = getIRC();
            if ((irc >= 0) && (irc < 10) && (!ddlFuente.SelectedValue.ToUpper().Equals("RECLAMO DE CLIENTE")))
            {
                if (ddlClasificacion.SelectedValue.Equals("*Sin especificar"))
                {
                    errors.Add("El tipo de Evento no permite una Clasificación \"*Sin especificar\"");
                }

                if (ddlSubclasificacion.SelectedValue.Equals("*Sin especificar"))
                {
                    errors.Add("El tipo de Evento no permite una Sub-clasificación \"*Sin especificar\"");
                }
            }

            return errors;
        }


        private List<string> validatePanelArchivos()
        {
            List<string> errors = new List<string>();

            /*ACEPTA REGISTRAR EVENTOS SIN ARCHIVOS COMO EVIDENCIA OBLIGATORIA
            if (getListaArchivos().Count < 1)
            {
                errors.Add("No se han seleccionado los archivos adjuntos");
            }
            */

            return errors;
        }


        private List<string> validatePanelAccionInmediata()
        {
            List<string> errors = new List<string>();

            if (ddlOrigenFalla.SelectedIndex < 1)
            {
                errors.Add("No se ha seleccionado el origen de la falla");
            }

            if (ddlCausaInmediata.SelectedIndex < 1)
            {
                errors.Add("No se ha seleccionado la causa inmediata");
            }

            if (ddlSubcausaInmediata.SelectedIndex < 1)
            {
                errors.Add("No se ha seleccionado la sub-causa inmediata");
            }

            if (ddlCausaBasica.SelectedIndex < 1)
            {
                errors.Add("No se ha seleccionado la causa básica");
            }

            if (ddlSubcausaBasica.SelectedIndex < 1)
            {
                errors.Add("No se ha seleccionado la sub-causa básica");
            }


            if (ddlResponsableEvento.SelectedIndex < 1)
            {
                errors.Add("No se ha seleccionado el responsable del Evento");
            }


            if (txtAccionInmediata.Text.Length < 1)
            {
                errors.Add("No se ha indicado la acción inmediata");
            }

            if (txtFechaAccionInmediata.Text.Length < 1)
            {
                errors.Add("No se ha indicado la fecha de la acción inmediata");
            }

            if (rblEfectividad.SelectedIndex < 0)
            {
                errors.Add("Se debe seleccionar la efectividad de la Acción");
            }

            return errors;
        }


        protected void ddlFuente_DataBound(object sender, EventArgs e)
        {
            ddlFuente.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlSubarea_DataBound(object sender, EventArgs e)
        {
            ddlSubarea.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlClasificacion_DataBound(object sender, EventArgs e)
        {
            ddlClasificacion.Items.Insert(0, new ListItem("Seleccione..", ""));
        }


        protected void ddlSubclasificacion_DataBound(object sender, EventArgs e)
        {
            ddlSubclasificacion.Items.Insert(0, new ListItem("Seleccione..", ""));
        }

        protected void ddlProbabilidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarCriticidad();
        }

        protected void ddlConsecuencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarCriticidad();
        }


        private double getIRC()
        {
            if ((ddlProbabilidad.SelectedIndex > 0) && (ddlConsecuencia.SelectedIndex > 0))
            {
                int probabilidad = Convert.ToInt32(ddlProbabilidad.SelectedItem.Value);
                int consecuencia = Convert.ToInt32(ddlConsecuencia.SelectedItem.Value);

                double irc = probabilidad * consecuencia;

                if (ddlFuente.SelectedValue.ToUpper().Equals("RECLAMO DE CLIENTE"))
                {
                    irc = irc * (1.2);
                }

                return irc;
            }
            else
            {
                return -1;
            }
        }


        private string getCriticidad(double irc)
        {
            if (irc < 1)
            {
                return null;
            }
            else if (irc <= 4)
            {
                return "No crítico";
            }
            else if (irc < 10)
            {
                return "Medianamente crítico";
            }
            else if (irc <= 300)
            {
                return "Altamente crítico";
            }
            else
            {
                return "Super crítico";
            }
        }


        private void mostrarCriticidad()
        {
            double irc = getIRC();
            if (irc > 0)
            {
                lbIRC.Text = Convert.ToString(irc);

                string criticidad = getCriticidad(irc);

                if (criticidad != null)
                {
                    lbCriticidad.Text = criticidad;
                }
                else
                {
                    lbCriticidad.Text = "--";
                }

            }
            else
            {
                lbIRC.Text = "--";
                lbCriticidad.Text = "--";
            }
        }


        protected void ibNextDesviacion_Click(object sender, ImageClickEventArgs e)
        {
            if (!showCheckBoxAccionInmediata())
            {
                string msg = "Error al recuperar tu información de Usuario";
                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
            }

            List<string> errors = validatePanelDesviacion();
            if (errors.Count == 0)
            {
                showPanel(PANEL_ARCHIVOS);
            }
            else
            {
                showSummary(errors);
            }

        }


        private bool showCheckBoxAccionInmediata()
        {
            if (Session["usuario"] == null)
            {
                return false;
            }

            Usuario usuario = (Usuario)Session["usuario"];
            string rol = usuario.getNombreRol();

            if ((rol.Equals("Jefe Calidad")) || (rol.Equals("Coordinador")) || (rol.Equals("Inspector")))
            {
                if ((getIRC() >= 10) || (ddlFuente.SelectedValue.ToUpper().Equals("RECLAMO DE CLIENTE")))
                {
                    chbAccionInmediata.Checked = false;
                    chbAccionInmediata.Visible = false;
                }
                else
                {
                    chbAccionInmediata.Visible = true;
                }
            }
            else
            {
                chbAccionInmediata.Visible = false;
            }

            return true;
        }


        private void clearPanelComponente()
        {
            txtWO.Text = "";

            ddlCliente.Items.Clear();
            ddlCliente.DataBind();

            ddlTipo.Items.Clear();
            ddlTipo.DataBind();

            ddlModelo.Items.Clear();
            ddlModelo.DataBind();

            txtSerieEquipo.Text = "";

            ddlSistema.Items.Clear();
            ddlSistema.DataBind();

            ddlSubsistema.Items.Clear();
            ddlSubsistema.DataBind();

            ddlComponente.Items.Clear();
            ddlComponente.DataBind();

            txtSerieComponente.Text = "";
        }


        private void clearPanelDesviacion()
        {
            txtParte.Text = "";
            txtNumeroParte.Text = "";
            txtHoras.Text = "";
            txtFecha.Text = "";

            ddlFuente.Items.Clear();
            ddlFuente.DataBind();

            ddlArea.Items.Clear();
            ddlArea.DataBind();

            ddlSubarea.Items.Clear();
            ddlSubarea.DataBind();

            ddlClasificacion.Items.Clear();
            ddlClasificacion.DataBind();

            ddlSubclasificacion.Items.Clear();
            ddlSubclasificacion.DataBind();

            txtAgenteCorrector.Text = "";
            txtDetalle.Text = "";


            ddlProbabilidad.SelectedIndex = 0;

            ddlConsecuencia.SelectedIndex = 0;

            lbIRC.Text = "";
            lbCriticidad.Text = "";

            chbAccionInmediata.Checked = false;

            ltSummary.Text = "";
        }


        private void clearPanelAccionInmediata()
        {
            ddlOrigenFalla.Items.Clear();
            ddlOrigenFalla.DataBind();

            resetCausas();

            ddlCausaInmediata.Items.Clear();
            ddlCausaInmediata.DataBind();

            ddlSubcausaInmediata.Items.Clear();
            ddlSubcausaInmediata.DataBind();

            ddlCausaBasica.Items.Clear();
            ddlCausaBasica.DataBind();

            ddlSubcausaBasica.Items.Clear();
            ddlSubcausaBasica.DataBind();


            txtAccionInmediata.Text = "";
            txtFechaAccionInmediata.Text = "";
            rblEfectividad.ClearSelection();
            txtAccionInmediataObservacion.Text = "";

            ltSummary.Text = "";
        }




        private string registrarEvento()
        {
            string error = validatePanelsNuevoRegistro();
            if (error != null)
            {
                return error;
            }

            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];


            int horas;
            if (txtHoras.Text.Length > 0)
            {
                if (!Utils.validateNumber(txtHoras.Text))
                {
                    return "La cantidad de horas de trabajo debe ser numérica";
                }

                horas = Convert.ToInt32(txtHoras.Text);
                if (horas < 0)
                {
                    return "La cantidad de horas de trabajo debe ser un número positivo";
                }
            }
            else
            {
                horas = -1;
            }


            double irc = getIRC();
            if (irc < 1)
            {
                return "Error al calcular el IRC";
            }

            string criticidad = getCriticidad(irc);
            if (criticidad == null)
            {
                return "Error al calcular la criticidad";
            }

            List<Archivo> listArchivos = getListaArchivos();
            if (listArchivos == null)
            {
                return "Error al recuperar la lista de archivos adjuntos";
            }


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }

            Usuario usuario = (Usuario)Session["usuario"];
            PersonaInfo owner = LogicController.getPersonaInfo(usuario.getRutPersona());
            if (owner == null)
            {
                return "Error al recuperar tu información";
            }

            string registrado = LogicController.registerEvento
                                                                    (
                                                                        txtWO.Text,
                                                                        txtFecha.Text,
                                                                        ddlCliente.SelectedItem.Text,
                                                                        id_centro,
                                                                        ddlArea.SelectedItem.Text,
                                                                        ddlSubarea.SelectedItem.Text,
                                                                        ddlFuente.SelectedItem.Text,
                                                                        ddlModelo.SelectedItem.Text,
                                                                        ddlTipo.SelectedItem.Text,
                                                                        txtSerieEquipo.Text,
                                                                        ddlSistema.SelectedItem.Text,
                                                                        ddlSubsistema.SelectedItem.Text,
                                                                        ddlComponente.SelectedItem.Text,
                                                                        txtSerieComponente.Text,
                                                                        txtParte.Text,
                                                                        txtNumeroParte.Text,
                                                                        horas,
                                                                        ddlClasificacion.SelectedItem.Text,
                                                                        ddlSubclasificacion.SelectedItem.Text,
                                                                        txtAgenteCorrector.Text,
                                                                        txtDetalle.Text,
                                                                        ddlProbabilidad.SelectedItem.Text,
                                                                        ddlConsecuencia.SelectedItem.Text,
                                                                        irc,
                                                                        criticidad,
                                                                        listArchivos,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"],
                                                                        owner
                                                                    );
            if (registrado.ToUpper().StartsWith("CODIGO:"))
            {
                Session.Remove("listArchivos");

                string codigo = registrado.ToUpper().Replace("CODIGO:", "");


                lbMessageComponente.Text = "El Evento se registró satisfactoriamente con código <b>" + codigo + "</b> y se ha clasificado como ";
                lbMessageEquipo.Text = "El Evento se registró satisfactoriamente con código <b>" + codigo + "</b> y se ha clasificado como ";
                if (Utils.getEventoType(ddlFuente.SelectedItem.Text, irc) == 0)
                {
                    lbMessageComponente.Text += "<b>Hallazgo</b>";
                    lbMessageEquipo.Text += "<b>Hallazgo</b>";
                }
                else
                {
                    lbMessageComponente.Text += "<b>No conformidad</b>";
                    lbMessageEquipo.Text += "<b>No conformidad</b>";
                }


                return null;
            }
            else
            {
                return "Error al registrar el Evento: " + registrado;
            }
        }


        protected void btMessageComponenteSi_Click(object sender, EventArgs e)
        {

            Session.Remove("listArchivos");
            updateGVArchivos();


            clearPanelAccionInmediata();
            Session.Remove("listArchivosAccionInmediata");
            updateGVArchivosAccionInmediata();
            List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();
            saveListInvolucrados(listInvolucrados);
            updateGVInvolucrados();

            clearPanelDesviacion();
            showPanel(PANEL_DESVIACION);
            hfMessageComponente_ModalPopupExtender.Hide();
        }


        protected void btMessageComponenteNo_Click(object sender, EventArgs e)
        {
            hfMessageComponente_ModalPopupExtender.Hide();
            uPanel.Update();
            Response.Redirect("~/Registros/ListarEventos.aspx", true);
        }


        protected void btMessageEquipoSi_Click(object sender, EventArgs e)
        {

            Session.Remove("listArchivos");
            updateGVArchivos();


            clearPanelAccionInmediata();
            Session.Remove("listArchivosAccionInmediata");
            updateGVArchivosAccionInmediata();
            List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();
            saveListInvolucrados(listInvolucrados);
            updateGVInvolucrados();


            //Panel Componente
            {
                ddlSistema.Items.Clear();
                ddlSistema.DataBind();

                ddlSubsistema.Items.Clear();
                ddlSubsistema.DataBind();

                ddlComponente.Items.Clear();
                ddlComponente.DataBind();

                txtSerieComponente.Text = "";
            }

            //Panel Desviacion
            {
                txtParte.Text = "";
                txtNumeroParte.Text = "";
                txtHoras.Text = "";

                ddlClasificacion.Items.Clear();
                ddlClasificacion.DataBind();

                ddlSubclasificacion.Items.Clear();
                ddlSubclasificacion.DataBind();

                txtAgenteCorrector.Text = "";
                txtDetalle.Text = "";


                ddlProbabilidad.SelectedIndex = 0;

                ddlConsecuencia.SelectedIndex = 0;

                lbIRC.Text = "";
                lbCriticidad.Text = "";

                chbAccionInmediata.Checked = false;

                ltSummary.Text = "";
            }


            showPanel(PANEL_COMPONENTE);
            hfMessageEquipo_ModalPopupExtender.Hide();
        }


        protected void btMessageEquipoNo_Click(object sender, EventArgs e)
        {
            hfMessageEquipo_ModalPopupExtender.Hide();
            uPanel.Update();
            Response.Redirect("~/Registros/ListarEventos.aspx", true);
        }


        private string validatePanelsNuevoRegistro()
        {
            List<string> errors = validatePanelComponente();
            if (errors.Count > 0)
            {
                return "Existen campos sin completar en el panel de Componente";
            }

            errors = validatePanelDesviacion();
            if (errors.Count > 0)
            {
                return "Existen campos sin completar en el panel de Desviación";
            }

            return null;
        }


        protected void gvArchivos_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (false)
            {

            }
            else if (e.CommandName.Equals("RemoveArchivo"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index >= 0)
                {
                    List<Archivo> listArchivos = getListaArchivos();
                    if (index < listArchivos.Count)
                    {
                        string filename = listArchivos[index].getNombre();
                        listArchivos.RemoveAt(index);
                        setListaArchivos(listArchivos);

                        showMessageInfo("Se ha quitado el archivo \"" + filename + "\" del Evento");
                        updateGVArchivos();
                    }
                }
                else // index = -1
                {
                    return;
                }
            }
        }


        private List<Archivo> getListaArchivos()
        {
            List<Archivo> listArchivos;

            if (Session["listArchivos"] != null)
                listArchivos = (List<Archivo>)Session["listArchivos"];
            else
            {
                listArchivos = new List<Archivo>();
                setListaArchivos(listArchivos);
            }

            return listArchivos;
        }


        private void setListaArchivos(List<Archivo> listArchivo)
        {
            Session["listArchivos"] = listArchivo;
        }


        protected void ibAddArchivo_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";

            addFileToList();
        }


        private void addFileToList()
        {

            if (fuArchivo.HasFile)
            {
                List<Archivo> listArchivos = getListaArchivos();
                try
                {
                    byte[] contentFile;
                    BinaryReader binaryReader;
                    HttpFileCollection hfc = Request.Files;
                    HttpPostedFile hpf;
                    string fileType;
                    string contentType;
                    string fileName;
                    string valid_filename;
                    string ext;
                    int max_file_size;
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = hfc[i];

                        fileName = Path.GetFileName(hpf.FileName);
                        valid_filename = Utils.validateFilename(fileName);
                        if (valid_filename != null)
                        {
                            showMessageError(valid_filename);
                            return;
                        }

                        ext = Utils.getFileExtension(fileName);
                        contentType = Utils.getContentType(ext);
                        if (contentType == null)
                        {
                            showMessageError("Extensión de archivo \"" + ext + "\" no permitida");
                            return;
                        }

                        max_file_size = LogicController.getMaxFileSizeByExtension(ext);
                        if (max_file_size < 0)
                        {
                            showMessageError("No se puede comprobar el tamaño máximo permitido para el tipo de archivo");

                            return;
                        }


                        if ((hpf.ContentLength) > max_file_size)
                        {
                            showMessageError("El tipo de archivo no permite un tamaño superior a " + Convert.ToString(max_file_size / 1000000) + " Mb");

                            return;
                        }


                        fileType = Utils.getFileType(ext);

                        contentFile = new byte[hpf.ContentLength];
                        binaryReader = new BinaryReader(hpf.InputStream);
                        binaryReader.Read(contentFile, 0, hpf.ContentLength);
                        Archivo archivo = new Archivo(Utils.getUniqueCode(), fileName, contentFile, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }
                catch (Exception ex)
                {
                    showMessageError("Se ha producido un error al cargar los archivos");
                    return;
                }

                setListaArchivos(listArchivos);

                updateGVArchivos();
            }
            else
            {
                showMessageWarning("No se ha seleccionado el archivo");
            }
        }


        private void updateGVArchivos()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Tamaño");

            DataRow dr;

            List<Archivo> listArchivos = getListaArchivos();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();

                dt.Rows.Add(dr);

            }

            dt.AcceptChanges();
            gvArchivos.DataSource = dt;
            gvArchivos.DataBind();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                ((Label)gvArchivos.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivos[i].getNombre();
                ((Label)gvArchivos.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivos[i].getTipoArchivo();
                ((Label)gvArchivos.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivos[i].getSize()) + " bytes";
            }
        }


        protected void ibPreviousArchivos_Click(object sender, ImageClickEventArgs e)
        {
            showPanel(PANEL_DESVIACION);
        }


        protected void ibRegisterEvento_Click(object sender, ImageClickEventArgs e)
        {
            List<string> errors = validatePanelArchivos();
            if (errors.Count == 0)
            {
                if (chbAccionInmediata.Checked)
                {
                    showPanel(PANEL_EVENTO_ACCION_INMEDIATA);
                }
                else
                {
                    string status = registrarEvento();
                    if (status == null)
                    {
                        Session.Remove("listArchivos");

                        if (Session["id_centro"] != null)
                        {
                            string id_centro = (string)Session["id_centro"];

                            if (id_centro.Equals("CSAR"))
                            {
                                hfMessageEquipo_ModalPopupExtender.Show();
                            }
                            else
                            {
                                hfMessageComponente_ModalPopupExtender.Show();
                            }
                        }

                    }
                    else
                    {
                        showMessageError(status);
                    }
                }

            }
            else
            {
                showSummary(errors);
            }
        }


        protected void ibByPassDesviacion_Click(object sender, ImageClickEventArgs e)
        {
            showPanel(PANEL_ARCHIVOS);
        }



        private void showMessageInfo(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxInfo", "<script type=\"text/javascript\">showMessageInfo('" + message + "');</script>", false);
        }


        private void showMessageWarning(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxWarning", "<script type=\"text/javascript\">showMessageWarning('" + message + "');</script>", false);
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


        protected void fuArchivo_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            showMessageSuccess("Se ha cargado el archivo \"" + fuArchivo.FileName + "\" al Evento");
        }


        protected void fuArchivo_UploadedFileError(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            showMessageError("Falló la carga del archivo \"" + fuArchivo.FileName + "\".Inténtelo nuevamente");
        }


        protected void ibMatrizConsecuencia_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["id_centro"] == null)
            {
                showMessageError("Error al recuperar el Centro de servicios");

                return;
            }

            string centro = (string)Session["id_centro"];
            imgMatrizConsecuencia.ImageUrl = ResolveClientUrl("~/Images/MatrizConsecuencia/" + centro + ".png");
            hfMatrizConsecuencia_ModalPopupExtender.Show();
        }


        protected void btCerrarMatrizConsecuencia_Click(object sender, EventArgs e)
        {
            hfMatrizConsecuencia_ModalPopupExtender.Hide();

            uPanel.Update();
        }

        protected void ddlFuente_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarCriticidad();
        }

        protected void txtWO_TextChanged(object sender, EventArgs e)
        {
            if (txtWO.Text.Length < 1)
            {
                return;
            }

            if (Session["id_centro"] == null)
            {
                return;
            }

            string id_centro = (string)Session["id_centro"];
            WOInfo woInfo = LogicController.getWOInfo(txtWO.Text, id_centro);
            if (woInfo != null)
            {
                setWOInfo(woInfo);

                lbMessageLoadWO.Text = "Se ha encontrado el componente asociado a la <b>W/O " + woInfo.getCodigoWO() + "</b>. ¿Desea cargar los datos?";
                hfLoadWO_ModalPopupExtender.Show();
            }
            else
            {
                Session.Remove("woinfo");
            }
        }


        protected void btLoadWOSi_Click(object sender, EventArgs e)
        {
            loadWOInfo();
        }


        protected void btLoadWONo_Click(object sender, EventArgs e)
        {
            Session.Remove("woinfo");
            hfLoadWO_ModalPopupExtender.Hide();
        }

        protected void chbAccionInmediata_CheckedChanged(object sender, EventArgs e)
        {
            if (chbAccionInmediata.Checked)
            {
                ibRegisterEvento.ImageUrl = "~/Images/Button/next.png";
            }
            else
            {
                ibRegisterEvento.ImageUrl = "~/Images/Button/save.png";
            }
        }

        protected void ibPreviousAccionInmediata_Click(object sender, ImageClickEventArgs e)
        {
            showPanel(PANEL_ARCHIVOS);
        }

        protected void txtFecha_TextChanged(object sender, EventArgs e)
        {
            txtFechaAccionInmediata.Text = txtFecha.Text;
        }


        private void updateGVInvolucrados()
        {
            DataTable dt = getDTPersonasFormat();
            List<PersonaInfo> listInvolucrados = getListInvolucrados();

            DataRow dr;

            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Button();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvInvolucrados.DataSource = dt;
            gvInvolucrados.DataBind();


            for (int i = 0; i < listInvolucrados.Count; i++)
            {
                ((Label)gvInvolucrados.Rows[i].FindControl("lbRUT")).Text = listInvolucrados[i].getRut();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombre")).Text = listInvolucrados[i].getNombre();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbNombreCentro")).Text = listInvolucrados[i].getNombreCentro();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbCargo")).Text = listInvolucrados[i].getCargo();
                ((Label)gvInvolucrados.Rows[i].FindControl("lbAntiguedad")).Text = Convert.ToString(listInvolucrados[i].getAntiguedad());
            }
        }


        protected void gvInvolucrados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("BuscarInvolucrado"))
            {
                uPanel.Update();

                ((ButtonField)gvBuscarPersonas.Columns[0]).CommandName = "AddInvolucrado";
                hfBuscarPersona_ModalPopupExtender.Show();

                return;
            }

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

            List<PersonaInfo> listInvolucrados = getListInvolucrados();
            if (index < listInvolucrados.Count)
            {
                listInvolucrados.RemoveAt(index);

                saveListInvolucrados(listInvolucrados);
                updateGVInvolucrados();
            }
        }


        private List<PersonaInfo> getListInvolucrados()
        {
            List<PersonaInfo> listInvolucrados;

            if (Session["listInvolucrados"] == null)
                listInvolucrados = new List<PersonaInfo>();
            else
                listInvolucrados = (List<PersonaInfo>)Session["listInvolucrados"];

            return listInvolucrados;
        }


        private bool isPersonaInList(List<PersonaInfo> listPersona, string rut)
        {
            foreach (PersonaInfo p in listPersona)
            {
                if (p.getRut().Equals(rut))
                    return true;
            }

            return false;
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



        private static DataTable getDTPersonasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("RUT");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Centro");
            dt.Columns.Add("Cargo");
            dt.Columns.Add("Antiguedad");
            dt.Columns.Add("Opciones");

            dt.AcceptChanges();

            return dt;
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

            if (e.CommandName.Equals("AddInvolucrado"))
            {
                List<PersonaInfo> listInvolucrados = getListInvolucrados();
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                if (isPersonaInList(listInvolucrados, rut))
                {
                    showBuscarPersonaMessageError("La Persona \"" + nombre + "\" ya existe en la lista de Involucrados");
                    return;
                }

                string nombre_centro = gvBuscarPersonas.Rows[index].Cells[4].Text;
                string cargo = gvBuscarPersonas.Rows[index].Cells[5].Text;
                int antiguedad = Convert.ToInt32(gvBuscarPersonas.Rows[index].Cells[6].Text);

                PersonaInfo p = new PersonaInfo(rut, nombre, nombre_centro, cargo, antiguedad);
                listInvolucrados.Add(p);

                saveListInvolucrados(listInvolucrados);
                showBuscarPersonaMessageSuccess("Se ha ingresado a \"" + nombre + "\" a la lista de Involucrados");

                updateGVInvolucrados();
                uPanel.Update();
                hfBuscarPersona_ModalPopupExtender.Show();
                upInvolucrados.Update();
            }


            txtBuscarPersonaApellido.Text = "";
            //ltBuscarPersonaSummary.Text = "";

            //hfBuscarPersona_ModalPopupExtender.Hide();
        }


        protected void btBuscarPersonaCerrar_Click(object sender, EventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";
            hfBuscarPersona_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        private void saveListInvolucrados(List<PersonaInfo> listInvolucrados)
        {
            Session["listInvolucrados"] = listInvolucrados;
        }


        protected void gvArchivosAccionInmediata_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            int index = -1;

            if (false)
            {

            }
            else if (e.CommandName.Equals("RemoveArchivo"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index >= 0)
                {
                    List<Archivo> listArchivos = getListaArchivosAccionInmediata();
                    if (index < listArchivos.Count)
                    {
                        string filename = listArchivos[index].getNombre();
                        listArchivos.RemoveAt(index);
                        setListaArchivosAccionInmediata(listArchivos);

                        showMessageInfo("Se ha quitado el archivo \"" + filename + "\" de la Acción Inmediata");
                        updateGVArchivosAccionInmediata();
                    }
                }
                else // index = -1
                {
                    return;
                }
            }
        }


        protected void ibAddArchivoAccionInmediata_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";

            addFileToListAccionInmediata();
        }


        private void setListaArchivosAccionInmediata(List<Archivo> listArchivo)
        {
            Session["listArchivosAccionInmediata"] = listArchivo;
        }


        private List<Archivo> getListaArchivosAccionInmediata()
        {
            List<Archivo> listArchivos;

            if (Session["listArchivosAccionInmediata"] != null)
                listArchivos = (List<Archivo>)Session["listArchivosAccionInmediata"];
            else
            {
                listArchivos = new List<Archivo>();
                setListaArchivosAccionInmediata(listArchivos);
            }

            return listArchivos;
        }


        private void addFileToListAccionInmediata()
        {

            if (fuArchivoAccionInmediata.HasFile)
            {
                List<Archivo> listArchivos = getListaArchivosAccionInmediata();
                try
                {
                    byte[] contentFile;
                    BinaryReader binaryReader;
                    HttpFileCollection hfc = Request.Files;
                    HttpPostedFile hpf;
                    string fileType;
                    string contentType;
                    string fileName;
                    string valid_filename;
                    string ext;
                    int max_file_size;
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = hfc[i];

                        fileName = Path.GetFileName(hpf.FileName);
                        valid_filename = Utils.validateFilename(fileName);
                        if (valid_filename != null)
                        {
                            showMessageError(valid_filename);
                            return;
                        }
                        ext = Utils.getFileExtension(fileName);
                        contentType = Utils.getContentType(ext);
                        if (contentType == null)
                        {
                            showMessageError("Extensión de archivo \"" + ext + "\" no permitida");
                            return;
                        }

                        max_file_size = LogicController.getMaxFileSizeByExtension(ext);
                        if (max_file_size < 0)
                        {
                            showMessageError("No se puede comprobar el tamaño máximo permitido para el tipo de archivo");

                            return;
                        }


                        if ((hpf.ContentLength) > max_file_size)
                        {
                            showMessageError("El tipo de archivo no permite un tamaño superior a " + Convert.ToString(max_file_size / 1000000) + " Mb");

                            return;
                        }

                        fileType = Utils.getFileType(ext);

                        contentFile = new byte[hpf.ContentLength];
                        binaryReader = new BinaryReader(hpf.InputStream);
                        binaryReader.Read(contentFile, 0, hpf.ContentLength);
                        Archivo archivo = new Archivo(Utils.getUniqueCode(), fileName, contentFile, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }
                catch (Exception ex)
                {
                    showMessageError("Se ha producido un error al cargar los archivos");
                    return;
                }

                setListaArchivosAccionInmediata(listArchivos);

                updateGVArchivosAccionInmediata();
            }
            else
            {
                showMessageWarning("No se ha seleccionado el archivo");
            }
        }


        private void updateGVArchivosAccionInmediata()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Tamaño");

            DataRow dr;

            List<Archivo> listArchivos = getListaArchivosAccionInmediata();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();

                dt.Rows.Add(dr);

            }

            dt.AcceptChanges();
            gvArchivosAccionInmediata.DataSource = dt;
            gvArchivosAccionInmediata.DataBind();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                ((Label)gvArchivosAccionInmediata.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivos[i].getNombre();
                ((Label)gvArchivosAccionInmediata.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivos[i].getTipoArchivo();
                ((Label)gvArchivosAccionInmediata.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivos[i].getSize()) + " bytes";
            }
        }

        protected void ibRegistrarEventoAccionInmediata_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";
            List<string> errors = validatePanelAccionInmediata();
            if (errors.Count == 0)
            {
                if (rblEfectividad.SelectedValue.Equals("Si"))
                {
                    lbMessageConfirmAccionInmediata.Text = "Se va a registrar el Evento junto a la Acción Inmediata como <b>Efectiva</b>. ¿Desea continuar?";
                }
                else
                {
                    lbMessageConfirmAccionInmediata.Text = "Se va a registrar el Evento junto a la Acción Inmediata como <b>No efectiva</b>. ¿Desea continuar?";
                }


                hfConfirmAccionInmediata_ModalPopupExtender.Show();

            }
            else
            {
                showSummary(errors);
            }
        }


        protected void btConfirmAccionInmediataNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void btConfirmAccionInmediataSi_Click(object sender, EventArgs e)
        {
            string status = registrarNuevoEventoAccionInmediata();
            if (status == null)
            {
                Session.Remove("listArchivos");
                Session.Remove("listArchivosAccionInmediata");
                Session.Remove("listInvolucrados");

                if (Session["id_centro"] != null)
                {
                    string id_centro = (string)Session["id_centro"];

                    if (id_centro.Equals("CSAR"))
                    {
                        hfConfirmAccionInmediata_ModalPopupExtender.Hide();
                        hfMessageEquipo_ModalPopupExtender.Show();
                    }
                    else
                    {
                        hfConfirmAccionInmediata_ModalPopupExtender.Hide();
                        hfMessageComponente_ModalPopupExtender.Show();
                    }
                }

                
                uPanel.Update();
            }
            else
            {
                showMessageError(status);
            }
        }



        private string registrarNuevoEventoAccionInmediata()
        {

            string error = validatePanelsNuevoRegistro();
            if (error != null)
            {
                return error;
            }

            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];


            int horas;
            if (txtHoras.Text.Length > 0)
            {
                if (!Utils.validateNumber(txtHoras.Text))
                {
                    return "La cantidad de horas de trabajo debe ser numérica";
                }

                horas = Convert.ToInt32(txtHoras.Text);
                if (horas < 0)
                {
                    return "La cantidad de horas de trabajo debe ser un número positivo";
                }
            }
            else
            {
                horas = -1;
            }


            double irc = getIRC();
            if (irc < 1)
            {
                return "Error al calcular el IRC";
            }

            string criticidad = getCriticidad(irc);
            if (criticidad == null)
            {
                return "Error al calcular la criticidad";
            }

            List<Archivo> listArchivos = getListaArchivos();
            if (listArchivos == null)
            {
                return "Error al recuperar la lista de archivos adjuntos";
            }


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }

            Usuario usuario = (Usuario)Session["usuario"];
            PersonaInfo owner = LogicController.getPersonaInfo(usuario.getRutPersona());
            if (owner == null)
            {
                return "Error al recuperar tu información";
            }


            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            List<Archivo> listArchivosAccionInmediata = getListaArchivosAccionInmediata();
            List<PersonaInfo> listInvolucrados = getListInvolucrados();


            if (owner == null)
            {
                return "Error al recuperar tu información";
            }


            string registrado = LogicController.registerEventoAccionInmediata
                                                                    (
                                                                        txtWO.Text,
                                                                        txtFecha.Text,
                                                                        ddlCliente.SelectedItem.Text,
                                                                        id_centro,
                                                                        ddlArea.SelectedItem.Text,
                                                                        ddlSubarea.SelectedItem.Text,
                                                                        ddlFuente.SelectedItem.Text,
                                                                        ddlModelo.SelectedItem.Text,
                                                                        ddlTipo.SelectedItem.Text,
                                                                        txtSerieEquipo.Text,
                                                                        ddlSistema.SelectedItem.Text,
                                                                        ddlSubsistema.SelectedItem.Text,
                                                                        ddlComponente.SelectedItem.Text,
                                                                        txtSerieComponente.Text,
                                                                        txtParte.Text,
                                                                        txtNumeroParte.Text,
                                                                        horas,
                                                                        ddlClasificacion.SelectedItem.Text,
                                                                        ddlSubclasificacion.SelectedItem.Text,
                                                                        txtAgenteCorrector.Text,
                                                                        txtDetalle.Text,
                                                                        ddlProbabilidad.SelectedItem.Text,
                                                                        ddlConsecuencia.SelectedItem.Text,
                                                                        irc,
                                                                        criticidad,
                                                                        listArchivos,
                                                                        ddlOrigenFalla.SelectedValue,
                                                                        ddlCausaInmediata.SelectedValue,
                                                                        ddlSubcausaInmediata.SelectedValue,
                                                                        ddlCausaBasica.SelectedValue,
                                                                        ddlSubcausaBasica.SelectedValue,
                                                                        ddlResponsableEvento.SelectedValue,
                                                                        txtAccionInmediata.Text,
                                                                        txtFechaAccionInmediata.Text,
                                                                        rblEfectividad.SelectedValue,
                                                                        txtAccionInmediataObservacion.Text,
                                                                        listArchivosAccionInmediata,
                                                                        listInvolucrados,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"],
                                                                        owner
                                                                    );

            if (registrado.ToUpper().StartsWith("CODIGO:"))
            {
                Session.Remove("listArchivos");
                Session.Remove("listArchivosAccionInmediata");
                Session.Remove("listInvolucrados");

                string codigo = registrado.ToUpper().Replace("CODIGO:", "");

                lbMessageComponente.Text = "La se registró satisfactoriamente el Evento con código <b>" + codigo + "</b> junto a la Acción Inmediata";
                lbMessageEquipo.Text = "La se registró satisfactoriamente el Evento con código <b>" + codigo + "</b> junto a la Acción Inmediata";

                return null;
            }
            else
            {
                return "Error al registrar la Acción Inmediata: " + registrado;
            }
        }

        protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            string responsables_loaded = loadResponsablesEvento();
            if (responsables_loaded != null)
            {
                showMessageError(responsables_loaded);
            }
        }

        protected void ddlSubarea_SelectedIndexChanged(object sender, EventArgs e)
        {
            string responsables_loaded = loadResponsablesEvento();
            if (responsables_loaded != null)
            {
                showMessageError(responsables_loaded);
            }
        }

    }

}