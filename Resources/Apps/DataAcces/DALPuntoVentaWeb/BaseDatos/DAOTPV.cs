using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOTPV
    {
        public static DataSet ListaTPVs(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_ObtenerTPVS");
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

        public static int asignarMenu(Int32 ID_Menu, Int32 ID_TPV, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_AsignarMenuATPV");

                database.AddInParameter(command, "@ID_Menu", DbType.Int32, ID_Menu);
                database.AddInParameter(command, "@ID_Terminal", DbType.Int32, ID_TPV);
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

        public static int desasignarMenu(Int32 ID_TPV, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_DesasignarMenu ATPV");

                database.AddInParameter(command, "@ID_Terminal", DbType.Int32, ID_TPV);
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

    }
}
