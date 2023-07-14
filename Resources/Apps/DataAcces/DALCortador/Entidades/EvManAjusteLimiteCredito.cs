using System;

namespace DALCortador.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Evento Manual Ajuste Límite de Crédito
    /// </summary>
    public class EvManAjusteLimiteCredito
    {
        public int      IdEvento            { get; set; }
        public String   ClaveEvento         { get; set; }
        public long     IdColectiva         { get; set; }
        public long     IdColectivaCCM      { get; set; }
        public String   ClaveColectivaCCM   { get; set; }
        public String   IdEmisor            { get; set; }
        public String   MedioAcceso         { get; set; }
        public String   LimiteCreditoActual { get; set; }
        public String   LimiteCreditoNuevo  { get; set; }
        public String   Observaciones       { get; set; }
        
    }
}
