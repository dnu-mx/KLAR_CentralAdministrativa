using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;


namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Clientes
    /// </summary>
    public class DAOTiendasDiconsa
    {
        /// <summary>
        /// Consulta las tiendas que coinciden con los datos de ingreso en base de datos
        /// </summary>
        /// <param name="laTienda">Parámetros de búsqueda de la tienda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneTiendas(TiendaDiconsa laTienda, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneTiendasDiconsa");

                database.AddInParameter(command, "@Movil", DbType.String, laTienda.ClaveColectiva);
                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, laTienda.ClaveAlmacen);
                database.AddInParameter(command, "@ClaveTienda", DbType.String, laTienda.ClaveTienda);
                database.AddInParameter(command, "@ApPaterno", DbType.String, laTienda.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, laTienda.ApellidoMaterno);
                database.AddInParameter(command, "@Nombre", DbType.String, laTienda.Nombre);

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
        /// Consulta los datos de la tienda seleccionada en base de datos
        /// </summary>
        /// <param name="idColectiva">Identificador de la tienda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosTienda(int idColectiva, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneDatosTiendaDiconsa");

                database.AddInParameter(command, "@IdTienda", DbType.Int32, idColectiva);
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
        /// Actualiza los datos del operador de la tienda Diconsa en base de datos
        /// </summary>
        /// <param name="datosTienda">Datos del operador por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void actualizaDatosOperador(TiendaDiconsa datosOperdor, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ActualizaDatosOperadorDiconsa");

                database.AddInParameter(command, "@IdOperador", DbType.Int32, datosOperdor.ID_Operador);
                database.AddInParameter(command, "@eMail", DbType.String, datosOperdor.Email);
                database.AddInParameter(command, "@Movil", DbType.String, datosOperdor.Movil);
                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizaron los Datos del Operador en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene los tipos de operación permitidos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposOperacion(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneTiposOperacion");

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
        /// Obtiene los tipos de cuenta permitidos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposCuenta(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneTiposCuenta");

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
        /// Obtiene de base de datos los movimientos transaccionales del medio de acceso indicado
        /// </summary>
        /// <param name="fIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fFin">Fecha final del periodo de consulta</param>
        /// <param name="idTC">Identificador del tipo de cuenta</param>
        /// <param name="idTop">Identificador del tipo de operación</param>
        /// <param name="idCol">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la apliación</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneMovimientosTX(DateTime fIni, DateTime fFin, int idTC, int idTop, int idCol, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneMovimientosDiconsa");

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fFin);
                database.AddInParameter(command, "@IdTipoCuenta", DbType.Int32, idTC);
                database.AddInParameter(command, "@IdTipoOperacion", DbType.Int32, idTop);
                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idCol);
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
        /// Obtiene el catálogo de actividades o motivos de llamada de base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaMotivosLlamada(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneActividades");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta en la bitácora de actividades de base de datos la llamada del cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="idActividad">Identificador de la actividad</param>
        /// <param name="comments">Comentarios u observaciones</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaLlamadaCliente(int idCliente, int idActividad, string comments, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_InsertaLlamadaBitacoraActividades");
                
                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
                database.AddInParameter(command, "@UserID", DbType.Guid, elUsuario.UsuarioId);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@IdActividad", DbType.Int32, idActividad);
                database.AddInParameter(command, "@Comentarios", DbType.String, comments);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó la Llamada del Cliente en la Bitácora del Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

                /// <summary>
        /// Inserta una acción en la bitácora de actividades de base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="accion">Acción realizada</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActividad(int idCliente, string accion, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_InsertaAccionBitacoraActividades");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@Accion", DbType.String, accion);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Acción en la Bitácora de Actividades del Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene las credenciales para la conexión al Web Service de Sr Pago en base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadenade conexión a BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static DataSet ObtenerCredencialesWS(Int32 IdColectiva, String CadenaConexion, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneCredencialesSrPago");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, Ex.Message, Ex);
            }
        }
    }
}
