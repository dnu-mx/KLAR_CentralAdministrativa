using DALAutorizador.BaseDatos;
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
using System.Linq;

namespace DALAdministracion.BaseDatos
{
    public class DAOComercioTarjeta
    {
        public static DataSet ListaClientesCacao(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClientesCacao");

                 ds = database.ExecuteDataSet(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneClientesCacao";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                logPCI.Debug(logDbg);
                ///<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los clientres en base de datos");
            }
            return  ds;
        }

        public static void GuardarComercioTarjeta(string numeroAfiliacion, string numeroTarjeta, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaComercioTarjeta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@NumeroAfiliacion", numeroAfiliacion));
                        SqlCommand cmd = new SqlCommand();
                        SqlParameter param = cmd.CreateParameter();
                        param.ParameterName = "@NumeroTarjeta";
                        param.DbType = DbType.AnsiStringFixedLength;
                        param.Direction = ParameterDirection.Input;
                        param.Value = numeroTarjeta;
                        param.Size = numeroTarjeta.Length;
                        command.Parameters.Add(param);
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logdbg = new LogDebugMsg();
                        logdbg.M_Value = "web_CA_InsertaComercioTarjeta";
                        logdbg.C_Value = "";
                        logdbg.R_Value = "***************************";

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@NumeroAfiliacion=" + numeroAfiliacion);
                        parametros.Add("P2", "@NumeroTarjeta=" + MaskSensitiveData.cardNumber(numeroTarjeta));
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logdbg.Parameters = parametros;

                        pCI.Debug(logdbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                pCI.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al guardar comerccios tarjeta en base de datos");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <param name="afiliacion"></param>
        /// <param name="tarjeta"></param>
        /// <param name="elUsuario"></param>
        /// <param name="AppID"></param>
        /// <param name="elLog"></param>
        /// <returns></returns>
        public static DataSet ObtieneComercioTarjeta(long IdCliente, string afiliacion, string tarjeta,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneComerciotarjeta");

                database.AddInParameter(command, "@IdCliente", DbType.Int64, IdCliente);
                database.AddInParameter(command, "@Afiliacion", DbType.String, afiliacion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@Tarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(tarjeta) ? null : tarjeta;
                param.Size = tarjeta.Length;
                command.Parameters.Add(param);

                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneComerciotarjeta";
                logDbg.C_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdCliente=" + IdCliente);
                parametros.Add("P2", "@Afiliacion=" + afiliacion);
                parametros.Add("P3", "@Tarjeta=" + (string.IsNullOrEmpty(tarjeta) ? tarjeta :
                    MaskSensitiveData.cardNumber(tarjeta)));
                parametros.Add("P4", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P5", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los comercios-tarjetas en base de datos");
            }
            return ds;
        }

        public static void EliminaComercioTarjeta(string numeroAfiliacion, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaComerciotarjeta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Afiliacion", numeroAfiliacion));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logdbg = new LogDebugMsg();
                        logdbg.M_Value = "web_CA_EliminaComerciotarjeta";
                        logdbg.C_Value = "";
                        logdbg.R_Value = "***************************";

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Afiliacion=" + numeroAfiliacion);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logdbg.Parameters = parametros;

                        pCI.Debug(logdbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                pCI.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar comercios tarjeta en base de datos");
            }
        }

        public static DataSet ListaTarjeta(string numeroTarjeta, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjeta");

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@Tarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = numeroTarjeta;
                param.Size = numeroTarjeta.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                ds = database.ExecuteDataSet(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjeta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";


                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@Tarjeta=" + (string.IsNullOrEmpty(numeroTarjeta) ? numeroTarjeta :
                    MaskSensitiveData.cardNumber(numeroTarjeta)));
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al validar el Número de Tarjeta en base de datos");
            }

            return ds;
        }

        public static DataSet ListaComercioTarjeta( string numeroAfiliacion, string numeroTarjeta, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneListaComercioTarjeta");

                database.AddInParameter(command, "@Afiliacion", DbType.String, numeroAfiliacion);
                
                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@Tarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = numeroTarjeta;
                param.Size = numeroTarjeta.Length;
                command.Parameters.Add(param);

                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneListaComercioTarjeta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@Afiliacion=" + numeroAfiliacion);
                parametros.Add("P2", "@Tarjeta=" + (string.IsNullOrEmpty(numeroTarjeta) ? numeroTarjeta :
                    MaskSensitiveData.cardNumber(numeroTarjeta)));
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los clientres en base de datos");
            }
            return ds;
        }

        public static DataSet ObtienNombreORazonsocial(string numeroTarjeta, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNombreORazonSocial");

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@Tarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(numeroTarjeta) ? null : numeroTarjeta;
                param.Size = numeroTarjeta.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjeta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";


                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@Tarjeta=" + (string.IsNullOrEmpty(numeroTarjeta) ? numeroTarjeta :
                    MaskSensitiveData.cardNumber(numeroTarjeta)));
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los clientres en base de datos");
            }
            return ds;
        }
    }
}
