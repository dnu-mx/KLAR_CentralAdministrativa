using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALLealtad.BaseDatos;
using Interfases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DALLealtad.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio del nivel de acceso a datos
    /// </summary>
    public class LNReglasLealtad
    {
        /// <summary>
        /// Establece las condiciones de validación para la modificación de los valores de los parámetros
        /// de la regla indicada, para el grupo de cuentas indicado
        /// </summary>
        /// <param name="losValoresRegla">Lista con los valores de la regla por modificar</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="ID_Regla">Identificador de la regla</param>
        /// <param name="IdGpoCuenta">Identificador del grupo de cuentas</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ModificaValoresReglaPorGpoCuenta(List<ValorRegla> losValoresRegla, Int64 Id_cadena, Int64 ID_Regla,
            int IdGpoCuenta, IUsuario elUser)
        {
            try
            {
                using (SqlConnection conn = BDAutorizadorCash.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (ValorRegla unValor in losValoresRegla)
                            {
                                DAOReglasLealtad.ActualizaValorReglaPorGpoCuenta(unValor, Id_cadena, ID_Regla,
                                    IdGpoCuenta, conn, transaccionSQL, elUser);
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
                throw new Exception("Ha sucedido un error al actualizar los valores de la Regla: " + err);
            }
        }
    }
}
