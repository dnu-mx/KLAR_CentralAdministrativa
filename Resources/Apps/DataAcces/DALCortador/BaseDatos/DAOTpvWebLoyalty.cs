using DALAutorizador.BaseDatos;
using DALEventos.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DALCortador.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Evento
    /// </summary>
    public class DAOTpvWebLoyalty
    {
        /// <summary>
        /// Consulta las promociones en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtienePromociones(IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosPromociones");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneEventosPromociones";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener las promociones de base de datos.");
            }
        }

        /// <summary>
        /// Consulta los detalles de las promociones en el Autorizador
        /// </summary>
        /// <param name="Id_Evento">Identificador del Evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneDetallePromocion(int Id_Evento, IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDetallePromocion");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, Id_Evento);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneDetallePromocion";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdEvento=" + Id_Evento.ToString());
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener el detalle de la promoción de base de datos.");
            }
        }

        /// <summary>
        /// Consulta el saldo y la colectiva del cupón en base de datos
        /// </summary>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        /// <param name="IdDetalle">Identificador del detalle de la promoción</param>
        /// <param name="Cupon">Código del cupón</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaCuentaSaldoColectivaCupon(int IdPromocion, int IdDetalle, string Cupon, 
            IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSaldoCuentaColectivaCupon");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, IdPromocion);
                database.AddInParameter(command, "@IdGrupoMA", DbType.Int32, IdDetalle);
                database.AddInParameter(command, "@Cupon", DbType.String, Cupon);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneSaldoCuentaColectivaCupon";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdEvento=" + IdPromocion.ToString());
                parametros.Add("P2", "@IdGrupoMA=" + IdDetalle.ToString());
                parametros.Add("P3", "@Cupon=" + Cupon);
                parametros.Add("P4", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P5", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener el saldo del cupón en base de datos.");
            }
        }

        /// <summary>
        /// Consulta en base de datos la información necesaria para el evento manual
        /// "Abono de Saldo Cupón" 
        /// </summary>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoAbonoSaldoCupon(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventoAbonoSaldoCupon");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos la información necesaria para el evento manual
        /// "Cargo de Saldo Cupón" 
        /// </summary>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoCargoSaldoCupon(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventoCargoSaldoCupon");
                
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
