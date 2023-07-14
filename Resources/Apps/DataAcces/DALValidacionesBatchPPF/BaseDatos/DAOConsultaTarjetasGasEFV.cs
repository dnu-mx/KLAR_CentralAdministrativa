using DALAutorizador.BaseDatos;
using DALValidacionesBatchPPF.Entidades;
using DALValidacionesBatchPPF.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALValidacionesBatchPPF.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Tarjetas Efectivale
    /// </summary>
    public class DAOConsultaTarjetasGasEFV
    {
        /// <summary>
        /// Consulta el catálogo de acciones sugeridas en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaAcciones(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneAccionesSugeridas");

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

        /// <summary>
        /// Consulta en base de datos las estadísticas de la tarjeta con los filtros indicados
        /// </summary>
        /// <param name="estEFV">Datos de la entidad con los valores a filtrar en la búsqueda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneEstadisticasTarjeta(EstadisticasTarjetasEFV estEFV, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstadisticasTarjeta");

                database.AddInParameter(command, "@FechaInicial", DbType.Date, estEFV.FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, estEFV.FechaFinal);
                database.AddInParameter(command, "@Tarjeta", DbType.String, estEFV.NumTarjeta);
                database.AddInParameter(command, "@Acciones", DbType.String, estEFV.Acciones);
                database.AddInParameter(command, "@IdRegla", DbType.Int32, estEFV.IdRegla);
                database.AddInParameter(command, "@NumRegistros", DbType.Int32, estEFV.NumRegistros);

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

        /// <summary>
        /// Consulta en base de datos el estatus de la tarjeta
        /// </summary>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con la descripción y la bandera  estatus de la tarjeta:
        /// 0 = ACTIVA, 1 = BLOQUEADA, -1 = OTRO</returns>
        public static DataSet ConsultaEstatusTarjeta(String Tarjeta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusTarjetaGas");

                database.AddInParameter(command, "@Tarjeta", DbType.String, Tarjeta);

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

        /// <summary>
        /// Consulta en base de datos las operaciones realizadas por la tarjeta con los filtros indicados
        /// </summary>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdComercio">Número de Estación de Servicio</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneOperacionesPorTarjeta(String Tarjeta, DateTime FechaInicial, DateTime FechaFinal,
            String IdComercio, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneOperacionesTarjetaGas");

                database.AddInParameter(command, "@Tarjeta", DbType.String, Tarjeta);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@IdComercio", DbType.String, IdComercio);                

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

        /// <summary>
        /// Consulta las incidencias por tarjeta en base de datos que coinciden con los filtros indicados
        /// </summary>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="Accion">Cadena con los IDs de acción concatenados con ';'</param>
        /// <param name="IdRegla">Identificador de la regla</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneIncidenciasPorTarjeta(String Tarjeta, DateTime FechaInicial, DateTime FechaFinal,
            String Accion, int IdRegla, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneIncidenciasTarjetaGas");

                database.AddInParameter(command, "@Tarjeta", DbType.String, Tarjeta);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@Acciones", DbType.String, Accion);
                database.AddInParameter(command, "@IdRegla", DbType.Int32, IdRegla);           

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
