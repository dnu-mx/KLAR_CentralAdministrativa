using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentroContacto.Entidades;
using DALLoyalty.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Clientes
    /// </summary>
    public class DAOConsultaClientesSmartTicketMC
    {
        /// <summary>
        /// Consulta los clientes que coinciden con los datos de ingreso en base de datos
        /// </summary>
        /// <param name="elCliente">Parámetros de búsqueda de clientes</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneClientes(ClienteColectiva elCliente, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClientes");

                database.AddInParameter(command, "@Nombre", DbType.String, elCliente.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, elCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, elCliente.ApellidoMaterno);
                database.AddInParameter(command, "@fechaNac", DbType.DateTime, 
                    DateTime.Compare(elCliente.FechaNacimiento, DateTime.MinValue) == 0 ? (DateTime?)null : elCliente.FechaNacimiento);
                database.AddInParameter(command, "@eMail", DbType.String, elCliente.Email);
                database.AddInParameter(command, "@Telefono", DbType.String, elCliente.Telefono);

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
        /// <param name="idColectiva">Identificador de la colectiva </param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosCliente(int idCliente, int idColectiva, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosClienteSmartTicket");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
                database.AddInParameter(command, "@IdColectiva", DbType.Int32, Convert.ToInt32(idColectiva));

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta estatus de bloqueo del cliente seleccionado en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="idColectiva">Identificador de la colectiva </param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneBloqueoCliente(string Correo, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEAdminEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusBloqueoClienteSmartTicket");

                database.AddInParameter(command, "@Correo", DbType.String, Correo);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el estatus de bloqueo por contracargo del cliente en base de datos
        /// </summary>
        /// <param name="datosCliente">Datos del cliente a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusBloqueoCliente(string Correo, bool Estatus, string Comentarios, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEAdminEcommerceMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaEstatusBloqueoClienteSmartTicket");

                database.AddInParameter(command, "@Correo", DbType.String, Correo);
                database.AddInParameter(command, "@estatus", DbType.String, Estatus);
                database.AddInParameter(command, "@comentarios", DbType.String, Comentarios);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó el Estatus de Bloqueo del Cliente " + Correo +
                    " en Administrador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene de base de datos los pedidos del cliente en el periodo indicado
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="fIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fFin">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtienePedidos(ClienteColectiva elCliente, DateTime fIni, DateTime fFin, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePedidosCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, elCliente.ID_Cliente);
                database.AddInParameter(command, "@eMail", DbType.String, elCliente.Email);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, fIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fFin);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene de base de datos los movimientos de lealtad del cliente en el periodo indicado
        /// </summary>
        /// <param name="fIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fFin">Fecha final del periodo de consulta</param>
        /// <param name="idCol">Identificador de la colectiva del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros</returns>
        public static DataSet ObtieneMovimientos(DateTime fIni, DateTime fFin, int idCol, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorRedVoucherMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMovimientosMoshi");

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fFin);
                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idCol);

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
        /// Actualiza el estatus de confirmacion del Cliente en base de datos
        /// </summary>
        /// <param name="IdColectiva">Id Colectiva del cliente</param>
        /// <param name="idEstatus">Identificador del estatus de medio de acceso por establecer</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusConfirma(int IdColectiva, int idEstatus, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorRedVoucherMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaEstatusClienteSmartTicket");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, Convert.ToInt32(IdColectiva));
                database.AddInParameter(command, "@EstatusConfirma", DbType.Int32, idEstatus);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó el Estatus de Confirmación de correo del ID Colectiva " + IdColectiva.ToString() +
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Activa o inactiva el medio de acceso (email) del cliente en base de datos
        /// </summary>
        /// <param name="IdUsuario">Id Usuario del cliente</param>
        /// <param name="contrasena">Nueva contraseña del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaContrasenaCliente(string Correo, string Contrasena, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEAdminEcommerceMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaContrasenaClienteSmartTicket");

                database.AddInParameter(command, "@Correo", DbType.String, Correo);
                database.AddInParameter(command, "@Contrasena", DbType.String, Contrasena);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó la Contraseña del Usuario " + Correo +
                    " en el Administrador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
