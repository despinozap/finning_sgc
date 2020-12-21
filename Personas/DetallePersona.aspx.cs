using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.Personas
{
    public partial class DetallePersona : System.Web.UI.Page
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

                if ((Session["RutDetalle"] != null) && (Session["PreviousPagePersona"] != null))
                {
                    string rut = (string)Session["RutDetalle"];
                    string previuosPage = (string)Session["PreviousPagePersona"];
                    if ((rut != null) && (previuosPage != null))
                    {
                        hfRutPersona.Value = rut;
                        hfPreviousPage.Value = previuosPage;
                    }
                    else
                    {
                        string msg = "Error al pasar el parámetro";
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }
                }
                else
                {
                    string msg = "No se ha pasado el parámetro";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }
        }

        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }

    }
}