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
    public class DAOConsultaClientesCuponClick
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
                SqlDatabase database = new SqlDatabase(BDEcommerceCuponClick.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClientesCuponClick");

                database.AddInParameter(command, "@Nombre", DbType.String, elCliente.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, elCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, elCliente.ApellidoMaterno);
                database.AddInParameter(command, "@fechaNac", DbType.DateTime, 
                    DateTime.Compare(elCliente.FechaNacimiento, DateTime.MinValue) == 0 ? (DateTime?)null : elCliente.FechaNacimiento);
                database.AddInParameter(command, "@eMail", DbType.String, elCliente.Email);
                database.AddInParameter(command, "@Membresia", DbType.String, elCliente.Membresia);

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
                SqlDatabase database = new SqlDatabase(BDEcommerceCuponClick.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosClienteCuponClick");

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
                SqlDatabase database = new SqlDatabase(BDAdminCuponClick.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusBloqueoClienteCuponClick");

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
                SqlDatabase database = new SqlDatabase(BDAdminCuponClick.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaEstatusBloqueoClienteCuponClick");

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
        /// Actualiza el estatus de confirmacion del Cliente en base de datos
        /// </summary>
        /// <param name="IdColectiva">Id Colectiva del cliente</param>
        /// <param name="idEstatus">Identificador del estatus de medio de acceso por establecer</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaEstatusConfirma(int IdColectiva, int idEstatus, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCuponClick.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaEstatusClienteCuponClick");

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
                SqlDatabase database = new SqlDatabase(BDAdminCuponClick.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaContrasenaClienteCuponClick");

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

        /// <summary>
        /// Activa o inactiva el medio de acceso (email) del cliente en base de datos
        /// </summary>
        /// <param name="IdUsuario">Id Usuario del cliente</param>
        /// <param name="Membresia">Nueva membresia del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaMembresiaCliente(string Correo, string Membresia, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceCuponClick.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaMembresiaClienteCuponClick");

                database.AddInParameter(command, "@Correo", DbType.String, Correo);
                database.AddInParameter(command, "@membresia", DbType.String, Membresia);
                database.AddInParameter(command, "@Usuario", DbType.String, elUsuario.ClaveUsuario);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó la Membresia del Usuario " + Correo, elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
