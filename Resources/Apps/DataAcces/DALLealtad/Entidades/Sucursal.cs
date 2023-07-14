using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALLealtad.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Sucursal
    /// </summary>
    public class Sucursal
    {
        public int      ID_Cadena           { get; set; }
        public int      ID_Sucursal         { get; set; }
        public String   Clave               { get; set; }
        public String   Nombre              { get; set; }
        public String   Direccion           { get; set; }
        public String   Colonia             { get; set; }
        public String   Ciudad              { get; set; }
        public String   CodigoPostal        { get; set; }
        public String   ClavePais           { get; set; }
        public String   ClaveEstado         { get; set; }
        public String   Telefono            { get; set; }
        public String   Latitud             { get; set; }
        public String   Longitud            { get; set; }
        public int?     Activa              { get; set; }
        public int?     ID_Clasificacion    { get; set; }
    }
}
