using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using DALAutorizador.BaseDatos;
using System.Data.Common;
using DALPuntoVentaWeb.Entidades;

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOMenu
    {
        public static DataSet ListaMenus(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_ObtieneMenus");
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

        public static int insertar(MenuTPV elMenuTPV, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_InsertarMenu");

                database.AddInParameter(command, "@Clave_Menu", DbType.String, elMenuTPV.Clave);
                database.AddInParameter(command, "@Descripcion_Menu", DbType.String, elMenuTPV.Descripcion);
                database.AddInParameter(command, "@Version_Menu", DbType.Int32, elMenuTPV.Version);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                
                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Agregado un MenuTPV al Autorizador", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int actualizar(MenuTPV elMenuTPV, Usuario elUsuario, Guid AppID)
        {

            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_ActualizarMenu");

                database.AddInParameter(command, "@ID_Menu", DbType.Int32, elMenuTPV.ID_Menu);
                database.AddInParameter(command, "@Clave_Menu", DbType.String, elMenuTPV.Clave);
                database.AddInParameter(command, "@Descripcion_Menu", DbType.String, elMenuTPV.Descripcion);
                database.AddInParameter(command, "@Version_Menu", DbType.Int32, elMenuTPV.Version);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento(String.Format("Se ha actualizado el menú {0}", elMenuTPV.ID_Menu), elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int eliminar(MenuTPV elMenuTPV, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_EliminarMenu");

                int codigoRespuesta = -1;

                database.AddInParameter(command, "@ID_Menu", DbType.Int32, elMenuTPV.ID_Menu);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha eliminado un menú del Autorizador", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ListaPromocionesMenus(Int32 ID_Menu, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_ObtienePromocionesMenu");
                database.AddInParameter(command, "@ID_Menu", DbType.Int32, ID_Menu);
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

        public static int asignarPromocion(Int32 ID_Menu, Int32 ID_Promocion, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_AsignarPromocionAMenu");

                database.AddInParameter(command, "@ID_Menu", DbType.Int32, ID_Menu);
                database.AddInParameter(command, "@ID_Promocion", DbType.Int32, ID_Promocion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha asignado un Menu a una TPV", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int desasignarPromocion(Int32 ID_Menu, Int32 ID_Promocion, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_DesasignarPromocionAMenu");

                database.AddInParameter(command, "@ID_Menu", DbType.Int32, ID_Menu);
                database.AddInParameter(command, "@ID_Promocion", DbType.Int32, ID_Promocion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha desasignado un Menu a una TPV", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int subirBajarPromocion(Int32 ID_Menu, Int32 ID_Promocion, Int32 subirBajar, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_OrdenPromocionMenu");

                database.AddInParameter(command, "@ID_Menu", DbType.Int32, ID_Menu);
                database.AddInParameter(command, "@ID_Promocion", DbType.Int32, ID_Promocion);
                database.AddInParameter(command, "@Accion", DbType.Int32, subirBajar);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha cambiado el orden de la promoción", elUsuario.ClaveUsuario);
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
