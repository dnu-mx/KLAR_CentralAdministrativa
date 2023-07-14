using DALCentralAplicaciones.Entidades;
using DALCentroContacto.Entidades;
using DALLoyalty.BaseDatos;
using DALLoyalty.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALCentroContacto.BaseDatos
{
    /// <summary>
    /// Objetos de acceso a datos para la funcionalidad de Consulta de Clientes
    /// </summary>
    public class DAOPrecalculoPuntosCash
    {
        /// <summary>
        /// Consulta los registros de la programación del precalculo de puntos 
        /// </summary>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtienePrecalculoPuntos(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneProgramaPrecalculo");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva progamacion a base de datos
        /// </summary>
        /// <param name="fecha">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaPrecalculo(DateTime fecha, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_InsertaProgramaPrecalculo");

                database.AddInParameter(command, "@FechaCalcular", DbType.Date, fecha);
                database.AddInParameter(command, "@Usuario", DbType.String, elUsuario.ClaveUsuario);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una nueva progrmación de precalculo.", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta una nueva progamacion a base de datos
        /// </summary>
        /// <param name="fecha">Identificador del cliente</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaPrecalculo(int id, int estatus, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaEstatusProgramaPrecalculo");

                database.AddInParameter(command, "@ID_Programacion", DbType.Int32, id);
                database.AddInParameter(command, "@ID_EstatusPrecalculo", DbType.Int32, estatus);
                database.AddInParameter(command, "@Usuario", DbType.String, elUsuario.ClaveUsuario);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se actualizó la progrmación de precalculo.", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene el reporte de precalculo realizado
        /// </summary>
        public static DataTable ObtieneReportePrecalculo(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneReportePrecalculo");

                //return database.ExecuteDataSet(command);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

    }
}
