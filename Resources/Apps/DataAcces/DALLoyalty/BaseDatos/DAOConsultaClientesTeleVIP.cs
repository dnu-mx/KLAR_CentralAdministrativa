using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Clientes TeleVIP
    /// </summary>
    public class DAOConsultaClientesTeleVIP
    {
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
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneClientesTeleVIP");

                database.AddInParameter(command, "@Tag", DbType.String, elCliente.MedioAcceso);
                database.AddInParameter(command, "@ApPaterno", DbType.String, elCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, elCliente.ApellidoMaterno);
                database.AddInParameter(command, "@Nombre", DbType.String, elCliente.Nombre);
                database.AddInParameter(command, "@IdCliente", DbType.Int32, elCliente.ID_Cliente);
                database.AddInParameter(command, "@eMail", DbType.String, elCliente.Email);
                database.AddInParameter(command, "@Cuenta", DbType.Int32, elCliente.ID_Cuenta);
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
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneDatosClienteTeleVIP");

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
        /// Obtiene los tags ligados al cliente en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneTagsCliente(int idCliente, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_ObtieneTagsClienteTeleVIP");

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
        /// Obtiene de base de datos el resumen de los últimos meses del tag indicado
        /// </summary>
        /// <param name="tag">Clave del tag</param>
        /// <param name="cta">Cuenta Televia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneResumenMeses(string tag, string cta, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("ws_Televia_ObtieneEstatusCumplimientoPeriodoActual");

                database.AddInParameter(command, "@CuentaTelevia", DbType.String, cta);
                database.AddInParameter(command, "@Tag", DbType.String, tag);
                
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene de base de datos los movimientos del tag indicado
        /// </summary>
        /// <param name="fIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fFin">Fecha final del periodo de consulta</param>
        /// <param name="tag">Clave del tag</param>
        /// <param name="cta">Cuenta Televia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneMovimientosTag(DateTime fIni, DateTime fFin, string tag, string cta, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("ws_Televia_ObtieneTrayectosPorPeriodoFechas");

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fFin);
                database.AddInParameter(command, "@Tag", DbType.String, tag);
                database.AddInParameter(command, "@ClaveTelevia", DbType.String, cta);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene de base de datos las recompensas del tag indicado
        /// </summary>
        /// <param name="tag">Clave del tag</param>
        /// <param name="cta">Cuenta Televia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneRecompensas(string tag, string cta, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("ws_Televia_ObtieneRecompensasporTagoCuenta");

                database.AddInParameter(command, "@ClaveTelevia", DbType.String, cta);
                database.AddInParameter(command, "@Tag", DbType.String, tag);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
