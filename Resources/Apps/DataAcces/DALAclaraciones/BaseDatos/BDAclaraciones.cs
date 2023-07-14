using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using DALCentralAplicaciones.Utilidades;

namespace DALAclaraciones.BaseDatos
{
    public static class BDAclaraciones
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
                return Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDLecturaAcl").Valor;
            }
        }

        public static String strBDEscritura
        {
            get
            {
                return Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BDEscrituraAcl").Valor;
            }
        }
    }
}
