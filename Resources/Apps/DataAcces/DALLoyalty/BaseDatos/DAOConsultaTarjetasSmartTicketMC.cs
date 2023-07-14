using DALCentroContacto.Entidades;
using DALLoyalty.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Tarjetas Smart Ticket
    /// </summary>
    public class DAOConsultaTarjetasSmartTicketMC
    {
        /// <summary>
        /// Consulta el catálogo de estatus de tarjeta en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoEstatusTarjeta(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusTarjeta");

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
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneClientes(Cliente elCliente, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClientesTarjetas");

                database.AddInParameter(command, "@Nombre", DbType.String, elCliente.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, elCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, elCliente.ApellidoMaterno);
                database.AddInParameter(command, "@eMail", DbType.String, elCliente.Email);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las tarjetas del cliente seleccionado en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneTarjetasCliente(int idCliente, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTarjetasCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el estatus de la tarjeta del cliente en base de datos
        /// </summary>
        /// <param name="idTarjeta">ID de la tarjeta</param>
        /// <param name="idEstatus">ID del nuevo estatus de la tarjeta</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusTarjetaCliente(int idTarjeta, int idEstatus, SqlConnection connection, 
            SqlTransaction transaccionSQL, IUsuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusTarjetaCliente", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdTarjeta", idTarjeta));
                command.Parameters.Add(new SqlParameter("@IdEstatus", idEstatus));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();

                Loguear.Evento("Se actualizó el estatus de la tarjeta con ID " + idTarjeta.ToString() + 
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception Ex)
            {
                throw new Exception("ActualizaEstatusTarjetaCliente()", Ex);
            }
        }
    }
}
