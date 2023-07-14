using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DALCajero.Entidades
{
    public class FichaDeposito
    {

        public Int64 ID_FichaDeposito {get;set;}
        public EstatusFichaDeposito elEstatus { get; set; }
        public String ClaveUsuario { get; set; }
        public String ClaveColectivaBanco { get; set; }
        public String ClaveSucursalBancaria { get; set; }
        public String ClaveCajaBancaria { get; set; }
        public String ClaveOperador { get; set; }
        public String FechaOperacion { get; set; }
        public Int64 Consecutivo { get; set; }
        public String Referencia { get; set; }
        public float Importe { get; set; }
        public String ClaveTipoMA { get; set; }
        public String ClaveMedioAcceso { get; set; }
        public String Observaciones { get; set; }
        public String Afiliacion { get; set; }
        public String AFLDescripcion { get; set; }
        public String FechaRegistro { get; set; }
        public TipoOperacionTransaccional Operacion { get; set; }
        public String DataTransaccionales { get; set; }
        public Boolean EsCorrectoParaAgregar()
        {

        
            if ((ClaveUsuario == null) || (ClaveUsuario==""))
            {
                throw new Exception("No se definió ID de la Ficha de Deposito");
            }

            if ((ClaveColectivaBanco == null) || (ClaveColectivaBanco==""))
            {
                throw new Exception("No se definió Banco de Deposito");
            }
            if ((FechaOperacion == null) || (FechaOperacion==""))
            {
                throw new Exception("No se definió Fecha de Deposito");
            }

            //if ((Consecutivo == null) || (Consecutivo==0))
            if (Consecutivo == 0)
            {
                throw new Exception("No se definió Consecutivo de Deposito");
            }

            if ((Operacion.CodigoProceso == null) || (Operacion.CodigoProceso==""))
            {
                throw new Exception("No se definió Destino del Deposito");
            }

            //if ((Importe == null) || (Importe==0))
            if (Importe == 0)
            {
                throw new Exception("No se definió Importe de Deposito");
            }
               
            return true;
           

        }

        public Boolean EsCorrectoParaAsignar()
        {

            //if ((ID_FichaDeposito == null) || (ID_FichaDeposito == 0))
            if (ID_FichaDeposito == 0)
            {
                throw new Exception("No se definió ID de la Ficha de Deposito");
            }

            if ((ClaveUsuario == null) || (ClaveUsuario == ""))
            {
                throw new Exception("No se definió ID de la Ficha de Deposito");
            }

            if ((ClaveColectivaBanco == null) || (ClaveColectivaBanco == ""))
            {
                throw new Exception("No se definió Banco de Deposito");
            }
            if ((FechaOperacion == null) || (FechaOperacion == ""))
            {
                throw new Exception("No se definió Fecha de Deposito");
            }

            //if ((Consecutivo == null) || (Consecutivo == 0))
            if (Consecutivo == 0)
            {
                throw new Exception("No se definió Consecutivo de Deposito");
            }

            if ((Operacion.CodigoProceso == null) || (Operacion.CodigoProceso == ""))
            {
                throw new Exception("No se definió Destino del Deposito");
            }

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

            resp.Append("ID_FichaDeposito:" + this.ID_FichaDeposito + "; ");
            resp.Append("ClaveUsuario:" + this.ClaveUsuario + "; ");
            resp.Append("ClaveColectivaBanco:" + this.ClaveColectivaBanco + "; ");
            resp.Append("FechaRegistro:" + this.FechaRegistro + "; ");
            resp.Append("Importe:" + this.Importe + "; ");

            return resp.ToString();
        }
    }
}
