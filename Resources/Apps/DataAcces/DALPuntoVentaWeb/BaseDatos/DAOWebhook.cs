using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Entidades;
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

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOWebhook
    {
        /// <summary>
        /// Actualiza el estatus de procesamiento de un mensaje webhook en base de datos
        /// </summary>
        /// <param name="IdMensaje">Identificador del mensaje webhook por modificar</param>
        /// <param name="urlMsj">URL donde se reenviará el mensaje webhook por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusMsjWebhook(int IdMensaje, String urlMsj, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhook.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaMensajeProcesado", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMensaje", IdMensaje));
                        command.Parameters.Add(new SqlParameter("@URL", urlMsj));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaMensajeProcesado";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMensaje=" + IdMensaje);
                        parametros.Add("P2", "@URL=" + urlMsj);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el mensaje en base de datos");
            }
        }

        /// <summary>
        /// Actualiza en base de datos, de forma masiva, el estatus de procesamiento de 
        /// los mensaje webhooks que cumplen los filtros
        /// </summary>
        /// <param name="idColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="fecha">Fecha de la operación</param>
        /// <param name="idEstatusEnvio">Bandera con el estatus de envío (1 = enviado, 0 = no enviado)</param>
        /// <param name="idTipoOperacion">Identificador del tipo de operación (prioridad)</param>
        /// <param name="idPoliza">Identificador de la póliza</param>
        /// <param name="nombre">Nombre del tarjetahabiente</param>
        /// <param name="urlMsj">URL donde se reenviará el mensaje webhook por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaMsjsWebhook(Int64 idColectiva, DateTime fecha, int idEstatusEnvio,
            int idTipoOperacion, Int64 idPoliza, String nombre, String urlMsj, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhook.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaMensajesMasivo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdClienteCacao", idColectiva));
                        command.Parameters.Add(new SqlParameter("@Fecha", fecha));
                        command.Parameters.Add(new SqlParameter("@IdEstatusEnvio", idEstatusEnvio));
                        command.Parameters.Add(new SqlParameter("@IdPrioridad", idTipoOperacion));
                        command.Parameters.Add(new SqlParameter("@IdPoliza", idPoliza));
                        command.Parameters.Add(new SqlParameter("@Nombre", nombre));
                        command.Parameters.Add(new SqlParameter("@URL", urlMsj));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaMensajesMasivo";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdClienteCacao=" + idColectiva);
                        parametros.Add("P2", "@Fecha=" + fecha);
                        parametros.Add("P3", "@IdEstatusEnvio=" + idEstatusEnvio);
                        parametros.Add("P4", "@IdPrioridad=" + idTipoOperacion);
                        parametros.Add("P5", "@IdPoliza=" + idPoliza);
                        parametros.Add("P6", "@Nombre=" + nombre);
                        parametros.Add("P7", "@URL=" + urlMsj);
                        parametros.Add("P8", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        pCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar los mensajes en base de datos");
            }
        }

        /// <summary>
        /// Obtiene la lista de estatus de envío de los mensajes webhook onboarding
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ListaEstatusEnvioOnboarding(ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusEnvioWebhook");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneEstatusEnvioWebhook";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus de colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Consulta en base de datos los mensajes webhook onboarding que coinciden con los filtros
        /// </summary>
        /// <param name="claveColectiva">Clave del SubEmisor (colectiva)</param>
        /// <param name="fecha">Fecha de la operación</param>
        /// <param name="idEstatusEnvio">Identificador del estatus de envío</param>
        /// <param name="idMensaje">Identificador del mensaje</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneWebhookOnboarding(string claveColectiva, DateTime fecha, int idEstatusEnvio,
            string idMensaje, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog); 

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneWebhookOnBoarding");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@ClaveCliente", DbType.String, claveColectiva);
                database.AddInParameter(command, "@Fecha", DbType.DateTime, fecha);
                database.AddInParameter(command, "@IdEstatus", DbType.Int32, idEstatusEnvio);
                database.AddInParameter(command, "@IdMensaje", DbType.Int64, 
                    string.IsNullOrEmpty(idMensaje) ? null : idMensaje);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneWebhookOnBoarding";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ClaveCliente=" + claveColectiva);
                parametros.Add("P2", "@Fecha=" + fecha);
                parametros.Add("P3", "@IdEstatus=" + idEstatusEnvio);
                parametros.Add("P4", "@IdMensaje=" + idMensaje);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Webhook Onboarding de base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos los parámetros de contrato requeridos para enviar los 
        /// mensajes webhook onboarding
        /// </summary>
        /// <param name="idColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista Parametro con los datos obtenidos</returns>
        public static List<Parametro> ObtieneParametrosContratoWebhookOnboarding(long idColectiva, ILogHeader elLog)
        {
            List<Parametro> losParametros = new List<Parametro>(); 
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneParamsContratoWebhookOnB");
                database.AddInParameter(command, "@IdCliente", DbType.Int64, idColectiva);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_ObtieneParamsContratoWebhookOnB";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdCliente=" + idColectiva.ToString());
                logDBG.Parameters = parametros;

                log.Debug(logDBG);
                /////<<<LOG DEBUG

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Parametro parametro = new Parametro();
                    parametro.Nombre = dt.Rows[i]["Nombre"].ToString();
                    parametro.Valor = dt.Rows[i]["Valor"].ToString();
                    losParametros.Add(parametro);
                }

                return losParametros;
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los parámetros de contrato del cliente de base de datos.");
            }
        }


        /// <summary>
        /// Actualiza el estatus de envío de un mensaje Webhook Onboarding en base de datos
        /// </summary>
        /// <param name="IdMensaje">Identificador del mensaje webhook por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusMsjWebhookOnB(int IdMensaje, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaMensajeEnviado", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMensaje", IdMensaje));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaMensajeEnviado";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMensaje=" + IdMensaje);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus de envío del mensaje en base de datos");
            }
        }
    }
}
