using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;

namespace NCCSAN.Administracion
{
    public partial class MatrizConsecuencia : System.Web.UI.Page
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
            }
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


        private Archivo getFileUploaderFile()
        {
            if (fuArchivo.HasFile)
            {
                string ext = Utils.getFileExtension(fuArchivo.FileName);
                String contentType = Utils.getContentType(ext);
                if (contentType == null)
                {
                    showMessageError("Extensión de archivo \"" + ext + "\" no permitida");
                    return null;
                }


                int max_file_size = LogicController.getMaxFileSizeByExtension(ext);
                if (max_file_size < 0)
                {
                    showMessageError("No se puede comprobar el tamaño máximo permitido para el tipo de archivo");

                    return null;
                }

                if ((fuArchivo.FileBytes.Length) > max_file_size)
                {
                    showMessageError("El tipo de archivo no permite un tamaño superior a " + Convert.ToString(max_file_size / 1000000) + " Mb");

                    return null;
                }

                String fileType = Utils.getFileType(ext);

                Archivo archivo = new Archivo(Utils.getUniqueCode(), fuArchivo.FileName, fuArchivo.FileBytes, fileType, contentType);

                return archivo;
            }
            else
            {
                showMessageWarning("No se ha seleccionado el archivo");

                return null;
            }
        }



        protected void ibActualizarMatrizConsecuencia_Click(object sender, ImageClickEventArgs e)
        {

            if (Session["usuario"] == null)
            {
                showMessageError("Error al recuperar la información del Usuario");

                return;

            }


            if (Session["id_centro"] == null)
            {
                showMessageError("Error al recuperar la información del Centro");

                return;
            }

            string id_centro = (string)Session["id_centro"];

            Archivo archivo = getFileUploaderFile();
            if (archivo == null)
            {
                showMessageError("Error al cargar el archivo");

                return;
            }


            Usuario usuario = (Usuario)Session["usuario"];
            PersonaInfo owner = LogicController.getPersonaInfo(usuario.getRutPersona());
            if (owner == null)
            {
                showMessageError("Error al recuperar tú información");

                return;
            }


            string status = LogicController.updateMatrizSeveridad(archivo, id_centro, usuario.getUsuario(), Request.ServerVariables["REMOTE_ADDR"], owner);
            if (status == null)
            {
                showMessageSuccess("Se ha actualizado exitosamente la Matriz de Consecuencia para el Centro");
            }
            else
            {
                showMessageError(status);
            }

        }
    }
}