
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALLealtad.BaseDatos;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALLealtad.LogicaNegocio
{
    public class LNEcommerce
    {
        /// <summary>
        /// Establece las condiciones de validación para modificar el tipo de cupón
        /// de la promoción indicada en base de datos
        /// </summary>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="idTipoCupon">Identificador del tipo de cupón</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaTipoCuponPromo(int idPromocion, int idTipoCupon, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = BDEcommerce.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommerce.ActualizaTipoCuponPromocion(idPromocion, idTipoCupon, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al Actualizar la Promoción en Base de Datos ", ex);
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, usuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, usuario.ClaveUsuario);
                throw ex;
            }

            finally
            {
                if (null != conn && ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la cadena en el Autorizador
        /// </summary>
        /// <param name="ClaveCadena">Clave de la cadena</param>
        /// <param name="Cadena">Nombre de la cadena</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaCadenaEnAutorizador(String ClaveCadena, String Cadena, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = BDAutorizador.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommerce.InsertaActualizaCadenaAut(ClaveCadena, Cadena, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al Actualizar la Promoción en el Autorizador", ex);
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, usuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, usuario.ClaveUsuario);
                throw ex;
            }

            finally
            {
                if (null != conn && ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
        }
    }
}
