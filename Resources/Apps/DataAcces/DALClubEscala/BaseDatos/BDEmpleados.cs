using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace DALClubEscala.BaseDatos
{
    class BDEmpleados
    {
        public static SqlConnection BDLectura
        {
            get
            {
                //return new SqlConnection(strBDLectura);
                SqlConnection unaConexion = new SqlConnection(strBDLectura);
                unaConexion.Open();
                return unaConexion;
            }
        }

        public static SqlConnection BDEscritura
        {
            get
            {
                //return new SqlConnection(strBDEscritura);
                SqlConnection unaConexion = new SqlConnection(strBDEscritura);
                unaConexion.Open();
                return unaConexion;
            }
        }

        public static String strBDLectura
        {
            get
            {
                return ConfigurationManager.AppSettings["SettingDatabaseCEEM"].ToString();
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return ConfigurationManager.AppSettings["SettingDatabaseCEEM"].ToString();
            }
        }

        
    }
}
