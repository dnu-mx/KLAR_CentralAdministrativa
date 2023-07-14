using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Tarjeta
    /// </summary>
    public class DAOTarjeta
    {
        /// <summary>
        /// Consulta el último identificador de medios de acceso en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static int ConsultaUltimoIdMA(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerUltimoIdMA");
                
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return (int)database.ExecuteScalar(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta la lista de bines que corresponden al grupo de medios de acceso
        /// dentro del Autorizador
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del grupo de medios de acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataTable ListaBinesGrupoMA(int IdGrupoMA, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneBinesGruposMA");

                database.AddInParameter(command, "@IdGrupoMA", DbType.Int32, IdGrupoMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el grupo de medios de acceso de un BIN-Grupo de Medios
        /// de Acceso en el Autorizador
        /// </summary>
        /// <param name="idBinGpoMA">Identificador del registro por modificar</param>
        /// <param name="idGpoMA">Identificador del nuevo grupo de medios de acceso</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaGrupoMABin(int idBinGpoMA, int idGpoMA, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaGrupoMABIN", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdBINGrupoMA", idBinGpoMA));
                command.Parameters.Add(new SqlParameter("@IdGrupoMA", idGpoMA));
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
        /// Actualiza el estatus de un BIN-Grupo de Medios de Acceso en el Autorizador
        /// </summary>
        /// <param name="idBinGpoMA">Identificador del registro por modificar</param>
        /// <param name="estatus">Nuevo estatus del registro</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusBinGrupoMA(int idBinGpoMA, int estatus, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusBINGrupoMA", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdBINGrupoMA", idBinGpoMA));
                command.Parameters.Add(new SqlParameter("@Estatus", estatus));
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
        /// Inserta un nuevo BIN para el Grupo de Medios de Acceso en el Autorizador
        /// </summary>
        /// <param name="idGpoMA">Identificador del grupo de medios de acceso</param>
        /// <param name="claveBin">Clave del nuevo BIN</param>
        /// <param name="descripcion">Descripción del nuevo BIN</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaBinGrupoMA(int idGpoMA, string claveBin, string descripcion,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaBINGrupoMA", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ID_GrupoMA", idGpoMA));
                command.Parameters.Add(new SqlParameter("@ClaveBIN", claveBin));
                command.Parameters.Add(new SqlParameter("@Descripcion", descripcion));
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
        /// Obtiene el listado de parámetros multiasignación CVVDinamicos del producto
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataTable ListaCVVDinamicos(int idProducto, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProdCVVDinamico");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable ds = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneProdCVVDinamico";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar CVV de productos en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el estatus CVV Dinamico del Producto
        /// </summary>
        /// <param name="idProducto">Identificador del Producto por modificar</param>
        /// <param name="estatus">Nuevo estatus del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="log">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusEventoManual(int idProducto, int estatus
            , Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaProdCVVDinamico", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", idProducto));
                        command.Parameters.Add(new SqlParameter("@ActivarCVVDinamico", estatus));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaProdCVVDinamico";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + idProducto);
                        parametros.Add("P2", "@ActivarCVVDinamico=" + estatus);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        pCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al activar/desactivar el evento en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el nombre de embozado de una tarjeta en base de datos
        /// </summary>
        /// <param name="tarjeta">Objeto Tarjeta con los datos por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaNombreEmbozo(Tarjeta tarjeta, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaNombreEmbozo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTarjeta", tarjeta.ID_MA));
                        command.Parameters.Add(new SqlParameter("@Embozo", tarjeta.NombreEmbozo));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaNombreEmbozo";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTarjeta=" + tarjeta.ID_MA.ToString());
                        parametros.Add("P2", "@Embozo=" + tarjeta.NombreEmbozo);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        pCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el nombre de embozado en base de datos");
            }
        }
    }
}
