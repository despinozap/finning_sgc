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
    public partial class SiteMaster : System.Web.UI.MasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string pagename = HttpContext.Current.Request.Url.LocalPath;

            /*
            if (!IsPostBack)
            {
                if (Request.Cookies.Get("isfirsttime") == null)
                {
                    string pn = "~" + pagename;

                    HttpCookie cookie = new HttpCookie("isfirsttime");
                    cookie.Expires = DateTime.Now.AddDays(360);
                    cookie.Value = "1";
                    Response.Cookies.Add(cookie);

                    Response.Redirect(pn);
                }
            }
            */

            //Response.Write("<script>alert('" + pagename + "')</script>");

            //Redireccionar usuarios no autenticados

            if (Session["usuario"] == null)
            {
                //Al ingresar por primera vez al sistema intenta ejecutar el trigger diario
                //string server_home_path = Utils.getAppFullURL(Request, ResolveUrl("~"));
                string server_home_path = "http://CLSF014-CSN03" + ResolveUrl("~");
                Trigger.executeDailyTrigger(server_home_path);


                if (Request.Cookies["usertoken"] != null)
                {
                    string user = LogicController.getUsuarioFromToken(Request.Cookies["usertoken"].Value);
                    if (user != null)
                    {
                        Usuario u = LogicController.getUsuario(user);
                        Session["usuario"] = u;
                        Session["id_centro"] = u.getIDCentro();


                        if (Session["LinkedPage"] != null)
                        {
                            string linkedPage = (string)Session["LinkedPage"];
                            Session.Remove("LinkedPage");

                            Response.Redirect(linkedPage, true);
                        }
                        else if (u.getNombreRol().Equals("Cliente"))
                        {
                            string nombre_cliente = LogicController.getNombreClienteByClientePersona(u.getRutPersona());
                            if (nombre_cliente != null)
                            {
                                Session["nombre_cliente"] = nombre_cliente;
                                Response.Redirect("~/Clientes/EventosRegistrados.aspx", true);
                            }
                            else
                            {
                                Response.Redirect("~/Logout.aspx", true);
                            }
                        }
                        else
                        {
                            Response.Redirect("~/Personas/TareasPendientes.aspx", true);
                        }
                    }
                }


                if (!pagename.ToUpper().EndsWith("LOGIN.ASPX")) //Si no esta logeado y consulta alguna pagina (!=login)
                {
                    Response.Redirect("~/Account/Login.aspx", true);
                }

            }
            else
            {
                if (pagename.ToUpper().EndsWith("LOGIN.ASPX"))
                {
                    //do nothing
                }
                else
                {
                    if (pagename.ToUpper().EndsWith("TAREASPENDIENTES.ASPX")) //Si esta logeado y recibe el postback del login (hacia tareaspendientes)
                    {
                        if (Session["LinkedPage"] != null)
                        {
                            string linkedPage = (string)Session["LinkedPage"];
                            Session.Remove("LinkedPage");

                            Response.Redirect(linkedPage, true);
                        }
                    }

                    try
                    {
                        pnUsuario.Visible = true;
                        Usuario u = (Usuario)Session["usuario"];
                        if (u.getSexoPersona().ToUpper().Equals("F"))
                            lbSesion.Text = "Bienvenida ";
                        else
                            lbSesion.Text = "Bienvenido ";
                        lbNombre.Text = u.getNombrePersona().Split(',')[1].Split(' ')[1] + " " + u.getNombrePersona().Split(',')[0] + " / " + u.getUsuario();
                        lbRol.Text = "[" + u.getNombreRol() + "]";
                        lbCentro.Text = u.getNombreCentro();
                        imgPais.ImageUrl = "~/Images/Country/" + u.getPaisCentro().ToLower() + ".png";
                        
                    }
                    catch (Exception ex)
                    {
                        pnUsuario.Visible = false;
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode("Se ha producido un error al recuperar información del usuario"), false);
                    }
                }
            }


            if (Session["usuario"] != null)
            {
                Usuario u = (Usuario)Session["usuario"];
                List<MenuItem> listMenuItem = getListMenuItems();

                NavigationMenu.Items.Clear();

                if (listMenuItem == null)
                {
                    listMenuItem = LogicController.getNavigationItems(u.getNombreRol());
                    setListMenuItem(listMenuItem);
                }

                if (listMenuItem != null)
                {
                    foreach (MenuItem mi in listMenuItem)
                        NavigationMenu.Items.Add(mi);
                }
            }

        }


        private List<MenuItem> getListMenuItems()
        {
            List<MenuItem> listMenuItem;
            if (Session["listMenuItems"] != null)
            {
                listMenuItem = (List<MenuItem>)Session["listMenuItems"];
            }
            else
            {
                listMenuItem = null;
            }

            return listMenuItem;
        }


        private void setListMenuItem(List<MenuItem> listMenuItem)
        {
            if (listMenuItem != null)
                Session["listMenuItems"] = listMenuItem;
        }

    }
}
