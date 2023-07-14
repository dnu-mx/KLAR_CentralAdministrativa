using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCajero.Entidades
{
    public class TipoOperacionTransaccional
    {
        public TipoOperacionTransaccional(int ID, String CodigoProceso, String Descripcion)
        {
            this.ID_TipoOperacionTransaccional = ID;
            this.CodigoProceso = CodigoProceso;
            this.Descripcion = Descripcion;
        }
        public int ID_TipoOperacionTransaccional  { get; set; }
        public String CodigoProceso  { get; set; }
        public String Descripcion  { get; set; }
        public String EventoAutorizador  { get; set; }
        public Boolean GeneraReverso { get; set; }
    }
}
