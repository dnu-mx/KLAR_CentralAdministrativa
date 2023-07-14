using DALCentralAplicaciones.LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using ValidacionBatch;
using ValidacionBatch.App_data;

namespace ValidacionBatch
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            // Código que se ejecuta al iniciarse la aplicación
            ValoresInicial.InicializarContexto();

            GlobalConfiguration.Configure(WebApiConfig.Register);

          
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }
    }
}
