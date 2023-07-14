using DALCentralAplicaciones.Utilidades;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class BDEcommerceRedVoucherMC
    {
        public static SqlConnection BDLectura
        {
            get
            {
                return new SqlConnection(strBDLectura);
            }
        }

        public static SqlConnection BDEscritura
        {
            get
            {
                return new SqlConnection(strBDEscritura);
            }
        }

        public static String strBDLectura
        {
            get
            {
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDLecturaEcommRedVoucherMC").Valor;
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDEscrituraEcommRedVoucherMC").Valor;
            }
        }
    }
}
