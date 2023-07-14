using Autenticacion;
using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Web;


namespace CentralAplicaciones
{
    public partial class ResetSession : System.Web.UI.Page
    {
        #region Variables privadas

        //LOG HEADER Reset Sessions
        private LogHeader LH_ResetSession = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LH_ResetSession.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ResetSession.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ResetSession.User = this.txtUsername.Text;
            LH_ResetSession.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ResetSession);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ResetSession Page_Load()");


                if (Request.QueryString["token"] == null)
                {
                    string msj = "Posible intento de acceso No Autorizado. Acceso Denegado.";
                    log.Warn(msj);
                    throw new Exception(msj);
                }

                log.Info("TERMINA ResetSession Page_Load()");
            }
            catch(Exception ex)
            {
                log.ErrorException(ex);
                errRedirect = "AccesoRestringido.aspx";
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
        /// Controla el evento Click al botón de Aceptar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnSetSession_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ResetSession);

            try
            {
                unLog.Info("INICIA ObtieneDatosAutenticacionUsuario()");
                Password losDatos = DAOUsuario.ObtieneDatosAutenticacionToken(this.txtUsername.Text, LH_ResetSession);
                unLog.Info("TERMINA ObtieneDatosAutenticacionUsuario()");

                if (Hashing.TokenOK(this.txtToken.Text, losDatos.TokenHash, losDatos.SaltHash,
                    Convert.ToInt32(losDatos.Iteraciones), LH_ResetSession))
                {
                    unLog.Info("INICIA ModificaFechasCierreSesion()");
                    LNUsuarios.ModificaFechasCierreSesion(this.txtUsername.Text, LH_ResetSession);
                    unLog.Info("TERMINA ModificaFechasCierreSesion()");

                    this.WdwResetSession.Hide();
                    this.WdwFinal.Show();
                }
                else
                {
                    throw new CAppException(8012, "El código ingresado es incorrecto. Intenta de nuevo.");
                }
            }

            catch (CAppException err)
            {
                unLog.Warn(err.Mensaje());
                X.Msg.Alert("Restablecer Sesión", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Restablecer Sesión", "Ocurrió un error al restablecer la sesión. Intenta más tarde.").Show();
            }
        }
        
        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana final de reset de sesión
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnOkFinal_Click(object sender, DirectEventArgs e)
        {
            Response.Redirect("Logout.aspx", false);

            Response.Flush();
            Response.Close();
        }
    }
}
