using System;

namespace DALPuntoVentaWeb.Entidades
{
    public class PayCard
    {
        public int IdReporte { get; set; }
        public string NumeroTarjeta { get; set; }
        public string CuentaInterna { get; set; }
        public string ID { get; set; }
        public DateTime FechaOperacion { get; set; }
        public string EstatusEnvio { get; set; }
        public string Mensaje { get; set; }
    }
}
