using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.BaseDatos
{
    public class DAOProgramas
    {
        public static DataSet ListarGrupoCuentas(Usuario elUsuario, Guid AppID)
        {
            try {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerProgramas");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            } catch(Exception ex) {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ListarColectivas(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_GMA_ObtenerColectivas");

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

        public static int insertar(GrupoCuenta elGrupoCuenta, Usuario elUsuario, Guid AppID)
        {
            int codigoRespuesta = -1;

            SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
            DbCommand command = database.GetStoredProcCommand("web_GMA_InsertaPrograma");

            database.AddInParameter(command, "@ID_ColectivaEmisor", DbType.Int32, elGrupoCuenta.ID_ColectivaEmisor);
            database.AddInParameter(command, "@ClaveGrupoCuenta", DbType.String, elGrupoCuenta.ClaveGrupoCuenta);
            database.AddInParameter(command, "@Descripcion", DbType.String, elGrupoCuenta.Descripcion);
            database.AddInParameter(command, "@ID_Vigencia", DbType.Int32, elGrupoCuenta.ID_Vigencia);
    
            database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
            database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

            codigoRespuesta = database.ExecuteNonQuery(command);
            Loguear.Evento("Se ha Agregado un GrupoCuenta al Autorizador", elUsuario.ClaveUsuario);

            return codigoRespuesta;
        }

        public static int actualizar(GrupoCuenta elGrupoCuenta, Usuario elUsuario, Guid AppID)
        {
            int codigoRespuesta = -1;

            SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
            DbCommand command = database.GetStoredProcCommand("web_GMA_ActualizaPrograma");

            database.AddInParameter(command, "@ID_GrupoCuenta", DbType.Int32, elGrupoCuenta.ID_GrupoCuenta);
            database.AddInParameter(command, "@ID_ColectivaEmisor", DbType.Int32, elGrupoCuenta.ID_ColectivaEmisor);
            database.AddInParameter(command, "@ClaveGrupoCuenta", DbType.String, elGrupoCuenta.ClaveGrupoCuenta);
            database.AddInParameter(command, "@Descripcion", DbType.String, elGrupoCuenta.Descripcion);
            database.AddInParameter(command, "@ID_Vigencia", DbType.Int32, elGrupoCuenta.ID_Vigencia);

            database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
            database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

            codigoRespuesta = database.ExecuteNonQuery(command);
            Loguear.Evento("Se ha Actualizado un GrupoCuenta al Autorizador", elUsuario.ClaveUsuario);

            return codigoRespuesta;
        }
    }
}
