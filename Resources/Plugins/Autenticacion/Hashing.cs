using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Autenticacion
{
    public class Hashing
    {
        #region Constantes

        protected const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#$*?";

        #endregion

        /// <summary>
        /// Genera un valor SALT criptográfico necesario para el PBKDF2
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Valor SALT</returns>
        protected static byte[] GeneraSalt(ILogHeader elLog)
        {
            try
            {
                var csprng = new RNGCryptoServiceProvider();
                var saltBytes = new byte[16];

                csprng.GetBytes(saltBytes);

                return saltBytes;
            }
            catch (Exception ex)
            {
                LogPCI logPCI = new LogPCI(elLog);
                logPCI.Error("GeneraSalt()");
                logPCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Genera un número aleatorio de iteraciones para el PBKDF2
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Número de iteraciones</returns>
        protected static int GeneraIteraciones(ILogHeader elLog)
        {
            try
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buffer = new byte[5];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);

                return new Random(result).Next(1000, 1010);
            }
            catch (Exception ex)
            {
                LogPCI logPCI = new LogPCI(elLog);
                logPCI.Error("GeneraIteraciones()");
                logPCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Crea las claves PBKDF2 para el password del usuario
        /// </summary>
        /// <param name="password">Password tecleado</param>
        /// <param name="pwdKey">Referencia a la variable donde se establece la llave del password</param>
        /// <param name="pwdSalt">Referencia a la variable donde se establece el valor del Password Salt</param>
        /// <param name="iter">Referencia a la variable donde se establece el número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaPasswordUsuario(string password, ref string pwdKey, ref string pwdSalt, ref int iter,
            ref string ipAdr, ILogHeader logHeader, string ip, string user = "")
        {
            try
            {
                byte[] salt = GeneraSalt(logHeader);
                int it = GeneraIteraciones(logHeader);

                var pbkdf2 = new Rfc2898DeriveBytes(password + user + ip, salt, it);
                byte[] key = pbkdf2.GetBytes(32);

                if (!string.IsNullOrEmpty(ip))
                {
                    var ip_bkdf = new Rfc2898DeriveBytes(ip, salt, it);
                    byte[] ip_key = ip_bkdf.GetBytes(32);

                    ipAdr = Convert.ToBase64String(ip_key);
                }

                pwdKey = Convert.ToBase64String(key);
                pwdSalt = Convert.ToBase64String(salt);
                iter = it;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al generar el valor hash");
            }
        }

        /// <summary>
        /// Realiza la comprobación de las claves hash del password
        /// </summary>
        /// <param name="enterPassword">Password tecleado</param>
        /// <param name="password">Clave contra la que se compara el password</param>
        /// <param name="salt">Valor del Password Salt</param>
        /// <param name="iterations">Número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <param name="ip">Dirección IP local</param>
        /// <param name="ipHashed">Dirección IP con la que se creó la clave</param>
        /// <param name="user">Nombre de usuario</param>
        /// <returns>TRUE si los password coinciden</returns>
        public static bool PasswordOK(string enterPassword, string password, string salt, int iterations,
            ILogHeader logHeader, string ip, string ipHashed, string user = "")
        {
            try
            {
                byte[] byteSalt = Convert.FromBase64String(salt);

                if (!string.IsNullOrEmpty(ip))
                {
                    var pbkdf2_IP = new Rfc2898DeriveBytes(ip, byteSalt, iterations);
                    byte[] byte_IP = pbkdf2_IP.GetBytes(32);

                    if (Convert.ToBase64String(byte_IP) != ipHashed)
                        return false;
                }

                var pbkdf2_EP = new Rfc2898DeriveBytes(enterPassword + user + ip, byteSalt, iterations);
                byte[] byte_EP = pbkdf2_EP.GetBytes(32);

                if (Convert.ToBase64String(byte_EP) == password)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.Error("PasswordOK()");
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Verifica si el password tecleado ya forma parte del valor que está en historial de contraseñas
        /// </summary>
        /// <param name="password">Password tecleado</param>
        /// <param name="userN">User ID o nombre del usuario</param>
        /// <param name="key">Llave del password</param>
        /// <param name="salt">Valor del Password Salt</param>
        /// <param name="iter">Número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si el password tecleado ya es parte del historial</returns>
        public static bool EsHistoria(string password, string userN, string key, string salt, string iter,
            ILogHeader logHeader)
        {
            try
            {
                byte[] h_salt = Convert.FromBase64String(salt);
                int h_it = int.Parse(iter);

                var h_pbkdf2 = new Rfc2898DeriveBytes(password + userN, h_salt, h_it);
                byte[] h_key = h_pbkdf2.GetBytes(32);

                if (Convert.ToBase64String(h_key) == key)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.Error("EsHistoria()");
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        public static string GetClientIp(bool validateHashIpSecurity = true)
        {

            string responseIP = string.Empty;

            responseIP = HttpContext.Current.Request.UserHostAddress;
            if (responseIP == "::1") return "127.0.0.1";


            return responseIP;
        }

        /// <summary>
        /// Crea las claves PBKDF2 para el historial de contraseñas del usuario
        /// </summary>
        /// <param name="password">Password tecleado</param>
        /// <param name="userN">User ID o nombre del usuario</param>
        /// <param name="pwdKey">Referencia a la variable donde se establece la llave del password</param>
        /// <param name="pwdSalt">Referencia a la variable donde se establece el valor del Password Salt</param>
        /// <param name="iter">Referencia a la variable donde se establece el número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaPasswordHistorial(string password, string userN, ref string pwdKey, ref string pwdSalt,
            ref int iter, ILogHeader logHeader)
        {
            try
            {
                byte[] h_salt = GeneraSalt(logHeader);
                int h_it = GeneraIteraciones(logHeader);

                var h_pbkdf2 = new Rfc2898DeriveBytes(password + userN, h_salt, h_it);
                byte[] h_key = h_pbkdf2.GetBytes(32);

                pwdKey = Convert.ToBase64String(h_key);
                pwdSalt = Convert.ToBase64String(h_salt);
                iter = h_it;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al generar el valor historial hash");
            }
        }

        /// <summary>
        /// Crea el valor PBKDF2 para la nueva dirección IP del usuario
        /// </summary>
        /// <param name="ip">Dirección IP local</param>
        /// <param name="ipAdr">Nueva dirección IP establecida en la edición del usuario</param>
        /// <param name="ipSalt">Valor del salt actual del usuario</param>
        /// <param name="newIpSalt">Valor del nuevo salt del usuario</param>
        /// <param name="iter">Valor de las iteraciones actuales del usuario</param>
        /// <param name="newIter">Valor de las nuevas iteraciones del usuario</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaDireccionIP(string ip, ref string ipAdr, string ipSalt, ref string newIpSalt, int iter,
            ref int newIter, ILogHeader logHeader)
        {
            try
            {
                byte[] salt = iter == 0 ? GeneraSalt(logHeader) : Encoding.ASCII.GetBytes(ipSalt);
                int it = iter == 0 ? GeneraIteraciones(logHeader) : iter;

                var ip_bkdf = new Rfc2898DeriveBytes(ip, salt, it);
                byte[] ip_key = ip_bkdf.GetBytes(32);

                ipAdr = Convert.ToBase64String(ip_key);
                newIpSalt = Convert.ToBase64String(salt);
                newIter = it;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al generar el valor hash de la Dirección IP");
            }
        }

        /// <summary>
        /// Realiza la comprobación de las claves hash de la dirección IP
        /// </summary>
        /// <param name="ip">Dirección IP del usuario</param>
        /// <param name="ipKey">Clave contra la que se compara la dirección IP</param>
        /// <param name="salt">Valor salt de la dirección IP</param>
        /// <param name="iter">Número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si la comprobación es correcta</returns>
        public static bool IpOK(string ip, string ipKey, string salt, string iter, ILogHeader logHeader)
        {
            try
            {
                byte[] origSalt = Convert.FromBase64String(salt);
                int iterations = int.Parse(iter);

                if (!string.IsNullOrEmpty(ip))
                {
                    var pbkdf2_IP = new Rfc2898DeriveBytes(ip, origSalt, iterations);
                    byte[] byte_IP = pbkdf2_IP.GetBytes(32);

                    if (Convert.ToBase64String(byte_IP) != ipKey)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.Error("IpOK()");
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Genera un número aleatorio para token
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Número aleatorio </returns>
        protected static int GeneraToken(ILogHeader elLog)
        {
            try
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buffer = new byte[9];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);

                return new Random(result).Next(10000000, 99999999);
            }
            catch (Exception ex)
            {
                LogPCI logPCI = new LogPCI(elLog);
                logPCI.Error("GeneraToken()");
                logPCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Crea un token PBKDF2 para restablecer sesiones del usuario
        /// </summary>
        /// <param name="userSalt">Salt del usuario</param>
        /// <param name="iter">Número de iteraciones</param>
        /// <param name="token">Referencia a la variable donde se establece el token</param>
        /// <param name="tokenKey">Referencia a la variable donde se establece el token keys</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaTokenSesion(string userSalt, int iter, ref string token, ref string tokenKey,
            ILogHeader logHeader)
        {
            try
            {
                string tkn = GeneraToken(logHeader).ToString();
                byte[] b_Salt = Convert.FromBase64String(userSalt);

                var tknPbkdf2 = new Rfc2898DeriveBytes(tkn, b_Salt, iter);
                byte[] tKey = tknPbkdf2.GetBytes(32);

                token = tkn;
                tokenKey = Convert.ToBase64String(tKey);
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al generar el valor hash");
            }
        }

        /// <summary>
        /// Realiza la comprobación de las claves hash de los tokens
        /// </summary>
        /// <param name="capturedToken">Token tecleado</param>
        /// <param name="token">Clave contra la que se compara el token</param>
        /// <param name="salt">Valor del Password Salt</param>
        /// <param name="iterations">Número de iteraciones</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>TRUE si los tokens coinciden</returns>
        public static bool TokenOK(string capturedToken, string token, string salt, int iterations, ILogHeader logHeader)
        {
            try
            {
                byte[] byteSalt = Convert.FromBase64String(salt);

                var tkn_PBKDF2 = new Rfc2898DeriveBytes(capturedToken, byteSalt, iterations);
                byte[] byte_TKN = tkn_PBKDF2.GetBytes(32);

                if (Convert.ToBase64String(byte_TKN) == token)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.Error("TokenOK()");
                unLog.ErrorException(ex);
                throw ex;
            }
        }
    }
}
