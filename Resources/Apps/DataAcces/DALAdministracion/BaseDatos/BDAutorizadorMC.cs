using DALCentralAplicaciones.Utilidades;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DALAdministracion.BaseDatos
{
    public class BDAutorizadorMC
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
                return Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDLecturaAutMC").Valor;
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDEscrituraAutMC").Valor;
            }
        }
    }
}
