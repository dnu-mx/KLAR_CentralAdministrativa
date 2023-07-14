using DALCentralAplicaciones.Entidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALLoyalty.Utilidades;
using Executer.BaseDatos;
using Executer.Entidades;
using Interfases.Entidades;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DALCentroContacto.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio del acceso a datos de Clientes StarCinema
    /// </summary>
    public class LNClientesStarCinema
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion

        /// <summary>
        /// Establece las condiciones de validación para la consulta de clientes
        /// </summary>
        /// <param name="cliente">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DatSet con los registros resultados de la búsqueda en BD</returns>
        public static DataSet ConsultaClientes(ClienteColectiva cliente, Usuario usuario)
        {
            //Se verifica que se haya capturado al menos un criterio de búsqueda
            if (String.IsNullOrEmpty(cliente.Nombre) && String.IsNullOrEmpty(cliente.ApellidoPaterno) &&
                String.IsNullOrEmpty(cliente.ApellidoMaterno) && DateTime.Compare(cliente.FechaNacimiento, DateTime.MinValue) == 0
                && String.IsNullOrEmpty(cliente.Email) && String.IsNullOrEmpty(cliente.Telefono))
            {
                throw new CAppException(8006, "Selecciona al menos un criterio de búsqueda");
            }

            //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
            if (!String.IsNullOrEmpty(cliente.Email))
            {
                Match matchExpression;
                Regex matchEmail = new Regex(regexEmail);

                matchExpression = matchEmail.Match(cliente.Email);

                if (!matchExpression.Success)
                {
                    throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                }
            }

            try
            {
                return DAOConsultaClientesStarCinema.ObtieneClientes(cliente, usuario);
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
        public static DataSet ConsultaPedidos(ClienteColectiva unCliente, DateTime fechaInicial, DateTime fechaFinal, Usuario usuario)
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
                return DAOConsultaClientesStarCinema.ObtienePedidos(unCliente, fechaInicial, fechaFinal, usuario);
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
            Usuario usuario, Guid AppID)
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
                return DAOConsultaClientesStarCinema.ObtieneMovimientos(fechaInicial, fechaFinal,
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
        /// Establece las condiciones de validación para la actualización del estatus del medio
        /// de acceso del cliente
        /// </summary>
        /// <param name="IdColectiva">Id colectiva del cliente</param>
        /// <param name="IdEstatus">Identificador del estatus de medio de acceso por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaEstatusConfirmaClientes(int IdEstatus, int IdColectiva, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizadorStarCinema.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaClientesStarCinema.ActualizaEstatusConfirma(IdColectiva, IdEstatus, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar el Estatus de Confirmación de Correo en Base de Datos ", err);
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
        /// Establece una nueva contraseña de acceso al cliente
        /// </summary>
        /// <param name="IdColectiva">Id colectiva del cliente</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void GenerarContrasenaClientes(int IdColectiva, string Correo, string Contrasena, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizadorStarCinema.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaClientesStarCinema.ActualizaContrasenaCliente(Correo, Contrasena, usuario);
                            DAOConsultaClientesStarCinema.ActualizaEstatusConfirma(IdColectiva, 1, usuario);
                            
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
                            throw new CAppException(8006, "Falla al Actualizar la Nueva Contraseña en Base de Datos ", err);
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
        /// Aplica bloqueo o desbloqueo por contracargo de un cliente
        /// </summary>
        /// <param name="Correo">Id que identifica al cliente</param>
        /// <param name="Estatus">Estatus nuevo de bloqueo o desbloqueo</param>
        /// <param name="Comentarios">Motivo de cambio de estatus</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void AplicaBloqueoContracargoCliente(string Correo, bool Estatus, string Comentarios, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDEAdminEcommerceStarCinema.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaClientesStarCinema.ActualizaEstatusBloqueoCliente(Correo, Estatus, Comentarios, usuario);

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
                            throw new CAppException(8006, "Falla al Actualizar la Nueva Contraseña en Base de Datos ", err);
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
