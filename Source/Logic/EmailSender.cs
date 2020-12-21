using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using NCCSAN.Source.Entity;
using System.Threading;

namespace NCCSAN.Source.Logic
{

    public class EmailSender
    {
        public static Dictionary<string, string> listEmailCredential = null;
        public static bool active;
        private static Queue<EmailMessage> queueEmail = null;
        private static readonly string[] nombre_meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };


        public static void addEmail(EmailMessage em)
        {

            if (queueEmail == null)
            {
                queueEmail = new Queue<EmailMessage>();
            }


            if (LogicController.thEmailSender == null)
            {
                if(EmailSender.listEmailCredential == null)
                {
                    listEmailCredential = LogicController.getEmailCredential();
                    if (listEmailCredential != null)
                    {
                        if (listEmailCredential["Active"] != null)
                        {
                            if (listEmailCredential["Active"].Equals("Si"))
                            {
                                EmailSender.active = true;
                            }
                            else
                            {
                                EmailSender.active = false;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                
                
                LogicController.thEmailSender = new Thread(() => sendEmails(listEmailCredential));
                LogicController.thEmailSender.Start();
            }

            if (em.getRecipient() != null)
            {
                queueEmail.Enqueue(em);
            }
        }


        public static void sendEmails(Dictionary<string, string> listEmailCredential)
        {
            if (listEmailCredential == null)
            {
                return;
            }

            if (queueEmail.Count > 0)
            {
                EmailMessage em = queueEmail.Dequeue();

                if (EmailSender.active)
                {
                    EmailSender.sendRAWMail(listEmailCredential, em);
                }
            }
            else
            {
                System.Threading.Thread.Sleep(10000);
            }

            EmailSender.sendEmails(listEmailCredential);
        }


        private static bool sendRAWMail(Dictionary<string, string> listEmailCredential, EmailMessage em)
        {
            string server = listEmailCredential["Server"];
            string domain = listEmailCredential["Domain"];
            int timeout = Convert.ToInt32(listEmailCredential["Timeout"]);
            string user = listEmailCredential["User"];
            string password = listEmailCredential["Password"];
            string email = listEmailCredential["Email"];

            try
            {
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential = new NetworkCredential(user, password, domain);
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress(email);

                smtpClient.Host = server;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;
                smtpClient.Timeout = timeout;

                message.From = fromAddress;
                message.Subject = em.getSubject();
                message.IsBodyHtml = true;
                message.Body = em.getBody();
                message.To.Add(em.getRecipient());

                if (em.getAttachments() != null)
                {
                    foreach (string attachment in em.getAttachments())
                    {
                        message.Attachments.Add(new System.Net.Mail.Attachment(attachment));
                    }
                }

                smtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static string sendMailRecuperacionClave(string usuario)
        {
            if (usuario == null)
            {
                return "Usuario inválido";
            }

            Usuario u = LogicController.getUsuario(usuario);
            if (u == null)
            {
                return "El usuario no existe en el sistema";
            }

            EmailInfo einfo = LogicController.getEmailInfo(u.getRutPersona());
            if (einfo == null)
            {
                return "No se puede recuperar la información de Usuario";
            }

            string clave = LogicController.getClaveAcceso(usuario);
            if (clave == null)
            {
                return "No se puede recuperar la información de Usuario";
            }


            string usuario_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Datos de Acceso</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Usuario</b></td><td><b>" + u.getUsuario() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clave de acceso</b></td><td>" + clave + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Perfil de Sistema</b></td><td>" + u.getNombreRol() + "</td></tr>"
                                + "</table>";


            string header = getHeader(einfo);
            if (header == null)
                return "Se ha producido un error al generar el correo electrónico";

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le recuerda "
                  + "sus datos de acceso."
                  + "<br />"
                  + "Para acceder al SGC debe utilizar las siguientes credenciales:"
                  + "<br />";


            body += "<br/><br />"
                  + usuario_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Recuperación Clave de Acceso SGC";

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);

            return "EMAIL:" + einfo.getDireccion();

        }


        public static bool sendMailUsuarioCreado(string usuario)
        {
            string server_home_path = /* THIS CODE HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

            if (usuario == null)
            {
                return false;
            }

            Usuario u = LogicController.getUsuario(usuario);
            if (u == null)
            {
                return false;
            }

            EmailInfo einfo = LogicController.getEmailInfo(u.getRutPersona());
            if (einfo == null)
            {
                return false;
            }

            string clave = LogicController.getClaveAcceso(u.getUsuario());
            if (clave == null)
            {
                return false;
            }


            string usuario_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Datos de Acceso</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Usuario</b></td><td><b>" + u.getUsuario() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clave inicial</b></td><td>" + clave + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Perfil de Sistema</b></td><td>" + u.getNombreRol() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>URL</b></td><td>" + server_home_path + "</td></tr>"
                                + "</table>";


            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se ha creado un usuario en la plataforma."
                  + "<br />"
                  + "Para acceder al SGC debe utilizar las siguientes credenciales:"
                  + "<br />";


            body += "<br/><br />"
                  + usuario_info
                  + "<br /><br />"
                  + "Recuerde cambiar la clave de acceso una vez que inicie sesión."
                  + "<br />"
                  + "Para ingresar con mayor facilidad se recomienda agregar la dirección a ls favoritos del explorador o crear un acceso directo.";

            body += getFooter();

            string subject = "Acceso a plataforma SGC";

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }



        /* -1: Excepción o error
         * >=0: Cantidad de emails enviados
        */
        public static int executeSendMailAllCentro(string server_home_path)
        {
            List<string> listIDCentros = LogicController.getListIDCentros();
            if (listIDCentros == null)
            {
                return -1;
            }


            int email_sent_total = 0;
            int email_sent_centro;

            foreach (string id_centro in listIDCentros)
            {

                email_sent_centro = executeSendMailCentro(id_centro, server_home_path);
                if (email_sent_centro < 0)
                {
                    return -1;
                }

                email_sent_total += email_sent_centro;
            }


            return email_sent_total;
        }



        public static int executeSendMailCentro(string id_centro, string server_home_path)
        {

            if (id_centro == null)
            {
                return -1;
            }


            if (server_home_path == null)
            {
                return -1;
            }


            List<EmailInfo> listEmailInfoJefeCalidad = LogicController.getListEmailInfoJefeCalidad(id_centro);
            if (listEmailInfoJefeCalidad == null)
            {
                return -1;
            }


            List<EmailInfo> listEmailInfoCoordinador = LogicController.getListEmailInfoCoordinador(id_centro);
            if (listEmailInfoCoordinador == null)
            {
                return -1;
            }

            ConfigEmailSender ces;

            List<EmailMessage> listRecipent = new List<EmailMessage>();


            {//Resumen Jefe Calidad

                ces = LogicController.getConfigEmailSender("Resumen", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return -1;
                }


                if (ces.getActivo())
                {
                    if (DateTime.Now.Day >= (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - ces.getDiasMensual()))
                    {
                        List<EmailMessage> listEmailMessageResumenJefeCalidad = getListEmailResumenJefeCalidad(id_centro, listEmailInfoJefeCalidad);
                        if (listEmailMessageResumenJefeCalidad == null)
                        {
                            return -1;
                        }

                        listRecipent.AddRange(listEmailMessageResumenJefeCalidad);
                    }
                }
            }


            {//Resumen Coordinador

                ces = LogicController.getConfigEmailSender("Resumen", "Coordinador", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    if (DateTime.Now.Day >= (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - ces.getDiasMensual()))
                    {
                        List<EmailMessage> listEmailMessageResumenCoordinador = getListEmailResumenCoordinador(id_centro, ces.getDiasMensual(), listEmailInfoCoordinador);
                        if (listEmailMessageResumenCoordinador == null)
                        {
                            return -1;
                        }

                        listRecipent.AddRange(listEmailMessageResumenCoordinador);
                    }
                }
            }


            {//Acciones inmediatas Inspector

                ces = LogicController.getConfigEmailSender("Acción inmediata pendiente", "Inspector", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageAccionesInmediatasPendienteInspector = getListEmailAccionesInmediatasPendienteInspector(id_centro, ces.getDiasAlerta(), ces.getDiasLimite());
                    if (listEmailMessageAccionesInmediatasPendienteInspector == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageAccionesInmediatasPendienteInspector);
                }
            }


            {//Alerta de Acciones correctivas Responsable

                ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Usuario", id_centro); //Usuario=*Responsable
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageAccionesCorrectivasAlertaResponsable = getListEmailAccionesCorrectivasAlertaResponsable(id_centro, ces.getDiasAlerta(), server_home_path);
                    if (listEmailMessageAccionesCorrectivasAlertaResponsable == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageAccionesCorrectivasAlertaResponsable);
                }
            }


            {//Alerta de Acciones correctivas Jefe Calidad

                ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageAccionesCorrectivasAlertaJefeCalidad = getListEmailAccionesCorrectivasAlertaJefeCalidad(id_centro, ces.getDiasAlerta(), listEmailInfoJefeCalidad, server_home_path);
                    if (listEmailMessageAccionesCorrectivasAlertaJefeCalidad == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageAccionesCorrectivasAlertaJefeCalidad);
                }
            }


            {//Alerta de Acciones correctivas Coordinador

                ces = LogicController.getConfigEmailSender("Acción correctiva en curso", "Coordinador", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageAccionesCorrectivasAlertaCoordinador = getListEmailAccionesCorrectivasAlertaCoordinador(id_centro, ces.getDiasAlerta(), listEmailInfoCoordinador, server_home_path);
                    if (listEmailMessageAccionesCorrectivasAlertaCoordinador == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageAccionesCorrectivasAlertaCoordinador);
                }
            }


            {//Alerta de Investigación pendiente Jefe Calidad

                ces = LogicController.getConfigEmailSender("Investigación pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageInvestigacionesPendientesAlertaJefeCalidad = getListEmailInvestigacionesPendientesAlertaJefeCalidad(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoJefeCalidad, server_home_path);
                    if (listEmailMessageInvestigacionesPendientesAlertaJefeCalidad == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageInvestigacionesPendientesAlertaJefeCalidad);
                }
            }


            {//Alerta de Investigación pendiente Coordinador

                ces = LogicController.getConfigEmailSender("Investigación pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageInvestigacionesPendientesAlertaCoordinador = getListEmailInvestigacionesPendientesAlertaCoordinador(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoCoordinador, server_home_path);
                    if (listEmailMessageInvestigacionesPendientesAlertaCoordinador == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageInvestigacionesPendientesAlertaCoordinador);
                }
            }



            {//Alerta de Investigación en curso Jefe Calidad

                ces = LogicController.getConfigEmailSender("Investigación en curso", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad = getListEmailInvestigacionesEnCursoAlertaJefeCalidad(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoJefeCalidad, server_home_path);
                    if (listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad);
                }
            }



            {//Alerta de Investigación en curso Coordinador

                ces = LogicController.getConfigEmailSender("Investigación en curso", "Coordinador", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageInvestigacionesEnCursoAlertaCoordinador = getListEmailInvestigacionesEnCursoAlertaCoordinador(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoCoordinador, server_home_path);
                    if (listEmailMessageInvestigacionesEnCursoAlertaCoordinador == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageInvestigacionesEnCursoAlertaCoordinador);
                }
            }



            {//Alerta de Investigación en curso Responsable

                ces = LogicController.getConfigEmailSender("Investigación en curso", "Usuario", id_centro); //Usuario=*Responsable
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageInvestigacionesEnCursoAlertaResponsable = getListEmailInvestigacionEnCursoAlertaResponsable(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), server_home_path);
                    if (listEmailMessageInvestigacionesEnCursoAlertaResponsable == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageInvestigacionesEnCursoAlertaResponsable);
                }
            }



            {//Alerta de Evaluación pendiente Jefe Calidad

                ces = LogicController.getConfigEmailSender("Evaluación pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageEvaluacionesPendientesAlertaJefeCalidad = getListEmailEvaluacionesPendientesAlertaJefeCalidad(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoJefeCalidad, server_home_path);
                    if (listEmailMessageEvaluacionesPendientesAlertaJefeCalidad == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageEvaluacionesPendientesAlertaJefeCalidad);
                }
            }


            {//Alerta de Evaluación pendiente Coordinador

                ces = LogicController.getConfigEmailSender("Evaluación pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessageEvaluacionesPendientesAlertaCoordinador = getListEmailEvaluacionesPendientesAlertaCoordinador(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoCoordinador, server_home_path);
                    if (listEmailMessageEvaluacionesPendientesAlertaCoordinador == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessageEvaluacionesPendientesAlertaCoordinador);
                }
            }


            {//Alerta de Plan Acción pendiente Coordinador

                ces = LogicController.getConfigEmailSender("Plan de acción pendiente", "Coordinador", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessagePlanesAccionPendientesAlertaCoordinador = getListEmailPlanesAccionPendientesAlertaCoordinador(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoCoordinador, server_home_path);
                    if (listEmailMessagePlanesAccionPendientesAlertaCoordinador == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessagePlanesAccionPendientesAlertaCoordinador);
                }
            }



            {//Alerta de Plan Acción pendiente Jefe Calidad

                ces = LogicController.getConfigEmailSender("Plan de acción pendiente", "Jefe Calidad", id_centro);
                if (ces == null)
                {
                    return -1;
                }

                if (ces.getActivo())
                {
                    List<EmailMessage> listEmailMessagePlanesAccionPendientesAlertaJefeCalidad = getListEmailPlanesAccionPendientesAlertaJefeCalidad(id_centro, ces.getDiasAlerta(), ces.getDiasLimite(), listEmailInfoJefeCalidad, server_home_path);
                    if (listEmailMessagePlanesAccionPendientesAlertaJefeCalidad == null)
                    {
                        return -1;
                    }

                    listRecipent.AddRange(listEmailMessagePlanesAccionPendientesAlertaJefeCalidad);
                }
            }


            foreach (EmailMessage em in listRecipent)
            {
                addEmail(em);
            }

            return listRecipent.Count;
        }



        public static List<EmailMessage> getListEmailPlanesAccionPendientesAlertaJefeCalidad(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoJefeCalidad, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessagPlanesAccionPendientesAlertaJefeCalidad = new List<EmailMessage>();
            if (listEmailInfoJefeCalidad == null)
            {
                return null;
            }
            else if (listEmailInfoJefeCalidad.Count < 1)
            {
                return listEmailMessagPlanesAccionPendientesAlertaJefeCalidad;
            }


            List<Evento> listPlanesAccionPendientesAlerta = LogicController.getListEventosPlanAccionPendienteAlerta(id_centro, dias_desde, dias_hasta);
            if (listPlanesAccionPendientesAlerta == null)
            {
                return null;
            }


            string subject = "Plan de Acción pendiente";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listPlanesAccionPendientesAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoJefeCalidad)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    investigacion = LogicController.getInvestigacion(evento.getCodigo());
                    if (investigacion == null)
                    {
                        return null;
                    }

                    fecha_limite = Convert.ToDateTime(investigacion.getFechaRespuesta()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaRespuesta())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "el siguiente Evento requiere que se ingrese el Plan de Acción y se está llegando a la fecha límite para hacerlo (<b>" + fecha_limite + "</b>):"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Investigación</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Responsable</b></td><td><b>" + investigacion.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de inicio</b></td><td><b>" + investigacion.getFechaInicio() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de cierre</b></td><td><b>" + investigacion.getFechaCierre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de evaluación</b></td><td><b>" + investigacion.getFechaRespuesta() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede ingresar el Plan de Acción visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "PlanesAccion/ListarPlanesAccionPendientes.aspx?ce=" + investigacion.getCodigoEvento()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessagPlanesAccionPendientesAlertaJefeCalidad.Add(em);
                }
            }

            return listEmailMessagPlanesAccionPendientesAlertaJefeCalidad;

        }


        public static List<EmailMessage> getListEmailPlanesAccionPendientesAlertaCoordinador(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoCoordinador, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessagPlanesAccionPendientesAlertaCoordinador = new List<EmailMessage>();
            if (listEmailInfoCoordinador == null)
            {
                return null;
            }
            else if (listEmailInfoCoordinador.Count < 1)
            {
                return listEmailMessagPlanesAccionPendientesAlertaCoordinador;
            }


            List<Evento> listPlanesAccionPendientesAlerta = LogicController.getListEventosPlanAccionPendienteAlerta(id_centro, dias_desde, dias_hasta);
            if (listPlanesAccionPendientesAlerta == null)
            {
                return null;
            }


            string subject = "Plan de Acción pendiente";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listPlanesAccionPendientesAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoCoordinador)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    investigacion = LogicController.getInvestigacion(evento.getCodigo());
                    if (investigacion == null)
                    {
                        return null;
                    }

                    fecha_limite = Convert.ToDateTime(investigacion.getFechaRespuesta()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaRespuesta())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "el siguiente Evento requiere que se ingrese el Plan de Acción y se está llegando a la fecha límite para hacerlo (<b>" + fecha_limite + "</b>):"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Investigación</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Responsable</b></td><td><b>" + investigacion.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de inicio</b></td><td><b>" + investigacion.getFechaInicio() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de cierre</b></td><td><b>" + investigacion.getFechaCierre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de evaluación</b></td><td><b>" + investigacion.getFechaRespuesta() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede ingresar el Plan de Acción visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "PlanesAccion/ListarPlanesAccionPendientes.aspx?ce=" + investigacion.getCodigoEvento()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessagPlanesAccionPendientesAlertaCoordinador.Add(em);
                }
            }

            return listEmailMessagPlanesAccionPendientesAlertaCoordinador;

        }

        public static List<EmailMessage> getListEmailEvaluacionesPendientesAlertaCoordinador(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoCoordinador, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessageEvaluacionesPendientesAlertaCoordinador = new List<EmailMessage>();
            if (listEmailInfoCoordinador == null)
            {
                return null;
            }
            else if (listEmailInfoCoordinador.Count < 1)
            {
                return listEmailMessageEvaluacionesPendientesAlertaCoordinador;
            }


            List<Evento> listEvaluacionesPendientesAlerta = LogicController.getListEventosEvaluacionPendienteAlerta(id_centro, dias_desde, dias_hasta);
            if (listEvaluacionesPendientesAlerta == null)
            {
                return null;
            }


            string subject = "Evaluación pendiente";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listEvaluacionesPendientesAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoCoordinador)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    investigacion = LogicController.getInvestigacion(evento.getCodigo());
                    if (investigacion == null)
                    {
                        return null;
                    }

                    fecha_limite = Convert.ToDateTime(investigacion.getFechaCierre()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaCierre())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Investigación no se ha evaluado y se está llegando a la fecha límite para hacerlo (<b>" + fecha_limite + "</b>):"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Investigación</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Responsable</b></td><td><b>" + investigacion.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de inicio</b></td><td><b>" + investigacion.getFechaInicio() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de cierre</b></td><td><b>" + investigacion.getFechaCierre() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede evaluar la Investigación visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Investigaciones/ListarEvaluacionesPendientes.aspx?ce=" + investigacion.getCodigoEvento()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessageEvaluacionesPendientesAlertaCoordinador.Add(em);
                }
            }

            return listEmailMessageEvaluacionesPendientesAlertaCoordinador;

        }

        public static List<EmailMessage> getListEmailEvaluacionesPendientesAlertaJefeCalidad(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoJefeCalidad, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessageEvaluacionesPendientesAlertaJefeCalidad = new List<EmailMessage>();
            if (listEmailInfoJefeCalidad == null)
            {
                return null;
            }
            else if (listEmailInfoJefeCalidad.Count < 1)
            {
                return listEmailMessageEvaluacionesPendientesAlertaJefeCalidad;
            }


            List<Evento> listEvaluacionesPendientesAlerta = LogicController.getListEventosEvaluacionPendienteAlerta(id_centro, dias_desde, dias_hasta);
            if (listEvaluacionesPendientesAlerta == null)
            {
                return null;
            }


            string subject = "Evaluación pendiente";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listEvaluacionesPendientesAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoJefeCalidad)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    investigacion = LogicController.getInvestigacion(evento.getCodigo());
                    if (investigacion == null)
                    {
                        return null;
                    }

                    fecha_limite = Convert.ToDateTime(investigacion.getFechaCierre()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaCierre())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Investigación no se ha evaluado y se está llegando a la fecha límite para hacerlo (<b>" + fecha_limite + "</b>):"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Investigación</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Responsable</b></td><td><b>" + investigacion.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de inicio</b></td><td><b>" + investigacion.getFechaInicio() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de cierre</b></td><td><b>" + investigacion.getFechaCierre() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }

                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede evaluar la Investigación visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Investigaciones/ListarEvaluacionesPendientes.aspx?ce=" + investigacion.getCodigoEvento()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessageEvaluacionesPendientesAlertaJefeCalidad.Add(em);
                }
            }

            return listEmailMessageEvaluacionesPendientesAlertaJefeCalidad;

        }

        public static List<EmailMessage> getListEmailInvestigacionEnCursoAlertaResponsable(string id_centro, int dias_desde, int dias_hasta, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }


            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<Evento> listEventosInvestigacionEnCursoAlerta = LogicController.getListEventosInvestigacionEnCursoAlerta(id_centro, dias_desde, dias_hasta);
            if (listEventosInvestigacionEnCursoAlerta == null)
            {
                return null;
            }

            List<EmailMessage> listEmailInvestigacionesAlerta = new List<EmailMessage>();

            string subject = "Alerta de Investigación";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailInfo einfo;
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listEventosInvestigacionEnCursoAlerta)
            {

                investigacion = LogicController.getInvestigacion(evento.getCodigo());
                if (investigacion == null)
                {
                    return null;
                }


                einfo = LogicController.getEmailInfo(investigacion.getResponsable().getRut());
                if (einfo == null)
                {
                    return null;
                }

                header = getHeader(einfo);
                if (header == null)
                {
                    return null;
                }


                fecha_limite = Convert.ToDateTime(investigacion.getFechaInicio()).AddDays(dias_hasta).ToShortDateString();
                dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaInicio())).TotalDays);

                body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                        + "la siguiente Investigación asignada está llegando a la fecha límite (<b>" + fecha_limite + "</b>) y no la ha reportado:"
                        + "<br />"
                        + "<br />"
                        + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                        + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Investigación</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de inicio</b></td><td><b>" + investigacion.getFechaInicio() + "</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite</b></td><td><b>" + fecha_limite + "</b></td></tr>"
                        + "</table>"
                        + "<br />"
                        + "<br />"
                        + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                        + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                        + "</table>"
                        + "<br />"
                        + "<br />";


                if (dias == 0)
                {
                    body += "El plazo límite es <b>HOY</b>, por tanto, resuelva la situación a la brevedad.";
                }
                else if (dias == 1)
                {
                    body += "Queda <b>1 día</b> para su vencimiento, por tanto, resuelva la situación a la brevedad.";
                }
                else
                {
                    body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, resuelva la situación a la brevedad.";
                }

                link_message = "<br />"
                             + "<br />"
                             + "<br />"
                             + "Puede revisar el detalle del Evento visitando el siguiente vínculo:"
                             + "<br />"
                             + "<br />"
                             + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Registros/DetalleEvento.aspx?ce=" + investigacion.getCodigoEvento()
                             + "<br />"
                             + "<br />"
                             + "<br />";

                message = header + body + link_message + footer;

                em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                listEmailInvestigacionesAlerta.Add(em);

            }


            return listEmailInvestigacionesAlerta;
        }

        public static List<EmailMessage> getListEmailInvestigacionesEnCursoAlertaCoordinador(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoCoordinador, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessageInvestigacionesEnCursoAlertaCoordinador = new List<EmailMessage>();
            if (listEmailInfoCoordinador == null)
            {
                return null;
            }
            else if (listEmailInfoCoordinador.Count < 1)
            {
                return listEmailMessageInvestigacionesEnCursoAlertaCoordinador;
            }


            List<Evento> listInvestigacionesEnCursoAlerta = LogicController.getListEventosInvestigacionEnCursoAlerta(id_centro, dias_desde, dias_hasta);
            if (listInvestigacionesEnCursoAlerta == null)
            {
                return null;
            }


            string subject = "Investigación en curso";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listInvestigacionesEnCursoAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoCoordinador)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    investigacion = LogicController.getInvestigacion(evento.getCodigo());
                    if (investigacion == null)
                    {
                        return null;
                    }

                    fecha_limite = Convert.ToDateTime(investigacion.getFechaInicio()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaInicio())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Investigación está llegando al plazo límite (<b>" + fecha_limite + "</b>) y el responsable no la ha reportado:"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del la Investigación</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Responsable</b></td><td><b>" + investigacion.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar el cierre de la Investigación visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Investigaciones/ListarInvestigacionesEnCurso.aspx?ce=" + investigacion.getCodigoEvento()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessageInvestigacionesEnCursoAlertaCoordinador.Add(em);
                }
            }

            return listEmailMessageInvestigacionesEnCursoAlertaCoordinador;

        }

        public static List<EmailMessage> getListEmailInvestigacionesEnCursoAlertaJefeCalidad(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoJefeCalidad, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad = new List<EmailMessage>();
            if (listEmailInfoJefeCalidad == null)
            {
                return null;
            }
            else if (listEmailInfoJefeCalidad.Count < 1)
            {
                return listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad;
            }


            List<Evento> listInvestigacionesEnCursoAlerta = LogicController.getListEventosInvestigacionEnCursoAlerta(id_centro, dias_desde, dias_hasta);
            if (listInvestigacionesEnCursoAlerta == null)
            {
                return null;
            }


            string subject = "Investigación en curso";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            Investigacion investigacion;
            string link_message;
            foreach (Evento evento in listInvestigacionesEnCursoAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoJefeCalidad)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    investigacion = LogicController.getInvestigacion(evento.getCodigo());
                    if (investigacion == null)
                    {
                        return null;
                    }

                    fecha_limite = Convert.ToDateTime(investigacion.getFechaInicio()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(investigacion.getFechaInicio())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Investigación está llegando al plazo límite (<b>" + fecha_limite + "</b>) y el responsable no la ha reportado:"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del la Investigación</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Responsable</b></td><td><b>" + investigacion.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar el cierre de la Investigación visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Investigaciones/ListarInvestigacionesEnCurso.aspx?ce=" + investigacion.getCodigoEvento()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad.Add(em);
                }
            }

            return listEmailMessageInvestigacionesEnCursoAlertaJefeCalidad;

        }

        public static List<EmailMessage> getListEmailInvestigacionesPendientesAlertaCoordinador(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoCoordinador, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessageInvestigacionesPendientesAlertaCoordinador = new List<EmailMessage>();
            if (listEmailInfoCoordinador == null)
            {
                return null;
            }
            else if (listEmailInfoCoordinador.Count < 1)
            {
                return listEmailMessageInvestigacionesPendientesAlertaCoordinador;
            }


            List<Evento> listInvestigacionesPendientesAlerta = LogicController.getListEventosInvestigacionPendienteAlerta(id_centro, dias_desde, dias_hasta);
            if (listInvestigacionesPendientesAlerta == null)
            {
                return null;
            }


            string subject = "Investigación pendiente no iniciada";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            EmailMessage em;
            string link_message;
            foreach (Evento evento in listInvestigacionesPendientesAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoCoordinador)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    fecha_limite = Convert.ToDateTime(evento.getFechaIngreso()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(evento.getFechaIngreso())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Investigación está llegando al plazo límite (<b>" + fecha_limite + "</b>) y no se ha iniciado:"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de registro</b></td><td><b>" + evento.getFechaIngreso().ToShortDateString() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar el inicio de la Investigación visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Investigaciones/ListarInvestigacionesPendientes.aspx?ce=" + evento.getCodigo()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessageInvestigacionesPendientesAlertaCoordinador.Add(em);
                }
            }

            return listEmailMessageInvestigacionesPendientesAlertaCoordinador;

        }

        public static List<EmailMessage> getListEmailInvestigacionesPendientesAlertaJefeCalidad(string id_centro, int dias_desde, int dias_hasta, List<EmailInfo> listEmailInfoJefeCalidad, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }


            if (dias_hasta < dias_desde)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<EmailMessage> listEmailMessageInvestigacionesPendientesAlertaJefeCalidad = new List<EmailMessage>();
            if (listEmailInfoJefeCalidad == null)
            {
                return null;
            }
            else if (listEmailInfoJefeCalidad.Count < 1)
            {
                return listEmailMessageInvestigacionesPendientesAlertaJefeCalidad;
            }


            List<Evento> listInvestigacionesPendientesAlerta = LogicController.getListEventosInvestigacionPendienteAlerta(id_centro, dias_desde, dias_hasta);
            if (listInvestigacionesPendientesAlerta == null)
            {
                return null;
            }


            string subject = "Investigación pendiente no iniciada";

            string header;
            string body;
            string message;
            int dias;
            string fecha_limite;
            string footer = getFooter();
            string link_message;
            EmailMessage em;
            foreach (Evento evento in listInvestigacionesPendientesAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoJefeCalidad)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }


                    fecha_limite = Convert.ToDateTime(evento.getFechaIngreso()).AddDays(dias_hasta).ToShortDateString();
                    dias = dias_hasta - Convert.ToInt32((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(evento.getFechaIngreso())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Investigación está llegando al plazo límite (<b>" + fecha_limite + "</b>) y no se ha iniciado:"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de registro</b></td><td><b>" + evento.getFechaIngreso().ToShortDateString() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";


                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar el inicio de la Investigación visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "Investigaciones/ListarInvestigacionesPendientes.aspx?ce=" + evento.getCodigo()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailMessageInvestigacionesPendientesAlertaJefeCalidad.Add(em);
                }
            }

            return listEmailMessageInvestigacionesPendientesAlertaJefeCalidad;

        }

        public static List<EmailMessage> getListEmailAccionesCorrectivasAlertaCoordinador(string id_centro, int dias_alerta, List<EmailInfo> listEmailInfoCoordinador, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_alerta < 0)
            {
                return null;
            }

            if (listEmailInfoCoordinador == null)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<AccionCorrectiva> listAccionesCorrectivasAlerta = LogicController.getListAccionesCorrectivasAlerta(id_centro, dias_alerta);
            if (listAccionesCorrectivasAlerta == null)
            {
                return null;
            }


            List<EmailMessage> listEmailAccionesCorrectivasAlerta = new List<EmailMessage>();

            string subject = "Alerta de Acción Correctiva";

            string header;
            string body;
            string message;
            string codigo_evento;
            int dias;
            string footer = getFooter();
            EmailMessage em;
            string link_message;
            foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivasAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoCoordinador)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }

                    codigo_evento = LogicController.getCodigoPlanAccionAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                    if (codigo_evento == null)
                    {
                        return null;
                    }


                    dias = Convert.ToInt32((Convert.ToDateTime(accion_correctiva.getFechaLimite()) - Convert.ToDateTime(DateTime.Now.ToShortDateString())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Acción Correctiva está llegando a la fecha límite (<b>" + accion_correctiva.getFechaLimite() + "</b>) y no se ha realizado:"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Acción Correctiva</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código Plan de Acción</b></td><td><b>" + codigo_evento + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>RUT responsable</b></td><td><b>" + accion_correctiva.getResponsable().getRut() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Nombre responsable</b></td><td><b>" + accion_correctiva.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Tarea asignada</b></td><td><b>" + accion_correctiva.getDescripcion() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite</b></td><td><b>" + accion_correctiva.getFechaLimite() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";
                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar la Acción Correctiva visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "PlanesAccion/RegistrarAccionCorrectiva.aspx?iac=" + accion_correctiva.getIdAccionCorrectiva()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailAccionesCorrectivasAlerta.Add(em);
                }

            }


            return listEmailAccionesCorrectivasAlerta;
        }

        public static List<EmailMessage> getListEmailAccionesCorrectivasAlertaJefeCalidad(string id_centro, int dias_alerta, List<EmailInfo> listEmailInfoJefeCalidad, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_alerta < 0)
            {
                return null;
            }


            if (listEmailInfoJefeCalidad == null)
            {
                return null;
            }


            List<AccionCorrectiva> listAccionesCorrectivasAlerta = LogicController.getListAccionesCorrectivasAlerta(id_centro, dias_alerta);
            if (listAccionesCorrectivasAlerta == null)
            {
                return null;
            }


            List<EmailMessage> listEmailAccionesCorrectivasAlerta = new List<EmailMessage>();

            string subject = "Alerta de Acción Correctiva";

            string header;
            string body;
            string message;
            string codigo_evento;
            int dias;
            string footer = getFooter();
            EmailMessage em;
            string link_message;
            foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivasAlerta)
            {
                foreach (EmailInfo einfo in listEmailInfoJefeCalidad)
                {
                    header = getHeader(einfo);
                    if (header == null)
                    {
                        return null;
                    }

                    codigo_evento = LogicController.getCodigoPlanAccionAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                    if (codigo_evento == null)
                    {
                        return null;
                    }


                    dias = Convert.ToInt32((Convert.ToDateTime(accion_correctiva.getFechaLimite()) - Convert.ToDateTime(DateTime.Now.ToShortDateString())).TotalDays);

                    body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                            + "la siguiente Acción Correctiva está llegando a la fecha límite  (<b>" + accion_correctiva.getFechaLimite() + "</b>) y no se ha realizado:"
                            + "<br />"
                            + "<br />"
                            + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                            + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Acción Correctiva</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Código Plan de Acción</b></td><td><b>" + codigo_evento + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>RUT responsable</b></td><td><b>" + accion_correctiva.getResponsable().getRut() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Nombre responsable</b></td><td><b>" + accion_correctiva.getResponsable().getNombre() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Tarea asignada</b></td><td><b>" + accion_correctiva.getDescripcion() + "</b></td></tr>"
                            + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite</b></td><td><b>" + accion_correctiva.getFechaLimite() + "</b></td></tr>"
                            + "</table>"
                            + "<br />"
                            + "<br />";
                    if (dias == 0)
                    {
                        body += "El plazo límite es <b>HOY</b>, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else if (dias == 1)
                    {
                        body += "Queda <b>1 día</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }
                    else
                    {
                        body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, se debe resolver la situación a la brevedad.";
                    }


                    link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar la Acción Correctiva visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "PlanesAccion/RegistrarAccionCorrectiva.aspx?iac=" + accion_correctiva.getIdAccionCorrectiva()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                    message = header + body + link_message + footer;

                    em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                    listEmailAccionesCorrectivasAlerta.Add(em);
                }

            }


            return listEmailAccionesCorrectivasAlerta;
        }

        public static List<EmailMessage> getListEmailAccionesCorrectivasAlertaResponsable(string id_centro, int dias_alerta, string server_home_path)
        {
            if (id_centro == null)
            {
                return null;
            }


            if (dias_alerta < 0)
            {
                return null;
            }


            if (server_home_path == null)
            {
                return null;
            }


            List<AccionCorrectiva> listAccionesCorrectivasAlerta = LogicController.getListAccionesCorrectivasAlerta(id_centro, dias_alerta);
            if (listAccionesCorrectivasAlerta == null)
            {
                return null;
            }

            List<EmailMessage> listEmailAccionesCorrectivasAlerta = new List<EmailMessage>();

            string subject = "Alerta de Acción Correctiva";

            string header;
            string body;
            string message;
            string codigo_evento;
            int dias;
            string footer = getFooter();
            EmailInfo einfo;
            EmailMessage em;
            string link_message;
            foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivasAlerta)
            {

                einfo = LogicController.getEmailInfo(accion_correctiva.getResponsable().getRut());
                if (einfo == null)
                {
                    return null;
                }

                header = getHeader(einfo);
                if (header == null)
                {
                    return null;
                }

                codigo_evento = LogicController.getCodigoPlanAccionAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                if (codigo_evento == null)
                {
                    return null;
                }


                dias = Convert.ToInt32((Convert.ToDateTime(accion_correctiva.getFechaLimite()) - Convert.ToDateTime(DateTime.Now.ToShortDateString())).TotalDays);

                body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                        + "tiene la siguiente Acción Correctiva pendiente:"
                        + "<br />"
                        + "<br />"
                        + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                        + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Acción Correctiva</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Código Plan de Acción</b></td><td><b>" + codigo_evento + "</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Tarea asignada</b></td><td><b>" + accion_correctiva.getDescripcion() + "</b></td></tr>"
                        + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite</b></td><td><b>" + accion_correctiva.getFechaLimite() + "</b></td></tr>"
                        + "</table>"
                        + "<br />"
                        + "<br />";
                if (dias == 0)
                {
                    body += "El plazo límite es <b>HOY</b>, por tanto, resuelva la situación a la brevedad.";
                }
                else if (dias == 1)
                {
                    body += "Queda <b>1 día</b> para su vencimiento, por tanto, resuelva la situación a la brevedad.";
                }
                else
                {
                    body += "Quedan <b>" + Convert.ToString(dias) + " días</b> para su vencimiento, por tanto, resuelva la situación a la brevedad.";
                }


                link_message = "<br />"
                                 + "<br />"
                                 + "<br />"
                                 + "Puede registrar la Acción Correctiva visitando el siguiente vínculo:"
                                 + "<br />"
                                 + "<br />"
                                 + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + server_home_path + "PlanesAccion/RegistrarAccionCorrectiva.aspx?iac=" + accion_correctiva.getIdAccionCorrectiva()
                                 + "<br />"
                                 + "<br />"
                                 + "<br />";


                message = header + body + link_message + footer;

                em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                listEmailAccionesCorrectivasAlerta.Add(em);

            }


            return listEmailAccionesCorrectivasAlerta;
        }

        public static List<EmailMessage> getListEmailAccionesInmediatasPendienteInspector(string id_centro, int dias_desde, int dias_hasta)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (dias_desde < 0)
            {
                return null;
            }

            if (dias_hasta < 0)
            {
                return null;
            }

            if (dias_hasta < dias_desde)
            {
                return null;
            }

            List<string> listRutInspectores = LogicController.getListRUTInspectores(id_centro);
            if (listRutInspectores == null)
            {
                return null;
            }

            List<EmailMessage> listEmailAccionesInmediatasPendienteInspector = new List<EmailMessage>();

            string subject = "Acciones inmediatas pendiente";

            int cantidad_acciones_inmediatas_pendiente;
            string header;
            string body;
            string message;
            string footer = getFooter();
            EmailInfo einfo;
            EmailMessage em;
            foreach (string rut_inspector in listRutInspectores)
            {
                cantidad_acciones_inmediatas_pendiente = LogicController.getCantidadAccionesInmediatasPendienteInspector(id_centro, rut_inspector, dias_desde, dias_hasta);
                if (cantidad_acciones_inmediatas_pendiente < 0)
                {
                    return null;
                }

                if (cantidad_acciones_inmediatas_pendiente < 1)
                {
                    continue;
                }

                einfo = LogicController.getEmailInfo(rut_inspector);
                if (einfo == null)
                {
                    return null;
                }

                header = getHeader(einfo);
                if (header == null)
                {
                    return null;
                }

                body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                        + "a la fecha tiene las siguientes tareas pendientes:"
                        + "<br />"
                        + "<br />"
                        + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                        + "     <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Resumen</b></td></tr>"
                        + "     <tr><td style=\"background-color:#FFCC00\"><b>Acciones inmediatas pendiente</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_acciones_inmediatas_pendiente) + "</b>&nbsp;&nbsp;</td></tr>"
                        + "</table>"
                        + "<br />"
                        + "<br />"
                        + "Resuelva la situación a la brevedad.";

                message = header + body + footer;

                em = new EmailMessage(einfo.getDireccion(), subject, message, null);

                listEmailAccionesInmediatasPendienteInspector.Add(em);

            }


            return listEmailAccionesInmediatasPendienteInspector;
        }

        public static List<EmailMessage> getListEmailResumenCoordinador(string id_centro, int dias_mensual, List<EmailInfo> listEmailInfoCoordinador)
        {

            if (id_centro == null)
            {
                return null;
            }


            if (dias_mensual < 0)
            {
                return null;
            }


            if (listEmailInfoCoordinador == null)
            {
                return null;
            }


            int cantidad_acciones_inmediatas_pendiente = LogicController.getCantidadAccionesInmediatasPendiente(id_centro);
            if (cantidad_acciones_inmediatas_pendiente < 0)
            {
                return null;
            }

            int cantidad_planes_accion_sin_cerrar = LogicController.getCantidadPlanesAccionSinCerrar(id_centro);
            if (cantidad_planes_accion_sin_cerrar < 0)
            {
                return null;
            }


            ConfigEmailSender ces;

            ces = LogicController.getConfigEmailSender("Verificación pendiente", "Coordinador", id_centro);
            if (ces == null)
            {
                return null;
            }

            int cantidad_planes_accion_sin_verificar;
            if (ces.getActivo())
            {
                cantidad_planes_accion_sin_verificar = LogicController.getCantidadPlanesAccionSinVerificar(id_centro, ces.getDiasAlerta());
                if (cantidad_planes_accion_sin_verificar < 0)
                {
                    return null;
                }
            }
            else
            {
                cantidad_planes_accion_sin_verificar = 0;
            }


            List<EmailMessage> listEmailResumenCoordinador = new List<EmailMessage>();

            if ((cantidad_acciones_inmediatas_pendiente < 1) && (cantidad_planes_accion_sin_cerrar < 1) && (cantidad_planes_accion_sin_verificar < 1))
            {
                return listEmailResumenCoordinador;
            }

            int dias = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;


            string body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                        + "a la fecha tiene las siguientes tareas pendientes:"
                        + "<br />"
                        + "<br />"
                        + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                        + "     <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Resumen</b></td></tr>";
            if (cantidad_acciones_inmediatas_pendiente > 0)
            {
                body += "       <tr><td style=\"background-color:#FFCC00\"><b>Acciones inmediatas pendiente</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_acciones_inmediatas_pendiente) + "</b>&nbsp;&nbsp;</td></tr>";
            }

            if (cantidad_planes_accion_sin_cerrar > 0)
            {
                body += "       <tr><td style=\"background-color:#FFCC00\"><b>Planes de acción sin cerrar (100% ejecución)</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_planes_accion_sin_cerrar) + "</b>&nbsp;&nbsp;</td></tr>";
            }

            if (cantidad_planes_accion_sin_verificar > 0)
            {
                body += "       <tr><td style=\"background-color:#FFCC00\"><b>Planes de acción sin verificar</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_planes_accion_sin_verificar) + "</b>&nbsp;&nbsp;</td></tr>";
            }
            body += "</table>"
                 + "<br />"
                 + "<br />"
                 + "Quedan <b>" + Convert.ToString(dias) + " días</b> para cerrar el mes, por lo tanto, resuelva la situación a la brevedad.";

            string subject = "Resumen de tareas " + nombre_meses[DateTime.Now.Month - 1] + " " + Convert.ToString(DateTime.Now.Year);
            string header;
            string message;
            string footer = getFooter();
            EmailMessage em;
            foreach (EmailInfo einfo in listEmailInfoCoordinador)
            {
                header = getHeader(einfo);
                if (header == null)
                {
                    continue;
                }

                message = header + body + footer;

                em = new EmailMessage(einfo.getDireccion(), subject, message, null);
                listEmailResumenCoordinador.Add(em);
            }


            return listEmailResumenCoordinador;
        }


        public static List<EmailMessage> getListEmailResumenJefeCalidad(string id_centro, List<EmailInfo> listEmailInfoJefeCalidad)
        {

            if (id_centro == null)
            {
                return null;
            }


            if (listEmailInfoJefeCalidad == null)
            {
                return null;
            }


            int cantidad_acciones_inmediatas_pendiente = LogicController.getCantidadAccionesInmediatasPendiente(id_centro);
            if (cantidad_acciones_inmediatas_pendiente < 0)
            {
                return null;
            }

            int cantidad_planes_accion_sin_cerrar = LogicController.getCantidadPlanesAccionSinCerrar(id_centro);
            if (cantidad_planes_accion_sin_cerrar < 0)
            {
                return null;
            }


            ConfigEmailSender ces;

            ces = LogicController.getConfigEmailSender("Verificación pendiente", "Jefe Calidad", id_centro);
            if (ces == null)
            {
                return null;
            }

            int cantidad_planes_accion_sin_verificar;
            if (ces.getActivo())
            {
                cantidad_planes_accion_sin_verificar = LogicController.getCantidadPlanesAccionSinVerificar(id_centro, ces.getDiasAlerta());
                if (cantidad_planes_accion_sin_verificar < 0)
                {
                    return null;
                }
            }
            else
            {
                cantidad_planes_accion_sin_verificar = 0;
            }


            List<EmailMessage> listEmailResumenJefeCalidad = new List<EmailMessage>();

            if ((cantidad_acciones_inmediatas_pendiente < 1) && (cantidad_planes_accion_sin_cerrar < 1) && (cantidad_planes_accion_sin_verificar < 1))
            {
                return listEmailResumenJefeCalidad;
            }

            int dias = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;


            string body = "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                        + "a la fecha tiene las siguientes tareas pendientes:"
                        + "<br />"
                        + "<br />"
                        + "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                        + "     <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Resumen</b></td></tr>";
            if (cantidad_acciones_inmediatas_pendiente > 0)
            {
                body += "       <tr><td style=\"background-color:#FFCC00\"><b>Acciones inmediatas pendiente</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_acciones_inmediatas_pendiente) + "</b>&nbsp;&nbsp;</td></tr>";
            }

            if (cantidad_planes_accion_sin_cerrar > 0)
            {
                body += "       <tr><td style=\"background-color:#FFCC00\"><b>Planes de acción sin cerrar (100% ejecución)</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_planes_accion_sin_cerrar) + "</b>&nbsp;&nbsp;</td></tr>";
            }

            if (cantidad_planes_accion_sin_verificar > 0)
            {
                body += "       <tr><td style=\"background-color:#FFCC00\"><b>Planes de acción sin verificar</b></td><td>&nbsp;&nbsp;<b>" + Convert.ToString(cantidad_planes_accion_sin_verificar) + "</b>&nbsp;&nbsp;</td></tr>";
            }
            body += "</table>"
                 + "<br />"
                 + "<br />"
                 + "Quedan <b>" + Convert.ToString(dias) + " días</b> para cerrar el mes, por lo tanto, resuelva la situación a la brevedad.";

            string subject = "Resumen de tareas " + nombre_meses[DateTime.Now.Month - 1] + " " + Convert.ToString(DateTime.Now.Year);
            string header;
            string message;
            string footer = getFooter();
            EmailMessage em;
            foreach (EmailInfo einfo in listEmailInfoJefeCalidad)
            {
                header = getHeader(einfo);
                if (header == null)
                {
                    continue;
                }

                message = header + body + footer;

                em = new EmailMessage(einfo.getDireccion(), subject, message, null);
                listEmailResumenJefeCalidad.Add(em);
            }


            return listEmailResumenJefeCalidad;
        }


        public static bool sendMailAccionCorrectivanEliminada(string codigo_evento, AccionCorrectiva accion_correctiva)
        {
            if (!active)
                return true;

            if (accion_correctiva == null)
                return false;

            EmailInfo einfo = LogicController.getEmailInfo(accion_correctiva.getResponsable().getRut());
            if (einfo == null)
                return false;

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
                return false;


            string evento_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                                + "</table>";


            string accioncorrectiva_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Acción Correctiva</b></td></tr>"
                                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código Plan de Acción</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                                + " <tr><td style=\"background-color:#FFCC00\"><b>Tarea asignada</b></td><td><b>" + accion_correctiva.getDescripcion() + "</b></td></tr>"
                                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite</b></td><td><b>" + accion_correctiva.getFechaLimite() + "</b></td></tr>"
                                           + "</table>";


            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se ha eliminado la Acción Correctiva correspondiente al Plan de Acción con código <b>" + codigo_evento + "</b>"
                  + " y por tanto ya no debe realizarla"
                  + "<br />";


            body += "<br/><br />"
                  + accioncorrectiva_info
                  + "<br /><br />";


            body += "<br/><br />"
                  + evento_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Acción correctiva eliminada";

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }


        public static bool sendMailAccionCorrectivanActualizada(string codigo_evento, AccionCorrectiva accion_correctiva_pre, AccionCorrectiva accion_correctiva_post)
        {
            if (!active)
                return true;

            if (accion_correctiva_pre == null)
                return false;

            if (accion_correctiva_post == null)
                return false;

            EmailInfo einfo = LogicController.getEmailInfo(accion_correctiva_pre.getResponsable().getRut());
            if (einfo == null)
                return false;

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
                return false;


            string planaccion_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código Plan de Acción</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                                + "</table>";

            string accion_pre_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información previa</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Tarea previa</b></td><td><b>" + accion_correctiva_pre.getDescripcion() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite previa</b></td><td><b>" + accion_correctiva_pre.getFechaLimite() + "</b></td></tr>"
                                + "</table>";


            string accion_post_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Nueva información</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Nueva tarea asignada</b></td><td><b>" + accion_correctiva_post.getDescripcion() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Nueva fecha límite</b></td><td><b>" + accion_correctiva_post.getFechaLimite() + "</b></td></tr>"
                                + "</table>";

            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se ha modificado la Acción Correctiva asignada correspondiente al Plan de Acción con código <b>" + codigo_evento + "</b>"
                  + "<br />";

            body += "<br/><br />"
                  + accion_pre_info
                  + "<br /><br />";

            body += "<br/><br />"
                  + accion_post_info
                  + "<br /><br />";

            body += "<br/><br />"
                  + planaccion_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Acción correctiva asignada";

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }


        public static bool sendMailAccionCorrectivanAsignada(string codigo_evento, AccionCorrectiva accion_correctiva)
        {
            if (!active)
                return true;

            if (accion_correctiva == null)
                return false;

            EmailInfo einfo = LogicController.getEmailInfo(accion_correctiva.getResponsable().getRut());
            if (einfo == null)
                return false;

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
                return false;


            string evento_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                                + "</table>";


            string accioncorrectiva_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Acción Correctiva</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código Plan de Acción</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Tarea asignada</b></td><td><b>" + accion_correctiva.getDescripcion() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha límite</b></td><td><b>" + accion_correctiva.getFechaLimite() + "</b></td></tr>"
                                + "</table>";


            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se le ha asignado la responsabilidad de realizar una Acción Correctiva correspondiente al Plan de Acción con código <b>" + codigo_evento + "</b>"
                  + "<br />";

            body += "<br/><br />"
                  + accioncorrectiva_info
                  + "<br /><br />";


            body += "<br/><br />"
                  + evento_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Acción correctiva asignada";

            if (Convert.ToDateTime(accion_correctiva.getFechaLimite()) < DateTime.Now)
            {
                //Si la fecha límite es anterior a la fecha de hoy NO se envía el correo
                return true;
            }


            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }



        public static bool sendMailInvestigacionAsignadaEliminada(Investigacion investigacion)
        {
            if (!active)
                return true;

            if (investigacion == null)
                return false;

            EmailInfo einfo = LogicController.getEmailInfo(investigacion.getResponsable().getRut());
            if (einfo == null)
                return false;

            Evento evento = LogicController.getEvento(investigacion.getCodigoEvento());
            if (evento == null)
                return false;

            string evento_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                                + "</table>";

            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se ha eliminado la Investigación asignada correspondiente al evento con código <b>" + investigacion.getCodigoEvento() + "</b>";

            body += "<br/><br />"
                  + evento_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Investigación eliminada " + investigacion.getCodigoEvento();

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }


        public static bool sendMailInvestigacionAsignada(string id_centro, string rut, string codigo_evento, string fecha_inicio)
        {
            if (!active)
                return true;

            if (id_centro == null)
                return false;

            EmailInfo einfo = LogicController.getEmailInfo(rut);
            if (einfo == null)
                return false;

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
                return false;

            DateTime finicio;
            try
            {
                finicio = Convert.ToDateTime(fecha_inicio);
            }
            catch (Exception ex)
            {
                return false;
            }

            ConfigEmailSender ces = LogicController.getConfigEmailSender("Investigación en curso", "Usuario", id_centro);
            if (ces == null)
            {
                return false;
            }

            DateTime fvencimiento = finicio.AddDays(ces.getDiasLimite());

            string evento_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                                + "</table>";

            string investigacion_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información de la Investigación</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Vencimiento de Investigación</b></td><td><b>" + fvencimiento.ToShortDateString() + "</b></td></tr>"
                                + "</table>";

            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se le ha asignado la responsabilidad de realizar la Investigación correspondiente al evento con código <b>" + codigo_evento + "</b>"
                  + ", iniciada con fecha " + finicio.ToShortDateString() + ".<br />"
                  + "Recuerde que tiene un <b>plazo máximo de " + Convert.ToString(ces.getDiasLimite()) + " días</b> a partir de la fecha de inicio para reportar su ejecución.";

            body += "<br/><br />"
                  + investigacion_info
                  + "<br /><br />";


            body += "<br/><br />"
                  + evento_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Investigación asignada " + codigo_evento;

            if (fvencimiento < DateTime.Now)
            {
                //Si la fecha de vencimiento es anterior a la fecha de hoy NO se envía el correo
                return true;
            }

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }



        public static bool sendMailEventoCreado(string rut, string codigo_evento)
        {
            if (!active)
            {
                return true;
            }

            EmailInfo einfo = LogicController.getEmailInfo(rut);
            if (einfo == null)
            {
                return false;
            }

            if (einfo.getDireccion() == null)
            {
                return false;
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
                return false;

            string evento_info = "<table align=\"center\" border=\"1\" style=\"text-align:left\">"
                                + " <tr><td colspan=\"2\" style=\"background-color:#000000;color:#FFCC00;text-align:center;\"><b>Información del Evento</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Código</b></td><td><b>" + evento.getCodigo() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>W/O</b></td><td>" + evento.getWO() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Cliente</b></td><td>" + evento.getNombreCliente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Equipo</b></td><td>" + evento.getModeloEquipo() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Componente</b></td><td>" + evento.getNombreComponente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fuente</b></td><td>" + evento.getNombreFuente() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Clasificación</b></td><td>" + evento.getNombreClasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Sub-clasificación</b></td><td>" + evento.getNombreSubclasificacion() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Detalle</b></td><td style=\"color:#FF0000\">" + evento.getDetalle() + "</td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>IRC</b></td><td><b>" + evento.getIRC() + "</b></td></tr>"
                                + " <tr><td style=\"background-color:#FFCC00\"><b>Fecha de registro</b></td><td><b>" + DateTime.Now.ToShortDateString() + "</b></td></tr>"
                                + "</table>";

            string header = getHeader(einfo);
            if (header == null)
                return false;

            string body = header;

            body += "El Sistema de Gestión de Calidad (SGC) de Finning le informa que "
                  + "se ha registrado exitosamente el nuevo evento con código <b>" + codigo_evento + "</b>"
                  + " a la fecha " + DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString() + " horas.";

            body += "<br/><br />"
                  + evento_info
                  + "<br /><br />";

            body += getFooter();

            string subject = "Nuevo Evento registrado " + codigo_evento;

            EmailMessage em = new EmailMessage(einfo.getDireccion(), subject, body, null);
            EmailSender.addEmail(em);
            return true;
        }

        private static string getHeader(EmailInfo einfo)
        {
            string body = "";
            if (einfo.getSexo().Equals("M"))
                body += "Estimado ";
            else if (einfo.getSexo().Equals("F"))
                body += " Estimada ";
            else
                return null;

            body += einfo.getNombre() + ":<br /><br />";
            return body;
        }

        private static string getFooter()
        {
            string footer = "<br /><br />Por favor no responda este correo ya que no será leído.<br />Saludos cordiales,<br /><br />";

            footer += "<p>"
                   + "   <font face=\"Arial, Helvetica, sans-serif\" color=\"#666666\"><em style=\"font-size:12px; text-align:center;\">Sistema Gestión de Calidad (SGC)</font><br />"
                   + "   <font face=\"Arial, Helvetica, sans-serif\" color=\"#666666\"><em style=\"font-size:12px; text-align:center;\">Aseguramiento de Calidad y Mejoramiento continuo</font><br />"
                   + "   <font face=\"Arial, Helvetica, sans-serif\" color=\"#666666\"><em style=\"font-size:12px; text-align:center;\">Finning S.A</font>"
                   + "</p>";

            return footer;
        }



    }
}