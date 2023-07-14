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

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Clase de acceso a datos para todo lo relacionado con Producto
    /// </summary>
    public class DAOInfoOnBoarding
    {
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
        public static DataSet ObtieneNodosPorClaveDesc(string KeyONombre, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNodos");

                database.AddInParameter(command, "@KeyONombre", DbType.String, KeyONombre);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneNodos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@KeyONombre=" + KeyONombre);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los Nodos en base de datos");
            }
        }

        /// <summary>
        /// Consulta los parámetros asociados al nodo con el ID indicado en base de datos
        /// </summary>
        /// <param name="idNodo">Identificador del Nodo</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneParametrosDeNodo(int idNodo, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosNodo");

                database.AddInParameter(command, "@IdNodo", DbType.Int32, idNodo);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneParametrosNodo";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdNodo=" + idNodo);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros del nodo en base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo nodo en base de datos
        /// </summary>
        /// <param name="unNodo">Datos del nuevo nodo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaNodo(Nodo unNodo, ParametroNodo unParam, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                string IDNuevoNodo, Mensaje;

                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaNuevoNodo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@NombreKey", unNodo.NombreKey));
                        command.Parameters.Add(new SqlParameter("@NombreNodoPadre", unNodo.NombreNodoPadre));
                        command.Parameters.Add(new SqlParameter("@ValorKeyParam", unParam.ValorKey));
                        command.Parameters.Add(new SqlParameter("@DescripcionValorParam", unParam.DescripcionValor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@IdNuevoNodo", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        IDNuevoNodo = sqlParameter1.Value.ToString();
                        Mensaje = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaNuevoNodo";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@IDNuevoNodo=" + IDNuevoNodo + "|@Mensaje=" + Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@NombreKey=" + unNodo.NombreKey);
                        parametros.Add("P2", "@NombreNodoPadre=" + unNodo.NombreNodoPadre);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevoNodo");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevoNodo"] = IDNuevoNodo;
                        dt.Rows[0]["Mensaje"] = Mensaje;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el nodo en base de datos");
            }
        }

        /// <summary>
        /// Actualiza un nodo en base de datos
        /// </summary>
        /// <param name="unNodo">Datos del nuevo nodo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>String con las respuestas del SP</returns>
        public static string ActualizaNodo(Nodo unNodo, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                string Mensaje;

                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaNodo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdScoreNodo", unNodo.ID_ScoreNodo));
                        command.Parameters.Add(new SqlParameter("@NombreKey", unNodo.NombreKey));
                        command.Parameters.Add(new SqlParameter("@NombreNodoPadre", unNodo.NombreNodoPadre));
                        command.Parameters.Add(new SqlParameter("@EsActivo", unNodo.EsActivo));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        sqlParameter1.Size = 200;
                        command.Parameters.Add(sqlParameter1);

                        conn.Open();
                        command.ExecuteNonQuery();

                        Mensaje = sqlParameter1.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaNodo";
                        logDBG.C_Value = "";
                        logDBG.R_Value = Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdScoreNodo=" + unNodo.ID_ScoreNodo);
                        parametros.Add("P2", "@NombreKey=" + unNodo.NombreKey);
                        parametros.Add("P3", "@NombreNodoPadre=" + unNodo.NombreNodoPadre);
                        parametros.Add("P4", "@EsActivo=" + unNodo.EsActivo);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return Mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el nodo en base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo nodo en base de datos
        /// </summary>
        /// <param name="unParametro">Datos del nuevo Parámetro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable InsertaParametro(ParametroNodo unParametro, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                string IDNuevoParametro, Mensaje;

                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaNuevoParametro", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdScoreNodo", unParametro.ID_ScoreNodo));
                        command.Parameters.Add(new SqlParameter("@ValorKey", unParametro.ValorKey));
                        command.Parameters.Add(new SqlParameter("@DescripcionValor", unParametro.DescripcionValor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@IdNuevoParametro", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        IDNuevoParametro = sqlParameter1.Value.ToString();
                        Mensaje = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaNuevoParametro";
                        logDBG.C_Value = "";
                        logDBG.R_Value = "@IDNuevoParametro=" + IDNuevoParametro + "|@Mensaje=" + Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdScoreNodo=" + unParametro.ID_ScoreNodo);
                        parametros.Add("P2", "@ValorKey=" + unParametro.ValorKey);
                        parametros.Add("P3", "@DescripcionValor=" + unParametro.DescripcionValor);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_NuevoParametro");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_NuevoParametro"] = IDNuevoParametro;
                        dt.Rows[0]["Mensaje"] = Mensaje;

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el parámetro en base de datos");
            }
        }

        /// <summary>
        /// Actualiza un Parámetro en base de datos
        /// </summary>
        /// <param name="idParametro">Datos del Parámetro</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>String con las respuestas del SP</returns>
        public static string ActualizaParametro(int idValorNodo, string keyParametro, string descParametro, bool esActivo, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                string Mensaje;

                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaParametro", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdValorNodo", idValorNodo));
                        command.Parameters.Add(new SqlParameter("@ValorKey", keyParametro));
                        command.Parameters.Add(new SqlParameter("@DescripcionValor", descParametro));
                        command.Parameters.Add(new SqlParameter("@EsActivoValor", esActivo));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        sqlParameter1.Size = 200;
                        command.Parameters.Add(sqlParameter1);

                        conn.Open();
                        command.ExecuteNonQuery();

                        Mensaje = sqlParameter1.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaParametro";
                        logDBG.C_Value = "";
                        logDBG.R_Value = Mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdValorNodo=" + idValorNodo);
                        parametros.Add("P2", "@ValorKey=" + keyParametro);
                        parametros.Add("P3", "@DescripcionValor=" + descParametro);
                        parametros.Add("P4", "@EsActivoValor=" + esActivo);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return Mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el parámetro en base de datos");
            }
        }
    }
}
