using System;

namespace DALAdministracion.Entidades
{
    public class CampanyaMSI
    {
        public int      ID_Campanya             { get; set; }
        public string   Clave                   { get; set; }
        public string   Descripcion             { get; set; }
        public DateTime FechaInicio             { get; set; }
        public DateTime FechaFin                { get; set; }
        public bool     Activa                  { get; set; }
    }

    public class PromocionMSI
    {
        public int      ID_CampanyaPromocion    { get; set; }
        public int      ID_Promocion            { get; set; }
        public int      Diferimiento            { get; set; }
        public int      Meses                   { get; set; }
        public decimal  TasaInteres             { get; set; }
    }
}
