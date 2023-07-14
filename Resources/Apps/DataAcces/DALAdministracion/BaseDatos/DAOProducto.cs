using DALAdministracion.Entidades;
using DALAdministracion.Utilidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Clase de acceso a datos para todo lo relacionado con Producto
    /// </summary>
    public class DAOProducto
    {
        /// <summary>
        /// Consulta el catálogo de tipos de producto en el Autorizador, permitidos para el
        /// usuario y la aplicación
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposProducto(int idColectiva, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTipoProductos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                database.AddInParameter(command, "@ID_Colectiva", DbType.Int32, idColectiva);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTipoProductos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                parametros.Add("P3", "@ID_Colectiva=" + idColectiva);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de producto en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de grupos de cuenta en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ListaGruposCuenta(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGruposCuentas");

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
        /// Obtiene el listado de grupos de cuenta en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ListaGruposCuenta(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGruposCuentas");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneGruposCuentas";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los grupos de cuentas en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de integración dentro del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaTiposIntegracion(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposIntegracion");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneTiposIntegracion";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de integración en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de grupos de medios de acceso en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaGruposMediosAcceso(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGruposMA");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg _logDBG = new LogDebugMsg();
                _logDBG.M_Value = "web_CA_ObtieneGruposMA";
                _logDBG.C_Value = "";
                _logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                _logDBG.Parameters = parametros;

                unLog.Debug(_logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los grupos de MA de base de datos.");
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de parámegtros multiasignación dentro del Autorizador
        /// </summary>
        /// <param name="esSubProd">Bandera de control de origen (parweb_CA_ActualizaProductoa producto o subproducto)</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaTiposParametrosMA(bool esSubProd, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposParametrosMultiasignacion");

                database.AddInParameter(command, "@EsSubproducto", DbType.Boolean, esSubProd);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTiposParametrosMultiasignacion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@EsSubproducto=" + esSubProd);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de parámetros en base de datos");
            }
        }

        /// <summary>
        /// Consulta los tipos de medios de acceso, relacionados con el tipo de producto, en el Autorizador
        /// </summary>
        /// <param name="idTipoProd">Identificador del tipo de producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista con las claves de tipos de medios de acceso</returns>
        public static List<string> ListaTiposMAPorTipoProducto(int idTipoProd, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            List<string> tiposMA = new List<string>();
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneTiposMAPorTipoProducto";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@IdTipoProducto", SqlDbType.Int);
                        param.Value = idTipoProd;
                        cmd.Parameters.Add(param);

                        SqlParameter param2 = new SqlParameter("@UserTemp", SqlDbType.UniqueIdentifier);
                        param2.Value = elUsuario.UsuarioTemp;
                        cmd.Parameters.Add(param2);

                        SqlParameter param3 = new SqlParameter("@AppId", SqlDbType.UniqueIdentifier);
                        param3.Value = AppID;
                        cmd.Parameters.Add(param3);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    tiposMA.Add(SqlReader["ClaveMA"].ToString().Trim());
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneTiposMAPorTipoProducto";
                                logDBG.C_Value = "";
                                logDBG.R_Value = string.Join("|", tiposMA);

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdTipoProducto=" + idTipoProd);
                                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                                parametros.Add("P3", "@AppId=" + AppID);
                                logDBG.Parameters = parametros;

                                logPCI.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            logPCI.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los tipos MA de base de datos.");
                        }
                    }
                }

                return tiposMA;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los tipos MA por tipo de producto en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene los productos padre que corresponden al ID de tipo de producto en el Autorizador
        /// </summary>
        /// <param name="IdTipoProducto">Identificador del tipo de producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con la lista de productos</returns>
        public static DataTable ObtieneProductosPadrePorTipo(int IdTipoProducto, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProductosTitularesPorTipo");

                database.AddInParameter(command, "@IdTipoProducto", DbType.Int32, IdTipoProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneProductosTitularesPorTipo";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoProducto=" + IdTipoProducto);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los productos padre de base de datos");
            }
        }

        /// <summary>
        /// Obtiene los tipos de medios de acceso externos que se encuentran en el catálogo del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los tipos de medios de acceso externos</returns>
        public static DataTable ObtieneTiposMAExternos(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposMAExternos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTiposMAExternos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos MA externos de base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros de contrato necesarios para el alta de timpos de medios de acceso
        /// CLABE, dentro del Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de a colectiva dueña del contrato</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los datos de los parámetros</returns>
        public static DataSet ObtieneParametrosContratoCLABE(Int64 IdColectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosCLABE");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, IdColectiva);
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
        /// Actualiza el valor del parámetro de contrato de la colectiva en el Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva dueña del contrato</param>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaOActualizaValorParametroContrato(long idColectiva, ParametroValor elParametro,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaActualizaContratoValorFijo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                        command.Parameters.Add(new SqlParameter("@Nombre", elParametro.Nombre));
                        command.Parameters.Add(new SqlParameter("@NuevoValor", elParametro.Valor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaActualizaContratoValorFijo";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdColectiva=" + idColectiva);
                        parametros.Add("P2", "@Nombre=" + elParametro.Nombre);
                        parametros.Add("P3", "@NuevoValor=" + elParametro.Valor);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDBG.Parameters = parametros;

                        unLog.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el valor del parámetro en base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo producto en base de datos
        /// </summary>
        /// <param name="unProducto">Datos del nuevo producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaProducto(DALAutorizador.Entidades.Producto unProducto, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                string IDNuevoProducto, Mensaje;

                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaNuevoProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@ClaveProducto", unProducto.Clave));
                        command.Parameters.Add(new SqlParameter("@Descripcion", unProducto.Descripcion));
                        command.Parameters.Add(new SqlParameter("@IdTipoProducto", unProducto.ID_TipoProducto));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", unProducto.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdProductoPadre",
                            unProducto.ID_ProductoPadre == -1 ? (int?)null : unProducto.ID_ProductoPadre));
                        command.Parameters.Add(new SqlParameter("@IdTipoIntegracion", unProducto.ID_TipoIntegracion));
                        command.Parameters.Add(new SqlParameter("@IdDisTarjetasFisicas",
                            unProducto.ID_CatDisTarjFisicas == -1 ? (int?)null : unProducto.ID_CatDisTarjFisicas));
                        command.Parameters.Add(new SqlParameter("@GruposMA", unProducto.GruposMA));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        command.Parameters.Add(new SqlParameter("@ClaveTipoMA", unProducto.ClaveTipoMAExterno));
                        command.Parameters.Add(new SqlParameter("@DescTipoMA", unProducto.DescripcionTipoMAExterno));
                        command.Parameters.Add(new SqlParameter("@GeneraTipoMACLABE", unProducto.GeneraTipoMACLABE));

                        var sqlParameter1 = new SqlParameter("@IdNuevoProducto", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        IDNuevoProducto = sqlParameter1.Value.ToString();
                        Mensaje = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaNuevoProducto";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@IdNuevoProducto=" + IDNuevoProducto +
                            "|@Mensaje=" + Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@ClaveProducto=" + unProducto.Clave);
                        parametros.Add("P2", "@Descripcion=" + unProducto.Descripcion);
                        parametros.Add("P3", "@IdTipoProducto=" + unProducto.ID_TipoProducto);
                        parametros.Add("P4", "@IdColectiva=" + unProducto.ID_Colectiva);
                        parametros.Add("P5", "@IdProductoPadre=" + unProducto.ID_ProductoPadre);
                        parametros.Add("P6", "@IdTipoIntegracion=" + unProducto.ID_TipoIntegracion);
                        parametros.Add("P7", "@IdDisTarjetasFisicas=" + unProducto.ID_CatDisTarjFisicas);
                        parametros.Add("P8", "@GruposMA=" + unProducto.GruposMA);
                        parametros.Add("P9", "@Usuario=" + elUsuario.ClaveColectiva);
                        parametros.Add("P10", "@ClaveTipoMA=" + unProducto.ClaveTipoMAExterno);
                        parametros.Add("P11", "@DescTipoMA=" + unProducto.DescripcionTipoMAExterno);
                        parametros.Add("P12", "@GeneraTipoMACLABE=" + unProducto.GeneraTipoMACLABE);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevoProducto");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevoProducto"] = IDNuevoProducto;
                        dt.Rows[0]["Mensaje"] = Mensaje;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el producto en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los productos que corresponden al ID de colectiva, clave o descripción
        /// del producto en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="ClaveDescProd">Clave o descripción del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ObtieneProductosPorClaveDescOColectiva(int IdColectiva, String ClaveDescProd,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProductosPorFiltro");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@ClaveProducto", DbType.String, ClaveDescProd);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneProductosPorFiltro";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@ClaveProducto=" + ClaveDescProd);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los productos en base de datos");
            }
        }

        /// <summary>
        /// Consulta la información adicional del producto con el ID indicado en base de datos
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Objeto Producto con los datos obtenidos</returns>
        public static DALAutorizador.Entidades.Producto ObtieneInfoAdicionalProducto(int idProducto, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            DALAutorizador.Entidades.Producto prod = new DALAutorizador.Entidades.Producto();
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneInfoProducto";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdProducto", SqlDbType.Int);
                        param.Value = idProducto;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();
                        
                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    prod.Clave = SqlReader["Clave"].ToString();
                                    prod.Descripcion = SqlReader["Descripcion"].ToString();
                                    prod.ID_TipoProducto = (int)SqlReader["ID_TipoProducto"];
                                    prod.ID_Colectiva = (int)SqlReader["ID_Colectiva"];
                                    prod.ID_TipoIntegracion = (int)SqlReader["ID_TipoIntegracion"];
                                    prod.GruposMA = SqlReader["GruposMA"].ToString();
                                    prod.ID_ProductoPadre =
                                        SqlReader["ID_ProductoPadre"] == DBNull.Value ? (int?)null : (int)SqlReader["ID_ProductoPadre"];
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneInfoProducto";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdProducto=" + idProducto);
                                logDBG.Parameters = parametros;

                                pCI.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            pCI.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los datos del producto en base de datos.");
                        }
                    }
                }

                return prod;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los datos del producto en de base de datos.");
            }
        }

        /// <summary>
        /// Obtiene los tipos de cuenta relacionados con el producto dentro del Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ObtieneTiposCuentaProducto(int idProducto, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposCuentaProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTiposCuentaProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                pCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de cuenta del producto en base de datos");
            }
        }

        /// <summary>
        /// Actualiza los datos de un producto en el Autorizador
        /// </summary>
        /// <param name="producto">Entidad con los datos a modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string ActualizaProducto(DALAutorizador.Entidades.Producto producto, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", producto.ID_Producto));
                        command.Parameters.Add(new SqlParameter("@Descripcion", producto.Descripcion));
                        command.Parameters.Add(new SqlParameter("@IdTipoProducto", producto.ID_TipoProducto));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", producto.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdProductoPadre",
                            producto.ID_ProductoPadre == -1 ? (int?)null : producto.ID_ProductoPadre));
                        command.Parameters.Add(new SqlParameter("@IdTipoIntegracion", producto.ID_TipoIntegracion));
                        command.Parameters.Add(new SqlParameter("@GruposMA", producto.GruposMA));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter = new SqlParameter("@MensajeResp", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string resp = sqlParameter.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaProducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + producto.ID_Producto);
                        parametros.Add("P2", "@Descripcion=" + producto.Descripcion);
                        parametros.Add("P3", "@IdTipoProducto=" + producto.ID_TipoProducto);
                        parametros.Add("P4", "@IdColectiva=" + producto.ID_Colectiva);
                        parametros.Add("P5", "@IdProductoPadre=" + producto.ID_ProductoPadre);
                        parametros.Add("P6", "@IdTipoIntegracion=" + producto.ID_TipoIntegracion);
                        parametros.Add("P7", "@GruposMA=" + producto.GruposMA);
                        parametros.Add("P8", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el producto en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los eventos asociados al ID de producto en el Autorizador
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de eventos</returns>
        public static DataSet ObtieneEventosDeProducto(int IdProducto, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventosProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, IdProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneEventosProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + IdProducto);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los eventos del producto en base de datos");
            }
        }

        /// <summary>
        /// Consulta todos los datos del evento con el ID indicado en base de datos
        /// </summary>
        /// <param name="idEvento">Identificador del evento</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneDatosEvento(int idEvento, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosEvento");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, idEvento);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDatosEvento";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdEvento=" + idEvento);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los datos del evento en base de datos");
            }
        }

        /// <summary>
        /// Consulta todos los datos de los scripts asociados al evento con el ID indicado en base de datos
        /// </summary>
        /// <param name="idEvento">Identificador del evento</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneDatosScriptsEvento(int idEvento, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDetalleScriptsEvento");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, idEvento);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDetalleScriptsEvento";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdEvento=" + idEvento);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los scripts del evento en base de datos");
            }
        }

        /// <summary>
        /// Consulta los bines asociados al producto con el ID indicado en base de datos
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneBinesDeProducto(int idProducto, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneBinesProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneBinesProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los bines del producto en base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo BIN y lo asocia al producto en base de datos
        /// </summary>
        /// <param name="idProd">Identificador del producto</param>
        /// <param name="claveBIN">Nueva clave del BIN</param>
        /// <param name="descBIN">Nueva descripción del BIN</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaBIN(int idProd, String claveBIN, String descBIN, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                string IDNuevoBIN, Mensaje;

                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaNuevoBinAProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", idProd));
                        command.Parameters.Add(new SqlParameter("@ClaveBin", claveBIN));
                        command.Parameters.Add(new SqlParameter("@DescripcionBin", descBIN));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@IdNuevoBIN", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        IDNuevoBIN = sqlParameter1.Value.ToString();
                        Mensaje = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaNuevoBinAProducto";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@IdNuevoBIN=" + IDNuevoBIN +
                            "|@Mensaje=" + Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + idProd);
                        parametros.Add("P2", "@ClaveBin=" + claveBIN);
                        parametros.Add("P3", "@DescripcionBin=" + descBIN);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevoBin");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevoBin"] = IDNuevoBIN;
                        dt.Rows[0]["Mensaje"] = Mensaje;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el BIN en base de datos");
            }
        }

        /// <summary>
        /// Actualiza los datos de un BIN en el Autorizador
        /// </summary>
        /// <param name="idBIN">Identificador del BIN a modificar</param>
        /// <param name="claveBIN">Nueva clave del BIN</param>
        /// <param name="descBIN">Nueva descripción del BIN</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string ActualizaBINProducto(int idBIN, String claveBIN, String descBIN,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaBINProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdBin", idBIN));
                        command.Parameters.Add(new SqlParameter("@ClaveBin", claveBIN));
                        command.Parameters.Add(new SqlParameter("@DescripcionBin", descBIN));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter = new SqlParameter("@MensajeResp", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string msjResp = sqlParameter.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaBINProducto";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@MensajeResp=" + msjResp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdBin=" + idBIN);
                        parametros.Add("P2", "@ClaveBin=" + claveBIN);
                        parametros.Add("P3", "@DescripcionBin=" + descBIN);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return msjResp;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el BIN en base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo subproducto y lo asocia al producto en base de datos
        /// </summary>
        /// <param name="subProd">Objeto con los datos del nuevo subproducto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaSubproducto(Plantilla subProd, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaNuevoSubproductoAProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", subProd.ID_Producto));
                        command.Parameters.Add(new SqlParameter("@Clave", subProd.Clave));
                        command.Parameters.Add(new SqlParameter("@Descripcion", subProd.Descripcion));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@IdNuevoSubProd", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string IDNuevoSubp = sqlParameter1.Value.ToString();
                        string Mensaje = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaNuevoSubproductoAProducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = "@IdNuevoSubProd=" + IDNuevoSubp +
                            "|@Mensaje=" + Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + subProd.ID_Producto);
                        parametros.Add("P2", "@Clave=" + subProd.Clave);
                        parametros.Add("P3", "@Descripcion=" + subProd.Descripcion);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevoSubproducto");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevoSubproducto"] = IDNuevoSubp;
                        dt.Rows[0]["Mensaje"] = Mensaje;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el subproducto en base de datos");
            }
        }

        /// <summary>
        /// Consulta los subproductos asociados al producto con el ID indicado en base de datos
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneSubproductosDeProducto(int idProducto, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSubproductosProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneSubproductosProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los subproductos en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los productos asociados a la colectiva dentro del Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ObtieneProductosDeColectiva(int idColectiva, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProductosDeColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneProductosDeColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los productos en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los productos que corresponden al ID de colectiva, clave o descripción
        /// del producto en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ObtieneSubproductosPorColectivaProducto(int IdColectiva, int IdProducto,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSubProductosPorFiltro");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@IdProducto", DbType.Int32, IdProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneSubProductosPorFiltro";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@IdProducto=" + IdProducto);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los subproductos de base de datos");
            }
        }

        /// <summary>
        /// Actualiza los datos de un subproducto en el Autorizador
        /// </summary>
        /// <param name="subproducto">Entidad con los datos a modificar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string ActualizaSubproducto(Plantilla subproducto, Usuario elUsuario, ILogHeader elLog, string pantalla = "")
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaSubproducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //command.CommandTimeout = 0;

                        command.Parameters.Add(new SqlParameter("@IdSubProducto", subproducto.ID_Plantilla));
                        command.Parameters.Add(new SqlParameter("@Clave", subproducto.Clave));
                        command.Parameters.Add(new SqlParameter("@Descripcion", subproducto.Descripcion));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@PantallaOrigen", pantalla));

                        var sqlParameter = new SqlParameter("@MensajeResp", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string resp = sqlParameter.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaSubproducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdSubProducto=" + subproducto.ID_Plantilla);
                        parametros.Add("P2", "@Clave=" + subproducto.Clave);
                        parametros.Add("P3", "@Descripcion=" + subproducto.Descripcion);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        parametros.Add("P5", "@PantallaOrigen=" + pantalla);

                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el subproducto en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación asignados al subproducto, y que 
        /// pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idSubproducto">Identificador del subproducto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ObtieneParametrosMASubproducto(int idTipoPMA, int idSubproducto, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMASubProducto");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdSubProducto", DbType.Int32, idSubproducto);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosMASubProducto";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdSubProducto=" + idSubproducto);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros del subproducto");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación sin asignar al subproducto, y que 
        /// pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idSubproducto">Identificador del subproducto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con lor parámetros</returns>
        public static DataSet ObtienePMAPorTipoSinAsignarSubProd(int idTipoPMA, int idSubproducto, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMASinAsignarSubProducto");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdSubProducto", DbType.Int32, idSubproducto);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtienePMASinAsignarSubProducto";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdSubProducto=" + idSubproducto);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros sin asignar del subproducto");
            }
        }

        /// <summary>
        /// Inserta el valor de parámetro multiasignación al subproducto en el Autorizador
        /// </summary>
        /// <param name="idPMA">Identificador del parámetro multiasignación</param>
        /// <param name="idSubproducto">Identificador del subproducto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaPMASubproducto(int idPMA, int idSubproducto, Usuario elUsuario, ILogHeader elLog, string pantalla = "")
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaValorPMASubproducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdParametroMultiasignacion", idPMA));
                        command.Parameters.Add(new SqlParameter("@IdSubProducto", idSubproducto));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@PantallaOrigen", pantalla));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaValorPMASubproducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdParametroMultiasignacion=" + idPMA);
                        parametros.Add("P2", "@IdSubProducto=" + idSubproducto);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        parametros.Add("P4", "@PantallaOrigen=" + pantalla);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el valor del parámetro en base de datos");
            }
        }

        /// <summary>
        /// Obtiene las tarjetas asignadas al subproducto en el Autorizador
        /// </summary>
        /// <param name="idSubproducto">Identificador del subproducto</param>
        /// <param name="binTarjeta">Clave o BIN de tarjeta a filtrar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataTable ObtieneTarjetasSubproducto(long idSubproducto, string binTarjeta, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjetasSubProducto");

                database.AddInParameter(command, "@IdSubProducto", DbType.Int32, idSubproducto);
                database.AddInParameter(command, "@BinTarjeta", DbType.String, binTarjeta);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjetasSubProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdSubProducto=" + idSubproducto);
                parametros.Add("P2", "@BinTarjeta=" + binTarjeta.Substring(0, 4) + "****");
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las tarjetas asociadas al subproducto en base de datos");
            }
        }

        /// <summary>
        /// Obtiene las pertenencias manuales en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de pertenencias</returns>
        public static DataTable ObtienePertenenciasManuales(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePertenenciasManuales");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtienePertenenciasManuales";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las pertenencias manuales de base de datos");
            }
        }

        /// <summary>
        /// Consulta el valor del IVA configurado en los parámetros del Autorizador
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Cadena con el valor del IVA</returns>
        public static string ObtieneValorIVA(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ObtieneValorPMAIVA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        string resp = command.ExecuteScalar().ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDb = new LogDebugMsg();
                        logDb.M_Value = "web_CA_ObtieneValorPMAIVA";
                        logDb.C_Value = "";
                        logDb.R_Value = resp;

                        log.Debug(logDb);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el IVA de base de datos");
            }
        }

        /// <summary>
        /// Obtiene los eventos relacionados con la pertenencia en el Autorizador
        /// </summary>
        /// <param name="IdPertetencia">Identificador de la pertenencia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con la lista de pertenencias</returns>
        public static List<string> ListaEventosDePertenenciaManual(Int64 IdPertetencia, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            List<string> losEventos = new List<string>();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneEventosDePertenenciaManual";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@IdPertenencia", SqlDbType.BigInt);
                        param.Value = IdPertetencia;
                        cmd.Parameters.Add(param);

                        SqlParameter param2 = new SqlParameter("@UserTemp", SqlDbType.UniqueIdentifier);
                        param2.Value = elUsuario.UsuarioTemp;
                        cmd.Parameters.Add(param2);

                        SqlParameter param3 = new SqlParameter("@AppId", SqlDbType.UniqueIdentifier);
                        param3.Value = AppID;
                        cmd.Parameters.Add(param3);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    losEventos.Add(SqlReader["ClaveEvento"].ToString().Trim());
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDebug = new LogDebugMsg();
                                logDebug.M_Value = "web_CA_ObtieneEventosDePertenenciaManual";
                                logDebug.C_Value = "";
                                logDebug.R_Value = string.Join("|", losEventos);

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdCuenta=" + IdPertetencia);
                                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                                parametros.Add("P3", "@AppId=" + AppID);
                                logDebug.Parameters = parametros;

                                log.Debug(logDebug);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            log.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los eventos de la pertenencia manual en base de datos.");
                        }
                    }
                }

                return losEventos;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los eventos de la pertenencia manual en base de datos.");
            }
        }

        /// <summary>
        /// Consulta el identificador del grupo de cuentas que corresponde a la clave de prodcto
        /// dentro del Autorizador
        /// </summary>
        /// <param name="claveProd">Clave de producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Identificador del gruppo de cuentas</returns>
        public static int ObtieneIdGrupoCuentas(string claveProd, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ObtieneIDGrupoCuentas", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add((new SqlParameter("@ClaveProducto", claveProd)));

                        conn.Open();

                        int resp = Convert.ToInt32(command.ExecuteScalar());

                        /////>>>LOG DEBUG
                        LogDebugMsg logDb = new LogDebugMsg();
                        logDb.M_Value = "web_CA_ObtieneIDGrupoCuentas";
                        logDb.C_Value = "";
                        logDb.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@ClaveProducto=" + claveProd);
                        logDb.Parameters = parametros;

                        logPCI.Debug(logDb);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el ID Grupo Cuentas de base de datos");
            }
        }

        /// <summary>
        /// Obtiene los subproductos que corresponden al ID de colectiva e ID de producto, con plantillas
        /// de tipo Preautorizador, permitidos para el usuario y la aplicación en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ObtieneSubproductosPreAutPorColectivaProducto(int IdColectiva, int IdProducto,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePlantillasPorFiltro");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@IdProducto", DbType.Int32, IdProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePlantillasPorFiltro";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@IdProducto=" + IdProducto);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los grupos de base de datos");
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de parámetros multiasignación preautorizador en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaTipoPMAPreaut(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTipoPreautPMA");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTipoPreautPMA";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de parámetro de base de datos");
            }
        }

        /// <summary>
        /// Inserta una nueva plantilla preautorizador de nivel 2 y la asocia al producto en base de datos
        /// </summary>
        /// <param name="plantilla">Objeto con los datos de la nueva plantilla</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaPlantillaPreautNivel2(Plantilla plantilla, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaPlantillaPreautNivel2", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", plantilla.ID_Producto));
                        command.Parameters.Add(new SqlParameter("@Clave", plantilla.Clave));
                        command.Parameters.Add(new SqlParameter("@Descripcion", plantilla.Descripcion));

                        var sqlParameter1 = new SqlParameter("@IdNuevoGrupo", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string IDNuevoGrupo = sqlParameter1.Value.ToString();
                        string Mensaje= sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaPlantillaPreautNivel2";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@IdNuevoGrupo=" + IDNuevoGrupo +
                            "|@Mensaje=" + Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + plantilla.ID_Producto);
                        parametros.Add("P2", "@Clave=" + plantilla.Clave);
                        parametros.Add("P3", "@Descripcion=" + plantilla.Descripcion);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevoGrupo");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevoGrupo"] = IDNuevoGrupo;
                        dt.Rows[0]["Mensaje"] = Mensaje;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la plantilla en base de datos");
            }
        }

        /// <summary>
        /// Obtiene las tarjetas asignadas a la plantilla de tipo preautorizador en el Autorizador
        /// </summary>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="tarjeta">Número de tarjeta a filtrar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataTable ObtieneTarjetasPlantillaPreaut(int idPlantilla, String tarjeta, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjetasPlantillaPreaut");

                database.AddInParameter(command, "@IdPlantilla", DbType.Int64, idPlantilla);
                
                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@NumTarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = tarjeta;
                paramTarj.Size = tarjeta.Length;
                command.Parameters.Add(paramTarj);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjetasPlantillaPreaut";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdPlantilla=" + idPlantilla);
                parametros.Add("P2", "@NumTarjeta=" + (string.IsNullOrEmpty(tarjeta) ? tarjeta :
                    tarjeta.Length < 16 ? "******" : MaskSensitiveData.cardNumber(tarjeta)));
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las tarjetas en base de datos");
            }
        }

        /// <summary>
        /// Obtiene las tarjetas candidatas a asignarse a la plantilla de tipo preautorizador en el Autorizador
        /// </summary>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="tarjeta">Número de tarjeta a filtrar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataTable ObtienePosiblesTarjetasPlantillaPreaut(int idPlantilla, String tarjeta, 
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjetasPorAsignarPlantillaPreaut");

                database.AddInParameter(command, "@IdPlantilla", DbType.Int64, idPlantilla);
                
                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@NumTarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = tarjeta;
                paramTarj.Size = tarjeta.Length;
                command.Parameters.Add(paramTarj);

                database.AddInParameter(command, "@BinTarjeta", DbType.String, tarjeta.Substring(0, 8));
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjetasPorAsignarPlantillaPreaut";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdPlantilla=" + idPlantilla);
                parametros.Add("P2", "@NumTarjeta=" + MaskSensitiveData.cardNumber(tarjeta));
                parametros.Add("P3", "@BinTarjeta=" + "**** ****");
                parametros.Add("P4", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P5", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las tarjetas en base de datos");
            }
        }

        /// <summary>
        /// Actualiza la plantilla preautorizador a las cuentas relacionadas con la tarjeta en base de datos
        /// </summary>
        /// <param name="idTarjeta">Identificador de la tarjeta</param>
        /// <param name="idPlantilla">Identificador de la plantilla a la que se actualizan las cuentas</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaPlantillaPreautTarjeta(int idTarjeta, Int64 idPlantilla, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaPlantillaPreautCuentas", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTarjeta", idTarjeta));
                        command.Parameters.Add(new SqlParameter("@IdPlantilla", idPlantilla));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaPlantillaPreautCuentas";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTarjeta=" + idTarjeta);
                        parametros.Add("P2", "@IdPlantilla=" + idPlantilla);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la plantilla de las cuentas en base de datos");
            }
        }

        /// <summary>
        /// Valida si el número de tarjeta existe en base de datos
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Mensaje de respuesta de la validación</returns>
        public static String ValidaExisteTarjeta(String tarjeta, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ValidaExistenciaTarjeta");

                database.AddInParameter(command, "@NumTarjeta", DbType.String, tarjeta);

                return database.ExecuteScalar(command).ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la validación en base de datos entre los productos de la tarjeta y la plantilla
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Mensaje de respuesta de la validación de productos en base de datos</returns>
        public static String ValidaProductosTarjetaPlantilla(String tarjeta, Int64 idPlantilla, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ValidaProductoTarjeta");

                database.AddInParameter(command, "@IdPlantilla", DbType.Int64, idPlantilla);
                database.AddInParameter(command, "@NumTarjeta", DbType.String, tarjeta);

                return database.ExecuteScalar(command).ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la validación en base de datos de tarjeta cancelada o no
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Mensaje de respuesta de la validación en base de datos</returns>
        public static String ValidaTarjetaNoCancelada(String tarjeta, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ValidaTarjetaNoCancelada");

                database.AddInParameter(command, "@NumTarjeta", DbType.String, tarjeta);

                return database.ExecuteScalar(command).ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtFileTmp"></param>
        /// <param name="connection"></param>
        /// <param name="transaccionSQL"></param>
        /// <param name="elUsuario"></param>
        public static void InsertaTarjetasTMP(DataTable dtFileTmp, SqlConnection connection, 
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaBeneficiosPrana", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@Beneficios", dtFileTmp));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene las tarjetas candidatas a asignarse a la plantilla de tipo preautorizador en el Autorizador
        /// </summary>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="dtTarjetas">Tabla con las tarjetas que se cargaron de forma manual al sistema</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataTable ObtienePlantillasTarjetasPreaut(long idPlantilla, DataTable dtTarjetas,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePlantillasTarjetasPreaut");

                database.AddInParameter(command, "@IdPlantilla", DbType.Int64, idPlantilla);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                command.Parameters.Add(new SqlParameter("@tarjetas", dtTarjetas));

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePlantillasTarjetasPreaut";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdPlantilla=" + idPlantilla);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                parametros.Add("P4", "@tarjetas=*********");
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las plantillas de las tarjetas en base de datos");
            }
        }

        /// <summary>
        /// Actualiza la plantilla preautorizador a las cuentas relacionadas con las tarjetas en base de datos
        /// </summary>
        /// <param name="idPlantilla">Identificador de la plantilla a la que se actualizan las cuentas</param>
        /// <param name="lasTarjetas">Tabla con los números de tarjet</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaPlantillaPreautTarjetas(Int64 idPlantilla, DataTable lasTarjetas, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaPlantillaPreautCuentasMasivo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPlantilla", idPlantilla));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@Tarjetas", lasTarjetas));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaPlantillaPreautCuentasMasivo";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPlantilla=" + idPlantilla);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveColectiva);
                        parametros.Add("P3", "@Tarjetas=****************");
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la plantilla de las cuentas en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los elementos de un catálogo de valores prestablecidos de parámetros multiasignación, 
        /// para la colectiva e identificador de parámetro, en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdParametroMA">Identificador del parámetro mutltiasignación</param>
        /// <param name="ClaveParametroMA">Clave del parámetro mutltiasignación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los elementos del catálogo</returns>
        public static DataSet ListaElementosCatalogoPMA(long IdColectiva, long IdParametroMA, string ClaveParametroMA,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoPMA");

                database.AddInParameter(command, "@IdParametroMA", DbType.Int64, IdParametroMA);
                database.AddInParameter(command, "@IdColectiva", DbType.Int64, IdColectiva);
                database.AddInParameter(command, "@ClavePMA", DbType.String, string.IsNullOrEmpty(ClaveParametroMA) ?
                    null : ClaveParametroMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCatalogoPMA";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdParametroMA=" + IdParametroMA.ToString());
                parametros.Add("P2", "@IdColectiva=" + IdColectiva.ToString());
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el catálogo de valores del parámetro en base de datos.");
            }
        }

        /// <summary>
        /// Consulta las campañas asociadas al producto con el ID indicado en base de datos
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneCampanyasDeProducto(int idProducto, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCampanyasProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, idProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneCampanyasProducto";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las campañas en base de datos");
            }
        }

        /// <summary>
        /// Inserta o actualiza una campaña de producto en el Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="campanya">Entidad con los datos a modificar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string InsertaOActualizaCampanya(int idProducto, CampanyaMSI campanya, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaActualizaCampanyaProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                     
                        command.Parameters.Add(new SqlParameter("@IdCampanya", campanya.ID_Campanya ));
                        command.Parameters.Add(new SqlParameter("@IdProducto", idProducto));
                        command.Parameters.Add(new SqlParameter("@Clave", campanya.Clave));
                        command.Parameters.Add(new SqlParameter("@Descripcion", campanya.Descripcion));
                        command.Parameters.Add(new SqlParameter("@FechaInicio", campanya.FechaInicio));
                        command.Parameters.Add(new SqlParameter("@FechaFin", campanya.FechaFin));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter = new SqlParameter("@MensajeResp", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string resp = sqlParameter.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaActualizaCampanyaProducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCampanya=" + campanya.ID_Campanya.ToString());
                        parametros.Add("P2", "@IdProducto=" + idProducto.ToString());
                        parametros.Add("P3", "@Clave=" + campanya.Clave);
                        parametros.Add("P4", "@Descripcion=" + campanya.Descripcion); 
                        parametros.Add("P5", "@FechaInicio=" + campanya.FechaInicio.ToShortDateString());
                        parametros.Add("P6", "@FechaFin=" + campanya.FechaFin.ToShortDateString());
                        parametros.Add("P7", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la campaña en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el estatus de una campaña de producto en el Autorizador
        /// </summary>
        /// <param name="idCampanya">Identificador de la campaña por actualizar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusCampanya(int idCampanya, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusCampanyaProducto", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCampanya", idCampanya));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaEstatusCampanyaProducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCampanya=" + idCampanya.ToString());
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus de la campaña en base de datos");
            }
        }

        /// <summary>
        /// Consulta las promociones asociadas a la campaña con el ID indicado en base de datos
        /// </summary>
        /// <param name="idCampanya">Identificador de la campaña</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtienePromocionesDeCampanya(int idCampanya, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromocionesCampanya");

                database.AddInParameter(command, "@IdCampanya", DbType.Int32, idCampanya);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePromocionesCampanya";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdCampanya=" + idCampanya.ToString());
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las promociones de la campanya en base de datos.");
            }
        }

        /// <summary>
        /// Inserta o actualiza una configuración de campaña en el Autorizador
        /// </summary>
        /// <param name="idCampanya">Identificador de la campaña</param>
        /// <param name="promocion">Entidad con los datos a modificar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string InsertaOActualizaCampaniaConfiguracion(int idCampanya, PromocionMSI promocion, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaActualizaPromocionCampanya", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCampaniaConfiguracion", promocion.ID_CampanyaPromocion));
                        command.Parameters.Add(new SqlParameter("@IdCampanya", idCampanya));
                        command.Parameters.Add(new SqlParameter("@IdPromocion", promocion.ID_Promocion));
                        command.Parameters.Add(new SqlParameter("@Meses", promocion.Meses));
                        command.Parameters.Add(new SqlParameter("@TasaInteres", promocion.TasaInteres));
                        command.Parameters.Add(new SqlParameter("@Diferimiento", promocion.Diferimiento));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter = new SqlParameter("@MensajeResp", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string resp = sqlParameter.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaActualizaPromocionCampanya";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCampaniaConfiguracion=" + promocion.ID_CampanyaPromocion.ToString());
                        parametros.Add("P2", "@IdCampanya=" + idCampanya.ToString());
                        parametros.Add("P3", "@IdPromocion=" + promocion.ID_Promocion.ToString());
                        parametros.Add("P4", "@Meses=" + promocion.Meses.ToString());
                        parametros.Add("P5", "@TasaInteres=" + promocion.TasaInteres.ToString());
                        parametros.Add("P6", "@Diferimiento=" + promocion.Diferimiento.ToString());
                        parametros.Add("P7", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al crear/actualizar la configuración de campaña en base de datos");
            }
        }

        /// <summary>
        /// Desactiva la configuración de una campaña de producto en el Autorizador
        /// </summary>
        /// <param name="IdCampanyaCfg">Identificador de la configuración de campaña por eliminar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusCampaniaConfiguracion(int IdCampanyaCfg, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_DesactivaCampaniaConfiguracion", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCampaniaConfiguracion", IdCampanyaCfg));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_DesactivaCampaniaConfiguracion";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCampaniaConfiguracion=" + IdCampanyaCfg.ToString());
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar la configuración de campaña en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de promociones en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaPromociones(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromociones");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg _logDBG = new LogDebugMsg();
                _logDBG.M_Value = "web_CA_ObtienePromociones";
                _logDBG.C_Value = "";
                _logDBG.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                _logDBG.Parameters = parametros;

                unLog.Debug(_logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las promociones de base de datos.");
            }
        }
    }
}
