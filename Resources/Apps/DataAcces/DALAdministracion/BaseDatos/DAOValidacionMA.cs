using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Validación Multiasignación
    /// </summary>
    public class DAOValidacionMA
    {
       /// <summary>
       /// Consulta las Validaciones Multiasignación a las que tiene acceso la cadena comercial
       /// </summary>
       /// <param name="IdCadena">Id de la cadena comercial</param>
       /// <param name="elUsuario">Usuario en sesión</param>
       /// <param name="AppID">Id de la aplicación</param>
       /// <returns>Dataset con los registros de la consulta</returns>
        public static DataSet ConsultaVMA(Int64 IdCadena, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerVMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
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

        ///// <summary>
        ///// Consulta las Validaciones Multiasignación para la entidad tipo de cuenta de la cadena comercial
        ///// </summary>
        ///// <param name="IdCadena">Id de la cadena comercial</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Id de la aplicación</param>
        ///// <returns>Dataset con los registros de la consulta</returns>
        //public static DataSet ConsultaTipoCuentaVMA(Int64 IdCadena, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTipoCuentaVMA");

        //        database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Consulta las Validaciones Multiasignación para la entidad grupo de cuenta de la cadena comercial
        ///// </summary>
        ///// <param name="IdCadena">Id de la cadena comercial</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Id de la aplicación</param>
        ///// <returns>Dataset con los registros de la consulta</returns>
        //public static DataSet ConsultaGrupoCuentaVMA(Int64 IdCadena, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGrupoCuentaVMA");

        //        database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Consulta las Validaciones Multiasignación para la entidad grupo de tarjeta de la cadena comercial
        ///// </summary>
        ///// <param name="IdCadena">Id de la cadena comercial</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Id de la aplicación</param>
        ///// <returns>Dataset con los registros de la consulta</returns>
        //public static DataSet ConsultaGrupoTarjetaVMA(Int64 IdCadena, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerGrupoTarjetaVMA");
        //        database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Consulta las Validaciones Multiasignación para la entidad tarjeta/cuenta de la cadena comercial
        ///// </summary>
        ///// <param name="IdCadena">Id de la cadena comercial</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        ///// <param name="AppID">Id de la aplicación</param>
        ///// <returns>Dataset con los registros de la consulta</returns>
        //public static DataSet ConsultaTarjetaCuentaVMA(Int64 IdCadena, IUsuario elUsuario, Guid AppID)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerTarjetaCuentaVMA");
        //        database.AddInParameter(command, "@IdCadena", DbType.Int64, IdCadena);
        //        database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
        //        database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

        //        return database.ExecuteDataSet(command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        /// <summary>
        /// Inserta una nueva Validación Multiasignación (VMA) en base de datos
        /// </summary>
        /// <param name="laValidacion">Datos de la VMA</param>
        /// <param name="idValidacionPadre">Id de la validación padre</param>
        /// <param name="tipoValidacion">Tipo de validación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaVMA(ValidacionMA laValidacion, Int64 idValidacionPadre, Int32 tipoValidacion, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaVMA");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, laValidacion.ID_CadenaComercial);
                database.AddInParameter(command, "@ID_GMA", DbType.Int64, laValidacion.ID_Producto);
                database.AddInParameter(command, "@ID_Vigencia", DbType.Int64, laValidacion.ID_Vigencia);
                database.AddInParameter(command, "@ID_BaseValidacion", DbType.Int64, idValidacionPadre);
                database.AddInParameter(command, "@TipoValidacion", DbType.Int32, tipoValidacion);
                database.AddInParameter(command, "@NombreValidacion", DbType.String, laValidacion.Nombre);
                database.AddInParameter(command, "@CampoValidacion", DbType.String, laValidacion.Campo);
                database.AddInParameter(command, "@ClaveTipoElementoValidacion", DbType.String, laValidacion.ClaveTipoElemento);
                database.AddInParameter(command, "@FormulaValidacion", DbType.String, laValidacion.Formula);
                database.AddInParameter(command, "@CodigoErrorValidacion", DbType.String, laValidacion.CodigoError);
                database.AddInParameter(command, "@DeclinarValidacion", DbType.Boolean, laValidacion.Declinar);
                database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, laValidacion.PreRegla);
                database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, laValidacion.PostRegla);
                database.AddInParameter(command, "@OrdenValidacion", DbType.Int32, laValidacion.OrdenValidacion);
                database.AddInParameter(command, "@Prioridad", DbType.Int32, laValidacion.Prioridad);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Nueva Validación Multiasignación en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        ///// <summary>
        ///// Inserta una nueva Validación Multiasignación (VMA) para la entidad Tipo de Cuenta en base de datos.
        ///// </summary>
        ///// <param name="laValidacion">Datos de la VMA</param>
        ///// <param name="idValidacionPadre">Id de la validación padre</param>
        ///// <param name="tipoValidacion">Tipo de validación</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        //public static void InsertaTipoCuentaVMA(ValidacionMA laValidacion, Int64 idValidacionPadre, Int32 tipoValidacion, Usuario elUsuario)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaTipoCuentaVMA");

        //        database.AddInParameter(command, "@ID_Vigencia", DbType.Int64, laValidacion.ID_Vigencia);
        //        database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, laValidacion.ID_CadenaComercial);
        //        database.AddInParameter(command, "@ID_BaseValidacion", DbType.Int64, idValidacionPadre);
        //        database.AddInParameter(command, "@TipoValidacion", DbType.Int32, tipoValidacion);
        //        database.AddInParameter(command, "@NombreValidacion", DbType.String, laValidacion.Nombre);
        //        database.AddInParameter(command, "@CampoValidacion", DbType.String, laValidacion.Campo);
        //        database.AddInParameter(command, "@ClaveTipoElementoValidacion", DbType.String, laValidacion.ClaveTipoElemento);
        //        database.AddInParameter(command, "@FormulaValidacion", DbType.String, laValidacion.Formula);
        //        database.AddInParameter(command, "@CodigoErrorValidacion", DbType.String, laValidacion.CodigoError);
        //        database.AddInParameter(command, "@DeclinarValidacion", DbType.Boolean, laValidacion.Declinar);
        //        database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, laValidacion.PreRegla);
        //        database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, laValidacion.PostRegla);
        //        database.AddInParameter(command, "@OrdenValidacion", DbType.Int32, laValidacion.OrdenValidacion);
        //        database.AddInParameter(command, "@Prioridad", DbType.Int32, laValidacion.Prioridad);

        //        database.ExecuteNonQuery(command);
        //        Loguear.Evento("Se Insertó una Nueva Validación Multiasignación para un Tipo de Cuenta en el Autorizador", elUsuario.ClaveUsuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Inserta una nueva Validación Multiasignación (VMA) para la entidad Grupo de Cuenta en base de datos.
        ///// </summary>
        ///// <param name="laValidacion">Datos de la VMA</param>
        ///// <param name="idValidacionPadre">Id de la validación padre</param>
        ///// <param name="tipoValidacion">Tipo de validación</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        //public static void InsertaGrupoCuentaVMA(ValidacionMA laValidacion, Int64 idValidacionPadre, Int32 tipoValidacion, Usuario elUsuario)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaGrupoCuentaVMA");

        //        database.AddInParameter(command, "@ID_Vigencia", DbType.Int64, laValidacion.ID_Vigencia);
        //        database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, laValidacion.ID_CadenaComercial);
        //        database.AddInParameter(command, "@ID_BaseValidacion", DbType.Int64, idValidacionPadre);
        //        database.AddInParameter(command, "@TipoValidacion", DbType.Int32, tipoValidacion);
        //        database.AddInParameter(command, "@NombreValidacion", DbType.String, laValidacion.Nombre);
        //        database.AddInParameter(command, "@CampoValidacion", DbType.String, laValidacion.Campo);
        //        database.AddInParameter(command, "@ClaveTipoElementoValidacion", DbType.String, laValidacion.ClaveTipoElemento);
        //        database.AddInParameter(command, "@FormulaValidacion", DbType.String, laValidacion.Formula);
        //        database.AddInParameter(command, "@CodigoErrorValidacion", DbType.String, laValidacion.CodigoError);
        //        database.AddInParameter(command, "@DeclinarValidacion", DbType.Boolean, laValidacion.Declinar);
        //        database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, laValidacion.PreRegla);
        //        database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, laValidacion.PostRegla);
        //        database.AddInParameter(command, "@OrdenValidacion", DbType.Int32, laValidacion.OrdenValidacion);
        //        database.AddInParameter(command, "@Prioridad", DbType.Int32, laValidacion.Prioridad);

        //        database.ExecuteNonQuery(command);
        //        Loguear.Evento("Se Insertó una Nueva Validación Multiasignación para un Grupo de Cuenta en el Autorizador", elUsuario.ClaveUsuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Inserta una nueva Validación Multiasignación (VMA) para la entidad Grupo de Tarjeta en base de datos.
        ///// </summary>
        ///// <param name="laValidacion">Datos de la VMA</param>
        ///// <param name="idValidacionPadre">Id de la validación padre</param>
        ///// <param name="tipoValidacion">Tipo de validación</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        //public static void InsertaGrupoTarjetaVMA(ValidacionMA laValidacion, Int64 idValidacionPadre, Int32 tipoValidacion, Usuario elUsuario)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaGrupoTarjetaVMA");

        //        database.AddInParameter(command, "@ID_Vigencia", DbType.Int64, laValidacion.ID_Vigencia);
        //        database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, laValidacion.ID_CadenaComercial);
        //        database.AddInParameter(command, "@ID_BaseValidacion", DbType.Int64, idValidacionPadre);
        //        database.AddInParameter(command, "@TipoValidacion", DbType.Int32, tipoValidacion);
        //        database.AddInParameter(command, "@NombreValidacion", DbType.String, laValidacion.Nombre);
        //        database.AddInParameter(command, "@CampoValidacion", DbType.String, laValidacion.Campo);
        //        database.AddInParameter(command, "@ClaveTipoElementoValidacion", DbType.String, laValidacion.ClaveTipoElemento);
        //        database.AddInParameter(command, "@FormulaValidacion", DbType.String, laValidacion.Formula);
        //        database.AddInParameter(command, "@CodigoErrorValidacion", DbType.String, laValidacion.CodigoError);
        //        database.AddInParameter(command, "@DeclinarValidacion", DbType.Boolean, laValidacion.Declinar);
        //        database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, laValidacion.PreRegla);
        //        database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, laValidacion.PostRegla);
        //        database.AddInParameter(command, "@OrdenValidacion", DbType.Int32, laValidacion.OrdenValidacion);
        //        database.AddInParameter(command, "@Prioridad", DbType.Int32, laValidacion.Prioridad);

        //        database.ExecuteNonQuery(command);
        //        Loguear.Evento("Se Insertó una Nueva Validación Multiasignación para un Grupo de Tarjeta en el Autorizador", elUsuario.ClaveUsuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Inserta una nueva Validación Multiasignación (VMA) para la entidad Tarjeta/Cuenta en base de datos.
        ///// </summary>
        ///// <param name="laValidacion">Datos de la VMA</param>
        ///// <param name="idValidacionPadre">Id de la validación padre</param>
        ///// <param name="tipoValidacion">Tipo de validación</param>
        ///// <param name="elUsuario">Usuario en sesión</param>
        //public static void InsertaTarjetaCuentaVMA(ValidacionMA laValidacion, Int64 idValidacionPadre, Int32 tipoValidacion, Usuario elUsuario)
        //{
        //    try
        //    {
        //        SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
        //        DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaTarjetaCuentaVMA");

        //        database.AddInParameter(command, "@ID_Vigencia", DbType.Int64, laValidacion.ID_Vigencia);
        //        database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, laValidacion.ID_CadenaComercial);
        //        database.AddInParameter(command, "@ID_BaseValidacion", DbType.Int64, idValidacionPadre);
        //        database.AddInParameter(command, "@TipoValidacion", DbType.Int32, tipoValidacion);
        //        database.AddInParameter(command, "@NombreValidacion", DbType.String, laValidacion.Nombre);
        //        database.AddInParameter(command, "@CampoValidacion", DbType.String, laValidacion.Campo);
        //        database.AddInParameter(command, "@ClaveTipoElementoValidacion", DbType.String, laValidacion.ClaveTipoElemento);
        //        database.AddInParameter(command, "@FormulaValidacion", DbType.String, laValidacion.Formula);
        //        database.AddInParameter(command, "@CodigoErrorValidacion", DbType.String, laValidacion.CodigoError);
        //        database.AddInParameter(command, "@DeclinarValidacion", DbType.Boolean, laValidacion.Declinar);
        //        database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, laValidacion.PreRegla);
        //        database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, laValidacion.PostRegla);
        //        database.AddInParameter(command, "@OrdenValidacion", DbType.Int32, laValidacion.OrdenValidacion);
        //        database.AddInParameter(command, "@Prioridad", DbType.Int32, laValidacion.Prioridad);

        //        database.ExecuteNonQuery(command);
        //        Loguear.Evento("Se Insertó una Nueva Validación Multiasignación para una Tarjeta/Cuenta en el Autorizador", elUsuario.ClaveUsuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        Loguear.Error(ex, elUsuario.ClaveUsuario);
        //        throw new CAppException(8010, ex.Message, ex);
        //    }
        //}

        /// <summary>
        /// Activa o inactiva una validación mulltiasignación (VMA) en base de datos
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad VMA</param>
        /// <param name="activar">Valor de la activación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaActivacionVMA(ValidacionMA laValidacion, bool activar, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActivarInactivarVMA");

                database.AddInParameter(command, "@ID_VMA", DbType.Int64, laValidacion.ID_ValidadorMultiasignacion);
                database.AddInParameter(command, "@ActivarValidacion", DbType.Boolean, activar);

                database.ExecuteNonQuery(command);
                Loguear.Evento(String.Format("Se ha {0} una Validación Multiasignación del Autorizador", activar ? "Activado" : "Desactivado"), elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Elimina de base de datos una validación multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad VMA</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void EliminarVMA(ValidacionMA laValidacion, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_EliminarVMA");

                database.AddInParameter(command, "@ID_VMA", DbType.Int64, laValidacion.ID_ValidadorMultiasignacion);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Eliminó una Validación Multiasignación del Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Cambia en base de datos las pre reglas de una validación multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad VMA</param>
        /// <param name="preReglas">Valor de las pre reglas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void PreReglas(ValidacionMA laValidacion, bool preReglas, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_CambiarPreReglasVMA");

                database.AddInParameter(command, "@ID_VMA", DbType.Int32, laValidacion.ID_ValidadorMultiasignacion);
                database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, preReglas);

                database.ExecuteNonQuery(command);
                Loguear.Evento(String.Format("Se ha {0} la Ejecución de Pre Reglas de una Validación Multiasignación del Autorizador", preReglas ? "Activado" : "Desactivado"), elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Cambia en base de datos las post reglas de una validación multiasignación
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad VMA</param>
        /// <param name="preReglas">Valor de las post reglas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void PostReglas(ValidacionMA laValidacion, bool postReglas, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_CambiarPostReglasVMA");

                database.AddInParameter(command, "@ID_VMA", DbType.Int32, laValidacion.ID_ValidadorMultiasignacion);
                database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, postReglas);

                database.ExecuteNonQuery(command);
                Loguear.Evento(String.Format("Se ha {0} la Ejecución de Post Reglas de una Validación Multiasignación del Autorizador", postReglas ? "Activado" : "Desactivado"), elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los datos faltantes (si es validación base, campo y código de error) para edición de  
        /// una Validación Multiasignación en base de datos
        /// </summary>
        /// <param name="IdCadena">Id de la cadena comercial</param>
        /// <param name="IdVMA">Id de la validación multiasignacion</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Id de la aplicación</param>
        /// <returns>1 en caso de ser base</returns>
        public static DataSet ConsultaDatosPorEditarVMA(int IdCadena, int IdVMA, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerDatosEdicionVMA");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@IdVMA", DbType.Int32, IdVMA);
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
        /// Actualiza los valores de una validación mulltiasignación (VMA) en base de datos
        /// </summary>
        /// <param name="laValidacion">Datos de la entidad VMA</param>
        /// <param name="activar">Valor de la activación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ModificaVMA(ValidacionMA laValidacion, Int64 idValidacionPadre, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizarVMA");

                database.AddInParameter(command, "@ID_VMA", DbType.Int64, idValidacionPadre);
                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, laValidacion.ID_CadenaComercial);
                database.AddInParameter(command, "@ID_GMA", DbType.Int64, laValidacion.ID_Producto);
                database.AddInParameter(command, "@Vigencia", DbType.String, laValidacion.Vigencia);
                database.AddInParameter(command, "@NombreValidacion", DbType.String, laValidacion.Nombre);
                database.AddInParameter(command, "@CampoValidacion", DbType.String, laValidacion.Campo);
                database.AddInParameter(command, "@ClaveTipoElementoValidacion", DbType.String, laValidacion.ClaveTipoElemento);
                database.AddInParameter(command, "@FormulaValidacion", DbType.String, laValidacion.Formula);
                database.AddInParameter(command, "@CodigoErrorValidacion", DbType.String, laValidacion.CodigoError);
                database.AddInParameter(command, "@DeclinarValidacion", DbType.Boolean, laValidacion.Declinar);
                database.AddInParameter(command, "@PreReglasValidacion", DbType.Boolean, laValidacion.PreRegla);
                database.AddInParameter(command, "@PostReglasValidacion", DbType.Boolean, laValidacion.PostRegla);
                database.AddInParameter(command, "@OrdenValidacion", DbType.Int32, laValidacion.OrdenValidacion);
                database.AddInParameter(command, "@Prioridad", DbType.Int32, laValidacion.Prioridad);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Actualizó una Validación Multiasignación en el Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
