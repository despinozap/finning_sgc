using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Evento
    {
        private string codigo;
        private string work_order;
        private DateTime fecha;
        private DateTime fecha_ingreso;
        private string nombre_cliente;
        private string id_centro;
        private string nombre_area;
        private string nombre_subarea;
        private string nombre_fuente;
        private string modelo_equipo;
        private string tipo_equipo;
        private string serie_equipo;
        private string nombre_sistema;
        private string nombre_subsistema;
        private string nombre_componente;
        private string serie_componente;
        private string parte;
        private string numero_parte;
        private int horas;
        private string nombre_clasificacion;
        private string nombre_subclasificacion;
        private string agente_corrector;
        private string probabilidad;
        private string consecuencia;
        private double irc;
        private string criticidad;
        private string detalle;

        public Evento
        (
            string codigo,
            string work_order,
            DateTime fecha,
            DateTime fecha_ingreso,
            string nombre_cliente,
            string id_centro,
            string nombre_area,
            string nombre_subarea,
            string nombre_fuente,
            string modelo_equipo,
            string tipo_equipo,
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
            string probabilidad,
            string consecuencia,
            double irc,
            string criticidad,
            string detalle
        )
        {
            this.codigo = codigo;
            this.work_order = work_order;
            this.fecha = fecha;
            this.fecha_ingreso = fecha_ingreso;
            this.nombre_cliente = nombre_cliente;
            this.id_centro = id_centro;
            this.nombre_area = nombre_area;
            this.nombre_subarea = nombre_subarea;
            this.nombre_fuente = nombre_fuente;
            this.modelo_equipo = modelo_equipo;
            this.tipo_equipo = tipo_equipo;
            this.serie_equipo = serie_equipo;
            this.nombre_sistema = nombre_sistema;
            this.nombre_subsistema = nombre_subsistema;
            this.nombre_componente = nombre_componente;
            this.serie_componente = serie_componente;
            this.parte = parte;
            this.numero_parte = numero_parte;
            this.horas = horas;
            this.nombre_clasificacion = nombre_clasificacion;
            this.nombre_subclasificacion = nombre_subclasificacion;
            this.agente_corrector = agente_corrector;
            this.probabilidad = probabilidad;
            this.consecuencia = consecuencia;
            this.irc = irc;
            this.criticidad = criticidad;
            this.detalle = detalle;
        }


        public string getCodigo()
        {
            return this.codigo;
        }

        public string getWO()
        {
            return this.work_order;
        }

        public DateTime getFecha()
        {
            return this.fecha;
        }

        public DateTime getFechaIngreso()
        {
            return this.fecha_ingreso;
        }

        public string getNombreCliente()
        {
            return this.nombre_cliente;
        }

        public string getIDCentro()
        {
            return this.id_centro;
        }

        public string getNombreArea()
        {
            return this.nombre_area;
        }

        public string getNombreSubarea()
        {
            return this.nombre_subarea;
        }

        public string getNombreFuente()
        {
            return this.nombre_fuente;
        }

        public string getModeloEquipo()
        {
            return this.modelo_equipo;
        }
        
        public string getTipoEquipo()
        {
            return this.tipo_equipo;
        }

        public string getSerieEquipo()
        {
            return this.serie_equipo;
        }

        public string getNombreSistema()
        {
            return this.nombre_sistema;
        }

        public string getNombreSubsistema()
        {
            return this.nombre_subsistema;
        }

        public string getNombreComponente()
        {
            return this.nombre_componente;
        }

        public string getSerieComponente()
        {
            return this.serie_componente;
        }
        
        public string getParte()
        {
            return this.parte;
        }

        public string getNumeroParte()
        {
            return this.numero_parte;
        }

        public int getHoras()
        {
            return this.horas;
        }

        public string getNombreClasificacion()
        {
            return this.nombre_clasificacion;
        }

        public string getNombreSubclasificacion()
        {
            return this.nombre_subclasificacion;
        }

        public string getAgenteCorrector()
        {
            return this.agente_corrector;
        }

        public string getProbabilidad()
        {
            return this.probabilidad;
        }

        public string getConsecuencia()
        {
            return this.consecuencia;
        }

        public double getIRC()
        {
            return this.irc;
        }

        public string getCriticidad()
        {
            return this.criticidad;
        }

        public string getDetalle()
        {
            return this.detalle;
        }

    }
}