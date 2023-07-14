using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para los Parámetros Multiasignación de Entidades
    /// </summary>
    public class LNParametros
    {
        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación de un parámetro multiasignación
        /// </summary>
        /// <param name="elParametro">Datos de la entidad ParametroMA por crear o modificae</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaPMA(ParametroMA elParametro, Usuario usuario)
        {
            try
            {
                //Se valida que estén capturados todos los campos editables y requeridos para el cambio en BD
                if (String.IsNullOrEmpty(elParametro.ValorPMA) || elParametro.ID_Vigencia == 0)
                {
                    throw new CAppException(8006, "Proporciona Todos los Datos para Modificar el Parámetro");
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOParametroMA.InsertaOModificaPMA(elParametro, usuario);
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
                            throw new CAppException(8006, "Falla al Desactivar Validación en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación del valor de un parámetro 
        /// multiasignación que requiere de una autorización
        /// </summary>
        /// <param name="idValorPMA">Identificador del valor ParametroMA</param>
        /// <param name="ValorPorAutorizar">Valor por autorizar del ParametroMA</param>
        /// <param name="PaginaAspx">Usuario en sesión</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaModificaValorPMAPorAutorizar(long IdValorPMA, string ValorPorAutorizar, string PaginaAspx,
            Usuario usuario, Guid AppID, ILogHeader logHeader)
        {
            try
            {
                //Se valida que se haya ingresado un valor
                if (String.IsNullOrEmpty(ValorPorAutorizar))
                {
                    throw new CAppException(8006, "Proporciona el valor del Parámetro");
                }

                DAOParametroMA.InsertaActualizaValorPMAPorAutorizar(IdValorPMA, ValorPorAutorizar, PaginaAspx, usuario, 
                    AppID, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar el valor por autorizar del parámetro");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación y autorización del valor de un parámetro
        /// multiasignación
        /// </summary>
        /// <param name="unParametro">Objeto del parámetro por modificar</param>
        /// <param name="EsAutorYEjec">Bandera que indica que el usuario en sesión tiene los roles de Autorizador
        /// y Ejecutor</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaYAutorizaValorParametro(ParametroValor unParametro, bool EsAutorYEjec,
            Usuario elUser, ILogHeader logHeader)
        {
            try
            {
                DAOParametroMA.ActualizaYAutorizaValorParametro(unParametro, EsAutorYEjec, elUser, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al modificar y autorizar el valor del parámetro");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para rechazar el cambio de valor de un parámetro
        /// multiasignación
        /// </summary>
        /// <param name="IdValorParametro">Identificador del valor del parámetro</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RechazaValorParametroPorAutorizar(long IdValorParametro, ILogHeader logHeader)
        {
            try
            {
                DAOParametroMA.EliminaValorParametroPorAutorizar(IdValorParametro, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al rechazar el valor por autorizar del parámetro");
            }
        }
    }
}