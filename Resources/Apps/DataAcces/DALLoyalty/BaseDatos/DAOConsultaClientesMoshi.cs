using DALAutorizador.BaseDatos;
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
    public class DAOConsultaClientesMoshi
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
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);

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
        public static void ActualizaDatosCliente(ClienteColectiva datosCliente, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaDatosCliente");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, datosCliente.ID_Cliente);
                database.AddInParameter(command, "@Nombre", DbType.String, datosCliente.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, datosCliente.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, datosCliente.ApellidoMaterno);
                database.AddInParameter(command, "@fechaNac", DbType.DateTime, datosCliente.FechaNacimiento);

                database.AddInParameter(command, "@correoEmpresa", DbType.String, datosCliente.CorreoEmpresa);
                database.AddInParameter(command, "@ClaveEmpresa", DbType.String, datosCliente.ClaveEmpresa);
                database.AddInParameter(command, "@lugarTrabajo", DbType.String, datosCliente.LugarTrabajo);

                database.ExecuteNonQuery(command);
                
                Loguear.Evento("Se Actualizaron los Datos del Cliente con ID_Colectiva " + datosCliente.ID_Cliente.ToString() + 
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los alias de las direcciones del cliente seleccionado en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneAliasDireccionesCliente(int idCliente, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneAliasDomiciliosCliente");

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
        /// Consulta las direcciones del cliente seleccionado en base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDireccionesCliente(int idDomicilio, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDomicilioCliente");

                database.AddInParameter(command, "@IdDomicilio", DbType.Int32, idDomicilio);

                return database.ExecuteDataSet(command);
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
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
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
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
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
        /// Consulta el catálogo de niveles de lealtad en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneNivelesLealtad(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNivelesLealtad");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el nivel de lealtad del cliente en base de datos
        /// </summary>
        /// <param name="idCol">Identificador de la colectiva del cliente a actualizar</param>
        /// <param name="idNivel">Identificador del nivel de lealtad por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaNivelLealtad(int idCol, int idNivel, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaNivelLealtad");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idCol);
                database.AddInParameter(command, "@IdGpoCuenta", DbType.Int32, idNivel);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó el Grupo de Cuentas  del Cliente con ID_Colectiva " + idCol.ToString() +
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos los datos básicos del evento "Ajuste de Cargo" 
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoAjusteCargo(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosEventoAjusteCargo");

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
        /// Consulta en base de datos los datos básicos del evento "Ajuste de Abono" 
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoAjusteAbono(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosEventoAjusteAbono");

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
        /// Consulta en base de datos los datos básicos del evento "Ajuste Cargo Visitas" 
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoAjusteCargoVisitas(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventoAjusteCargoVisitas");

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
        /// Consulta en base de datos los datos básicos del evento "Ajuste Abono Visitas" 
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoAjusteAbonoVisitas(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEventoAjusteAbonoVisitas");

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
        /// Consulta el catálogo de estatus de medios de acceso en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneEstatusMediosAcceso(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusMedioAcceso");

                return database.ExecuteDataSet(command);
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
        /// <param name="email">Correo electrónico (medio de acceso) del cliente</param>
        /// <param name="idEstatus">Identificador del estatus de medio de acceso por establecer</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActivaInactivaMedioAcceso(String email, int idEstatus, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActivaInactivaMedioAcceso");

                database.AddInParameter(command, "@ClaveMA", DbType.String, email);
                database.AddInParameter(command, "@IdEstatusMA", DbType.Int32, idEstatus);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó el Estatus del Medio de Acceso " + email +
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el motivo del estatus del medio de acceso en base de datos
        /// </summary>
        /// <param name="idCol">Identificador de la colectiva dueña del medio de acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static String ConsultaMotivoEstatusMA(int idCol, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMotivoEstatusMedioAcceso");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idCol);

                object o = database.ExecuteScalar(command);

                return o == null ? "" : o.ToString();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        public static DataSet GetEmpresas(Usuario elUsuario, Guid AppId)
        {

            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_lista");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Empresa>();
                //vat test = (DataTable)dataset;
                return dataset;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

    }
}
