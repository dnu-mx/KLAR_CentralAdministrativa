using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALCentroContacto.BaseDatos
{
    public class DAOMonitoreo
    {
        /// <summary>
        /// Consulta los datos de monitoreo para registro, dentro del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista de objetos Monitoreo con los datos</returns>
        public static DataTable ListaDatosParaRegistro(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneDatosParaMonitoreo");

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
        /// Inserta el registro base en bitácora del dato de monitoreo enviado para llenado del usuario
        /// </summary>
        /// <param name="idDatoMonitoreo">Identificador del dato de monitoreo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaRegistroBaseEnBitacora(int idDatoMonitoreo, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_InsertaBitacoraBaseMonitoreo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMonitoreo", idDatoMonitoreo));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta el registro en bitácora del dato de monitoreo enviado para llenado del usuario
        /// </summary>
        /// <param name="idDatoMonitoreo">Identificador del dato de monitoreo</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaRegistroBitacora(int idDatoMonitoreo, String valor, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizador.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_ActualizaBitacoraMonitoreo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdMonitoreo", idDatoMonitoreo));
                        command.Parameters.Add(new SqlParameter("@Valor", valor));
                        command.Parameters.Add(new SqlParameter("@Usuario", elUsuario.ClaveUsuario));

                        conn.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
