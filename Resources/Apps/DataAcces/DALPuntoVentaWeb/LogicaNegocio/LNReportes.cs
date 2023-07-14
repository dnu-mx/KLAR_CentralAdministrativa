using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Data;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para los Reportes
    /// </summary>
    public class LNReportes
    {
        /// <summary>
        /// Establece las condiciones de validación para la consulta las operaciones transaccionales
        /// de las tiendas Diconsa
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial de consulta</param>
        /// <param name="FechaFinal">Fecha final de consulta</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="Beneficiario">Marca</param>
        /// <param name="Estatus">Estatus de las TXs</param>
        /// <param name="Telefono">Referencia numérica</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarOperacionesTiendaDiconsa(DateTime FechaInicial, DateTime FechaFinal,
         Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen, String Beneficiario, String Estatus,
          String Telefono, Int32 ID_Tienda, Usuario elUser, Guid AppId)
        {
            DataSet laRespuesta = new DataSet();
            try
            {

                laRespuesta = DAOReportes.ListarOperacionesTransaccionalesTiendaDiconsa(FechaInicial, FechaFinal, Sucursal, 
                    UnidadOperativa, Almacen, Beneficiario, Estatus, Telefono, ID_Tienda, elUser, AppId);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de cobros con tarjeta
        /// de las tiendas Diconsa
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial de consulta</param>
        /// <param name="FechaFinal">Fecha final de consulta</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="TipoTarjeta">Tip de tarjeta</param>
        /// <param name="Estatus">Estatus de las TXs</param>
        /// <param name="Tarjeta">Los 4 últimos dígitos de la tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarCobrosConTarjetaDiconsa(DateTime FechaInicial, DateTime FechaFinal,
         Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen, Int32 ID_Tienda, String TipoTarjeta, String Estatus,
          String Tarjeta, Usuario elUser, Guid AppId)
        {
            DataSet laRespuesta = new DataSet();

            try
            {
                laRespuesta = DAOReportes.ListaCobrosConTarjetaDiconsa(FechaInicial, FechaFinal, Sucursal,
                    UnidadOperativa, Almacen, ID_Tienda, TipoTarjeta, Estatus, Tarjeta, elUser, AppId);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de cortes y saldos de las tiendas Diconsa
        /// </summary>
        /// <param name="Corte">Id del periodo de corte</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarCortesYSaldosDiconsa(int Corte, Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen,
            Int32 ID_Tienda, Usuario elUser, Guid AppId)
        {
            DataSet laRespuesta = new DataSet();

            try
            {

                laRespuesta = DAOReportes.ListaCortesYSaldosDiconsa(Corte, Sucursal, UnidadOperativa, Almacen, 
                    ID_Tienda, elUser, AppId);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de comisiones de las tiendas Diconsa
        /// </summary>
        /// <param name="Corte">Id del periodo de corte</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarComisionesDiconsa(int Corte, Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen,
            Int32 ID_Tienda, Usuario elUser, Guid AppId)
        {
            DataSet laRespuesta = new DataSet();

            try
            {

                laRespuesta = DAOReportes.ListaComisionesDiconsa(Corte, Sucursal, UnidadOperativa, Almacen,
                    ID_Tienda, elUser, AppId);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de comisiones acumuladas de las tiendas Diconsa
        /// </summary>
        /// <param name="Corte">Id del periodo de corte</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarComisionesAcumuladasDiconsa(int Corte, Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen,
            Int32 ID_Tienda, Usuario elUser, Guid AppId)
        {
            DataSet laRespuesta = new DataSet();

            try
            {

                laRespuesta = DAOReportes.ListaComisionesAcumuladasDiconsa(Corte, Sucursal, UnidadOperativa, Almacen,
                    ID_Tienda, elUser, AppId);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de promociones Loyalty
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="Sucursal">Clave de la sucursal</param>
        /// <param name="Operador">Clave del operador</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarPromocionesLoyalty(DateTime FechaInicial, DateTime FechaFinal, int IdCadena,
            String Sucursal, String Operador, Usuario elUser, Guid AppId)
        {
            DataSet laRespuesta = new DataSet();

            try
            {
                laRespuesta = DAOReportes.ListaPromocionesLoyalty(FechaInicial, FechaFinal, IdCadena,
                                Sucursal, Operador, elUser, AppId);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }

                return laRespuesta;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la ruta del reporte en el Autorizador
        /// </summary>
        /// <param name="idReporte">Identificador del reporte</param>
        /// <param name="ruta">Valor de la nueva ruta del reporte</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaRutasReportes(int idReporte, string ruta, Usuario elUsuario, ILogHeader elLog)
        {
            try
            {
                DAOReportes.ActualizaRutasReportes(idReporte, ruta, elUsuario, elLog);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(elLog);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar la ruta del reporte.");
            }
        }

        /// <summary>
        /// Asignar un reporte a una colectiva 
        /// </summary>
        /// <param name="IdReporte">Identificador del reporte</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AsignaReporteColectiva(int IdReporte, long IdColectiva, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOReportes.InsertaReporteColectiva(IdReporte, IdColectiva, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al asignar el Reporte a la Colectiva");
            }
        }

        /// <summary>
        /// Actualiza la configuración de un reporte 
        /// </summary>
        /// <param name="IdReporteColectivaConfiguracion">Identificador del registro de la configuración del reporte a actualizar</param>
        /// <param name="IdReporteColectiva">Identificador de la relación del reporte</param>
        /// <param name="IdClasificacion">Identificador de la clasificación del reporte</param>
        /// <param name="IdTipoServicio">Identificador del tipo de servicio del reporte</param>
        /// <param name="Ruta">Ruta del reporte</param>
        /// <param name="NombreArchivo">Nombre del Archivo del reporte</param>
        /// <param name="HoraEjecucion">Hora de ejecución del reporte</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaConfigReporteColectiva(int IdReporteColectivaConfiguracion, int IdReporteColectiva, 
            int IdClasificacion, int IdTipoServicio, string Ruta, string NombreArchivo, TimeSpan HoraEjecucion,
            Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOReportes.ActualizaConfigReporteColectiva(IdReporteColectivaConfiguracion, IdReporteColectiva,
                    IdClasificacion, IdTipoServicio, Ruta, NombreArchivo, HoraEjecucion, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al actualizar la configuración del reporte");
            }
        }

        /// <summary>
        /// Obtiene un listado de registros de reportes 
        /// </summary>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneListadoReportes(ILogHeader logHeader)
        {
            DataTable laRespuesta = new DataTable();

            try
            {
                laRespuesta = DAOReportes.ObtenerListaReportes(logHeader);

                if (laRespuesta.Rows.Count <= 0)
                {
                    return null;
                }

                return laRespuesta;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la verificaciónde la existencia de un reporte
        /// </summary>
        /// <returns>True si existe el reporte</returns>
        public static bool VerificaExisteReporte(string claveReporte, ILogHeader logHeader)
        {
            try
            {
                 DataTable laRespuesta = DAOReportes.VerificaExisteReporte(claveReporte,logHeader);

                if (laRespuesta.Rows.Count > 0)
                {
                    return true;
                }

                return false;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para actualizar el estatus de reporte 
        /// </summary>
        /// <param name="IdReporte">Identificador del reporte</param>
        /// <param name="Estatus">Estatus Reporte</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns> int se se actualizo el registro</returns>
        public static int ActualizaEstatusReporte(int IdReporte, bool Estatus, Usuario usuario, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);
            int resp = 0;
            try
            {
                resp = DAOReportes.ActualizaEstatusReporte(IdReporte, Estatus, usuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                pCI.ErrorException(err);
                throw new CAppException(8011, "Falla al actualizar la estatus reporte.");
            }
            return resp;
        }

        /// <summary>
        /// agrega nuevo reporte 
        /// </summary>
        /// <param name="IdTipoGenReporte">Identificador del tipo de generación del reporte</param>
        /// <param name="ClaveReporte">clave del reporte</param>
        /// <param name="Nombre">nombre del reporte </param>
        /// <param name="SP">procedimiento almacenado que se asignara el reporte</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AgregarNuevoReporte(int IdTipoGenReporte, string ClaveReporte, string Nombre, string SP,
            Usuario usuario, ILogHeader logHeader)
        {
            try
            {
                DAOReportes.AgregarNuevoReporte(IdTipoGenReporte, ClaveReporte, Nombre, SP, usuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar nuevo Reporte ");
            }
        }

        /// <summary>
        /// Modificar datos reporte
        /// </summary>
        /// <param name="IdReporte">identificador del reporte</param>
        /// <param name="IdTipoGenRep">Identificador del tipo de generación de reporte</param>
        /// <param name="Nombre">nombre del reporte </param>
        /// <param name="SP">procedimiento almacenado que se asignara el reporte</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaDatosReporte(int IdReporte, int IdTipoGenRep, string Nombre, string SP, Usuario usuario, ILogHeader logHeader)
        {
            try
            {
                DAOReportes.ModificaDatosReporte(IdReporte, IdTipoGenRep, Nombre, SP, usuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar los datos del reporte ");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar el estatus de la configuración 
        /// de un reporte de colectiva.
        /// </summary>
        /// <param name="IdReporteColectivaConfiguracion">Identificador del registro de la configuración del reporte de colectiva</param>
        /// <param name="EsActivo">Bandera de estatus activo del reporte</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusConfigReporteColectiva(int IdReporteColectivaConfiguracion, int EsActivo,
            Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOReportes.ActualizaEstatusConfigReporteColectiva(IdReporteColectivaConfiguracion, EsActivo,
                    elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus de configuración del reporte");
            }
        }
    }
}
