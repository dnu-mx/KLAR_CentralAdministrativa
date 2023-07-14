using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;


namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOPropiedad
    {
        
        public static void Modicar(Propiedad laPropiedad, Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {

               // Dictionary<Guid, Aplicacion> lasApps = new Dictionary<Guid, Aplicacion>();
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_ModificarPropiedad");

                database.AddInParameter(command, "@ID_Cadena", DbType.String, Id_cadena);
                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);


                database.ExecuteNonQuery(command);

               
            }
            catch (Exception Ex)
            {
                //Loguear.Error(ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al obtener las propiedades de la Aplicacion: " + Ex);
            }

        }

        public static void Agregar(Propiedad laPropiedad, String CadenaConexion, IUsuario elUser)
        {
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {

                //Dictionary<Guid, Aplicacion> lasApps = new Dictionary<Guid, Aplicacion>();
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_AgregarPropiedad");

                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);
                database.AddInParameter(command, "@AppID", DbType.Guid, Guid.NewGuid());
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

        public static List<Propiedad> ObtenerPropiedades(Int64 ID_CadenaComercial, String CadenaConexion)
        {
            DataSet losDatos = null;
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {

                //Dictionary<Guid, Aplicacion> lasApps = new Dictionary<Guid, Aplicacion>();
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneParametrosCadena");
                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, ID_CadenaComercial);

                losDatos = database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        Respuesta.Add(new Propiedad((String)losDatos.Tables[0].Rows[k]["Name"], (String)losDatos.Tables[0].Rows[k]["Value"], (String)losDatos.Tables[0].Rows[k]["Descripcion"], (Int32)losDatos.Tables[0].Rows[k]["ID_SystemValue"]));
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

        /// <summary>
        /// Obtiene los parámetros de la cadena comercial en base de datos
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadenade conexión a BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static List<Propiedad> ObtenerParametrosEnPDV(Int64 ID_CadenaComercial, String CadenaConexion, IUsuario elUsuario, Guid AppID)
        {
            DataSet losDatos = null;
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);                
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_ObtieneParametrosCadena");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, ID_CadenaComercial);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                losDatos = database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        Respuesta.Add(new Propiedad((String)losDatos.Tables[0].Rows[k]["Name"], (String)losDatos.Tables[0].Rows[k]["Value"], (String)losDatos.Tables[0].Rows[k]["Descripcion"], (Int32)losDatos.Tables[0].Rows[k]["ID_SystemValue"]));
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ha sucedido un error al obtener los parámetros en punto de venta de la Cadena: " + Ex);
            }

            return Respuesta;
        }

        /// <summary>
        /// Actualiza el valor de los parámetros en base de datos
        /// </summary>
        /// <param name="laPropiedad">Datos del parámetro por actualizar</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaParametros(Propiedad laPropiedad, Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_ActualizaParametrosCadena");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, Id_cadena);
                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros en punto de venta de la Cadena: " + Ex);
            }
        }

        /// <summary>
        /// Actualiza la versión de los parámetros en base de datos
        /// </summary>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaVersionParametros(Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_ActualizaVersionParametrosCadena");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, Id_cadena);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar la versión de los parámetros de la Cadena: " + Ex);
            }
        }

        /// <summary>
        /// Obtiene los parámetros de las condiciones comerciales de la cadena en base de datos
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadenade conexión a BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Lista del tipo Propiedad con los parámetros</returns>
        public static List<Propiedad> ObtenerCondicionesComerciales(Int64 ID_CadenaComercial, String CadenaConexion, IUsuario elUsuario, Guid AppID)
        {
            DataSet losDatos = null;
            List<Propiedad> Respuesta = new List<Propiedad>();

            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_ObtieneCondicionesComerciales");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, ID_CadenaComercial);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                losDatos = database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        Respuesta.Add(new Propiedad((String)losDatos.Tables[0].Rows[k]["Name"], (String)losDatos.Tables[0].Rows[k]["Value"], (String)losDatos.Tables[0].Rows[k]["Descripcion"], (Int32)losDatos.Tables[0].Rows[k]["ID_SystemValue"]));
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ha sucedido un error al obtener las condiciones comerciales de la Cadena: " + Ex);
            }

            return Respuesta;
        }

        /// <summary>
        /// Actualiza el valor de los parámetros de las condiciones comerciales de la cadena comercial en base de datos
        /// </summary>
        /// <param name="laPropiedad">Datos del parámetro por actualizar</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaCondiciones(Propiedad laPropiedad, Int64 Id_cadena, String CadenaConexion, IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_ActualizaValorContrato");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, Id_cadena);
                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);

                database.ExecuteNonQuery(command);
            }

            catch (Exception Ex)
            {
                Loguear.Error(Ex, elUser.ClaveUsuario);
                throw new Exception("Ha sucedido un error al actualizar los parámetros de las condiciones comerciales de la Cadena: " + Ex);
            }
        }

        /// <summary>
        /// Inserta el registro del cambio en un parámetro en la bitácora detalle de base de datos
        /// </summary>
        /// <param name="laPropiedad">Datos del parámetro por registrar</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaRegistroBitacoraServ(Propiedad laPropiedad, String CadenaConexion, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_InsertaCambioParamBitacoraDetalle");

                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Registro en la Bitácora Detalle de ServicioTPV", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception("Ha sucedido un error al insertar un registro en la Bitácora Detalle de ServicioTPV: " + ex);
            }
        }

        /// <summary>
        /// Inserta el registro del cambio en el valor del contrato de la cadena comercial en la bitácora detalle de base de datos
        /// </summary>
        /// <param name="laPropiedad">Datos del parámetro por registrar</param>
        /// <param name="CadenaConexion">Cadena de conexión</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void InsertaRegistroBitacoraAut(Propiedad laPropiedad, String CadenaConexion, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(CadenaConexion);
                DbCommand command = database.GetStoredProcCommand("web_CA_PDV_InsertaCambioValorContratoBitacoraDetalle");

                database.AddInParameter(command, "@ValorNuevo", DbType.String, laPropiedad.Valor);
                database.AddInParameter(command, "@UserName", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@Nombre", DbType.String, laPropiedad.Nombre);

                database.ExecuteNonQuery(command);
                Loguear.Evento("Se Insertó una Registro en la Bitácora Detalle del Autorizador", elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception("Ha sucedido un error al insertar un registro en la Bitácora Detalle del Autorizador: " + ex);
            }
        }
    }
}
