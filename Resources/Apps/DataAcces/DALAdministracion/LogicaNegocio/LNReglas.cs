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
    /// Establece la lógica de negocio para las Reglas Multiasignación de Entidades
    /// </summary>
    public class LNReglas
    {
        ///// <summary>
        ///// Establece las condiciones de validación para la modificación del orden de ejecución
        ///// de una regla multiasignación
        ///// </summary>
        ///// <param name="ordenEjecucion">Valor del nuevo orden de ejecución de la RMA</param>
        ///// <param name="usuario">Usuario en sesión</param>
        ///// <param name="AppID">Identificador de la aplicación</param>
        //public static void ModificaOrdenEjecRMA(int IDRegla, int ordenEjecucion, Usuario usuario)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = BDAutorizador.BDEscritura)
        //        {
        //            conn.Open();

        //            using (SqlTransaction transaccionSQL = conn.BeginTransaction())
        //            {
        //                try
        //                {
        //                    DAOReglaMA.ActualizaOrdenEjecRMA(IDRegla, ordenEjecucion, usuario);
        //                    transaccionSQL.Commit();
        //                }
        //                catch (CAppException err)
        //                {
        //                    transaccionSQL.Rollback();
        //                    throw err;
        //                }
        //                catch (Exception err)
        //                {
        //                    transaccionSQL.Rollback();
        //                    throw new CAppException(8006, "Falla al Desactivar Validación en Base de Datos ", err);
        //                }
        //            }
        //        }
        //    }
        //    catch (CAppException err)
        //    {
        //        Loguear.Error(err, usuario.ClaveUsuario);
        //        throw err;
        //    }
        //    catch (Exception err)
        //    {
        //        Loguear.Error(err, usuario.ClaveUsuario);
        //        throw err;
        //    }
        //}

        /// <summary>
        /// Establece las condiciones de validación para la creación o modificación
        /// de una regla multiasignación
        /// </summary>
        /// <param name="dtReglasMA">Tabla con los valores de las reglas por crear o modificar</param>
        /// <param name="cadena">Identificador de la cadena coemrcial</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaModificaRMA(DataTable dtReglasMA, int cadena, Usuario usuario)
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
                            ReglaMA laReglaMA = new ReglaMA();

                            for (int iRow = 0; iRow < dtReglasMA.Rows.Count; iRow++)
                            {
                                laReglaMA.ID_Regla = int.Parse(dtReglasMA.Rows[iRow]["ID_Regla"].ToString());
                                laReglaMA.ID_ReglaMultiasignacion = int.Parse(dtReglasMA.Rows[iRow]["ID_ReglaMultiasignacion"].ToString());
                                laReglaMA.Prioridad = int.Parse(dtReglasMA.Rows[iRow]["Prioridad"].ToString());
                                laReglaMA.ID_Vigencia = int.Parse(dtReglasMA.Rows[iRow]["ID_Vigencia"].ToString());
                                laReglaMA.ID_CadenaComercial = cadena;
                                laReglaMA.ID_Producto = int.Parse(dtReglasMA.Rows[iRow]["ID_Producto"].ToString());
                                laReglaMA.OrdenEjecucionRMA = int.Parse(dtReglasMA.Rows[iRow]["OrdenEjecucion"].ToString());

                                DAOReglaMA.InsertaActualizaRMA(laReglaMA, usuario);
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
                            throw new CAppException(8006, "Falla al Desactivar Validación en Base de Datos ", err);
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