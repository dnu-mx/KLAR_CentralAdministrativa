namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control para la lista de resumen de movimientos de cuenta
    /// </summary>
    public class ResumenMovimientosCuenta
    {
        public float    LimiteCredito       { get; set; }
        public float    Cargos              { get; set; }
        public float    Abonos              { get; set; }
        public float    SaldoCorte          { get; set; }
        public float    SaldoActual         { get; set; }
        public float    CreditoDisponible   { get; set; }
        public float    SaldoDisponible     { get; set; }
        public float    SaldoInicial        { get; set; }
        public float    SaldoFinal          { get; set; }
    }
}
