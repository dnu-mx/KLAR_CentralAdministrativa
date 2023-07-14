using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
//using DALCentralAplicaciones.Utilidades;

namespace DALClubEscala.BaseDatos
{
    public static class DBClubEscala
    {
       // static SqlConnection _BDLectura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString());
       // static SqlConnection _BDEscritura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString());

        public static SqlConnection BDLectura
        {
            get
            {
                SqlConnection unaConexion = new SqlConnection(strBDLectura);
                unaConexion.Open();
                return unaConexion;
            }
        }

        public static SqlConnection BDEscritura
        {
            get
            {
                //return new SqlConnection();
                SqlConnection unaConexion = new SqlConnection(strBDEscritura);
                unaConexion.Open();
                return unaConexion;
            }
        }

        public static String strBDLectura
        {
            get
            {
                return ConfigurationManager.AppSettings["SettingDatabase"].ToString();
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return ConfigurationManager.AppSettings["SettingDatabase"].ToString();
            }
        }

        
    }
}
