using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALCortador.Entidades;
using DALEventos.LogicaNegocio;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace DALAdministracion.LogicaNegocio
{
    //// <summary>
    /// Establece la lógica de negocio para las Cuentas
    /// </summary>
    public class LNCuentas
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion

        /// <summary>
        /// Establece las condiciones de validación para la actualización de los datos
        /// personales de la cuenta
        /// </summary>
        /// <param name="losDatos">Datos de la entidad DatosPersonalesCuenta por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDatosTitularCuenta(DatosPersonalesCuenta losDatos, Tarjeta laTarjeta,
            Usuario usuario, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                //Se valida que se tenga ingresado por lo menos el nombre del cliente
                if (String.IsNullOrEmpty(losDatos.Nombre))
                {
                    throw new CAppException(8006, "Ingresa el nombre del titular de la cuenta.");
                }

                //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
                if (!String.IsNullOrEmpty(losDatos.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(losDatos.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                pCI.Info("INICIA ActualizaDatosPersonalesCuenta()");
                DAOTarjetaCuenta.ActualizaDatosPersonalesCuenta(losDatos, usuario, logHeader);
                pCI.Info("TERMINA ActualizaDatosPersonalesCuenta()");

                pCI.Info("INICIA ActualizaNombreEmbozo()");
                DAOTarjeta.ActualizaNombreEmbozo(laTarjeta, usuario, logHeader);
                pCI.Info("TERMINA ActualizaNombreEmbozo()");

            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar los datos del cuentahabiente");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación del acceso a datos para la cancelación de un cambio al
        /// límte de crédito de la cuenta
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CancelaCambioLimiteCredito(Int64 IdCuenta, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.EliminaCambioLimiteCredito(IdCuenta, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al cancelar la modificación al límite de crédito");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación del acceso a datos para la solicitud de un cambio al
        /// límte de crédito de la cuenta
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="IdTarjeta">Identificador de la tarjeta o medio de acceso</param>
        /// <param name="NuevoLDC">Valor del nuevo límite de crédito</param>
        /// <param name="PaginaAspx">Nombre de la página web que solicita el cambio</param>
        /// <param name="Observaciones">Observaciones por las que se solicita el cambio</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void SolicitaCambioLimiteCredito(Int64 IdCuenta, int IdTarjeta, string NuevoLDC, string PaginaAspx,
            string Observaciones, Usuario usuario, Guid AppID, ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.ControlaCambioLimiteCredito(IdCuenta, IdTarjeta, NuevoLDC, PaginaAspx,
                    Observaciones, usuario, AppID, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al solicitar la modificación al límite de crédito");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el ajuste en el saldo de la cuenta
        /// límite de crédito, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="elEvento">Entidad con los datos del evento</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoLDC">Nuevo límite de crédito establecido</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AjustaSaldoLimiteCredito(EventoManual elEvento, long IdCuenta, string NuevoLDC, 
            Usuario usuario, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //Ejecuta el evento manual para el cambio en el límite de crédito
                            logPCI.Info("INICIA RegistraEvManual_AjustaLimiteCredito()");
                            LNEvento.RegistraEvManual_AjustaLimiteCredito(elEvento, conn, transaccionSQL, 
                                usuario, logHeader);
                            logPCI.Info("TERMINA RegistraEvManual_AjustaLimiteCredito()");

                            //Registra el cambio del límite de crédito en base de datos
                            logPCI.Info("INICIA RegistraCambioLimiteCredito()");
                            RegistraCambioLimiteCredito(IdCuenta, NuevoLDC, conn, transaccionSQL,
                                usuario, logHeader);
                            logPCI.Info("TERMINA RegistraCambioLimiteCredito()");

                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception ex)
                        {
                            logPCI.ErrorException(ex);
                            transaccionSQL.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                throw err; 
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al realizar el ajuste en el límite de crédito");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación del acceso a datos para para el registro
        /// del cambio del Límte de Crédito de la cuenta indicada
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoLDC">Nuevo límite de crédito establecido</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraCambioLimiteCredito(Int64 IdCuenta, string NuevoLDC, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario usuario, ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.RegistraCambioLimiteCredito(IdCuenta, NuevoLDC, connection, transaccionSQL, 
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
                throw new CAppException(8011, "Falla al registrar el cambio en el límite de crédito");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro multiasignación a 
        /// una cuenta en el Autorizador
        /// </summary>
        /// <param name="IdParametroMA">Idetnificador del parametro</param>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="IdPlantilla">Identificador de la plantilla</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AgregaParametroMAACuenta(int IdParametroMA, int IdTarjeta, long IdPlantilla, 
            Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.InsertaPMACuenta(IdParametroMA, IdTarjeta, IdPlantilla, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al asignar el valor del parámetro");
            }
        }

        /// <summary>
        /// Verifica que a fcha valor sea válida y que sea posterior a la última fecha de corte de la cuenta
        /// </summary>
        /// <param name="laFecha">Fecha capturada del movimiento</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void VerificaFechaCorte(object laFecha, long IdCuenta, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                DateTime fechaValor, fechaCorte;

                //Se valida que se haya capturado una fecha válida
                if (!DateTime.TryParse(laFecha.ToString(), out fechaValor))
                {
                    throw new CAppException(8006, "La Fecha Real del Movimiento es una fecha inválida.");
                }

                pCI.Info("INICIA ValidaFechaValor()");
                fechaCorte = DAOTarjetaCuenta.ValidaFechaValor(IdCuenta, logHeader);
                pCI.Info("TERMINA ValidaFechaValor()");

                if (DateTime.Compare(fechaValor, fechaCorte) <= 0)
                {
                    throw new CAppException(8006, "La Fecha Real del Movimiento no puede ser anterior al último corte." +
                        "<br/><br/>El Movimiento sólo puede aplicarse a partir del día " +
                        fechaCorte.AddDays(1).ToShortDateString() + ".");
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al verificar la fecha de corte");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de los datos OnBoarding
        /// personales de la cuenta
        /// </summary>
        /// <param name="losDatos">Datos de la entidad DatosPersonalesCuentaTDC por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDatosPersonalesCuentaOnB(DatosPersonalesCuenta losDatos, Usuario usuario,
            ILogHeader logHeader)
        {
            try
            {
                //Se valida que se tenga ingresado por lo menos el nombre del cliente
                if (String.IsNullOrEmpty(losDatos.Nombre))
                {
                    throw new CAppException(8006, "Ingresa el nombre del titular de la cuenta.");
                }

                //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
                if (!String.IsNullOrEmpty(losDatos.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(losDatos.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                DAOTarjetaCuenta.ActualizaDatosPersonalesCuentaOnB(losDatos, usuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar los datos personales de la cuenta");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro multiasignación, con
        /// el tipo de plantilla Preautorizador, a una cuenta del Autorizador
        /// </summary>
        /// <param name="IdParametroMA">Idetnificador del parametro</param>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="IdPlantilla">Identificador de la plantilla</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AgregaPMAPreAutACuenta(int IdParametroMA, int IdTarjeta, Int64 IdPlantilla, IUsuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.InsertaPMAPreAutCuenta(IdParametroMA, IdTarjeta, IdPlantilla, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al agregar el parámetro a la cuenta");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la verificación del nivel de la cuenta
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="NumTarjeta">Número de tarjeta asociado a la cuenta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados de la validación</returns>
        public static DataTable ValidaNivelCumplimientoCuenta(long IdProducto, string NumTarjeta, ILogHeader logHeader)
        {
            try
            {
                DataTable dtResponse = DAOTarjetaCuenta.VerificaNivelCumplimientoCuenta(
                    IdProducto, NumTarjeta, logHeader);

                string mensaje = dtResponse.Rows[0]["Mensaje"].ToString();

                if (!mensaje.ToUpper().Contains("OK"))
                {
                    throw new CAppException(8006, mensaje);
                }

                return dtResponse;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al verificar el nivel de cumplimiento de la cuenta");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación del valor de los parámetros
        /// multiasignación relacionados con el nivel de cuenta personalizado, y que requieren de autorización
        /// </summary>
        /// <param name="IdPlantilla">Identificador de la plantilla</param>
        /// <param name="MontoSolic">Monto solicitado (abono máximo personalizado)</param>
        /// <param name="MontoAcum">Monto acumulado (saldo máximo personalizado)</param>
        /// <param name="Motivo">Motivo por el que se realiza la solicitud</param>
        /// <param name="RespBlackList">Identificador de la tarjeta</param>
        /// <param name="PaginaAspx">Usuario en sesión</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaModificaSolicitudCambioNivelCuenta(long IdPlantilla, string MontoSolic, string MontoAcum,
            string Motivo, string RespBlackList, string PaginaAspx, Usuario usuario, Guid AppID, ILogHeader logHeader,
            string Tarjeta)
        {
            try
            {
                DAOTarjetaCuenta.InsertaSolicNivelCuentaVIP(IdPlantilla, MontoSolic, MontoAcum, Motivo, RespBlackList,
                    PaginaAspx, usuario, AppID, logHeader, Tarjeta);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar los valores por autorizar de los parámetros");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para consultar las solicitudes de cambio de nivel de cuentas a VIP
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="NumTarjeta">Número de tarjeta asociado a la cuenta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes de cambio</returns>
        public static DataTable ConsultaSolicitudesCambioCuentaVIP(long IdProducto, string NumTarjeta, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                DataTable dtSolicitudes = new DataTable();
                DataTable dtPorAutorizar = new DataTable();
                int counter = 0;

                pCI.Info("INICIA ObtienePlantillasN3()");
                DataTable dtPlantillasTarjProd = DAOTarjetaCuenta.ObtienePlantillasN3(IdProducto, NumTarjeta, logHeader);
                pCI.Info("TERMINA ObtienePlantillasN3()");

                pCI.Info("INICIA ObtieneSolicitudesCambioCuentasVIP()");
                DataTable dtRegistrosPorAutorizar = DAOTarjetaCuenta.ObtieneSolicitudesCambioCuentasVIP(logHeader);
                pCI.Info("TERMINA ObtieneSolicitudesCambioCuentasVIP()");

                if (dtRegistrosPorAutorizar.Rows.Count > 0)
                {
                    dtPorAutorizar.Columns.Add("ID_EjecutorAutorizador");
                    dtPorAutorizar.Columns.Add("ID_ParametroMultiasignacion");
                    dtPorAutorizar.Columns.Add("ID_Plantilla");
                    dtPorAutorizar.Columns.Add("Motivo");
                    dtPorAutorizar.Columns.Add("ResultadoBlackList");
                    dtPorAutorizar.Columns.Add("ID_RegistroAfectado");
                    dtPorAutorizar.Columns.Add("NuevoValorCampo");
                    dtPorAutorizar.Rows.Add();

                    foreach (DataRow row in dtRegistrosPorAutorizar.Rows)
                    {
                        dtPorAutorizar.Rows[counter]["ID_EjecutorAutorizador"] = row["ID_EjecutorAutorizador"];
                        string[] paramKeys = row["ParametrosLlavePantalla"].ToString().Split(';');

                        dtPorAutorizar.Rows[counter]["ID_ParametroMultiasignacion"] = paramKeys[0].Split('=')[1].ToString();
                        dtPorAutorizar.Rows[counter]["ID_Plantilla"] = paramKeys[1].Split('=')[1].ToString();
                        dtPorAutorizar.Rows[counter]["Motivo"] = paramKeys[2].Split('=')[1].ToString();
                        dtPorAutorizar.Rows[counter]["ResultadoBlackList"] = paramKeys[3].Split('=')[1].ToString();

                        dtPorAutorizar.Rows[counter]["ID_RegistroAfectado"] = row["ID_RegistroAfectado"];
                        dtPorAutorizar.Rows[counter]["NuevoValorCampo"] = row["NuevoValorCampo"];

                        dtPorAutorizar.Rows.Add();

                        counter++;
                    }

                    dtSolicitudes.Columns.Add("ID_Plantilla");
                    dtSolicitudes.Columns.Add("ID_EA_NivelCtaCumpl");
                    dtSolicitudes.Columns.Add("ID_PMA_NivelCtaCumpl");
                    dtSolicitudes.Columns.Add("ID_VPMA_NivelCtaCumpl");
                    dtSolicitudes.Columns.Add("ID_EA_NivelCtaCumplPers");
                    dtSolicitudes.Columns.Add("ID_PMA_NivelCtaCumplPers");
                    dtSolicitudes.Columns.Add("ID_EA_SaldoMaxPers");
                    dtSolicitudes.Columns.Add("ID_PMA_SaldoMaxPers");
                    dtSolicitudes.Columns.Add("ID_EA_MaxAbonoPers");
                    dtSolicitudes.Columns.Add("ID_PMA_MaxAbonoPers");
                    dtSolicitudes.Columns.Add("MontoSolicitado");
                    dtSolicitudes.Columns.Add("MontoAcumulado");
                    dtSolicitudes.Columns.Add("Motivo");
                    dtSolicitudes.Columns.Add("RespListaNegra");
                    dtSolicitudes.Rows.Add();

                    counter = 0;

                    foreach (DataRow rowPTP in dtPlantillasTarjProd.Rows)
                    {
                        string plantilla = "ID_Plantilla = " + rowPTP["ID_Plantilla"];
                        string IdVPMA = null;
                        DataRow[] matches = dtPorAutorizar.Select(plantilla);

                        foreach (DataRow rowMatch in matches)
                        {
                            dtSolicitudes.Rows[counter]["ID_Plantilla"] = rowMatch["ID_Plantilla"];
                            if (string.IsNullOrEmpty(IdVPMA) && rowMatch["ID_RegistroAfectado"].ToString().Length < 12)
                            {
                                IdVPMA = rowMatch["ID_RegistroAfectado"].ToString();
                            }

                            dtSolicitudes.Rows[counter]["ID_EA_NivelCtaCumpl"] =
                                rowMatch["ID_RegistroAfectado"].ToString().Length < 12 ?
                                rowMatch["ID_EjecutorAutorizador"] :
                                (dtSolicitudes.Rows[counter]["ID_EA_NivelCtaCumpl"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_EA_NivelCtaCumpl"] : 0;
                            dtSolicitudes.Rows[counter]["ID_PMA_NivelCtaCumpl"] =
                                rowMatch["ID_RegistroAfectado"].ToString().Length < 12 ?
                                rowMatch["ID_ParametroMultiasignacion"] :
                                (dtSolicitudes.Rows[counter]["ID_PMA_NivelCtaCumpl"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_PMA_NivelCtaCumpl"] : 0;
                            dtSolicitudes.Rows[counter]["ID_VPMA_NivelCtaCumpl"] =
                                rowMatch["ID_RegistroAfectado"].ToString().Length < 12 ?
                                rowMatch["ID_RegistroAfectado"] :
                                (dtSolicitudes.Rows[counter]["ID_VPMA_NivelCtaCumpl"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_VPMA_NivelCtaCumpl"] : 0;

                            dtSolicitudes.Rows[counter]["ID_EA_NivelCtaCumplPers"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '1' ?
                                rowMatch["ID_EjecutorAutorizador"] :
                                (dtSolicitudes.Rows[counter]["ID_EA_NivelCtaCumplPers"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_EA_NivelCtaCumplPers"] : 0;
                            dtSolicitudes.Rows[counter]["ID_PMA_NivelCtaCumplPers"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '1' ?
                                rowMatch["ID_ParametroMultiasignacion"] :
                                (dtSolicitudes.Rows[counter]["ID_PMA_NivelCtaCumplPers"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_PMA_NivelCtaCumplPers"] : 0;

                            dtSolicitudes.Rows[counter]["ID_EA_SaldoMaxPers"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '3' ?
                                rowMatch["ID_EjecutorAutorizador"] :
                                (dtSolicitudes.Rows[counter]["ID_EA_SaldoMaxPers"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_EA_SaldoMaxPers"] : 0;
                            dtSolicitudes.Rows[counter]["ID_PMA_SaldoMaxPers"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '3' ?
                                rowMatch["ID_ParametroMultiasignacion"] :
                                (dtSolicitudes.Rows[counter]["ID_PMA_SaldoMaxPers"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_PMA_SaldoMaxPers"] : 0;

                            dtSolicitudes.Rows[counter]["ID_EA_MaxAbonoPers"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '2' ?
                                rowMatch["ID_EjecutorAutorizador"] :
                                (dtSolicitudes.Rows[counter]["ID_EA_MaxAbonoPers"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_EA_MaxAbonoPers"] : 0;
                            dtSolicitudes.Rows[counter]["ID_PMA_MaxAbonoPers"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '2' ?
                                rowMatch["ID_ParametroMultiasignacion"] :
                                (dtSolicitudes.Rows[counter]["ID_PMA_MaxAbonoPers"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["ID_PMA_MaxAbonoPers"] : 0;

                            dtSolicitudes.Rows[counter]["MontoSolicitado"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '2' ?
                                rowMatch["NuevoValorCampo"] :
                                (dtSolicitudes.Rows[counter]["MontoSolicitado"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["MontoSolicitado"] : 0;
                            dtSolicitudes.Rows[counter]["MontoAcumulado"] =
                                rowMatch["ID_RegistroAfectado"].ToString() == IdVPMA.PadRight(11, '0') + '3' ?
                                rowMatch["NuevoValorCampo"] :
                                (dtSolicitudes.Rows[counter]["MontoAcumulado"].ToString() != "{}") ?
                                dtSolicitudes.Rows[counter]["MontoAcumulado"] : 0;

                            dtSolicitudes.Rows[counter]["Motivo"] = rowMatch["Motivo"];
                            dtSolicitudes.Rows[counter]["RespListaNegra"] = rowMatch["ResultadoBlackList"];
                        }

                        if (matches.AsEnumerable().ToList().Count > 0)
                        {
                            dtSolicitudes.Rows.Add();
                            counter++;
                        }
                    }

                    if (dtSolicitudes.Rows.Count == 0)
                    {
                        throw new CAppException(8011, "No existen Solicitudes de Cambio pendientes de Autorización para el " +
                            "SubEmisor/Producto/Tarjeta seleccionado.");
                    }
                    else if (string.IsNullOrEmpty(dtSolicitudes.Rows[counter]["ID_Plantilla"].ToString()))
                    {
                        dtSolicitudes.Rows.RemoveAt(counter);
                    }
                    
                    return dtSolicitudes;
                }
                else
                {
                    throw new CAppException(8011, "No existen Solicitudes de Cambio pendientes de Autorización.");
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al obtener las solicitudes de cambio de nivel de las cuentas");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para consultar las solicitudes de cambio de nivel de cuentas a VIP
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="NumTarjeta">Número de tarjeta asociado a la cuenta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes de cambio</returns>
        public static DataTable ConsultaTarjetasVIPDetalle(long IdProducto, string NumTarjeta, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                pCI.Info("INICIA ObtieneSolicitudesCambioCuentasVIP()");
                DataTable dtRegistrosPorAutorizar = DAOTarjetaCuenta.ObtieneRegistrosTarjetasVIPDetalle(IdProducto, NumTarjeta, logHeader);
                pCI.Info("TERMINA ObtieneSolicitudesCambioCuentasVIP()");

                if (dtRegistrosPorAutorizar.Rows.Count == 0)
                {
                    throw new CAppException(8011, "No existen coincidencias con los filtros solicitados.");
                }

                return dtRegistrosPorAutorizar;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al obtener las solicitudes de cambio de nivel de las cuentas");
            }
        }


        /// <summary>
        /// Establece las condiciones de validación para la modificación y autorización del cambio de nivel
        /// personalizado de la cuenta, para hacerse VIP
        /// </summary>
        /// <param name="unaPlantilla">Objeto de la plantilla por modificar</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaYAutorizaCambioCuentaVIP(PlantillaCuentaVIP unaPlantilla, Usuario elUser,
            ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.ActualizaYAutorizaValoresCuentaVIP(unaPlantilla, elUser, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar y autorizar la solicitud de cambio de la cuenta");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para rechazar el cambio de nivel personalizado de
        /// la cuenta, para hacerse VIP
        /// </summary>
        /// <param name="IdValorParametro">Identificador del valor del parámetro</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RechazaCambioCuentaVIP(PlantillaCuentaVIP laPlantilla, Usuario elUser, ILogHeader logHeader)
        {
            try
            {
                DAOTarjetaCuenta.EliminaValoresCuentaVIP(laPlantilla, elUser, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al rechazar la solicitud de cambio de la cuenta");
            }
        }
        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de Login del web service Parabilium
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static Parametros.Headers LoginWebServiceParabilium(ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsLoginResp = null;
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                _headers.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_URL", logHeader).Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Usr", logHeader).Valor;
                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Pwd", logHeader).Valor;


                log.Info("INICIA Parabilium.Login()");
                wsLoginResp = Parabilium.Login(_headers, _body, logHeader);
                log.Info("TERMINA Parabilium.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headers.Token = wsLoginResp;
                _headers.Credenciales = Cifrado.Base64Encode(_body.NombreUsuario + ":" + _body.Password);

                return _headers;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al establecer establecer el login con el Servicio Web");
            }
        }

        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de Activar Tarjeta del web service Parabilium
        /// </summary>
        /// <param name="headers">Parámetros del header del método</param>
        /// <param name="tarjeta">Valor en claro de la tarjeta</param>
        /// <param name="tarjetaEnmascarada">Valor enmascarado de la tarjeta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static string ActivaTarjetaWebServiceParabilium(Parametros.Headers headers, string tarjeta,
            string tarjetaEnmascarada, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsActResp = null;

                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.ActivarTarjetaBody activar = new Parametros.ActivarTarjetaBody();
                activar.IDSolicitud = "";
                activar.Tarjeta = tarjeta;
                activar.MedioAcceso = "";
                activar.TipoMedioAcceso = "";

                Parametros.ActivarTarjetaBody maskedBody = new Parametros.ActivarTarjetaBody();
                maskedBody.IDSolicitud = "";
                maskedBody.Tarjeta = tarjetaEnmascarada;
                maskedBody.MedioAcceso = "";
                maskedBody.TipoMedioAcceso = "";
                //

                log.Info("INICIA Parabilium.ActivarTarjeta()");
                wsActResp = Parabilium.ActivarTarjeta(headers, activar, maskedBody, logHeader);
                log.Info("TERMINA Parabilium.ActivarTarjeta()");

                if (!wsActResp.Contains("OK"))
                {
                    throw new CAppException(8006, wsActResp);
                }

                return wsActResp;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla en la activación de la tarjeta");
            }
        }

        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de Activar Tarjeta del web service Parabilium
        /// </summary>
        /// <param name="headers">Parámetros del header del método</param>
        /// <param name="tarjeta">Valor en claro de la tarjeta</param>
        /// <param name="tarjetaEnmascarada">Valor enmascarado de la tarjeta</param>
        /// <param name="motivo">Motivo del bloqueo (código)</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static string BloqueaTarjetaWebServiceParabilium(Parametros.Headers headers, string tarjeta,
            string tarjetaEnmascarada, string motivo, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsBloqResp = null;

                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.BloquearTarjetaBody bloqBody = new Parametros.BloquearTarjetaBody();
                bloqBody.Tarjeta = tarjeta;
                bloqBody.MedioAcceso = "";
                bloqBody.TipoMedioAcceso = "";
                bloqBody.MotivoBloqueo = motivo;

                Parametros.BloquearTarjetaBody maskedBody = new Parametros.BloquearTarjetaBody();
                maskedBody.Tarjeta = tarjetaEnmascarada;
                maskedBody.MedioAcceso = "";
                maskedBody.TipoMedioAcceso = "";
                maskedBody.MotivoBloqueo = motivo;
                //

                log.Info("INICIA Parabilium.BloquearTarjeta()");
                wsBloqResp = Parabilium.BloquearTarjeta(headers, bloqBody, maskedBody, logHeader);
                log.Info("TERMINA Parabilium.BloquearTarjeta()");

                if (!wsBloqResp.Contains("OK"))
                {
                    throw new CAppException(8006, wsBloqResp);
                }

                return wsBloqResp;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla en el bloqueo de la tarjeta");
            }
        }

        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de VerOperacionDiferida del web service Parabilium
        /// </summary>
        /// <param name="headers">Parámetros del header del método</param>
        /// <param name="idOperacion">Identificador de la operación</param>
        /// <param name="tarjeta">Valor en claro de la tarjeta</param>
        /// <param name="tarjetaEnmascarada">Valor enmascarado de la tarjeta</param>
        /// <param name="idConfigCamp">Identificador de la configuración de campaña</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static List<OperacionDiferimiento> DetalleOperacionDiferimientoWSParabilium(Parametros.Headers headers,
            string idOperacion, string tarjeta, string tarjetaEnmascarada, string idConfigCamp, ref decimal total, 
            ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.DiferirOperacionBody detOpBody = new Parametros.DiferirOperacionBody();
                detOpBody.IdOperacion = idOperacion;
                detOpBody.Tarjeta = tarjeta;
                detOpBody.IdPlan = idConfigCamp;

                Parametros.DiferirOperacionBody maskedBody = new Parametros.DiferirOperacionBody();
                maskedBody.IdOperacion = idOperacion;
                maskedBody.Tarjeta = tarjetaEnmascarada;
                maskedBody.IdPlan = idConfigCamp;

                log.Info("INICIA Parabilium.VerDetalleOperacionDiferida()");
                List<OperacionDiferimiento> losDetalles =
                    Parabilium.VerDetalleOperacionDiferida(headers, detOpBody, maskedBody, logHeader);
                log.Info("TERMINA Parabilium.VerDetalleOperacionDiferida()");

                foreach (OperacionDiferimiento detalle in losDetalles)
                {
                    total += Convert.ToDecimal(detalle.Total);
                }

                return losDetalles;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla en la consulta de detalle de diferimiento de la operación.");
            }
        }

        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de Diferir Operacion del web service Parabilium
        /// </summary>
        /// <param name="headers">Parámetros del header del método</param>
        /// <param name="idOperacion">Identificador de la operación</param>
        /// <param name="tarjeta">Valor en claro de la tarjeta</param>
        /// <param name="tarjetaEnmascarada">Valor enmascarado de la tarjeta</param>
        /// <param name="idConfigCamp">Identificador de la configuración de campaña</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static void DifiereOperacionWSParabilium(Parametros.Headers headers, string idOperacion, string tarjeta,
            string tarjetaEnmascarada, string idConfigCamp, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.DiferirOperacionBody detOpBody = new Parametros.DiferirOperacionBody();
                detOpBody.IdOperacion = idOperacion;
                detOpBody.Tarjeta = tarjeta;
                detOpBody.IdPlan = idConfigCamp;

                Parametros.DiferirOperacionBody maskedBody = new Parametros.DiferirOperacionBody();
                maskedBody.IdOperacion = idOperacion;
                maskedBody.Tarjeta = tarjetaEnmascarada;
                maskedBody.IdPlan = idConfigCamp;

                log.Info("INICIA Parabilium.DiferirOperacion()");
                string resp = Parabilium.DiferirOperacion(headers, detOpBody, maskedBody, logHeader);
                log.Info("TERMINA Parabilium.DiferirOperacion()");

                if (!resp.Contains("OK"))
                {
                    throw new CAppException(8006, resp);
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla  en el diferimiento de la operación.");
            }
        }
    }
}