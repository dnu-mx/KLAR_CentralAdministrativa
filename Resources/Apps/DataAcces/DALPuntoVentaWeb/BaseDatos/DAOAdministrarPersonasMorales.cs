using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Entidades;
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

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOAdministrarPersonasMorales
    {
        /// <summary>
        /// Obtiene el catálogo de estatus del alta de personas morales en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Dataset con los resultados</returns>
        public static DataSet ListaEstatusPersonaMoral(ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusAltaPersonaMoral");

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneEstatusAltaPersonaMoral";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    
                
                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus de personas morales de base de datos.");
            }
        }

        /// <summary>
        /// Inserta o actualiza una nueva persona moral en base de datos
        /// </summary>
        /// <param name="unaPersona">Objeto PersonaMoral con los datos por insertar o actualizar</param>
        /// <param name="pagina">Nombre de la página que realiza la inserción o actualización</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Identificador de la nueva persona moral (folio)</returns>
        public static long InsertaActualizaPersonaMoral(PersonaMoral unaPersona, string pagina,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaOActualizaPersonaMoralEnProceso", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPM", unaPersona.ID_PersonaMoral));
                        command.Parameters.Add(new SqlParameter("@ClaveCliente", unaPersona.ClaveEmpresa));
                        command.Parameters.Add(new SqlParameter("@RazonSocial", unaPersona.RazonSocial));
                        command.Parameters.Add(new SqlParameter("@RFC", unaPersona.RFC));
                        command.Parameters.Add(new SqlParameter("@GiroEmpresa", unaPersona.GiroEmpresa));
                        command.Parameters.Add(new SqlParameter("@RepresentanteLegal", unaPersona.RepresentanteLegal));
                        command.Parameters.Add(new SqlParameter("@Telefono", unaPersona.Telefono));
                        command.Parameters.Add(new SqlParameter("@CorreoElectronico", unaPersona.CorreoElectronico));
                        command.Parameters.Add(new SqlParameter("@Calle", unaPersona.Calle));
                        command.Parameters.Add(new SqlParameter("@NumExt", unaPersona.NumeroExterior));
                        command.Parameters.Add(new SqlParameter("@NumInt", unaPersona.NumeroInterior));
                        command.Parameters.Add(new SqlParameter("@EntreCalles", unaPersona.EntreCalles));
                        command.Parameters.Add(new SqlParameter("@Referencias", unaPersona.Referencias));
                        command.Parameters.Add(new SqlParameter("@CP", unaPersona.CodigoPostal));
                        command.Parameters.Add(new SqlParameter("@Municipio", unaPersona.Municipio));
                        command.Parameters.Add(new SqlParameter("@Ciudad", unaPersona.Ciudad));
                        command.Parameters.Add(new SqlParameter("@Estado", unaPersona.Estado));
                        command.Parameters.Add(new SqlParameter("@Pais", unaPersona.Pais));
                        command.Parameters.Add(new SqlParameter("@Latitud", unaPersona.Latitud));
                        command.Parameters.Add(new SqlParameter("@Longitud", unaPersona.Longitud));
                        command.Parameters.Add(new SqlParameter("@OrigenRecursos", unaPersona.OrigenRecursos));
                        command.Parameters.Add(new SqlParameter("@DestinoRecursos", unaPersona.DestinoRecursos));
                        command.Parameters.Add(new SqlParameter("@TipoRecursos", unaPersona.TipoRecursos));
                        command.Parameters.Add(new SqlParameter("@NaturalezaRecursos", unaPersona.NaturalezaRecursos));
                        command.Parameters.Add(new SqlParameter("@DesempenaFuncionesPublicas", unaPersona.DesempenaFuncionesPublicas));
                        command.Parameters.Add(new SqlParameter("@ParentescoPPE", unaPersona.ParentescoPPE));
                        command.Parameters.Add(new SqlParameter("@SocioDePPE", unaPersona.SocioDePPE));
                        command.Parameters.Add(new SqlParameter("@DineroIngresoPorMes", unaPersona.DineroIngresoPorMes));
                        command.Parameters.Add(new SqlParameter("@NumVecesDeIngresoPorMes", unaPersona.NumVecesDeIngresoPorMes));
                        command.Parameters.Add(new SqlParameter("@CundoIngresaDineroPorMes", unaPersona.CuandoIngresaDineroPorMes));
                        command.Parameters.Add(new SqlParameter("@CentroCostos", unaPersona.CentroCostos));
                        command.Parameters.Add(new SqlParameter("@Colonia", unaPersona.Colonia));
                        command.Parameters.Add(new SqlParameter("@Clabe", unaPersona.CLABE));
                        command.Parameters.Add(new SqlParameter("@AppID", AppID));
                        command.Parameters.Add(new SqlParameter("@PaginaAspx", pagina));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        var sqlParameter1 = new SqlParameter("@IdNuevaPM", SqlDbType.BigInt);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        conn.Open();

                        command.ExecuteNonQuery();

                        long resp = Convert.ToInt64(sqlParameter1.Value);

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaOActualizaPersonaMoralEnProceso";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPM=" + unaPersona.ID_PersonaMoral);
                        parametros.Add("P2", "@ClaveCliente=" + unaPersona.ClaveEmpresa);
                        parametros.Add("P3", "@RazonSocial=" + unaPersona.RazonSocial);
                        parametros.Add("P4", "@RFC=" + (String.IsNullOrEmpty(unaPersona.RFC) ? unaPersona.RFC :
                            MaskSensitiveData.generalInfo(unaPersona.RFC)));
                        parametros.Add("P5", "@GiroEmpresa=" + unaPersona.GiroEmpresa);
                        parametros.Add("P6", "@RepresentanteLegal=" + unaPersona.RepresentanteLegal);
                        parametros.Add("P7", "@Telefono=" + (String.IsNullOrEmpty(unaPersona.Telefono) ? 
                            unaPersona.Telefono : MaskSensitiveData.generalInfo(unaPersona.Telefono)));
                        parametros.Add("P8", "@CorreoElectronico=" + (String.IsNullOrEmpty(unaPersona.CorreoElectronico) ?
                            unaPersona.CorreoElectronico : MaskSensitiveData.Email(unaPersona.CorreoElectronico)));
                        parametros.Add("P9", "@Calle=" + unaPersona.Calle);
                        parametros.Add("P10", "@NumExt=" + unaPersona.NumeroExterior);
                        parametros.Add("P11", "@NumInt=" + unaPersona.NumeroInterior);
                        parametros.Add("P12", "@EntreCalles=" + unaPersona.EntreCalles);
                        parametros.Add("P13", "@Referencias=" + unaPersona.Referencias);
                        parametros.Add("P14", "@CP=" + unaPersona.CodigoPostal);
                        parametros.Add("P15", "@Municipio=" + unaPersona.Municipio);
                        parametros.Add("P16", "@Ciudad=" + unaPersona.Ciudad);
                        parametros.Add("P17", "@Estado=" + unaPersona.Estado);
                        parametros.Add("P18", "@Pais=" + unaPersona.Pais);
                        parametros.Add("P19", "@Latitud=" + unaPersona.Latitud);
                        parametros.Add("P20", "@Longitud=" + unaPersona.Longitud);
                        parametros.Add("P21", "@OrigenRecursos=" + unaPersona.OrigenRecursos);
                        parametros.Add("P22", "@DestinoRecursos=" + unaPersona.DestinoRecursos);
                        parametros.Add("P23", "@TipoRecursos=" + unaPersona.TipoRecursos);
                        parametros.Add("P24", "@NaturalezaRecursos=" + unaPersona.NaturalezaRecursos);
                        parametros.Add("P25", "@DesempenaFuncionesPublicas=" + unaPersona.DesempenaFuncionesPublicas);
                        parametros.Add("P26", "@ParentescoPPE=" + unaPersona.ParentescoPPE);
                        parametros.Add("P27", "@SocioDePPE=" + unaPersona.SocioDePPE);
                        parametros.Add("P28", "@DineroIngresoPorMes=" + unaPersona.DineroIngresoPorMes);
                        parametros.Add("P29", "@NumVecesDeIngresoPorMes=" + unaPersona.NumVecesDeIngresoPorMes);
                        parametros.Add("P30", "@CundoIngresaDineroPorMes=" + unaPersona.CuandoIngresaDineroPorMes);
                        parametros.Add("P31", "@CentroCostos=" + unaPersona.CentroCostos);
                        parametros.Add("P32", "@Colonia=" + unaPersona.Colonia);
                        parametros.Add("P33", "@Clabe=" + (String.IsNullOrEmpty(unaPersona.CLABE) ? unaPersona.CLABE :
                            MaskSensitiveData.CLABE(unaPersona.CLABE)));
                        parametros.Add("P34", "@AppID=" + AppID);
                        parametros.Add("P35", "@PaginaAspx=" + pagina);
                        parametros.Add("P36", "@Usuario=" + elUsuario.ClaveUsuario);
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
                throw new CAppException(8010, "Ocurrió un error al actualizar los datos de la persona moral" +
                    " en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene la lista de personal morales que coinciden con los filtros indicados en base de datos
        /// </summary>
        /// <param name="claveEmpresa">Clave de la empresa (SubEmisor)</param>
        /// <param name="razonSocial">Razón social de la persona moral</param>
        /// <param name="idEstatus">ID del estatus del alta de la persona moral</param>
        /// <param name="clabe">CLABE de la persona moral</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet ObtienePersonasMorales(String claveEmpresa, String razonSocial, int idEstatus,
            string clabe, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePersonasMorales");

                database.AddInParameter(command, "@ClaveCliente", DbType.String, claveEmpresa);
                database.AddInParameter(command, "@RazonSocial", DbType.String, razonSocial);
                database.AddInParameter(command, "@IdEstatus", DbType.Int32, idEstatus);
                database.AddInParameter(command, "@Clabe", DbType.String, clabe);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienePersonasMorales";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ClaveCliente=" + claveEmpresa);
                parametros.Add("P2", "@RazonSocial=" + razonSocial);
                parametros.Add("P3", "@IdEstatus=" + idEstatus);
                parametros.Add("P4", "@Clabe=" + (String.IsNullOrEmpty(clabe) ? clabe :
                    MaskSensitiveData.CLABE(clabe)));
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las personas morales de base de datos.");
            }
        }

        /// <summary>
        /// Consulta la información adicional del producto con el ID indicado en base de datos
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Objeto Producto con los datos obtenidos</returns>
        public static PersonaMoral ObtieneDatosPersonaMoral(Int64 idPersonaMoral, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            PersonaMoral personaMoral = new PersonaMoral();
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conx = new SqlConnection(BDWebhookMati.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_CA_ObtieneDatosPersonaMoral";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdPersonaMoral", SqlDbType.BigInt);
                        param.Value = idPersonaMoral;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    personaMoral.RazonSocial = SqlReader["RazonSocial"].ToString();
                                    personaMoral.Estatus = SqlReader["Estatus"].ToString();
                                    personaMoral.CLABE = SqlReader["Clabe"].ToString();
                                    personaMoral.RFC = SqlReader["RFC"].ToString();
                                    personaMoral.GiroEmpresa = SqlReader["GiroEmpresa"].ToString();
                                    personaMoral.RepresentanteLegal = SqlReader["RepresentanteLegal"].ToString();
                                    personaMoral.Telefono = SqlReader["Telefono"].ToString();
                                    personaMoral.CorreoElectronico = SqlReader["CorreoElectronico"].ToString();
                                    personaMoral.Calle = SqlReader["Calle"].ToString();
                                    personaMoral.NumeroExterior = SqlReader["NumExt"].ToString();
                                    personaMoral.NumeroInterior = SqlReader["NumInt"].ToString();
                                    personaMoral.EntreCalles = SqlReader["EntreCalles"].ToString();
                                    personaMoral.Referencias = SqlReader["Referencias"].ToString();
                                    personaMoral.CodigoPostal = SqlReader["CP"].ToString();
                                    personaMoral.Municipio = SqlReader["Municipio"].ToString();
                                    personaMoral.Ciudad = SqlReader["Ciudad"].ToString();
                                    personaMoral.Estado = SqlReader["Estado"].ToString();
                                    personaMoral.Pais = SqlReader["Pais"].ToString();
                                    personaMoral.Latitud = SqlReader["Latitud"].ToString();
                                    personaMoral.Longitud = SqlReader["Longitud"].ToString();
                                    personaMoral.OrigenRecursos = SqlReader["OrigenRecursos"].ToString();
                                    personaMoral.DestinoRecursos = SqlReader["DestinoRecursos"].ToString();
                                    personaMoral.TipoRecursos = SqlReader["TipoRecursos"].ToString();
                                    personaMoral.NaturalezaRecursos = SqlReader["NaturalezaRecursos"].ToString();
                                    personaMoral.DesempenaFuncionesPublicas = SqlReader["DesempenaFuncionesPublicas"].ToString();
                                    personaMoral.ParentescoPPE = SqlReader["ParentescoPPE"].ToString();
                                    personaMoral.SocioDePPE = SqlReader["SocioDePPE"].ToString();
                                    personaMoral.DineroIngresoPorMes = SqlReader["DineroIngresoPorMes"].ToString();
                                    personaMoral.NumVecesDeIngresoPorMes = SqlReader["NumVecesDeIngresoPorMes"].ToString();
                                    personaMoral.CuandoIngresaDineroPorMes = SqlReader["CundoIngresaDineroPorMes"].ToString();
                                    personaMoral.CentroCostos = SqlReader["CentroCostos"].ToString();
                                    personaMoral.Colonia = SqlReader["Colonia"].ToString();
                                    personaMoral.NumeroTarjeta = SqlReader["Tarjeta"].ToString();
                                    personaMoral.TipoManufactura = SqlReader["TipoManufactura"].ToString();
                                }

                                /////>>>LOG DEBUG
                                LogDebugMsg logDBG = new LogDebugMsg();
                                logDBG.M_Value = "web_CA_ObtieneDatosPersonaMoral";
                                logDBG.C_Value = "";
                                logDBG.R_Value = "***************************";

                                Dictionary<string, string> parametros = new Dictionary<string, string>();
                                parametros.Add("P1", "@IdPersonaMoral=" + idPersonaMoral);
                                logDBG.Parameters = parametros;

                                log.Debug(logDBG);
                                /////<<<LOG DEBUG
                            }
                        }
                        catch (Exception _ex)
                        {
                            log.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los datos de la persona moral de base de datos.");
                        }
                    }
                }

                return personaMoral;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los datos de la persona moral de base de datos.");
            }
        }

        /// <summary>
        /// Obtiene la lista de documentos asociados a la persona moral en base de datos
        /// </summary>
        /// <param name="idPersonaMoral">Identificador de la persona moral</param>
        /// <param name="esEjecutor">Bandera para indicar si el usuario en sesión es o no ejecutor</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet ObtieneDocumentosPersonaMoral(Int64 idPersonaMoral, bool esEjecutor, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhookMati.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDocumentosPersonaMoral");

                database.AddInParameter(command, "@IdPM", DbType.Int64, idPersonaMoral);
                database.AddInParameter(command, "@EsEjecutor", DbType.Boolean, esEjecutor);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneDocumentosPersonaMoral";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdPM=" + idPersonaMoral);
                parametros.Add("P2", "@EsEjecutor=" + esEjecutor);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los documentos de la persona moral " +
                    " en base de datos.");
            }
        }

        /// <summary>
        /// Inserta un registro de documento para la persona moral en base de datos
        /// </summary>
        /// <param name="idPersonaMoral">Identificador de la persona moral</param>
        /// <param name="archivo">Nombre del archivo o documento</param>
        /// <param name="ruta">Ruta del archivo o documento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaDocumentoPersonaMoral(Int64 idPersonaMoral, String archivo, String ruta,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaDocumentoPersonaMoral", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPM", idPersonaMoral));
                        command.Parameters.Add(new SqlParameter("@NombreArchivo", archivo));
                        command.Parameters.Add(new SqlParameter("@RutaArchivo", ruta));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaDocumentoPersonaMoral";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPM=" + idPersonaMoral);
                        parametros.Add("P2", "@NombreArchivo=" + archivo);
                        parametros.Add("P3", "@RutaArchivo=" + ruta);
                        parametros.Add("P4", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        pCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el documento de la persona moral " +
                    " en base de datos.");
            }
        }

        /// <summary>
        /// Elimina un registro de documento para la persona moral en base de datos
        /// </summary>
        /// <param name="idDocumento">Identificador del documento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void EliminaDocumentoPersonaMoral(Int64 idDocumento, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EliminaDocumentoPersonaMoral", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdDocumentoPM", idDocumento));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_EliminaDocumentoPersonaMoral";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdDocumentoPM=" + idDocumento);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        unLog.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al eliminar el documento de la persona moral " +
                    " en base de datos.");
            }
        }

        /// <summary>
        /// Registra en bitácora de base de datos la autorización de generación de persona moral
        /// </summary>
        /// <param name="idPersonaMoral">Identificador de la persona moral</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaAutorizacionPersonaMoral(long idPersonaMoral, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_AutorizaPersonaMoral", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPM", idPersonaMoral));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_AutorizaPersonaMoral";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPM=" + idPersonaMoral);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        log.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                log.ErrorException(Ex);
                throw new CAppException(8010, "Error al registrar la autorización de la persona moral en base de datos");
            }
        }

        /// <summary>
        /// Establece el estatus en base de datos de la persona moral a En Revisión
        /// </summary>
        /// <param name="idPersonaMoral">Identificador de la persona moral</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusPM_EnRevision(long idPersonaMoral, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EstatusPersonaMoralEnRevision", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPM", idPersonaMoral));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_EstatusPersonaMoralEnRevision";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPM=" + idPersonaMoral);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                logPCI.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus en revisión de la persona moral " +
                    "en base de datos");
            }
        }

        /// <summary>
        /// Establece el estatus en base de datos de la persona moral a En Proceso
        /// </summary>
        /// <param name="IdPersonaMoral">Identificador de la persona moral</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusPM_EnProceso(long idPersonaMoral, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhookMati.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_EstatusPersonaMoralEnProceso", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdPM", idPersonaMoral));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_EstatusPersonaMoralEnProceso";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdPM=" + idPersonaMoral);
                        parametros.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception Ex)
            {
                log.ErrorException(Ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus en proceso de la persona moral " +
                    "en base de datos");
            }
        }
    }
}
