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


namespace CentralAplicaciones.Account
{
    public partial class CambioPassword : System.Web.UI.Page
    {
        #region Variables privadas

        //LOG HEADER Cambio de Password
        private LogHeader LH_CambioPswd = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LH_CambioPswd.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_CambioPswd.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_CambioPswd.User = HttpContext.Current.Session["UsrName"].ToString();
            LH_CambioPswd.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_CambioPswd);
            HttpContext.Current.Session.Add("LogHeaderChngPwd", LH_CambioPswd);

            try
            {
                int longPswd = Convert.ToInt32(ConfigurationManager.AppSettings["MinRequiredPasswordLength"]);

                this.lblCondiciones.Text = " Tu nueva contraseña debe estar compuesta de al menos " +
                    longPswd.ToString() + " caracteres. Debe contener al menos una letra minúscula, una letra " +
                    "mayúscula, un número y un caracter especial.";

                this.txtNuevoPassword.MinLength = longPswd;
                this.txtReNuevoPassword.MinLength = longPswd;

                if (Convert.ToBoolean(Request.QueryString["PswdExp"]))
                {
                    this.btnCancel.Hide();
                    X.Msg.Alert("Sesión", "Tu contraseña ha expirado.").Show();
                }
            }

            catch(Exception ex)
            {
                log.ErrorException(ex);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Cambiar Contraseña
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnCambiarPswd_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_CambioPswd);

            try
            {
                Password losDatosPwd = new Password();
                losDatosPwd.NombreUsuario = HttpContext.Current.Session["UsrName"].ToString();
                losDatosPwd.Actual = this.txtPasswordActual.Text;
                losDatosPwd.Nuevo = this.txtNuevoPassword.Text;
                losDatosPwd.Repeticion = this.txtReNuevoPassword.Text;
                losDatosPwd.ExpresionRegular = ConfigurationManager.AppSettings["PasswordStrengthRegularExpression"].ToString();
                losDatosPwd.NumMaxHistorial = ConfigurationManager.AppSettings["HistMaxPswd"].ToString();

                //Cumple todas las validaciones de contraseña
                if (Validaciones.CondicionesPassword(losDatosPwd, false, LH_CambioPswd))
                {
                    string passwordHash = string.Empty;
                    string passwordSaltHash = string.Empty;
                    int iteraciones = 0;
                    string ipAdressHash = string.Empty;

                    unLog.Info("INICIA ObtieneDatosEditarUsuario()");
                    Usuario miUser = DAOUsuario.ObtieneDatosEditarUsuario(losDatosPwd.NombreUsuario, LH_CambioPswd);
                    unLog.Info("TERMINA ObtieneDatosEditarUsuario()");

                    string ip = string.Empty;
                    if (!string.IsNullOrEmpty(miUser.LocalIP))
                        ip = Hashing.GetClientIp();

                    Hashing.CreaPasswordUsuario(losDatosPwd.Nuevo, ref passwordHash, ref passwordSaltHash,
                            ref iteraciones, ref ipAdressHash, LH_CambioPswd, ip, losDatosPwd.NombreUsuario);

                    losDatosPwd.Hash = passwordHash;
                    losDatosPwd.SaltHash = passwordSaltHash;
                    losDatosPwd.Iteraciones = iteraciones.ToString();
                    losDatosPwd.LocalIP = ipAdressHash;

                    ///Historial
                    string historyPwdHash = string.Empty;
                    string historyPwdSaltHash = string.Empty;
                    int historyIteraciones = 0;

                    Hashing.CreaPasswordHistorial(losDatosPwd.Nuevo, losDatosPwd.NombreUsuario, ref historyPwdHash,
                        ref historyPwdSaltHash, ref historyIteraciones, LH_CambioPswd);

                    losDatosPwd.HistoryHash = historyPwdHash;
                    losDatosPwd.HistorySaltHash = historyPwdSaltHash;
                    losDatosPwd.HistoryIteraciones = historyIteraciones.ToString();

                    unLog.Info("INICIA ModificaPasswordUsuario()");
                    LNUsuarios.ModificaPasswordUsuario(losDatosPwd, LH_CambioPswd);
                    unLog.Info("TERMINA ModificaPasswordUsuario()");

                    this.WdwCambioPassword.Hide();
                    this.WdwReinicia.Show();
                }
            }

            catch (CAppException err)
            {
                unLog.Warn(err.Mensaje());
                X.Msg.Alert("Cambio de Contraseña", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Cambio de Contraseña", "Ocurrió un error al realizar el cambio de contraseña. Intenta más tarde.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Cancelar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnCancel_Click(object sender, DirectEventArgs e)
        {
            Response.Redirect("../Principal.aspx", false);

            Response.Flush();
            Response.Close();
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana de reinicio de contraseña
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnOkReinicia_Click(object sender, DirectEventArgs e)
        {
            Response.Redirect("../Logout.aspx", false);

            Response.Flush();
            Response.Close();
        }
    }
}
