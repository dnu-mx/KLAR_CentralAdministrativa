
using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using System;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para la entidad Vigencia
    /// </summary>
    public class LNVigencias
    {
        /// <summary>
        /// Establece las condiciones de validación para la creación de una nueva vigencia.
        /// </summary>
        /// <param name="vig">Nueva vigencia</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ValidaNuevaVigencia(Vigencia vig, Usuario usuario)
        {
            try
            {
                //Validaciones por tipo de vigencia
                switch (vig.ID_TipoVigencia)
                {
                    //Si es del tipo fecha
                    case 1:
                    case 3:
                    case 4:
                        if (DateTime.Compare(vig.FechaInicial, vig.FechaFinal) > 0)
                        {
                            throw new CAppException(8006, "La Fecha Inicial debe ser Menor o Igual a la Fecha Final. Favor de Verificar ");
                        }
                        break;

                    //Si es del tipo horario
                    case 2:
                    case 5:
                    case 6:
                        //Que la hora inicial sea menor o igual a la hora final
                        if (TimeSpan.Compare(vig.HoraInicial, vig.HoraFinal) > 0)
                        {
                            throw new CAppException(8006, "La Hora Inicial debe ser Menor o Igual a la Hora Final. Favor de Verificar ");
                        }
                        break;

                    default: //Si es del tipo periodo
                        break;
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOVigencia.insertar(vig, usuario);
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
                            throw new CAppException(8006, "Falla al Insertar nueva Vigencia GMA en Base de Datos ", err);
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
        /// Establece las condiciones de validación para la actualización de una vigencia.
        /// </summary>
        /// <param name="vigencia">Vigencia por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaVigencia(Vigencia vigencia, Usuario usuario)
        {
            try
            {
                //Validaciones por tipo de vigencia
                switch (vigencia.ID_TipoVigencia)
                {
                    //Si es del tipo fecha
                    case 1:
                    case 3:
                    case 4:
                        if (DateTime.Compare(vigencia.FechaInicial, vigencia.FechaFinal) > 0)
                        {
                            throw new CAppException(8006, "La Fecha Inicial debe ser Menor o Igual a la Fecha Final. Favor de Verificar ");
                        }
                        break;

                    //Si es del tipo horario
                    case 2:
                    case 5:
                    case 6:
                        //Que la hora inicial sea menor o igual a la hora final
                        if (TimeSpan.Compare(vigencia.HoraInicial, vigencia.HoraFinal) > 0)
                        {
                            throw new CAppException(8006, "La Hora Inicial debe ser Menor o Igual a la Hora Final. Favor de Verificar ");
                        }
                        break;

                    default: //Si es del tipo periodo
                        break;
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOVigencia.actualizar(vigencia, usuario);
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
                            throw new CAppException(8006, "Falla al Insertar nueva Vigencia GMA en Base de Datos ", err);
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