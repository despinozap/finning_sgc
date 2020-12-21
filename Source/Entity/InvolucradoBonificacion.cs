using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class InvolucradoBonificacion
    {
        private string rut_involucrado;
        private string nombre_involucrado;
        private string rut_supervisor;
        private string nombre_supervisor;
        private Dictionary<string, int> eventos;
        private int total_eventos;


        public InvolucradoBonificacion
        (
            string rut_involucrado,
            string nombre_involucrado,
            string rut_supervisor,
            string nombre_supervisor,
            Dictionary<string, int> eventos
        )
        {
            this.rut_involucrado = rut_involucrado;
            this.nombre_involucrado = nombre_involucrado;
            this.rut_supervisor = rut_supervisor;
            this.nombre_supervisor = nombre_supervisor;
            this.eventos = eventos;
            this.total_eventos = 0;
        }


        public void addNoConformidad(int cantidad)
        {
            this.eventos["No conformidad"] = this.eventos["No conformidad"] + cantidad;

            this.total_eventos += cantidad;
        }


        public void addHallazgo(string nombre_area, int cantidad)
        {
            this.eventos[nombre_area] = this.eventos[nombre_area] + cantidad;

            this.total_eventos += cantidad;
        }


        public int getCantidadEventos(string nombre_area)
        {
            if (eventos.ContainsKey(nombre_area))
            {
                return eventos[nombre_area];
            }
            else
            {
                return -1;
            }
        }


        public string getRutInvolucrado()
        {
            return this.rut_involucrado;
        }

        public string getNombreInvolucrado()
        {
            return this.nombre_involucrado;
        }

        public string getRutSupervisor()
        {
            return this.rut_supervisor;
        }

        public string getNombreSupervisor()
        {
            return this.nombre_supervisor;
        }

        public Dictionary<string, int> getEventos()
        {
            return this.eventos;
        }

        public int getTotalEventos()
        {
            return this.total_eventos;
        }
    }
}