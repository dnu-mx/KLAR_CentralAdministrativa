using DALAclaraciones.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALAclaraciones.BaseDatos
{
    static public class DAOCatalogos
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public static DataSet ListaGruposMedioAcceso(Usuario elUsuario, Guid AppID)
        public static DataSet ListaGruposMedioAcceso()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_ConsultaGruposMA");
                //database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                //database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataSet ListaReasonCodes()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_ConsultaCLRReasonCodes");
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataSet ListaDocumentIndicator()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_ConsultaCLRDI");
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8007, ex.Message);
            }
        }
    }
}
