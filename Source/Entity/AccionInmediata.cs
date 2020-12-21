using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class AccionInmediata
    {
        private string codigo_evento;
        private string nombre_origen;
        private string tipo_causainmediata;
        private string nombre_causainmediata;
        private string nombre_subcausainmediata;
        private string tipo_causabasica;
        private string nombre_causabasica;
        private string nombre_subcausabasica;
        private string accion_inmediata;
        private string fecha_accion;
        private string efectividad;
        private string observacion;
        List<PersonaInfo> listInvolucrados;

        public AccionInmediata(string codigo_evento, string nombre_origen, string tipo_causainmediata, string nombre_causainmediata, string nombre_subcausainmediata, string tipo_causabasica, string nombre_causabasica, string nombre_subcausabasica, string accion_inmediata, string fecha_accion, string efectividad, string observacion, List<PersonaInfo> listInvolucrados)
        {
            this.codigo_evento = codigo_evento;
            this.nombre_origen = nombre_origen;
            this.tipo_causainmediata = tipo_causainmediata;
            this.nombre_causainmediata = nombre_causainmediata;
            this.nombre_subcausainmediata = nombre_subcausainmediata;
            this.tipo_causabasica = tipo_causabasica;
            this.nombre_causabasica = nombre_causabasica;
            this.nombre_subcausabasica = nombre_subcausabasica;
            this.accion_inmediata = accion_inmediata;
            this.fecha_accion = fecha_accion;
            this.efectividad = efectividad;
            this.observacion = observacion;
            this.listInvolucrados = listInvolucrados;
        }


        public string getCodigoEvento()
        {
            return this.codigo_evento;
        }


        public string getNombreOrigen()
        {
            return this.nombre_origen;
        }


        public string getTipoCausaInmediata()
        {
            return this.tipo_causainmediata;
        }


        public string getNombreCausaInmediata()
        {
            return this.nombre_causainmediata;
        }

        public string getNombreSubcausaInmediata()
        {
            return this.nombre_subcausainmediata;
        }


        public string getTipoCausaBasica()
        {
            return this.tipo_causabasica;
        }


        public string getNombreCausaBasica()
        {
            return this.nombre_causabasica;
        }

        public string getNombreSubcausaBasica()
        {
            return this.nombre_subcausabasica;
        }


        public string getAccionInmediata()
        {
            return this.accion_inmediata;
        }

        public string getFechaAccion()
        {
            return this.fecha_accion;
        }

        public string getEfectividad()
        {
            return this.efectividad;
        }

        public string getObservacion()
        {
            return this.observacion;
        }

        public List<PersonaInfo> getListInvolucrados()
        {
            return this.listInvolucrados;
        }
    }
}