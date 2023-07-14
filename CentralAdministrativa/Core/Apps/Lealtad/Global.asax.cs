using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using DALCentralAplicaciones.LogicaNegocio;
using Interfases;
using System.Configuration;

namespace Lealtad
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciarse la aplicación
            ValoresInicial.InicializarContexto();

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

        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{
        //    //Fires upon attempting to authenticate the use
        //    // Extract the forms authentication cookie
        //    string cookieName = FormsAuthentication.FormsCookieName;
        //    HttpCookie authCookie = Context.Request.Cookies["USERLOG"];

        //    if (null == authCookie)
        //    {
        //        // There is no authentication cookie.
        //        return;
        //    }

        //    FormsAuthenticationTicket authTicket = null;
        //    try
        //    {
        //        authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception details (omitted for simplicity)
        //        return;
        //    }

        //    if (null == authTicket)
        //    {
        //        // Cookie failed to decrypt.
        //        return;
        //    }

        //    // When the ticket was created, the UserData property was assigned a
        //    // pipe delimited string of role names.
        //    string[] roles = authTicket.UserData.Split(new char[] { '|' });

        //    // Create an Identity object
        //    FormsIdentity id = new FormsIdentity(authTicket);

        //    // This principal will flow throughout the request.
        //    GenericPrincipal principal = new GenericPrincipal(id, roles);
        //    // Attach the new principal object to the current HttpContext object
        //    Context.User = principal;

          

        //}    
    
    }
}
