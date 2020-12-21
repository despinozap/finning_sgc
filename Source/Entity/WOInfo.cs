using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class WOInfo
    {
        private string codigo_wo;
        private string nombre_cliente;
        private string id_centro;
        private string nombre_centro;
        private string modelo_equipo;
        private string tipo_equipo;
        private string serie_equipo;
        private string nombre_sistema;
        private string nombre_subsistema;
        private string nombre_componente;
        private string serie_componente;
        private string parte;
        private string numero_parte;
        private int horas;


        public WOInfo(string codigo_wo, string nombre_cliente, string id_centro, string nombre_centro, string modelo_equipo, string tipo_equipo, string serie_equipo, string nombre_sistema, string nombre_subsistema, string nombre_componente, string serie_componente, string parte, string numero_parte, int horas)
        {
            this.codigo_wo = codigo_wo;
            this.nombre_cliente = nombre_cliente;
            this.id_centro = id_centro;
            this.nombre_centro = nombre_centro;
            this.modelo_equipo = modelo_equipo;
            this.tipo_equipo = tipo_equipo;
            this.serie_equipo = serie_equipo;
            this.nombre_sistema = nombre_sistema;
            this.nombre_subsistema = nombre_subsistema;
            this.nombre_componente = nombre_componente;
            this.serie_componente = serie_componente;
            this.parte = parte;
            this.numero_parte = numero_parte;
            this.horas = horas;
        }


        public string getCodigoWO()
        {
            return this.codigo_wo;
        }

        public string getNombreCliente()
        {
            return this.nombre_cliente;
        }

        public string getIDCentro()
        {
            return this.id_centro;
        }

        public string getNombreCentro()
        {
            return this.nombre_centro;
        }

        public string getModeloEquipo()
        {
            return this.modelo_equipo;
        }

        public string getTipoEquipo()
        {
            return this.tipo_equipo;
        }

        public string getSerieEquipo()
        {
            return this.serie_equipo;
        }

        public string getNombreSistema()
        {
            return this.nombre_sistema;
        }

        public string getNombreSubsistema()
        {
            return this.nombre_subsistema;
        }

        public string getNombreComponente()
        {
            return this.nombre_componente;
        }

        public string getSerieComponente()
        {
            return this.serie_componente;
        }

        public string getParte()
        {
            return this.parte;
        }

        public string getNumeroParte()
        {
            return this.numero_parte;
        }

        public int getHoras()
        {
            return this.horas;
        }

    }
}