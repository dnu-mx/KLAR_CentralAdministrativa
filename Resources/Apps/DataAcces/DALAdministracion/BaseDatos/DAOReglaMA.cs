using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
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
using System.Threading.Tasks;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Regla Multiasignación
    /// </summary>
    public class DAOReglaMA
    {
        /// <summary>
        /// Consulta los datos básicos del catálogo de Reglas en base de datos
        /// </summary>
        /// <param name="nombreRegla">Nombre de la regla</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ListaCatalogoReglas(string nombreRegla, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerCatalogoReglas");

                database.AddInParameter(command, "@Desc", DbType.String, nombreRegla);
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
        /// Consulta el total las reglas multiasignación asociadas o no a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaTotalReglasMA(int IdCadenaComercial, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTotalReglasMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@IdGMA", DbType.Int32, IdGrupoMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta las reglas multiasignación asociadas a la cadena comercial
        /// </summary>
        /// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ConsultaReglasMA(int IdCadenaComercial, int IdGrupoMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerReglasMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadenaComercial);
                database.AddInParameter(command, "@IdGMA", DbType.Int32, IdGrupoMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        ///// <summary>
        ///// Consulta las reglas multiasignación para los tipos de cuenta asociados a la cadena comercial
        ///// </summary>
        ///// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Identificador de la aplicación</param>
        ///// <returns>DataSet con los registros</returns>
        //public static DataSet ConsultaTipoCuentaRMA(Int16 IdCadenaComercial, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTipoCuentaRMA");

        //        database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Consulta las reglas multiasignación para los grupos de cuenta asociados a la cadena comercial
        ///// </summary>
        ///// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Identificador de la aplicación</param>
        ///// <returns>DataSet con los registros</returns>
        //public static DataSet ConsultaGrupoCuentaRMA(Int16 IdCadenaComercial, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGrupoCuentaRMA");

        //        database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Consulta las reglas multiasignación para los grupos de tarjeta asociados a la cadena comercial
        ///// </summary>
        ///// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Identificador de la aplicación</param>
        ///// <returns>DataSet con los registros</returns>
        //public static DataSet ConsultaGrupoTarjetaRMA(Int16 IdCadenaComercial, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGrupoTarjetaRMA");

        //        database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Consulta las reglas multiasignación para la tarjeta/cuenta asociadas a la cadena comercial
        ///// </summary>
        ///// <param name="IdCadenaComercial">Identificador de la Cadena</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Identificador de la aplicación</param>
        ///// <returns>DataSet con los registros</returns>
        //public static DataSet ConsultaTarjetaCuentaRMA(Int16 IdCadenaComercial, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTarjetaCuentaRMA");

        //        database.AddInParameter(command, "@IdCadena", DbType.Int16, IdCadenaComercial);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppID", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Actualiza el orden de ejecucion de una regla multiasignación en base de datos
        ///// </summary>
        ///// <param name="IdRegla">Identificador de la regla por modificar</param>
        ///// <param name="nuevoOrdenEjec">Valor del nuevo orden de ejecución de la regla</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        //public static void ActualizaOrdenEjecRMA(int IdRegla, int nuevoOrdenEjec, IUsuario elUsuario)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizaOrdenEjecucionRegla");

        //        database.AddInParameter(command, "@IdRegla", DbType.Int64, IdRegla);
        //        database.AddInParameter(command, "@Orden", DbType.Int32, nuevoOrdenEjec);

        //        database.ExecuteNonQuery(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}


      /// <summary>
        /// Inserta o actualiza una regla multiasignación en base de datos
        /// </summary>
        /// <param name="laRMA">Valores del tipo ReglaMA</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActualizaRMA(ReglaMA laRMA, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaActualizaRMA");

                database.AddInParameter(command, "@IdRegla", DbType.Int32, laRMA.ID_Regla);
                database.AddInParameter(command, "@IdRMA", DbType.Int64, laRMA.ID_ReglaMultiasignacion);
                database.AddInParameter(command, "@Prioridad", DbType.Int32, laRMA.Prioridad);
                database.AddInParameter(command, "@IdVigencia", DbType.Int32, laRMA.ID_Vigencia);
                database.AddInParameter(command, "@IdCadena", DbType.Int64, laRMA.ID_CadenaComercial);
                database.AddInParameter(command, "@IdGMA", DbType.Int32, laRMA.ID_Producto);
                database.AddInParameter(command, "@Orden", DbType.Int32, laRMA.OrdenEjecucionRMA);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
