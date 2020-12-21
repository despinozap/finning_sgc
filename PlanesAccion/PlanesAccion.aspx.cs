using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using System.Data;
using NCCSAN.Source.Entity;

namespace NCCSAN.PlanesAccion
{
    public partial class PlanesAccion : System.Web.UI.Page
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


            int plan_accion_pendientes = LogicController.getCantidadEventos(id_centro, "Plan de acción pendiente");
            if (plan_accion_pendientes < 0)
            {
                return "No se puede recuperar la información de Planes de acción pendientes";
            }

            int plan_accion_en_curso = LogicController.getCantidadEventos(id_centro, "Plan de acción en curso");
            if (plan_accion_en_curso < 0)
            {
                return "No se puede recuperar la información de Planes de acción en curso";
            }

            updateGVResumenInvestigaciones();

            ((Image)gvResumenPlanesAccion.Rows[0].FindControl("imgIcono")).ImageUrl = "~/Images/Icon/ball_red.png";
            ((Label)gvResumenPlanesAccion.Rows[0].FindControl("lbEstado")).Text = "Plan de acción pendiente";
            ((Label)gvResumenPlanesAccion.Rows[0].FindControl("lbCantidad")).Text = Convert.ToString(plan_accion_pendientes);

            ((Image)gvResumenPlanesAccion.Rows[1].FindControl("imgIcono")).ImageUrl = "~/Images/Icon/ball_yellow.png";
            ((Label)gvResumenPlanesAccion.Rows[1].FindControl("lbEstado")).Text = "Plan de acción en curso";
            ((Label)gvResumenPlanesAccion.Rows[1].FindControl("lbCantidad")).Text = Convert.ToString(plan_accion_en_curso);

            return null;
        }


        private void updateGVResumenInvestigaciones()
        {
            DataTable dt = getDTResumenInvestigaciones();
            DataRow dr;

            for (int i = 0; i < 2; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Image();
                dr[1] = new Label();
                dr[2] = new Label();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvResumenPlanesAccion.DataSource = dt;
            gvResumenPlanesAccion.DataBind();
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