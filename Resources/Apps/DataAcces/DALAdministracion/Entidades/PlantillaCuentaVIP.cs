namespace DALAdministracion.Entidades
{
    public class PlantillaCuentaVIP
    {
        public int      IdEA_NivelCtaCumpl      { get; set; }
        public int      IdEA_NivelCtaCumplPers  { get; set; }
        public int      IdEA_SaldoMaxPers       { get; set; }
        public int      IdEA_MaxAbonoPers       { get; set; }
        public long     IdPlantilla             { get; set; }
        public long     IdPMA_NivelCtaCumplPers { get; set; }
        public long     IdPMA_SaldoMaxPers      { get; set; }
        public long     IdPMA_MaxAbonoPers      { get; set; }
        public long     IdVPMA_NivelCtaCumpl    { get; set; }
        public string   Motivo                  { get; set; }
        public string   RespListaNegra          { get; set; }
    }
}
