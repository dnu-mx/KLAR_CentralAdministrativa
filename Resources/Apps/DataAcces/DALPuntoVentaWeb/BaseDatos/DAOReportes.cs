using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using log4net.Util;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace DALPuntoVentaWeb.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Reportes
    /// </summary>
    public class DAOReportes
    {
        public class Params3DES
        {
            public String Key       { get; set; }
            public String Vector    { get; set; }
        }

        /// <summary>
        /// Obtiene el "catálogo" de tipos de tarjeta de base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposTarjeta(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_PtoVta_ObtieneTiposTarjeta");

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consuta en base de datos las operaciones transaccionales de las tiendas Diconsa
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial de consulta</param>
        /// <param name="FechaFinal">Fecha final de consulta</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="Beneficiario">Marca</param>
        /// <param name="Estatus">Estatus de las TXs</param>
        /// <param name="Telefono">Referencia numérica</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListarOperacionesTransaccionalesTiendaDiconsa(DateTime FechaInicial, DateTime FechaFinal,
          Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen, String Beneficiario, String Estatus,
           String Telefono, Int32 ID_Tienda, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_ObtieneOperacionesTiendaDiconsa");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@Sucursal", DbType.Int32, Sucursal);
                database.AddInParameter(command, "@UnidadOperativa", DbType.Int32, UnidadOperativa);
                database.AddInParameter(command, "@Almacen", DbType.Int32, Almacen);
                database.AddInParameter(command, "@ID_Tienda", DbType.Int32, ID_Tienda);
                database.AddInParameter(command, "@Beneficiario", DbType.String, Beneficiario);
                database.AddInParameter(command, "@ID_Estatus", DbType.String, Estatus);
                database.AddInParameter(command, "@Telefono", DbType.String, Telefono);

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
        /// Consuta en base de datos los cobros con tarjeta de las tiendas Diconsa
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial de consulta</param>
        /// <param name="FechaFinal">Fecha final de consulta</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="TipoTarjeta">Tip de tarjeta</param>
        /// <param name="Estatus">Estatus de las TXs</param>
        /// <param name="Tarjeta">Los 4 últimos dígitos de la tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaCobrosConTarjetaDiconsa(DateTime FechaInicial, DateTime FechaFinal, Int32 Sucursal,
            Int32 UnidadOperativa, Int32 Almacen, Int32 ID_Tienda, String TipoTarjeta, String Estatus, String Tarjeta,
            Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_ObtieneCobrosConTarjetaTiendasDiconsa");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@Sucursal", DbType.Int32, Sucursal);
                database.AddInParameter(command, "@UnidadOperativa", DbType.Int32, UnidadOperativa);
                database.AddInParameter(command, "@Almacen", DbType.Int32, Almacen);
                database.AddInParameter(command, "@ID_Tienda", DbType.Int32, ID_Tienda);
                database.AddInParameter(command, "@Beneficiario", DbType.String, TipoTarjeta);
                database.AddInParameter(command, "@ID_Estatus", DbType.String, Estatus);
                database.AddInParameter(command, "@Tarjeta", DbType.String, Tarjeta);

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
        /// Consuta en base de datos los cortes y saldos de las tiendas Diconsa
        /// </summary>
        /// <param name="Corte">Id del periodo de corte</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaCortesYSaldosDiconsa(int Corte, Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen,
            Int32 ID_Tienda, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DIC_ObtieneDatosCortes");

                database.AddInParameter(command, "@Sucursal", DbType.Int32, Sucursal);
                database.AddInParameter(command, "@UnidadOperativa", DbType.Int32, UnidadOperativa);
                database.AddInParameter(command, "@Almacen", DbType.Int32, Almacen);
                database.AddInParameter(command, "@ID_Tienda", DbType.Int32, ID_Tienda);
                database.AddInParameter(command, "@AntiguedadCorte", DbType.Int32, Corte);

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
        /// Consuta en base de datos las comisiones de las tiendas Diconsa
        /// </summary>
        /// <param name="Corte">Id del periodo de corte</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaComisionesDiconsa(int Corte, Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen,
            Int32 ID_Tienda, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DIC_ObtieneComisionesEnCortes");

                database.AddInParameter(command, "@Sucursal", DbType.Int32, Sucursal);
                database.AddInParameter(command, "@UnidadOperativa", DbType.Int32, UnidadOperativa);
                database.AddInParameter(command, "@Almacen", DbType.Int32, Almacen);
                database.AddInParameter(command, "@ID_Tienda", DbType.Int32, ID_Tienda);
                database.AddInParameter(command, "@AntiguedadCorte", DbType.Int32, Corte);

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
        /// Consuta en base de datos las comisiones acumuladas de las tiendas Diconsa
        /// </summary>
        /// <param name="Corte">Id del periodo de corte</param>
        /// <param name="Sucursal">Sucursal</param>
        /// <param name="UnidadOperativa">Unidad Operativa</param>
        /// <param name="Almacen">Almacén</param>
        /// <param name="ID_Tienda">Identificador de la tienda</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaComisionesAcumuladasDiconsa(int Corte, Int32 Sucursal, Int32 UnidadOperativa, Int32 Almacen,
            Int32 ID_Tienda, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DIC_ObtieneComisionesAcumuladasEnCortes");

                database.AddInParameter(command, "@Sucursal", DbType.Int32, Sucursal);
                database.AddInParameter(command, "@UnidadOperativa", DbType.Int32, UnidadOperativa);
                database.AddInParameter(command, "@Almacen", DbType.Int32, Almacen);
                database.AddInParameter(command, "@ID_Tienda", DbType.Int32, ID_Tienda);
                database.AddInParameter(command, "@AntiguedadCorte", DbType.Int32, Corte);

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
        /// Obtiene las colectivas requeridas para los filtros del reporte de Promociones
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="claveFiltro">Clave del tipo de colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con la información de las colectivas requeridas</returns>
        public static DataSet ObtieneColectivasFiltrosPromociones(int IdCadenaComercial, string claveFiltro,
            Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColectivasFiltrosReportePromociones");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@ClaveTipoColectiva", DbType.String, claveFiltro);
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
        /// Consuta en base de datos las promociones
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial de consulta</param>
        /// <param name="FechaFinal">Fecha final de consulta</param>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="Sucursal">Clave de la sucursal</param>
        /// <param name="Operador">Clave del operador</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ListaPromocionesLoyalty(DateTime FechaInicial, DateTime FechaFinal, int IdCadena,
            String Sucursal, String Operador, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReportePromociones");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@Sucursal", DbType.String, Sucursal);
                database.AddInParameter(command, "@Operador", DbType.String, Operador);

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
        /// Consulta el reporte de operaciones transaccionales
        /// </summary>
        /// <param name="IdGpoComercial">Identificador del grupo comercial</param>
        /// <param name="fIniOp">Fecha inicial de la operación</param>
        /// <param name="fFinOp">Fecha final de la operación</param>
        /// <param name="fIniPres">Fecha inicial de presentación</param>
        /// <param name="fFinPres">Fecha final de presentación</param>
        /// <param name="NumTarjeta">Número de tarjeta</param>
        /// <param name="claveEstatusOp">Clave del estatus de la operación</param>
        /// <param name="idMotRech">Identificador del código de motivo de rechazo</param>
        /// <param name="idEstatusComp">Identificador del estatus de compensación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ListarOperacionesParabilium(Int64 IdGpoComercial, DateTime fIniOp, DateTime fFinOp,
            DateTime fIniPres, DateTime fFinPres, String NumTarjeta, String claveEstatusOp, int idMotRech,
            int idEstatusComp, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_ObtieneOperacionesParabilium");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdColectivaGpoComercial", DbType.Int64, IdGpoComercial);

                database.AddInParameter(command, "@FechaHoraInicialOp", DbType.DateTime,
                    fIniOp.Equals(DateTime.MinValue) ? (DateTime?)null : fIniOp);
                database.AddInParameter(command, "@FechaHoraFinalOp", DbType.DateTime,
                    fFinOp.Equals(DateTime.MinValue) ? (DateTime?)null : fFinOp);

                database.AddInParameter(command, "@FechaInicialPres", DbType.Date,
                    fIniPres.Equals(DateTime.MinValue) ? (DateTime?)null : fIniPres);
                database.AddInParameter(command, "@FechaFinalPres", DbType.Date,
                    fFinPres.Equals(DateTime.MinValue) ? (DateTime?)null : fFinPres);

                SqlCommand cmd = new SqlCommand();
                SqlParameter paramSSN = cmd.CreateParameter();
                paramSSN.ParameterName = "@NumTarjeta";
                paramSSN.DbType = DbType.AnsiStringFixedLength;
                paramSSN.Direction = ParameterDirection.Input;
                paramSSN.Value = string.IsNullOrEmpty(NumTarjeta) ? null : NumTarjeta;
                paramSSN.Size = NumTarjeta.Length;
                command.Parameters.Add(paramSSN);

                database.AddInParameter(command, "@ClaveEstatusOp", DbType.String, claveEstatusOp);
                database.AddInParameter(command, "@ID_CodRespuestaExterno", DbType.Int32, idMotRech);
                database.AddInParameter(command, "@ID_EstatusPostOperacion", DbType.Int32, idEstatusComp);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_Reporte_ObtieneOperacionesParabilium";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectivaGpoComercial=" + IdGpoComercial);
                parametros.Add("P2", "@FechaHoraInicialOp=" + fIniOp);
                parametros.Add("P3", "@FechaHoraFinalOp=" + fFinOp);
                parametros.Add("P4", "@FechaInicialPres=" + fIniPres);
                parametros.Add("P5", "@FechaFinalPres=" + fFinPres);
                parametros.Add("P6", "@NumTarjeta=" + (string.IsNullOrEmpty(NumTarjeta) ? NumTarjeta :
                    MaskSensitiveData.cardNumber(NumTarjeta)));
                parametros.Add("P7", "@ClaveEstatusOp=" + claveEstatusOp);
                parametros.Add("P8", "@ID_CodRespuestaExterno=" + idMotRech);
                parametros.Add("P9", "@ID_EstatusPostOperacion=" + idEstatusComp);
                parametros.Add("P10", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P11", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de operaciones en base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de códigos de respuesta externos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros del catálogo</returns>
        public static DataSet ListaCodigosRespuestaExternos(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneCodigosRespuestaExternos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_ObtieneCodigosRespuestaExternos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los códigos de respuesta externos " +
                    "de base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de estatus post operación en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros del catálogo</returns>
        public static DataSet ListaEstatusPostOperacion(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneEstatusPostOperacion");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_ObtieneEstatusPostOperacion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG
                
                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus post operación de base de datos");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de procesos que trabajan con archivos de base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaProcesos(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDProcesadorNocturno.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProcesos");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneProcesos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los procesos batch de base de datos");
            }
        }

        /// <summary>
        /// Obtiene el catálogo de procesos que trabajan con archivos de base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaEstatusFicheros(Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneEstatusFichero");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneEstatusFichero";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P2", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                pCI.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los estatus de fichero en base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos el reporte de procesos batch
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="dtCatProcesos">Objeto de tipo tabla con los procesos en catálogo</param>
        /// <param name="IdProceso">Identificador del proceso</param>
        /// <param name="IdEstatus">Identificador del estatus del fichero</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneReporteProcesoBatch(DateTime FechaInicial, DateTime FechaFinal, DataTable dtCatProcesos,
            int IdProceso, int IdEstatus, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDT112.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteProcesosBatch");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);

                command.Parameters.Add(new SqlParameter("@Procesos", dtCatProcesos));

                database.AddInParameter(command, "@IdProceso", DbType.Int32, IdProceso);
                database.AddInParameter(command, "@IdEstatus", DbType.Int32, IdEstatus);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ReporteProcesosBatch";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                parametros.Add("P3", "@Procesos=" + string.Join(Environment.NewLine,
                    dtCatProcesos.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray))));
                parametros.Add("P4", "@IdProceso=" + IdProceso);
                parametros.Add("P5", "@IdEstatus=" + IdEstatus);
                parametros.Add("P6", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P7", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de procesos batch de base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos el reporte de estados de movimientos
        /// </summary>
        /// <param name="IdColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="fechaCorte">Fecha del corte (mes/año)</param>
        /// <param name="estatusEnvio">Bandera con el estatus de envío</param>
        /// <param name="numTarjeta">Número de tarjeta</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneReporteEdosMovs(Int64 IdColectiva, String fechaCorte, UInt16 estatusEnvio,
            String numTarjeta, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_EstadosMovimientos");

                database.AddInParameter(command, "@IdClienteCacao", DbType.Int64, IdColectiva);
                database.AddInParameter(command, "@FechaCorte", DbType.String, fechaCorte);
                database.AddInParameter(command, "@Enviado", DbType.Boolean, estatusEnvio);

                SqlCommand cmd = new SqlCommand();
                SqlParameter paramSSN = cmd.CreateParameter();
                paramSSN.ParameterName = "@NumTarjeta";
                paramSSN.DbType = DbType.AnsiStringFixedLength;
                paramSSN.Direction = ParameterDirection.Input;
                paramSSN.Value = string.IsNullOrEmpty(numTarjeta) ? null : numTarjeta;
                paramSSN.Size = numTarjeta.Length;
                command.Parameters.Add(paramSSN);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_Reporte_EstadosMovimientos";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdClienteCacao=" + IdColectiva);
                parametros.Add("P2", "@FechaCorte=" + fechaCorte);
                parametros.Add("P3", "@Enviado=" + estatusEnvio);
                parametros.Add("P4", "@NumTarjeta=" + (String.IsNullOrEmpty(numTarjeta) ? numTarjeta :
                    MaskSensitiveData.cardNumber(numTarjeta)));
                parametros.Add("P5", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P6", "@AppId=" + AppID);
                logDBG.Parameters = parametros;

                logPCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el estado de movimientos de base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de tipos de operación (prioridad) de los mensajes webhook
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los registros del catálogo</returns>
        public static DataSet ListaTiposOperacionWbHk(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhook.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneTiposOperacion");

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_ObtieneTiposOperacion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";                    

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de operación de base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos el reporte de estatus de mensajes webhook
        /// </summary>
        /// <param name="IdColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="fecha">Fecha de la operación</param>
        /// <param name="idEstatusEnvio">Bandera con el estatus de envío (1 = enviado, 0 = no enviado)</param>
        /// <param name="idTipoOperacion">Identificador del tipo de operación (prioridad)</param>
        /// <param name="idPoliza">Identificador de la póliza</param>
        /// <param name="nombre">Nombre del tarjetahabiente</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneReporteMsjsWebhook(Int64 IdColectiva, DateTime fecha, int idEstatusEnvio,
            int idTipoOperacion, Int64 idPoliza, String nombre, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDWebhook.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_NotificacionesWebhook");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdClienteCacao", DbType.Int64, IdColectiva);
                database.AddInParameter(command, "@Fecha", DbType.DateTime, fecha);
                database.AddInParameter(command, "@IdEstatusEnvio", DbType.Int32, idEstatusEnvio);
                database.AddInParameter(command, "@IdPrioridad", DbType.Int32, idTipoOperacion);
                database.AddInParameter(command, "@IdPoliza", DbType.Int32, idPoliza);
                database.AddInParameter(command, "@Nombre", DbType.String, nombre);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_Reporte_NotificacionesWebhook";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";                    

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdClienteCacao=" + IdColectiva);
                parametros.Add("P2", "@Fecha=" + fecha);
                parametros.Add("P3", "@IdEstatusEnvio=" + idEstatusEnvio);
                parametros.Add("P4", "@IdPrioridad=" + idTipoOperacion);
                parametros.Add("P5", "@IdPoliza=" + idPoliza);
                parametros.Add("P6", "@Nombre=" + nombre);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de notificaciones Webhook " +
                    "de base de datos");
            }
        }

        /// <summary>
        /// Consulta los valores de contrato relacionados con el cifrado 3DES del SubEmisor
        /// </summary>
        /// <param name="IdColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros del catálogo</returns>
        public static Params3DES ObtieneValores3DESubEmisor(long IdColectiva, ILogHeader elLog)
        {
            SqlDataReader SqlReader = null;
            SqlParameter param = null;
            Params3DES p3DES = new Params3DES();
            LogPCI unLog = new LogPCI(elLog);
            

            try
            {
                using (SqlConnection conx = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conx;
                        cmd.CommandText = "web_ObtieneValores3DESClienteCacao";
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = new SqlParameter("@IdCliente", SqlDbType.BigInt);
                        param.Value = IdColectiva;
                        cmd.Parameters.Add(param);

                        conx.Open();

                        SqlReader = cmd.ExecuteReader();

                        try
                        {
                            if (null != SqlReader)
                            {
                                while (SqlReader.Read())
                                {
                                    switch (SqlReader["Nombre"].ToString().Trim())
                                    {
                                        case "@WH_3DESKey":
                                            p3DES.Key = SqlReader["Valor"].ToString().Trim();
                                            break;

                                        default: // "@WH_3DESVector":
                                            p3DES.Vector = SqlReader["Valor"].ToString().Trim();
                                            break;
                                    }
                                }
                            }

                            /////>>>LOG DEBUG
                            LogDebugMsg logDBG = new LogDebugMsg();
                            logDBG.M_Value = "web_ObtieneValores3DESClienteCacao";
                            logDBG.C_Value = "";
                            logDBG.R_Value = "***************************";

                            Dictionary<string, string> parametros = new Dictionary<string, string>();
                            parametros.Add("P1", "@IdCliente=" + IdColectiva);
                            logDBG.Parameters = parametros;

                            unLog.Debug(logDBG);
                            /////<<<LOG DEBUG
                        }
                        catch (Exception _ex)
                        {
                            unLog.ErrorException(_ex);
                            throw new CAppException(8010, "Error al extraer los datos de cifrado de base de datos.");
                        }
                    }
                }

                return p3DES;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Error al consultar los datos de cifado de base de datos.");
            }
        }

        /// <summary>
        /// Consulta el valor de la URL para los mensajes Webhook del SubEmisor
        /// </summary>
        /// <param name="IdColectiva">Identificador del SubEmisor (colectiva)</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Cadena con la URL</returns>
        public static string ObtieneValorURLSubEmisor(long IdColectiva, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDLectura))
                {
                    using (SqlCommand command = new SqlCommand("web_ObtieneValorURLWHClienteCacao", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add((new SqlParameter("@IdCliente", IdColectiva)));

                        conn.Open();

                        string resp = command.ExecuteScalar().ToString();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDb = new LogDebugMsg();
                        logDb.M_Value = "web_ObtieneValorURLWHClienteCacao";
                        logDb.C_Value = "";
                        logDb.R_Value = resp;

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdCliente=" + IdColectiva);
                        logDb.Parameters = parametros;

                        log.Debug(logDb);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar la URL del cliente en base de datos");
            }
        }

        /// <summary>
        /// Consulta el reporte de todas las operaciones de la cadena, con los filtros indicados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="Sucursal">Identificador de la sucursal</param>
        /// <param name="Afiliacion">Identificador de la afiliación</param>
        /// <param name="Terminal">Identificador de la terminal</param>
        /// <param name="Operador">Identificador del operador</param>
        /// <param name="Beneficiario">Identificador del beneficiario</param>
        /// <param name="Estatus">Estatus de la operación</param>
        /// <param name="Telefono">Número telefónico</param>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuarien sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataTable con los registros del reporte</returns>
        public static DataTable ObtieneOperacionesCadenaConSaldo(DateTime FechaInicial, DateTime FechaFinal, String Sucursal,
            String Afiliacion, String Terminal, String Operador, String Beneficiario, String Estatus, String Telefono,
            int ID_CadenaComercial, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_Reporte_TodasOperacionesCadena");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int32, ID_CadenaComercial);
                database.AddInParameter(command, "@Sucursal", DbType.String, Sucursal);
                database.AddInParameter(command, "@Afiliacion", DbType.String, Afiliacion);
                database.AddInParameter(command, "@Terminal", DbType.String, Terminal);
                database.AddInParameter(command, "@Beneficiario", DbType.String, Beneficiario);
                database.AddInParameter(command, "@Operador", DbType.String, Operador);
                database.AddInParameter(command, "@ID_Estatus", DbType.String, Estatus);
                database.AddInParameter(command, "@Telefono", DbType.String, Telefono);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los SubEmisores
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static DataSet ListaSubEmisores(ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneListaClientesCacao");

                ds = database.ExecuteDataSet(command);

                ///>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneListaClientesCacao";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    ds.Tables[0].Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));


                logPCI.Debug(logDbg);
                ///<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al consultar los clientres en base de datos");
            }
            return ds;
        }

        /// <summary>
        /// Consulta el reporte de todas las conciliaciones Open Pay, con los filtros indicados
        /// </summary>
        /// <param name="idColectiva">Id de la colectiva</param>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static DataSet ListaRegistrosOpenPay(string idColectiva, DateTime fechaInicial, DateTime fechaFinal, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRegistrosOpenPay");

                database.AddInParameter(command, "@IdColectiva", DbType.String, idColectiva);
                database.AddInParameter(command, "@FechaInicio", DbType.DateTime, fechaInicial);
                database.AddInParameter(command, "@FechaFin", DbType.DateTime, fechaFinal);

                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneRegistrosOpenPay";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";


                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva);
                parametros.Add("P2", "@FechaInicio=" + fechaInicial);
                parametros.Add("P3", "@FechaFin=" + fechaFinal);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG

                //return ds;
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al consultar los clientres en base de datos");
            }
            return ds;
        }

        /// <summary>
        /// Consulta el reporte de todas las conciliaciones Open Pay, con los filtros indicados
        /// </summary>
        /// <param name="cliente">Nombre del cliente</param>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="estatus">Estaus del envio</param>
        ///  <param name="tarjeta">Numero de la tarjeta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static DataSet ListaClabesPendientes(int idColectiva, DateTime fechaInicial, string estatus, 
            string tarjeta, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClabesPendientes");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicial);
                database.AddInParameter(command, "@Estatus", DbType.String, estatus);

                SqlCommand cmd = new SqlCommand();
                SqlParameter paramT = cmd.CreateParameter();
                paramT.ParameterName = "@Tarjeta";
                paramT.DbType = DbType.AnsiStringFixedLength;
                paramT.Direction = ParameterDirection.Input;
                paramT.Value = string.IsNullOrEmpty(tarjeta) ? null : tarjeta;
                paramT.Size = tarjeta.Length;
                command.Parameters.Add(paramT);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneClabesPendientes";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + idColectiva);
                parametros.Add("P2", "@FechaInicial=" + fechaInicial);
                parametros.Add("P3", "@Estatus=" + estatus);
                parametros.Add("P4", "@Tarjeta=" + (string.IsNullOrEmpty(tarjeta) ? tarjeta :
                    MaskSensitiveData.cardNumber(tarjeta)));
                parametros.Add("P5", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P6", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                logPCI.Debug(logDbg);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al consultar las cuentas CLABE pendientes en base de datos");
            }
            return ds;
        }

        /// <summary>
        /// Actualiza el estatus de las clabes Pendiesntes
        /// </summary>
        /// <param name="IdLog">Identificador de la tabla</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static int ActualizaEstatusClabePendiente(string IdLog, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);
            
            try
            {
                using (SqlConnection conn = new SqlConnection(BDWebhook.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusClabePendiente", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdLog", IdLog));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ActualizaEstatusClabePendiente";
                        logDbg.C_Value = "***************************";


                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdLog=" + IdLog);
                        logDbg.Parameters = parametros;

                        logPCI.Debug(logDbg);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus de la CLABE en base de datos");
            }
        }


        /// <summary>
        /// Obtiene los registros realizados por Spei's de la base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ReporteTraspasosSpei(int idColectiva, string ClaveMA, DateTime fechaInicial, DateTime fechaFinal, 
            Boolean esSpeiIn, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneInfoSPEI");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);

                SqlCommand cmd = new SqlCommand();
                SqlParameter paramCMA = cmd.CreateParameter();
                paramCMA.ParameterName = "@ClaveMA";
                paramCMA.DbType = DbType.AnsiStringFixedLength;
                paramCMA.Direction = ParameterDirection.Input;
                paramCMA.Value = ClaveMA;
                paramCMA.Size = ClaveMA.Length;
                command.Parameters.Add(paramCMA);

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFinal);
                database.AddInParameter(command, "@EsSpeiIn", DbType.Boolean, esSpeiIn);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneInfoSPEI";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();

                parametros.Add("P1", "@IdColectiva=" + idColectiva);
                parametros.Add("P2", "@ClaveMA=" + ((ClaveMA.Length == 18) ? MaskSensitiveData.CLABE(ClaveMA) :
                                                    (ClaveMA.Length == 16) ? MaskSensitiveData.cardNumber(ClaveMA) : ClaveMA));
                parametros.Add("P3", "@FechaInicial=" + fechaInicial);
                parametros.Add("P4", "@FechaFinal=" + fechaFinal);
                parametros.Add("P5", "@EsSpeiIn=" + esSpeiIn);

                parametros.Add("P6", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P7", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los procesos batch de base de datos");
            }
        }

        /// <summary>
        /// Obtiene los registros realizados por Spei's de la base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ReporteTraspasos(int idColectiva, string ClaveMA, DateTime fechaInicial, DateTime fechaFinal,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneInfoTraspasos");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);

                SqlCommand cmd = new SqlCommand();
                SqlParameter paramCMA = cmd.CreateParameter();
                paramCMA.ParameterName = "@ClaveMA";
                paramCMA.DbType = DbType.AnsiStringFixedLength;
                paramCMA.Direction = ParameterDirection.Input;
                paramCMA.Value = ClaveMA;
                paramCMA.Size = ClaveMA.Length;
                command.Parameters.Add(paramCMA);

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFinal);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneInfoTraspasos";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();

                parametros.Add("P1", "@IdColectiva=" + idColectiva);
                parametros.Add("P2", "@ClaveMA=" + ((ClaveMA.Length == 18) ? MaskSensitiveData.CLABE(ClaveMA) :
                                                    (ClaveMA.Length == 16) ? MaskSensitiveData.cardNumber(ClaveMA) : ClaveMA));
                parametros.Add("P3", "@FechaInicial=" + fechaInicial);
                parametros.Add("P4", "@FechaFinal=" + fechaFinal);

                parametros.Add("P5", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P6", "@AppId=" + AppID);
                logDbg.Parameters = parametros;

                unLog.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los procesos batch de base de datos");
            }
        }


        /// <summary>
        /// Consulta los registros pendientes de autorización para cambio de nivel de cuenta dentro del Autorizador
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes</returns>
        public static DataTable ObtieneRechazosSpeiInOut(DateTime FechaInicial, DateTime FechaFinal, int EsSpeiIn, string ClaveRastreo, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteRechazosSpeiInOut");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@EsSpeiIn", DbType.Int64, EsSpeiIn);
                database.AddInParameter(command, "@ClaveRastreo", DbType.String, string.IsNullOrEmpty(ClaveRastreo) ? null : ClaveRastreo);
                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ReporteRechazosSpeiInOut";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                parametros.Add("P3", "@EsSpeiIn=" + EsSpeiIn);
                parametros.Add("P4", "@ClaveRastreo=" + ClaveRastreo);
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los rechazos Spei In/Out del Spei/ClaveRastreo en base de datos");
            }
        }

        /// <summary>
        /// Consulta las rutas de los reportes para el generador nocturno
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes</returns>
        public static DataSet ListaRutasReportes(int IdColectiva, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);
            
            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRutasReportes");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, IdColectiva);
                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneRutasReportes";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las rutas de los reportes en base de datos");
            }

            return ds;
        }

        /// <summary>
        /// Consulta los reportes sin asignar 
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las solicitudes</returns>
        public static DataSet ListaReportesSinAsignar(int IdColectiva, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            DataSet ds = null;
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneRutasReportesSinAsignar");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, IdColectiva);
                ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDBG = new LogDebugMsg();
                logDBG.M_Value = "web_CA_ObtieneRutasReportesSinAsignar";
                logDBG.C_Value = "";
                logDBG.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@IdColectiva=" + IdColectiva);
                logDBG.Parameters = parametros;

                pCI.Debug(logDBG);
                /////<<<LOG DEBUG
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los reportes sin asignar en base de datos");
            }

            return ds;
        }

        /// <summary>
        /// Actualiza la ruta del reporte indicado en base de datos
        /// </summary>
        /// <param name="idReporte">Identificador del reporte</param>
        /// <param name="ruta">Valor de la nueva ruta del reporte</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaRutasReportes(int idReporte, string ruta, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaRutasReportes", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@idReporte", idReporte));
                        command.Parameters.Add(new SqlParameter("@ruta", ruta));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaRutasReportes";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@idReporte=" + idReporte);
                        parametros.Add("P2", "@ruta=" + ruta);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la ruta del reporte en base de datos");
            }
        }

        /// <summary>
        /// Consulta el reporte de Ingresos de las Tarjetas Cash In y Monto Promedio
        /// </summary>
        /// <param name="fInicio">Fecha inicial de la operación</param>
        /// <param name="fFin">Fecha final de la operación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ListaIngresosDeTarjetasCashInYMontoPromedio( DateTime fInicio, DateTime fFin, Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteIngresosEnTarjetas");
                command.CommandTimeout = 0;


                database.AddInParameter(command, "@FechaInicial", DbType.DateTime,
                    fInicio.Equals(DateTime.MinValue) ? (DateTime?)null : fInicio);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime,
                    fFin.Equals(DateTime.MinValue) ? (DateTime?)null : fFin);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ReporteIngresosEnTarjetas";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + fInicio);
                parametros.Add("P2", "@FechaFinal=" + fFin);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de Ingresos en las tarjetas (CASH IN) en base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de tipos de servicios de los reportes
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los tipos de servicios</returns>
        public static DataSet ListaTiposServicios(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposServicios");

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTiposServicios";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de servicio de base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de tipos de servicios de los reportes
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataSet con los tipos de servicios</returns>
        public static DataSet ListaClasificacion(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneClasificacion");

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneClasificacion";
                logDbg.C_Value = "";
                logDbg.R_Value = "***************************";

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return ds;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar las Clasificaciones de base de datos");
            }
        }

        /// <summary>
        /// Inserta el valor de un reporte a la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdReporte">Identificador del reporte</param>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void InsertaReporteColectiva(int IdReporte, long idColectiva, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_AsignarReporte", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdColectiva", idColectiva));
                        command.Parameters.Add(new SqlParameter("@IdReporte", IdReporte));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();
                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_AsignarReporte";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdColectiva=" + idColectiva);
                        parametros.Add("P2", "@IdReporte=" + idColectiva);
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG

                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al insertar el Reporte a la Colectiva en base de datos");
            }
        }

        /// <summary>
        /// Inserta el valor de un reporte a la colectiva en el Autorizador
        /// </summary>
        /// <param name="IdReporteColectivaConfiguracion">Identificador del registro de la configuración del reporte a actualizar</param>
        /// <param name="IdReporteColectiva">Identificador de la relación del reporte</param>
        /// <param name="IdClasificacion">Identificador de la clasificación del reporte</param>
        /// <param name="IdTipoServicio">Identificador del tipo de servicio del reporte</param>
        /// <param name="Ruta">Ruta del reporte</param>
        /// <param name="NombreArchivo">Nombre del Archivo del reporte</param>
        /// <param name="horaEjecucion">Hora de ejecución del reporte</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaConfigReporteColectiva(int IdReporteColectivaConfiguracion, int IdReporteColectiva, 
            int IdClasificacion, int IdTipoServicio, string Ruta, string NombreArchivo, TimeSpan horaEjecucion,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ConfiguraReporte", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdReporteColectivaConfiguracion", IdReporteColectivaConfiguracion));
                        command.Parameters.Add(new SqlParameter("@IdReporteColectiva", IdReporteColectiva));
                        command.Parameters.Add(new SqlParameter("@IdClasificacion", IdClasificacion));
                        command.Parameters.Add(new SqlParameter("@IdTipoServicio", IdTipoServicio));
                        command.Parameters.Add(new SqlParameter("@Ruta", Ruta));
                        command.Parameters.Add(new SqlParameter("@NombreArchivo", NombreArchivo));
                        command.Parameters.Add(new SqlParameter("@HoraEjecucion", horaEjecucion));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();
                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ConfiguraReporte";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdReporteColectivaConfiguracion=" + IdReporteColectivaConfiguracion.ToString());
                        parametros.Add("P2", "@IdReporteColectiva=" + IdReporteColectiva.ToString());
                        parametros.Add("P3", "@IdClasificacion=" + IdClasificacion.ToString());
                        parametros.Add("P4", "@IdTipoServicio=" + IdTipoServicio.ToString());
                        parametros.Add("P5", "@Ruta=" + Ruta);
                        parametros.Add("P6", "@NombreArchivo=" + NombreArchivo);
                        parametros.Add("P7", "@HoraEjecucion=" + horaEjecucion.ToString());
                        parametros.Add("P8", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar la configuración del reporte en base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros de contrato sin asignar a la colectiva,
        /// que pertenecen a la clasificación marcada en el Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns></returns>
        public static DataSet ListaParametrosSinAsignarSFTPEmisor(Int64 idColectiva,
            Usuario elUsuario, Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosSinAsignarSFTPEmisor");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosSinAsignarSFTPEmisor";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                //parametros.Add("P1", "@IdClasificacion=" + idClasificacion);
                parametros.Add("P2", "@IdColectiva=" + idColectiva);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Parámetros sin Asignar de base de datos");
            }
        }

        /// <summary>
        /// Obtiene el listado de parámetros de contrato asignados a la colectiva,
        /// que pertenecen a la clasificación marcada en el Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns></returns>
        public static DataSet ListaParametrosAsignadosSFTPEmisor(//int idClasificacion, 
            Int64 idColectiva, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI pCI = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosAsignadosSFTPEmisor");

                database.AddInParameter(command, "@IdColectiva", DbType.Int64, idColectiva);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataSet ds = database.ExecuteDataSet(command);

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ObtieneParametrosAsignadosSFTPEmisor";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                // parametros.Add("P1", "@IdClasificacion=" + idClasificacion);
                parametros.Add("P2", "@IdColectiva=" + idColectiva);
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                pCI.Debug(logDebug);
                /////<<<LOG DEBUG

                return ds;
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar los Parámetros de la Colectiva de base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos el reporte de ejecuciones de solicitudes de estados de cuenta externos
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneReporteSolicitudesEdoCtaExternas(DateTime FechaInicial, DateTime FechaFinal, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDBatch.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteSolicitudesEdoCtaExt");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_ReporteSolicitudesEdoCtaExt";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + FechaInicial);
                parametros.Add("P2", "@FechaFinal=" + FechaFinal);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de solicitudes de estados de " +
                    "cuenta externos en base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos el detalle de la solicitud de estado de cuenta externo
        /// </summary>
        /// <param name="idArchivo">Identificador del archivo de la solicitud</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneDetalleSolicitudEdoCtaExterno(int idArchivo, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDBatch.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DetalleSolicitudEdoCtaExt");

                database.AddInParameter(command, "@IdArchivo", DbType.Int32, idArchivo);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_CA_DetalleSolicitudEdoCtaExt";
                logDebug.C_Value = "";
                logDebug.R_Value = "***************************";

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "IdArchivo=" + idArchivo.ToString());
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el detalle de la solicitud de estados " +
                    "de cuenta externo en base de datos");
            }
        }

        /// <summary>
        /// Consulta en base de datos los estados de cuenta timbrados
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los registros de la consulta</returns>
        public static DataTable ObtieneEstadosCuentaTimbrados(DateTime FechaInicial, DateTime FechaFinal, Usuario elUsuario,
            Guid AppID, ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ReporteEdosCtaTimbrados");

                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, FechaFinal);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDebug = new LogDebugMsg();
                logDebug.M_Value = "web_ReporteEdosCtaTimbrados";
                logDebug.C_Value = "";
                logDebug.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@FechaInicial=" + FechaInicial.ToShortDateString());
                parametros.Add("P2", "@FechaFinal=" + FechaFinal.ToShortDateString());
                parametros.Add("P3", "@UserTemp=" + elUsuario.UsuarioTemp);
                parametros.Add("P4", "@AppId=" + AppID);
                logDebug.Parameters = parametros;

                unLog.Debug(logDebug);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al consultar el reporte de timbrados en base de datos");
            }
        }

        /// <summary>
        /// Consulta el catálogo de tipos de generación de reportes
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los tipos de servicios</returns>
        public static DataTable ListaTiposGeneracionReportes(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneTiposGeneracionReportes");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneTiposGeneracionReportes";
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
                throw new CAppException(8010, "Ocurrió un error al consultar los tipos de generación de reportes en base de datos");
            }
        }

        /// <summary>
        /// Consulta la lista de reportes disponibles en base de datos
        /// </summary>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con los datos solicitados</returns>
        public static DataTable ObtenerListaReportes(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneListaReportes");

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_ObtieneListaReportes";
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
                throw new CAppException(8010, "Ocurrió un error al consultar la lista de reportes de base de datos");
            }
        }

        /// <summary>
        /// Verifica la existencia de un reporte en base de datos
        /// </summary>
        /// <param name="claveReporte">Clave del reporte a verificar</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con lps datos del reporte, si existe</returns></returns>
        public static DataTable VerificaExisteReporte(string claveReporte, ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_VerificaExisteReporte");
                database.AddInParameter(command, "@ClaveReporte", DbType.String, claveReporte);

                DataTable dt = database.ExecuteDataSet(command).Tables[0];

                /////>>>LOG DEBUG
                LogDebugMsg logDbg = new LogDebugMsg();
                logDbg.M_Value = "web_CA_VerificaExisteReporte";
                logDbg.C_Value = "";
                logDbg.R_Value = string.Join(Environment.NewLine,
                    dt.Rows.OfType<DataRow>().Select(x => string.Join("|", x.ItemArray)));

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("P1", "@ClaveReporte=" + claveReporte);
                logDbg.Parameters = parametros;

                log.Debug(logDbg);
                /////<<<LOG DEBUG

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al validar la existencia del reporte en base de datos");
            }
        }

        /// <summary>
        /// Inserta un nuevo reporte en base de datos
        /// </summary>
        /// <param name="idTipoGenRep">Identificador del tipo de generación del reporte</param>
        /// <param name="ClaveReporte">Clave del reporte</param>
        /// <param name="Nombre">Nombre del reporte </param>
        /// <param name="SP">Nombre del procedimiento almacenado que se asignara el reporte</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void AgregarNuevoReporte(int idTipoGenRep, string ClaveReporte, string Nombre, string SP,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_AgregarNuevoReporte", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdTipoGeneracionReporte", idTipoGenRep));
                        command.Parameters.Add(new SqlParameter("@ClaveReporte", ClaveReporte));
                        command.Parameters.Add(new SqlParameter("@Nombre", Nombre));
                        command.Parameters.Add(new SqlParameter("@SP", SP));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();
                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_AgregarNuevoReporte";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdTipoGeneracionReporte=" + idTipoGenRep.ToString());
                        parametros.Add("P2", "@ClaveReporte=" + ClaveReporte);
                        parametros.Add("P3", "@Nombre=" + Nombre);
                        parametros.Add("P4", "@SP=" + SP);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al agregar nuevo reporte en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el estatus de un reporte en base de datos
        /// </summary>
        /// <param name="IdReporte">Identificador del reporte</param>
        /// <param name="Estatus">Estatus Reporte</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static int ActualizaEstatusReporte(int IdReporte, bool Estatus, Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusReporte", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ID_Reporte", IdReporte));
                        command.Parameters.Add(new SqlParameter("@Estatus", Estatus));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ActualizaEstatusReporte";
                        logDbg.C_Value = resp.ToString();


                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@ID_Reporte=" + IdReporte.ToString());
                        parametros.Add("P2", "@Estatus=" + Estatus.ToString());
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDbg.Parameters = parametros;

                        logPCI.Debug(logDbg);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus del reporte en base de datos");
            }
        }

        /// <summary>
        /// Actualiza los datos de un reporte en base de datos
        /// </summary>
        /// <param name="IdReporte">Identificador del reporte</param>
        /// <param name="IdTipoGenRep">Identificador del tipo de generación de reporte</param>
        /// <param name="Nombre">Nombre de reporte</param>
        /// <param name="sp">Nombre del procedimiento almacenado</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static int ModificaDatosReporte(int IdReporte, int IdTipoGenRep, string Nombre, string sp,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ModificaDatosReporte", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ID_Reporte", IdReporte));
                        command.Parameters.Add(new SqlParameter("@IdTipoGeneracion", IdTipoGenRep));
                        command.Parameters.Add(new SqlParameter("@Nombre", Nombre));
                        command.Parameters.Add(new SqlParameter("@SP", sp));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDbg = new LogDebugMsg();
                        logDbg.M_Value = "web_CA_ModificaDatosReporte";
                        logDbg.C_Value = resp.ToString();


                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@ID_Reporte=" + IdReporte.ToString());
                        parametros.Add("P2", "@IdTipoGeneracion=" + IdTipoGenRep.ToString());
                        parametros.Add("P3", "@Nombre=" + Nombre);
                        parametros.Add("P4", "@SP=" + sp);
                        parametros.Add("P5", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDbg.Parameters = parametros;

                        logPCI.Debug(logDbg);
                        /////<<<LOG DEBUG

                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.Error(ex.Message);
                throw new CAppException(8010, "Ocurrió un error al actualizar los datos del reporte en base de datos");
            }
        }

        /// <summary>
        /// Actualiza el estatus de la configuración de un reporte de colectiva en base de datos.
        /// </summary>
        /// <param name="IdReporteColectivaConfiguracion">Identificador del registro de la configuración del reporte a actualizar</param>
        /// <param name="EsActivo">Bandera de estatus activo del reporte</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ActualizaEstatusConfigReporteColectiva(int IdReporteColectivaConfiguracion, int EsActivo,
            Usuario elUsuario, ILogHeader elLog)
        {
            LogPCI logPCI = new LogPCI(elLog);

            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaEstatusReporteColectiva", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdReporteColectivaConfiguracion", IdReporteColectivaConfiguracion));
                        command.Parameters.Add(new SqlParameter("@EsActivo", EsActivo));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();
                        int resp = command.ExecuteNonQuery();

                        /////>>>LOG DEBUG
                        LogDebugMsg logDBG = new LogDebugMsg();
                        logDBG.M_Value = "web_CA_ActualizaEstatusReporteColectiva";
                        logDBG.C_Value = "";
                        logDBG.R_Value = resp.ToString();

                        Dictionary<string, string> parametros = new Dictionary<string, string>();
                        parametros.Add("P1", "@IdReporteColectivaConfiguracion=" + IdReporteColectivaConfiguracion.ToString());
                        parametros.Add("P2", "@EsActivo=" + EsActivo.ToString());
                        parametros.Add("P3", "@Usuario=" + elUsuario.ClaveUsuario);
                        logDBG.Parameters = parametros;

                        logPCI.Debug(logDBG);
                        /////<<<LOG DEBUG
                    }
                }
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8010, "Ocurrió un error al actualizar el estatus de la configuración del reporte en base de datos");
            }
        }
    }
}
