namespace DALCortador.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Evento Manual Ajuste Saldo Cuenta Eje
    /// </summary>
    public class EventoCargoAbonoCuentaEjeCacao
    {
        public int      IdEvento            { get; set; }
        public string   ClaveEvento         { get; set; }
        public long     IdCadenaComercial   { get; set; }
        public long     IdGrupoComercial    { get; set; }
        public string   ClaveColectiva      { get; set; }
        public long     IdCuentahabiente    { get; set; }
        public string   Importe             { get; set; }
        public string   Concepto            { get; set; }
        public string   Observaciones       { get; set; }
    }
}
