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
    public class LNEcommercePrana
    {
        public static void InsertaArchivoTMP(DataTable dtFileToImport, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
              conn = BDEcommercePrana.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommercePrana.InsertaPromocionesTMP(dtFileToImport, conn, transaccionSQL, usuario);
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
                var data = BDEcommercePrana.BDEscritura;
             
                using (conn = data)
                {
                    conn.Open();

                    var result = DAOEcommercePrana.AplicaCambiosAProductos(conn, usuario);

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
                conn = BDEcommercePrana.BDEscritura;

                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        DAOEcommercePrana.InsertaSucursalesTMP(dtSucursales, conn, transaccionSQL, usuario);
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
                var data = BDEcommercePrana.BDEscritura;
             
                using (conn = data)
                {
                    conn.Open();

                    var result = DAOEcommercePrana.InsertaActualizaSucursales(conn, elUser);

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

        /// <summary>
        /// Establece las condiciones de validación para crear una nueva promoción inactiva en base de datos,
        /// sólo con sus datos base, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdCadena">Identificador de la cadena a la que se asocia la nueva promoción</param>
        /// <param name="ClavePromocion">Clave de la nueva promoción</param>
        /// <param name="DescripcionPromocion">Descripción de la nueva promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable CreaNuevaPromoInactiva(int IdCadena, string ClavePromocion, string DescripcionPromocion,
            string PalabrasClave, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable _dtPromo = DAOEcommercePrana.InsertaPromocionInactiva(IdCadena, ClavePromocion,
                                DescripcionPromocion, PalabrasClave, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return _dtPromo;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaNuevaPromoInactiva() Falla al insertar la nueva promoción en base de datos", err);
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
        /// Establece las condiciones de validación para crear una nueva cadena en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="nuevaCadena">Entidad Cadena con los datos de la nueva cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable CreaNuevaCadena(Cadena nuevaCadena, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable _dt = DAOEcommercePrana.InsertaCadena(nuevaCadena, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return _dt;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaNuevaCadena() Falla al insertar la nueva cadena en base de datos", err);
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
        /// Establece las condiciones de validación para modificar los datos de una promoción en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="laPromocion">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaPromocion(Promocion laPromocion, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEcommercePrana.ActualizaPromocion(laPromocion, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ModificaPromocion() Falla al actualizar la promoción en base de datos", err);
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
        /// Establece las condiciones de validación para modificar los programas de una promoción
        /// en base de datos, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="programas">Programas que se añaden a la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaProgramasPromocion(int IdPrograma, List<Programa> programas, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (Programa programa in programas)
                            {
                                DAOEcommercePrana.ActualizaProgramaPromocion(IdPrograma, programa.ClavePrograma,
                                    programa.Activo, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ModificaProgramasPromocion() Falla al actualizar el programa de la promoción en base de datos", err);
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
        /// Establece las condiciones de validación para modificar los datos de una cadena en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="laCadena">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaCadena(Cadena laCadena, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEcommercePrana.ActualizaCadena(laCadena, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ModificaCadena() Falla al actualizar la cadena en base de datos", err);
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
        /// Establece las condiciones de validación para crear una nueva sucursal en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="nuevaSucursal">Entidad Cadena con los datos de la nueva cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable CreaNuevaSucursal(Sucursal nuevaSucursal, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable dtSuc = DAOEcommercePrana.InsertaSucursal(nuevaSucursal, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return dtSuc;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaNuevaSucursal() Falla al insertar la nueva sucursal en base de datos", err);
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
        /// Establece las condiciones de validación para modificar los datos de una sucursal en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="unaSucursal">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaSucursal(Sucursal unaSucursal, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEcommercePrana.ActualizaSucursal(unaSucursal, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ModificaSucursal() Falla al actualizar la sucursal en base de datos", err);
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
        /// Actualiza el estatus de ExisteLogo por Promociones/Programas
        /// </summary>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="idTipoCupon">Identificador del tipo de cupón</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaExisteLogoCadenas(string Cadenas, string Programa, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEcommercePrana.ActualizaExisteLogo(Cadenas, Programa, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ActualizaExisteLogoCadenas() Falla al actualizar ExisteLogo cadenas en base de datos", err);
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
        /// Establece las condiciones de validación para crear una nueva cadena en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="NuevaCampana">Entidad Cadena con los datos de la nueva cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable CreaNuevaCampana(Campana NuevaCampana, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable _dt = DAOEcommercePrana.InsertaCampana(NuevaCampana, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return _dt;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaNuevaCampana() Falla al insertar la nueva campaña en base de datos", err);
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
        /// Establece las condiciones de validación para modificar los datos de una campaña en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="Campana">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaCampana(Campana Campana, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEcommercePrana.ActualizaCampana(Campana, conn, transaccionSQL, elUsuario);
                            DAOEcommercePrana.ActualizaPromocionesCampana(Campana.ID_Campana, "", conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "ModificaCampana() Falla al actualizar la campaña en base de datos", err);
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
        /// Asigna las promociones de una campaña en base de datos, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdCampana">Identificador de la campana</param>
        /// <param name="Promociones">Promociones separadas por comas que se asignarán a la campana</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void AplicaPromociones(int IdCampana, string Promociones, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOEcommercePrana.ActualizaPromocionesCampana(IdCampana, Promociones, conn, transaccionSQL, elUsuario);
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
                            throw new CAppException(8006, "AplicaPromociones() Falla al actualizar las promociones en base de datos", err);
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
        /// Establece las condiciones de validación para crear un nuevo Objeto en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="NuevoObjeto">Entidad Objeto con los datos de la nueva cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable CreaNuevoObjeto(ObjetoPrograma objetoPrograma, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = DAOEcommercePrana.InsertaObjeto(objetoPrograma, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return dt;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaNuevoObjeto() Falla al insertar la nuevo Objeto en base de datos", err);
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
        /// Establece las condiciones para modificar o eliminar un Objeto en base de datos,
        /// </summary>
        /// <param name="objetoPrograma">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static DataTable ModificaObjeto(ObjetoPrograma objetoPrograma, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = DAOEcommercePrana.ActualizaObjeto(objetoPrograma, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return dt;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "ModificaObjeto() Falla al actualizar el Objeto en base de datos", err);
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
        /// Establece las condiciones de validación para crear un nuevo SubGiro en base de datos,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="objetoSubGiro">Entidad Subgiro con los datos de la nuevo subgiro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los resultados respuesta del SP</returns>
        public static DataTable CreaNuevoSubGiro(SubGiro objetoSubGiro, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = DAOEcommercePrana.InsertaSubGiro(objetoSubGiro, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return dt;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "CreaNuevoSubGiro() Falla al insertar la nuevo SubGiro en base de datos", err);
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
        /// Establece las condiciones para modificar o eliminar un SubGiro en la base de datos,
        /// </summary>
        /// <param name="objetoSubGiro">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static DataTable ModificaSubGiro(SubGiro subGiro, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommercePrana.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = DAOEcommercePrana.ActualizaSubGiro(subGiro, conn, transaccionSQL, elUsuario);
                            transaccionSQL.Commit();

                            return dt;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "ModificaSubGiro() Falla al actualizar el SubGiro en base de datos", err);
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
