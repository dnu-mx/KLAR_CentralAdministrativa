using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Dirección Colectiva
    /// </summary>
    public class DireccionColectiva
    {
        public Int64    ID_Colectiva        { get; set; }
        public Int32    ID_Direccion        { get; set; }
        public Int32    ID_TipoDireccion    { get; set; }
        public String   Calle               { get; set; }
        public String   NumExterior         { get; set; }
        public String   NumInterior         { get; set; }
        public String   EntreCalles         { get; set; }
        public String   Referencias         { get; set; }
        public String   CodigoPostal        { get; set; }
        public Int32    ID_Asentamiento     { get; set; }
        public String   Colonia             { get; set; }
        public String   ClaveMunicipio      { get; set; }
        public String   ClaveEstado         { get; set; }


    }
}
