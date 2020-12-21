using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;

namespace NCCSAN.Registros
{
    public partial class TareasVencidas : System.Web.UI.Page
    {
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

                string loaded = loadTareasVencidas();
                if (loaded != null)
                {
                    string msg = loaded;
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }
        }



        private Dictionary<string, bool> getFlags()
        {
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            flags.Add("Investigación pendiente", chbInvestigacionPendiente.Checked);
            flags.Add("Investigación en curso", chbInvestigacionEnCurso.Checked);
            flags.Add("Evaluación pendiente", chbEvaluacionPendiente.Checked);
            flags.Add("Plan de acción pendiente", chbPlanAccionPendiente.Checked);
            flags.Add("Acción correctiva en curso", chbAccionCorrectivaEnCurso.Checked);
            flags.Add("Verificación pendiente", chbVerificacionPendiente.Checked);

            return flags;
        }



        private string loadTareasVencidas()
        {

            if (Session["usuario"] == null)
            {
                return "Error al recuperar la información de Usuario";
            }
            Usuario usuario = (Usuario)Session["usuario"];

            Dictionary<string, bool> flags = getFlags();

            List<Tarea> listTareasVencidas = LogicController.getListExpiredTasks(usuario.getIDCentro(), usuario.getNombreRol(), flags);
            if (listTareasVencidas == null)
            {
                return "Error al recuperar la lista de tareas vencidas";
            }

            DataTable dt = getDTTareasVencidas();

            DataRow dr;
            for (int i = 0; i < listTareasVencidas.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Label();
                dr[5] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvTareasVencidas.DataSource = dt;
            gvTareasVencidas.DataBind();

            string nombre_tarea;
            ImageButton ibIr;
            for (int i = 0; i < listTareasVencidas.Count; i++)
            {
                nombre_tarea = listTareasVencidas[i].getNombreTarea();

                ((Label)gvTareasVencidas.Rows[i].Cells[0].FindControl("lbCodigoEvento")).Text = listTareasVencidas[i].getCodigoEvento();
                ((Label)gvTareasVencidas.Rows[i].Cells[1].FindControl("lbWO")).Text = listTareasVencidas[i].getWO();
                ((Label)gvTareasVencidas.Rows[i].Cells[2].FindControl("lbTarea")).Text = nombre_tarea;
                ((Label)gvTareasVencidas.Rows[i].Cells[3].FindControl("lbFechaVencimiento")).Text = listTareasVencidas[i].getFechaVencimiento();
                ((Label)gvTareasVencidas.Rows[i].Cells[4].FindControl("lbDiasVencimiento")).Text = Convert.ToString(listTareasVencidas[i].getDiasVencimiento());
                if (listTareasVencidas[i].getRUTResponsable() != null)
                {
                    ((Label)gvTareasVencidas.Rows[i].Cells[4].FindControl("lbRUTResponsable")).Text = listTareasVencidas[i].getRUTResponsable();
                }
                else
                {
                    ((Label)gvTareasVencidas.Rows[i].Cells[4].FindControl("lbRUTResponsable")).Text = "--";
                }

                if (listTareasVencidas[i].getNombreResponsable() != null)
                {
                    ((Label)gvTareasVencidas.Rows[i].Cells[5].FindControl("lbNombreResponsable")).Text = listTareasVencidas[i].getNombreResponsable();
                }
                else
                {
                    ((Label)gvTareasVencidas.Rows[i].Cells[5].FindControl("lbNombreResponsable")).Text = "--";
                }

                ibIr = (ImageButton)gvTareasVencidas.Rows[i].Cells[6].FindControl("ibIr");

                if (nombre_tarea.Equals("Investigación pendiente"))
                {
                    ibIr.CommandName = "IrInvestigacionPendiente";
                }
                else if (nombre_tarea.Equals("Investigación en curso"))
                {
                    ibIr.CommandName = "IrInvestigacionEnCurso";
                }
                else if (nombre_tarea.Equals("Evaluación pendiente"))
                {
                    ibIr.CommandName = "IrEvaluacionPendiente";
                }
                else if (nombre_tarea.Equals("Plan de acción pendiente"))
                {
                    ibIr.CommandName = "IrPlanAccionPendiente";
                }
                else if (nombre_tarea.Equals("Acción correctiva en curso"))
                {
                    ibIr.CommandName = "IrPlanAccionEnCurso";
                }
                else if (nombre_tarea.Equals("Verificación pendiente"))
                {
                    ibIr.CommandName = "IrVerificacionPendiente";
                }


                ibIr.CommandArgument = listTareasVencidas[i].getCodigoEvento();

            }

            return null;
        }



        private DataTable getDTTareasVencidas()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Codigo");
            dt.Columns.Add("WO");
            dt.Columns.Add("Tarea");
            dt.Columns.Add("Fecha de Vencimiento");
            dt.Columns.Add("RUT Responsable");
            dt.Columns.Add("Nombre Responsable");

            dt.AcceptChanges();

            return dt;
        }

        protected void gvTareasVencidas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string codigo_evento;
            if (e.CommandName.Equals("IrInvestigacionPendiente"))
            {
                codigo_evento = (string)e.CommandArgument;

                Response.Redirect("~/Investigaciones/ListarInvestigacionesPendientes.aspx?ce=" + codigo_evento, true);
            }
            else if (e.CommandName.Equals("IrInvestigacionEnCurso"))
            {
                codigo_evento = (string)e.CommandArgument;

                Response.Redirect("~/Investigaciones/ListarInvestigacionesEnCurso.aspx?ce=" + codigo_evento, true);
            }
            else if (e.CommandName.Equals("IrEvaluacionPendiente"))
            {
                codigo_evento = (string)e.CommandArgument;

                Response.Redirect("~/Investigaciones/ListarEvaluacionesPendientes.aspx?ce=" + codigo_evento, true);
            }
            else if (e.CommandName.Equals("IrPlanAccionPendiente"))
            {
                codigo_evento = (string)e.CommandArgument;

                Response.Redirect("~/PlanesAccion/ListarPlanesAccionPendientes.aspx?ce=" + codigo_evento, true);
            }
            else if (e.CommandName.Equals("IrPlanAccionEnCurso"))
            {
                codigo_evento = (string)e.CommandArgument;

                Response.Redirect("~/PlanesAccion/ListarPlanesAccionEnCurso.aspx?ce=" + codigo_evento, true);
            }
            else if (e.CommandName.Equals("IrVerificacionPendiente"))
            {
                codigo_evento = (string)e.CommandArgument;

                Response.Redirect("~/PlanesAccion/ListarPlanesAccionCerrados.aspx?ce=" + codigo_evento, true);
            }
        }

        protected void btFilter_Click(object sender, EventArgs e)
        {
            string loaded = loadTareasVencidas();
            if (loaded != null)
            {
                string msg = loaded;
                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
            }
        }

    }
}