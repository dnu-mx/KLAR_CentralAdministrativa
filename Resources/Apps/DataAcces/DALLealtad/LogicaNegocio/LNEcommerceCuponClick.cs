using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DALLealtad.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio del nivel de acceso a datos de Ecommerce Prana
    /// </summary>
    public class LNEcommerceCuponClick
    {
        /// <summary>
        /// Establece las condiciones de validación para insertar las membresias
        /// del archivo a base de datos
        /// </summary>
        /// <param name="dtmembresias"></param>
        /// <param name="usuario"></param>
        public static void InsertaMembresiasTMP(DataTable dtmembresias, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = BDEcommerceCuponClick.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommerceCuponClick.InsertaMembresiasTMP(dtmembresias, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al Cargar el Archivo en Base de Datos ", ex);
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
        /// Establece las condiciones de validación para los cambios de las membresias 
        /// en base de datos
        /// </summary>
        /// <param name="elUser">Usuario en sesión</param>
        /// <returns>Cadena con el resultado de los cambios en base de datos</returns>
        public static string AplicaCambiosAMembresias(int programa, Usuario elUser)
        {
            SqlConnection conn = null;

            try
            {
                var data = BDEcommerceCuponClick.BDEscritura;
             
                using (conn = data)
                {
                    conn.Open();

                    var result = DAOEcommerceCuponClick.InsertaActualizaMembresias(conn, programa, elUser);

                    return result;
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUser.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUser.ClaveUsuario);
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
