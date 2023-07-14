using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Evento
    /// </summary>
    public class DAOEvento
    {
        /// <summary>
        /// Consulta los tipos de evento en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposDeEvento(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_ObtieneTiposEvento");

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
        /// Inserta o actualiza un evento en base de datos
        /// </summary>
        /// <param name="elEvento">Valores del tipo Evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaEvento(Evento elEvento, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_InsertaActualizaEvento");

                database.AddInParameter(command, "@Clave", DbType.String, elEvento.Clave);
                database.AddInParameter(command, "@Descripcion", DbType.String, elEvento.Descripcion);
                database.AddInParameter(command, "@Activo", DbType.Boolean, elEvento.Activo);
                database.AddInParameter(command, "@Reversable", DbType.Boolean, elEvento.Reversable);
                database.AddInParameter(command, "@Cancelable", DbType.Boolean, elEvento.Cancelable);
                database.AddInParameter(command, "@Transaccional", DbType.Boolean, elEvento.Transaccional);
                database.AddInParameter(command, "@IdTipoEvento", DbType.Int32, elEvento.ID_TipoEvento);
                database.AddInParameter(command, "@DescEdoCta", DbType.String, elEvento.DescEdoCta);
                database.AddInParameter(command, "@Poliza", DbType.Boolean, elEvento.GeneraPoliza);
                database.AddInParameter(command, "@PreValidaciones", DbType.Boolean, elEvento.PreValidaciones);
                database.AddInParameter(command, "@PostValidaciones", DbType.Boolean, elEvento.PostValidaciones);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los datos básicos del catálogo de eventos en base de datos
        /// </summary>
        /// <param name="claveEvento">Clave del evento</param>
        /// <param name="descrEvento">Descripcióndel evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoEventos(string claveEvento, string descrEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_ObtieneCatalogoEventos");

                database.AddInParameter(command, "@Clave", DbType.String, claveEvento);
                database.AddInParameter(command, "@Desc", DbType.String, descrEvento);
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
        /// Consulta las reglas asociadas o no al evento indicado en base de datos
        /// </summary>
        /// <param name="IdEvento">Identificador del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaReglasEvento(int IdEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_ObtieneEventoReglas");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, IdEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza una relación evento-regla en base de datos
        /// </summary>
        /// <param name="idEvento">Identificador del evento</param>
        /// <param name="idRegla">Identificador de la regla</param>
        /// <param name="activa">Booleano de relación activa o inactiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaReglaEvento(int idEvento, int idRegla, bool activa, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_InsertaActualizaEventosReglas");

                database.AddInParameter(command, "@IdRegla", DbType.Int32, idRegla);
                database.AddInParameter(command, "@IdEvento", DbType.Int32, idEvento);
                database.AddInParameter(command, "@Activa", DbType.Boolean, activa);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        /// <summary>
        /// Consulta los plugins asociados o no al evento indicado en base de datos
        /// </summary>
        /// <param name="IdEvento">Identificador del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaPluginsEvento(int IdEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_ObtieneEventoPlugins");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, IdEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza una relación evento-regla en base de datos
        /// </summary>
        /// <param name="elEventoPlugin">Datos del tipo EventoPlugin por modificar o crear</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaPluginEvento(EventoPlugin elEventoPlugin, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_InsertaActualizaEventosPlugins");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, elEventoPlugin.ID_Evento);
                database.AddInParameter(command, "@IdPlugin", DbType.Int32, elEventoPlugin.ID_Plugin);
                database.AddInParameter(command, "@Activo", DbType.Boolean, elEventoPlugin.Activo);
                database.AddInParameter(command, "@OrdenEjec", DbType.Int32, elEventoPlugin.OrdenEjecucion);
                database.AddInParameter(command, "@RespISO", DbType.Boolean, elEventoPlugin.RespuestaISO);
                database.AddInParameter(command, "@ObligatorioRev", DbType.Boolean, elEventoPlugin.ObligatorioEnReverso);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los plugins del tipo salida asociados o no al evento indicado en base de datos
        /// </summary>
        /// <param name="IdEvento">Identificador del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaPluginsAutorizadorEvento(int IdEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_TRJ_ObtieneEventoPluginsSalida");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, IdEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

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
