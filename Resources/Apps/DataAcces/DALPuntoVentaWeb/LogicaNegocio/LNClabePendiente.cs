using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Data;
using System.Linq;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    public class LNClabePendiente
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="IdLog">Identificador de la tabla</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static int ActualizaEstatusClabePendiente(string IdLog, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);
            int resp = 0;
            try
            {
                resp = DAOReportes.ActualizaEstatusClabePendiente(IdLog, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al actualizar la clabe pendiente.");
            }
            return resp;
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación de una solicitud
        /// de cuenta CLABE
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="Clabe">Número de cuenta CLABE</param>
        /// <param name="PaginaAspx">Usuario en sesión</param>
        /// <param name="EsAutorizador">Bandera para indicar si el usuario tiene el rol de Autorizador</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaModificaSolicitudCambioCuentaCLABE(int IdProducto, string Tarjeta, string Clabe,
            string PaginaAspx, bool EsAutorizador, Usuario usuario, Guid AppID, ILogHeader logHeader)
        {
            try
            {
                string resp = DAOClabePendiente.InsertaSolicitudCambioCuentaCLABE(IdProducto, Tarjeta, Clabe, 
                    PaginaAspx, EsAutorizador, usuario, AppID, logHeader);

                if (!resp.Contains("OK"))
                {
                    throw new CAppException(8011, resp);
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(err);
                throw new CAppException(8011, "Falla al crear o modificar la solicitus de cuenta CLABE");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación y autorización de la cuenta CLABE
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="IdEjecutorAutorizador">Identificador del registro en la tabla de control</param>
        /// <param name="CLABE">Valor de la nueva cuenta CLABE</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaYAutorizaSolicitudCuentaCLABE(int IdTarjeta, int IdEjecutorAutorizador,
            string CLABE, Usuario elUser, ILogHeader logHeader)
        {
            try
            {
                DAOClabePendiente.ActualizaYAutorizaCuentaCLABE(IdTarjeta, IdEjecutorAutorizador, CLABE,
                    elUser, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar y autorizar la solicitud de cuenta CLABE");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el rechazo de solicitud de la cuenta CLABE
        /// </summary>
        /// <param name="IdEjecutorAutorizador">Identificador del registro en la tabla de control</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RechazaSolicitudCuentaCLABE(int IdEjecutorAutorizador, Usuario elUser, ILogHeader logHeader)
        {
            try
            {
                DAOClabePendiente.RechazaCuentaCLABE(IdEjecutorAutorizador, elUser, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al rechazar la solicitud de cuenta CLABE");
            }
        }
    }
}
