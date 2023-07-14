using System;

namespace DALLealtad.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Cadena
    /// </summary>
    public class Cadena
    {
        public Int64    ID_Cadena       { get; set; }
        public String   ClaveCadena     { get; set; }
        public String   NombreComercial { get; set; }
        public int      ID_Giro         { get; set; }
        public int?     ID_Presencia    { get; set; }
        public String   Facebook        { get; set; }
        public String   Web             { get; set; }
        public String   CuentaCLABE     { get; set; }
        public String   Contacto        { get; set; }
        public String   TelContacto     { get; set; }
        public String   Cargo           { get; set; }
        public String   CelContacto     { get; set; }
        public String   Correo          { get; set; }
        public String   Extracto        { get; set; }

        public int?     ID_SubGiro      { get; set; }
        public String   TicketPromedio  { get; set; }
        public int?     ID_PerfilNSE    { get; set; }
        public int?     ID_TipoEstablecimiento { get; set; }
    }
}
