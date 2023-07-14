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
    public class BovedaDigital
    {
        /// <summary>
        /// Establece la llamada al método Login al Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Token de autenticación recibido en caso de éxito, mensaje de error en caso contrario</returns>
        public static string Login(Parametros.Headers_BD headers, Parametros.LoginBody_BD body, ILogHeader logHeader)
        {
            string token = null;
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                var _lotsRequest = new RestClient(headers.URL);
                _lotsRequest.ReadWriteTimeout = 5000;

                var _loginBDRequest = new RestRequest("Login", Method.POST);
                _loginBDRequest.ReadWriteTimeout = 5000;

                //Headers
                _loginBDRequest.AddHeader("Content-Type", "application/json");
                _loginBDRequest.AddHeader("Credentials", Cifrado.Base64Encode(body.user_name + ":" + body.password));

                //Body
                _loginBDRequest.AddJsonBody(body);

                REST_Log.LogRequest(_lotsRequest, _loginBDRequest, logHeader);

                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_Login()");
                IRestResponse response = _lotsRequest.Execute(_loginBDRequest);
                unLog.Info("TERMINA Petición WS_Login()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.Login_BD resp = JsonConvert.DeserializeObject<RespuestasJSON.Login_BD>(content);
                    if (!string.IsNullOrEmpty(resp.code_response))
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat("ERROR. Login NO exitoso. Código de Respuesta: ({0}). Descripción: {1}.",
                            resp.code_response, resp.desc_response);
                        token = sbResponse.ToString();
                    }
                    else
                    {
                        //Se valida el token obtenido
                        token = String.IsNullOrEmpty(resp.token) ?
                            "ERROR. Autenticación NO exitosa. Token nulo." :
                            resp.token;
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
        /// Establece la llamada GET para los emisores en el Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista con los emisores consultados</returns>
        public static List<EmisorBovedaDigital> Emisores(Parametros.Headers_BD headers, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            List<EmisorBovedaDigital> losEmisores = new List<EmisorBovedaDigital>();

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera la petición
                var _issuersClient = new RestClient(headers.URL);
                _issuersClient.ReadWriteTimeout = 5000;

                var _issuersRequest = new RestRequest("issuers", Method.GET);
                _issuersRequest.ReadWriteTimeout = 5000;

                //Headers
                _issuersRequest.AddHeader("Content-Type", "application/json");
                _issuersRequest.AddHeader("Credentials", headers.Credentials);
                _issuersRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                REST_Log.LogRequest(_issuersClient, _issuersRequest, logHeader);

                //Ejecuta la petición
                log.Info("INICIA Petición WS_issuers()");
                IRestResponse response = _issuersClient.Execute(_issuersRequest);
                log.Info("TERMINA Petición WS_issuers()");

                REST_Log.LogResponse(response, logHeader);

                string statusCode = response.StatusCode.ToString();

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    List<RespuestasJSON.Issuers> issuers = JsonConvert.DeserializeObject<List<RespuestasJSON.Issuers>>(content);

                    foreach (RespuestasJSON.Issuers issuer in issuers)
                    {
                        if (!string.IsNullOrEmpty(issuer.code_response))
                        {
                            sbResponse = new StringBuilder();
                            sbResponse.AppendFormat(
                                "ERROR. Consulta de Emisores NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                                Environment.NewLine, issuer.code_response, Environment.NewLine, issuer.desc_response);
                            throw new CAppException(8011, sbResponse.ToString());
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(issuer.issuer_key))
                            {
                                EmisorBovedaDigital unEmisor = new EmisorBovedaDigital();
                                unEmisor.ClaveEmisor = issuer.issuer_key;
                                unEmisor.NombreEmisor = issuer.issuer_name;

                                losEmisores.Add(unEmisor);
                            }
                        }
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el Servicio Web - Emisores. Intenta más tarde.");
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
                throw new CAppException(8011, "Ocurrió un error al consultar los Emisores en el Servicio Web");
            }

            return losEmisores;
        }

        /// <summary>
        /// Establece la llamada GET para los productos (subgrupos de productos) en el Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista de productos obtenidos</returns>
        public static List<ProductoBovedaDigital> Productos(Parametros.Headers_BD headers, Parametros.Productos_BD body,
            ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            List<ProductoBovedaDigital> losProductos = new List<ProductoBovedaDigital>();

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera la petición
                var _prodsClient = new RestClient(headers.URL);
                _prodsClient.Timeout = 30000;

                var _prodsRequest = new RestRequest("sub_bins_groups", Method.GET);
                _prodsRequest.Timeout = 30000;

                //Headers
                _prodsRequest.AddHeader("Content-Type", "application/json");
                _prodsRequest.AddHeader("Credentials", headers.Credentials);
                _prodsRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                //Parameters
                _prodsRequest.AddParameter("issuer_key", body.issuer_key);
                _prodsRequest.AddParameter("sub_bins_group_key", body.sub_bins_group_key);

                REST_Log.LogRequest(_prodsClient, _prodsRequest, logHeader);

                //Ejecuta la petición
                log.Info("INICIA Petición WS_sub_bins_groups()");
                IRestResponse response = _prodsClient.Execute(_prodsRequest);
                log.Info("TERMINA Petición WS_sub_bins_groups()");

                REST_Log.LogResponse(response, logHeader);

                string statusCode = response.StatusCode.ToString();

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    List<RespuestasJSON.SubBinsGroup> productos = JsonConvert.DeserializeObject<List<RespuestasJSON.SubBinsGroup>>(content);

                    foreach (RespuestasJSON.SubBinsGroup producto in productos)
                    {
                        if (!string.IsNullOrEmpty(producto.code_response))
                        {
                            sbResponse = new StringBuilder();
                            sbResponse.AppendFormat(
                                "ERROR. Consulta de Productos NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                                Environment.NewLine, producto.code_response, Environment.NewLine, producto.desc_response);
                            throw new CAppException(8011, sbResponse.ToString());
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(producto.sub_bins_group_key))
                            {
                                ProductoBovedaDigital unProducto = new ProductoBovedaDigital();
                                unProducto.ClaveProducto = producto.sub_bins_group_key;
                                unProducto.NombreProducto = producto.sub_bins_group_name;
                                unProducto.ClaveEmisor = producto.issuer_key;

                                RespuestasJSON.Stock stock = JsonConvert.DeserializeObject<RespuestasJSON.Stock>(producto.stock.ToString());
                                unProducto.StockDisponible = stock.available;
                                unProducto.ReservadasFisicas = stock.reserved_physical;
                                unProducto.ReservadasVirtuales = stock.reserved_virtual;
                                unProducto.EmitidasFisicas = stock.issued_physical;
                                unProducto.EmitidasVirtuales = stock.issued_virtual;

                                losProductos.Add(unProducto);
                            }
                        }
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el Servicio Web - Productos. Intenta más tarde.");
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
                throw new CAppException(8011, "Ocurrió un error al consultar los Productos en el Servicio Web");
            }

            return losProductos;
        }

        /// <summary>
        /// Establece la llamada GET para los tipos de manufactura en el Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista de tipos de manufactura obtenidos</returns>
        public static List<TipoManufacturaBovedaDigital> TiposManufactura(Parametros.Headers_BD headers, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            List<TipoManufacturaBovedaDigital> losTipos = new List<TipoManufacturaBovedaDigital>();

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera la petición
                var _typesClient = new RestClient(headers.URL);
                _typesClient.ReadWriteTimeout = 5000;

                var _typesRequest = new RestRequest("manufacturing_types", Method.GET);
                _typesRequest.ReadWriteTimeout = 5000;

                //Headers
                _typesRequest.AddHeader("Content-Type", "application/json");
                _typesRequest.AddHeader("Credentials", headers.Credentials);
                _typesRequest.AddHeader("Authorization", "Bearer " + headers.Token);

                REST_Log.LogRequest(_typesClient, _typesRequest, logHeader);

                //Ejecuta la petición
                log.Info("INICIA Petición WS_manufacturing_types()");
                IRestResponse response = _typesClient.Execute(_typesRequest);
                log.Info("TERMINA Petición WS_manufacturing_types()");

                REST_Log.LogResponse(response, logHeader);

                string statusCode = response.StatusCode.ToString();

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    List<RespuestasJSON.ManufacturingTypes> tipos = JsonConvert.DeserializeObject<List<RespuestasJSON.ManufacturingTypes>>(content);

                    foreach (RespuestasJSON.ManufacturingTypes tipo in tipos)
                    {
                        if (!string.IsNullOrEmpty(tipo.code_response))
                        {
                            sbResponse = new StringBuilder();
                            sbResponse.AppendFormat(
                                "ERROR. Consulta de Tipos de Manufactura NO exitosa.{0}Código de Respuesta: ({1}).{2}Descripción: {3}.",
                                Environment.NewLine, tipo.code_response, Environment.NewLine, tipo.desc_response);
                            throw new CAppException(8011, sbResponse.ToString());
                        }
                        else
                        {
                            if (tipo.manufacturing_Type_Key != null)
                            {
                                TipoManufacturaBovedaDigital unTipo = new TipoManufacturaBovedaDigital();
                                unTipo.ClaveTipoManufactura = tipo.manufacturing_Type_Key;
                                unTipo.NombreTipoManufactura = tipo.manufacturing_Type_Name;
                                losTipos.Add(unTipo);
                            }
                        }
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el Servicio Web - Tipos de Manufactura. Intenta más tarde.");
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
                throw new CAppException(8011, "Ocurrió un error al consultar los Tipos de Manufactura en el Servicio Web");
            }

            return losTipos;
        }

        /// <summary>
        /// Establece la llamada al método Lots al Web Service
        /// </summary>
        /// <param name="headers">Parámetros del encabezado de la petición</param>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <param name="numLote">Número de lote generado</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista con el lote de tarjetas solicitado</returns>
        public static RespuestasJSON.LotValues[] CrearLote(Parametros.Headers_BD headers, Parametros.Lots_BD body,
            ref string numLote, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);
            RespuestasJSON.LotValues[] losValores;

            try
            {
                StringBuilder sbResponse = new StringBuilder();

                var _lotsClient = new RestClient(headers.URL);
                _lotsClient.ReadWriteTimeout = 5000;

                var _lotsRequest = new RestRequest("lots", Method.POST);
                _lotsRequest.ReadWriteTimeout = 5000;

                //Headers
                _lotsRequest.AddHeader("Content-Type", "application/json");
                _lotsRequest.AddHeader("Credentials", headers.Credentials);
                _lotsRequest.AddHeader("Authorization", "Bearer " + headers.Token);
                
                //Body
                _lotsRequest.AddJsonBody(body);

                REST_Log.LogRequest(_lotsClient, _lotsRequest, logHeader);

                //Ejecuta la petición
                unLog.Info("INICIA Petición WS_Lots()");
                IRestResponse response = _lotsClient.Execute(_lotsRequest);
                unLog.Info("TERMINA Petición WS_Lots()");

                string statusCode = response.StatusCode.ToString();

                REST_Log.LogResponse(response, logHeader);

                if (statusCode == "OK" || statusCode == "Created" || statusCode == "Accepted")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.Lots elLote = JsonConvert.DeserializeObject<RespuestasJSON.Lots>(content);
                    if (!string.IsNullOrEmpty(elLote.code_response))
                    {
                        sbResponse = new StringBuilder();
                        sbResponse.AppendFormat("ERROR. Reserva de lote NO exitoso. Código de Respuesta: ({0}). Descripción: {1}.",
                            elLote.code_response, elLote.desc_response);
                        throw new CAppException(8011, sbResponse.ToString());
                    }
                    else
                    {
                        //Se valida el no. de lote recibido
                        if (string.IsNullOrEmpty(elLote.lot_key))
                        {
                            throw new CAppException(8011, "ERROR. El número de lote recibido es nulo.");
                        }
                        else
                        {
                            numLote = elLote.lot_key;
                            //lote = string.IsNullOrEmpty(elLote.lot_key) ?
                            //"ERROR. El número de lote recibido es nulo." :
                            //elLote.lot_key;

                            losValores = JsonConvert.DeserializeObject<RespuestasJSON.LotValues[]>(elLote.values.ToString());

                            if (losValores == null)
                            {
                                throw new CAppException(8011, "ERROR. El lote de tarjetas recibido es nulo.");
                            }
                        }
                    }
                }
                else
                {
                    throw new CAppException(8011, "Error de comunicación con el Servicio Web - Reserva de Lotes. Intenta más tarde.");
                }

                return losValores;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al reservar el lote en el Servicio Web");
            }
        }
    }
}
