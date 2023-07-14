using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALCentroContacto.Entidades
{
    public class TiendaDiconsa
    {
        public int      ID_Tienda       { get; set; }
        public string   ClaveColectiva  { get; set; }
        public string   ClaveAlmacen    { get; set; }
        public string   ClaveTienda     { get; set; }
        public int      ID_Operador     { get; set; }
        public string   Nombre          { get; set; }
        public string   ApellidoPaterno { get; set; }
        public string   ApellidoMaterno { get; set; }
        public string   Email           { get; set; }
        public string   Movil           { get; set; }
    }
}
