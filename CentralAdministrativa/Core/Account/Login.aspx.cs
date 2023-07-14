using Autenticacion;
using CentralAplicaciones.Entidades;
using CentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using DNU.Monitoreo.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;


namespace CentralAplicaciones.Account
{
    public partial class Login : System.Web.UI.Page
    {
        #region Variables privadas

        private LogHeader LH_Login = new LogHeader();

        #endregion

        /// <summary>
        /// Establece las condiciones de validación al monitoreo del aplicativo
        /// </summary>
        protected void ValidacionesMonitoreo()
        {
            LogPCI log = new LogPCI(LH_Login);

            //Libreria Monitoreo
            LNLibreriaMonitoreo _libreriaMonitoreo = new LNLibreriaMonitoreo();
            LNLibreriaMonitoreo.CargarValoresFijos();

            //Validar Firma de Parametros
            RespuestaGenerica RespuestaEvaluarFirmaParametros = _libreriaMonitoreo.EvaluarFirmaParametros();
            if (RespuestaEvaluarFirmaParametros.Codigo != 0)
            {
                log.Error("No se pudo evaluar la firma del archivo de configuraciones, favor de volver a firmar el archivo con el configurador");
                log.Error(RespuestaEvaluarFirmaParametros.Respuesta.ToString());
                log.Info("No se pudo Evaluar la firma del archivo de configuraciones, favor de volver a firmar el archivo con el configurador");
                log.Info(RespuestaEvaluarFirmaParametros.Respuesta.ToString());
                log.Warn("No se pudo Evaluar la firma del archivo de configuraciones, favor de volver a firmar el archivo con el configurador");
                EntidadAlertamientos.LibreriaMonitoreoError = true;
                return;
            }
            else
            {
                log.Info(RespuestaEvaluarFirmaParametros.Respuesta.ToString());
                EntidadAlertamientos.LibreriaMonitoreoError = false;
            }

            ////////////Monitoreo Tablas
            RespuestaGenerica RespuestaVerificarFirmasAplicativos = _libreriaMonitoreo.VerificarFirmasAplicativos();
            if (RespuestaVerificarFirmasAplicativos.Codigo != 0)
            {
                log.Error("No se pudo Iniciar la libreria Monitoreo firmas");
                log.Error(RespuestaVerificarFirmasAplicativos.Respuesta.ToString());
                log.Info("No se pudo Iniciar la libreria Monitoreo firmas");
                log.Info(RespuestaVerificarFirmasAplicativos.Respuesta.ToString());
                EntidadAlertamientos.LibreriaMonitoreoError = true;
                return;
            }
            else
            {
                log.Info(RespuestaVerificarFirmasAplicativos.Respuesta.ToString());
                EntidadAlertamientos.LibreriaMonitoreoError = false;
            }

            RespuestaGenerica RespuestaEncenderMonitoreoTabla = _libreriaMonitoreo.EncenderMonitoreoTabla();
            if (RespuestaEncenderMonitoreoTabla.Codigo != 0)
            {
                log.Error("No se pudo Iniciar la libreria Monitoreo Tabla");
                log.Error(RespuestaEncenderMonitoreoTabla.Respuesta.ToString());
                log.Info("No se pudo Iniciar la libreria Monitoreo Tabla");
                log.Info(RespuestaEncenderMonitoreoTabla.Respuesta.ToString());
            }
            else
            {
                log.Info(RespuestaEncenderMonitoreoTabla.Respuesta.ToString());

            }

            //////////////Monitoreo Archivos            
            RespuestaGenerica RespuestaEncenderMonitoreoArchivos = _libreriaMonitoreo.EncenderMonitoreoArchivo();
            if (RespuestaEncenderMonitoreoArchivos.Codigo != 0)
            {
                log.Error("No se pudo Iniciar la libreria Monitoreo Archivo");
                log.Error(RespuestaEncenderMonitoreoArchivos.Respuesta.ToString());
                log.Info("No se pudo Iniciar la libreria Monitoreo Archivo");
                log.Info(RespuestaEncenderMonitoreoArchivos.Respuesta.ToString());
                EntidadAlertamientos.LibreriaMonitoreoError = true;
            }
            else
            {
                log.Info(RespuestaEncenderMonitoreoArchivos.Respuesta.ToString());
                EntidadAlertamientos.LibreriaMonitoreoError = false;
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Login
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLogin_Click(object sender, DirectEventArgs e)
        {
            ///LOG HEADER
            LH_Login.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_Login.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_Login.Trace_ID = Guid.NewGuid();

            string username = this.txtUsername.Text;
            string password = this.txtPassword.Text;

            LH_Login.User = username;
            LogPCI log = new LogPCI(LH_Login);
            log.Info("Login()");

            bool esCambioPswdNA = false;
            string redirect = string.Empty;
            this.hdnPswdCounter.Value =
                string.IsNullOrEmpty(this.hdnPswdCounter.Value.ToString()) ? 0 : this.hdnPswdCounter.Value;

            try
            {
                //Inicio Librería Monitoreo
                log.Info("INICIA LibreriaMonitoreo()");
                ValidacionesMonitoreo();
                
                if (EntidadAlertamientos.LibreriaMonitoreoError)
                {
                    log.Warn("Debe Iniciar la libreria Monitoreo");
                    log.Error("Debe Iniciar la libreria Monitoreo");
                    throw new CAppException(9999, "Debe Iniciar la libreria Monitoreo");
                }

                LNLibreriaMonitoreo _libreriaMonitoreo = new LNLibreriaMonitoreo();

                log.Info("INICIA ConsultarAlertamientosAbiertosXClave()");
                RespuestaGenerica RespuestaConsultarAlertamientosAbiertosXClave = _libreriaMonitoreo.ConsultarAlertamientosAbiertosXClave();
                log.Info("TERMINA ConsultarAlertamientosAbiertosXClave()");
                if (RespuestaConsultarAlertamientosAbiertosXClave.Codigo != 0)
                {
                    log.Warn("[btnLogin_Click] No se pudo ConsultarAlertamientosAbiertosXClave Codigo:" + RespuestaConsultarAlertamientosAbiertosXClave.Codigo + " Respuesta: " + RespuestaConsultarAlertamientosAbiertosXClave.Respuesta.ToString());
                    log.Error("[btnLogin_Click] No se pudo ConsultarAlertamientosAbiertosXClave Codigo:" + RespuestaConsultarAlertamientosAbiertosXClave.Codigo + " Respuesta: " + RespuestaConsultarAlertamientosAbiertosXClave.Respuesta.ToString());
                    throw new CAppException(9999, "No se pudo Consultar alertamientos");
                }

                if (EntidadAlertamientos.TieneAlertamientos)
                {
                    log.Warn("[btnLogin_Click] No puede operar hasta que resuelva los Alertamientos");
                    log.Error("[btnLogin_Click] No puede operar hasta que resuelva los Alertamientos");
                    throw new CAppException(9999, "No puede operar hasta que resuelva los Alertamientos");
                }
                
                log.Info("TERMINA LibreriaMonitoreo()");
                //Fin Librería Monitoreo

                log.Info("INICIA ObtieneDatosUsuarioValido()");
                Usuario elUsuario = DAOUsuario.ObtieneDatosUsuarioValido(username, LH_Login);
                log.Info("TERMINA ObtieneDatosUsuarioValido()");

                if (elUsuario.UserName == null)
                {
                    log.Warn("No existe ningún usuario con el nombre " + username);
                    throw new CAppException(8012, "Credenciales inválidas");
                }

                HttpContext.Current.Session.Add("TraceId", Guid.NewGuid());
                HttpContext.Current.Session.Add("UsrName", username);

                //Se verifica si el usuario está bloqueado por intentos de acceso con pswd erróneo
                log.Info("INICIA VerificaBloqueoUsuarioErrorPswds()");
                string resp = LNUsuarios.VerificaBloqueoUsuarioErrorPswds(elUsuario.UserName,
                    Convert.ToInt32(ConfigurationManager.AppSettings["PasswordAttemptWindow"]), LH_Login);
                log.Info("TERMINA VerificaBloqueoUsuarioErrorPswds()");

                if (resp.Contains("OK"))
                {
                    if (Validaciones.Credenciales(username, password, LH_Login))
                    {
                        this.WdwLogin.Hide();

                        log.Info("INICIA EstatusSesionUsuario()");
                        string estatusMsj = Validaciones.EstatusSesionUsuario(elUsuario.UserName, LH_Login);
                        log.Info("TERMINA EstatusSesionUsuario()");

                        if (!estatusMsj.Contains("OK"))
                        {
                            X.Msg.Confirm("Sesión", estatusMsj + "</br></br>Para iniciar una nueva, deberás ingresar y " +
                                "confirmar el código que se enviará a tu correo electrónico.</br></br>¿Deseas continuar?",
                                new MessageBoxButtonsConfig
                                {
                                    Yes = new MessageBoxButtonConfig
                                    {
                                        Handler = "Login.ClicRestableceSesion()",
                                        Text = "Aceptar"
                                    },
                                    No = new MessageBoxButtonConfig
                                    {
                                        Handler = "Login.CancelaRestableceSesion()",
                                        Text = "Cancelar"
                                    }
                                }).Show();
                        }

                        else
                        {
                            esCambioPswdNA = !elUsuario.IsApproved;
                            int diasCaducidad = Convert.ToInt32(ConfigurationManager.AppSettings["DiasCadPswd"]);

                            //Si el usuario no está aprobado o la vigencia de la contraseña ya pasó,
                            //se requiere de un cambio de contraseña
                            if (esCambioPswdNA ||
                                elUsuario.LastPasswordChangedDate.Date.AddDays(diasCaducidad) <= DateTime.Now.Date)
                            {
                                redirect = "CambioPassword.aspx?ChngPswd=" + esCambioPswdNA + "&PswdExp=" + true;
                            }

                            else
                            {
                                HttpContext.Current.Session.Timeout = 5;

                                //GENERA EL USUARIO TEMPORAL PARA LA SESION
                                log.Info("INICIA InsertaUsuarioTemporal()");
                                DAOUsuario.InsertaUsuarioTemporal(txtUsername.Text, LH_Login);
                                log.Info("TERMINA InsertaUsuarioTemporal()");

                                log.Info("INICIA ObtieneCaracteristicasUsuario()");
                                elUsuario = DAOUsuario.ObtieneCaracteristicasUsuario(username, LH_Login);
                                log.Info("TERMINA ObtieneCaracteristicasUsuario()");

                                string encryptedTicket;
                                StringBuilder rolesString = new StringBuilder(50);
                                HttpCookie authCookie;

                                // Create the authentication ticket
                                FormsAuthenticationTicket authTicket = new
                                    FormsAuthenticationTicket(1,   // version
                                    txtUsername.Text,                       // user name
                                    DateTime.Now,               // creation
                                    DateTime.Now.AddMinutes(60),// Expiration
                                    false,                      // Persistent
                                    elUsuario.RolesToString());                    // User data

                                // Now encrypt the ticket.
                                encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                                // Create a cookie and add the encrypted ticket to the 
                                // cookie as data.
                                authCookie = new HttpCookie(elUsuario.ClaveUsuario, encryptedTicket);
                                // Add the cookie to the outgoing cookies collection. 
                                Response.Cookies.Add(authCookie);

                                HttpCookie endSessionCookie = new HttpCookie("ES_usr");
                                endSessionCookie.Value = encryptedTicket;
                                endSessionCookie.Expires = DateTime.Now.AddHours(10);
                                Response.Cookies.Add(endSessionCookie);

                                //ingresa a variable de Sesion:
                                HttpContext.Current.Session.Add("usuario", elUsuario);

                                /////LOG HEADER FILTROS
                                LogHeader logHF = new LogHeader();
                                logHF.IP_Address = LH_Login.IP_Address;
                                logHF.Application_ID = LH_Login.Application_ID;
                                logHF.User = LH_Login.User;
                                logHF.Trace_ID = Guid.NewGuid();

                                LogPCI logFiltros = new LogPCI(logHF);

                                //Llena los filtros de seguridad en las aplicacioens
                                logFiltros.Info("INICIA ExportaFiltros()");
                                LNFiltro.ExportaFiltros(elUsuario, logHF);
                                logFiltros.Info("TERMINA ExportaFiltros()");

                                if (Request.QueryString["ReturnUrl"] != null)
                                {
                                    String dir = Request.QueryString["ReturnUrl"];

                                    FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, false);
                                    redirect = dir;
                                }
                                else
                                {
                                    FormsAuthentication.SetAuthCookie(txtUsername.Text, false);
                                    redirect = "../Principal.aspx";
                                }
                            }
                        }
                    }
                    else
                    {
                        int counter = Convert.ToInt32(this.hdnPswdCounter.Value) + 1;

                        //Se alcanzó el número máximo de contraseñas inválidas
                        if (counter >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxInvalidPasswordAttempts"]))
                        {
                            Usuario newUser = new Usuario();
                            newUser.ClaveUsuario = "CENTRAL ADMINISTRATIVA";

                            log.Info("INICIA DesactivarUsuario()");
                            LNUsuarios.DesactivarUsuario(username, LH_Login, newUser);
                            log.Info("TERMINA DesactivarUsuario()");

                            X.Msg.Alert("Inicio de Sesión", "Usuario Bloqueado").Show();
                        }
                        else
                        {
                            log.Warn("Intento de acceso inválido # " + counter.ToString() +". Usuario: " + username);
                            this.hdnPswdCounter.Value = counter;
                            X.Msg.Alert("Inicio de Sesión", "Credenciales inválidas").Show();
                        }
                    }
                }
                else
                {
                    X.Msg.Alert("Inicio de Sesión", resp).Show();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Inicio de Sesión", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);

                X.Msg.Alert("Inicio de Sesión", "Ocurrió un error al iniciar tu sesión. Intenta de nuevo.").Show();
            }
            finally
            {
                if (!string.IsNullOrEmpty(redirect))
                {
                    Response.Redirect(redirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        /// <summary>
        /// Controla el evento Click al link de olvido de contraseña
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnReset_Click(object sender, DirectEventArgs e)
        {
            LH_Login.User = this.txtUsername.Text;
            LogPCI unLog = new LogPCI(LH_Login);

            try
            {
                unLog.Info("INICIA ReseteaPasswordUsuario()");
                LNUsuarios.ReseteaPasswordUsuario(this.txtUserReset.Text, this.txtEmail.Text, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_Login);
                unLog.Info("TERMINA ReseteaPasswordUsuario()");

                X.Msg.Alert("Restablecer Contraseña", "Correo enviado exitosamente. Verifica también tu bandeja de " +
                    "correos no deseados.", new MessageBoxButtonsConfig
                    {
                        Yes = new MessageBoxButtonConfig
                        {
                            Handler = "ResetPswd.ClicFin()",
                            Text = "Aceptar"
                        }
                    }).Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Restablecer Contraseña", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Restablecer Contraseña", "Ocurrió un error al restablecer tu contraseña. Intenta más tarde.").Show();
            }
        }

        /// <summary>
        /// Finaliza el ciclo de recuperación de contraseña
        /// </summary>
        [DirectMethod(Namespace = "ResetPswd")]
        public void ClicFin()
        {
            Response.Redirect("../DefaultOut.aspx", false);

            Response.Flush();
            Response.Close();
        }

        /// <summary>
        /// Controla el evento Click al botón Cancelar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnCancel_Click(object sender, DirectEventArgs e)
        {
            Response.Redirect("../DefaultOut.aspx", false);

            Response.Flush();
            Response.Close();
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic del botón oculto cancelar restablecer sesión
        /// </summary>
        [DirectMethod(Namespace = "Login")]
        public void CancelaRestableceSesion()
        {
            this.btnCancelaRstSesn.FireEvent("click");
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic del botón oculto restablecer sesión
        /// </summary>
        [DirectMethod(Namespace = "Login")]
        public void ClicRestableceSesion()
        {
            this.btnSessionMail.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón Restablece Sesión
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnRestableceSesion_Click(object sender, DirectEventArgs e)
        {
            LH_Login.User = this.txtUsername.Text;
            LogPCI unLog = new LogPCI(LH_Login);

            try
            {
                string user = HttpContext.Current.Session["UsrName"].ToString();
                string new_Token = string.Empty;
                string token_Key = string.Empty;

                unLog.Info("INICIA ObtieneDatosAutenticacionUsuario()");
                Password losDatos = DAOUsuario.ObtieneDatosAutenticacionUsuario(user, LH_Login);
                unLog.Info("TERMINA ObtieneDatosAutenticacionUsuario()");

                //Genera token seguro
                Validaciones.NuevoTokenSesion(ref new_Token, ref token_Key, losDatos.SaltHash, losDatos.Iteraciones, LH_Login);

                unLog.Info("INICIA ReseteaSesionUsuario()");
                LNUsuarios.ReseteaSesionUsuario(this.txtSessionEmail.Text, new_Token, token_Key,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_Login);
                unLog.Info("TERMINA ReseteaSesionUsuario()");

                X.Msg.Alert("Restablecer Sesión", "Correo enviado exitosamente. Verifica también tu bandeja de " +
                    "correos no deseados.", new MessageBoxButtonsConfig
                    {
                        Yes = new MessageBoxButtonConfig
                        {
                            Handler = "ResetPswd.ClicFin()",
                            Text = "Aceptar"
                        }
                    }).Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Restablecer Sesión", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Restablecer Sesión", "Ocurrió un error al restablecer tu contraseña. Intenta más tarde.").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza mensajes de máscaras en botones
        /// </summary>
        [DirectMethod(Namespace = "Login")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
