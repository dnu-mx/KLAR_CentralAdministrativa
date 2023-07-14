using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALLealtad.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class DAOEcommercePrana
    {
        public static void InsertaPromocionesTMP(DataTable dtFileTmp, SqlConnection connection, SqlTransaction transaccionSQL,
         Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaBeneficiosPrana", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@Beneficios", dtFileTmp));

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
                SqlCommand command = new SqlCommand("sp_CA_cargaCatalogoPromociones", connection);
            
                var sqlParameter = new SqlParameter("@message", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 1000;
               
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
                SqlCommand command = new SqlCommand("web_CA_InsertaSucursalesPrana", connection);

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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
        /// Consulta en base de datos el catálogo de programas
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaProgramas(Usuario elUsuario, Guid AppID, Boolean todos = true)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProgramas");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                database.AddInParameter(command, "@Todos", DbType.Boolean, todos);

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
        /// <param name="IdPrograma">Identificador del programa</param>
        /// <param name="IdCadena">Identificador de cadena</param>
        /// <param name="Cadena">Nombre comercial de la cadena</param>
        /// <param name="IdGiro">Identificador de giro</param>
        /// <param name="Activa">Bandera de promoción activa</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de modificación de la promoción</param>
        /// <param name="FechaFinal">Fecha final del periodo de modificación de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReportePromociones(int IdPrograma, int IdCadena, String Cadena, int IdGiro,
            int Activa, DateTime FechaInicial, DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReportePromociones");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdPrograma", DbType.Int32, IdPrograma);
                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@Cadena", DbType.String, Cadena);
                database.AddInParameter(command, "@IdGiro", DbType.Int32, IdGiro);
                database.AddInParameter(command, "@Activa", DbType.Int32, Activa);
                database.AddInParameter(command, "@FechaIniModif", DbType.Date,
                    FechaInicial.Equals(DateTime.MinValue) ? (DateTime?)null : FechaInicial);
                database.AddInParameter(command, "@FechaFinModif", DbType.Date,
                    FechaFinal.Equals(DateTime.MinValue) ? (DateTime?)null : FechaFinal);

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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
        public static DataSet ListaEstados(string clavePais, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstados");
                database.AddInParameter(command, "@ClavePais", DbType.String, clavePais);

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
        /// <param name="FechaInicial">Fecha inicial del periodo de modificación de la promoción</param>
        /// <param name="FechaFinal">Fecha final del periodo de modificación de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteSucursales(int IdCadena, String Cadena, int IdSucursal, String Sucursal,
            string ClavePais, String ClaveEstado, int Activa, DateTime FechaInicial, DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteSucursales");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@Cadena", DbType.String, Cadena);
                database.AddInParameter(command, "@IdSucursal", DbType.Int32, IdSucursal);
                database.AddInParameter(command, "@Sucursal", DbType.String, Sucursal);
                database.AddInParameter(command, "@ClavePais", DbType.String, ClavePais);
                database.AddInParameter(command, "@ClaveEstado", DbType.String, ClaveEstado);
                database.AddInParameter(command, "@Activa", DbType.Int32, Activa);
                database.AddInParameter(command, "@FechaIniModif", DbType.Date,
                    FechaInicial.Equals(DateTime.MinValue) ? (DateTime?)null : FechaInicial);
                database.AddInParameter(command, "@FechaFinModif", DbType.Date,
                    FechaFinal.Equals(DateTime.MinValue) ? (DateTime?)null : FechaFinal);

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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
        /// Consulta los programas activos de la promoción indicada en el Autorizador
        /// </summary>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ConsultaProgramasPromocion(int idPromocion, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
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

        /// <summary>
        /// Obtiene el reporte de clientes con los filtros indicados
        /// </summary>
        /// <param name="Nombre">Nombre del cliente</param>
        /// <param name="Apellido">Apellido materno o paterno del cliente</param>
        /// <param name="Correo">Correo electrónico del cliente</param>
        /// <param name="Membresia">Número de membresía del cliente</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de registro del cliente</param>
        /// <param name="FechaFinal">Fecha final del periodo de registro del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteClientes(String Nombre, String Apellido, String Correo,
            String Membresia, DateTime FechaInicial, DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteClientesPrana");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@DominioCorreo", DbType.String, Correo);
                database.AddInParameter(command, "@Nombre", DbType.String, Nombre);
                database.AddInParameter(command, "@Apellido", DbType.String, Apellido);
                database.AddInParameter(command, "@Membresia", DbType.String, Membresia);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de emisión de cupones en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposEmision(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposEmisionCupones");

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
        /// Obtiene el reporte de categorías favoritas en el periodo indicado
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de registro del cliente</param>
        /// <param name="FechaFinal">Fecha final del periodo de registro del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteCategoriasFavoritas(DateTime FechaInicial,
            DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteCategoriasFavoritas");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el reporte de promociones favoritas en el periodo indicado
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de registro de la promoción como favorita</param>
        /// <param name="FechaFinal">Fecha final del periodo de registro de la promoción como favorita</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReportePromocionesFavoritas(DateTime FechaInicial,
            DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReportePromocionesFavoritas");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de presencias
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaPresencias(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePresencias");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de clasificaciones
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaClasificaciones(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClasificacion");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las promociones que coinciden con la clave o descripción indicados en base de datos
        /// </summary>
        /// <param name="clavePromo">Clave de la promoción</param>
        /// <param name="descPromo">Descripción de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtienePromosPorClaveODescripcion(string clavePromo, string descPromo, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromos");

                database.AddInParameter(command, "@ClavePromocion", DbType.String, clavePromo);
                database.AddInParameter(command, "@Descripcion", DbType.String, descPromo);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta la información adicional de la promoción con el ID indicado en base de datos
        /// </summary>
        /// <param name="idPromo">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static Promocion ObtieneInfoAdicionalPromocion(int idPromo, Usuario elUsuario)
        {
            SqlDataReader SqlReader = null;
            SqlDataReader SqlReaderRango = null;
            SqlParameter param = null;
            Promocion promocion = new Promocion();

            try
            {
                using (SqlConnection conx = new SqlConnection(BDEcommercePrana.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneInfoPromocion";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdPromocion", SqlDbType.Int);
                        param.Value = idPromo;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    promocion.Id_Promocion = idPromo;
                                    promocion.ClaveCadena = SqlReader["ClaveCadena"].ToString();
                                    promocion.Cadena = SqlReader["NombreComercial"].ToString();
                                    promocion.Descripcion = SqlReader["DescripcionPromo"].ToString();
                                    promocion.Giro = SqlReader["Giro"].ToString();
                                    promocion.TituloPromocion = SqlReader["TituloPromocion"] == null ? "" : SqlReader["TituloPromocion"].ToString();
                                    promocion.TipoDescuento = SqlReader["TipoDescuento"] == null ? "" : SqlReader["TipoDescuento"].ToString();
                                    promocion.Restricciones = SqlReader["Restricciones"] == null ? "" : SqlReader["Restricciones"].ToString();
                                    promocion.EsHotDeal = SqlReader["EsHotDeal"] == DBNull.Value ? (int?)null : (int)SqlReader["EsHotDeal"];
                                    promocion.CarruselHome = SqlReader["CarruselHome"] == DBNull.Value ? (int?)null : (int)SqlReader["CarruselHome"];
                                    promocion.PromoHome = SqlReader["PromoHome"] == DBNull.Value ? (int?)null : (int)SqlReader["PromoHome"];
                                    promocion.Orden = SqlReader["Orden"] == DBNull.Value ? (int?)null : (int)SqlReader["Orden"];
                                    promocion.VigenciaInicio = SqlReader["VigenciaInicio"] == DBNull.Value ? (DateTime?)null : (DateTime)SqlReader["VigenciaInicio"];
                                    promocion.VigenciaFin = SqlReader["VigenciaFin"] == DBNull.Value ? (DateTime?)null : (DateTime)SqlReader["VigenciaFin"];
                                    promocion.Activa = Convert.ToInt32(SqlReader["Activa"]);
                                    promocion.ID_Clasificacion = SqlReader["ID_Clasificacion"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_Clasificacion"];
                                    promocion.PalabrasClave = SqlReader["PalabrasClave"] == null ? "" : SqlReader["PalabrasClave"].ToString();

                                    promocion.URLCupon = SqlReader["URLCupon"] == null ? "" : SqlReader["URLCupon"].ToString();
                                    promocion.ID_Genero = SqlReader["ID_Genero"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_Genero"];
                                    promocion.ID_TipoRedencion = SqlReader["ID_TipoRedencion"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_TipoRedencion"];
                                    promocion.ID_PromoPlus = SqlReader["ID_PromoPlus"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_PromoPlus"];
                                }
                            }
                        }
                        catch (Exception _ex)
                        {
                            throw _ex;
                        }
                    }
                    conx.Close();
                }
                                

                using (SqlConnection conx = new SqlConnection(BDEcommercePrana.strBDLectura))
                {
                    using (SqlCommand cmdRango = new SqlCommand())
                    {
                        cmdRango.Connection = conx;
                        cmdRango.CommandText = "web_CA_ObtieneRagoEdadPromocion";
                        cmdRango.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@ID_Promocion", SqlDbType.Int);
                        param.Value = idPromo;
                        cmdRango.Parameters.Add(param);

                        conx.Open();
                        SqlReaderRango = cmdRango.ExecuteReader();

                        try
                        {
                            if (null != SqlReaderRango)
                            {
                                promocion.RangosEdad = new List<int>();

                                while (SqlReaderRango.Read())
                                {
                                    promocion.RangosEdad.Add ((int)SqlReaderRango["Id_RangoEdad"]);
                                    
                                }
                            }
                        }
                        catch (Exception _ex)
                        {
                            throw _ex;
                        }
                    }
                }

                return promocion;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los programas asignados a la  promoción con el ID indicado en base de datos
        /// </summary>
        /// <param name="idPromo">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtieneProgramasPromocion(int idPromo, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProgramasPromocion");

                database.AddInParameter(command, "@IdPromocion", DbType.Int32, idPromo);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva promoción inactiva en la base de datos, sólo con los datos básicos
        /// </summary>
        /// <param name="idCad">Identificador de la cadena a la que se asocia la nueva promoción</param>
        /// <param name="clavePromo">Clave de la nueva promoción</param>
        /// <param name="descPromo">Descripción de la nueva promoción</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable InsertaPromocionInactiva(int idCad, string clavePromo, string descPromo, string PalabrasClave,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaPromocionInactiva", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdCadena", idCad));
                command.Parameters.Add(new SqlParameter("@ClavePromocion", clavePromo));
                command.Parameters.Add(new SqlParameter("@Descripcion", descPromo));
                command.Parameters.Add(new SqlParameter("@PalabrasClave", PalabrasClave));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter1 = new SqlParameter("@IdNuevaPromo", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;
                command.Parameters.Add(sqlParameter2);

                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("IdNuevaPromo");
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["IdNuevaPromo"] = sqlParameter1.Value.ToString();
                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva cadena en la base de datos
        /// </summary>
        /// <param name="unaCadena">Datos de la nueva cadena</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable InsertaCadena(Cadena unaCadena, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaCadena", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ClaveCadena", unaCadena.ClaveCadena));
                command.Parameters.Add(new SqlParameter("@NombreComercial", unaCadena.NombreComercial));
                command.Parameters.Add(new SqlParameter("@IdGiro", unaCadena.ID_Giro));
                command.Parameters.Add(new SqlParameter("@IdPresencia", unaCadena.ID_Presencia));
                command.Parameters.Add(new SqlParameter("@Facebook", unaCadena.Facebook));
                command.Parameters.Add(new SqlParameter("@Web", unaCadena.Web));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.Parameters.Add(new SqlParameter("@CuentaCLABE", unaCadena.CuentaCLABE));
                command.Parameters.Add(new SqlParameter("@NombreContacto", unaCadena.Contacto));
                command.Parameters.Add(new SqlParameter("@TelefonoContacto", unaCadena.TelContacto));
                command.Parameters.Add(new SqlParameter("@Cargo", unaCadena.Cargo));
                command.Parameters.Add(new SqlParameter("@CelularContacto", unaCadena.CelContacto));
                command.Parameters.Add(new SqlParameter("@Correo", unaCadena.Correo));
                command.Parameters.Add(new SqlParameter("@Extracto", unaCadena.Extracto));                

                if (unaCadena.ID_SubGiro == null)
                {
                    command.Parameters.Add(new SqlParameter("@IDSubGiro", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@IDSubGiro", unaCadena.ID_SubGiro));
                }

                if (unaCadena.ID_PerfilNSE == null)
                {
                    command.Parameters.Add(new SqlParameter("@IDPerfilNSE", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@IDPerfilNSE", unaCadena.ID_PerfilNSE));
                }

                if (unaCadena.ID_TipoEstablecimiento == null)
                {
                    command.Parameters.Add(new SqlParameter("@IDTipoEstablecimiento", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@IDTipoEstablecimiento", unaCadena.ID_TipoEstablecimiento));
                }

                command.Parameters.Add(new SqlParameter("@TicketPromedio", unaCadena.TicketPromedio));

                var sqlParameter1 = new SqlParameter("@IdNuevaCadena", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;
                command.Parameters.Add(sqlParameter2);

                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("IdNuevaCadena");
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["IdNuevaCadena"] = sqlParameter1.Value.ToString();
                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de la promoción en base de datos
        /// </summary>
        /// <param name="promocion">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaPromocion(Promocion promocion, SqlConnection connection, 
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaPromocion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdPromocion", promocion.Id_Promocion));
                command.Parameters.Add(new SqlParameter("@TituloPromo", promocion.TituloPromocion));
                command.Parameters.Add(new SqlParameter("@TipoDescuento", promocion.TipoDescuento));
                command.Parameters.Add(new SqlParameter("@Descripcion", promocion.Descripcion));
                command.Parameters.Add(new SqlParameter("@Restricciones", promocion.Restricciones));
                command.Parameters.Add(new SqlParameter("@EsHotDeal", promocion.EsHotDeal));
                command.Parameters.Add(new SqlParameter("@CarruselHome", promocion.CarruselHome));
                command.Parameters.Add(new SqlParameter("@PromoHome", promocion.PromoHome));
                command.Parameters.Add(new SqlParameter("@Orden", promocion.Orden));
                command.Parameters.Add(new SqlParameter("@VigenciaInicio", promocion.VigenciaInicio));
                command.Parameters.Add(new SqlParameter("@VigenciaFin", promocion.VigenciaFin));
                command.Parameters.Add(new SqlParameter("@Activa", promocion.Activa));
                command.Parameters.Add(new SqlParameter("@IdClasificacion", promocion.ID_Clasificacion));
                command.Parameters.Add(new SqlParameter("@PalabrasClave", promocion.PalabrasClave));

                command.Parameters.Add(new SqlParameter("@URLCupon", promocion.URLCupon));

                if (promocion.ID_TipoRedencion == null)
                {
                    command.Parameters.Add(new SqlParameter("@ID_TipoRedencion", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@ID_TipoRedencion", promocion.ID_TipoRedencion));
                }

                if (promocion.ID_Genero == null)
                {
                    command.Parameters.Add(new SqlParameter("@ID_Genero", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@ID_Genero", promocion.ID_Genero));
                }

                if (promocion.ID_PromoPlus == null)
                {
                    command.Parameters.Add(new SqlParameter("@ID_PromoPlus", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@ID_PromoPlus", promocion.ID_PromoPlus));
                }

                var rangos = "";
                var coma = "";

                foreach (var item in promocion.RangosEdad)
                {
                    rangos = rangos + coma + item.ToString();
                    coma = ",";
                }

                command.Parameters.Add(new SqlParameter("@IDsRangoEdad", rangos));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de la promoción en base de datos
        /// </summary>
        /// <param name="idPromo">Identificador de la promoción</param>
        /// <param name="claveProg">Clave del programa</param>
        /// <param name="activo">Bandera que indica si el programa queda activo o no en la promoción</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaProgramaPromocion(int idPromo, String claveProg, int activo,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaProgramaPromocion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdPromocion", idPromo));
                command.Parameters.Add(new SqlParameter("@ClavePrograma", claveProg));
                command.Parameters.Add(new SqlParameter("@Activo", activo));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las cadenas que coinciden con la clave o descripción indicados en base de datos
        /// </summary>
        /// <param name="claveCad">Clave de la cadena</param>
        /// <param name="nombreCad">Nombre comercial de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtieneCadenasPorClaveONombre(string claveCad, string nombreCad, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenasPorFiltros");

                database.AddInParameter(command, "@ClaveCadena", DbType.String, claveCad);
                database.AddInParameter(command, "@NombreComercial", DbType.String, nombreCad);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta la información adicional de la cadena con el ID indicado en base de datos
        /// </summary>
        /// <param name="idPromo">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static Cadena ObtieneInfoAdicionalCadena(int idCadena, Usuario elUsuario)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            Cadena cadena = new Cadena();

            try
            {
                using (SqlConnection conx = new SqlConnection(BDEcommercePrana.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneInfoCadena";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdCadena", SqlDbType.Int);
                        param.Value = idCadena;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    cadena.ID_Cadena = idCadena;
                                    cadena.ClaveCadena = SqlReader["ClaveCadena"].ToString();
                                    cadena.NombreComercial = SqlReader["NombreComercial"].ToString();
                                    cadena.ID_Giro = Convert.ToInt32(SqlReader["ID_Giro"]);
                                    cadena.ID_Presencia = SqlReader["ID_Presencia"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_Presencia"];
                                    cadena.Facebook = SqlReader["Facebook"] == null ? "" : SqlReader["Facebook"].ToString();
                                    cadena.Web = SqlReader["Web"] == null ? "" : SqlReader["Web"].ToString();

                                    cadena.CuentaCLABE = SqlReader["CuentaCLABE"] == null ? "" : SqlReader["CuentaCLABE"].ToString();
                                    cadena.Contacto = SqlReader["NombreContacto"] == null ? "" : SqlReader["NombreContacto"].ToString();
                                    cadena.TelContacto = SqlReader["TelefonoContacto"] == null ? "" : SqlReader["TelefonoContacto"].ToString();
                                    cadena.Cargo = SqlReader["Cargo"] == null ? "" : SqlReader["Cargo"].ToString();
                                    cadena.CelContacto = SqlReader["CelularContacto"] == null ? "" : SqlReader["CelularContacto"].ToString();
                                    cadena.Correo = SqlReader["Correo"] == null ? "" : SqlReader["Correo"].ToString();
                                    cadena.Extracto = SqlReader["Extracto"] == null ? "" : SqlReader["Extracto"].ToString();

                                    cadena.ID_SubGiro = SqlReader["ID_SubGiro"] == DBNull.Value ? (int?)null : Convert.ToInt32(SqlReader["ID_SubGiro"]);
                                    cadena.TicketPromedio = SqlReader["TicketPromedio"] == null ? "" : SqlReader["TicketPromedio"].ToString();
                                    cadena.ID_PerfilNSE = SqlReader["ID_PerfilNSE"] == DBNull.Value ? (int?)null : Convert.ToInt32(SqlReader["ID_PerfilNSE"]);
                                    cadena.ID_TipoEstablecimiento = SqlReader["ID_TipoEstablecimiento"] == DBNull.Value ? (int?)null : Convert.ToInt32(SqlReader["ID_TipoEstablecimiento"]);

                                }
                            }
                        }
                        catch (Exception _ex)
                        {
                            throw _ex;
                        }
                    }
                }

                return cadena;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las sucursales que pertenecen a la cadena con el ID indicado en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtieneSucursalesPorIDCadena(int idCadena, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursalesDeCadena");

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
        /// Actualiza los datos de la cadena en base de datos
        /// </summary>
        /// <param name="cadena">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaCadena(Cadena cadena, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaCadena", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdCadena", cadena.ID_Cadena));
                command.Parameters.Add(new SqlParameter("@NombreComercial", cadena.NombreComercial));
                command.Parameters.Add(new SqlParameter("@IdGiro", cadena.ID_Giro));
                command.Parameters.Add(new SqlParameter("@IdPresencia", cadena.ID_Presencia));
                command.Parameters.Add(new SqlParameter("@Facebook", cadena.Facebook));
                command.Parameters.Add(new SqlParameter("@Web", cadena.Web));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.Parameters.Add(new SqlParameter("@CuentaCLABE", cadena.CuentaCLABE));
                command.Parameters.Add(new SqlParameter("@NombreContacto", cadena.Contacto));
                command.Parameters.Add(new SqlParameter("@TelefonoContacto", cadena.TelContacto));
                command.Parameters.Add(new SqlParameter("@Cargo", cadena.Cargo));
                command.Parameters.Add(new SqlParameter("@CelularContacto", cadena.CelContacto));
                command.Parameters.Add(new SqlParameter("@Correo", cadena.Correo));
                command.Parameters.Add(new SqlParameter("@Extracto", cadena.Extracto));

                if (cadena.ID_SubGiro == null)
                {
                    command.Parameters.Add(new SqlParameter("@IDSubGiro", DBNull.Value));
                } else
                {
                    command.Parameters.Add(new SqlParameter("@IDSubGiro", cadena.ID_SubGiro));
                }

                if (cadena.ID_PerfilNSE == null)
                {
                    command.Parameters.Add(new SqlParameter("@IDPerfilNSE", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@IDPerfilNSE", cadena.ID_PerfilNSE));
                }

                if (cadena.ID_TipoEstablecimiento == null)
                {
                    command.Parameters.Add(new SqlParameter("@IDTipoEstablecimiento", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@IDTipoEstablecimiento", cadena.ID_TipoEstablecimiento));
                }

                command.Parameters.Add(new SqlParameter("@TicketPromedio", cadena.TicketPromedio));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las sucursales que coinciden con la clave o nombre indicados en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="claveSuc">Clave de la sucursal</param>
        /// <param name="nombreSuc">Nombre de la sucursal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtieneSucursalesPorClaveONombre(int idCadena, string claveSuc, 
            string nombreSuc, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursalesPorFiltros");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);
                database.AddInParameter(command, "@Clave", DbType.String, claveSuc);
                database.AddInParameter(command, "@Nombre", DbType.String, nombreSuc);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta la información adicional de la sucursal con el ID indicado en base de datos
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static Sucursal ObtieneInfoAdicionalSucursal(int idSucursal, Usuario elUsuario)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            Sucursal sucursal = new Sucursal();

            try
            {
                using (SqlConnection conx = new SqlConnection(BDEcommercePrana.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneInfoSucursal";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdSucursal", SqlDbType.Int);
                        param.Value = idSucursal;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    sucursal.Direccion = SqlReader["Direccion"].ToString();
                                    sucursal.Colonia = SqlReader["colonia"].ToString();
                                    sucursal.Ciudad = SqlReader["ciudad"].ToString();
                                    sucursal.CodigoPostal = SqlReader["cp"].ToString();
                                    sucursal.ClavePais = SqlReader["ClavePais"].ToString();
                                    sucursal.ClaveEstado = SqlReader["CveEstado"].ToString();
                                    sucursal.Telefono = SqlReader["telefono"].ToString();
                                    sucursal.Latitud = SqlReader["latitud"].ToString();
                                    sucursal.Longitud = SqlReader["longitud"].ToString();
                                    sucursal.Activa = SqlReader["Activa"] == DBNull.Value ? (int?)null : Convert.ToInt32(SqlReader["Activa"]);
                                    sucursal.ID_Clasificacion = SqlReader["ID_Clasificacion"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_Clasificacion"];
                                }
                            }
                        }
                        catch (Exception _ex)
                        {
                            throw _ex;
                        }
                    }
                }

                return sucursal;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva sucursal en la base de datos
        /// </summary>
        /// <param name="unaSucursal">Datos de la nueva sucursal</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable InsertaSucursal(Sucursal unaSucursal, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaSucursal", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdCadena", unaSucursal.ID_Cadena));
                command.Parameters.Add(new SqlParameter("@Clave", unaSucursal.Clave));
                command.Parameters.Add(new SqlParameter("@Nombre", unaSucursal.Nombre));
                command.Parameters.Add(new SqlParameter("@Direccion", unaSucursal.Direccion));
                command.Parameters.Add(new SqlParameter("@Colonia", unaSucursal.Colonia));
                command.Parameters.Add(new SqlParameter("@Ciudad", unaSucursal.Ciudad));
                command.Parameters.Add(new SqlParameter("@CP", unaSucursal.CodigoPostal));
                command.Parameters.Add(new SqlParameter("@ClavePais", unaSucursal.ClavePais));
                command.Parameters.Add(new SqlParameter("@ClaveEstado", unaSucursal.ClaveEstado));
                command.Parameters.Add(new SqlParameter("@Telefono", unaSucursal.Telefono));
                command.Parameters.Add(new SqlParameter("@Latitud", unaSucursal.Latitud));
                command.Parameters.Add(new SqlParameter("@Longitud", unaSucursal.Longitud));
                command.Parameters.Add(new SqlParameter("@IdClasificacion", unaSucursal.ID_Clasificacion));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter1 = new SqlParameter("@IdNuevaSucursal", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;
                command.Parameters.Add(sqlParameter2);

                command.ExecuteNonQuery();

                DataTable _dt = new DataTable();
                _dt.Columns.Add("IdNuevaSucursal");
                _dt.Columns.Add("Mensaje");
                _dt.Rows.Add();

                _dt.Rows[0]["IdNuevaSucursal"] = sqlParameter1.Value.ToString();
                _dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return _dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de la cadena en base de datos
        /// </summary>
        /// <param name="cadena">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaSucursal(Sucursal sucursal, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaSucursal", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdSucursal", sucursal.ID_Sucursal));
                command.Parameters.Add(new SqlParameter("@Nombre", sucursal.Nombre));
                command.Parameters.Add(new SqlParameter("@Direccion", sucursal.Direccion));
                command.Parameters.Add(new SqlParameter("@Colonia", sucursal.Colonia));
                command.Parameters.Add(new SqlParameter("@Ciudad", sucursal.Ciudad));
                command.Parameters.Add(new SqlParameter("@CodigoPostal", sucursal.CodigoPostal));
                command.Parameters.Add(new SqlParameter("@ClavePais", sucursal.ClavePais));
                command.Parameters.Add(new SqlParameter("@ClaveEstado", sucursal.ClaveEstado));
                command.Parameters.Add(new SqlParameter("@Telefono", sucursal.Telefono));
                command.Parameters.Add(new SqlParameter("@Latitud", sucursal.Latitud));
                command.Parameters.Add(new SqlParameter("@Longitud", sucursal.Longitud));
                command.Parameters.Add(new SqlParameter("@Activa", sucursal.Activa));
                command.Parameters.Add(new SqlParameter("@IdClasificacion", sucursal.ID_Clasificacion));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static void ActualizaExisteLogo(string Cadenas, string Programa, SqlConnection connection, SqlTransaction transaccionSQL,
         Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaExisteLogo", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@Cadenas", Cadenas));
                command.Parameters.Add(new SqlParameter("@valuePrograma", Programa));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las campanas que coinciden con la clave o descripción indicados en base de datos
        /// </summary>
        /// <param name="ClaveCamp">Clave de la campana</param>
        /// <param name="NombreCamp">Nombre de la campaña</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtieneCampanasPorClaveONombre(string ClaveCamp, string NombreCamp, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCampanasPorFiltros");
                        
                database.AddInParameter(command, "@ClaveCampana", DbType.String, ClaveCamp);
                database.AddInParameter(command, "@Nombre", DbType.String, NombreCamp);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva cadena en la base de datos
        /// </summary>
        /// <param name="Campana">Datos de la nueva cadena</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable InsertaCampana(Campana Campana, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaCampana", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ClaveCampana", Campana.ClaveCampana));
                command.Parameters.Add(new SqlParameter("@Nombre", Campana.NombreComercial));
                command.Parameters.Add(new SqlParameter("@IdPrograma", Campana.ID_Programa));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                command.Parameters.Add(new SqlParameter("@PathImagen", Campana.path));

                var sqlParameter1 = new SqlParameter("@IdNuevaCampana", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;
                command.Parameters.Add(sqlParameter2);

                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("IdNuevaCampana");
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["IdNuevaCampana"] = sqlParameter1.Value.ToString();
                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de la campaña en base de datos
        /// </summary>
        /// <param name="Campana">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaCampana(Campana Campana, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaCampana", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdCampana", Campana.ID_Campana));
                command.Parameters.Add(new SqlParameter("@Nombre", Campana.NombreComercial));
                command.Parameters.Add(new SqlParameter("@IdPrograma", Campana.ID_Programa));
                command.Parameters.Add(new SqlParameter("@Activa", Campana.Activo));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                command.Parameters.Add(new SqlParameter("@PathImagen", Campana.path));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las cadenas que coinciden con la clave o descripción indicados en base de datos
        /// </summary>
        /// <param name="Disponibles">Indica se se requiere promociones disponibles o no</param>
        /// <param name="IdCampana">Identificador de campaña</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="ClavePromocion">Clave Promocion que se busca</param>
        /// <param name="Descripcion">Descripcion de la Promocion que se busca</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtienePromocionesPorCampana(int Disponibles, int IdCampana, Usuario elUsuario,
            string ClavePromocion = "", string Descripcion = "")
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromocionesCampanas");

                database.AddInParameter(command, "@IdCampana", DbType.Int32, IdCampana);
                database.AddInParameter(command, "@Disponibles", DbType.Int32, Disponibles);
                database.AddInParameter(command, "@ClavePromocion", DbType.String, ClavePromocion);
                database.AddInParameter(command, "@Descripcion", DbType.String, Descripcion);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de la promoción en base de datos
        /// </summary>
        /// <param name="IdCampana">Entidad de la campaña</param>
        /// <param name="Promociones">Id's de las promociones a asignar divididos por comas</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaPromocionesCampana(int IdCampana, string Promociones, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaPromocionesCampanas", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@Promociones", Promociones));
                command.Parameters.Add(new SqlParameter("@IdCampana", IdCampana));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de Tipos de Objeto
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="todos">true si se agrega la opción Todos</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaTipoObjeto(Usuario elUsuario, Boolean todos)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTipoObjetos");

                database.AddInParameter(command, "@Todos", DbType.Boolean, todos);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de Entidades
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="todos">true si se agrega la opción Todos</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaEntidades(Usuario elUsuario, Boolean todos)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEntidades");

                database.AddInParameter(command, "@Todos", DbType.Boolean, todos);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos los objetos por programa y tipo objeto
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="idPrograma">identificador del programa</param>
        /// <param name="idTipoObjeto">Indentificador del tipo de objeto</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ObtieneListaObjetos(Usuario elUsuario, int idPrograma, int idTipoObjeto)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProgramasObjetos");

                database.AddInParameter(command, "@ID_Programa", DbType.Int32, idPrograma);
                database.AddInParameter(command, "@ID_TipoObjeto", DbType.Int32, idTipoObjeto);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta un nuevo objeto en la base de datos
        /// </summary>
        /// <param name="Objeto">Datos del nuevo objeto</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable InsertaObjeto(ObjetoPrograma objeto, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaObjeto", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ID_TipoObjeto", objeto.ID_TipoObjeto));
                command.Parameters.Add(new SqlParameter("@ID_Programa", objeto.ID_Programa));
                command.Parameters.Add(new SqlParameter("@ID_Entidad", objeto.ID_Entidad));
                command.Parameters.Add(new SqlParameter("@Orden", objeto.Orden));
                command.Parameters.Add(new SqlParameter("@ID_TipoEntidad", objeto.ID_TipoEntidad));
                command.Parameters.Add(new SqlParameter("@URL", objeto.URL));
                command.Parameters.Add(new SqlParameter("@PathImagen", objeto.PathImagen));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter1 = new SqlParameter("@ID_NuevoObjeto", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;

                command.Parameters.Add(sqlParameter2);
                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("IdNuevoObjeto");
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["IdNuevoObjeto"] = sqlParameter1.Value.ToString();
                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un Objeto en base de datos
        /// </summary>
        /// <param name="Campana">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static DataTable ActualizaObjeto(ObjetoPrograma objetoPrograma, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaObjeto", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ID_TipoObjeto", objetoPrograma.ID_TipoObjeto));
                command.Parameters.Add(new SqlParameter("@ID_Programa", objetoPrograma.ID_Programa));

                command.Parameters.Add(new SqlParameter("@ID_ProgramaObjeto", objetoPrograma.ID_ProgramaObjeto));
                command.Parameters.Add(new SqlParameter("@ID_Entidad", objetoPrograma.ID_Entidad));
                command.Parameters.Add(new SqlParameter("@Orden", objetoPrograma.Orden));
                command.Parameters.Add(new SqlParameter("@ID_TipoEntidad", objetoPrograma.ID_TipoEntidad));
                command.Parameters.Add(new SqlParameter("@URL", objetoPrograma.URL));
                command.Parameters.Add(new SqlParameter("@PathImagen", objetoPrograma.PathImagen));
                command.Parameters.Add(new SqlParameter("@Activo", objetoPrograma.Activo));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;

                command.Parameters.Add(sqlParameter2);

                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de subgiros
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaSubGiros(Usuario elUsuario, int idGiro)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSubGiros");

                database.AddInParameter(command, "@ID_Giro", DbType.Int32, idGiro);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de perfiles de niveles económicos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaPerfilNSE(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePerfilNSE");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de géneros
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaGenero(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGeneros");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de rango de edades
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaRangoEdad(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRangoEdades");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de tipos de redenciones
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaTipoRedencion(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposRedenciones");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de promociones plus
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaPromoPlus(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromosPlus");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta un nuevo subGiro en la base de datos
        /// </summary>
        /// <param name="subGiro">Datos del nuevo Subgiro</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable InsertaSubGiro(SubGiro subGiro, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaSubGiro", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@Clave", subGiro.Clave));
                command.Parameters.Add(new SqlParameter("@Descripcion", subGiro.Descripcion));
                command.Parameters.Add(new SqlParameter("@ID_Giro", subGiro.Id_Giro));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter1 = new SqlParameter("@ID_NuevoSubGiro", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;

                command.Parameters.Add(sqlParameter2);
                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("IdNuevoSubGiro");
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["IdNuevoSubGiro"] = sqlParameter1.Value.ToString();
                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un SubGiro en base de datos
        /// </summary>
        /// <param name="subGiro">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static DataTable ActualizaSubGiro(SubGiro subGiro, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaSubgiro", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ID_SubGiro", subGiro.Id_SubGiro));
                command.Parameters.Add(new SqlParameter("@Clave", subGiro.Clave));
                command.Parameters.Add(new SqlParameter("@Descripcion", subGiro.Descripcion));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;

                command.Parameters.Add(sqlParameter2);

                command.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Columns.Add("Mensaje");
                dt.Rows.Add();

                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                return dt;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el reporte de Subscripciones NewsLetter con los filtros indicados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de insertado</param>
        /// <param name="FechaFinal">Fecha final del periodo de insertado</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteNewsLetter(DateTime FechaInicial, DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("ws_ObtenerSuscripcionNewsletterPorFecha");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el reporte de Bitacora de Validación de Membresias con los filtros indicados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de insertado</param>
        /// <param name="FechaFinal">Fecha final del periodo de insertado</param>
        /// <param name="Membresia">Fecha final del periodo de insertado</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteBitacoraMembresia(DateTime FechaInicial, DateTime FechaFinal, String Membresia, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteBitacoraValidacionesMembresia");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@Membresia", DbType.String, Membresia == "" ? null : Membresia);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de tipos de establecimiento
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaTipoEstablecimiento(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTipoEstablecimiento");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos el catálogo de paises
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaPaises(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePaises");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
