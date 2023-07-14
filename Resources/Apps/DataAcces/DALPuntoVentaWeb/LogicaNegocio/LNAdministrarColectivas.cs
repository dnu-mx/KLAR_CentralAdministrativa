using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Utilerias;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para la Administración de Colectivas
    /// </summary>
    public class LNAdministrarColectivas
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion


        /// <summary>
        /// Establece las condiciones de validación para crear una nueva colectiva en el Autorizador,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdTipoColectiva">Identificador del tipo de colectiva</param>
        /// <param name="IdColectivaPadre">Identificador de la colectiva padre</param>
        /// <param name="ClaveColectiva">Clave de la nueva colectiva</param>
        /// <param name="RazonSocial">Nombre o razón social</param>
        /// <param name="NombreComercial">Nombre comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static DataTable CreaNuevaColectivaEnAutorizador(int IdTipoColectiva, int IdColectivaPadre,
            string ClaveColectiva, string RazonSocial, string NombreComercial, Usuario elUsuario)
        {
            try
            {
                return DAOAdministrarColectivas.InsertaColectiva(IdTipoColectiva, IdColectivaPadre,
                       ClaveColectiva, RazonSocial, NombreComercial, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear una nueva colectiva en el Autorizador,
        /// </summary>
        /// <param name="IdTipoColectiva">Identificador del tipo de colectiva</param>
        /// <param name="IdColectivaPadre">Identificador de la colectiva padre</param>
        /// <param name="ClaveColectiva">Clave de la nueva colectiva</param>
        /// <param name="RazonSocial">Nombre o razón social</param>
        /// <param name="NombreComercial">Nombre comercial</param>
        /// <param name="IdDivisa">Identificador de la divisa</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static DataTable CreaNuevaColectivaEnAutorizador(int IdTipoColectiva, int IdColectivaPadre,
            string ClaveColectiva, string RazonSocial, string NombreComercial, int IdDivisa, Usuario elUsuario,
            ILogHeader logHeader)
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
                            logPCI.Info("INICIA InsertaColectiva()");
                            DataTable dtResp = DAOAdministrarColectivas.InsertaColectiva(IdTipoColectiva,
                                IdColectivaPadre, ClaveColectiva, RazonSocial, NombreComercial, IdDivisa, conn,
                                transaccionSQL, elUsuario, logHeader);
                            logPCI.Info("TERMINA InsertaColectiva()");

                            string idColectivaNueva = dtResp.Rows[0]["ID_NuevaColectiva"].ToString();
                            if (idColectivaNueva == "-1")
                            {
                                throw new CAppException(8010, dtResp.Rows[0]["Mensaje"].ToString());
                            }

                            long idCtaCCLC = Convert.ToInt64(dtResp.Rows[0]["ID_CuentaCCLC"]);
                            if (idCtaCCLC != 0)
                            {
                                string idCta = idCtaCCLC.ToString() + Tarjetas.GeneraDigitoVerificador(
                                    idCtaCCLC.ToString().PadRight(16, '0'), logHeader);

                                string ctaInt = idColectivaNueva.PadLeft(8, '0') +
                                    idCtaCCLC.ToString().PadLeft(8, '0');

                                logPCI.Info("INICIA CreaMediosAcceso()");
                                DAOAdministrarColectivas.CreaMediosAcceso(idColectivaNueva, idCta, ctaInt,
                                    conn, transaccionSQL, logHeader);
                                logPCI.Info("TERMINA CreaMediosAcceso()");
                            }

                            transaccionSQL.Commit();
                            return dtResp;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception ex)
                        {
                            transaccionSQL.Rollback();
                            logPCI.ErrorException(ex);
                            throw new CAppException(8011, "Falla al crear la nueva Colectiva.");
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
                throw new CAppException(8011, "Falla en el método de creación de la nueva Colectiva.");
            }
        }

        /// <summary>
        /// <summary>
        /// Establece las condiciones de validación para modificar los datos de una colectiva en el Autorizador
        /// </summary>
        /// <param name="laColectiva">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaColectivaEnAutorizador(Colectiva laColectiva, Usuario elUsuario)
        {
            try
            {
                //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
                if (!String.IsNullOrEmpty(laColectiva.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(laColectiva.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                if (!String.IsNullOrEmpty(laColectiva.Password))
                {
                    if (String.Compare(laColectiva.Password, laColectiva.RePassword) != 0)
                    {
                        throw new CAppException(8006, "Las Contraseñas no coinciden.");
                    }
                }

                DAOAdministrarColectivas.ActualizaColectiva(laColectiva, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar los datos de una colectiva en el Autorizador
        /// </summary>
        /// <param name="laColectiva">Entidad con los datos por modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaColectivaEnAutorizador(Colectiva laColectiva, ILogHeader logHeader, Usuario elUsuario)
        {
            LogPCI log = new LogPCI(logHeader);
            string msj = string.Empty;

            try
            {
                //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
                if (!String.IsNullOrEmpty(laColectiva.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(laColectiva.Email);

                    if (!matchExpression.Success)
                    {
                        msj = "El correo electrónico que ingresaste no es una dirección válida.";
                        log.Warn(msj);
                        throw new CAppException(8010, msj);
                    }
                }

                if (!String.IsNullOrEmpty(laColectiva.Telefono) && laColectiva.Telefono.Length < 10)
                {
                    msj = "El número telefónico fijo que ingresaste no es válido. La longitud mínima es de 10 dígitos.";
                    log.Warn(msj);
                    throw new CAppException(8010, msj);
                }

                if (!String.IsNullOrEmpty(laColectiva.Movil) && laColectiva.Movil.Length < 10)
                {
                    msj = "El número telefónico celular que ingresaste no es válido. La longitud mínima es de 10 dígitos.";
                    log.Warn(msj);
                    throw new CAppException(8010, msj);
                }

                if (!String.IsNullOrEmpty(laColectiva.Password))
                {
                    if (String.Compare(laColectiva.Password, laColectiva.RePassword) != 0)
                    {
                        msj = "Las contraseñas no coinciden.";
                        log.Warn(msj);
                        throw new CAppException(8006, msj);
                    }
                }

                DAOAdministrarColectivas.ActualizaColectiva(laColectiva, logHeader, elUsuario);
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la dirección de una colectiva en el Autorizador
        /// </summary>
        /// <param name="laColectiva">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaDireccionColectiva(DireccionColectiva unaDireccion, Usuario elUsuario)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaDireccionColectiva(unaDireccion, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la dirección de una colectiva en el Autorizador
        /// </summary>
        /// <param name="laColectiva">Entidad con los datos por modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaDireccionColectiva(DireccionColectiva unaDireccion, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaDireccionColectiva(unaDireccion, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el Domicilio de la Colectiva");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear una nueva cuenta para la
        /// colectiva en el Autorizador
        /// </summary>
        /// <param name="unaCuenta">Datos de la nueva cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaNuevaCuentaColectiva(Cuenta unaCuenta, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            log.Info("INICIA InsertaCuentaColectiva()");
                            int idCuenta = DAOAdministrarColectivas.InsertaCuentaColectiva(unaCuenta, conn, 
                                transaccionSQL, elUsuario, logHeader);
                            log.Info("TERMINA InsertaCuentaColectiva()");

                            if (idCuenta > -1)
                            {
                                log.Info("INICIA InsertaMA_IDCuentaColectiva()");
                                DAOAdministrarColectivas.InsertaMA_IDCuentaColectiva(unaCuenta.ID_Colectiva, 
                                    idCuenta.ToString(), conn, transaccionSQL, elUsuario, logHeader);
                                log.Info("TERMINA InsertaMA_IDCuentaColectiva()");
                            }

                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception ex)
                        {
                            transaccionSQL.Rollback();
                            log.ErrorException(ex);
                            throw new CAppException(8011, "Falla al crear la cuenta de la Colectiva.");
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
                throw new CAppException(8011, "Falla al crear la nueva Cuenta de la Colectiva");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear una nueva cuenta para la
        /// colectiva en el Autorizador
        /// </summary>
        /// <param name="unaCuenta">Datos de la nueva cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void CreaNuevaCuentaColectiva(Cuenta unaCuenta, Usuario elUsuario)
        {
            try
            {
                DAOAdministrarColectivas.InsertaCuentaColectiva(unaCuenta, elUsuario);
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la cuenta de una colectiva en el Autorizador
        /// </summary>
        /// <param name="cuenta">Entidad con los datos por modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaCuentaColectiva(Cuenta cuenta, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaDatosCuenta(cuenta, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar la Cuenta de la Colectiva");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la cuenta de una colectiva en el Autorizador
        /// </summary>
        /// <param name="cuenta">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaCuentaColectiva(Cuenta cuenta, Usuario elUsuario)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaDatosCuenta(cuenta, elUsuario);
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para bloquear o desbloquear la cuenta en el Autorizador
        /// </summary>
        /// <param name="IdCuenta">Idetnificador de la cuenta</param>
        /// <param name="ClaveEstatus">Clave del estatus actual de la cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void BloqueaDesbloqueaCuenta(Int64 IdCuenta, String ClaveEstatus, Usuario elUsuario)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaEstatusCuenta(IdCuenta, ClaveEstatus, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para bloquear o desbloquear la cuenta en el Autorizador,
        /// </summary>
        /// <param name="IdCuenta">Idetnificador de la cuenta</param>
        /// <param name="ClaveEstatus">Clave del estatus actual de la cuenta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void BloqueaDesbloqueaCuenta(long IdCuenta, string ClaveEstatus, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaEstatusCuenta(IdCuenta, ClaveEstatus, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus de la Cuenta");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para clonar los parámetros de un contrato en el Autorizador,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdColectivaOrigen">Identificador de la colectiva origen</param>
        /// <param name="IdColectivaDestino">Identificador de la colectiva destino</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ClonaParametrosContrato(Int64 IdColectivaOrigen, Int64 IdColectivaDestino,
            Usuario elUsuario)
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
                            DAOAdministrarColectivas.ClonaContratoColectiva(IdColectivaOrigen, IdColectivaDestino,
                                conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ClonaParametrosContrato() Falla al clonar el contrato de la colectiva en el Autorizador", err);
                        }
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro a un contrato en el Autorizador
        /// </summary>
        /// <param name="IdValorParametro">Idetnificador del parametro</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static string AgregaParametroAContrato(Int32 IdValorParametro, Int64 IdColectiva, Usuario elUsuario)
        {
            try
            {
                return DAOAdministrarColectivas.InsertaValorContrato(IdValorParametro, IdColectiva, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro a un contrato en el Autorizador,
        /// </summary>
        /// <param name="IdValorParametro">Idetnificador del parametro</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static string AgregaParametroAContrato(int IdValorParametro, long IdColectiva, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOAdministrarColectivas.InsertaValorContrato(IdValorParametro, IdColectiva, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al asignar el Parámetro a la Colectiva");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro extra a la colectiva
        /// en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdParametroExtra">Identificador del parámetro extra</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void AgregaParametroExtraColectiva(Int32 IdParametroExtra, Int64 IdColectiva,
            Usuario elUsuario)
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
                            DAOAdministrarColectivas.InsertaValorParametroExtra(IdParametroExtra, IdColectiva,
                                conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "AgregaParametroExtraColectiva() Falla al agregar parámetro extra a la colectiva en el Autorizador", err);
                        }
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del valor de un parámetro
        /// extra de la colectiva indicada, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="unParametro">Objeto del parámetro extra por modificar</param>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ModificaParametroExtra(ParametroExtra unParametro, Int64 ID_Colectiva, Usuario elUser)
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
                            DAOAdministrarColectivas.ActualizaValorParametroExtra(unParametro, ID_Colectiva,
                                conn, transaccionSQL, elUser);
                            transaccionSQL.Commit();
                        }

                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }

                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }

            catch (Exception err)
            {
                throw new Exception("ModificaParametroExtra(). Falla al actualizar los parámetros extra de la colectiva: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para eliminar el parámetro extra a la colectiva
        /// en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdParametroExtra">Identificador del parámetro extra</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void BorraParametroExtraColectiva(Int32 IdParametroExtra, Int64 IdColectiva,
            Usuario elUsuario)
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
                            DAOAdministrarColectivas.EliminaValorParametroExtra(IdParametroExtra, IdColectiva,
                                conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "BorraParametroExtraColectiva() Falla al eliminar el parámetro extra a la colectiva en el Autorizador", err);
                        }
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para eliminar el parámetro de contrato a la colectiva
        /// en el Autorizador
        /// </summary>
        /// <param name="IdParametro">Identificador del parámetro de contrato</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void BorraParametroAColectiva(Int32 IdParametro, Int64 IdColectiva, Usuario elUsuario)
        {
            try
            {
                DAOAdministrarColectivas.EliminaValorParametro(IdParametro, IdColectiva, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para eliminar el parámetro de contrato a la colectiva
        /// en el Autorizador
        /// </summary>
        /// <param name="IdParametro">Identificador del parámetro de contrato</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void BorraParametroAColectiva(Int32 IdParametro, Int64 IdColectiva, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.EliminaValorParametro(IdParametro, IdColectiva, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al eliminar el Parámetro de la Colectiva");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del valor de un parámetro
        /// de contrato de la colectiva indicada
        /// </summary>
        /// <param name="unParametro">Objeto del parámetro por modificar</param>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ModificaValorParametro(Parametro unParametro, Usuario elUser)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaValorParametro(unParametro, elUser);
            }

            catch (Exception err)
            {
                throw new Exception("ModificaValorParametro(). Falla al actualizar el valor del parámetro de la colectiva: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del valor de un parámetro
        /// de contrato de la colectiva indicada
        /// </summary>
        /// <param name="unParametro">Objeto del parámetro por modificar</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaValorParametro(Parametro unParametro, Usuario elUser, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaValorParametro(unParametro, elUser, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al actualizar el valor del Parámetro de la Colectiva");
            }
        }


        public static WsResponse CreaNuevoTokenAColectiva(NewToken token, Usuario usuario)
        {
            WsResponse resp = new WsResponse();
            resp.success = false;
            resp.Messages = new System.Collections.Generic.List<string>(); 
            resp.Messages.Add("Ocurrio un error al añadir el nuevo Refresh Token");

            using (HttpClient client = new HttpClient())
            {
                WsTokenErrorResponse errorResponse = new WsTokenErrorResponse();
                WsTokenOkResponse okResponse = new WsTokenOkResponse();
                try
                {
                    String urlbase = DALCentralAplicaciones.Utilidades.Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WsURL").Valor;
                    String url = String.Format("{0}/account/register", urlbase);
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    httpRequest.ContentType = "application/json";
                    httpRequest.Method = "POST";

                    byte[] bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(token));
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Close();
                    }

                    try
                    {
                        using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                        {
                            using (Stream stream = httpResponse.GetResponseStream())
                            {
                                okResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WsTokenOkResponse>((new StreamReader(stream)).ReadToEnd());

                                resp.code = (int)httpResponse.StatusCode;
                                resp.Messages = new System.Collections.Generic.List<string>();
                                resp.Messages.Add("<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ");
                                resp.success = true;
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        using (WebResponse response = e.Response)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)response;
                            Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                            using (Stream data = response.GetResponseStream())
                            using (var reader = new StreamReader(data))
                            {
                                errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WsTokenErrorResponse>(reader.ReadToEnd());
                                resp.code = (int)httpResponse.StatusCode;
                                resp.Messages = new System.Collections.Generic.List<string>();
                                if(errorResponse.errors == null)
                                {
                                    resp.Messages.Add(errorResponse.message);
                                }
                                else
                                {
                                    foreach (var item in errorResponse.errors)
                                    {
                                        resp.Messages.Add(item.defaultMessage);
                                    }
                                }
                                
                                
                                resp.success = false;
                            }
                        }
                    }

                    return resp;
                }
                catch(Exception )
                {
                    return resp;
                }
            }
        }

        public static void SendTokenByEmail(String emails, int tokenId, Usuario user, Guid AppId)
        {
            try
            {
                String WsSMTP = DALCentralAplicaciones.Utilidades.Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WsSMTP").Valor;
                String WsSMTPUser = DALCentralAplicaciones.Utilidades.Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WsSMTPUser").Valor;
                String WsSMTPPassword = DALCentralAplicaciones.Utilidades.Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WsSMTPPassword").Valor;
                String WsSMTPPort = DALCentralAplicaciones.Utilidades.Configuracion.Get(new Guid(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WsSMTPPort").Valor;


                DataSet tokenData = DAOAdministrarColectivas.ConsultaTokenById(tokenId, user, AppId);


                if(tokenData.Tables == null)
                {
                    throw new Exception("No se localizó la información del token");
                }

                if (tokenData.Tables.Count == 0)
                {
                    throw new Exception("No se localizó la información del token");
                }

                string refreshToken = tokenData.Tables[0].Rows[0]["token"].ToString();
                string from = WsSMTPUser;

                MailMessage message = new MailMessage();

                message.From = new MailAddress(from);

                string[] listaDist = emails.Split(';');

                if (listaDist.Length == 0)
                {
                    Loguear.Error(new CAppException(500,"Lista de distribucion vacia"), "");
                    return;
                }

                foreach (String toSend in listaDist)
                {
                    if (toSend.Contains("@"))
                    {
                        message.To.Add(new MailAddress(toSend));
                    }
                }


                message.Subject = "Consulta de Refresh Token";
                message.Body = String.Format("El refresh token asociado es : {0}", refreshToken);
                SmtpClient client = new SmtpClient(WsSMTP);
                client.Port = Convert.ToInt32(WsSMTPPort);
                client.Credentials = new NetworkCredential(WsSMTPUser, WsSMTPPassword);

                client.Send(message);

            }
            catch (Exception ex)
            {
                throw new Exception("SendTokenByEmail(). Falla al enviar el token por email: " + ex);
            }
        }

        public static void DeleteToken(int tokenId,Usuario usuario)
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
                            DAOAdministrarColectivas.DeleteTokenById(tokenId, usuario, conn, transaccionSQL);
                            transaccionSQL.Commit();
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }

                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("DeleteToken(). Falla al momento de eliminar un token: " + ex);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar el estatus del producto plugin de la colectiva
        /// en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdProductoPlugin">Identificador del producto plugin</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="Estatus">Estatus del producto plugin</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaEstatusProductoPluginColectiva(int IdProductoPlugin, Int64 IdColectiva, int Estatus,
            Usuario elUsuario)
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
                            DAOAdministrarColectivas.InsertaActualizaEstatusProductoPluginColectiva(IdProductoPlugin,
                                IdColectiva, Estatus, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ModificaEstatusProductoPluginColectiva() Falla al modificar el estatus del producto plugin de la colectiva en el Autorizador", err);
                        }
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear un nuevo producto plugin de tipo Smart Points (DIESTEL),
        /// y lo relaciona con la colectiva en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="SKU">SKU del nuevo producto plugin</param>
        /// <param name="BIN">BIN del nuevo producto plugin</param>
        /// <param name="Clave">Clave del nuevo producto plugin</param>
        /// <param name="Descripcion">Descripción del nuevo producto plugin</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void CreaProductoPluginColectiva(string SKU, string BIN, string Clave, string Descripcion,
            Int64 IdColectiva, Usuario elUsuario)
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
                            DAOAdministrarColectivas.InsertaProductoPluginAColectiva(SKU, BIN, Clave, Descripcion,
                                IdColectiva, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "CreaProductoPluginSmartPoints() Falla al insertar el producto plugin en el Autorizador", err);
                        }
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Inserta o actualizar la Cuenta CLABE de la colectiva en el Autorizador
        /// </summary>
        /// <param name="idColectiva">ID de la Colectiva</param>
        /// <param name="CLABE">Cuenta CLABE a actualizar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizarCuentaCLABEColectiva(long idColectiva, string CLABE, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            log.Info("INICIA ActualizaCuentaCLABE()");
                            int Id_MA = DAOAdministrarColectivas.ActualizaCuentaCLABE(idColectiva, CLABE, elUsuario,
                                conn, transaccionSQL, logHeader);
                            log.Info("TERMINA ActualizaCuentaCLABE()");

                            log.Info("INICIA InsertaRegistroEnBitacora()");
                            DAOAdministrarColectivas.InsertaRegistroEnBitacora("web_CA_ActualizaCLABEPorColectiva",
                                "MediosAcceso", "ClaveMA", Id_MA, CLABE, elUsuario, conn, transaccionSQL, logHeader);
                            log.Info("TERMINA InsertaRegistroEnBitacora()");

                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception ex)
                        {
                            transaccionSQL.Rollback();
                            log.ErrorException(ex);
                            throw new CAppException(8011, "Falla al actualizar la Cuenta CLABE de la Colectiva");
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
                throw new CAppException(8011, "Falla en el método de actualización de la Cuenta CLABE de la Colectiva."); 
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear un nuevo elemento al catálogo de parámetros
        /// multiasignación de la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdParametro">Idetnificador del parámetro multiasignación</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="Clave">Clave del nuevo elemento del catálogo</param>
        /// <param name="Descripcion">Descripción del nuevo elemento del catálogo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaNuevoItemCatalogoPMAColectiva(long IdParametro, int IdColectiva, string Clave,
            string Descripcion, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                string msjResp = DAOAdministrarColectivas.InsertaItemCatalogoPMAColectiva(IdParametro, 
                    IdColectiva, Clave, Descripcion, elUsuario, logHeader);

                if (!msjResp.Contains("OK"))
                {
                    throw new CAppException(8011, msjResp);
                }
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el nuevo elemento del catálogo de la colectiva");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para activar o desactivar un elemento del catálogo
        /// en el Autorizador
        /// </summary>
        /// <param name="idCatalogo">Identificador del elemento del catálogo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ActivaDesactivaItemCatalogo(int idCatalogo, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.ActualizaEstatusItemCatalogoPMA(idCatalogo, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus del elemento del catálogo");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro a una Colectiva en el Autorizador,
        /// </summary>
        /// <param name="IdValorParametro">Idetnificador del parametro</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AgregaParametroAdicionalColectiva(int IdParametro, long IdColectiva, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarColectivas.InsertaValorParametroAdicional(IdParametro, IdColectiva, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al asignar el Parámetro a la Colectiva");
            }
        }
    }
}
