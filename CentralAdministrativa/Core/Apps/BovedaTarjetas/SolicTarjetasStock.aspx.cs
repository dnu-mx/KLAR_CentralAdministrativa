using DALAdministracion.BaseDatos;
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
using WebServices.Entidades;

namespace BovedaTarjetas
{
    public partial class SolicTarjetasStock : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Bóveda Solicitar Tarjetas Stock
        private LogHeader LH_BovedaSolicTarjStock = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Solicitud de Tarjetas en Stock
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_BovedaSolicTarjStock.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_BovedaSolicTarjStock.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_BovedaSolicTarjStock.User = this.Usuario.ClaveUsuario;
            LH_BovedaSolicTarjStock.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_BovedaSolicTarjStock);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA SolicTarjetasStock Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("tknAPIBovDig", null);
                    HttpContext.Current.Session.Add("credAPIBovDig", null);

                    EstableceEmisores();
                }

                log.Info("TERMINA SolicTarjetasStock Page_Load()");
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
        /// Solicita los emisores permitidos para el usuario en el Autorizador, y los establece
        /// en el control destinado a ello.
        /// </summary>
        protected void EstableceEmisores()
        {
            LogPCI unLog = new LogPCI(LH_BovedaSolicTarjStock);

            try
            {
                unLog.Info("INICIA ListaSubemisores()");
                DataTable dtSubEmisores = DAOBoveda.ListaSubemisores(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaSolicTarjStock);
                unLog.Info("TERMINA ListaSubemisores()");

                this.StoreEmisores.DataSource = dtSubEmisores;
                this.StoreEmisores.DataBind();

                if (dtSubEmisores.Rows.Count == 1)
                {
                    this.cBoxEmisor.SelectedIndex = 0;
                    this.cBoxEmisor.FireEvent("select");
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Establece Emisores", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Emisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceProductos(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_BovedaSolicTarjStock);

            try
            {
                pCI.Info("INICIA ObtieneProductosDeColectiva()");
                DataTable dtProds = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxEmisor.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaSolicTarjStock);
                pCI.Info("TERMINA ObtieneProductosDeColectiva()");

                if (dtProds.Rows.Count == 0)
                {
                    X.Msg.Alert("Productos", "El Emisor no tiene Productos asociados.").Show();
                }
                else
                {
                    this.StoreProducto.DataSource = dtProds;
                    this.StoreProducto.DataBind();

                    if (dtProds.Rows.Count == 1)
                    {
                        this.cBoxProducto.SelectedIndex = 0;
                        this.cBoxProducto.FireEvent("select");
                    }

                    this.cBoxProducto.Disabled = false;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al establecer los Productos del Emisor").Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceTiposTarjeta(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_BovedaSolicTarjStock);

            try
            {
                List<TipoManufacturaBovedaDigital> listaTipos = new List<TipoManufacturaBovedaDigital>();
                ParametrosAPI parametrosAPI = new ParametrosAPI();

                string _token = string.Empty;
                string _credenciales = string.Empty;

                unLog.Info("INICIA LNBoveda.ConectaAPI()");
                LNBoveda.ConectaAPI(Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    ref _token, ref _credenciales, LH_BovedaSolicTarjStock);
                unLog.Info("TERMINA LNBoveda.ConectaAPI()");

                HttpContext.Current.Session.Add("tknAPIBovDig", _token);
                HttpContext.Current.Session.Add("credAPIBovDig", _credenciales);

                parametrosAPI.Token = _token;
                parametrosAPI.Credenciales = _credenciales;

                unLog.Info("INICIA LNBoveda.SolicitaTiposTarjetaAPI()");
                List<TipoManufacturaBovedaDigital> losTipos =
                    LNBoveda.SolicitaTiposTarjetaAPI(parametrosAPI,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaSolicTarjStock);
                unLog.Info("TERMINA LNBoveda.SolicitaTiposTarjetaAPI()");
                                
                foreach (TipoManufacturaBovedaDigital tipo in losTipos)
                {
                    TipoManufacturaBovedaDigital unTipo = new TipoManufacturaBovedaDigital();
                    unTipo.ClaveTipoManufactura = tipo.ClaveTipoManufactura;
                    unTipo.NombreTipoManufactura = tipo.NombreTipoManufactura;
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
                unLog.ErrorException(ex);
                X.Msg.Alert("Tipos de Tarjeta", "Ocurrió un error al establecer los Tipos de Tarjeta.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Tipo de Manufactura
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceDisenyos(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_BovedaSolicTarjStock);

            try
            {
                if (this.cBoxTipoTar.SelectedItem.Value.ToUpper().Contains("FIS"))
                {
                    string clavePMA = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClavePMACatDisenyoTarjFis").Valor;

                    pCI.Info("INICIA ListaElementosCatalogoPMA()");
                    DataSet dsDis = DAOProducto.ListaElementosCatalogoPMA(
                        Convert.ToInt64(this.cBoxEmisor.SelectedItem.Value), -1, clavePMA,
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_BovedaSolicTarjStock);
                    pCI.Info("TERMINA ListaElementosCatalogoPMA()");


                    if (dsDis.Tables[0].Rows.Count == 0)
                    {
                        X.Msg.Alert("Diseño de Tarjeta", "El Emisor no tiene Diseños de Tarjeta en su catálogo.").Show();
                    }
                    else
                    {
                        this.StoreDisenyo.DataSource = dsDis;
                        this.StoreDisenyo.DataBind();

                        if (dsDis.Tables[0].Rows.Count == 1)
                        {
                            this.cBoxDisenyo.SelectedIndex = 0;
                        }

                        this.cBoxDisenyo.Disabled = false;
                    }
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Diseño de Tarjeta", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Diseño de Tarjeta", "Ocurrió un error al establecer los Diseños de Tarjetas del Emisor").Show();
            }
        }

        /// <summary>
        /// Controla el evento Clic del botón Solicitar Información
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnSolicitaInfo_click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_BovedaSolicTarjStock);

            try
            {
                ParametrosAPI losParametros = new ParametrosAPI();
                losParametros.Token = HttpContext.Current.Session["tknAPIBovDig"].ToString();
                losParametros.Credenciales = HttpContext.Current.Session["credAPIBovDig"].ToString();
                losParametros.ClaveProducto = this.hdnCveProducto.Value.ToString();

                logPCI.Info("INICIA LNBoveda.SolicitaProductosAPI()");
                List<ProductoBovedaDigital> losProductos =
                    LNBoveda.SolicitaProductosAPI(losParametros,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaSolicTarjStock);
                logPCI.Info("TERMINA LNBoveda.SolicitaProductosAPI()");

                foreach (ProductoBovedaDigital elProducto in losProductos)
                {
                    this.lblTarjDisp.Text = this.cBoxTipoTar.SelectedItem.Text.ToUpper().Contains("VIRT") ?
                        string.IsNullOrEmpty(elProducto.ReservadasVirtuales) ? "Sin información recibida" :
                        string.Format("{0:n0}", int.Parse(elProducto.ReservadasVirtuales)) :
                        string.IsNullOrEmpty(elProducto.ReservadasFisicas) ? "Sin información recibida" :
                        string.Format("{0:n0}", int.Parse(elProducto.ReservadasFisicas));

                    this.lblTarjEmit.Text = this.cBoxTipoTar.SelectedItem.Text.ToUpper().Contains("VIRT") ?
                        string.IsNullOrEmpty(elProducto.EmitidasVirtuales) ? "Sin información recibida" :
                        string.Format("{0:n0}", int.Parse(elProducto.EmitidasVirtuales)) : 
                        string.IsNullOrEmpty(elProducto.EmitidasFisicas) ? "Sin información recibida" :
                        string.Format("{0:n0}", int.Parse(elProducto.EmitidasFisicas));
                }

                this.FieldSetInfo.Collapsed = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Información en Bóveda", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Información en Bóveda", "Ocurrió un error al solicitar la información del Bin.").Show();
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

            this.cBoxDisenyo.Reset();
            this.cBoxDisenyo.Disabled = true;
            this.StoreDisenyo.RemoveAll();

            this.FieldSetInfo.Collapse();
            this.FormPanelInfo.Reset();
        }

        /// <summary>
        /// Solicita la reservación de tarjetas a la bóveda digital, tras la confirmación del usuario
        /// </summary>
        [DirectMethod(Namespace = "SolicitaTarjetasStock")]
        public void ConfirmaEmisionTarjetas()
        {
            LogPCI unLog = new LogPCI(LH_BovedaSolicTarjStock);

            try
            {
                PeticionAltaTarjetas peticion = new PeticionAltaTarjetas();

                peticion.ID_Producto = Convert.ToInt32(this.cBoxProducto.SelectedItem.Value);
                peticion.ClaveTipoPeticion = "STO";
                peticion.ID_ValorPrestablecido = string.IsNullOrEmpty(this.cBoxDisenyo.SelectedItem.Value) ? -1 :
                    int.Parse(this.cBoxDisenyo.SelectedItem.Value);
                peticion.Cantidad = int.Parse(this.txtTarjEmitir.Text);
                peticion.ClaveEmisor = this.hdnClaveEmisor.Value.ToString();

                ParametrosAPI parametros = new ParametrosAPI();
                parametros.Token = HttpContext.Current.Session["tknAPIBovDig"].ToString();
                parametros.Credenciales = HttpContext.Current.Session["credAPIBovDig"].ToString();
                parametros.ClaveProducto = this.hdnCveProducto.Value.ToString();
                parametros.ClaveLote = this.txtClaveSolic.Text;
                parametros.ClaveTipoLote = "SOL";
                parametros.ClaveTipoManufactura = this.cBoxTipoTar.SelectedItem.Value;
                parametros.Cantidad = this.txtTarjEmitir.Text;
                
                unLog.Info("INICIA LNBoveda.SolicitarTarjetasStock()");
                LNBoveda.SolicitarTarjetasStock(peticion, parametros, 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_BovedaSolicTarjStock);
                unLog.Info("TERMINA LNBoveda.SolicitarTarjetasStock()");

                X.Msg.Notify("", "<br />  <br /> <b> No. Lote:  " + peticion.NumLote + "</b> <br />  <br /> ").Show();

                X.Msg.Notify("", "Solicitud realizada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                RestablecePagina();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Solicitar Tarjetas Stock", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Solicitar Tarjetas Stock", "Ocurrió un error en la solicitud de tarjetas.").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }
    }
}