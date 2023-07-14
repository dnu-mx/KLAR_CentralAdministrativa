using System;

namespace DALValidacionesBatchPPF.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Estadisticas Tarjetas Efectivale
    /// </summary>
    public class EstadisticasTarjetasEFV
    {
        public string   NumTarjeta          { get; set; }
        public DateTime FechaInicial        { get; set; }
        public DateTime FechaFinal          { get; set; }
        public string   Acciones            { get; set; }
        public int      IdAccion            { get; set; }
        public int      IdRegla             { get; set; }
        public int      NumRegistros        { get; set; }
        public int      OperConIncidencia   { get; set; }
    }
}
