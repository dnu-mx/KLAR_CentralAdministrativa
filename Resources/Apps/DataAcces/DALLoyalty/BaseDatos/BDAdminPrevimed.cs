using DALCentralAplicaciones.Utilidades;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DALCentroContacto.BaseDatos
{
    public static class BDAdminPrevimed
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
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDLecturaAdminPrevimed").Valor;
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDEscrituraAdminPrevimed").Valor;
            }
        }
    }
}
