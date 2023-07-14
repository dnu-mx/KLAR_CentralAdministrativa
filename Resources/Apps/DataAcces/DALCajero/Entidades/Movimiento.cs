using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfases;

namespace DALCajero.Entidades
{
    public class Movimiento
    {

        public Int64 ID_Movimiento { get; set; }
        public enumEstatusMovimiento elEstatus { get; set; }
        public String ClaveUsuarioRegistro  { get; set; }
        public TipoRegistro elTipoRegistro { get; set; }
        public String ClaveColectivaBanco  { get; set; }
        public String ClaveSucursalBancaria  { get; set; }
        public String ClaveCajaBancaria  { get; set; }
        public String ClaveOperador  { get; set; }
        public  DateTime FechaOperacion  { get; set; }
        public Int64 ConsecutivoBancario { get; set; }
        public String Referencia  { get; set; }
        public TipoMovimiento elTipoMovimiento { get; set; }
        public float Importe { get; set; }
        public String Observaciones  { get; set; }
        public String ID_MensajeISO  { get; set; }
        public String CodigoRespuesta  { get; set; }
        public DateTime FechaRegistro  { get; set; }
        public DateTime FechaValor  { get; set; }
        public String NumeroCheque  { get; set; }
       // public TipoOperacionTransaccional elTipoOperacion { get; set; }

        public Boolean EsCorrectoParaAgregar()
        {


            if ((ClaveUsuarioRegistro == null) || (ClaveUsuarioRegistro == ""))
            {
                throw new Exception("No se Clave de Usuario");
            }

            if ((ClaveColectivaBanco == null) || (ClaveColectivaBanco == ""))
            {
                throw new Exception("No se definió Banco de Deposito");
            }
            if ((FechaOperacion == null))
            {
                throw new Exception("No se definió Fecha de Deposito");
            }

            //if ((ConsecutivoBancario == null) || (ConsecutivoBancario == 0))
            if (ConsecutivoBancario == 0)
            {
                throw new Exception("No se definió Consecutivo de Deposito");
            }

            //if ((elTipoOperacion.CodigoProceso == null) || (elTipoOperacion.CodigoProceso == ""))
            //{
            //    throw new Exception("No se definió Destino del Deposito");
            //}

            //if ((Importe == null) || (Importe == 0))
            if (Importe == 0)
            {
                throw new Exception("No se definió Importe de Deposito");
            }

            return true;


        }

        public Boolean EsCorrectoParaAsignar()
        {

            //if ((ID_Movimiento == null) || (ID_Movimiento == 0))
            if (ID_Movimiento == 0)
            {
                throw new Exception("No se definió ID del Movimiento");
            }

            if ((ClaveUsuarioRegistro == null) || (ClaveUsuarioRegistro == ""))
            {
                throw new Exception("No se Clave de Usuario");
            }

            if ((ClaveColectivaBanco == null) || (ClaveColectivaBanco == ""))
            {
                throw new Exception("No se definió Banco de Deposito");
            }
            if ((FechaOperacion == null))
            {
                throw new Exception("No se definió Fecha de Deposito");
            }

            //if ((ConsecutivoBancario == null) || (ConsecutivoBancario == 0))
            if (ConsecutivoBancario == 0)
            {
                throw new Exception("No se definió Consecutivo de Deposito");
            }

            //if ((elTipoOperacion.CodigoProceso == null) || (elTipoOperacion.CodigoProceso == ""))
            //{
            //    throw new Exception("No se definió Destino del Deposito");
            //}

            //if ((Importe == null) || (Importe == 0))
            if (Importe == 0)
            {
                throw new Exception("No se definió Importe de Deposito");
            }

            return true;


        }

        public override String ToString()
        {
            StringBuilder resp = new StringBuilder();

            resp.Append("ID_Movimiento:" + this.ID_Movimiento + "; ");
            resp.Append("ClaveUsuario:" + this.ClaveUsuarioRegistro + "; ");
            resp.Append("ClaveColectivaBanco:" + this.ClaveColectivaBanco + "; ");
            resp.Append("FechaRegistro:" + this.FechaRegistro + "; ");
            resp.Append("Importe:" + this.Importe + "; ");

            return resp.ToString();
        }

    }

}
