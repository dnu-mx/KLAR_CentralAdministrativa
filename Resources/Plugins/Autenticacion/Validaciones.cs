using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;

namespace Autenticacion
{
    public class Validaciones
    {
        /// <summary>
        /// Valida que los datos ingresados sean los correctos para la autenticación del usuario.
        /// Si son correctos, solicita se actualicen los datos de sesión en base de datos.
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si los datos de autenticación son correctos</returns>
        public static bool Credenciales(string userName, string password, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                unLog.Info("INICIA ObtieneDatosAutenticacionUsuario()");
                Password losDatos = DAOUsuario.ObtieneDatosAutenticacionUsuario(userName, elLog);
                unLog.Info("TERMINA ObtieneDatosAutenticacionUsuario()");

                string ip = string.Empty;
                if (!string.IsNullOrEmpty(losDatos.LocalIP))
                    ip = Hashing.GetClientIp();

                if (Hashing.PasswordOK(password, losDatos.Hash, losDatos.SaltHash, Convert.ToInt32(losDatos.Iteraciones),
                    elLog, ip, losDatos.LocalIP, userName))
                {
                        return true;
                }

                return false;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Realiza todas las validaciones correspondientes a la contraseña ingresada
        /// </summary>
        /// <param name="elPassword">Contraseña</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si se cumplen todas las validaciones</returns>
        public static bool CondicionesPassword(Password elPassword, bool esReset, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            bool esCambio = string.IsNullOrEmpty(elPassword.Actual) ? false : true;

            try
            {
                /////Contraseña/repetición no coinciden
                if (String.Compare(elPassword.Nuevo, elPassword.Repeticion) != 0)
                {
                    throw new CAppException(8012, "La contraseña y su repetición no son iguales.");
                }

                /////La contraseña no cumple con los requisitos de seguridad
                Regex matchEx = new Regex(elPassword.ExpresionRegular);

                Match matchExpression = matchEx.Match(elPassword.Nuevo);

                if (!matchExpression.Success)
                {
                    throw new CAppException(8012, "La contraseña no cumple con los requisitos de seguridad.");
                }

                /////Si es cambio o restablecimiento de contraseña
                if (esReset || esCambio)
                {
                    if (esCambio)
                    {
                        //Sólo si es cambio de contraseña, se valida que la contraseña actual sea la correcta
                        if (!Credenciales(elPassword.NombreUsuario, elPassword.Actual, elLog))
                        {
                            throw new CAppException(8012, "La contraseña actual es inválida.");
                        }
                    }

                    //Se valida que la dirección IP corresponda
                    if (!IpUsuario(elPassword.NombreUsuario, elLog))
                    {
                        throw new CAppException(8012, "Nodo inválido.");
                    }

                    //Verifica el historial de contraseñas del usuario
                    if (ExistePswdEnHistorial(elPassword.NombreUsuario, elPassword.Nuevo, elLog))
                    {
                        throw new CAppException(8012, "La nueva contraseña no debe ser igual a tus últimas " +
                            elPassword.NumMaxHistorial + " contraseñas.");
                    }
                }

                return true;
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Verifica si el password recién ingresado por el usuario, ya existe en su historial de contraseñas
        /// </summary>
        /// <param name="NombreUsuario">Nombre de usuario</param>
        /// <param name="NuevoPassword">Nueva contrasñea</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE en caso de que ya exista</returns>
        public static bool ExistePswdEnHistorial(string NombreUsuario, string NuevoPassword, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                unLog.Info("INICIA ObtieneHistorialUsuario()");
                DataTable elHistorial = DAOUsuario.ObtieneHistorialUsuario(NombreUsuario, elLog);
                unLog.Info("TERMINA ObtieneHistorialUsuario()");

                foreach (DataRow row in elHistorial.Rows)
                {
                    if (Hashing.EsHistoria(NuevoPassword, NombreUsuario, row["Password"].ToString(), 
                            row["PasswordSalt"].ToString(), row["Mixed"].ToString(), elLog))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Establece un nuevo valor hash para un cambio den dirección IP asociado al usuario
        /// </summary>
        /// <param name="enterIP">Nuevo dirección IP ingresada</param>
        /// <param name="newIP">Valor de la nueva dirección IP</param>
        /// <param name="currentSalt">Valor actual del salt del usuario</param>
        /// <param name="salt">Valor del salt del usuario</param>
        /// <param name="currentIter">Valor actual de las iteraciones del usuario</param>
        /// <param name="_iter">Valor de las iteraciones del usuario</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Nuevo valor hash de la dirección IP ingresada</returns>
        public static void NuevaDireccionIP(string enterIP, ref string newIP, string currentSalt, ref string salt,
            string currentIter, ref string _iter, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                string ipAdressHash = string.Empty;
                string IpSaltHash = string.Empty;
                int lasIteraciones = 0;

                Hashing.CreaDireccionIP(enterIP, ref ipAdressHash, currentSalt, ref IpSaltHash,
                    Convert.ToInt32(currentIter), ref lasIteraciones, elLog);

                if (Convert.ToInt32(currentIter) == 0)
                {
                    salt = IpSaltHash;
                    _iter = lasIteraciones.ToString();
                }
                else
                {
                    salt = currentSalt;
                    _iter = currentIter;
                }

                newIP = ipAdressHash;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Verifica si la dirección IP desde donde se está firmando el usuario sea la que se le asignó
        /// </summary>
        /// <param name="NombreUsuario">Nombre de usuario</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE en caso de que ya exista</returns>
        public static bool IpUsuario(string NombreUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                unLog.Info("INICIA ObtieneDatosAutenticacionUsuario()");
                Password losDatos = DAOUsuario.ObtieneDatosAutenticacionUsuario(NombreUsuario, elLog);
                unLog.Info("TERMINA ObtieneDatosAutenticacionUsuario()");

                string ipAddress = string.Empty;
                if (!string.IsNullOrEmpty(losDatos.LocalIP))
                    ipAddress = Hashing.GetClientIp();
                 
                return Hashing.IpOK(ipAddress, losDatos.LocalIP, losDatos.SaltHash, losDatos.Iteraciones, elLog);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        public static bool ValidateIpSecurityParameters(bool isChecked, string text)
        {
            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

            if (!isChecked)
                return true;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            Match matchExpression = ip.Match(text);

            if (!matchExpression.Success)
                return false;
            else
                if (!IPAddress.TryParse(text, out _))
                    return false;

            return true;
        }

        /// <summary>
        /// Valida que los datos ingresados sean los correctos para la autenticación del usuario.
        /// Si son correctos, solicita se actualicen los datos de sesión en base de datos.
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si los datos de autenticación son correctos</returns>
        public static string EstatusSesionUsuario(string userName, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                unLog.Info("INICIA ValidaSesionUsuario()");
                string resp = LNUsuarios.ValidaSesionUsuario(userName, elLog);
                unLog.Info("TERMINA ValidaSesionUsuario()");

                if (resp.Equals("OK"))
                {
                    unLog.Info("INICIA ModificaDatosSesionUsuario()");
                    LNUsuarios.ModificaDatosSesionUsuario(userName, elLog);
                    unLog.Info("TERMINA ModificaDatosSesionUsuario()");
                }

                return resp;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Establece un nuevo valor hash para el token de reset de sesión del usuario
        /// </summary>
        /// <param name="newToken">Referencia a la variable donde se creará el nuevo token</param>
        /// <param name="tokenKey">Referencia a la variable donde se creará el nuevo token</param>
        /// <param name="currentSalt">Valor del salt del usuario</param>
        /// <param name="currentIter">Valor de las iteraciones del usuario</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void NuevoTokenSesion(ref string newToken, ref string tokenKey, string currentSalt,
            string currentIter, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                string simpleToken = string.Empty;
                string tokenHash = string.Empty;

                Hashing.CreaTokenSesion(currentSalt, Convert.ToInt32(currentIter), ref simpleToken, 
                    ref tokenHash, elLog);

                newToken = simpleToken;
                tokenKey = tokenHash;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }
    }

    public enum HashIpValidationsLifeCycle : Int32
    {
        NONE = 0,
        NONE_TO_ACTIVE=1,
        ACTIVE_TO_NONE=2,
        ACTIVE = 3

    }
}
