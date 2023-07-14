using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALEventos.LogicaNegocio;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para el Producto
    /// </summary>
    public class LNInfoOnBoarding
    {
        /// <summary>
        /// Inserta nuevo Nodo
        /// </summary>
        /// <param name="elNodo">Datos del nuevo Nodo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable CreaNuevoNodo(Nodo elNodo, ParametroNodo elParametro, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOInfoOnBoarding.InsertaNodo(elNodo, elParametro, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el Nodo");
            }
        }

        /// <summary>
        /// Actualiza Nodo existente
        /// </summary>
        /// <param name="elNodo">Datos del nuevo Nodo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static string ModificaNodo(Nodo elNodo, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOInfoOnBoarding.ActualizaNodo(elNodo, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el Nodo");
            }
        }

        /// <summary>
        /// Inserta nuevo Parámetro
        /// </summary>
        /// <param name="elParametro">Datos del nuevo Parámetro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable CreaNuevoParametro(ParametroNodo elParametro, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOInfoOnBoarding.InsertaParametro(elParametro, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el Parámetro");
            }
        }

        /// <summary>
        /// Inserta nuevo Parámetro
        /// </summary>
        /// <param name="elParametro">Datos del nuevo Parámetro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static string ModificaParametro(int idValorNodo, string KeyParametro, string DescParametro, bool esActivo, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOInfoOnBoarding.ActualizaParametro(idValorNodo, KeyParametro, DescParametro, esActivo, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al actualizar el Parámetro");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un registro de bitacora detalle en base de datos
        /// </summary>
        /// <param name="SP">Nombre del procedimiento almacenado que realizó la inserción/actualización</param>
        /// <param name="Tabla">Nombre de la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="Campo">Nombre del campo en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="IdRegistro">Identificador del registro en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="Valor">Valor del campo en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraEnBitacora(string SP, string Tabla, string Campo, string IdRegistro, string Valor,
            string Observaciones, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            pCI.Info("INICIA InsertaRegistroBitacoraDetalle()");
                            LNEvento.InsertaRegistroBitacoraDetalle(SP, Tabla, Campo, IdRegistro, Valor, Observaciones,
                                conn, transaccionSQL, elUsuario, logHeader);
                            pCI.Info("TERMINA InsertaRegistroBitacoraDetalle()");

                            transaccionSQL.Commit();
                        }
                        catch (CAppException caEx)
                        {
                            transaccionSQL.Rollback();
                            throw caEx;
                        }
                        catch (Exception ex)
                        {
                            transaccionSQL.Rollback();
                            pCI.ErrorException(ex);
                            throw new CAppException(8011, "Falla al registrar en bitácora");
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
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar en bitácora");
            }
        }
    }
}