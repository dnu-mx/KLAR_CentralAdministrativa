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
using System.Data.SqlClient;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objeto de acceso a datos para la entidad Grupo de Medios de Acceso
    /// </summary>
    public class DAOGruposMA
    {
        /// <summary>
        /// Consulta los registros disponibles de Grupos de Medios de Acceso en base de datos
        /// y genera el árbol necesario para mostrarse en menú
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dictionary con el árbol de los registros</returns>
        public static Dictionary<Int64, Producto> ListaGruposMA (Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerProductos");
             
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                
                DataSet ds = database.ExecuteDataSet(command);
                Dictionary<Int64, Producto> prods = new Dictionary<Int64, Producto>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    UInt16 indiceMenu;
                    UInt16 indiceSubMenuL1;
                    UInt16 indiceSubMenuL2;
                    UInt16 indiceRama;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        indiceMenu = 1;
                        indiceSubMenuL1 = 1;

                        Producto producto = new Producto();                       

                        producto.ID_GrupoMA = Convert.ToInt64(String.IsNullOrEmpty(ds.Tables[0].Rows[i]["ID_GrupoMA"].ToString()) ? "0" : ds.Tables[0].Rows[i]["ID_GrupoMA"].ToString());
                        producto.ClaveGrupo = ds.Tables[0].Rows[i]["Clave"].ToString();
                        producto.Descripcion = ds.Tables[0].Rows[i]["Descripcion"].ToString();
                        producto.ID_Vigencia = Convert.ToInt64(String.IsNullOrEmpty(ds.Tables[0].Rows[i]["ID_Vigencia"].ToString()) ? "0" : ds.Tables[0].Rows[i]["ID_Vigencia"].ToString());
                        producto.Vigencia = String.IsNullOrEmpty(ds.Tables[0].Rows[i]["Vigencia"].ToString()) ? "" : ds.Tables[0].Rows[i]["Vigencia"].ToString();

                        producto.Menus.Add(indiceMenu++, new ConfiguracionGMA("Propiedades", indiceSubMenuL1++));
                        producto.Menus.Add(indiceMenu++, new ConfiguracionGMA("Configuraciones", indiceSubMenuL1++)); 

                        prods.Add(producto.ID_GrupoMA, producto);

                        foreach (ConfiguracionGMA menu in prods[Convert.ToInt64(ds.Tables[0].Rows[i]["ID_GrupoMA"])].Menus.Values)
                        {
                            indiceSubMenuL2 = 1;

                            if (String.Compare(menu.Nombre, "Configuraciones") == 0)
                            {
                                indiceRama = 1;
                                menu.Ramas.Add(indiceRama++, new ConfiguracionGMA("Rangos", indiceSubMenuL2++));
                                menu.Ramas.Add(indiceRama++, new ConfiguracionGMA("Reglas", indiceSubMenuL2++));
                                menu.Ramas.Add(indiceRama++, new ConfiguracionGMA("Parámetros", indiceSubMenuL2++));
                                menu.Ramas.Add(indiceRama++, new ConfiguracionGMA("Validaciones", indiceSubMenuL2++));
                            }

                            foreach (ConfiguracionGMA item in menu.Ramas.Values)
                            {
                                indiceRama = 1;

                                if (String.Compare(item.Nombre, "Parámetros") == 0)
                                {
                                    item.Ramas.Add(indiceRama++, new ConfiguracionGMA("Grupo"));

                                    //item.Ramas.Add(indiceRama++, new ConfiguracionGMA("Regla"));
                                    //item.Ramas.Add(indiceRama++, new ConfiguracionGMA("Tipo de Cuenta"));
                                    //item.Ramas.Add(indiceRama++, new ConfiguracionGMA("Grupo de Cuenta"));
                                    //item.Ramas.Add(indiceRama++, new ConfiguracionGMA("Grupo de Tarjeta"));
                                    //item.Ramas.Add(indiceRama++, new ConfiguracionGMA("Tarjeta/Cuenta"));
                                }
                            }
                        }
                    }
                }

                return prods;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los datos del producto o Grupo de Medios de Acceso en base de datos según su ID
        /// </summary>
        /// <param name="IdGMA">Identificador del producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaProducto(Int64 IdGMA, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerProductoPorID");

                database.AddInParameter(command, "@IdGrupoMA", DbType.Int64, IdGMA);
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
        /// Consulta los registros disponibles de Grupos de Medios de Acceso en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaGruposMA(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGruposMA");
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
        /// Consulta los registros disponibles de Vigencias en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaVigencias(Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerVigencias");
                //database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                //database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los registros disponibles de Tipos de Vigencia en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaTiposVigencia(Usuario elUsuario, Guid AppID) 
        {
            try 
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTipoViegncias");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                return database.ExecuteDataSet(command);
            }
            catch(Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los registros disponibles de Periodos en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaPeriodos(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerPeriodos");
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
        /// Consulta los registros disponibles de Rangos en base de datos, para el IDGrupoMA seleccionado
        /// </summary>
        /// <param name="IDGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListarRangos(int IDGrupoMA, Usuario elUsuario, Guid AppID) 
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerRangosXGrupoMA");
                database.AddInParameter(command, "@ID_GrupoMA", DbType.Int32, IDGrupoMA);
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
        /// Inserta el registro en base de datos del nuevo Grupo de Medios de Acceso
        /// </summary>
        /// <param name="elGrupoMA">Nuevo Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        public static void insertar(GrupoMA elGrupoMA, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertarProducto");

                database.AddInParameter(command, "@ClaveGrupo", DbType.String, elGrupoMA.ClaveGrupo);
                database.AddInParameter(command, "@Descripcion", DbType.String, elGrupoMA.Descripcion);
                database.AddInParameter(command, "@ID_Vigencia", DbType.Int32, elGrupoMA.ID_Vigencia);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Agregado un grupoMA al Autorizador", elUsuario.ClaveUsuario);
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
        /// <param name="elGrupoMA"></param>
        /// <param name="elUsuario"></param>
        /// <param name="AppID"></param>
        /// <returns></returns>
        public static void actualiza(GrupoMA elGrupoMA, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizaProducto");

                database.AddInParameter(command, "@ID_Grupo", DbType.Int32, elGrupoMA.ID_GrupoMA);
                database.AddInParameter(command, "@ClaveGrupo", DbType.String, elGrupoMA.ClaveGrupo);
                database.AddInParameter(command, "@Descripcion", DbType.String, elGrupoMA.Descripcion);
                database.AddInParameter(command, "@ID_Vigencia", DbType.Int32, elGrupoMA.ID_Vigencia);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Actualizado un grupoMA al Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las Cadenas Comerciales en base de datos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCadenasComercialesAutMC(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtenerCadenasComerciales");

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
        public static DataSet ConsultaProductosPorNombre(String producto, Usuario elUsuario, Guid AppID)
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
        /// Obtiene los parámetros de la cadena comercial en base de datos
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static List<ParametroGMA> ObtenerParametrosProductoTDC(Int64 IdProducto, Usuario elUsuario, Guid AppID)
        {
            DataSet losParametros = null;
            List<ParametroGMA> Respuesta = new List<ParametroGMA>();

            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ObtieneParamsProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                losParametros = database.ExecuteDataSet(command);

                if (null != losParametros)
                {
                    for (int k = 0; k < losParametros.Tables[0].Rows.Count; k++)
                    {
                        Respuesta.Add(new ParametroGMA((String)losParametros.Tables[0].Rows[k]["Name"], 
                            (String)losParametros.Tables[0].Rows[k]["Value"], 
                            (String)losParametros.Tables[0].Rows[k]["Descripcion"],
                            Convert.ToInt32(losParametros.Tables[0].Rows[k]["ID_SystemValue"].ToString())));
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Sucedió un Error al Obtener los Parámetros del Producto TDC: " + Ex);
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
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametrosProducto(ParametroGMA losParametros, Int64 IdProducto, Usuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ActualizaParametroProducto");

                database.AddInParameter(command, "@IdProducto", DbType.Int64, IdProducto);
                database.AddInParameter(command, "@NombreParam", DbType.String, losParametros.Nombre);
                database.AddInParameter(command, "@NuevoValor", DbType.String, losParametros.Valor);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros del producto: " + Ex);
            }
        }

        /// <summary>
        /// Actualiza los rangos de tarjetas del producto en base de datos
        /// </summary>
        /// <param name="losParametros">Datos del rango por actualizar</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaRangosProducto(int IdRango, string Rini, string Rfin, Usuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorMC.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_GMA_ActualizaRangosProducto");

                database.AddInParameter(command, "@IdRango", DbType.Int32, IdRango);
                database.AddInParameter(command, "@NuevoRangoInicial", DbType.String, Rini);
                database.AddInParameter(command, "@NuevoRangoFinal", DbType.String, Rfin);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los rangos de las tarjetas: " + Ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de grupos de medios de acceso que pertenecen a la colectiva con el
        /// identificador indicado dentro del Autorizador
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ListaGruposMABines(int idColectiva, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneGruposMABines");

                database.AddInParameter(command, "@IdColectiva", DbType.Int32, idColectiva);
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
        /// Actualiza el tipo de interación del grupo de medios de acceso en el Autorizador
        /// </summary>
        /// <param name="idGpoMA">Identificador del grupo de medios de acceso</param>
        /// <param name="idTipoInt">Identificador del nuevo tipo de integración</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaTipoIntegracionGrupoMA(int idGpoMA, int idTipoInt, SqlConnection connection,
            SqlTransaction transaccionSQL, Usuario elUsuario)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaTipoIntegracionGrupoMA", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@IdGrupoMA", idGpoMA));
                command.Parameters.Add(new SqlParameter("@IdTipoIntegracion", idTipoInt));
                command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
