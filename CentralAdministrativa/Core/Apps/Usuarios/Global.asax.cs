#undef CUBOS
//Esta directiva se establece indefinida, ya que sólo es necesaria en clases que requieren la biblioteca Framework

using DALCentralAplicaciones.LogicaNegocio;
using Dnu.AutorizadorParabiliaAzure.Services;
using Log_PCI;
using Log_PCI.Entidades;
using log4net;
using log4net.Config;
using System;
using System.Configuration;

namespace Usuarios
{
    public class Global : System.Web.HttpApplication
    {
        // Código que se ejecuta al iniciarse la aplicación
        void Application_Start(object sender, EventArgs e)
        {
            ///LOG HEADER
            LogHeader LH_GlobalUsuarios = new LogHeader();
            var ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            GlobalContext.Properties["ts"] = ts;

            XmlConfigurator.Configure();

            LH_GlobalUsuarios.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_GlobalUsuarios.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_GlobalUsuarios);
            log.Info("INICIA Usuarios_Application_Start()");

            try
            {
                if (bool.Parse(ConfigurationManager.AppSettings["AzureOn"].ToString()))
                {
                    string appKeyVal = ConfigurationManager.AppSettings["appIdKey"].ToString();
                    string cliKey = ConfigurationManager.AppSettings["clientKey"].ToString();
                    KeyVaultProvider.RegistrarProvedorCEK(appKeyVal, cliKey);
                }

                ValoresInicial.InicializarContexto(LH_GlobalUsuarios);

#if (CUBOS)
                Framework.ContextoInicial.InicializarContexto();
#endif

                log.Info("TERMINA Usuarios_Application_Start()");
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Código que se ejecuta cuando se cierra la aplicación

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Código que se ejecuta al producirse un error no controlado

        }

        void Session_Start(object sender, EventArgs e)
        {

            // Código que se ejecuta cuando se inicia una nueva sesión
            //ContextoInicial.InicializarContexto();
        }

        void Session_End(object sender, EventArgs e)
        {
            // Código que se ejecuta cuando finaliza una sesión.
            // Nota: el evento Session_End se desencadena sólo cuando el modo sessionstate
            // se establece como InProc en el archivo Web.config. Si el modo de sesión se establece como StateServer 
            // o SQLServer, el evento no se genera.

        }
    }
}
