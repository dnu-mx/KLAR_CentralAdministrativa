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

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objeto de acceso a datos para el Configurador de Tarjetas Bancarias
    /// </summary>
    public class DAOConfiguradorTarjetas
    {
        /// <summary>
        /// Consulta las Cadenas Comerciales en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListarCadenasComercialesAutMC(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneCadenasComerciales");

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
        /// Consulta en base de datos los productos que coinciden con el nombre indicado
        /// </summary>
        /// <param name="producto">Descripción o nombre del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultarProductosPorNombre(String producto, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneProductos");

                database.AddInParameter(command, "@Producto", DbType.String, producto);
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
        /// Obtiene los parámetros de la cadena comercial en base de datos (método copia de ObtenerParametrosProductoTDC,
        /// para no afectar lo creado para F&F
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static List<ParametroGMA_V2> ListarParametrosProductoTDC(Int64 IdProducto, Usuario elUsuario, Guid AppID)
        {
            List<ParametroGMA_V2> Respuesta = new List<ParametroGMA_V2>();

            try
            {
                DataSet losParametros = null;

                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParametrosMultiasignacionProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                losParametros = database.ExecuteDataSet(command);

                for (int k = 0; k < losParametros.Tables[0].Rows.Count; k++)
                {
                    Respuesta.Add(
                        new ParametroGMA_V2(Convert.ToInt32(losParametros.Tables[0].Rows[k]["ID_Parametro"].ToString()),
                            (String)losParametros.Tables[0].Rows[k]["Nombre"],
                            (String)losParametros.Tables[0].Rows[k]["Descripcion"],
                            (String)losParametros.Tables[0].Rows[k]["ValorActual"],
                            (String)losParametros.Tables[0].Rows[k]["ValorPendiente"]));
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }

            return Respuesta;
        }

        /// <summary>
        /// Consulta los rangos de las tarjetas del producto indicado en base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtenerRangosProducto(Int64 IdProducto, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneRangosProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int32, IdProducto);
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
        /// Actualiza el valor de un parámetro del producto en base de datos
        /// </summary>
        /// <param name="losParametros">Datos del parámetro por actualizar</param>
        /// <param name="IdCadena">Identificador de la cadena comercial seleccionada</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        public static void ControlarCambiosParametrosProducto(ParametroGMA_V2 losParametros, Int64 IdCadena, Int64 IdProducto,
            String pagina, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ControlaParametroProducto");

                database.AddInParameter(command, "@IdCadenaComercial", DbType.Int64, IdCadena);
                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@NombreParam", DbType.String, losParametros.Nombre);
                database.AddInParameter(command, "@NuevoValor", DbType.String, losParametros.Valor);
                database.AddInParameter(command, "@PaginaAspx", DbType.String, pagina);

                database.AddInParameter(command, "@UsuarioEjecutor", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros del producto: " + Ex);
            }
        }

        /// <summary>
        /// Actualiza el valor de un parámetro del producto en base de datos
        /// </summary>
        /// <param name="losParametros">Datos del parámetro por actualizar</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizarParametrosProducto(ParametroGMA_V2 losParametros, Int64 IdProducto, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaParametroProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@NombreParam", DbType.String, losParametros.Nombre);
                database.AddInParameter(command, "@NuevoValor", DbType.String, losParametros.Valor);
                database.AddInParameter(command, "@Usuario", DbType.String, elUsuario.ClaveUsuario);
                

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros del producto: " + Ex);
            }
        }

        /// <summary>
        /// Autoriza el cambio de valores a los parámetros del producto en base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void AutorizarCambiosParametrosProducto(Int64 IdProducto, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_AutorizaCambiosParametrosProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@UsuarioAutorizador", DbType.String, elUsuario.ClaveUsuario);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUsuario.ClaveUsuario);
                throw new Exception("Ha sucedido un error al autorizar los cambios a los parámetros del producto: " + Ex);
            }
        }

        /// <summary>
        /// Actualiza los rangos de tarjetas del producto en base de datos
        /// </summary>
        /// <param name="IdRango">Identificador del rango por actualizar</param>
        /// <param name="Rini">Nuevo valor del rango inicial</param>
        /// <param name="Rfin">Nuevo valor del rango final</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizarRangosProducto(int IdRango, string Rini, string Rfin, Usuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaRangosProducto");

                database.AddInParameter(command, "@IdRango", DbType.Int32, IdRango);
                database.AddInParameter(command, "@NuevoRangoInicial", DbType.String, Rini);
                database.AddInParameter(command, "@NuevoRangoFinal", DbType.String, Rfin);
                database.AddInParameter(command, "@Usuario", DbType.String, elUser.ClaveUsuario);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los rangos de las tarjetas: " + Ex);
            }
        }
    }
}
