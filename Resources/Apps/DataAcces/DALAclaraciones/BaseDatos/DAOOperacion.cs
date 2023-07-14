using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using DALAclaraciones.Entidades;
using System.Configuration;
using Interfases;
using Interfases.Exceptiones;
using System.Data.SqlClient;
using DALAclaraciones.Utilidades;

namespace DALAclaraciones.BaseDatos
{
    public class DAOOperacion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdOper"></param>
        /// <param name="elUsuario"></param>
        public static void EliminarOperacionContracargo (Int64 IdOper, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_EliminaOperacion");

                database.AddInParameter(command, "@IdOperacion", DbType.Int64, IdOper);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdOperacion"></param>
        /// <param name="elUsuario"></param>
        /// <returns></returns>
        public static DataSet ListaEnviosAContracargos(Int64 IdOperacion, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_ConsultaHistorialArchivo");
                database.AddInParameter(command, "@IdOperacion", DbType.Int64, IdOperacion);
                Loguear.Evento("Consulta Envios a Contracargo", elUsuario.ClaveUsuario);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elUsuario"></param>
        /// <param name="OperacionPorAclarar"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public static DataSet BuscaOperacionPorAclarar(IUsuario elUsuario, Operacion OperacionPorAclarar, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_BuscaOperacion");
                database.AddInParameter(command, "@Tarjeta", DbType.String, OperacionPorAclarar.Tarjeta);
                database.AddInParameter(command, "@Importe", DbType.Currency, OperacionPorAclarar.Importe);
                database.AddInParameter(command, "@FechaInicial", DbType.DateTime, OperacionPorAclarar.FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, OperacionPorAclarar.FechaFinal);
                database.AddInParameter(command, "@IdGrupoMA", DbType.Int16, OperacionPorAclarar.ID_GrupoMA);

                Loguear.Evento("Búsqueda de operación por aclarar", elUsuario.ClaveUsuario);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OpAclarar"></param>
        /// <param name="elUsuario"></param>
        public static void InsertarOperacionAclaracion(Operacion OpAclarar, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAclaraciones.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("Web_Aclaraciones_InsertaOperacion");

                database.AddInParameter(command, "@IdOperacion", DbType.Int64, OpAclarar.Id_Operacion);
                database.AddInParameter(command, "@ImporteOp", DbType.Decimal, decimal.Parse(OpAclarar.ImporteOper.ToString()));
                database.AddInParameter(command, "@ImporteAcl", DbType.Decimal, decimal.Parse(OpAclarar.ImporteAcl.ToString()));
                database.AddInParameter(command, "@IdReasonCode", DbType.Int32, OpAclarar.Id_RC);
                database.AddInParameter(command, "@IdDocumentIndicator", DbType.Int32, OpAclarar.Id_DI);
                database.AddInParameter(command, "@Observaciones", DbType.String, OpAclarar.Observaciones);
                
                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
