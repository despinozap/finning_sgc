using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class EmailInfo
    {
        private string nombre;
        private string direccion;
        private string sexo;

        public EmailInfo(string direccion, string nombre, string sexo)
        {
            this.direccion = direccion;
            this.nombre = nombre;
            this.sexo = sexo;
        }

        public string getDireccion()
        {
            return this.direccion;
        }

        public string getNombre()
        {
            return this.nombre;
        }

        public string getSexo()
        {
            return this.sexo;
        }
    }
}