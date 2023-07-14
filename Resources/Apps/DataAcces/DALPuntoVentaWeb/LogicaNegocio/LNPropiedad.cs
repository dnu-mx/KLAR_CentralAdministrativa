using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    public class LNPropiedad
    {
        public static List<Propiedad> ObtieneParametros(Int64 ID_CadenaComercial, String CadenaConexion)
        {

            try
            {
                    List<Propiedad> resp = new List<Propiedad>();

                    resp = DAOPropiedad.ObtenerPropiedades( ID_CadenaComercial,  CadenaConexion);

              
                return resp;
            }
            catch (CAppException err)
            {
               // Loguear.Error(err, "");
                throw err;
            }
            catch (Exception)
            {
              //  Loguear.Error(err, "");
                return new List<Propiedad>();
            }
        }

        public static void ModificaParametros(List<Propiedad> lasPropiedades, Int64 Id_cadena,  String CadenaConexion, IUsuario elUser)
        {
            try
            {
                try
                {
                    foreach (Propiedad unaProp in lasPropiedades)
                    {
                        DAOPropiedad.Modicar(unaProp, Id_cadena, CadenaConexion, elUser);
                    }

                }
                catch (CAppException err)
                {
                    throw err;
                }
                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al Actualizar los parametros, No se Realizó la Actualizacion", err);
                }
            }
            catch (CAppException err)
            {
               // Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
              //  Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }


        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de parámetros en el punto de venta
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión a base de datos</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static List<Propiedad> ObtieneParametrosEnPDV(Int64 ID_CadenaComercial, String CadenaConexion, IUsuario usuario, Guid AppID)
        {
            try
            {
                List<Propiedad> resp = new List<Propiedad>();

                resp = DAOPropiedad.ObtenerParametrosEnPDV(ID_CadenaComercial, CadenaConexion, usuario, AppID);

                return resp;
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception)
            {
                return new List<Propiedad>();
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de parámetros del punto de venta
        /// </summary>
        /// <param name="lasPropiedades">Datos de los parámetros que cambiaron</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametrosEnPDV(List<Propiedad> lasPropiedades, Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(CadenaConexion))
                    {
                        conn.Open();

                        using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                        {
                            try
                            {
                                foreach (Propiedad unaProp in lasPropiedades)
                                {
                                    DAOPropiedad.ActualizaParametros(unaProp, Id_cadena, CadenaConexion, elUser);
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
                                throw new CAppException(8006, "Falla al Actualizar el Parámetro de la Cadena en Base de Datos ", err);
                            }
                        }
                    }
                }

                catch (CAppException err)
                {
                    throw err;
                }

                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al Actualizar los Parámetros, no se Realizó la Actualización", err);
                }
            }

            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de la versión de los parámetros de la cadena
        /// </summary>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaVersionParametrosEnPDV(Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(CadenaConexion))
                    {
                        conn.Open();

                        using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                        {
                            try
                            {
                                DAOPropiedad.ActualizaVersionParametros(Id_cadena, CadenaConexion, elUser);
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
                                throw new CAppException(8006, "Falla al Actualizar el Parámetro de la Cadena en Base de Datos ", err);
                            }
                        }
                    }
                }

                catch (CAppException err)
                {
                    throw err;
                }

                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al Actualizar los Parámetros, no se Realizó la Actualización", err);
                }
            }

            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de condiciones comerciales
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión a base de datos</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static List<Propiedad> ObtieneCondicionesComerciales(Int64 ID_CadenaComercial, String CadenaConexion, IUsuario usuario, Guid AppID)
        {
            try
            {
                List<Propiedad> resp = new List<Propiedad>();

                resp = DAOPropiedad.ObtenerCondicionesComerciales(ID_CadenaComercial, CadenaConexion, usuario, AppID);

                return resp;
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception)
            {
                return new List<Propiedad>();
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de parámetros de las condiciones comerciales
        /// </summary>
        /// <param name="lasPropiedades">Datos de los parámetros que cambiaron</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaCondicionesComerciales(List<Propiedad> lasPropiedades, Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(CadenaConexion))
                    {
                        conn.Open();

                        using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                        {
                            try
                            {
                                foreach (Propiedad unaProp in lasPropiedades)
                                {
                                    DAOPropiedad.ActualizaCondiciones(unaProp, Id_cadena, CadenaConexion, elUser);
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
                                throw new CAppException(8006, "Falla al Actualizar el Valor del Parámetro de Condiciones Comerciales de la Cadena en Base de Datos ", err);
                            }
                        }
                    }
                }

                catch (CAppException err)
                {
                    throw err;
                }

                catch (Exception err)
                {
                    throw new CAppException(8006, "Falla al Actualizar los Parámetros, no se Realizó la Actualización", err);
                }
            }

            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

    }
}
