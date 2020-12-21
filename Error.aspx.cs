using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                lbErrorSaved.Visible = false;

                if (Request.QueryString["msg"] != null)
                {
                    string msg = Request.QueryString["msg"];
                    lbMensaje.Text = msg;

                    if (saveErrorMessage(msg))
                    {
                        lbErrorSaved.Visible = true;
                    }
                }
                else
                {
                    lbMensaje.Text = "Error desconocido";
                }
            }
        }


        private bool saveErrorMessage(string msg)
        {
            if (Session["id_centro"] == null)
            {
                return false;
            }

            string id_centro = (string)Session["id_centro"];

            if (Session["usuario"] == null)
            {
                return false;
            }

            Usuario usuario = (Usuario)Session["usuario"];
            string previous_page = Request.ServerVariables["HTTP_REFERER"];

            return ActionLogger.error(id_centro, usuario.getUsuario(), msg, previous_page, Request.ServerVariables["REMOTE_ADDR"]);
        }
    }
}