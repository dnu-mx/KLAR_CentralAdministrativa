using DALAdministracion.Entidades;
using DALAutorizador.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Cuentas Bancarias
    /// </summary>
    public class DAOCuentasBancarias
    {
        /// <summary>
        /// Consulta las cuentas que coinciden con los datos de ingreso en base de datos
        /// </summary>
        /// <param name="datosCuenta">Parámetros de búsqueda de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCuentas(DatosPersonalesCuenta datosCuenta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneCuentas");

                database.AddInParameter(command, "@IdCuenta", DbType.Int32, datosCuenta.ID_Cuenta);
                database.AddInParameter(command, "@TarjetaTitular", DbType.String, datosCuenta.TarjetaTitular);
                database.AddInParameter(command, "@Nombre", DbType.String, datosCuenta.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, datosCuenta.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, datosCuenta.ApellidoMaterno);
                database.AddInParameter(command, "@TarjetaAdicional", DbType.String, datosCuenta.TarjetaAdicional);

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
        /// Consulta los datos personales de la cuenta seleccionada en base de datos
        /// </summary>
        /// <param name="idCta">Identificador de la cuenta</param>
        /// <param name="idTarj">Identificador de la tarjeta</param>
        /// <param name="idCol">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosPersonalesCuenta(int idCta, int idTarj, int idCol, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneDatosPersonalesCuenta");

                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCta);
                database.AddInParameter(command, "@IdTarjeta", DbType.Int32, idTarj);
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
        /// Obtiene los datos de colonia, municipio y estado a partir del código postal indicado.
        /// </summary>
        /// <param name="codigoPostal">Código postal</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaDatosPorCodigoPostal(string codigoPostal, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneColMpioEdoPorCP");

                database.AddInParameter(command, "@CP", DbType.String, codigoPostal);
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
        /// Actualiza los datos personales de la cuenta bancaria en base de datos
        /// </summary>
        /// <param name="datosCuenta">Datos de la cuenta a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaDatosPersonalesCuenta(DatosPersonalesCuenta datosCuenta, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ActualizaDatosPersonalesCuenta");

                database.AddInParameter(command, "@IdTitular", DbType.Int32, datosCuenta.ID_Colectiva);
                database.AddInParameter(command, "@Nombre", DbType.String, datosCuenta.Nombre);
                database.AddInParameter(command, "@ApPaterno", DbType.String, datosCuenta.ApellidoPaterno);
                database.AddInParameter(command, "@ApMaterno", DbType.String, datosCuenta.ApellidoMaterno);
                database.AddInParameter(command, "@RFC", DbType.String, datosCuenta.RFC);

                database.AddInParameter(command, "@IdDireccion", DbType.Int32, datosCuenta.IdDireccion);
                database.AddInParameter(command, "@Calle", DbType.String, datosCuenta.Calle);
                database.AddInParameter(command, "@NumExterior", DbType.String, datosCuenta.NumExterior);
                database.AddInParameter(command, "@NumInterior", DbType.String, datosCuenta.NumInterior);
                database.AddInParameter(command, "@EntreCalles", DbType.String, datosCuenta.EntreCalles);
                database.AddInParameter(command, "@Referencias", DbType.String, datosCuenta.Referencias);
                database.AddInParameter(command, "@IdColonia", DbType.Int32, datosCuenta.IdColonia);

                database.AddInParameter(command, "@TelParticular", DbType.String, datosCuenta.NumTelParticular);
                database.AddInParameter(command, "@TelCelular", DbType.String, datosCuenta.NumTelCelular);
                database.AddInParameter(command, "@TelTrabajo", DbType.String, datosCuenta.NumTelTrabajo);
                database.AddInParameter(command, "@eMail", DbType.String, datosCuenta.Email);
                
                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Actualizaron los Datos Personales de la Cuenta en el Autorizador MasterCard", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los datos de la cuenta seleccionada en base de datos
        /// </summary>
        /// <param name="idCta">Identificador de la cuenta</param>
        /// <param name="idTarj">Identificador de la tarjeta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosCuenta(int idCta, int idTarj, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneDatosCuenta");

                database.AddInParameter(command, "@IdMA", DbType.Int32, idTarj);
                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCta);
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
        /// Consulta las tarjetas adicionales de la cuenta seleccionada en base de datos
        /// </summary>
        /// <param name="idCta">Identificador de la cuenta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneTarjetasAdicionales(int idCta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneTarjetasAdicionales");

                database.AddInParameter(command, "@IdCuenta", DbType.Int32, idCta);
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
        /// Actualiza el valor de un parámetro de nivel Cuenta del producto indicado,
        /// en base de datos
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="Param">Parámetro por actualizar</param>
        /// <param name="Valor">Nuevo valor del parámetro por actualizar</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametroProductoCuenta(Int64 IdCuenta, Int64 IdProducto, String Param,
            String Valor, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ActualizaParametroProductoCuenta");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@IdCuenta", DbType.Int64, IdCuenta);
                database.AddInParameter(command, "@NombreParam", DbType.String, Param);
                database.AddInParameter(command, "@NuevoValor", DbType.String, Valor);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros cuenta del producto: " + Ex);
            }
        }


        /// <summary>
        /// Actualiza el valor de un parámetro del producto indicado en base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto por actualizar</param>
        /// <param name="Param">Parámetro por actualizar</param>
        /// <param name="Valor">Nuevo valor del parámetro por actualizar</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametroProducto(Int64 IdProducto, String Param, String Valor, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ActualizaParametroMedioAcceso");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@NombreParam", DbType.String, Param);
                database.AddInParameter(command, "@NuevoValor", DbType.String, Valor);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros del producto: " + Ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos los datos básicos del evento 
        /// "Ajuste Manual a Límite de Crédito" 
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaEventoAjusteManualLDC(int IdColectiva, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_EV_ObtieneDatosAjusteLimiteCredito");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, IdColectiva);
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
    }
}
