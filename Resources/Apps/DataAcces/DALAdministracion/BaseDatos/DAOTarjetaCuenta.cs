using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
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

namespace DALAdministracion.BaseDatos
{
    public class DAOTarjetaCuenta
    {
        /// <summary>
        /// Consulta "el catálogo" de relaciones tarjeta/cuenta en base de datos
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        /// <param name="nombre">Nombre del tarjetahabiente</param>
        /// <param name="apPat">Apellido paterno del tarjetahabiente</param>
        /// <param name="apMat">Apellido materno del tarjetahabiente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoTarjetaCuenta(string tarjeta, string nombre, string apPat, string apMat, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerCatalogoTarjetaCuenta");

                database.AddInParameter(command, "@NumTarjeta", DbType.String, tarjeta);
                database.AddInParameter(command, "@ApPaterno", DbType.String, apPat);
                database.AddInParameter(command, "@ApMaterno", DbType.String, apMat);
                database.AddInParameter(command, "@Nombre", DbType.String, nombre);
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
        /// Consulta las cuentas que coinciden con los filtros indicados en el Autorizador
        /// </summary>
        /// <param name="datosCuenta">Parámetros de búsqueda de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCuentasPorFiltro(DatosPersonalesCuenta datosCuenta, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasPorFiltro");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdCuenta", DbType.Int32, datosCuenta.ID_Cuenta);
                database.AddInParameter(command, "@Nombre", DbType.String, datosCuenta.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, datosCuenta.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, datosCuenta.ApellidoMaterno);
                
                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@NumeroTarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ? null : datosCuenta.TarjetaTitular;
                param.Size = datosCuenta.TarjetaTitular.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@EsAdicional", DbType.Boolean, datosCuenta.SoloAdicionales);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCuentasPorFiltro";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdCuenta=" + datosCuenta.ID_Cuenta);
                parametros.Add("P2", "@Nombre=" + datosCuenta.Nombre);
                parametros.Add("P3", "@ApPaterno=" + datosCuenta.ApellidoPaterno);
                parametros.Add("P4", "@ApMaterno=" + datosCuenta.ApellidoMaterno);
                parametros.Add("P5", "@NumeroTarjeta=" + (string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ?
                    datosCuenta.TarjetaTitular : datosCuenta.TarjetaTitular.Length < 16 ?
                    "******" : MaskSensitiveData.cardNumber(datosCuenta.TarjetaTitular)));
                parametros.Add("P6", "@EsAdicional=" + datosCuenta.SoloAdicionales);
                parametros.Add("P7", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P8", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG
                
                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las cuentas de base de datos");
            }
        }

        /// <summary>
        /// Consulta los datos personales de la cuenta seleccionada en base de datos
        /// </summary>
        /// <param name="idTarj">Identificador de la tarjeta</param>
        /// <param name="idCol">Identificador de la colectiva</param>
        /// <param name="embozo">Referencia a la variable donde se creará el nuevo token</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DatosPersonalesCuenta ObtieneDatosPersonalesCuenta(int idTarj, int idCol, ref string embozo,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            DatosPersonalesCuenta datos = new DatosPersonalesCuenta();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneDatosPersonalesCuenta";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdTarjeta", SqlDbType.Int);
                        param.Value = idTarj;
                        cmd.Parameters.Add(param);

                        param = new SqlParameter("@IdColectiva", SqlDbType.Int);
                        param.Value = idCol;
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
                                    datos.ID_Colectiva = (int)SqlReader["ID_Colectiva"];
                                    datos.TarjetaTitular = SqlReader["NumeroTarjeta"].ToString();
                                    datos.Nombre = SqlReader["Nombre"].ToString();
                                    datos.ApellidoPaterno = SqlReader["ApPaterno"].ToString();
                                    datos.ApellidoMaterno = SqlReader["ApMaterno"].ToString();
                                    datos.RFC = SqlReader["RFC"].ToString();
                                    datos.IdDireccion = (int)SqlReader["ID_Direccion"];
                                    datos.Calle = SqlReader["Calle"].ToString();
                                    datos.NumExterior = SqlReader["NumExterior"].ToString();
                                    datos.NumInterior = SqlReader["NumInterior"].ToString();
                                    datos.EntreCalles = SqlReader["EntreCalles"].ToString();
                                    datos.Referencias = SqlReader["Referencias"].ToString();
                                    datos.CodigoPostal = SqlReader["CodigoPostal"].ToString();
                                    datos.IdColonia = (int)SqlReader["ID_Colonia"];
                                    datos.Colonia = SqlReader["Colonia"].ToString();
                                    datos.ClaveMunicipio = SqlReader["ClaveMunicipio"].ToString();
                                    datos.Municipio = SqlReader["Municipio"].ToString();
                                    datos.ClaveEstado = SqlReader["ClaveEstado"].ToString();
                                    datos.Estado = SqlReader["Estado"].ToString();
                                    datos.NumTelParticular = SqlReader["TelParticular"].ToString();
                                    datos.NumTelCelular = SqlReader["TelCelular"].ToString();
                                    datos.NumTelTrabajo = SqlReader["TelTrabajo"].ToString();
                                    datos.Email = SqlReader["Email"].ToString();
                                    datos.Enmascara = SqlReader["Enmascara"].ToString();
                                    embozo = SqlReader["NombreEmbozo"].ToString();
                                    datos.CURP = SqlReader["CURP"].ToString();
                                    datos.FechaNacimiento = Convert.ToDateTime(SqlReader["FechaNacimiento"]);

                                    datos.IdDireccionFiscal = (int)SqlReader["ID_DireccionFiscal"];
                                    datos.CalleFiscal = SqlReader["CalleFiscal"].ToString();
                                    datos.NumExteriorFiscal = SqlReader["NumExteriorFiscal"].ToString();
                                    datos.NumInteriorFiscal = SqlReader["NumInteriorFiscal"].ToString();
                                    datos.CodigoPostalFiscal = SqlReader["CodigoPostalFiscal"].ToString();
                                    datos.IdColoniaFiscal = (int)SqlReader["ID_ColoniaFiscal"];
                                    datos.ColoniaFiscal = SqlReader["ColoniaFiscal"].ToString();
                                    datos.ClaveMunicipioFiscal = SqlReader["ClaveMunicipioFiscal"].ToString();
                                    datos.MunicipioFiscal = SqlReader["MunicipioFiscal"].ToString();
                                    datos.ClaveEstadoFiscal = SqlReader["ClaveEstadoFiscal"].ToString();
                                    datos.EstadoFiscal = SqlReader["EstadoFiscal"].ToString();
                                    datos.IdRegimenFiscal = (int)SqlReader["ID_RegimenFiscal"];
                                    datos.ClaveRegimenFiscal = SqlReader["ClaveRegimenFiscal"].ToString();
                                    datos.RegimenFiscal = SqlReader["RegimenFiscal"].ToString();
                                    datos.IdUsoCFDI = (int)SqlReader["ID_UsoCFDI"];
                                    datos.ClaveUsoCFDI = SqlReader["ClaveUsoCFDI"].ToString();
                                    datos.UsoCFDI = SqlReader["UsoCFDI"].ToString();
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneDatosPersonalesCuenta";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdTarjeta=" + idTarj);
                                parametros.Add("P2", "@IdColectiva=" + idCol);
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
                            throw new CAppException(8010, "Error al extraer los datos personales de la cuenta en base de datos.");
                        }
                    }

                    return datos;
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los datos personales de la cuenta en de base de datos.");
            }
        }

        /// <summary>
        /// Obtiene los datos de colonia, municipio y estado a partir del código postal indicado.
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaDatosPorCodigoPostal(string codigoPostal, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneColMpioEdoPorCP");

                database.AddInParameter(command, "@CP", DbType.String, codigoPostal);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_GMA_ObtieneColMpioEdoPorCP";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@CP=" + codigoPostal);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las colonias del Código Postal en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los datos de colonia, municipio, ciudad y estado a partir del código postal indicado.
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaDatosPorCodigoPostal_V2(string codigoPostal, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColMpioCdEdoPorCP");

                database.AddInParameter(command, "@CP", DbType.String, codigoPostal);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneColMpioCdEdoPorCP";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@CP=" + codigoPostal);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los datos por código postal de base de datos");
            }
        }

        /// <summary>
        /// Actualiza los datos personales de la cuenta en base de datos
        /// </summary>
        /// <param name="datosCuenta">Datos de la cuenta a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDatosPersonalesCuenta(DatosPersonalesCuenta datosCuenta, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaInfoCuentahabiente", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTitular", datosCuenta.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@Nombre", datosCuenta.Nombre));
                        command.Parameters.Add(new SqlParameter("@ApPaterno", datosCuenta.ApellidoPaterno));
                        command.Parameters.Add(new SqlParameter("@ApMaterno", datosCuenta.ApellidoMaterno));
                        command.Parameters.Add(new SqlParameter("@RFC", datosCuenta.RFC));
                        command.Parameters.Add(new SqlParameter("@FechaNacimiento", 
                            datosCuenta.FechaNacimiento.Equals(DateTime.MinValue) ? (DateTime?)null : datosCuenta.FechaNacimiento));
                        command.Parameters.Add(new SqlParameter("@CURP", datosCuenta.CURP));

                        command.Parameters.Add(new SqlParameter("@IdDireccion", datosCuenta.IdDireccion));
                        command.Parameters.Add(new SqlParameter("@Calle", datosCuenta.Calle));
                        command.Parameters.Add(new SqlParameter("@NumExterior", datosCuenta.NumExterior));
                        command.Parameters.Add(new SqlParameter("@NumInterior", datosCuenta.NumInterior));
                        command.Parameters.Add(new SqlParameter("@EntreCalles", datosCuenta.EntreCalles));
                        command.Parameters.Add(new SqlParameter("@Referencias", datosCuenta.Referencias));
                        command.Parameters.Add(new SqlParameter("@IdColonia", datosCuenta.IdColonia));

                        command.Parameters.Add(new SqlParameter("@TelParticular", datosCuenta.NumTelParticular));
                        command.Parameters.Add(new SqlParameter("@TelCelular", datosCuenta.NumTelCelular));
                        command.Parameters.Add(new SqlParameter("@TelTrabajo", 
                            string.IsNullOrEmpty(datosCuenta.NumTelTrabajo) ? null : datosCuenta.NumTelTrabajo));
                        command.Parameters.Add(new SqlParameter("@eMail", datosCuenta.Email));

                        command.Parameters.Add(new SqlParameter("@IdDireccionFiscal", datosCuenta.IdDireccionFiscal));
                        command.Parameters.Add(new SqlParameter("@CalleFiscal", datosCuenta.CalleFiscal));
                        command.Parameters.Add(new SqlParameter("@NumExtFiscal", datosCuenta.NumExteriorFiscal));
                        command.Parameters.Add(new SqlParameter("@NumIntFiscal", datosCuenta.NumInteriorFiscal));
                        command.Parameters.Add(new SqlParameter("@IdColoniaFiscal", datosCuenta.IdColoniaFiscal));
                        command.Parameters.Add(new SqlParameter("@IdRegimenFiscal", datosCuenta.IdRegimenFiscal));
                        command.Parameters.Add(new SqlParameter("@RegimenFiscal", datosCuenta.ClaveRegimenFiscal));
                        command.Parameters.Add(new SqlParameter("@IdUsoCFDI", datosCuenta.IdUsoCFDI));
                        command.Parameters.Add(new SqlParameter("@UsoCFDI", datosCuenta.ClaveUsoCFDI));

                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@CodResp", SqlDbType.Int);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@DescResp", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 500;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        string cod = sqlParameter1.Value.ToString();
                        string mensaje = sqlParameter2.Value.ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logdbg = new LogDebugMsg();
                        logdbg.M_Value = "web_CA_ActualizaInfoCuentahabiente";
                        logdbg.C_Value = cod;
                        logdbg.R_Value = mensaje;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTitular=" + datosCuenta.ID_Colectiva);
                        parametros.Add("P2", "@Nombre=" + datosCuenta.Nombre);
                        parametros.Add("P3", "@ApPaterno=" + datosCuenta.ApellidoPaterno);
                        parametros.Add("P4", "@ApMaterno=" + datosCuenta.ApellidoMaterno);
                        parametros.Add("P5", "@RFC=" + (string.IsNullOrEmpty(datosCuenta.RFC) ? datosCuenta.RFC :
                            MaskSensitiveData.generalInfo(datosCuenta.RFC)));
                        parametros.Add("P6", "@FechaNacimiento=" + (datosCuenta.FechaNacimiento.Equals(DateTime.MinValue)
                            ? datosCuenta.FechaNacimiento.ToShortDateString() : 
                            MaskSensitiveData.generalInfo(datosCuenta.FechaNacimiento.ToShortDateString())));
                        parametros.Add("P7", "@CURP=" + (string.IsNullOrEmpty(datosCuenta.CURP) ? datosCuenta.CURP :
                            MaskSensitiveData.generalInfo(datosCuenta.CURP)));
                        parametros.Add("P8", "@IdDireccion=" + datosCuenta.IdDireccion);
                        parametros.Add("P9", "@Calle=" + datosCuenta.Calle);
                        parametros.Add("P10", "@NumExterior=" + datosCuenta.NumExterior);
                        parametros.Add("P11", "@NumInterior=" + datosCuenta.NumInterior);
                        parametros.Add("P12", "@EntreCalles=" + datosCuenta.EntreCalles);
                        parametros.Add("P13", "@Referencias=" + datosCuenta.Referencias);
                        parametros.Add("P14", "@IdColonia=" + datosCuenta.IdColonia);
                        parametros.Add("P15", "@TelParticular=" + (string.IsNullOrEmpty(datosCuenta.NumTelParticular) ?
                            datosCuenta.NumTelParticular : MaskSensitiveData.generalInfo(datosCuenta.NumTelParticular)));
                        parametros.Add("P16", "@TelCelular=" + (string.IsNullOrEmpty(datosCuenta.NumTelCelular) ?
                            datosCuenta.NumTelCelular : MaskSensitiveData.generalInfo(datosCuenta.NumTelCelular)));
                        parametros.Add("P17", "@TelTrabajo=" + (string.IsNullOrEmpty(datosCuenta.NumTelTrabajo) ?
                            datosCuenta.NumTelTrabajo : MaskSensitiveData.generalInfo(datosCuenta.NumTelTrabajo)));
                        parametros.Add("P18", "@eMail=" + (string.IsNullOrEmpty(datosCuenta.Email) ? datosCuenta.Email :
                            MaskSensitiveData.Email(datosCuenta.Email)));
                        parametros.Add("P19", "@IdDireccionFiscal=" + datosCuenta.IdDireccionFiscal);
                        parametros.Add("P20", "@CalleFiscal=" + datosCuenta.CalleFiscal);
                        parametros.Add("P21", "@NumExtFiscal=" + datosCuenta.NumExteriorFiscal);
                        parametros.Add("P22", "@NumIntFiscal=" + datosCuenta.NumInteriorFiscal);
                        parametros.Add("P23", "@IdColoniaFiscal=" + datosCuenta.IdColoniaFiscal);
                        parametros.Add("P24", "@Referencias=" + datosCuenta.Referencias);
                        parametros.Add("P25", "@IdRegimenFiscal=" + datosCuenta.IdRegimenFiscal);
                        parametros.Add("P26", "@RegimenFiscal=" + datosCuenta.ClaveRegimenFiscal);
                        parametros.Add("P27", "@Usuario=" + elUsuario.ClaveUsuario);
                        logdbg.Parameters = parametros;

                        pCI.Debug(logdbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la información del cuentahabiente en " +
                    "base de datos");
            }
        }

        /// <summary>
        /// Consulta la "familia" de tarjetas (titular y adicionales) de la tarjeta indicada
        /// dentro del Autorizador
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneFamiliaTarjetas(int IdTarjeta, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjetasAdicionales");

                database.AddInParameter(command, "@ID_MA", DbType.Int32, IdTarjeta);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjetasAdicionales";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ID_MA=" + IdTarjeta.ToString());
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
                throw new CAppException(8010, "Ocurrió un error al consultar la familia de tarjetas de base de datos");
            }
        }

        /// <summary>
        /// Consulta el límite de crédito de la cuenta en base de datos
        /// </summary>
        /// <param name="idMa">Identificador de la tarjeta (medio de acceso)</param>
        /// <param name="idCta">Identificador de la cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Valor del límite de crédito</returns>
        public static DataSet ObtieneLimiteCreditoCuenta(int idMa, int idCta, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneLimiteCreditoDeCuenta");

                database.AddInParameter(command, "@IdMA", DbType.Int32, idMa);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCta);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_ObtieneLimiteCreditoDeCuenta";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdMA=" + idMa);
                parametros.Add("P2", "@IdCuenta=" + idCta);
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
                throw new CAppException(8010, "Ocurrió un error al consultar el límite de crédito en base de datos");
            }
        }

        /// <summary>
        /// Controla en base de datos la eliminación del cambio al límite de crédito de la cuenta
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaCambioLimiteCredito(Int64 IdCuenta, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_CancelaCambioLDC", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", IdCuenta));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logdbg = new LogDebugMsg();
                        logdbg.M_Value = "web_CA_CancelaCambioLDC";
                        logdbg.C_Value = "";
                        logdbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + IdCuenta);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logdbg.Parameters = parametros;

                        pCI.Debug(logdbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                pCI.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al cancelar el cambio de límite de créditoen base de datos");
            }
        }

        /// <summary>
        /// Controla en base de datos el cambio en el límite de crédito de la cuenta
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="IdMA">Identificador del medio de acceso</param>
        /// <param name="NuevoLDC">Valor del nuevo límite de crédito</param>
        /// <param name="PaginaAspx">Nombre de la página ASPX que solicita el cambio</param>
        /// <param name="Observ">Observaciones por las que se solicita el cambio</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ControlaCambioLimiteCredito(long IdCuenta, int IdMA, string NuevoLDC, string PaginaAspx,
           string Observ, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaCambioLDC", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdCuenta", IdCuenta));
                        command.Parameters.Add(new SqlParameter("@IdMA", IdMA));
                        command.Parameters.Add(new SqlParameter("@NuevoLDC", NuevoLDC));
                        command.Parameters.Add(new SqlParameter("@PaginaAspx", PaginaAspx));
                        command.Parameters.Add(new SqlParameter("@Observaciones", Observ));

                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@AppId", AppID));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaCambioLDC";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + IdCuenta);
                        parametros.Add("P2", "@IdMA=" + IdMA);
                        parametros.Add("P3", "@NuevoLDC=" + NuevoLDC);
                        parametros.Add("P4", "@PaginaAspx=" + PaginaAspx);
                        parametros.Add("P5", "@Observaciones=" + Observ);
                        parametros.Add("P6", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                        parametros.Add("P7", "@AppId=" + AppID);
                        logDebug.Parameters = parametros;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                logPCI.ErrorException(Ex);
                throw new Exception("Ocurrió un error al insertar el cambio de límite de crédito en base de datos");
            }
        }

        /// <summary>
        /// Registra en bitácora de base de datos el cambio en el límite de crédito del cliente
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="nuevoLDC">Nuevo límite de crédito</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraCambioLimiteCredito(Int64 IdCuenta, string nuevoLDC, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlCommand command = new SqlCommand("web_CA_AutorizaCambioLDC", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdCuenta", IdCuenta));
                command.Parameters.Add(new SqlParameter("@NuevoLDC", nuevoLDC));
                command.Parameters.Add(new SqlParameter("@UsuarioAutorizador", elUsuario.ClaveUsuario));

                int resp = command.ExecuteNonQuery();

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_AutorizaCambioLDC";
                logDebug.C_Value = "";
                logDebug.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdCuenta=" + IdCuenta);
                parametros.Add("P2", "@NuevoLDC=" + nuevoLDC);
                parametros.Add("P3", "@UsuarioAutorizador=" + elUsuario.ClaveUsuario);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new Exception("Ocurrió un error al insertar el registro del cambio en el límite de crédito " +
                    "en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación sin asignar a la cuenta, y que 
        /// pertenecen al tipo marcado, en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con lor parámetros</returns>
        public static DataSet ObtienePMAPorTipoSinAsignarCuenta(int idTipoPMA, int idCuenta, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMASinAsignarCuenta");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCuenta);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtienePMASinAsignarCuenta";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdCuenta=" + idCuenta);
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
        /// Obtiene los parámetros multiasignación asignados a la cuenta, y que 
        /// pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ObtieneParametrosMACuenta(int idTipoPMA, int idCuenta, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMACuenta");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCuenta);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosMACuenta";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdCuenta=" + idCuenta);
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
        /// Inserta el valor de parámetro multiasignación a la cuenta en el Autorizador
        /// </summary>
        /// <param name="idPMA">Identificador del parámetro multiasignación</param>
        /// <param name="idTarjeta">Identificador de la tarjeta</param>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaPMACuenta(int idPMA, int idTarjeta, Int64 idPlantilla, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaValorPMACuenta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdParametroMultiasignacion", idPMA));
                        command.Parameters.Add(new SqlParameter("@IdTarjeta", idTarjeta));
                        command.Parameters.Add(new SqlParameter("@IdPlantilla", idPlantilla));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaValorPMACuenta";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdParametroMultiasignacion=" + idPMA);
                        parametros.Add("P2", "@IdTarjeta=" + idTarjeta);
                        parametros.Add("P3", "@IdPlantilla=" + idPlantilla);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el valor del parámetro en base de datos");
            }
        }

        /// <summary>
        /// Consulta el resumen de los saldos y movimientos de las cuentas con los IDs indicados al Autorizador
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdCuenta_CDC">Identificador de la cuenta de tipo CDC</param>
        /// <param name="IdCuenta_CLDC">Identificador de la cuenta de tipo CLDC</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Objeto con los datos del resumen</returns>
        public static ResumenMovimientosCuenta ObtieneResumenMovimientos(DateTime FechaInicial, DateTime FechaFinal,
            Int64 IdCuenta_CDC, Int64 IdCuenta_CLDC, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            ResumenMovimientosCuenta resumen = new ResumenMovimientosCuenta();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneResumenSaldosCuenta";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@FechaInicial", SqlDbType.Date);
                        param.Value = FechaInicial;
                        cmd.Parameters.Add(param);

                        SqlParameter param2 = new SqlParameter("@FechaFinal", SqlDbType.Date);
                        param2.Value = FechaFinal;
                        cmd.Parameters.Add(param2);

                        SqlParameter param3 = new SqlParameter("@IdCuentaCDC", SqlDbType.BigInt);
                        param3.Value = IdCuenta_CDC;
                        cmd.Parameters.Add(param3);

                        SqlParameter param4 = new SqlParameter("@IdCuentaCLDC", SqlDbType.BigInt);
                        param4.Value = IdCuenta_CLDC;
                        cmd.Parameters.Add(param4);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    resumen.SaldoInicial = float.Parse(SqlReader["SaldoInicialPeriodo"].ToString());
                                    resumen.Cargos = float.Parse(SqlReader["Cargos"].ToString());
                                    resumen.Abonos = float.Parse(SqlReader["Abonos"].ToString());
                                    resumen.SaldoFinal = float.Parse(SqlReader["SaldoFinalPeriodo"].ToString());
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneResumenSaldosCuenta";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                                parametros.Add("P3", "@IdCuentaCDC=" + IdCuenta_CDC);
                                parametros.Add("P4", "@IdCuentaCLDC=" + IdCuenta_CLDC);
                                logDBG.Parameters = parametros;

                                log.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            log.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer el resumen de movimientos de la cuenta en base de datos.");
                        }
                    }

                    return resumen;
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener el resumen de movimientos de la cuenta " +
                    "en base de datos");
            }
        }


        /// <summary>
        /// Consulta el resumen de los saldos y movimientos al corte y al día de hoy
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdCuenta_CDC">Identificador de la cuenta de tipo CDC</param>
        /// <param name="IdCuenta_CCLC">Identificador de la cuenta de tipo CCLC</param>
        /// <param name="IdCuenta_CLDC">Identificador de la cuenta de tipo CLDC</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Objeto con los datos del resumen</returns>
        public static ResumenMovimientosCuenta ObtieneResumenMovimientosAlCorte(DateTime FechaInicial, DateTime FechaFinal,
            Int64 IdCuenta_CDC, Int64 IdCuenta_CLDC, Int64 IdCuenta_CCLC, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            ResumenMovimientosCuenta resumen = new ResumenMovimientosCuenta();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneResumenSaldoActual";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@FechaInicial", SqlDbType.Date);
                        param.Value = FechaInicial;
                        cmd.Parameters.Add(param);

                        SqlParameter param2 = new SqlParameter("@FechaFinal", SqlDbType.Date);
                        param2.Value = FechaFinal;
                        cmd.Parameters.Add(param2);

                        SqlParameter param3 = new SqlParameter("@IdCuentaCDC", SqlDbType.BigInt);
                        param3.Value = IdCuenta_CDC;
                        cmd.Parameters.Add(param3);

                        SqlParameter param4 = new SqlParameter("@IdCuentaCLDC", SqlDbType.BigInt);
                        param4.Value = IdCuenta_CLDC;
                        cmd.Parameters.Add(param4);

                        SqlParameter param5 = new SqlParameter("@IdCuentaCCLC", SqlDbType.BigInt);
                        param5.Value = IdCuenta_CCLC;
                        cmd.Parameters.Add(param5);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    resumen.SaldoCorte = float.Parse(SqlReader["SaldoCorte"].ToString());
                                    resumen.SaldoActual = float.Parse(SqlReader["SaldoActual"].ToString());
                                    resumen.CreditoDisponible = float.Parse(SqlReader["CreditoDisponible"].ToString());
                                    resumen.SaldoDisponible = float.Parse(SqlReader["SaldoDisponible"].ToString());
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneResumenSaldoActual";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                                parametros.Add("P3", "@IdCuentaCDC=" + IdCuenta_CDC);
                                parametros.Add("P4", "@IdCuentaCLDC=" + IdCuenta_CLDC);
                                parametros.Add("P5", "@IdCuentaCCLC=" + IdCuenta_CCLC);
                                logDBG.Parameters = parametros;

                                log.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            log.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer el resumen de movimientos de la cuenta en base de datos.");
                        }
                    }

                    return resumen;
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al obtener el resumen de movimientos de la cuenta " +
                    "en base de datos");
            }
        }

        /// <summary>
        /// Consulta el detalle de movimientos de la cuenta con el ID indicado, dentro del periodo marcado,
        /// en el Autorizador
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdCuenta_CCLC">Identificador de la cuenta de tipo CCLC</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con el detalle de movimientos</returns>
        public static DataTable ObtieneDetalleMovimientos(DateTime FechaInicial, DateTime FechaFinal,
            Int64 IdCuenta_CCLC, Int64 IdCuenta_CDC, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_DetalleMovsCuenta");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@IdCuentaCCLC", DbType.Int64, IdCuenta_CCLC);
                database.AddInParameter(command, "@IdCuentaCDC", DbType.Int64, IdCuenta_CDC);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_Reporte_DetalleMovsCuenta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                parametros.Add("P3", "@IdCuentaCCLC=" + IdCuenta_CCLC);
                parametros.Add("P4", "@IdCuentaCDC=" + IdCuenta_CDC);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el detalle de movimientos de base de datos");
            }
        }

        /// <summary>
        /// Consulta las operaciones en tránsito de la cuenta con el ID indicado, dentro del periodo marcado,
        /// en el Autorizador
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdCuenta_CCLC">Identificador de la cuenta de tipo CCLC</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las operaciones</returns>
        public static DataTable ObtieneOperacionesEnTransito(DateTime FechaInicial, DateTime FechaFinal,
            long IdCuenta_CCLC, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_OperacionesEnTransito");

                database.AddInParameter(command, "@IdCuenta", DbType.Int64, IdCuenta_CCLC);
                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_Reporte_OperacionesEnTransito";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ID_Cuenta=" + IdCuenta_CCLC);
                parametros.Add("P2", "@FechaInicial=" + FechaInicial);
                parametros.Add("P3", "@FechaFinal=" + FechaFinal);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las operaciones pendientes de base de datos");
            }
        }

        /// <summary>
        /// Consulta la fecha del último corte en el Autorizador, de la cuenta con el ID indicado
        /// </summary>
        /// <param name="idCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Fecha del último corte</returns>
        public static DateTime ValidaFechaValor(Int64 idCuenta, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneFechaUltimoCorteCuenta";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@IdCuenta", SqlDbType.BigInt);
                        param.Value = idCuenta;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        DateTime FechaDeCorte = Convert.ToDateTime(cmd.ExecuteScalar());

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ObtieneDatosPersonalesCuentaOnB";
                        logDBG.C_Value = "";
                        logDBG.R_Value = FechaDeCorte.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCuenta=" + idCuenta);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG

                        return FechaDeCorte;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la fecha del último corte en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de estados de la República Mexicana
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaCatalogoEstadosMexico(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneEstados");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_ObtieneEstados";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el catálogo de estados de la república mexicana en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de géneros en el Autorizador
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaCatalogoGenero(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGeneros");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneGeneros";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el catálogo de géneros en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de estados de la República Mexicana
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaCatalogoPaises(ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePaises");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePaises";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el catálogo de países en base de datos");
            }
        }

        /// <summary>
        /// Consulta los datos OnBoarding de la cuenta seleccionada en base de datos
        /// </summary>
        /// <param name="idTarj">Identificador de la tarjeta</param>
        /// <param name="idCol">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DatosPersonalesCuenta ObtieneDatosPersonalesCtaOnB(int idTarj, int idCol, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            DatosPersonalesCuenta datos = new DatosPersonalesCuenta();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneDatosPersonalesCuentaOnB";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdTarjeta", SqlDbType.Int);
                        param.Value = idTarj;
                        cmd.Parameters.Add(param);

                        param = new SqlParameter("@IdColectiva", SqlDbType.Int);
                        param.Value = idCol;
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
                                    datos.ID_Colectiva = (int)SqlReader["ID_Colectiva"];
                                    datos.TarjetaTitular = SqlReader["NumeroTarjeta"].ToString();
                                    datos.Nombre = SqlReader["Nombre"].ToString();
                                    datos.ApellidoPaterno = SqlReader["ApPaterno"].ToString();
                                    datos.ApellidoMaterno = SqlReader["ApMaterno"].ToString();
                                    datos.FechaNacimiento = Convert.ToDateTime(SqlReader["FechaNacimiento"]);
                                    datos.ClaveEdoNacimiento = SqlReader["ClaveEdoNacimiento"].ToString();
                                    datos.EstadoNacimiento = SqlReader["EstadoNacimiento"].ToString();
                                    datos.Genero = SqlReader["Genero"].ToString();
                                    datos.NumeroIdentificacion = SqlReader["NumID"].ToString();
                                    datos.RFC = SqlReader["RFC"].ToString();
                                    datos.CURP = SqlReader["CURP"].ToString();
                                    datos.Ocupacion = SqlReader["Ocupacion"].ToString();
                                    datos.Profesion = SqlReader["Profesion"].ToString();
                                    datos.Nacionalidad = SqlReader["Nacionalidad"].ToString();

                                    datos.IdDireccion = (int)SqlReader["ID_Direccion"];
                                    datos.Calle = SqlReader["Calle"].ToString();
                                    datos.NumExterior = SqlReader["NumExterior"].ToString();
                                    datos.NumInterior = SqlReader["NumInterior"].ToString();
                                    datos.EntreCalles = SqlReader["EntreCalles"].ToString();
                                    datos.Referencias = SqlReader["Referencias"].ToString();
                                    datos.CodigoPostal = SqlReader["CodigoPostal"].ToString();
                                    datos.IdColonia = (int)SqlReader["ID_Colonia"];
                                    datos.Colonia = SqlReader["Colonia"].ToString();
                                    datos.ClaveMunicipio = SqlReader["ClaveMunicipio"].ToString();
                                    datos.Municipio = SqlReader["Municipio"].ToString();
                                    datos.Ciudad = SqlReader["Ciudad"].ToString();
                                    datos.ClaveEstado = SqlReader["ClaveEstado"].ToString();
                                    datos.Estado = SqlReader["Estado"].ToString();
                                    datos.IdPais =
                                         SqlReader["IdPais"] == DBNull.Value ? (int?)null : (int)SqlReader["IdPais"];
                                    datos.Pais = SqlReader["Pais"].ToString();
                                    datos.GiroNegocio = SqlReader["GiroNegocio"].ToString();
                                    datos.Latitud = SqlReader["Latitud"].ToString();
                                    datos.Longitud = SqlReader["Longitud"].ToString();

                                    datos.NumTelParticular = SqlReader["TelParticular"].ToString();
                                    datos.NumTelCelular = SqlReader["TelCelular"].ToString();
                                    datos.NumTelTrabajo = SqlReader["TelTrabajo"].ToString();
                                    datos.Email = SqlReader["Email"].ToString();
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneDatosPersonalesCuentaOnB";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdTarjeta=" + idTarj);
                                parametros.Add("P2", "@IdColectiva=" + idCol);
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
                            throw new CAppException(8010, "Error al extraer los datos personales de la cuenta en base de datos.");
                        }
                    }

                    return datos;
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los datos personales de la cuenta en base de datos.");
            }
        }

        /// <summary>
        /// Consulta la "familia" de tarjetas (titular y adicionales) en datos OnBoarding,
        /// de la tarjeta indicada, dentro del Autorizador
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneFamiliaTarjetasOnB(string numTarjeta, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjetasAdicionalesOnB");

                database.AddInParameter(command, "@tarjeta", DbType.String, numTarjeta);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los distintos medios de acceso de la tarjeta indicada dentro del Autorizador
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneTiposMediosAcceso(int IdTarjeta, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposMAPorTarjeta");

                database.AddInParameter(command, "@ID_Tarjeta", DbType.Int32, IdTarjeta);
                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTiposMAPorTarjeta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ID_Tarjeta=" + IdTarjeta);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los tipos de MA de base de datos.");
            }
        }

        /// <summary>
        /// Actualiza los datos personales de la cuenta  OnBoarding en base de datos
        /// </summary>
        /// <param name="datosCuenta">Datos de la cuenta a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaDatosPersonalesCuentaOnB(DatosPersonalesCuenta datosCuenta, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaInfoCuentahabienteOnB", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTitular", datosCuenta.ID_Colectiva));
                        command.Parameters.Add(new SqlParameter("@Nombre", datosCuenta.Nombre));
                        command.Parameters.Add(new SqlParameter("@ApPaterno", datosCuenta.ApellidoPaterno));
                        command.Parameters.Add(new SqlParameter("@ApMaterno", datosCuenta.ApellidoMaterno));
                        command.Parameters.Add(new SqlParameter("@FechaNacimiento", datosCuenta.FechaNacimiento));
                        command.Parameters.Add(new SqlParameter("@ClaveEdoNacimiento", datosCuenta.ClaveEdoNacimiento));
                        command.Parameters.Add(new SqlParameter("@Genero", datosCuenta.Genero));
                        command.Parameters.Add(new SqlParameter("@NumID", datosCuenta.NumeroIdentificacion));
                        command.Parameters.Add(new SqlParameter("@RFC", datosCuenta.RFC));
                        command.Parameters.Add(new SqlParameter("@CURP", datosCuenta.CURP));
                        command.Parameters.Add(new SqlParameter("@Ocupacion", datosCuenta.Ocupacion));
                        command.Parameters.Add(new SqlParameter("@Profesion", datosCuenta.Profesion));
                        command.Parameters.Add(new SqlParameter("@Nacionalidad", datosCuenta.Nacionalidad));

                        command.Parameters.Add(new SqlParameter("@IdDireccion", datosCuenta.IdDireccion));
                        command.Parameters.Add(new SqlParameter("@Calle", datosCuenta.Calle));
                        command.Parameters.Add(new SqlParameter("@NumExterior", datosCuenta.NumExterior));
                        command.Parameters.Add(new SqlParameter("@NumInterior", datosCuenta.NumInterior));
                        command.Parameters.Add(new SqlParameter("@EntreCalles", datosCuenta.EntreCalles));
                        command.Parameters.Add(new SqlParameter("@Referencias", datosCuenta.Referencias));
                        command.Parameters.Add(new SqlParameter("@IdColonia", datosCuenta.IdColonia));
                        command.Parameters.Add(new SqlParameter("@IdPais", datosCuenta.IdPais));
                        command.Parameters.Add(new SqlParameter("@GiroNegocio", datosCuenta.GiroNegocio));
                        command.Parameters.Add(new SqlParameter("@Latitud", datosCuenta.Latitud));
                        command.Parameters.Add(new SqlParameter("@Longitud", datosCuenta.Longitud));

                        command.Parameters.Add(new SqlParameter("@TelParticular", datosCuenta.NumTelParticular));
                        command.Parameters.Add(new SqlParameter("@TelCelular", datosCuenta.NumTelCelular));
                        command.Parameters.Add(new SqlParameter("@TelTrabajo", datosCuenta.NumTelTrabajo));
                        command.Parameters.Add(new SqlParameter("@eMail", datosCuenta.Email));

                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logdbg = new LogDebugMsg();
                        logdbg.M_Value = "web_CA_ActualizaInfoCuentahabienteOnB";
                        logdbg.C_Value = "";
                        logdbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTitular=" + datosCuenta.ID_Colectiva);
                        parametros.Add("P2", "@Nombre=" + datosCuenta.Nombre);
                        parametros.Add("P3", "@ApPaterno=" + datosCuenta.ApellidoPaterno);
                        parametros.Add("P4", "@ApMaterno=" + datosCuenta.ApellidoMaterno);
                        parametros.Add("P5", "@FechaNacimiento=" + datosCuenta.FechaNacimiento);
                        parametros.Add("P6", "@ClaveEdoNacimiento=" + datosCuenta.ClaveEdoNacimiento);
                        parametros.Add("P7", "@Genero=" + datosCuenta.Genero);
                        parametros.Add("P8", "@NumID=" + datosCuenta.NumeroIdentificacion);
                        parametros.Add("P9", "@RFC=" + (string.IsNullOrEmpty(datosCuenta.RFC) ? datosCuenta.RFC :
                            MaskSensitiveData.generalInfo(datosCuenta.RFC)));
                        parametros.Add("P10", "@CURP=" + datosCuenta.CURP);
                        parametros.Add("P11", "@Ocupacion=" + datosCuenta.Ocupacion);
                        parametros.Add("P12", "@Profesion=" + datosCuenta.Profesion);
                        parametros.Add("P13", "@Nacionalidad=" + datosCuenta.Nacionalidad);
                        parametros.Add("P14", "@IdDireccion=" + datosCuenta.IdDireccion);
                        parametros.Add("P15", "@Calle=" + datosCuenta.Calle);
                        parametros.Add("P16", "@NumExterior=" + datosCuenta.NumExterior);
                        parametros.Add("P17", "@NumInterior=" + datosCuenta.NumInterior);
                        parametros.Add("P18", "@EntreCalles=" + datosCuenta.EntreCalles);
                        parametros.Add("P19", "@Referencias=" + datosCuenta.Referencias);
                        parametros.Add("P20", "@IdColonia=" + datosCuenta.IdColonia);
                        parametros.Add("P21", "@IdPais=" + datosCuenta.IdPais);
                        parametros.Add("P22", "@GiroNegocio=" + datosCuenta.GiroNegocio);
                        parametros.Add("P23", "@Latitud=" + datosCuenta.Latitud);
                        parametros.Add("P24", "@Longitud=" + datosCuenta.Longitud);
                        parametros.Add("P25", "@TelParticular=" + (string.IsNullOrEmpty(datosCuenta.NumTelParticular) ?
                            datosCuenta.NumTelParticular : MaskSensitiveData.generalInfo(datosCuenta.NumTelParticular)));
                        parametros.Add("P26", "@TelCelular=" + (string.IsNullOrEmpty(datosCuenta.NumTelCelular) ?
                            datosCuenta.NumTelCelular : MaskSensitiveData.generalInfo(datosCuenta.NumTelCelular)));
                        parametros.Add("P27", "@TelTrabajo=" + (string.IsNullOrEmpty(datosCuenta.NumTelTrabajo) ?
                            datosCuenta.NumTelTrabajo : MaskSensitiveData.generalInfo(datosCuenta.NumTelTrabajo)));
                        parametros.Add("P28", "@eMail=" + (string.IsNullOrEmpty(datosCuenta.Email) ? datosCuenta.Email :
                            MaskSensitiveData.Email(datosCuenta.Email)));
                        parametros.Add("P29", "@Usuario=" + elUsuario.ClaveUsuario);
                        logdbg.Parameters = parametros;

                        log.Debug(logdbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar los datos de la colectiva en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación con el tipo de plantilla Preautorizador sin asignar
        /// a la cuenta, y que son del tipo de parámetro indicado dentro del Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con lor parámetros</returns>
        public static DataSet ObtienePMAPreAutPorTipoSinAsignarCuenta(int idTipoPMA, int idCuenta, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMAPreautSinAsignarCuenta");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCuenta);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePMAPreautSinAsignarCuenta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdCuenta=" + idCuenta);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros sin asignar en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación con el tipo de plantilla Preautorizador asignados
        /// a la cuenta, y que pertenecen al tipo marcado en el Autorizador
        /// </summary>
        /// <param name="idTipoPMA">Identificador del tipo de parámetro multiasignación</param>
        /// <param name="idCuenta">Identificador de la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ObtienePMAPreAutCuenta(int idTipoPMA, int idCuenta, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePMAPreautCuenta");

                database.AddInParameter(command, "@IdTipoPMA", DbType.Int32, idTipoPMA);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCuenta);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePMAPreautCuenta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdTipoPMA=" + idTipoPMA);
                parametros.Add("P2", "@IdCuenta=" + idCuenta);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros en base de datos");
            }
        }

        /// <summary>
        /// Inserta el valor de parámetro multiasignación con el tipo de plantilla Preautorizador
        /// a la cuenta en el Autorizador
        /// </summary>
        /// <param name="idPMA">Identificador del parámetro multiasignación</param>
        /// <param name="idTarjeta">Identificador de la tarjeta</param>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaPMAPreAutCuenta(int idPMA, int idTarjeta, Int64 idPlantilla, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaValorPMAPreautCuenta", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdParametroMultiasignacion", idPMA));
                        command.Parameters.Add(new SqlParameter("@IdTarjeta", idTarjeta));
                        command.Parameters.Add(new SqlParameter("@IdPlantilla", idPlantilla));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaValorPMAPreautCuenta";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdParametroMultiasignacion=" + idPMA);
                        parametros.Add("P2", "@IdTarjeta=" + idTarjeta);
                        parametros.Add("P3", "@IdPlantilla=" + idPlantilla);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la plantilla al parámetro en base de datos");
            }
        }

        /// <summary>
        /// Consulta el nombre de embozado de la tarjeta en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Cadena con el nombre de embozado</returns>
        public static string ObtieneNombreEmbozo(string NumTarjeta, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNombreEmbozo");

                database.AddInParameter(command, "@NumeroTarjeta", DbType.String, NumTarjeta);

                return database.ExecuteScalar(command).ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las cuentas OnBoarding que coinciden con los filtros indicados en el Autorizador
        /// </summary>
        /// <param name="datosCuenta">Parámetros de búsqueda de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCuentasPorFiltroOnB(DatosPersonalesCuenta datosCuenta, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasPorFiltroOnB");
                command.CommandTimeout = 0;

                SqlCommand cmdCtaInt = new SqlCommand();
                SqlParameter paramCtaInt = cmdCtaInt.CreateParameter();
                paramCtaInt.ParameterName = "@CuentaInterna";
                paramCtaInt.DbType = DbType.AnsiStringFixedLength;
                paramCtaInt.Direction = ParameterDirection.Input;
                paramCtaInt.Value = string.IsNullOrEmpty(datosCuenta.CuentaInterna) ? null : datosCuenta.CuentaInterna;
                paramCtaInt.Size = datosCuenta.CuentaInterna.Length;
                command.Parameters.Add(paramCtaInt);

                database.AddInParameter(command, "@Nombre", DbType.String, 
                    string.IsNullOrEmpty(datosCuenta.Nombre) ? null : datosCuenta.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, 
                    string.IsNullOrEmpty(datosCuenta.ApellidoPaterno) ? null : datosCuenta.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, 
                    string.IsNullOrEmpty(datosCuenta.ApellidoMaterno) ? null : datosCuenta.ApellidoMaterno);

                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@NumeroTarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ? null : datosCuenta.TarjetaTitular;
                paramTarj.Size = datosCuenta.TarjetaTitular.Length;
                command.Parameters.Add(paramTarj);

                database.AddInParameter(command, "@EsAdicional", DbType.Boolean, datosCuenta.SoloAdicionales);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCuentasPorFiltroOnB";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@CuentaInterna=" + (string.IsNullOrEmpty(datosCuenta.CuentaInterna) ? 
                    datosCuenta.CuentaInterna : datosCuenta.CuentaInterna.Length < 10 ? "******" :
                    MaskSensitiveData.internalAccount(datosCuenta.CuentaInterna)));
                parametros.Add("P2", "@Nombre=" + datosCuenta.Nombre);
                parametros.Add("P3", "@ApPaterno=" + datosCuenta.ApellidoPaterno);
                parametros.Add("P4", "@ApMaterno=" + datosCuenta.ApellidoMaterno);
                parametros.Add("P5", "@NumeroTarjeta=" + (string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ?
                    datosCuenta.TarjetaTitular : MaskSensitiveData.cardNumber(datosCuenta.TarjetaTitular)));
                parametros.Add("P6", "@EsAdicional=" + datosCuenta.SoloAdicionales);
                parametros.Add("P7", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P8", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las cuentas en base de datos");
            }
        }

        /// <summary>
        /// Obtiene los parámetros multiasignación con el tipo de plantilla Preautorizador
        /// relacionados con la tarjeta indicada dentro del Autorizador
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="claveTPMA">Clave del tipo de parámetro multiasignación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los parámetros</returns>
        public static DataSet ObtienePMAPreAutTarjeta(String numTarjeta, String claveTPMA, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosPreAutTarjeta");

                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@NumTarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = numTarjeta;
                paramTarj.Size = numTarjeta.Length;
                command.Parameters.Add(paramTarj);

                database.AddInParameter(command, "@ClaveTipoParametro", DbType.String, claveTPMA);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneParametrosPreAutTarjeta";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@NumTarjeta=" + MaskSensitiveData.cardNumber(numTarjeta));
                parametros.Add("P2", "@ClaveTipoParametro=" + claveTPMA);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros de la tarjeta en base de datos");
            }
        }

        /// <summary>
        /// Consulta en el Autorizador si el nivel de cumplimiento de la cuenta es el correcto (nivel 3)
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="tarjeta">Número de tarjeta asociado a la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje de respuesta de base de datos</returns>
        public static DataTable VerificaNivelCumplimientoCuenta(long idProducto, string tarjeta, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ValidaNivelCumplimientoCuenta_3");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, idProducto);
                
                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@Tarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = tarjeta;
                paramTarj.Size = tarjeta.Length;
                command.Parameters.Add(paramTarj);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ValidaNivelCumplimientoCuenta_3";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@Tarjeta=" + MaskSensitiveData.cardNumber(tarjeta));
                logDBG.Parameters = parametros;

                log.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al solicitar la validación del nivel de la cuenta " +
                    "a base de datos");
            }
        }

        /// <summary>
        /// Inserta una solicitud de cambio en el nivel de la cuenta en la tabla de control Ejecutor/Autorizador
        /// del Autorizador
        /// </summary>
        /// <param name="idPlantilla">Identificador de la plantilla</param>
        /// <param name="montoSolic">Monto solicitado (abono máximo personalizado)</param>
        /// <param name="montoAcum">Monto acumulado (saldo máximo personalizado)</param>
        /// <param name="motivo">Motivo por el que se realiza la solicitud</param>
        /// <param name="respBlackList">Identificador de la tarjeta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaSolicNivelCuentaVIP(long idPlantilla, string montoSolic, string montoAcum, string motivo,
            string respBlackList, string paginaAspx, Usuario elUsuario, Guid AppID, ILogHeader elLog, string tarjeta)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaSolicitudCuentaNivelVIP", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPlantilla", idPlantilla));
                        command.Parameters.Add(new SqlParameter("@NuevoValorMontoSolic", montoSolic));
                        command.Parameters.Add(new SqlParameter("@NuevoValorMontoAcum", montoAcum));
                        command.Parameters.Add(new SqlParameter("@Motivo", motivo));
                        command.Parameters.Add(new SqlParameter("@ResultadoBlackList", respBlackList));
                        command.Parameters.Add(new SqlParameter("@PaginaAspx", paginaAspx));
                        command.Parameters.Add(new SqlParameter("@UsuarioEjecutor", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@AppID", AppID));

                        SqlCommand cmdTarj = new SqlCommand();
                        SqlParameter paramTarj = cmdTarj.CreateParameter();
                        paramTarj.ParameterName = "@Tarjeta";
                        paramTarj.DbType = DbType.AnsiStringFixedLength;
                        paramTarj.Direction = ParameterDirection.Input;
                        paramTarj.Value = tarjeta;
                        paramTarj.Size = tarjeta.Length;
                        command.Parameters.Add(paramTarj);

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_InsertaSolicitudCuentaNivelVIP";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPlantilla=" + idPlantilla);
                        parametros.Add("P2", "@NuevoValorMontoSolic=" + montoSolic);
                        parametros.Add("P3", "@NuevoValorMontoAcum=" + montoAcum);
                        parametros.Add("P4", "@Motivo=" + motivo);
                        parametros.Add("P5", "@ResultadoBlackList=" + respBlackList);
                        parametros.Add("P6", "@PaginaAspx=" + paginaAspx);
                        parametros.Add("P7", "@UsuarioEjecutor=" + elUsuario.ClaveUsuario);
                        parametros.Add("P8", "@AppID=" + AppID);
                        parametros.Add("P9", "@Tarjeta=" + MaskSensitiveData.cardNumber(tarjeta));
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar la solicitud de cambio en base de datos");
            }
        }

        /// <summary>
        /// Consulta la lista de plantillas de nivel 3 que corresponden al producto y/o tarjeta marcados
        /// dentro del Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="tarjeta">Número de tarjeta asociado a la cuenta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las plantillas</returns>
        public static DataTable ObtienePlantillasN3(long idProducto, string tarjeta, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePlantillasProdTarjeta");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdProducto", DbType.Int64, idProducto);
                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@Tarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(tarjeta) ? null : tarjeta;
                param.Size = tarjeta.Length;
                command.Parameters.Add(param);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtienePlantillasProdTarjeta";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@Tarjeta=" + (string.IsNullOrEmpty(tarjeta) ? tarjeta :
                    MaskSensitiveData.cardNumber(tarjeta)));
                logDBG.Parameters = parametros;

                log.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las plantillas del producto/tarjeta en base de datos");
            }
        }

        /// <summary>
        /// Consulta los registros pendientes de autorización para cambio de nivel de cuenta dentro del Autorizador
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes</returns>
        public static DataTable ObtieneSolicitudesCambioCuentasVIP(ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRegistrosPorAutorizarClientesVIP");
                command.CommandTimeout = 0;

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneRegistrosPorAutorizarClientesVIP";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las solicitudes de cambio en base de datos");
            }
        }


        /// <summary>
        /// Consulta los registros pendientes de autorización para cambio de nivel de cuenta dentro del Autorizador
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes</returns>
        public static DataTable ObtieneRegistrosTarjetasVIPDetalle(long idProducto, string tarjeta, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRegistrosTarjetasVIPDetalle");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdProducto", DbType.Int64, idProducto);
                
                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@Tarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(tarjeta) ? null : tarjeta;
                param.Size = tarjeta.Length;
                command.Parameters.Add(param);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];


                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneRegistrosTarjetasVIPDetalle";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdProducto=" + idProducto);
                parametros.Add("P2", "@Tarjeta=" + (string.IsNullOrEmpty(tarjeta) ? tarjeta :
                    MaskSensitiveData.cardNumber(tarjeta)));
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las plantillas del producto/tarjeta en base de datos");
            }
        }

        /// <summary>
        /// Actualiza y autoriza el cambio en los valores de los parámetro multiasignación de nivel de cumplimiento
        /// de la cuenta personalizado (VIP) en base de datos
        /// </summary>
        /// <param name="laPlantilla">Datos de la plantilla/parámetros por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaYAutorizaValoresCuentaVIP(PlantillaCuentaVIP laPlantilla, Usuario elUsuario,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaYAutorizaValoresPMA_CuentaVIP", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPlantilla", laPlantilla.IdPlantilla));
                        command.Parameters.Add(new SqlParameter("@Id_EA_NivelCta", laPlantilla.IdEA_NivelCtaCumpl));
                        command.Parameters.Add(new SqlParameter("@Id_EA_NivelCtaPers", laPlantilla.IdEA_NivelCtaCumplPers));
                        command.Parameters.Add(new SqlParameter("@Id_EA_SaldoMax", laPlantilla.IdEA_SaldoMaxPers));
                        command.Parameters.Add(new SqlParameter("@Id_EA_AbonoMax", laPlantilla.IdEA_MaxAbonoPers));
                        command.Parameters.Add(new SqlParameter("@Id_VPMA_NivelCta", laPlantilla.IdVPMA_NivelCtaCumpl));
                        command.Parameters.Add(new SqlParameter("@Id_PMA_NivelCtaPers", laPlantilla.IdPMA_NivelCtaCumplPers));
                        command.Parameters.Add(new SqlParameter("@Id_PMA_SaldoMax", laPlantilla.IdPMA_SaldoMaxPers));
                        command.Parameters.Add(new SqlParameter("@Id_PMA_AbonoMax", laPlantilla.IdPMA_MaxAbonoPers));
                        command.Parameters.Add(new SqlParameter("@Motivo", laPlantilla.Motivo));
                        command.Parameters.Add(new SqlParameter("@ResultadoBlackList", laPlantilla.RespListaNegra));
                        command.Parameters.Add(new SqlParameter("@UsuarioAutorizador", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_ActualizaYAutorizaValoresPMA_CuentaVIP";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPlantilla=" + laPlantilla.IdPlantilla);
                        parametros.Add("P2", "@Id_EA_NivelCta=" + laPlantilla.IdEA_NivelCtaCumpl);
                        parametros.Add("P3", "@Id_EA_NivelCtaPers=" + laPlantilla.IdEA_NivelCtaCumplPers);
                        parametros.Add("P4", "@Id_EA_SaldoMax=" + laPlantilla.IdEA_SaldoMaxPers);
                        parametros.Add("P5", "@Id_EA_AbonoMax=" + laPlantilla.IdEA_MaxAbonoPers);
                        parametros.Add("P6", "@Id_VPMA_NivelCta=" + laPlantilla.IdVPMA_NivelCtaCumpl);
                        parametros.Add("P7", "@Id_PMA_NivelCtaPers=" + laPlantilla.IdPMA_NivelCtaCumplPers);
                        parametros.Add("P8", "@Id_PMA_SaldoMax=" + laPlantilla.IdPMA_SaldoMaxPers);
                        parametros.Add("P9", "@Id_PMA_AbonoMax=" + laPlantilla.IdPMA_MaxAbonoPers);
                        parametros.Add("P10", "@Motivo=" + laPlantilla.Motivo);
                        parametros.Add("P11", "@ResultadoBlackList=" + laPlantilla.RespListaNegra);
                        parametros.Add("P12", "@UsuarioAutorizador=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        unLog.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar y autorizar la solicitud de cambio en base de datos");
            }
        }

        /// <summary>
        /// Elimina de la tabla de control de autorizaciones los IDs de la lista de plantillas/cuenta
        /// en base de datos
        /// </summary>
        /// <param name="laPlantilla">Datos de la plantilla/parámetros por actualizar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaValoresCuentaVIP(PlantillaCuentaVIP unaPlantilla, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_RechazaValoresPMA_CuentaVIP", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Id_EA_NivelCta", unaPlantilla.IdEA_NivelCtaCumpl));
                        command.Parameters.Add(new SqlParameter("@Id_EA_NivelCtaPers", unaPlantilla.IdEA_NivelCtaCumplPers));
                        command.Parameters.Add(new SqlParameter("@Id_EA_SaldoMax", unaPlantilla.IdEA_SaldoMaxPers));
                        command.Parameters.Add(new SqlParameter("@Id_EA_AbonoMax", unaPlantilla.IdEA_MaxAbonoPers));
                        command.Parameters.Add(new SqlParameter("@Id_VPMA_NivelCta", unaPlantilla.IdVPMA_NivelCtaCumpl));
                        command.Parameters.Add(new SqlParameter("@IdPlantilla", unaPlantilla.IdPlantilla));
                        command.Parameters.Add(new SqlParameter("@UsuarioAutorizador", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_RechazaValoresPMA_CuentaVIP";
                        logDbg.C_Value = "";
                        logDbg.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@Id_EA_NivelCta=" + unaPlantilla.IdEA_NivelCtaCumpl);
                        parametros.Add("P2", "@Id_EA_NivelCtaPers=" + unaPlantilla.IdEA_NivelCtaCumplPers);
                        parametros.Add("P3", "@Id_EA_SaldoMax=" + unaPlantilla.IdEA_SaldoMaxPers);
                        parametros.Add("P4", "@Id_EA_AbonoMax=" + unaPlantilla.IdEA_MaxAbonoPers);
                        parametros.Add("P5", "@Id_VPMA_NivelCta=" + unaPlantilla.IdVPMA_NivelCtaCumpl);
                        parametros.Add("P6", "@IdPlantilla=" + unaPlantilla.IdPlantilla);
                        parametros.Add("P7", "@UsuarioAutorizador=" + elUsuario.ClaveUsuario);
                        logDbg.Parameters = parametros;

                        unLog.Debug(logDbg);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                unLog.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar la solicitud de cambio en base de datos");
            }
        }

        /// <summary>
        /// Consulta la tarjetas de la tarjeta indicada
        /// dentro del Autorizador
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="soloTarjeta">Si se desea solo la tarjeta (1) o excluir la tarjeta (0)</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet web_CA_ObtieneMediosAcceso(string numTarjeta, int @soloTarjeta, Usuario elUsuario, 
            Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMediosAcceso");

                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@tarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = numTarjeta;
                paramTarj.Size = numTarjeta.Length;
                command.Parameters.Add(paramTarj);

                database.AddInParameter(command, "@soloTarjeta", DbType.Int32, @soloTarjeta);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTarjetas";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@tarjeta=" + MaskSensitiveData.cardNumber(numTarjeta));
                parametros.Add("P2", "@soloTarjeta=" + @soloTarjeta);
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
                throw new CAppException(8010, "Ocurrió un error al consultar los medios de acceso de base de datos");
            }
        }

        /// <summary>
        /// Consulta los distintos medios de acceso de la tarjeta indicada dentro del Autorizador
        /// </summary>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCuentas(string numTarjeta, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentas");

                SqlCommand cmdCveMA = new SqlCommand();
                SqlParameter paramCveMA = cmdCveMA.CreateParameter();
                paramCveMA.ParameterName = "@ClaveMA";
                paramCveMA.DbType = DbType.AnsiStringFixedLength;
                paramCveMA.Direction = ParameterDirection.Input;
                paramCveMA.Value = numTarjeta;
                paramCveMA.Size = numTarjeta.Length;
                command.Parameters.Add(paramCveMA);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneCuentas";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ClaveMA=" + MaskSensitiveData.cardNumber(numTarjeta));
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar las Cuentas de base de datos.");
            }
        }

        /// <summary>
        /// Consulta las cuentas OnBoarding que coinciden con los filtros indicados en el Autorizador
        /// </summary>
        /// <param name="datosCuenta">Parámetros de búsqueda de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCuentasPorFiltroOnB_2(DatosPersonalesCuenta datosCuenta, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasPorFiltroOnB_2");
                command.CommandTimeout = 0;

                SqlCommand cmdCtaInt = new SqlCommand();
                SqlParameter paramCtaInt = cmdCtaInt.CreateParameter();
                paramCtaInt.ParameterName = "@CuentaInterna";
                paramCtaInt.DbType = DbType.AnsiStringFixedLength;
                paramCtaInt.Direction = ParameterDirection.Input;
                paramCtaInt.Value = string.IsNullOrEmpty(datosCuenta.CuentaInterna) ? null : datosCuenta.CuentaInterna;
                paramCtaInt.Size = datosCuenta.CuentaInterna.Length;
                command.Parameters.Add(paramCtaInt);

                SqlCommand cmdTarj = new SqlCommand();
                SqlParameter paramTarj = cmdTarj.CreateParameter();
                paramTarj.ParameterName = "@NumeroTarjeta";
                paramTarj.DbType = DbType.AnsiStringFixedLength;
                paramTarj.Direction = ParameterDirection.Input;
                paramTarj.Value = string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ? null : datosCuenta.TarjetaTitular;
                paramTarj.Size = datosCuenta.TarjetaTitular.Length;
                command.Parameters.Add(paramTarj);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCuentasPorFiltroOnB_2";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@CuentaInterna=" + (string.IsNullOrEmpty(datosCuenta.CuentaInterna) ?
                    datosCuenta.CuentaInterna : datosCuenta.CuentaInterna.Length < 10 ? "******" : 
                    MaskSensitiveData.internalAccount(datosCuenta.CuentaInterna)));
                parametros.Add("P2", "@NumeroTarjeta=" + (string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ?
                    datosCuenta.TarjetaTitular : datosCuenta.TarjetaTitular.Length < 16 ? "******" :
                    MaskSensitiveData.cardNumber(datosCuenta.TarjetaTitular)));
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
                throw new CAppException(8010, "Ocurrió un error al consultar las cuentas en base de datos");
            }
        }

        /// <summary>
        /// Consulta las cuentas de tipo crédito que coinciden con los filtros indicados en el Autorizador
        /// </summary>
        /// <param name="IdColectiva">Identificador del subemisor</param>
        /// <param name="datosCuenta">Parámetros de búsqueda de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCuentasTDCSubEmisor(int IdColectiva, DatosPersonalesCuenta datosCuenta,
            IUsuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuentasSubEmPorFiltro");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdColectiva", DbType.Int32,
                    IdColectiva == -1 ? (int?)null : IdColectiva);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, 
                    datosCuenta.ID_Cuenta == -1 ? (int?)null : datosCuenta.ID_Cuenta);
                database.AddInParameter(command, "@Nombre", DbType.String, 
                    string.IsNullOrEmpty(datosCuenta.Nombre) ? null : datosCuenta.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String,
                    string.IsNullOrEmpty(datosCuenta.ApellidoPaterno) ? null : datosCuenta.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String,
                    string.IsNullOrEmpty(datosCuenta.ApellidoMaterno) ? null : datosCuenta.ApellidoMaterno);

                SqlCommand cmd = new SqlCommand();
                SqlParameter param = cmd.CreateParameter();
                param.ParameterName = "@NumeroTarjeta";
                param.DbType = DbType.AnsiStringFixedLength;
                param.Direction = ParameterDirection.Input;
                param.Value = string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ? null : datosCuenta.TarjetaTitular;
                param.Size = datosCuenta.TarjetaTitular.Length;
                command.Parameters.Add(param);

                database.AddInParameter(command, "@EsAdicional", DbType.Boolean, datosCuenta.SoloAdicionales);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCuentasSubEmPorFiltro";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                parametros.Add("P2", "@IdCuenta=" + datosCuenta.ID_Cuenta);
                parametros.Add("P3", "@Nombre=" + datosCuenta.Nombre);
                parametros.Add("P4", "@ApPaterno=" + datosCuenta.ApellidoPaterno);
                parametros.Add("P5", "@ApMaterno=" + datosCuenta.ApellidoMaterno);
                parametros.Add("P6", "@NumeroTarjeta=" + (string.IsNullOrEmpty(datosCuenta.TarjetaTitular) ?
                    datosCuenta.TarjetaTitular : MaskSensitiveData.cardNumber(datosCuenta.TarjetaTitular)));
                parametros.Add("P7", "@EsAdicional=" + datosCuenta.SoloAdicionales);
                parametros.Add("P8", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P9", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las cuentas de base de datos");
            }
        }

        /// <summary>
        /// Consulta la "familia" de tarjetas (titular y adicionales) de la tarjeta indicada
        /// dentro del Autorizador
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneFamTarjetas(int IdTarjeta, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneFamiliaTarjetas");

                database.AddInParameter(command, "@ID_MA", DbType.Int32, IdTarjeta);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneFamiliaTarjetas";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ID_MA=" + IdTarjeta.ToString());
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la familia de tarjetas de base de datos");
            }
        }
    }
}
