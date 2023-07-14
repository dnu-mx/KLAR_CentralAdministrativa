using System;
using System.Text;

namespace WebServices.Utilerias
{
    public class Cifrado
    {
        /// <summary>
        /// Codifica en Base64 la cadena recibida
        /// </summary>
        /// <param name="plainText">Cadena por codificar</param>
        /// <returns>Cadena codificada a Base64</returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
