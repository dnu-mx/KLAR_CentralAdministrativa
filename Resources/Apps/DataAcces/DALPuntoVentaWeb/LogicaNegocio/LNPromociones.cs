using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Utilidades;
using Interfases.Exceptiones;
using System;
using System.Data.SqlClient;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para las Promociones
    /// </summary>
    public class LNPromociones
    {
        /// <summary>
        /// Establece las condiciones de validación para modificar los datos del cupón en base de datos
        /// </summary>
        /// <param name="IdOperacion">Identificador de la operación</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="NumCupon">Número de cupón</param>
        /// <param name="Ticket">Nuevo valor del ticket</param>
        /// <param name="FormaPago">Nueva forma de pago</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaDatosCupon(int IdOperacion, int IdCadena, string NumCupon, string Ticket,
            string FormaPago, Usuario elUsuario)
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
                            DAOPromociones.ActualizaDatosCupon(IdOperacion, IdCadena, NumCupon, Ticket, FormaPago, elUsuario);
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
                            throw new CAppException(8006, "ModificaDatosCupon() Falla al Actualizar los datos del Cupón en Base de Datos ", err);
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
