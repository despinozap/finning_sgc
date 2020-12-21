using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;

namespace NCCSAN
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] != null)
            {
                Usuario u = (Usuario) Session["usuario"];
                if (u != null)
                    LogicController.removeSesion(u.getUsuario());
            }

            Session.RemoveAll();

            Response.Cookies["usertoken"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("~/Default.aspx", true);
        }
    }
}