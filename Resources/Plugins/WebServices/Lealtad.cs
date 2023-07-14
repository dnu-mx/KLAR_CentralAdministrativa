using System;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System.Text;
using WebServices.Entidades;
using WebServices.Utilerias;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using System.Configuration;
using static WebServices.Entidades.RespuestasJSON;

namespace WebServices
{
    public class Lealtad
    {
        /// <summary>
        /// Establece la llamada al método ValidarMembresiaSams del Web Service
        /// </summary>
        /// <param name="body">Parámetros del cuerpo de la petición</param>
        /// <returns></returns>
        public static ValidaMembresiaSams ValidarMembresiaSams(String URL, Parametros.BodyValidarMembresiaSams body, Usuario elUsuario)
        {
            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST
                var _validaMembresia = new RestClient(URL);
                var _Request = new RestRequest("ValidarMembresiaSams", Method.POST);

                //Headers
                _Request.AddHeader("Content-Type", "application/json");

                //Body
                _Request.AddJsonBody(body);

                //REST_Log.LogRequestValidaMembresia(_Request, elUsuario.ClaveUsuario);

                //Ejecuta la petición
                IRestResponse response = _validaMembresia.Execute(_Request);
                string statusCode = response.StatusCode.ToString();

                //REST_Log.LogResponse(response, elUsuario.ClaveUsuario);

                //Petición enviada y recibida OK
                if (statusCode == "OK")
                {
                    var content = response.Content;

                    //Procesa la respuesta
                    RespuestasJSON.ValidaMembresiaSams validaResp = JsonConvert.DeserializeObject<RespuestasJSON.ValidaMembresiaSams>(content);
                    return validaResp;
                }
                else
                {
                    return null; // _resp = "Error de comunicación con el sevicio web. Intenta más tarde.";
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }
    }
}
