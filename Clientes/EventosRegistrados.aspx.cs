using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;

namespace NCCSAN.Clientes
{
    public partial class EventosRegistrados : System.Web.UI.Page
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

                    hfRutPersona.Value = u.getRutPersona();
                }


                ddlMes.Items.Clear();
                ddlMes.Items.Add(new ListItem("[Todos]", ""));
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
                ddlAnio.Items.Add(new ListItem("[Todos]", ""));
                int currentYear = DateTime.Now.Year;
                for (int year = 2014; year <= currentYear; year++)
                {
                    ddlAnio.Items.Add(new ListItem(Convert.ToString(year), Convert.ToString(year)));
                }

                {
                    List<ListItem> listItemsEstados = getListItemsEstados();
                    ddlEstado1.Items.Clear();
                    ddlEstado1.Items.Add(new ListItem("Seleccione..", ""));
                    ddlEstado2.Items.Clear();
                    ddlEstado2.Items.Add(new ListItem("Seleccione..", ""));
                    ddlEstado3.Items.Clear();
                    ddlEstado3.Items.Add(new ListItem("Seleccione..", ""));

                    foreach (ListItem li in listItemsEstados)
                    {
                        ddlEstado1.Items.Add(li);
                        ddlEstado2.Items.Add(li);
                        ddlEstado3.Items.Add(li);
                    }
                }

                Session.Remove("CodigoEventoSeleccionado");

                {//Filtrar búsqueda por parámetro
                    if (Request.QueryString["ce"] != null)
                    {
                        string codigo_evento = Request.QueryString["ce"];
                        txtCodigoWO.Text = codigo_evento;

                        SDSEventos.DataBind();
                        gvEventos.DataBind();
                    }
                }
            }
        }


        private List<ListItem> getListItemsEstados()
        {
            List<ListItem> listItemsEstados = new List<ListItem>();
            listItemsEstados.Add(new ListItem("Acción inmediata pendiente", "Acción inmediata pendiente"));
            listItemsEstados.Add(new ListItem("Investigación pendiente", "Investigación pendiente"));
            listItemsEstados.Add(new ListItem("Investigación en curso", "Investigación en curso"));
            listItemsEstados.Add(new ListItem("Evaluación pendiente", "Evaluación pendiente"));
            listItemsEstados.Add(new ListItem("Plan de acción pendiente", "Plan de acción pendiente"));
            listItemsEstados.Add(new ListItem("Plan de acción en curso", "Plan de acción en curso"));
            listItemsEstados.Add(new ListItem("Verificación pendiente", "Verificación pendiente"));
            listItemsEstados.Add(new ListItem("Cerrado", "Cerrado"));

            return listItemsEstados;
        }


        protected void ibClearCodigoWO_Click(object sender, ImageClickEventArgs e)
        {
            txtCodigoWO.Text = "";
        }


        protected void ddlAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            setFechaValue();
        }


        protected void ddlMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            setFechaValue();
        }


        private void setFechaValue()
        {
            if (ddlAnio.SelectedIndex > 0)
            {
                if (ddlMes.SelectedIndex > 0)
                {
                    hfFecha.Value = "-" + ddlMes.SelectedValue + "-" + ddlAnio.SelectedValue;
                }
                else
                {
                    hfFecha.Value = "-" + ddlAnio.SelectedValue;
                }
            }
            else
            {
                if (ddlMes.SelectedIndex > 0)
                {
                    hfFecha.Value = "-" + ddlMes.SelectedValue + "-";
                }
                else
                {
                    hfFecha.Value = "";
                }
            }
        }


        protected void gvEventos_DataBound(object sender, EventArgs e)
        {
            if (gvEventos.Rows.Count > 0)
            {
                pnOpcionesLista.Visible = true;
            }
            else
            {
                pnOpcionesLista.Visible = false;
            }

            Image imgEstadoIcono;
            Label lbDiasActivo;
            string estado;
            for (int i = 0; i < gvEventos.Rows.Count; i++)
            {
                imgEstadoIcono = (Image)gvEventos.Rows[i].Cells[0].FindControl("imgEstadoIcono");
                lbDiasActivo = (Label)gvEventos.Rows[i].Cells[8].FindControl("lbDiasActivo");
                estado = HttpUtility.HtmlDecode(gvEventos.Rows[i].Cells[9].Text);

                if (estado.Equals("Registrado"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_white.png";
                }
                else if (estado.Equals("Revisión pendiente"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                }
                else if (estado.Equals("Acción inmediata pendiente"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red_step.gif";
                }

                else if (estado.Equals("Investigación pendiente"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                }
                else if (estado.Equals("Investigación en curso"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";
                }
                else if (estado.Equals("Evaluación pendiente"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red_step.gif";
                }
                else if (estado.Equals("Plan de acción pendiente"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                }
                else if (estado.Equals("Plan de acción en curso"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";
                }
                else if (estado.Equals("Verificación pendiente"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_grey.png";
                }
                else if (estado.Equals("Cerrado"))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_green.png";
                    lbDiasActivo.Text = "--";
                }

                imgEstadoIcono.ToolTip = estado;
            }
        }


        private void ExportToExcel(string prefixFilename, List<Control> controls)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Page pagina = new Page();
            HtmlForm form = new HtmlForm();

            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string FileName = prefixFilename + "(" + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToLongTimeString() + ")" + ".xls";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            pagina.EnableEventValidation = false;
            pagina.DesignerInitialize();
            pagina.Controls.Add(form);

            foreach (Control control in controls)
                form.Controls.Add(control);

            pagina.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }


        protected void ibExportToExcel_Click(object sender, ImageClickEventArgs e)
        {
            gvEventos.GridLines = GridLines.Both;
            gvEventos.HeaderStyle.Font.Bold = true;
            gvEventos.EnableViewState = false;
            gvEventos.AllowPaging = false;
            gvEventos.DataBind();

            int column_count = gvEventos.Columns.Count;
            gvEventos.Columns[0].Visible = false;
            for (int i = 10; i < column_count; i++)
            {
                gvEventos.Columns[i].Visible = false;
            }

            List<Control> controls = new List<Control>();
            DataTable dtFiltro = new DataTable();
            dtFiltro.Columns.Add("Filtros");
            dtFiltro.Columns.Add("Valor");
            DataRow dr;

            dr = dtFiltro.NewRow();
            dr[0] = "Código o W/O";
            dr[1] = txtCodigoWO.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Año";
            dr[1] = ddlAnio.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Mes";
            dr[1] = ddlMes.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            string estados = "";
            if (chbFiltroEstado.Checked)
            {
                estados = "[Todos]";
            }
            else
            {
                if (ddlEstado1.SelectedIndex > 0)
                {
                    estados += ddlEstado1.SelectedValue;
                }

                if (ddlEstado2.SelectedIndex > 0)
                {
                    if (estados.Length > 0)
                    {
                        estados += ", ";
                    }

                    estados += ddlEstado2.SelectedValue;
                }

                if (ddlEstado3.SelectedIndex > 0)
                {
                    if (estados.Length > 0)
                    {
                        estados += ", ";
                    }

                    estados += ddlEstado3.SelectedValue;
                }
            }

            dr = dtFiltro.NewRow();
            dr[0] = "Estado";
            dr[1] = estados;
            dtFiltro.Rows.Add(dr);


            dtFiltro.AcceptChanges();
            GridView gvFiltro = new GridView();
            gvFiltro.DataSource = dtFiltro;
            gvFiltro.DataBind();

            gvFiltro.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#000000");
            gvFiltro.HeaderStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
            gvFiltro.RowStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC00");
            gvFiltro.RowStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000");


            controls.Add(gvFiltro);

            controls.Add(gvEventos);
            ExportToExcel("Eventos", controls);

            gvEventos.AllowPaging = true;
            gvEventos.DataBind();

            gvEventos.Columns[0].Visible = true;
            for (int i = 10; i < column_count; i++)
            {
                gvEventos.Columns[i].Visible = true;
            }

        }


        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }

        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }

        protected void gvEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetalleEvento"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    Session["CodigoEventoSeleccionado"] = gvEventos.Rows[index].Cells[1].Text;
                    Session["PreviousPageEvento"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Clientes/DetalleEvento.aspx");
                }
            }
            else if (e.CommandName.Equals("EditarEvento"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    string codigo_evento = gvEventos.Rows[index].Cells[1].Text;

                    if (Session["usuario"] == null)
                    {
                        showMessageError("No se puede recuperar la información del Usuario");

                        return;
                    }

                    //Filtro de edición
                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();
                    if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                    {
                        PersonaInfo creador = LogicController.getCreadorEvento(codigo_evento);
                        if (creador == null)
                        {
                            showMessageError("No se puede recuperar la información del Evento");

                            return;
                        }

                        if (!creador.getRut().Equals(usuario.getRutPersona()))
                        {
                            showMessageError("Sólo puedes editar los eventos que tú has reportado");

                            return;
                        }
                    }


                    int existsCode = LogicController.evaluacionExists(codigo_evento);
                    if (existsCode < 0)
                    {
                        showMessageError("Fallo al determinar si el Evento tiene alguna Evaluación asociada");

                        return;
                    }
                    else if (existsCode > 0)
                    {
                        showMessageError("No se puede editar el Evento mientras tenga una Evaluación asociada");

                        return;
                    }


                    existsCode = LogicController.investigacionExists(codigo_evento);
                    if (existsCode < 0)
                    {
                        showMessageError("Falló al determinar si el Evento tiene alguna Investigación asociada");

                        return;
                    }
                    else if (existsCode > 0)
                    {
                        string nombre_rol = usuario.getNombreRol();
                        if ((!nombre_rol.Equals("Jefe Calidad")) && (!nombre_rol.Equals("Coordinador")) && (!nombre_rol.Equals("Inspector")))
                        {
                            showMessageError("No se puede editar el Evento mientras tenga una Investigación asociada");

                            return;
                        }
                    }


                    existsCode = LogicController.accionInmediataExists(codigo_evento);
                    if (existsCode < 0)
                    {
                        showMessageError("Falló al determinar si el Evento tiene alguna Acción Inmediata asociada");

                        return;
                    }
                    else if (existsCode > 0)
                    {
                        showMessageError("No se puede editar el Evento mientras tenga una Acción Inmediata asociada");

                        return;
                    }


                    Session["CodigoEventoSeleccionado"] = gvEventos.Rows[index].Cells[1].Text;
                    Session["PreviousPage"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Clientes/EditarEvento.aspx");
                }
            }
            else if (e.CommandName.Equals("EliminarEvento"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {
                    string codigo_evento = gvEventos.Rows[index].Cells[1].Text;

                    if (Session["usuario"] == null)
                    {
                        showMessageError("No se puede recuperar la información del Usuario");

                        return;
                    }

                    //Filtro de eliminación
                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();
                    if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                    {
                        PersonaInfo creador = LogicController.getCreadorEvento(codigo_evento);
                        if (creador == null)
                        {
                            showMessageError("No se puede recuperar la información del Evento");

                            return;
                        }

                        if (!creador.getRut().Equals(usuario.getRutPersona()))
                        {
                            showMessageError("Sólo puedes eliminar los eventos que tú has reportado");

                            return;
                        }
                    }


                    int existsCode = LogicController.evaluacionExists(codigo_evento);
                    if (existsCode < 0)
                    {
                        showMessageError("Fallo al determinar si el Evento tiene alguna Evaluación asociada");

                        return;
                    }
                    else if (existsCode > 0)
                    {
                        showMessageError("No se puede eliminar el Evento mientras tenga una Evaluación asociada");

                        return;
                    }


                    existsCode = LogicController.investigacionExists(codigo_evento);
                    if (existsCode < 0)
                    {
                        showMessageError("Falló al determinar si el Evento tiene alguna Investigación asociada");

                        return;
                    }
                    else if (existsCode > 0)
                    {
                        showMessageError("No se puede eliminar el Evento mientras tenga una Investigación asociada");

                        return;
                    }


                    existsCode = LogicController.accionInmediataExists(codigo_evento);
                    if (existsCode < 0)
                    {
                        showMessageError("Falló al determinar si el Evento tiene alguna Acción Inmediata asociada");

                        return;
                    }
                    else if (existsCode > 0)
                    {
                        showMessageError("No se puede eliminar el Evento mientras tenga una Acción Inmediata asociada");

                        return;
                    }


                    hfCodigoEventoEliminar.Value = codigo_evento;

                    lbMessageConfirmEliminarEvento.Text = "¿Realmente deseas eliminar el Evento con código <b>" + codigo_evento + "</b>?";
                    hfConfirmEliminarEvento_ModalPopupExtender.Show();
                }
            }
        }


        private string removeEvento(string codigo_evento)
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

            string status = LogicController.removeEvento(
                                                            codigo_evento,
                                                            usuario.getUsuario(),
                                                            Request.ServerVariables["REMOTE_ADDR"]
                                                        );

            if (status == null)
            {
                return null;
            }
            else
            {
                return "Error al eliminar el Evento con código " + codigo_evento;
            }
        }



        protected void chbFiltroEstado_CheckedChanged(object sender, EventArgs e)
        {
            if (chbFiltroEstado.Checked)
            {
                ddlEstado1.Enabled = ddlEstado2.Enabled = ddlEstado3.Enabled = false;
                hfFiltroEstado.Value = "";
            }
            else
            {
                ddlEstado1.Enabled = ddlEstado2.Enabled = ddlEstado3.Enabled = true;
                hfFiltroEstado.Value = Utils.getUniqueCode();
            }

            uPanel.Update();
        }


        protected void btConfirmEliminarEventoNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void btConfirmEliminarEventoSi_Click(object sender, EventArgs e)
        {
            string status = removeEvento(hfCodigoEventoEliminar.Value);
            if (status == null)
            {
                Session.Remove("listArchivos");

                lbMessage.Text = "La eliminó satisfactoriamente el Evento con código <b>" + hfCodigoEventoEliminar.Value + "</b>";

                hfConfirmEliminarEvento_ModalPopupExtender.Hide();
                hfMessage_ModalPopupExtender.Show();
                uPanel.Update();
            }
            else
            {
                hfConfirmEliminarEvento_ModalPopupExtender.Hide();
                showMessageError(status);
            }
        }


        protected void btMessageAceptar_Click(object sender, EventArgs e)
        {
            SDSEventos.DataBind();
            gvEventos.DataBind();
            uPanel.Update();
        }
    }
}