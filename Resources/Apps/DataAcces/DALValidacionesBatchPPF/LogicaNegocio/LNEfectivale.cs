using DALAutorizador.BaseDatos;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Entidades;
using DALValidacionesBatchPPF.Utilidades;
using Interfases;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALValidacionesBatchPPF.LogicaNegocio
{
    public class LNEfectivale
    {
        /// <summary>
        /// Establece las condiciones de validación para el bloqueo de la tarjeta
        /// en la conexión a base de datos
        /// </summary>
        /// <param name="NumTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void BloqueaTarjeta(String NumTarjeta, IUsuario elUser)
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
                            DAOEfectivaleOnline.ActualizaEstatusTarjeta_Bloqueada(NumTarjeta, elUser);
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al bloquear la Tarjeta: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el desbloqueo de la tarjeta
        /// en la conexión a base de datos
        /// </summary>
        /// <param name="NumTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void DesbloqueaTarjeta(String NumTarjeta, IUsuario elUser)
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
                            DAOEfectivaleOnline.ActualizaEstatusTarjeta_Desbloqueada(NumTarjeta, elUser);
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al bloquear la Tarjeta: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la cancelación de la tarjeta
        /// en la conexión a base de datos
        /// </summary>
        /// <param name="NumTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void CancelaTarjeta(String NumTarjeta, IUsuario elUser)
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
                            DAOEfectivaleOnline.ActualizaEstatusTarjeta_Cancelada(NumTarjeta, elUser);
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al bloquear la Tarjeta: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el cambio de estatus de la tarjeta
        /// en la conexión a base de datos
        /// </summary>
        /// <param name="IdNuevoEstatus">Identificador del nuevo estatus</param>
        /// <param name="NumTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static string ModificaEstatusTarjeta(int IdNuevoEstatus, String NumTarjeta, IUsuario elUser)
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
                            string s = DAOEfectivaleOnline.ActualizaEstatusTarjeta(IdNuevoEstatus, NumTarjeta, conn,
                                transaccionSQL, elUser);
                            transaccionSQL.Commit();

                            return s;
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para el cierre del caso en la
        /// conexión a base de datos
        /// </summary>
        /// <param name="IdDictamen">Identificador del dictamen</param>
        /// <param name="IdTipoFraude">Identificador del tipo de fraude</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void CierraCaso(String ClaveDictamen, int IdTipoFraude, String Tarjeta, IUsuario elUser)
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
                            DAOEfectivaleOnline.InsertaCasoCerrado(conn, transaccionSQL, 
                                ClaveDictamen, IdTipoFraude, Tarjeta, elUser);
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al cerrar el caso: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación de una 
        /// operación respuesta regla en la conexión a base de datos
        /// </summary>
        /// <param name="dtEventoReglas">Tabla con IDs de las operaciones por modificar</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ModificaOperacionRespuestaRegla(DataTable dtOperaciones, IUsuario elUser)
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
                            foreach (DataRow row in dtOperaciones.Rows)
                            {
                                DAOEfectivaleOnline.ActualizaOperacionRespuestaRegla(
                                    int.Parse(row["ID_Operacion"].ToString()), elUser);
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al marcar la operación: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del valor
        /// de un parámetro - regla en la conexión a base de datos
        /// </summary>
        /// <param name="unValor">Instancia con los datos del nuevo valor del parámetro</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ModificaValorParametroRegla(ValorParametroMARegla unValor, IUsuario elUser)
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
                            DAOEfectivaleOnline.ActualizaValorParametroRegla(conn, transaccionSQL,
                                unValor, elUser);
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al modificar el parámetro: " + err);
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para marcar una operación como incidencia manual
        /// en la conexión a base de datos
        /// </summary>
        /// <param name="Id_Operacion">ID de la operación</param>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="Comentarios">Comentarios de la incidencia manual</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <returns>Cadena con la respuesta de base de datos</returns>
        public static string MarcaOperacionIncidencia(Int64 Id_Operacion, String Tarjeta,
            String Comentarios, IUsuario elUser)
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
                            string resp = DAOEfectivaleOnline.InsertaIncidenciaManual(conn, transaccionSQL,
                                Id_Operacion, Tarjeta, Comentarios, elUser);
                            transaccionSQL.Commit();

                            return resp;
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
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al marcar la operación como incidencia: " + err);
            }
        }
    }
}
