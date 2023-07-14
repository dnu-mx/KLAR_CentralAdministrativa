using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALCentroContacto.BaseDatos;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using DALCentralAplicaciones.Utilidades;

namespace DALCentroContacto.LogicaNegocio
{
    public class LNReportes
    {
        public static DataSet ReporteClientes(DateTime fechaInicio, DateTime fechaFin, string tipoColectiva, string sucursal, string tipoMedioAcceso, string email, Usuario elUser, Guid AppId,bool IsGrupoReportes=false)
        {
            DataSet laRespuesta = new DataSet();
            try
            {
                laRespuesta = DAOReportes.obtenerReporteClientes(elUser, AppId,fechaInicio, fechaFin, tipoColectiva, sucursal, tipoMedioAcceso, email);

                if(!IsGrupoReportes)
                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }
            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte de Clientes:" + err.Mensaje());
            }
            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte de Clientes: " + err.Message);
            }

        }

        public static DataSet ReporteOperaciones(DateTime fechaInicio, DateTime fechaFin, string tipoColectiva, string sucursal, string tipoMedioAcceso, Usuario elUser, Guid AppId, bool IsGrupoReportes = false)
        {
            DataSet laRespuesta = new DataSet();
            try
            {
                laRespuesta = DAOReportes.obtenerReporteOperaciones(elUser, AppId, fechaInicio, fechaFin, tipoColectiva, sucursal, tipoMedioAcceso);
                if (!IsGrupoReportes)
                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }
            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte de Operaciones:" + err.Mensaje());
            }
            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte de Operaciones: " + err.Message);
            }

        }

        public static DataSet ReporteBeneficios(DateTime fechaInicio, DateTime fechaFin, string tipoColectiva, string sucursal, string tipoPromocion, Usuario elUser, Guid AppId, bool IsGrupoReportes = false)
        {
            DataSet laRespuesta = new DataSet();
            try
            {
                laRespuesta = DAOReportes.obtenerReporteBeneficios(elUser, AppId,fechaInicio, fechaFin, tipoColectiva, sucursal, tipoPromocion);
                if (!IsGrupoReportes)
                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }
            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte de Beneficios:" + err.Mensaje());
            }
            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte de Beneficios: " + err.Message);
            }

        }

        public static DataTable ReporteTiemposRepartoMoshi(DateTime fechaIni, DateTime fechaFin, int IdSucursal, 
            int IdTipoServicio, int IdRepartidor, Usuario elUser)
        {
            DataSet laRespuesta = new DataSet();

            try
            {
                laRespuesta = DAOReportes.ObtenerReporteTiemposRepartoMoshi(fechaIni, fechaFin, IdSucursal, IdTipoServicio,
                    IdRepartidor, elUser);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }

                return laRespuesta.Tables[0];
            }

            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte :" + err.Mensaje());
            }

            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte : " + err.Message);
            }

        }


        public static DataSet ReporteActividades(DateTime fechaInicio, DateTime fechaFin, string tipoColectiva, Usuario elUser, Guid AppId, bool IsGrupoReportes = false)
        {
            DataSet laRespuesta = new DataSet();
            try
            {
                laRespuesta = DAOReportes.obtenerReporteActividades(elUser, AppId, fechaInicio, fechaFin, tipoColectiva);
                if (!IsGrupoReportes)
                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }
            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte de Actividades:" + err.Mensaje());
            }
            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte de Actividades: " + err.Message);
            }

        }

        public static DataSet ReporteLlamadas(DateTime fechaInicio, DateTime fechaFin, string tipoColectiva, Usuario elUser, Guid AppId, bool IsGrupoReportes = false)
        {
            DataSet laRespuesta = new DataSet();
            try
            {
                laRespuesta = DAOReportes.obtenerReporteLlamadas(elUser, AppId,fechaInicio, fechaFin, tipoColectiva);
                if (!IsGrupoReportes)
                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    throw new Exception("La Consulta no generó Resultados");
                }

                return laRespuesta;
            }
            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte de Llamadas:" + err.Mensaje());
            }
            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte de Llamadas: " + err.Message);
            }

        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta del reporte de llamadas en Tiendas Diconsa
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del reporte</returns>
        public static DataSet ReporteLlamadasTiendasDiconsa(DateTime fechaInicial, DateTime fechaFinal, Usuario elUser, Guid AppId, bool IsGrupoReportes = false)
        {
            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fechaInicial, DateTime.MinValue) == 0 ||
                DateTime.Compare(fechaFinal, DateTime.MinValue) == 0)
            {
                throw new CAppException(8006, "El periodo de tiempo de consulta del reporte es obligatorio");
            }

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }

            try
            {
                DataSet dsReporte = DAOReportes.ObtenerReporteLlamadasTiendasDiconsa(fechaInicial, fechaFinal, elUser, AppId);

                if (dsReporte.Tables[0].Rows.Count <= 0)
                    {
                        throw new CAppException(8006, "La Consulta no generó Resultados");
                    }

                return dsReporte;
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



        public static DataTable ReportePedidosMoshi(DateTime fechaInicial, DateTime fechaFinal, int IdSucursal, 
            int IdTipoServicio, Usuario usuario)
        {
            DataSet laRespuesta = new DataSet();

            try
            {
                laRespuesta = DAOReportes.ObtenerReportePedidosMoshi(fechaInicial, fechaFinal, 
                    IdSucursal, IdTipoServicio, usuario);
    
                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }

                return laRespuesta.Tables[0];

                //return  null;
                /*
                laRespuesta = DAOReportes.obtenerReporteBeneficios(elUser, AppId, fechaInicio, fechaFin, tipoColectiva, sucursal, tipoPromocion);
                if (!IsGrupoReportes)
                    if (laRespuesta.Tables[0].Rows.Count <= 0)
                    {
                        throw new Exception("La Consulta no generó Resultados");
                    }

                return laRespuesta;*/
            }
            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte :" + err.Mensaje());
            }
            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte : " + err.Message);
            }
        }

        public static DataTable ReporteLealtadMoshi(DateTime fechaIni, DateTime fechaFin, string sucursal, int idTipoMovimiento,
            Usuario usuario, Guid guid)
        {
            DataSet laRespuesta = new DataSet();
            try
            {
                laRespuesta = DAOReportes.ObtenerReporteLealtadMoshi(fechaIni, fechaFin, sucursal, idTipoMovimiento, usuario, guid);

                if (laRespuesta.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }

                return laRespuesta.Tables[0];
            }

            catch (CAppException err)
            {
                throw new Exception("Obteniendo Reporte :" + err.Mensaje());
            }

            catch (Exception err)
            {
                throw new Exception("Obteniendo Reporte : " + err.Message);
            }
        }
    }
}
