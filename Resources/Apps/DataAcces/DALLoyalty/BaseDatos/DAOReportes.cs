using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALLoyalty.BaseDatos;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace DALCentroContacto.BaseDatos
{
    public class DAOReportes
    {
        public static DataSet obtenerReporteClientes(Usuario elUsuario, Guid AppId, DateTime fechaInicio, DateTime fechaFin, string tipoColectiva = null, string sucursal = null, string tipoMedioAcceso = null, string email = null)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                if (tipoColectiva == "-1") tipoColectiva = null;
                if (sucursal == "-1") sucursal = null;
                if (tipoMedioAcceso == "-1") tipoMedioAcceso = null;
                if (email == "") email = null;
                DbCommand command = database.GetStoredProcCommand("web_Reporte_Clientes");
                database.AddInParameter(command, "@fechaInicio", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@fechaFin", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, tipoColectiva);
                database.AddInParameter(command, "@sucursal", DbType.Int64, sucursal);
                database.AddInParameter(command, "@tipoMedioAcceso", DbType.Int64, tipoMedioAcceso);
                database.AddInParameter(command, "@email", DbType.String, email);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet obtenerReporteOperaciones(Usuario elUsuario, Guid AppId, DateTime fechaInicio, DateTime fechaFin, string tipoColectiva = null, string sucursal = null, string tipoTransaccion = null)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                if (sucursal == "") sucursal = null;
                if (tipoTransaccion == "") tipoTransaccion = null;
                DbCommand command = database.GetStoredProcCommand("web_Reporte_Operaciones");
                database.AddInParameter(command, "@fechaInicio", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@fechaFin", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, tipoColectiva);
                database.AddInParameter(command, "@sucursal", DbType.Int64, sucursal);
                database.AddInParameter(command, "@tipoTransaccion", DbType.String, tipoTransaccion);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet obtenerReporteBeneficios(Usuario elUsuario, Guid AppId, DateTime fechaInicio, DateTime fechaFin, string tipoColectiva = null, string sucursal = null, string tipoPromocion = null)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                if (sucursal == "") sucursal = null;
                if (tipoPromocion == "") tipoPromocion = null;
                DbCommand command = database.GetStoredProcCommand("web_Reporte_Descuentos");
                database.AddInParameter(command, "@fechaInicio", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@fechaFin", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, tipoColectiva);
                database.AddInParameter(command, "@sucursal", DbType.Int64, sucursal);
                database.AddInParameter(command, "@tipoPromocion", DbType.String, tipoPromocion);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ObtenerReporteTiemposRepartoMoshi(DateTime fechaIni, DateTime fechaFin, int IdSuc,
            int idTServicio, int IdR, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_Reportes_ObtieneTiemposEntregaPedidos");

                database.AddInParameter(command, "@Fechainicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@idSucursal", DbType.Int32, IdSuc);
                database.AddInParameter(command, "@idTipoServicio", DbType.Int32, idTServicio);
                database.AddInParameter(command, "@idRepartidor", DbType.Int32, IdR);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        public static DataSet obtenerReporteActividades(Usuario elUsuario, Guid AppId, DateTime fechaInicio, DateTime fechaFin, string tipoColectiva = null)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_Actividades");
                database.AddInParameter(command, "@fechaInicio", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@fechaFin", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, tipoColectiva);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet obtenerReporteLlamadas(Usuario elUsuario, Guid AppId, DateTime fechaInicio, DateTime fechaFin, string tipoColectiva = null)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);

                DbCommand command = database.GetStoredProcCommand("web_Reporte_Llamadas");
                database.AddInParameter(command, "@fechaInicio", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@fechaFin", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, tipoColectiva);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el reporte de llamadas Tiendas Diconsa dentro del periodo indicado en base de datos
        /// </summary>
        /// <param name="fechaIni">Fecha inicial de consulta</param>
        /// <param name="fechaFin">Fecha final de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del reporte</returns>
        public static DataSet ObtenerReporteLlamadasTiendasDiconsa(DateTime fechaIni, DateTime fechaFin, Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_LlamadasTiendasDiconsa");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteDataSet(command);
                //vat test = (DataTable)dataset;
                return dataset;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static object ListaSucursalesMoshi(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursales");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static object ListaTiposServicio(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_lista_tiposservicios");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static object ListaRepartidores(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_lista_repartidores");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        internal static DataSet ObtenerReportePedidosMoshi(DateTime fechaIni, DateTime fechaFin, int idSuc,
            int idTipoServ, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_Reportes_Pedidos");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@idSucursal", DbType.Int32, idSuc);
                database.AddInParameter(command, "@idTipoServicio", DbType.Int32, idTipoServ);

                var values = database.ExecuteDataSet(command);

                return values;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        internal static DataSet ObtenerReporteLealtadMoshi(DateTime fechaIni, DateTime fechaFin, string sucursal,
            int idTipoMov, Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_Reporte_LealtadMoshi");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@Sucursal", DbType.String, sucursal);
                database.AddInParameter(command, "@IdTipoMovimiento ", DbType.Int32, idTipoMov);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static object ListaTiposDeMovimientosMoshi(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposMovimientos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataTable ObtenerReporteClientesV2(Usuario elUsuario, DateTime fechaInicio, DateTime fechaFin, int status, string dominioCorreo, int? nivelLealtad, int? estatusUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerce.strBDLectura);


                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteCliente");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@DominioCorreo", DbType.String, dominioCorreo);
                database.AddInParameter(command, "@IdNivel", DbType.Int32, nivelLealtad);
                database.AddInParameter(command, "@EstatusUsuario", DbType.Int32, estatusUsuario);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de niveles de lealtad en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoNivelesLealtad()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNivelesLealtad");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de estatus de colectiva del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoEstatusColectiva(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusColectiva");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="telefono"></param>
        /// <param name="idNivelLealtad"></param>
        /// <param name="idEstatusUsuario"></param>
        /// <param name="elUsuario"></param>
        /// <returns></returns>
        public static DataTable ObtenerReporteClientesCash(DateTime fechaInicio, DateTime fechaFin, string telefono,
            int idNivelLealtad, int idEstatusUsuario, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteClientes");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);
                database.AddInParameter(command, "@Telefono", DbType.String, telefono);
                database.AddInParameter(command, "@IdNivelLealtad", DbType.Int32, idNivelLealtad);
                database.AddInParameter(command, "@IdEstatusColectiva", DbType.Int32, idEstatusUsuario);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo  de sucursales en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del catálogo</returns>
        public static DataTable ListaSucursalesCash(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneSucursales");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo  de tipos de movimientos de lealtad en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del catálogo</returns>
        public static DataTable ListaTiposDeMovimientosCash(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposMovimientos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el reporte de movimientos de lealtad para Cash en el Autorizador
        /// </summary>
        /// <param name="fechaIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="claveSucursal">Identificador de la sucursal</param>
        /// <param name="idTipoMov">Identificador del tipo de movimiento (evento)</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtenerReporteLealtadCash(DateTime fechaIni, DateTime fechaFin, string claveSucursal,
             int idTipoMov, string medioAcceso, Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteLealtad");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@ClaveSucursal", DbType.String, claveSucursal);
                database.AddInParameter(command, "@IdTipoMovimiento ", DbType.Int32, idTipoMov);
                database.AddInParameter(command, "@MedioAcceso", DbType.String, medioAcceso);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de niveles de lealtad en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoNivelesLealtadCash()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNivelesLealtad");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el reporte de movimientos de Monitoreo
        /// </summary>
        /// <param name="fechaIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="usuario">Identificador del usuario</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneDatosMonitoreo(DateTime fechaIni, DateTime fechaFin, string usuario,
            Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ReporteDatosMonitoreo");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@Usuario", DbType.String, usuario);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el reporte de movimientos de Parco
        /// </summary>
        /// <param name="fechaIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="nombre">Nombre del cliente</param>
        /// <param name="primerApellido">Primer Apellido del cliente</param>
        /// <param name="segundoApellido">Segundo Apellido del cliente</param>
        /// <param name="correo">Segundo Apellido del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtenerReporteClientesParco(DateTime fechaIni, DateTime fechaFin, string nombre,
             string primerApellido, string segundoApellido, string correo, Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceParco.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteClientesPARCO");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@nombre", DbType.String, nombre);
                database.AddInParameter(command, "@ApellidoPat", DbType.String, primerApellido);
                database.AddInParameter(command, "@ApellidoMat", DbType.String, segundoApellido);
                database.AddInParameter(command, "@correo", DbType.String, correo);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el reporte de movimientos de CuponClick
        /// </summary>
        /// <param name="fechaIni">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFin">Fecha final del periodo de consulta</param>
        /// <param name="nombre">Nombre del cliente</param>
        /// <param name="primerApellido">Primer Apellido del cliente</param>
        /// <param name="segundoApellido">Segundo Apellido del cliente</param>
        /// <param name="correo">Segundo Apellido del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtenerReporteClientesCuponClick(DateTime fechaIni, DateTime fechaFin, string nombre,
             string primerApellido, string segundoApellido, string correo, Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceCuponClick.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteClientesCC");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFin);
                database.AddInParameter(command, "@nombre", DbType.String, nombre);
                database.AddInParameter(command, "@ApellidoPat", DbType.String, primerApellido);
                database.AddInParameter(command, "@ApellidoMat", DbType.String, segundoApellido);
                database.AddInParameter(command, "@correo", DbType.String, correo);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los usuarios que existen en base de datos AppConnect
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneUsuariosAppConnect(ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAppConnect.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneUsuarios");
                command.CommandTimeout = 0;

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneUsuarios";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los usuarios en base de datos");
            }
        }

        /// <summary>
        /// Consulta el historial de llamadas, asociadas al ID del medio de acceso indicado, en base de datos
        /// </summary>
        /// <param name="Id_MedioAcceso">Identificador del medio de acceso</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneHistorialLlamadas(int Id_MedioAcceso, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_HistorialLlamadas");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdMA", DbType.Int32, Id_MedioAcceso);
                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_Reporte_HistorialLlamadas";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> paramsLog = new Dictionary<string, string>();
                paramsLog.Add("P1", "@IdMA=" + Id_MedioAcceso.ToString());
                logDbg.Parameters = paramsLog;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el historial de llamadas en base de datos");
            }
        }

        /// <summary>
        /// Consulta el reporte de llamadas por subemisor, dentro del periodo indicado, en base de datos
        /// </summary>
        /// <param name="idSubemisor">Identificador de la colectiva tipo subemisor</param>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos obtenidos</returns>
        public static DataTable ObtieneLlamadasSubemisor(int idSubemisor, DateTime fechaInicial, DateTime fechaFinal,
            ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_LlamadasTarjetahabiente");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdSubemisor", DbType.Int32, idSubemisor);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, fechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, fechaFinal);
                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_Reporte_LlamadasTarjetahabiente";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> paramsLog = new Dictionary<string, string>();
                paramsLog.Add("P1", "@IdSubemisor=" + idSubemisor.ToString());
                paramsLog.Add("P2", "@FechaInicial=" + fechaInicial.ToShortDateString());
                paramsLog.Add("P3", "@FechaFinal=" + fechaFinal.ToShortDateString());
                logDbg.Parameters = paramsLog;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las llamadas del subemisor en base de datos");
            }
        }
    }
}
