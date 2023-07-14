using DALCentralAplicaciones.LogicaNegocio;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace CentralAplicaciones
{
    public partial class LogOut : System.Web.UI.Page
    {
        #region Variables privadas

        private LogHeader LH_LogOut = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            LH_LogOut.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_LogOut.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_LogOut.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_LogOut);

            try
            {
                log.Info("INICIA LogOut Page_Load()");

                try
                {
                    HttpCookie cookie = Request.Cookies.Get("ES_usr");
                    FormsAuthenticationTicket formAT = FormsAuthentication.Decrypt(cookie.Value);
                    LH_LogOut.User = formAT.Name;
                }
                catch 
                {
                    LH_LogOut.User = string.Empty;
                    log.Warn("No se pudo recuperar cookie. El nombre de usuario es nulo.");
                }

                CerrarSesion();

                log.Info("TERMINA LogOut Page_Load()");
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
            }
            finally
            {
                Response.Redirect("DefaultOut.aspx", false);

                Response.Flush();
                Response.Close();
            }
        }

        /// <summary>
        /// Solicita el cierre de la sesión del usuario en base de datos, cierra la sesión HTTP y elimina cookies
        /// </summary>
        protected void CerrarSesion()
        {
            LogPCI log = new LogPCI(LH_LogOut);

            try
            {
                log.Info("INICIA CerrarSesionUsuario()");
                LNUsuarios.CerrarSesionUsuario(LH_LogOut.User, LH_LogOut);
                log.Info("TERMINA CerrarSesionUsuario()");

                Response.Cookies.Clear();
                Session.Abandon();
                Session.Clear();
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_LogOut);
                unLog.WarnException(ex);
            }
        }
    }
}