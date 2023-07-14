using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCajero.Entidades
{
    public class TipoRegistro
    {
        public TipoRegistro(int ID, String Clave, String Descr)
        {
            this.Clave = Clave;
            this.ID_TipoRegistro = ID;
            this.Descripcion = Descr;
        }
       public int ID_TipoRegistro { get; set; }
       public String Clave { get; set; }
       public String Descripcion { get; set; }
    }
}
