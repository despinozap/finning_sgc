using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Evaluacion
    {
        private string codigo_evento;
        private string nombre_origen;
        private string tipo_causainmediata;
        private string nombre_causainmediata;
        private string nombre_subcausainmediata;
        private string tipo_causabasica;
        private string nombre_causabasica;
        private string nombre_subcausabasica;
        private string aceptado;
        private string observacion;
        private string rut_responsable;
        private int antiguedad_responsable;
        private string fecha;
        List<PersonaInfo> listInvolucrados;


        public Evaluacion(string codigo_evento, string nombre_origen, string tipo_causainmediata, string nombre_causainmediata, string nombre_subcausainmediata, string tipo_causabasica, string nombre_causabasica, string nombre_subcausabasica, string aceptado, string observacion, string rut_responsable, int antiguedad_responsable, string fecha, List<PersonaInfo> listInvolucrados)
        {
            this.tipo_causainmediata = tipo_causainmediata;
            this.codigo_evento = codigo_evento;
            this.nombre_origen = nombre_origen;
            this.nombre_causainmediata = nombre_causainmediata;
            this.nombre_subcausainmediata = nombre_subcausainmediata;
            this.tipo_causabasica = tipo_causabasica;
            this.nombre_causabasica = nombre_causabasica;
            this.nombre_subcausabasica = nombre_subcausabasica;
            this.aceptado = aceptado;
            this.observacion = observacion;
            this.rut_responsable = rut_responsable;
            this.antiguedad_responsable = antiguedad_responsable;
            this.fecha = fecha;
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

        public string getAceptado()
        {
            return this.aceptado;
        }

        public string getObservacion()
        {
            return this.observacion;
        }

        public string getRutResponsable()
        {
            return this.rut_responsable;
        }

        public int getAntiguedadResponsable()
        {
            return this.antiguedad_responsable;
        }

        public string getFecha()
        {
            return this.fecha;
        }

        public List<PersonaInfo> getListInvolucrados()
        {
            return this.listInvolucrados;
        }
    }
}