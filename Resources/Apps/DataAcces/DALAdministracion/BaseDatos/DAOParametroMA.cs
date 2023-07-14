using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
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
using System.Linq;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Parámetro Multiasignación
    /// </summary>
    public class DAOParametroMA
    {
        /// <summary>
        /// Consulta las Cadenas Comerciales en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCadenasComerciales (IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerCadenasComerciales");
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
        /// Consulta la totalidad de parámetros multiasignación para la entidad Regla, asociados o no a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdRegla">Identificador de la Regla</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTotalReglasPMA(int IdCadenaComercial, int IdRegla, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTotalReglasPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@IdRegla", DbType.Int32, IdRegla);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta los parámetros multiasignación para regla asociados a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdRegla">Identificador de la Regla</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaReglasPMA(int IdCadenaComercial, int IdRegla, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerReglasPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@IdRegla", DbType.Int32, IdRegla);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta la totalidad de parámetros multiasignación para la entidad Tipo de Cuenta, asociados o no a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdTipoCuenta">Identificador del Tipo de Cuenta</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTotalTiposCuentaPMA(int IdCadenaComercial, int IdTipoCuenta, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTotalTiposCuentaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadenaComercial);
                database.AddInParameter(command, "@IdTipoCuenta", DbType.Int64, IdTipoCuenta);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta los parámetros multiasignación para tipo de cuenta asociados a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdTipoCuenta">Identificador del Tipo de Cuenta</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTiposCuentaPMA(int IdCadenaComercial, int IdTipoCuenta, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTiposCuentaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadenaComercial);
                database.AddInParameter(command, "@IdTipoCuenta", DbType.Int64, IdTipoCuenta);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta la totalidad de parámetros multiasignación para la entidad Grupo de Cuenta, asociados o no a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdGrupoCuenta">Identificador del Grupo de Cuenta</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTotalGrupoCuentaPMA(int IdCadenaComercial, int IdGrupoCuenta, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTotalGrupoCuentaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@IdGpoCta", DbType.Int32, IdGrupoCuenta);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta los parámetros multiasignación para grupo de cuenta asociados a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdGrupoCuenta">Identificador del Grupo de Cuenta</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaGrupoCuentaPMA(int IdCadenaComercial, int IdGrupoCuenta, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGrupoCuentaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@IdGpoCta", DbType.Int32, IdGrupoCuenta);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta la totalidad de los parámetros multiasignación para grupo de tarjeta asociados o no a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso u Origen</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTotalGrupoTarjetaPMA(Int16 IdCadenaComercial, Int16 IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTotalGrupoTarjetaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
                database.AddInParameter(command, "@ID_Origen", DbType.Int16, IdGrupoMA);
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
        /// Consulta los parámetros multiasignación para grupo de tarjeta asociados a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso u Origen</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaGrupoTarjetaPMA(Int16 IdCadenaComercial, Int16 IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGrupoTarjetaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
                database.AddInParameter(command, "@ID_Origen", DbType.Int16, IdGrupoMA);
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
        /// Consulta la totalidad de los parámetros multiasignación para tarjeta/cuenta asociados o no a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdCuenta">Identificador de la Cuenta</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTotalTarjetaCuentaPMA(int IdCadenaComercial, int IdCuenta, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTotalTarjetaCuentaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
                database.AddInParameter(command, "@IdCuenta", DbType.Int16, IdCuenta);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdGrupoMA);
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
        /// Consulta los parámetros multiasignación para tarjeta/cuenta asociados a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdCuenta">Identificador de la Cuenta</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTarjetaCuentaPMA(int IdCadenaComercial, int IdCuenta, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTarjetaCuentaPMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
                database.AddInParameter(command, "@ID_Origen", DbType.Int16, IdGrupoMA);
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
        /// Inserta o actualiza el valor de un parámetro multiasignación en base de datos
        /// </summary>
        /// <param name="nuevoPMA">Valores del tipo ParametroMA</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaOModificaPMA(ParametroMA nuevoPMA, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaActualizaPMA");

                database.AddInParameter(command, "@IdValorPMA", DbType.Int64, nuevoPMA.ID_ValorParametroMultiasignacion);
                database.AddInParameter(command, "@IdPMA", DbType.Int64, nuevoPMA.ID_ParametroMultiasignacion);
                database.AddInParameter(command, "@IdEntidad", DbType.Int64, nuevoPMA.ID_Entidad);
                database.AddInParameter(command, "@IdOrigen", DbType.Int64, nuevoPMA.ID_Origen);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, nuevoPMA.ID_Producto);
                database.AddInParameter(command, "@IdVigencia", DbType.Int64, nuevoPMA.ID_Vigencia);
                database.AddInParameter(command, "@IdCadenaComercial", DbType.Int64, nuevoPMA.ID_CadenaComercial);
                database.AddInParameter(command, "@Valor", DbType.String, nuevoPMA.ValorPMA);
                database.AddInParameter(command, "@UserTemp", DbType.String, elUsuario.ClaveUsuario);
                
                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el valor de un parámetro multiasignación en base de datos
        /// </summary>
        /// <param name="nuevoPMA">Valores del tipo ParametroMA</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaPMA(ParametroMA nuevoPMA, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizaPMA");

                database.AddInParameter(command, "@IdValorPMA", DbType.Int64, nuevoPMA.ID_ValorParametroMultiasignacion);
                database.AddInParameter(command, "@IdVigencia", DbType.Int64, nuevoPMA.ID_Vigencia);
                database.AddInParameter(command, "@Valor", DbType.String, nuevoPMA.ValorPMA);
                database.AddInParameter(command, "@UserTemp", DbType.String, elUsuario.ClaveUsuario);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros multiasignación del producto, y que 
        /// pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ListaParametrosMAPorTipo(int idTipoPMA, int idProducto, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMAProducto");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneParametrosMAProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdProducto=" + idProducto);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros del producto en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el valor del parámetro multiasignación del producto en el Autorizador
        /// </summary>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaValorParametro(ParametroValor elParametro, Usuario elUsuario, ILogHeader elLog, string pantalla = "")
        {
            LogPCI unLog = new LogPCI(elLog);
            
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaValorPMA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPMA", elParametro.ID_Parametro));
                        command.Parameters.Add(new SqlParameter("@IdPlantilla", elParametro.ID_Plantilla));
                        command.Parameters.Add(new SqlParameter("@IdValorPMA", elParametro.ID_ValordelParametro));
                        command.Parameters.Add(new SqlParameter("@NuevoValor", elParametro.Valor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@PantallaOrigen", pantalla));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaValorPMA";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPMA=" + elParametro.ID_Parametro);
                        parametros.Add("P2", "@IdPlantilla=" + elParametro.ID_Plantilla);
                        parametros.Add("P3", "@IdValorPMA=" + elParametro.ID_ValordelParametro);
                        parametros.Add("P4", "@NuevoValor=" + elParametro.Valor);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        parametros.Add("P6", "@PantallaOrigen=" + pantalla);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el valor del parámetro en base de datos");
            }
        }

        /// <summary>
        /// Elimina el valor de parámetro multiasignación en el Autorizador
        /// </summary>
        /// <param name="idValorPMA">Identificador del valor de parámetro multiasignación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="pantalla">Sufijo de nombre de Pantalla para identificar en el Log</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaValorParametro(int idValorPMA, Usuario elUsuario, ILogHeader elLog, string pantalla = "")
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaValorPMA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorPMA", idValorPMA));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@PantallaOrigen", pantalla));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_EliminaValorPMA";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorPMA=" + idValorPMA);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        parametros.Add("P3", "@PantallaOrigen=" + pantalla);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar el valor del parámetro en base de datos");
            }
        }

        /// <summary>
        /// Consulta la lista de parámetros necesarios para la cuenta CLABE del cliente
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista con los parámetros valor</returns>
        public static List<ParametroValor> ListaParametrosCLABE(long IdColectiva, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                List<ParametroValor> larespuesta = new List<ParametroValor>();

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosCLABE");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, IdColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneParametrosCLABE";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ParametroValor unParam = new ParametroValor();

                    unParam.ID_Parametro = Convert.ToInt32(dt.Rows[i]["ID_Parametro"]);
                    unParam.Nombre = dt.Rows[i]["Nombre"].ToString();
                    unParam.Descripcion = dt.Rows[i]["Descripcion"].ToString();
                    unParam.ID_ValordelParametro = Convert.ToInt32(dt.Rows[i]["ID_Valor"]);
                    unParam.Valor = dt.Rows[i]["Valor"].ToString();

                    larespuesta.Add(unParam);
                }

                return larespuesta;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros CLABE de base de datos");
            }
        }

        /// <summary>
        /// Obtiene del Autorizador el listado de parámetros multiasignación del producto, que pertenecen
        /// al tipo marcado, y cuyo tipo de plantilla es de tipo Preautorizador.
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ListaPMAsPreautProdPorTipo(int idTipoPMA, int idProducto, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMAPreAutProducto");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePMAPreAutProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdProducto=" + idProducto);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros del producto en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros multiasignación del producto, y que 
        /// pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ListaParametrosMAProdTemp(int idTipoPMA, int idProducto, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMAsProducto_temp");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
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
        /// Obtiene del Autorizador los parámetros multiasignación de una plantilla y que pertenecen
        /// al tipo marcado
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idPlantilla">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ObtienePMAsPorPlantilla(int idTipoPMA, long idPlantilla, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMAPlantilla");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdPlantilla", DbType.Int64, idPlantilla);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtienePMAPlantilla";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdPlantilla=" + idPlantilla);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros de base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros multiasignación OpenPay del producto
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="esAutorizador">Bandera para el rol Autorizador del usuario en sesión</param>
        /// <param name="esEjecutor">Bandera para el rol Ejecutor del usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ListaParametrosMAOpenPay(int idProducto, bool esAutorizador, bool esEjecutor, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMA_EA");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);
                database.AddInParameter(command, "@EsAutorizador", DbType.Boolean, esAutorizador);
                database.AddInParameter(command, "@EsEjecutor", DbType.Boolean, esEjecutor);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneParametrosMA_EA";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@EsAutorizador=" + esAutorizador);
                parametros.Add("P3", "@EsEjecutor=" + esEjecutor);
                parametros.Add("P4", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros en base de datos");
            }
        }

        /// <summary>
        /// Inserta o actualiza el valor de un parámetro multiasignación que requiere autorización en base de datos
        /// </summary>
        /// <param name="idValorPMA">Identificador del valor ParametroMA</param>
        /// <param name="valorPorAutorizar">Valor por autorizar del ParametroMA</param>
        /// <param name="paginaAspx">Usuario en sesión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaActualizaValorPMAPorAutorizar(long idValorPMA, string valorPorAutorizar, 
            string paginaAspx, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaCambioValorPMA_EA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorPMA", idValorPMA));
                        command.Parameters.Add(new SqlParameter("@NuevoValor", valorPorAutorizar));
                        command.Parameters.Add(new SqlParameter("@PaginaAspx", paginaAspx));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@AppID", AppID));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaCambioValorPMA_EA";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorPMA=" + idValorPMA);
                        parametros.Add("P2", "@NuevoValor=" + valorPorAutorizar);
                        parametros.Add("P3", "@PaginaAspx=" + paginaAspx);
                        parametros.Add("P4", "@UsuarioEjecutor=" + elUsuario.ClaveColectiva);
                        parametros.Add("P5", "@AppID=" + AppID);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el valor por autorizar en base de datos");
            }
        }

        /// <summary>
        /// Actualiza y autoriza el valor del parámetro multiasignación en base de datos
        /// </summary>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="esAutorYEjec">Bandera que indica que el usuario en sesión tiene los roles de Autorizador
        /// y Ejecutor</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaYAutorizaValorParametro(ParametroValor elParametro, bool esAutorYEjec,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaYAutorizaValorPMA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPMA", elParametro.ID_Parametro));
                        command.Parameters.Add(new SqlParameter("@IdPlantilla", elParametro.ID_Plantilla));
                        command.Parameters.Add(new SqlParameter("@IdValorPMA", elParametro.ID_ValordelParametro));
                        command.Parameters.Add(new SqlParameter("@NuevoValor", elParametro.ValorPorAutorizar));
                        command.Parameters.Add(new SqlParameter("@UsuarioAutorizador", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@EsAutEsEjec", esAutorYEjec));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaYAutorizaValorPMA";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPMA=" + elParametro.ID_Parametro);
                        parametros.Add("P2", "@IdPlantilla=" + elParametro.ID_Plantilla);
                        parametros.Add("P3", "@IdValorPMA=" + elParametro.ID_ValordelParametro);
                        parametros.Add("P4", "@NuevoValor=" + elParametro.ValorPorAutorizar);
                        parametros.Add("P5", "@UsuarioAutorizador=" + elUsuario.ClaveColectiva);
                        parametros.Add("P6", "@EsAutEsEjec=" + esAutorYEjec);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar y autorizar el valor del parámetro en base de datos");
            }
        }

        /// <summary>
        /// Elimina de la tabla de control de autorizaciones el ID del valor del parámetro multiasignación
        /// en base de datos
        /// </summary>
        /// <param name="idValorParametro">Identificador del valor del parámetro</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaValorParametroPorAutorizar(long idValorParametro, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RechazaValorPMA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@IdValorPMA", idValorParametro));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_RechazaValorPMA";
                        logDbg.C_Value = "";
                        logDbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorPMA=" + idValorParametro);
                        logDbg.Parameters = parametros;

                        unLog.Debug(logDbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar el valor por autorizar del parámetro en base de datos");
            }
        }
    }
}
