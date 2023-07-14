using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace TpvWeb
{
    public partial class ActualizacionDocumentos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                }
            }

            catch (Exception err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de búsqueda, invocando la búsqueda
        /// de tiendas a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultado de búsqueda de clientes
        /// </summary>
        protected void LlenarGridResultados()
        {
            try
            {
                DataSet dsResultados = LNSrPago.ConsultaCatalogoTiendas(
                    txtBusq_ClaveAlmacen.Text, txtBusq_ClaveTienda.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 100)
                {
                    X.Msg.Alert("Consulta de Tiendas", "Demasiadas coincidencias, por favor afine su búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Tiendas", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                GridResultados.GetStore().DataSource = dsResultados;
                GridResultados.GetStore().DataBind();
            }

            catch (CAppException err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Tiendas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Tiendas", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
            }
        }

        /// <summary>
        /// Limpia los controles asociados a la selección previa de una tienda en
        /// el Grid de Resultados Tiendas
        /// </summary>
        protected void limpiaSeleccionPrevia()
        {
            FormPanelDocumentos.Reset();
        }

        /// <summary>
        /// Limpia los controles asociados a la búsqueda previa de tiendas dentro
        /// del Grid de Resultados Tiendas
        /// </summary>
        /// <param name="esBtnLimpiar">Indica si el método de origen fue el clic al botón Limpiar</param>
        protected void limpiaBusquedaPrevia(bool esBtnLimpiar)
        {
            if (esBtnLimpiar)
            {
                FormPanelBusqueda.Reset();
            }

            StoreTiendas.RemoveAll();
            FormPanelResultados.Reset();

            FormPanelDocumentos.Reset();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de búsqueda, limpiando los controles
        /// asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia(true);
        }

        /// <summary>
        /// Controla el evento Refresh en el grid de resultados, invocando nuevamente
        /// la búsqueda de tiendas a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento Refresh Data del Store Clientes</param>
        protected void StoreTiendas_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdColectiva = 0;
                string ClaveAlmacen = null, ClaveTienda = null;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Colectiva": IdColectiva = int.Parse(column.Value); break;
                        case "ClaveAlmacen": ClaveAlmacen = column.Value; break;
                        case "ClaveTienda": ClaveTienda = column.Value; break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();

                ConsultaDatosTienda(IdColectiva, ClaveAlmacen, ClaveTienda);

            }

            catch (CAppException err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Resultados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Resultados", "Ocurrió un Error al Seleccionar la Tienda").Show();
            }
        }

        /// <summary>
        /// Consulta a base de datos aquellos que corresponden a la tienda seleccionada
        /// y llena el FormPanel correspondiente
        /// </summary>
        /// <param name="idColectiva">Identificador de la tienda seleccionada</param>
        /// <param name="claveAlmacen">Clave de almacén de la tienda seleccionada</param>
        /// <param name="claveTienda">Clave de la tienda seleccionada</param>
        protected void ConsultaDatosTienda(int idColectiva, string claveAlmacen, string claveTienda)
        {
            try
            {
                FormPanelDocumentos.Reset();

                DataSet dsDatos = DAOSrPago.ObtieneDatosDocumentosTienda(
                    idColectiva, claveAlmacen, claveTienda, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.txtID_Colectiva.Text = idColectiva.ToString();
                this.txtEmail.Text = dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim();
                this.txtTarjeta.Text = dsDatos.Tables[0].Rows[0]["Tarjeta"].ToString().Trim();
            }

            catch (CAppException err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Documentación de la Tienda", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Documentación de la Tienda", "Ocurrió un Error al Consultar los Documentos de la Tienda").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar del formulario de documentos, invocando la actualización
        /// de la documentación en elWeb Service de Sr Pago
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                DatosSrPago datosDocumentos = new DatosSrPago();

                datosDocumentos.Email = this.txtEmail.Text;
                datosDocumentos.NumeroTarjeta = this.txtTarjeta.Text;
                datosDocumentos.TipoDocumentoURL = this.cmbTipoDocumento.Value.ToString();
                datosDocumentos.URLDocumento = this.txtURL.Text;

                WSPostProfileDocuments(datosDocumentos);

                X.Msg.Notify("", "Documentos Actualizados Exitosamente").Show();
            }

            catch (CAppException err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Documentación de la Tienda", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Documentación de la Tienda", "Ocurrió un Error al Actualizar los Documentos de la Tienda").Show();
            }
        }

        /// <summary>
        /// Establece a Base64 una cadena
        /// </summary>
        /// <param name="plainText">Texto plano</param>
        /// <returns>Cadena en formato Base64</returns>
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Establece la llamada al Web Service de Sr Pago para el alta y actualización de documentos
        /// </summary>
        /// <param name="losDatos">Datos obtenidos del formulario por el usuario</param>
        /// <returns>Resultado de la llamada al Web Service</returns>
        public bool WSPostProfileDocuments(DatosSrPago losDatos)
        {
            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST para el transfer
                var clientRS = new RestClient(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_URL").Valor);
                var request = new RestRequest(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_ProfileDocuments").Valor, Method.POST);

                //Body
                request.AddParameter("number", Convert.ToInt64(losDatos.NumeroTarjeta));
                request.AddParameter("email", losDatos.Email);
                request.AddParameter("url", losDatos.URLDocumento);
                request.AddParameter("type", losDatos.TipoDocumentoURL);

                //Headers HTTP
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic " + Base64Encode(
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_AppKey").Valor + ":" +
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_AppSecret").Valor));

                DALPuntoVentaWeb.Utilidades.Loguear.EntradaSalida("WSPostProfileDocuments|eMail: " + losDatos.Email, this.Usuario.ClaveUsuario, false);

                //Ejecuta la petición
                IRestResponse response = clientRS.Execute(request);
                var content = response.Content;

                //Procesa la respuesta
                WSJsonResponses.PostProfileDocuments wsPostProfileDocument = JsonConvert.DeserializeObject<WSJsonResponses.PostProfileDocuments>(content);
                if (!wsPostProfileDocument.Success)
                {
                    WSJsonResponses.Error elError = JsonConvert.DeserializeObject<WSJsonResponses.Error>(wsPostProfileDocument.Error.ToString());
                    string JsonErrorRecibido = wsPostProfileDocument.Error.ToString().Replace("\r\n", "").Replace("\"", "").Replace('"', '\0').Replace(",    ", ";");

                    DALPuntoVentaWeb.Utilidades.Loguear.EntradaSalida("WSPostProfileDocumentsResult_ERROR: " + JsonErrorRecibido + "|eMail: " + losDatos.Email, "", true);

                    sbResponse.AppendFormat("Actualización de documentos no exitosa. Mensaje de error del WS: ({0}) {1}",
                        elError.Code, elError.Message);
                    throw new Exception(sbResponse.ToString());
                }
                else
                {
                    string JsonRecibido = wsPostProfileDocument.Result.ToString().Replace("\r\n", "").Replace("\"", "").Replace('"', '\0').Replace(",    ", ";");
                    DALPuntoVentaWeb.Utilidades.Loguear.EntradaSalida("WSPostProfileDocumentsResult: " + JsonRecibido + "|eMail: " + losDatos.Email, "", true);
                }

                return wsPostProfileDocument.Success;
            }

            catch (Exception ex)
            {
                throw new Exception("WSPostProfileDocuments(): " + ex.Message);
            }
        }
    }
}