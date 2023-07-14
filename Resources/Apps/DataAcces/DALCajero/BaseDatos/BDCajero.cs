using DALCentralAplicaciones.Utilidades;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DALCajero.BaseDatos
{
    public static class BDCajero
    {
       // static SqlConnection _BDLectura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString());
       // static SqlConnection _BDEscritura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString());

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
                return Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDLecturaCaj").Valor;
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDEscrituraCaj").Valor;
            }
        }
    }
}
