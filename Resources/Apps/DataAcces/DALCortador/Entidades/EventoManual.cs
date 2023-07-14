namespace DALCortador.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Evento Manual
    /// </summary>
    public class EventoManual
    {
        public int      IdEvento                { get; set; }
        public string   ClaveEvento             { get; set; }
        public long     IdCadenaComercial       { get; set; }
        public string   ClaveColectiva          { get; set; }
        public long     IdColectivaOrigen       { get; set; }
        public int      IdTipoColectivaOrigen   { get; set; }
        public long     IdColectivaDestino      { get; set; }
        public int      IdTipoColectivaDestino  { get; set; }
        public string   Importe                 { get; set; }
        public string   Concepto                { get; set; }
        public long     Referencia              { get; set; }
        public string   Observaciones           { get; set; }
        public string   MedioAcceso             { get; set; }
        public string   TipoMedioAcceso         { get; set; }
        public string   IVA                     { get; set; }
        public string   FechaAplicacion         { get; set; }
        public string   SaldoCuentaCLDC         { get; set; }
        public string   IdCuentaCLDC            { get; set; }
    }
}
