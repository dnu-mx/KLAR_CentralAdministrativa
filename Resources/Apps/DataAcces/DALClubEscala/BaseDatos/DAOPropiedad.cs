using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DALCentralAplicaciones.Entidades;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Configuration;
using Interfases;
using DALClubEscala.Entidades;
//using DALCentralAplicaciones.Utilidades;

namespace DALClubEscala.BaseDatos
{
    public class DAOPropiedad
    {
        
        public static void Modicar(Propiedad laPropiedad, IUsuario elUser)
        {
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {

               // Dictionary<Guid, Aplicacion> lasApps = new Dictionary<Guid, Aplicacion>();
                SqlDatabase database = new SqlDatabase(DBClubEscala.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_ModificarPropiedad");

                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);
                database.AddInParameter(command, "@AppID", DbType.Guid, laPropiedad.AppID);
                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@UserID", DbType.String, elUser.ClaveUsuario);

                database.ExecuteNonQuery(command);

               
            }
            catch (Exception Ex)
            {
                //Loguear.Error(ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al obtener las propiedades de la Aplicacion: " + Ex);
            }

        }

        public static void Agregar(Propiedad laPropiedad, IUsuario elUser)
        {
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {

                //Dictionary<Guid, Aplicacion> lasApps = new Dictionary<Guid, Aplicacion>();
                SqlDatabase database = new SqlDatabase(DBClubEscala.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_AgregarPropiedad");

                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);
                database.AddInParameter(command, "@AppID", DbType.Guid, laPropiedad.AppID);
                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@UserID", DbType.String, elUser.ClaveUsuario);
                database.AddInParameter(command, "@Descripcion", DbType.String, elUser.ClaveUsuario);

                database.ExecuteNonQuery(command);


            }
            catch (Exception Ex)
            {
               // Loguear.Error(ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al obtener las propiedades de la Aplicacion: " + Ex);
            }

        }

        public static List<Propiedad> ObtenerPropiedades(Guid AppId)
        {
            DataSet losDatos = null;
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {

                //Dictionary<Guid, Aplicacion> lasApps = new Dictionary<Guid, Aplicacion>();
                SqlDatabase database = new SqlDatabase(DBClubEscala.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneParametros");
                database.AddInParameter(command, "@AppID", DbType.Guid, AppId);

                losDatos = database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        Respuesta.Add(new Propiedad((String)losDatos.Tables[0].Rows[k]["Name"], (String)losDatos.Tables[0].Rows[k]["Value"], (String)losDatos.Tables[0].Rows[k]["Descripcion"], (Guid)losDatos.Tables[0].Rows[k]["ID_SystemValue"], (Guid)losDatos.Tables[0].Rows[k]["ApplicationId"], (String)losDatos.Tables[0].Rows[k]["ValueDescription"]));
                    }
                }
            }
            catch (Exception Ex)
            {
                //Loguear.Error(ex, "");
                throw new Exception("Ha sucedido un error al obtener las propiedades de la Aplicacion: " + Ex);
            }

            return Respuesta;
        }

    }
}
