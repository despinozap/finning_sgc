using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class ConfigEmailSender
    {

        private string estado;
        private string nombre_rol;
        private string id_centro;
        private int dias_alerta;
        private int dias_limite;
        private int dias_mensual;
        private bool activo;

        public ConfigEmailSender(string estado, string nombre_rol, string id_centro, int dias_alerta, int dias_limite, int dias_mensual, bool activo)
        {
            this.estado = estado;
            this.nombre_rol = nombre_rol;
            this.id_centro = id_centro;
            this.dias_alerta = dias_alerta;
            this.dias_limite = dias_limite;
            this.dias_mensual = dias_mensual;
            this.activo = activo;
        }


        public string getEstado()
        {
            return this.estado;
        }

        public string getNombreRol()
        {
            return this.nombre_rol;
        }

        public string getIDCentro()
        {
            return this.id_centro;
        }

        public int getDiasAlerta()
        {
            return this.dias_alerta;
        }

        public int getDiasLimite()
        {
            return this.dias_limite;
        }

        public int getDiasMensual()
        {
            return this.dias_mensual;
        }

        public bool getActivo()
        {
            return this.activo;
        }
    }
}