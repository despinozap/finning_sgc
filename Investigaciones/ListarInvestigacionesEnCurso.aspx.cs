using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;

namespace NCCSAN.Investigaciones
{
    public partial class ListarInvestigacionesEnCurso : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                {//Acceso del Usuario a la Página
                    if (Session["usuario"] == null)
                    {
                        Session["LinkedPage"] = Request.Url.ToString();

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

                Session.Remove("CodigoEventoSeleccionado");

                {//Filtrar búsqueda por parámetro
                    if (Request.QueryString["ce"] != null)
                    {
                        string codigo_evento = Request.QueryString["ce"];
                        txtCodigoWO.Text = codigo_evento;

                        SDSInvestigacionesEnCurso.DataBind();
                        gvEventos.DataBind();
                    }
                }
            }
        }


        private void setIndexRow(int index)
        {
            Session["IndexRow"] = index;
        }


        private int getIndexRow()
        {
            if (Session["IndexRow"] == null)
                return -1;
            else
                return (int)Session["IndexRow"];
        }

        protected void ddlCliente_DataBound(object sender, EventArgs e)
        {
            ddlCliente.Items.Insert(0, new ListItem("[Todos]", ""));
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


        protected void gvEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            setIndexRow(-1);

            if (e.CommandName.Equals("CerrarInvestigacion"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {

                    if (Session["usuario"] == null)
                    {
                        return;
                    }

                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();

                    if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                    {
                        showMessageError("No tienes privilegios para cerrar Investigaciones");
                        return;
                    }


                    setIndexRow(index);
                    txtFechaInvestigacionRealizada.Text = "";
                    clearListaArchivos();

                    hfCalendar_ModalPopupExtender.Show();
                }
            }
            else if (e.CommandName.Equals("EliminarInicioInvestigacion"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvEventos.Rows.Count))
                {

                    if (Session["usuario"] == null)
                    {
                        return;
                    }

                    Usuario usuario = (Usuario)Session["usuario"];
                    string rol = usuario.getNombreRol();

                    if ((!rol.Equals("Jefe Calidad")) && (!rol.Equals("Coordinador")))
                    {
                        showMessageError("No tienes privilegios para eliminar Investigaciones");
                        return;
                    }

                    setIndexRow(index);
                    lbMessageEliminarInicioInvestigacion.Text = "¿Realmente deseas eliminar el inicio de la Investigación para el evento con código " + gvEventos.Rows[index].Cells[0].Text + "?";
                    hfEliminarInicioInvestigacion_ModalPopupExtender.Show();
                }
            }
        }


        private string eliminarInicioInvestigacion(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return "El evento seleccionado es inválido";
            }

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

            string registrado = LogicController.removeInvestigacion
                                                                    (
                                                                        codigo_evento,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );
            if (registrado == null)
            {
                return null;
            }
            else
            {
                return registrado;
            }
        }


        private string registrarInvestigacionRealizada(string codigo_evento, string fecha_cierre)
        {
            if (codigo_evento == null)
            {
                return "El evento seleccionado es inválido";
            }

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

            List<Archivo> listArchivos = getListaArchivos();

            string registrado = LogicController.registerInvestigacionRealizada
                                                                    (
                                                                        codigo_evento,
                                                                        fecha_cierre,
                                                                        listArchivos,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );
            if (registrado == null)
            {
                return null;
            }
            else
            {
                return registrado;
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
            dr[0] = "Cliente";
            dr[1] = ddlCliente.SelectedItem.Text;
            dtFiltro.Rows.Add(dr);

            dr = dtFiltro.NewRow();
            dr[0] = "Área";
            dr[1] = ddlArea.SelectedItem.Text;
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

            controls.Add(gvEventos);
            ExportToExcel("InvestigacionesEnCurso", controls);

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


        private void showMessageSuccess(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxSuccess", "<script type=\"text/javascript\">showMessageSuccess('" + message + "');</script>", false);
        }


        private void showMessageError(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxError", "<script type=\"text/javascript\">showMessageError('" + message + "');</script>", false);
        }


        private void showMessageInfo(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxInfo", "<script type=\"text/javascript\">showMessageInfo('" + message + "');</script>", false);
        }


        private void showMessageWarning(string message)
        {
            if (message != null)
                AjaxControlToolkit.ToolkitScriptManager.RegisterStartupScript(this, GetType(), "messageBoxWarning", "<script type=\"text/javascript\">showMessageWarning('" + message + "');</script>", false);
        }


        protected void gvEventos_DataBound(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                string msg = "Error al recuperar tu información de Usuario";
                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
            }

            Usuario usuario = (Usuario)Session["usuario"];


            if (gvEventos.Rows.Count > 0)
            {
                pnOpcionesLista.Visible = true;
            }
            else
            {
                pnOpcionesLista.Visible = false;
            }


            ConfigEmailSender ces = LogicController.getConfigEmailSender("Investigación en curso", usuario.getNombreRol(), usuario.getIDCentro());
            if (ces == null)
            {
                return;
            }


            Image imgEstadoIcono;
            string dias_en_cursoS;
            int dias_en_curso;
            for (int i = 0; i < gvEventos.Rows.Count; i++)
            {
                imgEstadoIcono = (Image)gvEventos.Rows[i].Cells[0].FindControl("imgEstadoIcono");
                dias_en_cursoS = HttpUtility.HtmlDecode(gvEventos.Rows[i].Cells[8].Text);

                if (Utils.validateNumber(dias_en_cursoS))
                {
                    dias_en_curso = Convert.ToInt32(dias_en_cursoS);

                    if (dias_en_curso > ces.getDiasLimite())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_alert.gif";
                        imgEstadoIcono.ToolTip = "Investigación vencida";
                    }
                    else if (dias_en_curso == ces.getDiasLimite())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                        imgEstadoIcono.ToolTip = "La Investigación vence hoy";
                    }
                    else if (dias_en_curso >= ces.getDiasAlerta())
                    {
                        imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";

                        if ((ces.getDiasLimite() - dias_en_curso) == 1)
                        {
                            imgEstadoIcono.ToolTip = "La Investigación vence en 1 día";
                        }
                        else
                        {
                            imgEstadoIcono.ToolTip = "La Investigación vence en " + Convert.ToString(ces.getDiasLimite() - dias_en_curso) + " días";
                        }
                    }
                }
            }
        }


        private List<Archivo> getListaArchivos()
        {
            List<Archivo> listArchivos;

            if (Session["listArchivos"] != null)
                listArchivos = (List<Archivo>)Session["listArchivos"];
            else
            {
                listArchivos = new List<Archivo>();
                setListaArchivos(listArchivos);
            }

            return listArchivos;
        }


        private void setListaArchivos(List<Archivo> listArchivo)
        {
            Session["listArchivos"] = listArchivo;
        }

        private void clearListaArchivos()
        {
            Session.Remove("listArchivos");
            updateGVArchivos();
        }


        protected void gvArchivosEvaluacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = -1;

            if (e.CommandName.Equals("RemoveArchivo"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    hfCalendar_ModalPopupExtender.Show();

                    return;
                }

                if (index >= 0)
                {
                    List<Archivo> listArchivos = getListaArchivos();
                    if (index < listArchivos.Count)
                    {
                        string filename = listArchivos[index].getNombre();
                        listArchivos.RemoveAt(index);
                        setListaArchivos(listArchivos);

                        showMessageInfo("Se ha quitado el archivo \"" + filename + "\" de la Evaluación");
                        updateGVArchivos();
                    }

                    hfCalendar_ModalPopupExtender.Show();
                }
                else // index = -1
                {
                    hfCalendar_ModalPopupExtender.Show();

                    return;
                }
            }
        }



        private void addFileToList()
        {

            if (fuArchivo.HasFile)
            {
                List<Archivo> listArchivos = getListaArchivos();
                try
                {
                    byte[] contentFile;
                    BinaryReader binaryReader;
                    HttpFileCollection hfc = Request.Files;
                    HttpPostedFile hpf;
                    string fileType;
                    string contentType;
                    string fileName;
                    string valid_filename;
                    string ext;
                    int max_file_size;
                    for (int i = 0; i < hfc.Count; i++)
                    {
                        hpf = hfc[i];

                        fileName = Path.GetFileName(hpf.FileName);
                        valid_filename = Utils.validateFilename(fileName);
                        if (valid_filename != null)
                        {
                            showMessageError(valid_filename);
                            return;
                        }

                        ext = Utils.getFileExtension(fileName);
                        contentType = Utils.getContentType(ext);
                        if (contentType == null)
                        {
                            showMessageError("Extensión de archivo \"" + ext + "\" no permitida");
                            return;
                        }

                        max_file_size = LogicController.getMaxFileSizeByExtension(ext);
                        if (max_file_size < 0)
                        {
                            showMessageError("No se puede comprobar el tamaño máximo permitido para el tipo de archivo");

                            return;
                        }


                        if ((hpf.ContentLength) > max_file_size)
                        {
                            showMessageError("El tipo de archivo no permite un tamaño superior a " + Convert.ToString(max_file_size / 1000000) + " Mb");

                            return;
                        }

                        fileType = Utils.getFileType(ext);

                        contentFile = new byte[hpf.ContentLength];
                        binaryReader = new BinaryReader(hpf.InputStream);
                        binaryReader.Read(contentFile, 0, hpf.ContentLength);
                        Archivo archivo = new Archivo(Utils.getUniqueCode(), fileName, contentFile, fileType, contentType);
                        listArchivos.Add(archivo);
                    }

                    setListaArchivos(listArchivos);
                    updateGVArchivos();
                    hfCalendar_ModalPopupExtender.Show();
                }
                catch (Exception ex)
                {
                    showMessageError("Se ha producido un error al cargar los archivos");
                    hfCalendar_ModalPopupExtender.Show();
                    return;
                }

            }
            else
            {
                showMessageWarning("No se ha seleccionado el archivo");

                hfCalendar_ModalPopupExtender.Show();
            }
        }



        private void updateGVArchivos()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Tamaño");

            DataRow dr;

            List<Archivo> listArchivos = getListaArchivos();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();

                dt.Rows.Add(dr);

            }

            dt.AcceptChanges();
            gvArchivosEvaluacion.DataSource = dt;
            gvArchivosEvaluacion.DataBind();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                ((Label)gvArchivosEvaluacion.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivos[i].getNombre();
                ((Label)gvArchivosEvaluacion.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivos[i].getTipoArchivo();
                ((Label)gvArchivosEvaluacion.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivos[i].getSize()) + " bytes";
            }
        }


        protected void ibAddArchivo_Click(object sender, ImageClickEventArgs e)
        {
            addFileToList();
        }


        protected void btRegistrarInvestigacionRealizada_Click(object sender, EventArgs e)
        {
            int index = getIndexRow();

            if (index < 0)
            {
                showMessageError("El Evento seleccionado es inválido");
                clearListaArchivos();
                return;
            }

            string fecha_inicio = txtFechaInvestigacionRealizada.Text;
            if (!Utils.validateFecha(fecha_inicio))
            {
                showMessageError("La fecha indicada como cierre de la Investigación es inválida");
                clearListaArchivos();
                return;
            }

            string codigo_evento = gvEventos.Rows[index].Cells[1].Text;
            string status = registrarInvestigacionRealizada(codigo_evento, fecha_inicio);
            if (status == null)
            {
                showMessageSuccess("Se ha cerrado la Investigación con código " + codigo_evento + " a la fecha " + fecha_inicio);
                gvEventos.DataBind();
            }
            else
            {
                showMessageError(status);
            }

            setIndexRow(-1);
            hfCalendar_ModalPopupExtender.Hide();
            clearListaArchivos();
        }


        protected void btCancelarInvestigacionRealizada_Click(object sender, EventArgs e)
        {
            setIndexRow(-1);
            hfCalendar_ModalPopupExtender.Hide();
            clearListaArchivos();
        }


        protected void btEliminarInicioInvestigacion_Click(object sender, EventArgs e)
        {
            int index = getIndexRow();

            if (index < 0)
            {
                showMessageError("El Evento seleccionado es inválido");
                return;
            }

            string codigo_evento = gvEventos.Rows[index].Cells[1].Text;
            string status = eliminarInicioInvestigacion(codigo_evento);
            if (status == null)
            {
                showMessageSuccess("Se ha eliminado el inicio de la Investigación con código " + codigo_evento);
                gvEventos.DataBind();
            }
            else
            {
                showMessageError(status);
            }

            setIndexRow(-1);
            hfEliminarInicioInvestigacion_ModalPopupExtender.Hide();
        }


        protected void btCancelarEliminarInicioInvestigacion_Click(object sender, EventArgs e)
        {
            setIndexRow(-1);
            hfEliminarInicioInvestigacion_ModalPopupExtender.Hide();
        }

        protected void ddlArea_DataBound(object sender, EventArgs e)
        {
            ddlArea.Items.Insert(0, new ListItem("[Todas]", ""));
        }

    }
}