using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCajero.Entidades
{
    public class EstatusMovimiento
    {
        public EstatusMovimiento(String clave, String descripcion, Int32 ID)
        {
            this.Clave = clave;
            this.Descripcion = descripcion;
            this.ID_EstatusMovimiento = ID;

        }
        public EstatusMovimiento()
        {
        }

       public String Clave { get; set; }
       public String Descripcion { get; set; }
       public int ID_EstatusMovimiento { get; set; }
    }
}
