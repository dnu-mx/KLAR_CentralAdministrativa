using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace WebServices
{
    public class DNUUsuarios
    {
        /// <summary>
        /// Establece la llamada al método Login del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Token de autenticación recibido en caso de éxito, mensaje de error en caso contrario</returns>
        public static string Login(Parametros.Headers headers, Parametros.LoginDnuBody body, ILogHeader logHeader)
        {
            string token = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                var _loginClient = new RestClient(headers.URL);
                _loginClient.ReadWriteTimeout = 5000;

                var _loginRequest = new RestRequest("Login", Method.POST);
                _loginRequest.ReadWriteTimeout = 5000;

                //Headers
                _loginRequest.AddHeader("Content-Type", "application/json");
                _loginRequest.AddHeader("Credenciales", Cifrado.Base64Encode(body.NombreUsuario + ":" + body.Password));

                //Body
                _loginRequest.AddJsonBody(body);

                REST_Log.LogRequest(_loginClient, _loginRequest, logHeader);

                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_Login()");
                IRestResponse response = _loginClient.Execute(_loginRequest);
                unLog.Info("TERMINA Petición WS_Login()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.LoginDNU resp = JsonConvert.DeserializeObject<RespuestasJSON.LoginDNU>(content);
                    if (resp.CodigoRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat("ERROR. Login NO exitoso. Código de Respuesta: ({0}). Descripción: {1}.",
                            resp.CodigoRespuesta, resp.Mensaje);
                        token = sbResponse.ToString();
                    }
                    else
                    {
                        //Se valida el token obtenido
                        token = String.IsNullOrEmpty(resp.Token) ?
                            "ERROR. Autenticación NO exitosa. Token nulo." :
                            resp.Token;
                    }
                }
                else
                {
                    unLog.Error("StatusCode: " + statusCode);
                    token = "Error de comunicación con el servicio de autenticación. Intenta más tarde.";
                }
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al establecer el login con el Servicio Web");
            }

            return token;
        }

        /// <summary>
        /// Establece el método PUT de mensajes SMS
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void PutSMS(Parametros.Headers headers, Parametros.SmsBody body, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera la petición
                var _smsPutClient = new RestClient(headers.URL);
                _smsPutClient.ReadWriteTimeout = 5000;

                var _smsPutRequest = new RestRequest("SMS", Method.PUT);
                _smsPutRequest.ReadWriteTimeout = 5000;

                //Headers
                _smsPutRequest.AddHeader("Content-Type", "application/json");
                _smsPutRequest.AddHeader("Credenciales", headers.Credenciales);
                _smsPutRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _smsPutRequest.AddJsonBody(body);

                REST_Log.LogRequest(_smsPutClient, _smsPutRequest, logHeader);

                //Ejecuta la petición
                log.Info("INICIA Petición WS_SMS_PUT()");
                IRestResponse response = _smsPutClient.Execute(_smsPutRequest);
                log.Info("TERMINA Petición WS_SMS_PUT()");

                REST_Log.LogResponse(response, logHeader);

                string statusCode = response.StatusCode.ToString();

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.Sms smsResponse = JsonConvert.DeserializeObject<RespuestasJSON.Sms>(content);

                    if (smsResponse.CodigoRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Envío de mensaje SMS NO exitoso.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, smsResponse.CodigoRespuesta, Environment.NewLine, smsResponse.Mensaje);
                        throw new CAppException(8011, sbResponse.ToString());
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el Servicio Web SMS. Intenta más tarde.");
                }
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al enviar el mensaje SMS en el Servicio Web");
            }
        }

        /// <summary>
        /// Establece el método POST de mensajes SMS
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void PostSMS(Parametros.Headers headers, Parametros.SmsBody body, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera la petición
                var _smsPostClient = new RestClient(headers.URL);
                _smsPostClient.ReadWriteTimeout = 5000;

                var _smsPostRequest = new RestRequest("SMS", Method.POST);
                _smsPostRequest.ReadWriteTimeout = 5000;

                //Headers
                _smsPostRequest.AddHeader("Content-Type", "application/json");
                _smsPostRequest.AddHeader("Credenciales", headers.Credenciales);
                _smsPostRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _smsPostRequest.AddJsonBody(body);

                REST_Log.LogRequest(_smsPostClient, _smsPostRequest, logHeader);

                //Ejecuta la petición
                log.Info("INICIA Petición WS_SMS_POST()");
                IRestResponse response = _smsPostClient.Execute(_smsPostRequest);
                log.Info("TERMINA Petición WS_SMS_POST()");

                REST_Log.LogResponse(response, logHeader);

                string statusCode = response.StatusCode.ToString();

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.Sms smsResp = JsonConvert.DeserializeObject<RespuestasJSON.Sms>(content);

                    if (smsResp.CodigoRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Validación de token NO exitoso.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, smsResp.CodigoRespuesta, Environment.NewLine, smsResp.Mensaje);
                        throw new CAppException(8011, sbResponse.ToString());
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el Servicio Web SMS. Intenta más tarde.");
                }
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al validar el token del mensaje SMS en el Servicio Web");
            }
        }
    }
}
