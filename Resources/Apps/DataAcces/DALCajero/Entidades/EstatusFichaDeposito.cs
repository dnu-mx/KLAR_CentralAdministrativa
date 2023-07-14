using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCajero.Entidades
{
    public class EstatusFichaDeposito
    {
        
        public EstatusFichaDeposito(String clave, String descripcion, Int32 ID)
        {
            this.Clave = clave;
            this.Descripcion = descripcion;
            this.ID_EstatusFichaDeposito = ID;

        }
        public EstatusFichaDeposito()
        {
        }
       
        public String Clave { get; set; }
        public String Descripcion { get; set; }
        public int ID_EstatusFichaDeposito { get; set; }
    }
}
