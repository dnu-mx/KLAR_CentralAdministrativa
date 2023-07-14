using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para las Validaciones Multiasignación de Entidades
    /// </summary>
    public class LNValidaciones
    {
        /// <summary>
        /// Establece las condiciones de validación para la consulta de validaciones multiasignación
        /// para la cadena comercial y la entidad seleccionadas.
        /// </summary>
        /// <param name="idc">Id de la cadena comercial.</param>
        /// <param name="e">Nombre de la entidad.</param>
        /// <param name="usuario">Usuario en sesión.</param>
        /// <returns>Dataset con los datos consultados de BD</returns>
        //public static DataSet ObtenerVMAPorEntidad(string idc, string e, IUsuario usuario)
        public static DataSet ObtenerVMA(string idc, IUsuario usuario)
        {
            if (String.IsNullOrEmpty(idc.Trim()))
            {
                throw new CAppException(8006, "Selecciona la Cadena Comercial");
            }

            try
            {
                return DAOValidacionMA.ConsultaVMA(
                       Convert.ToInt64(idc), usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));


                //DataSet dsValidaciones = new DataSet();

                //if (e == "Regla")
                //{
                //    dsValidaciones = DAOValidacionMA.ConsultaReglasVMA(
                //       Convert.ToInt64(idc), usuario,
                //       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                //}
                //else if (e == "Tipo de Cuenta")
                //{
                //    dsValidaciones = DAOValidacionMA.ConsultaTipoCuentaVMA(
                //        Convert.ToInt64(idc), usuario,
                //        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                //}
                //else if (e == "Grupo de Cuenta")
                //{
                //    dsValidaciones = DAOValidacionMA.ConsultaGrupoCuentaVMA(
                //        Convert.ToInt64(idc), usuario,
                //        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                //}
                //else if (e == "Grupo de Tarjeta")
                //{
                //    dsValidaciones = DAOValidacionMA.ConsultaGrupoTarjetaVMA(
                //        Convert.ToInt64(idc), usuario,
                //        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                //}
                //else //if (Entidad == "Tarjeta/Cuenta")
                //{
                //    dsValidaciones = DAOValidacionMA.ConsultaTarjetaCuentaVMA(
                //        Convert.ToInt64(idc), usuario,
                //        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                //}

                //return dsValidaciones;
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

        ///// <summary>
        ///// Establece las condiciones de validación para la inserción de una nueva validación multiasignación
        ///// para la cadena comercial y la entidad seleccionadas.
        ///// </summary>
        ///// <param name="e">Nombre de la entidad.</param>
        ///// <param name="laNuevaValidacion">Id de la cadena comercial.</param>
        ///// <param name="IdVP">Nombre de la entidad.</param>
        ///// <param name="tipoV">Nombre de la entidad.</param>
        ///// <param name="usuario">Usuario en sesión.</param>
        //public static void InsertaVMAPorEntidad(string e, ValidacionMA laNuevaValidacion, long IdVP, int tipoV, Usuario usuario)
        //{
        //    if (String.IsNullOrEmpty(e.Trim()))
        //    {
        //        throw new CAppException(8006, "Selecciona una Opción del Menú");
        //    }

        //    try
        //    {
        //        using (SqlConnection conn = BDAutorizador.BDEscritura)
        //        {
        //            conn.Open();

        //            using (SqlTransaction transaccionSQL = conn.BeginTransaction())
        //            {
        //                try
        //                {
        //                    DAOValidacionMA.InsertaVMA(laNuevaValidacion, IdVP, tipoV, usuario);

        //                    //if (e == "Regla")
        //                    //{
        //                    //    DAOValidacionMA.InsertaReglaVMA(laNuevaValidacion, IdVP, tipoV, usuario);
        //                    //}
        //                    //else if (e == "Tipo de Cuenta")
        //                    //{
        //                    //    DAOValidacionMA.InsertaTipoCuentaVMA(laNuevaValidacion, IdVP, tipoV, usuario);
        //                    //}
        //                    //else if (e == "Grupo de Cuenta")
        //                    //{
        //                    //    DAOValidacionMA.InsertaGrupoCuentaVMA(laNuevaValidacion, IdVP, tipoV, usuario);
        //                    //}
        //                    //else if (e == "Grupo de Tarjeta")
        //                    //{
        //                    //    DAOValidacionMA.InsertaGrupoTarjetaVMA(laNuevaValidacion, IdVP, tipoV, usuario);
        //                    //}
        //                    //else if (e == "Tarjeta/Cuenta")
        //                    //{
        //                    //    DAOValidacionMA.InsertaTarjetaCuentaVMA(laNuevaValidacion, IdVP, tipoV, usuario);
        //                    //}

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
        //                    throw new CAppException(8006, "Falla al Insertar nueva Validación en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la inserción de una nueva validación multiasignación
        /// para la cadena comercial y el grupo de medios de acceso seleccionados.
        /// </summary>
        /// <param name="laNuevaValidacion">Id de la cadena comercial.</param>
        /// <param name="IdVP">Nombre de la entidad.</param>
        /// /// <param name="tipoV">Tipo de validación.</param>
        /// <param name="usuario">Usuario en sesión.</param>
        public static void InsertaVMA(ValidacionMA laNuevaValidacion, long IdVP, int tipoV, Usuario usuario)
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
                            DAOValidacionMA.InsertaVMA(laNuevaValidacion, IdVP, tipoV, usuario);
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
                            throw new CAppException(8006, "Falla al Insertar nueva Validación en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la desactivación de validaciones multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad ValidacionMA por inactivar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void DesactivaVMA(ValidacionMA laValidacion, Usuario usuario)
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
                            DAOValidacionMA.ModificaActivacionVMA(laValidacion, false, usuario);
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

        /// <summary>
        /// Establece las condiciones de validación para la activación de validaciones multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad ValidacionMA por activar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActivaVMA(ValidacionMA laValidacion, Usuario usuario)
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
                            DAOValidacionMA.ModificaActivacionVMA(laValidacion, true, usuario);
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
                            throw new CAppException(8006, "Falla al Activar Validación en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la modificación de Post Reglas
        /// de validaciones multiasignación.
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad ValidacionMA por activar</param>
        /// <param name="activar">Bandera de activación de la Post Regla</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaPostReglasVMA(ValidacionMA laValidacion, bool activar, Usuario usuario)
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
                            DAOValidacionMA.PostReglas(laValidacion, activar, usuario);
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
                            throw new CAppException(8006, "Falla al Modificar PostRegla de Validación en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la modificación de Pre Reglas
        /// de validaciones multiasignación.
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad ValidacionMA por activar</param>
        /// <param name="activar">Bandera de activación de la Pre Regla</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaPreReglasVMA(ValidacionMA laValidacion, bool activar, Usuario usuario)
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
                            DAOValidacionMA.PreReglas(laValidacion, activar, usuario);
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
                            throw new CAppException(8006, "Falla al Modificar PreRegla de Validación en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la eliminación de validaciones multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad ValidacionMA por eliminar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void EliminaVMA(ValidacionMA laValidacion, Usuario usuario)
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
                            DAOValidacionMA.EliminarVMA(laValidacion, usuario);
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
                            throw new CAppException(8006, "Falla al Eliminar Validación en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la modificación de validaciones multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad ValidacionMA por inactivar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaVMA(ValidacionMA laValidacion, long IdVPadre, Usuario usuario)
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
                            DAOValidacionMA.ModificaVMA(laValidacion, IdVPadre, usuario);
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