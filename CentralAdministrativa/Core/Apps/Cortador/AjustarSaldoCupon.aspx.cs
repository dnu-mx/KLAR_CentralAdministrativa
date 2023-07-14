using DALCentralAplicaciones;
using DALCortador.BaseDatos;
using DALCortador.Entidades;
using DALEventos.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;

namespace Cortador
{
    public partial class AjustarSaldoCupon : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER AJUSTE SALDO CUPON
        private LogHeader LH_AjSaldoCup = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Ajustar Saldo de Cupón
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_AjSaldoCup.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_AjSaldoCup.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_AjSaldoCup.User = this.Usuario.ClaveUsuario;
            LH_AjSaldoCup.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_AjSaldoCup);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AjustarSaldoCupon Page_Load()");

                if (!IsPostBack)
                {
                    this.FormPanelResultados.Collapsed = true;

                    this.LlenaComboPromociones();
                }

                log.Info("TERMINA AjustarSaldoCupon Page_Load()");
            }

            catch (CAppException)
            {
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
        /// Llena el combo con las promociones permitidas para el usuario y la aplicación
        /// </summary>
        protected void LlenaComboPromociones()
        {
            LogPCI log = new LogPCI(LH_AjSaldoCup);

            try
            {
                log.Info("INICIA ObtienePromociones()");
                cmbPromocion.GetStore().DataSource = DAOTpvWebLoyalty.ObtienePromociones(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_AjSaldoCup);
                log.Info("TERMINA ObtienePromociones()");

                cmbPromocion.GetStore().DataBind();
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento de selección de un elemento del combo de Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void LlenaDetallePromocion(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_AjSaldoCup);

            try
            {
                this.cmbDetallePromocion.Reset();

                logPCI.Info("INICIA ObtieneDetallePromocion()");
                cmbDetallePromocion.GetStore().DataSource = DAOTpvWebLoyalty.ObtieneDetallePromocion(
                    int.Parse(this.cmbPromocion.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_AjSaldoCup);
                logPCI.Info("TERMINA ObtieneDetallePromocion()");

                cmbDetallePromocion.GetStore().DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ajustar Saldo de Cupón", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Ajustar Saldo de Cupón", "Ocurrió un error al consultar los Detalles de la Promoción.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel de búsqueda de cupón
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, DirectEventArgs e)
        {
            BuscaDatosYLigaPanelResultados();
        }

        /// <summary>
        /// Busca los datos del cupón y los valida, ligando la información al panel de resultados
        /// </summary>
        protected void BuscaDatosYLigaPanelResultados()
        {
            LogPCI pCI = new LogPCI(LH_AjSaldoCup);

            try
            {
                this.FormPanelResultados.Reset();

                pCI.Info("INICIA ConsultaCuentaSaldoColectivaCupon()");
                DataSet dsCupon =
                    DAOTpvWebLoyalty.ConsultaCuentaSaldoColectivaCupon(
                        int.Parse(this.cmbPromocion.SelectedItem.Value),
                        int.Parse(this.cmbDetallePromocion.SelectedItem.Value),
                        this.txtCupon.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_AjSaldoCup);
                pCI.Info("TERMINA ConsultaCuentaSaldoColectivaCupon()");

                if (dsCupon.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Búsqueda de Cupón", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }

                else
                {
                    this.hidIdColectiva.Text = dsCupon.Tables[0].Rows[0]["ID_Colectiva"].ToString().Trim();
                    this.hidIdTipoColectiva.Text = dsCupon.Tables[0].Rows[0]["ID_TipoColectivaCCM"].ToString().Trim();
                    this.hidClaveCadena.Text = dsCupon.Tables[0].Rows[0]["ClaveCCM"].ToString().Trim();
                    this.txtSaldoActual.Text = String.Format("{0:F0}", float.Parse(dsCupon.Tables[0].Rows[0]["SaldoActual"].ToString().Trim()));
                }

                this.FormPanelResultados.Title = "Cupón " + this.txtCupon.Text;
                this.FormPanelResultados.Collapsed = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Búsqueda de Cupón", err.Mensaje()).Show();
            }

            catch (Exception err)
            {
                pCI.ErrorException(err);
                X.Msg.Alert("Búsqueda de Cupón", "Ocurrió un error al consultar el saldo del cupón.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de búsqueda, 
        /// limpiando los controles del mismo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.FormPanelBusqueda.Reset();

            this.FormPanelResultados.Reset();
            this.FormPanelResultados.Collapsed = true;
        }

        /// <summary>
        /// Controla el evento Click al botón de Aceptar del formulario de resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAceptar_Click(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_AjSaldoCup);
            pCI.Info("btnAceptar_Click()");

            try
            {
                if (Convert.ToDecimal(nfSaldoNuevo.Text) == Convert.ToDecimal(txtSaldoActual.Text))
                {
                    X.Msg.Alert("Ajuste de Saldo", "El nuevo saldo del cupón no puede ser igual al saldo actual").Show();
                    return;
                }

                EvManAjusteSaldoCupon elEvento = new EvManAjusteSaldoCupon();
                DataSet dsEvento = new DataSet();
                Decimal importe;

                if (Convert.ToDecimal(nfSaldoNuevo.Text) > Convert.ToDecimal(txtSaldoActual.Text))
                {
                    importe = Convert.ToDecimal(nfSaldoNuevo.Text) - Convert.ToDecimal(txtSaldoActual.Text);

                    dsEvento = DAOTpvWebLoyalty.ConsultaEventoAbonoSaldoCupon(this.Usuario);
                }
                else
                {
                    importe = Convert.ToDecimal(txtSaldoActual.Text) - Convert.ToDecimal(nfSaldoNuevo.Text);

                    dsEvento = DAOTpvWebLoyalty.ConsultaEventoCargoSaldoCupon(this.Usuario);
                }

                elEvento.IdColectiva = Convert.ToInt32(this.hidIdColectiva.Text);
                elEvento.IdTipoColectiva = Convert.ToInt32(this.hidIdTipoColectiva.Text);
                elEvento.ClaveColectivaCCM = this.hidClaveCadena.Text;
                elEvento.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                elEvento.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                elEvento.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                elEvento.Importe = importe.ToString();
                elEvento.Observaciones = this.txtObservaciones.Text;
                elEvento.MedioAcceso = this.txtCupon.Text;

                LogHeader LH_AjSaldoCupon = new LogHeader();
                LNEvento.RegistraEvManual_AjusteSaldoCupon(elEvento, this.Usuario, LH_AjSaldoCupon);

                X.Msg.Notify("", "Ajuste de Saldo de Cupón <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();

                this.FormPanelResultados.Reset();

                BuscaDatosYLigaPanelResultados();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ajuste de Saldo", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Ajuste de Saldo", "Ocurrió un Error al Ajustar el Saldo del Cupón").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Cancelar del formulario de resultados, 
        /// limpiando los controles del mismo y restableciendo la búsqueda original
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.FormPanelResultados.Reset();

            BuscaDatosYLigaPanelResultados();
        }
    }
}