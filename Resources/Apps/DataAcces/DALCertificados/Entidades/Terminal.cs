using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCertificados.Entidades
{
    public class Terminal
    {
        public Int64 ID_CadenaComercial { get; set; }
        public Int64 ID_ColectivaTerminal { get; set; }
        public String Sucursal { get; set; }
        public String Afiliacion { get; set; }
        public String laTerminal { get; set; }
        public String ClaveCadena { get; set; }
    }
}
