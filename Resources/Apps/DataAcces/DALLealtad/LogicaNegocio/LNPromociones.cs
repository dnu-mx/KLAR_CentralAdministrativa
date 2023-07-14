using DALAutorizador.BaseDatos;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using DALLealtad.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALLealtad.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio del nivel de acceso a datos de las Promociones
    /// </summary>
    public class LNPromociones
    {
        /// <summary>
        /// Establece las condiciones de validación para la modificación de la vigencia
        /// de una promoción
        /// </summary>
        /// <param name="dtPertenencias">Tabla con los datos de las pertenencias</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ModificaVigenciaPromocion(DataTable dtPertenencias, IUsuario elUser)
        {
            try
            {
                int IdEvento;
                string Valor;
                bool esFechaInicial;

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            for (int fila = 0; fila < dtPertenencias.Rows.Count; fila++)
                            {
                                IdEvento = int.Parse(dtPertenencias.Rows[fila]["ID_Evento"].ToString());
                                Valor = dtPertenencias.Rows[fila]["Valor"].ToString();
                                esFechaInicial = Convert.ToBoolean(dtPertenencias.Rows[fila]["EsFechaInicial"].ToString());

                                DAOPromociones.ActualizaVigenciaPromocion(IdEvento, Valor, esFechaInicial, elUser);
                            }

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
                throw new Exception("Ha sucedido un error al actualizar la vigencia de la Promoción: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la inserción de sucursales en la tabla de control
        /// para activar una promoción
        /// </summary>
        /// <param name="dtSucursales">DataTable con los registros de las sucursales</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void InsertaSucursalesTemp(DataTable dtSucursales, IUsuario usuario)
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
                        DAOPromociones.InsertaSucursalesAltaPromocion(dtSucursales, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al Cargar las Sucursales en la tabla temporal ", ex);
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
        /// Establece las condiciones de validación para la modificación de las colectivas de la cadena
        /// (cadena, sucursales, afiliaciones y terminales) en el Autorizador
        /// </summary>
        /// <param name="IdCadena">Identificador de la cadena en e-Commerce</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaColectivasCadena(int IdCadena, IUsuario usuario)
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
                        DAOPromociones.InsertaOActualizaColectivasCadena(IdCadena, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al crear la cadena en el Autorizador ", ex);
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
        /// Establece las condiciones de validación para la creación de la promoción de la
        /// e-Commerce en el Autorizador, para el programa elegido
        /// </summary>
        /// <param name="IdCadena">Identificador de la cadena en e-Commerce</param>
        /// <param name="IdPromocion">Identificador de la promoción en e-Commerce</param>
        /// <param name="Programa">Nombre del programa al que se activa la promoción</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaPromocionEnAutorizador(int IdCadena, int IdPromocion, string Programa, IUsuario usuario)
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
                        DAOPromociones.InsertaPromocionEcommerce(IdCadena, IdPromocion, Programa, conn,
                            transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al modificar la promoción en el Autorizador ", ex);
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
        /// Establece las condiciones de validación para la inserción en la tabla layout de operadores
        /// e-Commerce del Autorizador, el objeto recibido por archivo
        /// </summary>
        /// <param name="dtFileToImport">Tabla de datos qe contiene el archivo cargado</param>
        /// <param name="usuario"></param>
        public static void InsertaOperadoresTMP(DataTable dtFileToImport, IUsuario usuario)
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
                        DAOPromociones.InsertaOperadoresTMP(dtFileToImport, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al Cargar el Archivo de Operadores en Base de Datos ", ex);
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
        /// Establece las condiciones de validación para el alta de los usuarios en el Autorizador
        /// </summary>
        /// <param name="usuario">Usuario en sesión</param>
        public static void AltaUsuariosEnAutorizador(IUsuario usuario)
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
                        DAOPromociones.InsertaOperadoresEnAutorizador(conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al insertar los operadores en el Autorizador ", ex);
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
        /// Establece las condiciones de validación para el alta de un usuario en el Autorizador
        /// </summary>
        /// <param name="ClaveCadena">Clave de la cadena</param>
        /// <param name="ClaveSucursal">Clave de la sucursal</param>
        /// <param name="Email">Correo electrónico del operador</param>
        /// <param name="EsGerente">Bandera que indica si el operador será o no gerente</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void AltaUsuarioEnAutorizador(String ClaveCadena, String ClaveSucursal, String Email, 
            int EsGerente, IUsuario usuario)
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
                        DAOPromociones.InsertaOperadorEnAutorizador(ClaveCadena, ClaveSucursal, Email, EsGerente,
                            conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al insertar el operador en el Autorizador ", ex);
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
        /// Establece las condiciones de validación para la eliminación lógica de un usuario en el Autorizador
        /// </summary>
        /// <param name="IdOperador">Identificador del operador</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void EliminaUsuarioEnAutorizador(Int64 IdOperador, IUsuario usuario)
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
                        DAOPromociones.EliminaOperadorEnAutorizador(IdOperador, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al eliminar el operador en el Autorizador ", ex);
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
        /// Establece las condiciones de validación para la modificación del rol del usuario
        /// en base de datos
        /// </summary>
        /// <param name="Email">Correo electrónico (clave) del usuario</param>
        /// <param name="EsGerente">Bandera que indica si el operador será o no gerente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaRolUsuario(String Email, int EsGerente, IUsuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                conn = BDAdminPromociones.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOPromociones.ActualizaRolUsuario(Email, EsGerente, conn, transaccionSQL, usuario);
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
                        throw new CAppException(8006, "Falla al modificar el tipo de acceso del usuario ", ex);
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
        /// Establece las condiciones de validación para la creación de cupones de una promoción masiva
        /// en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="elCupon">Entidad cupón con la configuración de los cupones por generar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>ID de la colectiva (lote) recién creado</returns>
        public static Int64 CreaCuponesPromocionMasiva(Cupon elCupon, IUsuario elUsuario)
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
                            Int64 IdColectiva = DAOPromociones.GeneraCuponesPromoMasiva(elCupon, conn, transaccionSQL,
                                elUsuario);
                            transaccionSQL.Commit();

                            return IdColectiva;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaCuponesPromocionMasiva() Falla al generar los códigos en el Autorizador", err);
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
    }
}
