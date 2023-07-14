using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALPuntoVentaWeb.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Administración de Colectivas
    /// </summary>
    public class DAOAdministrarColectivas
    {
        /// <summary>
        /// Obtiene la lista los estatus de las colectivas en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaEstatusColectivas(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusColectivas");

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
        /// Obtiene la lista los estatus de las colectivas en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaEstatusColectivas(Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusColectivas");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneEstatusColectivas";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus de colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene la lista de tipos de colectiva con sus correspondientes tipos de 
        /// colectiva padre en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaTiposColectivaPadre(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposColectivaPadre");

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
        /// Obtiene la lista de tipos de colectiva con sus correspondientes tipos de 
        /// colectiva padre en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaTiposColectivaPadre(Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposColectivaPadre");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneTiposColectivaPadre";
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
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de Colectiva Padre de base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el listado de tipos de cuenta en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con el catálogo de tipos de cuenta</returns>
        public static DataSet ListaTiposCuenta(Usuario elUsuario, Guid AppID)
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
        /// Obtiene el listado de tipos de cuenta en el Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador del usuario</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con el catálogo de tipos de cuenta</returns>
        public static DataSet ListaTiposCuenta(int idColectiva, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposCuenta");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneTiposCuenta";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de cuenta en base de datos.");
            }
        }

        /// Inserta una nueva colectiva en base de datos
        /// </summary>
        /// <param name="idTipoCol">Identificador del tipo de colectiva</param>
        /// <param name="idColPadre">Identificador de la colectiva padre</param>
        /// <param name="claveCol">Clave de la colectiva</param>
        /// <param name="razonSocial">Nombre o razón social</param>
        /// <param name="nombreComercial">Nombre comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaColectiva(int idTipoCol, int idColPadre, string claveCol, string razonSocial,
            string nombreComercial, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaNuevaColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTipoColectiva", idTipoCol));
                        command.Parameters.Add(new SqlParameter("@IdColectivaPadre",
                            idColPadre == 0 ? (int?)null : idColPadre));
                        command.Parameters.Add(new SqlParameter("@ClaveColectiva", claveCol));
                        command.Parameters.Add(new SqlParameter("@NombreORazonSocial", razonSocial));
                        command.Parameters.Add(new SqlParameter("@NombreComercial", nombreComercial));
                        

                        var sqlParameter1 = new SqlParameter("@IdNuevaColectiva", SqlDbType.BigInt);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevaColectiva");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevaColectiva"] = sqlParameter1.Value.ToString();
                        dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva colectiva en base de datos
        /// </summary>
        /// <param name="idTipoCol">Identificador del tipo de colectiva</param>
        /// <param name="idColPadre">Identificador de la colectiva padre</param>
        /// <param name="claveCol">Clave de la colectiva</param>
        /// <param name="razonSocial">Nombre o razón social</param>
        /// <param name="nombreComercial">Nombre comercial</param>
        /// <param name="idDivisa">Identificador de la divisa</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaColectiva(int idTipoCol, int idColPadre, string claveCol, string razonSocial,
            string nombreComercial, int idDivisa, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaNuevaColectiva", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdTipoColectiva", idTipoCol));
                command.Parameters.Add(new SqlParameter("@IdColectivaPadre",
                    idColPadre == 0 ? (int?)null : idColPadre));
                command.Parameters.Add(new SqlParameter("@ClaveColectiva", claveCol));
                command.Parameters.Add(new SqlParameter("@NombreORazonSocial", razonSocial));
                command.Parameters.Add(new SqlParameter("@NombreComercial", nombreComercial));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                command.Parameters.Add(new SqlParameter("@IdDivisa", idDivisa));

                var sqlParameter1 = new SqlParameter("@IdNuevaColectiva", SqlDbType.BigInt);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Size = 200;
                command.Parameters.Add(sqlParameter2);

                var sqlParameter3 = new SqlParameter("@IdCuentaCCLC", SqlDbType.BigInt);
                sqlParameter3.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter3);

                command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_InsertaNuevaColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = "@ID_NuevaColectiva=" + sqlParameter1.Value.ToString() +
                    "|@Mensaje=" + sqlParameter2.Value.ToString() + "|@IdCuentaCCLC=" + sqlParameter3.Value.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoColectiva=" + idTipoCol);
                parametros.Add("P2", "@IdColectivaPadre=" + idColPadre);
                parametros.Add("P3", "@ClaveColectiva=" + claveCol);
                parametros.Add("P4", "@NombreORazonSocial=" + razonSocial);
                parametros.Add("P5", "@NombreComercial=" + nombreComercial);
                parametros.Add("P6", "@Usuario=" + elUsuario.ClaveUsuario);
                parametros.Add("P7", "@IdDivisa=" + idDivisa);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                DataTable dt = new DataTable();
                dt.Columns.Add("ID_NuevaColectiva");
                dt.Columns.Add("Mensaje");
                dt.Columns.Add("ID_CuentaCCLC");
                dt.Rows.Add();

                dt.Rows[0]["ID_NuevaColectiva"] = sqlParameter1.Value.ToString();
                dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();
                dt.Rows[0]["ID_CuentaCCLC"] = sqlParameter3.Value.ToString();

                return dt;

            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la colectiva en base de datos.");
            }
        }

        /// Obtiene la información de la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaInfoColectiva(int IdColectiva, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColectivaPorID");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la información de la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaInfoColectiva(int IdColectiva, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColectivaPorID");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneColectivaPorID";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar la Colectiva por ID.");
            }
        }

        /// Actualiza los datos de una colectiva en el Autorizador
        /// </summary>
        /// <param name="colectiva">Entidad con los datos a modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaColectiva(Colectiva colectiva, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", colectiva.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@Clave", colectiva.ClaveColectiva));
                        command.Parameters.Add(new SqlParameter("@Nombre", colectiva.NombreORazonSocial));
                        command.Parameters.Add(new SqlParameter("@ApPaterno", colectiva.APaterno));
                        command.Parameters.Add(new SqlParameter("@ApMaterno", colectiva.AMaterno));
                        command.Parameters.Add(new SqlParameter("@FechaNacimiento", colectiva.FechaNacimiento));
                        command.Parameters.Add(new SqlParameter("@RFC", colectiva.RFC));
                        command.Parameters.Add(new SqlParameter("@CURP", colectiva.CURP));
                        command.Parameters.Add(new SqlParameter("@Telefono", colectiva.Telefono));
                        command.Parameters.Add(new SqlParameter("@Movil", colectiva.Movil));
                        command.Parameters.Add(new SqlParameter("@Email", colectiva.Email));
                        command.Parameters.Add(new SqlParameter("@IdCadenaRelacionada", colectiva.IdCadenaRelacionada));
                        command.Parameters.Add(new SqlParameter("@IdEstatus", colectiva.IdEstatus));
                        command.Parameters.Add(new SqlParameter("@Password", colectiva.Password));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de una colectiva en el Autorizador
        /// </summary>
        /// <param name="colectiva">Entidad con los datos a modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaColectiva(Colectiva colectiva, ILogHeader logHeader, Usuario elUsuario)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", colectiva.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@Clave", colectiva.ClaveColectiva));
                        command.Parameters.Add(new SqlParameter("@Nombre", colectiva.NombreORazonSocial));
                        command.Parameters.Add(new SqlParameter("@ApPaterno", colectiva.APaterno));
                        command.Parameters.Add(new SqlParameter("@ApMaterno", colectiva.AMaterno));
                        command.Parameters.Add(new SqlParameter("@FechaNacimiento", colectiva.FechaNacimiento));
                        command.Parameters.Add(new SqlParameter("@RFC", colectiva.RFC));
                        command.Parameters.Add(new SqlParameter("@CURP", colectiva.CURP));
                        command.Parameters.Add(new SqlParameter("@Telefono", colectiva.Telefono));
                        command.Parameters.Add(new SqlParameter("@Movil", colectiva.Movil));
                        command.Parameters.Add(new SqlParameter("@Email", colectiva.Email));
                        command.Parameters.Add(new SqlParameter("@IdCadenaRelacionada", colectiva.IdCadenaRelacionada));
                        command.Parameters.Add(new SqlParameter("@IdEstatus", colectiva.IdEstatus));
                        command.Parameters.Add(new SqlParameter("@NombreComercial", colectiva.NombreComercial));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaColectiva";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdColectiva=" + colectiva.ID_Colectiva);
                        parametros.Add("P2", "@Clave=" + colectiva.ClaveColectiva);
                        parametros.Add("P3", "@Nombre=" + colectiva.NombreORazonSocial);
                        parametros.Add("P4", "@ApPaterno=" + colectiva.APaterno);
                        parametros.Add("P5", "@ApMaterno=" + colectiva.AMaterno);
                        parametros.Add("P6", "@FechaNacimiento=" + colectiva.FechaNacimiento);
                        parametros.Add("P7", "@RFC=" + (String.IsNullOrEmpty(colectiva.RFC) ? colectiva.RFC :
                            MaskSensitiveData.generalInfo(colectiva.RFC)));
                        parametros.Add("P8", "@CURP=" + colectiva.CURP);
                        parametros.Add("P9", "@Telefono=" + (String.IsNullOrEmpty(colectiva.Telefono) ? colectiva.Telefono :
                            MaskSensitiveData.generalInfo(colectiva.Telefono)));
                        parametros.Add("P10", "@Movil=" + (String.IsNullOrEmpty(colectiva.Movil) ? colectiva.Movil :
                            MaskSensitiveData.generalInfo(colectiva.Movil)));
                        parametros.Add("P11", "@Email=" + (String.IsNullOrEmpty(colectiva.Email) ? colectiva.Email :
                            MaskSensitiveData.Email(colectiva.Email)));
                        parametros.Add("P12", "@IdCadenaRelacionada=" + colectiva.IdCadenaRelacionada);
                        parametros.Add("P13", "@IdEstatus=" + colectiva.IdEstatus);

                        if (!string.IsNullOrEmpty(colectiva.Password))
                        {
                            parametros.Add("P14", "@Password=******");
                            parametros.Add("P15", "@NombreComercial=" + colectiva.NombreComercial);
                        }
                        else
                        {
                            parametros.Add("P14", "@NombreComercial=" + colectiva.NombreComercial);
                        }

                        parametros.Add("P16", "@Usuario=" + elUsuario.ClaveUsuario);

                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar los datos de la Colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene la dirección de la colectiva en el Autorizador, según el tipo indicado
        /// (Física = 1, Fiscal = 2)
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdTipoDireccion">Identificador del tipo de dirección</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ConsultaDireccionColectiva(int IdColectiva, int IdTipoDireccion, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDireccionColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@TipoDireccion", DbType.Int32, IdTipoDireccion);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la dirección de la colectiva en el Autorizador, según el tipo indicado
        /// (Física = 1, Fiscal = 2)
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdTipoDireccion">Identificador del tipo de dirección</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ConsultaDireccionColectiva(int IdColectiva, int IdTipoDireccion, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDireccionColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@TipoDireccion", DbType.Int32, IdTipoDireccion);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneDireccionColectiva";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@TipoDireccion=" + IdTipoDireccion);
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el domicilio de la Colectiva en base de datos");
            }
        }

        /// Valida que el código postal capturado exista en base de datos
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Leyenda con la validación</returns>
        public static string ValidaCodigoPostal(string codigoPostal, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                using (SqlCommand command = new SqlCommand("web_CA_ValidaCodigoPostal", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@CP", codigoPostal));
                    command.Parameters.Add("@ValidaCP", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;

                    conn.Open();
                    command.ExecuteNonQuery();

                    string validacion = command.Parameters["@ValidaCP"].Value.ToString();

                    conn.Close();

                    return validacion;
                }
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Valida que el código postal capturado exista en base de datos
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Leyenda con la validación</returns>
        public static string ValidaCodigoPostal(string codigoPostal, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                using (SqlCommand command = new SqlCommand("web_CA_ValidaCodigoPostal", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@CP", codigoPostal));
                    command.Parameters.Add("@ValidaCP", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;

                    conn.Open();
                    command.ExecuteNonQuery();

                    string validacion = command.Parameters["@ValidaCP"].Value.ToString();

                    /////>>>LOG DEBUG
                    LogDebugMsg logDebug = new LogDebugMsg();
                    logDebug.M_Value = "web_CA_ValidaCodigoPostal";
                    logDebug.C_Value = "";
                    logDebug.R_Value = "@ValidaCP=" + validacion;

                    log.Debug(logDebug);
                    /////<<<LOG DEBUG

                    return validacion;
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al validar el Código Postal en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los datos de colonia, municipio y estado a partir del código postal indicado.
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaDatosPorCodigoPostal(string codigoPostal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColoniaMunicipio");

                database.AddInParameter(command, "@CP", DbType.String, codigoPostal);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene los datos de colonia, municipio y estado a partir del código postal indicado.
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaDatosPorCodigoPostal(string codigoPostal, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColoniaMunicipio");

                database.AddInParameter(command, "@CP", DbType.String, codigoPostal);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneColoniaMunicipio";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@CP=" + codigoPostal);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las colonias del Código Postal");
            }
        }

        /// <summary>
        /// Actualiza los datos de una colectiva en el Autorizador
        /// </summary>
        /// <param name="colectiva">Entidad con los datos a modificar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaDireccionColectiva(DireccionColectiva direccion, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaDireccionColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", direccion.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdDireccion", direccion.ID_Direccion));
                        command.Parameters.Add(new SqlParameter("@IdTipoDireccion", direccion.ID_TipoDireccion));
                        command.Parameters.Add(new SqlParameter("@IdColonia", direccion.ID_Asentamiento));
                        command.Parameters.Add(new SqlParameter("@Calle", direccion.Calle));
                        command.Parameters.Add(new SqlParameter("@NumExterior", direccion.NumExterior));
                        command.Parameters.Add(new SqlParameter("@NumInterior", direccion.NumInterior));
                        command.Parameters.Add(new SqlParameter("@EntreCalles", direccion.EntreCalles));
                        command.Parameters.Add(new SqlParameter("@Referencias", direccion.Referencias));
                        command.Parameters.Add(new SqlParameter("@Colonia", direccion.Colonia));
                        command.Parameters.Add(new SqlParameter("@CP", direccion.CodigoPostal));
                        command.Parameters.Add(new SqlParameter("@ClaveMpio", direccion.ClaveMunicipio));
                        command.Parameters.Add(new SqlParameter("@ClaveEdo", direccion.ClaveEstado));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de una colectiva en el Autorizador
        /// </summary>
        /// <param name="colectiva">Entidad con los datos a modificar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDireccionColectiva(DireccionColectiva direccion, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaDireccionColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", direccion.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdDireccion", direccion.ID_Direccion));
                        command.Parameters.Add(new SqlParameter("@IdTipoDireccion", direccion.ID_TipoDireccion));
                        command.Parameters.Add(new SqlParameter("@IdColonia", direccion.ID_Asentamiento));
                        command.Parameters.Add(new SqlParameter("@Calle", direccion.Calle));
                        command.Parameters.Add(new SqlParameter("@NumExterior", direccion.NumExterior));
                        command.Parameters.Add(new SqlParameter("@NumInterior", direccion.NumInterior));
                        command.Parameters.Add(new SqlParameter("@EntreCalles", direccion.EntreCalles));
                        command.Parameters.Add(new SqlParameter("@Referencias", direccion.Referencias));
                        command.Parameters.Add(new SqlParameter("@Colonia", direccion.Colonia));
                        command.Parameters.Add(new SqlParameter("@CP", direccion.CodigoPostal));
                        command.Parameters.Add(new SqlParameter("@ClaveMpio", direccion.ClaveMunicipio));
                        command.Parameters.Add(new SqlParameter("@ClaveEdo", direccion.ClaveEstado));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaDireccionColectiva";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdColectiva=" + direccion.ID_Colectiva);
                        parametros.Add("P2", "@IdDireccion=" + direccion.ID_Direccion);
                        parametros.Add("P3", "@IdTipoDireccion=" + direccion.ID_TipoDireccion);
                        parametros.Add("P4", "@Calle=" + direccion.Calle);
                        parametros.Add("P5", "@NumExterior=" + direccion.ID_Colectiva);
                        parametros.Add("P6", "@NumInterior=" + direccion.ID_Colectiva);
                        parametros.Add("P7", "@EntreCalles=" + direccion.ID_Colectiva);
                        parametros.Add("P8", "@Referencias=" + direccion.ID_Colectiva);
                        parametros.Add("P9", "@Colonia=" + direccion.ID_Colectiva);
                        parametros.Add("P10", "@IdColectiva=" + direccion.ID_Colectiva);
                        parametros.Add("P11", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el Domicilio de la Colectiva en base de datos");
            }
        }

        /// Consulta las cuentas de la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet ConsultaCuentasColectiva(int IdColectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las cuentas de la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet ConsultaCuentasColectiva(int IdColectiva, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneCuentasColectiva";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppID=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener las Cuentas de la Colectiva en base de datos");
            }
        }

        /// <summary>
        /// Obtiene la lista de periodos en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaPeriodos(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePeriodos");

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
        /// Obtiene la lista de periodos en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaPeriodos(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePeriodos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePeriodos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Periodos de las Cuentas en base de datos");
            }
        }

        /// <summary>
        /// Consulta la información adicionasl de la cuenta en el Autorizador
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet ConsultaDatosExtraCuenta(Int64 IdCuenta, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneInfoExtraCuenta");

                database.AddInParameter(command, "@IdCuenta", DbType.Int64, IdCuenta);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta la información adicional de la cuenta en el Autorizador
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet ConsultaDatosExtraCuenta(Int64 IdCuenta, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneInfoExtraCuenta");

                database.AddInParameter(command, "@IdCuenta", DbType.Int64, IdCuenta);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneInfoExtraCuenta";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdCuenta=" + IdCuenta);
                logDBG.Parameters = parametros;

                logPCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los datos extra de las Cuenta en base de datos");
            }
        }

        /// <summary>
        /// Inserta una nueva cuenta para la colectiva en base de datos
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la nueva cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaCuentaColectiva(Cuenta nuevaCuenta, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaCuentaColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", nuevaCuenta.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdTipoCuenta", nuevaCuenta.ID_TipoCuenta));
                        command.Parameters.Add(new SqlParameter("@IdGpoCuenta", nuevaCuenta.ID_GrupoCuentas));
                        command.Parameters.Add(new SqlParameter("@DescripcionCuenta", nuevaCuenta.Descripcion));
                        command.Parameters.Add(new SqlParameter("@Vigencia",
                            nuevaCuenta.Vigencia.Equals(DateTime.MinValue) ? DBNull.Value : (object)nuevaCuenta.Vigencia));
                        command.Parameters.Add(new SqlParameter("@IdCadenaRelacionada",
                            nuevaCuenta.ID_ColectivaCadenaComercial == -1 ? DBNull.Value : (object)nuevaCuenta.ID_ColectivaCadenaComercial));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva cuenta para la colectiva en base de datos
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la nueva cuenta</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static int InsertaCuentaColectiva(Cuenta nuevaCuenta, SqlConnection connection, SqlTransaction transaccionSQL,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaCuentaColectiva", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdColectiva", nuevaCuenta.ID_Colectiva));
                command.Parameters.Add(new SqlParameter("@IdTipoCuenta", nuevaCuenta.ID_TipoCuenta));
                command.Parameters.Add(new SqlParameter("@DescripcionCuenta", nuevaCuenta.Descripcion));
                command.Parameters.Add(new SqlParameter("@Vigencia",
                    nuevaCuenta.Vigencia.Equals(DateTime.MinValue) ? DBNull.Value : (object)nuevaCuenta.Vigencia));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                var sqlParameter = new SqlParameter("@IdNuevaCuentaCCLC", SqlDbType.BigInt);
                sqlParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter);

                int resp = command.ExecuteNonQuery();

                int idCta = Convert.ToInt32(sqlParameter.Value);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_InsertaCuentaColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + nuevaCuenta.ID_Colectiva);
                parametros.Add("P2", "@IdTipoCuenta=" + nuevaCuenta.ID_TipoCuenta);
                parametros.Add("P3", "@DescripcionCuenta=" + nuevaCuenta.Descripcion);
                parametros.Add("P4", "@Vigencia=" + nuevaCuenta.Vigencia);
                parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                parametros.Add("P6", "@IdNuevaCuentaCCLC=" + idCta.ToString());
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return idCta;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la Cuenta de la Colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Actualiza los datos de una cuenta existente en el Autorizador
        /// </summary>
        /// <param name="unaCuenta">Datos de la cuenta a modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaDatosCuenta(Cuenta unaCuenta, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaCuentaColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTipoCuenta", unaCuenta.ID_TipoCuenta));
                        command.Parameters.Add(new SqlParameter("@IdGpoCuenta", unaCuenta.ID_GrupoCuentas));
                        command.Parameters.Add(new SqlParameter("@DescripcionCuenta", unaCuenta.Descripcion));
                        command.Parameters.Add(new SqlParameter("@Nivel",
                            unaCuenta.Nivel == -1 ? DBNull.Value : (object)unaCuenta.Nivel));
                        command.Parameters.Add(new SqlParameter("@HeredaSaldo",
                            unaCuenta.HeredaSaldo == -1 ? DBNull.Value : (object)unaCuenta.HeredaSaldo));
                        command.Parameters.Add(new SqlParameter("@Vigencia",
                            unaCuenta.Vigencia.Equals(DateTime.MinValue) ? DBNull.Value : (object)unaCuenta.Vigencia));
                        command.Parameters.Add(new SqlParameter("@IdCadenaRelacionada",
                            unaCuenta.ID_ColectivaCadenaComercial == -1 ? DBNull.Value : (object)unaCuenta.ID_ColectivaCadenaComercial));
                        command.Parameters.Add(new SqlParameter("@IdPeriodo",
                            unaCuenta.ID_Periodo == -1 ? DBNull.Value : (object)unaCuenta.ID_Periodo));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de una cuenta existente en el Autorizador
        /// </summary>
        /// <param name="unaCuenta">Datos de la cuenta a modificar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDatosCuenta(Cuenta unaCuenta, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaCuentaColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", unaCuenta.ID_Cuenta));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", unaCuenta.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@DescripcionCuenta", unaCuenta.Descripcion));
                        command.Parameters.Add(new SqlParameter("@Vigencia",
                            unaCuenta.Vigencia.Equals(DateTime.MinValue) ? DBNull.Value : (object)unaCuenta.Vigencia));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaCuentaColectiva";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + unaCuenta.ID_Cuenta);
                        parametros.Add("P2", "@IdColectiva=" + unaCuenta.ID_Colectiva);
                        parametros.Add("P3", "@DescripcionCuenta=" + unaCuenta.Descripcion);
                        parametros.Add("P4", "@Vigencia=" + unaCuenta.Vigencia);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        pCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la Cuenta en base de datos");
            }
        }

        /// Actualiza el estatus de una cuenta existente en el Autorizador
        /// </summary>
        /// <param name="idCta">Identificador de  la cuenta</param>
        /// <param name="estatus">Clave del estatus actual de la cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusCuenta(Int64 idCta, String estatus, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_BloqueaDesbloqueaCuenta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", idCta));
                        command.Parameters.Add(new SqlParameter("@ClaveEstatusActual", estatus));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el estatus de una cuenta existente en el Autorizador
        /// </summary>
        /// <param name="idCta">Identificador de  la cuenta</param>
        /// <param name="estatus">Clave del estatus actual de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusCuenta(long idCta, string estatus, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_BloqueaDesbloqueaCuenta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", idCta));
                        command.Parameters.Add(new SqlParameter("@ClaveEstatusActual", estatus));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_BloqueaDesbloqueaCuenta";
                        logDbg.C_Value = "";
                        logDbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + idCta);
                        parametros.Add("P2", "@ClaveEstatusActual=" + estatus);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDbg.Parameters = parametros;

                        log.Debug(logDbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus de la Cuenta en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el listado de valores de contrato de la colectiva en el Autorizador
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los valores de contrato</returns>
        public static DataTable ConsultaValoresContratoColectiva(Int64 ID_Colectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresContratoColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, ID_Colectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el listado de valores de contrato sin asignar a la colectiva en el Autorizador
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns></returns>
        public static DataSet ListaValoresContratoSinAsignar(Int64 ID_Colectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresContratoSinAsignar");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, ID_Colectiva);
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
        /// Clona el contrato y sus valores de una colectiva a otra en el Autorizador
        /// </summary>
        /// <param name="idColOrig">Identificador de  la colectiva origen</param>
        /// <param name="idColDest">Identificador de la colectiva destino</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ClonaContratoColectiva(Int64 idColOrig, Int64 idColDest, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ClonarValoresContratoColectiva", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdColectivaOrigen", idColOrig));
                command.Parameters.Add(new SqlParameter("@IdColectivaDestino", idColDest));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta el valor de contrato a la colectiva en el Autorizador
        /// </summary>
        /// <param name="idValor">Identificador del valor de contrato</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static string InsertaValorContrato(Int32 idValor, Int64 idColectiva, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaValorFijoContratoColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorContrato", idValor));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));

                        var sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        return command.Parameters["@Mensaje"].Value.ToString();
                    }
                }
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta el valor de contrato a la colectiva en el Autorizador
        /// </summary>
        /// <param name="idValor">Identificador del valor de contrato</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static string InsertaValorContrato(int idValor, long idColectiva, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaValorFijoContratoColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorContrato", idValor));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter.Direction = ParameterDirection.Output;
                        sqlParameter.Size = 200;
                        command.Parameters.Add(sqlParameter);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string msjResp = command.Parameters["@Mensaje"].Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaValorFijoContratoColectiva";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@Mensaje=" + msjResp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorContrato=" + idValor);
                        parametros.Add("P2", "@IdColectiva=" + idColectiva);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return msjResp;
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el Parámetro a la Colectiva en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros extra sin asignar a la colectiva en el Autorizador
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns></returns>
        public static DataSet ListaParamsExtraSinAsignar(Int64 ID_Colectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParamsExtraSinAsignar");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, ID_Colectiva);
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
        /// Obtiene el listado de parámetros extra sin asignar a la colectiva en el Autorizador
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns></returns>
        public static DataSet ListaParamsExtraSinAsignar(Int64 ID_Colectiva, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParamsExtraSinAsignar");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, ID_Colectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneParamsExtraSinAsignar";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + ID_Colectiva);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los Parámetos Extra de la Colectiva en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros extra de la colectiva en el Autorizador
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static DataSet ConsultaParamsExtraColectiva(Int64 ID_Colectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParamsExtraColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, ID_Colectiva);
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
        /// Inserta el parámetro extra de la colectiva en la tabla de valores del Autorizador
        /// </summary>
        /// <param name="idParam">Identificador del parámetro extra</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaValorParametroExtra(Int32 idParam, Int64 idColectiva, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaParametroExtraColectiva", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdParametroExtra", idParam));
                command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static object ConsultaTokensColectiva(int idColectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTokensColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ConsultaTokenById(int TokenId, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTokenById");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);
                database.AddInParameter(command, "@TokenId", DbType.Int32, TokenId);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        internal static void DeleteTokenById(int tokenId, Usuario usuario,
              SqlConnection connection, SqlTransaction transaccionSQL)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_EliminaTokenById", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@TokenId", tokenId));

                command.ExecuteNonQuery();

            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, usuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el valor del parámetro extra de la colectiva en el Autorizador
        /// </summary>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaValorParametroExtra(ParametroExtra elParametro, Int64 IdColectiva,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaValorParametroExtra", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdColectiva", IdColectiva));
                command.Parameters.Add(new SqlParameter("@IdParametroExtra", elParametro.ID_Parametro));
                command.Parameters.Add(new SqlParameter("@IdParametroPrestablecido", elParametro.ID_ParametroPrestablecido));
                command.Parameters.Add(new SqlParameter("@NuevoValor", elParametro.Valor));

                command.ExecuteNonQuery();
            }

            catch (Exception Ex)
            {
                Utilidades.Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("ActualizaValorParametroExtra(). Error al actualizar el parámetro extra de la colectiva: " + Ex);
            }
        }

        /// <summary>
        /// Obtiene el catálogo de valores para el parámetro extra colectiva con el ID indicado
        /// en el Autorizador
        /// </summary>
        /// <param name="idParamExtra">Identificador del parámetro extra colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaCatalogoValoresParamsExtraColectiva(int idParamExtra, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresParametroExtraColectiva");

                database.AddInParameter(command, "@IdParametroExtra", DbType.Int32, idParamExtra);
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
        /// Elimina el parámetro extra de la colectiva en la tabla de valores del Autorizador
        /// </summary>
        /// <param name="idParam">Identificador del parámetro extra</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void EliminaValorParametroExtra(Int32 idParam, Int64 idColectiva, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_EliminaParametroExtraColectiva", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdParametroExtra", idParam));
                command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el listado de clasificaciones de parámetros en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ListaClasificacionParametros(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClasificacionParametros");

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
        /// Obtiene el listado de clasificaciones de parámetros en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con la lista de grupos</returns>
        public static DataSet ListaClasificacionParametros(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClasificacionParametros");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneClasificacionParametros";
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
                throw new CAppException(8010, "Ocurrió un error al consultar la clasificación de parámetros en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros de contrato sin asignar a la colectiva,
        /// que pertenecen a la clasificación marcada en el Autorizador
        /// </summary>
        /// <param name="idClasificacion">Identificador de la clasificación</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns></returns>
        public static DataSet ListaParametrosSinAsignar(int idClasificacion, Int64 idColectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosSinAsignar");

                database.AddInParameter(command, "@IdClasificacion", DbType.Int32, idClasificacion);
                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
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
        /// Obtiene el listado de parámetros de contrato sin asignar a la colectiva,
        /// que pertenecen a la clasificación marcada en el Autorizador
        /// </summary>
        /// <param name="idClasificacion">Identificador de la clasificación</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns></returns>
        public static DataSet ListaParametrosSinAsignar(int idClasificacion, Int64 idColectiva, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosSinAsignar");

                database.AddInParameter(command, "@IdClasificacion", DbType.Int32, idClasificacion);
                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosSinAsignar";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdClasificacion=" + idClasificacion);
                parametros.Add("P2", "@IdColectiva=" + idColectiva);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los Parámetros sin Asignar de base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros de contrato asignados a la colectiva,
        /// que pertenecen a la clasificación marcada en el Autorizador
        /// </summary>
        /// <param name="idClasificacion">Identificador de la clasificación</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns></returns>
        public static DataSet ListaParametrosAsignados(int idClasificacion, Int64 idColectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosAsignados");

                database.AddInParameter(command, "@IdClasificacion", DbType.Int32, idClasificacion);
                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
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
        /// Obtiene el listado de parámetros de contrato asignados a la colectiva,
        /// que pertenecen a la clasificación marcada en el Autorizador
        /// </summary>
        /// <param name="idClasificacion">Identificador de la clasificación</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns></returns>
        public static DataSet ListaParametrosAsignados(int idClasificacion, Int64 idColectiva, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosAsignados");

                database.AddInParameter(command, "@IdClasificacion", DbType.Int32, idClasificacion);
                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosAsignados";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdClasificacion=" + idClasificacion);
                parametros.Add("P2", "@IdColectiva=" + idColectiva);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Parámetros de la Colectiva de base de datos");
            }
        }

        /// Obtiene el catálogo de valores para el parámetro de contrato con el ID indicado
        /// en el Autorizador
        /// </summary>
        /// <param name="idParametro">Identificador del parámetro extra colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaCatalogoValoresParametro(int idParametro, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresParametroContrato");

                database.AddInParameter(command, "@IdValorContrato", DbType.Int32, idParametro);
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
        /// Obtiene el catálogo de valores para el parámetro de contrato con el ID indicado
        /// en el Autorizador
        /// </summary>
        /// <param name="idParametro">Identificador del parámetro extra colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaCatalogoValoresParametro(int idParametro, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresParametroContrato");

                database.AddInParameter(command, "@IdValorContrato", DbType.Int32, idParametro);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneValoresParametroContrato";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdValorContrato=" + idParametro);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los valores preestablecidos " +
                    "del Parámetro en base de datos");
            }
        }

        /// <summary>
        /// Elimina el parámetro de contrato de la colectiva en la tabla de valores del Autorizador
        /// </summary>
        /// <param name="idParam">Identificador del parámetro de contrato</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void EliminaValorParametro(Int32 idParam, Int64 idColectiva, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaContratoValorFijoColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorContrato", idParam));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Elimina el parámetro de contrato de la colectiva en la tabla de valores del Autorizador
        /// </summary>
        /// <param name="idParam">Identificador del parámetro de contrato</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaValorParametro(int idParam, long idColectiva, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaContratoValorFijoColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorContrato", idParam));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_EliminaContratoValorFijoColectiva";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorContrato=" + idParam);
                        parametros.Add("P2", "@IdColectiva=" + idColectiva);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar el valor del Parámetro en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el valor del parámetro de contrato de la colectiva en el Autorizador
        /// </summary>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaValorParametro(Parametro elParametro, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaContratoValorFijo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorContrato", elParametro.ID_Parametro));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", elParametro.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdValorPrestablecido", elParametro.ID_ParametroPrestablecido));
                        command.Parameters.Add(new SqlParameter("@NuevoValor", elParametro.Valor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception Ex)
            {
                Utilidades.Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("ActualizaValorParametro(). Error al actualizar el parámetro de la colectiva: " + Ex);
            }
        }

        /// <summary>
        /// Actualiza el valor del parámetro de contrato de la colectiva en el Autorizador
        /// </summary>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaValorParametro(Parametro elParametro, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaContratoValorFijo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorContrato", elParametro.ID_Parametro));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", elParametro.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@IdValorPrestablecido", elParametro.ID_ParametroPrestablecido));
                        command.Parameters.Add(new SqlParameter("@NuevoValor", elParametro.Valor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ActualizaContratoValorFijo";
                        logDbg.C_Value = "";
                        logDbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorContrato=" + elParametro.ID_ValorContrato);
                        parametros.Add("P2", "@IdColectiva=" + elParametro.ID_Colectiva);
                        parametros.Add("P3", "@IdValorPrestablecido=" + elParametro.ID_ParametroPrestablecido);
                        parametros.Add("P4", "@NuevoValor=" + elParametro.Valor);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDbg.Parameters = parametros;

                        unLog.Debug(logDbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el Parámetro de la Colectiva");
            }
        }

        /// <summary>
        /// Obtiene el listado de prouctos plugin Samrt Points asignados o no a la colectiva en el Autorizador
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con el listado</returns>
        public static DataSet ObtieneProductosPluginPuntosBancarios(Int64 ID_Colectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProductosPluginColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, ID_Colectiva);
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
        /// Inserta o actualiza el estatus del producto plugin en la colectiva dentro del Autorizador
        /// </summary>
        /// <param name="idProdPlugin">Identificador del producto plugin</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="estatus">Estatus del producto plugin</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaEstatusProductoPluginColectiva(int idProdPlugin, Int64 idColectiva, int estatus,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaActualizaProductoPluginColectiva", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                command.Parameters.Add(new SqlParameter("@IdProductoPlugin", idProdPlugin));
                command.Parameters.Add(new SqlParameter("@Activo", estatus));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta un producto plugin de tipo Smart Points (DIESTEL) y lo relaciona con la colectiva
        /// indicada en del Autorizador
        /// </summary>
        /// <param name="sku">SKU del nuevo producto plugin</param>
        /// <param name="bin">BIN del nuevo producto plugin</param>
        /// <param name="clave">Clave del nuevo producto plugin</param>
        /// <param name="descripcion">Descripción del nuevo producto plugin</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaProductoPluginAColectiva(string sku, string bin, string clave, string descripcion,
            Int64 idColectiva, SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaBINSmartPoints", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@SKU", sku));
                command.Parameters.Add(new SqlParameter("@BIN", bin));
                command.Parameters.Add(new SqlParameter("@Clave", clave));
                command.Parameters.Add(new SqlParameter("@Descripcion", descripcion));
                command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la lista las divisas por cada tipo de colectiva en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaDivisasColectivas(int idTipoColectiva, Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDivisasPorTipoColectiva");

                database.AddInParameter(command, "@idTipoColectiva", DbType.Int64, idTipoColectiva);
                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneDivisasPorTipoColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las divisas por tipo de colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Crea los medios de acceso de tipo IDCTA y CTAINT en base de datos, para una nueva colectiva
        /// </summary>
        /// <param name="idNuevaCol">Identificador de la nueva colectiva</param>
        /// <param name="MA_IdCta">Medio de acceso de tipo IDCTA</param>
        /// <param name="MA_CtaInterna">Medio de acceso Cuenta Interna</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void CreaMediosAcceso(string idNuevaCol, string MA_IdCta, string MA_CtaInterna,
            SqlConnection connection, SqlTransaction transaccionSQL, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_AltaMediosAccesoNuevaColectiva", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                SqlParameter paramMAI = command.CreateParameter();
                paramMAI.ParameterName = "@ClaveMAIdCuenta";
                paramMAI.DbType = DbType.AnsiStringFixedLength;
                paramMAI.Direction = ParameterDirection.Input;
                paramMAI.Value = MA_IdCta;
                paramMAI.Size = int.Parse(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "LongClaveMA", elLog).Valor);
                command.Parameters.Add(paramMAI);

                SqlParameter paramMAC = command.CreateParameter();
                paramMAC.ParameterName = "@ClaveMACacao";
                paramMAC.DbType = DbType.AnsiStringFixedLength;
                paramMAC.Direction = ParameterDirection.Input;
                paramMAC.Value = MA_CtaInterna;
                paramMAC.Size = int.Parse(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "LongClaveMA", elLog).Valor);
                command.Parameters.Add(paramMAC);

                command.Parameters.Add(new SqlParameter("@IdColectiva", idNuevaCol));

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_AltaMediosAccesoNuevaColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ClaveMAIdCuenta=" + MA_IdCta);
                parametros.Add("P2", "@ClaveMACacao=" + MA_CtaInterna);
                parametros.Add("P3", "@IdColectiva=" + idNuevaCol);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al crear los medios de acceso de la colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el Medio de Acceso de la colectiva en el Autorizador
        /// ("-1" para obtener todos los tipos de MA)
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="ClaveTipoMA">Identificador del tipo de dirección</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ConsultaMediosAccesoColectiva(int IdColectiva, string ClaveTipoMA, Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMediosAccesoPorColectiva");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@ClaveTipoMA", DbType.String, ClaveTipoMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneMediosAccesoPorColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva.ToString());
                parametros.Add("P2", "@ClaveTipoMA=" + ClaveTipoMA);
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
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza o inserta el valor de una cuenta CLABE de la colectiva en el Autorizador
        /// </summary>
        /// <param name="idColectiva">Idetnificador de la Colectiva</param>
        /// <param name="CLABE">Datos de la cuenta CLABE</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static int ActualizaCuentaCLABE(long idColectiva, string CLABE, Usuario elUsuario,
            SqlConnection connection, SqlTransaction transaccionSQL, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaCLABEPorColectiva", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));

                SqlParameter paramCBE = command.CreateParameter();
                paramCBE.ParameterName = "@CLABE";
                paramCBE.DbType = DbType.AnsiStringFixedLength;
                paramCBE.Direction = ParameterDirection.Input;
                paramCBE.Value = CLABE;
                paramCBE.Size = int.Parse(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "LongClaveMA", elLog).Valor);
                command.Parameters.Add(paramCBE);

                var sqlParameter1 = new SqlParameter("@IdMA", SqlDbType.Int);
                sqlParameter1.Direction = ParameterDirection.Output;
                command.Parameters.Add(sqlParameter1);

                command.ExecuteNonQuery();
                int resp = Convert.ToInt32(sqlParameter1.Value);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ActualizaCLABEPorColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva.ToString());
                parametros.Add("P2", "@CLABE=" + MaskSensitiveData.CLABE(CLABE));
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return resp;
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la Cuenta CLABE de la Colectiva en base de datos.");
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
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaRegistroEnBitacora(string SP, string tabla, string campo, int idRegistro,
            string valor, Usuario elUsuario, SqlConnection connection, SqlTransaction transaccionSQL, ILogHeader elLog)
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

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_InsertaRegistroBitacoraDetalle";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@NombreSP=" + SP);
                parametros.Add("P2", "@TablaModificada=" + tabla);
                parametros.Add("P3", "@CampoModificado=" + campo);
                parametros.Add("P4", "@ID_RegistroModificado=" + idRegistro.ToString());
                parametros.Add("P5", "@NuevoValorCampo=" + valor);
                parametros.Add("P6", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el registro en bitácora en base de datos.");
            }
        }

        /// <summary>
        /// Inserta el medio de acceso ID_Cuenta a la colectiva en base de datos
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="claveMA">Clave del medio de acceso (ID_Cuenta)</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaMA_IDCuentaColectiva(long idColectiva, string claveMA, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaMA_IDCTA", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                SqlParameter paramIdCta = command.CreateParameter();
                paramIdCta.ParameterName = "@ClaveMAIdCuenta";
                paramIdCta.DbType = DbType.AnsiStringFixedLength;
                paramIdCta.Direction = ParameterDirection.Input;
                paramIdCta.Value = claveMA;
                paramIdCta.Size = int.Parse(Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "LongClaveMA", elLog).Valor);
                command.Parameters.Add(paramIdCta);
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_InsertaMA_IDCTA";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva);
                parametros.Add("P2", "@ClaveMAIdCuenta=" + claveMA);
                parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el medio de acceso ID_Cuenta a la colectiva en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el listado de tipos de cuenta en el Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador del usuario</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con el catálogo de tipos de cuenta</returns>
        public static DataSet ListaParametrosCatalogo(int idColectiva, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposCuenta");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneTiposCuenta";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de cuenta en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros de tipo catálogo en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con el catálogo de tipos de cuenta</returns>
        public static DataSet ListaParamsCatalogo(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogosPMA");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCatalogosPMA";
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
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros tipo catálogo en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el listado de elementos de un catálogo en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdParametro">Identificador del parámetro multiasognación de tipo catálogo</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los elementos del catálogo</returns>
        public static DataSet ListaElementosCatalogo(int IdColectiva, long IdParametro, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneItemsCatalogoPMA");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
                database.AddInParameter(command, "@IdParametro", DbType.Int64, IdParametro);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneItemsCatalogoPMA";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@IdParametro=" + IdParametro);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los elementos del catálogo en base de datos.");
            }
        }

        /// <summary>
        /// Inserta un nuevo elemento al catálogo de parámetros multiasignación de la colectiva en base de datos
        /// </summary>
        /// <param name="idPMA">Idetnificador del parámetro multiasignación</param>
        /// <param name="idCol">Identificador de la colectiva</param>
        /// <param name="cve">Clave del nuevo elemento del catálogo</param>
        /// <param name="desc">Descripción del nuevo elemento del catálogo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje con la respuesta de base de datos a la creación del elemento</returns>
        public static string InsertaItemCatalogoPMAColectiva(long idPMA, int idCol, string cve, string desc, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaItemCatalogoPMA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdParametro", idPMA));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", idCol));
                        command.Parameters.Add(new SqlParameter("@Clave", cve));
                        command.Parameters.Add(new SqlParameter("@Descripcion", desc));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        var sqlPar = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlPar.Direction = ParameterDirection.Output;
                        sqlPar.Size = 200;
                        command.Parameters.Add(sqlPar);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string msj = sqlPar.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaItemCatalogoPMA";
                        logDebug.C_Value = "";
                        logDebug.R_Value = msj;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdParametro=" + idPMA);
                        parametros.Add("P2", "@IdColectiva=" + idCol);
                        parametros.Add("P3", "@Clave=" + cve);
                        parametros.Add("P4", "@Descripcion=" + desc);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG

                        return msj;
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el elemento al catálogo en base de datos.");
            }
        }

        /// <summary>
        /// Actualiza el estatus de un elemento del catálogo de parámetros multiasignación en base de datos
        /// </summary>
        /// <param name="IdCatalogo">Identificador del elemento del catálogo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusItemCatalogoPMA(int IdCatalogo, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActivaDesactivaItemCatalogoPMA", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorPreestablecidoPMA", IdCatalogo));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActivaDesactivaItemCatalogoPMA";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorPreestablecidoPMA=" + IdCatalogo);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatis del elemento del catálogo en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación sin asignar a la colectiva
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con lor parámetros</returns>
        public static DataSet ObtienePMASinAsignar(int idTipoPMA, int idColectiva, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMASinAsignarColectiva");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtienePMASinAsignarColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdColectiva=" + idColectiva);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros sin asignar de base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación asignados a la Colectiva, y que 
        /// pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idColectiva">Identificador de la Colectiva</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ObtieneParametrosMA(int idTipoPMA, int idColectiva, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMAColectiva");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosMAColectiva";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdColectiva=" + idColectiva);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros de base de datos");
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
        /// Inserta el parámetro a la colectiva en el Autorizador
        /// </summary>
        /// <param name="idParam">Identificador del parametro</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaValorParametroAdicional(int idParam, long idColectiva, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaValorPMAColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdParametroMultiasignacion", idParam));
                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaValorPMAColectiva";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdParametroMultiasignacion=" + idParam);
                        parametros.Add("P2", "@IdColectiva=" + idColectiva);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG

                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el Parámetro a la Colectiva en base de datos");
            }
        }
    }
}
