using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class TipoCuenta
    {
        public int     ID_TipoCuenta        { get; set; }
        public string  CodigoTipoCuentaISO  { get; set; }
        public string  ClaveTipoCuenta      { get; set; }
        public string  Descripcion          { get; set; }
        public Boolean GeneraDetalle        { get; set; }
        public Boolean GeneraCorte          { get; set; }
        public int     ID_Divisa            { get; set; }
        public int     ID_Periodo           { get; set; }
        public string  BreveDescripcion     { get; set; }
        public Boolean EditarSaldoGrid      { get; set; }
        public Boolean InteractuaCajero     { get; set; }
        public int     ID_NaturalezaCuenta  { get; set; }
    }
}
