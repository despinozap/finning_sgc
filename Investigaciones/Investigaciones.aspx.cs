using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using System.Data;
using NCCSAN.Source.Entity;

namespace NCCSAN.Investigaciones
{
    public partial class Investigaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string msg;

                {//Acceso del Usuario a la Página
                    if (Session["usuario"] == null)
                    {
                        msg = "Error al recuperar tu información de Usuario";
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

                msg = loadResumeTable();
                if (msg != null)
                {
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }
        }


        private string loadResumeTable()
        {
            if (Session["id_centro"] == null)
            {
                return "No se puede recuperar información del Centro";
            }

            string id_centro = (string)Session["id_centro"];

            int investigaciones_pendientes = LogicController.getCantidadEventos(id_centro, "Investigación pendiente");
            if (investigaciones_pendientes < 0)
            {
                return "No se puede recuperar la información de Investigaciones pendientes";
            }

            int investigaciones_en_curso = LogicController.getCantidadEventos(id_centro, "Investigación en curso");
            if (investigaciones_en_curso < 0)
            {
                return "No se puede recuperar la información de Investigaciones en curso";
            }

            int evaluaciones_pendientes = LogicController.getCantidadEventos(id_centro, "Evaluación pendiente");
            if (evaluaciones_pendientes < 0)
            {
                return "No se puede recuperar la información de Evaluaciones pendientes";
            }

            updateGVResumenInvestigaciones();

            ((Image)gvResumenInvestigaciones.Rows[0].FindControl("imgIcono")).ImageUrl = "~/Images/Icon/ball_red.png";
            ((Label)gvResumenInvestigaciones.Rows[0].FindControl("lbEstado")).Text = "Investigación pendiente";
            ((Label)gvResumenInvestigaciones.Rows[0].FindControl("lbCantidad")).Text = Convert.ToString(investigaciones_pendientes);

            ((Image)gvResumenInvestigaciones.Rows[1].FindControl("imgIcono")).ImageUrl = "~/Images/Icon/ball_yellow.png";
            ((Label)gvResumenInvestigaciones.Rows[1].FindControl("lbEstado")).Text = "Investigación en curso";
            ((Label)gvResumenInvestigaciones.Rows[1].FindControl("lbCantidad")).Text = Convert.ToString(investigaciones_en_curso);

            ((Image)gvResumenInvestigaciones.Rows[2].FindControl("imgIcono")).ImageUrl = "~/Images/Icon/ball_red_step.gif";
            ((Label)gvResumenInvestigaciones.Rows[2].FindControl("lbEstado")).Text = "Evaluación pendiente";
            ((Label)gvResumenInvestigaciones.Rows[2].FindControl("lbCantidad")).Text = Convert.ToString(evaluaciones_pendientes);

            return null;
        }


        private void updateGVResumenInvestigaciones()
        {
            DataTable dt = getDTResumenInvestigaciones();
            DataRow dr;

            for (int i = 0; i < 3; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Image();
                dr[1] = new Label();
                dr[2] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvResumenInvestigaciones.DataSource = dt;
            gvResumenInvestigaciones.DataBind();
        }


        private DataTable getDTResumenInvestigaciones()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Icono");
            dt.Columns.Add("Estado");
            dt.Columns.Add("Cantidad");

            dt.AcceptChanges();

            return dt;
        }
    }
}