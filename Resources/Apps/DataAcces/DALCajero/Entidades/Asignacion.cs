using System;
using System.Text;

namespace DALCajero.Entidades
{
    public class Asignacion
    {
        String _ClaveTipoAsignacion="";

        public Int64 ID_MBDeposito { get; set; }
        public Movimiento elMovimiento { get; set; }
        public FichaDeposito laFichaDeposito { get; set; }
        public String ClaveTipoAsignacion { get { return _ClaveTipoAsignacion.PadLeft(3, '0'); } set { _ClaveTipoAsignacion = value; } }
        public String ClaveUsuarioAsignador { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public EstatusAsignacion elEstatusAsignacion { get; set; }
        public String ID_MensajeISO { get; set; }
        public String CodigoRespuesta { get; set; }
        public TipoOperacionTransaccional tipoOperacionTrx { get; set; }

        public override String ToString()
        {
            StringBuilder resp = new StringBuilder();

            resp.Append("ID_Movimiento:" + this.elMovimiento.ID_Movimiento + "; ");
            resp.Append("ID_Ficha:" + this.laFichaDeposito.ID_FichaDeposito + "; ");
            resp.Append("ClaveTipoAsignacion:" + this.ClaveTipoAsignacion + "; ");
            resp.Append("FechaAsignacion:" + this.FechaAsignacion + "; ");
            resp.Append("Importe Movimiento:" + this.elMovimiento.Importe + "; ");
            resp.Append("Importe Ficha:" + this.laFichaDeposito.Importe + "; ");

            return resp.ToString();
        }

    }
}
