using DALBovedaTarjetas.BaseDatos;
using DALBovedaTarjetas.Entidades;
using DALBovedaTarjetas.LogicaNegocio;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using WebServices;
using WebServices.Entidades;

namespace BovedaTarjetas
{
    public partial class ReservarTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Bóveda Reserva Tarjetas
        private LogHeader LH_BovedaReservaTarjetas = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reservar Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_BovedaReservaTarjetas.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_BovedaReservaTarjetas.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_BovedaReservaTarjetas.User = this.Usuario.ClaveUsuario;
            LH_BovedaReservaTarjetas.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_BovedaReservaTarjetas);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReservarTarjetas Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("tknAPIBovDig", null);
                    HttpContext.Current.Session.Add("credAPIBovDig", null);
                    HttpContext.Current.Session.Add("prodListBovDig", null);

                    string _token = string.Empty;
                    string _credenciales = string.Empty;

                    log.Info("INICIA LNBoveda.ConectaAPI()");
                    LNBoveda.ConectaAPI(Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        ref _token, ref _credenciales, LH_BovedaReservaTarjetas);
                    log.Info("TERMINA LNBoveda.ConectaAPI()");

                    HttpContext.Current.Session.Add("tknAPIBovDig", _token);
                    HttpContext.Current.Session.Add("credAPIBovDig", _credenciales);

                    EstableceEmisores();
                }

                log.Info("TERMINA ReservarTarjetas Page_Load()");
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        /// <summary>
        /// Solicita al API la lista de emisores disponibles, al Autorizador los emisores permitidos
        /// para el usuario, y descarta de ambas listas sólo los que el usuario puede ver, estableciéndolos
        /// en el combo correspondiente
        /// </summary>
        protected void EstableceEmisores()
        {
            LogPCI unLog = new LogPCI(LH_BovedaReservaTarjetas);

            try
            {
                Parametros.Headers_BD _headers = new Parametros.Headers_BD();

                _headers.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "APIBovDig_URL").Valor;
                _headers.Token = HttpContext.Current.Session["tknAPIBovDig"].ToString();
                _headers.Credentials = HttpContext.Current.Session["credAPIBovDig"].ToString();

                unLog.Info("INICIA BovedaDigital.Emisores()");
                List<EmisorBovedaDigital> emisores = BovedaDigital.Emisores(_headers, LH_BovedaReservaTarjetas);
                unLog.Info("TERMINA BovedaDigital.Emisores()");

                if (emisores.Count == 0)
                {
                    X.Msg.Alert("Emisores", "El servicio web no devolvió emisores. Intenta más tarde.").Show();
                    return;
                }
                else
                {
                    unLog.Info("INICIA ListaSubemisores()");
                    DataTable dtSubEmisores = DAOBoveda.ListaSubemisores(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_BovedaReservaTarjetas);
                    unLog.Info("TERMINA ListaSubemisores()");

                    List<EmisorBovedaDigital> listaFinal = new List<EmisorBovedaDigital>();

                    foreach (DataRow cliente in dtSubEmisores.Rows)
                    {
                        foreach (EmisorBovedaDigital emisor in emisores)
                        {
                            if (emisor.ClaveEmisor == cliente["ClaveColectiva"].ToString())
                            {
                                EmisorBovedaDigital elEmisor = new EmisorBovedaDigital();
                                elEmisor.ClaveEmisor = emisor.ClaveEmisor;
                                elEmisor.NombreEmisor = emisor.NombreEmisor;

                                listaFinal.Add(elEmisor);
                            }
                        }
                    }

                    if (listaFinal.Count == 0)
                    {
                        X.Msg.Alert("Emisores", "No tienes permisos por vista para los emisores de Bóveda.").Show();
                    }
                    else
                    {
                        this.StoreEmisor.DataSource = listaFinal;
                        this.StoreEmisor.DataBind();
                        if (listaFinal.Count == 1)
                        {
                            this.cBoxEmisor.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de Emisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceProductos(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_BovedaReservaTarjetas);

            try
            {
                ParametrosAPI parametrosAPI = new ParametrosAPI();
                parametrosAPI.Token = HttpContext.Current.Session["tknAPIBovDig"].ToString();
                parametrosAPI.Credenciales = HttpContext.Current.Session["credAPIBovDig"].ToString();
                parametrosAPI.ClaveEmisor = this.cBoxEmisor.SelectedItem.Value;

                logPCI.Info("INICIA LNBoveda.SolicitaProductosAPI()");
                List<ProductoBovedaDigital> losProductos = 
                    LNBoveda.SolicitaProductosAPI(parametrosAPI, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaReservaTarjetas);
                logPCI.Info("TERMINA LNBoveda.SolicitaProductosAPI()");

                HttpContext.Current.Session.Add("prodListBovDig", losProductos);
                List<ProductoBovedaDigital> listaProductos = new List<ProductoBovedaDigital>();

                foreach (ProductoBovedaDigital elProducto in losProductos)
                {
                    //Productos
                    ProductoBovedaDigital unProducto = new ProductoBovedaDigital();
                    unProducto.ClaveProducto = elProducto.ClaveProducto;
                    unProducto.NombreProducto = elProducto.NombreProducto;
                    listaProductos.Add(unProducto);
                }

                this.StoreProducto.DataSource = listaProductos;
                this.StoreProducto.DataBind();

                if (listaProductos.Count == 1)
                {
                    this.cBoxProducto.SelectedIndex = 0;
                }

                this.cBoxProducto.Disabled = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al establecer los Productos del Emisor.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de BIN
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceTiposMan(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_BovedaReservaTarjetas);

            try
            {
                List<TipoManufacturaBovedaDigital> listaTipos = new List<TipoManufacturaBovedaDigital>();
                ParametrosAPI parametrosAPI = new ParametrosAPI();

                parametrosAPI.Token = HttpContext.Current.Session["tknAPIBovDig"].ToString();
                parametrosAPI.Credenciales = HttpContext.Current.Session["credAPIBovDig"].ToString();

                logPCI.Info("INICIA LNBoveda.SolicitaTiposTarjetaAPI()");
                List<TipoManufacturaBovedaDigital> losTipos =
                    LNBoveda.SolicitaTiposTarjetaAPI(parametrosAPI,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaReservaTarjetas);
                logPCI.Info("TERMINA LNBoveda.SolicitaTiposTarjetaAPI()");

                foreach (TipoManufacturaBovedaDigital elTipo in losTipos)
                {
                    //Productos
                    TipoManufacturaBovedaDigital unTipo = new TipoManufacturaBovedaDigital();
                    unTipo.ClaveTipoManufactura = elTipo.ClaveTipoManufactura;
                    unTipo.NombreTipoManufactura = elTipo.NombreTipoManufactura;
                    listaTipos.Add(unTipo);
                }

                this.StoreTipoMan.DataSource = listaTipos;
                this.StoreTipoMan.DataBind();

                if (listaTipos.Count == 1)
                {
                    this.cBoxTipoTar.SelectedIndex = 0;
                }

                this.cBoxTipoTar.Disabled = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Tipos de Tarjeta", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Tipos de Tarjeta", "Ocurrió un error al establecer los Tipos de Tarjeta.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo Tipo de Tarjeta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceCantidades(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_BovedaReservaTarjetas);

            try
            {
                List<ProductoBovedaDigital> losProductos = HttpContext.Current.Session["prodListBovDig"] as List<ProductoBovedaDigital>;

                foreach (ProductoBovedaDigital elProducto in losProductos)
                {
                    if (elProducto.ClaveProducto == this.cBoxProducto.SelectedItem.Value)
                    {
                        this.lblTarjDisp.Text = string.IsNullOrEmpty(elProducto.StockDisponible) ?
                            "Sin información recibida" : string.Format("{0:n0}", int.Parse(elProducto.StockDisponible));

                        this.lblTarjReserv.Text = this.cBoxTipoTar.SelectedItem.Text.ToUpper().Contains("VIRT") ?
                            string.IsNullOrEmpty(elProducto.ReservadasVirtuales) ? "Sin información recibida" :
                            string.Format("{0:n0}", int.Parse(elProducto.ReservadasVirtuales)) : 
                            string.IsNullOrEmpty(elProducto.ReservadasFisicas) ? "Sin información recibida" :
                            string.Format("{0:n0}", int.Parse(elProducto.ReservadasFisicas));
                    }
                }

                this.FieldSetInfo.Collapsed = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Cantidades", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Cantidades", "Ocurrió un error al establecer las cantidades del Tipo de Tarjeta.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            RestablecePagina();
        }

        /// <summary>
        /// Establece los controles de la página a su estdo de carga inicial,
        /// excepto el combo Emisor
        /// </summary>
        protected void RestablecePagina()
        {
            this.cBoxEmisor.Reset();
            this.cBoxProducto.Reset();
            this.cBoxProducto.Disabled = true;
            this.StoreProducto.RemoveAll();

            this.cBoxTipoTar.Reset();
            this.cBoxTipoTar.Disabled = true;
            this.StoreTipoMan.RemoveAll();

            this.FieldSetInfo.Collapse();
            this.FormPanelInfo.Reset();

            HttpContext.Current.Session.Add("prodListBovDig", null);
        }

        /// <summary>
        /// Solicita la reservación de tarjetas a la bóveda digital, tras la confirmación del usuario
        /// </summary>
        [DirectMethod(Namespace = "ReservaTarjetasBoveda")]
        public void ConfirmaReservarTarjetas()
        {
            LogPCI unLog = new LogPCI(LH_BovedaReservaTarjetas);
            string numLote = string.Empty;

            try
            {
                ParametrosAPI losParametros = new ParametrosAPI();
                losParametros.Token = HttpContext.Current.Session["tknAPIBovDig"].ToString();
                losParametros.Credenciales = HttpContext.Current.Session["credAPIBovDig"].ToString();
                losParametros.ClaveProducto = this.cBoxProducto.SelectedItem.Value;
                losParametros.ClaveLote = "-1";
                losParametros.ClaveTipoLote = "RES";
                losParametros.ClaveTipoManufactura = this.cBoxTipoTar.SelectedItem.Value;
                losParametros.Cantidad = this.txtTarjReserv.Text;

                unLog.Info("INICIA LNBoveda.GeneraLoteAPI()");
                LNBoveda.GeneraLoteAPI(losParametros, ref numLote, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_BovedaReservaTarjetas);
                unLog.Info("TERMINA LNBoveda.GeneraLoteAPI()");

                X.Msg.Notify("", "<br />  <br /> <b> No. Lote:  " + numLote + "</b> <br />  <br /> ").Show();

                X.Msg.Notify("", "Reservación realizada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                RestablecePagina();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reservar Tarjetas", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Reservar Tarjetas", "Ocurrió un error al solicitar la reserva de tarjetas.").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }
    }
}