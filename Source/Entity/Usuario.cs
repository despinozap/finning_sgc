using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Usuario
    {
        private string usuario;
        private string nombre_rol;
        private string rut_persona;
        private string nombre_persona;
        private string sexo_persona;
        private string id_centro;
        private string nombre_centro;
        private string pais_centro;

        public Usuario(string usuario, string nombre_rol, string rut_persona, string nombre_persona, string sexo_persona, string id_centro, string nombre_centro, string pais_centro)
        {
            this.usuario = usuario;
            this.nombre_rol = nombre_rol;
            this.rut_persona = rut_persona;
            this.nombre_persona = nombre_persona;
            this.sexo_persona = sexo_persona;
            this.id_centro = id_centro;
            this.nombre_centro = nombre_centro;
            this.pais_centro = pais_centro;
        }

        public string getUsuario()
        {
            return this.usuario;
        }

        public string getNombreRol()
        {
            return this.nombre_rol;
        }

        public string getRutPersona()
        {
            return this.rut_persona;
        }

        public string getNombrePersona()
        {
            return this.nombre_persona;
        }

        public string getSexoPersona()
        {
            return this.sexo_persona;
        }

        public string getIDCentro()
        {
            return this.id_centro;
        }

        public string getNombreCentro()
        {
            return this.nombre_centro;
        }

        public string getPaisCentro()
        {
            return this.pais_centro;
        }
    }
}
