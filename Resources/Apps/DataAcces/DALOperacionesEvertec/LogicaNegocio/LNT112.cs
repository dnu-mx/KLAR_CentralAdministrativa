using DALCentralAplicaciones.Entidades;
using DALOperacionesEvertec.BaseDatos;
using DALOperacionesEvertec.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Data.SqlClient;

namespace DALOperacionesEvertec.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio del nivel de acceso a datos de la BD T112
    /// </summary>
    public class LNT112
    {
        /// <summary>
        /// Establece las condiciones de validación para escribir los datos de control de los archivos MB112
        /// en base de datos
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void EscribeControles_MB(ILogHeader logHeader)
        {
            try
            {
                DAOReportesOpsEvertec.ActualizaIDsFechas_MB(logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al escribir los controles de los ficheros MB");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para escribir los datos de control de los archivos MI112
        /// en base de datos
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void EscribeControles_MI(ILogHeader logHeader)
        {
            try
            {
                DAOReportesOpsEvertec.ActualizaIDsFechas_MI(logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al escribir los controles de los ficheros MI");
            }
        }
    }
}
