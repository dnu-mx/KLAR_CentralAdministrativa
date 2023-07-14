using DALCentralAplicaciones.LogicaNegocio;
using Dnu.AutorizadorParabiliaAzure.Services;
using Log_PCI;
using Log_PCI.Entidades;
using log4net;
using log4net.Config;
using System;
using System.Configuration;

namespace BovedaTarjetas
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ///LOG HEADER
            LogHeader LH_GlobalBoveda = new LogHeader();
            var ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            GlobalContext.Properties["ts"] = ts;

            XmlConfigurator.Configure();

            LH_GlobalBoveda.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_GlobalBoveda.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_GlobalBoveda);
            log.Info("INICIA BovedaTarjetas_Application_Start()");

            try
            {
                if (bool.Parse(ConfigurationManager.AppSettings["AzureOn"].ToString()))
                {
                    string appKeyVal = ConfigurationManager.AppSettings["appIdKey"].ToString();
                    string cliKey = ConfigurationManager.AppSettings["clientKey"].ToString();
                    KeyVaultProvider.RegistrarProvedorCEK(appKeyVal, cliKey);
                }

                ValoresInicial.InicializarContexto(LH_GlobalBoveda);

                log.Info("TERMINA BovedaTarjetas_Application_Start()");
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}