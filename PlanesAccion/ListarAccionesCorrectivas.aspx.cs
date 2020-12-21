using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Logic;
using NCCSAN.Source.Entity;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;

namespace NCCSAN.PlanesAccion
{
    public partial class ListarAccionesCorrectivas : System.Web.UI.Page
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
            }

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


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }


        private void setListIDArchivosAccionCorrectiva(List<string> listIDArchivosAccionCorrectiva)
        {
            Session["listIDArchivosAccionCorrectiva"] = listIDArchivosAccionCorrectiva;
        }


        private List<string> getListIDArchivosAccionCorrectiva()
        {
            if (Session["listIDArchivosAccionCorrectiva"] != null)
            {
                return (List<string>)Session["listIDArchivosAccionCorrectiva"];
            }
            else
            {
                return null;
            }
        }


        private bool loadDetalleAccionCorrectiva(AccionCorrectiva accion_correctiva)
        {
            if (accion_correctiva == null)
            {
                return false;
            }

            //Datos
            {
                lbDescripcionAccionCorrectiva.Text = accion_correctiva.getDescripcion();
                lbNombreResponsableAccionCorrectiva.Text = accion_correctiva.getResponsable().getNombre();
                lbFechaLimiteAccionCorrectiva.Text = accion_correctiva.getFechaLimite();
                lbFechaRealizadoAccionCorrectiva.Text = accion_correctiva.getFechaRealizado();
                lbObservacionAccionCorrectiva.Text = accion_correctiva.getObservacion();
            }


            //Archivos
            {
                List<Archivo> listArchivosAccionCorrectiva = LogicController.getArchivosAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                if (listArchivosAccionCorrectiva == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosAccionCorrectiva = new List<string>();

                foreach (Archivo archivo in listArchivosAccionCorrectiva)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosAccionCorrectiva.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosAccionCorrectiva.DataSource = dt;
                gvArchivosAccionCorrectiva.DataBind();

                setListIDArchivosAccionCorrectiva(listIDArchivosAccionCorrectiva);

                for (int i = 0; i < listArchivosAccionCorrectiva.Count; i++)
                {
                    ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosAccionCorrectiva[i].getNombre();
                    ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosAccionCorrectiva[i].getTipoArchivo();
                    ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosAccionCorrectiva[i].getSize()) + " bytes";
                }

            }

            return true;
        }


        protected void gvAccionesCorrectivas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DetalleAccionCorrectiva"))
            {
                string id_accion_correctiva = (string)e.CommandArgument;

                AccionCorrectiva accion_correctiva = LogicController.getAccionCorrectiva(id_accion_correctiva);
                if (accion_correctiva == null)
                {
                    showMessageError("Error al recuperar información de la Acción Correctiva");

                    return;
                }


                if (loadDetalleAccionCorrectiva(accion_correctiva))
                {
                    hfDetalleAccionCorrectiva_ModalPopupExtender.Show();
                }
                else
                {
                    showMessageError("Error al cargar información de la Acción Correctiva");
                }
            }
        }


        protected void gvAccionesCorrectivas_DataBound(object sender, EventArgs e)
        {
            Image imgEstadoIcono;
            string fecha_limite;
            string fecha_realizacion;
            string estado;
            ImageButton ibDetalleAccionCorrectiva;
            for (int i = 0; i < gvAccionesCorrectivas.Rows.Count; i++)
            {
                imgEstadoIcono = (Image)gvAccionesCorrectivas.Rows[i].Cells[0].FindControl("imgEstadoIcono");
                fecha_limite = HttpUtility.HtmlDecode(gvAccionesCorrectivas.Rows[i].Cells[3].Text);
                fecha_realizacion = HttpUtility.HtmlDecode(gvAccionesCorrectivas.Rows[i].Cells[4].Text);

                ibDetalleAccionCorrectiva = (ImageButton)gvAccionesCorrectivas.Rows[i].Cells[7].FindControl("ibDetalleAccionCorrectiva");

                if (Utils.validateFecha(fecha_realizacion))
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_green.png";
                    estado = "Realizada";
                }
                else if (Utils.validateFecha(fecha_limite))
                {
                    ibDetalleAccionCorrectiva.Visible = false;

                    DateTime dt = Convert.ToDateTime(fecha_limite);

                    if (Convert.ToDateTime(DateTime.Now.ToShortDateString()) == dt)
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                        estado = "Vence hoy";
                    }
                    else if (dt > DateTime.Now)
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";
                        estado = "Pendiente";
                    }
                    else
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                        estado = "Vencida";
                    }
                }
                else
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_grey.png";
                    estado = "Error de comprobación";
                }

                imgEstadoIcono.ToolTip = estado;
            }
        }


        protected void btDetalleAccionCorrectivaCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleAccionCorrectiva_ModalPopupExtender.Hide();
        }


        protected void gvArchivosAccionCorrectiva_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosAccionCorrectiva.Rows.Count))
                {
                    List<string> listIDArchivosAccionCorrectiva = getListIDArchivosAccionCorrectiva();
                    if (listIDArchivosAccionCorrectiva == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados a la Acción Correctiva");
                        return;
                    }

                    string id_archivo = listIDArchivosAccionCorrectiva[index];
                    Archivo archivo = LogicController.downloadFile(id_archivo);
                    if (archivo != null)
                    {
                        //Limpiamos la salida
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.Clear();

                        //Añadimos al encabezado la información de nuestro archivo
                        Response.AddHeader("Content-type", archivo.getTipoContenido());
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + archivo.getNombre());

                        //Finalmente escribimos los bytes en la respuesta de la página web
                        Response.BinaryWrite(archivo.getContenido());
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        showMessageError("Se ha producido un error al recuperar el archivo para descargar");
                    }
                }
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
            gvAccionesCorrectivas.GridLines = GridLines.Both;
            gvAccionesCorrectivas.HeaderStyle.Font.Bold = true;
            gvAccionesCorrectivas.EnableViewState = false;
            gvAccionesCorrectivas.AllowPaging = false;
            gvAccionesCorrectivas.DataBind();

            int column_count = gvAccionesCorrectivas.Columns.Count;
            gvAccionesCorrectivas.Columns[0].Visible = false;
            for (int i = 6; i < column_count; i++)
            {
                gvAccionesCorrectivas.Columns[i].Visible = false;
            }

            List<Control> controls = new List<Control>();
            DataTable dtFiltro = new DataTable();
            dtFiltro.Columns.Add("Filtros");
            dtFiltro.Columns.Add("Valor");
            DataRow dr;

            dr = dtFiltro.NewRow();
            dr[0] = "RUT responsable";
            dr[1] = txtBuscarRUTResponsable.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Nombre responsable";
            dr[1] = txtBuscarNombreResponsable.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Año";
            dr[1] = ddlAnio.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Mes";
            dr[1] = ddlMes.SelectedItem.Text;
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

            controls.Add(gvAccionesCorrectivas);
            ExportToExcel("Eventos", controls);

            gvAccionesCorrectivas.AllowPaging = true;
            gvAccionesCorrectivas.DataBind();

            gvAccionesCorrectivas.Columns[0].Visible = true;
            for (int i = 6; i < column_count; i++)
            {
                gvAccionesCorrectivas.Columns[i].Visible = true;
            }

        }

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("[Todas]", ""));
        }
    }
}