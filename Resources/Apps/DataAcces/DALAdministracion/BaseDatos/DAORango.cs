using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALAdministracion.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la entidad Rango
    /// </summary>
    public class DAORango
    {
        /// <summary>
        /// Inserta el registro en base de datos del nuevo rango
        /// </summary>
        /// <param name="elRango">Rango nuevo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void insertar(Rango elRango, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaRango");

			    database.AddInParameter(command, "@ID_GrupoMA", DbType.Int32, elRango.ID_GrupoMA);
			    database.AddInParameter(command, "@Clave", DbType.String, elRango.Clave);
			    database.AddInParameter(command, "@Descripcion", DbType.String, elRango.Descripcion);
			    database.AddInParameter(command, "@Inicio", DbType.Decimal, elRango.Inicio);
			    database.AddInParameter(command, "@Fin", DbType.Decimal, elRango.Fin);
                database.AddInParameter(command, "@esActivo", DbType.Boolean, elRango.esActivo);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Agregado un rango al Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza el registro del rango en base de datos
        /// </summary>
        /// <param name="elRango">Rango editado</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void actualizar(Rango elRango, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizaRango");

                database.AddInParameter(command, "@IDRango", DbType.Int32, elRango.ID_Rango);
                database.AddInParameter(command, "@Clave", DbType.String, elRango.Clave);
                database.AddInParameter(command, "@Descripcion", DbType.String, elRango.Descripcion);
                database.AddInParameter(command, "@Inicio", DbType.Decimal, elRango.Inicio);
                database.AddInParameter(command, "@Fin", DbType.Decimal, elRango.Fin);
                database.AddInParameter(command, "@Activo", DbType.Boolean, elRango.esActivo);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Modificado un rango en el Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta la longitud del tipo de medio de acceso en base de datos
        /// </summary>
        /// <param name="idTipoMA">Identificador del tipo de medio de acceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static int ConsultaLongitudMA(int idTipoMA, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerLongitudTipoMA");

                database.AddInParameter(command, "@IdTipoMA", DbType.Int32, idTipoMA);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return (int)database.ExecuteScalar(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }            
    }
}
