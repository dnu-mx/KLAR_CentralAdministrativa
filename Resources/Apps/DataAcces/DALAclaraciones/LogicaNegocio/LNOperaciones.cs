using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DALAclaraciones.BaseDatos;
using DALAclaraciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using DALCentralAplicaciones.LogicaNegocio;
using System.Data;
using DALAclaraciones.Utilidades;

namespace DALAclaraciones.LogicaNegocio
{
    public class LNOperaciones
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OperPorBuscar"></param>
        /// <param name="usuario"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public static DataSet BuscarOperacion (Operacion OperPorBuscar, IUsuario usuario, Guid AppId)
        {
            DataSet operaciones;

            try
            {
                //valida los datos proporcionados por el usuario.
                int DatosProporcionados = 0;

                if (OperPorBuscar.Tarjeta.Trim() != "")
                {
                    DatosProporcionados++;
                }

                if (OperPorBuscar.Importe != 0)
                {
                    DatosProporcionados++;
                }

                if (OperPorBuscar.ID_GrupoMA != 0)
                {
                    DatosProporcionados++;
                }

                if (OperPorBuscar.FechaInicial != null && OperPorBuscar.FechaFinal != null)
                {
                    DatosProporcionados++;
                }

                //Si no se proporcionan todos los datos no se realiza la búsqueda
                if (DatosProporcionados < 4)
                {
                    throw new CAppException(8006, "Proporciona Todos los Datos para la Búsqueda");
                }

                //La fecha inicial no puede ser mayor a la final ni mayor a la fecha actual
                //La fecha final no puede ser mayor a la fecha actual
                if (DateTime.Compare(OperPorBuscar.FechaInicial, OperPorBuscar.FechaFinal) > 0
                    || DateTime.Compare(OperPorBuscar.FechaInicial, DateTime.Now) > 0
                    || DateTime.Compare(OperPorBuscar.FechaFinal, DateTime.Now) > 0)
                {
                    throw new CAppException(8006, "El Periodo de Fechas Solicitado es Incorrecto. Favor de Verificar ");
                }

                using (SqlConnection conn = BDAclaraciones.BDLectura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            operaciones = DAOOperacion.BuscaOperacionPorAclarar(usuario, OperPorBuscar, AppId);
                            Loguear.Evento("Se realizó la búsqueda de una operación por aclarar con los datos: " + OperPorBuscar.ToString(), usuario.ClaveUsuario);
                        }
                        catch (CAppException err)
                        {
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Error al buscar la operación en Base de Datos " + OperPorBuscar.ToString(), err);
                        }
                    }

                }            

                return operaciones;
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
        /// 
        /// </summary>
        /// <param name="IdOperacion"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public static DataSet ConsultarHistorialContracargos(Int64 IdOperacion, IUsuario usuario)
        {
            DataSet envios;

            try
            {
                using (SqlConnection conn = BDAclaraciones.BDLectura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            envios = DAOOperacion.ListaEnviosAContracargos(IdOperacion, usuario);
                            Loguear.Evento("Se consultó el historial de envíos a contracargo de la operación ID: " + IdOperacion.ToString(), usuario.ClaveUsuario);
                        }
                        catch (CAppException err)
                        {
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Error al buscar la operación en Base de Datos " + IdOperacion.ToString(), err);
                        }
                    }

                }

                return envios;
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
        /// 
        /// </summary>
        /// <param name="IdOperacion"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public static void EliminarOperacionDeContracargos(Int64 IdOperacion, IUsuario usuario)
        {
            try
            {
                DAOOperacion.EliminarOperacionContracargo(IdOperacion, usuario);
                Loguear.Evento("Se eliminó operación del historial de envíos a contracargo con ID: " + IdOperacion.ToString(), usuario.ClaveUsuario);
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
        /// 
        /// </summary>
        /// <param name="OperPorAclarar"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public static void EnviarOperacionAclaracion(Operacion OperPorAclarar, IUsuario usuario)
        {
            try
            {
                //Validación de campos
                if (OperPorAclarar.ImporteAcl == 0  ||  OperPorAclarar.Id_RC == 0  ||  OperPorAclarar.Id_DI == 0)
                {
                    throw new CAppException(8006, "Proporciona Todos los Datos para Enviar la Aclaración");
                }

                if (OperPorAclarar.ImporteAcl > OperPorAclarar.ImporteOper)
                {
                    throw new CAppException(8006, "El Importe de la Aclaración debe ser Menor o Igual al Importe de la Operación Original.");
                }

                DAOOperacion.InsertarOperacionAclaracion(OperPorAclarar, usuario);
                Loguear.Evento("Se insertó operación a envíos contracargo con ID: " + OperPorAclarar.Id_Operacion.ToString(), usuario.ClaveUsuario);
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
