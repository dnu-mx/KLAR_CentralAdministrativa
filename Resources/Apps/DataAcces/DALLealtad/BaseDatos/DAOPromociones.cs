using DALAutorizador.BaseDatos;
using DALLealtad.Entidades;
using DALLealtad.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Moshi
    /// </summary>
    public class DAOPromociones
    {
        /// <summary>
        /// Consulta las promociones en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtienePromociones(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosPromociones");

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
        /// Consulta los detalles de las promociones en el Autorizador
        /// </summary>
        /// <param name="Id_Evento">Identificador del Evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneDetallePromocion(int Id_Evento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDetallePromocion");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, Id_Evento);

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
        /// Consulta en base de datos las fechas de vigencia de la promoción para la promoción y
        /// el detalle indicados
        /// </summary>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        /// <param name="IdDetalle">Identificador del detalle de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Diccionario con las fechas</returns>
        public static DataSet ObtieneFechasVigencia(int IdPromocion, int IdDetalle, IUsuario elUsuario, Guid AppID) 
        {
            try
            {
                DataSet dsResponse = new DataSet();

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneVigenciaPromocion");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, IdPromocion);
                database.AddInParameter(command, "@IdGrupoMA", DbType.Int64, IdDetalle);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                dsResponse = database.ExecuteDataSet(command);

                for (int registro = 0; registro < dsResponse.Tables[0].Rows.Count; registro++)
                {
                    DateTime dt = Convert.ToDateTime(dsResponse.Tables[0].Rows[registro]["Valor"].ToString());

                    if (dt.Year == 1900)
                    {
                        dsResponse.Tables[0].Rows[registro]["Valor"] = "";
                    }
                }

                return dsResponse;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Actualiza en base de datos la vigencia de la promoción indicada
        /// </summary>
        /// <param name="idEvento">Identificador del evento o promoción</param>
        /// <param name="valor">Valor de la pertenencia</param>
        /// <param name="inicial">Bandera de identificación de la fecha inicial</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaVigenciaPromocion(int idEvento, String valor, bool inicial, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaFechaVigenciaPromocion");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, idEvento);
                database.AddInParameter(command, "@ValorFecha", DbType.Date, valor);
                database.AddInParameter(command, "@EsFechaInicial", DbType.Boolean, inicial);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("ActualizaVigenciaPromocion()", Ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de emisores de programa en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ListaEmisores(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEmisoresPrograma");

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
        /// Consulta el catálogo de candenas comerciales en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ListaCadenasComerciales(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCadenasComerciales");

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
        /// Consulta el catálogo de sucursales que corresponden a la 
        /// cadena comercial en el Autorizador
        /// </summary>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ListaSucursalesDeCadena(Int64 IdCadena, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursalesDeCadena");

                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
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
        /// Consulta el reporte de redenciones e-Commerce con los filtros indicados
        /// </summary>
        /// <param name="Emisor">Emisor del cupón</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <param name="ClaveEstado">Clave del estado de la República Mexicana</param>
        /// <param name="Ciudad">Nombre de la ciudad</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con el resultado</returns>
        public static DataSet ObtieneReporteRedenciones(String Emisor, Int64 IdCadena, Int64 IdSucursal,
            String ClaveEstado, String Ciudad, DateTime FechaInicial, DateTime FechaFinal,
            IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteRedencionesEcommerce");

                database.AddInParameter(command, "@Emisor", DbType.String, Emisor);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
                database.AddInParameter(command, "@IdSucursal", DbType.Int64, IdSucursal);
                database.AddInParameter(command, "@ClaveEstado", DbType.String, ClaveEstado);
                database.AddInParameter(command, "@Ciudad", DbType.String, Ciudad);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);

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
        /// Consulta el reporte de operaciones de sucursales e-Commerce con los filtros indicados
        /// </summary>
        /// <param name="Emisor">Emisor del cupón</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="IdSucursal">Identificador de la sucursal</param>
        /// <param name="ClaveEstado">Clave del estado de la República Mexicana</param>
        /// <param name="Ciudad">Nombre de la ciudad</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con el resultado</returns>
        public static DataSet ObtieneReporteOperacionesSucursales(String Emisor, Int64 IdCadena, Int64 IdSucursal,
            String ClaveEstado, String Ciudad, DateTime FechaInicial, DateTime FechaFinal, IUsuario elUsuario,
            Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteOperacionesSucursales");

                database.AddInParameter(command, "@Emisor", DbType.String, Emisor);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
                database.AddInParameter(command, "@IdSucursal", DbType.Int64, IdSucursal);
                database.AddInParameter(command, "@ClaveEstado", DbType.String, ClaveEstado);
                database.AddInParameter(command, "@Ciudad", DbType.String, Ciudad);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);

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
        /// Consulta el catálogo de colectivas según su clave en base de datos
        /// </summary>
        /// <param name="idColectivaPadre">Identificador de la colectiva padre</param>
        /// <param name="claveColectiva">Clave de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con el resultado</returns>
        public static DataSet ObtieneColectivasParaFiltros(Int64 idColectivaPadre, String claveColectiva, 
            IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneColectivasParaFiltros");

                database.AddInParameter(command, "@IdColectivaPadre", DbType.Int64, idColectivaPadre);
                database.AddInParameter(command, "@ClaveColectiva", DbType.String, claveColectiva);                
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
        /// Obtiene el reporte de detalle de promociones con los filtros indicados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="idTipoEmision">Identificador del tipo de emisión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteCuponesRedimidos(DateTime FechaInicial, DateTime FechaFinal, Int64 idCadena,
            Int64 idSucursal, int idPromocion, int idTipoEmision,  IUsuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneReporteDetallePromociones()", "");

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_CuponesRedimidos");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@IdPromocion", DbType.Int32, idPromocion);
                database.AddInParameter(command, "@IdTipoEmision", DbType.Int32, idTipoEmision);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, idCadena);                
                database.AddInParameter(command, "@IdSucursal", DbType.Int64, idSucursal);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneReporteDetallePromociones()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta a base de datos el reporte de cupones entregados con los filtros marcados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="idTipoEmision">Identificador del tipo de emisión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteCuponesEntregados(DateTime FechaInicial, DateTime FechaFinal, int idCadena,
            int idPromocion, int idTipoEmision, IUsuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneReporteCuponesEntregados()", "");

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_CuponesEntregados");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);
                database.AddInParameter(command, "@IdPromocion", DbType.Int32, idPromocion);
                database.AddInParameter(command, "@IdTipoEmision", DbType.Int32, idTipoEmision);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneReporteCuponesEntregados()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de promociones de cupones de la cadena indicada
        /// que se encuentran en el Autorizador
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtienePromocionesCuponesDeCadena(Int64 idCadena, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtienePromocionesCupones");

                database.AddInParameter(command, "@IdCadena", DbType.Int64, idCadena);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los usuarios activos de la cadena indicada en base de datos
        /// </summary>
        /// <param name="claveCadena">Clave de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos obtenidos</returns>
        public static DataSet ObtieneUsuariosActivosCadena(string claveCadena, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_UsuariosActivosCadena");

                database.AddInParameter(command, "@ClaveCadena", DbType.String, claveCadena);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Realiza la inserción de los registros a la tabla temporal de sucursales en base de datos
        /// </summary>
        /// <param name="dtSuc">DataTable con los registros de las sucursales</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaSucursalesAltaPromocion(DataTable dtSuc, SqlConnection connection, 
            SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaSucursalesAltaPromocion", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@sucursales", dtSuc));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta o actualiza en el Autorizador las colectivas relacionadas con la cadena
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena en e-Commerce</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void InsertaOActualizaColectivasCadena(int idCadena, SqlConnection connection, SqlTransaction transaccionSQL,
            IUsuario elUser)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaOModificaColectivasEcommerce", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdCadena", idCadena));

                command.ExecuteNonQuery();
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("InsertaColectivasCadena()", Ex);
            }
        }

        /// <summary>
        /// Inserta en el Autorizador la promoción de la cadena
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena en e-Commerce</param>
        /// <param name="idPromo">Identificador de la promoción en e-Commerce</param>
        /// <param name="programa">Nombre del programa al que se activa la promoción</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void InsertaPromocionEcommerce(int idCadena, int idPromo, string programa, SqlConnection connection,
            SqlTransaction transaccionSQL, IUsuario elUser)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaPromocionEcommerce", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;
                command.CommandTimeout = 0;

                command.Parameters.Add(new SqlParameter("@IdCadena", idCadena));
                command.Parameters.Add(new SqlParameter("@IdPromocion", idPromo));
                command.Parameters.Add(new SqlParameter("@Programa", programa));

                command.ExecuteNonQuery();
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("InsertaPromocionSams()", Ex);
            }
        }

        /// <summary>
        /// Inserta en la tabla layout de operadores e-Commerce del Autorizador el objeto recibido por archivo
        /// </summary>
        /// <param name="dtFileTmp">Tabla de datos qe contiene el archivo cargado</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaOperadoresTMP(DataTable dtFileTmp, SqlConnection connection, SqlTransaction transaccionSQL,
            IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaOperadoresLayoutEcommerce", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@operadores", dtFileTmp));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta los operadores como colectivas en el Autorizador
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaOperadoresEnAutorizador(SqlConnection connection, SqlTransaction transaccionSQL,
            IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_CreaOperadoresDeLayout", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta los operadores como colectivas en el Autorizador
        /// </summary>
        /// <param name="claveCadena">Clave de la cadena</param>
        /// <param name="claveSucursal">Clave de la sucursal</param>
        /// <param name="email">Correo electrónico del operador</param>
        /// <param name="esGerente">Bandera que indica si el operador será o no gerente</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaOperadorEnAutorizador(String claveCadena, String claveSucursal, String email, 
            int esGerente, SqlConnection connection, SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_CreaOperadorEcommerce", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ClaveCadena", claveCadena));
                command.Parameters.Add(new SqlParameter("@ClaveSucursal", claveSucursal));
                command.Parameters.Add(new SqlParameter("@Email", email));
                command.Parameters.Add(new SqlParameter("@EsGerente", esGerente));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Elimina al operador en el Autorizador
        /// </summary>
        /// <param name="idOper">Identificador del operador</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void EliminaOperadorEnAutorizador(Int64 idOper, SqlConnection connection, SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_EliminaOperadorEcommerce", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdOperador", idOper));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el rol del usuario en base de datos
        /// </summary>
        /// <param name="email">Correo electrónico (clave) del usuario</param>
        /// <param name="esGerente">Bandera que indica si el operador será o no gerente</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaRolUsuario(String email, int esGerente, SqlConnection connection, 
            SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaRolUsuario", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@EmailUsuario", email));
                command.Parameters.Add(new SqlParameter("@Gerente", esGerente));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los eventos que coinciden con la clave del evento en el Autorizador
        /// </summary>
        /// <param name="ClaveEvento">Clave del evento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del catálogo</returns>
        public static DataTable ListaEventosGenerarCodigos(String ClaveEvento, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosGeneraCodigos");

                database.AddInParameter(command, "@ClaveEvento", DbType.String, ClaveEvento);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Solicita la generación de cupones para una promoción masiva al Autorizador
        /// </summary>
        /// <param name="unCupon">Entidad cupón con la configuración de los cupones por generar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>ID de la colectiva (lote) recién creado</returns>
        public static Int64 GeneraCuponesPromoMasiva(Cupon unCupon, SqlConnection connection, 
            SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_Promocion_GenerarCupones_PromocionesMasivo", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;
                command.CommandTimeout = 0;

                command.Parameters.Add(new SqlParameter("@ClaveEvento", unCupon.ClaveEvento));
                command.Parameters.Add(new SqlParameter("@CantidadCupones", unCupon.CantidadCupones));
                command.Parameters.Add(new SqlParameter("@Promocode", unCupon.PromoCode));
                command.Parameters.Add(new SqlParameter("@TipoEmision", unCupon.TipoEmision));
                command.Parameters.Add(new SqlParameter("@Algoritmo", unCupon.Algoritmo));
                command.Parameters.Add(new SqlParameter("@Longitud", unCupon.Longitud));
                command.Parameters.Add(new SqlParameter("@FechaExpiracion", unCupon.FechaExpiracion));
                command.Parameters.Add(new SqlParameter("@ValorCupon", unCupon.ValorCupon));
                command.Parameters.Add(new SqlParameter("@ConsumosValidos", unCupon.ConsumosValidos));
                command.Parameters.Add(new SqlParameter("@ClaveCadenaComercial", unCupon.ClaveCadenaComercial));
                command.Parameters.Add(new SqlParameter("@TipoCupon", unCupon.TipoCupon));

                var sqlParameter1 = new SqlParameter("@ID_Colectiva", SqlDbType.BigInt);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);
                
                command.ExecuteNonQuery();

                return Convert.ToInt64(sqlParameter1.Value);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los códigos recién generados en el Autorizador, que "se entregaron" a la colectiva
        /// con el identificador proporcionado (promocón entrega masiva)
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los códigos</returns>
        public static DataTable ConsultaCodigosGenerados(String ClaveEvento, Int64 IdColectiva, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuponesPromocionesMasivo");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@ClaveEvento", DbType.String, ClaveEvento);
                database.AddInParameter(command, "@IdColectivaPromoCode", DbType.Int64, IdColectiva);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        /// <summary>
        /// Obtiene la lista de promociones que corresponden con el ID de cadena y 
        /// que tienen cupones en base de datos
        /// </summary>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaPromocionesConCupones(int idCadena, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommercePrana.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromocionesConCuponesDeCadena");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCadena);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta a base de datos el reporte de cupones entregados con los filtros marcados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="idTipoEmision">Identificador del tipo de emisión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteReferidosSams(DateTime FechaInicial, DateTime FechaFinal, string idCadena,
            IUsuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneReporteReferidosSams()", "");

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_CA_ReporteReferidosCanjeados");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@Cadena", DbType.String, idCadena);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneReporteReferidosSams()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        /// <summary>
        /// Consulta a base de datos el reporte de referidos ventas SAMS con los filtros marcados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idCadena">Identificador de la cadena</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <param name="idPromocion">Identificador de la promoción</param>
        /// <param name="idTipoEmision">Identificador del tipo de emisión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteReferidosVentasSams(DateTime FechaInicial, DateTime FechaFinal, string idCadena,
            IUsuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneReporteReferidosVentasSams()", "");

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_CA_ReporteReferidosVentas");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@Cadena", DbType.String, idCadena);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneReporteReferidosVentasSams()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
