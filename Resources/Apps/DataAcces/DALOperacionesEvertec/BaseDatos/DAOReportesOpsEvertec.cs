using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALOperacionesEvertec.BaseDatos
{
    public class DAOReportesOpsEvertec
    {
        /// <summary>
        /// Consulta el catálogo de SubEmisores en base de datos, permitidos para
        /// el usuario y aplicación firmados
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros del catálogo</returns>
        public static DataTable ListaSubEmisores(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDOperacionesEvertec.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneClientesCacao");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_ObtieneClientesCacao";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los SubEmisores en base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de estatus de autorización
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros del catálogo</returns>
        public static DataSet ListaEstatusAutorizacion(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDOperacionesEvertec.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneEstatusAutorizacion");

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_ObtieneEstatusAutorizacion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus de autorización en base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de estatus post operación en base de datos
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros del catálogo</returns>
        public static DataSet ListaEstatusPostOperacion(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDOperacionesEvertec.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneEstatusPostOperacion");

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_ObtieneEstatusPostOperacion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus post operación en base de datos");
            }
        }

        /// <summary>
        /// Actualiza las fechas e IDs de fichero de los archivos MB en la tabla de control de base de datos
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaIDsFechas_MB(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDT112.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EstableceIDsFechasMB_ProcT112ApiCacao", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_EstableceIDsFechasMB_ProcT112ApiCacao";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar IDs y fechas de ficheros MB en base de datos");
            }
        }

        /// <summary>
        /// Actualiza las fechas e IDs de fichero de los archivos MI en la tabla de control de base de datos
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaIDsFechas_MI(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDT112.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EstableceIDsFechasMI_ProcT112ApiCacao", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_EstableceIDsFechasMI_ProcT112ApiCacao";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar IDs y fechas de ficheros MI en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el reporte de promociones con los filtros indicados
        /// </summary>
        /// <param name="fIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fFin">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteCompensaciones(DateTime fIni, DateTime fFin, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteCompT112ApiCacao");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fFin);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ReporteCompT112ApiCacao";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + fIni);
                parametros.Add("P2", "@FechaFinal=" + fFin);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte en base de datos");
            }
        }

        /// <summary>
        /// Consulta el reporte de operaciones en la base de datos Operaciones Evertec,
        /// según los filtros que se indiquen
        /// </summary>
        /// <param name="IdClienteCacao">Identificador del SubEmisor</param>
        /// <param name="fIniOp">Fecha inicial de la operación</param>
        /// <param name="fFinOp">Fecha final de la operación</param>
        /// <param name="fIniPres">Fecha inicial de presentación</param>
        /// <param name="fFinPres">Fecha final de presentación</param>
        /// <param name="fIniProc">Fecha inicial de procesamiento</param>
        /// <param name="fFinProc">Fecha final de procesamiento</param>
        /// <param name="NumTarjeta">Número de tarjeta</param>
        /// <param name="idEstatusOp">Identificador del estatus de la operación</param>
        /// <param name="idEstatusComp">Identificador del estatus de compensación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneReporteOperacionesApiCacao(int IdClienteCacao, DateTime fIniOp, DateTime fFinOp, 
            DateTime fIniPres, DateTime fFinPres, DateTime fIniProc, DateTime fFinProc, String NumTarjeta,
            int idEstatusOp, int idEstatusComp, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDOperacionesEvertec.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_ObtieneOperacionesApiCacao");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdClienteCacao", DbType.Int32, IdClienteCacao);

                database.AddInParameter(command, "@FechaInicialOp", DbType.Date,
                    fIniOp.Equals(DateTime.MinValue) ? (DateTime?)null : fIniOp);
                database.AddInParameter(command, "@FechaFinalOp", DbType.Date,
                    fFinOp.Equals(DateTime.MinValue) ? (DateTime?)null : fFinOp);

                database.AddInParameter(command, "@FechaInicialPres", DbType.Date,
                    fIniPres.Equals(DateTime.MinValue) ? (DateTime?)null : fIniPres);
                database.AddInParameter(command, "@FechaFinalPres", DbType.Date,
                    fFinPres.Equals(DateTime.MinValue) ? (DateTime?)null : fFinPres);

                database.AddInParameter(command, "@FechaInicialProc", DbType.Date,
                    fIniProc.Equals(DateTime.MinValue) ? (DateTime?)null : fIniProc);
                database.AddInParameter(command, "@FechaFinalProc", DbType.Date,
                    fFinProc.Equals(DateTime.MinValue) ? (DateTime?)null : fFinProc);

                database.AddInParameter(command, "@NumTarjeta", DbType.String, NumTarjeta);
                database.AddInParameter(command, "@IdEstatusOp", DbType.Int32, idEstatusOp);
                database.AddInParameter(command, "@ID_EstatusPostOperacion", DbType.Int32, idEstatusComp);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_Reporte_ObtieneOperacionesApiCacao";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdClienteCacao=" + IdClienteCacao);
                parametros.Add("P2", "@FechaInicialOp=" + fIniOp);
                parametros.Add("P3", "@FechaFinalOp=" + fFinOp);
                parametros.Add("P4", "@FechaInicialPres=" + fIniPres);
                parametros.Add("P5", "@FechaFinalPres=" + fFinPres);
                parametros.Add("P6", "@FechaInicialProc=" + fIniProc);
                parametros.Add("P7", "@FechaFinalProc=" + fFinProc);
                parametros.Add("P8", "@NumTarjeta=" + (string.IsNullOrEmpty(NumTarjeta) ? NumTarjeta :
                    NumTarjeta.Length < 16 ? "******" : MaskSensitiveData.cardNumber(NumTarjeta)));
                parametros.Add("P9", "@IdEstatusOp=" + idEstatusOp);
                parametros.Add("P10", "@ID_EstatusPostOperacion=" + idEstatusComp);
                parametros.Add("P11", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P12", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de base de datos");
            }
        }
    }
}
