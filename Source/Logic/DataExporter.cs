using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using NCCSAN.Source.Persistent;
using NCCSAN.Source.Entity;

namespace NCCSAN.Source.Logic
{
    public class DataExporter
    {


        public static DataTable getDataTableInvolucradosMensualArea(string id_centro, int anio, int mes, string nombre_area)
        {

			if (id_centro == null)
            {
                return null;
            }

            if (anio < 1)
            {
                return null;
            }

            if ((mes < 1) || (mes > 12))
            {
                return null;
            }
			
			if(nombre_area == null)
			{
				return null;
			}
			
			
            SqlConnection conn = null;
            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("mes", System.Data.SqlDbType.Int).Value = mes;
				cmd.Parameters.Add("nombre_area", System.Data.SqlDbType.VarChar, 70).Value = nombre_area;

                DataTable dt = new DataTable();
                dt.Columns.Add("Código de Evento");
				dt.Columns.Add("W/O");
				dt.Columns.Add("Fecha incidente");
				dt.Columns.Add("Fecha ingreso");
                dt.Columns.Add("Área");
				dt.Columns.Add("Sub-área");
				dt.Columns.Add("RUT involucrado");
				dt.Columns.Add("Nombre involucrado");
                dt.Columns.Add("Descripción");
                dt.Columns.Add("Nombre creador");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    while (sdr.Read())
                    {
                        dr = dt.NewRow();
                        dr[0] = sdr.GetString(0);
						dr[1] = sdr.GetString(1);
						dr[2] = sdr.GetString(2);
						dr[3] = sdr.GetString(3);
						dr[4] = sdr.GetString(4);
						dr[5] = sdr.GetString(5);
						dr[6] = sdr.GetString(6);
						dr[7] = sdr.GetString(7);
						dr[8] = sdr.GetString(8);
						dr[9] = sdr.GetString(9);

                        dt.Rows.Add(dr);
                    }
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static DataTable getDataTableResumenInvolucradosMensual(string id_centro, int anio, int mes)
        {
            if (id_centro == null)
            {
                return null;
            }

            if (anio < 1)
            {
                return null;
            }

            if ((mes < 1) || (mes > 12))
            {
                return null;
            }

            List<string> list_areas = new List<string>();
            list_areas.Add("No conformidad");

            SqlConnection conn = null;

            try
            {
                conn = DBController.getNewConnection();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("mes", System.Data.SqlDbType.Int).Value = mes;

                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        list_areas.Add(sdr.GetString(0));
                    }
                }

                sdr.Close();

                Dictionary<string, int> default_eventos = new Dictionary<string, int>();
                foreach (string nombre_area in list_areas)
                {
                    default_eventos.Add(nombre_area, 0);
                }

                cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                cmd.Parameters.Clear();
                cmd.Parameters.Add("id_centro", System.Data.SqlDbType.VarChar, 70).Value = id_centro;
                cmd.Parameters.Add("anio", System.Data.SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("mes", System.Data.SqlDbType.Int).Value = mes;

                sdr = cmd.ExecuteReader();

                ListInvolucradoBonificacion listInvolucradosBonificacion = new ListInvolucradoBonificacion(default_eventos);
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        listInvolucradosBonificacion.addRegistry(
                                                                    sdr.GetString(0),
                                                                    sdr.GetString(1),
                                                                    sdr.GetString(2),
                                                                    sdr.GetString(3),
                                                                    sdr.GetString(4),
                                                                    sdr.GetInt32(5),
                                                                    sdr.GetInt32(6)
                                                                );
                    }
                }

                sdr.Close();
                conn.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("RUT involucrado");
                dt.Columns.Add("Nombre involucrado");
                dt.Columns.Add("RUT supervisor");
                dt.Columns.Add("Nombre supervisor");
                foreach (string nombre_area in list_areas)
                {
                    dt.Columns.Add(nombre_area);
                }
                dt.Columns.Add("Total");

                DataRow dr;
                int suma_total = 0;
                foreach (InvolucradoBonificacion involucrado_bonificacion in listInvolucradosBonificacion.getListInvolucradoBonificacion())
                {
                    {//Total de eventos por área
                        foreach (string nombre_area in list_areas)
                        {
                            default_eventos[nombre_area] = default_eventos[nombre_area] + involucrado_bonificacion.getCantidadEventos(nombre_area);
                        }
                    }

                    suma_total += involucrado_bonificacion.getTotalEventos();

                    dr = dt.NewRow();

                    dr["RUT involucrado"] = involucrado_bonificacion.getRutInvolucrado();
                    dr["Nombre involucrado"] = involucrado_bonificacion.getNombreInvolucrado();
                    dr["RUT supervisor"] = involucrado_bonificacion.getRutSupervisor();
                    dr["Nombre supervisor"] = involucrado_bonificacion.getNombreSupervisor();

                    foreach (string nombre_area in list_areas)
                    {
                        dr[nombre_area] = involucrado_bonificacion.getCantidadEventos(nombre_area);
                    }

                    dr["Total"] = involucrado_bonificacion.getTotalEventos();

                    dt.Rows.Add(dr);
                }


                {
                    dr = dt.NewRow();

                    foreach (KeyValuePair<string, int> eventos in default_eventos)
                    {
                        dr[eventos.Key] = eventos.Value;
                    }

                    dr["Total"] = suma_total;

                    dt.Rows.Add(dr);
                }


                return dt;
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



        public static DataTable getDataTableXFull(string id_centro)
        {
            if (id_centro == null)
            {
                return null;
            }

            SqlConnection conn = null;
            SqlDataReader sdr = null;
            try
            {
                conn = DBController.getNewConnection();
                if (conn == null)
                {
                    return null;
                }

                SqlCommand cmd = conn.CreateCommand();

                DataTable dt = new DataTable();
                {//DataTable

                    dt.Columns.Add("Tabla");
                    dt.Columns.Add("Evento_codigo");
                    dt.Columns.Add("Evento_work_order");
                    dt.Columns.Add("Evento_fecha_deteccion");
                    dt.Columns.Add("Evento_fecha_ingreso");
                    dt.Columns.Add("Evento_cliente");
                    dt.Columns.Add("Evento_centro");
                    dt.Columns.Add("Evento_pais");
                    dt.Columns.Add("Evento_area");
                    dt.Columns.Add("Evento_subarea");
                    dt.Columns.Add("Evento_fuente");
                    dt.Columns.Add("Evento_tipo_equipo");
                    dt.Columns.Add("Evento_modelo_equipo");
                    dt.Columns.Add("Evento_serie_equipo");
                    dt.Columns.Add("Evento_sistema");
                    dt.Columns.Add("Evento_subsistema");
                    dt.Columns.Add("Evento_componente");
                    dt.Columns.Add("Evento_serie_componente");
                    dt.Columns.Add("Evento_parte");
                    dt.Columns.Add("Evento_numero_parte");
                    dt.Columns.Add("Evento_horas");
                    dt.Columns.Add("Evento_clasificacion");
                    dt.Columns.Add("Evento_subclasificacion");
                    dt.Columns.Add("Evento_detalle");
                    dt.Columns.Add("Evento_probabilidad");
                    dt.Columns.Add("Evento_consecuencia");
                    dt.Columns.Add("Evento_irc");
                    dt.Columns.Add("Evento_criticidad");
                    dt.Columns.Add("Evento_fecha_cierre");
                    dt.Columns.Add("Evento_estado");
                    dt.Columns.Add("Investigacion_rut_responsable");
                    dt.Columns.Add("Investigacion_nombre_responsable");
                    dt.Columns.Add("Investigacion_fecha_inicio");
                    dt.Columns.Add("Investigacion_fecha_cierre");
                    dt.Columns.Add("Evaluacion_fecha");
                    dt.Columns.Add("Evaluacion_origen_falla");
                    dt.Columns.Add("Evaluacion_causa_inmediata");
                    dt.Columns.Add("Evaluacion_subcausa_inmediata");
                    dt.Columns.Add("Evaluacion_causa_basica");
                    dt.Columns.Add("Evaluacion_subcausa_basica");
                    dt.Columns.Add("Evaluacion_aceptado");
                    dt.Columns.Add("Evaluacion_observacion");
                    dt.Columns.Add("Evaluacion_rut_evaluador");
                    dt.Columns.Add("Evaluacion_nombre_evaluador");
                    dt.Columns.Add("Evaluacion_personas_involucradas");
                    dt.Columns.Add("PlanAccion_detalle_correccion");
                    dt.Columns.Add("PlanAccion_fecha_correccion");
                    dt.Columns.Add("PlanAccion_progreso");
                    dt.Columns.Add("PlanAccion_fecha_cierre");
                    dt.Columns.Add("AccionCorrectiva_descripcion");
                    dt.Columns.Add("AccionCorrectiva_fecha_limite");
                    dt.Columns.Add("AccionCorrectiva_fecha_ejecucion");
                    dt.Columns.Add("AccionCorrectiva_observacion");
                    dt.Columns.Add("AccionCorrectiva_rut_responsable");
                    dt.Columns.Add("AccionCorrectiva_nombre_responsable");
                }


                {//Evento
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */

                    cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;


                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataRow dr;
                        string serie_equipo;
                        string serie_componente;
                        string parte;
                        string numero_parte;
                        string horas;
                        string fecha_cierre;
                        while (sdr.Read())
                        {
                            if (!sdr.IsDBNull(12))
                            {
                                serie_equipo = sdr.GetString(12);
                            }
                            else
                            {
                                serie_equipo = "";
                            }

                            if (!sdr.IsDBNull(16))
                            {
                                serie_componente = sdr.GetString(16);
                            }
                            else
                            {
                                serie_componente = "";
                            }

                            if (!sdr.IsDBNull(17))
                            {
                                parte = sdr.GetString(17);
                            }
                            else
                            {
                                parte = "";
                            }

                            if (!sdr.IsDBNull(18))
                            {
                                numero_parte = sdr.GetString(18);
                            }
                            else
                            {
                                numero_parte = "";
                            }

                            if (!sdr.IsDBNull(19))
                            {
                                horas = Convert.ToString(sdr.GetInt32(19));
                            }
                            else
                            {
                                horas = "";
                            }

                            if (!sdr.IsDBNull(27))
                            {
                                fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre = "";
                            }

                            dr = dt.NewRow();
                            dr[0] = "Evento";
                            dr[1] = sdr.GetString(0);
                            dr[2] = sdr.GetString(1);
                            dr[3] = sdr.GetDateTime(2).ToShortDateString();
                            dr[4] = sdr.GetDateTime(3).ToShortDateString();
                            dr[5] = sdr.GetString(4);
                            dr[6] = sdr.GetString(5);
                            dr[7] = sdr.GetString(6);
                            dr[8] = sdr.GetString(7);
                            dr[9] = sdr.GetString(8);
                            dr[10] = sdr.GetString(9);
                            dr[11] = sdr.GetString(10);
                            dr[12] = sdr.GetString(11);
                            dr[13] = serie_equipo;
                            dr[14] = sdr.GetString(13);
                            dr[15] = sdr.GetString(14);
                            dr[16] = sdr.GetString(15);
                            dr[17] = serie_componente;
                            dr[18] = parte;
                            dr[19] = numero_parte;
                            dr[20] = horas;
                            dr[21] = sdr.GetString(20);
                            dr[22] = sdr.GetString(21);
                            dr[23] = sdr.GetString(22);
                            dr[24] = sdr.GetString(23);
                            dr[25] = sdr.GetString(24);
                            dr[26] = sdr.GetDecimal(25).ToString();
                            dr[27] = sdr.GetString(26);
                            dr[28] = fecha_cierre;
                            dr[29] = sdr.GetString(28);


                            dt.Rows.Add(dr);
                        }
                    }

                    sdr.Close();
                }


                {//AccionInmediata
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataRow dr;
                        string serie_equipo;
                        string serie_componente;
                        string parte;
                        string numero_parte;
                        string horas;
                        string fecha_cierre;
                        string codigo_evento;
                        List<PersonaInfo> listInvolucrados;
                        string involucrados;
                        while (sdr.Read())
                        {
                            codigo_evento = sdr.GetString(0);

                            if (!sdr.IsDBNull(12))
                            {
                                serie_equipo = sdr.GetString(12);
                            }
                            else
                            {
                                serie_equipo = "";
                            }

                            if (!sdr.IsDBNull(16))
                            {
                                serie_componente = sdr.GetString(16);
                            }
                            else
                            {
                                serie_componente = "";
                            }

                            if (!sdr.IsDBNull(17))
                            {
                                parte = sdr.GetString(17);
                            }
                            else
                            {
                                parte = "";
                            }

                            if (!sdr.IsDBNull(18))
                            {
                                numero_parte = sdr.GetString(18);
                            }
                            else
                            {
                                numero_parte = "";
                            }

                            if (!sdr.IsDBNull(19))
                            {
                                horas = Convert.ToString(sdr.GetInt32(19));
                            }
                            else
                            {
                                horas = "";
                            }

                            if (!sdr.IsDBNull(27))
                            {
                                fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre = "";
                            }


                            listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                            if (listInvolucrados == null)
                            {
                                return null;
                            }

                            involucrados = "";
                            for (int i = 0; i < listInvolucrados.Count; i++)
                            {
                                involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                                if (i < listInvolucrados.Count - 1)
                                {
                                    involucrados += "; ";
                                }
                            }


                            dr = dt.NewRow();
                            dr[0] = "AccionInmediata";
                            dr[1] = sdr.GetString(0);
                            dr[2] = sdr.GetString(1);
                            dr[3] = sdr.GetDateTime(2).ToShortDateString();
                            dr[4] = sdr.GetDateTime(3).ToShortDateString();
                            dr[5] = sdr.GetString(4);
                            dr[6] = sdr.GetString(5);
                            dr[7] = sdr.GetString(6);
                            dr[8] = sdr.GetString(7);
                            dr[9] = sdr.GetString(8);
                            dr[10] = sdr.GetString(9);
                            dr[11] = sdr.GetString(10);
                            dr[12] = sdr.GetString(11);
                            dr[13] = serie_equipo;
                            dr[14] = sdr.GetString(13);
                            dr[15] = sdr.GetString(14);
                            dr[16] = sdr.GetString(15);
                            dr[17] = serie_componente;
                            dr[18] = parte;
                            dr[19] = numero_parte;
                            dr[20] = horas;
                            dr[21] = sdr.GetString(20);
                            dr[22] = sdr.GetString(21);
                            dr[23] = sdr.GetString(22);
                            dr[24] = sdr.GetString(23);
                            dr[25] = sdr.GetString(24);
                            dr[26] = sdr.GetDecimal(25).ToString();
                            dr[27] = sdr.GetString(26);
                            dr[28] = fecha_cierre;
                            dr[29] = sdr.GetString(28);
                            dr[30] = sdr.GetString(29);
                            dr[31] = sdr.GetString(30);
                            dr[32] = sdr.GetString(31);
                            dr[33] = sdr.GetString(32);
                            dr[34] = sdr.GetString(33);
                            dr[35] = sdr.GetDateTime(34).ToShortDateString();
                            dr[36] = sdr.GetString(35);
                            if (!sdr.IsDBNull(36))
                            {
                                dr[37] = sdr.GetString(36);
                            }
                            else
                            {
                                dr[37] = "";
                            }
                            dr[38] = involucrados;

                            dt.Rows.Add(dr);
                        }
                    }

                    sdr.Close();
                }


                {//Evaluacion
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataRow dr;
                        string serie_equipo;
                        string serie_componente;
                        string parte;
                        string numero_parte;
                        string horas;
                        string fecha_cierre;
                        string codigo_evento;
                        List<PersonaInfo> listInvolucrados;
                        string involucrados;
                        string fecha_cierre_investigacion;
                        string observacion_evaluacion;

                        while (sdr.Read())
                        {
                            codigo_evento = sdr.GetString(0);

                            if (!sdr.IsDBNull(12))
                            {
                                serie_equipo = sdr.GetString(12);
                            }
                            else
                            {
                                serie_equipo = "";
                            }

                            if (!sdr.IsDBNull(16))
                            {
                                serie_componente = sdr.GetString(16);
                            }
                            else
                            {
                                serie_componente = "";
                            }

                            if (!sdr.IsDBNull(17))
                            {
                                parte = sdr.GetString(17);
                            }
                            else
                            {
                                parte = "";
                            }

                            if (!sdr.IsDBNull(18))
                            {
                                numero_parte = sdr.GetString(18);
                            }
                            else
                            {
                                numero_parte = "";
                            }

                            if (!sdr.IsDBNull(19))
                            {
                                horas = Convert.ToString(sdr.GetInt32(19));
                            }
                            else
                            {
                                horas = "";
                            }

                            if (!sdr.IsDBNull(27))
                            {
                                fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre = "";
                            }

                            listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                            if (listInvolucrados == null)
                            {
                                return null;
                            }

                            involucrados = "";
                            for (int i = 0; i < listInvolucrados.Count; i++)
                            {
                                involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                                if (i < listInvolucrados.Count - 1)
                                {
                                    involucrados += "; ";
                                }
                            }

                            if (!sdr.IsDBNull(32))
                            {
                                fecha_cierre_investigacion = sdr.GetDateTime(32).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre_investigacion = "";
                            }

                            if (!sdr.IsDBNull(38))
                            {
                                observacion_evaluacion = sdr.GetString(38);
                            }
                            else
                            {
                                observacion_evaluacion = "";
                            }

                            dr = dt.NewRow();
                            dr[0] = "Evaluacion";
                            dr[1] = sdr.GetString(0);
                            dr[2] = sdr.GetString(1);
                            dr[3] = sdr.GetDateTime(2).ToShortDateString();
                            dr[4] = sdr.GetDateTime(3).ToShortDateString();
                            dr[5] = sdr.GetString(4);
                            dr[6] = sdr.GetString(5);
                            dr[7] = sdr.GetString(6);
                            dr[8] = sdr.GetString(7);
                            dr[9] = sdr.GetString(8);
                            dr[10] = sdr.GetString(9);
                            dr[11] = sdr.GetString(10);
                            dr[12] = sdr.GetString(11);
                            dr[13] = serie_equipo;
                            dr[14] = sdr.GetString(13);
                            dr[15] = sdr.GetString(14);
                            dr[16] = sdr.GetString(15);
                            dr[17] = serie_componente;
                            dr[18] = parte;
                            dr[19] = numero_parte;
                            dr[20] = horas;
                            dr[21] = sdr.GetString(20);
                            dr[22] = sdr.GetString(21);
                            dr[23] = sdr.GetString(22);
                            dr[24] = sdr.GetString(23);
                            dr[25] = sdr.GetString(24);
                            dr[26] = sdr.GetDecimal(25).ToString();
                            dr[27] = sdr.GetString(26);
                            dr[28] = fecha_cierre;
                            dr[29] = sdr.GetString(28);

                            dr[30] = sdr.GetString(29);
                            dr[31] = sdr.GetString(30);
                            dr[32] = sdr.GetDateTime(31).ToShortDateString();
                            dr[33] = sdr.GetDateTime(32).ToShortDateString();
                            dr[34] = sdr.GetDateTime(33).ToShortDateString();
                            dr[35] = sdr.GetString(34);
                            dr[36] = sdr.GetString(35);
                            dr[37] = sdr.GetString(36);
                            dr[38] = sdr.GetString(37);
                            dr[39] = sdr.GetString(38);
                            dr[40] = sdr.GetString(39);
                            if (!sdr.IsDBNull(40))
                            {
                                dr[41] = sdr.GetString(40);
                            }
                            else
                            {
                                dr[41] = "";
                            }
                            dr[42] = sdr.GetString(41);
                            dr[43] = sdr.GetString(42);
                            dr[44] = involucrados;

                            dt.Rows.Add(dr);
                        }
                    }

                    sdr.Close();
                }


                {//PlanAccion
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataRow dr;
                        string serie_equipo;
                        string serie_componente;
                        string parte;
                        string numero_parte;
                        string horas;
                        string fecha_cierre;
                        string codigo_evento;
                        List<PersonaInfo> listInvolucrados;
                        string involucrados;
                        string fecha_cierre_investigacion;
                        string observacion_evaluacion;
                        string fecha_cierre_planaccion;

                        while (sdr.Read())
                        {
                            codigo_evento = sdr.GetString(0);

                            if (!sdr.IsDBNull(12))
                            {
                                serie_equipo = sdr.GetString(12);
                            }
                            else
                            {
                                serie_equipo = "";
                            }

                            if (!sdr.IsDBNull(16))
                            {
                                serie_componente = sdr.GetString(16);
                            }
                            else
                            {
                                serie_componente = "";
                            }

                            if (!sdr.IsDBNull(17))
                            {
                                parte = sdr.GetString(17);
                            }
                            else
                            {
                                parte = "";
                            }

                            if (!sdr.IsDBNull(18))
                            {
                                numero_parte = sdr.GetString(18);
                            }
                            else
                            {
                                numero_parte = "";
                            }

                            if (!sdr.IsDBNull(19))
                            {
                                horas = Convert.ToString(sdr.GetInt32(19));
                            }
                            else
                            {
                                horas = "";
                            }

                            if (!sdr.IsDBNull(27))
                            {
                                fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre = "";
                            }

                            listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                            if (listInvolucrados == null)
                            {
                                return null;
                            }

                            involucrados = "";
                            for (int i = 0; i < listInvolucrados.Count; i++)
                            {
                                involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                                if (i < listInvolucrados.Count - 1)
                                {
                                    involucrados += "; ";
                                }
                            }

                            if (!sdr.IsDBNull(32))
                            {
                                fecha_cierre_investigacion = sdr.GetDateTime(32).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre_investigacion = "";
                            }

                            if (!sdr.IsDBNull(38))
                            {
                                observacion_evaluacion = sdr.GetString(38);
                            }
                            else
                            {
                                observacion_evaluacion = "";
                            }

                            if (!sdr.IsDBNull(46))
                            {
                                fecha_cierre_planaccion = sdr.GetDateTime(46).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre_planaccion = "";
                            }

                            dr = dt.NewRow();
                            dr[0] = "PlanAccion";
                            dr[1] = sdr.GetString(0);
                            dr[2] = sdr.GetString(1);
                            dr[3] = sdr.GetDateTime(2).ToShortDateString();
                            dr[4] = sdr.GetDateTime(3).ToShortDateString();
                            dr[5] = sdr.GetString(4);
                            dr[6] = sdr.GetString(5);
                            dr[7] = sdr.GetString(6);
                            dr[8] = sdr.GetString(7);
                            dr[9] = sdr.GetString(8);
                            dr[10] = sdr.GetString(9);
                            dr[11] = sdr.GetString(10);
                            dr[12] = sdr.GetString(11);
                            dr[13] = serie_equipo;
                            dr[14] = sdr.GetString(13);
                            dr[15] = sdr.GetString(14);
                            dr[16] = sdr.GetString(15);
                            dr[17] = serie_componente;
                            dr[18] = parte;
                            dr[19] = numero_parte;
                            dr[20] = horas;
                            dr[21] = sdr.GetString(20);
                            dr[22] = sdr.GetString(21);
                            dr[23] = sdr.GetString(22);
                            dr[24] = sdr.GetString(23);
                            dr[25] = sdr.GetString(24);
                            dr[26] = sdr.GetDecimal(25).ToString();
                            dr[27] = sdr.GetString(26);
                            dr[28] = fecha_cierre;
                            dr[29] = sdr.GetString(28);

                            dr[30] = sdr.GetString(29);
                            dr[31] = sdr.GetString(30);
                            dr[32] = sdr.GetDateTime(31).ToShortDateString();
                            dr[33] = sdr.GetDateTime(32).ToShortDateString();
                            dr[34] = sdr.GetDateTime(33).ToShortDateString();
                            dr[35] = sdr.GetString(34);
                            dr[36] = sdr.GetString(35);
                            dr[37] = sdr.GetString(36);
                            dr[38] = sdr.GetString(37);
                            dr[39] = sdr.GetString(38);
                            dr[40] = sdr.GetString(39);
                            if (!sdr.IsDBNull(40))
                            {
                                dr[41] = sdr.GetString(40);
                            }
                            else
                            {
                                dr[41] = "";
                            }
                            dr[42] = sdr.GetString(41);
                            dr[43] = sdr.GetString(42);
                            dr[44] = involucrados;

                            dr[45] = sdr.GetString(43);
                            dr[46] = sdr.GetDateTime(44).ToShortDateString();
                            dr[47] = sdr.GetInt32(45) + "%";
                            dr[48] = fecha_cierre_planaccion;

                            dt.Rows.Add(dr);
                        }
                    }

                    sdr.Close();
                }


                {//AccionCorrectiva
                    cmd.CommandText = /* THIS SQL QUERY HAS BEEN HIDDEN FOR CONFIDENTIALITY PURPOSES */


                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataRow dr;
                        string serie_equipo;
                        string serie_componente;
                        string parte;
                        string numero_parte;
                        string horas;
                        string fecha_cierre;
                        string codigo_evento;
                        List<PersonaInfo> listInvolucrados;
                        string involucrados;
                        string fecha_cierre_investigacion;
                        string observacion_evaluacion;
                        string fecha_cierre_planaccion;
                        string fecha_ejecucion_accioncorrectiva;
                        string observacion_accioncorrectiva;

                        while (sdr.Read())
                        {
                            codigo_evento = sdr.GetString(0);

                            if (!sdr.IsDBNull(12))
                            {
                                serie_equipo = sdr.GetString(12);
                            }
                            else
                            {
                                serie_equipo = "";
                            }

                            if (!sdr.IsDBNull(16))
                            {
                                serie_componente = sdr.GetString(16);
                            }
                            else
                            {
                                serie_componente = "";
                            }

                            if (!sdr.IsDBNull(17))
                            {
                                parte = sdr.GetString(17);
                            }
                            else
                            {
                                parte = "";
                            }

                            if (!sdr.IsDBNull(18))
                            {
                                numero_parte = sdr.GetString(18);
                            }
                            else
                            {
                                numero_parte = "";
                            }

                            if (!sdr.IsDBNull(19))
                            {
                                horas = Convert.ToString(sdr.GetInt32(19));
                            }
                            else
                            {
                                horas = "";
                            }

                            if (!sdr.IsDBNull(27))
                            {
                                fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre = "";
                            }

                            listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                            if (listInvolucrados == null)
                            {
                                return null;
                            }

                            involucrados = "";
                            for (int i = 0; i < listInvolucrados.Count; i++)
                            {
                                involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                                if (i < listInvolucrados.Count - 1)
                                {
                                    involucrados += "; ";
                                }
                            }

                            if (!sdr.IsDBNull(32))
                            {
                                fecha_cierre_investigacion = sdr.GetDateTime(32).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre_investigacion = "";
                            }

                            if (!sdr.IsDBNull(38))
                            {
                                observacion_evaluacion = sdr.GetString(38);
                            }
                            else
                            {
                                observacion_evaluacion = "";
                            }

                            if (!sdr.IsDBNull(46))
                            {
                                fecha_cierre_planaccion = sdr.GetDateTime(46).ToShortDateString();
                            }
                            else
                            {
                                fecha_cierre_planaccion = "";
                            }

                            if (!sdr.IsDBNull(49))
                            {
                                fecha_ejecucion_accioncorrectiva = sdr.GetDateTime(49).ToShortDateString();
                            }
                            else
                            {
                                fecha_ejecucion_accioncorrectiva = "";
                            }

                            if (!sdr.IsDBNull(50))
                            {
                                observacion_accioncorrectiva = sdr.GetString(50);
                            }
                            else
                            {
                                observacion_accioncorrectiva = "";
                            }

                            dr = dt.NewRow();
                            dr[0] = "AccionCorrectiva";
                            dr[1] = sdr.GetString(0);
                            dr[2] = sdr.GetString(1);
                            dr[3] = sdr.GetDateTime(2).ToShortDateString();
                            dr[4] = sdr.GetDateTime(3).ToShortDateString();
                            dr[5] = sdr.GetString(4);
                            dr[6] = sdr.GetString(5);
                            dr[7] = sdr.GetString(6);
                            dr[8] = sdr.GetString(7);
                            dr[9] = sdr.GetString(8);
                            dr[10] = sdr.GetString(9);
                            dr[11] = sdr.GetString(10);
                            dr[12] = sdr.GetString(11);
                            dr[13] = serie_equipo;
                            dr[14] = sdr.GetString(13);
                            dr[15] = sdr.GetString(14);
                            dr[16] = sdr.GetString(15);
                            dr[17] = serie_componente;
                            dr[18] = parte;
                            dr[19] = numero_parte;
                            dr[20] = horas;
                            dr[21] = sdr.GetString(20);
                            dr[22] = sdr.GetString(21);
                            dr[23] = sdr.GetString(22);
                            dr[24] = sdr.GetString(23);
                            dr[25] = sdr.GetString(24);
                            dr[26] = sdr.GetDecimal(25).ToString();
                            dr[27] = sdr.GetString(26);
                            dr[28] = fecha_cierre;
                            dr[29] = sdr.GetString(28);

                            dr[30] = sdr.GetString(29);
                            dr[31] = sdr.GetString(30);
                            dr[32] = sdr.GetDateTime(31).ToShortDateString();
                            dr[33] = sdr.GetDateTime(32).ToShortDateString();
                            dr[34] = sdr.GetDateTime(33).ToShortDateString();
                            dr[35] = sdr.GetString(34);
                            dr[36] = sdr.GetString(35);
                            dr[37] = sdr.GetString(36);
                            dr[38] = sdr.GetString(37);
                            dr[39] = sdr.GetString(38);
                            dr[40] = sdr.GetString(39);
                            if (!sdr.IsDBNull(40))
                            {
                                dr[41] = sdr.GetString(40);
                            }
                            else
                            {
                                dr[41] = "";
                            }
                            dr[42] = sdr.GetString(41);
                            dr[43] = sdr.GetString(42);
                            dr[44] = involucrados;

                            dr[45] = sdr.GetString(43);
                            dr[46] = sdr.GetDateTime(44).ToShortDateString();
                            dr[47] = sdr.GetInt32(45) + "%";
                            dr[48] = fecha_cierre_planaccion;

                            dr[49] = sdr.GetString(47);
                            dr[50] = sdr.GetDateTime(48).ToShortDateString();
                            dr[51] = fecha_ejecucion_accioncorrectiva;
                            dr[52] = observacion_accioncorrectiva;
                            dr[53] = sdr.GetString(51);
                            dr[54] = sdr.GetString(52);

                            dt.Rows.Add(dr);
                        }
                    }

                    sdr.Close();
                }

                conn.Close();
                return dt;

            }
            catch (Exception ex)
            {
                if (sdr != null)
                {
                    sdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }

                return null;
            }

        }

        public static DataTable getDataTableXAccionCorrectiva(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Tabla");
                dt.Columns.Add("Evento_codigo");
                dt.Columns.Add("Evento_work_order");
                dt.Columns.Add("Evento_fecha_deteccion");
                dt.Columns.Add("Evento_fecha_ingreso");
                dt.Columns.Add("Evento_cliente");
                dt.Columns.Add("Evento_centro");
                dt.Columns.Add("Evento_pais");
                dt.Columns.Add("Evento_area");
                dt.Columns.Add("Evento_subarea");
                dt.Columns.Add("Evento_fuente");
                dt.Columns.Add("Evento_tipo_equipo");
                dt.Columns.Add("Evento_modelo_equipo");
                dt.Columns.Add("Evento_serie_equipo");
                dt.Columns.Add("Evento_sistema");
                dt.Columns.Add("Evento_subsistema");
                dt.Columns.Add("Evento_componente");
                dt.Columns.Add("Evento_serie_componente");
                dt.Columns.Add("Evento_parte");
                dt.Columns.Add("Evento_numero_parte");
                dt.Columns.Add("Evento_horas");
                dt.Columns.Add("Evento_clasificacion");
                dt.Columns.Add("Evento_subclasificacion");
                dt.Columns.Add("Evento_detalle");
                dt.Columns.Add("Evento_probabilidad");
                dt.Columns.Add("Evento_consecuencia");
                dt.Columns.Add("Evento_irc");
                dt.Columns.Add("Evento_criticidad");
                dt.Columns.Add("Evento_fecha_cierre");
                dt.Columns.Add("Evento_estado");
                dt.Columns.Add("Investigacion_rut_responsable");
                dt.Columns.Add("Investigacion_nombre_responsable");
                dt.Columns.Add("Investigacion_fecha_inicio");
                dt.Columns.Add("Investigacion_fecha_cierre");
                dt.Columns.Add("Evaluacion_fecha");
                dt.Columns.Add("Evaluacion_origen_falla");
                dt.Columns.Add("Evaluacion_causa_inmediata");
                dt.Columns.Add("Evaluacion_subcausa_inmediata");
                dt.Columns.Add("Evaluacion_causa_basica");
                dt.Columns.Add("Evaluacion_subcausa_basica");
                dt.Columns.Add("Evaluacion_aceptado");
                dt.Columns.Add("Evaluacion_observacion");
                dt.Columns.Add("Evaluacion_rut_evaluador");
                dt.Columns.Add("Evaluacion_nombre_evaluador");
                dt.Columns.Add("Evaluacion_personas_involucradas");
                dt.Columns.Add("PlanAccion_detalle_correccion");
                dt.Columns.Add("PlanAccion_fecha_correccion");
                dt.Columns.Add("PlanAccion_progreso");
                dt.Columns.Add("PlanAccion_fecha_cierre");
                dt.Columns.Add("AccionCorrectiva_descripcion");
                dt.Columns.Add("AccionCorrectiva_fecha_limite");
                dt.Columns.Add("AccionCorrectiva_fecha_ejecucion");
                dt.Columns.Add("AccionCorrectiva_observacion");
                dt.Columns.Add("AccionCorrectiva_rut_responsable");
                dt.Columns.Add("AccionCorrectiva_nombre_responsable");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string serie_equipo;
                    string serie_componente;
                    string parte;
                    string numero_parte;
                    string horas;
                    string fecha_cierre;
                    string codigo_evento;
                    List<PersonaInfo> listInvolucrados;
                    string involucrados;
                    string fecha_cierre_investigacion;
                    string observacion_evaluacion;
                    string fecha_cierre_planaccion;
                    string fecha_ejecucion_accioncorrectiva;
                    string observacion_accioncorrectiva;

                    while (sdr.Read())
                    {
                        codigo_evento = sdr.GetString(0);

                        if (!sdr.IsDBNull(12))
                        {
                            serie_equipo = sdr.GetString(12);
                        }
                        else
                        {
                            serie_equipo = "";
                        }

                        if (!sdr.IsDBNull(16))
                        {
                            serie_componente = sdr.GetString(16);
                        }
                        else
                        {
                            serie_componente = "";
                        }

                        if (!sdr.IsDBNull(17))
                        {
                            parte = sdr.GetString(17);
                        }
                        else
                        {
                            parte = "";
                        }

                        if (!sdr.IsDBNull(18))
                        {
                            numero_parte = sdr.GetString(18);
                        }
                        else
                        {
                            numero_parte = "";
                        }

                        if (!sdr.IsDBNull(19))
                        {
                            horas = Convert.ToString(sdr.GetInt32(19));
                        }
                        else
                        {
                            horas = "";
                        }

                        if (!sdr.IsDBNull(27))
                        {
                            fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }

                        listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                        if (listInvolucrados == null)
                        {
                            return null;
                        }

                        involucrados = "";
                        for (int i = 0; i < listInvolucrados.Count; i++)
                        {
                            involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                            if (i < listInvolucrados.Count - 1)
                            {
                                involucrados += "; ";
                            }
                        }

                        if (!sdr.IsDBNull(32))
                        {
                            fecha_cierre_investigacion = sdr.GetDateTime(32).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre_investigacion = "";
                        }

                        if (!sdr.IsDBNull(38))
                        {
                            observacion_evaluacion = sdr.GetString(38);
                        }
                        else
                        {
                            observacion_evaluacion = "";
                        }

                        if (!sdr.IsDBNull(46))
                        {
                            fecha_cierre_planaccion = sdr.GetDateTime(46).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre_planaccion = "";
                        }

                        if (!sdr.IsDBNull(49))
                        {
                            fecha_ejecucion_accioncorrectiva = sdr.GetDateTime(49).ToShortDateString();
                        }
                        else
                        {
                            fecha_ejecucion_accioncorrectiva = "";
                        }

                        if (!sdr.IsDBNull(50))
                        {
                            observacion_accioncorrectiva = sdr.GetString(50);
                        }
                        else
                        {
                            observacion_accioncorrectiva = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = "AccionCorrectiva";
                        dr[1] = sdr.GetString(0);
                        dr[2] = sdr.GetString(1);
                        dr[3] = sdr.GetDateTime(2).ToShortDateString();
                        dr[4] = sdr.GetDateTime(3).ToShortDateString();
                        dr[5] = sdr.GetString(4);
                        dr[6] = sdr.GetString(5);
                        dr[7] = sdr.GetString(6);
                        dr[8] = sdr.GetString(7);
                        dr[9] = sdr.GetString(8);
                        dr[10] = sdr.GetString(9);
                        dr[11] = sdr.GetString(10);
                        dr[12] = sdr.GetString(11);
                        dr[13] = serie_equipo;
                        dr[14] = sdr.GetString(13);
                        dr[15] = sdr.GetString(14);
                        dr[16] = sdr.GetString(15);
                        dr[17] = serie_componente;
                        dr[18] = parte;
                        dr[19] = numero_parte;
                        dr[20] = horas;
                        dr[21] = sdr.GetString(20);
                        dr[22] = sdr.GetString(21);
                        dr[23] = sdr.GetString(22);
                        dr[24] = sdr.GetString(23);
                        dr[25] = sdr.GetString(24);
                        dr[26] = sdr.GetDecimal(25).ToString();
                        dr[27] = sdr.GetString(26);
                        dr[28] = fecha_cierre;
                        dr[29] = sdr.GetString(28);

                        dr[30] = sdr.GetString(29);
                        dr[31] = sdr.GetString(30);
                        dr[32] = sdr.GetDateTime(31).ToShortDateString();
                        dr[33] = sdr.GetDateTime(32).ToShortDateString();
                        dr[34] = sdr.GetDateTime(33).ToShortDateString();
                        dr[35] = sdr.GetString(34);
                        dr[36] = sdr.GetString(35);
                        dr[37] = sdr.GetString(36);
                        dr[38] = sdr.GetString(37);
                        dr[39] = sdr.GetString(38);
                        dr[40] = sdr.GetString(39);
                        if (!sdr.IsDBNull(40))
                        {
                            dr[41] = sdr.GetString(40);
                        }
                        else
                        {
                            dr[41] = "";
                        }
                        dr[42] = sdr.GetString(41);
                        dr[43] = sdr.GetString(42);
                        dr[44] = involucrados;

                        dr[45] = sdr.GetString(43);
                        dr[46] = sdr.GetDateTime(44).ToShortDateString();
                        dr[47] = sdr.GetInt32(45) + "%";
                        dr[48] = fecha_cierre_planaccion;

                        dr[49] = sdr.GetString(47);
                        dr[50] = sdr.GetDateTime(48).ToShortDateString();
                        dr[51] = fecha_ejecucion_accioncorrectiva;
                        dr[52] = observacion_accioncorrectiva;
                        dr[53] = sdr.GetString(51);
                        dr[54] = sdr.GetString(52);

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getDataTableXPlanAccion(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Tabla");
                dt.Columns.Add("Evento_codigo");
                dt.Columns.Add("Evento_work_order");
                dt.Columns.Add("Evento_fecha_deteccion");
                dt.Columns.Add("Evento_fecha_ingreso");
                dt.Columns.Add("Evento_cliente");
                dt.Columns.Add("Evento_centro");
                dt.Columns.Add("Evento_pais");
                dt.Columns.Add("Evento_area");
                dt.Columns.Add("Evento_subarea");
                dt.Columns.Add("Evento_fuente");
                dt.Columns.Add("Evento_tipo_equipo");
                dt.Columns.Add("Evento_modelo_equipo");
                dt.Columns.Add("Evento_serie_equipo");
                dt.Columns.Add("Evento_sistema");
                dt.Columns.Add("Evento_subsistema");
                dt.Columns.Add("Evento_componente");
                dt.Columns.Add("Evento_serie_componente");
                dt.Columns.Add("Evento_parte");
                dt.Columns.Add("Evento_numero_parte");
                dt.Columns.Add("Evento_horas");
                dt.Columns.Add("Evento_clasificacion");
                dt.Columns.Add("Evento_subclasificacion");
                dt.Columns.Add("Evento_detalle");
                dt.Columns.Add("Evento_probabilidad");
                dt.Columns.Add("Evento_consecuencia");
                dt.Columns.Add("Evento_irc");
                dt.Columns.Add("Evento_criticidad");
                dt.Columns.Add("Evento_fecha_cierre");
                dt.Columns.Add("Evento_estado");
                dt.Columns.Add("Investigacion_rut_responsable");
                dt.Columns.Add("Investigacion_nombre_responsable");
                dt.Columns.Add("Investigacion_fecha_inicio");
                dt.Columns.Add("Investigacion_fecha_cierre");
                dt.Columns.Add("Evaluacion_fecha");
                dt.Columns.Add("Evaluacion_origen_falla");
                dt.Columns.Add("Evaluacion_causa_inmediata");
                dt.Columns.Add("Evaluacion_subcausa_inmediata");
                dt.Columns.Add("Evaluacion_causa_basica");
                dt.Columns.Add("Evaluacion_subcausa_basica");
                dt.Columns.Add("Evaluacion_aceptado");
                dt.Columns.Add("Evaluacion_observacion");
                dt.Columns.Add("Evaluacion_rut_evaluador");
                dt.Columns.Add("Evaluacion_nombre_evaluador");
                dt.Columns.Add("Evaluacion_personas_involucradas");
                dt.Columns.Add("PlanAccion_detalle_correccion");
                dt.Columns.Add("PlanAccion_fecha_correccion");
                dt.Columns.Add("PlanAccion_progreso");
                dt.Columns.Add("PlanAccion_fecha_cierre");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string serie_equipo;
                    string serie_componente;
                    string parte;
                    string numero_parte;
                    string horas;
                    string fecha_cierre;
                    string codigo_evento;
                    List<PersonaInfo> listInvolucrados;
                    string involucrados;
                    string fecha_cierre_investigacion;
                    string observacion_evaluacion;
                    string fecha_cierre_planaccion;

                    while (sdr.Read())
                    {
                        codigo_evento = sdr.GetString(0);

                        if (!sdr.IsDBNull(12))
                        {
                            serie_equipo = sdr.GetString(12);
                        }
                        else
                        {
                            serie_equipo = "";
                        }

                        if (!sdr.IsDBNull(16))
                        {
                            serie_componente = sdr.GetString(16);
                        }
                        else
                        {
                            serie_componente = "";
                        }

                        if (!sdr.IsDBNull(17))
                        {
                            parte = sdr.GetString(17);
                        }
                        else
                        {
                            parte = "";
                        }

                        if (!sdr.IsDBNull(18))
                        {
                            numero_parte = sdr.GetString(18);
                        }
                        else
                        {
                            numero_parte = "";
                        }

                        if (!sdr.IsDBNull(19))
                        {
                            horas = Convert.ToString(sdr.GetInt32(19));
                        }
                        else
                        {
                            horas = "";
                        }

                        if (!sdr.IsDBNull(27))
                        {
                            fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }

                        listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                        if (listInvolucrados == null)
                        {
                            return null;
                        }

                        involucrados = "";
                        for (int i = 0; i < listInvolucrados.Count; i++)
                        {
                            involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                            if (i < listInvolucrados.Count - 1)
                            {
                                involucrados += "; ";
                            }
                        }

                        if (!sdr.IsDBNull(32))
                        {
                            fecha_cierre_investigacion = sdr.GetDateTime(32).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre_investigacion = "";
                        }

                        if (!sdr.IsDBNull(38))
                        {
                            observacion_evaluacion = sdr.GetString(38);
                        }
                        else
                        {
                            observacion_evaluacion = "";
                        }

                        if (!sdr.IsDBNull(46))
                        {
                            fecha_cierre_planaccion = sdr.GetDateTime(46).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre_planaccion = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = "PlanAccion";
                        dr[1] = sdr.GetString(0);
                        dr[2] = sdr.GetString(1);
                        dr[3] = sdr.GetDateTime(2).ToShortDateString();
                        dr[4] = sdr.GetDateTime(3).ToShortDateString();
                        dr[5] = sdr.GetString(4);
                        dr[6] = sdr.GetString(5);
                        dr[7] = sdr.GetString(6);
                        dr[8] = sdr.GetString(7);
                        dr[9] = sdr.GetString(8);
                        dr[10] = sdr.GetString(9);
                        dr[11] = sdr.GetString(10);
                        dr[12] = sdr.GetString(11);
                        dr[13] = serie_equipo;
                        dr[14] = sdr.GetString(13);
                        dr[15] = sdr.GetString(14);
                        dr[16] = sdr.GetString(15);
                        dr[17] = serie_componente;
                        dr[18] = parte;
                        dr[19] = numero_parte;
                        dr[20] = horas;
                        dr[21] = sdr.GetString(20);
                        dr[22] = sdr.GetString(21);
                        dr[23] = sdr.GetString(22);
                        dr[24] = sdr.GetString(23);
                        dr[25] = sdr.GetString(24);
                        dr[26] = sdr.GetDecimal(25).ToString();
                        dr[27] = sdr.GetString(26);
                        dr[28] = fecha_cierre;
                        dr[29] = sdr.GetString(28);

                        dr[30] = sdr.GetString(29);
                        dr[31] = sdr.GetString(30);
                        dr[32] = sdr.GetDateTime(31).ToShortDateString();
                        dr[33] = sdr.GetDateTime(32).ToShortDateString();
                        dr[34] = sdr.GetDateTime(33).ToShortDateString();
                        dr[35] = sdr.GetString(34);
                        dr[36] = sdr.GetString(35);
                        dr[37] = sdr.GetString(36);
                        dr[38] = sdr.GetString(37);
                        dr[39] = sdr.GetString(38);
                        dr[40] = sdr.GetString(39);
                        if (!sdr.IsDBNull(40))
                        {
                            dr[41] = sdr.GetString(40);
                        }
                        else
                        {
                            dr[41] = "";
                        }
                        dr[42] = sdr.GetString(41);
                        dr[43] = sdr.GetString(42);
                        dr[44] = involucrados;

                        dr[45] = sdr.GetString(43);
                        dr[46] = sdr.GetDateTime(44).ToShortDateString();
                        dr[47] = sdr.GetInt32(45) + "%";
                        dr[48] = fecha_cierre_planaccion;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getDataTableXEvaluacion(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Tabla");
                dt.Columns.Add("Evento_codigo");
                dt.Columns.Add("Evento_work_order");
                dt.Columns.Add("Evento_fecha_deteccion");
                dt.Columns.Add("Evento_fecha_ingreso");
                dt.Columns.Add("Evento_cliente");
                dt.Columns.Add("Evento_centro");
                dt.Columns.Add("Evento_pais");
                dt.Columns.Add("Evento_area");
                dt.Columns.Add("Evento_subarea");
                dt.Columns.Add("Evento_fuente");
                dt.Columns.Add("Evento_tipo_equipo");
                dt.Columns.Add("Evento_modelo_equipo");
                dt.Columns.Add("Evento_serie_equipo");
                dt.Columns.Add("Evento_sistema");
                dt.Columns.Add("Evento_subsistema");
                dt.Columns.Add("Evento_componente");
                dt.Columns.Add("Evento_serie_componente");
                dt.Columns.Add("Evento_parte");
                dt.Columns.Add("Evento_numero_parte");
                dt.Columns.Add("Evento_horas");
                dt.Columns.Add("Evento_clasificacion");
                dt.Columns.Add("Evento_subclasificacion");
                dt.Columns.Add("Evento_detalle");
                dt.Columns.Add("Evento_probabilidad");
                dt.Columns.Add("Evento_consecuencia");
                dt.Columns.Add("Evento_irc");
                dt.Columns.Add("Evento_criticidad");
                dt.Columns.Add("Evento_fecha_cierre");
                dt.Columns.Add("Evento_estado");
                dt.Columns.Add("Investigacion_rut_responsable");
                dt.Columns.Add("Investigacion_nombre_responsable");
                dt.Columns.Add("Investigacion_fecha_inicio");
                dt.Columns.Add("Investigacion_fecha_cierre");
                dt.Columns.Add("Evaluacion_fecha");
                dt.Columns.Add("Evaluacion_origen_falla");
                dt.Columns.Add("Evaluacion_causa_inmediata");
                dt.Columns.Add("Evaluacion_subcausa_inmediata");
                dt.Columns.Add("Evaluacion_causa_basica");
                dt.Columns.Add("Evaluacion_subcausa_basica");
                dt.Columns.Add("Evaluacion_aceptado");
                dt.Columns.Add("Evaluacion_observacion");
                dt.Columns.Add("Evaluacion_rut_evaluador");
                dt.Columns.Add("Evaluacion_nombre_evaluador");
                dt.Columns.Add("Evaluacion_personas_involucradas");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string serie_equipo;
                    string serie_componente;
                    string parte;
                    string numero_parte;
                    string horas;
                    string fecha_cierre;
                    string codigo_evento;
                    List<PersonaInfo> listInvolucrados;
                    string involucrados;
                    string fecha_cierre_investigacion;
                    string observacion_evaluacion;

                    while (sdr.Read())
                    {
                        codigo_evento = sdr.GetString(0);

                        if (!sdr.IsDBNull(12))
                        {
                            serie_equipo = sdr.GetString(12);
                        }
                        else
                        {
                            serie_equipo = "";
                        }

                        if (!sdr.IsDBNull(16))
                        {
                            serie_componente = sdr.GetString(16);
                        }
                        else
                        {
                            serie_componente = "";
                        }

                        if (!sdr.IsDBNull(17))
                        {
                            parte = sdr.GetString(17);
                        }
                        else
                        {
                            parte = "";
                        }

                        if (!sdr.IsDBNull(18))
                        {
                            numero_parte = sdr.GetString(18);
                        }
                        else
                        {
                            numero_parte = "";
                        }

                        if (!sdr.IsDBNull(19))
                        {
                            horas = Convert.ToString(sdr.GetInt32(19));
                        }
                        else
                        {
                            horas = "";
                        }

                        if (!sdr.IsDBNull(27))
                        {
                            fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }

                        listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                        if (listInvolucrados == null)
                        {
                            return null;
                        }

                        involucrados = "";
                        for (int i = 0; i < listInvolucrados.Count; i++)
                        {
                            involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                            if (i < listInvolucrados.Count - 1)
                            {
                                involucrados += "; ";
                            }
                        }

                        if (!sdr.IsDBNull(32))
                        {
                            fecha_cierre_investigacion = sdr.GetDateTime(32).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre_investigacion = "";
                        }

                        if (!sdr.IsDBNull(38))
                        {
                            observacion_evaluacion = sdr.GetString(38);
                        }
                        else
                        {
                            observacion_evaluacion = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = "Evaluacion";
                        dr[1] = sdr.GetString(0);
                        dr[2] = sdr.GetString(1);
                        dr[3] = sdr.GetDateTime(2).ToShortDateString();
                        dr[4] = sdr.GetDateTime(3).ToShortDateString();
                        dr[5] = sdr.GetString(4);
                        dr[6] = sdr.GetString(5);
                        dr[7] = sdr.GetString(6);
                        dr[8] = sdr.GetString(7);
                        dr[9] = sdr.GetString(8);
                        dr[10] = sdr.GetString(9);
                        dr[11] = sdr.GetString(10);
                        dr[12] = sdr.GetString(11);
                        dr[13] = serie_equipo;
                        dr[14] = sdr.GetString(13);
                        dr[15] = sdr.GetString(14);
                        dr[16] = sdr.GetString(15);
                        dr[17] = serie_componente;
                        dr[18] = parte;
                        dr[19] = numero_parte;
                        dr[20] = horas;
                        dr[21] = sdr.GetString(20);
                        dr[22] = sdr.GetString(21);
                        dr[23] = sdr.GetString(22);
                        dr[24] = sdr.GetString(23);
                        dr[25] = sdr.GetString(24);
                        dr[26] = sdr.GetDecimal(25).ToString();
                        dr[27] = sdr.GetString(26);
                        dr[28] = fecha_cierre;
                        dr[29] = sdr.GetString(28);

                        dr[30] = sdr.GetString(29);
                        dr[31] = sdr.GetString(30);
                        dr[32] = sdr.GetDateTime(31).ToShortDateString();
                        dr[33] = sdr.GetDateTime(32).ToShortDateString();
                        dr[34] = sdr.GetDateTime(33).ToShortDateString();
                        dr[35] = sdr.GetString(34);
                        dr[36] = sdr.GetString(35);
                        dr[37] = sdr.GetString(36);
                        dr[38] = sdr.GetString(37);
                        dr[39] = sdr.GetString(38);
                        dr[40] = sdr.GetString(39);
                        if (!sdr.IsDBNull(40))
                        {
                            dr[41] = sdr.GetString(40);
                        }
                        else
                        {
                            dr[41] = "";
                        }
                        dr[42] = sdr.GetString(41);
                        dr[43] = sdr.GetString(42);
                        dr[44] = involucrados;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getDataTableXAccionInmediata(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Tabla");
                dt.Columns.Add("Evento_codigo");
                dt.Columns.Add("Evento_work_order");
                dt.Columns.Add("Evento_fecha_deteccion");
                dt.Columns.Add("Evento_fecha_ingreso");
                dt.Columns.Add("Evento_cliente");
                dt.Columns.Add("Evento_centro");
                dt.Columns.Add("Evento_pais");
                dt.Columns.Add("Evento_area");
                dt.Columns.Add("Evento_subarea");
                dt.Columns.Add("Evento_fuente");
                dt.Columns.Add("Evento_tipo_equipo");
                dt.Columns.Add("Evento_modelo_equipo");
                dt.Columns.Add("Evento_serie_equipo");
                dt.Columns.Add("Evento_sistema");
                dt.Columns.Add("Evento_subsistema");
                dt.Columns.Add("Evento_componente");
                dt.Columns.Add("Evento_serie_componente");
                dt.Columns.Add("Evento_parte");
                dt.Columns.Add("Evento_numero_parte");
                dt.Columns.Add("Evento_horas");
                dt.Columns.Add("Evento_clasificacion");
                dt.Columns.Add("Evento_subclasificacion");
                dt.Columns.Add("Evento_detalle");
                dt.Columns.Add("Evento_probabilidad");
                dt.Columns.Add("Evento_consecuencia");
                dt.Columns.Add("Evento_irc");
                dt.Columns.Add("Evento_criticidad");
                dt.Columns.Add("Evento_fecha_cierre");
                dt.Columns.Add("Evento_estado");
                dt.Columns.Add("AccionInmediata_causa_inmediata");
                dt.Columns.Add("AccionInmediata_subcausa_inmediata");
                dt.Columns.Add("AccionInmediata_causa_basica");
                dt.Columns.Add("AccionInmediata_subcausa_basica");
                dt.Columns.Add("AccionInmediata_accion_inmediata");
                dt.Columns.Add("AccionInmediata_fecha_acción");
                dt.Columns.Add("AccionInmediata_efectividad");
                dt.Columns.Add("AccionInmediata_observación");
                dt.Columns.Add("AccionInmediata_personas_involucradas");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string serie_equipo;
                    string serie_componente;
                    string parte;
                    string numero_parte;
                    string horas;
                    string fecha_cierre;
                    string codigo_evento;
                    List<PersonaInfo> listInvolucrados;
                    string involucrados;
                    while (sdr.Read())
                    {
                        codigo_evento = sdr.GetString(0);

                        if (!sdr.IsDBNull(12))
                        {
                            serie_equipo = sdr.GetString(12);
                        }
                        else
                        {
                            serie_equipo = "";
                        }

                        if (!sdr.IsDBNull(16))
                        {
                            serie_componente = sdr.GetString(16);
                        }
                        else
                        {
                            serie_componente = "";
                        }

                        if (!sdr.IsDBNull(17))
                        {
                            parte = sdr.GetString(17);
                        }
                        else
                        {
                            parte = "";
                        }

                        if (!sdr.IsDBNull(18))
                        {
                            numero_parte = sdr.GetString(18);
                        }
                        else
                        {
                            numero_parte = "";
                        }

                        if (!sdr.IsDBNull(19))
                        {
                            horas = Convert.ToString(sdr.GetInt32(19));
                        }
                        else
                        {
                            horas = "";
                        }

                        if (!sdr.IsDBNull(27))
                        {
                            fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }


                        listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                        if (listInvolucrados == null)
                        {
                            return null;
                        }

                        involucrados = "";
                        for (int i = 0; i < listInvolucrados.Count; i++)
                        {
                            involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                            if (i < listInvolucrados.Count - 1)
                            {
                                involucrados += "; ";
                            }
                        }


                        dr = dt.NewRow();
                        dr[0] = "AccionInmediata";
                        dr[1] = sdr.GetString(0);
                        dr[2] = sdr.GetString(1);
                        dr[3] = sdr.GetDateTime(2).ToShortDateString();
                        dr[4] = sdr.GetDateTime(3).ToShortDateString();
                        dr[5] = sdr.GetString(4);
                        dr[6] = sdr.GetString(5);
                        dr[7] = sdr.GetString(6);
                        dr[8] = sdr.GetString(7);
                        dr[9] = sdr.GetString(8);
                        dr[10] = sdr.GetString(9);
                        dr[11] = sdr.GetString(10);
                        dr[12] = sdr.GetString(11);
                        dr[13] = serie_equipo;
                        dr[14] = sdr.GetString(13);
                        dr[15] = sdr.GetString(14);
                        dr[16] = sdr.GetString(15);
                        dr[17] = serie_componente;
                        dr[18] = parte;
                        dr[19] = numero_parte;
                        dr[20] = horas;
                        dr[21] = sdr.GetString(20);
                        dr[22] = sdr.GetString(21);
                        dr[23] = sdr.GetString(22);
                        dr[24] = sdr.GetString(23);
                        dr[25] = sdr.GetString(24);
                        dr[26] = sdr.GetDecimal(25).ToString();
                        dr[27] = sdr.GetString(26);
                        dr[28] = fecha_cierre;
                        dr[29] = sdr.GetString(28);
                        dr[30] = sdr.GetString(29);
                        dr[31] = sdr.GetString(30);
                        dr[32] = sdr.GetString(31);
                        dr[33] = sdr.GetString(32);
                        dr[34] = sdr.GetString(33);
                        dr[35] = sdr.GetDateTime(34).ToShortDateString();
                        dr[36] = sdr.GetString(35);
                        if (!sdr.IsDBNull(36))
                        {
                            dr[37] = sdr.GetString(36);
                        }
                        else
                        {
                            dr[37] = "";
                        }
                        dr[38] = involucrados;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getDataTableXEvento(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Tabla");
                dt.Columns.Add("Evento_codigo");
                dt.Columns.Add("Evento_work_order");
                dt.Columns.Add("Evento_fecha_deteccion");
                dt.Columns.Add("Evento_fecha_ingreso");
                dt.Columns.Add("Evento_cliente");
                dt.Columns.Add("Evento_centro");
                dt.Columns.Add("Evento_pais");
                dt.Columns.Add("Evento_area");
                dt.Columns.Add("Evento_subarea");
                dt.Columns.Add("Evento_fuente");
                dt.Columns.Add("Evento_tipo_equipo");
                dt.Columns.Add("Evento_modelo_equipo");
                dt.Columns.Add("Evento_serie_equipo");
                dt.Columns.Add("Evento_sistema");
                dt.Columns.Add("Evento_subsistema");
                dt.Columns.Add("Evento_componente");
                dt.Columns.Add("Evento_serie_componente");
                dt.Columns.Add("Evento_parte");
                dt.Columns.Add("Evento_numero_parte");
                dt.Columns.Add("Evento_horas");
                dt.Columns.Add("Evento_clasificacion");
                dt.Columns.Add("Evento_subclasificacion");
                dt.Columns.Add("Evento_detalle");
                dt.Columns.Add("Evento_probabilidad");
                dt.Columns.Add("Evento_consecuencia");
                dt.Columns.Add("Evento_irc");
                dt.Columns.Add("Evento_criticidad");
                dt.Columns.Add("Evento_fecha_cierre");
                dt.Columns.Add("Evento_estado");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string serie_equipo;
                    string serie_componente;
                    string parte;
                    string numero_parte;
                    string horas;
                    string fecha_cierre;
                    while (sdr.Read())
                    {
                        if (!sdr.IsDBNull(12))
                        {
                            serie_equipo = sdr.GetString(12);
                        }
                        else
                        {
                            serie_equipo = "";
                        }

                        if (!sdr.IsDBNull(16))
                        {
                            serie_componente = sdr.GetString(16);
                        }
                        else
                        {
                            serie_componente = "";
                        }

                        if (!sdr.IsDBNull(17))
                        {
                            parte = sdr.GetString(17);
                        }
                        else
                        {
                            parte = "";
                        }

                        if (!sdr.IsDBNull(18))
                        {
                            numero_parte = sdr.GetString(18);
                        }
                        else
                        {
                            numero_parte = "";
                        }

                        if (!sdr.IsDBNull(19))
                        {
                            horas = Convert.ToString(sdr.GetInt32(19));
                        }
                        else
                        {
                            horas = "";
                        }

                        if (!sdr.IsDBNull(27))
                        {
                            fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = "Evento";
                        dr[1] = sdr.GetString(0);
                        dr[2] = sdr.GetString(1);
                        dr[3] = sdr.GetDateTime(2).ToShortDateString();
                        dr[4] = sdr.GetDateTime(3).ToShortDateString();
                        dr[5] = sdr.GetString(4);
                        dr[6] = sdr.GetString(5);
                        dr[7] = sdr.GetString(6);
                        dr[8] = sdr.GetString(7);
                        dr[9] = sdr.GetString(8);
                        dr[10] = sdr.GetString(9);
                        dr[11] = sdr.GetString(10);
                        dr[12] = sdr.GetString(11);
                        dr[13] = serie_equipo;
                        dr[14] = sdr.GetString(13);
                        dr[15] = sdr.GetString(14);
                        dr[16] = sdr.GetString(15);
                        dr[17] = serie_componente;
                        dr[18] = parte;
                        dr[19] = numero_parte;
                        dr[20] = horas;
                        dr[21] = sdr.GetString(20);
                        dr[22] = sdr.GetString(21);
                        dr[23] = sdr.GetString(22);
                        dr[24] = sdr.GetString(23);
                        dr[25] = sdr.GetString(24);
                        dr[26] = sdr.GetDecimal(25).ToString();
                        dr[27] = sdr.GetString(26);
                        dr[28] = fecha_cierre;
                        dr[29] = sdr.GetString(28);


                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /***********************************************************************************************************************************/
        public static DataTable getDataTableAccionCorrectiva(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Código de Evento");
                dt.Columns.Add("Descripción");
                dt.Columns.Add("Fecha límite");
                dt.Columns.Add("Fecha de ejecución");
                dt.Columns.Add("Observación");
                dt.Columns.Add("RUT responsable");
                dt.Columns.Add("Nombre responsable");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string fecha_realizado;
                    string observacion;
                    DataRow dr;
                    while (sdr.Read())
                    {
                        if (!sdr.IsDBNull(3))
                        {
                            fecha_realizado = sdr.GetDateTime(3).ToShortDateString();
                        }
                        else
                        {
                            fecha_realizado = "";
                        }

                        if (!sdr.IsDBNull(4))
                        {
                            observacion = sdr.GetString(4);
                        }
                        else
                        {
                            observacion = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = sdr.GetString(0);
                        dr[1] = sdr.GetString(1);
                        dr[2] = sdr.GetDateTime(2).ToShortDateString();
                        dr[3] = fecha_realizado;
                        dr[4] = observacion;
                        dr[5] = sdr.GetString(5);
                        dr[6] = sdr.GetString(6);


                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static DataTable getDataTablePlanAccion(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Código de Evento");
                dt.Columns.Add("Detalle");
                dt.Columns.Add("Fecha de corrección");
                dt.Columns.Add("Progreso");
                dt.Columns.Add("Fecha de cierre");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string fecha_cierre;

                    while (sdr.Read())
                    {
                        if (!sdr.IsDBNull(4))
                        {
                            fecha_cierre = sdr.GetDateTime(4).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = sdr.GetString(0);
                        dr[1] = sdr.GetString(1);
                        dr[2] = sdr.GetDateTime(2).ToShortDateString();
                        dr[3] = sdr.GetInt32(3) + "%";
                        dr[4] = fecha_cierre;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static DataTable getDataTableEvaluacion(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Código de Evento");
                dt.Columns.Add("RUT responsable Investigación");
                dt.Columns.Add("Nombre responsable Investigación");
                dt.Columns.Add("Fecha inicio Investigación");
                dt.Columns.Add("Fecha cierre Investigación");
                dt.Columns.Add("Fecha respuesta a Cliente Investigación");
                dt.Columns.Add("Fecha Evaluación");
                dt.Columns.Add("Origen Falla");
                dt.Columns.Add("Causa inmediata");
                dt.Columns.Add("Sub-Causa inmediata");
                dt.Columns.Add("Causa básica");
                dt.Columns.Add("Sub-Causa básica");
                dt.Columns.Add("Aceptado");
                dt.Columns.Add("Observación Evaluación");
                dt.Columns.Add("RUT Evaluador");
                dt.Columns.Add("Nombre Evaluador");
                dt.Columns.Add("Personas involucradas");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string codigo_evento;
                    List<PersonaInfo> listInvolucrados;
                    string involucrados;
                    string fecha_cierre_investigacion;
                    string observacion_evaluacion;
                    DataRow dr;

                    while (sdr.Read())
                    {
                        codigo_evento = sdr.GetString(0);
                        listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                        if (listInvolucrados == null)
                        {
                            return null;
                        }

                        involucrados = "";
                        for (int i = 0; i < listInvolucrados.Count; i++)
                        {
                            involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                            if (i < listInvolucrados.Count - 1)
                            {
                                involucrados += "; ";
                            }
                        }

                        if (!sdr.IsDBNull(4))
                        {
                            fecha_cierre_investigacion = sdr.GetDateTime(4).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre_investigacion = "";
                        }

                        if (!sdr.IsDBNull(10))
                        {
                            observacion_evaluacion = sdr.GetString(10);
                        }
                        else
                        {
                            observacion_evaluacion = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = codigo_evento;
                        dr[1] = sdr.GetString(1);
                        dr[2] = sdr.GetString(2);
                        dr[3] = sdr.GetDateTime(3).ToShortDateString();
                        dr[4] = sdr.GetDateTime(4).ToShortDateString();
                        dr[5] = sdr.GetDateTime(5).ToShortDateString();
                        dr[6] = sdr.GetDateTime(6).ToShortDateString();
                        dr[7] = sdr.GetString(7);
                        dr[8] = sdr.GetString(8);
                        dr[9] = sdr.GetString(9);
                        dr[10] = sdr.GetString(10);
                        dr[11] = sdr.GetString(11);
                        dr[12] = sdr.GetString(12);
                        if (!sdr.IsDBNull(13))
                        {
                            dr[13] = sdr.GetString(13);
                        }
                        else
                        {
                            dr[13] = "";
                        }
                        dr[14] = sdr.GetString(14);
                        dr[15] = sdr.GetString(15);
                        dr[16] = involucrados;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static DataTable getDataTableAccionInmediata(string id_centro)
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

                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Código de Evento");
                dt.Columns.Add("Causa inmediata");
                dt.Columns.Add("Sub-Causa inmediata");
                dt.Columns.Add("Causa básica");
                dt.Columns.Add("Sub-Causa básica");
                dt.Columns.Add("Acción inmediata");
                dt.Columns.Add("Fecha de acción");
                dt.Columns.Add("Efectividad");
                dt.Columns.Add("Observación");
                dt.Columns.Add("Personas involucradas");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    string codigo_evento;
                    List<PersonaInfo> listInvolucrados;
                    string involucrados;
                    DataRow dr;
                    while (sdr.Read())
                    {
                        codigo_evento = sdr.GetString(0);
                        listInvolucrados = LogicController.getListInvolucradosEvento(codigo_evento);
                        if (listInvolucrados == null)
                        {
                            return null;
                        }

                        involucrados = "";
                        for (int i = 0; i < listInvolucrados.Count; i++)
                        {
                            involucrados += listInvolucrados[i].getNombre() + " [" + listInvolucrados[i].getRut() + "]";

                            if (i < listInvolucrados.Count - 1)
                            {
                                involucrados += "; ";
                            }
                        }

                        dr = dt.NewRow();
                        dr[0] = codigo_evento;
                        dr[1] = sdr.GetString(1);
                        dr[2] = sdr.GetString(2);
                        dr[3] = sdr.GetString(3);
                        dr[4] = sdr.GetString(4);
                        dr[5] = sdr.GetString(5);
                        dr[6] = sdr.GetDateTime(6).ToShortDateString();
                        dr[7] = sdr.GetString(7);
                        if (!sdr.IsDBNull(8))
                        {
                            dr[8] = sdr.GetString(8);
                        }
                        else
                        {
                            dr[8] = "";
                        }
                        dr[9] = involucrados;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static DataTable getDataTableEvento(string id_centro)
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
                
                cmd.Parameters.Add("id_centro", SqlDbType.VarChar, 70).Value = id_centro;

                DataTable dt = new DataTable();
                dt.Columns.Add("Código");
                dt.Columns.Add("W/O");
                dt.Columns.Add("Fecha detección");
                dt.Columns.Add("Fecha ingreso");
                dt.Columns.Add("Cliente");
                dt.Columns.Add("Centro");
                dt.Columns.Add("País");
                dt.Columns.Add("Área");
                dt.Columns.Add("Sub-Área");
                dt.Columns.Add("Fuente");
                dt.Columns.Add("Tipo equipo");
                dt.Columns.Add("Modelo");
                dt.Columns.Add("Serie equipo");
                dt.Columns.Add("Sistema");
                dt.Columns.Add("Sub-Sistema");
                dt.Columns.Add("Componente");
                dt.Columns.Add("Serie componente");
                dt.Columns.Add("Parte");
                dt.Columns.Add("Número parte");
                dt.Columns.Add("Horas");
                dt.Columns.Add("Clasificación");
                dt.Columns.Add("Sub-Clasificación");
                dt.Columns.Add("Detalle");
                dt.Columns.Add("Probabilidad");
                dt.Columns.Add("Consecuencia");
                dt.Columns.Add("IRC");
                dt.Columns.Add("Criticidad");
                dt.Columns.Add("Fecha cierre");
                dt.Columns.Add("Estado");

                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    DataRow dr;
                    string serie_equipo;
                    string serie_componente;
                    string parte;
                    string numero_parte;
                    string horas;
                    string fecha_cierre;
                    while (sdr.Read())
                    {
                        if (!sdr.IsDBNull(12))
                        {
                            serie_equipo = sdr.GetString(12);
                        }
                        else
                        {
                            serie_equipo = "";
                        }

                        if (!sdr.IsDBNull(16))
                        {
                            serie_componente = sdr.GetString(16);
                        }
                        else
                        {
                            serie_componente = "";
                        }

                        if (!sdr.IsDBNull(17))
                        {
                            parte = sdr.GetString(17);
                        }
                        else
                        {
                            parte = "";
                        }

                        if (!sdr.IsDBNull(18))
                        {
                            numero_parte = sdr.GetString(18);
                        }
                        else
                        {
                            numero_parte = "";
                        }

                        if (!sdr.IsDBNull(19))
                        {
                            horas = Convert.ToString(sdr.GetInt32(19));
                        }
                        else
                        {
                            horas = "";
                        }

                        if (!sdr.IsDBNull(27))
                        {
                            fecha_cierre = sdr.GetDateTime(27).ToShortDateString();
                        }
                        else
                        {
                            fecha_cierre = "";
                        }

                        dr = dt.NewRow();
                        dr[0] = sdr.GetString(0);
                        dr[1] = sdr.GetString(1);
                        dr[2] = sdr.GetDateTime(2).ToShortDateString();
                        dr[3] = sdr.GetDateTime(3).ToShortDateString();
                        dr[4] = sdr.GetString(4);
                        dr[5] = sdr.GetString(5);
                        dr[6] = sdr.GetString(6);
                        dr[7] = sdr.GetString(7);
                        dr[8] = sdr.GetString(8);
                        dr[9] = sdr.GetString(9);
                        dr[10] = sdr.GetString(10);
                        dr[11] = sdr.GetString(11);
                        dr[12] = serie_equipo;
                        dr[13] = sdr.GetString(13);
                        dr[14] = sdr.GetString(14);
                        dr[15] = sdr.GetString(15);
                        dr[16] = serie_componente;
                        dr[17] = parte;
                        dr[18] = numero_parte;
                        dr[19] = horas;
                        dr[20] = sdr.GetString(20);
                        dr[21] = sdr.GetString(21);
                        dr[22] = sdr.GetString(22);
                        dr[23] = sdr.GetString(23);
                        dr[24] = sdr.GetString(24);
                        dr[25] = sdr.GetDecimal(25).ToString();
                        dr[26] = sdr.GetString(26);
                        dr[27] = fecha_cierre;
                        dr[28] = sdr.GetString(28);


                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                }

                sdr.Close();
                conn.Close();

                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}