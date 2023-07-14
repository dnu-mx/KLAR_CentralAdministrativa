using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
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

namespace DALMonitoreo.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Alertamiento de Aplicativos
    /// </summary>
    public class DAOAlertamientoAplicativos
    {
        /// <summary>
        /// Obtiene la lista de aplicativos en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ListaAplicativos(Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoAplicativos");

                //database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                //database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCatalogoAplicativos";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                //Dictionary<string, string> parametros = new Dictionary<string, string>();
                //parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                //parametros.Add("P2", "@AppId=" + AppID);
                //logDebug.Parameters = parametros;

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el catálogo de aplicativos en base de datos.");
            }
        }

        /// <summary>
        /// Obtiene los alertamientos por aplicativo con los filtros indicados en base de datos
        /// </summary>
        /// <param name="estatus">Estatus del alertamiento (activo/inactivo)</param>
        /// <param name="laFecha">Fecha de creación de la alerta</param>
        /// <param name="idAplicativo">Identificador del aplicativo</param>
        /// <param name="instancia">Nombre de la instancia donde se generó el alertamiento</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ObtieneAlertamientosAplicativo(int estatus, DateTime laFecha, int idAplicativo, 
            string instancia, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneAlertamientosAplicativos");

                command.Parameters.Add(new SqlParameter("@Estatus", estatus));
                command.Parameters.Add(new SqlParameter("@Fecha",
                    laFecha.Equals(DateTime.MinValue) ? DBNull.Value : (object)laFecha));
                command.Parameters.Add(new SqlParameter("@IdAplicativo", idAplicativo));
                command.Parameters.Add(new SqlParameter("@Instancia", instancia));

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneCatalogoAplicativos";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@Estatus=" + estatus.ToString());
                parametros.Add("P2", "@Fecha=" + laFecha.ToShortDateString());
                parametros.Add("P3", "@IdAplicativo=" + idAplicativo.ToString());
                parametros.Add("P4", "@Instancia=" + instancia);
                logDebug.Parameters = parametros;

                logPCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los alertamientos en base de datos.");
            }
        }

        /// <summary>
        /// Desactiva el alertamiento de aplicativo con el ID indicados en base de datos
        /// </summary>
        /// <param name="idAlerta">Identificador del alertamiento</param>
        /// <param name="comments">Comentarios del cierre</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void DesactivaAlertamiento(int idAlerta, string comments, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_DesactivaAlertamientoAplicacion", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add((new SqlParameter("@IdAlertamiento", idAlerta)));
                        command.Parameters.Add((new SqlParameter("@Comentarios", comments)));
                        command.Parameters.Add((new SqlParameter("@Usuario", elUsuario.ClaveUsuario)));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDebug = new LogDebugMsg();
                        logDebug.M_Value = "web_CA_DesactivaAlertamientoAplicacion";
                        logDebug.C_Value = "";
                        logDebug.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdAlertamiento=" + idAlerta.ToString());
                        parametros.Add("P2", "@Comentarios=" + comments);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDebug.Parameters = parametros;

                        log.Debug(logDebug);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8010, "Ocurrió un error al desactivar el alertamiento en base de datos");
            }
        }
    }
}
