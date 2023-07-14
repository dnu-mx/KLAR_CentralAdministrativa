using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALPuntoVentaWeb.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Conciliaciones
    /// </summary>
    public class DAOConciliaciones
    {
        /// <summary>
        /// Consulta los archivos de Smart Points que coinciden con los filtros marcados dentro del
        /// Procesador de Archivos
        /// </summary>
        /// <param name="nombreArchivo">Nombre del archivo</param>
        /// <param name="fechaArchivo">Fecha en el nombre del archivo</param>
        /// <param name="idEstatus">Identificador del estatus de procesamiento (1 = Procesado, 0 = No procesado)</param>
        /// <param name="fechaProceso">Fecha de procesamiento del archivo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con el resultado de la consulta</returns>
        public static DataSet ObtieneArchivosSmartPoints(String nombreArchivo, DateTime fechaArchivo, int idEstatus,
            DateTime fechaProceso, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDProcesadorArchivos.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneFicherosSmartPoints");

                database.AddInParameter(command, "@NombreArchivo", DbType.String, nombreArchivo);
                database.AddInParameter(command, "@FechaArchivo", DbType.DateTime, 
                    fechaArchivo.Equals(DateTime.MinValue) ? (DateTime?)null : fechaArchivo);
                database.AddInParameter(command, "@IdEstatus", DbType.Int32, idEstatus);
                database.AddInParameter(command, "@FechaProceso", DbType.DateTime, 
                    fechaProceso.Equals(DateTime.MinValue) ? (DateTime?)null : fechaProceso);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el valor WHERE necesario para las consultas de archivos de Smart Points
        /// </summary>
        /// <returns>Cadena con el valor</returns>
        public static String ObtieneWhereSmartPoints(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDProcesadorArchivos.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneConsultaWhereSmartPoints");

                return database.ExecuteScalar(command).ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveColectiva);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las operaciones del ID fichero Smart Points que están en archivo y en la plataforma (conciliadas),
        /// dentro del Procesador de Archivos
        /// </summary>
        /// <param name="laConsulta">Valor WHERE para la consulta de archivos</param>
        /// <param name="idFichero">Identificador del fichero</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con las operaciones</returns>
        public static DataTable ObtieneOpsConciliadas_SmartPoints(String laConsulta, Int64 idFichero, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDProcesadorArchivos.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_OperacionesConciliadas_SmartPoints");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@ConsultaWhere", DbType.String, laConsulta);
                database.AddInParameter(command, "@ID_Fichero", DbType.Int64, idFichero);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las operaciones del ID fichero Smart Points que están en archivo y que no están en la plataforma,
        /// dentro del Procesador de Archivos
        /// </summary>
        /// <param name="laConsulta">Valor WHERE para la consulta de archivos</param>
        /// <param name="idFichero">Identificador del fichero</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con las operaciones</returns>
        public static DataTable ObtieneOpsSiANoP_SmartPoints(String laConsulta, Int64 idFichero, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDProcesadorArchivos.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_OperacionesArchivoSIBDNO_SmartPoints");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@ConsultaWhere", DbType.String, laConsulta);
                database.AddInParameter(command, "@ID_Fichero", DbType.Int64, idFichero);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las operaciones del ID fichero Smart Points que no están en archivo pero sí en la plataforma,
        /// dentro del Procesador de Archivos
        /// </summary>
        /// <param name="laConsulta">Valor WHERE para la consulta de archivos</param>
        /// <param name="idFichero">Identificador del fichero</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con las operaciones</returns>
        public static DataTable ObtieneOpsNoASiP_SmartPoints(String laConsulta, Int64 idFichero, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDProcesadorArchivos.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_OperacionesArchivoNOBDSI_SmartPoints");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@ConsultaWhere", DbType.String, laConsulta);
                database.AddInParameter(command, "@ID_Fichero", DbType.Int64, idFichero);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

    }
}
