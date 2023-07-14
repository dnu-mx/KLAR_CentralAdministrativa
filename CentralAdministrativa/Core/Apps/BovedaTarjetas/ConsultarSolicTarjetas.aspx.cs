using DALCentralAplicaciones;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;

namespace BovedaTarjetas
{
    public partial class ConsultarSolicTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Bóveda Consulta Solicitudes de Tarjetas
        private LogHeader LH_BovedaConsSolicTarjetas = new LogHeader();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_BovedaConsSolicTarjetas.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_BovedaConsSolicTarjetas.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_BovedaConsSolicTarjetas.User = this.Usuario.ClaveUsuario;
            LH_BovedaConsSolicTarjetas.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_BovedaConsSolicTarjetas);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConsultarSolicTarjetas Page_Load()");

                if (!IsPostBack)
                {
                }

                log.Info("TERMINA ConsultarSolicTarjetas Page_Load()");
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
    }
}