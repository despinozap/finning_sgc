using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NCCSAN.Source.Persistent;
using NCCSAN.Source.Entity;

namespace NCCSAN.Source.Logic
{
    public class IndicatorBuilder
    {
        public static Dictionary<string, int[]> getGestionPeriodoCreadorResumenCentroCliente(string rut_creador, string id_centro, string nombre_cliente, int anio, int mes_desde, int mes_hasta)
        {
            if (rut_creador == null)
            {
                return null;
            }

            if (id_centro == null)
            {
                return null;
            }

            if (nombre_cliente == null)
            {
                return null;
            }

            if (LogicController.personaExists(rut_creador) < 1)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
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

                Dictionary<string, int[]> resumen = new Dictionary<string, int[]>();

                resumen = getGestionPeriodoCreador_Evento_CentroCliente(cmd, resumen, rut_creador, id_centro, nombre_cliente, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }


                conn.Close();

                return resumen;
            }
            catch (Exception e)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodoCreadorResumenCentro(string rut_creador, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (rut_creador == null)
            {
                return null;
            }

            if (id_centro == null)
            {
                return null;
            }

            if (LogicController.personaExists(rut_creador) < 1)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
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

                Dictionary<string, int[]> resumen = new Dictionary<string, int[]>();

                resumen = getGestionPeriodoCreador_Evento_Centro(cmd, resumen, rut_creador, id_centro, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }


                conn.Close();

                return resumen;
            }
            catch (Exception e)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodoCreadorResumen(string rut_creador, int anio, int mes_desde, int mes_hasta)
        {
            if (rut_creador == null)
            {
                return null;
            }

            if (LogicController.personaExists(rut_creador) < 1)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
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

                Dictionary<string, int[]> resumen = new Dictionary<string, int[]>();

                resumen = getGestionPeriodoCreador_Evento(cmd, resumen, rut_creador, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }
   

                conn.Close();

                return resumen;
            }
            catch (Exception e)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodoCreador_Evento_CentroCliente(SqlCommand cmd, Dictionary<string, int[]> resumen, string rut_creador, string id_centro, string nombre_cliente, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (rut_creador == null)
            {
                return null;
            }

            if (id_centro == null)
            {
                return null;
            }

            if (nombre_cliente == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                int[] values = new int[3];

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("rut_creador", System.Data.SqlDbType.VarChar, 15).Value = rut_creador;
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("nombre_cliente", System.Data.SqlDbType.VarChar, 70).Value = nombre_cliente;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                    cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();


                        if (sdr.GetInt32(2) > 0)
                        {
                            values[0] = sdr.GetInt32(0);
                            values[1] = sdr.GetInt32(1);
                            values[2] = sdr.GetInt32(2);
                        }
                        else
                        {
                            values[0] = 0;
                            values[1] = 0;
                            values[2] = 0;
                        }

                        resumen.Add("Evento", values);
                        sdr.Close();

                        return resumen;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }



        public static Dictionary<string, int[]> getGestionPeriodoCreador_Evento_Centro(SqlCommand cmd, Dictionary<string, int[]> resumen, string rut_creador, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (rut_creador == null)
            {
                return null;
            }

            if (id_centro == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                int[] values = new int[3];

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("rut_creador", System.Data.SqlDbType.VarChar, 15).Value = rut_creador;
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                    cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();


                        if (sdr.GetInt32(2) > 0)
                        {
                            values[0] = sdr.GetInt32(0);
                            values[1] = sdr.GetInt32(1);
                            values[2] = sdr.GetInt32(2);
                        }
                        else
                        {
                            values[0] = 0;
                            values[1] = 0;
                            values[2] = 0;
                        }

                        resumen.Add("Evento", values);
                        sdr.Close();

                        return resumen;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodoCreador_Evento(SqlCommand cmd, Dictionary<string, int[]> resumen, string rut_creador, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (rut_creador == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                int[] values = new int[3];

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("rut_creador", System.Data.SqlDbType.VarChar, 15).Value = rut_creador;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                    cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();


                        if (sdr.GetInt32(2) > 0)
                        {
                            values[0] = sdr.GetInt32(0);
                            values[1] = sdr.GetInt32(1);
                            values[2] = sdr.GetInt32(2);
                        }
                        else
                        {
                            values[0] = 0;
                            values[1] = 0;
                            values[2] = 0;
                        }

                        resumen.Add("Evento", values);
                        sdr.Close();

                        return resumen;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static Dictionary<string, int> getGestionPeriodoIndicadoresAnual(string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (LogicController.centroExists(id_centro) < 1)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
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

                Dictionary<string, int> indicadores = new Dictionary<string, int>();

                int indicador;


                indicador = getGestionPeriodo_DiasPromedioRespuestaCliente(cmd, id_centro, anio, mes_desde, mes_hasta);
                if (indicador >= 0)
                {
                    indicadores.Add("DiasPromedioRespuestaCliente", indicador);
                }
                else
                {
                    conn.Close();

                    return null;
                }


                conn.Close();

                return indicadores;
            }
            catch (Exception e)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static Dictionary<string, int> getGestionPeriodoIndicadoresMensual(string id_centro, int anio, int mes_desde, int mes_hasta, int planaccion_mes, int planaccion_dias)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (LogicController.centroExists(id_centro) < 1)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            if ((planaccion_mes < 1) || (planaccion_mes > 12))
            {
                return null;
            }

            if (planaccion_dias < 0)
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

                Dictionary<string, int> indicadores = new Dictionary<string, int>();

                int indicador;

                indicador = getGestionPeriodo_PlanesAccionAbiertosMasXDias(cmd, id_centro, anio, planaccion_mes, planaccion_dias);
                if (indicador >= 0)
                {
                    indicadores.Add("PlanesAccionAbiertosMasXDias", indicador);
                }
                else
                {
                    conn.Close();

                    return null;
                }


                indicador = getGestionPeriodo_DiasPromedioRespuestaCliente(cmd, id_centro, anio, mes_desde, mes_hasta);
                if (indicador >= 0)
                {
                    indicadores.Add("DiasPromedioRespuestaCliente", indicador);
                }
                else
                {
                    conn.Close();

                    return null;
                }


                conn.Close();

                return indicadores;
            }
            catch (Exception e)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodoResumen(string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (LogicController.centroExists(id_centro) < 1)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
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

                Dictionary<string, int[]> resumen = new Dictionary<string, int[]>();

                resumen = getGestionPeriodo_Evento(cmd, resumen, id_centro, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }


                resumen = getGestionPeriodo_NoConformidad(cmd, resumen, id_centro, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }


                resumen = getGestionPeriodo_Hallazgo(cmd, resumen, id_centro, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }


                resumen = getGestionPeriodo_PlanAccion(cmd, resumen, id_centro, anio, mes_desde, mes_hasta);
                if (resumen == null)
                {
                    conn.Close();

                    return null;
                }


                conn.Close();

                return resumen;
            }
            catch (Exception e)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }
        }


        public static int getGestionPeriodo_PlanesAccionAbiertosMasXDias(SqlCommand cmd, string id_centro, int anio, int mes, int dias)
        {
            if (cmd == null)
            {
                return -1;
            }

            if (id_centro == null)
            {
                return -1;
            }

            if (anio < 0)
            {
                return -1;
            }

            if ((mes < 1) || (mes > 12))
            {
                return -1;
            }

            if ((dias < 1))
            {
                return -1;
            }

            try
            {

                int cantidad;

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes", System.Data.SqlDbType.Int).Value = mes;
                    cmd.Parameters.Add("dias", System.Data.SqlDbType.Int).Value = dias;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        if (!sdr.IsDBNull(0))
                        {
                            cantidad = sdr.GetInt32(0);
                        }
                        else
                        {
                            cantidad = 0;
                        }

                        sdr.Close();

                        return cantidad;
                    }
                    else
                    {
                        sdr.Close();

                        return -1;
                    }
                }

            }
            catch (Exception e)
            {
                return -1;
            }
        }



        public static int getGestionPeriodo_DiasPromedioRespuestaCliente(SqlCommand cmd, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return -1;
            }

            if (id_centro == null)
            {
                return -1;
            }

            if (anio < 0)
            {
                return -1;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return -1;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return -1;
            }

            if (mes_desde > mes_hasta)
            {
                return -1;
            }

            try
            {

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    int cantidad;

                    if (!sdr.IsDBNull(0))
                    {
                        cantidad = sdr.GetInt32(0);
                    }
                    else
                    {
                        cantidad = 0;
                    }

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
                return -1;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodo_PlanAccion(SqlCommand cmd, Dictionary<string, int[]> resumen, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (id_centro == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {

                int[] values = new int[3];

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                    cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        if (sdr.GetInt32(2) > 0)
                        {
                            values[0] = sdr.GetInt32(0);
                            values[1] = sdr.GetInt32(1);
                            values[2] = sdr.GetInt32(2);
                        }
                        else
                        {
                            values[0] = 0;
                            values[1] = 0;
                            values[2] = 0;
                        }

                        sdr.Close();

                        resumen.Add("PlanAccion", values);
                        sdr.Close();

                        return resumen;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodo_Hallazgo(SqlCommand cmd, Dictionary<string, int[]> resumen, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (id_centro == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();

                    int[] values = new int[3];
                    if (sdr.GetInt32(2) > 0)
                    {
                        values[0] = sdr.GetInt32(0);
                        values[1] = sdr.GetInt32(1);
                        values[2] = sdr.GetInt32(2);
                    }
                    else
                    {
                        values[0] = 0;
                        values[1] = 0;
                        values[2] = 0;
                    }

                    resumen.Add("Hallazgo", values);
                    sdr.Close();

                    return resumen;
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
                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodo_NoConformidad(SqlCommand cmd, Dictionary<string, int[]> resumen, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (id_centro == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                int[] values = new int[3];

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                    cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();

                        if (sdr.GetInt32(2) > 0)
                        {
                            values[0] = sdr.GetInt32(0);
                            values[1] = sdr.GetInt32(1);
                            values[2] = sdr.GetInt32(2);
                        }
                        else
                        {
                            values[0] = 0;
                            values[1] = 0;
                            values[2] = 0;
                        }

                        resumen.Add("NoConformidad", values);
                        sdr.Close();

                        return resumen;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static Dictionary<string, int[]> getGestionPeriodo_Evento(SqlCommand cmd, Dictionary<string, int[]> resumen, string id_centro, int anio, int mes_desde, int mes_hasta)
        {
            if (cmd == null)
            {
                return null;
            }


            if (id_centro == null)
            {
                return null;
            }

            if (anio < 0)
            {
                return null;
            }

            if ((mes_desde < 1) || (mes_desde > 12))
            {
                return null;
            }

            if ((mes_hasta < 1) || (mes_hasta > 12))
            {
                return null;
            }

            if (mes_desde > mes_hasta)
            {
                return null;
            }

            SqlConnection conn = null;

            try
            {
                int[] values = new int[3];

                {
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                    cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                    cmd.Parameters.Add("mes_desde", System.Data.SqlDbType.Int).Value = mes_desde;
                    cmd.Parameters.Add("mes_hasta", System.Data.SqlDbType.Int).Value = mes_hasta;

                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();


                        if (sdr.GetInt32(2) > 0)
                        {
                            values[0] = sdr.GetInt32(0);
                            values[1] = sdr.GetInt32(1);
                            values[2] = sdr.GetInt32(2);
                        }
                        else
                        {
                            values[0] = 0;
                            values[1] = 0;
                            values[2] = 0;
                        }

                        resumen.Add("Evento", values);
                        sdr.Close();

                        return resumen;
                    }
                    else
                    {
                        sdr.Close();
                        conn.Close();

                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}