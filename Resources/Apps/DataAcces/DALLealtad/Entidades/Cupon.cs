using System;

namespace DALLealtad.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Cupon
    /// </summary>
    public class Cupon
    {
        public String   ClaveEvento             { get; set; }
        public int      CantidadCupones         { get; set; }
        public String   PromoCode               { get; set; }
        public String   TipoEmision             { get; set; }
        public String   Algoritmo               { get; set; }
        public int      Longitud                { get; set; }
        public DateTime FechaExpiracion         { get; set; }
        public double   ValorCupon              { get; set; }
        public int      ConsumosValidos         { get; set; }
        public String   ClaveCadenaComercial    { get; set; }
        public String   TipoCupon               { get; set; }
    }
}
