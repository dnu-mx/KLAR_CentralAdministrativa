using DALPuntoVentaWeb.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para la escritura de datos hacia base de datos en general
    /// </summary>
    public class LNEscrituraGeneral
    {
        /// <summary>
        /// Establece las condiciones de validación para la modificación del valor referido de las pertetencias
        /// del grupo de medios de acceso indicado
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del grupo de medios de accso</param>
        /// <param name="IdValorReferido">Identificador del valor referido</param>
        /// <param name="Valor">Nuevo valor</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaValorReferidoPertenencia(int IdGrupoMA, int IdValorReferido, string Valor,
            ILogHeader logHeader)
        {
            try
            {
                DAOCVDP.ActualizaValorReferidoPertenencia(IdGrupoMA, IdValorReferido, Valor, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al actualizar el valor referido del Grupo de Tarjetas");
            }
        }
    }
}
