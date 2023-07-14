using System;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Menú TPV
    /// </summary>
    public class MenuTPV
    {
        public Int64 ID_Menu { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public Int64 Version { get; set; }
    }
}
