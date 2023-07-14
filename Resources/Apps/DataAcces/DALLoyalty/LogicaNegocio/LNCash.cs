using DALCentralAplicaciones.Entidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALLoyalty.BaseDatos;
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
    /// Establece la lógica del negocio del acceso a datos de Moshi Moshi
    /// </summary>
    public class LNCash
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
        public static DataSet ConsultaClientes(ClienteColectiva cliente, Usuario usuario, Guid AppID)
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
                return DAOConsultaClientesCash.ObtieneClientes(cliente, usuario, AppID);
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
        /// Establece las condiciones de validación para la actualización de los datos del cliente
        /// </summary>
        /// <param name="elCliente">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaDatosCliente(ClienteColectiva elCliente, Usuario usuario)
        {
            try
            {
                //Se valida que se hayan capturado todos los datos del cliente
                if (String.IsNullOrEmpty(elCliente.Nombre) && String.IsNullOrEmpty(elCliente.ApellidoPaterno))
                {
                    throw new CAppException(8006, "Ingresa los datos completos del cliente.");
                }

                using (SqlConnection conn = BDEcommerceCash.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaClientesCash.ActualizaDatosCliente(elCliente, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los datos del Cliente en Base de Datos ", err);
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
                return DAOConsultaClientesCash.ObtienePedidos(unCliente, fechaInicial, fechaFinal, usuario);
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
                return DAOConsultaClientesCash.ObtieneMovimientos(fechaInicial, fechaFinal,
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
        public static void ActualizaNivelLealtadCliente(int IdColectiva, int IdNivel, Usuario usuario)
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
                            DAOConsultaClientesCash.ActualizaNivelLealtad(IdColectiva, IdNivel, usuario);
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
        public static void ActualizaEstatusMA(String medioAcceso, int IdEstatus, Usuario usuario)
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
                            DAOConsultaClientesCash.ActivaInactivaMedioAcceso(medioAcceso, IdEstatus, usuario);
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

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del registro del evento manual
        /// "Ajuste de Cargo"
        /// </summary>
        /// <param name="evento">Datos del evento</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraEvManual_AjusteCargo(EventosManuales evento, Usuario usuario)
        {
            using (SqlConnection conn = BDAutorizadorCash.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {

                    try
                    {
                        Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                        ////Se consultan los parámetros del contrato
                        losParametros = DAOEvento.ListaDeParamentrosContrato
                            (evento.ClaveCadenaComercial, "", evento.ClaveEvento, usuario.ClaveUsuario);

                        losParametros["@ID_CuentaHabiente"] = new Parametro() { Nombre = "@ID_CuentaHabiente",
                            Valor = evento.IdColectiva.ToString(), Descripcion = "ID_CuentaHabiente",
                            ID_TipoColectiva = evento.IdTipoColectiva };
                        losParametros["@Importe"] = new Parametro() { Nombre = "@Importe",
                            Valor = evento.Importe, Descripcion = "Importe" };

                        //Genera y Aplica la Poliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(evento.IdEvento,
                            evento.Concepto, false, evento.Referencia, losParametros, evento.Observaciones,
                            conn, transaccionSQL);
                        Poliza laPoliza = aplicador.AplicaContablilidad();

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            throw new Exception("No se generó la Póliza: " + laPoliza.DescripcionRespuesta);
                        }

                        transaccionSQL.Commit();
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        Loguear.Error(err, "");
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece y configura los parámetros para la ejecución del registro del evento manual
        /// "Ajuste de Abono"
        /// </summary>
        /// <param name="evento">Datos del evento</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraEvManual_AjusteAbono(EventosManuales evento, Usuario usuario)
        {
            using (SqlConnection conn = BDAutorizadorCash.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {

                    try
                    {
                        Dictionary<String, Parametro> losParametros = new Dictionary<string, Parametro>();

                        ////Se consultan los parámetros del contrato
                        losParametros = DAOEvento.ListaDeParamentrosContrato
                            (evento.ClaveCadenaComercial, "", evento.ClaveEvento, usuario.ClaveUsuario);

                        losParametros["@ID_CuentaHabiente"] = new Parametro()
                        {
                            Nombre = "@ID_CuentaHabiente",
                            Valor = evento.IdColectiva.ToString(),
                            Descripcion = "ID_CuentaHabiente",
                            ID_TipoColectiva = evento.IdTipoColectiva
                        };
                        losParametros["@Importe"] = new Parametro()
                        {
                            Nombre = "@Importe",
                            Valor = evento.Importe,
                            Descripcion = "Importe"
                        };

                        //Genera y Aplica la Poliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(evento.IdEvento,
                            evento.Concepto, false, evento.Referencia, losParametros, evento.Observaciones,
                            conn, transaccionSQL);
                        Poliza laPoliza = aplicador.AplicaContablilidad();

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            throw new Exception("No se generó la Póliza: " + laPoliza.DescripcionRespuesta);
                        }

                        transaccionSQL.Commit();
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        Loguear.Error(err, "");
                        throw err;
                    }
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de un precalculo
        /// </summary>
        /// <param name="fecha">Fecha de Progromacion del precalculo</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void InsertaPrecalculo(DateTime fecha, Usuario usuario)
        {
            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fecha, DateTime.MinValue) == 0)
            {
                throw new Exception("La fecha es obligatoria");
            }

            try
            {
                using (SqlConnection conn = BDAutorizadorCash.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOPrecalculoPuntosCash.InsertaPrecalculo(fecha, usuario);
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
                            throw new Exception(err.Message, err);
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
        /// Establece las condiciones de validación para la actualización de un precalculo
        /// </summary>
        /// <param name="id">Id de programación a actualizar</param>
        /// <param name="estatus">Estatus a cambiar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaPrecalculo(int id, int estatus, Usuario usuario)
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
                            DAOPrecalculoPuntosCash.ActualizaPrecalculo(id, estatus, usuario);
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
                            throw new Exception(err.Message, err);
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
