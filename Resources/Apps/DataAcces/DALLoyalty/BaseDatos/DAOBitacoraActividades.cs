using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para el registro en Bitácora de Actividades
    /// </summary>
    public class DAOBitacoraActividades
    {
        /// <summary>
        /// Inserta una acción en la bitácora de actividades de base de datos
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="accion">Acción realizada</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActividad(int idCliente, string accion, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_LYL_InsertaAccionBitacoraActividades");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idCliente);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@Accion", DbType.String, accion);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Acción en la Bitácora de Actividades del Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una acción en la bitácora de actividades de base de datos (Moshi Moshi)
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <param name="accion">Acción realizada</param>
        /// <param name="obs">Observaciones</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaActividadMoshiEnBitacora(int idColectiva, string accion, string obs, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaAccionBitacoraActividades");

                database.AddInParameter(command, "@IdCliente", DbType.Int32, idColectiva);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@Accion", DbType.String, accion);
                database.AddInParameter(command, "@Observaciones", DbType.String, obs);         

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Acción en la Bitácora de Actividades del Autorizador", elUsuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
