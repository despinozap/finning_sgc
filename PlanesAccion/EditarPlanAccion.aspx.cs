using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCCSAN.Source.Entity;
using NCCSAN.Source.Logic;
using System.Data;

namespace NCCSAN.PlanesAccion
{
    public partial class EditarPlanAccion : System.Web.UI.Page
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

                setIndexRow(-1);
                Session.Remove("listAccionesCorrectivas");
                Session.Remove("listRemovedAccionesCorrectivas");
                Session.Remove("PlanAccion");

                if ((Session["CodigoPlanAccionSeleccionado"] != null) && (Session["PreviousPagePlanAccion"] != null))
                {
                    string codigo = (string)Session["CodigoPlanAccionSeleccionado"];
                    string previuosPage = (string)Session["PreviousPagePlanAccion"];

                    if ((codigo != null) && (previuosPage != null))
                    {
                        hfCodigoEvento.Value = codigo;
                        hfPreviousPage.Value = previuosPage;

                        PlanAccion plan_acccion = LogicController.getPlanAccion(codigo);
                        if (plan_acccion != null)
                        {
                            Session["PlanAccion"] = plan_acccion;
                            loadPlanAccion();
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
                else
                {
                    string msg = "No se ha pasado el parámetro";
                    Response.Redirect("~/Error.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
                }
            }

        }


        private bool loadDetalleEvento(string codigo_evento)
        {
            try
            {
                Evento evento = LogicController.getEvento(codigo_evento);
                if (evento == null)
                {
                    return false;
                }

                PersonaInfo creador = LogicController.getCreadorEvento(codigo_evento);
                if (creador == null)
                {
                    return false;
                }

                lbDetalleEventoCodigo.Text = evento.getCodigo();
                lbDetalleEventoWO.Text = evento.getWO();

                lbDetalleEventoNombreCliente.Text = evento.getNombreCliente();
                lbDetalleEventoNombreFuente.Text = evento.getNombreFuente();
                lbDetalleEventoFecha.Text = evento.getFecha().ToShortDateString();
                lbDetalleEventoTipoEquipo.Text = evento.getTipoEquipo();
                lbDetalleEventoModeloEquipo.Text = evento.getModeloEquipo();

                if (evento.getSerieEquipo() != null)
                {
                    lbDetalleEventoSerieEquipo.Text = evento.getSerieEquipo();
                }

                lbDetalleEventoNombreSistema.Text = evento.getNombreSistema();
                lbDetalleEventoNombreSubsistema.Text = evento.getNombreSubsistema();
                lbDetalleEventoNombreComponente.Text = evento.getNombreComponente();

                if (evento.getSerieComponente() != null)
                {
                    lbDetalleEventoSerieComponente.Text = evento.getSerieComponente();
                }

                if (evento.getParte() != null)
                {
                    lbDetalleEventoParte.Text = evento.getParte();
                }

                if (evento.getNumeroParte() != null)
                {
                    lbDetalleEventoNumeroParte.Text = evento.getNumeroParte();
                }

                if (evento.getHoras() >= 0)
                {
                    lbDetalleEventoHoras.Text = Convert.ToString(evento.getHoras());
                }
                else
                {
                    lbDetalleEventoHoras.Text = "--";
                }

                lbDetalleEventoNombreArea.Text = evento.getNombreArea();
                lbDetalleEventoNombreSubarea.Text = evento.getNombreSubarea();
                lbDetalleEventoNombreClasificacion.Text = evento.getNombreClasificacion();
                lbDetalleEventoNombreSubclasificacion.Text = evento.getNombreSubclasificacion();
                lbDetalleEventoCriticidad.Text = evento.getCriticidad();
                lbDetalleEventoProbabilidad.Text = evento.getProbabilidad();
                lbDetalleEventoConsecuencia.Text = evento.getConsecuencia();
                lbDetalleEventoIRC.Text = Convert.ToString(evento.getIRC());
                lbDetalleEventoDetalle.Text = evento.getDetalle();
                lbtDetalleEventoDetalleCreador.CommandName = "DetallePersona";

                lbtDetalleEventoDetalleCreador.CommandArgument = creador.getRut();
                lbtDetalleEventoDetalleCreador.Text = creador.getNombre();

                if (loadArchivosEvento(codigo_evento))
                {
                    return true;
                }
                else
                {
                    showMessageError("Error al cargar los archivos asociados al Evento");

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private bool loadArchivosEvento(string codigo_evento)
        {
            try
            {
                List<Archivo> listArchivosEvento = LogicController.getArchivosEvento(codigo_evento);
                if (listArchivosEvento == null)
                {
                    return false;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Tamaño");
                dt.Columns.Add("Descargar");

                DataRow dr;
                List<string> listIDArchivosEvento = new List<string>();

                foreach (Archivo archivo in listArchivosEvento)
                {
                    dr = dt.NewRow();

                    dr[1] = new Label();
                    dr[2] = new Label();
                    dr[3] = new Button();

                    dt.Rows.Add(dr);

                    listIDArchivosEvento.Add(archivo.getIdArchivo());
                }

                dt.AcceptChanges();
                gvArchivosEvento.DataSource = dt;
                gvArchivosEvento.DataBind();

                setListIDArchivosEvento(listIDArchivosEvento);

                for (int i = 0; i < listArchivosEvento.Count; i++)
                {
                    ((Label)gvArchivosEvento.Rows[i].Cells[0].FindControl("lbNombre")).Text = listArchivosEvento[i].getNombre();
                    ((Label)gvArchivosEvento.Rows[i].Cells[0].FindControl("lbTipo")).Text = listArchivosEvento[i].getTipoArchivo();
                    ((Label)gvArchivosEvento.Rows[i].Cells[0].FindControl("lbSize")).Text = Convert.ToString(listArchivosEvento[i].getSize()) + " bytes";

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        private void setListIDArchivosEvento(List<string> listIDArchivosEvento)
        {
            Session["listIDArchivosEvento"] = listIDArchivosEvento;
        }


        private List<string> getListIDArchivosEvento()
        {
            if (Session["listIDArchivosEvento"] != null)
            {
                return (List<string>)Session["listIDArchivosEvento"];
            }
            else
            {
                return null;
            }
        }


        protected void lbtDetalleEventoDetalleCreador_Click(object sender, EventArgs e)
        {
            if (lbtDetalleEventoDetalleCreador.CommandName.Equals("DetallePersona"))
            {
                if (lbtDetalleEventoDetalleCreador.CommandArgument != null)
                {
                    string rut = Convert.ToString(lbtDetalleEventoDetalleCreador.CommandArgument);
                    Session["RutDetalle"] = rut;
                    Session["PreviousPagePersona"] = ResolveClientUrl(HttpContext.Current.Request.Url.LocalPath);

                    Response.Redirect("~/Personas/DetallePersona.aspx", true);
                }
            }
            uPanel.Update();
        }


        protected void btDetalleEventoCerrar_Click(object sender, EventArgs e)
        {
            hfDetalleEvento_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        protected void gvArchivosEvento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DescargarArchivo"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if ((index >= 0) && (index < gvArchivosEvento.Rows.Count))
                {
                    List<string> listIDArchivosEvento = getListIDArchivosEvento();
                    if (listIDArchivosEvento == null)
                    {
                        showMessageError("Error al recuperar la lista de archivos asociados al Evento");
                        return;
                    }

                    string id_archivo = listIDArchivosEvento[index];
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

        protected void ibVerDetalleEvento_Click(object sender, ImageClickEventArgs e)
        {
            if (loadDetalleEvento(hfCodigoEvento.Value))
            {
                hfDetalleEvento_ModalPopupExtender.Show();
            }
            else
            {
                showMessageError("Error al cargar los detalles del Evento");
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


        private void loadPlanAccion()
        {
            PlanAccion plan_accion = getPlanAccion();
            if (plan_accion == null)
                return;

            txtDetalleCorreccion.Text = plan_accion.getDetalleCorreccion();
            txtFechaCorreccion.Text = plan_accion.getFechaCorreccion().ToShortDateString();

            List<AccionCorrectiva> listAccionesCorrectivas = plan_accion.getListAccionesCorrectivas();
            setListAccionesCorrectivas(listAccionesCorrectivas);
            updateGVAccionesCorrectivas();

            List<string> listRemovedAccionesCorrectivas = new List<string>();
            setListRemovedAccionesCorrectivas(listRemovedAccionesCorrectivas);
        }


        private void setListAccionesCorrectivas(List<AccionCorrectiva> listAccionesCorrectivas)
        {
            Session["listAccionesCorrectivas"] = listAccionesCorrectivas;
        }


        private List<AccionCorrectiva> getListAccionesCorrectivas()
        {
            List<AccionCorrectiva> listAccionesCorrectivas;

            if (Session["listAccionesCorrectivas"] == null)
                listAccionesCorrectivas = new List<AccionCorrectiva>();
            else
                listAccionesCorrectivas = (List<AccionCorrectiva>)Session["listAccionesCorrectivas"];

            return listAccionesCorrectivas;
        }


        private void setListRemovedAccionesCorrectivas(List<string> listRemovedAccionesCorrectivas)
        {
            Session["listRemovedAccionesCorrectivas"] = listRemovedAccionesCorrectivas;
        }

        private void addToListRemovedAccionesCorrectivas(string id_accion_correctiva)
        {
            List<string> listRemovedAccionesCorrectivas = getListRemovedAccionesCorrectivas();
            listRemovedAccionesCorrectivas.Add(id_accion_correctiva);
            setListRemovedAccionesCorrectivas(listRemovedAccionesCorrectivas);
        }


        private List<string> getListRemovedAccionesCorrectivas()
        {
            List<string> listRemovedAccionesCorrectivas;

            if (Session["listAccionesCorrectivas"] == null)
                listRemovedAccionesCorrectivas = new List<string>();
            else
                listRemovedAccionesCorrectivas = (List<string>)Session["listRemovedAccionesCorrectivas"];

            return listRemovedAccionesCorrectivas;
        }


        private void setShowHeaderOnEmpty(GridView gv)
        {
            DataTable dt = getDTAccionesCorrectivasFormat();
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            gv.DataSource = dt;
            gv.DataBind();
        }


        private DataTable getDTAccionesCorrectivasFormat()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("");
            dt.Columns.Add("Descripción");
            dt.Columns.Add("Fecha límite");
            dt.Columns.Add("Responsable");
            dt.Columns.Add("Opciones");

            dt.AcceptChanges();

            return dt;
        }


        private void updateGVAccionesCorrectivas()
        {
            DataTable dt = getDTAccionesCorrectivasFormat();
            List<AccionCorrectiva> listAccionesCorrectivas = getListAccionesCorrectivas();

            DataRow dr;

            for (int i = 0; i < listAccionesCorrectivas.Count; i++)
            {
                dr = dt.NewRow();

                dr[0] = new Label();
                dr[1] = new Label();
                dr[2] = new Label();
                dr[3] = new Label();
                dr[4] = new Button();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
            gvAccionesCorrectivas.DataSource = dt;
            gvAccionesCorrectivas.DataBind();

            Image imgEstadoIcono;
            for (int i = 0; i < listAccionesCorrectivas.Count; i++)
            {

                imgEstadoIcono = (Image)gvAccionesCorrectivas.Rows[i].FindControl("imgEstadoIcono");

                if (listAccionesCorrectivas[i].getFechaRealizado() != null)
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_green.png";
                    gvAccionesCorrectivas.Rows[i].Cells[5].Controls[0].Visible = false;
                }
                else if (Convert.ToDateTime(listAccionesCorrectivas[i].getFechaLimite()) < DateTime.Now)
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_red.png";
                }
                else
                {
                    imgEstadoIcono.ImageUrl = "~/Images/Icon/ball_yellow.png";
                }

                ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbDescripcion")).Text = listAccionesCorrectivas[i].getDescripcion();
                ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbFechaLimite")).Text = listAccionesCorrectivas[i].getFechaLimite();
                if (listAccionesCorrectivas[i].getFechaRealizado() != null)
                {
                    ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbFechaRealizado")).Text = listAccionesCorrectivas[i].getFechaRealizado();
                }
                ((Label)gvAccionesCorrectivas.Rows[i].FindControl("lbNombreResponsable")).Text = listAccionesCorrectivas[i].getResponsable().getNombre();
            }
        }


        protected void ibVolver_Click(object sender, ImageClickEventArgs e)
        {
            hfConfirmVolver_ModalPopupExtender.Show();
        }


        protected void btMessageAceptar_Click(object sender, EventArgs e)
        {
            Session.Remove("PlanAccion");
            Session.Remove("listInvolucrados");
            Session.Remove("listAccionesCorrectivas");
            Session.Remove("listRemovedAccionesCorrectivas");

            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
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


        private List<string> validatePanelPlanAccion()
        {
            List<string> errors = new List<string>();

            if (txtDetalleCorreccion.Text.Length < 1)
            {
                errors.Add("No ha indicado el detalle de la corrección inmediata");
            }

            if (txtFechaCorreccion.Text.Length < 1)
            {
                errors.Add("No ha indicado la fecha de corrección inmediata");
            }

            List<AccionCorrectiva> listAccionesCorrectivas = getListAccionesCorrectivas();
            if (listAccionesCorrectivas.Count == 0)
            {
                errors.Add("El Plan de Acción debe tener al menos una Acción Correctiva");
            }

            return errors;
        }


        private string updatePlanAccion()
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


            string registrado = LogicController.updatePlanAccion
                                                                    (
                                                                        hfCodigoEvento.Value,
                                                                        txtDetalleCorreccion.Text,
                                                                        txtFechaCorreccion.Text,
                                                                        getListAccionesCorrectivas(),
                                                                        getListRemovedAccionesCorrectivas(),
                                                                        usuario.getUsuario(),
                                                                        Request.ServerVariables["REMOTE_ADDR"]
                                                                    );
            if (registrado == null)
            {
                return null;
            }
            else
            {
                return "Error al registrar el Plan de Acción: " + registrado;
            }
        }


        protected void ibRegistrarPlanAccion_Click(object sender, ImageClickEventArgs e)
        {
            PlanAccion plan_accion = getPlanAccion();

            if (plan_accion == null)
            {
                showMessageError("El Plan de Acción que se está intentando editar no existe");

                return;
            }

            ltSummary.Text = "";
            List<string> errors = validatePanelPlanAccion();
            if (errors.Count == 0)
            {
                hfConfirmEdicion_ModalPopupExtender.Show();
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


        private void showSummaryAccionCorrectiva(List<string> errors)
        {
            string summary = "<b>Se han encontrado los siguientes errores:</b><br />";
            foreach (string error in errors)
            {
                summary += "* " + error + "<br />";
            }

            ltSummaryAccionCorrectiva.Text = summary;

            hfAgregarAccionCorrectiva_ModalPopupExtender.Show();
            uPanel.Update();
        }


        protected void gvAccionesCorrectivas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            setIndexRow(-1);

            if (e.CommandName.Equals("AddAccionCorrectiva"))
            {
                ((ButtonField)gvBuscarPersonas.Columns[0]).CommandName = "SetResponsableAccionCorrectiva";

                btBuscarPersonaCancelar.Text = "Cancelar";

                txtDescripcion.Text = "";
                txtFechaLimiteAccionCorrectiva.Text = "";
                hfAgregarAccionCorrectiva_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("DelAccionCorrectiva"))
            {

                int index = -1;
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index < 0)
                    return;

                setIndexRow(index);
                hfConfirmRemoveAccionCorrectiva_ModalPopupExtender.Show();
            }
            else if (e.CommandName.Equals("EditAccionCorrectiva"))
            {

                int index = -1;
                try
                {
                    index = Convert.ToInt32(e.CommandArgument);
                }
                catch (Exception ex)
                {
                    return;
                }

                if (index < 0)
                    return;

                List<AccionCorrectiva> listAccionesCorrectivas = getListAccionesCorrectivas();
                if (index < listAccionesCorrectivas.Count)
                {

                    if (listAccionesCorrectivas[index].getFechaRealizado() == null)
                    {
                        setIndexRow(index);
                        AccionCorrectiva accion_correctiva = listAccionesCorrectivas[index];

                        txtDescripcion.Text = accion_correctiva.getDescripcion();
                        txtFechaLimiteAccionCorrectiva.Text = accion_correctiva.getFechaLimite();
                        hfRutResponsableAccionCorrectiva.Value = accion_correctiva.getResponsable().getRut();
                        txtNombreResponsableAccionCorrectiva.Text = accion_correctiva.getResponsable().getNombre();

                        hfAgregarAccionCorrectiva_ModalPopupExtender.Show();
                    }
                    else
                    {
                        showMessageError("No se puede editar una Acción Correctiva que ya se ejecutó");
                    }
                }
            }
        }


        protected void btBuscarResponsableAccionCorrectiva_Click(object sender, EventArgs e)
        {
            hfAgregarAccionCorrectiva_ModalPopupExtender.Hide();

            ltBuscarPersonaSummary.Text = "";
            hfBuscarPersona_ModalPopupExtender.Show();
            uPanel.Update();
        }


        protected void btRegistrarAccionCorrectiva_Click(object sender, EventArgs e)
        {
            ltSummary.Text = "";
            List<string> errors = validatePanelAccionCorrectiva();
            if (errors.Count == 0)
            {
                List<string> msgs = new List<string>();

                if (!Utils.validateFecha(txtFechaLimiteAccionCorrectiva.Text))
                {
                    msgs.Add("Error al validar la fecha límite de la Acción Correctiva");
                }

                DateTime fecha_limite_accion_correctiva = Convert.ToDateTime(txtFechaLimiteAccionCorrectiva.Text);

                PersonaInfo responsable = LogicController.getPersonaInfo(hfRutResponsableAccionCorrectiva.Value);
                if (responsable == null)
                {
                    msgs.Add("Error al recuperar información del Responsable");
                }

                if (msgs.Count == 0)
                {
                    int index = getIndexRow();

                    AccionCorrectiva accion_correctiva;
                    List<AccionCorrectiva> listAccionesCorrectivas = getListAccionesCorrectivas();
                    if (index < 0)
                    {
                        accion_correctiva = new AccionCorrectiva(null, txtDescripcion.Text, fecha_limite_accion_correctiva.ToShortDateString(), null, responsable, null);
                        listAccionesCorrectivas.Add(accion_correctiva);
                    }
                    else
                    {
                        accion_correctiva = listAccionesCorrectivas[index];

                        accion_correctiva.setDescripcion(txtDescripcion.Text);
                        accion_correctiva.setFechaLimite(fecha_limite_accion_correctiva.ToShortDateString());
                        accion_correctiva.setResponsable(responsable);

                        listAccionesCorrectivas[index] = accion_correctiva;
                    }

                    setListAccionesCorrectivas(listAccionesCorrectivas);

                    updateGVAccionesCorrectivas();

                    txtDescripcion.Text = "";
                    txtFechaLimiteAccionCorrectiva.Text = "";
                    hfRutResponsableAccionCorrectiva.Value = "";
                    txtNombreResponsableAccionCorrectiva.Text = "";
                    ltSummaryAccionCorrectiva.Text = "";

                    hfAgregarAccionCorrectiva_ModalPopupExtender.Hide();

                    uPanel.Update();
                }
                else
                {
                    showSummaryAccionCorrectiva(msgs);
                }


            }
            else
            {
                showSummaryAccionCorrectiva(errors);
            }
        }


        protected void btCancelarRegistrarAccionCorrectiva_Click(object sender, EventArgs e)
        {
            txtNombreResponsableAccionCorrectiva.Text = "";
            hfRutResponsableAccionCorrectiva.Value = "";

            hfAgregarAccionCorrectiva_ModalPopupExtender.Hide();
            uPanel.Update();
        }


        private List<string> validatePanelAccionCorrectiva()
        {
            List<string> errors = new List<string>();

            if (txtDescripcion.Text.Length < 1)
            {
                errors.Add("No ha indicado el detalle de la Acción Correctiva");
            }

            if (txtFechaLimiteAccionCorrectiva.Text.Length < 1)
            {
                errors.Add("No ha indicado la fecha límite de la Acción Correctiva");
            }

            if (txtNombreResponsableAccionCorrectiva.Text.Length < 1)
            {
                errors.Add("No se ha seleccionado el Responsable de la Acción Correctiva");
            }

            return errors;
        }


        private void showBuscarPersonaMessage(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span>" + msg + "</span>";
        }


        private void showBuscarPersonaMessageSuccess(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #339933;\">" + msg + "</span>";
        }


        private void showBuscarPersonaMessageError(string msg)
        {
            if (msg != null)
                ltBuscarPersonaSummary.Text = "<span style=\"color: #FF0000;\">" + msg + "</span>";
        }

        protected void btBuscarPersonaLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarPersonaApellido.Text = "";
        }

        protected void gvBuscarPersonas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ltBuscarPersonaSummary.Text = "";

            int index = -1;
            try
            {
                index = Convert.ToInt32(e.CommandArgument);
            }
            catch (Exception ex)
            {
                return;
            }

            if (index < 0)
                return;


            if (e.CommandName.Equals("SetResponsableAccionCorrectiva"))
            {
                string rut = gvBuscarPersonas.Rows[index].Cells[1].Text;
                string nombre = gvBuscarPersonas.Rows[index].Cells[2].Text;

                PersonaInfo responsable = LogicController.getPersonaInfo(rut);

                if (responsable == null)
                {
                    txtNombreResponsableAccionCorrectiva.Text = "";
                    hfRutResponsableAccionCorrectiva.Value = "";

                    showBuscarPersonaMessageError("Error al recuperar la información de \"" + nombre + "\"");
                }
                else
                {
                    txtNombreResponsableAccionCorrectiva.Text = responsable.getNombre();
                    hfRutResponsableAccionCorrectiva.Value = responsable.getRut();

                    txtBuscarPersonaApellido.Text = "";
                    ltBuscarPersonaSummary.Text = "";

                    hfBuscarPersona_ModalPopupExtender.Hide();
                    hfAgregarAccionCorrectiva_ModalPopupExtender.Show();
                    uPanel.Update();
                }
            }
        }


        protected void SDSBuscarPersonas_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows > 0)
                showBuscarPersonaMessage(Convert.ToString(e.AffectedRows) + " registros coinciden con la búsqueda");
            else
                showBuscarPersonaMessage("No se han encontrado coincidencias con la búsqueda");
        }


        protected void btBuscarPersonaCancelar_Click(object sender, EventArgs e)
        {

            //Cancelar seleccionar responsable
            hfBuscarPersona_ModalPopupExtender.Hide();
            hfAgregarAccionCorrectiva_ModalPopupExtender.Show();
            uPanel.Update();
        }


        protected void btConfirmEdicionNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void btConfirmEdicionSi_Click(object sender, EventArgs e)
        {
            string status = updatePlanAccion();
            if (status == null)
            {
                Session.Remove("listAccionesCorrectivas");
                Session.Remove("listRemovedAccionesCorrectivas");
                Session.Remove("PlanAccion");

                lbMessage.Text = "El Plan de Acción se modifió satisfactoriamente";
                hfConfirmEdicion_ModalPopupExtender.Hide();
                hfMessage_ModalPopupExtender.Show();

            }
            else
            {
                showMessageError(status);
            }

            uPanel.Update();
        }

        protected void btConfirmRemoveAccionCorrectivaNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }


        protected void btConfirmRemoveAccionCorrectivaSi_Click(object sender, EventArgs e)
        {
            int index = getIndexRow();

            if (index < 0)
                return;

            List<AccionCorrectiva> listAccionesCorrectivas = getListAccionesCorrectivas();
            if (index < listAccionesCorrectivas.Count)
            {
                if (listAccionesCorrectivas[index].getIdAccionCorrectiva() == null)
                {
                    listAccionesCorrectivas.RemoveAt(index);
                }
                else
                {
                    addToListRemovedAccionesCorrectivas(listAccionesCorrectivas[index].getIdAccionCorrectiva());
                    listAccionesCorrectivas.RemoveAt(index);
                }

                setListAccionesCorrectivas(listAccionesCorrectivas);
                updateGVAccionesCorrectivas();

                uPanel.Update();
            }
        }


        protected void btConfirmVolverSi_Click(object sender, EventArgs e)
        {
            Session.Remove("PlanAccion");
            Session.Remove("listAccionesCorrectivas");
            Session.Remove("listRemovedAccionesCorrectivas");

            string previousPage = hfPreviousPage.Value;
            Response.Redirect(previousPage, true);
        }

        protected void btConfirmVolverNo_Click(object sender, EventArgs e)
        {
            uPanel.Update();
        }

    }
}