using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Entidades;
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

namespace DALPuntoVentaWeb.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Compensaciones
    /// </summary>
    public class DAOCompensaciones
    {
        /// <summary>
        /// Consulta los bines que corresponden al SubEmisor con el identificador indicado
        /// dentro del Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva (SubEmisor)</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Cadena con el resultado de la consulta</returns>
        public static string ObtieneBinesCliente(Int64 IdColectiva, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneBinesCliente");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, IdColectiva);
                
                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                string s = string.Join(" ", dt.Rows.OfType<DataRow>().Select(x =>
                        string.Join(" ; ", x.ItemArray))).Replace(" ", ";");

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneBinesCliente";
                logDebug.C_Value = "";
                logDebug.R_Value = s;

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                if (dt.Rows.Count > 1)
                {
                    return s.Remove(s.Length - 1, 1) + "";
                }
                else
                {
                    return s;
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los bines del cliente en base de datos.");
            }
        }

        /// <summary>
        /// Consulta las transacciones en los ficheros T112 que coinciden con los filtros indicados
        /// </summary>
        /// <param name="bines">Cadena con los bines del SubEmisor</param>
        /// <param name="fechaInicio">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneTXsFicherosT112(String bines, DateTime fechaInicio, DateTime fechaFin,
            String numTarjeta, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneOperacionesEnFichero");

                database.AddInParameter(command, "@BinesCliente", DbType.String, bines);
                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@Tarjeta", DbType.String, 
                    String.IsNullOrEmpty(numTarjeta) ? (String)null: numTarjeta);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneOperacionesEnFichero";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@BinesCliente=" + bines);
                parametros.Add("P2", "@FechaInicial=" + fechaInicio.ToString());
                parametros.Add("P3", "@FechaFinal=" + fechaFin.ToString());
                parametros.Add("P4", "@Tarjeta=" + (String.IsNullOrEmpty(numTarjeta) ? numTarjeta :
                    MaskSensitiveData.cardNumber(numTarjeta)));
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los ficheros T112 en base de datos.");
            }
        }

        /// <summary>
        /// Consulta el número de operaciones en el Autorizador que son candidatas a compensación T112,
        /// según los filtros indicados
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="numAutorizacion">Número de autorización</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static int ObtieneNumOperacionesT112PorCompensar(String numTarjeta, String numAutorizacion,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ObtieneNumOperacionesCompT112", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add((new SqlParameter("@NumTarjeta", numTarjeta)));
                        command.Parameters.Add((new SqlParameter("@NumAutorizacion", numAutorizacion)));

                        conn.Open();

                        int resp = Convert.ToInt32(command.ExecuteScalar());

                        /////>>>LOG DEBUG
                        LogDebugMsg logDb = new LogDebugMsg();
                        logDb.M_Value = "web_CA_ObtieneNumOperacionesCompT112";
                        logDb.C_Value = "";
                        logDb.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@NumTarjeta=" + MaskSensitiveData.cardNumber(numTarjeta));
                        parametros.Add("P2", "@NumAutorizacion=" + numAutorizacion);
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
                throw new CAppException(8010, "Ocurrió un error al consultar el número de operaciones por compensar" +
                    " en base de datos.");
            }
        }

        /// <summary>
        /// Consulta las operaciones en el Autorizador que son candidatas a compensación T112, según los
        /// filtros indicados
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="fechaInicio">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneOperacionesT112PorCompensar(String numTarjeta, DateTime fechaInicio,
            DateTime fechaFin, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneOperacionesCompT112");

                database.AddInParameter(command, "@NumTarjeta", DbType.String, numTarjeta);
                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];
                
                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneOperacionesCompT112";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@NumTarjeta=" + MaskSensitiveData.cardNumber(numTarjeta));
                parametros.Add("P2", "@FechaInicial=" + fechaInicio);
                parametros.Add("P3", "@FechaFinal=" + fechaFin);
                parametros.Add("P4", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P5", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las operaciones por compensar " +
                    "de base de datos.");
            }
        }

        /// <summary>
        /// Consulta el nuevo identificador de estatus de compensación para la operación con el ID
        /// indicado en el Autorizador
        /// </summary>
        /// <param name="idOperacion">Identificador de la operación</param>
        /// <param name="codigoTX">Código o clave de la operación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>ID del identificador de estatus</returns>
        public static int ObtieneNuevoIdEstatusCompensacion(Int64 idOperacion, String codigoTX, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ObtieneNuevoEstatusCompDeOperacion", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add((new SqlParameter("@IdOperacion", idOperacion)));
                        command.Parameters.Add((new SqlParameter("@ClaveOperacion", codigoTX)));

                        conn.Open();

                        int resp = command.ExecuteScalar() == DBNull.Value ? 0 :
                            Convert.ToInt32(command.ExecuteScalar());

                        /////>>>LOG DEBUG
                        LogDebugMsg logDb = new LogDebugMsg();
                        logDb.M_Value = "web_CA_ObtieneNuevoEstatusCompDeOperacion";
                        logDb.C_Value = "";
                        logDb.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdOperacion=" + idOperacion);
                        parametros.Add("P2", "@ClaveOperacion=" + codigoTX);
                        logDb.Parameters = parametros;

                        pCI.Debug(logDb);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el nuevo estatus de la operación " +
                    "en base de datos.");
            }
        }

        /// <summary>
        /// Consulta los datos de la transacción con el ID fichero detalle indicado, necesarios
        /// para el registro en la operación del Autorizador
        /// </summary>
        /// <param name="idFicheroDet">Identificador del fichero detalle</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Entidad Operacion con los datos de la transacción</returns>
        public static Operacion ObtieneDatosTX_T112(Int64 idFicheroDet, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            Operacion unaOperacion = new Operacion();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDT112.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneDatosCompensacionDeOperacion";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdFicheroDetalle", SqlDbType.BigInt);
                        param.Value = idFicheroDet;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    unaOperacion.ImporteCompensadoPesos = (decimal)SqlReader["ImporteCompensadoPesos"];
                                    unaOperacion.ImporteCompensadoDolar = (decimal)SqlReader["ImporteCompensadoDolar"];
                                    unaOperacion.ImporteCompensadoLocal = (decimal)SqlReader["ImporteCompensadoLocal"];
                                    unaOperacion.CodigoMonedaLocal = SqlReader["CodigoMonedaLocal"].ToString();
                                    unaOperacion.CuotaIntercambio = (decimal)SqlReader["CuotaIntercambio"];
                                    unaOperacion.FechaPresentacion = (DateTime)SqlReader["FechaPresentacion"];
                                    unaOperacion.NombreArchivo = SqlReader["NombreArchivo"].ToString();
                                    unaOperacion.CodigoTx = SqlReader["CodigoTx"].ToString();
                                    unaOperacion.Comercio = SqlReader["Comercio"].ToString();
                                    unaOperacion.Ciudad = SqlReader["Ciudad"].ToString();
                                    unaOperacion.Pais = SqlReader["Pais"].ToString();
                                    unaOperacion.MCC = SqlReader["MCC"].ToString();
                                    unaOperacion.Moneda1 = SqlReader["Moneda1"].ToString();
                                    unaOperacion.Moneda2 = SqlReader["Moneda2"].ToString();
                                    unaOperacion.Referencia = SqlReader["Referencia"].ToString();
                                    unaOperacion.FechaJuliana = SqlReader["FechaJuliana"].ToString();
                                    unaOperacion.FechaConsumo = SqlReader["FechaConsumo"].ToString();
                                    unaOperacion.Ciclo = SqlReader["Ciclo"].ToString();
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneDatosCompensacionDeOperacion";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdFicheroDetalle=" + idFicheroDet);
                                logDBG.Parameters = parametros;

                                log.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            log.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los datos de la operación de base de datos.");
                        }
                    }

                    return unaOperacion;
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los datos de la operación de base de datos.");
            }
        }

        /// <summary>
        /// Actualiza los campos T112 de la operación con los correspondientes a la compensación e inserta un
        /// registro de compensación manual en el Autorizador
        /// </summary>
        /// <param name="laOperacion">Datos de la operación a actualizar</param>
        /// <param name="IdFicheroDetalle">Identificador del fichero detalle</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaOperacionCompensada_T112(Operacion laOperacion, Int64 IdFicheroDetalle,
            SqlConnection connection, SqlTransaction transaccionSQL, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_InsertaActualizaOperacionCompensadaT112", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdOperacion", laOperacion.ID_Operacion));
                command.Parameters.Add(new SqlParameter("@ImporteCompensadoPesos", laOperacion.ImporteCompensadoPesos));
                command.Parameters.Add(new SqlParameter("@ImporteCompensadoDolar", laOperacion.ImporteCompensadoDolar));
                command.Parameters.Add(new SqlParameter("@ImporteCompensadoLocal", laOperacion.ImporteCompensadoLocal));
                command.Parameters.Add(new SqlParameter("@CodigoMonedaLocal", laOperacion.CodigoMonedaLocal));
                command.Parameters.Add(new SqlParameter("@CuotaIntercambio", laOperacion.CuotaIntercambio));
                command.Parameters.Add(new SqlParameter("@FechaPresentacion", laOperacion.FechaPresentacion));
                command.Parameters.Add(new SqlParameter("@NombreArchivo", laOperacion.NombreArchivo));
                command.Parameters.Add(new SqlParameter("@CodigoTx", laOperacion.CodigoTx));
                command.Parameters.Add(new SqlParameter("@Comercio", laOperacion.Comercio));
                command.Parameters.Add(new SqlParameter("@Ciudad", laOperacion.Ciudad));
                command.Parameters.Add(new SqlParameter("@Pais", laOperacion.Pais));
                command.Parameters.Add(new SqlParameter("@MCC", laOperacion.MCC));
                command.Parameters.Add(new SqlParameter("@Moneda1", laOperacion.Moneda1));
                command.Parameters.Add(new SqlParameter("@Moneda2", laOperacion.Moneda2));
                command.Parameters.Add(new SqlParameter("@Referencia", laOperacion.Referencia));
                command.Parameters.Add(new SqlParameter("@FechaJuliana", laOperacion.FechaJuliana));
                command.Parameters.Add(new SqlParameter("@FechaConsumo", laOperacion.FechaConsumo));
                command.Parameters.Add(new SqlParameter("@Ciclo", laOperacion.Ciclo));
                command.Parameters.Add(new SqlParameter("@IdFicheroDetalle", IdFicheroDetalle));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_InsertaActualizaOperacionCompensadaT112";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdOperacion=" + laOperacion.ID_Operacion);
                parametros.Add("P2", "@ImporteCompensadoPesos=" + laOperacion.ImporteCompensadoPesos);
                parametros.Add("P3", "@ImporteCompensadoDolar=" + laOperacion.ImporteCompensadoDolar);
                parametros.Add("P4", "@ImporteCompensadoLocal=" + laOperacion.ImporteCompensadoLocal);
                parametros.Add("P5", "@CodigoMonedaLocal=" + laOperacion.CodigoMonedaLocal);
                parametros.Add("P6", "@CuotaIntercambio=" + laOperacion.CuotaIntercambio);
                parametros.Add("P7", "@FechaPresentacion=" + laOperacion.FechaPresentacion);
                parametros.Add("P8", "@NombreArchivo=" + laOperacion.NombreArchivo);
                parametros.Add("P9", "@CodigoTx=" + laOperacion.CodigoTx);
                parametros.Add("P10", "@Comercio=" + laOperacion.Comercio);
                parametros.Add("P11", "@Ciudad=" + laOperacion.Ciudad);
                parametros.Add("P12", "@Pais=" + laOperacion.Pais);
                parametros.Add("P13", "@MCC=" + laOperacion.MCC);
                parametros.Add("P14", "@Moneda1=" + laOperacion.Moneda1);
                parametros.Add("P15", "@Moneda2=" + laOperacion.Moneda2);
                parametros.Add("P16", "@Referencia=" + laOperacion.Referencia);
                parametros.Add("P17", "@FechaJuliana=" + laOperacion.FechaJuliana);
                parametros.Add("P18", "@FechaConsumo=" + laOperacion.FechaConsumo);
                parametros.Add("P19", "@Ciclo=" + laOperacion.Ciclo);
                parametros.Add("P20", "@IdFicheroDetalle=" + IdFicheroDetalle);
                parametros.Add("P21", "@Usuario=" + elUsuario.ClaveUsuario);
                logDebug.Parameters = parametros;

                log.Debug(logDebug);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al actualizar los datos T112 de la operación en base de datos.");
            }
        }

        /// <summary>
        /// Actualiza el estatus de compensacion de una transacción de los ficheros T112
        /// </summary>
        /// <param name="idFicheroDetalle">Identificador del fichero detalle</param>
        /// <param name="idEstatusComp">Identificador del estatus a actualizar</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusCompensacionTX_T112(Int64 idFicheroDetalle, int idEstatusComp,
            SqlConnection connection, SqlTransaction transaccionSQL, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusCompensacionFicheroDetalle", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdFicheroDetalle", idFicheroDetalle));
                command.Parameters.Add(new SqlParameter("@IdEstatusCompensacion", idEstatusComp));

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ActualizaEstatusCompensacionFicheroDetalle";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdFicheroDetalle=" + idFicheroDetalle);
                parametros.Add("P2", "@IdEstatusCompensacion=" + idEstatusComp);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Error al actualizar el estatus de compensación del fichero detalle " +
                    "en base de datos.");
            }
        }

        /// <summary>
        /// Consulta la carga de archivos en T112, según los filtros indicados
        /// </summary>
        /// <param name="ArchivosPendientes">Número de tarjeta</param>
        /// <param name="FechaPresentacion">Fecha inicial del periodo de consulta</param>
        /// <param name="Marca">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneCargaArchivosT112(bool ArchivosPendientes, DateTime FechaPresentacion,
            string Marca, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCargaArchivos");

                database.AddInParameter(command, "@ArchivosPendientes", DbType.Boolean, ArchivosPendientes);
                database.AddInParameter(command, "@Fecha", DbType.Date, ArchivosPendientes ? (DateTime?)null : FechaPresentacion);
                database.AddInParameter(command, "@Marca", DbType.String, string.IsNullOrEmpty(Marca) ? null : Marca);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneCargaArchivos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ArchivosPendientes=" + ArchivosPendientes);
                parametros.Add("P2", "@Fecha=" + (ArchivosPendientes ? "NULL" : FechaPresentacion.ToShortDateString()));
                parametros.Add("P3", "@Marca=" + Marca);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la carga de archivos en base de datos.");
            }
        }

        /// <summary>
        /// Consulta la homologación de registros en T112, según los filtros indicados
        /// </summary>
        /// <param name="ArchivosPendientes">Número de tarjeta</param>
        /// <param name="FechaPresentacion">Fecha inicial del periodo de consulta</param>
        /// <param name="Marca">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneHomologacionRegistrosT112(bool ArchivosPendientes, DateTime FechaPresentacion,
            string Marca, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneHomologacionRegistros");

                database.AddInParameter(command, "@ArchivosPendientes", DbType.Boolean, ArchivosPendientes);
                database.AddInParameter(command, "@Fecha", DbType.Date, ArchivosPendientes ? (DateTime?)null : FechaPresentacion);
                database.AddInParameter(command, "@Marca", DbType.String, string.IsNullOrEmpty(Marca) ? null : Marca);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneHomologacionRegistros";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ArchivosPendientes=" + ArchivosPendientes);
                parametros.Add("P2", "@Fecha=" + (ArchivosPendientes ? "NULL" : FechaPresentacion.ToShortDateString()));
                parametros.Add("P3", "@Marca=" + Marca);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la homologación de registros en base de datos.");
            }
        }

        /// <summary>
        /// Consulta la homologación de registros en T112, según los filtros indicados
        /// </summary>
        /// <param name="ArchivosPendientes">Número de tarjeta</param>
        /// <param name="FechaPresentacion">Fecha inicial del periodo de consulta</param>
        /// <param name="Marca">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneCompensacionRegistrosT112(bool ArchivosPendientes, DateTime FechaPresentacion,
            string Marca, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCompensacionRegistros");

                database.AddInParameter(command, "@ArchivosPendientes", DbType.Boolean, ArchivosPendientes);
                database.AddInParameter(command, "@Fecha", DbType.Date, ArchivosPendientes ? (DateTime?)null : FechaPresentacion);
                database.AddInParameter(command, "@Marca", DbType.String, string.IsNullOrEmpty(Marca) ? null : Marca);
                
                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneCompensacionRegistros";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ArchivosPendientes=" + ArchivosPendientes);
                parametros.Add("P2", "@Fecha=" + (ArchivosPendientes ? "NULL" : FechaPresentacion.ToShortDateString()));
                parametros.Add("P3", "@Marca=" + Marca);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la compensación de registros en base de datos.");
            }
        }

        /// <summary>
        /// Consulta el detalle de los registros inválidos durante la homologación en T112,
        /// según el identificador de fichero temporal indicado
        /// </summary>
        /// <param name="id">Identificador del fichero temporal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneDetalleRegistrosInvalidos(long id, IUsuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDetalleRegistrosInvalidos");

                database.AddInParameter(command, "@IdFicheroTemp", DbType.Int64, id);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDetalleRegistrosInvalidos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdFicheroTemp=" + id.ToString());
                parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                parametros.Add("P3", "@AppId=" + AppID.ToString());
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el detalle de registros inválidos en base de datos.");
            }
        }

        /// <summary>
        /// Consulta el detalle de los registros erróneos durante la compensación en T112,
        /// según el identificador de fichero temporal indicado
        /// </summary>
        /// <param name="id">Identificador del fichero</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneDetalleRegistrosErroneos(long id, IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDetalleRegistrosError");

                database.AddInParameter(command, "@IdFichero", DbType.Int64, id);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDetalleRegistrosError";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdFichero=" + id.ToString());
                parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                parametros.Add("P3", "@AppId=" + AppID.ToString());
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el detalle de registros erróneos en base de datos.");
            }
        }

        /// <summary>
        /// Consulta los registros de compensación que cumplen los filtros indicados en Autorizador
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="claveMA">Número de tarjeta</param>
        /// <param name="referencia">Referencia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneCompensacionRegistrosAut(DateTime fechaInicial, DateTime fechaFinal, string claveMA,
            string referencia, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRegistrosCompensacion");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime,
                   fechaFinal == DateTime.MinValue ? (DateTime?)null : fechaFinal);

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@ClaveMA";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(claveMA) ? null : claveMA;
                param.Size = claveMA.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@Referencia", DbType.String,
                    String.IsNullOrEmpty(referencia) ? (string)null : referencia);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneRegistrosCompensacion";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + fechaInicial.ToShortDateString());
                parametros.Add("P2", "@FechaFinal=" + fechaFinal.ToShortDateString());
                parametros.Add("P3", "@ClaveMA=" + (string.IsNullOrEmpty(claveMA) ? "" : MaskSensitiveData.cardNumber(claveMA)));
                parametros.Add("P4", "@Referencia=" + referencia);
                parametros.Add("P5", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P6", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los registros de compensación en base de datos.");
            }
        }

        /// <summary>
        /// Consulta las operaciones relacionadas a la tarjeta indicada en Autorizador
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneOperacionesDeTarjeta(string tarjeta, Usuario elUsuario, Guid AppID, 
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneOperacionesRegistrosCompensacion");
                command.CommandTimeout = 0;

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@ClaveMA";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = tarjeta;
                param.Size = tarjeta.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneOperacionesRegistrosCompensacion";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ClaveMA=" + MaskSensitiveData.cardNumber(tarjeta));
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las operaciones en base de datos.");
            }
        }

        /// <summary>
        /// Inserta una relación de compensación sin operación en el Autorizador
        /// </summary>
        /// <param name="IdRegComp">Identificador del registro de compensación</param>
        /// <param name="IdOperacion">Identificador de la operación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaRelacionCompensacionOperacion(long IdRegComp, long IdOperacion,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaRelacionCSO", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdRegistroCompensacion", IdRegComp));
                        command.Parameters.Add(new SqlParameter("@IdOperacion", IdOperacion));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaRelacionCSO";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdRegistroCompensacion=" + IdRegComp.ToString());
                        parametros.Add("P2", "@IdOperacion=" + IdOperacion.ToString());
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
                throw new CAppException(8010, "Ocurrió un error al insertar la relación de CSO en base de datos");
            }
        }

        /// <summary>
        /// Consulta la relación de compensaciones sin operación que cumplen los filtros indicados en Autorizador
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="claveMA">Número de tarjeta</param>
        /// <param name="referencia">Referencia</param>
        /// <param name="idEstatus">Identificador del estatus de relación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneRelacionesCompensacionSinOperacion(DateTime fechaInicial, DateTime fechaFinal,
            string claveMA, string referencia, int idEstatus, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRelacionesRegistrosCompensacion");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime,
                   fechaFinal == DateTime.MinValue ? (DateTime?)null : fechaFinal);

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@ClaveMA";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(claveMA) ? null : claveMA;
                param.Size = claveMA.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@Referencia", DbType.String,
                    string.IsNullOrEmpty(referencia) ? (string)null : referencia);
                database.AddInParameter(command, "@IdEstatusRelacionCSO", DbType.Int32, idEstatus);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneRelacionesRegistrosCompensacion";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + fechaInicial.ToShortDateString());
                parametros.Add("P2", "@FechaFinal=" + fechaFinal.ToShortDateString());
                parametros.Add("P3", "@ClaveMA=" + (string.IsNullOrEmpty(claveMA) ? "" : MaskSensitiveData.cardNumber(claveMA)));
                parametros.Add("P4", "@Referencia=" + referencia);
                parametros.Add("P5", "@IdEstatusRelacionCSO=" + idEstatus.ToString());
                parametros.Add("P6", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P7", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la relación de compensaciones sin operación en base de datos.");
            }
        }

        /// <summary>
        /// Consulta el catálogo de estatus de relaciones de compensaciones sin operación dentro del Autorizador
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneEstatusRelacionCSO(ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusRelacionCSO");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneEstatusRelacionCSO";
                logDebug.C_Value = "";
                logDebug.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus de relaciones de " +
                    "compensación sin operación en base de datos.");
            }
        }

        /// <summary>
        /// Consulta los detalles de la operación con el identificador indicado en el Autorizador
        /// </summary>
        /// <param name="idOperacion">Identificador de la operación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el resultado de la consulta</returns>
        public static DataTable ObtieneOperacionRelacionCSO(long idOperacion, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneOperacionRelacionCSO");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdOperacion", DbType.Int64, idOperacion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneOperacionRelacionCSO";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdOperacion=" + idOperacion.ToString());
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los detalles de la operación relacionada en base de datos.");
            }
        }

        /// <summary>
        /// Actualiza el estatus de una relación de compensación sin operación en el Autorizador
        /// </summary>
        /// <param name="IdRelacionCSO">Identificador de la relación de compensación sin operación</param>
        /// <param name="Motivo">Motivo de la cancelación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaRelacionCompensacionOperacion(long IdRelacionCSO, string Motivo, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaRelacionCSO", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdRelacionCSO", IdRelacionCSO));
                        command.Parameters.Add(new SqlParameter("@Motivo", Motivo));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaRelacionCSO";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdRelacionCSO=" + IdRelacionCSO.ToString());
                        parametros.Add("P2", "@Motivo=" + Motivo);
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
                throw new CAppException(8010, "Ocurrió un error al actualizar la relación de CSO en base de datos");
            }
        }
    }
}
