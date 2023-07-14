using DALCentralAplicaciones.Entidades;
using DALComercioElectronico.BaseDatos;
using DALComercioElectronico.Utilidades;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALComercioElectronico.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio del acceso a datos de Moshi-Moshi
    /// </summary>
    public class LNMoshi
    {
        /// <summary>
        /// Establece las condiciones de validación en la conexión a base de datos para 
        /// la inserción del archivo en la tabla temporal
        /// </summary>
        /// <param name="dtFileToImport">Información por importar, extraída del archivo</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void InsertaArchivoTMP(DataTable dtFileToImport, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection (BDCommerce.strBDEscritura);
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOMoshi.InsertaProductosTMP(dtFileToImport, conn, transaccionSQL, usuario);
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
        /// Establece las condiciones de validación en la conexión a base de datos para 
        /// la inserción de valores de atributos desde la tabla temporal
        /// </summary>
        /// <param name="usuario">Usuario en sesión</param>
        public static void InsertaValoresAtributos(Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(BDCommerce.strBDEscritura);
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOMoshi.InsertaValAtribTMP(conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al Insertar los Valores de los Atributos en Base de Datos ", ex);
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
        /// Establece las condiciones de validación en la conexión a base de datos para 
        /// cargar los cambios al catálogo de productos
        /// </summary>
        /// <param name="usuario"></param>
        public static string AplicaCambios(Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                var data = BDCommerce.strBDEscritura;
                using (conn = new SqlConnection(BDCommerce.strBDEscritura))
                {
                    conn.Open();

                    var result = DAOMoshi.AplicaCambiosAProductos(conn, usuario);

                    return result;
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
        /// Establece las condiciones de validación en la conexión a base de datos para 
        /// la modificación de la disponibilidad de un producto en la sucursal
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <param name="DispActual">Bandera de disponibilidad actual del producto</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaDisponibilidadProducto(int IdProducto, int IdSucursal, int DispActual, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(BDCommerce.strBDEscritura);
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOMoshi.ActualizaEstatusProducto(conn, transaccionSQL, IdProducto, IdSucursal, 
                            DispActual, usuario);
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
                        throw new CAppException(8006, "Falla al Modificar la Disponibilidad del Producto en la Sucursal en Base de Datos ", ex);
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
