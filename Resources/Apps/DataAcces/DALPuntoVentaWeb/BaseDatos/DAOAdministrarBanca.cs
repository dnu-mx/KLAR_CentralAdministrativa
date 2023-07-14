using DALAutorizador.BaseDatos;
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

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOAdministrarBanca
    {
        /// <summary>
        /// Obtiene la lista de divisas en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaDivisas(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDivisas");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDivisas";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las divisas de base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo tipo de cambio en base de datos
        /// </summary>
        /// <param name="fecha">Fecha</param>
        /// <param name="idDivisa">Identificador de la divisa o moneda</param>
        /// <param name="pesos">Valor del tipo de cambio en pesos</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static string InsertaTipoDeCambio(DateTime fecha, int idDivisa, float pesos,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaTipoCambioDivisa", conn))
                    {
                        string respuesta = "";

                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Fecha", fecha));
                        command.Parameters.Add(new SqlParameter("@IdDivisa", idDivisa));
                        command.Parameters.Add(new SqlParameter("@Pesos", pesos));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        command.Parameters.Add("@Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;

                        conn.Open();

                        command.ExecuteNonQuery();

                        if (!String.IsNullOrEmpty(command.Parameters["@Respuesta"].Value.ToString()))
                        {
                            respuesta = "Ya existe el tipo de cambio de la moneda con la fecha indicada";
                        }

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaTipoCambioDivisa";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@Respuesta=" + respuesta;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Fecha=" + fecha);
                        parametros.Add("P2", "@IdDivisa=" + idDivisa);
                        parametros.Add("P3", "@Pesos=" + pesos);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el tipo de cambio en base de datos");
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de cambio de la divisa indicada en el Autorizador
        /// </summary>
        /// <param name="idDivisa">Identificados de la divisa</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaTiposCambioDivisa(int idDivisa, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ConsultaTiposCambioDivisa");

                database.AddInParameter(command, "@IdDivisa", DbType.Int32, idDivisa);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ConsultaTiposCambioDivisa";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdDivisa=" + idDivisa);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de cambio de la divisa en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el valor del tipo de cambio con el ID indicado en base de datos
        /// </summary>
        /// <param name="idTC">Identificador del tipo de cambio</param>
        /// <param name="pesos">Valor del tipo de cambio en pesos</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaTipoDeCambio(int idTC, float pesos, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaTipoCambioDivisa", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTipoCambioDivisa", idTC));
                        command.Parameters.Add(new SqlParameter("@Pesos", pesos));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaTipoCambioDivisa";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTipoCambioDivisa=" + idTC);
                        parametros.Add("P2", "@Pesos=" + pesos);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el tipo de cambio en base de datos");
            }
        }

        /// <summary>
        /// Elimina el tipo de cambio con el ID indicado en base de datos
        /// </summary>
        /// <param name="idTC">Identificador del tipo de cambio</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaTipoDeCambio(int idTC, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaTipoCambioDivisa", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTipoCambioDivisa", idTC));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_EliminaTipoCambioDivisa";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTipoCambioDivisa=" + idTC);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar el tipo de cambio en base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos la lista de días inhábiles comprendidos en el periodo marcado,
        /// y para el país indicado
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="idPais">Identificador del país</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ListaDiasInhabiles(DateTime fechaInicio, DateTime fechaFin, int idPais,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDiasNoBancarios");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdPais", DbType.Int32, idPais);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDiasNoBancarios";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + fechaInicio);
                parametros.Add("P2", "@FechaFinal=" + fechaFin);
                parametros.Add("P3", "@IdPais=" + idPais);
                parametros.Add("P4", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P5", "@AppId=" + AppID);

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los días no bancarios de base de datos");
            }
        }

        /// <summary>
        /// Inserta una nueva fecha como día inhábil bancario, para el país con el ID indicado,
        /// en base de datos
        /// </summary>
        /// <param name="fecha">Fecha del nuevo día inhábil</param>
        /// <param name="idPais">Identificador del país</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaCorteDiaInhabil(DateTime fecha, int idPais, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaDiaNoBancario", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Fecha", fecha));
                        command.Parameters.Add(new SqlParameter("@IdPais", idPais));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaDiaNoBancario";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Fecha=" + fecha);
                        parametros.Add("P2", "@IdPais=" + idPais);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                log.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el día no bancario en base de datos");
            }
        }

        /// <summary>
        /// Elimina la fecha como día inhábil bancario, para el país con el ID indicado,
        /// en base de datos
        /// </summary>
        /// <param name="fecha">Fecha del día inhábil</param>
        /// <param name="idPais">Identificador del país</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaCorteDiaInhabil(DateTime fecha, int idPais, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaDiaNoBancario", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Fecha", fecha));
                        command.Parameters.Add(new SqlParameter("@IdPais", idPais));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_EliminaDiaNoBancario";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Fecha=" + fecha);
                        parametros.Add("P2", "@IdPais=" + idPais);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                log.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar el día no bancario de base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos la lista de días inhábiles comprendidos en el periodo marcado,
        /// separada por día, mes y año, y para el país indicado
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="idPais">Identificador del país</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ListaCalendarioAnual(DateTime fechaInicio, DateTime fechaFin, int idPais,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneListaAnualDiasNoBancarios");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdPais", DbType.Int32, idPais);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneListaAnualDiasNoBancarios";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";
                    
                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + fechaInicio);
                parametros.Add("P2", "@FechaFinal=" + fechaFin);
                parametros.Add("P3", "@IdPais=" + idPais);
                parametros.Add("P4", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P5", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un erro al consultar la lista anual de días no bancarios " +
                    "de base de datos");
            }
        }
    }
}
