using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using DALCentralAplicaciones.Utilidades;


namespace DALComercioElectronico.BaseDatos
{
    public static class BdEcommerce
    {
        // static SqlConnection _BDLectura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString());
        // static SqlConnection _BDEscritura = new SqlConnection(ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString());

        public static SqlConnection BdEcoomerce
        {
            get
            {
                return new SqlConnection(strBdEcomerce);
            }
        }

    
        



        public static String strBdEcomerce
        {
            get
            {
                //var guid=new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString());
                //var guid = new Guid("58FBBCCF-5FD5-426B-A2E9-D386C6124BF0");
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "BdEcommerce").Valor;
            }
        }

        public static String strDbMoshiprod
        {
            get
            {
                //var guid=new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString());
                //var guid = new Guid("58FBBCCF-5FD5-426B-A2E9-D386C6124BF0");
                return Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "DbMoshiProd").Valor;
            }
        }



    }
}
