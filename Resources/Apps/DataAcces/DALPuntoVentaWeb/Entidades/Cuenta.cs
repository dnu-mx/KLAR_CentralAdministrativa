using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Cuenta
    /// </summary>
    public class Cuenta
    {
        public Int64    ID_Cuenta                   { get; set; }
        public int      ID_TipoCuenta               { get; set; }
        public int      ID_GrupoCuentas             { get; set; }
        public int      ID_EstatusCuenta            { get; set; }
        public Int64    ID_Colectiva                { get; set; }
        public Int64    ID_CuentaLC                 { get; set; }
        public int      Nivel                       { get; set; }
        public String   Descripcion                 { get; set; }
        public float    SaldoActual                 { get; set; }
        public int      HeredaSaldo                 { get; set; }
        public Int64    ID_ColectivaCadenaComercial { get; set; }
        public int      ID_Periodo                  { get; set; }
        public DateTime Vigencia                    { get; set; }
    }
}
