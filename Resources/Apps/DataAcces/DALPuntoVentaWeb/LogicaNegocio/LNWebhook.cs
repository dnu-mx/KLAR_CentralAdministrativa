using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    public class LNWebhook
    {
        /// <summary>
        /// Establece las condiciones de validación para la modificación del estatus de envío
        /// de un mensaje webhook en base de datos
        /// </summary>
        /// <param name="ID_Mensaje">Identificador del mensaje webhook por modificar</param>
        /// <param name="urlMensaje">URL donde se reenviará el mensaje webhook por modificar</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusMsjWebhook(int ID_Mensaje, string urlMensaje, Usuario usuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOWebhook.ActualizaEstatusMsjWebhook(ID_Mensaje, urlMensaje, usuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus del mensaje");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del estatus de envío
        /// de los mensaje webhook que cumplen los filtros en base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="Fecha">Fecha de la operación</param>
        /// <param name="IdEstatusEnvio">Bandera con el estatus de envío (1 = enviado, 0 = no enviado)</param>
        /// <param name="IdTipoOperacion">Identificador del tipo de operación (prioridad)</param>
        /// <param name="IdPoliza">Identificador de la póliza</param>
        /// <param name="Nombre">Nombre del tarjetahabiente</param>
        /// <param name="UrlMensaje">URL donde se reenviará el mensaje webhook por modificar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusMsjsWebhook(Int64 IdColectiva, DateTime Fecha, int IdEstatusEnvio,
            int IdTipoOperacion, Int64 IdPoliza, String Nombre, String UrlMensaje, Usuario usuario, ILogHeader logHeader)
        {
            try
            {
                DAOWebhook.ActualizaMsjsWebhook(IdColectiva, Fecha, IdEstatusEnvio, IdTipoOperacion,
                    IdPoliza, Nombre, UrlMensaje, usuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus de los mensajes");
            }
        }

        /// <summary>
        /// Realiza el reenvío de un mensaje Webhook Onboarding
        /// </summary>
        /// <param name="httpData">Datos del mensaje por enviar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ReenviaWebhookOnboarding(HttpWebhookOnB httpData, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);
            StringBuilder _sb = new StringBuilder();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpData.URL);
                WebHeaderCollection myWebHeaderCollection = request.Headers;

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 200000;

                Dictionary<string, string> headersSend = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(httpData.XApiKey))
                {
                    headersSend.Add("X-Api-Key", httpData.XApiKey);
                }
                if (!string.IsNullOrEmpty(httpData.XTrackId))
                {
                    headersSend.Add("X-Request-Track-Id", httpData.XTrackId);
                }
                if (!string.IsNullOrEmpty(httpData.AutBasic_Pwd) && !string.IsNullOrEmpty(httpData.AutBasic_User))
                {
                    string autBasic = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        httpData.AutBasic_User + ":" + httpData.AutBasic_Pwd));
                    httpData.AuthKey = autBasic;
                    headersSend.Add("Authorization", "Basic " + autBasic);
                }

                unLog.Trace(LogRequestWebhookOnB(httpData));


                byte[] bytes = Encoding.UTF8.GetBytes(httpData.JsonBody);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                unLog.Trace(LogResponseWebhookOnB(response));
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (WebException wEx)
            {
                unLog.ErrorException(wEx);

                HttpWebResponse errorResponse = wEx.Response as HttpWebResponse;
                unLog.Trace(LogResponseWebhookOnB(errorResponse));
                throw new CAppException(8011, "Falla al reenviar el mensaje del Webhook Onboarding");
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla en el reenvío del mensaje Webhook Onboarding");
            }
        }

        /// <summary>
        /// Arma la cadena con los datos de la petición de envío del mensaje Webhook Onboarding
        /// </summary>
        /// <param name="httpReq">Datos de la petición</param>
        /// <returns>Cadena por loguear</returns>
        protected static string LogRequestWebhookOnB(HttpWebhookOnB httpRequest)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("HTTP_WebhookOnboarding>>Request POST/{0}||Content-Type: application/json||" +
                "X-Api-Key: {1}||X-Request-Track-Id: {2}||Authorization: Basic {3}||{4}",
                httpRequest.URL, httpRequest.XApiKey, httpRequest.XTrackId, httpRequest.AuthKey, httpRequest.JsonBody);

            return sb.ToString();
        }

        /// <summary>
        /// Arma la cadena con los datos de la respuesta al envío del mensaje Webhook Onboarding
        /// </summary>
        /// <param name="httpResponse">Datos de la respuesta</param>
        /// <returns>Cadena por loguear</returns>
        protected static string LogResponseWebhookOnB(HttpWebResponse httpResponse)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder headers = new StringBuilder();
            string ResponseContent;

            using (StreamReader r = new StreamReader(httpResponse.GetResponseStream()))
            {
                ResponseContent = r.ReadToEnd();
            }

            foreach (string key in httpResponse.Headers.AllKeys)
            {
                headers.AppendFormat("{0}: {1}||", key, httpResponse.Headers[key]);
            }

            sb.AppendFormat("HTTP_WebhookOnboarding>>Response POST/||ResponseUri: {0}||StatusCode: {1}||{2}",
                httpResponse.ResponseUri.ToString(), httpResponse.StatusCode.ToString(), headers);

            return sb.ToString();
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del estatus de envío de un mensaje
        /// Webhook Onboarding en base de datos
        /// </summary>
        /// <param name="ID_Mensaje">Identificador del mensaje webhook por modificar</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusMsjWebhookOnB(int ID_Mensaje, Usuario usuario, ILogHeader logHeader)
        {
            try
            {
                DAOWebhook.ActualizaEstatusMsjWebhookOnB(ID_Mensaje, usuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx; 
            }

            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus de envío del mensaje");
            }
        }
    }
}
