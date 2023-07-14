using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class DAOEcommerceCuponClick
    {
        /// <summary>
        /// Obtiene el reporte de membresias de CuponClick
        /// </summary>
        /// <param name="Membresia">Membresia</param>
        /// <param name="FechaInicial">Fecha inicial de insert de la membresia</param>
        /// <param name="FechaFinal">Fecha final de insert de la membresia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteMembresiasCuponClick(String Membresia, DateTime FechaInicial, DateTime FechaFinal, 
            Usuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneReporteMembresiasCuponClick()", "");

                SqlDatabase database = new SqlDatabase(BDEcommerceCuponClick.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMembresiasCuponClick");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@Membresia", DbType.String, Membresia.Equals("") ? null : Membresia);
                database.AddInParameter(command, "@FechaInicio", DbType.Date, FechaInicial.Equals(DateTime.MinValue) ? (DateTime?)null : FechaInicial);
                database.AddInParameter(command, "@FechaFin", DbType.Date, FechaFinal.Equals(DateTime.MinValue) ? (DateTime?)null : FechaFinal);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneReporteMembresiasCuponClick()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la inserción de la registros a la tabla temporal de membresias en base de datos
        /// </summary>
        /// <param name="dtSucTmp">DataTable con los registros de las membresias</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaMembresiasTMP(DataTable dtMembTmp, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaMembresiasCuponClick", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@membresias", dtMembTmp));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        /// <summary>
        /// Inserta en el catálogo membresias de base de datos los cambios del archivo
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Cadena con el resultado del SP</returns>
        public static string InsertaActualizaMembresias(SqlConnection connection, int programa, Usuario elUsuario)
        {
            try
            {
                string BD_response = "";
                SqlCommand command = new SqlCommand("web_CA_InsertaActualizaMembresias", connection);
                command.CommandType = CommandType.StoredProcedure;

                var sqlParameter = new SqlParameter("@message", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 200;

                command.Parameters.Add(sqlParameter);
                command.Parameters.Add(new SqlParameter("@usuario", elUsuario.ClaveUsuario));

                command.Parameters.Add(new SqlParameter("@programa", programa));

                command.CommandTimeout = 5;
                command.ExecuteNonQuery();

                BD_response = command.Parameters["@message"].Value.ToString();

                return BD_response;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

    }
}
