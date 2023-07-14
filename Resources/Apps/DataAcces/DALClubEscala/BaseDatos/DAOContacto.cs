using DALClubEscala.Entidades;
using Framework;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DALClubEscala.BaseDatos
{
    /// <summary>
    /// Objeto de acceso a datos para la entidad Contacto
    /// </summary>
    public class DAOContacto
    {
        /// <summary>
        /// Inserta el registro de un nuevo correo electrónico recibido como contacto
        /// </summary>
        /// <param name="elContacto">Datos del contacto.</param>
        public static void InsertaEmailContacto(EmailContacto elContacto)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(ContextoInicial.ContextoServicio.ConnectionString);
                DbCommand command = database.GetStoredProcCommand("web_InsertaMailContacto");

                database.AddInParameter(command, "@Nombre", DbType.String, elContacto.Nombre);
                database.AddInParameter(command, "@Email", DbType.String, elContacto.Email);
                database.AddInParameter(command, "@Asunto", DbType.String, elContacto.Asunto);
                database.AddInParameter(command, "@Mensaje", DbType.String, elContacto.Mensaje);

                database.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
