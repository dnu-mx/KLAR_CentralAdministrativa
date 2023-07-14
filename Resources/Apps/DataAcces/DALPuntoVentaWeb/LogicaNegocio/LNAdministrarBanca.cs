using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para la Administración de Tipo de Cambio
    /// </summary>
    public class LNAdministrarBanca
    {
        /// <summary>
        /// Establece las condiciones de validación para crear un nuevo tipo de cambio en el Autorizador
        /// </summary>
        /// <param name="fecha">Fecha del tipo de cambio</param>
        /// <param name="IdDivisa">Identificador de la divisa</param>
        /// <param name="TipoDeCambio">Valor del tipo de cambio en pesos</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static string CreaNuevoTipoCambio(string fecha, int IdDivisa, string TipoDeCambio, Usuario elUsuario,
            ILogHeader logHeader)
        {
            string resp = "";

            try
            {
                float tipoCambio;
                DateTime _laFecha;

                DateTime.TryParse(fecha, out _laFecha);

                if (_laFecha == DateTime.MinValue)
                {
                    throw new CAppException(8006, "Fecha inválida.");
                }
                else if (_laFecha > DateTime.Today)
                {
                    throw new CAppException(8006, "La fecha máxima es la actual.");
                }

                bool esNumerico = float.TryParse(TipoDeCambio, out tipoCambio);

                if (!esNumerico)
                {
                    throw new CAppException(8006, "El tipo de cambio debe ser un valor numérico.");
                }

                //Se verifica que se haya capturado la fecha
                if (DateTime.Compare(_laFecha, DateTime.MinValue) == 0)
                {
                    throw new CAppException(8006, "La fecha del nuevo tipo de cambio es obligatoria");
                }

                resp = DAOAdministrarBanca.InsertaTipoDeCambio(_laFecha, IdDivisa, tipoCambio, elUsuario, logHeader);

                return resp;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al crear el nuevo tipo de cambio.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar un tipo de cambio en el Autorizador
        /// </summary>
        /// <param name="IdTipoCambio">Identificador del tipo de cambio</param>
        /// <param name="NuevoTipoDeCambio">Nuevo valor del tipo de cambio en pesos</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaTipoCambio(int IdTipoCambio, string NuevoTipoDeCambio, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                float tipoCambio;

                bool esNumerico = float.TryParse(NuevoTipoDeCambio, out tipoCambio);

                if (!esNumerico)
                {
                    throw new CAppException(8006, "El Tipo de Cambio debe ser un valor numérico");
                }

                DAOAdministrarBanca.ActualizaTipoDeCambio(IdTipoCambio, tipoCambio, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el tipo de cambio.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para borrar un tipo de cambio en el Autorizador
        /// </summary>
        /// <param name="IdTipoCambio">Identificador del tipo de cambio</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void BorraTipoCambio(int IdTipoCambio, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarBanca.EliminaTipoDeCambio(IdTipoCambio, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(err);
                throw new CAppException(8011, "Falla al eliminar el tipo de cambio.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear una nueva fecha como día inhábil bancario,
        /// para el país con el ID indicado
        /// </summary>
        /// <param name="fecha">Fecha del tipo de cambio</param>
        /// <param name="IdPais">Identificador de la divisa</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaCorteDiaInhabil(DateTime Fecha, int IdPais, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarBanca.InsertaCorteDiaInhabil(Fecha, IdPais, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al crear el día no bancario.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para borrar la fecha como día inhábil bancario,
        /// para el país con el ID indicado
        /// </summary>
        /// <param name="fecha">Fecha del tipo de cambio</param>
        /// <param name="IdPais">Identificador de la divisa</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void BorraCorteDiaInhabil(DateTime Fecha, int IdPais, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarBanca.EliminaCorteDiaInhabil(Fecha, IdPais, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al eliminar el día no bancario.");
            }
        }
    }
}
