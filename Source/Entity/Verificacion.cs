using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Verificacion
    {
        private string codigo_planaccion;
        private string efectivo;
        private string observacion;
        private string rut_responsable;
        private string fecha;


        public Verificacion(string codigo_planaccion, string efectivo, string observacion, string rut_responsable, string fecha)
        {
            this.codigo_planaccion = codigo_planaccion;
            this.efectivo = efectivo;
            this.observacion = observacion;
            this.rut_responsable = rut_responsable;
            this.fecha = fecha;
        }


        public string getCodigoPlanAccion()
        {
            return this.codigo_planaccion;
        }

        public string getEfectivo()
        {
            return this.efectivo;
        }

        public string getObservacion()
        {
            return this.observacion;
        }

        public string getRutResponsable()
        {
            return this.rut_responsable;
        }

        public string getFecha()
        {
            return this.fecha;
        }
    }
}