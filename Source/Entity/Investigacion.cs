using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Investigacion
    {
        private string codigo_evento;
        private PersonaInfo responsable;
        private int antiguedad_responsable;
        private string fecha_inicio;
        private string fecha_cierre;
        private string fecha_respuesta;


        public Investigacion(string codigo_evento, PersonaInfo responsable, int antiguedad_responsable, string fecha_inicio, string fecha_cierre, string fecha_respuesta)
        {
            this.codigo_evento = codigo_evento;
            this.responsable = responsable;
            this.antiguedad_responsable = antiguedad_responsable;
            this.fecha_inicio = fecha_inicio;
            this.fecha_cierre = fecha_cierre;
            this.fecha_respuesta = fecha_respuesta;
        }


        public string getCodigoEvento()
        {
            return this.codigo_evento;
        }

        public PersonaInfo getResponsable()
        {
            return this.responsable;
        }

        public int getAntiguedadResponsable()
        {
            return this.antiguedad_responsable;
        }

        public string getFechaInicio()
        {
            return this.fecha_inicio;
        }

        public string getFechaCierre()
        {
            return this.fecha_cierre;
        }

        public string getFechaRespuesta()
        {
            return this.fecha_respuesta;
        }
    }
}