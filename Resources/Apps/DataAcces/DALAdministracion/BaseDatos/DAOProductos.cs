using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objeto de acceso a datos para la funcionalidad de Productos
    /// </summary>
    public class DAOProductos
    {
        /// <summary>
        /// Obtiene la lista de cadenas comerciales del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaCadenasComerciales(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenasComerciales");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de productos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoProductos(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProductos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de parámetros multiasignación en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoParametrosMA(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMultiasignacion");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la inserción de un nuevo parámetro multiasignación en el Autorizador
        /// </summary>
        /// <param name="clavePMA">Clave del parámetro multiasignación</param>
        /// <param name="desc">Descripción del parámetro multiasignación</param>
        /// <param name="tipoDatoJava">Tipo de dato Java</param>
        /// <param name="tipoDatoSQL">Tipo de dato SQL</param>
        /// <param name="valorDef">Valor por defecto del parámetro multiasignación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        public static String InsertaParametroMultiasignacion(string clavePMA, string desc, string tipoDatoJava, 
            string tipoDatoSQL, string valorDef, Usuario elUsuario, SqlConnection connection,
            SqlTransaction transaccionSQL)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaParametroMultiasignacion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ClavePMA", clavePMA));
                command.Parameters.Add(new SqlParameter("@Descripcion", desc));
                command.Parameters.Add(new SqlParameter("@TipoDatoJava", tipoDatoJava));
                command.Parameters.Add(new SqlParameter("@TipoDatoSQL", tipoDatoSQL));
                command.Parameters.Add(new SqlParameter("@ValorDefault", valorDef));

                object o = command.ExecuteScalar();

                return o == null ? "" : o.ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta al Autorizador los parámetros multiasignación otorgados a la cadena y
        /// producto indicados
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena comercial</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ConsultaParametrosMA_Asignados(Int64 idCadena, int idProducto, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMultiasignacionOtorgados");

                database.AddInParameter(command, "@IdCadenaComercial", DbType.Int64, idCadena);
                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de entidades en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaEntidades(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEntidades");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de la entidad con el ID indicado en base de datos
        /// </summary>
        /// <param name="IdEntidad">Identificador de la entidad</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaRegistrosEntidad(int IdEntidad, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRegistrosEntidad");

                database.AddInParameter(command, "@IdEntidad", DbType.Int32, IdEntidad);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la inserción/asignación del parámetro multiasignación para el producto
        /// y cadena comercial indicados en el Autorizador
        /// </summary>
        /// <param name="idPMA">Identificador del parámetro multiasignación</param>
        /// <param name="idProd">Identificador del producto</param>
        /// <param name="idCad">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        public static void InsertaParametroACadena(int idPMA, int idCad, int idProd, Usuario elUsuario, 
            SqlConnection connection, SqlTransaction transaccionSQL)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaParametroMultiasignacionACadena", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdParametroMultiasignacion", idPMA));
                command.Parameters.Add(new SqlParameter("@IdCadenaComercial", idCad));
                command.Parameters.Add(new SqlParameter("@IdProducto", idProd));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la actualización de la entidad, registro entidad y valor del parámetro multiasignación
        /// para el identificador indicado en el Autorizador
        /// </summary>
        /// <param name="idValorPMA">Identificador del valor parámetro multiasignación</param>
        /// <param name="idEnt">Identificador de la entidad</param>
        /// <param name="idRegEnt">Identificador del registro entidad</param>
        /// <param name="valor">Nuevo valor del parámetro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <returns>Cadena con la respuesta a la modificación (exitosa o mensaje de error)</returns>
        public static String ActualizaValorParametroMultiasignacion(Int64 idValorPMA, int idEnt, Int64 idRegEnt,
            string valor, Usuario elUsuario, SqlConnection connection, SqlTransaction transaccionSQL)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaValorParametroMultiasignacion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdValorParametroMultiasignacion", idValorPMA));
                command.Parameters.Add(new SqlParameter("@IdEntidad", idEnt));
                command.Parameters.Add(new SqlParameter("@IdRegistroEntidad", idRegEnt));
                command.Parameters.Add(new SqlParameter("@Valor", valor));

                var sqlParameter = new SqlParameter("@Respuesta", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 100;
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();

                return sqlParameter.Value.ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Elimina el registro de valores del parámetro multiasignación con el identificador
        /// indicado en el Autorizador
        /// </summary>
        /// <param name="idValorPMA">Identificador del valor parámetro multiasignación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        public static void EliminaValorParametroMultiasignacion(Int64 idValorPMA, Usuario elUsuario,
            SqlConnection connection, SqlTransaction transaccionSQL)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_EliminaValorParametroMultiasignacion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdValorParametroMultiasignacion", idValorPMA));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
