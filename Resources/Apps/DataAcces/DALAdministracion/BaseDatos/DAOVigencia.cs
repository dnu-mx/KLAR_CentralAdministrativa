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
    /// Objeto de acceso a datos para la entidad Vigencia
    /// </summary>
    public class DAOVigencia
    {
        /// <summary>
        /// Inserta el registro en base de datos de la nueva vigencia
        /// </summary>
        /// <param name="laVigencia">Nueva vigencia</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void insertar(Vigencia laVigencia, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaVigencia");

                database.AddInParameter(command, "@Clave", DbType.String, laVigencia.Clave);
		        database.AddInParameter(command, "@Descripcion", DbType.String, laVigencia.Descripcion);
		        database.AddInParameter(command, "@ID_TipoVigencia", DbType.Int32, laVigencia.ID_TipoVigencia);
                if (laVigencia.FechaInicial == DateTime.MinValue)
                {
                    database.AddInParameter(command, "@FechaIncial", DbType.Date, null);
                }
                else
                {
                    database.AddInParameter(command, "@FechaIncial", DbType.Date, laVigencia.FechaInicial);
                }
                if (laVigencia.FechaFinal == DateTime.MinValue)
                {
                    database.AddInParameter(command, "@FechaFinal", DbType.Date, null);
                }
                else
                {
                    database.AddInParameter(command, "@FechaFinal", DbType.Date, laVigencia.FechaFinal);
                }
                if (laVigencia.HoraInicial == TimeSpan.Zero)
                {
                    database.AddInParameter(command, "@HoraInicial", DbType.Time, null);
                }
                else
                {
                    database.AddInParameter(command, "@HoraInicial", DbType.Time, Convert.ToDateTime(Convert.ToString(laVigencia.HoraInicial)));
                }
                if (laVigencia.HoraFinal == TimeSpan.Zero)
                {
                    database.AddInParameter(command, "@HoraFinal", DbType.Time, null);
                }
                else
                {
                    database.AddInParameter(command, "@HoraFinal", DbType.Time, Convert.ToDateTime(Convert.ToString(laVigencia.HoraFinal)));
                }
                if (laVigencia.ID_Periodo == 0)
                {
                    database.AddInParameter(command, "@ID_Periodo", DbType.Int32, null);
                }
                else 
                {
                    database.AddInParameter(command, "@ID_Periodo", DbType.Int32, laVigencia.ID_Periodo);
                }

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Nueva Vigencia al Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Actualiza una vigencia en base de datos
        /// </summary>
        /// <param name="laVigencia">Valores de la vigencia a actualizar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static int actualizar(Vigencia laVigencia, Usuario elUsuario)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizarVigencia");

                database.AddInParameter(command, "@ID_Vigencia", DbType.Int32, laVigencia.ID_Vigencia);
                database.AddInParameter(command, "@Clave", DbType.String, laVigencia.Clave);
                database.AddInParameter(command, "@Descripcion", DbType.String, laVigencia.Descripcion);
                database.AddInParameter(command, "@ID_TipoVigencia", DbType.Int32, laVigencia.ID_TipoVigencia);
                database.AddInParameter(command, "@FechaIncial", DbType.Date, laVigencia.FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, laVigencia.FechaFinal);
                database.AddInParameter(command, "@HoraInicial", DbType.Time, Convert.ToDateTime(Convert.ToString(laVigencia.HoraInicial)));
                database.AddInParameter(command, "@HoraFinal", DbType.Time, Convert.ToDateTime(Convert.ToString(laVigencia.HoraFinal)));
                database.AddInParameter(command, "@ID_Periodo", DbType.Int32, laVigencia.ID_Periodo);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Actualizado una Vigencia en el Autorizador", elUsuario.ClaveUsuario);
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
