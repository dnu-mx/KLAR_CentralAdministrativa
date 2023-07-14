using System;

namespace DALValidacionesBatchPPF.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Valor Parametro Multiasignación - Regla
    /// </summary>
    public class ValorParametroMARegla
    {
        public Int32    ID_ParametroMA      { get; set; }
        public Int32    ID_Entidad          { get; set; }
        public Int32    ID_RegistroEntidad  { get; set; }
        public Int64    ID_CadenaComercial  { get; set; }
        public Int32    ID_Producto         { get; set; }
        public String   Valor               { get; set; }
        public String   ValorAlertar        { get; set; }
        public String   ValorRechazar       { get; set; }
        public String   ValorBloquear       { get; set; }
    }
}
