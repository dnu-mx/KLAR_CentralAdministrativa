using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace WebServices
{
    public class Parabilium
    {
        /// <summary>
        /// Establece la llamada al método Login del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Token de autenticación recibido en caso de éxito, mensaje de error en caso contrario</returns>
        public static string Login(Parametros.Headers headers, Parametros.LoginBody body, ILogHeader logHeader)
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
                    RespuestasJSON.Login resp = JsonConvert.DeserializeObject<RespuestasJSON.Login>(content);
                    if (resp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat("ERROR. Login NO exitoso. Código de Respuesta: ({0}). Descripción: {1}.",
                            resp.CodRespuesta, resp.DescRespuesta);
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
        /// Establece la llamada al método AltaPersonaMoral del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service al alta</returns>
        public static string AltaPersonaMoral(Parametros.Headers headers, Parametros.AltaPersonaMoralBody body,
            ILogHeader logHeader)
        {
            string _resp = null;
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _altaClient = new RestClient(headers.URL);
                _altaClient.ReadWriteTimeout = 5000;

                var _altaRequest = new RestRequest("AltaPersonaMoral", Method.POST);
                _altaRequest.ReadWriteTimeout = 5000;

                //Headers
                _altaRequest.AddHeader("Content-Type", "application/json");
                _altaRequest.AddHeader("Credenciales", headers.Credenciales);
                _altaRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _altaRequest.AddJsonBody(body);

                REST_Log.LogRequest(_altaClient, _altaRequest, logHeader);

                //Ejecuta la petición
                logPCI.Info("INICIA Petición WS_AltaPersonaMoral()");
                IRestResponse response = _altaClient.Execute(_altaRequest);
                logPCI.Info("TERMINA Petición WS_AltaPersonaMoral()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.AltaPersonaMoral altaResp = JsonConvert.DeserializeObject<RespuestasJSON.AltaPersonaMoral>(content);
                    if (altaResp.CodRespuesta != "0000")
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Alta de Persona Moral NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, altaResp.CodRespuesta, Environment.NewLine, altaResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                    else
                    {
                        _resp = "OK";
                    }
                }
                else
                {
                    _resp = "Error de comunicación con el Servicio Web. Intenta más tarde.";
                }
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al generar el alta de la Persona Moral en el Servicio Web");
            }

            return _resp;
        }

        /// <summary>
        /// Establece la llamada al método ActivarTarjeta del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service a la activación</returns>
        public static string ActivarTarjeta(Parametros.Headers headers, Parametros.ActivarTarjetaBody body,
            Parametros.ActivarTarjetaBody logBody, ILogHeader logHeader)
        {
            string _resp = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _activarClient = new RestClient(headers.URL);
                _activarClient.ReadWriteTimeout = 5000;

                var _activarRequest = new RestRequest("ActivarTarjeta", Method.POST);
                _activarRequest.ReadWriteTimeout = 5000;

                //Headers
                _activarRequest.AddHeader("Content-Type", "application/json");
                _activarRequest.AddHeader("Credenciales", headers.Credenciales);
                _activarRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _activarRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var logRequest = new RestRequest("ActivarTarjeta", Method.POST);

                //Headers
                logRequest.AddHeader("Content-Type", "application/json");
                logRequest.AddHeader("Credenciales", headers.Credenciales);
                logRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                logRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_activarClient, logRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_ActivarTarjeta()");
                IRestResponse response = _activarClient.Execute(_activarRequest);
                unLog.Info("TERMINA Petición WS_ActivarTarjeta()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.ActivarTarjeta activarResp = JsonConvert.DeserializeObject<RespuestasJSON.ActivarTarjeta>(content);

                    if (activarResp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Activación de tarjeta NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, activarResp.CodRespuesta, Environment.NewLine, activarResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                    else
                    {
                        _resp = "OK";
                    }

                }
                else
                {
                    _resp = "Error de comunicación con el sevicio web de activaciones (ActivarTarjeta). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al activar la tarjeta en el Servicio Web");
            }

            return _resp;
        }

        /// <summary>
        /// Establece la llamada al método ConsultartarjetacreditoV2 del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service con el resumen de saldos al día de hoy</returns>
        public static RespuestasJSON.ConsultarTarjetaCredito ObtenerMovimientosTarjeta(Parametros.Headers headers, Parametros.ConsultarTarjetaCreditoBody body,
            Parametros.ConsultarTarjetaCreditoBody logBody, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);
            RespuestasJSON.ConsultarTarjetaCredito consultaResp = new RespuestasJSON.ConsultarTarjetaCredito();
            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _consultarClient = new RestClient(headers.URL);
                _consultarClient.ReadWriteTimeout = 5000;

                var _consultarRequest = new RestRequest("ConsultartarjetacreditoV2", Method.POST);
                _consultarRequest.ReadWriteTimeout = 5000;

                //Headers
                _consultarRequest.AddHeader("Content-Type", "application/json");
                _consultarRequest.AddHeader("Credenciales", headers.Credenciales);
                _consultarRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _consultarRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var logRequest = new RestRequest("ConsultartarjetacreditoV2", Method.POST);

                //Headers
                logRequest.AddHeader("Content-Type", "application/json");
                logRequest.AddHeader("Credenciales", headers.Credenciales);
                logRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                logRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_consultarClient, logRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_ConsultartarjetacreditoV2()");
                IRestResponse response = _consultarClient.Execute(_consultarRequest);
                unLog.Info("TERMINA Petición WS_ConsultartarjetacreditoV2()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    consultaResp = JsonConvert.DeserializeObject<RespuestasJSON.ConsultarTarjetaCredito>(content);
                }
                else
                {
                    consultaResp.DescRespuesta = "Error de comunicación con el sevicio web (ConsultarTarjetaCredito). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al consultar la tarjeta en el Servicio Web");
            }

            return consultaResp;
        }

        /// <summary>
        /// Establece la llamada al método Login del Web Service (english)
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Token de autenticación recibido en caso de éxito, mensaje de error en caso contrario</returns>
        public static string LoginENG(Parametros.Headers headers, Parametros.LoginBodyENG body, ILogHeader logHeader)
        {
            string token = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                var _loginENGClient = new RestClient(headers.URL);
                _loginENGClient.ReadWriteTimeout = 5000;

                var _loginENGRequest = new RestRequest("Login", Method.POST);
                _loginENGRequest.ReadWriteTimeout = 5000;

                //Headers
                _loginENGRequest.AddHeader("Content-Type", "application/json");
                _loginENGRequest.AddHeader("Credentials", Cifrado.Base64Encode(body.user + ":" + body.password));

                //Body
                _loginENGRequest.AddJsonBody(body);

                REST_Log.LogRequest(_loginENGClient, _loginENGRequest, logHeader);

                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_Login()");
                IRestResponse response = _loginENGClient.Execute(_loginENGRequest);
                unLog.Info("TERMINA Petición WS_Login()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.Login resp = JsonConvert.DeserializeObject<RespuestasJSON.Login>(content);
                    if (resp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat("ERROR. Login NO exitoso. Código de Respuesta: ({0}). Descripción: {1}.",
                            resp.CodRespuesta, resp.DescRespuesta);
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
        /// Establece la llamada al método ActivarTarjeta del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service a la activación</returns>
        public static string BlackList(Parametros.Headers headers, Parametros.BlackListBody body,
            ILogHeader logHeader)
        {
            string respBL = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                //Se genera el POST
                var _bListClient = new RestClient(headers.URL);
                _bListClient.ReadWriteTimeout = 5000;

                var _bListRequest = new RestRequest("prospects/find", Method.POST);
                _bListRequest.ReadWriteTimeout = 5000;

                //Headers
                _bListRequest.AddHeader("Content-Type", "application/json");
                _bListRequest.AddHeader("Credentials", headers.Credenciales);
                _bListRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _bListRequest.AddJsonBody(body);

                REST_Log.LogRequest(_bListClient, _bListRequest, logHeader);

                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_Prospects/Find()");
                IRestResponse response = _bListClient.Execute(_bListRequest);
                unLog.Info("TERMINA Petición WS_Prospects/Find()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.BlackList items = JsonConvert.DeserializeObject<RespuestasJSON.BlackList>(content);

                    if (items == null)
                    {
                        throw new CAppException(8011, "La verificación de la Cuenta en Black List no devolvió datos.");
                    }
                    else
                    {
                        //Se procesan los prospectos y se obtiene el primero activo
                        RespuestasJSON.BlackListMatches[] prospectos = JsonConvert.DeserializeObject<RespuestasJSON.BlackListMatches[]>(items.matches.ToString());

                        if (prospectos == null)
                        {
                            throw new CAppException(8011, "La verificación de la Cuenta en Black List no devolvió prospectos.");
                        }
                        else
                        {
                            foreach (RespuestasJSON.BlackListMatches prospecto in prospectos)
                            {
                                if (prospecto.status.ToUpper().Contains("ACTIVO"))
                                {
                                    respBL = prospecto.porcentaje_name;
                                    break;
                                }
                            }
                        }
                    }

                    return respBL;
                }
                else
                {
                    throw new Exception("StatusCode: " + statusCode + ". Error de comunicación con el sevicio web de activaciones (BlackList). Intenta más tarde.");
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al verificar la Cuenta en Black List del Servicio Web.");
            }
        }

        /// <summary>
        /// Establece la llamada al método CancelarTarjeta del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service a la activación</returns>
        public static string EliminarTarjeta(Parametros.Headers headers, Parametros.EliminarTarjetaBody body,
            Parametros.EliminarTarjetaBody logBody, ILogHeader logHeader)
        {
            string _resp = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _eliminarClient = new RestClient(headers.URL);
                _eliminarClient.ReadWriteTimeout = 5000;

                var _eliminarRequest = new RestRequest("CancelarTarjeta", Method.POST);
                _eliminarRequest.ReadWriteTimeout = 5000;

                //Headers
                _eliminarRequest.AddHeader("Content-Type", "application/json");
                _eliminarRequest.AddHeader("Credenciales", headers.Credenciales);
                _eliminarRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _eliminarRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var logRequest = new RestRequest("CancelarTarjeta", Method.POST);

                //Headers
                logRequest.AddHeader("Content-Type", "application/json");
                logRequest.AddHeader("Credenciales", headers.Credenciales);
                logRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                logRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_eliminarClient, logRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_CancelarTarjeta()");
                IRestResponse response = _eliminarClient.Execute(_eliminarRequest);
                unLog.Info("TERMINA Petición WS_CancelarTarjeta()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.EliminarTarjeta eliminarResp = JsonConvert.DeserializeObject<RespuestasJSON.EliminarTarjeta>(content);

                    if (eliminarResp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Eliminación de tarjeta NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, eliminarResp.CodRespuesta, Environment.NewLine, eliminarResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                    else
                    {
                        _resp = "OK";
                    }

                }
                else
                {
                    _resp = "Error de comunicación con el sevicio web (CancelarTarjeta). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al eliminar la tarjeta en el Servicio Web");
            }

            return _resp;
        }

        /// <summary>
        /// Establece la llamada al método BajaTarjetahabiente del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service a la activación</returns>
        public static string CancelarCuentas(Parametros.Headers headers, Parametros.CancelarCuentasBody body,
            Parametros.CancelarCuentasBody logBody, ILogHeader logHeader)
        {
            string _resp = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _cancelarClient = new RestClient(headers.URL);
                _cancelarClient.ReadWriteTimeout = 5000;

                var _cancelarRequest = new RestRequest("BajaTarjetahabiente", Method.POST);
                _cancelarRequest.ReadWriteTimeout = 5000;

                //Headers
                _cancelarRequest.AddHeader("Content-Type", "application/json");
                _cancelarRequest.AddHeader("Credenciales", headers.Credenciales);
                _cancelarRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _cancelarRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var logRequest = new RestRequest("BajaTarjetahabiente", Method.POST);

                //Headers
                logRequest.AddHeader("Content-Type", "application/json");
                logRequest.AddHeader("Credenciales", headers.Credenciales);
                logRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                logRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_cancelarClient, logRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_BajaTarjetahabiente()");
                IRestResponse response = _cancelarClient.Execute(_cancelarRequest);
                unLog.Info("TERMINA Petición WS_BajaTarjetahabiente()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.CancelarCuentas cancelarResp = JsonConvert.DeserializeObject<RespuestasJSON.CancelarCuentas>(content);

                    if (cancelarResp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Cancelación de cuentas NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, cancelarResp.CodRespuesta, Environment.NewLine, cancelarResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                    else
                    {
                        _resp = "OK";
                    }

                }
                else
                {
                    _resp = "Error de comunicación con el sevicio web (WS_BajaTarjetahabiente). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al cancelar las cuentas en el Servicio Web");
            }

            return _resp;
        }

        /// <summary>
        /// Establece la llamada al método BloquearTarjeta del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service al bloqueo</returns>
        public static string BloquearTarjeta(Parametros.Headers headers, Parametros.BloquearTarjetaBody body,
            Parametros.BloquearTarjetaBody logBody, ILogHeader logHeader)
        {
            string _resp = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _bloqClient = new RestClient(headers.URL);
                _bloqClient.ReadWriteTimeout = 5000;

                var _bloqRequest = new RestRequest("BloquearTarjeta", Method.POST);
                _bloqRequest.ReadWriteTimeout = 5000;

                //Headers
                _bloqRequest.AddHeader("Content-Type", "application/json");
                _bloqRequest.AddHeader("Credenciales", headers.Credenciales);
                _bloqRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _bloqRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var bloqRequest = new RestRequest("BloquearTarjeta", Method.POST);

                //Headers
                bloqRequest.AddHeader("Content-Type", "application/json");
                bloqRequest.AddHeader("Credenciales", headers.Credenciales);
                bloqRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                bloqRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_bloqClient, bloqRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_BloquearTarjeta()");
                IRestResponse response = _bloqClient.Execute(_bloqRequest);
                unLog.Info("TERMINA Petición WS_BloquearTarjeta()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.BloquearTarjeta bloquearResp = JsonConvert.DeserializeObject<RespuestasJSON.BloquearTarjeta>(content);

                    if (bloquearResp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Bloqueo de tarjeta NO exitoso.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, bloquearResp.CodRespuesta, Environment.NewLine, bloquearResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                    else
                    {
                        _resp = "OK";
                    }

                }
                else
                {
                    _resp = "Error de comunicación con el sevicio web de bloqueos (BloquearTarjeta). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al bloquear la tarjeta en el Servicio Web");
            }

            return _resp;
        }

        /// <summary>
        /// Establece la llamada al método VerOperacionMSI del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service al bloqueo</returns>
        public static List<OperacionDiferimiento> VerDetalleOperacionDiferida(Parametros.Headers headers, Parametros.DiferirOperacionBody body,
            Parametros.DiferirOperacionBody logBody, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);
            List<OperacionDiferimiento> lasParcialidades = new List<OperacionDiferimiento>();

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _verOpClient = new RestClient(headers.URL);
                _verOpClient.ReadWriteTimeout = 5000;

                var _verOpRequest = new RestRequest("VerOperacionDiferida", Method.POST);
                _verOpRequest.ReadWriteTimeout = 5000;

                //Headers
                _verOpRequest.AddHeader("Content-Type", "application/json");
                _verOpRequest.AddHeader("Credenciales", headers.Credenciales);
                _verOpRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _verOpRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var verOpRequest = new RestRequest("VerOperacionDiferida", Method.POST);

                //Headers
                verOpRequest.AddHeader("Content-Type", "application/json");
                verOpRequest.AddHeader("Credenciales", headers.Credenciales);
                verOpRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                verOpRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_verOpClient, verOpRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_VerOperacionDiferida()");
                IRestResponse response = _verOpClient.Execute(_verOpRequest);
                unLog.Info("TERMINA Petición WS_VerOperacionDiferida()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.VerOperacionDif operacionResp = JsonConvert.DeserializeObject<RespuestasJSON.VerOperacionDif>(content);

                    if (operacionResp.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR en la consulta.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, operacionResp.CodRespuesta, Environment.NewLine, operacionResp.DescRespuesta);
                        throw new CAppException(8011, sbResponse.ToString());
                    }
                    else
                    {
                        List<RespuestasJSON.ParcialidadesOperacionDif> parcialidades = JsonConvert.DeserializeObject<List<RespuestasJSON.ParcialidadesOperacionDif>>(operacionResp.Parcialidades.ToString());

                        foreach (RespuestasJSON.ParcialidadesOperacionDif parcialidad in parcialidades)
                        {
                            OperacionDiferimiento unDetalle = new OperacionDiferimiento();
                            unDetalle.No = parcialidad.No;
                            unDetalle.Fecha = parcialidad.Fecha;
                            unDetalle.Intereses = parcialidad.Intereses;
                            unDetalle.IvaIntereses = parcialidad.IvaIntereses;
                            unDetalle.Capital = parcialidad.Capital;
                            unDetalle.Total = parcialidad.Total;

                            lasParcialidades.Add(unDetalle);
                        }
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el servicio web (VerOperacionDiferida). Intenta más tarde.");
                }
            }
            catch (CAppException caEx)
            {
                unLog.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al consultar el detalle de diferimiento de la operación en el Servicio Web");
            }

            return lasParcialidades;
        }

        /// <summary>
        /// Establece la llamada al método DiferirOperacionMSI del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logBody">Parámetros del cuerpo de la petición (sólo para el registro en log)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service al bloqueo</returns>
        public static string DiferirOperacion(Parametros.Headers headers, Parametros.DiferirOperacionBody body,
            Parametros.DiferirOperacionBody logBody, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);
            string respDif;

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _diferirOpClient = new RestClient(headers.URL);
                _diferirOpClient.ReadWriteTimeout = 5000;

                var _diferirOpRequest = new RestRequest("DiferirOperacion", Method.POST);
                _diferirOpRequest.ReadWriteTimeout = 5000;

                //Headers
                _diferirOpRequest.AddHeader("Content-Type", "application/json");
                _diferirOpRequest.AddHeader("Credenciales", headers.Credenciales);
                _diferirOpRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _diferirOpRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var diferirOpRequest = new RestRequest("DiferirOperacion", Method.POST);

                //Headers
                diferirOpRequest.AddHeader("Content-Type", "application/json");
                diferirOpRequest.AddHeader("Credenciales", headers.Credenciales);
                diferirOpRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                diferirOpRequest.AddJsonBody(logBody);

                REST_Log.LogRequest(_diferirOpClient, diferirOpRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_DiferirOperacion");
                IRestResponse response = _diferirOpClient.Execute(_diferirOpRequest);
                unLog.Info("TERMINA Petición WS_DiferirOperacion()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.DifiereOperacion laRespuesta = JsonConvert.DeserializeObject<RespuestasJSON.DifiereOperacion> (content);

                    if (laRespuesta.CodRespuesta != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Diferimiento de operación NO exitoso.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, laRespuesta.CodRespuesta, Environment.NewLine, laRespuesta.DescRespuesta);
                        respDif = sbResponse.ToString();
                    }
                    else
                    {
                        respDif = "OK";
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el servicio web (DiferirOperacion). Intenta más tarde.");
                }
            }
            catch (CAppException caEx)
            {
                unLog.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al diferir la operación en el Servicio Web");
            }

            return respDif;
        }
    }
}
