using Interfases;
using Log_PCI;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;

namespace WebServices.Utilerias
{
    public class REST_Log
    {
        /// <summary>
        /// Escribe en log la petición al web service, junto con URI, headers y parámetros
        /// </summary>
        /// <param name="_restClient">Cliente establecido</param>
        /// <param name="_restRequest">Petición elaborada</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void LogRequest(RestClient _restClient, RestRequest _restRequest, ILogHeader elLog)
        {
            var requestToLog = new
            {
                uri = _restClient.BuildUri(_restRequest),
                parameters = _restRequest.Parameters.Select(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
            };

            LogPCI log = new LogPCI(elLog);
            log.Trace("WEBSERVICE>>LogRequest " + JsonConvert.SerializeObject(requestToLog));
        }

        /// <summary>
        /// Escribe en log la respuesta del web service, junto con URI, headers y parámetros
        /// </summary>
        /// <param name="response">Respuesta recibida</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void LogResponse(IRestResponse response, ILogHeader elLog)
        {
            var responseToLog = new
            {
                statusCode = response.StatusCode,
                content = response.Content,
                headers = response.Headers,
                responseUri = response.ResponseUri,
                errorMessage = response.ErrorMessage,
            };

            LogPCI log = new LogPCI(elLog);
            log.Trace("WEBSERVICE>>LogResponse " + JsonConvert.SerializeObject(responseToLog));
        }


        /// <summary>
        /// Escribe en log la petición al web service, junto con URI, headers y parámetros
        /// </summary>
        /// <param name="_restClient">Cliente establecido</param>
        /// <param name="_restRequest">Petición elaborada</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void LogRequestValidaMembresia(RestRequest _restRequest, string usuario)
        {
            var requestToLog = new
            {
                parameters = _restRequest.Parameters.Select(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
            };

            //Loguear.EntradaSalida(JsonConvert.SerializeObject(requestToLog), usuario, false);
        }
    }
}
