using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCajero.Entidades
{
    public class TipoMovimiento
    {
        public TipoMovimiento(int ID, String Clave, String Descr)
        {
            this.Clave = Clave;
            this.ID_TipoMovimiento = ID;
            this.Descripcion = Descr;
        }
       public int ID_TipoMovimiento { get; set; }
        public String Clave { get; set; }
        public String Descripcion { get; set; }
    }
}
