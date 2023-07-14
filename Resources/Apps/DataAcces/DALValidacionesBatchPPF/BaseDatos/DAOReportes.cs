using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALValidacionesBatchPPF.BaseDatos
{
    public class DAOReportes
    {
        /// <summary>
        /// Consulta el reporte de casos dentro del periodo indicado en base de datos
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="fechaIni">Fecha inicial de consulta</param>
        /// <param name="fechaFin">Fecha final de consulta</param>
        /// <param name="idDictamen">Identificador del dictamen</param>
        /// <param name="idTipoFraude">Identificador del tipo de fraude</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del reporte</returns>
        public static DataSet ReporteCasosEFV(String tarjeta, DateTime fechaIni, DateTime fechaFin, int idDictamen,
            int idTipoFraude, Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ReporteCasosTarjetasVG");

                database.AddInParameter(command, "@Tarjeta", DbType.String, tarjeta);
                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdDictamen", DbType.Int32, idDictamen);
                database.AddInParameter(command, "@IdTipoFraude", DbType.Int32, idTipoFraude);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command); ;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las incidencias relacionadas al caso con el identificador proporcionado en base de datos
        /// </summary>
        /// <param name="idCaso">Identificador del caso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneIncidenciasPorCaso(Int64 idCaso, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneIncidenciasCaso");

                database.AddInParameter(command, "@IdCaso", DbType.Int64, idCaso);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
