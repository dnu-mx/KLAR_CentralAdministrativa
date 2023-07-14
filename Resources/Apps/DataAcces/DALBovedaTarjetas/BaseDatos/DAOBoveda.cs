using DALAutorizador.BaseDatos;
using DALBovedaTarjetas.Entidades;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALBovedaTarjetas.BaseDatos
{
    public class DAOBoveda
    {
        /// <summary>
        /// Obtiene la lista de subemisores (GCM) del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ListaSubemisores(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSubemisoresBoveda");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneSubemisoresBoveda";
                logDbg.C_Value = "";
                logDbg.R_Value = "*************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los subemisores en base de datos.");
            }
        }

        /// <summary>
        /// Inserta el registro de una petición de alta de tarjetas en base de datos
        /// </summary>
        /// <param name="laPeticion">Datos de la petición</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static long InsertaPeticionTarjetas(PeticionAltaTarjetas laPeticion, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaPeticionAltaTarjeta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdProducto", laPeticion.ID_Producto));
                        command.Parameters.Add(new SqlParameter("@ClaveTipoPeticion", laPeticion.ClaveTipoPeticion));
                        command.Parameters.Add(new SqlParameter("@IdValorPreestablecido",
                            laPeticion.ID_ValorPrestablecido == -1 ? (int?)null : laPeticion.ID_ValorPrestablecido));
                        command.Parameters.Add(new SqlParameter("@Cantidad", laPeticion.Cantidad));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlPar = new SqlParameter("@IdPeticion", SqlDbType.BigInt);
                        sqlPar.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlPar);

                        conn.Open();

                        command.ExecuteNonQuery();

                        long idPeticion = Convert.ToInt64(sqlPar.Value);

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaPeticionAltaTarjeta";
                        logDBG.C_Value = "";
                        logDBG.R_Value = idPeticion.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + laPeticion.ID_Producto);
                        parametros.Add("P2", "@ClaveTipoPeticion=" + laPeticion.ClaveTipoPeticion);
                        parametros.Add("P3", "@IdValorPreestablecido=" + laPeticion.ID_ValorPrestablecido);
                        parametros.Add("P4", "@Cantidad=" + laPeticion.Cantidad);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return idPeticion;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la petición de alta en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el registro de una petición de alta de tarjetas en base de datos
        /// </summary>
        /// <param name="laPeticion">Datos de la petición por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaPeticionTarjetas(PeticionAltaTarjetas laPeticion, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaPeticionAltaTarjeta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPeticion", laPeticion.ID_Peticion));
                        command.Parameters.Add(new SqlParameter("@Archivo", laPeticion.NombreArchivo));
                        command.Parameters.Add(new SqlParameter("@NumLote", laPeticion.NumLote));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaPeticionAltaTarjeta";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPeticion=" + laPeticion.ID_Producto);
                        parametros.Add("P2", "@Archivo=" + laPeticion.NombreArchivo);
                        parametros.Add("P3", "@NumLote=" + laPeticion.NumLote);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveColectiva);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la petición de alta en base de datos");
            }
        }

        /// <summary>
        /// Consulta la ruta de entrada de los archivos de tarjetas stock dentro del Procesador Nocturno
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Cadena con la ruta de entrada</returns>
        public static string ObtieneRutaEntradaTarjetasStock(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            string value = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(BDProcesadorNocturno.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ObtieneRutaEntradaArchivos", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        var resp = command.ExecuteScalar();

                        if (resp != null)
                        {
                            value = resp.ToString();
                        }
                        value = Convert.ToString(command.ExecuteScalar());

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ObtieneRutaEntradaArchivos";
                        logDbg.C_Value = "";
                        logDbg.R_Value = value;

                        log.Debug(logDbg);
                        /////<<<LOG DEBUG

                        return value;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la ruta de entrada del procesamiento de tarjetas stock en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el valor de la vigencia de los plásticos del subemisor con el ID indicado dentro del Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static string ObtieneVigenciaPlastico(int idProducto, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            string vig = string.Empty;

            try
            {
                using (SqlConnection _conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand _command = new SqlCommand("web_CA_ObtieneVigPlasticoProducto", _conn))
                    {
                        _command.CommandType = CommandType.StoredProcedure;
                        _command.Parameters.Add((new SqlParameter("@IdProducto", idProducto)));
                        _command.Parameters.Add((new SqlParameter("@UserTemp", elUsuario.UsuarioTemp)));
                        _command.Parameters.Add((new SqlParameter("@AppId", AppID)));

                        _conn.Open();

                        var resp = _command.ExecuteScalar();

                        if (resp != null)
                        {
                            vig = resp.ToString();
                        }

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ObtieneVigPlasticoProducto";
                        logDbg.C_Value = "";
                        logDbg.R_Value = vig;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdProducto=" + idProducto.ToString());
                        parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                        parametros.Add("P3", "@AppId=" + AppID);
                        logDbg.Parameters = parametros;

                        log.Debug(logDbg);
                        /////<<<LOG DEBUG

                        return vig;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la vigencia de las tarjetas en base de datos.");
            }
        }

        /// <summary>
        /// Consulta la ruta de salida de los archivos de tarjetas stock dentro del Procesador Nocturno
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Cadena con la ruta de entrada</returns>
        public static string ObtieneRutaSalidaTarjetasStock(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            string value = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(BDProcesadorNocturno.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ObtieneRutaSalidaArchivos", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        var resp = command.ExecuteScalar();

                        if (resp != null)
                        {
                            value = resp.ToString();
                        }

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ObtieneRutaSalidaArchivos";
                        logDbg.C_Value = "";
                        logDbg.R_Value = value;

                        log.Debug(logDbg);
                        /////<<<LOG DEBUG

                        return value;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la ruta de salida del procesamiento de tarjetas stock en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene las peticiones de tarjetas stock registradas en la tabla de control del Autorizador
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static List<PeticionAltaTarjetas> ObtienePeticionesStock(DateTime fechaInicio, DateTime fechaFin, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            List<PeticionAltaTarjetas> lasPeticiones = new List<PeticionAltaTarjetas>();

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtienePeticionesStock";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@FechaInicial", SqlDbType.DateTime);
                        param.Value = fechaInicio;
                        cmd.Parameters.Add(param);
                        param = new SqlParameter("@FechaFinal", SqlDbType.DateTime);
                        param.Value = fechaFin;
                        cmd.Parameters.Add(param);
                        param = new SqlParameter("@UserTemp", SqlDbType.UniqueIdentifier);
                        param.Value = elUsuario.UsuarioTemp;
                        cmd.Parameters.Add(param);
                        param = new SqlParameter("@AppId", SqlDbType.UniqueIdentifier);
                        param.Value = AppID;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    PeticionAltaTarjetas peticion = new PeticionAltaTarjetas();

                                    peticion.ID_Peticion = (long)SqlReader["ID_PeticionAltaTarjetas"];
                                    peticion.FechaSolicitud = SqlReader["FechaSolicitud"] == DBNull.Value ?
                                        (DateTime?)null : Convert.ToDateTime(SqlReader["FechaSolicitud"]);
                                    peticion.NumLote = SqlReader["Lote"].ToString();
                                    peticion.Cantidad = (int)SqlReader["Cantidad"];
                                    peticion.EstatusPeticion = SqlReader["EstatusPeticion"].ToString();
                                    peticion.Emisor = SqlReader["Emisor"].ToString();
                                    peticion.Producto = SqlReader["Producto"].ToString();
                                    peticion.TipoManufactura = SqlReader["TipoManufactura"].ToString();
                                    peticion.NombreArchivo = SqlReader["NombreFichero"].ToString();

                                    lasPeticiones.Add(peticion);
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtienePeticionesStock";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@FechaInicial=" + fechaInicio.ToShortDateString());
                                parametros.Add("P2", "@FechaFinal=" + fechaFin.ToShortDateString());
                                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                                parametros.Add("P4", "@AppId=" + AppID);
                                logDBG.Parameters = parametros;

                                log.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            log.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los datos de las peticiones en base de datos.");
                        }
                    }
                }

                return lasPeticiones;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las solicitudes de tarjetas stock en base de datos.");
            }
        }
    }
}
