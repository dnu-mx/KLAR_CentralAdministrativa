using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCortador.Entidades;
using DALEventos.BaseDatos;
using DALEventos.Entidades;
using Executer.Entidades;
using Interfases;
using Interfases.Entidades;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DALEventos.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para un Evento
    /// </summary>
    public class LNEvento
    {
        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación
        /// de un evento
        /// </summary>
        /// <param name="ev">Datos de la entidad Evento por crear o modificar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaEvento(EventoConfigurador ev, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEvento.InsertaActualizaEvento(ev, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar o Insertar Evento en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación
        /// de un evento automático agrupado
        /// </summary>
        /// <param name="idCfgCorte">Identificador de la configuración del corte</param>
        /// <param name="idEv">Identificador del evento</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaEventoAgrupado(int idCfgCorte, int idEv, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEvento.InsertaActualizaEventoAgrupado(idCfgCorte, idEv, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar o Insertar Evento en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación
        /// de un evento transaccional
        /// </summary>
        /// <param name="idEvAgrupado">Identificador del evento agrupado</param>
        /// <param name="idCfgCorte">Identificador de la configuración del corte</param>
        /// <param name="idEv">Identificador del evento</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaEventoTX(int idEvAgrupado, int idCfgCorte, int idEv, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEvento.InsertaActualizaEventoTX(idEvAgrupado, idCfgCorte, idEv, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar o Insertar Evento en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la inserción del corte asociado a la cadena comercial
        /// o a la modificación de su periodicidad
        /// </summary>
        /// <param name="idCorteAsignado">Identificador del corte asignado</param>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="idCfgCorte">Identificador de la configuración de corte</param>
        /// <param name="idPeriodo">Identificador del periodo</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaPeriodoCorteCadena(int idCorteAsignado, int idCadena, int idCfgCorte, int idPeriodo, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEvento.InsertaActualizaPeriodoCorteCadena(idCorteAsignado, idCadena, idCfgCorte, idPeriodo, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar o Insertar Evento en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la inserción o modificación
        /// del corte asociado a la cadena comercial
        /// </summary>
        /// <param name="idCorteAsignado">Identificador del corte asignado</param>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="idCfgCorte">Identificador de la configuración de corte</param>
        /// <param name="idPeriodo">Identificador del periodo</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaCorteCadena(int idCorteAsignado, int idCadena, int idCfgCorte, int idPeriodo, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEvento.InsertaActualizaCorteCadena(idCorteAsignado, idCadena, idCfgCorte, idPeriodo, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar o Insertar Evento en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del registro del evento manual
        /// "Recepción de Efectivo Diconsa"
        /// </summary>
        /// <param name="datosEfectivo">Datos del evento Efectivo</param>
        public static void RegistraEvManual_RecepEfectDiconsa(EfectivoDiconsa datosEfectivo, ILogHeader elLog)
        {
            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {

                    try
                    {
                        Poliza laPoliza = null;

                        Dictionary<String, Parametro> losParametros = new Dictionary<string,Parametro>();

                        losParametros["@ColectivaOrigen"] = new Parametro() { Nombre = "@ColectivaOrigen", 
                            Valor = datosEfectivo.IdColectivaOrigen.ToString(), Descripcion = "Colectiva Origen",
                            ID_TipoColectiva = datosEfectivo.IdTipoColectivaOrigen};
                        losParametros["@ColectivaDestino"] = new Parametro() { Nombre = "@ColectivaDestino",
                            Valor = datosEfectivo.IdColectivaDestino.ToString(), Descripcion = "Colectiva Destino",
                            ID_TipoColectiva = datosEfectivo.IdTipoColectivaDestino };
                        losParametros["@Importe"] = new Parametro() { Nombre = "@Importe", 
                            Valor = datosEfectivo.Importe, Descripcion = "Importe" };
                        losParametros["@ID_CadenaComercial"] = new Parametro() { Nombre = "@ID_CadenaComercial", 
                            Valor = "0", Descripcion = "ID_CadenaComercial" };

                        //Genera y Aplica la Poliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(datosEfectivo.IdEvento, 
                            datosEfectivo.Concepto, false, datosEfectivo.Referencia, losParametros, 
                            datosEfectivo.Observaciones, conn, transaccionSQL, elLog);
                        laPoliza = aplicador.AplicaContablilidad(elLog);

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            throw new Exception("No se generó la Póliza: " + laPoliza.DescripcionRespuesta);
                        }
                        else
                        {
                            transaccionSQL.Commit();
                        }
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        Utilidades.Loguear.Error(err, "");
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del registro del evento manual
        /// "Depósito de Recolector Diconsa"
        /// </summary>
        /// <param name="elDeposito">Datos del depósito</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraEvManual_DepRecolectorDiconsa(EventoDepositoRecolector elDeposito, Usuario usuario,
            ILogHeader elLog)
        {
            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                        //Se consultan los parámetros del contrato
                        losParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                            (elDeposito.ClaveColectiva, "", elDeposito.ClaveEvento, elLog);

                        losParametros["@ColectivaOrigen"] = new Parametro() { Nombre = "@ColectivaOrigen", 
                            Valor = elDeposito.IdColectivaOrigen.ToString(), Descripcion = "Colectiva Origen",
                            ID_TipoColectiva = elDeposito.IdTipoColectivaOrigen};
                        losParametros["@Importe"] = new Parametro() { Nombre = "@Importe", 
                            Valor = elDeposito.Importe, Descripcion = "Importe" };

                        //Genera y Aplica la Poliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(elDeposito.IdEvento,
                            elDeposito.Concepto, false, elDeposito.Referencia, losParametros, elDeposito.Observaciones,
                            conn, transaccionSQL, elLog);
                        Poliza laPoliza = aplicador.AplicaContablilidad(elLog);

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            throw new Exception("No se generó la Póliza: " + laPoliza.DescripcionRespuesta);
                        }

                        else
                        {
                            transaccionSQL.Commit();
                        }
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        Utilidades.Loguear.Error(err, "");
                        throw err;
                    }
                }
            }
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="elEvento"></param>
       /// <param name="usuario"></param>
       /// <param name="elLog"></param>
        public static void RegistraEvManual_AjusteSaldoCupon(EvManAjusteSaldoCupon elEvento, Usuario usuario,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                        //Se consultan los parámetros del contrato
                        losParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                            (elEvento.ClaveColectivaCCM, "", elEvento.ClaveEvento, elLog);

                        losParametros["@Importe"] = new Parametro()
                        {
                            Nombre = "@Importe",
                            Valor = elEvento.Importe,
                            Descripcion = "Importe"
                        };
                        losParametros["@ID_CadenaComercial"] = new Parametro()
                        {
                            Nombre = "@ID_CadenaComercial",
                            Valor = elEvento.IdColectiva.ToString(),
                            Descripcion = "ID Cadena Comercial",
                            ID_TipoColectiva = elEvento.IdTipoColectiva
                        };
                        losParametros["@MedioAcceso"] = new Parametro()
                        {
                            Nombre = "@MedioAcceso",
                            Valor = elEvento.MedioAcceso,
                            Descripcion = "Medio de Acceso"
                        };
                        losParametros["@TipoMedioAcceso"] = new Parametro()
                        {
                            Nombre = "@TipoMedioAcceso"
                        };


                        //Genera y Aplica la Poliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(elEvento.IdEvento,
                            elEvento.Concepto, false, 0, losParametros, elEvento.Observaciones,
                            conn, transaccionSQL, elLog);
                        Poliza laPoliza = aplicador.AplicaContablilidad(elLog);

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                                laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                            unLog.Warn(msg);
                            throw new CAppException(8011, msg);
                        }

                        else
                        {
                            transaccionSQL.Commit();
                        }
                    }

                    catch (CAppException caEx)
                    {
                        throw caEx;
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        unLog.ErrorException(err);
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el control de movimientos al saldo de una cuenta eje
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoSaldo">Nuevo saldo de la cuenta</param>
        /// <param name="Importe">Importe por aumentar o decrementar al saldo de la cuenta</param>
        /// <param name="IdMovim">Identificador del tipo de movimiento al saldo (aumento o decremento)</param>
        /// <param name="Observ">Observaciones para el evento manual</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void NuevoMovimientoCuentaEje(int IdCuenta, String NuevoSaldo, String Importe, int IdMovim,
            String Observ, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                DAOEvento.InsertaMovimientoSaldoCuentaEje(IdCuenta, NuevoSaldo, Importe, IdMovim,
                    Observ, elUsuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                logPCI.ErrorException(err);
                throw new CAppException(8011, "Falla al registrar la captura de movimiento de saldo de la Cuenta Eje.");
            }
        }

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del evento manual Fondeo Cliente
        /// "Depósito de Recolector Diconsa"
        /// </summary>
        /// <param name="elEvento">Datos del evento de fondeo</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraEventoAjusteSaldoCliente(EventoCargoAbonoCuentaEjeCacao elEvento, Usuario usuario,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                        //Se consultan los parámetros del contrato
                        unLog.Info("INICIA Executer_ListaParametrosDeContrato()");
                        losParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                            (elEvento.ClaveColectiva, "", elEvento.ClaveEvento, elLog);
                        unLog.Info("TERMINA Executer_ListaParametrosDeContrato()");

                        losParametros["@ID_GrupoComercial"].Valor = elEvento.IdGrupoComercial.ToString();
                        losParametros["@ID_CuentaHabiente"].Valor = elEvento.IdCuentahabiente.ToString();
                        losParametros["@Importe"] = new Parametro()
                        {
                            Nombre = "@Importe",
                            Valor = elEvento.Importe,
                            Descripcion = "Importe"
                        };

                        //Genera y Aplica la Poliza
                        unLog.Info("INICIA new Executer.EventoManual()");
                        Executer.EventoManual aplicador = new Executer.EventoManual(elEvento.IdEvento,
                            elEvento.Concepto, false, 0, losParametros, elEvento.Observaciones,
                            conn, transaccionSQL, elLog);
                        unLog.Info("TERMINA new Executer.EventoManual()");

                        unLog.Info("INICIA Executer.AplicaContablilidad()");
                        Poliza laPoliza = aplicador.AplicaContablilidad(elLog);
                        unLog.Info("TERMINA Executer.AplicaContablilidad()");

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                                laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                            unLog.Warn(msg);
                            throw new CAppException(8011, msg);
                        }

                        else
                        {
                            transaccionSQL.Commit();
                        }
                    }

                    catch (CAppException caEx)
                    {
                        throw caEx;
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        unLog.ErrorException(err);
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación del acceso a datos para registrar
        /// el nuevo saldo de la cuenta indicada
        /// </summary>
        /// <param name="IdMovCuentaEje">Identificador del registro en la tabla de movimientos</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoSaldo">Nuevo saldo de la cuenta</param>
        /// <param name="UsuarioEjecutor">Usuario que ejecutó el movimiento</param>
        /// <param name="usuario">Usuario en sesión (Autorizador)</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraNuevoSaldoCuentaEje(int IdMovCuentaEje, Int64 IdCuenta, string NuevoSaldo,
            string UsuarioEjecutor, Usuario usuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                log.Info("INICIA RegistraAjusteSaldoCuentaEje()");
                DAOEvento.RegistraAjusteSaldoCuentaEje(IdMovCuentaEje, IdCuenta, NuevoSaldo,
                    UsuarioEjecutor, usuario, elLog);
                log.Info("TERMINA RegistraAjusteSaldoCuentaEje()");
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar el nuevo saldo de la cuenta eje.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el rechazo al ajuste de saldo de una cuenta eje
        /// </summary>
        /// <param name="IdMovCuentaEje">Identificador del registro en la tabla de movimientos</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RechazaCambioSaldoCuentaEje(int IdMovCuentaEje, Usuario usuario, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                unLog.Info("INICIA RegistraRechazoAjusteSaldoCuentaEje()");
                DAOEvento.RegistraRechazoAjusteSaldoCuentaEje(IdMovCuentaEje, usuario, logHeader);
                unLog.Info("TERMINA RegistraRechazoAjusteSaldoCuentaEje()");
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                unLog.ErrorException(err);
                throw new CAppException(8011, "Falla al registrar el rechazo al ajuste de saldo de la Cuenta Eje");
            }
        }

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del evento manual
        /// traspaso entre cuentas del mismo tipo
        /// </summary>
        /// <param name="elEvento">Datos del evento de fondeo</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraEventoTraspasoCuentasMismoTipo(EventoManual elEvento, Usuario usuario, ILogHeader elLog)
        {
            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                        losParametros["@ColectivaOrigen"] = new Parametro()
                        {
                            Nombre = "@ColectivaOrigen",
                            Valor = elEvento.IdColectivaOrigen.ToString(),
                            Descripcion = "Colectiva Origen",
                            ID_TipoColectiva = elEvento.IdTipoColectivaOrigen
                        };
                        losParametros["@ColectivaDestino"] = new Parametro()
                        {
                            Nombre = "@ColectivaDestino",
                            Valor = elEvento.IdColectivaDestino.ToString(),
                            Descripcion = "Colectiva Destino",
                            ID_TipoColectiva = elEvento.IdTipoColectivaDestino
                        };
                        losParametros["@Importe"] = new Parametro()
                        {
                            Nombre = "@Importe",
                            Valor = elEvento.Importe,
                            Descripcion = "Importe"
                        };
                        losParametros["@ID_CadenaComercial"] = new Parametro()
                        {
                            Nombre = "@ID_CadenaComercial",
                            Valor = "0",
                            Descripcion = "ID_CadenaComercial"
                        };

                        //Genera y Aplica la Poliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(elEvento.IdEvento,
                            elEvento.Concepto, false, 0, losParametros, elEvento.Observaciones,
                            conn, transaccionSQL, elLog);
                        Poliza laPoliza = aplicador.AplicaContablilidad(elLog);

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            //transaccionSQL.Rollback();
                            throw new Exception("No se generó la Póliza: " + laPoliza.DescripcionRespuesta);
                        }

                        else
                        {
                            transaccionSQL.Commit();
                        }
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        Utilidades.Loguear.Error(err, "");
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación de un evento manual
        /// en el Autorizador.
        /// </summary>
        /// <param name="ClaveEvento">Clave del nuevo evento manual</param>
        /// <param name="DescripcionInterna">Descripción del nuevo evento manual</param>
        /// <param name="DescripcionEdoCta">Descripción para el estado de cuenta del nuevo evento manual</param>
        /// <param name="IdTipoMovimiento">Identificador del tipo al que corresponde el nuevo evento
        /// 1 = Fondeo, 2 = Retiro </param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Identificador del nuevo evento manual</returns>
        public static DataTable CreaEventoManual(string ClaveEvento, string DescripcionInterna, string DescripcionEdoCta,
            int IdTipoMovimiento, Usuario usuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            
            try
            {
                DataTable dt = DAOEvento.InsertaNuevoEventoManual(ClaveEvento, DescripcionInterna,
                    DescripcionEdoCta, IdTipoMovimiento, usuario, elLog);

                return dt;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar el nuevo evento manual.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del estatus de un
        /// evento manual en el Autorizador
        /// </summary>
        /// <param name="IdEvento">Identificador del evento manual</param>
        /// <param name="Estatus">Valor del nuevo estatus (activo o inactivo)</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusEventoManual(int IdEvento, int Estatus, Usuario usuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                DAOEvento.ActualizaEstatusEventoManual(IdEvento, Estatus, usuario, elLog);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus del evento manual.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación de las descripciones de un
        /// evento manual en el Autorizador
        /// </summary>
        /// <param name="IdEvento">Identificador del evento manual</param>
        /// <param name="DescripcionInterna">Descripción del nuevo evento manual</param>
        /// <param name="DescripcionEdoCta">Descripción para el estado de cuenta del nuevo evento manual</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaDescripcionesEventoManual(int IdEvento, string DescripcionInterna,
            string DescripcionEdoCta, Usuario usuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                DAOEvento.ActualizaDescripcionesEventoManual(IdEvento, DescripcionInterna,
                    DescripcionEdoCta, usuario, elLog);
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar las descripciones del evento");
            }
        }

        /// <summary>
        /// Controla las validaciones y solicita el evento manual de Ajuste de Límte de Crédito
        /// </summary>
        /// <param name="elEvento">Entidad con los datos del evento</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraEvManual_AjustaLimiteCredito(EventoManual elEvento, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario usuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                Poliza laPoliza = null;

                Dictionary<String, Parametro> TodosLosParametros = new Dictionary<string, Parametro>();

                //se consultan los parámetros del contrato
                log.Info("INICIA ListaParametrosDeContrato()");
                TodosLosParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                    ("CCM", elEvento.MedioAcceso, elEvento.ClaveEvento, elLog);
                log.Info("TERMINA ListaParametrosDeContrato()");

                //Se añade el parámetro de importe
                TodosLosParametros["@Importe"] = new Parametro() { Nombre = "@Importe", Valor = elEvento.Importe, Descripcion = "Importe" };
                TodosLosParametros["@Saldo_CLDC"] = new Parametro() { Nombre = "@Saldo_CLDC", Valor = elEvento.SaldoCuentaCLDC, Descripcion = "Saldo CLDC" };

                //Se genera y aplica la póliza
                log.Info("INICIA new Executer.EventoManual()");
                Executer.EventoManual aplicador = new Executer.EventoManual(Convert.ToInt32(TodosLosParametros["@ID_Evento"].Valor),
                    TodosLosParametros["@DescEvento"].Valor, false, elEvento.Referencia, TodosLosParametros,
                    elEvento.Observaciones, connection, transaccionSQL, elLog);
                log.Info("TERMINA new Executer.EventoManual()");

                log.Info("INICIA Executer.AplicaContablilidad()");
                laPoliza = aplicador.AplicaContablilidad(elLog);
                log.Info("TERMINA Executer.AplicaContablilidad()");

                if (laPoliza.CodigoRespuesta != 0)
                {
                    string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                        laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                    log.Warn(msg);
                    throw new CAppException(8011, msg);
                }
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
        /// Establece y configura los parámetros para la ejecución de los eventos de
        /// autorización o rechazo del abono  por fondeo rápido al cliente
        /// </summary>
        /// <param name="elEvento">Datos del evento de fondeo rápido</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraEventoFondeoRapido(EventoManual elEvento, Usuario usuario, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                            //Se consultan los parámetros del contrato
                            unLog.Info("INICIA ListaParametrosDeContrato()");
                            losParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                                (elEvento.ClaveColectiva, "", elEvento.ClaveEvento, logHeader);
                            unLog.Info("TERMINA ListaParametrosDeContrato()");

                            losParametros["@ID_CuentaHabiente"].Valor = elEvento.IdColectivaDestino.ToString();
                            losParametros["@Importe"] = new Parametro()
                            {
                                Nombre = "@Importe",
                                Valor = elEvento.Importe,
                                Descripcion = "Importe"
                            };

                            //Genera y Aplica la Poliza
                            unLog.Info("INICIA new Executer.EventoManual()");
                            Executer.EventoManual aplicador = new Executer.EventoManual(
                                Convert.ToInt32(losParametros["@ID_Evento"].Valor), losParametros["@DescEvento"].Valor,
                                false, 0, losParametros, elEvento.Observaciones, conn, transaccionSQL, logHeader);
                            unLog.Info("TERMINA new Executer.EventoManual()");

                            unLog.Info("INICIA Executer.AplicaContablilidad()");
                            Poliza laPoliza = aplicador.AplicaContablilidad(logHeader);
                            unLog.Info("TERMINA Executer.AplicaContablilidad()");

                            if (laPoliza.CodigoRespuesta != 0)
                            {
                                transaccionSQL.Rollback();
                                string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                                    laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                                unLog.Warn(msg);
                                throw new CAppException(8011, msg);
                            }

                            else
                            {
                                transaccionSQL.Commit();
                            }
                        }

                        catch (CAppException caEx)
                        {
                            throw caEx;
                        }

                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            unLog.ErrorException(err);
                            throw err;
                        }
                    }
                }
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception err)
            {
                unLog.ErrorException(err);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación del acceso a datos para registrar en bitácora
        /// el abono de fondeo rápido a la cuenta indicada
        /// </summary>
        /// <param name="IdMovFondeo">Identificador del registro en la tabla de movimientos</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoSaldo">Nuevo saldo de la cuenta</param>
        /// <param name="UsuarioEjecutor">Usuario que ejecutó el movimiento</param>
        /// <param name="usuario">Usuario en sesión (Autorizador)</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraNuevoSaldoFondeoRapido(int IdMovFondeo, Int64 IdCuenta, string NuevoSaldo,
            string UsuarioEjecutor, Usuario usuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                DAOEvento.RegistraAbonoFondeoRapido(IdMovFondeo, IdCuenta, NuevoSaldo,
                    UsuarioEjecutor, usuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al aceptar el fondeo rápido");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para registrar el rechazo a un fondeo 
        /// rápido de una cuenta
        /// </summary>
        /// <param name="IdMovCuentaEje">Identificador del registro en la tabla de movimientos</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RechazaFondeoRapido(int IdMovFondeoRapido, Usuario usuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                DAOEvento.RegistraRechazoFondeoRapido(IdMovFondeoRapido, usuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar el rechazo al fondeo rápido");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para registrar la compensación manual entre operaciones
        /// del Autorizador y de los archivos T112
        /// </summary>
        /// <param name="elEvento">Datos del evento manual</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraEventoCompensacionManual(EventoManual elEvento, SqlConnection connection,
            SqlTransaction transaccionSQL, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                //Se consultan los parámetros del contrato
                unLog.Info("INICIA ListaParametrosDeContrato()");
                losParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                    (elEvento.ClaveColectiva, "", elEvento.ClaveEvento, logHeader);
                unLog.Info("TERMINA ListaParametrosDeContrato()");

                losParametros["@ID_Emisor"].Valor = elEvento.IdColectivaOrigen.ToString();
                losParametros["@ID_PadreCuentaHabiente"].Valor = elEvento.IdColectivaDestino.ToString();
                losParametros["@Importe"] = new Parametro()
                {
                    Nombre = "@Importe",
                    Valor = elEvento.Importe,
                    Descripcion = "Importe"
                };

                //Genera y Aplica la Poliza
                unLog.Info("INICIA new Executer.EventoManual()");
                Executer.EventoManual aplicador = new Executer.EventoManual(
                    Convert.ToInt32(losParametros["@ID_Evento"].Valor), losParametros["@DescEvento"].Valor,
                    false, 0, losParametros, elEvento.Observaciones, connection, transaccionSQL, logHeader);
                unLog.Info("TERMINA new Executer.EventoManual()");

                unLog.Info("INICIA Executer.AplicaContablilidad()");
                Poliza laPoliza = aplicador.AplicaContablilidad(logHeader);
                unLog.Info("TERMINA Executer.AplicaContablilidad()");

                if (laPoliza.CodigoRespuesta != 0)
                {
                    string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                        laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                    unLog.Warn(msg);
                    throw new CAppException(8011, msg);
                }
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception err)
            {
                unLog.ErrorException(err);
                throw err;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID_Evento"></param>
        /// <param name="ID_Contrato"></param>
        /// <param name="ConceptoPoliza"></param>
        /// <param name="losParametrosManuales"></param>
        /// <param name="elUsuario"></param>
        /// <param name="AppID"></param>
        /// <param name="Observaciones"></param>
        /// <param name="RefNum"></param>
        /// <param name="elLog"></param>
        /// <returns></returns>
        public static int Ejecutar(int ID_Evento, int ID_Contrato, string ConceptoPoliza, Dictionary<string, Parametro> losParametrosManuales,
            Usuario elUsuario, Guid AppID, string Observaciones, long RefNum, ILogHeader elLog)
        {
            int Resp = -1;
            LogPCI log = new LogPCI(elLog);

            //obtener los Parametros de los contratos.
            log.Info("INICIA AgregarParametrosContrato()");
            DAOValores.AgregarParametrosContrato(ID_Contrato, ref losParametrosManuales, elUsuario, AppID, elLog);
            log.Info("TERMINA AgregarParametrosContrato()");

            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        //Genera y Aplica la Poliza
                        log.Info("INICIA new Executer.EventoManual()");
                        Executer.EventoManual aplicador = new Executer.EventoManual(ID_Evento,
                            ConceptoPoliza, false, RefNum, losParametrosManuales, Observaciones,
                             conn, transaccionSQL, elLog);
                        log.Info("TERMINA new Executer.EventoManual()");

                        log.Info("INICIA Executer.AplicaContablilidad()");
                        Poliza laPoliza = aplicador.AplicaContablilidad(elLog);
                        log.Info("TERMINA Executer.AplicaContablilidad()");

                        Resp = laPoliza.CodigoRespuesta;

                        //Guardar el Lote
                        if (Resp != 0)
                        {
                            transaccionSQL.Rollback();
                            string msg = "No se generó la póliza del Evento seleccionado. CodigoRespuesta: " +
                                Resp + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                            log.Warn(msg);
                            throw new CAppException(8011, msg);
                        }
                        else
                        {
                            InsertaRegistroBitacoraDetalle("pantalla_EjecutarEventoManual_AplicaEvento", "Poliza", "ID_Poliza", laPoliza.ID_Poliza.ToString(), 
                                String.Format("{0:C}", laPoliza.Importe), "Aplicación Evento: '" + laPoliza.Concepto + "'", 
                                conn, transaccionSQL, elUsuario, elLog);

                            transaccionSQL.Commit();
                        }
                    }
                    catch (CAppException CaEx)
                    {
                        throw CaEx;
                    }
                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        log.ErrorException(err);

                        if (Resp == 0)
                            Resp = (int)Interfases.Enums.CodRespuesta03.NO_SE_GENERO_POLIZA;
                    }

                    return Resp;
                }
            }
        }

        /// <summary>
        /// Inserta registro de bitacora detalle en base de datos
        /// </summary>
        /// <param name="SP">Nombre del procedimiento almacenado que realizó la inserción/actualización</param>
        /// <param name="tabla">Nombre de la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="campo">Nombre del campo en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="idRegistro">Identificador del registro en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="valor">Valor del campo en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaRegistroBitacoraDetalle(string SP, string tabla, string campo, string idRegistro, 
            string valor, string observaciones, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario, ILogHeader elLog)
        {
            try
            {
                DAOEvento.InsertaRegistroBitacora(SP, tabla, campo, idRegistro, valor, observaciones, connection,
                    transaccionSQL, elUsuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI pCI = new LogPCI(elLog);
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al insertar registros en bitacora");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para verificar si los permisos del usuario en sesión
        /// son suficientes para ejecutar eventos manuales al tarjetahabiente
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos de respuesta</returns>
        public static DataTable VerificaUsuarioSubemisor(string tarjeta, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            try
            {
                return DAOEvento.ValidaPermisosASubemisor(tarjeta, elUsuario, AppID, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI pCI = new LogPCI(elLog);
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al verificar permisos del usuario");
            }
        }

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del evento manual de tipo tarjetahabiente
        /// </summary>
        /// <param name="elEvento">Datos del evento manual a ejecutar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraEvManual_Tarjetahabiente(EventoManual elEvento, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            using (SqlConnection conn = BDAutorizador.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        Dictionary<string, Parametro> losParametros = new Dictionary<string, Parametro>();

                        unLog.Info("INICIA ListaParametrosSubemisor()");
                        losParametros = Executer.BaseDatos.DAOEvento.ListaParametrosSubemisor
                            (elEvento.ClaveColectiva, elEvento.MedioAcceso, elEvento.ClaveEvento, logHeader);
                        unLog.Info("TERMINA ListaParametrosSubemisor()");

                        losParametros["@Importe"] = new Parametro()
                        {
                            Nombre = "@Importe",
                            Valor = elEvento.Importe,
                            Descripcion = "Importe"
                        };
                        losParametros["@Saldo_CLDC"] = new Parametro()
                        {
                            Nombre = "@Saldo_CLDC",
                            Valor = elEvento.SaldoCuentaCLDC,
                            Descripcion = "Saldo CLDC"
                        };

                        //Genera y Aplica la Poliza
                        unLog.Info("INICIA new Executer.EventoManual()");
                        Executer.EventoManual aplicador = new Executer.EventoManual(elEvento.IdEvento,
                            elEvento.Concepto, false, 0, losParametros, elEvento.Observaciones,
                            conn, transaccionSQL, logHeader);
                        unLog.Info("TERMINA new Executer.EventoManual()");

                        unLog.Info("INICIA Executer.AplicaContablilidad()");
                        Poliza laPoliza = aplicador.AplicaContablilidad(logHeader);
                        unLog.Info("TERMINA Executer.AplicaContablilidad()");

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                                laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                            unLog.Warn(msg);
                            throw new CAppException(8011, msg);
                        }
                        else
                        {
                            string obs = "Ejecución de Evento " + elEvento.Concepto + 
                                ". Tarjeta: " + MaskSensitiveData.cardNumber4Digits(elEvento.MedioAcceso);
                            unLog.Info("INICIA RegistraEjecucionEvManual_Tarjetahabiente()");
                            RegistraEjecucionEvManual_Tarjetahabiente(laPoliza.ID_Poliza, elEvento.Importe,
                                obs, conn, transaccionSQL, elUsuario, logHeader);
                            unLog.Info("TERMINA RegistraEjecucionEvManual_Tarjetahabiente()");
                            transaccionSQL.Commit();
                        }
                    }

                    catch (CAppException caEx)
                    {
                        throw caEx;
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        unLog.ErrorException(err);
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el registro en bitácora de la ejecución
        /// de un evento manual de "tipo" tarjetahabiente
        /// </summary>
        /// <param name="IdPoliza">Identificador de la póliza</param>
        /// <param name="Importe">Importe registrado en la póliza</param>
        /// <param name="Observ">Observaciones del evento manual</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraEjecucionEvManual_Tarjetahabiente(long IdPoliza, string Importe, string Observ,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario usuario, ILogHeader logHeader)
        {
            try
            {
                DAOEvento.InsertaEjecucionEvManualTH(IdPoliza, Importe, Observ, connection, transaccionSQL,
                    usuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar la ejecución del evento manual del tarjetahabiente");
            }
        }
    }
}