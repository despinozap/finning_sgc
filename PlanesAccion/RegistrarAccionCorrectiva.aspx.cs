using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;
using System.IO;

namespace NCCSAN.PlanesAccion
{
    public partial class RegistrarAccionCorrectiva : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Usuario u;
                {//Acceso del Usuario a la Página
                    if (Session["usuario"] == null)
                    {
                        Session["LinkedPage"] = Request.Url.ToString();

                        string msg = "Error al recuperar tu información de Usuario";
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }

                    u = (Usuario)Session["usuario"];
                    string pagename = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);
                    string prefix = ResolveUrl("~");
                    if (LogicController.isPageAllowed(u.getNombreRol(), pagename, prefix) < 1)
                    {
                        Response.Redirect("~/AccesoDenegado.aspx", true);
                    }
                }

                string codigo = null;
                if (Session["CodigoAccionCorrectivaSeleccionada"] != null)
                {
                    codigo = (string)Session["CodigoAccionCorrectivaSeleccionada"];
                }
                else if (Request.QueryString["iac"] != null)
                {
                    codigo = Request.QueryString["iac"];
                }
                else
                {
                    string msg = "No se ha pasado el parámetro";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }


                if (codigo != null)
                {
                    hfCodigoAccionCorrectiva.Value = codigo;

                    AccionCorrectiva accion_correctiva = LogicController.getAccionCorrectiva(codigo);
                    if (accion_correctiva != null)
                    {
                        if (accion_correctiva.getFechaRealizado() != null)
                        {
                            string msg = "La Acción Correctiva ya está registrada";
                            Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                        }
                        else if ((!u.getNombreRol().Equals("Jefe Calidad")) && (!u.getNombreRol().Equals("Coordinador")))
                        {
                            if (!u.getRutPersona().Equals(accion_correctiva.getResponsable().getRut()))
                            {
                                string msg = "Sólo puedes registrar una Acción Correctiva si tú eres el responsable";
                                Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                            }
                        }

                        Session["AccionCorrectiva"] = accion_correctiva;
                        loadAccionCorrectiva();

                        updateGVArchivos();
                    }
                    else
                    {
                        string msg = "Error al recuperar el Plan de Acción";
                        Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                    }
                }
                else
                {
                    string msg = "Error al recuperar la información del Plan de Acción";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }

        }


        private AccionCorrectiva getAccionCorrectiva()
        {
            if (Session["AccionCorrectiva"] != null)
            {
                AccionCorrectiva accion_correctiva = (AccionCorrectiva)Session["AccionCorrectiva"];

                return accion_correctiva;
            }
            else
            {
                return null;
            }
        }


        private void loadAccionCorrectiva()
        {
            AccionCorrectiva accion_correctiva = getAccionCorrectiva();
            if (accion_correctiva == null)
                return;


            lbDescripcion.Text = accion_correctiva.getDescripcion();
            lbFechaLimite.Text = accion_correctiva.getFechaLimite();
            lbNombreResponsable.Text = accion_correctiva.getResponsable().getNombre();
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

        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            hfConfirmVolver_ModalPopupExtender.Show();
        }

        protected void btConfirmVolverSi_Click(object sender, EventArgs e)
        {
            Session.Remove("AccionCorrectiva");
            Session.Remove("CodigoAccionCorrectivaSeleccionada");

            if (Session["PreviousPageAccionCorrectiva"] == null)
            {
                Response.Redirect("~/Default.aspx", true);
            }
            else
            {
                string previousPage = (string)Session["PreviousPageAccionCorrectiva"];

                Response.Redirect(previousPage, true);
            }

        }

        protected void btConfirmVolverNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        private void addFileToList()
        {
            int max_file_size;
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
                }
                catch (Exception ex)
                {
                    showMessageError("Se ha producido un error al cargar los archivos");
                    return;
                }

                setListaArchivos(listArchivos);

                updateGVArchivos();
            }
            else
            {
                showMessageWarning("No se ha seleccionado el archivo");
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
            gvArchivosAccionCorrectiva.DataSource = dt;
            gvArchivosAccionCorrectiva.DataBind();

            for (int i = 0; i < listArchivos.Count; i++)
            {
                ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivos[i].getNombre();
                ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivos[i].getTipoArchivo();
                ((Label)gvArchivosAccionCorrectiva.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivos[i].getSize()) + " bytes";
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


        protected void ibAddArchivo_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";
            addFileToList();
        }


        protected void gvArchivosEvaluacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = -1;

            if (false)
            {

            }
            else if (e.CommandName.Equals("RemoveArchivo"))
            {
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
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

                        showMessageInfo("Se ha quitado el archivo \"" + filename + "\" de la Acción Correctiva");
                        updateGVArchivos();
                    }
                }
                else // index = -1
                {
                    return;
                }
            }
        }


        private List<string> validatePanelAccionCorrectiva()
        {
            List<string> errors = new List<string>();

            if (!Utils.validateFecha(txtFechaRealizado.Text))
            {
                errors.Add("No se indicó la fecha en que se realizó la acción correctiva");
            }

            return errors;
        }

        private PlanAccion getPlanAccion()
        {
            if (Session["PlanAccion"] != null)
            {
                PlanAccion plan_accion = (PlanAccion)Session["PlanAccion"];

                return plan_accion;
            }
            else
            {
                return null;
            }
        }


        private string registrarAccionCorrectiva()
        {
            AccionCorrectiva accion_correctiva = getAccionCorrectiva();
            if (accion_correctiva == null)
            {
                return "Error al recuperar información de la Acción Correctiva";
            }


            PlanAccion plan_accion = getPlanAccion();
            if (plan_accion == null)
            {
                string codigo_planaccion = LogicController.getCodigoPlanAccionAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                if (codigo_planaccion == null)
                {
                    return "Error al recuperar información del Plan de Acción";
                }

                plan_accion = LogicController.getPlanAccion(codigo_planaccion);
                if (plan_accion == null)
                {
                    return "Error al recuperar información del Plan de Acción";
                }
            }


            if (Session["id_centro"] == null)
            {
                return "Error al recuperar el Centro de servicios";
            }

            string id_centro = (string)Session["id_centro"];

            List<Archivo> listArchivos = getListaArchivos();


            if (Session["usuario"] == null)
            {
                return "Error al recuperar información del Usuario";
            }

            Usuario usuario = (Usuario)Session["usuario"];
            PersonaInfo owner = LogicController.getPersonaInfo(usuario.getRutPersona());
            if (owner == null)
            {
                return "Error al recuperar tu información";
            }


            string registrado = LogicController.registerAccionCorrectivaRealizada
                                                                    (
                                                                        plan_accion.getCodigoEvento(),
                                                                        accion_correctiva.getIdAccionCorrectiva(),
                                                                        accion_correctiva.getFechaLimite(),
                                                                        txtFechaRealizado.Text,
                                                                        txtObservacion.Text,
                                                                        listArchivos,
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );
            if (registrado == null)
            {
                Session.Remove("listArchivos");

                return null;
            }
            else
            {
                return "Error al registrar la Acción Correctiva: " + registrado;
            }
        }


        protected void ibRegistrarAccionCorrectiva_Click(object sender, ImageClickEventArgs e)
        {
            ltSummary.Text = "";
            List<string> errors = validatePanelAccionCorrectiva();
            if (errors.Count == 0)
            {
                string status = registrarAccionCorrectiva();
                if (status == null)
                {
                    Session.Remove("listArchivos");

                    lbMessage.Text = "La acción correctiva se registró satisfactoriamente";

                    hfMessage_ModalPopupExtender.Show();
                    uPanel.Update();
                }
                else
                {
                    showMessageError(status);
                }
            }
            else
            {
                showSummary(errors);
            }
        }


        private void showSummary(List<string> errors)
        {
            showMessageError("Para continuar debes completar los campos requeridos en el formulario");
            string summary = "<b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            ltSummary.Text = summary;
        }

        protected void btMessageAceptar_Click(object sender, EventArgs e)
        {
            Session.Remove("AccionCorrectiva");
            Session.Remove("CodigoAccionCorrectivaSeleccionada");

            if (Session["PreviousPageAccionCorrectiva"] == null)
            {
                Response.Redirect("~/Default.aspx", true);
            }
            else
            {
                string previousPage = (string)Session["PreviousPageAccionCorrectiva"];
                Response.Redirect(previousPage, true);
            }
        }
    }
}