using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.Utilidades;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace DALPuntoVentaWeb.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para Sr Pago
    /// </summary>
    public class LNSrPago
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion

        /// <summary>
        /// Establece las condiciones de validación para insertar los datos del archivo a base de datos
        /// </summary>
        /// <param name="losDatos">Datos del registro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaDatosDeArchivoEnBD(DatosSrPago losDatos, Usuario elUsuario)
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
                            DAOSrPago.InsertaDatosDeArchivo(losDatos, elUsuario);
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
                            throw new CAppException(8006, "ActualizaDatosDeArchivoEnBD() Falla al Actualizar los datos de la Tienda en Base de Datos ", err);
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

        /// <summary>
        /// Establece las condiciones de validación para la consulta del catálogo de tiendas
        /// </summary>
        /// <param name="ClaveAlmacen">Clave del almacénn</param>
        /// <param name="ClaveTienda">Nombre de la tienda</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DatSet con los registros resultados de la búsqueda en BD</returns>
        public static DataSet ConsultaCatalogoTiendas(string ClaveAlmacen, string ClaveTienda, Usuario usuario, Guid AppID)
        {
            //Se verifica que se haya capturado al menos un criterio de búsqueda
            if (String.IsNullOrEmpty(ClaveAlmacen) && String.IsNullOrEmpty(ClaveTienda))
            {
                throw new CAppException(8006, "Ingresa al menos un criterio de búsqueda");
            }

            //Se verifica que sean datos numéricos las claves ingresadas
            if (!String.IsNullOrEmpty(ClaveAlmacen))
            {
                Int64 almacen;

                if (!Int64.TryParse(ClaveAlmacen, out almacen))
                {
                    throw new CAppException(8006, "La clave del almacén debe ser numérica");
                }
            }

            if (!String.IsNullOrEmpty(ClaveTienda))
            {
                Int64 tienda;

                if (!Int64.TryParse(ClaveTienda, out tienda))
                {
                    throw new CAppException(8006, "La clave de la tienda debe ser numérica");
                }
            }

            try
            {
                return DAOSrPago.ObtieneCatalogoTiendas(ClaveAlmacen, ClaveTienda, usuario, AppID);
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
        /// Establece las condiciones de validación para la complementación de
        /// los datos de la tienda
        /// </summary>
        /// <param name="losDatos">Datos de tienda por complementar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CompletaDatosTienda(DatosSrPago losDatos, Usuario usuario)
        {
            try
            {
                //Se valida que se hayan capturado sólo números para el código postal
                if (!String.IsNullOrEmpty(losDatos.CodigoPostal))
                {
                    Int32 cp;

                    if (!Int32.TryParse(losDatos.CodigoPostal, out cp))
                    {
                        throw new CAppException(8006, "El Código Postal debe ser numérico.");
                    }
                }

                //Se valida que se hayan capturado sólo números para el teléfono
                if (!String.IsNullOrEmpty(losDatos.Telefono))
                {
                    Int64 tel;

                    if (!Int64.TryParse(losDatos.Telefono, out tel))
                    {
                        throw new CAppException(8006, "El teléfono debe ser numérico.");
                    }
                }

                //Se verifica que el correo electrónico sea una cadena válida
                if (!String.IsNullOrEmpty(losDatos.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(losDatos.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                //Se valida que se hayan capturado sólo números para la tarjeta
                if (!String.IsNullOrEmpty(losDatos.NumeroTarjeta))
                {
                    Int64 tarjeta;

                    if (!Int64.TryParse(losDatos.NumeroTarjeta, out tarjeta))
                    {
                        throw new CAppException(8006, "La tarjeta debe ser numérica.");
                    }
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOSrPago.CompletaDatosTienda(losDatos, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Datos de la Tienda en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la actualización de
        /// los datos de la tienda
        /// </summary>
        /// <param name="losDatos">Datos de tienda por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaDatosTienda(DatosSrPago losDatos, Usuario usuario)
        {
            try
            {
                //Se valida que se hayan capturado sólo números para el código postal
                if (!String.IsNullOrEmpty(losDatos.CodigoPostal))
                {
                    Int32 cp;

                    if (!Int32.TryParse(losDatos.CodigoPostal, out cp))
                    {
                        throw new CAppException(8006, "El Código Postal debe ser numérico.");
                    }
                }

                //Se valida que se hayan capturado sólo números para el teléfono
                if (!String.IsNullOrEmpty(losDatos.Telefono))
                {
                    Int64 tel;

                    if (!Int64.TryParse(losDatos.Telefono, out tel))
                    {
                        throw new CAppException(8006, "El teléfono debe ser numérico.");
                    }
                }

                //Se verifica que el correo electrónico sea una cadena válida
                if (!String.IsNullOrEmpty(losDatos.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(losDatos.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                //Se valida que se hayan capturado sólo números para la tarjeta
                if (!String.IsNullOrEmpty(losDatos.NumeroTarjeta))
                {
                    Int64 tarjeta;

                    if (!Int64.TryParse(losDatos.NumeroTarjeta, out tarjeta))
                    {
                        throw new CAppException(8006, "La tarjeta debe ser numérica.");
                    }
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOSrPago.ActualizaDatosTienda(losDatos, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Datos de la Tienda en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la actualización del estatus 
        /// del alta en Sr Pago de una tienda
        /// </summary>
        /// <param name="losDatos">Datos de tienda por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaEstatusAltaSrPago(DatosSrPago losDatos, Usuario usuario)
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
                            DAOSrPago.ActualizaAltaSrPago(losDatos, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Datos de la Tienda en Base de Datos ", err);
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
