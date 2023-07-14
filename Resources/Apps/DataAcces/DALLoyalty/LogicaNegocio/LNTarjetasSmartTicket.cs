using DALAutorizador.BaseDatos;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALLoyalty.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace CentroContacto.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para los Clientes
    /// </summary>
    public class LNTarjetasSmartTicket
    {
        /// <summary>
        /// Establece las condiciones de validación para la consulta de clientes
        /// </summary>
        /// <param name="cliente">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <returns>DatSet con los registros resultados de la búsqueda en BD</returns>
        public static DataSet ConsultaClientes(Cliente cliente, IUsuario usuario)
        {
            //Se verifica que se haya capturado al menos un criterio de búsqueda
            if (String.IsNullOrEmpty(cliente.Nombre) && 
                String.IsNullOrEmpty(cliente.ApellidoPaterno) &&
                String.IsNullOrEmpty(cliente.ApellidoMaterno) &&
                String.IsNullOrEmpty(cliente.Email))
            {
                throw new CAppException(8006, "Selecciona al menos un criterio de búsqueda");
            }

            try
            {
                return DAOConsultaTarjetasSmartTicket.ObtieneClientes(cliente, usuario);
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
        /// Establece las condiciones de validación para la actualización del estatus de la tarjeta
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta por actualizar</param>
        /// <param name="IdEstatus">Identificador de la tarjeta por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaEstatusTarjeta(int IdTarjeta, int IdEstatus, IUsuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDEcommerce.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaTarjetasSmartTicket.ActualizaEstatusTarjetaCliente(IdTarjeta,
                                IdEstatus, conn, transaccionSQL, usuario);
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
                            throw new CAppException(8006, "Falla al actualizar el estatus de la tarjeta del cliente en Base de Datos ", err);
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
        /// Establece las condiciones de validación de la consulta de pedidos de un cliente
        /// </summary>
        /// <param name="IdCliente">Identificador del cliente</param>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaPedidos(ClienteColectiva unCliente, DateTime fechaInicial, DateTime fechaFinal, IUsuario usuario)
        {
            //Se verifica que se haya seleccionado algún cliente del grid de resultados
            if (unCliente.ID_Cliente == 0)
            {
                throw new CAppException(8006, "Selecciona un Cliente");
            }

            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fechaInicial, DateTime.MinValue) == 0 ||
                DateTime.Compare(fechaFinal, DateTime.MinValue) == 0)
            {
                throw new CAppException(8006, "El periodo de tiempo de los pedidos es obligatorio");
            }

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }


            try
            {
                return DAOConsultaClientesMoshi.ObtienePedidos(unCliente, fechaInicial, fechaFinal, usuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, usuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación de la consulta de movimientos de lealtad del cliente
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdColectiva">ID de colectiva</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaMovimientos(DateTime fechaInicial, DateTime fechaFinal, int IdColectiva,
            IUsuario usuario, Guid AppID)
        {
            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fechaInicial, DateTime.MinValue) == 0 ||
                DateTime.Compare(fechaFinal, DateTime.MinValue) == 0)
            {
                throw new CAppException(8006, "El periodo de tiempo de los pedidos es obligatorio");
            }

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }


            try
            {
                return DAOConsultaClientesMoshi.ObtieneMovimientos(fechaInicial, fechaFinal, 
                    IdColectiva, usuario, AppID);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, usuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }


        /// <summary>
        /// Establece las condiciones de validación para la actualización del
        /// nivel de lealtad del cliente
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva del cliente</param>
        /// <param name="IdNivel">Identificador del nivel de lealtad por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaNivelLealtadCliente(int IdColectiva, int IdNivel, IUsuario usuario)
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
                            DAOConsultaClientesMoshi.ActualizaNivelLealtad(IdColectiva, IdNivel, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar el Nivel de Lealtad del Cliente en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la actualización del estatus del medio
        /// de acceso del cliente
        /// </summary>
        /// <param name="medioAcceso">Medio de acceso (email) del cliente</param>
        /// <param name="IdEstatus">Identificador del estatus de medio de acceso por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaEstatusMA(String medioAcceso, int IdEstatus, IUsuario usuario)
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
                            DAOConsultaClientesMoshi.ActivaInactivaMedioAcceso(medioAcceso, IdEstatus, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar el Estatus del Medio de Acceso del Cliente en Base de Datos ", err);
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