using DALCajero.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DALCajero.BaseDatos
{
    static public class DAOCatalogos
    {
        //static string _DBCajeroConsulta = DALCajero.BaseDatos.BDCajero.strBDLectura;// ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString();
        //static string _DBCajeroEscritura =DALCajero.BaseDatos.BDCajero.strBDEscritura;// ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString();

        public static DataSet ListaBancos()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaBancos");
                return database.ExecuteDataSet(command);

            }
            catch (Exception ex)
            {
                Loguear.Error(ex,"");
                throw new CAppException(8007, ex.Message);
            }
        }

        public static DataSet ListaTiposOperacion()
        {
            try{
            SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
            DbCommand command = database.GetStoredProcCommand("web_SeleccionaTiposOperacion");
            return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }

        }

        public static DataSet ListaCadenasComerciales()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaCadenasComerciales");
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }

        }

        public static DataSet ListaTiposMedioAcceso()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaTiposMedioAcceso");
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }
        }

        public static String PDVOL_ListaBancos()
        {
            DataSet datos = null;
            StringBuilder elXML = new StringBuilder();
            Boolean isFirst = true;

            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaBancos");
               // database.ExecuteDataSet(command);


                datos = database.ExecuteDataSet(command);

                elXML.Append("\"BANCOS\" : {");
                elXML.Append("\"BANCO\" : [ ");


                if (null != datos)
                {
                    for (int k = 0; k < datos.Tables[0].Rows.Count; k++)
                    {
                        if (!isFirst)
                        {
                            elXML.Append(",");
                        }

                        elXML.Append("{");
                        elXML.Append("\"CLAVE\" : \"");
                        elXML.Append(datos.Tables[0].Rows[k]["Clave"].ToString().Trim());
                        elXML.Append("\",");
                        elXML.Append("\"DESCRIPCION\" : \"");
                        elXML.Append(datos.Tables[0].Rows[k]["Descripcion"].ToString().Trim());
                        elXML.Append("\"");
                        elXML.Append("}");

                        isFirst = false;

                    }
                }
                elXML.Append("]");
                elXML.Append("}");

                return elXML.ToString();

            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }
        }

       
      

    }
}
