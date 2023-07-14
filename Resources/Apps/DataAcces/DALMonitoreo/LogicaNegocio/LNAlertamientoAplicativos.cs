using DALCentralAplicaciones.Entidades;
using DALMonitoreo.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;

namespace DALMonitoreo.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para el Alertamiento de Aplicativos
    /// </summary>
    public class LNAlertamientoAplicativos
    {
        /// <summary>
        /// Establece las condiciones de validación para el cierre o inactivación de un alertamiento de aplicativo
        /// </summary>
        /// <param name="idAlertamiento">Identificador del alertamiento</param>
        /// <param name="comentarios">Comentarios del cierre</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CierraAlertamiento(int idAlertamiento, string comentarios, Usuario elUser,
            ILogHeader logHeader)
        {
            try
            {
                DAOAlertamientoAplicativos.DesactivaAlertamiento(idAlertamiento, comentarios, elUser, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al realizar el cierre del alertamiento");
            }
        }
    }
}
