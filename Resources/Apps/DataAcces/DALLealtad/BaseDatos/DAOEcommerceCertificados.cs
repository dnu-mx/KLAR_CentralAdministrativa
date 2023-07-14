using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DALLealtad.BaseDatos
{
    public class DAOEcommerceCertificados
    {
        
        /// <summary>
        /// Obtiene el reporte de folios con los filtros indicados
        /// </summary>
        /// <param name="Nombre">Nombre del cliente</param>
        /// <param name="Apellido">Apellido paterno del cliente</param>
        /// <param name="Correo">Correo electrónico del cliente</param>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="IdPedido">Identificador del pedido</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteFoliosSmartGifts(String Nombre, String Apellido, String Correo, DateTime FechaInicial,
            DateTime FechaFinal, int IdPedido, Usuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneReporteFolios()", "");

                SqlDatabase database = new SqlDatabase(BDEcommerceCertificados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteCertificados");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@Nombre", DbType.String, Nombre);
                database.AddInParameter(command, "@Apellido", DbType.String, Apellido);
                database.AddInParameter(command, "@Correo", DbType.String, Correo);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal);
                database.AddInParameter(command, "@IdPedido", DbType.Int32, IdPedido);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneReporteFolios()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
