using DALCentralAplicaciones.Utilidades;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class BDEcommerceRedVoucher
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
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDLecturaEcommRedVoucher").Valor;
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDEscrituraEcommRedVoucher").Valor;
            }
        }
    }
}
