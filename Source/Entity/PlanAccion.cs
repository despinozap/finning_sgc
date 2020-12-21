using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class PlanAccion
    {
        private string codigo_evento;
        private string detalle_correccion;
        private DateTime fecha_correccion;
        private int progreso;
        List<AccionCorrectiva> listAccionesCorrectivas;

        public PlanAccion(string codigo_evento, string detalle_correccion, DateTime fecha_correccion, int progreso, List<AccionCorrectiva> listAccionesCorrectivas)
        {
            this.codigo_evento = codigo_evento;
            this.detalle_correccion = detalle_correccion;
            this.fecha_correccion = fecha_correccion;
            this.progreso = progreso;
            this.listAccionesCorrectivas = listAccionesCorrectivas;
        }


        public string getCodigoEvento()
        {
            return this.codigo_evento;
        }

        public string getDetalleCorreccion()
        {
            return this.detalle_correccion;
        }

        public DateTime getFechaCorreccion()
        {
            return this.fecha_correccion;
        }

        public int getProgreso()
        {
            return this.progreso;
        }


        public List<AccionCorrectiva> getListAccionesCorrectivas()
        {
            return this.listAccionesCorrectivas;
        }
    }
}