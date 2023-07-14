using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Entidades;
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
using System.Linq;

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOClabePendiente
    {
        public static DataSet ListaClientesCacao(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneListaClientesCacao");

                ds = database.ExecuteDataSet(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneListaClientesCacao";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    ds.Tables[0].Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));


                logPCI.Debug(logDbg);
                ///<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los clientres en base de datos");
            }
            return ds;
        }

        /// <summary>
        /// Inserta una solicitud de cuenta CLABE en la tabla de control Ejecutor/Autorizador del Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="tarjeta">Identificador de la tarjeta</param>
        /// <param name="Clabe">Identificador de la plantilla</param>
        /// <param name="paginaAspx">Monto solicitado (abono máximo personalizado)</param>
        /// <param name="elUsuario">Monto acumulado (saldo máximo personalizado)</param>
        /// <param name="AppID">Motivo por el que se realiza la solicitud</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static string InsertaSolicitudCambioCuentaCLABE(int idProducto, string tarjeta, string Clabe,
            string paginaAspx, bool esAut, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaSolicitudCuentaCLABE", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

                        SqlCommand _cmd = new SqlCommand();
                        SqlParameter param_T = _cmd.CreateParameter();
                        param_T.ParameterName = "@Tarjeta";
                        param_T.DbType = DbType.AnsiStringFixedLength;
                        param_T.Direction = ParameterDirection.Input;
                        param_T.Value = string.IsNullOrEmpty(tarjeta) ? null : tarjeta;
                        param_T.Size = tarjeta.Length;
                        command.Parameters.Add(param_T);

                        SqlParameter param_CC = _cmd.CreateParameter();
                        param_CC.ParameterName = "@Cuenta";
                        param_CC.DbType = DbType.AnsiStringFixedLength;
                        param_CC.Direction = ParameterDirection.Input;
                        param_CC.Value = string.IsNullOrEmpty(Clabe) ? null : Clabe;
                        param_CC.Size = Clabe.Length;
                        command.Parameters.Add(param_CC);

                        command.Parameters.Add(new SqlParameter("@NuevoValor", Clabe));
                        command.Parameters.Add(new SqlParameter("@CtaEnmasc", MaskSensitiveData.CLABE(Clabe)));
                        command.Parameters.Add(new SqlParameter("@PaginaAspx", paginaAspx));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@EsAutorizador", esAut));
                        command.Parameters.Add(new SqlParameter("@AppID", AppID));
                        var sqlParameterM = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameterM.Direction = ParameterDirection.Output;
                        sqlParameterM.Size = 200;
                        command.Parameters.Add(sqlParameterM);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string msjResp = sqlParameterM.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaSolicitudCuentaCLABE";
                        logDBG.C_Value = "";
                        logDBG.R_Value = msjResp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + idProducto);
                        parametros.Add("P2", "@Tarjeta=" + MaskSensitiveData.cardNumber(tarjeta));
                        parametros.Add("P3", "@NuevoValor=" + MaskSensitiveData.CLABE(Clabe));
                        parametros.Add("P4", "@CtaEnmasc=" + MaskSensitiveData.CLABE(Clabe));
                        parametros.Add("P5", "@PaginaAspx=" + paginaAspx);
                        parametros.Add("P6", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                        parametros.Add("P7", "@EsAutorizador=" + esAut);
                        parametros.Add("P8", "@AppID=" + AppID);

                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return msjResp;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la solicitud de cuenta CLABE en base de datos");
            }
        }

        /// <summary>
        /// Consulta los registros pendientes de autorización para las cuentas CLABE dentro del Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="tarjeta">Número de tarjeta asociado a la cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes</returns>
        public static DataTable ObtieneSolicitudesAutorizarCuentaCLABE(long idProducto, string tarjeta, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRegistrosPorAutorizarCuentaCLABE");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdProducto", DbType.Int64, idProducto);
                SqlCommand cmd = new SqlCommand();
                SqlParameter paramT = cmd.CreateParameter();
                paramT.ParameterName = "@Tarjeta";
                paramT.DbType = DbType.AnsiStringFixedLength;
                paramT.Direction = ParameterDirection.Input;
                paramT.Value = string.IsNullOrEmpty(tarjeta) ? null : tarjeta;
                paramT.Size = tarjeta.Length;
                command.Parameters.Add(paramT);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneRegistrosPorAutorizarCuentaCLABE";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";
                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@Tarjeta=" + (string.IsNullOrEmpty(tarjeta) ? tarjeta :
                    MaskSensitiveData.cardNumber(tarjeta)));
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las solicitudes de cuenta CLABE en base de datos");
            }
        }

        /// <summary>
        /// Autoriza la creación de la cuenta CLABE de una persona moral en base de datos, actualizando
        /// el registro en la tabla de control
        /// </summary>
        /// <param name="idTarjeta">Identificador de la tarjeta</param>
        /// <param name="idEjecAut">Identificador del registro en la tabla de control</param>
        /// <param name="clabe">Valor de la nueva cuenta CLABE</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaYAutorizaCuentaCLABE(int idTarjeta, int idEjecAut, string clabe,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_AutorizaCuentaCLABE", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMa", idTarjeta));
                        command.Parameters.Add(new SqlParameter("@IdEjecutorAutorizador", idEjecAut));

                        SqlCommand cmdCC = new SqlCommand();
                        SqlParameter paramCC = cmdCC.CreateParameter();
                        paramCC.ParameterName = "@CuentaCLABE";
                        paramCC.DbType = DbType.AnsiStringFixedLength;
                        paramCC.Direction = ParameterDirection.Input;
                        paramCC.Value = clabe;
                        paramCC.Size = clabe.Length;
                        command.Parameters.Add(paramCC);

                        command.Parameters.Add(new SqlParameter("@CtaEnmasc", MaskSensitiveData.CLABE(clabe)));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        
                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_AutorizaCuentaCLABE";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMa=" + idTarjeta);
                        parametros.Add("P2", "@IdEjecutorAutorizador=" + idEjecAut);
                        parametros.Add("P3", "@CuentaCLABE=" + MaskSensitiveData.CLABE(clabe));
                        parametros.Add("P4", "@CtaEnmasc=" + MaskSensitiveData.CLABE(clabe));
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                       
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al autorizar la solicitud de cuenta CLABE en base de datos");
            }
        }

        /// <summary>
        /// Rechaza la creación de la cuenta CLABE de una persona moral en base de datos, eliminando el registro
        /// de la tabla de control
        /// </summary>
        /// <param name="idEjecAut">Identificador del registro en la tabla de control</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RechazaCuentaCLABE(int idEjecAut, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RechazaCuentaCLABE", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdEjecutorAutorizador", idEjecAut));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_RechazaCuentaCLABE";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdEjecutorAutorizador=" + idEjecAut);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);

                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al rechazar la solicitud de cuenta CLABE en base de datos");
            }
        }
    }
}
