using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using NCCSAN.Source.Entity;
using System.Web.UI.WebControls;
using NCCSAN.Source.Persistent;
using System.Data;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace NCCSAN.Source.Logic
{
    public class LogicController
    {
        public static Thread thEmailSender = null;


        public static int updateJpegFiles()
        {
            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return -1;
                }

                trans = DBController.getNewTransaction(conn);

                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                List<Archivo> listArchivos = new List<Archivo>();

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string extension;
                    string fileType;
                    string contentType;
                    byte[] contenido;
                    Archivo archivo;
                    while (sdr.Read())
                    {
                        contenido = (byte[])sdr[4];
                        extension = Utils.getFileExtension(sdr.GetString(1));
                        if (extension == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return -1;
                        }

                        fileType = Utils.getFileType(extension);
                        if (fileType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return -1;
                        }

                        contentType = Utils.getContentType(extension);
                        if (contentType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return -1;
                        }

                        archivo = new Archivo(sdr.GetString(0), sdr.GetString(1), contenido, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }

                sdr.Close();

                int cont = 0;
                foreach (Archivo archivo in listArchivos)
                {
                    Utils.convertImageFileToJpeg(archivo);

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return -1;
                    }
                    else
                    {
                        cont++;
                    }
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return -1;
                }

                return cont;
                
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return -1;
            }
        }


        public static string updateConfiguracionEmailCredential
        (
            Dictionary<string, string> listEmailCredential,
            string id_centro,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (listEmailCredential == null)
            {
                return "No se puede recuperar la credencial para actualizar";
            }
            else if (listEmailCredential.Count < 1)
            {
                return "La credencial para actualizar es inválida";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Error al conectar con la base de datos";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Error al conectar con la base de datos";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    DBController.doRollback(conn, trans);

                    return "Error al conectar con la base de datos";
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                foreach (KeyValuePair<string, string> pair in listEmailCredential)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("valor", SqlDbType.VarChar, 100).Value = pair.Value;
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + pair.Key;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Se ha producido un error al actualizar la configuración de correo";
                    }
                }


                if (!ActionLogger.administracion(cmd, id_centro, usuario, "Cambiar credencial de Correo", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al actualizar la configuración de correo";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al actualizar la configuración de correo";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }

                return "Se ha producido un error al actualizar la configuración de correo";
            }
        }



        public static Dictionary<string, string> getEmailCredential()
        {
            Dictionary<string, string> listEmailCredential = new Dictionary<string, string>();

            string email_parameter;
            string parameter_value;

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                SqlDataReader sdr;

                //SERVER
                {
                    email_parameter = "Server";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

                //DOMAIN
                {
                    email_parameter = "Domain";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

                //TIMEOUT
                {
                    email_parameter = "Timeout";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

                //USER
                {
                    email_parameter = "User";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

                //PASSWORD
                {
                    email_parameter = "Password";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

                //EMAIL
                {
                    email_parameter = "Email";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }


                //ACTIVE
                {
                    email_parameter = "Active";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("clave", SqlDbType.VarChar, 100).Value = "email_sender_" + email_parameter;

                    sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        parameter_value = sdr.GetString(0);

                        listEmailCredential.Add(email_parameter, parameter_value);
                        sdr.Close();
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

                conn.Close();

                return listEmailCredential;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static string updateConfiguracionMaxFileSize
        (
            Dictionary<string, int> listMaxFileSize,
            string id_centro,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (listMaxFileSize == null)
            {
                return "No se puede recuperar la configuración para actualizar";
            }
            else if (listMaxFileSize.Count < 1)
            {
                return "La configuración para actualizar es inválida";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Error al conectar con la base de datos";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Error al conectar con la base de datos";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    DBController.doRollback(conn, trans);

                    return "Error al conectar con la base de datos";
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                foreach (KeyValuePair<string, int> pair in listMaxFileSize)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("max_file_size", SqlDbType.VarChar, 100).Value = pair.Value;
                    cmd.Parameters.Add("file_type_group", SqlDbType.VarChar, 100).Value = "max_file_size_" + pair.Key;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Se ha producido un error al actualizar la configuración de Parámetros";
                    }
                }


                if (!ActionLogger.administracion(cmd, id_centro, usuario, "Cambiar configuración de Parámetros", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al actualizar la configuración de Parámetros";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al actualizar la configuración de Parámetros";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }

                return "Se ha producido un error al actualizar la configuración de Parámetros";
            }
        }


        public static Dictionary<string, int> getListMaxFileSize()
        {
            Dictionary<string, int> listMaxFileSize = new Dictionary<string, int>();

            string file_type_group;

            file_type_group = "Imagen";
            int max_file_size = LogicController.getMaxFileSizeByGroup(file_type_group);
            if (max_file_size < 0)
            {
                return null;
            }
            else
            {
                listMaxFileSize.Add(file_type_group, max_file_size);
            }


            file_type_group = "Documento";
            max_file_size = LogicController.getMaxFileSizeByGroup(file_type_group);
            if (max_file_size < 0)
            {
                return null;
            }
            else
            {
                listMaxFileSize.Add(file_type_group, max_file_size);
            }


            file_type_group = "Video";
            max_file_size = LogicController.getMaxFileSizeByGroup(file_type_group);
            if (max_file_size < 0)
            {
                return null;
            }
            else
            {
                listMaxFileSize.Add(file_type_group, max_file_size);
            }


            file_type_group = "Audio";
            max_file_size = LogicController.getMaxFileSizeByGroup(file_type_group);
            if (max_file_size < 0)
            {
                return null;
            }
            else
            {
                listMaxFileSize.Add(file_type_group, max_file_size);
            }


            file_type_group = "Diapositivas";
            max_file_size = LogicController.getMaxFileSizeByGroup(file_type_group);
            if (max_file_size < 0)
            {
                return null;
            }
            else
            {
                listMaxFileSize.Add(file_type_group, max_file_size);
            }


            file_type_group = "Comprimido";
            max_file_size = LogicController.getMaxFileSizeByGroup(file_type_group);
            if (max_file_size < 0)
            {
                return null;
            }
            else
            {
                listMaxFileSize.Add(file_type_group, max_file_size);
            }

            return listMaxFileSize;
        }




        public static int getMaxFileSizeByGroup(string file_type_group)
        {
            if (file_type_group == null)
            {
                return -1;
            }

            string max_file_size = LogicController.getConfiguracionValor("max_file_size_" + file_type_group);
            if (max_file_size == null)
            {
                return -1;
            }

            if (!Utils.validateNumber(max_file_size))
            {
                return -1;
            }


            return (Convert.ToInt32(max_file_size) * 1000000); //bytes (1 MB = 1000000 B)
        }



        public static int getMaxFileSizeByExtension(string extension)
        {

            if (extension == null)
            {
                return -1;
            }


            string file_type_group = Utils.getFileTypeGroup(extension);
            if (file_type_group == null)
            {
                return -1;
            }

            string max_file_size = LogicController.getConfiguracionValor("max_file_size_" + file_type_group);
            if (max_file_size == null)
            {
                return -1;
            }

            if (!Utils.validateNumber(max_file_size))
            {
                return -1;
            }


            return (Convert.ToInt32(max_file_size) * 1000000); //bytes (1 MB = 1000000 B)
        }


        public static List<Tarea> getListExpiredTasks(string id_centro, string nombre_rol, Dictionary<string, bool> flags)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {

                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();
                if (cmd == null)
                {
                    conn.Close();
                    return null;
                }



                List<Tarea> listExpiredTasks = new List<Tarea>();

                List<Tarea> listAuxExpiredTasks;

                if (flags["Investigación pendiente"] == true)
                {
                    //Investigación pendiente
                    listAuxExpiredTasks = LogicController.getListExpiredTaskInvestigacionPendiente(id_centro, nombre_rol, cmd);
                    if (listAuxExpiredTasks != null)
                    {
                        listExpiredTasks.AddRange(listAuxExpiredTasks);
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }
                }


                if (flags["Investigación en curso"] == true)
                {
                    //Investigación en curso
                    listAuxExpiredTasks = LogicController.getListExpiredTaskInvestigacionEnCurso(id_centro, nombre_rol, cmd);
                    if (listAuxExpiredTasks != null)
                    {
                        listExpiredTasks.AddRange(listAuxExpiredTasks);
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }
                }


                if (flags["Evaluación pendiente"] == true)
                {
                    //Evaluación pendiente
                    listAuxExpiredTasks = LogicController.getListExpiredTaskEvaluacionPendiente(id_centro, nombre_rol, cmd);
                    if (listAuxExpiredTasks != null)
                    {
                        listExpiredTasks.AddRange(listAuxExpiredTasks);
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }
                }


                if (flags["Plan de acción pendiente"] == true)
                {
                    //Plan de acción pendiente
                    listAuxExpiredTasks = LogicController.getListExpiredTaskPlanAccionPendiente(id_centro, nombre_rol, cmd);
                    if (listAuxExpiredTasks != null)
                    {
                        listExpiredTasks.AddRange(listAuxExpiredTasks);
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }
                }


                if (flags["Acción correctiva en curso"] == true)
                {
                    //Acción correctiva en curso
                    listAuxExpiredTasks = LogicController.getListExpiredTaskAccionCorrectivaEnCurso(id_centro, nombre_rol, cmd);
                    if (listAuxExpiredTasks != null)
                    {
                        listExpiredTasks.AddRange(listAuxExpiredTasks);
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }
                }


                if (flags["Verificación pendiente"] == true)
                {
                    //Verificación pendiente
                    listAuxExpiredTasks = LogicController.getListExpiredTaskVerificacionPendiente(id_centro, nombre_rol, cmd);
                    if (listAuxExpiredTasks != null)
                    {
                        listExpiredTasks.AddRange(listAuxExpiredTasks);
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }
                }


                conn.Close();

                return listExpiredTasks;

            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        private static List<Tarea> getListExpiredTaskVerificacionPendiente(string id_centro, string nombre_rol, SqlCommand cmd)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            if (cmd == null)
            {
                return null;
            }

            string estado = "Verificación pendiente";
            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;

                SqlDataReader sdr = cmd.ExecuteReader();
                int dias_alerta;
                if (sdr.HasRows)
                {
                    sdr.Read();
                    dias_alerta = sdr.GetInt32(0);

                    sdr.Close();
                }
                else
                {
                    sdr.Close();

                    return null;
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("dias_alerta", SqlDbType.Int).Value = dias_alerta;
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;

                sdr = cmd.ExecuteReader();

                List<Tarea> listExpiredTaskVerificacionPendiente = new List<Tarea>();
                if (sdr.HasRows)
                {
                    Tarea tarea;
                    while (sdr.Read())
                    {
                        tarea = new Tarea("PlanAccion", sdr.GetString(0), sdr.GetString(1), estado, sdr.GetDateTime(2).ToShortDateString(), sdr.GetInt32(3), null, null);
                        listExpiredTaskVerificacionPendiente.Add(tarea);
                    }
                }

                sdr.Close();

                return listExpiredTaskVerificacionPendiente;

            }
            catch (Exception ex)
            {
                return null;
            }
        }




        private static List<Tarea> getListExpiredTaskAccionCorrectivaEnCurso(string id_centro, string nombre_rol, SqlCommand cmd)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            if (cmd == null)
            {
                return null;
            }

            string estado = "Acción correctiva en curso";
            try
            {

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Plan de acción en curso";

                SqlDataReader sdr = cmd.ExecuteReader();

                List<Tarea> listExpiredTaskAccionCorrectivaEnCurso = new List<Tarea>();
                if (sdr.HasRows)
                {
                    Tarea tarea;
                    while (sdr.Read())
                    {
                        tarea = new Tarea("PlanAccion", sdr.GetString(0), sdr.GetString(1), estado, sdr.GetDateTime(2).ToShortDateString(), sdr.GetInt32(3), sdr.GetString(4), sdr.GetString(5));
                        listExpiredTaskAccionCorrectivaEnCurso.Add(tarea);
                    }
                }

                sdr.Close();

                return listExpiredTaskAccionCorrectivaEnCurso;

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        private static List<Tarea> getListExpiredTaskPlanAccionPendiente(string id_centro, string nombre_rol, SqlCommand cmd)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            if (cmd == null)
            {
                return null;
            }

            string estado = "Plan de acción pendiente";
            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;

                SqlDataReader sdr = cmd.ExecuteReader();
                int dias_limite;
                if (sdr.HasRows)
                {
                    sdr.Read();
                    dias_limite = sdr.GetInt32(0);

                    sdr.Close();
                }
                else
                {
                    sdr.Close();

                    return null;
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("dias_limite", SqlDbType.Int).Value = dias_limite;
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;

                sdr = cmd.ExecuteReader();

                List<Tarea> listExpiredTaskPlanAccionPendiente = new List<Tarea>();
                if (sdr.HasRows)
                {
                    Tarea tarea;
                    while (sdr.Read())
                    {
                        tarea = new Tarea("PlanAccion", sdr.GetString(0), sdr.GetString(1), estado, sdr.GetDateTime(2).ToShortDateString(), sdr.GetInt32(3), null, null);
                        listExpiredTaskPlanAccionPendiente.Add(tarea);
                    }
                }

                sdr.Close();

                return listExpiredTaskPlanAccionPendiente;

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        private static List<Tarea> getListExpiredTaskEvaluacionPendiente(string id_centro, string nombre_rol, SqlCommand cmd)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            if (cmd == null)
            {
                return null;
            }

            string estado = "Evaluación pendiente";
            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;

                SqlDataReader sdr = cmd.ExecuteReader();
                int dias_limite;
                if (sdr.HasRows)
                {
                    sdr.Read();
                    dias_limite = sdr.GetInt32(0);

                    sdr.Close();
                }
                else
                {
                    sdr.Close();

                    return null;
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("dias_limite", SqlDbType.Int).Value = dias_limite;
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;

                sdr = cmd.ExecuteReader();

                List<Tarea> listExpiredTaskIEvaluacionPendiente = new List<Tarea>();
                if (sdr.HasRows)
                {
                    Tarea tarea;
                    while (sdr.Read())
                    {
                        tarea = new Tarea("Evaluacion", sdr.GetString(0), sdr.GetString(1), estado, sdr.GetDateTime(2).ToShortDateString(), sdr.GetInt32(3), null, null);
                        listExpiredTaskIEvaluacionPendiente.Add(tarea);
                    }
                }

                sdr.Close();

                return listExpiredTaskIEvaluacionPendiente;

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        private static List<Tarea> getListExpiredTaskInvestigacionEnCurso(string id_centro, string nombre_rol, SqlCommand cmd)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            if (cmd == null)
            {
                return null;
            }

            string estado = "Investigación en curso";
            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;

                SqlDataReader sdr = cmd.ExecuteReader();
                int dias_limite;
                if (sdr.HasRows)
                {
                    sdr.Read();
                    dias_limite = sdr.GetInt32(0);

                    sdr.Close();
                }
                else
                {
                    sdr.Close();

                    return null;
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("dias_limite", SqlDbType.Int).Value = dias_limite;
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;

                sdr = cmd.ExecuteReader();

                List<Tarea> listExpiredTaskInvestigacionEnCurso = new List<Tarea>();
                if (sdr.HasRows)
                {
                    Tarea tarea;
                    while (sdr.Read())
                    {
                        tarea = new Tarea("Investigacion", sdr.GetString(0), sdr.GetString(1), estado, sdr.GetDateTime(2).ToShortDateString(), sdr.GetInt32(3), sdr.GetString(4), sdr.GetString(5));
                        listExpiredTaskInvestigacionEnCurso.Add(tarea);
                    }
                }

                sdr.Close();

                return listExpiredTaskInvestigacionEnCurso;

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        private static List<Tarea> getListExpiredTaskInvestigacionPendiente(string id_centro, string nombre_rol, SqlCommand cmd)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (nombre_rol == null)
            {
                return null;
            }

            if (cmd == null)
            {
                return null;
            }

            string estado = "Investigación pendiente";
            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;

                SqlDataReader sdr = cmd.ExecuteReader();
                int dias_limite;
                if (sdr.HasRows)
                {
                    sdr.Read();
                    dias_limite = sdr.GetInt32(0);

                    sdr.Close();
                }
                else
                {
                    sdr.Close();

                    return null;
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("dias_limite", SqlDbType.Int).Value = dias_limite;
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;

                sdr = cmd.ExecuteReader();

                List<Tarea> listExpiredTaskInvestigacionPendiente = new List<Tarea>();
                if (sdr.HasRows)
                {
                    Tarea tarea;
                    while (sdr.Read())
                    {
                        tarea = new Tarea("Investigacion", sdr.GetString(0), sdr.GetString(1), estado, sdr.GetDateTime(2).ToShortDateString(), sdr.GetInt32(3), null, null);
                        listExpiredTaskInvestigacionPendiente.Add(tarea);
                    }
                }

                sdr.Close();

                return listExpiredTaskInvestigacionPendiente;

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static string updateConfiguracionEmailSender
        (
            List<ConfigEmailSender> listConfigEmailSender,
            string id_centro,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (listConfigEmailSender == null)
            {
                return "No se puede recuperar la configuración para actualizar";
            }

            if (listConfigEmailSender.Count < 1)
            {
                return null;
            }

            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "No se puede conectar con la base de datos";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Error de conexión con la base de datos";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Error de conexión con la base de datos";
                }

                string activo;

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                foreach (ConfigEmailSender ces in listConfigEmailSender)
                {
                    if (ces.getActivo())
                    {
                        activo = "Si";
                    }
                    else
                    {
                        activo = "No";
                    }

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("dias_alerta", SqlDbType.Int).Value = ces.getDiasAlerta();
                    cmd.Parameters.Add("dias_limite", SqlDbType.Int).Value = ces.getDiasLimite();
                    cmd.Parameters.Add("dias_mensual", SqlDbType.Int).Value = ces.getDiasMensual();
                    cmd.Parameters.Add("activo", SqlDbType.VarChar, 2).Value = activo;
                    cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = ces.getEstado();
                    cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = ces.getNombreRol();
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = ces.getIDCentro();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Se ha producido un error al actualizar la configuración de Emails";
                    }
                }


                if (!ActionLogger.administracion(cmd, id_centro, usuario, "Cambiar configuración de Emails", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al actualizar la configuración de Emails";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al actualizar la configuración de Emails";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }

                return "Se ha producido un error al actualizar la configuración de Emails";
            }

        }



        public static bool setFechaUltimoTriggerEjecutado(string fecha)
        {
            if (setConfiguracionValor("ultimo_trigger", fecha))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static string getFechaUltimoTriggerEjecutado()
        {
            string fecha = getConfiguracionValor("ultimo_trigger");
            if (fecha == null)
            {
                return null;
            }

            return fecha;
        }



        public static List<Evento> getListEventosPlanAccionPendienteAlerta(string id_centro, int dias_desde, int dias_hasta)
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

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Plan de acción pendiente";
                cmd.Parameters.Add("dias_desde", SqlDbType.Int).Value = dias_desde;
                cmd.Parameters.Add("dias_hasta", SqlDbType.Int).Value = dias_hasta;

                SqlDataReader sdr = cmd.ExecuteReader();


                List<Evento> listEventosInvestigacionPendienteAlerta = new List<Evento>();

                if (sdr.HasRows)
                {
                    Evento evento;

                    while (sdr.Read())
                    {
                        evento = LogicController.getEvento(sdr.GetString(0));
                        if (evento != null)
                        {
                            listEventosInvestigacionPendienteAlerta.Add(evento);
                        }
                        else
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listEventosInvestigacionPendienteAlerta;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static List<Evento> getListEventosEvaluacionPendienteAlerta(string id_centro, int dias_desde, int dias_hasta)
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

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Evaluación pendiente";
                cmd.Parameters.Add("dias_desde", SqlDbType.Int).Value = dias_desde;
                cmd.Parameters.Add("dias_hasta", SqlDbType.Int).Value = dias_hasta;

                SqlDataReader sdr = cmd.ExecuteReader();


                List<Evento> listEventosEvaluacionPendienteAlerta = new List<Evento>();

                if (sdr.HasRows)
                {
                    Evento evento;

                    while (sdr.Read())
                    {
                        evento = LogicController.getEvento(sdr.GetString(0));
                        if (evento != null)
                        {
                            listEventosEvaluacionPendienteAlerta.Add(evento);
                        }
                        else
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listEventosEvaluacionPendienteAlerta;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }




        public static List<Evento> getListEventosInvestigacionEnCursoAlerta(string id_centro, int dias_desde, int dias_hasta)
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

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Investigación en curso";
                cmd.Parameters.Add("dias_desde", SqlDbType.Int).Value = dias_desde;
                cmd.Parameters.Add("dias_hasta", SqlDbType.Int).Value = dias_hasta;

                SqlDataReader sdr = cmd.ExecuteReader();


                List<Evento> listEventosInvestigacionEnCursoAlerta = new List<Evento>();

                if (sdr.HasRows)
                {
                    Evento evento;

                    while (sdr.Read())
                    {
                        evento = LogicController.getEvento(sdr.GetString(0));
                        if (evento != null)
                        {
                            listEventosInvestigacionEnCursoAlerta.Add(evento);
                        }
                        else
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listEventosInvestigacionEnCursoAlerta;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static List<Evento> getListEventosInvestigacionPendienteAlerta(string id_centro, int dias_desde, int dias_hasta)
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

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Investigación pendiente";
                cmd.Parameters.Add("dias_desde", SqlDbType.Int).Value = dias_desde;
                cmd.Parameters.Add("dias_hasta", SqlDbType.Int).Value = dias_hasta;

                SqlDataReader sdr = cmd.ExecuteReader();


                List<Evento> listEventosInvestigacionPendienteAlerta = new List<Evento>();

                if (sdr.HasRows)
                {
                    Evento evento;

                    while (sdr.Read())
                    {
                        evento = LogicController.getEvento(sdr.GetString(0));
                        if (evento != null)
                        {
                            listEventosInvestigacionPendienteAlerta.Add(evento);
                        }
                        else
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listEventosInvestigacionPendienteAlerta;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static List<AccionCorrectiva> getListAccionesCorrectivasAlerta(string id_centro, int dias_alerta)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (dias_alerta < 0)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("dias_alerta", SqlDbType.Int).Value = dias_alerta;

                SqlDataReader sdr = cmd.ExecuteReader();


                List<AccionCorrectiva> listAccionesCorrectivasAlerta = new List<AccionCorrectiva>();

                if (sdr.HasRows)
                {
                    AccionCorrectiva accion_correctiva;

                    while (sdr.Read())
                    {
                        accion_correctiva = LogicController.getAccionCorrectiva(sdr.GetString(0));
                        if (accion_correctiva != null)
                        {
                            listAccionesCorrectivasAlerta.Add(accion_correctiva);
                        }
                        else
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listAccionesCorrectivasAlerta;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static List<string> getListRUTInspectores(string id_centro)
        {
            if (id_centro == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = "Inspector";
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                SqlDataReader sdr = cmd.ExecuteReader();

                List<string> listRutInspectores = new List<string>();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        listRutInspectores.Add(sdr.GetString(0));
                    }
                }

                sdr.Close();
                conn.Close();

                return listRutInspectores;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static int getCantidadPlanesAccionSinVerificar(string id_centro, int dias_desde_cierre_plan_accion)
        {
            if (id_centro == null)
            {
                return -1;
            }

            if (dias_desde_cierre_plan_accion < 0)
            {
                return -1;
            }


            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return -1;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Verificación pendiente";
                cmd.Parameters.Add("dias_desde_cierre_plan_accion", SqlDbType.Int).Value = dias_desde_cierre_plan_accion;

                SqlDataReader sdr = cmd.ExecuteReader();

                int cantidad;
                if (sdr.HasRows)
                {
                    sdr.Read();

                    cantidad = sdr.GetInt32(0);
                }
                else
                {
                    cantidad = -1;
                }

                sdr.Close();
                conn.Close();

                return cantidad;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return -1;
            }
        }


        public static int getCantidadPlanesAccionSinCerrar(string id_centro) //con 100% ejecución
        {
            if (id_centro == null)
            {
                return -1;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return -1;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                SqlDataReader sdr = cmd.ExecuteReader();

                int cantidad;
                if (sdr.HasRows)
                {
                    sdr.Read();

                    cantidad = sdr.GetInt32(0);
                }
                else
                {
                    cantidad = -1;
                }

                sdr.Close();
                conn.Close();

                return cantidad;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return -1;
            }
        }


        public static int getCantidadAccionesInmediatasPendienteInspector(string id_centro, string rut_persona, int dias_desde, int dias_hasta)
        {
            if (id_centro == null)
            {
                return -1;
            }

            if (rut_persona == null)
            {
                return -1;
            }

            if (dias_desde < 0)
            {
                return -1;
            }

            if (dias_hasta < 0)
            {
                return -1;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return -1;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Acción inmediata pendiente";
                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut_persona;
                cmd.Parameters.Add("dias_desde", SqlDbType.Int).Value = dias_desde;
                cmd.Parameters.Add("dias_hasta", SqlDbType.Int).Value = dias_hasta;
                SqlDataReader sdr = cmd.ExecuteReader();

                int cantidad;
                if (sdr.HasRows)
                {
                    sdr.Read();

                    cantidad = sdr.GetInt32(0);
                }
                else
                {
                    cantidad = -1;
                }

                sdr.Close();
                conn.Close();

                return cantidad;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return -1;
            }
        }



        public static int getCantidadAccionesInmediatasPendiente(string id_centro)
        {
            if (id_centro == null)
            {
                return -1;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return -1;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Acción inmediata pendiente";

                SqlDataReader sdr = cmd.ExecuteReader();

                int cantidad;
                if (sdr.HasRows)
                {
                    sdr.Read();

                    cantidad = sdr.GetInt32(0);
                }
                else
                {
                    cantidad = -1;
                }

                sdr.Close();
                conn.Close();

                return cantidad;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return -1;
            }
        }



        public static List<EmailInfo> getListEmailInfoCoordinador(string id_centro)
        {
            if (id_centro == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = "Coordinador";

                SqlDataReader sdr = cmd.ExecuteReader();

                List<EmailInfo> listEmailInfoCoordinador = new List<EmailInfo>();
                if (sdr.HasRows)
                {
                    EmailInfo ei;

                    while (sdr.Read())
                    {
                        ei = new EmailInfo(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2));
                        listEmailInfoCoordinador.Add(ei);
                    }
                }

                sdr.Close();
                conn.Close();

                return listEmailInfoCoordinador;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static List<EmailInfo> getListEmailInfoJefeCalidad(string id_centro)
        {
            if (id_centro == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = "Jefe Calidad";

                SqlDataReader sdr = cmd.ExecuteReader();

                List<EmailInfo> listEmailInfoJefeCalidad = new List<EmailInfo>();
                if (sdr.HasRows)
                {
                    EmailInfo ei;

                    while (sdr.Read())
                    {
                        ei = new EmailInfo(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2));
                        listEmailInfoJefeCalidad.Add(ei);
                    }
                }

                sdr.Close();
                conn.Close();

                return listEmailInfoJefeCalidad;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }

        public static List<PersonaInfo> getListSupervisoresCentroArea(string id_centro, string nombre_area)
        {
            if (id_centro == null)
                return null;

            if (nombre_area == null)
                return null;


            if (centroAreaExists(id_centro, nombre_area) < 1)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                List<PersonaInfo> listSupervisores = new List<PersonaInfo>();

                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                SqlDataReader sdr_pi = cmd.ExecuteReader();


                PersonaInfo supervisor;
                if (sdr_pi.HasRows)
                {
                    while (sdr_pi.Read())
                    {
                        supervisor = LogicController.getPersonaInfo(sdr_pi.GetString(0));
                        if (supervisor != null)
                        {
                            listSupervisores.Add(supervisor);
                        }
                        else
                        {
                            sdr_pi.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr_pi.Close();

                conn.Close();

                return listSupervisores;
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static List<PersonaInfo> getListSupervisoresCentroAreaSubarea(string id_centro, string nombre_area, string nombre_subarea)
        {
            if (id_centro == null)
                return null;

            if (nombre_area == null)
                return null;

            if (nombre_subarea == null)
                return null;

            if (centroAreaSubareaExists(id_centro, nombre_area, nombre_subarea) < 1)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                List<PersonaInfo> listSupervisores = new List<PersonaInfo>();

                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                SqlDataReader sdr_pi = cmd.ExecuteReader();


                PersonaInfo supervisor;
                if (sdr_pi.HasRows)
                {
                    while (sdr_pi.Read())
                    {
                        supervisor = LogicController.getPersonaInfo(sdr_pi.GetString(0));
                        if (supervisor != null)
                        {
                            listSupervisores.Add(supervisor);
                        }
                        else
                        {
                            sdr_pi.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr_pi.Close();

                conn.Close();

                return listSupervisores;
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static string removeSupervisorCentroAreaSubarea(string rut, string id_centro, string nombre_area, string nombre_subarea, string usuario, string ip)
        {
            if (rut == null)
            {
                return "Supervisor inválido";
            }

            if (LogicController.getPersonaInfo(rut) == null)
            {
                return "No se puede recuperar información del Supervisor";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }


            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }

            if (nombre_subarea == null)
            {
                return "Nombre de sub-área inválido";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            int subarea_exists = subareaExists(nombre_subarea);
            if (subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }



            int centroarea_exists = centroAreaExists(id_centro, nombre_area);

            if (centroarea_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }
            else if (centroarea_exists == 0)
            {
                return "No se puede recuperar información del Área";
            }


            int centroareasubarea_exists = centroAreaSubareaExists(id_centro, nombre_area, nombre_subarea);

            if (centroareasubarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }
            else if (centroareasubarea_exists == 0)
            {
                return "No se puede recuperar información del Sub-área";
            }


            int supervisor_exists = LogicController.centroAreaSubareaSupervisorExists(id_centro, nombre_area, nombre_subarea, rut);
            if (supervisor_exists < 0)
            {
                return "No se puede recuperar información del Supervisor";
            }
            else if (supervisor_exists == 0)
            {
                return "No existe el Supervisor en el Sub-área indicado";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                    cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = rut;


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la eliminación del Supervisor";
                    }
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Supervisor";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación del Supervisor";
            }
        }



        public static string addSupervisorCentroAreaSubarea(string rut, string id_centro, string nombre_area, string nombre_subarea, string usuario, string ip)
        {
            if (rut == null)
            {
                return "Supervisor inválido";
            }

            if (LogicController.getPersonaInfo(rut) == null)
            {
                return "No se puede recuperar información del Supervisor";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }


            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }

            if (nombre_subarea == null)
            {
                return "Nombre de sub-área inválido";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            int subarea_exists = subareaExists(nombre_subarea);
            if (subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }



            int centroarea_exists = centroAreaExists(id_centro, nombre_area);

            if (centroarea_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }
            else if (centroarea_exists == 0)
            {
                return "No se puede recuperar información del Área";
            }


            int centroareasubarea_exists = centroAreaSubareaExists(id_centro, nombre_area, nombre_subarea);

            if (centroareasubarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }
            else if (centroareasubarea_exists == 0)
            {
                return "No se puede recuperar información del Sub-área";
            }


            int supervisor_exists = LogicController.centroAreaSubareaSupervisorExists(id_centro, nombre_area, nombre_subarea, rut);
            if (supervisor_exists < 0)
            {
                return "No se puede recuperar información del Supervisor";
            }
            else if (supervisor_exists == 1)
            {
                return "Ya existe el Supervisor en el Sub-área indicado";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                    cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = rut;


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del Supervisor";
                    }
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del Supervisor";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló el registro del Supervisor";
            }
        }



        public static string removeCentroAreaSubarea(string nombre_area, string nombre_subarea, string id_centro, string usuario, string ip)
        {
            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }

            if (nombre_subarea == null)
            {
                return "Nombre de sub-área inválido";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            int subarea_exists = subareaExists(nombre_subarea);
            if (subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }

            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }

            int centroarea_exists = centroAreaExists(id_centro, nombre_area);

            if (centroarea_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }
            else if (centroarea_exists == 0)
            {
                return "No se puede recuperar información del Área";
            }


            int centroareasubarea_exists = centroAreaSubareaExists(id_centro, nombre_area, nombre_subarea);

            if (centroareasubarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }
            else if (centroareasubarea_exists == 0)
            {
                return "No se puede recuperar información del Sub-área";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);


                int cantidad_eventos = getCantidadEventosCentroAreaSubarea(cmd, id_centro, nombre_area, nombre_subarea);
                if (cantidad_eventos < 0)
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la recuperación de información del Sub-área";
                }
                else if (cantidad_eventos > 0)
                {
                    DBController.doRollback(conn, trans);

                    return "No se puede eliminar un Sub-área con Eventos asociados";
                }


                //Eliminar CentroAreaSubarea
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                    cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la eliminación del Sub-área";
                    }
                }



                //Eliminar Subarea (si no está asociado a algun centro)
                {
                    int cantidad_centros = getCantidadAreasSubarea(cmd, nombre_subarea);
                    if (cantidad_centros < 0)
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la recuperación de información del Sub-área";
                    }
                    else if (cantidad_centros == 0)
                    {
                        cmd.Parameters.Clear();

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_subarea;

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la eliminación del Sub-área";
                        }

                    }
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Sub-área";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación del Sub-área";
            }
        }



        public static string updateCentroAreaSubarea(string previous_nombre_subarea, string nombre_subarea, string nombre_area, string id_centro, string usuario, string ip)
        {

            if (previous_nombre_subarea == null)
            {
                return "Nombre de sub-área inválido";
            }


            if (nombre_subarea == null)
            {
                return "Nombre de sub-área inválido";
            }


            if (nombre_area == null)
            {
                return "Nombre de Área inválido";
            }


            int previous_subarea_exists = subareaExists(previous_nombre_subarea);
            if (previous_subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }

            int subarea_exists = subareaExists(nombre_subarea);
            if (subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }


            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }


            int centro_area_subarea_exists = centroAreaSubareaExists(id_centro, nombre_area, nombre_subarea);
            if (centro_area_subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }
            else if (centro_area_subarea_exists == 1)
            {
                if (!nombre_subarea.ToUpper().Equals(previous_nombre_subarea.ToUpper()))
                {
                    return "El Sub-área indicado ya existe";
                }
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                int cant_areas_subarea = getCantidadAreasSubarea(cmd, previous_nombre_subarea);

                if (subarea_exists == 0)
                {

                    if (cant_areas_subarea == 1)
                    {
                        {
                            //Actualiza el nombre de Subarea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                            cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_subarea;
                            cmd.Parameters.Add("previous_nombre", SqlDbType.VarChar, 70).Value = previous_nombre_subarea;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Sub-área";
                            }
                        }
                    }
                    else if (cant_areas_subarea > 1)
                    {
                        {
                            //Ingresa el nuevo nombre de sub-área y lo actualizo
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                            cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_subarea;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Sub-área";
                            }
                        }

                        {//Actualiza el nombre de sub-área en CentroAreaSubarea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                            cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;
                            cmd.Parameters.Add("previous_nombre_subarea", SqlDbType.VarChar, 70).Value = previous_nombre_subarea;
                            cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Sub-área";
                            }
                        }
                    }
                }
                else if (subarea_exists == 1)
                {
                    if (cant_areas_subarea == 1)
                    {
                        {//Actualiza el nombre de sub-área en CentroAreaSubarea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                            cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;
                            cmd.Parameters.Add("previous_nombre_subarea", SqlDbType.VarChar, 70).Value = previous_nombre_subarea;
                            cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Sub-área";
                            }
                        }

                        if (!nombre_subarea.Equals(previous_nombre_subarea))
                        {//Elimina Subarea (ya no está en uso)
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                            cmd.Parameters.Add("previous_nombre_subarea", SqlDbType.VarChar, 70).Value = previous_nombre_subarea;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Sub-área";
                            }
                        }
                    }
                    else if (cant_areas_subarea > 1)
                    {
                        {//Actualiza el nombre de sub-área en CentroAreaSubarea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                            cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;
                            cmd.Parameters.Add("previous_nombre_subarea", SqlDbType.VarChar, 70).Value = previous_nombre_subarea;
                            cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Sub-área";
                            }
                        }
                    }
                }

                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de Sub-área";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la actualización de Sub-área";
            }
        }



        public static string registerCentroAreaSubarea(string nombre_area, string nombre_subarea, string id_centro, string usuario, string ip)
        {
            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }

            if (nombre_subarea == null)
            {
                return "Nombre de sub-área inválido";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            int subarea_exists = subareaExists(nombre_subarea);
            if (subarea_exists < 0)
            {
                return "No se puede recuperar información del Sub-área";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }

            int centroarea_exists = centroAreaExists(id_centro, nombre_area);

            if (centroarea_exists < 0)
            {
                return "No se puede recuperar información del área";
            }


            int centroareasubarea_exists = centroAreaSubareaExists(id_centro, nombre_area, nombre_subarea);

            if (centroareasubarea_exists < 0)
            {
                return "No se puede recuperar información del sub-área";
            }

            if (centroareasubarea_exists > 0)
            {
                return "Ya existe el sub-área";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                if (subarea_exists == 0)
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_subarea;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del nuevo Sub-área";
                    }
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", SqlDbType.VarChar, 70).Value = nombre_subarea;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Sub-área";
                }

                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Sub-área";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló el registro del nuevo Sub-área";
            }
        }



        public static PersonaInfo getCentroAreaJefe(string nombre_area, string id_centro)
        {

            if (nombre_area == null)
                return null;


            if (id_centro == null)
                return null;



            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string rut_jefe = sdr.GetString(0);
                    sdr.Close();
                    conn.Close();

                    return getPersonaInfo(rut_jefe);
                }
                else
                {
                    sdr.Close();
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static string removeCentroArea(string nombre_area, string id_centro, string usuario, string ip)
        {
            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }

            int centroarea_exists = centroAreaExists(id_centro, nombre_area);

            if (centroarea_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }
            else if (centroarea_exists == 0)
            {
                return "No se puede recuperar información del Área";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);


                int cantidad_eventos = getCantidadEventosCentroArea(cmd, id_centro, nombre_area);
                if (cantidad_eventos < 0)
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la recuperación de información del Área";
                }
                else if (cantidad_eventos > 0)
                {
                    DBController.doRollback(conn, trans);

                    return "No se puede eliminar un Área con Eventos asociados";
                }


                //Eliminar CentroArea
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la eliminación del Área";
                    }
                }



                //Eliminar Area (si no está asociado a algun centro)
                {
                    int cantidad_centros = getCantidadCentrosArea(cmd, nombre_area);
                    if (cantidad_centros < 0)
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la recuperación de información del Área";
                    }
                    else if (cantidad_centros == 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                

                        cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_area;

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la eliminación del Área";
                        }

                    }
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Área";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación del Área";
            }
        }



        public static string updateCentroArea(string previous_nombre_area, string nombre_area, string rut_jefe, string id_centro, string usuario, string ip)
        {

            if (previous_nombre_area == null)
            {
                return "Nombre de área inválido";
            }


            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }


            if (rut_jefe == null)
            {
                return "Jefe de Área inválido";
            }

            int previous_area_exists = areaExists(previous_nombre_area);
            if (previous_area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            if (personaExists(rut_jefe) < 1)
            {
                return "No se puede recuperar información del Jefe de Área";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }


            int centro_area_exists = centroAreaExists(id_centro, nombre_area);
            if (centro_area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }
            else if (centro_area_exists == 1)
            {
                if (!nombre_area.ToUpper().Equals(previous_nombre_area.ToUpper()))
                {
                    return "El Área indicado ya existe";
                }
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                int cant_centros_area = getCantidadCentrosArea(cmd, previous_nombre_area);

                if (area_exists == 0)
                {

                    if (cant_centros_area == 1)
                    {
                        {
                            //Actualiza el nombre de Área
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                            cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("previous_nombre", SqlDbType.VarChar, 70).Value = previous_nombre_area;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Área";
                            }
                        }
                    }
                    else if (cant_centros_area > 1)
                    {
                        {
                            //Ingresa el nuevo nombre de área y lo actualizo
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                            cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_area;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Área";
                            }
                        }

                        {//Actualiza el nombre de área en CentroArea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                            cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("rut_jefe", SqlDbType.VarChar, 15).Value = rut_jefe;
                            cmd.Parameters.Add("previous_nombre_area", SqlDbType.VarChar, 70).Value = previous_nombre_area;
                            cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Área";
                            }
                        }
                    }
                }
                else if (area_exists == 1)
                {
                    if (cant_centros_area == 1)
                    {
                        {//Actualiza el nombre de área en CentroArea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                            cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("rut_jefe", SqlDbType.VarChar, 15).Value = rut_jefe;
                            cmd.Parameters.Add("previous_nombre_area", SqlDbType.VarChar, 70).Value = previous_nombre_area;
                            cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Área";
                            }
                        }

                        if (!nombre_area.Equals(previous_nombre_area))
                        {//Elimina Área (ya no está en uso)
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                

                            cmd.Parameters.Add("previous_nombre_area", SqlDbType.VarChar, 70).Value = previous_nombre_area;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Área";
                            }
                        }
                    }
                    else if (cant_centros_area > 1)
                    {
                        {//Actualiza el nombre de área en CentroArea
                            cmd.Parameters.Clear();

                            cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                            cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                            cmd.Parameters.Add("rut_jefe", SqlDbType.VarChar, 15).Value = rut_jefe;
                            cmd.Parameters.Add("previous_nombre_area", SqlDbType.VarChar, 70).Value = previous_nombre_area;
                            cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                            if (!DBController.addQuery(cmd))
                            {
                                DBController.doRollback(conn, trans);

                                return "Falló la actualización de Área";
                            }
                        }
                    }
                }

                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de Área";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la actualización de Área";
            }
        }



        public static string registerCentroArea(string nombre_area, string rut_jefe, string id_centro, string usuario, string ip)
        {
            if (nombre_area == null)
            {
                return "Nombre de área inválido";
            }

            int area_exists = areaExists(nombre_area);
            if (area_exists < 0)
            {
                return "No se puede recuperar información del Área";
            }

            if (personaExists(rut_jefe) < 1)
            {
                return "No se puede recuperar información del Jefe de Área";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }

            int centroarea_exists = centroAreaExists(id_centro, nombre_area);

            if (centroarea_exists < 0)
            {
                return "No se puede recuperar información del área";
            }

            if (centroarea_exists > 0)
            {
                return "Ya existe el área";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                if (area_exists == 0)
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                    cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_area;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del nuevo Área";
                    }
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("rut_jefe", SqlDbType.VarChar, 15).Value = rut_jefe;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Área";
                }

                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Área";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló el registro del nuevo Área";
            }
        }



        public static string removeClientePersona(string nombre_cliente, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int exists = clienteExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (exists == 0)
            {
                return "No existe el cliente";
            }


            exists = clientePersonaExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del cliente";
            }
            else if (exists == 0)
            {
                return "No existe una cuenta asociada al cliente";
            }



            Persona persona = LogicController.getClientePersona(nombre_cliente);
            if (persona == null)
            {
                return "No se puede recuperar información de la persona";
            }


            string rut = persona.getRut();
            if (LogicController.hasPersonaUsuario(rut) > 0)
            {
                return "No se puede eliminar una Cuenta que tenga un Usuario activo";
            }

            if (LogicController.isPersonaJefeArea(rut) > 0)
            {
                return "No se puede eliminar una Cuenta que figure como jefe de área";
            }

            if (LogicController.isPersonaSupervisorSubarea(rut) > 0)
            {
                return "No se puede eliminar una Cuenta que figure como supervisor de sub-área";
            }

            if (LogicController.isPersonaSupervisorPersona(rut) > 0)
            {
                return "No se puede eliminar una Cuenta que figure como Supervisor de otro";
            }

            if (LogicController.isPersonaEventoCreador(rut) > 0)
            {
                return "No se puede eliminar Cuentas que han creado Eventos";
            }

            if (LogicController.isPersonaEventoJefeArea(rut) > 0)
            {
                return "No se puede eliminar Cuentas que figuran como jefe de área en Eventos";
            }

            if (LogicController.isPersonaEventoResponsable(rut) > 0)
            {
                return "No se puede eliminar Cuentas que figuran como responsable en Eventos";
            }

            if (LogicController.isPersonaEventoInvolucrado(rut) > 0)
            {
                return "No se puede eliminar Cuentas que figuren como involucradas en Eventos";
            }


            if (LogicController.hasPersonaAccionCorrectiva(rut) > 0)
            {
                return "No se puede eliminar Cuentas que tengan alguna Acción Correctiva asociada";
            }

            if (LogicController.hasPersonaEvaluacion(rut) > 0)
            {
                return "No se puede eliminar Cuentas que tengan alguna Evaluación de Evento asociada";
            }

            if (LogicController.hasPersonaInvestigacion(rut) > 0)
            {
                return "No se puede eliminar Cuentas que tengan alguna Investigación asociada";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();

                cmd.Parameters.Add("nombre_cliente", SqlDbType.VarChar, 70).Value = nombre_cliente;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación de la Cuenta";
                }



                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();
                cmd.Parameters.Add("rut", SqlDbType.VarChar, 15).Value = persona.getRut();


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación de la Cuenta";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Cuenta";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la actualización de la Cuenta";
            }
        }




        public static string updateClientePersona(string nombre_cliente, string email, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }


            int exists = clienteExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del cliente";
            }
            else if (exists == 0)
            {
                return "No existe el cliente";
            }


            if (email != null)
            {
                if (!Utils.validateEmail(email))
                {
                    return "Email invalido";
                }
            }


            exists = clientePersonaExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del cliente";
            }
            else if (exists == 0)
            {
                return "No existe una cuenta asociada al cliente";
            }


            Persona persona = LogicController.getClientePersona(nombre_cliente);
            if (persona == null)
            {
                return "No se puede recuperar información de la persona";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();
                cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                cmd.Parameters.Add("rut", SqlDbType.VarChar, 15).Value = persona.getRut();


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Cuenta";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Cuenta";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la actualización de la Cuenta";
            }
        }




        public static string registerClientePersona(string nombre_cliente, string email, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int cliente_exists = clienteExists(nombre_cliente);
            if (cliente_exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }


            string nombre = "Cliente " + nombre_cliente + ", Finning SGC";


            string rut;
            int persona_exists;

            //Generar un código (rut) único para la Cuenta (Persona)
            do
            {
                rut = Utils.getUniqueCode().Replace("-", "").Substring(0, 15);
                persona_exists = LogicController.personaExists(rut);

                if (persona_exists < 0)
                {
                    return "No se puede recuperar información de las Cuentas del sistema";
                }

            } while (persona_exists > 0);


            if (email != null)
            {
                if (!Utils.validateEmail(email))
                {
                    return "Email invalido";
                }
            }


            int clientepersona_exists = clientePersonaExists(nombre_cliente);
            if (clientepersona_exists < 0)
            {
                return "No se puede recuperar información del cliente";
            }
            else if (clientepersona_exists > 0)
            {
                return "Ya existe un cuenta para el cliente";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                {
                    //Registro de la Cuenta (Persona)

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("rut", SqlDbType.VarChar, 15).Value = rut;
                    cmd.Parameters.Add("nombre", SqlDbType.VarChar, 120).Value = nombre;
                    cmd.Parameters.Add("fecha_nacimiento", SqlDbType.DateTime).Value = DBNull.Value;
                    cmd.Parameters.Add("sexo", SqlDbType.VarChar, 1).Value = "M";
                    cmd.Parameters.Add("id_empleado", SqlDbType.VarChar, 30).Value = DBNull.Value;
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = "EXT";
                    cmd.Parameters.Add("id_store", SqlDbType.VarChar, 10).Value = DBNull.Value;
                    cmd.Parameters.Add("lob", SqlDbType.VarChar, 10).Value = DBNull.Value;
                    cmd.Parameters.Add("cargo", SqlDbType.VarChar, 70).Value = DBNull.Value;
                    cmd.Parameters.Add("nombre_clasificacionpersona", SqlDbType.VarChar, 30).Value = "Clientes";
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = DBNull.Value;
                    cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                    cmd.Parameters.Add("usuario", SqlDbType.VarChar, 50).Value = DBNull.Value;
                    cmd.Parameters.Add("fecha_ingreso", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    cmd.Parameters.Add("fecha_retiro", SqlDbType.DateTime).Value = DBNull.Value;


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar la Cuenta";
                    }
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();

                cmd.Parameters.Add("nombre_cliente", SqlDbType.VarChar, 70).Value = nombre_cliente;
                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Cuenta";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Cuenta";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló el registro de la Cuenta";
            }
        }




        public static string removeCentroCliente(string nombre_cliente, string id_centro, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int exists = clienteExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }

            if (id_centro == null)
            {
                return "Centro inválido";
            }

            exists = centroExists(id_centro);
            if (exists < 1)
            {
                return "No se puede recuperar información del Centro";
            }


            exists = centroClienteExists(id_centro, nombre_cliente);

            if (exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (exists == 0)
            {
                return "No se puede recuperar información del Cliente";
            }



            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);


                int cantidad_eventos = getCantidadEventosCentroCliente(cmd, id_centro, nombre_cliente);
                if (cantidad_eventos < 0)
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la recuperación de información del Cliente";
                }
                else if (cantidad_eventos > 0)
                {
                    DBController.doRollback(conn, trans);

                    return "No se puede eliminar un Cliente con Eventos asociados";
                }


                //Eliminar CentroCliente
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_cliente", SqlDbType.VarChar, 70).Value = nombre_cliente;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la eliminación del Cliente";
                    }
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Cliente";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación del Cliente";
            }
        }





        public static string updateCentroCliente(string nombre_cliente, string email, string id_centro, string usuario, string ip)
        {

            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int exists = clienteExists(nombre_cliente);
            if (exists < 1)
            {
                return "No se puede recuperar información del Cliente";
            }


            if (id_centro == null)
            {
                return "Centro inválido";
            }

            exists = centroExists(id_centro);
            if (exists < 1)
            {
                return "No se puede recuperar información del Centro";
            }


            int centrocliente_exists = centroClienteExists(id_centro, nombre_cliente);
            if (centrocliente_exists < 1)
            {
                return "No se puede recuperar información del Cliente";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);



                {//Actualiza el email del CentroCliente
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                    if (email != null)
                    {
                        cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                    }
                    else
                    {
                        cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = DBNull.Value;
                    }
                    cmd.Parameters.Add("nombre_cliente", SqlDbType.VarChar, 70).Value = nombre_cliente;
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización del Cliente";
                    }
                }




                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización del Cliente";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la actualización del Cliente";
            }
        }




        public static string registerCentroCliente(string nombre_cliente, string email, string id_centro, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int cliente_exists = clienteExists(nombre_cliente);
            if (cliente_exists < 1)
            {
                return "No se puede recuperar información del Cliente";
            }

            if (id_centro == null)
            {
                return "Centro inválido";
            }

            if (centroExists(id_centro) < 1)
            {
                return "No se puede recuperar información del Centro";
            }

            int centrocliente_exists = centroClienteExists(id_centro, nombre_cliente);

            if (centrocliente_exists < 0)
            {
                return "No se puede recuperar información del cliente";
            }
            else if (centrocliente_exists > 0)
            {
                return "Ya existe el cliente";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_cliente", SqlDbType.VarChar, 70).Value = nombre_cliente;
                if (email != null)
                {
                    cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                }
                else
                {
                    cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = DBNull.Value;
                }


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Cliente";
                }

                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Cliente";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló el registro del nuevo Cliente";
            }
        }




        public static string removeCliente(string nombre_cliente, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int exists = clienteExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }


            int has = hasClienteCentro(nombre_cliente);
            if (has < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (has > 0)
            {
                return "No se puede eliminar el Cliente mientras tenga un Centro asociado";
            }


            has = hasClienteEvento(nombre_cliente);
            if (has < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (has > 0)
            {
                return "No se puede eliminar el Cliente mientras tenga un Evento asociado";
            }


            exists = clientePersonaExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (exists > 0)
            {
                return "No se puede eliminar el Cliente mientras tenga una Cuenta asociada";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_cliente;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Cliente";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Cliente";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación del Cliente";
            }
        }




        public static string registerCliente(string nombre_cliente, string usuario, string ip)
        {
            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int cliente_exists = clienteExists(nombre_cliente);
            if (cliente_exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (cliente_exists > 0)
            {
                return "Ya existe el Cliente";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Add("nombre", SqlDbType.VarChar, 70).Value = nombre_cliente;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Cliente";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del nuevo Cliente";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló el registro del nuevo Cliente";
            }
        }




        public static string removeVerificacion
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Verificación";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Verificación pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Verificación";
                }



                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Verificación", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Verificación";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Verificación";
                }

                return null;

            }
            catch (Exception e)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }

                return "Fallo al eliminar la Verificación";
            }
        }



        public static string registerVerificacion
        (
            string codigo_evento,
            string efectivo,
            string observacion,
            string rut_responsable,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            if (planAccionExists(codigo_evento) < 1)
            {
                return "No se puede recuperar el Plan de Acción";
            }


            if (verificacionExists(codigo_evento) > 0)
            {
                return "El Evento ya tiene una Verificación asociada";
            }


            if ((efectivo == null) || ((!efectivo.Equals("Si")) && (!efectivo.Equals("No"))))
            {
                return "Efectividad inválida";
            }

            if (observacion == null)
            {
                return "Observación inválida";
            }

            PersonaInfo responsable = getPersonaInfo(rut_responsable);
            if (responsable == null)
            {
                return "Persona responsable inválida";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Fallo al recuperar la información del Evento";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_planaccion", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("efectivo", System.Data.SqlDbType.VarChar, 2).Value = efectivo;
                if (observacion.Length > 0)
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                }
                else
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                }
                cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = rut_responsable;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Verificación";
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el cierre del Evento";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Cerrado"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Verificación";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Verificar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Cerrar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Verificación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Verificación";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar la Verificación";
            }

        }


        public static List<Archivo> getArchivosAccionCorrectiva(string id_accion_correctiva)
        {
            if (id_accion_correctiva == null)
            {
                return null;
            }

            if (accionCorrectivaExists(id_accion_correctiva) < 1)
            {
                return null;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();
                
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Add("id_accioncorrectiva", SqlDbType.VarChar, 40).Value = id_accion_correctiva;

                List<Archivo> listArchivos = new List<Archivo>();

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string extension;
                    string fileType;
                    string contentType;
                    byte[] contenido;
                    Archivo archivo;
                    while (sdr.Read())
                    {
                        contenido = (byte[])sdr[4];
                        extension = Utils.getFileExtension(sdr.GetString(1));
                        if (extension == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        fileType = Utils.getFileType(extension);
                        if (fileType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        contentType = Utils.getContentType(extension);
                        if (contentType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        archivo = new Archivo(sdr.GetString(0), sdr.GetString(1), contenido, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }

                sdr.Close();
                conn.Close();

                return listArchivos;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static string registerAccionCorrectivaRealizada
        (
            string codigo_planaccion,
            string id_accion_correctiva,
            string fecha_limite,
            string fecha_realizado,
            string observacion,
            List<Archivo> listArchivos,
            string usuario,
            string ip
        )
        {

            if (codigo_planaccion == null)
            {
                return "Código de Plan de Acción inválido";
            }

            if (planAccionExists(codigo_planaccion) < 1)
            {
                return "Plan de Acción inválido";
            }


            if (id_accion_correctiva == null)
            {
                return "Acción Correctiva inválida";
            }

            if (accionCorrectivaExists(id_accion_correctiva) < 1)
            {
                return "Acción Correctiva inválida";
            }


            if (!Utils.validateFecha(fecha_limite))
            {
                return "Fecha límite inválida";
            }


            if (!Utils.validateFecha(fecha_realizado))
            {
                return "Fecha de ejecución inválida";
            }


            if (Convert.ToDateTime(fecha_realizado) > DateTime.Now)
            {
                return "La fecha de ejecución no puede ser futura";
            }


            string fecha_cierre_investigacion = LogicController.getFechaCierreInvestigacion(codigo_planaccion);
            if (fecha_cierre_investigacion == null)
            {
                return "Error al recuperar la fecha de la Investigación asociada al Plan de Acción";
            }


            if (Convert.ToDateTime(fecha_realizado) < Convert.ToDateTime(fecha_cierre_investigacion))
            {
                return "La fecha de ejecución no puede ser anterior al cierre de la Investigación";
            }


            /*Evita registrar ejecución de acciones correctivas despues de su fecha límite
            if (Convert.ToDateTime(fecha_realizado) > Convert.ToDateTime(fecha_limite))
            {
                return "La fecha de ejecución no puede ser posterior a la fecha límite";
            }
            */

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Clear();
                cmd.Parameters.Add("fecha_realizado", System.Data.SqlDbType.DateTime).Value = fecha_realizado;
                if (observacion.Length > 0)
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                }
                else
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                }

                cmd.Parameters.Add("codigo_planaccion", System.Data.SqlDbType.VarChar, 30).Value = codigo_planaccion;
                cmd.Parameters.Add("id_accion_correctiva", System.Data.SqlDbType.VarChar, 40).Value = id_accion_correctiva;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Acción Correctiva";
                }

                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {

                    //Filtra archivos de imagen y las convierte en PNG
                    {
                        Utils.convertImageFileToJpeg(archivo);
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_accion_correctiva", System.Data.SqlDbType.VarChar, 40).Value = id_accion_correctiva;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                    }

                }


                //Progreso
                string estado;
                {

                    int acciones_correctivas_realizadas = LogicController.getCantidadAccionesCorrectivasRealizadas(cmd, codigo_planaccion);
                    if (acciones_correctivas_realizadas < 0)
                    {
                        DBController.doRollback(conn, trans);

                        return "Error al recuperar las Acciones Correctivas del Plan de Acción";
                    }

                    int acciones_correctivas_total = LogicController.getCantidadAccionesCorrectivas(cmd, codigo_planaccion);
                    if (acciones_correctivas_total < 0)
                    {
                        DBController.doRollback(conn, trans);

                        return "Error al recuperar las Acciones Correctivas del Plan de Acción";
                    }

                    if (acciones_correctivas_realizadas < acciones_correctivas_total)
                    {
                        estado = "Plan de acción en curso";
                    }
                    else if (acciones_correctivas_realizadas == acciones_correctivas_total)
                    {
                        //estado = "Verificación pendiente";
                        estado = "Plan de acción en curso";
                    }
                    else
                    {
                        DBController.doRollback(conn, trans);

                        return "Error al calcular el progreso del Plan de Acción";
                    }

                    int progreso = (acciones_correctivas_realizadas * 100) / (acciones_correctivas_total);

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_planaccion;
                    cmd.Parameters.Add("progreso", SqlDbType.Int).Value = progreso;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al actualizar el progreso del Plan de Acción";
                    }
                }


                if (!setEstadoEvento(cmd, codigo_planaccion, estado))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Correctiva";
                }


                if (!ActionLogger.evento(cmd, codigo_planaccion, usuario, "Registrar Acción Correctiva ejecutada (id:" + id_accion_correctiva + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Correctiva";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Correctiva";
                }


                //MailSender.sendMailEventoCreado("david.espinoza@finning.cl", codigo);
                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar la Acción Correctiva";
            }

        }



        public static string openPlanAccion
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (LogicController.planAccionExists(codigo_evento) < 1)
            {
                return "Plan de Acción inválido";
            }


            int verificacion_exists = LogicController.verificacionExists(codigo_evento);
            if (verificacion_exists < 0)
            {
                return "No se puede recuperar la información del Plan de Acción";
            }


            if (verificacion_exists > 0)
            {
                return "No se puede abrir un Plan de Acción mientras tenga una Verificación asociada";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la apertura del Plan de Acción";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Plan de acción en curso"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la apertura del Plan de Acción";
                }



                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Abrir Plan de Acción", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la apertura del Plan de Acción";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la apertura del Plan de Acción";
                }


                return null;
            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }


                return "Falló la apertura del Plan de Acción";
            }

        }



        public static string removePlanAccion
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }



            List<AccionCorrectiva> listAccionesCorrectivas = LogicController.getListAccionesCorrectivas(codigo_evento);

            if (listAccionesCorrectivas == null)
            {
                return "Acciones correctivas inválidas";
            }


            List<AccionCorrectiva> listEmailAccionesCorrectivasEliminadas = new List<AccionCorrectiva>();
            foreach (AccionCorrectiva ac in listAccionesCorrectivas)
            {
                //Si se eliminará una acción correctiva que no se ha realizado
                if (ac.getFechaRealizado() == null)
                {
                    listEmailAccionesCorrectivasEliminadas.Add(ac);
                }
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }



                { 	//Acciones correctivas eliminadas
                    foreach (AccionCorrectiva ac in listAccionesCorrectivas)
                    {
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id_accion_correctiva", SqlDbType.VarChar, 40).Value = ac.getIdAccionCorrectiva();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la eliminación de archivos de la Accion Correctiva";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la eliminación de la Accion Correctiva";
                        }


                        if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Acción Correctiva (id:" + ac.getIdAccionCorrectiva() + ")", ip))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la eliminación de la Accion Correctiva";
                        }
                    }
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Plan de Acción";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Plan de acción pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Plan de Acción";
                }

                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Plan de Acción", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Plan de Acción";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Plan de Acción";
                }


                //Envío de correo a responsables
                {

                    foreach (AccionCorrectiva accion_correctiva in listEmailAccionesCorrectivasEliminadas)
                    {
                        EmailSender.sendMailAccionCorrectivanEliminada(codigo_evento, accion_correctiva);
                    }
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al eliminar el Plan de Acción";
            }

        }



        public static string closePlanAccion
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (LogicController.planAccionExists(codigo_evento) < 1)
            {
                return "Plan de Acción inválido";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el cierre del Plan de Acción";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Verificación pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el cierre del Plan de Acción";
                }



                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Cerrar Plan de Acción", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el cierre del Plan de Acción";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el cierre del Plan de Acción";
                }


                return null;
            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }


                return "Falló el cierre del Plan de Acción";
            }

        }



        public static string updatePlanAccion
        (
            string codigo_evento,
            string detalle_correccion,
            string fecha_correccion,
            List<AccionCorrectiva> listAccionesCorrectivas,
            List<string> listIDRemovedAccionesCorrectivas,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }


            if ((detalle_correccion == null) || (detalle_correccion.Length < 1))
            {
                return "Detalle de corrección inmediata inválida";
            }

            if ((fecha_correccion == null) || (fecha_correccion.Length < 1) || (!Utils.validateFecha(fecha_correccion)))
            {
                return "Fecha de corrección inmediata inválida";
            }


            if (LogicController.evaluacionExists(codigo_evento) < 1)
            {
                return "No se puede recuperar la información de la Evaluación asociada al Evento";
            }


            string fecha_cierre_investigacion = LogicController.getFechaCierreInvestigacion(codigo_evento);
            if (fecha_cierre_investigacion == null)
            {
                return "No se puede recuperar la fecha de la Investigación asociada al Evento";
            }

            if (Convert.ToDateTime(fecha_correccion) > Convert.ToDateTime(fecha_cierre_investigacion))
            {
                return "La fecha de corrección inmediata no puede posterior a la Investigación asociada al Evento";
            }

            if (Convert.ToDateTime(fecha_correccion) < evento.getFecha())
            {
                return "La fecha de la corrección inmediata no puede ser anterior a la fecha en que ocurrió el Evento";
            }


            if (listAccionesCorrectivas == null)
            {
                return "Acciones correctivas inválidas";
            }


            if (listAccionesCorrectivas.Count < 1)
            {
                return "El plan de acción no contiene acciones correctivas";
            }


            if (listIDRemovedAccionesCorrectivas == null)
            {
                return "Actualización de acciones correctivas inválida";
            }

            AccionCorrectiva ac;
            List<AccionCorrectiva> listEmailAccionesCorrectivasEliminadas = new List<AccionCorrectiva>();
            foreach (string id_accioncorrectiva in listIDRemovedAccionesCorrectivas)
            {
                ac = LogicController.getAccionCorrectiva(id_accioncorrectiva);
                if (ac == null)
                {
                    return "Error al recuperar información de las acciones correctivas que se eliminarán";
                }

                //Si se eliminará una acción correctiva que no se ha realizado
                if (ac.getFechaRealizado() == null)
                {
                    listEmailAccionesCorrectivasEliminadas.Add(ac);
                }
            }

            List<AccionCorrectiva> listEmailAccionesCorrectivasIngresadas = new List<AccionCorrectiva>();
            List<AccionCorrectiva> listEmailAccionesCorrectivasActualizadas_pre = new List<AccionCorrectiva>();
            List<AccionCorrectiva> listEmailAccionesCorrectivasActualizadas_post = new List<AccionCorrectiva>();
            foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivas)
            {
                if (accion_correctiva.getIdAccionCorrectiva() == null)
                {
                    //Si es una nueva acción correctiva
                    listEmailAccionesCorrectivasIngresadas.Add(accion_correctiva);
                }
                else
                {
                    ac = LogicController.getAccionCorrectiva(accion_correctiva.getIdAccionCorrectiva());
                    if (ac == null)
                    {
                        return "Error al recuperar información de las acciones correctivas que se actualizarán";
                    }

                    if (accion_correctiva.getResponsable().getRut().Equals(ac.getResponsable().getRut()))
                    {
                        //Si es actualización (mismo responsable)
                        if ((!accion_correctiva.getDescripcion().Equals(ac.getDescripcion())) || (!accion_correctiva.getFechaLimite().Equals(ac.getFechaLimite())))
                        {
                            //Si la acción correctiva sufre cambios
                            listEmailAccionesCorrectivasActualizadas_pre.Add(ac);
                            listEmailAccionesCorrectivasActualizadas_post.Add(accion_correctiva);
                        }
                    }
                    else
                    {
                        //Si es otro responsable se le informa la nueva asignación
                        listEmailAccionesCorrectivasIngresadas.Add(accion_correctiva);
                    }
                }
            }


            int acciones_correctivas_total = 0;
            int acciones_correctivas_realizadas = 0;

            foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivas)
            {
                if (accion_correctiva.getFechaRealizado() != null)
                {
                    acciones_correctivas_realizadas++;
                }

                acciones_correctivas_total++;
            }

            int progreso = (acciones_correctivas_realizadas * 100) / (acciones_correctivas_total);


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */



                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("detalle_correccion", System.Data.SqlDbType.VarChar, 3000).Value = detalle_correccion;
                cmd.Parameters.Add("fecha_correccion", System.Data.SqlDbType.DateTime).Value = fecha_correccion;
                cmd.Parameters.Add("progreso", SqlDbType.Int).Value = progreso;
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DBNull.Value;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del Plan de Acción";
                }


                { 	//Acciones correctivas eliminadas
                    foreach (string id_accion_correctiva in listIDRemovedAccionesCorrectivas)
                    {
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id_accion_correctiva", SqlDbType.VarChar, 40).Value = id_accion_correctiva;

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la actualización de la Acción Correctiva";
                        }


                        if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Acción Correctiva (id:" + id_accion_correctiva + ")", ip))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la actualización del Plan de Acción";
                        }
                    }
                }

                string id_new_accion_correctiva;
                //Acciones correctivas
                foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivas)
                {
                    if (accion_correctiva.getIdAccionCorrectiva() != null)
                    {
                        //Update AccionCorrectiva
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = accion_correctiva.getIdAccionCorrectiva();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("descripcion", System.Data.SqlDbType.VarChar, 3000).Value = accion_correctiva.getDescripcion();
                        cmd.Parameters.Add("fecha_limite", System.Data.SqlDbType.DateTime).Value = accion_correctiva.getFechaLimite();
                        cmd.Parameters.Add("fecha_realizado", System.Data.SqlDbType.DateTime).Value = DBNull.Value;
                        cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = accion_correctiva.getResponsable().getRut();
                        if (accion_correctiva.getObservacion() != null)
                        {
                            cmd.Parameters.Add("observacion", SqlDbType.VarChar, 3000).Value = accion_correctiva.getObservacion();
                        }
                        else
                        {
                            cmd.Parameters.Add("observacion", SqlDbType.VarChar, 3000).Value = DBNull.Value;
                        }

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la actualización de la acción correctiva \"" + accion_correctiva.getDescripcion() + "\"";
                        }


                    }
                    else if (accion_correctiva.getIdAccionCorrectiva() == null)
                    {
                        //Insert AccionCorrectiva

                        id_new_accion_correctiva = Utils.getUniqueCode();

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = id_new_accion_correctiva;
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("descripcion", System.Data.SqlDbType.VarChar, 3000).Value = accion_correctiva.getDescripcion();
                        cmd.Parameters.Add("fecha_limite", System.Data.SqlDbType.DateTime).Value = accion_correctiva.getFechaLimite();
                        cmd.Parameters.Add("fecha_realizado", System.Data.SqlDbType.DateTime).Value = DBNull.Value;
                        cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = accion_correctiva.getResponsable().getRut();
                        cmd.Parameters.Add("observacion", SqlDbType.VarChar, 3000).Value = DBNull.Value;

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro de la acción correctiva \"" + accion_correctiva.getDescripcion() + "\"";
                        }

                        if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Agregar Acción Correctiva (id:" + id_new_accion_correctiva + ")", ip))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló la actualización del Plan de Acción";
                        }
                    }

                }


                if (!setEstadoEvento(cmd, codigo_evento, "Plan de acción en curso"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización del Plan de Acción";
                }

                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Editar Plan de Acción", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización del Plan de Acción";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización del Plan de Acción";
                }


                //Envío de correo a responsables
                {
                    foreach (AccionCorrectiva accion_correctiva in listEmailAccionesCorrectivasIngresadas)
                    {
                        EmailSender.sendMailAccionCorrectivanAsignada(codigo_evento, accion_correctiva);
                    }


                    for (int i = 0; i < listEmailAccionesCorrectivasActualizadas_pre.Count; i++)
                    {
                        EmailSender.sendMailAccionCorrectivanActualizada(codigo_evento, listEmailAccionesCorrectivasActualizadas_pre[i], listEmailAccionesCorrectivasActualizadas_post[i]);
                    }


                    foreach (AccionCorrectiva accion_correctiva in listEmailAccionesCorrectivasEliminadas)
                    {
                        EmailSender.sendMailAccionCorrectivanEliminada(codigo_evento, accion_correctiva);
                    }
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al actualizar el Plan de Acción";
            }

        }


        public static List<AccionCorrectiva> getListAccionesCorrectivas(string codigo_planaccion)
        {

            if (codigo_planaccion == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                cmd.Parameters.Add("codigo_planaccion", SqlDbType.VarChar, 30).Value = codigo_planaccion;

                SqlDataReader sdr = cmd.ExecuteReader();

                List<AccionCorrectiva> listAccionesCorrectivas = new List<AccionCorrectiva>();
                if (sdr.HasRows)
                {
                    AccionCorrectiva accion_correctiva;
                    while (sdr.Read())
                    {
                        accion_correctiva = LogicController.getAccionCorrectiva(sdr.GetString(0));
                        if (accion_correctiva != null)
                        {
                            listAccionesCorrectivas.Add(accion_correctiva);
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listAccionesCorrectivas;

            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }

        }


        public static string registerPlanAccion
        (
            string codigo_evento,
            string detalle_correccion,
            string fecha_correccion,
            List<AccionCorrectiva> listAccionesCorrectivas,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }

            if (planAccionExists(codigo_evento) > 0)
            {
                return "El Evento ya tiene un Plan de Acción asociado";
            }


            if ((detalle_correccion == null) || (detalle_correccion.Length < 1))
            {
                return "Detalle de corrección inmediata inválida";
            }

            if ((fecha_correccion == null) || (fecha_correccion.Length < 1) || (!Utils.validateFecha(fecha_correccion)))
            {
                return "Fecha de corrección inmediata inválida";
            }


            if (LogicController.evaluacionExists(codigo_evento) < 1)
            {
                return "No se puede recuperar la información de la Evaluación asociada al Evento";
            }


            string fecha_cierre_investigacion = LogicController.getFechaCierreInvestigacion(codigo_evento);
            if (fecha_cierre_investigacion == null)
            {
                return "No se puede recuperar la Investigación asociada al Evento";
            }

            if (Convert.ToDateTime(fecha_correccion) > Convert.ToDateTime(fecha_cierre_investigacion))
            {
                return "La fecha de corrección inmediata no puede posterior a la Investigación asociada al Evento";
            }

            if (Convert.ToDateTime(fecha_correccion) < evento.getFecha())
            {
                return "La fecha de la corrección inmediata no puede ser anterior a la fecha en que ocurrió el Evento";
            }


            if (listAccionesCorrectivas == null)
            {
                return "Acciones correctivas inválidas";
            }


            if (listAccionesCorrectivas.Count < 1)
            {
                return "El plan de acción no contiene acciones correctivas";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("detalle_correccion", System.Data.SqlDbType.VarChar, 3000).Value = detalle_correccion;
                cmd.Parameters.Add("fecha_correccion", System.Data.SqlDbType.DateTime).Value = fecha_correccion;
                cmd.Parameters.Add("progreso", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DBNull.Value;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del Plan de Acción";
                }

                //Acciones correctivas
                foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivas)
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = Utils.getUniqueCode();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("descripcion", System.Data.SqlDbType.VarChar, 3000).Value = accion_correctiva.getDescripcion();
                    cmd.Parameters.Add("fecha_limite", System.Data.SqlDbType.DateTime).Value = accion_correctiva.getFechaLimite();
                    cmd.Parameters.Add("fecha_realizado", System.Data.SqlDbType.DateTime).Value = DBNull.Value;
                    cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = accion_correctiva.getResponsable().getRut();
                    cmd.Parameters.Add("observacion", SqlDbType.VarChar, 3000).Value = DBNull.Value;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro de la acción correctiva \"" + accion_correctiva.getDescripcion() + "\"";
                    }

                }



                if (!setEstadoEvento(cmd, codigo_evento, "Plan de acción en curso"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del Plan de Acción";
                }

                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Registrar Plan de Acción", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del Plan de Acción";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro del Plan de Acción";
                }


                foreach (AccionCorrectiva accion_correctiva in listAccionesCorrectivas)
                {
                    EmailSender.sendMailAccionCorrectivanAsignada(codigo_evento, accion_correctiva);
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el Plan de Acción";
            }

        }



        public static string removeEvaluacion
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
                return null;


            if (accionInmediataExists(codigo_evento) > 0)
            {
                return "No se puede eliminar la Evaluación mientras tenga una Acción Inmediata asociada";
            }


            if (planAccionExists(codigo_evento) > 0)
            {
                return "No se puede eliminar la Evaluación mientras tenga un Plan de Acción asociado";
            }

            List<Archivo> listArchivosEvaluacion = LogicController.getArchivosEvaluacion(codigo_evento);
            if (listArchivosEvaluacion == null)
            {
                return "No se pueden recuperar los archivos asociados a la Evaluación";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Evaluación";
                }



                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                cmd.Parameters.Clear();
                cmd.Parameters.Add("fecha_respuesta", SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Evaluación";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Evaluación pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Evaluación";
                }



                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Evaluación", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Evaluación";
                }



                //Eliminar Responsable en Evento
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Responsable";

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar el supervisor del Evento";
                    }
                }


                //Eliminar Involucrados en Evento
                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Involucrado";

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar los involucrados del Evento";
                    }
                }


                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                foreach (Archivo archivo in listArchivosEvaluacion)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_archivo", SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar los archivos asociados a la Evaluación";
                    }
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Evaluación";
                }

                return null;

            }
            catch (Exception e)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }

                return "Fallo al eliminar la Evaluación";
            }
        }



        public static List<Archivo> getArchivosEvaluacion(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return null;
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return null;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                List<Archivo> listArchivos = new List<Archivo>();

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string extension;
                    string fileType;
                    string contentType;
                    byte[] contenido;
                    Archivo archivo;
                    while (sdr.Read())
                    {
                        contenido = (byte[])sdr[4];
                        extension = Utils.getFileExtension(sdr.GetString(1));
                        if (extension == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        fileType = Utils.getFileType(extension);
                        if (fileType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        contentType = Utils.getContentType(extension);
                        if (contentType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        archivo = new Archivo(sdr.GetString(0), sdr.GetString(1), contenido, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }

                sdr.Close();
                conn.Close();

                return listArchivos;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }




        public static string registerEvaluacion
        (
            string codigo_evento,
            string nombre_origen,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string nombre_causainmediata,
            string nombre_subcausainmediata,
            string nombre_causabasica,
            string nombre_subcausabasica,
            string rut_responsable_evento,
            string aceptado,
            string observacion,
            string rut_responsable_evaluacion,
            List<Archivo> listArchivos,
            List<PersonaInfo> listInvolucrados,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            if (investigacionExists(codigo_evento) < 1)
            {
                return "No se puede recuperar la Investigación";
            }


            if (evaluacionExists(codigo_evento) > 0)
            {
                return "El Evento ya tiene una Evaluación asociada";
            }

            if (nombre_origen == null)
            {
                return "Origen de la falla inválida";
            }


            if (rut_responsable_evento == null)
            {
                return "Responsable del Evento inválido";
            }

            PersonaInfo responsable_evento = LogicController.getPersonaInfo(rut_responsable_evento);
            if (responsable_evento == null)
            {
                return "No se puede recuperar la información del Responsable del Evento";
            }




            if (nombre_clasificacion == null)
            {
                return "Clasificación de la falla inválida";
            }
            else if (nombre_clasificacion.Equals("*Sin especificar"))
            {
                return "Se debe especificar la clasificación de la falla";
            }


            if (nombre_subclasificacion == null)
            {
                return "Sub-clasificación de la falla inválida";
            }
            else if (nombre_subclasificacion.Equals("*Sin especificar"))
            {
                return "Se debe especificar la sub-clasificación de la falla";
            }


            if (nombre_causainmediata == null)
            {
                return "Causa inmediata inválida";
            }

            if (nombre_subcausainmediata == null)
            {
                return "Sub-causa inmediata inválida";
            }

            if (nombre_causabasica == null)
            {
                return "Causa básica inválida";
            }

            if (nombre_subcausabasica == null)
            {
                return "Sub-causa básica inválida";
            }

            if ((aceptado == null) || ((!aceptado.Equals("Si")) && (!aceptado.Equals("No"))))
            {
                return "Aceptación inválida";
            }

            if (observacion == null)
            {
                return "Observación inválida";
            }

            PersonaInfo responsable_evaluacion = getPersonaInfo(rut_responsable_evaluacion);
            if (responsable_evaluacion == null)
            {
                return "Persona responsable de Evaluación inválida";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            if (listInvolucrados == null)
            {
                return "Personas involucradas inválidas";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Fallo al recuperar la información del Evento";
            }

            string estado;
            if (aceptado.Equals("Si"))
            {
                if (evento.getIRC() < 10)
                {
                    estado = "Acción inmediata pendiente";
                }
                else
                {
                    estado = "Plan de acción pendiente";
                }
            }
            else
            {
                estado = "Cerrado";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                {//Clasificación del Evento

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                    cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización de clasificación del Evento";
                    }

                }


                {//Evaluación
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("nombre_origen", System.Data.SqlDbType.VarChar, 30).Value = nombre_origen;
                    cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                    cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                    cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                    cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;
                    cmd.Parameters.Add("aceptado", System.Data.SqlDbType.VarChar, 2).Value = aceptado;
                    if (observacion.Length > 0)
                    {
                        cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                    }
                    else
                    {
                        cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                    }
                    cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = rut_responsable_evaluacion;
                    cmd.Parameters.Add("antiguedad_responsable", System.Data.SqlDbType.Int).Value = responsable_evaluacion.getAntiguedad();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro de la la Evaluación";
                    }


                    //Eliminar/Ingresar Responsable en Evento
                    {

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                        
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del responsable del Evento";
                        }

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = responsable_evento.getRut();
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = responsable_evento.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del responsable del Evento";
                        }

                    }


                    //Ingresar Involucrados
                    foreach (PersonaInfo involucrado in listInvolucrados)
                    {

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = involucrado.getRut();
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = involucrado.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del involucrado \"" + involucrado.getNombre() + "\"";
                        }

                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("fecha_respuesta", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro de la la Evaluación";
                    }


                    //Ingresar Archivos
                    foreach (Archivo archivo in listArchivos)
                    {
                        //Filtra archivos de imagen y las convierte en PNG
                        {
                            Utils.convertImageFileToJpeg(archivo);
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                        cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                        cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                        cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                        cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }

                    }
                }


                if (!setEstadoEvento(cmd, codigo_evento, estado))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Evaluación";
                }


                if (estado.Equals("Cerrado"))
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el cierre del Evento";
                    }
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Evaluar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Evaluación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la la Evaluación";
                }


                //MailSender.sendMailEventoCreado("david.espinoza@finning.cl", codigo);
                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar la Evaluación";
            }

        }


        public static string removeInvestigacionCerrada
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            List<Archivo> listArchivosInvestigacion = LogicController.getArchivosInvestigacion(codigo_evento);
            if (listArchivosInvestigacion == null)
            {
                return "No se pueden recuperar los archivos asociados a la Investigación";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DBNull.Value;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al abrir la Investigación";
                }


                cmd.Parameters.Clear();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                foreach (Archivo archivo in listArchivosInvestigacion)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_archivo", SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar los archivos asociados a la Investigación";
                    }
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Investigación en curso"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al abrir la Investigación";
                }

                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Abrir la Investigación", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al abrir la Investigación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al abrir la Investigación";
                }

                //MailSender.sendMailEventoCreado("david.espinoza@finning.cl", codigo);
                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al abrir la Investigación";
            }
        }



        public static List<Archivo> getArchivosInvestigacion(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return null;
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return null;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                List<Archivo> listArchivos = new List<Archivo>();

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string extension;
                    string fileType;
                    string contentType;
                    byte[] contenido;
                    Archivo archivo;
                    while (sdr.Read())
                    {
                        contenido = (byte[])sdr[4];
                        extension = Utils.getFileExtension(sdr.GetString(1));
                        if (extension == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        fileType = Utils.getFileType(extension);
                        if (fileType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        contentType = Utils.getContentType(extension);
                        if (contentType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        archivo = new Archivo(sdr.GetString(0), sdr.GetString(1), contenido, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }

                sdr.Close();
                conn.Close();

                return listArchivos;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static string registerInvestigacionRealizada
        (
            string codigo_evento,
            string fecha_cierre,
            List<Archivo> listArchivos,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            if ((fecha_cierre == null) || (fecha_cierre.Length < 1) || (!Utils.validateFecha(fecha_cierre)))
            {
                return "Fecha de cierre es inválida";
            }

            if (Convert.ToDateTime(fecha_cierre) > DateTime.Now)
            {
                return "La fecha de cierre no puede ser futura";
            }


            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    try
                    {
                        sdr.Read();
                        DateTime fecha_inicio = sdr.GetDateTime(0);
                        if (Convert.ToDateTime(fecha_cierre) < fecha_inicio)
                        {
                            sdr.Close();
                            conn.Close();

                            return "La fecha no puede ser anterior a la fecha de inicio de la Investigación (" + fecha_inicio.ToShortDateString() + ")";
                        }
                    }
                    catch (Exception ex)
                    {
                        sdr.Close();
                        conn.Close();

                        return "No se puede recuperar la fecha de inicio de la Investigación";
                    }
                }
                else
                {
                    conn.Close();

                    return "La investigación no se ha iniciado";
                }

                //conn.Close();

                //conn = DBController.getNewConnection();
                sdr.Close();


                trans = DBController.getNewTransaction(conn);
                cmd = DBController.getNewCommand(conn, trans);

                {//Cierre de Investigación
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_cierre);


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el cierre de la Investigación";
                    }

                }


                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                    }

                }



                if (!setEstadoEvento(cmd, codigo_evento, "Evaluación pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el cierre de la Investigación";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Cerrar Investigación (" + fecha_cierre + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el cierre de la Investigación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el cierre de la Investigación";
                }

                //MailSender.sendMailEventoCreado("david.espinoza@finning.cl", codigo);
                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar la ejecución de la Investigación";
            }
        }



        public static string removeInvestigacion
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            Investigacion investigacion = LogicController.getInvestigacion(codigo_evento);
            if (investigacion == null)
            {
                return "Investigación inválida";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);


                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el inicio de la Investigación";
                }

                if (!setEstadoEvento(cmd, codigo_evento, "Investigación pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el inicio de la Investigación";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Investigación", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el inicio de la Investigación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el inicio de la Investigación";
                }

                EmailSender.sendMailInvestigacionAsignadaEliminada(investigacion);
                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al eliminar el inicio de la Investigación";
            }
        }



        public static string registerIniciarInvestigacion
        (
            string codigo_evento,
            string rut_responsable,
            string fecha_inicio,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }


            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }


            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }


            if (investigacionExists(codigo_evento) > 0)
            {
                return "El Evento ya tiene una Investigación asociada";
            }


            PersonaInfo responsable = getPersonaInfo(rut_responsable);
            if (responsable == null)
            {
                return "Persona responsable inválida";
            }

            if ((fecha_inicio == null) || (fecha_inicio.Length < 1) || (!Utils.validateFecha(fecha_inicio)))
            {
                return "Fecha de inicio inválida";
            }

            if (Convert.ToDateTime(fecha_inicio) > DateTime.Now)
            {
                return "La fecha de inicio no puede ser futura";
            }

            if (Convert.ToDateTime(fecha_inicio) < evento.getFecha())
            {
                return "La fecha de inicio no puede ser anterior a la fecha en que ocurrió el Evento";
            }

            Usuario u = LogicController.getUsuario(usuario);
            if (u == null)
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("rut_responsable", SqlDbType.VarChar, 15).Value = responsable.getRut();
                cmd.Parameters.Add("antiguedad_responsable", System.Data.SqlDbType.Int).Value = responsable.getAntiguedad();
                cmd.Parameters.Add("fecha_inicio", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_inicio);
                cmd.Parameters.Add("fecha_cierre", SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("fecha_respuesta", SqlDbType.DateTime).Value = DBNull.Value;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el inicio de la Investigación";
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Investigación en curso"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el inicio de la Investigación";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Registrar inicio de Investigación (" + fecha_inicio + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el inicio de la Investigación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el inicio de la Investigación";
                }

                EmailSender.sendMailInvestigacionAsignada(u.getIDCentro(), rut_responsable, codigo_evento, fecha_inicio);
                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el inicio de la Investigación";
            }
        }



        public static List<Archivo> getArchivosAccionInmediata(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return null;
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return null;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                List<Archivo> listArchivos = new List<Archivo>();

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string extension;
                    string fileType;
                    string contentType;
                    byte[] contenido;
                    Archivo archivo;
                    while (sdr.Read())
                    {
                        contenido = (byte[])sdr[4];
                        extension = Utils.getFileExtension(sdr.GetString(1));
                        if (extension == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        fileType = Utils.getFileType(extension);
                        if (fileType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        contentType = Utils.getContentType(extension);
                        if (contentType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        archivo = new Archivo(sdr.GetString(0), sdr.GetString(1), contenido, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }

                sdr.Close();
                conn.Close();

                return listArchivos;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static string updateAccionInmediata
        (
            string codigo_evento,
            string nombre_origen,
            string nombre_causainmediata,
            string nombre_subcausainmediata,
            string nombre_causabasica,
            string nombre_subcausabasica,
            string rut_responsable,
            string accion_inmediata,
            string fecha_accion,
            string efectividad,
            string observacion,
            List<Archivo> listArchivos,
            List<PersonaInfo> listInvolucrados,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }


            if (accionInmediataExists(codigo_evento) < 1)
            {
                return "No se puede recuperar la información de la Acción Inmediata";
            }


            PersonaInfo responsable = LogicController.getPersonaInfo(rut_responsable);
            if (responsable == null)
            {
                return "No se puede recuperar la información del Responsable";
            }


            int exists_evaluacion = LogicController.evaluacionExists(codigo_evento);
            if (exists_evaluacion < 0)
            {
                return "No se puede recuperar la información de la Evaluación asociada";
            }

            if (nombre_origen == null)
            {
                return "Origen de falla inválido";
            }

            if (nombre_causainmediata == null)
            {
                return "Causa inmediata inválida";
            }

            if (nombre_subcausainmediata == null)
            {
                return "Sub-causa inmediata inválida";
            }

            if (nombre_causabasica == null)
            {
                return "Causa básica inválida";
            }

            if (nombre_subcausabasica == null)
            {
                return "Sub-causa básica inválida";
            }



            if ((accion_inmediata == null) || (accion_inmediata.Length < 1))
            {
                return "Acción inmediata inválida";
            }

            if ((fecha_accion == null) || (fecha_accion.Length < 1) || (!Utils.validateFecha(fecha_accion)))
            {
                return "Fecha de realización es inválida";
            }

            if (Convert.ToDateTime(fecha_accion) > DateTime.Now)
            {
                return "La fecha de realización no puede ser futura";
            }

            if (Convert.ToDateTime(fecha_accion) < evento.getFecha())
            {
                return "La fecha de la acción no puede ser anterior a la fecha en que ocurrió el Evento";
            }


            if ((efectividad == null) || ((!efectividad.Equals("Si")) && (!efectividad.Equals("No"))))
            {
                return "Efectividad inválida";
            }

            if (observacion == null)
            {
                return "Observación inválida";
            }


            if (listInvolucrados == null)
            {
                return "Personas involucradas inválidas";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("nombre_origen", SqlDbType.VarChar, 30).Value = nombre_origen;
                cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;
                cmd.Parameters.Add("accion_inmediata", System.Data.SqlDbType.VarChar, 3000).Value = accion_inmediata;
                cmd.Parameters.Add("fecha_accion", System.Data.SqlDbType.DateTime).Value = fecha_accion;
                cmd.Parameters.Add("efectividad", System.Data.SqlDbType.VarChar, 2).Value = efectividad;
                if (observacion.Length < 1)
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                }

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Acción Inmediata";
                }



                //Eliminar/Ingresar Responsable en Evento
                if (responsable != null)
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del responsable del Evento";
                    }

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = responsable.getRut();
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = responsable.getAntiguedad();


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización del responsable del Evento";
                    }

                }



                //Eliminar archivos
                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al actualizar los archivos de la Acción Inmediata";
                    }
                }


                //Ingresar Archivos
                {
                    foreach (Archivo archivo in listArchivos)
                    {

                        //Filtra archivos de imagen y las convierte en PNG
                        {
                            Utils.convertImageFileToJpeg(archivo);
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                        cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                        cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                        cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                        cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }

                    }
                }


                //Eliminar Involucrados en Evento
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Involucrado";

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al actualizar los involucrados del Evento";
                    }
                }


                //Ingresar Involucrados
                foreach (PersonaInfo involucrado in listInvolucrados)
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = involucrado.getRut();
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = involucrado.getAntiguedad();


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del involucrado \"" + involucrado.getNombre() + "\"";
                    }

                }


                if (exists_evaluacion > 0)
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("nombre_origen", SqlDbType.VarChar, 30).Value = nombre_origen;
                    cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                    cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                    cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                    cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización de la Evaluación asociada";
                    }
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Cerrado"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Acción Inmediata";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Editar Acción Inmediata", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Acción Inmediata";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la Acción Inmediata";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al actualizar la Acción Inmediata";
            }

        }




        public static string updateEvaluacion
        (
            string codigo_evento,
            string nombre_origen,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string nombre_causainmediata,
            string nombre_subcausainmediata,
            string nombre_causabasica,
            string nombre_subcausabasica,
            string rut_responsable_evento,
            string aceptado,
            string observacion,
            string rut_responsable_evaluacion,
            List<Archivo> listArchivos,
            List<PersonaInfo> listInvolucrados,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            if (investigacionExists(codigo_evento) < 1)
            {
                return "No se puede recuperar la Investigación";
            }


            Evaluacion evaluacion = LogicController.getEvaluacion(codigo_evento);
            if (evaluacion == null)
            {
                return "No se puede recuperar información de la Evaluación";
            }


            int exists_accioninmediata = LogicController.accionInmediataExists(codigo_evento);
            if (exists_accioninmediata < 0)
            {
                return "No se puede recuperar la información de la Acción Inmediata asociada";
            }
            else if (exists_accioninmediata > 0)
            {
                //Si tiene Acción Inmediata y se quiere rechazar
                if ((!evaluacion.getAceptado().Equals(aceptado)) && (aceptado.Equals("No")))
                {
                    return "No se puede rechazar una Evaluación mientras tenga una Acción Inmediata asociada";
                }
            }


            int exists_planaccion = LogicController.planAccionExists(codigo_evento);
            if (exists_planaccion < 0)
            {
                return "No se puede recuperar información de la Evaluación";
            }
            else if (exists_planaccion > 0)
            {
                //Si tiene Plan de Acción y se quiere rechazar
                if ((!evaluacion.getAceptado().Equals(aceptado)) && (aceptado.Equals("No")))
                {
                    return "No se puede rechazar una Evaluación mientras tenga un Plan de Acción asociado";
                }
            }

            if (nombre_origen == null)
            {
                return "Origen de la falla inválida";
            }


            if (rut_responsable_evento == null)
            {
                return "Responsable del Evento inválido";
            }

            PersonaInfo responsable_evento = LogicController.getPersonaInfo(rut_responsable_evento);
            if (responsable_evento == null)
            {
                return "No se puede recuperar la información del Responsable del Evento";
            }

            if (nombre_clasificacion == null)
            {
                return "Clasificación de la falla inválida";
            }
            else if (nombre_clasificacion.Equals("*Sin especificar"))
            {
                return "Se debe especificar la clasificación de la falla";
            }


            if (nombre_subclasificacion == null)
            {
                return "Sub-clasificación de la falla inválida";
            }
            else if (nombre_subclasificacion.Equals("*Sin especificar"))
            {
                return "Se debe especificar la sub-clasificación de la falla";
            }


            if (nombre_causainmediata == null)
            {
                return "Causa inmediata inválida";
            }

            if (nombre_subcausainmediata == null)
            {
                return "Sub-causa inmediata inválida";
            }

            if (nombre_causabasica == null)
            {
                return "Causa básica inválida";
            }

            if (nombre_subcausabasica == null)
            {
                return "Sub-causa básica inválida";
            }

            if ((aceptado == null) || ((!aceptado.Equals("Si")) && (!aceptado.Equals("No"))))
            {
                return "Aceptación inválida";
            }

            if (observacion == null)
            {
                return "Observación inválida";
            }

            PersonaInfo responsable_evaluacion = getPersonaInfo(rut_responsable_evaluacion);
            if (responsable_evaluacion == null)
            {
                return "Persona responsable de Evaluación inválida";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            if (listInvolucrados == null)
            {
                return "Personas involucradas inválidas";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Fallo al recuperar la información del Evento";
            }


            string estado = null;
            if (!evaluacion.getAceptado().Equals(aceptado))
            {
                if (aceptado.Equals("Si"))
                {
                    if (evento.getIRC() < 10)
                    {
                        estado = "Acción inmediata pendiente";
                    }
                    else
                    {
                        estado = "Plan de acción pendiente";
                    }
                }
                else if (aceptado.Equals("No"))
                {
                    estado = "Cerrado";
                }
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                {//Clasificación del Evento
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                    cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización de clasificación del Evento";
                    }

                }


                {//Evaluación
                    cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("nombre_origen", System.Data.SqlDbType.VarChar, 30).Value = nombre_origen;
                    cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                    cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                    cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                    cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;
                    cmd.Parameters.Add("aceptado", System.Data.SqlDbType.VarChar, 2).Value = aceptado;
                    if (observacion.Length > 0)
                    {
                        cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                    }
                    else
                    {
                        cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                    }
                    cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = rut_responsable_evaluacion;
                    cmd.Parameters.Add("antiguedad_responsable", System.Data.SqlDbType.Int).Value = responsable_evaluacion.getAntiguedad();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización de la la Evaluación";
                    }


                    //Eliminar/Ingresar Responsable en Evento
                    {

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del responsable del Evento";
                        }

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = responsable_evento.getRut();
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = responsable_evento.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del responsable del Evento";
                        }

                    }


                    //Eliminar Involucrados en Evento
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Involucrado";

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al actualizar los involucrados del Evento";
                        }
                    }


                    //Ingresar Involucrados
                    foreach (PersonaInfo involucrado in listInvolucrados)
                    {

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = involucrado.getRut();
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = involucrado.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del involucrado \"" + involucrado.getNombre() + "\"";
                        }

                    }



                    //Eliminar archivos
                    {
                        cmd.Parameters.Clear();

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al actualizar los archivos de la Evaluación";
                        }
                    }


                    //Ingresar Archivos
                    foreach (Archivo archivo in listArchivos)
                    {


                        //Filtra archivos de imagen y las convierte en PNG
                        {
                            Utils.convertImageFileToJpeg(archivo);
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                        cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                        cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                        cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                        cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }

                    }
                }


                if (exists_accioninmediata > 0)
                {
                    cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("nombre_origen", SqlDbType.VarChar, 30).Value = nombre_origen;
                    cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                    cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                    cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                    cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización de la Acción Inmediata asociada";
                    }
                }


                if (estado != null)
                {
                    if (!setEstadoEvento(cmd, codigo_evento, estado))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la actualización de la la Evaluación";
                    }
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Evaluar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la la Evaluación";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la actualización de la la Evaluación";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al actualizar la Evaluación";
            }

        }



        public static string removeAccionInmediata
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
                return null;

            int evaluacion_exists = LogicController.evaluacionExists(codigo_evento);
            if (evaluacion_exists < 0)
            {
                return "No se pueden recuperar la información del Evento";
            }

            List<Archivo> listArchivosAccionInmediata = LogicController.getArchivosAccionInmediata(codigo_evento);
            if (listArchivosAccionInmediata == null)
            {
                return "No se pueden recuperar los archivos asociados a la Acción Inmediata";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Acción Inmediata";
                }



                if (!setEstadoEvento(cmd, codigo_evento, "Acción inmediata pendiente"))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Acción Inmediata";
                }



                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Acción Inmediata", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Accion Inmediata";
                }


                if (evaluacion_exists == 0)
                {
                    //Eliminar Responsable en Evento
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Responsable";

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al eliminar el responsable del Evento";
                        }
                    }


                    //Eliminar Involucrados
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                        cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Involucrado";

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al eliminar los involucrados del Evento";
                        }
                    }
                }



                cmd.Parameters.Clear();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                foreach (Archivo archivo in listArchivosAccionInmediata)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_archivo", SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar los archivos asociados a la Acción Inmediata";
                    }
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar la Acción Inmediata";
                }

                return null;

            }
            catch (Exception e)
            {
                if ((conn != null) && (trans != null))
                {
                    DBController.doRollback(conn, trans);
                }

                return "Fallo al eliminar la Acción Inmediata";
            }
        }



        public static string registerAccionInmediata
        (
            string codigo_evento,
            string nombre_origen,
            string nombre_causainmediata,
            string nombre_subcausainmediata,
            string nombre_causabasica,
            string nombre_subcausabasica,
            string rut_responsable,
            string accion_inmediata,
            string fecha_accion,
            string efectividad,
            string observacion,
            List<Archivo> listArchivos,
            List<PersonaInfo> listInvolucrados,
            string usuario,
            string ip
        )
        {

            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            if (eventoExists(codigo_evento) < 1)
            {
                return "Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }

            if (accionInmediataExists(codigo_evento) > 0)
            {
                return "El Evento ya tiene una Accion Inmediata asociada";
            }


            PersonaInfo responsable;
            if (rut_responsable == null)
            {
                responsable = null;
            }
            else
            {
                responsable = LogicController.getPersonaInfo(rut_responsable);
                if (responsable == null)
                {
                    return "No se puede recuperar la información del Responsable";
                }
            }


            if (nombre_origen == null)
            {
                return "Origen de falla inválido";
            }

            if (nombre_causainmediata == null)
            {
                return "Causa inmediata inválida";
            }

            if (nombre_subcausainmediata == null)
            {
                return "Sub-causa inmediata inválida";
            }

            if (nombre_causabasica == null)
            {
                return "Causa básica inválida";
            }

            if (nombre_subcausabasica == null)
            {
                return "Sub-causa básica inválida";
            }



            if ((accion_inmediata == null) || (accion_inmediata.Length < 1))
            {
                return "Acción inmediata inválida";
            }

            if ((fecha_accion == null) || (fecha_accion.Length < 1) || (!Utils.validateFecha(fecha_accion)))
            {
                return "Fecha de realización es inválida";
            }

            if (Convert.ToDateTime(fecha_accion) > DateTime.Now)
            {
                return "La fecha de realización no puede ser futura";
            }

            if (Convert.ToDateTime(fecha_accion) < evento.getFecha())
            {
                return "La fecha de la acción no puede ser anterior a la fecha en que ocurrió el Evento";
            }


            if ((efectividad == null) || ((!efectividad.Equals("Si")) && (!efectividad.Equals("No"))))
            {
                return "Efectividad inválida";
            }

            if (observacion == null)
            {
                return "Observación inválida";
            }


            if (listInvolucrados == null)
            {
                return "Personas involucradas inválidas";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("nombre_origen", SqlDbType.VarChar, 30).Value = nombre_origen;
                cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;
                cmd.Parameters.Add("accion_inmediata", System.Data.SqlDbType.VarChar, 3000).Value = accion_inmediata;
                cmd.Parameters.Add("fecha_accion", System.Data.SqlDbType.DateTime).Value = fecha_accion;
                cmd.Parameters.Add("efectividad", System.Data.SqlDbType.VarChar, 2).Value = efectividad;
                if (observacion.Length < 1)
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                }

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }


                //Eliminar/Ingresar Responsable en Evento
                if (responsable != null)
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del responsable del Evento";
                    }

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = responsable.getRut();
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = responsable.getAntiguedad();


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del responsable del Evento";
                    }

                }



                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {

                    //Filtra archivos de imagen y las convierte en PNG
                    {
                        Utils.convertImageFileToJpeg(archivo);
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                    }

                }


                //Ingresar Involucrados
                foreach (PersonaInfo involucrado in listInvolucrados)
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = involucrado.getRut();
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = involucrado.getAntiguedad();


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del involucrado \"" + involucrado.getNombre() + "\"";
                    }

                }


                //Cerrar Evento
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("fecha_cierre", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro de la Acción Inmediata";
                    }
                }


                if (!setEstadoEvento(cmd, codigo_evento, "Cerrado"))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Registrar Acción Inmediata", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Cerrar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar la Acción Inmediata";
            }

        }



        public static string updateEvento_Cliente
        (
            string codigo_evento,
            string work_order,
            string fecha,
            string nombre_cliente,
            string id_centro,
            string modelo_equipo,
            string nombre_tipoequipo,
            string serie_equipo,
            string nombre_sistema,
            string nombre_subsistema,
            string nombre_componente,
            string serie_componente,
            string parte,
            string numero_parte,
            int horas,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string detalle,
            List<Archivo> listArchivos,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }


            int existsCode = evaluacionExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Evaluación asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede editar el Evento mientras tenga una Evaluación asociada";
            }


            int investigacion_exists = investigacionExists(codigo_evento);
            if (investigacion_exists < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Investigación asociada";
            }
            else if (investigacion_exists > 0)
            {
                return "No se puede editar el Evento mientras tenga una Investigación asociada";
            }


            existsCode = accionInmediataExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Acción Inmediata asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede editar el Evento mientras tenga una Acción Inmediata asociada";
            }



            if (work_order == null)
            {
                return "Orden de trabajo inválida";
            }

            if ((fecha == null) || (fecha.Length < 1))
            {
                return "Fecha inválida";
            }

            if (Convert.ToDateTime(fecha) > DateTime.Now)
            {
                return "La fecha no puede ser futura";
            }

            if ((nombre_cliente == null) || (nombre_cliente.Length < 1))
            {
                return "Cliente inválido";
            }

            if ((id_centro == null) || (id_centro.Length < 1))
            {
                return "Centro de servicios inválido";
            }


            if ((modelo_equipo == null) || (modelo_equipo.Length < 1))
            {
                return "Modelo del Equipo inválido";
            }

            if ((nombre_tipoequipo == null) || (nombre_tipoequipo.Length < 1))
            {
                return "Tipo de Equipo inválido";
            }


            if (serie_equipo == null)
            {
                return "Serie del Equipo inválida";
            }

            if ((serie_componente == null) || (serie_componente.Length < 1))
            {
                return "Serie del Componente inválida";
            }



            if ((nombre_sistema == null) || (nombre_sistema.Length < 1))
            {
                return "Sistema del Componente inválido";
            }

            if ((nombre_subsistema == null) || (nombre_subsistema.Length < 1))
            {
                return "Sub-sistema del Componente inválido";
            }

            if ((nombre_componente == null) || (nombre_componente.Length < 1))
            {
                return "Componente inválido";
            }

            if ((nombre_clasificacion == null) || (nombre_clasificacion.Length < 1))
            {
                return "Clasificación inválida";
            }

            if ((nombre_subclasificacion == null) || (nombre_subclasificacion.Length < 1))
            {
                return "Sub-clasificación inválida";
            }

            if ((detalle == null) || (detalle.Length < 1))
            {
                return "Detalle inválido";
            }


            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }



                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("work_order", System.Data.SqlDbType.VarChar, 30).Value = work_order;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = fecha;
                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                cmd.Parameters.Add("modelo_equipo", System.Data.SqlDbType.VarChar, 15).Value = modelo_equipo;
                cmd.Parameters.Add("nombre_tipoequipo", SqlDbType.VarChar, 30).Value = nombre_tipoequipo;

                if ((serie_equipo != null) && (serie_equipo.Length > 0))
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = serie_equipo;
                }
                else
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_sistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_sistema;
                cmd.Parameters.Add("nombre_subsistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_subsistema;
                cmd.Parameters.Add("nombre_componente", System.Data.SqlDbType.VarChar, 70).Value = nombre_componente;

                if ((serie_componente != null) && (serie_componente.Length > 0))
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = serie_componente;
                }
                else
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((parte != null) && (parte.Length > 0))
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = parte;
                }
                else
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((numero_parte != null) && (numero_parte.Length > 0))
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = numero_parte;
                }
                else
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if (horas >= 0)
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = horas;
                }
                else
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;
                cmd.Parameters.Add("detalle", System.Data.SqlDbType.VarChar, 3000).Value = detalle;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al editar el Evento";
                }



                //Eliminar archivos
                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al actualizar los archivos del Evento";
                    }
                }


                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {

                    //Filtra archivos de imagen y las convierte en PNG
                    {
                        Utils.convertImageFileToJpeg(archivo);
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }

                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Editar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al editar el Evento";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al editar el Evento";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al editar el Evento";
            }

        }



        public static string registerEvento_Cliente
        (
            string work_order,
            string fecha,
            string nombre_cliente,
            string id_centro,
            string modelo_equipo,
            string nombre_tipoequipo,
            string serie_equipo,
            string nombre_sistema,
            string nombre_subsistema,
            string nombre_componente,
            string serie_componente,
            string parte,
            string numero_parte,
            int horas,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string detalle,
            List<Archivo> listArchivos,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (work_order == null)
            {
                return "Orden de trabajo inválida";
            }

            if ((fecha == null) || (fecha.Length < 1))
            {
                return "Fecha inválida";
            }

            if (Convert.ToDateTime(fecha) > DateTime.Now)
            {
                return "La fecha no puede ser futura";
            }

            if ((nombre_cliente == null) || (nombre_cliente.Length < 1))
            {
                return "Cliente inválido";
            }

            if ((id_centro == null) || (id_centro.Length < 1))
            {
                return "Centro de servicios inválido";
            }


            if ((modelo_equipo == null) || (modelo_equipo.Length < 1))
            {
                return "Modelo del Equipo inválido";
            }

            if ((nombre_tipoequipo == null) || (nombre_tipoequipo.Length < 1))
            {
                return "Tipo de Equipo inválido";
            }

            if (serie_equipo == null)
            {
                return "Serie del Equipo inválida";
            }

            if ((serie_componente == null) || (serie_componente.Length < 1))
            {
                return "Serie del Componente inválida";
            }

            int centrocliente_exists = LogicController.centroClienteExists(id_centro, nombre_cliente);
            if (centrocliente_exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (centrocliente_exists == 0)
            {
                return "El Cliente no está asociado al Centro";
            }



            if ((nombre_sistema == null) || (nombre_sistema.Length < 1))
            {
                return "Sistema del Componente inválido";
            }

            if ((nombre_subsistema == null) || (nombre_subsistema.Length < 1))
            {
                return "Sub-sistema del Componente inválido";
            }

            if ((nombre_componente == null) || (nombre_componente.Length < 1))
            {
                return "Componente inválido";
            }

            if ((nombre_clasificacion == null) || (nombre_clasificacion.Length < 1))
            {
                return "Clasificación inválida";
            }

            if ((nombre_subclasificacion == null) || (nombre_subclasificacion.Length < 1))
            {
                return "Sub-clasificación inválida";
            }

            if ((detalle == null) || (detalle.Length < 1))
            {
                return "Detalle inválido";
            }


            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }

            string estado = "Revisión pendiente";


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }

                string codigo = LogicController.getNewCodigoEvento(cmd, id_centro);
                if (codigo == null)
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al generar el código";
                }

                cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                cmd.Parameters.Add("work_order", System.Data.SqlDbType.VarChar, 30).Value = work_order;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = fecha;
                cmd.Parameters.Add("fecha_ingreso", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                cmd.Parameters.Add("nombre_fuente", System.Data.SqlDbType.VarChar, 70).Value = "Reclamo de Cliente";
                cmd.Parameters.Add("modelo_equipo", System.Data.SqlDbType.VarChar, 15).Value = modelo_equipo;
                cmd.Parameters.Add("nombre_tipoequipo", SqlDbType.VarChar, 30).Value = nombre_tipoequipo;

                if ((serie_equipo != null) && (serie_equipo.Length > 0))
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = serie_equipo;
                }
                else
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_sistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_sistema;
                cmd.Parameters.Add("nombre_subsistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_subsistema;
                cmd.Parameters.Add("nombre_componente", System.Data.SqlDbType.VarChar, 70).Value = nombre_componente;

                if ((serie_componente != null) && (serie_componente.Length > 0))
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = serie_componente;
                }
                else
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((parte != null) && (parte.Length > 0))
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = parte;
                }
                else
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((numero_parte != null) && (numero_parte.Length > 0))
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = numero_parte;
                }
                else
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if (horas >= 0)
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = horas;
                }
                else
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;
                cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                cmd.Parameters.Add("detalle", System.Data.SqlDbType.VarChar, 3000).Value = detalle;
                cmd.Parameters.Add("probabilidad", System.Data.SqlDbType.VarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("consecuencia", System.Data.SqlDbType.VarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("irc", System.Data.SqlDbType.Decimal).Value = DBNull.Value;
                cmd.Parameters.Add("criticidad", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                cmd.Parameters.Add("fecha_cierre", System.Data.SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("estado", System.Data.SqlDbType.VarChar, 70).Value = estado;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }


                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {

                    //Filtra archivos de imagen y las convierte en PNG
                    {
                        Utils.convertImageFileToJpeg(archivo);
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }

                }


                //Ingresar Creador
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = owner.getRut();
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Creador";
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = owner.getAntiguedad();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el creador del Evento";
                    }
                }


                if (!ActionLogger.evento(cmd, codigo, usuario, "Crear Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }

                EmailSender.sendMailEventoCreado(owner.getRut(), codigo);

                return "CODIGO:" + codigo;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el nuevo Evento";
            }

        }




        public static string registerEventoAccionInmediata
        (
            string work_order,
            string fecha,
            string nombre_cliente,
            string id_centro,
            string nombre_area,
            string nombre_subarea,
            string nombre_fuente,
            string modelo_equipo,
            string nombre_tipoequipo,
            string serie_equipo,
            string nombre_sistema,
            string nombre_subsistema,
            string nombre_componente,
            string serie_componente,
            string parte,
            string numero_parte,
            int horas,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string agente_corrector,
            string detalle,
            string probabilidad,
            string consecuencia,
            double irc,
            string criticidad,
            List<Archivo> listArchivos,
            string nombre_origen,
            string nombre_causainmediata,
            string nombre_subcausainmediata,
            string nombre_causabasica,
            string nombre_subcausabasica,
            string rut_responsable,
            string accion_inmediata,
            string fecha_accion,
            string efectividad,
            string observacion,
            List<Archivo> listArchivosAccionInmediata,
            List<PersonaInfo> listInvolucrados,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if ((work_order == null) || (work_order.Length < 1))
            {
                return "Orden de trabajo inválida";
            }

            if ((fecha == null) || (fecha.Length < 1))
            {
                return "Fecha inválida";
            }

            if (Convert.ToDateTime(fecha) > DateTime.Now)
            {
                return "La fecha no puede ser futura";
            }

            if ((nombre_cliente == null) || (nombre_cliente.Length < 1))
            {
                return "Cliente inválido";
            }

            if ((id_centro == null) || (id_centro.Length < 1))
            {
                return "Centro de servicios inválido";
            }

            if ((nombre_area == null) || (nombre_area.Length < 1))
            {
                return "Área inválida";
            }

            if ((nombre_subarea == null) || (nombre_subarea.Length < 1))
            {
                return "Sub-área inválida";
            }

            if ((nombre_fuente == null) || (nombre_fuente.Length < 1))
            {
                return "Fuente de detección inválida";
            }

            if ((modelo_equipo == null) || (modelo_equipo.Length < 1))
            {
                return "Modelo del Equipo inválido";
            }

            if ((nombre_tipoequipo == null) || (nombre_tipoequipo.Length < 1))
            {
                return "Tipo de Equipo inválido";
            }


            if (id_centro.Equals("CSAR"))
            {
                if (serie_equipo.Length < 1)
                {
                    return "Serie del Equipo inválida";
                }
            }


            int centrocliente_exists = LogicController.centroClienteExists(id_centro, nombre_cliente);
            if (centrocliente_exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (centrocliente_exists == 0)
            {
                return "El Cliente no está asociado al Centro";
            }



            if ((nombre_sistema == null) || (nombre_sistema.Length < 1))
            {
                return "Sistema del Componente inválido";
            }

            if ((nombre_subsistema == null) || (nombre_subsistema.Length < 1))
            {
                return "Sub-sistema del Componente inválido";
            }

            if ((nombre_componente == null) || (nombre_componente.Length < 1))
            {
                return "Componente inválido";
            }

            if ((nombre_clasificacion == null) || (nombre_clasificacion.Length < 1))
            {
                return "Clasificación inválida";
            }

            if ((nombre_subclasificacion == null) || (nombre_subclasificacion.Length < 1))
            {
                return "Sub-clasificación inválida";
            }

            if ((detalle == null) || (detalle.Length < 1))
            {
                return "Detalle inválido";
            }

            if ((probabilidad == null) || (probabilidad.Length < 1))
            {
                return "Probabilidad inválida";
            }

            if ((consecuencia == null) || (consecuencia.Length < 1))
            {
                return "Consecuencia inválida";
            }

            if (irc < 1)
            {
                return "IRC inválido";
            }

            if ((criticidad == null) || (criticidad.Length < 1))
            {
                return "Criticidad inválida";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            if ((irc >= 10) || (nombre_fuente.ToUpper().Equals("RECLAMO DE CLIENTE")))
            {
                return "No se puede registrar la Acción Inmediata porque el Evento requiere Investigación";
            }


            PersonaInfo jefe_area = LogicController.getCentroAreaJefe(nombre_area, id_centro);
            if (jefe_area == null)
            {
                return "No se puede recuperar la información del Jefe de Área";
            }


            if (rut_responsable == null)
            {
                return "Responsable inválido";
            }



            PersonaInfo responsable = LogicController.getPersonaInfo(rut_responsable);
            if (responsable == null)
            {
                return "No se puede recuperar la información del Responsable";
            }


            if (nombre_origen == null)
            {
                return "Origen de falla inválido";
            }

            if (nombre_causainmediata == null)
            {
                return "Causa inmediata inválida";
            }

            if (nombre_subcausainmediata == null)
            {
                return "Sub-causa inmediata inválida";
            }

            if (nombre_causabasica == null)
            {
                return "Causa básica inválida";
            }

            if (nombre_subcausabasica == null)
            {
                return "Sub-causa básica inválida";
            }



            if ((accion_inmediata == null) || (accion_inmediata.Length < 1))
            {
                return "Acción inmediata inválida";
            }

            if ((fecha_accion == null) || (fecha_accion.Length < 1) || (!Utils.validateFecha(fecha_accion)))
            {
                return "Fecha de realización es inválida";
            }

            if (Convert.ToDateTime(fecha_accion) > DateTime.Now)
            {
                return "La fecha de realización no puede ser futura";
            }

            if (Convert.ToDateTime(fecha_accion) < Convert.ToDateTime(fecha))
            {
                return "La fecha de la acción no puede ser anterior a la fecha en que ocurrió el Evento";
            }


            if ((efectividad == null) || ((!efectividad.Equals("Si")) && (!efectividad.Equals("No"))))
            {
                return "Efectividad inválida";
            }

            if (observacion == null)
            {
                return "Observación inválida";
            }


            if (listInvolucrados == null)
            {
                return "Personas involucradas inválidas";
            }

            if (listArchivosAccionInmediata == null)
            {
                return "Archivos adjuntos en Acción Inmediata inválidos";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }

                string codigo = LogicController.getNewCodigoEvento(cmd, id_centro);
                if (codigo == null)
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al generar el código";
                }

                {//EVENTO
                    cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("work_order", System.Data.SqlDbType.VarChar, 30).Value = work_order;
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = fecha;
                    cmd.Parameters.Add("fecha_ingreso", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                    cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                    cmd.Parameters.Add("nombre_fuente", System.Data.SqlDbType.VarChar, 70).Value = nombre_fuente;
                    cmd.Parameters.Add("modelo_equipo", System.Data.SqlDbType.VarChar, 15).Value = modelo_equipo;
                    cmd.Parameters.Add("nombre_tipoequipo", SqlDbType.VarChar, 30).Value = nombre_tipoequipo;

                    if ((serie_equipo != null) && (serie_equipo.Length > 0))
                    {
                        cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = serie_equipo;
                    }
                    else
                    {
                        cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                    }

                    cmd.Parameters.Add("nombre_sistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_sistema;
                    cmd.Parameters.Add("nombre_subsistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_subsistema;
                    cmd.Parameters.Add("nombre_componente", System.Data.SqlDbType.VarChar, 70).Value = nombre_componente;

                    if ((serie_componente != null) && (serie_componente.Length > 0))
                    {
                        cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = serie_componente;
                    }
                    else
                    {
                        cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                    }

                    if ((parte != null) && (parte.Length > 0))
                    {
                        cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = parte;
                    }
                    else
                    {
                        cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                    }

                    if ((numero_parte != null) && (numero_parte.Length > 0))
                    {
                        cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = numero_parte;
                    }
                    else
                    {
                        cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                    }

                    if (horas >= 0)
                    {
                        cmd.Parameters.Add("horas", SqlDbType.Int).Value = horas;
                    }
                    else
                    {
                        cmd.Parameters.Add("horas", SqlDbType.Int).Value = DBNull.Value;
                    }

                    cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                    cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;

                    if ((agente_corrector != null) && (agente_corrector.Length > 0))
                    {
                        cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = agente_corrector;
                    }
                    else
                    {
                        cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                    }

                    cmd.Parameters.Add("detalle", System.Data.SqlDbType.VarChar, 3000).Value = detalle;
                    cmd.Parameters.Add("probabilidad", System.Data.SqlDbType.VarChar, 50).Value = probabilidad;
                    cmd.Parameters.Add("consecuencia", System.Data.SqlDbType.VarChar, 50).Value = consecuencia;
                    cmd.Parameters.Add("irc", System.Data.SqlDbType.Decimal).Value = irc;
                    cmd.Parameters.Add("criticidad", System.Data.SqlDbType.VarChar, 70).Value = criticidad;
                    cmd.Parameters.Add("fecha_cierre", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("estado", System.Data.SqlDbType.VarChar, 70).Value = "Cerrado";

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el nuevo Evento";
                    }


                    //Eliminar/Ingresar JefeArea en Evento
                    {
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "JefeArea";


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del jefe de área del Evento";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = jefe_area.getRut();
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = jefe_area.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del jefe de área del Evento";
                        }
                    }


                    //Ingresar Archivos
                    foreach (Archivo archivo in listArchivos)
                    {

                        //Filtra archivos de imagen y las convierte en PNG
                        {
                            Utils.convertImageFileToJpeg(archivo);
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                        cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                        cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                        cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                        cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                        cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                        }

                    }


                    //Ingresar Creador
                    {
                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                        cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = owner.getRut();
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Creador";
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = owner.getAntiguedad();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Fallo al registrar el creador del Evento";
                        }
                    }


                    if (!ActionLogger.evento(cmd, codigo, usuario, "Crear Evento", ip))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el nuevo Evento";
                    }

                }


                {//ACCIÓN INMEDIATA
                    cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("nombre_origen", SqlDbType.VarChar, 30).Value = nombre_origen;
                    cmd.Parameters.Add("nombre_causainmediata", SqlDbType.VarChar, 120).Value = nombre_causainmediata;
                    cmd.Parameters.Add("nombre_subcausainmediata", SqlDbType.VarChar, 120).Value = nombre_subcausainmediata;
                    cmd.Parameters.Add("nombre_causabasica", SqlDbType.VarChar, 120).Value = nombre_causabasica;
                    cmd.Parameters.Add("nombre_subcausabasica", SqlDbType.VarChar, 120).Value = nombre_subcausabasica;
                    cmd.Parameters.Add("accion_inmediata", System.Data.SqlDbType.VarChar, 3000).Value = accion_inmediata;
                    cmd.Parameters.Add("fecha_accion", System.Data.SqlDbType.DateTime).Value = fecha_accion;
                    cmd.Parameters.Add("efectividad", System.Data.SqlDbType.VarChar, 2).Value = efectividad;
                    if (observacion.Length < 1)
                    {
                        cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("observacion", System.Data.SqlDbType.VarChar, 3000).Value = observacion;
                    }

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro de la Acción Inmediata";
                    }


                    //Eliminar/Ingresar Responsable en Evento
                    {

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del responsable del Evento";
                        }

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = responsable.getRut();
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = responsable.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del responsable del Evento";
                        }

                    }


                    //Ingresar Archivos
                    foreach (Archivo archivo in listArchivosAccionInmediata)
                    {

                        //Filtra archivos de imagen y las convierte en PNG
                        {
                            Utils.convertImageFileToJpeg(archivo);
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                        cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                        cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                        cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                        cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }


                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                        cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del archivo \"" + archivo.getNombre() + "\"";
                        }

                    }


                    //Ingresar Involucrados
                    foreach (PersonaInfo involucrado in listInvolucrados)
                    {

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                        cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = involucrado.getRut();
                        cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";
                        cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = involucrado.getAntiguedad();


                        if (!DBController.addQuery(cmd))
                        {
                            DBController.doRollback(conn, trans);

                            return "Falló el registro del involucrado \"" + involucrado.getNombre() + "\"";
                        }

                    }

                }



                if (!ActionLogger.evento(cmd, codigo, usuario, "Registrar Acción Inmediata", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }


                if (!ActionLogger.evento(cmd, codigo, usuario, "Cerrar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló el registro de la Acción Inmediata";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }

                EmailSender.sendMailEventoCreado(owner.getRut(), codigo);

                return "CODIGO:" + codigo;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el nuevo Evento";
            }

        }




        public static string removeEvento
        (
            string codigo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }


            int existsCode = evaluacionExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Evaluación asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede eliminar el Evento mientras tenga una Evaluación asociada";
            }


            existsCode = investigacionExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Investigación asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede eliminar el Evento mientras tenga una Investigación asociada";
            }


            existsCode = accionInmediataExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Acción Inmediata asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede eliminar el Evento mientras tenga una Acción Inmediata asociada";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }



                //Eliminar archivos
                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al actualizar los archivos del Evento";
                    }
                }


                //Eliminar Evento
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar el Evento";
                    }
                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Eliminar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el Evento";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el Evento";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al eliminar el Evento";
            }

        }



        public static string updateEvento
        (
            string codigo_evento,
            string work_order,
            string fecha,
            string nombre_cliente,
            string id_centro,
            string nombre_area,
            string nombre_subarea,
            string nombre_fuente,
            string modelo_equipo,
            string nombre_tipoequipo,
            string serie_equipo,
            string nombre_sistema,
            string nombre_subsistema,
            string nombre_componente,
            string serie_componente,
            string parte,
            string numero_parte,
            int horas,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string agente_corrector,
            string detalle,
            string probabilidad,
            string consecuencia,
            double irc,
            string criticidad,
            List<Archivo> listArchivos,
            int tipo_evento,
            int nuevo_tipo_evento,
            string usuario,
            string ip
        )
        {
            if (codigo_evento == null)
            {
                return "Código de Evento inválido";
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return "Evento inválido";
            }


            int existsCode = evaluacionExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Evaluación asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede editar el Evento mientras tenga una Evaluación asociada";
            }


            int investigacion_exists = investigacionExists(codigo_evento);
            if (investigacion_exists < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Investigación asociada";
            }
            else if (investigacion_exists > 0)
            {
                //Permite editar un Evento aunque tenga una Investigación asociada
                //return "No se puede editar el Evento mientras tenga una Investigación asociada"; 
            }


            existsCode = accionInmediataExists(codigo_evento);
            if (existsCode < 0)
            {
                return "Falló al determinar si el Evento tiene alguna Acción Inmediata asociada";
            }
            else if (existsCode > 0)
            {
                return "No se puede editar el Evento mientras tenga una Acción Inmediata asociada";
            }



            if ((work_order == null) || (work_order.Length < 1))
            {
                return "Orden de trabajo inválida";
            }

            if ((fecha == null) || (fecha.Length < 1))
            {
                return "Fecha inválida";
            }

            if (Convert.ToDateTime(fecha) > DateTime.Now)
            {
                return "La fecha no puede ser futura";
            }

            if ((nombre_cliente == null) || (nombre_cliente.Length < 1))
            {
                return "Cliente inválido";
            }

            if ((id_centro == null) || (id_centro.Length < 1))
            {
                return "Centro de servicios inválido";
            }

            if ((nombre_area == null) || (nombre_area.Length < 1))
            {
                return "Área inválida";
            }

            if ((nombre_subarea == null) || (nombre_subarea.Length < 1))
            {
                return "Sub-área inválida";
            }

            if ((nombre_fuente == null) || (nombre_fuente.Length < 1))
            {
                return "Fuente de detección inválida";
            }

            if ((modelo_equipo == null) || (modelo_equipo.Length < 1))
            {
                return "Modelo del Equipo inválido";
            }

            if ((nombre_tipoequipo == null) || (nombre_tipoequipo.Length < 1))
            {
                return "Tipo de Equipo inválido";
            }


            if (id_centro.Equals("CSAR"))
            {
                if (serie_equipo.Length < 1)
                {
                    return "Serie del Equipo inválida";
                }
            }


            if ((nombre_sistema == null) || (nombre_sistema.Length < 1))
            {
                return "Sistema del Componente inválido";
            }

            if ((nombre_subsistema == null) || (nombre_subsistema.Length < 1))
            {
                return "Sub-sistema del Componente inválido";
            }

            if ((nombre_componente == null) || (nombre_componente.Length < 1))
            {
                return "Componente inválido";
            }

            if ((nombre_clasificacion == null) || (nombre_clasificacion.Length < 1))
            {
                return "Clasificación inválida";
            }

            if ((nombre_subclasificacion == null) || (nombre_subclasificacion.Length < 1))
            {
                return "Sub-clasificación inválida";
            }

            if ((detalle == null) || (detalle.Length < 1))
            {
                return "Detalle inválido";
            }

            if ((probabilidad == null) || (probabilidad.Length < 1))
            {
                return "Probabilidad inválida";
            }

            if ((consecuencia == null) || (consecuencia.Length < 1))
            {
                return "Consecuencia inválida";
            }

            if (irc < 1)
            {
                return "IRC inválido";
            }

            if ((criticidad == null) || (criticidad.Length < 1))
            {
                return "Criticidad inválida";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }

            if (tipo_evento < 0)
            {
                return "Tipo de Evento inválido";
            }

            if (nuevo_tipo_evento < 0)
            {
                return "Nuevo tipo de Evento inválido";
            }


            PersonaInfo jefe_area = LogicController.getCentroAreaJefe(nombre_area, id_centro);
            if (jefe_area == null)
            {
                return "No se puede recuperar la información del Jefe de Área";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            string estado = null;
            if ((tipo_evento == 2) && (nuevo_tipo_evento == 2))
            {
                //Es una No conformidad que pasa de "Revisión pendiente" a "Investigación pendiente"
                estado = "Investigación pendiente";
            }
            else if (tipo_evento == 1)
            {
                if (nuevo_tipo_evento == 1)
                {
                    //Queda en el estado que estaba
                    estado = null;
                }
                else if (nuevo_tipo_evento == 0)
                {
                    //Ahora requiere una Acción Inmediata
                    estado = "Acción inmediata pendiente";
                }
            }
            else if (tipo_evento == 0)
            {
                if (nuevo_tipo_evento == 1)
                {
                    //Ahora requiere una Investigación
                    estado = "Investigación pendiente";
                }
                else if (nuevo_tipo_evento == 0)
                {
                    //Queda en el estado que estaba
                    estado = null;
                }
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }

                if ((investigacion_exists == 1) && (nuevo_tipo_evento == 0))
                {
                    //Si tiene una Investigación asociada y ahora clasifica como Hallazgo
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar los archivos de la Investigación";
                    }

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar la Investigación";
                    }

                }

                cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("work_order", System.Data.SqlDbType.VarChar, 30).Value = work_order;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = fecha;
                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                cmd.Parameters.Add("nombre_fuente", System.Data.SqlDbType.VarChar, 70).Value = nombre_fuente;
                cmd.Parameters.Add("modelo_equipo", System.Data.SqlDbType.VarChar, 15).Value = modelo_equipo;
                cmd.Parameters.Add("nombre_tipoequipo", SqlDbType.VarChar, 30).Value = nombre_tipoequipo;

                if ((serie_equipo != null) && (serie_equipo.Length > 0))
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = serie_equipo;
                }
                else
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_sistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_sistema;
                cmd.Parameters.Add("nombre_subsistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_subsistema;
                cmd.Parameters.Add("nombre_componente", System.Data.SqlDbType.VarChar, 70).Value = nombre_componente;

                if ((serie_componente != null) && (serie_componente.Length > 0))
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = serie_componente;
                }
                else
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((parte != null) && (parte.Length > 0))
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = parte;
                }
                else
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((numero_parte != null) && (numero_parte.Length > 0))
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = numero_parte;
                }
                else
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if (horas >= 0)
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = horas;
                }
                else
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;

                if ((agente_corrector != null) && (agente_corrector.Length > 0))
                {
                    cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = agente_corrector;
                }
                else
                {
                    cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                cmd.Parameters.Add("detalle", System.Data.SqlDbType.VarChar, 3000).Value = detalle;
                cmd.Parameters.Add("probabilidad", System.Data.SqlDbType.VarChar, 50).Value = probabilidad;
                cmd.Parameters.Add("consecuencia", System.Data.SqlDbType.VarChar, 50).Value = consecuencia;
                cmd.Parameters.Add("irc", System.Data.SqlDbType.Decimal).Value = irc;
                cmd.Parameters.Add("criticidad", System.Data.SqlDbType.VarChar, 70).Value = criticidad;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al editar el Evento";
                }


                if (estado != null) //Sólo si requiere cambiar de estado
                {
                    if (!setEstadoEvento(cmd, codigo_evento, estado))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al editar el Evento";
                    }
                }


                //Eliminar JefeArea en Evento
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "JefeArea";

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al eliminar el jefe de área del Evento";
                    }
                }


                //Ingresar JefeArea en Evento
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = jefe_area.getRut();
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "JefeArea";
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = jefe_area.getAntiguedad();


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del jefe de área del Evento";
                    }
                }


                //Eliminar archivos
                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al actualizar los archivos del Evento";
                    }
                }


                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {

                    //Filtra archivos de imagen y las convierte en PNG
                    {
                        Utils.convertImageFileToJpeg(archivo);
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }

                }


                if (!ActionLogger.evento(cmd, codigo_evento, usuario, "Editar Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al editar el Evento";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al editar el Evento";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al editar el Evento";
            }

        }




        public static PersonaInfo getResponsableEvento(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 15).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";

                    SqlDataReader sdr_pi = cmd.ExecuteReader();

                    PersonaInfo responsable;
                    if (sdr_pi.HasRows)
                    {
                        while (sdr_pi.Read())
                        {
                            responsable = LogicController.getPersonaInfo(sdr_pi.GetString(0));

                            sdr_pi.Close();
                            conn.Close();

                            return responsable;
                        }
                    }

                    sdr_pi.Close();

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static PersonaInfo getJefeAreaEvento(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 15).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "JefeArea";

                    SqlDataReader sdr_pi = cmd.ExecuteReader();

                    PersonaInfo jefe_area;
                    if (sdr_pi.HasRows)
                    {
                        while (sdr_pi.Read())
                        {
                            jefe_area = LogicController.getPersonaInfo(sdr_pi.GetString(0));

                            sdr_pi.Close();
                            conn.Close();

                            return jefe_area;
                        }
                    }

                    sdr_pi.Close();

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static List<Archivo> getArchivosEvento(string codigo_evento)
        {
            if (codigo_evento == null)
            {
                return null;
            }

            Evento evento = LogicController.getEvento(codigo_evento);
            if (evento == null)
            {
                return null;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                List<Archivo> listArchivos = new List<Archivo>();

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string extension;
                    string fileType;
                    string contentType;
                    byte[] contenido;
                    Archivo archivo;
                    while (sdr.Read())
                    {
                        contenido = (byte[])sdr[4];
                        extension = Utils.getFileExtension(sdr.GetString(1));
                        if (extension == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        fileType = Utils.getFileType(extension);
                        if (fileType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        contentType = Utils.getContentType(extension);
                        if (contentType == null)
                        {
                            sdr.Close();
                            conn.Close();

                            return null;
                        }

                        archivo = new Archivo(sdr.GetString(0), sdr.GetString(1), contenido, fileType, contentType);
                        listArchivos.Add(archivo);
                    }
                }

                sdr.Close();
                conn.Close();

                return listArchivos;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static string registerEvento
        (
            string work_order,
            string fecha,
            string nombre_cliente,
            string id_centro,
            string nombre_area,
            string nombre_subarea,
            string nombre_fuente,
            string modelo_equipo,
            string nombre_tipoequipo,
            string serie_equipo,
            string nombre_sistema,
            string nombre_subsistema,
            string nombre_componente,
            string serie_componente,
            string parte,
            string numero_parte,
            int horas,
            string nombre_clasificacion,
            string nombre_subclasificacion,
            string agente_corrector,
            string detalle,
            string probabilidad,
            string consecuencia,
            double irc,
            string criticidad,
            List<Archivo> listArchivos,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if ((work_order == null) || (work_order.Length < 1))
            {
                return "Orden de trabajo inválida";
            }

            if ((fecha == null) || (fecha.Length < 1))
            {
                return "Fecha inválida";
            }

            if (Convert.ToDateTime(fecha) > DateTime.Now)
            {
                return "La fecha no puede ser futura";
            }

            if ((nombre_cliente == null) || (nombre_cliente.Length < 1))
            {
                return "Cliente inválido";
            }

            if ((id_centro == null) || (id_centro.Length < 1))
            {
                return "Centro de servicios inválido";
            }

            if ((nombre_area == null) || (nombre_area.Length < 1))
            {
                return "Área inválida";
            }

            if ((nombre_subarea == null) || (nombre_subarea.Length < 1))
            {
                return "Sub-área inválida";
            }

            if ((nombre_fuente == null) || (nombre_fuente.Length < 1))
            {
                return "Fuente de detección inválida";
            }

            if ((modelo_equipo == null) || (modelo_equipo.Length < 1))
            {
                return "Modelo del Equipo inválido";
            }

            if ((nombre_tipoequipo == null) || (nombre_tipoequipo.Length < 1))
            {
                return "Tipo de Equipo inválido";
            }


            if (id_centro.Equals("CSAR"))
            {
                if (serie_equipo.Length < 1)
                {
                    return "Serie del Equipo inválida";
                }
            }


            int centrocliente_exists = LogicController.centroClienteExists(id_centro, nombre_cliente);
            if (centrocliente_exists < 0)
            {
                return "No se puede recuperar información del Cliente";
            }
            else if (centrocliente_exists == 0)
            {
                return "El Cliente no está asociado al Centro";
            }



            if ((nombre_sistema == null) || (nombre_sistema.Length < 1))
            {
                return "Sistema del Componente inválido";
            }

            if ((nombre_subsistema == null) || (nombre_subsistema.Length < 1))
            {
                return "Sub-sistema del Componente inválido";
            }

            if ((nombre_componente == null) || (nombre_componente.Length < 1))
            {
                return "Componente inválido";
            }

            if ((nombre_clasificacion == null) || (nombre_clasificacion.Length < 1))
            {
                return "Clasificación inválida";
            }

            if ((nombre_subclasificacion == null) || (nombre_subclasificacion.Length < 1))
            {
                return "Sub-clasificación inválida";
            }

            if ((detalle == null) || (detalle.Length < 1))
            {
                return "Detalle inválido";
            }

            if ((probabilidad == null) || (probabilidad.Length < 1))
            {
                return "Probabilidad inválida";
            }

            if ((consecuencia == null) || (consecuencia.Length < 1))
            {
                return "Consecuencia inválida";
            }

            if (irc < 1)
            {
                return "IRC inválido";
            }

            if ((criticidad == null) || (criticidad.Length < 1))
            {
                return "Criticidad inválida";
            }

            if (listArchivos == null)
            {
                return "Archivos adjuntos inválidos";
            }


            PersonaInfo jefe_area = LogicController.getCentroAreaJefe(nombre_area, id_centro);
            if (jefe_area == null)
            {
                return "No se puede recuperar la información del Jefe de Área";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }

            string estado;
            if ((irc >= 10) || (nombre_fuente.ToUpper().Equals("RECLAMO DE CLIENTE")))
            {
                estado = "Investigación pendiente";
            }
            else
            {
                estado = "Acción inmediata pendiente";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }

                string codigo = LogicController.getNewCodigoEvento(cmd, id_centro);
                if (codigo == null)
                {
                    DBController.doRollback(conn, trans);

                    return "Se ha producido un error al generar el código";
                }

                cmd.CommandText =  /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                cmd.Parameters.Add("work_order", System.Data.SqlDbType.VarChar, 30).Value = work_order;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = fecha;
                cmd.Parameters.Add("fecha_ingreso", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                cmd.Parameters.Add("nombre_fuente", System.Data.SqlDbType.VarChar, 70).Value = nombre_fuente;
                cmd.Parameters.Add("modelo_equipo", System.Data.SqlDbType.VarChar, 15).Value = modelo_equipo;
                cmd.Parameters.Add("nombre_tipoequipo", SqlDbType.VarChar, 30).Value = nombre_tipoequipo;

                if ((serie_equipo != null) && (serie_equipo.Length > 0))
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = serie_equipo;
                }
                else
                {
                    cmd.Parameters.Add("serie_equipo", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_sistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_sistema;
                cmd.Parameters.Add("nombre_subsistema", System.Data.SqlDbType.VarChar, 70).Value = nombre_subsistema;
                cmd.Parameters.Add("nombre_componente", System.Data.SqlDbType.VarChar, 70).Value = nombre_componente;

                if ((serie_componente != null) && (serie_componente.Length > 0))
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = serie_componente;
                }
                else
                {
                    cmd.Parameters.Add("serie_componente", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }


                if ((parte != null) && (parte.Length > 0))
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = parte;
                }
                else
                {
                    cmd.Parameters.Add("parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if ((numero_parte != null) && (numero_parte.Length > 0))
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = numero_parte;
                }
                else
                {
                    cmd.Parameters.Add("numero_parte", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                if (horas >= 0)
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = horas;
                }
                else
                {
                    cmd.Parameters.Add("horas", SqlDbType.Int).Value = DBNull.Value;
                }

                cmd.Parameters.Add("nombre_clasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_clasificacion;
                cmd.Parameters.Add("nombre_subclasificacion", System.Data.SqlDbType.VarChar, 70).Value = nombre_subclasificacion;

                if ((agente_corrector != null) && (agente_corrector.Length > 0))
                {
                    cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = agente_corrector;
                }
                else
                {
                    cmd.Parameters.Add("agente_corrector", System.Data.SqlDbType.VarChar, 70).Value = DBNull.Value;
                }

                cmd.Parameters.Add("detalle", System.Data.SqlDbType.VarChar, 3000).Value = detalle;
                cmd.Parameters.Add("probabilidad", System.Data.SqlDbType.VarChar, 50).Value = probabilidad;
                cmd.Parameters.Add("consecuencia", System.Data.SqlDbType.VarChar, 50).Value = consecuencia;
                cmd.Parameters.Add("irc", System.Data.SqlDbType.Decimal).Value = irc;
                cmd.Parameters.Add("criticidad", System.Data.SqlDbType.VarChar, 70).Value = criticidad;
                cmd.Parameters.Add("fecha_cierre", System.Data.SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("estado", System.Data.SqlDbType.VarChar, 70).Value = "Registrado";

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }


                //Ingresar JefeArea en Evento
                {

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = jefe_area.getRut();
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "JefeArea";
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = jefe_area.getAntiguedad();


                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló el registro del jefe de área del Evento";
                    }
                }


                //Ingresar Archivos
                foreach (Archivo archivo in listArchivos)
                {
                    //Filtra archivos de imagen y las convierte en PNG
                    {
                        Utils.convertImageFileToJpeg(archivo);
                    }

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();
                    cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 90).Value = archivo.getNombre();
                    cmd.Parameters.Add("tamano", System.Data.SqlDbType.VarChar, 30).Value = archivo.getSize();
                    cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                    cmd.Parameters.Add("contenido", System.Data.SqlDbType.VarBinary).Value = archivo.getContenido();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }


                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("id_archivo", System.Data.SqlDbType.VarChar, 40).Value = archivo.getIdArchivo();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el archivo \"" + archivo.getNombre() + "\"";
                    }

                }


                //Ingresar Creador
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                    cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = owner.getRut();
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Creador";
                    cmd.Parameters.Add("antiguedad", System.Data.SqlDbType.Int).Value = owner.getAntiguedad();

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Fallo al registrar el creador del Evento";
                    }
                }


                if (!setEstadoEvento(cmd, codigo, estado))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el estado del nuevo Evento";
                }


                if (!ActionLogger.evento(cmd, codigo, usuario, "Crear Evento", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el nuevo Evento";
                }

                EmailSender.sendMailEventoCreado(owner.getRut(), codigo);

                return "CODIGO:" + codigo;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el nuevo Evento";
            }

        }



        public static string updateUsuario
        (
            string nombre_usuario,
            string nombre_rol,
            string email,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (nombre_usuario == null)
            {
                return "Usuario inválido";
            }


            if (usuarioExists(nombre_usuario) < 1)
            {
                return "No existe el Usuario";
            }

            if (nombre_rol == null)
            {
                return "Rol inválido";
            }

            if (LogicController.rolExists(nombre_rol) < 1)
            {
                return "Rol inválido";
            }


            if (email != null)
            {
                if (!Utils.validateEmail(email))
                {
                    return "Email invalido";
                }
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }

            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar el Usuario";
                }


                cmd.Parameters.Clear();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar el Usuario";
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Actualizar Usuario (" + nombre_usuario + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar el Usuario";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar el Usuario";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al actualizar el Usuario";
            }

        }



        public static string removeUsuario
        (
            string nombre_usuario,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (nombre_usuario == null)
            {
                return "Usuario inválido";
            }

            if (usuarioExists(nombre_usuario) < 1)
            {
                return "Usuario inválido";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }


            if (usuario == nombre_usuario)
            {
                return "No puedes eliminar tu propia cuenta de Usuario";
            }


            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                //Eliminar Persona
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("nombre_usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la eliminación del Usuario";
                    }
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Eliminar Usuario (" + nombre_usuario + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al eliminar el Usuario";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación del Usuario";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación de la Persona";
            }
        }



        public static string registerUsuarioClientePersona
        (
            string nombre_cliente,
            string nombre_usuario,
            string clave,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {

            if (nombre_cliente == null)
            {
                return "Nombre de cliente inválido";
            }

            int exists = clienteExists(nombre_cliente);
            if (exists < 1)
            {
                return "No se puede recuperar información del Cliente";
            }


            exists = clientePersonaExists(nombre_cliente);
            if (exists < 0)
            {
                return "No se puede recuperar información del cliente";
            }
            else if (exists == 0)
            {
                return "No existe una cuenta asociada al cliente";
            }


            Persona persona = LogicController.getClientePersona(nombre_cliente);
            if (persona == null)
            {
                return "No se puede recuperar información de la persona";
            }

            exists = LogicController.hasPersonaUsuario(persona.getRut());
            if (exists < 0)
            {
                return "No se puede recuperar información de la persona";
            }
            else if (exists > 0)
            {
                return "El Cliente ya tiene asociado un Usuario";
            }


            if (nombre_usuario == null)
            {
                return "Usuario inválido";
            }


            if (usuarioExists(nombre_usuario) > 0)
            {
                return "Ya existe un Usuario con el nombre ingresado";
            }

            if (clave == null)
            {
                return "Clave inválida";
            }


            string clave_md5 = Utils.getMD5Hash(clave);
            string clave_sha1 = Utils.getSHA1Hash(clave);
            if ((clave_md5 == null) || (clave_sha1 == null))
            {
                return "Error al encriptar la clave";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }

            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;
                cmd.Parameters.Add("clave", SqlDbType.VarChar, 30).Value = clave;
                cmd.Parameters.Add("clave_md5", SqlDbType.VarChar, 40).Value = clave_md5;
                cmd.Parameters.Add("clave_sha1", SqlDbType.VarChar, 40).Value = clave_sha1;
                cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = "Cliente";
                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = persona.getRut();


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el Usuario";
                }



                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Crear Usuario (" + nombre_usuario + ") [RUT Cliente:" + persona.getRut() + "]", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el Usuario";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar la Persona";
                }


                EmailSender.sendMailUsuarioCreado(nombre_usuario);

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el Usuario";
            }

        }




        public static string registerUsuario
        (
            string nombre_usuario,
            string clave,
            string nombre_rol,
            string rut_persona,
            string email,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (nombre_usuario == null)
            {
                return "Usuario inválido";
            }


            if (usuarioExists(nombre_usuario) > 0)
            {
                return "Ya existe un Usuario con el nombre ingresado";
            }

            if (clave == null)
            {
                return "Clave inválida";
            }


            string clave_md5 = Utils.getMD5Hash(clave);
            string clave_sha1 = Utils.getSHA1Hash(clave);
            if ((clave_md5 == null) || (clave_sha1 == null))
            {
                return "Error al encriptar la clave";
            }


            if (nombre_rol == null)
            {
                return "Rol inválido";
            }

            if (LogicController.rolExists(nombre_rol) < 1)
            {
                return "Rol inválido";
            }


            if (rut_persona == null)
            {
                return "Persona inválida";
            }
            else
            {
                rut_persona = rut_persona.Replace(".", "");

                if (!Utils.validateRUTPersona(rut_persona))
                {
                    return "RUT o identificador de Persona inválido";
                }
            }


            Persona persona = LogicController.getPersona(rut_persona);
            if (persona == null)
            {
                return "Persona inválida";
            }


            int exists_persona = LogicController.hasPersonaUsuario(rut_persona);
            if (exists_persona < 0)
            {
                return "No se puede recuperar información de la persona";
            }
            else if (exists_persona > 0)
            {
                return "La Persona ya tiene asociado un Usuario";
            }


            if (email != null)
            {
                if (!Utils.validateEmail(email))
                {
                    return "Email invalido";
                }
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }

            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;
                cmd.Parameters.Add("clave", SqlDbType.VarChar, 30).Value = clave;
                cmd.Parameters.Add("clave_md5", SqlDbType.VarChar, 40).Value = clave_md5;
                cmd.Parameters.Add("clave_sha1", SqlDbType.VarChar, 40).Value = clave_sha1;

                if (!persona.getClasificacion().ToUpper().Equals("RSP"))
                {
                    cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = nombre_rol;
                }
                else
                {
                    cmd.Parameters.Add("nombre_rol", SqlDbType.VarChar, 30).Value = "RSP";
                }


                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut_persona;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el Usuario";
                }


                cmd.Parameters.Clear();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut_persona;

                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el Usuario";
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Crear Usuario (" + nombre_usuario + ") [RUT Persona:" + rut_persona + "]", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar el Usuario";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar la Persona";
                }

                EmailSender.sendMailUsuarioCreado(nombre_usuario);

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar el Usuario";
            }

        }






        public static string updatePersona
        (
            string rut,
            string nombres,
            string apellidos,
            string fecha_nacimiento,
            string sexo,
            string id_empleado,
            string id_centro,
            string id_store,
            string lob,
            string cargo,
            string nombre_clasificacionpersona,
            string rut_supervisor,
            string email,
            string fecha_ingreso,
            string fecha_retiro,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (rut == null)
            {
                return "RUT inválido";
            }
            else
            {
                rut = rut.Replace(".", "");

                if (!Utils.validateRUTPersona(rut))
                {
                    return "RUT o identificador de Persona inválido";
                }
            }

            Persona previous_persona = LogicController.getPersona(rut);
            if (previous_persona == null)
            {
                return "La Persona no existe";
            }


            if (!previous_persona.getIDCentro().ToUpper().Equals(id_centro.ToUpper()))
            {
                //Si cambia de centro
                if (LogicController.isPersonaJefeArea(rut) > 0)
                {
                    return "No se puede cambiar de centro una Persona que figure como jefe de área";
                }

                if (LogicController.isPersonaSupervisorSubarea(rut) > 0)
                {
                    return "No se puede cambiar de centro una Persona que figure como supervisor de sub-área";
                }

                int exists_usuario = LogicController.hasPersonaUsuario(previous_persona.getRut());
                if (exists_usuario < 0)
                {
                    return "No se puede recuperar información de los usuarios asociados a la Persona";
                }
                else if (exists_usuario > 0)
                {
                    //Si tiene usuario asociado

                    if (previous_persona.getIDCentro().ToUpper().Equals("RSP"))
                    {
                        //Si era RSP y ahora clasificará distinto
                        return "No se puede cambiar de centro a un RSP mientras tenga asociado un usuario";
                    }
                    else if (id_centro.ToUpper().Equals("RSP"))
                    {
                        //Si ahora será RSP
                        return "No se puede cambiar la clasificación de Persona a RSP mientras tenga asociado un usuario";
                    }
                }
            }


            if ((nombres == null) || (nombres.Length < 1))
            {
                return "Nombres inválidos";
            }


            if (!Utils.isFullMatched(nombres, "(([A-z]|[ÑñÁáÉéÍíÓóÚúÜü])+( )?)+([A-z])+"))
            {
                return "Nombres inválidos";
            }


            if ((apellidos == null) || (apellidos.Length < 1))
            {
                return "Apellidos inválidos";
            }

            if (!Utils.isFullMatched(apellidos, "(([A-z]|[ÑñÁáÉéÍíÓóÚúÜü])+( )?)+([A-z])+"))
            {
                return "Apellidos inválidos";
            }


            string nombre = apellidos + ", " + nombres;

            if (!Utils.validateFecha(fecha_nacimiento))
            {
                return "La fecha de nacimiento es inválida";
            }

            if (Convert.ToDateTime(fecha_nacimiento) > DateTime.Now)
            {
                return "La fecha de nacimiento no puede ser futura";
            }


            if (Convert.ToDateTime(fecha_ingreso) < Convert.ToDateTime(fecha_nacimiento))
            {
                return "La fecha de ingreso no puede ser anterior a la fecha de nacimiento";
            }

            if ((sexo == null) || (sexo.Length != 1))
            {
                return "Sexo inválido";
            }

            if ((!sexo.Equals("M") && (!sexo.Equals("F"))))
            {
                return "Sexo inválido";
            }


            if ((id_empleado == null) || (id_empleado.Length < 1))
            {
                return "ID de Empleado inválido";
            }

            string rut_empleado = LogicController.getRutEmpleado(id_empleado);
            if (rut_empleado != null)
            {
                if (!rut_empleado.Equals(rut))
                {
                    return "Ya existe una persona con ese ID de empleado";
                }
            }


            if (!nombre_clasificacionpersona.ToUpper().Equals("RSP"))
            {
                //Si no es RSP


                if ((id_centro == null) || (id_centro.Length < 1))
                {
                    return "Centro inválido";
                }

                if (centroExists(id_centro) < 1)
                {
                    return "El Centro no existe";
                }

                if ((id_store == null) || (id_store.Length < 1))
                {
                    return "Store inválido";
                }

                if (storeExists(id_store) < 1)
                {
                    return "El Store no existe";
                }

                if ((lob == null) || (lob.Length < 1))
                {
                    return "Lob inválido";
                }

            }



            if ((cargo == null) || (cargo.Length < 1))
            {
                return "Cargo inválido";
            }


            if ((nombre_clasificacionpersona == null) || (nombre_clasificacionpersona.Length < 1))
            {
                return "Clasificación inválida";
            }


            if (rut_supervisor == null)
            {
                return "RUT de Supervisor inválido";
            }


            PersonaInfo supervisor;
            if (rut_supervisor.Length > 0)
            {
                supervisor = LogicController.getPersonaInfo(rut_supervisor);
                if (supervisor == null)
                {
                    return "El Supervisor indicado no existe";
                }
            }
            else
            {
                supervisor = null;
            }



            if (email != null)
            {
                if (!Utils.validateEmail(email))
                {
                    return "Email invalido";
                }
            }


            if (!Utils.validateFecha(fecha_ingreso))
            {
                return "La fecha de ingreso es inválida";
            }

            if (Convert.ToDateTime(fecha_ingreso) > DateTime.Now)
            {
                return "La fecha de ingreso no puede ser futura";
            }


            if (fecha_retiro != null)
            {
                if (!Utils.validateFecha(fecha_retiro))
                {
                    return "La fecha de retiro es inválida";
                }

                if (Convert.ToDateTime(fecha_retiro) > DateTime.Now)
                {
                    return "La fecha de retiro no puede ser futura";
                }

                if (Convert.ToDateTime(fecha_retiro) < Convert.ToDateTime(fecha_ingreso))
                {
                    return "La fecha de ingreso no puede ser posterior a la de retiro";
                }
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("rut", SqlDbType.VarChar, 15).Value = rut;
                cmd.Parameters.Add("nombre", SqlDbType.VarChar, 120).Value = nombre;
                cmd.Parameters.Add("fecha_nacimiento", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_nacimiento);
                cmd.Parameters.Add("sexo", SqlDbType.VarChar, 1).Value = sexo;
                cmd.Parameters.Add("id_empleado", SqlDbType.VarChar, 30).Value = id_empleado;

                if (!nombre_clasificacionpersona.ToUpper().Equals("RSP"))
                {
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("id_store", SqlDbType.VarChar, 10).Value = id_store;
                    cmd.Parameters.Add("lob", SqlDbType.VarChar, 10).Value = lob;
                }
                else
                {
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = "RSP";
                    cmd.Parameters.Add("id_store", SqlDbType.VarChar, 10).Value = DBNull.Value;
                    cmd.Parameters.Add("lob", SqlDbType.VarChar, 10).Value = DBNull.Value;
                }

                cmd.Parameters.Add("cargo", SqlDbType.VarChar, 70).Value = cargo;
                cmd.Parameters.Add("nombre_clasificacionpersona", SqlDbType.VarChar, 30).Value = nombre_clasificacionpersona;

                if (supervisor != null)
                {
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = supervisor.getRut();
                }
                else
                {
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = DBNull.Value;
                }

                cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("fecha_ingreso", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_ingreso);
                if (fecha_retiro != null)
                {
                    cmd.Parameters.Add("fecha_retiro", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_retiro);
                }
                else
                {
                    cmd.Parameters.Add("fecha_retiro", SqlDbType.DateTime).Value = DBNull.Value;
                }


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar la Persona";
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Actualizar Persona (RUT:" + rut + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar la Persona";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar la Persona";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción alactualizar a la Persona";
            }

        }



        public static string removePersona
        (
            string rut,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (rut == null)
            {
                return "RUT inválido";
            }

            if (LogicController.getPersonaInfo(rut) == null)
            {
                return "Persona inválida";
            }

            if (LogicController.hasPersonaUsuario(rut) > 0)
            {
                return "No se puede eliminar una Persona que tenga un Usuario activo";
            }

            if (LogicController.isPersonaJefeArea(rut) > 0)
            {
                return "No se puede eliminar una Persona que figure como jefe de área";
            }

            if (LogicController.isPersonaSupervisorSubarea(rut) > 0)
            {
                return "No se puede eliminar una Persona que figure como supervisor de sub-área";
            }

            if (LogicController.isPersonaSupervisorPersona(rut) > 0)
            {
                return "No se puede eliminar una Persona que figure como Supervisor de otro";
            }

            if (LogicController.isPersonaEventoCreador(rut) > 0)
            {
                return "No se puede eliminar Personas que han creado Eventos";
            }

            if (LogicController.isPersonaEventoJefeArea(rut) > 0)
            {
                return "No se puede eliminar Personas que figuran como jefe de área en Eventos";
            }

            if (LogicController.isPersonaEventoResponsable(rut) > 0)
            {
                return "No se puede eliminar Personas que figuran como responsable en Eventos";
            }

            if (LogicController.isPersonaEventoInvolucrado(rut) > 0)
            {
                return "No se puede eliminar Personas que figuren como involucradas en Eventos";
            }


            if (LogicController.hasPersonaAccionCorrectiva(rut) > 0)
            {
                return "No se puede eliminar Personas que tengan alguna Acción Correctiva asociada";
            }

            if (LogicController.hasPersonaEvaluacion(rut) > 0)
            {
                return "No se puede eliminar Personas que tengan alguna Evaluación de Evento asociada";
            }

            if (LogicController.hasPersonaInvestigacion(rut) > 0)
            {
                return "No se puede eliminar Personas que tengan alguna Investigación asociada";
            }

            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }

            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = DBController.getNewConnection();
                trans = DBController.getNewTransaction(conn);
                SqlCommand cmd = DBController.getNewCommand(conn, trans);

                //Eliminar Persona
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("rut", SqlDbType.VarChar, 15).Value = rut;

                    if (!DBController.addQuery(cmd))
                    {
                        DBController.doRollback(conn, trans);

                        return "Falló la eliminación de la Persona";
                    }
                }

                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Eliminar Persona (RUT:" + rut + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al actualizar el Usuario";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Falló la eliminación de la Persona";
                }

                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Falló la eliminación de la Persona";
            }
        }



        public static string registerPersona
        (
            string rut,
            string nombres,
            string apellidos,
            string fecha_nacimiento,
            string sexo,
            string id_empleado,
            string id_centro,
            string id_store,
            string lob,
            string cargo,
            string nombre_clasificacionpersona,
            string rut_supervisor,
            string email,
            string fecha_ingreso,
            string fecha_retiro,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {
            if (rut == null)
            {
                return "RUT inválido";
            }
            else
            {
                rut = rut.Replace(".", "");

                if (!Utils.validateRUTPersona(rut))
                {
                    return "RUT o identificador de Persona inválido";
                }
            }

            int exists = LogicController.personaExists(rut);
            if (exists < 0)
            {
                return "No se puede recuperar información de la Persona";
            }
            if (exists > 0)
            {
                return "Ya existe una persona con ese RUT";
            }


            if ((nombres == null) || (nombres.Length < 1))
            {
                return "Nombres inválidos";
            }


            if (!Utils.isFullMatched(nombres, "(([A-z]|[ÑñÁáÉéÍíÓóÚúÜü])+( )?)+([A-z])+"))
            {
                return "Nombres inválidos";
            }


            if ((apellidos == null) || (apellidos.Length < 1))
            {
                return "Apellidos inválidos";
            }


            if (!Utils.isFullMatched(apellidos, "(([A-z]|[ÑñÁáÉéÍíÓóÚúÜü])+( )?)+([A-z])+"))
            {
                return "Apellidos inválidos";
            }


            string nombre = apellidos + ", " + nombres;

            if (!Utils.validateFecha(fecha_nacimiento))
            {
                return "La fecha de nacimiento es inválida";
            }

            if (Convert.ToDateTime(fecha_nacimiento) > DateTime.Now)
            {
                return "La fecha de nacimiento no puede ser futura";
            }


            if (Convert.ToDateTime(fecha_ingreso) < Convert.ToDateTime(fecha_nacimiento))
            {
                return "La fecha de ingreso no puede ser anterior a la fecha de nacimiento";
            }


            if ((sexo == null) || (sexo.Length != 1))
            {
                return "Sexo inválido";
            }

            if ((!sexo.Equals("M") && (!sexo.Equals("F"))))
            {
                return "Sexo inválido";
            }


            if ((nombre_clasificacionpersona == null) || (nombre_clasificacionpersona.Length < 1))
            {
                return "Clasificación inválida";
            }


            if ((id_empleado == null) || (id_empleado.Length < 1))
            {
                return "ID de Empleado inválido";
            }

            if (empleadoExists(id_empleado) > 0)
            {
                return "Ya existe una persona con ese ID de empleado";
            }


            if (!nombre_clasificacionpersona.ToUpper().Equals("RSP"))
            {
                //Si no es RSP


                if ((id_centro == null) || (id_centro.Length < 1))
                {
                    return "Centro inválido";
                }

                if (centroExists(id_centro) < 1)
                {
                    return "El Centro no existe";
                }

                if ((id_store == null) || (id_store.Length < 1))
                {
                    return "Store inválido";
                }

                if (storeExists(id_store) < 1)
                {
                    return "El Store no existe";
                }

                if ((lob == null) || (lob.Length < 1))
                {
                    return "Lob inválido";
                }

            }



            if ((cargo == null) || (cargo.Length < 1))
            {
                return "Cargo inválido";
            }


            if (rut_supervisor == null)
            {
                return "RUT de Supervisor inválido";
            }



            PersonaInfo supervisor;
            if (rut_supervisor.Length > 0)
            {
                supervisor = LogicController.getPersonaInfo(rut_supervisor);
                if (supervisor == null)
                {
                    return "El Supervisor indicado no existe";
                }
            }
            else
            {
                supervisor = null;
            }


            if (email != null)
            {
                if (!Utils.validateEmail(email))
                {
                    return "Email invalido";
                }
            }


            if (!Utils.validateFecha(fecha_ingreso))
            {
                return "La fecha de ingreso es inválida";
            }

            if (Convert.ToDateTime(fecha_ingreso) > DateTime.Now)
            {
                return "La fecha de ingreso no puede ser futura";
            }


            if (fecha_retiro != null)
            {
                if (!Utils.validateFecha(fecha_retiro))
                {
                    return "La fecha de retiro es inválida";
                }

                if (Convert.ToDateTime(fecha_retiro) > DateTime.Now)
                {
                    return "La fecha de retiro no puede ser futura";
                }

                if (Convert.ToDateTime(fecha_retiro) < Convert.ToDateTime(fecha_ingreso))
                {
                    return "La fecha de ingreso no puede ser posterior a la de retiro";
                }
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }


            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("rut", SqlDbType.VarChar, 15).Value = rut;
                cmd.Parameters.Add("nombre", SqlDbType.VarChar, 120).Value = nombre;
                cmd.Parameters.Add("fecha_nacimiento", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_nacimiento);
                cmd.Parameters.Add("sexo", SqlDbType.VarChar, 1).Value = sexo;
                cmd.Parameters.Add("id_empleado", SqlDbType.VarChar, 30).Value = id_empleado;

                if (!nombre_clasificacionpersona.ToUpper().Equals("RSP"))
                {
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("id_store", SqlDbType.VarChar, 10).Value = id_store;
                    cmd.Parameters.Add("lob", SqlDbType.VarChar, 10).Value = lob;
                }
                else
                {
                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = "RSP";
                    cmd.Parameters.Add("id_store", SqlDbType.VarChar, 10).Value = DBNull.Value;
                    cmd.Parameters.Add("lob", SqlDbType.VarChar, 10).Value = DBNull.Value;
                }

                cmd.Parameters.Add("cargo", SqlDbType.VarChar, 70).Value = cargo;
                cmd.Parameters.Add("nombre_clasificacionpersona", SqlDbType.VarChar, 30).Value = nombre_clasificacionpersona;

                if (supervisor != null)
                {
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = supervisor.getRut();
                }
                else
                {
                    cmd.Parameters.Add("rut_supervisor", SqlDbType.VarChar, 15).Value = DBNull.Value;
                }

                cmd.Parameters.Add("email", SqlDbType.VarChar, 70).Value = email;
                cmd.Parameters.Add("fecha_ingreso", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_ingreso);
                if (fecha_retiro != null)
                {
                    cmd.Parameters.Add("fecha_retiro", SqlDbType.DateTime).Value = Convert.ToDateTime(fecha_retiro);
                }
                else
                {
                    cmd.Parameters.Add("fecha_retiro", SqlDbType.DateTime).Value = DBNull.Value;
                }


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar la Persona";
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), usuario, "Registrar Persona (RUT:" + rut + ")", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar la Persona";
                }


                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al registrar la Persona";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al registrar a la Persona";
            }

        }


        public static bool setEstadoEvento(SqlCommand cmd, string codigo_evento, string estado)
        {
            if (cmd == null)
                return false;

            if (estado == null)
                return false;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Clear();
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                if (DBController.addQuery(cmd))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static string getNewCodigoEvento(SqlCommand cmd, string id_centro)
        {
            if (id_centro == null)
                return null;

            if (cmd == null)
                return null;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;

                sdr = cmd.ExecuteReader();
                if ((sdr.HasRows) && (sdr.Read()))
                {
                    int ultimo_anio = sdr.GetInt32(0);
                    int ultimo_evento = sdr.GetInt32(1);

                    sdr.Close();

                    if (ultimo_anio == DateTime.Now.Year)
                    {
                        ultimo_evento++;

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("ultimo_evento", System.Data.SqlDbType.Int).Value = ultimo_evento;
                        cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 30).Value = id_centro;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        ultimo_anio = DateTime.Now.Year;
                        ultimo_evento = 1;

                        cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("ultimo_anio", System.Data.SqlDbType.Int).Value = ultimo_anio;
                        cmd.Parameters.Add("ultimo_evento", System.Data.SqlDbType.Int).Value = ultimo_evento;
                        cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 30).Value = id_centro;

                        cmd.ExecuteNonQuery();

                    }

                    string ultimo_eventoS = Convert.ToString(ultimo_evento);
                    while (ultimo_eventoS.Length < 4)
                        ultimo_eventoS = "0" + ultimo_eventoS;

                    string codigo_evento = id_centro + Convert.ToString(ultimo_anio) + ultimo_eventoS;


                    return codigo_evento;
                }
                else
                {
                    sdr.Close();

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (sdr != null)
                    sdr.Close();

                return null;
            }

        }



        public static List<ConfigEmailSender> getListConfigEmailSender(string estado)
        {
            if (estado == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("estado", System.Data.SqlDbType.VarChar, 70).Value = estado;
                SqlDataReader sdr = cmd.ExecuteReader();

                List<ConfigEmailSender> listConfigEmailSender = new List<ConfigEmailSender>();

                if (sdr.HasRows)
                {
                    ConfigEmailSender ces;

                    while (sdr.Read())
                    {
                        bool activo;
                        if (sdr.GetString(6).Equals("Si"))
                        {
                            activo = true;
                        }
                        else if (sdr.GetString(6).Equals("No"))
                        {
                            activo = false;
                        }
                        else
                        {
                            conn.Close();

                            return null;
                        }

                        ces = new ConfigEmailSender(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetInt32(3), sdr.GetInt32(4), sdr.GetInt32(5), activo);

                        listConfigEmailSender.Add(ces);
                    }
                }


                conn.Close();

                return listConfigEmailSender;
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }




        public static ConfigEmailSender getConfigEmailSender(string estado, string nombre_rol, string id_centro)
        {
            if (estado == null)
                return null;

            if (nombre_rol == null)
                return null;

            if (id_centro == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("estado", System.Data.SqlDbType.VarChar, 70).Value = estado;
                cmd.Parameters.Add("nombre_rol", System.Data.SqlDbType.VarChar, 30).Value = nombre_rol;
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    bool activo;
                    if (sdr.GetString(6).Equals("Si"))
                    {
                        activo = true;
                    }
                    else if (sdr.GetString(6).Equals("No"))
                    {
                        activo = false;
                    }
                    else
                    {
                        conn.Close();

                        return null;
                    }


                    ConfigEmailSender ces = new ConfigEmailSender(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetInt32(3), sdr.GetInt32(4), sdr.GetInt32(5), activo);
                    conn.Close();

                    return ces;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static bool setConfiguracionValor(string clave, string valor)
        {
            if (clave == null)
                return false;

            if (valor == null)
                return false;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                cmd.Parameters.Add("valor", System.Data.SqlDbType.VarChar, 100).Value = valor;
                cmd.Parameters.Add("clave", System.Data.SqlDbType.VarChar, 100).Value = clave;

                int rows_affected = cmd.ExecuteNonQuery();
                conn.Close();

                if (rows_affected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return false;
            }
        }


        public static string getConfiguracionValor(string clave)
        {
            if (clave == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("clave", System.Data.SqlDbType.VarChar, 100).Value = clave;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string valor = sdr.GetString(0);
                    conn.Close();

                    return valor;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static string getCodigoPlanAccionAccionCorrectiva(string id_accion_correctiva)
        {
            if (id_accion_correctiva == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_accion_correctiva", System.Data.SqlDbType.VarChar, 40).Value = id_accion_correctiva;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string codigo_planaccion = sdr.GetString(0);
                    conn.Close();

                    return codigo_planaccion;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static int getCantidadEventosCentroAreaSubarea(SqlCommand cmd, string id_centro, string nombre_area, string nombre_subarea)
        {
            if (id_centro == null)
                return -1;

            if (nombre_area == null)
                return -1;

            if (nombre_subarea == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }


        public static int getCantidadEventosCentroArea(SqlCommand cmd, string id_centro, string nombre_area)
        {
            if (id_centro == null)
                return -1;

            if (nombre_area == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }


        public static int getCantidadAreasSubarea(SqlCommand cmd, string nombre_subarea)
        {
            if (nombre_subarea == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }





        public static int getCantidadCentrosArea(SqlCommand cmd, string nombre_area)
        {
            if (nombre_area == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }



        public static int getCantidadEventosCentroCliente(SqlCommand cmd, string id_centro, string nombre_cliente)
        {
            if (id_centro == null)
                return -1;

            if (nombre_cliente == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }




        public static int getCantidadAccionesCorrectivasRealizadas(SqlCommand cmd, string codigo_evento)
        {
            if (codigo_evento == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 100).Value = codigo_evento;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }


        public static int getCantidadAccionesCorrectivas(SqlCommand cmd, string codigo_evento)
        {
            if (codigo_evento == null)
                return -1;

            SqlDataReader sdr = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 100).Value = codigo_evento;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);

                    sdr.Close();
                    return cantidad;
                }
                else
                {
                    sdr.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return -1;
            }
        }



        public static List<string> getListIDCentros()
        {
            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_ext", SqlDbType.VarChar, 70).Value = "EXT";
                cmd.Parameters.Add("id_rsp", SqlDbType.VarChar, 70).Value = "RSP";

                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    List<string> listIDCentro = new List<string>();

                    while (sdr.Read())
                    {
                        listIDCentro.Add(sdr.GetString(0));

                    }


                    sdr.Close();
                    conn.Close();

                    return listIDCentro;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static string getFechaCierreInvestigacion(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 100).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string fecha = sdr.GetDateTime(0).ToShortDateString();
                    conn.Close();

                    return fecha;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static int getCantidadEventos(string id_centro, string estado)
        {
            if (id_centro == null)
            {
                return -1;
            }

            if (LogicController.centroExists(id_centro) < 1)
            {
                return -1;
            }

            if (estado == null)
            {
                return -1;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", System.Data.SqlDbType.VarChar, 70).Value = estado;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    int cantidad = sdr.GetInt32(0);
                    conn.Close();

                    return cantidad;
                }
                else
                {
                    conn.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        public static Archivo downloadFile(string id_archivo)
        {
            //Consultamos la base de datos para recuperar la secuencia de bytes del archivo
            byte[] contenido = null;
            string nombre = null;
            string contentType = null;
            SqlConnection conn = null;


            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_archivo", SqlDbType.VarChar, 40).Value = id_archivo;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                DataSet ds = new System.Data.DataSet();
                sda.Fill(ds);
                System.Data.DataRow row = ds.Tables[0].Rows[0];
                nombre = (string)row["nombre"];
                contenido = (byte[])row["contenido"];

                conn.Close();

                contentType = Utils.getContentType(System.IO.Path.GetExtension(nombre));
                if (contentType == null)
                {
                    return null;
                }

                Archivo archivo = new Archivo(id_archivo, nombre, contenido, null, contentType);
                return archivo;

            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static string updateMatrizSeveridad
        (
            Archivo archivo,
            string id_centro,
            string usuario,
            string ip,
            PersonaInfo owner
        )
        {

            if (archivo == null)
            {
                return "El archivo es inválido";
            }

            if (!archivo.getNombre().ToUpper().EndsWith(".PNG"))
            {
                return "El archivo debe ser una imágen PNG";
            }

            if (centroExists(id_centro) < 1)
            {
                return "Error al recuperar información del Centro";
            }


            if ((usuario == null) || (usuarioExists(usuario) < 1))
            {
                return "No se puede recuperar la información del Usuario";
            }

            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }

            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }



            //La carpeta "Images" debe tener permiso de escritura
            FileStream fs = null;
            try
            {
                string file_out_path = HttpContext.Current.Request.PhysicalApplicationPath + "\\Images\\MatrizConsecuencia\\" + id_centro + ".png";

                fs = new FileStream(file_out_path, FileMode.Create);
                fs.Write(archivo.getContenido(), 0, archivo.getSize());
                fs.Flush();
                fs.Close();

                return null;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }

                return "Error al escribir el archivo";
            }

            {//ActionLogger

                SqlConnection conn = null;
                SqlTransaction trans = null;

                try
                {
                    conn = DBController.getNewConnection();
                    trans = DBController.getNewTransaction(conn);
                    SqlCommand cmd = DBController.getNewCommand(conn, trans);

                    ActionLogger.administracion(cmd, id_centro, usuario, "Actualizar Matriz de Severidad", ip);

                    if (!DBController.doCommit(conn, trans))
                    {
                        DBController.doRollback(conn, trans);
                    }
                }
                catch (Exception ex)
                {

                }
            }


        }


        public static List<MenuItem> getNavigationItems(string rol)
        {
            if (rol == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                    return null;

                SqlCommand cmd = conn.CreateCommand();
                if (rol.Equals("Desarrollador"))
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                }
                else
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("rol", System.Data.SqlDbType.VarChar, 30).Value = rol;
                }


                SqlDataReader sdr = cmd.ExecuteReader();
                MenuItem mc;
                MenuItem mp = null;
                List<MenuItem> listMenuItem = new List<MenuItem>();
                bool firstItem = true;

                string rutaIcono;
                while (sdr.Read())
                {
                    if (sdr.IsDBNull(2))
                        rutaIcono = null;
                    else
                        rutaIcono = "~/Images/Menu/" + sdr.GetString(2);

                    if (sdr.GetInt32(3) == -1)
                    {
                        if (firstItem)
                        {
                            mp = new MenuItem(sdr.GetString(1), null, rutaIcono, "~/Default.aspx");
                            firstItem = false;
                        }
                        else
                        {
                            listMenuItem.Add(mp);
                            mp = new MenuItem(sdr.GetString(1), null, rutaIcono, "~" + sdr.GetString(0));
                        }
                    }
                    else
                    {
                        mc = new MenuItem(sdr.GetString(1), null, null, "~" + sdr.GetString(0));
                        mp.ChildItems.Add(mc);
                    }
                }

                if (mp != null)
                    listMenuItem.Add(mp);

                sdr.Close();
                conn.Close();

                return listMenuItem;

            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static bool saveSesion(string usuario, string usertoken)
        {
            if (usuario == null)
                return false;

            if (usertoken == null)
                return false;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                }
                else
                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                }
                sdr.Close();

                cmd.Parameters.Add("usertoken", System.Data.SqlDbType.VarChar, 50).Value = usertoken;
                cmd.ExecuteNonQuery();

                conn.Close();
                return true;

            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return false;
            }
        }


        public static bool removeSesion(string usuario)
        {
            if (usuario == null)
                return false;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.ExecuteNonQuery();

                conn.Close();
                return true;

            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return false;
            }
        }




        public static bool usuarioExists(SqlCommand cmd, string usuario)
        {
            if (cmd == null)
                return false;

            if (usuario == null)
                return false;

            SqlDataReader sdr = null;
            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Close();

                    return true;
                }
                else
                {
                    sdr.Close();

                    return false;
                }
            }
            catch (Exception e)
            {
                if (sdr != null)
                    sdr.Close();

                return false;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No es supervisor de subárea
         *  1: Si es supervisor de subárea
         */
        public static int isPersonaSupervisorSubarea(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No es jefe de área
         *  1: Si es jefe de área
         */
        public static int isPersonaJefeArea(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No es Supervisor de otra Persona
         *  1: Si es Supervisor de otra Persona
         */
        public static int isPersonaSupervisorPersona(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_supervisor", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Usuario asociado
         *  1: Si tiene Usuario asociado
         */
        public static int hasPersonaUsuario(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Evento involucrado
         *  1: Si tiene Evento involucrado
         */
        public static int isPersonaEventoInvolucrado(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No figura como responsable en Evento
         *  1: Si figura como responsable en Evento
         */
        public static int isPersonaEventoResponsable(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Responsable";
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }




        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No figura como jefe de área en Evento
         *  1: Si figura como jefe de área en Evento
         */
        public static int isPersonaEventoJefeArea(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "JefeArea";
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Evento creado
         *  1: Si tiene Evento creado
         */
        public static int isPersonaEventoCreador(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut;
                cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Creador";
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene AccionCorrectiva asociada
         *  1: Si tiene AccionCorrectiva asociada
         */
        public static int hasPersonaAccionCorrectiva(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Evaluación asociada
         *  1: Si tiene Evaluación asociada
         */
        public static int hasPersonaEvaluacion(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Investigación asociada
         *  1: Si tiene Investigación asociada
         */
        public static int hasPersonaInvestigacion(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_responsable", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe la Verificación
         *  1: Existe la Verificación
         */
        public static int verificacionExists(string codigo)
        {
            if (codigo == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe la acción correctiva
         *  1: Existe la acción correctiva
         */
        public static int accionCorrectivaExists(string id)
        {
            if (id == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 40).Value = id;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }




        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el plan de acción
         *  1: Existe el plan de acción
         */
        public static int planAccionExists(string codigo)
        {
            if (codigo == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe la Evaluación
         *  1: Existe la Evaluación
         */
        public static int evaluacionExists(string codigo)
        {
            if (codigo == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe la Investigación
         *  1: Existe la Investigación
         */
        public static int investigacionExists(string codigo)
        {
            if (codigo == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe la Acción Inmediata
         *  1: Existe la Acción Inmediata
         */
        public static int accionInmediataExists(string codigo)
        {
            if (codigo == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el evento
         *  1: Existe el evento
         */
        public static int eventoExists(string codigo)
        {
            if (codigo == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo", System.Data.SqlDbType.VarChar, 30).Value = codigo;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe la Persona
         *  1: Existe la Persona
         */
        public static int clientePersonaExists(string nombre_cliente)
        {
            if (nombre_cliente == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }




        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Evento asociado
         *  1: Si tiene Evento asociado
         */
        public static int hasClienteEvento(string nombre_cliente)
        {
            if (nombre_cliente == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No tiene Centro asociado
         *  1: Si tiene Centro asociado
         */
        public static int hasClienteCentro(string nombre_cliente)
        {
            if (nombre_cliente == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }




        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el centroCliente
         *  1: Existe el centroCliente
         */
        public static int centroClienteExists(string id_centro, string nombre_cliente)
        {
            if (id_centro == null)
                return -2;

            if (nombre_cliente == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el cliente
         *  1: Existe el cliente
         */
        public static int clienteExists(string nombre)
        {
            if (nombre == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 70).Value = nombre;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el store
         *  1: Existe el store
         */
        public static int storeExists(string id)
        {
            if (id == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 10).Value = id;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el supervisor
         *  1: Existe el centro supervisor
         */
        public static int centroAreaSubareaSupervisorExists(string id_centro, string nombre_area, string nombre_subarea, string rut_supervisor)
        {
            if (id_centro == null)
                return -2;

            if (nombre_area == null)
                return -2;

            if (nombre_subarea == null)
                return -2;

            if (rut_supervisor == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                cmd.Parameters.Add("rut_supervisor", System.Data.SqlDbType.VarChar, 15).Value = rut_supervisor;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el centro área subárea
         *  1: Existe el centro área subárea
         */
        public static int centroAreaSubareaExists(string id_centro, string nombre_area, string nombre_subarea)
        {
            if (id_centro == null)
                return -2;

            if (nombre_area == null)
                return -2;

            if (nombre_subarea == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                cmd.Parameters.Add("nombre_subarea", System.Data.SqlDbType.VarChar, 70).Value = nombre_subarea;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el centro área
         *  1: Existe el centro área
         */
        public static int centroAreaExists(string id_centro, string nombre_area)
        {
            if (id_centro == null)
                return -2;

            if (nombre_area == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el sub-área
         *  1: Existe el sub-área
         */
        public static int subareaExists(string nombre)
        {
            if (nombre == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 70).Value = nombre;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }




        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el área
         *  1: Existe el área
         */
        public static int areaExists(string nombre)
        {
            if (nombre == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre", System.Data.SqlDbType.VarChar, 70).Value = nombre;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el centro
         *  1: Existe el centro
         */
        public static int centroExists(string id)
        {
            if (id == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id", System.Data.SqlDbType.VarChar, 70).Value = id;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        public static string getRutEmpleado(string id_empleado)
        {
            if (id_empleado == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_empleado", System.Data.SqlDbType.VarChar, 30).Value = id_empleado;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string rut = sdr.GetString(0);
                    conn.Close();

                    return rut;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el empleado
         *  1: Existe el empleado
         */
        public static int empleadoExists(string id_empleado)
        {
            if (id_empleado == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_empleado", System.Data.SqlDbType.VarChar, 30).Value = id_empleado;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Excepción db
         * -1: Excepcion db
         *  0: No existe la persona
         *  1: Existe la persona
         */
        public static int personaExists(string rut)
        {
            if (rut == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el rol
         *  1: Existe el rol
         */
        public static int rolExists(string nombre)
        {
            if (nombre == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre_rol", System.Data.SqlDbType.VarChar, 30).Value = nombre;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        public static string resetDefaultPassword(string nombre_usuario, string ip, PersonaInfo owner)
        {
            if (nombre_usuario == null)
            {
                return "Usuario inválido";
            }


            if (usuarioExists(nombre_usuario) < 1)
            {
                return "No existe el Usuario";
            }

            string nueva_clave = "0000";

            string clave_md5 = Utils.getMD5Hash(nueva_clave);
            string clave_sha1 = Utils.getSHA1Hash(nueva_clave);
            if ((clave_md5 == null) || (clave_sha1 == null))
            {
                return "Error al encriptar la clave";
            }



            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }



            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;
                cmd.Parameters.Add("clave", SqlDbType.VarChar, 30).Value = nueva_clave;
                cmd.Parameters.Add("clave_md5", SqlDbType.VarChar, 40).Value = clave_md5;
                cmd.Parameters.Add("clave_sha1", SqlDbType.VarChar, 40).Value = clave_sha1;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al restablecer la clave de acceso";
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), nombre_usuario, "Restablecer clave de acceso", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al restablecer la clave de acceso";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al restablecer la clave de acceso";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al restablecer la clave de acceso";
            }
        }




        public static string changePassword(string nombre_usuario, string clave, string nueva_clave, string ip, PersonaInfo owner)
        {
            if (nombre_usuario == null)
            {
                return "Usuario inválido";
            }


            if (usuarioExists(nombre_usuario) < 1)
            {
                return "No existe el Usuario";
            }

            if (clave == null)
            {
                return "Clave inválida";
            }

            string clave_md5 = Utils.getMD5Hash(nueva_clave);
            string clave_sha1 = Utils.getSHA1Hash(nueva_clave);
            if ((clave_md5 == null) || (clave_sha1 == null))
            {
                return "Error al encriptar la clave";
            }


            if (LogicController.authenticateUsuario(nombre_usuario, clave) < 1)
            {
                return "La clave actual ingresada es incorrecta";
            }

            if (nueva_clave == null)
            {
                return "Nueva clave inválida";
            }

            {
                //Validación de clave
                if (nueva_clave.Length < 8)
                {
                    return "La nueva clave debe tener una longitud mínima de 8 caracteres";
                }

                if (!Utils.isFullMatched(nueva_clave, "([A-z]+[0-9]+)+"))
                {
                    return "La nueva clave debe contener letras, números y comenzar con una letra";
                }
            }



            if ((ip == null) || (ip.Length < 1))
            {
                return "La información del sistema remoto es inválida";
            }


            if (owner == null)
            {
                return "No se puede recuperar tu información";
            }



            Persona owner_persona = LogicController.getPersona(owner.getRut());
            if (owner_persona == null)
            {
                return "No se puede recuperar tu información";
            }


            SqlConnection conn = null;
            SqlTransaction trans = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return "Fallo en la conexión con la BD";
                }

                trans = DBController.getNewTransaction(conn);
                if (trans == null)
                {
                    conn.Close();

                    return "Falló la transacción con la BD";
                }

                SqlCommand cmd = DBController.getNewCommand(conn, trans);
                if (cmd == null)
                {
                    conn.Close();

                    return "Falló el comando con la BD";
                }


                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("usuario", SqlDbType.VarChar, 30).Value = nombre_usuario;
                cmd.Parameters.Add("clave", SqlDbType.VarChar, 30).Value = nueva_clave;
                cmd.Parameters.Add("clave_md5", SqlDbType.VarChar, 40).Value = clave_md5;
                cmd.Parameters.Add("clave_sha1", SqlDbType.VarChar, 40).Value = clave_sha1;


                if (!DBController.addQuery(cmd))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al cambiar la clave de acceso";
                }


                if (!ActionLogger.administracion(cmd, owner_persona.getIDCentro(), nombre_usuario, "Cambiar clave de acceso", ip))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al cambiar la clave de acceso";
                }



                if (!DBController.doCommit(conn, trans))
                {
                    DBController.doRollback(conn, trans);

                    return "Fallo al cambiar la clave de acceso";
                }


                return null;

            }
            catch (Exception ex)
            {
                if ((conn != null) && (trans != null))
                    DBController.doRollback(conn, trans);

                return "Se ha producido una excepción al cambiar la clave de acceso";
            }
        }



        /* -2: Parámetro inválido
         * -1: Excepcion db
         *  0: No existe el usuario
         *  1: Existe el usuario
         */
        public static int usuarioExists(string usuario)
        {
            if (usuario == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        /* -4: Error en cálculo de Hashes
         * -3: Clave nula
         * -2: Usuario nulo
         * -1: Excepcion db
         *  0: Clave incorrecta
         *  1: Autenticado
         */
        public static int authenticateUsuario(string usuario, string clave)
        {
            if (usuario == null)
                return -2;

            if (clave == null)
                return -3;

            string clave_md5 = Utils.getMD5Hash(clave);
            string clave_sha1 = Utils.getSHA1Hash(clave);
            if ((clave_md5 == null) || (clave_sha1 == null))
            {
                return -4;
            }

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.Parameters.Add("clave_md5", System.Data.SqlDbType.VarChar, 40).Value = clave_md5;
                cmd.Parameters.Add("clave_sha1", System.Data.SqlDbType.VarChar, 40).Value = clave_sha1;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        /* -6: Error al guardar el log
         * -5: Error en cálculo de Hashes
         * -4: IP nula
         * -3: Clave nula
         * -2: Usuario nulo
         * -1: Excepcion db
         *  0: Clave incorrecta
         *  1: Autenticado
         */
        public static int authenticateUsuario(string usuario, string clave, string ip)
        {
            if (usuario == null)
                return -2;

            if (clave == null)
                return -3;

            if (ip == null)
                return -4;


            string clave_md5 = Utils.getMD5Hash(clave);
            string clave_sha1 = Utils.getSHA1Hash(clave);
            if ((clave_md5 == null) || (clave_sha1 == null))
            {
                return -5;
            }


            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.Parameters.Add("clave_md5", System.Data.SqlDbType.VarChar, 40).Value = clave_md5;
                cmd.Parameters.Add("clave_sha1", System.Data.SqlDbType.VarChar, 40).Value = clave_sha1;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Close();
                    if (ActionLogger.login(cmd, usuario, ip))
                    {
                        conn.Close();

                        return 1;
                    }
                    else
                    {
                        conn.Close();

                        return -6;
                    }
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }


        public static EmailInfo getEmailInfo(string rut)
        {
            if (rut == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string email;
                    if (!sdr.IsDBNull(0))
                    {
                        email = sdr.GetString(0);
                    }
                    else
                    {
                        email = null;
                    }

                    EmailInfo einfo = new EmailInfo(email, sdr.GetString(1), sdr.GetString(2));
                    conn.Close();

                    return einfo;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static List<AccionCorrectiva> getListAccionesCorrectivasPersona(string rut_persona)
        {

            if (rut_persona == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut_persona;

                SqlDataReader sdr = cmd.ExecuteReader();

                List<AccionCorrectiva> listAccionesCorrectivas = new List<AccionCorrectiva>();
                if (sdr.HasRows)
                {
                    AccionCorrectiva accion_correctiva;
                    while (sdr.Read())
                    {
                        accion_correctiva = LogicController.getAccionCorrectiva(sdr.GetString(0));
                        if (accion_correctiva != null)
                        {
                            listAccionesCorrectivas.Add(accion_correctiva);
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listAccionesCorrectivas;

            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }

        }


        public static List<Investigacion> getListInvestigacionesPersona(string rut_persona)
        {
            if (rut_persona == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut_persona;

                SqlDataReader sdr = cmd.ExecuteReader();

                List<Investigacion> listInvestigaciones = new List<Investigacion>();
                if (sdr.HasRows)
                {
                    Investigacion investigacion;
                    while (sdr.Read())
                    {
                        investigacion = LogicController.getInvestigacion(sdr.GetString(0));
                        if (investigacion != null)
                        {
                            listInvestigaciones.Add(investigacion);
                        }
                    }
                }

                sdr.Close();
                conn.Close();

                return listInvestigaciones;

            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }

        }



        public static Verificacion getVerificacion(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.Parameters.Clear();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 100).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    Verificacion verificacion;
                    if (!sdr.IsDBNull(4))
                    {
                        verificacion = new Verificacion(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetDateTime(4).ToShortDateString());
                    }
                    else
                    {
                        verificacion = new Verificacion(sdr.GetString(0), sdr.GetString(1), null, sdr.GetString(3), sdr.GetDateTime(4).ToShortDateString());
                    }

                    conn.Close();

                    return verificacion;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static AccionCorrectiva getAccionCorrectiva(string id_accion_correctiva)
        {
            if (id_accion_correctiva == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();


                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_accion_correctiva", System.Data.SqlDbType.VarChar, 40).Value = id_accion_correctiva;
                SqlDataReader sdr = cmd.ExecuteReader();

                AccionCorrectiva accion_correctiva;
                string observacion;

                PersonaInfo responsable;
                if (sdr.HasRows)
                {
                    sdr.Read();

                    responsable = LogicController.getPersonaInfo(sdr.GetString(4));
                    if (responsable != null)
                    {
                        if (!sdr.IsDBNull(5))
                        {
                            observacion = sdr.GetString(5);
                        }
                        else
                        {
                            observacion = null;
                        }

                        if (!sdr.IsDBNull(3))
                        {
                            accion_correctiva = new AccionCorrectiva(sdr.GetString(0), sdr.GetString(1), sdr.GetDateTime(2).ToShortDateString(), sdr.GetDateTime(3).ToShortDateString(), responsable, observacion);
                        }
                        else
                        {
                            accion_correctiva = new AccionCorrectiva(sdr.GetString(0), sdr.GetString(1), sdr.GetDateTime(2).ToShortDateString(), null, responsable, observacion);
                        }

                        sdr.Close();
                        conn.Close();

                        return accion_correctiva;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }
                else
                {
                    sdr.Close();
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }

        public static PlanAccion getPlanAccion(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();


                //Acciones correctivas
                List<AccionCorrectiva> listAccionesCorrectivas = new List<AccionCorrectiva>();

                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 15).Value = codigo_evento;
                    SqlDataReader sdr_ac = cmd.ExecuteReader();

                    AccionCorrectiva accion_correctiva;
                    string observacion;

                    PersonaInfo responsable;
                    if (sdr_ac.HasRows)
                    {
                        while (sdr_ac.Read())
                        {
                            responsable = LogicController.getPersonaInfo(sdr_ac.GetString(4));
                            if (responsable != null)
                            {
                                if (!sdr_ac.IsDBNull(5))
                                {
                                    observacion = sdr_ac.GetString(5);
                                }
                                else
                                {
                                    observacion = null;
                                }

                                if (!sdr_ac.IsDBNull(3))
                                {
                                    accion_correctiva = new AccionCorrectiva(sdr_ac.GetString(0), sdr_ac.GetString(1), sdr_ac.GetDateTime(2).ToShortDateString(), sdr_ac.GetDateTime(3).ToShortDateString(), responsable, observacion);
                                }
                                else
                                {
                                    accion_correctiva = new AccionCorrectiva(sdr_ac.GetString(0), sdr_ac.GetString(1), sdr_ac.GetDateTime(2).ToShortDateString(), null, responsable, observacion);
                                }

                                listAccionesCorrectivas.Add(accion_correctiva);
                            }
                            else
                            {
                                sdr_ac.Close();
                                conn.Close();

                                return null;
                            }
                        }
                    }

                    sdr_ac.Close();
                }




                if (listAccionesCorrectivas.Count > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", SqlDbType.VarChar, 30).Value = codigo_evento;

                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        PlanAccion plan_accion = new PlanAccion(sdr.GetString(0), sdr.GetString(1), sdr.GetDateTime(2), sdr.GetInt32(3), listAccionesCorrectivas);

                        sdr.Close();
                        conn.Close();

                        return plan_accion;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }
                else
                {
                    conn.Close();

                    return null;
                }

            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static Evaluacion getEvaluacion(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                //Involucrados
                List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();

                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 15).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";

                    SqlDataReader sdr_pi = cmd.ExecuteReader();

                    PersonaInfo involucrado;
                    if (sdr_pi.HasRows)
                    {
                        while (sdr_pi.Read())
                        {
                            involucrado = LogicController.getPersonaInfo(sdr_pi.GetString(0));
                            if (involucrado != null)
                            {
                                listInvolucrados.Add(involucrado);
                            }
                            else
                            {
                                sdr_pi.Close();
                                conn.Close();

                                return null;
                            }
                        }
                    }

                    sdr_pi.Close();
                }

                cmd.Parameters.Clear();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 100).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    Evaluacion evaluacion;
                    if (!sdr.IsDBNull(9))
                    {
                        evaluacion = new Evaluacion(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), sdr.GetString(6), sdr.GetString(7), sdr.GetString(8), sdr.GetString(9), sdr.GetString(10), sdr.GetInt32(11), sdr.GetDateTime(12).ToShortDateString(), listInvolucrados);
                    }
                    else
                    {
                        evaluacion = new Evaluacion(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), sdr.GetString(6), sdr.GetString(7), sdr.GetString(8), null, sdr.GetString(10), sdr.GetInt32(11), sdr.GetDateTime(12).ToShortDateString(), listInvolucrados);
                    }

                    conn.Close();

                    return evaluacion;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }




        public static Investigacion getInvestigacion(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();


                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                string fecha_cierre;
                string fecha_respuesta;

                PersonaInfo responsable;
                if (sdr.HasRows)
                {
                    sdr.Read();

                    responsable = LogicController.getPersonaInfo(sdr.GetString(1));
                    if (responsable != null)
                    {
                        if (!sdr.IsDBNull(4))
                        {
                            fecha_cierre = sdr.GetDateTime(4).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = null;
                        }

                        if (!sdr.IsDBNull(5))
                        {
                            fecha_respuesta = sdr.GetDateTime(5).ToShortDateString();
                        }
                        else
                        {
                            fecha_respuesta = null;
                        }

                        Investigacion investigacion = new Investigacion(sdr.GetString(0), responsable, sdr.GetInt32(2), sdr.GetDateTime(3).ToShortDateString(), fecha_cierre, fecha_respuesta);

                        sdr.Close();
                        conn.Close();

                        return investigacion;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }
                else
                {
                    sdr.Close();
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static List<Evento> getListEventosAccionInmediataPendienteInspector(string id_centro, string rut_persona)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (rut_persona == null)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("estado", SqlDbType.VarChar, 70).Value = "Acción inmediata pendiente";
                cmd.Parameters.Add("rut_persona", SqlDbType.VarChar, 15).Value = rut_persona;
                SqlDataReader sdr = cmd.ExecuteReader();

                List<Evento> listEventosAccionInmediataPendienteInspector = new List<Evento>();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Evento evento = new Evento(sdr.GetString(0), null, DateTime.Now, sdr.GetDateTime(1), null, null, null, null, null, null, null, null, null, null, null, null, null, null, -1, null, null, null, null, null, -1, null, null);

                        listEventosAccionInmediataPendienteInspector.Add(evento);
                    }
                }

                sdr.Close();
                conn.Close();

                return listEventosAccionInmediataPendienteInspector;
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }



        public static AccionInmediata getAccionInmediata(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                //Involucrados
                List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();

                {
                    cmd.Parameters.Clear();

                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 15).Value = codigo_evento;
                    cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";

                    SqlDataReader sdr_pi = cmd.ExecuteReader();

                    PersonaInfo involucrado;
                    if (sdr_pi.HasRows)
                    {
                        while (sdr_pi.Read())
                        {
                            involucrado = LogicController.getPersonaInfo(sdr_pi.GetString(0));
                            if (involucrado != null)
                            {
                                listInvolucrados.Add(involucrado);
                            }
                            else
                            {
                                sdr_pi.Close();
                                conn.Close();

                                return null;
                            }
                        }
                    }

                    sdr_pi.Close();
                }


                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                AccionInmediata accion_inmediata;

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string observacion;
                    if (!sdr.IsDBNull(11))
                    {
                        observacion = sdr.GetString(11);
                    }
                    else
                    {
                        observacion = null;
                    }

                    accion_inmediata = new AccionInmediata(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), sdr.GetString(6), sdr.GetString(7), sdr.GetString(8), sdr.GetDateTime(9).ToShortDateString(), sdr.GetString(10), observacion, listInvolucrados);
                    sdr.Close();
                    conn.Close();

                    return accion_inmediata;
                }
                else
                {
                    sdr.Close();
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static List<PersonaInfo> getListCreadoresEventos(string id_centro)
        {
            if (id_centro == null)
                return null;


            if (centroExists(id_centro) < 1)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                //Creador
                List<PersonaInfo> listCreadores = new List<PersonaInfo>();

                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 15).Value = id_centro;
                cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Creador";

                SqlDataReader sdr_pi = cmd.ExecuteReader();

                PersonaInfo creador;
                if (sdr_pi.HasRows)
                {
                    while (sdr_pi.Read())
                    {
                        creador = LogicController.getPersonaInfo(sdr_pi.GetString(0));
                        if (creador != null)
                        {
                            listCreadores.Add(creador);
                        }
                        else
                        {
                            sdr_pi.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr_pi.Close();

                conn.Close();

                return listCreadores;
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static List<PersonaInfo> getListInvolucradosEvento(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;


            if (eventoExists(codigo_evento) < 1)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                //Involucrados
                List<PersonaInfo> listInvolucrados = new List<PersonaInfo>();

                cmd.Parameters.Clear();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 15).Value = codigo_evento;
                cmd.Parameters.Add("tipo", System.Data.SqlDbType.VarChar, 30).Value = "Involucrado";

                SqlDataReader sdr_pi = cmd.ExecuteReader();

                PersonaInfo involucrado;
                if (sdr_pi.HasRows)
                {
                    while (sdr_pi.Read())
                    {
                        involucrado = LogicController.getPersonaInfo(sdr_pi.GetString(0));
                        if (involucrado != null)
                        {
                            listInvolucrados.Add(involucrado);
                        }
                        else
                        {
                            sdr_pi.Close();
                            conn.Close();

                            return null;
                        }
                    }
                }

                sdr_pi.Close();

                conn.Close();

                return listInvolucrados;
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static WOInfo getWOInfoByCodigoEvento(string codigo_evento, string id_centro)
        {
            if (codigo_evento == null)
                return null;

            if (id_centro == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string serie_equipo;
                    if (sdr.IsDBNull(6))
                    {
                        serie_equipo = null;
                    }
                    else
                    {
                        serie_equipo = sdr.GetString(6);
                    }


                    string serie_componente;
                    if (sdr.IsDBNull(10))
                    {
                        serie_componente = null;
                    }
                    else
                    {
                        serie_componente = sdr.GetString(10);
                    }

                    string parte;
                    if (sdr.IsDBNull(11))
                    {
                        parte = null;
                    }
                    else
                    {
                        parte = sdr.GetString(11);
                    }

                    string numero_parte;
                    if (sdr.IsDBNull(12))
                    {
                        numero_parte = null;
                    }
                    else
                    {
                        numero_parte = sdr.GetString(12);
                    }

                    int horas;
                    if (sdr.IsDBNull(13))
                    {
                        horas = -1;
                    }
                    else
                    {
                        horas = sdr.GetInt32(13);
                    }


                    WOInfo woInfo = new WOInfo(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), serie_equipo, sdr.GetString(7), sdr.GetString(8), sdr.GetString(9), serie_componente, parte, numero_parte, horas);
                    conn.Close();

                    return woInfo;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static WOInfo getWOInfo(string codigo_wo, string id_centro)
        {
            if (codigo_wo == null)
                return null;

            if (id_centro == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("work_order", System.Data.SqlDbType.VarChar, 30).Value = codigo_wo;
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string serie_equipo;
                    if (sdr.IsDBNull(6))
                    {
                        serie_equipo = null;
                    }
                    else
                    {
                        serie_equipo = sdr.GetString(6);
                    }


                    string serie_componente;
                    if (sdr.IsDBNull(10))
                    {
                        serie_componente = null;
                    }
                    else
                    {
                        serie_componente = sdr.GetString(10);
                    }

                    string parte;
                    if (sdr.IsDBNull(11))
                    {
                        parte = null;
                    }
                    else
                    {
                        parte = sdr.GetString(11);
                    }

                    string numero_parte;
                    if (sdr.IsDBNull(12))
                    {
                        numero_parte = null;
                    }
                    else
                    {
                        numero_parte = sdr.GetString(12);
                    }

                    int horas;
                    if (sdr.IsDBNull(13))
                    {
                        horas = -1;
                    }
                    else
                    {
                        horas = sdr.GetInt32(13);
                    }


                    WOInfo woInfo = new WOInfo(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), serie_equipo, sdr.GetString(7), sdr.GetString(8), sdr.GetString(9), serie_componente, parte, numero_parte, horas);
                    conn.Close();

                    return woInfo;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static Evento getEvento(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();


                    string nombre_area;
                    if (sdr.IsDBNull(6))
                    {
                        nombre_area = null;
                    }
                    else
                    {
                        nombre_area = sdr.GetString(6);
                    }


                    string nombre_subarea;
                    if (sdr.IsDBNull(7))
                    {
                        nombre_subarea = null;
                    }
                    else
                    {
                        nombre_subarea = sdr.GetString(7);
                    }


                    string serie_equipo;
                    if (sdr.IsDBNull(11))
                    {
                        serie_equipo = null;
                    }
                    else
                    {
                        serie_equipo = sdr.GetString(11);
                    }


                    string serie_componente;
                    if (sdr.IsDBNull(15))
                    {
                        serie_componente = null;
                    }
                    else
                    {
                        serie_componente = sdr.GetString(15);
                    }

                    string parte;
                    if (sdr.IsDBNull(16))
                    {
                        parte = null;
                    }
                    else
                    {
                        parte = sdr.GetString(16);
                    }

                    string numero_parte;
                    if (sdr.IsDBNull(17))
                    {
                        numero_parte = null;
                    }
                    else
                    {
                        numero_parte = sdr.GetString(17);
                    }

                    int horas;
                    if (sdr.IsDBNull(18))
                    {
                        horas = -1;
                    }
                    else
                    {
                        horas = sdr.GetInt32(18);
                    }


                    string agente_corrector;
                    if (sdr.IsDBNull(21))
                    {
                        agente_corrector = null;
                    }
                    else
                    {
                        agente_corrector = sdr.GetString(21);
                    }


                    string probabilidad;
                    if (sdr.IsDBNull(22))
                    {
                        probabilidad = null;
                    }
                    else
                    {
                        probabilidad = sdr.GetString(22);
                    }


                    string consecuencia;
                    if (sdr.IsDBNull(23))
                    {
                        consecuencia = null;
                    }
                    else
                    {
                        consecuencia = sdr.GetString(23);
                    }


                    double irc;
                    if (sdr.IsDBNull(24))
                    {
                        irc = 0;
                    }
                    else
                    {
                        irc = Convert.ToDouble(sdr.GetDecimal(24));
                    }


                    string criticidad;
                    if (sdr.IsDBNull(25))
                    {
                        criticidad = null;
                    }
                    else
                    {
                        criticidad = sdr.GetString(25);
                    }


                    Evento evento = new Evento(sdr.GetString(0), sdr.GetString(1), sdr.GetDateTime(2), sdr.GetDateTime(3), sdr.GetString(4), sdr.GetString(5), nombre_area, nombre_subarea, sdr.GetString(8), sdr.GetString(9), sdr.GetString(10), serie_equipo, sdr.GetString(12), sdr.GetString(13), sdr.GetString(14), serie_componente, parte, numero_parte, horas, sdr.GetString(19), sdr.GetString(20), agente_corrector, probabilidad, consecuencia, irc, criticidad, sdr.GetString(26));
                    conn.Close();

                    return evento;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static decimal getIRCEvento(string codigo_evento)
        {
            if (codigo_evento == null)
                return -2;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    decimal irc = sdr.GetDecimal(0);
                    conn.Close();

                    return irc;
                }
                else
                {
                    conn.Close();

                    return -1;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }



        public static PersonaInfo getCreadorEvento(string codigo_evento)
        {
            if (codigo_evento == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("tipo", SqlDbType.VarChar, 30).Value = "Creador";
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string cargo;
                    if (!sdr.IsDBNull(3))
                    {
                        cargo = sdr.GetString(3);
                    }
                    else
                    {
                        cargo = null;
                    }

                    PersonaInfo p = new PersonaInfo(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), cargo, sdr.GetInt32(4));
                    conn.Close();

                    return p;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static string getNombreClienteByClientePersona(string rut_persona)
        {
            if (rut_persona == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut_persona", System.Data.SqlDbType.VarChar, 15).Value = rut_persona;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string nombre_cliente = sdr.GetString(0);

                    conn.Close();

                    return nombre_cliente;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static Persona getClientePersona(string nombre_cliente)
        {
            if (nombre_cliente == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string email;


                    if (sdr.IsDBNull(5))
                    {
                        email = null;
                    }
                    else
                    {
                        email = sdr.GetString(5);
                    }

                    Persona p = new Persona(sdr.GetString(0), sdr.GetString(1), null, sdr.GetString(2), null, sdr.GetString(3), null, null, null, sdr.GetString(4), null, email, sdr.GetDateTime(6).ToShortDateString(), null);
                    conn.Close();

                    return p;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static Persona getPersona(string rut)
        {
            if (rut == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string fecha_nacimento;
                    string id_empleado;
                    string id_store;
                    string lob;
                    string cargo;
                    string rut_supervisor;
                    string email;
                    string fecha_retiro;


                    if (sdr.IsDBNull(2))
                    {
                        fecha_nacimento = null;
                    }
                    else
                    {
                        fecha_nacimento = sdr.GetDateTime(2).ToShortDateString();
                    }


                    if (sdr.IsDBNull(4))
                    {
                        id_empleado = null;
                    }
                    else
                    {
                        id_empleado = sdr.GetString(4);
                    }


                    if (sdr.IsDBNull(6))
                    {
                        id_store = null;
                    }
                    else
                    {
                        id_store = sdr.GetString(6);
                    }


                    if (sdr.IsDBNull(7))
                    {
                        lob = null;
                    }
                    else
                    {
                        lob = sdr.GetString(7);
                    }


                    if (sdr.IsDBNull(8))
                    {
                        cargo = null;
                    }
                    else
                    {
                        cargo = sdr.GetString(8);
                    }


                    if (sdr.IsDBNull(10))
                    {
                        rut_supervisor = null;
                    }
                    else
                    {
                        rut_supervisor = sdr.GetString(10);
                    }


                    if (sdr.IsDBNull(11))
                    {
                        email = null;
                    }
                    else
                    {
                        email = sdr.GetString(11);
                    }

                    if (sdr.IsDBNull(13))
                    {
                        fecha_retiro = null;
                    }
                    else
                    {
                        fecha_retiro = sdr.GetDateTime(13).ToShortDateString();
                    }

                    Persona p = new Persona(sdr.GetString(0), sdr.GetString(1), fecha_nacimento, sdr.GetString(3), id_empleado, sdr.GetString(5), id_store, lob, cargo, sdr.GetString(9), rut_supervisor, email, sdr.GetDateTime(12).ToShortDateString(), fecha_retiro);
                    conn.Close();

                    return p;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }




        public static PersonaInfo getPersonaInfo(string rut)
        {
            if (rut == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rut", System.Data.SqlDbType.VarChar, 15).Value = rut;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string cargo;
                    if (!sdr.IsDBNull(3))
                    {
                        cargo = sdr.GetString(3);
                    }
                    else
                    {
                        cargo = null;
                    }


                    PersonaInfo p = new PersonaInfo(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), cargo, sdr.GetInt32(4));
                    conn.Close();

                    return p;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }



        public static string getClaveAcceso(string usuario)
        {
            if (usuario == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string clave = sdr.GetString(1);
                    conn.Close();

                    return clave;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (SqlException e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }




        public static Usuario getUsuario(string usuario)
        {
            if (usuario == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    Usuario u = new Usuario(sdr.GetString(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), sdr.GetString(4), sdr.GetString(5), sdr.GetString(6), sdr.GetString(7));
                    conn.Close();

                    return u;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (SqlException e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static string getUsuarioFromToken(string usertoken)
        {
            if (usertoken == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("usertoken", System.Data.SqlDbType.VarChar, 50).Value = usertoken;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    string usuario = sdr.GetString(0);
                    conn.Close();

                    return usuario;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (SqlException e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        public static string getNombreCentro(string id_centro)
        {
            if (id_centro == null)
                return null;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    string nombre_centro = sdr.GetString(0);

                    conn.Close();

                    return nombre_centro;
                }
                else
                {
                    conn.Close();

                    return null;
                }
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();

                return null;
            }
        }


        /* -3: Página nula
         * -2: Rol nulo
         * -1: Excepcion db
         *  0: Acceso denegado
         *  1: Acceso permitido
         */
        public static int isPageAllowed(string rol, string pagina, string prefix)
        {
            if (rol == null)
                return -2;

            if (pagina == null)
                return -3;

            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Add("rol", System.Data.SqlDbType.VarChar, 30).Value = rol;
                cmd.Parameters.Add("pagina", System.Data.SqlDbType.VarChar, 70).Value = prefix + pagina;
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    conn.Close();

                    return 1;
                }
                else
                {
                    conn.Close();

                    return 0;
                }
            }
            catch (SqlException e)
            {
                if (conn != null)
                    conn.Close();

                return -1;
            }
        }
    }
}