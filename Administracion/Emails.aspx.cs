using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;

namespace NCCSAN.Administracion
{
    public partial class Emails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["id_centro"] != null)
                {
                    string id_centro = (string)Session["id_centro"];

                    string loaded = loadTables(id_centro);
                    if (loaded != null)
                    {
                        string msg = loaded;
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }
                }
                else
                {

                }
            }
        }


        private string loadTables(string id_centro)
        {
            if (id_centro == null)
            {
                return "No se puede recuperar infomación del Centro";
            }


            if (!loadTableResumenes(id_centro))
            {
                return "No se puede cargar la tabla de Resumenes";
            }


            if (!loadTableAccionesInmediatas(id_centro))
            {
                return "No se puede cargar la tabla de Acciones Inmediatas";
            }


            if (!loadTableInvestigaciones(id_centro))
            {
                return "No se puede cargar la tabla de Investigaciones";
            }


            if (!loadTablePlanesAccion(id_centro))
            {
                return "No se puede cargar la tabla de Planes de Acción";
            }


            return null;
        }


        private bool loadTableResumenes(string id_centro)
        {
            if (id_centro == null)
            {
                return false;
            }

            ConfigEmailSender ces;

            {//Resumen Jefe Calidad
                ces = LogicController.getConfigEmailSender("Resumen", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtResumenJefeCalidadDias.Text = Convert.ToString(ces.getDiasMensual());
                chbResumenJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Resumen Coordinador
                ces = LogicController.getConfigEmailSender("Resumen", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtResumenCoordinadorDias.Text = Convert.ToString(ces.getDiasMensual());
                chbResumenCoordinadorActivado.Checked = ces.getActivo();
            }

            return true;
        }


        private bool loadTableAccionesInmediatas(string id_centro)
        {
            if (id_centro == null)
            {
                return false;
            }

            ConfigEmailSender ces;

            {//Acción inmediata pendiente Inspector
                ces = LogicController.getConfigEmailSender("Acción inmediata pendiente", "Inspector", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtAccionInmediataPendienteInspectorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtAccionInmediataPendienteInspectorDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbAccionInmediataPendienteInspectorActivado.Checked = ces.getActivo();
            }

            return true;
        }


        private bool loadTableInvestigaciones(string id_centro)
        {
            if (id_centro == null)
            {
                return false;
            }

            ConfigEmailSender ces;

            {//Investigación pendiente Jefe Calidad
                ces = LogicController.getConfigEmailSender("Investigación pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtInvestigacionPendienteJefeCalidadDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtInvestigacionPendienteJefeCalidadDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbInvestigacionPendienteJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Investigación pendiente Coordinador
                ces = LogicController.getConfigEmailSender("Investigación pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtInvestigacionPendienteCoordinadorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtInvestigacionPendienteCoordinadorDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbInvestigacionPendienteCoordinadorActivado.Checked = ces.getActivo();
            }


            {//Investigación en curso Jefe Calidad
                ces = LogicController.getConfigEmailSender("Investigación en curso", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtInvestigacionEnCursoJefeCalidadDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtInvestigacionEnCursoJefeCalidadDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbInvestigacionEnCursoJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Investigación en curso Coordinador
                ces = LogicController.getConfigEmailSender("Investigación en curso", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtInvestigacionEnCursoCoordinadorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtInvestigacionEnCursoCoordinadorDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbInvestigacionEnCursoCoordinadorActivado.Checked = ces.getActivo();
            }


            {//Investigación en curso Responsable
                ces = LogicController.getConfigEmailSender("Investigación en curso", "Usuario", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtInvestigacionEnCursoResponsableDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtInvestigacionEnCursoResponsableDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbInvestigacionEnCursoResponsableActivado.Checked = ces.getActivo();
            }



            {//Evaluación pendiente Jefe Calidad
                ces = LogicController.getConfigEmailSender("Evaluación pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtEvaluacionPendienteJefeCalidadDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtEvaluacionPendienteJefeCalidadDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbEvaluacionPendienteJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Evaluación pendiente Coordinador
                ces = LogicController.getConfigEmailSender("Evaluación pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtEvaluacionPendienteCoordinadorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtEvaluacionPendienteCoordinadorDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbEvaluacionPendienteCoordinadorActivado.Checked = ces.getActivo();
            }


            return true;
        }


        private bool loadTablePlanesAccion(string id_centro)
        {
            if (id_centro == null)
            {
                return false;
            }

            ConfigEmailSender ces;

            {//Plan de acción pendiente Jefe Calidad
                ces = LogicController.getConfigEmailSender("Plan de acción pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtPlanAccionPendienteJefeCalidadDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtPlanAccionPendienteJefeCalidadDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbPlanAccionPendienteJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Plan de acción pendiente Coordinador
                ces = LogicController.getConfigEmailSender("Plan de acción pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtPlanAccionPendienteCoordinadorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                txtPlanAccionPendienteCoordinadorDiasLimite.Text = Convert.ToString(ces.getDiasLimite());
                chbPlanAccionPendienteCoordinadorActivado.Checked = ces.getActivo();
            }


            {//Acción correctiva en curso Jefe Calidad
                ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtAccionCorrectivaEnCursoJefeCalidadDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                chbAccionCorrectivaEnCursoJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Acción correctiva en curso Coordinador
                ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtAccionCorrectivaEnCursoCoordinadorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                chbAccionCorrectivaEnCursoCoordinadorActivado.Checked = ces.getActivo();
            }


            {//Acción correctiva en curso Responsable
                ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Usuario", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtAccionCorrectivaEnCursoResponsableDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                chbAccionCorrectivaEnCursoResponsableActivado.Checked = ces.getActivo();
            }


            {//Verificación pendiente Jefe Calidad
                ces = LogicController.getConfigEmailSender("Verificación pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtVerificacionPendienteJefeCalidadDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                chbVerificacionPendienteJefeCalidadActivado.Checked = ces.getActivo();
            }


            {//Verificación pendiente Coordinador
                ces = LogicController.getConfigEmailSender("Verificación pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return false;
                }

                txtVerificacionPendienteCoordinadorDiasAlerta.Text = Convert.ToString(ces.getDiasAlerta());
                chbVerificacionPendienteCoordinadorActivado.Checked = ces.getActivo();
            }



            return true;
        }


        private List<string> validatePanels()
        {
            List<string> errors = new List<string>();
            errors.AddRange(validatePanelResumenes());
            errors.AddRange(validatePanelAccionesInmediatas());
            errors.AddRange(validatePanelInvestigaciones());
            errors.AddRange(validatePanelPlanesAccion());

            return errors;
        }


        private List<string> validatePanelResumenes()
        {
            List<string> errors = new List<string>();

            int dias;

            {//Resumen Jefe Calidad
                if (txtResumenJefeCalidadDias.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días para el resúmen al Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtResumenJefeCalidadDias.Text))
                    {
                        errors.Add("La cantidad de días para el resúmen al Jefe de Calidad debe ser un número");
                    }

                    dias = Convert.ToInt32(txtResumenJefeCalidadDias.Text);
                    if (dias < 1)
                    {
                        errors.Add("La cantidad de días para el resúmen al Jefe de Calidad debe ser mayor que 0");
                    }

                    if (dias > 28)
                    {
                        errors.Add("La cantidad de días para el resúmen al Jefe de Calidad no puede ser mayor que 28");
                    }
                }
            }


            {//Resumen Coordinador
                if (txtResumenCoordinadorDias.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días para el resúmen al Coordinador");
                }
                else
                {
                    if (!Utils.validateNumber(txtResumenCoordinadorDias.Text))
                    {
                        errors.Add("La cantidad de días para el resúmen al Coordinador debe ser un número");
                    }

                    dias = Convert.ToInt32(txtResumenCoordinadorDias.Text);
                    if (dias < 1)
                    {
                        errors.Add("La cantidad de días para el resúmen al Jefe de Calidad debe ser mayor que 0");
                    }

                    if (dias > 28)
                    {
                        errors.Add("La cantidad de días para el resúmen al Coordinador no puede ser mayor que 28");
                    }
                }
            }

            return errors;
        }


        private List<string> validatePanelAccionesInmediatas()
        {
            List<string> errors = new List<string>();

            int dias_alerta;
            int dias_limite;

            {//Acción Inmediata Inspector

                dias_alerta = -1;
                dias_limite = -1;

                if (txtAccionInmediataPendienteInspectorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Acción Inmediata pendiente en Inspector");
                }
                else
                {

                    if (!Utils.validateNumber(txtAccionInmediataPendienteInspectorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Acción Inmediata pendiente en Inspector debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtAccionInmediataPendienteInspectorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Acción Inmediata pendiente en Inspector debe ser mayor que 0");
                    }
                }

                if (txtAccionInmediataPendienteInspectorDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Acción Inmediata pendiente en Inspector");
                }
                else
                {

                    if (!Utils.validateNumber(txtAccionInmediataPendienteInspectorDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Acción Inmediata pendiente en Inspector debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtAccionInmediataPendienteInspectorDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Acción Inmediata pendiente en Inspector debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Acción Inmediata pendiente en Inspector debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }

            return errors;
        }


        private List<string> validatePanelInvestigaciones()
        {
            List<string> errors = new List<string>();

            int dias_alerta;
            int dias_limite;

            {// Investigación pendiente Jefe Calidad

                dias_alerta = -1;
                dias_limite = -1;

                if (txtInvestigacionPendienteJefeCalidadDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Investigación pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionPendienteJefeCalidadDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Investigación pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtInvestigacionPendienteJefeCalidadDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Investigación pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if (txtInvestigacionPendienteJefeCalidadDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Investigación pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionPendienteJefeCalidadDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Investigación pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtInvestigacionPendienteJefeCalidadDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Investigación pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Investigación pendiente en Jefe de Calidad debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }


            {// Investigación pendiente Coordinador

                dias_alerta = -1;
                dias_limite = -1;

                if (txtInvestigacionPendienteCoordinadorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Investigación pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionPendienteCoordinadorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Investigación pendiente en Coordinador debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtInvestigacionPendienteCoordinadorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Investigación pendiente en Coordinador debe ser mayor que 0");
                    }
                }

                if (txtInvestigacionPendienteCoordinadorDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Investigación pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionPendienteCoordinadorDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Investigación pendiente en Coordinador debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtInvestigacionPendienteCoordinadorDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Investigación pendiente en Coordinador debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Investigación pendiente en Coordinador debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }


            {// Investigación en curso Jefe Calidad

                dias_alerta = -1;
                dias_limite = -1;

                if (txtInvestigacionEnCursoJefeCalidadDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Investigación en curso en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionEnCursoJefeCalidadDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Investigación en curso en Jefe de Calidad debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtInvestigacionEnCursoJefeCalidadDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Investigación en curso en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if (txtInvestigacionEnCursoJefeCalidadDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Investigación en curso en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionEnCursoJefeCalidadDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Jefe de Calidad debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtInvestigacionEnCursoJefeCalidadDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Jefe de Calidad debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }


            {// Investigación en curso Coordinador

                dias_alerta = -1;
                dias_limite = -1;

                if (txtInvestigacionEnCursoCoordinadorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Investigación en curso en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionEnCursoCoordinadorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Investigación en curso en Coordinador debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtInvestigacionEnCursoCoordinadorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Investigación en curso en Coordinador debe ser mayor que 0");
                    }
                }

                if (txtInvestigacionEnCursoCoordinadorDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Investigación en curso en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionEnCursoCoordinadorDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Coordinador debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtInvestigacionEnCursoCoordinadorDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Coordinador debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Coordinador debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }


            {// Investigación en curso Responsable

                dias_alerta = -1;
                dias_limite = -1;

                if (txtInvestigacionEnCursoResponsableDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Investigación en curso en Responsable");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionEnCursoResponsableDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Investigación en curso en Responsable debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtInvestigacionEnCursoResponsableDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Investigación en curso en Responsable debe ser mayor que 0");
                    }
                }

                if (txtInvestigacionEnCursoResponsableDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Investigación en curso en Responsable");
                }
                else
                {

                    if (!Utils.validateNumber(txtInvestigacionEnCursoResponsableDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Responsable debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtInvestigacionEnCursoResponsableDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Responsable debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Investigación en curso en Responsable debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }



            {// Evaluación pendiente Jefe Calidad

                dias_alerta = -1;
                dias_limite = -1;

                if (txtEvaluacionPendienteJefeCalidadDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Evaluación pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtEvaluacionPendienteJefeCalidadDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Evaluación pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtEvaluacionPendienteJefeCalidadDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Evaluación pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if (txtEvaluacionPendienteJefeCalidadDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Evaluación pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtEvaluacionPendienteJefeCalidadDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Evaluación pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtEvaluacionPendienteJefeCalidadDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Evaluación pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Evaluación pendiente en Jefe de Calidad debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }



            {// Evaluación pendiente Coordinador

                dias_alerta = -1;
                dias_limite = -1;

                if (txtEvaluacionPendienteCoordinadorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Evaluación pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtEvaluacionPendienteCoordinadorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Evaluación pendiente en Coordinador debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtEvaluacionPendienteCoordinadorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Evaluación pendiente en Coordinador debe ser mayor que 0");
                    }
                }

                if (txtEvaluacionPendienteCoordinadorDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Evaluación pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtEvaluacionPendienteCoordinadorDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Evaluación pendiente en Coordinador debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtEvaluacionPendienteCoordinadorDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Evaluación pendiente en Coordinador debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Evaluación pendiente en Coordinador debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }



            return errors;
        }


        private List<string> validatePanelPlanesAccion()
        {
            List<string> errors = new List<string>();

            int dias_alerta;
            int dias_limite;


            {// Plan de Acción pendiente Jefe Calidad

                dias_alerta = -1;
                dias_limite = -1;

                if (txtPlanAccionPendienteJefeCalidadDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Plan de Acción pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtPlanAccionPendienteJefeCalidadDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Plan de Acción pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtPlanAccionPendienteJefeCalidadDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Plan de Acción pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if (txtPlanAccionPendienteJefeCalidadDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Plan de Acción pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtPlanAccionPendienteJefeCalidadDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Plan de Acción pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtPlanAccionPendienteJefeCalidadDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Plan de Acción pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Plan de Acción pendiente en Jefe de Calidad debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }



            {// Plan de Acción pendiente Coordinador

                dias_alerta = -1;
                dias_limite = -1;

                if (txtPlanAccionPendienteCoordinadorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Plan de Acción pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtPlanAccionPendienteCoordinadorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Plan de Acción pendiente en Coordinador debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtPlanAccionPendienteCoordinadorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Plan de Acción pendiente en Coordinador debe ser mayor que 0");
                    }
                }

                if (txtPlanAccionPendienteCoordinadorDiasLimite.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días límite para Plan de Acción pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtPlanAccionPendienteCoordinadorDiasLimite.Text))
                    {
                        errors.Add("La cantidad de días límite para Plan de Acción pendiente en Coordinador debe ser un número");
                    }

                    dias_limite = Convert.ToInt32(txtPlanAccionPendienteCoordinadorDiasLimite.Text);
                    if (dias_limite < 1)
                    {
                        errors.Add("La cantidad de días límite para Plan de Acción pendiente en Coordinador debe ser mayor que 0");
                    }
                }

                if ((dias_alerta >= 0) && (dias_limite >= 0))
                {
                    if (dias_limite < dias_alerta)
                    {
                        errors.Add("La cantidad de días límite para Plan de Acción pendiente en Coordinador debe ser igual o mayor a la cantidad de días alerta");
                    }
                }
            }


            {//Acción Correctiva en curso Jefe Calidad
                if (txtAccionCorrectivaEnCursoJefeCalidadDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Acción Correctiva en curso en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtAccionCorrectivaEnCursoJefeCalidadDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Acción Correctiva en curso en Jefe de Calidad debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtAccionCorrectivaEnCursoJefeCalidadDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Acción Correctiva en curso en Jefe de Calidad debe ser mayor que 0");
                    }
                }
            }


            {//Acción Correctiva en curso Coordinador
                if (txtAccionCorrectivaEnCursoCoordinadorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Acción Correctiva en curso en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtAccionCorrectivaEnCursoCoordinadorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Acción Correctiva en curso en Coordinador debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtAccionCorrectivaEnCursoCoordinadorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Acción Correctiva en curso en Coordinador debe ser mayor que 0");
                    }
                }
            }


            {//Acción Correctiva en curso Responsable
                if (txtAccionCorrectivaEnCursoResponsableDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Acción Correctiva en curso en Responsable");
                }
                else
                {

                    if (!Utils.validateNumber(txtAccionCorrectivaEnCursoResponsableDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Acción Correctiva en curso en Responsable debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtAccionCorrectivaEnCursoResponsableDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Acción Correctiva en curso en Responsable debe ser mayor que 0");
                    }
                }
            }



            {//Verificación pendiente Jefe Calidad
                if (txtVerificacionPendienteJefeCalidadDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Verificación pendiente en Jefe de Calidad");
                }
                else
                {

                    if (!Utils.validateNumber(txtVerificacionPendienteJefeCalidadDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Verificación pendiente en Jefe de Calidad debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtVerificacionPendienteJefeCalidadDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Verificación pendiente en Jefe de Calidad debe ser mayor que 0");
                    }
                }
            }



            {//Verificación pendiente Coordinador
                if (txtVerificacionPendienteCoordinadorDiasAlerta.Text.Length < 1)
                {
                    errors.Add("Se debe indicar la cantidad de días alerta para Verificación pendiente en Coordinador");
                }
                else
                {

                    if (!Utils.validateNumber(txtVerificacionPendienteCoordinadorDiasAlerta.Text))
                    {
                        errors.Add("La cantidad de días alerta para Verificación pendiente en Coordinador debe ser un número");
                    }

                    dias_alerta = Convert.ToInt32(txtVerificacionPendienteCoordinadorDiasAlerta.Text);
                    if (dias_alerta < 1)
                    {
                        errors.Add("La cantidad de días alerta para Verificación pendiente en Coordinador debe ser mayor que 0");
                    }
                }
            }


            return errors;
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


        protected void ibGuardarConfiguracionEmails_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";

            List<string> errors = validatePanels();

            if (errors.Count == 0)
            {
                string status = saveConfig();
                if (status == null)
                {
                    showMessageSuccess("Se ha actualizado exitosamente la configuración para el envío de Emails");
                }
                else
                {
                    showMessageError(status);
                }

            }
            else
            {
                showSummary(errors);
            }
        }


        private string saveConfig()
        {
            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];


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

            List<ConfigEmailSender> listConfigEmailSender = new List<ConfigEmailSender>();
            ConfigEmailSender ces;

            {//Resumenes

                ces = new ConfigEmailSender("Resumen", "Jefe Calidad", id_centro, -1, -1, Convert.ToInt32(txtResumenJefeCalidadDias.Text), chbResumenJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Resumen", "Coordinador", id_centro, -1, -1, Convert.ToInt32(txtResumenCoordinadorDias.Text), chbResumenCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);
            }


            {//Acciones Inmediatas

                ces = new ConfigEmailSender("Acción inmediata pendiente", "Inspector", id_centro, Convert.ToInt32(txtAccionInmediataPendienteInspectorDiasAlerta.Text), Convert.ToInt32(txtAccionInmediataPendienteInspectorDiasLimite.Text), -1, chbAccionInmediataPendienteInspectorActivado.Checked);
                listConfigEmailSender.Add(ces);
            }


            {//Investigaciones

                ces = new ConfigEmailSender("Investigación pendiente", "Jefe Calidad", id_centro, Convert.ToInt32(txtInvestigacionPendienteJefeCalidadDiasAlerta.Text), Convert.ToInt32(txtInvestigacionPendienteJefeCalidadDiasLimite.Text), -1, chbInvestigacionPendienteJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Investigación pendiente", "Coordinador", id_centro, Convert.ToInt32(txtInvestigacionPendienteCoordinadorDiasAlerta.Text), Convert.ToInt32(txtInvestigacionPendienteCoordinadorDiasLimite.Text), -1, chbInvestigacionPendienteCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Investigación en curso", "Jefe Calidad", id_centro, Convert.ToInt32(txtInvestigacionEnCursoJefeCalidadDiasAlerta.Text), Convert.ToInt32(txtInvestigacionEnCursoJefeCalidadDiasLimite.Text), -1, chbInvestigacionEnCursoJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Investigación en curso", "Coordinador", id_centro, Convert.ToInt32(txtInvestigacionEnCursoCoordinadorDiasAlerta.Text), Convert.ToInt32(txtInvestigacionEnCursoCoordinadorDiasLimite.Text), -1, chbInvestigacionEnCursoCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Investigación en curso", "Responsable", id_centro, Convert.ToInt32(txtInvestigacionEnCursoResponsableDiasAlerta.Text), Convert.ToInt32(txtInvestigacionEnCursoResponsableDiasLimite.Text), -1, chbInvestigacionEnCursoResponsableActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Evaluación pendiente", "Jefe Calidad", id_centro, Convert.ToInt32(txtEvaluacionPendienteJefeCalidadDiasAlerta.Text), Convert.ToInt32(txtEvaluacionPendienteJefeCalidadDiasLimite.Text), -1, chbEvaluacionPendienteJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Evaluación pendiente", "Coordinador", id_centro, Convert.ToInt32(txtEvaluacionPendienteCoordinadorDiasAlerta.Text), Convert.ToInt32(txtEvaluacionPendienteCoordinadorDiasLimite.Text), -1, chbEvaluacionPendienteCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);

            }


            {//Planes de Acción

                ces = new ConfigEmailSender("Plan de acción pendiente", "Jefe Calidad", id_centro, Convert.ToInt32(txtPlanAccionPendienteJefeCalidadDiasAlerta.Text), Convert.ToInt32(txtPlanAccionPendienteJefeCalidadDiasLimite.Text), -1, chbPlanAccionPendienteJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Plan de acción pendiente", "Coordinador", id_centro, Convert.ToInt32(txtPlanAccionPendienteCoordinadorDiasAlerta.Text), Convert.ToInt32(txtPlanAccionPendienteCoordinadorDiasLimite.Text), -1, chbPlanAccionPendienteCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Acción correctiva en curso", "Jefe Calidad", id_centro, Convert.ToInt32(txtAccionCorrectivaEnCursoJefeCalidadDiasAlerta.Text), -1, -1, chbAccionCorrectivaEnCursoJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Acción correctiva en curso", "Coordinador", id_centro, Convert.ToInt32(txtAccionCorrectivaEnCursoCoordinadorDiasAlerta.Text), -1, -1, chbAccionCorrectivaEnCursoCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Acción correctiva en curso", "Responsable", id_centro, Convert.ToInt32(txtAccionCorrectivaEnCursoResponsableDiasAlerta.Text), -1, -1, chbAccionCorrectivaEnCursoResponsableActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Verificación pendiente", "Jefe Calidad", id_centro, Convert.ToInt32(txtVerificacionPendienteJefeCalidadDiasAlerta.Text), -1, -1, chbVerificacionPendienteJefeCalidadActivado.Checked);
                listConfigEmailSender.Add(ces);

                ces = new ConfigEmailSender("Verificación pendiente", "Coordinador", id_centro, Convert.ToInt32(txtVerificacionPendienteCoordinadorDiasAlerta.Text), -1, -1, chbVerificacionPendienteCoordinadorActivado.Checked);
                listConfigEmailSender.Add(ces);
            }


            string status = LogicController.updateConfiguracionEmailSender(
                                                                            listConfigEmailSender,
                                                                            id_centro,
                                                                            usuario.getUsuario(),
                                                                            Request.ServerVariables["REMOTE_ADDR"],
                                                                            owner
                                                                        );
            if (status == null)
            {
                return null;
            }
            else
            {
                return status;
            }
        }


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }


        private void showMessageSuccess(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxSuccess", "<script type=\"text/javascript\">showMessageSuccess('" + message + "');</script>", false);
        }
    }
}