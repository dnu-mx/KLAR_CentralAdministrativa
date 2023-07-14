using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.BaseDatos
{
    public class DAOTipoCuenta
    {
        /// <summary>
        /// Consulta los datos básicos del catálogo de tipos de cuenta en base de datos
        /// </summary>
        /// <param name="descTipoCta">Descripción del tipo de cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoTiposCuenta(string descTipoCta, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerCatalogoTiposCuenta");

                database.AddInParameter(command, "@Desc", DbType.String, descTipoCta);
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

        public static DataSet ListarTiposCuenta(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTiposCuenta");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            } catch(Exception ex) {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ListarDivisas(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerDivisas");

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

        public static DataSet ListarCodTipoCuentaISO(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerCodTipoCuentaISO");

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

        public static int insertar(TipoCuenta elTipoCuenta, Usuario elUsuario, Guid AppID) 
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertarTipoCuenta");

                database.AddInParameter(command, "@CodigoTipoCuentaISO", DbType.String, elTipoCuenta.CodigoTipoCuentaISO);
                database.AddInParameter(command, "@ClaveTipoCuenta", DbType.String, elTipoCuenta.ClaveTipoCuenta);
                database.AddInParameter(command, "@Descripcion", DbType.String, elTipoCuenta.Descripcion);
                database.AddInParameter(command, "@GeneraDetalle", DbType.Boolean, elTipoCuenta.GeneraDetalle);
                database.AddInParameter(command, "@GeneraCorte", DbType.Boolean, elTipoCuenta.GeneraCorte);
                database.AddInParameter(command, "@ID_Divisa", DbType.Int32, elTipoCuenta.ID_Divisa);
                database.AddInParameter(command, "@ID_Periodo", DbType.Int32, elTipoCuenta.ID_Periodo);
                database.AddInParameter(command, "@BreveDescripcion", DbType.String, elTipoCuenta.BreveDescripcion);
                database.AddInParameter(command, "@EditarSaldoGrid", DbType.Boolean, elTipoCuenta.EditarSaldoGrid);
                database.AddInParameter(command, "@InteractuaCajero", DbType.Boolean, elTipoCuenta.InteractuaCajero);
                database.AddInParameter(command, "@ID_NaturalezaCuenta", DbType.Int32, elTipoCuenta.ID_NaturalezaCuenta);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Agregado un TipoCuenta al Autorizador", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int actualizar(TipoCuenta elTipoCuenta, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizarTipoCuenta");

                database.AddInParameter(command, "@ID_TipoCuenta", DbType.String, elTipoCuenta.ID_TipoCuenta);
                database.AddInParameter(command, "@CodigoTipoCuentaISO", DbType.String, elTipoCuenta.CodigoTipoCuentaISO);
                database.AddInParameter(command, "@ClaveTipoCuenta", DbType.String, elTipoCuenta.ClaveTipoCuenta);
                database.AddInParameter(command, "@Descripcion", DbType.String, elTipoCuenta.Descripcion);
                database.AddInParameter(command, "@GeneraDetalle", DbType.Boolean, elTipoCuenta.GeneraDetalle);
                database.AddInParameter(command, "@GeneraCorte", DbType.Boolean, elTipoCuenta.GeneraCorte);
                database.AddInParameter(command, "@ID_Divisa", DbType.Int32, elTipoCuenta.ID_Divisa);
                database.AddInParameter(command, "@ID_Periodo", DbType.Int32, elTipoCuenta.ID_Periodo);
                database.AddInParameter(command, "@BreveDescripcion", DbType.String, elTipoCuenta.BreveDescripcion);
                database.AddInParameter(command, "@EditarSaldoGrid", DbType.Boolean, elTipoCuenta.EditarSaldoGrid);
                database.AddInParameter(command, "@InteractuaCajero", DbType.Boolean, elTipoCuenta.InteractuaCajero);
                database.AddInParameter(command, "@ID_NaturalezaCuenta", DbType.Int32, elTipoCuenta.ID_NaturalezaCuenta);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Agregado un TipoCuenta al Autorizador", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
