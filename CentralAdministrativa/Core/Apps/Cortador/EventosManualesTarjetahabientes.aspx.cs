using DALCortador.Entidades;
using DALEventos.BaseDatos;
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
    public partial class EventosManualesTarjetahabientes : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER ADMINISTRAR EVENTOS MANUALES
        private LogHeader LH_EvManTH = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_EvManTH.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_EvManTH.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_EvManTH.User = this.Usuario.ClaveUsuario;
            LH_EvManTH.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_EvManTH);
            string errRedirect = string.Empty;
            
            try
            {
                log.Info("INICIA EventosManualesTarjetahabientes Page_Load()");

                if (!IsPostBack)
                {
                    LlenaComboEventos();
                }

                log.Info("TERMINA EventosManualesTarjetahabientes Page_Load()");
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
        /// Solicita el catálogo de eventos manuales de tarjetahabiente a base de datos y lo establece en 
        /// el combo correspondiente
        /// </summary>
        protected void LlenaComboEventos()
        {
            LogPCI unLog = new LogPCI(LH_EvManTH);

            unLog.Info("INICIA ListaEventosManualesTarjetahabiente()");
            this.StoreEvManTH.DataSource = DAOEvento.ListaEventosManualesTarjetahabiente(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                LH_EvManTH);
            unLog.Info("TERMINA ListaEventosManualesTarjetahabiente()");

            this.StoreEvManTH.DataBind();
        }

        /// <summary>
        /// Controla el evento click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiaFormuario();
        }

        /// <summary>
        /// Restablece los controles del formulario a su estatus de carga inicial
        /// </summary>
        protected void LimpiaFormuario()
        {
            this.cBoxEvento.Reset();
            this.txtTarjeta.Reset();
            this.txtImporte.Reset();
            this.txtReferencia.Reset();
            this.txtObservaciones.Reset();

            this.hdnSaldoCLDC.Reset();
        }

        /// <summary>
        /// Controla el evento Click al botón de Ejecutar Evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnEjecutaEvManTH_Click(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_EvManTH);

            try
            {
                pCI.Info("INICIA VerificaUsuarioSubemisor()");
                DataTable dtValidaciones = LNEvento.VerificaUsuarioSubemisor(this.txtTarjeta.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), LH_EvManTH);
                pCI.Info("TERMINA VerificaUsuarioSubemisor()");

                string permisos = dtValidaciones.Rows[0]["Respuesta"].ToString();

                if (!permisos.Contains("OK"))
                {
                    X.Msg.Alert("Ejecutar Evento", permisos).Show();
                    return;
                }

                this.hdnSaldoCLDC.Value = dtValidaciones.Rows[0]["SaldoCLDC"];
                this.hdnClaveSubemisor.Value = dtValidaciones.Rows[0]["ClaveSubemisor"];

                EjecutaEventoManual();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ejecutar Evento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Ejecutar Evento", "Ocurrió un error en la validación de permisos del usuario.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Ejecutar Evento
        /// </summary>
        protected void EjecutaEventoManual()
        {
            LogPCI unLog = new LogPCI(LH_EvManTH);

            try
            {
                char[] charsToTrim = { '$', ' ', };
                EventoManual elEvento = new EventoManual();

                elEvento.IdEvento = int.Parse(this.cBoxEvento.SelectedItem.Value);
                elEvento.Concepto = this.cBoxEvento.SelectedItem.Text;
                elEvento.ClaveColectiva = this.hdnClaveSubemisor.Value.ToString();
                elEvento.MedioAcceso = this.txtTarjeta.Text;
                elEvento.Importe = this.txtImporte.Text.Trim(charsToTrim).Replace(",",  "");
                elEvento.Referencia = string.IsNullOrEmpty(this.txtReferencia.Text) ? 0 : 
                    Convert.ToInt64(this.txtReferencia.Text);
                elEvento.Observaciones = this.txtObservaciones.Text;
                elEvento.SaldoCuentaCLDC = this.hdnSaldoCLDC.Value.ToString();

                unLog.Info("INICIA RegistraEvManual_Tarjetahabiente()_" + elEvento.Concepto);
                LNEvento.RegistraEvManual_Tarjetahabiente(elEvento, this.Usuario, LH_EvManTH);
                unLog.Info("TERMINA RegistraEvManual_Tarjetahabiente()_" + elEvento.Concepto);

                X.Msg.Notify("Ejecutar Evento", "Evento Manual Ejecutado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LimpiaFormuario();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ejecutar Evento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Ejecutar Evento", "Ocurrió un error en la ejecución del evento manual.").Show();
            }
        }
    }
}