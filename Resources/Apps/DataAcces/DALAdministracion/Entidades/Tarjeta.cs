using System;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Tarjeta
    /// </summary>
    public class Tarjeta
    {
        public string NumeroTarjeta     { get; set; }
        public string FechaExpiracion   { get; set; }
        public string CodigoServicio    { get; set; }

        public string CVV               { get; set; }
        public string CVV2              { get; set; }
        public string NIP               { get; set; }

        public int ID_MA                { get; set; }
        public string NombreEmbozo      { get; set; }
    }
}
