using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para la entidad Producto (Grupo de Medios de Acceso)
    /// </summary>
    public class LNProductos
    {
        /// <summary>
        /// Establece las condiciones de validación para la creación de un nuevo producto
        /// </summary>
        /// <param name="nuevoGMA">Nuevo Grupo de Medios de Acceso</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ValidaNuevoProducto(GrupoMA nuevoGMA, Usuario usuario)
        {
            //Se valida que estén todos los datos capturados para poder almacenar el registro
            if(String.IsNullOrEmpty(nuevoGMA.ClaveGrupo)
                || String.IsNullOrEmpty(nuevoGMA.Descripcion)
                || String.IsNullOrEmpty(nuevoGMA.ID_Vigencia.ToString()))
            {
                throw new CAppException(8006, "Proporciona Todos los Datos para Crear un Nuevo Producto");
            }

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOGruposMA.insertar(nuevoGMA, usuario);
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
                            throw new CAppException(8006, "Falla al Insertar nuevo GMA en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la actualización de un producto
        /// </summary>
        /// <param name="elGMA">Entidad GrupoMA con los datos por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaProducto(GrupoMA elGMA, Usuario usuario)
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
                            DAOGruposMA.actualiza(elGMA, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar GMA en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la actualización de parámetros del producto
        /// </summary>
        /// <param name="losParametros">Datos de los parámetros que cambiaron</param>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametrosProducto(List<ParametroGMA> losParametros, Int64 idProducto, Usuario elUser)
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
                                foreach (ParametroGMA parametro in losParametros)
                                {
                                    DAOGruposMA.ActualizaParametrosProducto(parametro, idProducto, elUser);
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
        /// Establece las condiciones de validación para la actualización de rangos del producto
        /// </summary>
        /// <param name="dtRangos">Tabla con los rangos</param>
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
                            DAOGruposMA.ActualizaRangosProducto(IdRango, RangoInicial, RangoFinal, usuario);
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

        /// <summary>
        /// Establece las condiciones de validación para la asignación del parámetro
        /// al producto y cadena comercial en base de datos
        /// </summary>
        /// <param name="idPMA">Identificador del parámetro multiasignación</param>
        /// <param name="idProd">Identificador del producto</param>
        /// <param name="idCad">Identificador de la cadena comercial</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void AsignaParametroACadena(int IdParametroMA, int IdCadena, int IdProducto, Usuario usuario)
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
                            DAOProductos.InsertaParametroACadena(IdParametroMA, IdCadena, IdProducto,
                                usuario, conn, transaccionSQL);
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
                            throw new CAppException(8006, "Falla al Asignar el Parámetro al Producto y Cadena en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la modificación de valores del parámetro
        /// multiasignación en base de datos
        /// </summary>
        /// <param name="IdValorParametroMA">Identificador del valor parámetro multiasignación</param>
        /// <param name="IdEntidad">Identificador de la entidad</param>
        /// <param name="IdRegistroEntidad">Identificador del registro entidad</param>
        /// <param name="Valor">Nuevo valor del parámetro</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <returns>Cadena con la respuesta a la modificación (exitosa o mensaje de error)</returns>
        public static String ModificaValorParametroMultiasignacion(Int64 IdValorParametroMA, int IdEntidad, 
            Int64 IdRegistroEntidad, string Valor, Usuario usuario)
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
                            string resp = DAOProductos.ActualizaValorParametroMultiasignacion(IdValorParametroMA, IdEntidad,
                                IdRegistroEntidad, Valor, usuario, conn, transaccionSQL);
                            transaccionSQL.Commit();

                            return resp;
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar los datos del Parámetro en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la baja de los valores del parámetro
        /// multiasignación en base de datos
        /// </summary>
        /// <param name="IdValorParametroMA">Identificador del valor parámetro multiasignación</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void BajaValorParametroMultiasignacion(Int64 IdValorParametroMA, Usuario usuario)
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
                            DAOProductos.EliminaValorParametroMultiasignacion(IdValorParametroMA, usuario,
                                conn, transaccionSQL);
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
                            throw new CAppException(8006, "Falla al Quitar el Parámetro al Producto y Cadena en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la inserción de un nuevo parámetro
        /// multiasignación en base de datos
        /// </summary>
        /// <param name="ClavePMA">Clave del parámetro multiasignación</param>
        /// <param name="Descripcion">Descripción del parámetro multiasignación</param>
        /// <param name="TipoDatoJava">Tipo de dato Java</param>
        /// <param name="TipoDatoSQL">Tipo de dato SQL</param>
        /// <param name="ValorDefault">Valor por defecto del parámetro multiasignación</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaNuevoParametroMultiasignacion(string ClavePMA, string Descripcion, string TipoDatoJava,
            string TipoDatoSQL, string ValorDefault, Usuario usuario)
        {
            try
            {
                //Se verifica que el nombre del nuevo parámetro inicie forzosamente con @
                if (!ClavePMA.StartsWith("@"))
                {
                    throw new CAppException(8006, "La clave del parámetro debe iniciar forzosamente con @");
                }

                using (SqlConnection conn = BDAutorizadorMC.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            string resp = DAOProductos.InsertaParametroMultiasignacion(ClavePMA, Descripcion, TipoDatoJava,
                                TipoDatoSQL, ValorDefault, usuario, conn, transaccionSQL);

                            if (!String.IsNullOrEmpty(resp))
                            {
                                throw new CAppException(8006, resp);
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
                            throw new CAppException(8006, "Falla al Crear el Parámetro en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la modificación del tipo de integración
        /// del grupo de medios de acceso en el Autorizador, controlando la transacción
        /// (commit o rollback)
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del grupo de medios de acceso</param>
        /// <param name="IdTipoIntegracion">Identificador del nuevo tipo de integración</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaTipoIntegracionGrupoMA(int IdGrupoMA, int IdTipoIntegracion, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                using (conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOGruposMA.ActualizaTipoIntegracionGrupoMA(IdGrupoMA, IdTipoIntegracion, 
                                conn, transaccionSQL, usuario);
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
                            throw new CAppException(8006, "ModificaTipoIntegracionGrupoMA() Falla al modificar el tipo de saldo del GrupoMA en el Autorizador", err);
                        }
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