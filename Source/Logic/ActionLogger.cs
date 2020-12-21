using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NCCSAN.Source.Persistent;

namespace NCCSAN.Source.Logic
{
    public class ActionLogger
    {

        private static bool LOGGER = true;


        public static bool error(string id_centro, string usuario, string mensaje, string pagina_previa, string ip)
        {
            if (id_centro == null)
                return false;

            if (usuario == null)
                return false;

            if (mensaje == null)
                return false;

            if (pagina_previa == null)
                return false;

            if (ip == null)
                return false;

            if (!LOGGER)
                return true;

            SqlConnection conn = null;
            try
            {

                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.Parameters.Add("mensaje", System.Data.SqlDbType.VarChar, 120).Value = mensaje;
                cmd.Parameters.Add("pagina_previa", System.Data.SqlDbType.VarChar, 120).Value = pagina_previa;
                cmd.Parameters.Add("ip", System.Data.SqlDbType.VarChar, 30).Value = ip;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }



        public static bool administracion(SqlCommand cmd, string id_centro, string usuario, string accion, string ip)
        {
            if (cmd == null)
                return false;

            if (id_centro == null)
                return false;

            if (usuario == null)
                return false;

            if (accion == null)
                return false;

            if (ip == null)
                return false;

            if (!LOGGER)
                return true;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();

                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.Parameters.Add("accion", System.Data.SqlDbType.VarChar, 120).Value = accion;
                cmd.Parameters.Add("ip", System.Data.SqlDbType.VarChar, 30).Value = ip;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }


        public static bool evento(SqlCommand cmd, string codigo_evento, string usuario, string accion, string ip)
        {
            if (cmd == null)
                return false;

            if (codigo_evento == null)
                return false;

            if (usuario == null)
                return false;

            if (accion == null)
                return false;

            if (ip == null)
                return false;

            if (!LOGGER)
                return true;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();

                cmd.Parameters.Add("codigo_evento", System.Data.SqlDbType.VarChar, 30).Value = codigo_evento;
                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.Parameters.Add("accion", System.Data.SqlDbType.VarChar, 120).Value = accion;
                cmd.Parameters.Add("ip", System.Data.SqlDbType.VarChar, 30).Value = ip;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }


        public static bool login(SqlCommand cmd, string usuario, string ip)
        {
            if (cmd == null)
                return false;

            if(usuario == null)
                return false;

            if(ip == null)
                return false;

            if(!LOGGER)
                return true;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */
                
                cmd.Parameters.Clear();

                cmd.Parameters.Add("usuario", System.Data.SqlDbType.VarChar, 30).Value = usuario;
                cmd.Parameters.Add("ip", System.Data.SqlDbType.VarChar, 30).Value = ip;
                cmd.Parameters.Add("fecha", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }

        }
    }
}