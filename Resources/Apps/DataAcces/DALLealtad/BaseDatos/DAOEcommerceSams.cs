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
    public class DAOEcommerceSams
    {
        public static void InsertaPromocionesTMP(DataTable dtFileTmp, SqlConnection connection, SqlTransaction transaccionSQL,
         Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("[web_CA_InsertaPromociones]", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                //database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);

                command.Parameters.Add(new SqlParameter("@promociones", dtFileTmp));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta en el catálogo de productos de base de datos los cambios del archivo
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static string AplicaCambiosAProductos(SqlConnection connection, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("[sp_CA_cargaCatalogoPromociones]", connection);

                var sqlParameter = new SqlParameter("@message", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 100;
                //command.Parameters.Add(new SqlParameter("@mensaje",SqlDbType.VarChar,250,ParameterDirection.Output));



                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(sqlParameter);
                command.Parameters.Add(new SqlParameter("@usuario", elUsuario.ClaveUsuario.ToString()));


                command.ExecuteNonQuery();

                if (sqlParameter.Value != null)
                    return sqlParameter.Value.ToString();

                return "Procedimiento ejecutado ";

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la inserción de la registros a la tabla temporal de sucursales en base de datos
        /// </summary>
        /// <param name="dtSucTmp">DataTable con los registros de las sucursales</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaSucursalesTMP(DataTable dtSucTmp, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaSucursalesLayout", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@sucursales", dtSucTmp));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta en el catálogo  sucursales de base de datos los cambios del archivo
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Cadena con el resultado del SP</returns>
        public static string InsertaActualizaSucursales(SqlConnection connection, Usuario elUsuario)
        {
            try
            {
                string BD_response = "";
                SqlCommand command = new SqlCommand("web_CA_InsertaActualizaSucursales", connection);
                command.CommandType = CommandType.StoredProcedure;

                var sqlParameter = new SqlParameter("@message", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 200;

                command.Parameters.Add(sqlParameter);
                command.Parameters.Add(new SqlParameter("@usuario", elUsuario.ClaveUsuario));

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

        /// <summary>
        /// Consulta en base de datos el catálogo de cadenas
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaCadenas(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenas");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de giros
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaGiros(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGiros");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de promociones
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaPromociones(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromociones");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el reporte de promociones con los filtros indicados
        /// </summary>
        /// <param name="IdCadena">Identificador de cadena</param>
        /// <param name="Cadena">Nombre comercial de la cadena</param>
        /// <param name="IdGiro">Identificador de giro</param>
        /// <param name="IdPromocion">Identificador de promoción</param>
        /// <param name="Activa">Bandera de promoción activa</param>
        /// <param name="FechaInicio">Fecha de inicio de vigencia de la promoción</param>
        /// <param name="FechaFin">Fecha de fin de vigencia de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReportePromociones(int IdCadena, String Cadena, int IdGiro, int IdPromocion,
            int Activa, DateTime FechaInicio, DateTime FechaFin, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReportePromociones");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@Cadena", DbType.String, Cadena);
                database.AddInParameter(command, "@IdGiro", DbType.Int32, IdGiro);
                database.AddInParameter(command, "@IdPromocion", DbType.Int32, IdPromocion);
                database.AddInParameter(command, "@Activa", DbType.Int32, Activa);
                database.AddInParameter(command, "@FechaInicio", DbType.Date, 
                    FechaInicio.Equals(DateTime.MinValue) ? (DateTime?)null : FechaInicio);
                database.AddInParameter(command, "@FechaFin", DbType.Date,
                    FechaFin.Equals(DateTime.MinValue) ? (DateTime?)null : FechaFin);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de sucursales
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaSucursales(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoSucursales");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de estados
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaEstados(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("sp_getEstados");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el reporte de sucursales con los filtros indicados
        /// </summary>
        /// <param name="IdCadena">Identificador de cadena</param>
        /// <param name="Cadena">Nombre comercial de la cadena</param>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <param name="Sucursal">Nombre de la sucursal</param>
        /// <param name="ClaveEstado">Clave del estado</param>
        /// <param name="Activa">Bandera de sucursal activa</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteSucursales(int IdCadena, String Cadena, int IdSucursal, String Sucursal,
            String ClaveEstado, int Activa, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteSucursales");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@Cadena", DbType.String, Cadena);
                database.AddInParameter(command, "@IdSucursal", DbType.Int32, IdSucursal);
                database.AddInParameter(command, "@Sucursal", DbType.String, Sucursal);
                database.AddInParameter(command, "@ClaveEstado", DbType.String, ClaveEstado);
                database.AddInParameter(command, "@Activa", DbType.Int32, Activa);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos las ciudades que corresponden a la clave de estado
        /// </summary>
        /// <param name="claveEdo">Clave del estado por buscar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaCiudades(String claveEdo, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCiudades");

                database.AddInParameter(command, "@ClaveEstado", DbType.String, claveEdo);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos la longitud de los campos de las tablas
        /// implicadas en la actualización de promociones
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ObtieneLongitudCampos(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneLongitudCamposPromociones");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de cadenas
        /// </summary>
        /// <param name="claveCadena">Clave de la cadena</param>
        /// <param name="nombreComercial">Nombre comercial de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ConsutaCadenasPromociones(String claveCadena, String nombreComercial, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenasPromociones");

                database.AddInParameter(command, "@ClaveCadena", DbType.String, claveCadena);
                database.AddInParameter(command, "@NombreCadena", DbType.String, nombreComercial);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las promociones activas de la cadena indicada en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtienePromocionesActivasCadena(int idCadena, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromocionesActivasCadena");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las sucursales activas de la cadena indicada en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneSucursalesActivasCadena(int idCadena, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursalesActivasCadena");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los programas activas de la promoción indicada en el Autorizador
        /// </summary>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ConsultaProgramasPromocion(int idPromocion, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ConsultaProgramasPromocion");

                database.AddInParameter(command, "@IdPromocion", DbType.Int32, idPromocion);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las sucursales activas de la promoción indicada en el Autorizador
        /// </summary>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ConsultaSucursalesPromocion(int idPromocion, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceSams.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ConsultaSucursalesPromocion");

                database.AddInParameter(command, "@IdPromocion", DbType.Int32, idPromocion);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
