namespace DALValidacionesBatchPPF.LogicaNegocio.Dtos
{
    public class DtoTransaccion
    {
        public int Dia { get; set; }
        public int Mes { get; set; }
        //public int Incidencias { get; set; }
        public int Operaciones { get; set; }

        public string Movimiento { get; set; }

        public decimal Monto { get; set; }

    }

    public class DtoIncidencia
    {
        public int Dia { get; set; }
        public int Mes { get; set; }
        //public int Incidencias { get; set; }
        public int Incidencias { get; set; }

    }

    public class DtoGrupoTarjeta
    {
        public int Dia { get; set; }
        public int Mes { get; set; }

        public int TarjetasAutorizadas { get; set; }
        public int TarjetasRechazadas{ get; set; }
    }
}