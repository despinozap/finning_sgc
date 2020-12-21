using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class AccionCorrectiva
    {
        private string id;
        private string descripcion;
        private string fecha_limite;
        private string fecha_realizado;
        private PersonaInfo responsable;
        private string observacion;

        public AccionCorrectiva(string id, string descripcion, string fecha_limite, string fecha_realizado, PersonaInfo responsable, string observacion)
        {
            this.id = id;
            this.descripcion = descripcion;
            this.fecha_limite = fecha_limite;
            this.fecha_realizado = fecha_realizado;
            this.responsable = responsable;
            this.observacion = observacion;
        }


        public string getIdAccionCorrectiva()
        {
            return this.id;
        }

        public string getDescripcion()
        {
            return this.descripcion;
        }

        public string getFechaLimite()
        {
            return this.fecha_limite;
        }

        public string getFechaRealizado()
        {
            return this.fecha_realizado;
        }

        public PersonaInfo getResponsable()
        {
            return this.responsable;
        }

        public string getObservacion()
        {
            return this.observacion;
        }

        public void setDescripcion(string descripcion)
        {
            if (descripcion != null)
                this.descripcion = descripcion;
        }

        public void setFechaLimite(string fecha_limite)
        {
            if (fecha_limite != null)
                this.fecha_limite = fecha_limite;
        }

        public void setResponsable(PersonaInfo responsable)
        {
            if (responsable != null)
                this.responsable = responsable;
        }

        public void setObservacion(string observacion)
        {
            this.observacion = observacion;
        }
    }
}