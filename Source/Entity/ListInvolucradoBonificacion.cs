using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCCSAN.Source.Entity
{
    public class ListInvolucradoBonificacion
    {
        private Dictionary<string, int> default_eventos;
        private List<InvolucradoBonificacion> list_involucrado_bonificacion;

        public ListInvolucradoBonificacion(Dictionary<string, int> default_eventos)
        {
            this.default_eventos = default_eventos;

            list_involucrado_bonificacion = new List<InvolucradoBonificacion>();
        }


        private Dictionary<string, int> generateNewDefaultEventos()
        {
            Dictionary<string, int> new_default_eventos = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> result in default_eventos)
            {
                new_default_eventos.Add(result.Key, result.Value);
            }

            return new_default_eventos;
        }


        public void addRegistry(
                                    string rut_involucrado,
                                    string nombre_involucrado,
                                    string rut_supervisor,
                                    string nombre_supervisor,
                                    string nombre_area,
                                    int cantidad_hallazgo,
                                    int cantidad_noconformidad
        )
        {
            InvolucradoBonificacion involucrado_bonificacion = getInvolucradoBonificacion(rut_involucrado);
            if (involucrado_bonificacion == null)
            {
                //Si no existe en la lista
                involucrado_bonificacion = new InvolucradoBonificacion(
                                                                                                    rut_involucrado,
                                                                                                    nombre_involucrado,
                                                                                                    rut_supervisor,
                                                                                                    nombre_supervisor,
                                                                                                    generateNewDefaultEventos()
                                                                                                );

                involucrado_bonificacion.addHallazgo(nombre_area, cantidad_hallazgo);
                involucrado_bonificacion.addNoConformidad(cantidad_noconformidad);

                list_involucrado_bonificacion.Add(involucrado_bonificacion);
            }
            else
            {
                involucrado_bonificacion.addHallazgo(nombre_area, cantidad_hallazgo);
                involucrado_bonificacion.addNoConformidad(cantidad_noconformidad);
            }
        }



        private InvolucradoBonificacion getInvolucradoBonificacion(string rut_involucrado)
        {
            foreach (InvolucradoBonificacion involucrado_bonificacion in list_involucrado_bonificacion)
            {
                if (involucrado_bonificacion.getRutInvolucrado().Equals(rut_involucrado))
                {
                    return involucrado_bonificacion;
                }
            }

            return null;
        }


        public List<InvolucradoBonificacion> getListInvolucradoBonificacion()
        {
            return this.list_involucrado_bonificacion;
        }
    }
}