using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class DAOEcommerce
    {
        /// <summary>
        /// Obtiene la lista de cadenas en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCadenas(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenas");

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
        /// Obtiene la lista de tipos de cupones en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposCupon(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposCupon");

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
        /// Obtiene la lista de promociones que corresponden con el ID de cadena y 
        /// clave de promoción en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="clavePromo">Clave de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaPromociones(int idCadena, string clavePromo, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromociones");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);
                database.AddInParameter(command, "@ClavePromocion", DbType.String, clavePromo);

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
        /// Realiza la actualización del tipo de cupón de la promoción indicada
        /// en base de datos
        /// </summary>
        /// <param name="idPromo">Identificador de la promoción</param>
        /// <param name="idTipoCupon">Identificador del tipo de cupón</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaTipoCuponPromocion(int idPromo, int idTC, SqlConnection connection, 
            SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaPromocion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdPromocion", idPromo));
                command.Parameters.Add(new SqlParameter("@IdTipoCupon", idTC));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza la cadena en el Autorizador
        /// </summary>
        /// <param name="claveCad">Clave de la cadena</param>
        /// <param name="cad">Nombre de la cadena</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaCadenaAut(String claveCad, String cad, SqlConnection connection,
            SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaActualizaCadena", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ClaveCadena", claveCad));
                command.Parameters.Add(new SqlParameter("@NombreCadena", cad));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza la promoción en el Autorizador
        /// </summary>
        /// <param name="promo">Descripción de la promoción</param>
        /// <param name="clavePromo">Clave de la promoción</param>
        /// <param name="claveCad">Clave de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaPromocionAut(String promo, String clavePromo, String claveCad, 
            IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_Promocion_GenerarNuevaPromocion");

                database.AddInParameter(command, "@ClavePromocion", DbType.String, clavePromo);
                database.AddInParameter(command, "@ClaveDivisa", DbType.String, clavePromo);
                database.AddInParameter(command, "@ClaveCadenaComercial", DbType.String, claveCad);
                database.AddInParameter(command, "@DescripcionPromocion", DbType.String, promo);

                database.ExecuteNonQuery(command);
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
        public static DataSet ListaTiposEmision(IUsuario elUsuario, Guid AppID)
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
        /// Obtiene la lista los algoritmos de emisión de cupones en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaAlgoritmos(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneAlgoritmosEmisionCupones");

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
        /// Actualiza la promoción en el Autorizador
        /// </summary>
        /// <param name="promo">Descripción de la promoción</param>
        /// <param name="clavePromo">Clave de la promoción</param>
        /// <param name="claveCad">Clave de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void GeneraCuponesEnAutorizador(String promo, String clavePromo, String claveCad,
            IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_Promocion_GenerarCupones");

                database.AddInParameter(command, "@ClaveEvento", DbType.String, clavePromo);
                database.AddInParameter(command, "@CantidadCupones", DbType.Int32, clavePromo);
                database.AddInParameter(command, "@TipoEmision", DbType.String, claveCad);
                database.AddInParameter(command, "@Algoritmo", DbType.String, promo);
                database.AddInParameter(command, "@Longitud", DbType.Int32, promo);
                database.AddInParameter(command, "@FechaExpiracion", DbType.DateTime, promo);
                database.AddInParameter(command, "@ValorCupon", DbType.Int32, promo);
                database.AddInParameter(command, "@ConsumosValidos", DbType.Int32, promo);
                database.AddInParameter(command, "@ClaveCadenaComercial", DbType.Int32, promo);
                database.AddInParameter(command, "@TipoCupon", DbType.String, promo);


                database.ExecuteNonQuery(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        public static DataSet ListaProgramas(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProgramas");
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
