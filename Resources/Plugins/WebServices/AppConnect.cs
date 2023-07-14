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
    public class AppConnect
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
                    RespuestasJSON.Login_AC resp = JsonConvert.DeserializeObject<RespuestasJSON.Login_AC>(content);
                    if (Convert.ToInt32(resp.CodRespuesta) != 0)
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
        /// Establece la llamada al método ConsultarUsuariosEmpresa del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service a la activación</returns>
        public static string ConsultaUsuarioEmpresa (Parametros.Headers headers, Parametros.ConsultarUsuariosEmpresaBody body,
            ILogHeader logHeader)
        {
            string _resp = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _consultarUE = new RestClient(headers.URL);
                _consultarUE.ReadWriteTimeout = 5000;

                var _consultarRequest = new RestRequest("ConsultarUsuariosEmpresa", Method.POST);
                _consultarRequest.ReadWriteTimeout = 5000;

                //Headers
                _consultarRequest.AddHeader("Content-Type", "application/json");
                _consultarRequest.AddHeader("Credenciales", headers.Credenciales);
                _consultarRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                _consultarRequest.AddJsonBody(body);


                /////>>>LOG TRACE
                var logRequest = new RestRequest("ConsultarUsuariosEmpresa", Method.POST);

                //Headers
                logRequest.AddHeader("Content-Type", "application/json");
                logRequest.AddHeader("Credenciales", headers.Credenciales);
                logRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                logRequest.AddJsonBody(body);

                REST_Log.LogRequest(_consultarUE, logRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_ConsultarUsuariosEmpresa()");
                IRestResponse response = _consultarUE.Execute(_consultarRequest);
                unLog.Info("TERMINA Petición WS_ConsultarUsuariosEmpresa()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.ConsultaUsuarioEmpresa consultaResp = JsonConvert.DeserializeObject<RespuestasJSON.ConsultaUsuarioEmpresa>(content);

                    if (Convert.ToInt32(consultaResp.CodRespuesta) != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Consulta de Usuario/Empresa NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, consultaResp.CodRespuesta, Environment.NewLine, consultaResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                    else
                    {
                        _resp = consultaResp.ClaveEmpresa;
                    }

                }
                else
                {
                    _resp = "Error de comunicación con el sevicio web (ConsultarUsuariosEmpresa). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al consultar la relación Usuario/Empresa en el Servicio Web");
            }

            return _resp;
        }

        /// <summary>
        /// Establece la llamada al método UsuariosEmpresa del Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del Web Service a la activación</returns>
        public static string UsuariosEmpresa(Parametros.Headers headers, Parametros.UsuariosEmpresaBody body,
            ILogHeader logHeader)
        {
            string _resp = "OK";
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var clienteUE = new RestClient(headers.URL);
                clienteUE.ReadWriteTimeout = 5000;

                var requestUE = new RestRequest("UsuariosEmpresa", Method.POST);
                requestUE.ReadWriteTimeout = 5000;

                //Headers
                requestUE.AddHeader("Content-Type", "application/json");
                requestUE.AddHeader("Credenciales", headers.Credenciales);
                requestUE.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                requestUE.AddJsonBody(body);


                /////>>>LOG TRACE
                var logRequest = new RestRequest("UsuariosEmpresa", Method.POST);

                //Headers
                logRequest.AddHeader("Content-Type", "application/json");
                logRequest.AddHeader("Credenciales", headers.Credenciales);
                logRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Body
                logRequest.AddJsonBody(body);

                REST_Log.LogRequest(clienteUE, logRequest, logHeader);
                /////<<<LOG TRACE


                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_UsuariosEmpresa()");
                IRestResponse response = clienteUE.Execute(requestUE);
                unLog.Info("TERMINA Petición WS_UsuariosEmpresa()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                //Procesa la respuesta
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.UsuariosEmpresa consultaResp = JsonConvert.DeserializeObject<RespuestasJSON.UsuariosEmpresa>(content);

                    if (Convert.ToInt32(consultaResp.CodRespuesta) != 0)
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat(
                            "ERROR. Actualización de Usuario/Empresa NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                            Environment.NewLine, consultaResp.CodRespuesta, Environment.NewLine, consultaResp.DescRespuesta);
                        _resp = sbResponse.ToString();
                    }
                }
                else
                {
                    _resp = "Error de comunicación con el sevicio web (UsuariosEmpresa). Intenta más tarde.";
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al actualizar el Usuario/Empresa en el Servicio Web");
            }

            return _resp;
        }
    }
}
