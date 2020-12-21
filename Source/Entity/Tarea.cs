using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Tarea
    {
        string tipo_objeto;
        string codigo_evento;
        string work_order;
        string nombre_tarea;
        string fecha_vencimiento;
        int dias_vencimiento;
        string rut_responsable;
        string nombre_responsable;


        public Tarea(string tipo_objeto, string codigo_evento, string work_order, string nombre_tarea, string fecha_vencimiento, int dias_vencimiento, string rut_responsable, string nombre_responsable)
        {
            this.tipo_objeto = tipo_objeto;
            this.codigo_evento = codigo_evento;
            this.work_order = work_order;
            this.nombre_tarea = nombre_tarea;
            this.fecha_vencimiento = fecha_vencimiento;
            this.dias_vencimiento = dias_vencimiento;
            this.rut_responsable = rut_responsable;
            this.nombre_responsable = nombre_responsable;
        }


        public string getTipoObjeto()
        {
            return this.tipo_objeto;
        }

        public string getCodigoEvento()
        {
            return this.codigo_evento;
        }

        public string getWO()
        {
            return this.work_order;
        }

        public string getNombreTarea()
        {
            return this.nombre_tarea;
        }

        public string getFechaVencimiento()
        {
            return this.fecha_vencimiento;
        }

        public int getDiasVencimiento()
        {
            return this.dias_vencimiento;
        }

        public string getRUTResponsable()
        {
            return this.rut_responsable;
        }

        public string getNombreResponsable()
        {
            return this.nombre_responsable;
        }
    }
}