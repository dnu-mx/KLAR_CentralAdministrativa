using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DALCentroContacto.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para los Clientes
    /// </summary>
    public class LNTiendasDiconsa
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion


        /// <summary>
        /// Establece las condiciones de validación para la consulta de tiendas
        /// </summary>
        /// <param name="tienda">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DatSet con los registros resultados de la búsqueda en BD</returns>
        public static DataSet ConsultaTiendas(TiendaDiconsa tienda, IUsuario usuario, Guid AppID)
        {
            //Se verifica que se haya capturado al menos un criterio de búsqueda
            if (String.IsNullOrEmpty(tienda.ClaveColectiva) && String.IsNullOrEmpty(tienda.ClaveAlmacen) &&
                String.IsNullOrEmpty(tienda.ClaveTienda) && String.IsNullOrEmpty(tienda.Nombre) &&
                String.IsNullOrEmpty(tienda.ApellidoPaterno) && String.IsNullOrEmpty(tienda.ApellidoMaterno))
            {
                throw new CAppException(8006, "Ingresa al menos un criterio de búsqueda");
            }

            if (!String.IsNullOrEmpty(tienda.ClaveColectiva))
            {
                Int64 cel;

                if (!Int64.TryParse(tienda.ClaveColectiva, out cel))
                {
                    throw new CAppException(8006, "El dato Móvil capturado no es numérico");
                }
            }

            try
            {
                return DAOTiendasDiconsa.ObtieneTiendas(tienda, usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
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
        /// Establece las condiciones de validación para la actualización de
        /// los datos del operador de la tienda
        /// </summary>
        /// <param name="elOperador">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaDatosOperador(TiendaDiconsa elOperador, IUsuario usuario)
        {
            try
            {
                //Si no se obtuvo el ID del operador, no es posible actualizar sus datos
                if (elOperador.ID_Operador == 0)
                {
                    throw new CAppException(8006, "No es posible actualizar los datos de este operador.");
                }

                //Se verifica que el correo electrónico sea una cadena válida
                if (!String.IsNullOrEmpty(elOperador.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(elOperador.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOTiendasDiconsa.actualizaDatosOperador(elOperador, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Datos del Operador de la Tienda en Base de Datos ", err);
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
        /// Establece las condiciones de validación de la consulta de movimientos de un medio de acceso
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idTipoCuenta">Identificador del tipo de cuenta</param>
        /// <param name="idTipoOper">Identificador del tipo de operación</param>
        /// <param name="medioAcceso">Clave del medio de acceso</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaMovimientos(DateTime fechaInicial, DateTime fechaFinal, int idTipoCuenta, int idTipoOper,
            int idColectiva, IUsuario usuario, Guid AppID)
        {
            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fechaInicial, DateTime.MinValue) == 0 ||
                DateTime.Compare(fechaFinal, DateTime.MinValue) == 0)
            {
                throw new CAppException(8006, "El periodo de tiempo de los movimientos es obligatorio");
            }

            //Se verifica que se haya ingresado el tipo de cuenta
            if (idTipoCuenta == 0)
            {
                throw new CAppException(8006, "Por favor ingresa el tipo de cuenta");
            }

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }


            try
            {
                return DAOTiendasDiconsa.ObtieneMovimientosTX(fechaInicial, fechaFinal, idTipoCuenta,
                            idTipoOper, idColectiva, usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
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
        /// Establece las condiciones de validación para la actualización del estatus de una tarjeta
        /// </summary>
        /// <param name="idMA">Identificador del medio de acceso</param>
        /// <param name="idEstatusMA">Identificador del estatus del medio de acceso</param>
        /// <param name="razones">Comentarios descritos como razones para la cancelación de la tarjeta</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaEstatusMA(int idMA, int idEstatusMA, string razones, IUsuario usuario)
        {
            try
            {
                //Se valida que se hayan capturado las razones de la cancelación
                if (String.IsNullOrEmpty(razones))
                {
                    throw new CAppException(8006, "Ingresa la razón de la cancelación.");
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //Se modifica el estatus del medio de acceso
                            DAOConsultaClientes.ModificaEstatusMA(idMA, idEstatusMA, usuario);

                            //Se inserta el registro en la bitázora de detalle
                            DAOConsultaClientes.InsertaRazonBitacoraDetalle(idMA, razones, usuario);

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
                            throw new CAppException(8006, "Falla al Actualizar el Estatus del MDA en Base de Datos ", err);
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
        /// Establece las condiciones de validación para el registro de la llamada del cliente
        /// </summary>
        /// <param name="ID_Cliente">Identificador del cliente</param>
        /// <param name="ID_Actividad">Identificador de la actividad</param>
        /// <param name="Comentarios">Comentarios</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraLlamadaCliente(int ID_Cliente, int ID_Actividad, string Comentarios, IUsuario usuario)
        {
            try
            {
                //Se valida que se capturen todos los datos del formulario
                if (ID_Actividad == 0 || String.IsNullOrEmpty(Comentarios))
                {
                    throw new CAppException(8006, "Captura todos los datos para registrar la llamada.");
                }
                if (ID_Cliente == 0)
                {
                    throw new CAppException(8006, "Debes seleccionar un cliente para registrar su llamada.");
                }


                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOTiendasDiconsa.InsertaLlamadaCliente(ID_Cliente, ID_Actividad, Comentarios, usuario);
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
    }
}