using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class Archivo
    {
        private string id_archivo;
        private string nombre;
        private byte[] contenido;
        private int size;
        private string tipo_archivo;
        private string tipo_contenido;

        public Archivo(string id_archivo, string nombre, byte[] contenido, string tipo_archivo, string tipo_contenido)
        {
            this.id_archivo = id_archivo;
            this.nombre = nombre;
            this.contenido = contenido;
            this.size = contenido.Length;
            this.tipo_archivo = tipo_archivo;
            this.tipo_contenido = tipo_contenido;
        }

        public string getIdArchivo()
        {
            return this.id_archivo;
        }

        public string getNombre()
        {
            return this.nombre;
        }

        public void setNombre(string nombre)
        {
            this.nombre = nombre;
        }

        public byte[] getContenido()
        {
            return this.contenido;
        }

        public void setContenido(byte[] contenido)
        {
            this.contenido = contenido;
        }

        public int getSize()
        {
            return this.size;
        }

        public string getTipoArchivo()
        {
            return this.tipo_archivo;
        }

        public string getTipoContenido()
        {
            return this.tipo_contenido;
        }
    }
}