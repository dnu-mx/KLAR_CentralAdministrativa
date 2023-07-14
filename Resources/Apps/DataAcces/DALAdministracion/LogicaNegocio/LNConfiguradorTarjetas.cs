using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para el Configurador de Tarjetas Bancarias
    /// </summary>
    public class LNConfiguradorTarjetas
    {
        /// <summary>
        /// Establece las condiciones de validación para la actualización de parámetros del producto
        /// </summary>
        /// <param name="losParametros">Datos de los parámetros que cambiaron</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametrosProducto(List<ParametroGMA_V2> losParametros, Int64 idProducto, Usuario elUser)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(BDAutorizadorMC.strBDEscritura))
                    {
                        conn.Open();

                        using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                        {
                            try
                            {
                                foreach (ParametroGMA_V2 parametro in losParametros)
                                {
                                    DAOConfiguradorTarjetas.ActualizarParametrosProducto(parametro, idProducto, elUser);
                                }

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
                                throw new CAppException(8006, "Falla al Actualizar el Parámetro del Producto en Base de Datos ", err);
                            }
                        }
                    }
                }

                catch (CAppException err)
                {
                    throw err;
                }

                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al Actualizar los Parámetros, no se Realizó la Actualización", err);
                }
            }

            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el control de cambios a los parámetros del producto
        /// </summary>
        /// <param name="losParametros">Datos de los parámetros que cambiaron</param>
        /// <param name="idCadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="paginaASPX">Nombre de la página ASPX que solicita la actualización</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        public static void ControlaCambiosParametrosProducto(List<ParametroGMA_V2> losParametros, Int64 idCadenaComercial,
            Int64 idProducto, String paginaASPX, Usuario elUser, Guid AppID)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(BDAutorizadorMC.strBDEscritura))
                    {
                        conn.Open();

                        using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                        {
                            try
                            {
                                foreach (ParametroGMA_V2 parametro in losParametros)
                                {
                                    DAOConfiguradorTarjetas.ControlarCambiosParametrosProducto(parametro, idCadenaComercial, idProducto,
                                        paginaASPX, elUser, AppID);
                                }

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
                                throw new CAppException(8006, "Falla al establecer el control de cambios del parámetro del producto en Base de Datos ", err);
                            }
                        }
                    }
                }

                catch (CAppException err)
                {
                    throw err;
                }

                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al establecer el control de cambios de los parámetros", err);
                }
            }

            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la autorización al cambio de
        /// valor de los parámetros del producto
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void AutorizaCambiosParametrosProducto(Int64 idProducto, Usuario elUser)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(BDAutorizadorMC.strBDEscritura))
                    {
                        conn.Open();

                        using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                        {
                            try
                            {
                                DAOConfiguradorTarjetas.AutorizarCambiosParametrosProducto(idProducto, elUser);
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
                                throw new CAppException(8006, "Falla al Autorizar los Cambos a los Parámetros del Producto en Base de Datos ", err);
                            }
                        }
                    }
                }

                catch (CAppException err)
                {
                    throw err;
                }

                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al Autorizar los Cambios a los Parámetros, no se Realizó la Actualización", err);
                }
            }

            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de rangos del producto
        /// </summary>
        /// <param name="IdRango">Identificador del rango por actualizar</param>
        /// <param name="RangoInicial">Nuevo valor del rango inicial</param>
        /// <param name="RangoFinal">Nuevo valor del rango final</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaRangosProducto(int IdRango, string RangoInicial, string RangoFinal, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizadorMC.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConfiguradorTarjetas.ActualizarRangosProducto(IdRango, RangoInicial, RangoFinal, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Rangos de Tarjetas en Base de Datos ", err);
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
    }
}