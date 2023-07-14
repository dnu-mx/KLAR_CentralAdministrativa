using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALLealtad.Entidades
{
    public class SubGiro
    {
        public int Id_SubGiro       { get; set; }
        public String Clave         { get; set; }
        public String Descripcion   { get; set; }
        public int Id_Giro          { get; set; }
    }
}
