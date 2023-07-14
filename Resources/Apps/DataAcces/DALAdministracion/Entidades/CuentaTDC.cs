using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Cuenta Tarjeta de Crédito
    /// </summary>
    public class CuentaTDC
    {
        public int      ID_Cuenta       { get; set; }
        public int      ID_MA           { get; set; }
        public string   NumeroTarjeta   { get; set; }
        public int      FechaCorte      { get; set; }
        public int      FechaLimitePago { get; set; }
        public decimal  LimiteCredito   { get; set; }
        public string   NombreEmbozado  { get; set; }
        public DateTime VigenciaTarjeta { get; set; }
    }
}
