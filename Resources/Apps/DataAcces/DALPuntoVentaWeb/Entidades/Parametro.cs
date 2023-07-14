using System;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Parámetro Extra
    /// </summary>
    public class Parametro
    {
        public Int32    ID_Parametro                { get; set; }
        public Int32    ID_ValorContrato            { get; set; }
        public String   Nombre                      { get; set; }
        public String   Descripcion                 { get; set; }
        public String   Valor                       { get; set; }
        public Boolean  Preestablecido              { get; set; }
        public Int32    ID_ParametroPrestablecido   { get; set; }
        public Int64    ID_Colectiva                { get; set; }
    }
}
