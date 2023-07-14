using DALAutorizador.BaseDatos;
using DALValidacionesBatchPPF.Entidades;
using DALValidacionesBatchPPF.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace DALValidacionesBatchPPF.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Efectivale Online
    /// </summary>
    public class DAOEfectivaleOnline
    {
        /// <summary>
        /// Consulta el catálogo de reglas antifraude en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaReglas(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneReglas");

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
        /// Consulta el catálogo de acciones en base de datos
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
        /// Consulta el catálogo de dictámenes por caso en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaDictamenes(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDictamenCaso");

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
        /// Consulta el catálogo de tipos de fraude en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposFraude(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposFraude");

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
        /// Consulta el catálogo de entidades en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaEntidades(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEntidades");

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
        /// Consulta el catálogo de la entidad con el ID indicado en base de datos
        /// </summary>
        /// <param name="IdEntidad">Identificador de la entidad</param>
        /// <param name="ClaveEntidad">Clave de la entidad</param>
        /// <param name="Descripcion">Descripción de la entidad</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoEntidad(int IdEntidad, string ClaveEntidad, string Descripcion,
            IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoEntidad");

                database.AddInParameter(command, "@IdEntidad", DbType.Int32, IdEntidad);
                database.AddInParameter(command, "@Clave", DbType.String, ClaveEntidad);
                database.AddInParameter(command, "@Descripcion", DbType.String, Descripcion);

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
        /// Consulta en base de datos las incidencias de tarjetas T&E con los filtros indicados
        /// </summary>
        /// <param name="estEFV">Datos de la entidad con los valores a filtrar en la búsqueda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ObtieneIncidenciasVG(EstadisticasTarjetasEFV estEFV, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneIncidenciasVG");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, estEFV.FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, estEFV.FechaFinal);
                database.AddInParameter(command, "@IdRegla", DbType.Int32, estEFV.IdRegla);
                database.AddInParameter(command, "@IdAccion", DbType.Int32, estEFV.IdAccion);
                database.AddInParameter(command, "@Tarjeta", DbType.String, estEFV.NumTarjeta);
                //database.AddInParameter(command, "@NumRegistros", DbType.Int32, estEFV.NumRegistros);
                database.AddInParameter(command, "@ConIncidencias", DbType.Int32, estEFV.OperConIncidencia);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Fecha Inicial: {0}|Fecha Final: {1}|IdRegla: {2}|IdAccion: {3}|Tarjeta: {4}|" +
                    "NumRegistros: {5}|ConIncidencias: {6}|UserTemp: {7}|AppId: {8}", estEFV.FechaInicial, estEFV.FechaFinal,
                    estEFV.IdRegla, estEFV.IdAccion, estEFV.NumTarjeta, estEFV.NumRegistros, estEFV.OperConIncidencia,
                    elUsuario.UsuarioTemp, AppID);
                
                Loguear.Evento("Solicita Reporte a BD", "");

                return database.ExecuteDataSet(command).Tables[0];
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
        public static DataSet ObtieneEstatusTarjeta(String Tarjeta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusTarjetaVG");

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
        /// <param name="Estatus">"Catálogo" filtro de estatus (1 = Aprobadas, 0 = Rechazadas, -1 = Todas)</param>
        /// <param name="Afiliacion">Número de Afiliacion del comercio</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneOperacionesPorTarjeta(String Tarjeta, DateTime FechaInicial, DateTime FechaFinal,
            int Estatus, String Afiliacion, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneOperacionesTarjetaVG");

                database.AddInParameter(command, "@Tarjeta", DbType.String, Tarjeta);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@Estatus", DbType.Int32, Estatus);
                database.AddInParameter(command, "@Afiliacion", DbType.String, Afiliacion);

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
        /// **NOTA: El objeto de BD espera ID de acción o cadena de IDs de acción, uno u otro, nunca los dos
        /// </summary>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idAccion">Identificador de la acción</param>
        /// <param name="Acciones">Cadena de IDs de acción por buscar, separador por ';'</param>
        /// <param name="IdRegla">Identificador de la regla</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneIncidenciasPorTarjeta(String Tarjeta, DateTime FechaInicial,  DateTime FechaFinal,
            int idAccion, String Acciones, int IdRegla, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneIncidenciasTarjetaVG");

                database.AddInParameter(command, "@Tarjeta", DbType.String, Tarjeta);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@IdAccion", DbType.Int32, idAccion);
                database.AddInParameter(command, "@Acciones", DbType.String, Acciones);
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

        /// <summary>
        /// Consulta todas las incidencias de la tarjeta en base de datos
        /// </summary>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneTotalIncidenciasPorTarjeta(String Tarjeta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTodasIncidenciasTarjetaVG");

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
        /// Actualiza en base de datos el estatus de la tarjeta, bloqueándola
        /// </summary>
        /// <param name="noTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaEstatusTarjeta_Bloqueada(String noTarjeta, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_BloqueaTarjeta");

                database.AddInParameter(command, "@Tarjeta", DbType.String, noTarjeta);
                database.AddInParameter(command, "@Usuario", DbType.String, elUser.ClaveUsuario);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("ActualizaEstatusTarjeta_Bloqueada()", Ex);
            }
        }

        /// <summary>
        /// Actualiza en base de datos el estatus de la tarjeta, desbloqueándola
        /// </summary>
        /// <param name="noTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaEstatusTarjeta_Desbloqueada(String noTarjeta, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DesbloqueaTarjeta");

                database.AddInParameter(command, "@Tarjeta", DbType.String, noTarjeta);
                database.AddInParameter(command, "@Usuario", DbType.String, elUser.ClaveUsuario);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("ActualizaEstatusTarjeta_Desbloqueada()", Ex);
            }
        }

        /// <summary>
        /// Actualiza en base de datos el estatus de la tarjeta, desbloqueándola
        /// </summary>
        /// <param name="noTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaEstatusTarjeta_Cancelada(String noTarjeta, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_CancelaTarjeta");

                database.AddInParameter(command, "@Tarjeta", DbType.String, noTarjeta);
                database.AddInParameter(command, "@Usuario", DbType.String, elUser.ClaveUsuario);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("ActualizaEstatusTarjeta_Cancelada()", Ex);
            }
        }

        /// <summary>
        /// Actualiza el estatus de la tarjeta en el Autorizador
        /// </summary>
        /// <param name="idEstatus">Identificador del nuevo estatus</param>
        /// <param name="noTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static string ActualizaEstatusTarjeta(int idEstatus, String noTarjeta, SqlConnection connection,
            SqlTransaction transaccionSQL, IUsuario elUser)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusTarjeta", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@Tarjeta", noTarjeta));
                command.Parameters.Add(new SqlParameter("@IdNuevoEstatus", idEstatus));
                command.Parameters.Add(new SqlParameter("@Usuario", elUser.ClaveUsuario));

                var sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 200;
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();

                return sqlParameter.Value.ToString();
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("ActualizaEstatusTarjeta()", Ex);
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
        /// Inserta en base de datos un nuevo caso cerrado
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="claveDictamen">Clave del dictamen</param>
        /// <param name="idTipoFraude">ID del tipo de fraude</param>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaCasoCerrado(SqlConnection connection, SqlTransaction transaccionSQL,
            String claveDictamen, int idTipoFraude, String tarjeta, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaCasoCerrado", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ClaveDictamen", claveDictamen));
                command.Parameters.Add(new SqlParameter("@IdTipoFraude", idTipoFraude));
                command.Parameters.Add(new SqlParameter("@Tarjeta", tarjeta));

                command.ExecuteNonQuery();
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("InsertaCasoCerrado()", Ex);
            }
        }

        /// <summary>
        /// Actualiza en base de datos el estatus de la operación respuesta regla
        /// </summary>
        /// <param name="IdOperacion">ID de la operación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaOperacionRespuestaRegla(int IdOperacion, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaOperacionRespuestaReglaFraude");

                database.AddInParameter(command, "@IdOperacionRegla", DbType.Int32, IdOperacion);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("ActualizaOperacionRespuestaRegla()", Ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos los parámetros y sus valores de la regla,
        /// entidad y producto indicados
        /// </summary>
        /// <param name="IdRegla">Identificador de la regla</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="IdEntidad">Identificador de la entidad</param>
        /// <param name="IdRegistroEntidad">Identificador del registro de la entidad</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneParametrosRegla(int IdRegla, Int64 IdCadena, 
            int IdEntidad, int IdRegistroEntidad, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosRegla");

                database.AddInParameter(command, "@IdRegla", DbType.Int32, IdRegla);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
                database.AddInParameter(command, "@IdEntidad", DbType.Int32, IdEntidad);
                database.AddInParameter(command, "@IdRegistroEntidad", DbType.Int32, IdRegistroEntidad);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza en base de datos los valores del parámetro-regla indicado
        /// </summary>
        /// <param name="elValor">Nuevo valor del parámetro MA y sus detalles</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaValorParametroRegla(SqlConnection connection, SqlTransaction transaccionSQL, 
            ValorParametroMARegla elValor, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaValorParametroRegla", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdParametro", elValor.ID_ParametroMA));
                command.Parameters.Add(new SqlParameter("@IdEntidad", elValor.ID_Entidad));
                command.Parameters.Add(new SqlParameter("@IdCadena", elValor.ID_CadenaComercial));
                command.Parameters.Add(new SqlParameter("@IdRegistroEntidad", elValor.ID_RegistroEntidad));
                command.Parameters.Add(new SqlParameter("@Valor", elValor.Valor));
                command.Parameters.Add(new SqlParameter("@ValorAlertar", elValor.ValorAlertar));
                command.Parameters.Add(new SqlParameter("@ValorRechazar", elValor.ValorRechazar));
                command.Parameters.Add(new SqlParameter("@ValorBloquear", elValor.ValorBloquear));

                command.ExecuteNonQuery();
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("ActualizaValorParametroRegla()", Ex);
            }
        }

        /// <summary>
        /// Consulta a base de datos si existen incidencias para la tarjeta
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>TRUE en caso de haber incidencias</returns>
        public static Boolean ExistenIncidenciasTarjeta(String numTarjeta, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_HayIncidenciasTarjeta");
                
                database.AddInParameter(command, "@Tarjeta", DbType.String, numTarjeta);
                database.AddOutParameter(command, "@Response", SqlDbType.Bit, 5);

                database.ExecuteNonQuery(command);

                return Convert.ToBoolean(database.GetParameterValue(command, "@Response"));
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta en base de datos una incidencia manual
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="Id_Operacion">ID de la operación</param>
        /// <param name="Tarjeta">Número de tarjeta</param>
        /// <param name="Comentarios">Comentarios de la incidencia manual</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Cadena con la respuesta de base de datos</returns>
        public static string InsertaIncidenciaManual(SqlConnection connection, SqlTransaction transaccionSQL,
            Int64 Id_Operacion, String Tarjeta, String Comentarios, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaIncidenciaManualTarjetaVG", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdOperacion", Id_Operacion));
                command.Parameters.Add(new SqlParameter("@Tarjeta", Tarjeta));
                command.Parameters.Add(new SqlParameter("@Comentarios", Comentarios));


                var sqlParameter = new SqlParameter("@Resultado", SqlDbType.VarChar);
                sqlParameter.Direction = ParameterDirection.Output;
                sqlParameter.Size = 200;
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();

                string response = command.Parameters["@Resultado"].Value.ToString();

                return response;
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("InsertaCasoCerrado()", Ex);
            }
        }
    }
}
