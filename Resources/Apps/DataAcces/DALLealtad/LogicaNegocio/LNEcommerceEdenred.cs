
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALLealtad.BaseDatos;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALLealtad.LogicaNegocio
{
    public class LNEcommerceEdenred
    {
        public static void InsertaArchivoTMP(DataTable dtFileToImport, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
              conn =BDEcommerceEdenred.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommerceEdenred.InsertaPromocionesTMP(dtFileToImport, conn, transaccionSQL, usuario);
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
        /// cargar los cambios al catálogo de productos
        /// </summary>
        /// <param name="usuario"></param>
        public static string AplicaCambios(Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                // var data = new SqlConnection(BDAutorizador.strBDEscritura);
                var data = BDEcommerceEdenred.BDEscritura;
               // var data = new SqlConnection("Data Source=64.34.163.109;Initial Catalog=Ecommerce_Edenred;Persist Security Info=True;User ID=developer06;Password=osv123456");
               

                using (conn = data)
                {
                    conn.Open();

                    var result = DAOEcommerceEdenred.AplicaCambiosAProductos(conn, usuario);

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
        /// Establece las condiciones de validación para insertar las sucursales 
        /// del archivo a base de datos
        /// </summary>
        /// <param name="dtSucursales"></param>
        /// <param name="usuario"></param>
        public static void InsertaSucursalesTMP(DataTable dtSucursales, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = BDEcommerceEdenred.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommerceEdenred.InsertaSucursalesTMP(dtSucursales, conn, transaccionSQL, usuario);
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
        /// Establece las condiciones de validación para los cambios de las sucursales 
        /// en base de datos
        /// </summary>
        /// <param name="elUser">Usuario en sesión</param>
        /// <returns>Cadena con el resultado de los cambios en base de datos</returns>
        public static string AplicaCambiosASucursales(Usuario elUser)
        {
            SqlConnection conn = null;

            try
            {
                var data = BDEcommerceEdenred.BDEscritura;
             
                using (conn = data)
                {
                    conn.Open();

                    var result = DAOEcommerceEdenred.InsertaActualizaSucursales(conn, elUser);

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
