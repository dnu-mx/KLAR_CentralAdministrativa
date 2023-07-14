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

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOCVDP
    {
        /// <summary>
        /// Consulta el reporte de los medios de acceso
        /// </summary>
        /// <param name="descripcion">Descripcion de grupo medioas acceso</param>
        /// <param name="elUsuario">Usuarien sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static DataSet ObtieneGruposMa(string descripcion, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);               
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGMAPorDescripcion");

                database.AddInParameter(command, "@Descripcion", DbType.String, descripcion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneGMAPorDescripcion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@Descripcion=" + descripcion);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppID=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                ///<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al consultar los grupos de tarjetas en base de datos");
            }
        }

        /// <summary>
        /// Consulta las claves del grupo de medios acceso
        /// </summary>
        /// <param name="idGrupoMA">Identificador  de Grupo medios acceso</param>
        /// <param name="elUsuario">Usuarien sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static DataSet ObtieneValoresParametros(int idGrupoMA, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresReferidosGrupoMA");

                database.AddInParameter(command, "@IdGrupoMA", DbType.Int32, idGrupoMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneValoresReferidosGrupoMA";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";


                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdGrupoMA=" + idGrupoMA);
                parametros.Add("P2", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P3", "@AppID=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                ///<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al consultar los parámetros en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el valor de la pertenencia en el grupoMA
        /// </summary>
        /// <param name="idGpoMA">Identificador del grupo de medios de accso</param>
        /// <param name="idValorRef">Identificador del valor referido</param>
        /// <param name="Valor">Nuevo valor</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaValorReferidoPertenencia(int idGpoMA, int idValorRef, string Valor, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);                
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaPertenenciaValoresReferidos");

                database.AddInParameter(command, "@IdGrupoMA", DbType.Int32, idGpoMA);
                database.AddInParameter(command, "@IdValorReferido", DbType.Int32, idValorRef);
                database.AddInParameter(command, "@Valor", DbType.String, Valor);

                int resp = database.ExecuteNonQuery(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ActualizaPertenenciaValoresReferidos";
                logDbg.C_Value = "";
                logDbg.R_Value = resp.ToString();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdGrupoMA=" + idGpoMA);
                parametros.Add("P2", "IdValorReferido=" + idValorRef);
                parametros.Add("P3", "@Valor=" + Valor);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                ///<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al actualizar el valor referido en base de datos");
            }
        }
    }
}
