using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALLealtad.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class DAOEcommerceEdoMex
    {
        /// <summary>
        /// Obtiene el reporte de promociones con los filtros indicados
        /// </summary>
        /// <param name="nombre">nombre</param>
        /// <param name="apellidoPat">apellido paterno</param>
        /// <param name="apellidoMat">apellido materno</param>
        /// <param name="correo">correo</param>
        /// <param name="placa">placa</param>
        /// <param name="CURP">CURP</param>
        /// <param name="FechaInicial">Fecha inicial del proceso</param>
        /// <param name="FechaFinal">Fecha final del proceso</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneReporteClientesEdoMex(string nombre, string apellidoPat, string apellidoMat, string correo, 
            string placa, string CURP, DateTime FechaInicial, DateTime FechaFinal, Usuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEcommerceEdoMex.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ReporteClientesEdoMex");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@Nombre", DbType.String, nombre.Equals("") ? null : nombre);
                database.AddInParameter(command, "@ApellidoPat", DbType.String, apellidoPat.Equals("") ? null : apellidoPat);
                database.AddInParameter(command, "@ApellidoMat", DbType.String, apellidoMat.Equals("") ? null : apellidoMat);
                database.AddInParameter(command, "@Correo", DbType.String, correo.Equals("") ? null : correo);
                database.AddInParameter(command, "@Placa", DbType.String, placa.Equals("") ? null : placa);
                database.AddInParameter(command, "@CURP", DbType.String, CURP.Equals("") ? null : CURP);
                database.AddInParameter(command, "@FechaInicial", DbType.Date, FechaInicial.Equals(DateTime.MinValue) ? (DateTime?)null : FechaInicial);
                database.AddInParameter(command, "@FechaFinal", DbType.Date, FechaFinal.Equals(DateTime.MinValue) ? (DateTime?)null : FechaFinal);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
