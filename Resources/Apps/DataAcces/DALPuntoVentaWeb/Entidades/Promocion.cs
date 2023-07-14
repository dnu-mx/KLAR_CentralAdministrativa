using System;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Promoción
    /// </summary>
    public class Promocion
    {
        public Int64    ID_Promocion    { get; set; }
        public String   Clave           { get; set; }
        public String   Descripcion     { get; set; }
        public Int64    Meses           { get; set; }
        public String   Etiqueta        { get; set; }
        public Int64    PrimerPago      { get; set; }
        public Int64    ID_Menu         { get; set; }
        public Int64    Orden           { get; set; }
    }
}
