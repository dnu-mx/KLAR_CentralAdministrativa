using DALCentralAplicaciones.Entidades;
using DALComercioElectronico.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALComercioElectronico.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Moshi-Moshi
    /// </summary>
    public class DAOMoshi
    {
        /// <summary>
        /// Realiza la inserción del archivo en la tabla temporal de base de datos
        /// </summary>
        /// <param name="dtFileTmp">Información por insertar, extraída del archivo</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaProductosTMP(DataTable dtFileTmp, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaLayoutProductosMoshi", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                //database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);

                command.Parameters.Add(new SqlParameter("@productos", dtFileTmp) );

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la inserción de los valores de ls atributos desde
        /// la tabla temporal de base de datos
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaValAtribTMP(SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaValorDeAtributos", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

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
        /// Inserta en el catálogo de productos de base de datos los cambios del archivo
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static string AplicaCambiosAProductos(SqlConnection connection, Usuario elUsuario)
        {
            try
            {
                SqlParameter param = null;

                SqlCommand command = new SqlCommand("sp_cargaCatalogoProductos", connection);

                param = new SqlParameter("@Usuario", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                command.Parameters.Add(param);

                param = new SqlParameter("@message", SqlDbType.VarChar);
                param.Direction=ParameterDirection.Output;
                param.Size = 100;
                command.Parameters.Add(param);

                command.CommandType = CommandType.StoredProcedure;

                command.ExecuteNonQuery();

                if (param.Value != null)
                    return param.Value.ToString();

                return "Procedimiento ejecutado ";
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de familias de productos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaFamilias(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_familias");

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
        /// Consulta el catálogo de sucursales en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaSucursales(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursales");

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
        /// Consulta en base de datos los productos con los filtros indicados
        /// </summary>
        /// <param name="IdFamilia">Identificador de la familia de productos</param>
        /// <param name="SKU">Código SKU del producto</param>
        /// <param name="Nombre">Nombre del producto</param>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <param name="Disponibles">"Código" de disponibilidad de productos:
        ///                             1 = Disponibles, 0 = No disponibles, -1 = Todos </param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los productos filtrados</returns>
        public static DataSet ObtieneProductos(int IdFamilia, String SKU, String Nombre, int IdSucursal, int Disponibles,
            IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProductosSucursal");

                database.AddInParameter(command, "@IdFamilia", DbType.Int32, IdFamilia);
                database.AddInParameter(command, "@SKU", DbType.String, SKU);
                database.AddInParameter(command, "@Nombre", DbType.String, Nombre);
                database.AddInParameter(command, "@IdSucursal", DbType.Int32, IdSucursal);
                database.AddInParameter(command, "@Disponibilidad", DbType.Int32, Disponibles);

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
        /// Actuaiza la disponibilidad del producto en la sucursal indicada en base de datos
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <param name="DispActual">Bandera de disponibilidad actual del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusProducto(SqlConnection connection, SqlTransaction transaccionSQL,
            int IdProducto, int IdSucursal, int DispActual, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaDisponibilidadProductoSucursal", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdProducto", IdProducto));
                command.Parameters.Add(new SqlParameter("@IdSucursal", IdSucursal));
                command.Parameters.Add(new SqlParameter("@Disponibilidad", DispActual));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

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
