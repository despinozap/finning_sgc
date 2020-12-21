using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Persona
    {
        private string rut;
        private string nombre;
        private string fecha_nacimiento;
        private string sexo;
        private string id_empleado;
        private string id_centro;
        private string id_store;
        private string lob;
        private string cargo;
        private string clasificacion;
        private string rut_supervisor;
        private string email;
        private string fecha_ingreso;
        private string fecha_retiro;


        public Persona(string rut, string nombre, string fecha_nacimiento, string sexo, string id_empleado, string id_centro, string id_store, string lob, string cargo, string clasificacion, string rut_supervisor, string email, string fecha_ingreso, string fecha_retiro)
        {
            this.rut = rut;
            this.nombre = nombre;
            this.fecha_nacimiento = fecha_nacimiento;
            this.sexo = sexo;
            this.id_empleado = id_empleado;
            this.id_centro = id_centro;
            this.id_store = id_store;
            this.lob = lob;
            this.cargo = cargo;
            this.clasificacion = clasificacion;
            this.rut_supervisor = rut_supervisor;
            this.email = email;
            this.fecha_ingreso = fecha_ingreso;
            this.fecha_retiro = fecha_retiro;
        }


        public string getRut()
        {
            return this.rut;
        }

        public string getNombre()
        {
            return this.nombre;
        }

        public string getFechaNacimiento()
        {
            return this.fecha_nacimiento;
        }

        public string getSexo()
        {
            return this.sexo;
        }

        public string getIDEmpleado()
        {
            return this.id_empleado;
        }

        public string getIDCentro()
        {
            return this.id_centro;
        }

        public string getIDStore()
        {
            return this.id_store;
        }

        public string getLob()
        {
            return this.lob;
        }

        public string getCargo()
        {
            return this.cargo;
        }

        public string getClasificacion()
        {
            return this.clasificacion;
        }

        public string getRutSupervisor()
        {
            return this.rut_supervisor;
        }

        public string getEmail()
        {
            return this.email;
        }

        public string getFechaIngreso()
        {
            return this.fecha_ingreso;
        }

        public string getFechaRetiro()
        {
            return this.fecha_retiro;
        }
    }
}