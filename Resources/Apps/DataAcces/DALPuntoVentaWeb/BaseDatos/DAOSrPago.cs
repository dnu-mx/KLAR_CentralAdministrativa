using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALPuntoVentaWeb.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Sr Pago
    /// </summary>
    public class DAOSrPago
    {
        /// <summary>
        /// Inserta los registros con los datos de las tiendas Diconsa en base de datos
        /// </summary>
        /// <param name="losDatos">Datos de las tiendas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaDatosDeArchivo(DatosSrPago losDatos, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaDatosTmpTiendaDiconsa");

                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, losDatos.ClaveAlmacen);
                database.AddInParameter(command, "@ClaveTienda", DbType.String, losDatos.ClaveTienda);
                database.AddInParameter(command, "@Calle", DbType.String, losDatos.Calle);
                database.AddInParameter(command, "@Localidad", DbType.String, losDatos.Localidad);
                database.AddInParameter(command, "@Municipio", DbType.String, losDatos.Municipio);
                database.AddInParameter(command, "@Estado", DbType.String, losDatos.Estado);
                database.AddInParameter(command, "@Telefono", DbType.String, losDatos.Telefono);
                database.AddInParameter(command, "@Pswd", DbType.String, losDatos.Password);
                database.AddInParameter(command, "@Email", DbType.String, losDatos.Email);
                database.AddInParameter(command, "@NombreOperador", DbType.String, losDatos.NombreOperador);
                database.AddInParameter(command, "@ApPaternoOperador", DbType.String, losDatos.ApellidoPaternoOperador);
                database.AddInParameter(command, "@ApMaternoOperador", DbType.String, losDatos.ApellidoMaternoOperador);
                database.AddInParameter(command, "@CodigoPostal", DbType.String, losDatos.CodigoPostal);
                database.AddInParameter(command, "@NumTarjeta", DbType.String, losDatos.NumeroTarjeta);
                database.AddInParameter(command, "@UrlIfeFrente", DbType.String, losDatos.URL_IFE_Anverso);
                database.AddInParameter(command, "@UrlIfeReverso", DbType.String, losDatos.URL_IFE_Reverso);
                database.AddInParameter(command, "@UrlFirma", DbType.String, losDatos.URL_Firma);
                database.AddInParameter(command, "@UrlDomicilio", DbType.String, losDatos.URL_CompDomicilio);

                database.ExecuteNonQuery(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de tiendas que coinciden con los filtros de búsqueda
        /// ingresados en base de datos
        /// </summary>
        /// <param name="Almacen">Clave del almacén</param>
        /// <param name="Tienda">Nombre de la tienda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneCatalogoTiendas(string Almacen, string Tienda, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DIC_ObtieneTiendas");

                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, Almacen);
                database.AddInParameter(command, "@NombreTienda", DbType.String, Tienda);

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
        /// Consulta los datos de la tienda seleccionada en base de datos
        /// </summary>
        /// <param name="idColectiva">Identificador de la tienda</param>
        /// <param name="cveAlmacen">Clave del almacén de la tienda</param>
        /// <param name="cveTienda">Clave de la tienda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosTienda(int idColectiva, string cveAlmacen, string cveTienda, 
            Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DIC_ObtieneDatosTienda");

                database.AddInParameter(command, "@IdTienda", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, cveAlmacen);
                database.AddInParameter(command, "@ClaveTienda", DbType.String, cveTienda);

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
        /// Complementa los datos de la tienda Diconsa en el Autorizador
        /// </summary>
        /// <param name="datos">Datos de la tienda por complementar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void CompletaDatosTienda(DatosSrPago datos, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                //SqlDatabase database = new SqlDatabase("Data Source=64.34.163.109;Initial Catalog=Autorizador_Diconsa;User ID=sa;Password=dnu6996;Persist Security Info=False;");
                DbCommand command = database.GetStoredProcCommand("web_PDVOL_CompletaDatosTiendasDiconsa");

                database.AddInParameter(command, "@Telefono", DbType.String, datos.Telefono);
                database.AddInParameter(command, "@Clave_Almacen", DbType.String, datos.ClaveAlmacen);
                database.AddInParameter(command, "@Clave_Tienda", DbType.String, datos.ClaveTienda);
                database.AddInParameter(command, "@Mail", DbType.String, datos.Email);
                database.AddInParameter(command, "@LOCALIDAD", DbType.String, datos.Localidad);
                database.AddInParameter(command, "@MUNICIPIO", DbType.String, datos.Municipio);
                database.AddInParameter(command, "@ESTADO", DbType.String, datos.Estado);
                database.AddInParameter(command, "@PSP", DbType.String, datos.Password);
                database.AddInParameter(command, "@NombreOperador", DbType.String, datos.NombreOperador);
                database.AddInParameter(command, "@APaternoOperador", DbType.String, datos.ApellidoPaternoOperador);
                database.AddInParameter(command, "@AMaternoOperador", DbType.String, datos.ApellidoMaternoOperador);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Completaron los Datos de la Tienda con ID " + datos.IdTienda +
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de la tienda Diconsa en base de datos
        /// </summary>
        /// <param name="datos">Datos de la tienda por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaDatosTienda(DatosSrPago datos, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaDatosTmpTiendaDiconsa");

                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, datos.ClaveAlmacen);
                database.AddInParameter(command, "@ClaveTienda", DbType.String, datos.ClaveTienda);
                database.AddInParameter(command, "@Calle", DbType.String, datos.Calle);
                database.AddInParameter(command, "@Localidad", DbType.String, datos.Localidad);
                database.AddInParameter(command, "@Municipio", DbType.String, datos.Municipio);
                database.AddInParameter(command, "@Estado", DbType.String, datos.Estado);
                database.AddInParameter(command, "@Pswd", DbType.String, datos.Password);
                database.AddInParameter(command, "@Email", DbType.String, datos.Email);
                database.AddInParameter(command, "@Telefono", DbType.String, datos.Telefono);
                database.AddInParameter(command, "@NombreOperador", DbType.String, datos.NombreOperador);
                database.AddInParameter(command, "@ApPaternoOperador", DbType.String, datos.ApellidoPaternoOperador);
                database.AddInParameter(command, "@ApMaternoOperador", DbType.String, datos.ApellidoMaternoOperador);
                database.AddInParameter(command, "@FechaNacOperador", DbType.Date, datos.FechaNacimientoOperador);
                database.AddInParameter(command, "@CodigoPostal", DbType.String, datos.CodigoPostal);
                database.AddInParameter(command, "@NumTarjeta", DbType.String, datos.NumeroTarjeta);
                database.AddInParameter(command, "@UrlIfeFrente", DbType.String, datos.URL_IFE_Anverso);
                database.AddInParameter(command, "@UrlIfeReverso", DbType.String, datos.URL_IFE_Reverso);
                database.AddInParameter(command, "@UrlFirma", DbType.String, datos.URL_Firma);
                database.AddInParameter(command, "@UrlDomicilio", DbType.String, datos.URL_CompDomicilio);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizaron los Datos de la Tienda con ID " + datos.IdTienda +
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza en base de datos el estatus de alta de una tienda Diconsa en Sr Pago
        /// </summary>
        /// <param name="datos">Datos de la tienda por actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaAltaSrPago(DatosSrPago datos, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaAltaSrPagoTmpTiendaDiconsa");

                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, datos.ClaveAlmacen);
                database.AddInParameter(command, "@ClaveTienda", DbType.String, datos.ClaveTienda);

                database.ExecuteNonQuery(command);

                Loguear.Evento("Se Actualizó el Alta en Sr Pago de la Tienda con ID " + datos.IdTienda + 
                    " en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los datos de la tienda seleccionada en la pantalla de actualización de
        /// documentos en base de datos
        /// </summary>
        /// <param name="idColectiva">Identificador de la tienda</param>
        /// <param name="cveAlmacen">Clave del almacén de la tienda</param>
        /// <param name="cveTienda">Clave de la tienda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneDatosDocumentosTienda(int idColectiva, string cveAlmacen, string cveTienda,
            Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_DIC_ObtieneDatosDocumTienda");

                database.AddInParameter(command, "@IdTienda", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@ClaveAlmacen", DbType.String, cveAlmacen);
                database.AddInParameter(command, "@ClaveTienda", DbType.String, cveTienda);

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
