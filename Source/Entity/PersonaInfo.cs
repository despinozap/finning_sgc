using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class PersonaInfo
    {
        private string rut;
        private string nombre;
        private string nombre_centro;
        private string cargo;
        private int antiguedad;

        public PersonaInfo(string rut, string nombre, string nombre_centro, string cargo, int antiguedad)
        {
            this.rut = rut;
            this.nombre = nombre;
            this.nombre_centro = nombre_centro;
            this.cargo = cargo;
            this.antiguedad = antiguedad;
        }

        public string getRut()
        {
            return this.rut;
        }

        public string getNombre()
        {
            return this.nombre;
        }

        public string getNombreCentro()
        {
            return this.nombre_centro;
        }

        public string getCargo()
        {
            return this.cargo;
        }

        public int getAntiguedad()
        {
            return this.antiguedad;
        }
    }
}