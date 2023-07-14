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
    public partial class ActualizacionDatos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Alta y Actualización de Datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    dfFechaNacimiento.MaxDate = DateTime.Today;
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
            FormPanelDatos.Reset();
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

            FormPanelDatos.Reset();
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
        /// Llena los controles del Form Panel Datos con los obtenidos de la
        /// consulta a base de datos
        /// </summary>
        /// <param name="idColectiva">Identificador de la tienda seleccionada</param>
        /// <param name="dsDatosEnBD">Datos de la consulta a base de datos</param>
        protected void LlenaFormPanelDatos(int idColectiva, DataSet dsDatosEnBD)
        {
            try
            {
                this.txtID_Colectiva.Text = idColectiva.ToString();
                this.txtClaveAlmacen.Text = dsDatosEnBD.Tables[0].Rows[0]["ClaveAlmacen"].ToString().Trim();
                this.txtClaveTienda.Text = dsDatosEnBD.Tables[0].Rows[0]["ClaveTienda"].ToString().Trim();
                this.txtNombreTienda.Text = dsDatosEnBD.Tables[0].Rows[0]["NombreTienda"].ToString().Trim();
                this.txtCalle.Text = dsDatosEnBD.Tables[0].Rows[0]["Calle"].ToString().Trim();
                this.txtLocalidad.Text = dsDatosEnBD.Tables[0].Rows[0]["Localidad"].ToString().Trim();
                this.txtMunicipio.Text = dsDatosEnBD.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                this.txtEstado.Text = dsDatosEnBD.Tables[0].Rows[0]["Estado"].ToString().Trim();
                this.txtCodigoPostal.Text = dsDatosEnBD.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim();
                this.txtTelefono.Text = dsDatosEnBD.Tables[0].Rows[0]["Telefono"].ToString().Trim();
                this.txtPassword.Text = dsDatosEnBD.Tables[0].Rows[0]["Pswd"].ToString().Trim();
                this.txtEmail.Text = dsDatosEnBD.Tables[0].Rows[0]["Email"].ToString().Trim();
                this.txtTarjeta.Text = dsDatosEnBD.Tables[0].Rows[0]["Tarjeta"].ToString().Trim();

                this.txtID_Operador.Text = dsDatosEnBD.Tables[0].Rows[0]["IdOperador"].ToString().Trim();
                this.txtNombreOperador.Text = dsDatosEnBD.Tables[0].Rows[0]["NombreOperador"].ToString().Trim();
                this.txtApPaternoOperador.Text = dsDatosEnBD.Tables[0].Rows[0]["ApPaternoOperador"].ToString().Trim();
                this.txtApMaternoOperador.Text = dsDatosEnBD.Tables[0].Rows[0]["ApMaternoOperador"].ToString().Trim();
                this.dfFechaNacimiento.Value = String.IsNullOrEmpty(dsDatosEnBD.Tables[0].Rows[0]["FechaNacimiento"].ToString()) ? "_" :
                                        String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dsDatosEnBD.Tables[0].Rows[0]["FechaNacimiento"].ToString()));

                this.txtURL_IFE_Frente.Text = dsDatosEnBD.Tables[0].Rows[0]["URLIFEFrente"].ToString().Trim();
                this.txtURL_IFE_Reverso.Text = dsDatosEnBD.Tables[0].Rows[0]["URLIFEReverso"].ToString().Trim();
                this.txtURL_Firma.Text = dsDatosEnBD.Tables[0].Rows[0]["URLFirma"].ToString().Trim();
                this.txtURL_Domicilio.Text = dsDatosEnBD.Tables[0].Rows[0]["URLDomicilio"].ToString().Trim();

                this.AltaEnSrPago.Text = dsDatosEnBD.Tables[0].Rows[0]["AltaSrPago"].ToString().Trim();
            }

            catch (CAppException caEx)
            {
                throw new CAppException(8006, "LlenaFormPanelDatos()", caEx);
            }

            catch (Exception ex)
            {
                throw new Exception("LlenaFormPanelDatos()" + ex.Message);
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
                FormPanelDatos.Reset();

                DataSet dsDatos = DAOSrPago.ObtieneDatosTienda(
                    idColectiva, claveAlmacen, claveTienda, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                LlenaFormPanelDatos(idColectiva, dsDatos);
            }

            catch (CAppException err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos de la Tienda", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos de la Tienda", "Ocurrió un Error al Consultar los Datos de la Tienda").Show();
            }
        }

        /// <summary>
        /// Llena la entidad DatosSrPago con los datos capturados en el formulario 
        /// </summary>
        protected DatosSrPago LlenaDatosSrPago()
        {
            try
            {
                DatosSrPago laTienda = new DatosSrPago();

                laTienda.ClaveAlmacen = this.txtClaveAlmacen.Text;
                laTienda.ClaveTienda = this.txtClaveTienda.Text;
                laTienda.NombreTienda = this.txtNombreTienda.Text;
                laTienda.Calle = this.txtCalle.Text;
                laTienda.Localidad = this.txtLocalidad.Text;
                laTienda.Municipio = this.txtMunicipio.Text;
                laTienda.Estado = this.txtEstado.Text;
                laTienda.CodigoPostal = this.txtCodigoPostal.Text;
                laTienda.Telefono = this.txtTelefono.Text;
                laTienda.Password = this.txtPassword.Text;
                laTienda.Email = this.txtEmail.Text;
                laTienda.NumeroTarjeta = this.txtTarjeta.Text;

                laTienda.NombreOperador = this.txtNombreOperador.Text;
                laTienda.ApellidoPaternoOperador = this.txtApPaternoOperador.Text;
                laTienda.ApellidoMaternoOperador = this.txtApMaternoOperador.Text;
                laTienda.FechaNacimientoOperador = String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(this.dfFechaNacimiento.Text));

                laTienda.URL_IFE_Anverso = this.txtURL_IFE_Frente.Text;
                laTienda.URL_IFE_Reverso = this.txtURL_IFE_Reverso.Text;
                laTienda.URL_Firma = this.txtURL_Firma.Text;
                laTienda.URL_CompDomicilio = this.txtURL_Domicilio.Text;

                return laTienda;
            }

            catch (CAppException caEx)
            {
                throw new CAppException(8006, "LlenaDatosSrPago()", caEx); 
            }

            catch (Exception ex)
            {
                throw new Exception("LlenaDatosSrPago() " + ex.Message);
            }
        }


        /// <summary>
        /// Controla el evento Click al botón Aceptar del formulario de datos, invocando la actualización
        /// de los datos en base de datos y en el Web Service de Sr Pago
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                DatosSrPago datosTienda = LlenaDatosSrPago();

                //Se complementan los datos en el Autorizador
                LNSrPago.CompletaDatosTienda(datosTienda, this.Usuario);

                //Se actualizan los datos en la tabla de trabajo
                LNSrPago.ActualizaDatosTienda(datosTienda, this.Usuario);

                //Si no se ha dado de alta en Sr Pago, se solicita
                if (!Convert.ToBoolean(this.AltaEnSrPago.Text))
                {
                    WSPostProfile(datosTienda);
                    LNSrPago.ActualizaEstatusAltaSrPago(datosTienda, this.Usuario);
                }

                X.Msg.Notify("", "Datos de la Tienda Actualizados Exitosamente").Show();
            }

            catch (CAppException err)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos de la Tienda", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALPuntoVentaWeb.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos de la Tienda", "Ocurrió un Error al Actualizar los Datos de la Tienda").Show();
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
        /// Concatena los parámetros necesarios a la petición REST que se hace al Web Service
        /// </summary>
        /// <param name="request">Instancia RestRequest prestablecida</param>
        /// <param name="losDatos">Datos obtenidos del formulario por el usuario</param>
        /// <returns>Instancia concatenada con los parámetros</returns>
        protected static RestRequest SetRequestParameters(RestRequest request, DatosSrPago losDatos)
        {
            request.AddParameter("email", losDatos.Email);
            request.AddParameter("password", losDatos.Password);
            request.AddParameter("name", losDatos.NombreOperador);
            request.AddParameter("last_name", losDatos.ApellidoPaternoOperador);
            request.AddParameter("second_last_name", losDatos.ApellidoMaternoOperador);
            request.AddParameter("street", losDatos.Calle);
            request.AddParameter("area", losDatos.Localidad);
            request.AddParameter("city", losDatos.Municipio);
            request.AddParameter("zip_code", losDatos.CodigoPostal);
            request.AddParameter("birth_date", losDatos.FechaNacimientoOperador);
            request.AddParameter("card_number", losDatos.NumeroTarjeta);

            ///URLs
            request.AddParameter("ife_front", String.IsNullOrEmpty(losDatos.URL_IFE_Anverso) ?
                "http://image.com/ife.png" : losDatos.URL_IFE_Anverso);
            request.AddParameter("ife_back", String.IsNullOrEmpty(losDatos.URL_IFE_Reverso) ? 
                "http://image.com/ifeback.png" : losDatos.URL_IFE_Reverso);
            request.AddParameter("signature", String.IsNullOrEmpty(losDatos.URL_Firma) ?
                "http://image.com/signature.png" : losDatos.URL_Firma);
            request.AddParameter("proof", String.IsNullOrEmpty(losDatos.URL_CompDomicilio) ?
                "http://image.com/proof.png" : losDatos.URL_CompDomicilio);

            request.AddParameter("company_name", losDatos.NombreTienda);

            ///Datos fake
            request.AddParameter("company_monthly_sales", "100000");
            request.AddParameter("company_average_price", "100");
            request.AddParameter("company_bussines_id", "2");

            request.AddParameter("phone", losDatos.Telefono);


            return request;
        }
        
        /// <summary>
        /// Establece la llamada al Web Service de Sr Pago para el alta y actualización de datos
        /// </summary>
        /// <param name="losDatos">Datos obtenidos del formulario por el usuario</param>
        /// <returns>Resultado de la llamada al Web Service</returns>
        public bool WSPostProfile(DatosSrPago losDatos)
        {
            try
            {
                StringBuilder sbResponse = new StringBuilder();

                //Se genera el POST para el transfer
                var clientRS = new RestClient(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_URL").Valor);
                var request = new RestRequest(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_Profile").Valor, Method.POST);

                //Body
                request = SetRequestParameters(request, losDatos);

                //Headers HTTP
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic " + Base64Encode(
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_AppKey").Valor + ":" +
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_AppSecret").Valor));

                //Código para almacenar la petición en el LOG
                List<RestSharp.Parameter> parametros = request.Parameters;
                StringBuilder sbPeticion = new StringBuilder();

                foreach (RestSharp.Parameter par in parametros)
                {
                    sbPeticion.Append(par).Append("|");
                }

                DALPuntoVentaWeb.Utilidades.Loguear.EntradaSalida("WSPostProfile|Petición: " + sbPeticion.ToString(), this.Usuario.ClaveUsuario, false);
                
                //Ejecuta la petición
                IRestResponse response = clientRS.Execute(request);
                var content = response.Content;

                //Procesa la respuesta
                WSJsonResponses.PostProfile wsPostProfile = JsonConvert.DeserializeObject<WSJsonResponses.PostProfile>(content);
                if (!wsPostProfile.Success)
                {
                    WSJsonResponses.Error elError = JsonConvert.DeserializeObject<WSJsonResponses.Error>(wsPostProfile.Error.ToString());
                    string JsonErrorRecibido = wsPostProfile.Error.ToString().Replace("\r\n", "").Replace("\"", "").Replace('"', '\0').Replace(",    ", ";");

                    DALPuntoVentaWeb.Utilidades.Loguear.EntradaSalida("WSPostProfileResult_ERROR: " + JsonErrorRecibido + "|Tienda: " + losDatos.IdTienda.ToString(), "", true);

                    //Este código lo manda cuando ya existe el registro de la tienda
                    if (elError.Code != "AlredyExistsException")
                    {
                        sbResponse.AppendFormat("Actualización de datos no exitosa. Mensaje de error del WS: ({0}) {1}",
                        elError.Code, elError.Message);
                        throw new Exception(sbResponse.ToString());
                    }                   
                }

                else
                {
                    string JsonRecibido = wsPostProfile.Result.ToString().Replace("\r\n", "").Replace("\"", "").Replace('"', '\0').Replace(",    ", ";");
                    DALPuntoVentaWeb.Utilidades.Loguear.EntradaSalida("WSPostProfileResult: " + JsonRecibido + "|Tienda: " + losDatos.IdTienda.ToString(), "", true);
                }

                return wsPostProfile.Success;
            }

            catch (Exception ex)
            {
                throw new Exception("WSPostProfile(): " + ex.Message);
            }
        }
    }
}