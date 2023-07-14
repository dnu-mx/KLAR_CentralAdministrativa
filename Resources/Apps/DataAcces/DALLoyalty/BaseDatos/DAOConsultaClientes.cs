using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.Entidades;
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

namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Clientes
    /// </summary>
    public class DAOConsultaClientes
    {
        /// <summary>
        /// Consulta las Cadenas Comerciales en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCadenasComerciales(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneCadenasComerciales");

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
        /// Consulta los clientes que coinciden con los datos de ingreso en base de datos
       /// </summary>
        /// <param name="elCliente">Parámetros de búsqueda de clientes</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneClientes(Cliente elCliente, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneClientes");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, elCliente.ID_Cadena);
                database.AddInParameter(command, "@NumTarjeta", DbType.String, elCliente.MedioAcceso);
                database.AddInParameter(command, "@ApPaterno", DbType.String, elCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, elCliente.ApellidoMaterno);
                database.AddInParameter(command, "@Nombre", DbType.String, elCliente.Nombre);
                database.AddInParameter(command, "@IdCliente", DbType.Int32, elCliente.ID_Cliente);
                database.AddInParameter(command, "@eMail", DbType.String, elCliente.Email);
                database.AddInParameter(command, "@fechaNac", DbType.DateTime, DateTime.Compare(elCliente.FechaNacimiento, DateTime.MinValue) == 0 ? (DateTime?)null : elCliente.FechaNacimiento);
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
        /// Consulta los datos del cliente seleccionado en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosCliente(int idCliente, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneDatosCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
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
        /// Obtiene los datos de colonia, municipio y estado a partir del código postal indicado.
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaDatosPorCodigoPostal(string codigoPostal, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneCMEDeCodigoPostal");

                database.AddInParameter(command, "@CP", DbType.String, codigoPostal);
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
        /// Actualiza los datos del cliente en base de datos
        /// </summary>
        /// <param name="datosCliente">Datos del cliente a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void actualizaCliente(Cliente datosCliente, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ActualizaDatosCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, datosCliente.ID_Cliente);
                database.AddInParameter(command, "@ApPaterno", DbType.String, datosCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, datosCliente.ApellidoMaterno);
                database.AddInParameter(command, "@Nombre", DbType.String, datosCliente.Nombre);
                database.AddInParameter(command, "@eMail", DbType.String, datosCliente.Email);
                database.AddInParameter(command, "@Telefono", DbType.String, datosCliente.Telefono);
                database.AddInParameter(command, "@fechaNac", DbType.DateTime, datosCliente.FechaNacimiento);
                database.AddInParameter(command, "@IdDireccion", DbType.Int32, datosCliente.IdDireccion);
                database.AddInParameter(command, "@IdColonia", DbType.Int32, datosCliente.IdColonia);
                database.AddInParameter(command, "@DescColonia", DbType.String, datosCliente.Colonia);
                database.AddInParameter(command, "@CP", DbType.String, datosCliente.CodigoPostal);
                database.AddInParameter(command, "@ClaveMpio", DbType.String, datosCliente.ClaveMunicipio);
                database.AddInParameter(command, "@ClaveEdo", DbType.String, datosCliente.ClaveEstado);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Actualizaron los Datos del Cliente en el Autorizador de Lealtad", elUsuario.ClaveUsuario);
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
        /// <param name="MA">Clave del medio de acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la apliación</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneMovimientosTX (DateTime fIni, DateTime fFin, int idTC, int idTop, int idMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneMovimientosMA");

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fFin);
                database.AddInParameter(command, "@IdTipoCuenta", DbType.Int32, idTC);
                database.AddInParameter(command, "@IdTipoOperacion", DbType.Int32, idTop);
                database.AddInParameter(command, "@IdMA", DbType.Int32, idMA);
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
        /// Obtiene los medios de acceso ligados al cliente en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneMediosAccesoCliente(int idCliente, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneMediosAccesoCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
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
        /// Realiza la solicitud de modificación del estatus del medio de acceso elegido a base de datos
        /// </summary>
        /// <param name="idMA">Identificador del medio de acceso</param>
        /// <param name="idEstatusMA">Identificador del estatus actual del medio de acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaEstatusMA(int idMA, int idEstatusMA, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ActualizaEstatusMedioAcceso");

                database.AddInParameter(command, "@IdMA", DbType.Int32, idMA);
                database.AddInParameter(command, "@IdEstatusActual", DbType.Int32, idEstatusMA);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Modificó el Estatus del Medio de Acceso del Cliente en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta en la bitácora de detalle de base de datos la razón de la cancelación de la tarjeta o MDA
        /// </summary>
        /// <param name="idMA">Identificador del medio de acceso</param>
        /// <param name="comentarios">Comentarios por los que se está cancelando el MA</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaRazonBitacoraDetalle(int idMA, string comentarios, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_InsertaMDAEstatusCanceladoBitacoraDetalle");

                database.AddInParameter(command, "@IdMA", DbType.Int32, idMA);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@Razon", DbType.String, comentarios);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó la Razón de la Cancelación del MDA en la Bitácora del Autorizador", elUsuario.ClaveUsuario);
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
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
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
        /// Consulta los bines asociados al producto con el ID indicado en base de datos
        /// </summary>
        /// <param name="idProducto">Identificador del producto</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneMotivosLlamada(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienMotivosLlamada");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtienMotivosLlamada";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los motivos de llamada en base de datos");
            }
        }

        /// <summary>
        /// Inserta en la bitácora de llamadas de base de datos  el registro correspondiente
        /// </summary>
        /// <param name="registro">Objeto con los detalles de la llamada a registrar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaLlamada(RegistroLlamada registro, IUsuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaRegistroBitacoraLlamada", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMotivoLlamada", registro.ID_MotivoLlamada));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@Parametros", registro.ParametrosLlamada));
                        command.Parameters.Add(new SqlParameter("@Observaciones", registro.Comentarios));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_InsertaNuevoSubproductoAProducto";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> paramsLog = new Dictionary<string, string>();
                        paramsLog.Add("P1", "@IdMotivoLlamada=" + registro.ID_MotivoLlamada.ToString());
                        paramsLog.Add("P2", "@Usuario=" + elUsuario.ClaveUsuario);
                        paramsLog.Add("P3", "@Parametros=" + registro.ParametrosLlamada);
                        paramsLog.Add("P4", "@Observaciones=" + registro.Comentarios);
                        logDebug.Parameters = paramsLog;

                        logPCI.Debug(logDebug);
                        /////<<<LOG DEBUG

                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el registro de llamada en base de datos");
            }
        }
    }
}
