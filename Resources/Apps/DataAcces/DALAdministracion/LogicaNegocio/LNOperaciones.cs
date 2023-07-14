using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para las Operaciones
    /// </summary>
    public class LNOperaciones
    {
        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación
        /// de un evento
        /// </summary>
        /// <param name="ev">Datos de la entidad Evento por crear o modificar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaEvento(Evento ev, Usuario usuario)
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
                            DAOEvento.InsertaActualizaEvento(ev, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar o Insertar Evento en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la creación o modificación
        /// de una regla-evento
        /// </summary>
        /// <param name="dtEventoReglas">Tabla con los valores de las reglas-eventos por crear o modificar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaEventoReglas(DataTable dtEventoReglas, Usuario usuario)
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
                            for (int iRow = 0; iRow < dtEventoReglas.Rows.Count; iRow++)
                            {
                                DAOEvento.InsertaActualizaReglaEvento(
                                    int.Parse(dtEventoReglas.Rows[iRow]["ID_Evento"].ToString()),
                                    int.Parse(dtEventoReglas.Rows[iRow]["ID_Regla"].ToString()),
                                    bool.Parse(dtEventoReglas.Rows[iRow]["Activa"].ToString()),
                                    usuario);
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
                            throw new CAppException(8006, "Falla al Crear/Actualizar Eventos-Reglas en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la creación o modificación
        /// de un plugin-evento
        /// </summary>
        /// <param name="dtEventoPlugins">Tabla con los valores de los plugins-eventos por crear o modificar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaEventoPlugins(DataTable dtEventoPlugins, Usuario usuario)
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
                            for (int iRow = 0; iRow < dtEventoPlugins.Rows.Count; iRow++)
                            {
                                EventoPlugin eventoPIn = new EventoPlugin();

                                eventoPIn.ID_Evento = int.Parse(dtEventoPlugins.Rows[iRow]["ID_Evento"].ToString());
                                eventoPIn.ID_Plugin = int.Parse(dtEventoPlugins.Rows[iRow]["ID_Plugin"].ToString());
                                eventoPIn.Activo = bool.Parse(dtEventoPlugins.Rows[iRow]["Activo"].ToString());
                                eventoPIn.OrdenEjecucion = int.Parse(dtEventoPlugins.Rows[iRow]["OrdenEjecucion"].ToString());
                                eventoPIn.RespuestaISO = bool.Parse(dtEventoPlugins.Rows[iRow]["EsRespuestaISO"].ToString());
                                eventoPIn.ObligatorioEnReverso = bool.Parse(dtEventoPlugins.Rows[iRow]["EsObligatorioParaReverso"].ToString());

                                DAOEvento.InsertaActualizaPluginEvento(eventoPIn, usuario);
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
                            throw new CAppException(8006, "Falla al Crear/Actualizar Eventos-Reglas en Base de Datos ", err);
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