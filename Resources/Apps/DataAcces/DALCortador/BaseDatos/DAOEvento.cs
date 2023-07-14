using DALAutorizador.BaseDatos;
using DALEventos.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace DALEventos.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Evento
    /// </summary>
    public class DAOEvento
    {
        /// <summary>
        /// Consulta los tipos de cuenta en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposCuenta(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposCuenta");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los eventos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaEventos(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los estatus de configuración en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaEstatusConfiguracion(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusConfiguracion");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los tipos de contrato en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposContrato(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposContrato");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza un evento en base de datos
        /// </summary>
        /// <param name="elEvento">Valores del tipo Evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaEvento(EventoConfigurador elEvento, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaActualizaEventoEjecConfig");

                database.AddInParameter(command, "@Nombre", DbType.String, elEvento.Nombre);
                database.AddInParameter(command, "@Clave", DbType.String, elEvento.Clave);
                database.AddInParameter(command, "@Descripcion", DbType.String, elEvento.Descripcion);
                database.AddInParameter(command, "@IdTipoCuenta", DbType.Int32, elEvento.ID_TipoCuenta);
                database.AddInParameter(command, "@IdEstatusConfig", DbType.Int32, elEvento.ID_EstatusConfiguracion);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los eventos del configurador en base de datos
        /// </summary>
        /// <param name="claveEvento">Clave del evento</param>
        /// <param name="descrEvento">Descripcióndel evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventosConfigurador(string claveEvento, string descrEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosEjecutorConfiguracion");

                database.AddInParameter(command, "@Clave", DbType.String, claveEvento);
                database.AddInParameter(command, "@Desc", DbType.String, descrEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los eventos automáticos en base de datos
        /// </summary>
        /// <param name="idCfgCorte">Identificador de la configuración de corte</param>
        /// <param name="claveEvento">Clave del evento</param>
        /// <param name="descrEvento">Descripcióndel evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventosAutomaticos(int idCfgCorte, string claveEvento, string descrEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosAutomaticos");

                database.AddInParameter(command, "@IdConfigCorte", DbType.Int32, idCfgCorte);
                database.AddInParameter(command, "@Clave", DbType.String, claveEvento);
                database.AddInParameter(command, "@Desc", DbType.String, descrEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los eventos transaccionales en base de datos
        /// </summary>
        /// <param name="idCfgCorte">Identificador de la configuración de corte</param>
        /// <param name="claveEvento">Clave del evento</param>
        /// <param name="descrEvento">Descripcióndel evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventosTransaccionales(int idCfgCorte, string claveEvento, string descrEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosTransaccionales");

                database.AddInParameter(command, "@IdConfigCorte", DbType.Int32, idCfgCorte);
                database.AddInParameter(command, "@Clave", DbType.String, claveEvento);
                database.AddInParameter(command, "@Desc", DbType.String, descrEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza un evento automático en base de datos
        /// </summary>
        /// <param name="idConfigCorte">Identificador de la configuración de corte</param>
        /// <param name="idEvento">Identificador del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaEventoAgrupado(int idConfigCorte, int idEvento, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaActualizaEventoAgrupado");

                database.AddInParameter(command, "@IdConfigCorte", DbType.Int32, idConfigCorte);
                database.AddInParameter(command, "@IdEvento", DbType.Int32, idEvento);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza un evento transaccional en base de datos
        /// </summary>
        /// <param name="idEvAgr">Identificador del evento agrupado</param>
        /// <param name="idConfigCorte">Identificador de la configuración de corte</param>
        /// <param name="idEvento">Identificador del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaEventoTX(int idEvAgr, int idConfigCorte, int idEvento, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaActualizaEventoOperacion");

                database.AddInParameter(command, "@IdEventoAgrupado", DbType.Int32, idEvAgr);
                database.AddInParameter(command, "@IdConfigCorte", DbType.Int32, idConfigCorte);
                database.AddInParameter(command, "@IdEvento", DbType.Int32, idEvento);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las cadenas comerciales con los datos indicados en base de datos
        /// </summary>
        /// <param name="clave">Clave de la cadena</param>
        /// <param name="nombre">Nombre de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaCadenasComerciales(string clave, string nombre, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenasComerciales");

                database.AddInParameter(command, "@Clave", DbType.String, clave);
                database.AddInParameter(command, "@Nombre", DbType.String, nombre);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los periodos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaPeriodos(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoPeriodos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los cortes de la cadena comercial indicada en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaCortesCadena(int idCadena, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCortesACadena");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta un corte a la cadena comercial o actualiza su periodicidad en base de datos
        /// </summary>
        /// <param name="idCorteAsg">Identificador del corte asignado</param>
        /// <param name="idCad">Identificador de la cadena</param>
        /// <param name="idCfg">Identificador de la configuraíón de corte</param>
        /// <param name="idPeriodo">Identificador del periodo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaPeriodoCorteCadena(int idCorteAsg, int idCad, int idCfg, int idPeriodo, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaActualizaPeriodoCorteCadena");

                database.AddInParameter(command, "@IdAsignacion", DbType.Int32, idCorteAsg);
                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCad);
                database.AddInParameter(command, "@IdConfigCorte", DbType.Int32, idCfg);
                database.AddInParameter(command, "@IdPeriodo", DbType.Int32, idPeriodo);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza el corte a la cadena comercial en base de datos
        /// </summary>
        /// <param name="idCorteAsg">Identificador del corte asignado</param>
        /// <param name="idCad">Identificador de la cadena</param>
        /// <param name="idCfg">Identificador de la configuraíón de corte</param>
        /// <param name="idPeriodo">Identificador del periodo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaCorteCadena(int idCorteAsg, int idCad, int idCfg, int idPeriodo, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaActualizaCorteCadena");

                database.AddInParameter(command, "@IdAsignacion", DbType.Int32, idCorteAsg);
                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCad);
                database.AddInParameter(command, "@IdConfigCorte", DbType.Int32, idCfg);
                database.AddInParameter(command, "@IdPeriodo", DbType.Int32, idPeriodo);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los recolectores Diconsa en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaRecolectoresDiconsa(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_EV_ObtieneRecolectoresDiconsa");

                database.AddInParameter(command, "@ClaveColectiva", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el importe del saldo actual del Recolector Diconsa en base de datos
        /// </summary>
        /// <param name="IdRecolector">Identificador del Recolector</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Cadena con el importe de la deuda</returns>
        public static string ConsultaSaldoRecolectorDiconsa(int IdRecolector, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_EV_ObtieneSaldoRecolectorDiconsa");

                database.AddInParameter(command, "@IdRecolector", DbType.Int32, IdRecolector);

                return database.ExecuteScalar(command).ToString();
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos los datos básicos del evento 
        /// "Recepción de Efectivo en Almacén Diconsa" 
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoRecepcionEfectivoDiconsa(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_EV_ObtieneTiposColectivaYEventoEfectivoDiconsa");

                database.AddInParameter(command, "@ClaveColectiva", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos los datos básicos del evento 
        /// "Depósito de Recolector Diconsa" 
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoDepositoRecolectorDiconsa(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_EV_ObtieneDatosDepositoRecolectorDiconsa");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las colectivas con cuentas eje
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaClientesCuentasEjeCacao(IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasEje");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneCuentasEje";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDBG.Parameters = parametros;

                log.Debug(logDBG);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Clientes.");
            }
        }

        /// <summary>
        /// Consulta los movimientos al saldo de las cuentas eje
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaMovimientosCuentasEjeCacao(IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMovimientosCuentaEje");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneMovimientosCuentaEje";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los movimientos de la cuenta eje.");
            }
        }

        /// <summary>
        /// Consulta el histórico de dispersiones al saldo de las cuentas eje
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo a consultar</param>
        /// <param name="FechaFinal">Fecha final del periodo a consultar</param>
        /// <param name="IdTipoMov">Identificador del tipo de movimiento</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="Estatus">Estatus del movimiento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaHistoricoDispersionesCuentasEje(DateTime FechaInicial, DateTime FechaFinal,
            int IdTipoMov, Int64 IdCuenta, string Estatus, IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteMovimientosCuentaEje");

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@IdTipoMovimiento", DbType.Int32, IdTipoMov);
                database.AddInParameter(command, "@IdCuenta", DbType.Int64, IdCuenta);
                database.AddInParameter(command, "@Estatus", DbType.String, Estatus);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ReporteMovimientosCuentaEje";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                parametros.Add("P3", "@IdTipoMovimiento=" + IdTipoMov);
                parametros.Add("P4", "@IdCuenta=" + IdCuenta);
                parametros.Add("P5", "@Estatus=" + Estatus);
                parametros.Add("P6", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P7", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de base de datos");
            }
        }

        /// <summary>
        /// Realiza la inserción de una nueva solicitud de movimiento de cuenta eje
        /// en la tabla de control del Autorizador
        /// </summary>
        /// <param name="idCuenta">Identificador de la cuenta</param>
        /// <param name="nuevoSaldo">Nuevo saldo de la cuenta</param>
        /// <param name="importe">Importe a aumentar o decrementar del saldo actual de la cuenta</param>
        /// <param name="idMov">Identificador del tio de movimiento</param>
        /// <param name="Obs">Observaciones</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaMovimientoSaldoCuentaEje(int idCuenta, String nuevoSaldo, String importe,
            int idMov, String Obs, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaMovimientoCuentaEje", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", idCuenta));
                        command.Parameters.Add(new SqlParameter("@NuevoSaldo", nuevoSaldo));
                        command.Parameters.Add(new SqlParameter("@Importe", importe));
                        command.Parameters.Add(new SqlParameter("@TipoMovimiento", idMov));
                        command.Parameters.Add(new SqlParameter("@Observaciones", Obs));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaMovimientoCuentaEje";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + idCuenta);
                        parametros.Add("P2", "@NuevoSaldo=" + nuevoSaldo);
                        parametros.Add("P3", "@Importe=" + importe);
                        parametros.Add("P4", "@TipoMovimiento=" + idMov);
                        parametros.Add("P5", "@Observaciones=" + Obs);
                        parametros.Add("P6", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el registro de movimiento en base de datos.");
            }
        }

        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaEjecucionEvManualTarjetahabiente(int idCuenta, String nuevoSaldo, String importe,
            int idMov, String Obs, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaMovimientoCuentaEje", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", idCuenta));
                        command.Parameters.Add(new SqlParameter("@NuevoSaldo", nuevoSaldo));
                        command.Parameters.Add(new SqlParameter("@Importe", importe));
                        command.Parameters.Add(new SqlParameter("@TipoMovimiento", idMov));
                        command.Parameters.Add(new SqlParameter("@Observaciones", Obs));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaMovimientoCuentaEje";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + idCuenta);
                        parametros.Add("P2", "@NuevoSaldo=" + nuevoSaldo);
                        parametros.Add("P3", "@Importe=" + importe);
                        parametros.Add("P4", "@TipoMovimiento=" + idMov);
                        parametros.Add("P5", "@Observaciones=" + Obs);
                        parametros.Add("P6", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el registro de movimiento en base de datos.");
            }
        }

        /// <summary>
        /// Consulta al Autorizador los datos requeridos para el evento de Fondeo Cliente (cuenta eje)
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva elegida (cliente)</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los datos requeridos</returns>
        public static DataSet ConsultaEventoFondeoCliente(Int64 idColectiva, IUsuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosEventoFondeoCliente");

                database.AddInParameter(command, "@IdColectivaPadre", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneDatosEventoFondeoCliente";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectivaPadre=" + idColectiva);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los datos del evento de fondeo.");
            }
        }

        /// <summary>
        /// Consulta al Autorizador los datos requeridos para el evento de Decremento Cliente (cuenta eje)
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva elegida (cliente)</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los datos requeridos</returns>
        public static DataSet ConsultaEventoDecrementoCliente(Int64 idColectiva, IUsuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosEventoDecrementoCliente");

                database.AddInParameter(command, "@IdColectivaPadre", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneDatosEventoDecrementoCliente";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectivaPadre=" + idColectiva);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDBG.Parameters = parametros;

                logPCI.Debug(logDBG);
                /////<<<LOG DEBUG
                ///
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los datos del evento de decremento.");
            }
        }

        /// <summary>
        /// Registra en bitácora de base de datos el cambio al saldo actual de la cuenta eje.
        /// </summary>
        /// <param name="idMovCtaEje">Identificador del registro de movimientos</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoSaldo">Nuevo saldo de la cuenta</param>
        /// <param name="Ejecutor">Usuario ejecutor del movimiento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraAjusteSaldoCuentaEje(int idMovCtaEje, Int64 IdCuenta, string NuevoSaldo,
            string Ejecutor, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RegistraAjusteCuentaEje", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMovimientoCuentaEje", idMovCtaEje));
                        command.Parameters.Add(new SqlParameter("@IdCuenta", IdCuenta));
                        command.Parameters.Add(new SqlParameter("@NuevoSaldo", NuevoSaldo));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", Ejecutor));
                        command.Parameters.Add(new SqlParameter("@UsuarioAutorizador", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_RegistraAjusteCuentaEje";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMovimientoCuentaEje=" + idMovCtaEje);
                        parametros.Add("P2", "@IdCuenta=" + IdCuenta);
                        parametros.Add("P3", "@NuevoSaldo=" + NuevoSaldo);
                        parametros.Add("P4", "@UsuarioEjecutor=" + Ejecutor);
                        parametros.Add("P5", "@UsuarioAutorizador=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Error al registrar el cambio en el saldo de la cuenta en base de datos.");
            }
        }

        /// <summary>
        /// Registra en base de datos el rechazo al ajuste de saldo de una cuenta eje
        /// </summary>
        /// <param name="idMovCtaEje">Identificador del registro en la tabla de control</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraRechazoAjusteSaldoCuentaEje(int idMovCtaEje, IUsuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RegistraRechazoAjusteCuentaEje", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMovimientoCuentaEje", idMovCtaEje));
                        command.Parameters.Add(new SqlParameter("@UsuarioRechaza", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg _logDebug = new LogDebugMsg();
                        _logDebug.M_Value = "web_CA_RegistraRechazoAjusteCuentaEje";
                        _logDebug.C_Value = "";
                        _logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMovimientoCuentaEje=" + idMovCtaEje);
                        parametros.Add("P2", "@UsuarioRechaza=" + elUsuario.ClaveUsuario);
                        _logDebug.Parameters = parametros;

                        pCI.Debug(_logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Error al registrar el rechazo al ajuste del saldo de la cuenta en base de datos.");
            }
        }

        /// <summary>
        /// Consulta los eventos de tipo traspaso entre cuentas del mismo tipo
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaEventosTraspaso(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosTraspaso");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de colectivas por tipo de cuenta del Autorizador
        /// </summary>
        /// <param name="idTCta">Identificador del tipo de cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaColectivasTraspasos(int idTCta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColectivasTraspasos");

                database.AddInParameter(command, "@IdTipoCuenta", DbType.Int32, idTCta);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los eventos manuales, permitidos para el usuario y la aplicación,
        /// para administración
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaEventosManualesParaAdministrar(IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Eventos_ObtieneEventosManualesAdmin");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_Eventos_ObtieneEventosManualesAdmin";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Eventos Manuales");
            }
        }

        /// <summary>
        /// Inserta un nuevo evento manual en el Autorizador, registrando la inserción en bitácora
        /// </summary>
        /// <param name="clave">Clave del nuevo evento manual</param>
        /// <param name="desc">Descripción del nuevo evento manual</param>
        /// <param name="descEdoCta">Descripción para el estado de cuenta del nuevo evento manual</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="log">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con la respuesta del SP</returns>
        public static DataTable InsertaNuevoEventoManual(string clave, string desc, string descEdoCta,
            int idTipoMov, IUsuario elUsuario, ILogHeader log)
        {
            LogPCI pCI = new LogPCI(log);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_Eventos_InsertaEventoManual", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Clave", clave));
                        command.Parameters.Add(new SqlParameter("@Descripcion", desc));
                        command.Parameters.Add(new SqlParameter("@DescripcionEdoCta", descEdoCta));
                        command.Parameters.Add(new SqlParameter("@IdTipoMovimiento", idTipoMov));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@IdEvento", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Respuesta", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 100;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        DataTable dt = new DataTable();
                        dt.Columns.Add("IdNuevoEvento");
                        dt.Columns.Add("Respuesta");
                        dt.Rows.Add();

                        dt.Rows[0]["IdNuevoEvento"] = sqlParameter1.Value.ToString();
                        dt.Rows[0]["Respuesta"] = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg elLog = new LogDebugMsg();
                        elLog.M_Value = "web_Eventos_InsertaEventoManual";
                        elLog.C_Value = "IdNuevoEvento=" + sqlParameter1.Value.ToString();
                        elLog.R_Value = sqlParameter2.Value.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Clave=" + clave);
                        parametros.Add("P2", "@Descripcion=" + desc);
                        parametros.Add("P3", "@DescripcionEdoCta=" + descEdoCta);
                        parametros.Add("P4", "@IdTipoMovimiento=" + idTipoMov);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        elLog.Parameters = parametros;

                        pCI.Debug(elLog);
                        /////<<<LOG DEBUG

                        return dt;
                    }
                }
            }
            catch (Exception Ex)
            {
                pCI.ErrorException(Ex);
                throw new CAppException(8010, "Ha ocurrido un eror al registrar el nuevo evento manual en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el estatus de un evento manual en el Autorizador
        /// </summary>
        /// <param name="idEvento">Identificador del evento por modificar</param>
        /// <param name="estatus">Nuevo estatus del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="log">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusEventoManual(int idEvento, int estatus, IUsuario elUsuario, ILogHeader log)
        {
            LogPCI pCI = new LogPCI(log);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaActivoEventoManual", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdEvento", idEvento));
                        command.Parameters.Add(new SqlParameter("@Estatus", estatus));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaActivoEventoManual";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdEvento=" + idEvento);
                        parametros.Add("P2", "@Estatus=" + estatus);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        pCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al activar/desactivar el evento en base de datos");
            }
        }

        /// <summary>
        /// Actualiza las descripciones de un evento manual en el Autorizador
        /// </summary>
        /// <param name="idEvento">Identificador del evento por modificar</param>
        /// <param name="desc">Descripción del evento manual</param>
        /// <param name="descEdoCta">Descripción para el estado de cuenta del evento manual</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="log">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDescripcionesEventoManual(int idEvento, string desc, string descEdoCta,
            IUsuario elUsuario, ILogHeader log)
        {
            LogPCI logPCI = new LogPCI(log);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_Eventos_ActualizaDescripcionesEventoManual", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdEvento", idEvento));
                        command.Parameters.Add(new SqlParameter("@Descripcion", desc));
                        command.Parameters.Add(new SqlParameter("@DescripcionEdoCta", descEdoCta));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_Eventos_ActualizaDescripcionesEventoManual";
                        logDbg.C_Value = "";
                        logDbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdEvento=" + idEvento);
                        parametros.Add("P2", "@Descripcion=" + desc);
                        parametros.Add("P3", "@DescripcionEdoCta=" + descEdoCta);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDbg.Parameters = parametros;

                        logPCI.Debug(logDbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la descripción del evento.");
            }
        }

        /// <summary>
        /// Consulta los movimientos de fondeo rápido pendientes de autorización
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaMovimientosFondeoRapidoCacao(IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMovimientosFondeoRapido");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable _dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneMovimientosFondeoRapido";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG

                return _dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener los movimientos de Fondeo Rápido de base de datos.");
            }
        }

        /// <summary>
        /// Registra en bitácora de base de datos el cambio al saldo actual de la cuenta indicada,
        /// autorizada por fondeo rápido
        /// </summary>
        /// <param name="idMovFR">Identificador del registro de movimientos</param>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="NuevoSaldo">Nuevo saldo de la cuenta</param>
        /// <param name="Ejecutor">Usuario ejecutor del movimiento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraAbonoFondeoRapido(int idMovFR, Int64 IdCuenta, string NuevoSaldo,
            string Ejecutor, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RegistraMovimientoFondeoRapido", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMovFondeoRapido", idMovFR));
                        command.Parameters.Add(new SqlParameter("@IdCuenta", IdCuenta));
                        command.Parameters.Add(new SqlParameter("@NuevoSaldo", NuevoSaldo));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", Ejecutor));
                        command.Parameters.Add(new SqlParameter("@UsuarioAutorizador", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_RegistraMovimientoFondeoRapido";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMovFondeoRapido=" + idMovFR);
                        parametros.Add("P2", "@IdCuenta=" + IdCuenta);
                        parametros.Add("P3", "@NuevoSaldo=" + NuevoSaldo);
                        parametros.Add("P4", "@UsuarioEjecutor=" + Ejecutor);
                        parametros.Add("P5", "@UsuarioAutorizador=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                log.ErrorException(Ex);
                throw new CAppException(8010, "Ha sucedido un error al registrar en bitácora el cambio" +
                    " en el saldo de la cuenta con ID " + IdCuenta.ToString());
            }
        }

        /// <summary>
        /// Registra en base de datos el rechazo al fondeo rápido una cuenta
        /// </summary>
        /// <param name="idMovFR">Identificador del registro en la tabla de control</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraRechazoFondeoRapido(int idMovFR, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RegistraRechazoFondeoRapido", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMovFondeoRapido", idMovFR));
                        command.Parameters.Add(new SqlParameter("@UsuarioRechaza", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_RegistraRechazoFondeoRapido";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdMovFondeoRapido=" + idMovFR);
                        parametros.Add("P2", "@UsuarioRechaza=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ha sucedido un error al registrar el rechazo al movimiento del saldo " +
                    "de la cuenta en bitácora.");
            }
        }

        /// <summary>
        /// Inserta un registro en la bitácora detalle del Autorizador
        /// </summary>
        /// <param name="SP">Nombre del procedimiento almacenado que realizó la inserción/actualización</param>
        /// <param name="tabla">Nombre de la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="campo">Nombre del campo en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="idRegistro">Identificador del registro en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="valor">Valor del campo en la tabla a la que se realizó la inserción/actualización</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaRegistroBitacora(string SP, string tabla, string campo, string idRegistro,
            string valor, string observaciones, SqlConnection connection, SqlTransaction transaccionSQL,
            IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaRegistroBitacoraDetalle", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@NombreSP", SP));
                command.Parameters.Add(new SqlParameter("@TablaModificada", tabla));
                command.Parameters.Add(new SqlParameter("@CampoModificado", campo));
                command.Parameters.Add(new SqlParameter("@ID_RegistroModificado", idRegistro));
                command.Parameters.Add(new SqlParameter("@NuevoValorCampo", valor));
                command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));
                command.Parameters.Add(new SqlParameter("@Observaciones", observaciones));


                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_InsertaRegistroBitacoraDetalle";
                logDbg.C_Value = "";
                logDbg.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@NombreSP=" + SP);
                parametros.Add("P2", "@TablaModificada=" + tabla);
                parametros.Add("P3", "@CampoModificado=" + campo);
                parametros.Add("P4", "@ID_RegistroModificado=" + idRegistro.ToString());
                parametros.Add("P5", "@NuevoValorCampo=" + valor);
                parametros.Add("P6", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                parametros.Add("P7", "@Observaciones=" + observaciones);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el registro en base de datos");
            }
        }


        /// <summary>
        /// Consulta los eventos manuales de "tipo" tarjetahabiente en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaEventosManualesTarjetahabiente(IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosTarjetahabiente");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable _dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneEventosTarjetahabiente";
                logDebug.C_Value = "";
                logDebug.R_Value = string.Join(Environment.NewLine,
                    _dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG

                return _dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener los eventos manuales de base de datos.");
            }
        }

        /// <summary>
        /// Valida si los permisos del usuario en sesión son suficientes para ejecutar eventos manuales al
        /// tarjetahabiente
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos de respuesta</returns>
        public static DataTable ValidaPermisosASubemisor(string numTarjeta, IUsuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ValidaSubemisorUsuario", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        SqlCommand cmd = new SqlCommand();
                        SqlParameter param = cmd.CreateParameter();
                        param.ParameterName = "@Tarjeta";
                        param.DbType = DbType.AnsiStringFixedLength;
                        param.Direction = ParameterDirection.Input;
                        param.Value = numTarjeta;
                        param.Size = numTarjeta.Length;
                        command.Parameters.Add(param);

                        command.Parameters.Add(new SqlParameter("@UserTemp", elUsuario.UsuarioTemp));
                        command.Parameters.Add(new SqlParameter("@AppId", AppID));

                        var sqlParameter1 = new SqlParameter("@MensajeResp", SqlDbType.VarChar);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        sqlParameter1.Size = 100;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@SaldoCLDC", SqlDbType.Money);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter2);

                        var sqlParameter3 = new SqlParameter("@ClaveSubemisor", SqlDbType.VarChar);
                        sqlParameter3.Direction = ParameterDirection.Output;
                        sqlParameter3.Size = 30;
                        command.Parameters.Add(sqlParameter3);

                        conn.Open();

                        command.ExecuteNonQuery();

                        DataTable dt = new DataTable();
                        dt.Columns.Add("Respuesta");
                        dt.Columns.Add("SaldoCLDC");
                        dt.Columns.Add("ClaveSubemisor");
                        dt.Rows.Add();

                        string resp = command.Parameters["@MensajeResp"].Value.ToString();
                        dt.Rows[0]["Respuesta"] = resp;
                        dt.Rows[0]["SaldoCLDC"] = sqlParameter2.Value;
                        dt.Rows[0]["ClaveSubemisor"] = sqlParameter3.Value;

                        /////>>>LOG DEBUG
                        LogDebugMsg logDb = new LogDebugMsg();
                        logDb.M_Value = "web_CA_ValidaSubemisorUsuario";
                        logDb.C_Value = "";
                        logDb.R_Value = resp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Tarjeta=" + MaskSensitiveData.cardNumber(numTarjeta));
                        parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                        parametros.Add("P3", "@AppId=" + AppID);
                        logDb.Parameters = parametros;

                        log.Debug(logDb);
                        /////<<<LOG DEBUG

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al validar los permisos del usuario en base de datos");
            }
        }

        /// <summary>
        /// Registra en bitácora de base de datos la ejecución de un evento manual de "tipo" tarjetahabiente
        /// </summary>
        /// <param name="idPoliza">Identificador de la póliza</param>
        /// <param name="importe">Importe registrado en la póliza</param>
        /// <param name="observaciones">Observaciones del evento manual</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaEjecucionEvManualTH(long idPoliza, string importe, string observaciones,
            SqlConnection connection, SqlTransaction transaccionSQL, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_RegistraEventoManualTarjetahabiente", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdPoliza", idPoliza));
                command.Parameters.Add(new SqlParameter("@Importe", importe));
                command.Parameters.Add(new SqlParameter("@Observ", observaciones));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_RegistraEventoManualTarjetahabiente";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdPoliza=" + idPoliza.ToString());
                parametros.Add("P2", "@Importe=" + importe);
                parametros.Add("P3", "@Observ=" + observaciones);
                parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el registro de ejecución de evento manual en base de datos");
            }
        }
    }
}
