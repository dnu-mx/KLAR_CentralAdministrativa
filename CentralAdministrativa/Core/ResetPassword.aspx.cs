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
using System.Data;
using System.Web;


namespace CentralAplicaciones
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        #region Variables privadas

        //LOG HEADER Reset Password
        private LogHeader LH_ResetPswd = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LH_ResetPswd.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ResetPswd.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ResetPswd.User = this.txtUsername.Text;
            LH_ResetPswd.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ResetPswd);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ResetPassword Page_Load()");

                int longPswd = Convert.ToInt32(ConfigurationManager.AppSettings["MinRequiredPasswordLength"]);

                this.lblCondiciones.Text = " Tu nueva contraseña debe estar compuesta de al menos " +
                    longPswd.ToString() + " caracteres. Debe contener al menos una letra minúscula, una letra " +
                    "mayúscula, un número y un caracter especial.";

                this.txtNuevoPassword.MinLength = longPswd;
                this.txtReNuevoPassword.MinLength = longPswd;

                if (Request.QueryString["token"] == null)
                {
                    string msj = "Posible intento de acceso No Autorizado. Acceso Denegado.";
                    log.Warn(msj);
                    throw new Exception(msj);
                }

                HttpContext.Current.Session.Add("TKN", Request.QueryString["token"]);

                log.Info("TERMINA ResetPassword Page_Load()");
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
        protected void btnSetPswd_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ResetPswd);

            try
            {
                if (ValidaDatosReset())
                {
                    Password losDatosPwd = new Password();
                    losDatosPwd.NombreUsuario = this.txtUsername.Text;
                    losDatosPwd.Nuevo = this.txtNuevoPassword.Text;
                    losDatosPwd.Repeticion = this.txtReNuevoPassword.Text;
                    losDatosPwd.ExpresionRegular = ConfigurationManager.AppSettings["PasswordStrengthRegularExpression"].ToString();
                    losDatosPwd.NumMaxHistorial = ConfigurationManager.AppSettings["HistMaxPswd"].ToString();

                    //Cumple todas las validaciones de contraseña
                    if (Validaciones.CondicionesPassword(losDatosPwd, true, LH_ResetPswd))
                    {
                        string passwordHash = string.Empty;
                        string passwordSaltHash = string.Empty;
                        int iteraciones = 0;
                        string ipAdressHash = string.Empty;

                        unLog.Info("INICIA ObtieneDatosEditarUsuario()");
                        Usuario miUser = DAOUsuario.ObtieneDatosEditarUsuario(losDatosPwd.NombreUsuario, LH_ResetPswd);
                        unLog.Info("TERMINA ObtieneDatosEditarUsuario()");

                        string ip = string.Empty;
                        if (!string.IsNullOrEmpty(miUser.LocalIP))
                            ip = Hashing.GetClientIp();

                        Hashing.CreaPasswordUsuario(losDatosPwd.Nuevo, ref passwordHash, ref passwordSaltHash,
                            ref iteraciones, ref ipAdressHash, LH_ResetPswd, ip, losDatosPwd.NombreUsuario);

                        losDatosPwd.Hash = passwordHash;
                        losDatosPwd.SaltHash = passwordSaltHash;
                        losDatosPwd.Iteraciones = iteraciones.ToString();
                        losDatosPwd.LocalIP = ipAdressHash;

                        ///Historial
                        string historyPwdHash = string.Empty;
                        string historyPwdSaltHash = string.Empty;
                        int historyIteraciones = 0;

                        Hashing.CreaPasswordHistorial(losDatosPwd.Nuevo, losDatosPwd.NombreUsuario, ref historyPwdHash,
                            ref historyPwdSaltHash, ref historyIteraciones, LH_ResetPswd);

                        losDatosPwd.HistoryHash = historyPwdHash;
                        losDatosPwd.HistorySaltHash = historyPwdSaltHash;
                        losDatosPwd.HistoryIteraciones = historyIteraciones.ToString();

                        unLog.Info("INICIA ModificaPasswordUsuario()");
                        LNUsuarios.ModificaPasswordUsuario(losDatosPwd, LH_ResetPswd);
                        unLog.Info("TERMINA ModificaPasswordUsuario()");

                        this.WdwResetPswd.Hide();
                        this.WdwFinal.Show();
                    }
                }
            }

            catch (CAppException err)
            {
                unLog.Warn(err.Mensaje());
                X.Msg.Alert("Restablecer Contraseña", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Restablecer Contraseña", "Ocurrió un error al restablecer la contraseña. Intenta más tarde.").Show();
            }
        }
        
        /// <summary>
        /// Manipula el token de llamada a la página para validarlo contra la información en base de datos
        /// </summary>
        /// <returns>TRUE si los datos de reset corresponden</returns>
        protected bool ValidaDatosReset()
        {
            LogPCI unLog = new LogPCI(LH_ResetPswd);

            try
            {
                long longToken = long.Parse(HttpContext.Current.Session["TKN"].ToString(), 
                    System.Globalization.NumberStyles.HexNumber);
                DateTime date = new DateTime(longToken);

                unLog.Info("INICIA VerificaDatosResetPswd()");
                DataTable dtDatos = LNUsuarios.VerificaDatosResetPswd(this.txtUsername.Text, date, LH_ResetPswd);
                unLog.Info("TERMINA VerificaDatosResetPswd()");

                string msj = dtDatos.Rows[0]["Mensaje"].ToString();

                if (msj.ToUpper().Contains("ERROR"))
                {
                    unLog.Warn("UserName: " + this.txtUsername.Text + ";" + msj);
                    throw new CAppException(8012, msj);
                }

                return true;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8012, "Ocurrió un error al validar los datos de recuperación de contraseña.");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana final de reset de contraseña
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnOkFinal_Click(object sender, DirectEventArgs e)
        {
            Response.Redirect("Logout.aspx", false);

            Response.Flush();
            Response.Close();
        }

        private int GetNextHastSecurityStatus(int statusHashIPSecurity)
        {
            if (statusHashIPSecurity == (int)HashIpValidationsLifeCycle.NONE_TO_ACTIVE)
                return (int)HashIpValidationsLifeCycle.ACTIVE;

            if (statusHashIPSecurity == (int)HashIpValidationsLifeCycle.ACTIVE_TO_NONE)
                return (int)HashIpValidationsLifeCycle.NONE;

            return statusHashIPSecurity;
        }
    }
}
