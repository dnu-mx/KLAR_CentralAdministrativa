using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Entidades;
using DALCortador.Entidades;
using DALEventos.LogicaNegocio;
using DALPuntoVentaWeb.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Data.SqlClient;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    public class LNCompensaciones
    {
        /// <summary>
        /// Establece las condiciones de validación para registrar una operación como compensada, tanto en los ficheros
        /// T112 como en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="evento">Datos del evento de compensación manual</param>
        /// <param name="operacion">Datos de la operación por compensar (Autorizador)</param>
        /// <param name="IdFicheroDetalle">Identificador del fichero detalle (T112)</param>
        /// <param name="IdEstatusCompensacion">Identificador del estatus a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CompensaOperacionManual_T112(EventoManual evento, Operacion operacion, Int64 IdFicheroDetalle,
            int IdEstatusCompensacion, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn_auto = BDAutorizador.BDEscritura)
                {
                    conn_auto.Open();

                    using (SqlTransaction trxSQL_auto = conn_auto.BeginTransaction())
                    {
                        try
                        {
                            //Ejecuta el evento manual
                            log.Info("INICIA RegistraEventoCompensacionManual()");
                            LNEvento.RegistraEventoCompensacionManual(evento, conn_auto, trxSQL_auto, logHeader);
                            log.Info("TERMINA RegistraEventoCompensacionManual()");

                            //Registra la operación como compensada en el Autorizador
                            log.Info("INICIA ActualizaOperacionCompensada_T112()");
                            DAOCompensaciones.ActualizaOperacionCompensada_T112(operacion, IdFicheroDetalle,
                                conn_auto, trxSQL_auto, elUsuario, logHeader);
                            log.Info("TERMINA ActualizaOperacionCompensada_T112()");

                            /////BDT112
                            using (SqlConnection conn_t112 = BDT112.BDEscritura)
                            {
                                conn_t112.Open();

                                using (SqlTransaction trxSQL_t112 = conn_t112.BeginTransaction())
                                {
                                    try
                                    {
                                        //Registra el nuevo estatus de compensación de la operación en el fichero detalle
                                        log.Info("INICIA ActualizaEstatusCompensacionTX_T112()");
                                        DAOCompensaciones.ActualizaEstatusCompensacionTX_T112(IdFicheroDetalle,
                                                IdEstatusCompensacion, conn_t112, trxSQL_t112, logHeader);
                                        log.Info("TERMINA ActualizaEstatusCompensacionTX_T112()");

                                        trxSQL_t112.Commit();
                                    }
                                    catch (CAppException caEx)
                                    {
                                        trxSQL_t112.Rollback();
                                        throw caEx;
                                    }
                                    catch (Exception ex)
                                    {
                                        trxSQL_t112.Rollback();
                                        log.ErrorException(ex);
                                        throw ex;
                                    }
                                }
                            }

                            trxSQL_auto.Commit();
                        }
                        catch (CAppException caEx)
                        {
                            trxSQL_auto.Rollback();
                            throw caEx;
                        }
                        catch (Exception ex)
                        {
                            trxSQL_auto.Rollback();
                            log.ErrorException(ex);
                            throw ex;
                        }
                    }
                }
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al ejecutar la compensación manual de la operación");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para relacionar una compensación con una operación en el Autorizador
        /// </summary>
        /// <param name="idRegCompensacion">Identificador del registro de compensación</param>
        /// <param name="idOperacion">Identificador dela operación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RelacionaCompensacionConOperacion(long idRegCompensacion, long idOperacion, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOCompensaciones.InsertaRelacionCompensacionOperacion(idRegCompensacion, idOperacion, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al relacionar la compensación con la operación");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para cancelar una relación de compensación sin operación
        /// en el Autorizador
        /// </summary>
        /// <param name="idRelacionCSO">Identificador de la relación de compensación sin operación</param>
        /// <param name="motivo">Motivo de la cancelación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CancelaRelacionCompensacionSinOperacion(long idRelacionCSO, string motivo, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOCompensaciones.ActualizaRelacionCompensacionOperacion(idRelacionCSO, motivo, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al cancelar la relación de compensación sin operación");
            }
        }
    }
}
