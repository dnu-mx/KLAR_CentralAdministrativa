using DALAdministracion.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.LogicaNegocio
{
    public class LNComercioTarjeta
    {
        /// <summary>
        /// Establece las condiciones de validación para crear una nueva colectiva en el Autorizador,
        /// </summary>
        /// <param name="numeroAfiliacion">Numero de afiliacion </param>
        /// <param name="numeroTarjeta">Numero de tarjeta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void CreaNuevoComercioTarjeta (string numeroAfiliacion, string numeroTarjeta, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                DAOComercioTarjeta.GuardarComercioTarjeta(numeroAfiliacion, numeroTarjeta, elUsuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al crear Comercio Tarjeta.");
            }
        }

        /// <summary>
        /// Elimina una relacion de comercio tarjeta
        /// </summary>
        /// <param name="numeroAfiliacion">Numero de afiliacion </param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaComercioTarjeta(string numeroAfiliacion, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                DAOComercioTarjeta.EliminaComercioTarjeta(numeroAfiliacion, elUsuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al eliminar Comercio Tarjeta.");
            }
        }
    }
}
