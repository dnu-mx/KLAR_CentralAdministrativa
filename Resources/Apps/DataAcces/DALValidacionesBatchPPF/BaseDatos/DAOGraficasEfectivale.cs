using DALAutorizador.BaseDatos;
using DALValidacionesBatchPPF.LogicaNegocio.Dtos;
using DALValidacionesBatchPPF.LogicaNegocio.Extensions;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DALValidacionesBatchPPF.BaseDatos
{
    /// <summary>
    /// 
    /// </summary>
    public class DAOGraficasEfectivale
    {
        public static class DataQuery
        {
            public static List<DtoTransaccion> GraficaTransacciones(DateTime fechaIni, DateTime fechaFin)
            {
                //fechaIni=fechaIni.AddHours((fechaIni.Hour )*-1);

                var str = BDAutorizador.strBDLectura;
                fechaIni = new DateTime(fechaIni.Year, fechaIni.Month, fechaIni.Day);

                fechaFin = fechaFin.AddHours((23 - fechaFin.Hour));
                try
                {
                    SqlDatabase database = new SqlDatabase(
                      str
                        //ConfigurationManager.AppSettings["BaseDatabase"]
                        //BDAutorizador.strBDLectura

                    );
                    //SqlDatabase database = new SqlDatabase(BdEcommerce.StrBdEcomerce);
                    DbCommand command = database.GetStoredProcCommand("web_CA_GraficaTransacciones");

                    database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                    database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);


                    var dataset = database.ExecuteDataSet(command);
                    var dataList = dataset.Tables[0].DataTableToList<DtoTransaccion>();
                    //vat test = (DataTable)dataset;
                    return dataList;

                    //database.AddInParameter(command, "@UserTemp", DbType.Guid, new Guid());
                    //elUsuario.UsuarioTemp
                    //database.AddInParameter(command, "@AppId", DbType.Guid,appId);


                    //var value = database.ExecuteScalar(command);

                    //var valueInt = (int) value;


                    //if (valueInt==55||valueInt==0) return  new Random(1).Next(1,9999);

                    //return valueInt;
                    //return (int)value;
                    //var dataset = database.ExecuteDataSet(command);


                    //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                    //vat test = (DataTable)dataset;
                    //return dataList;
                }

                catch (Exception ex)
                {
                    //Loguear.Error(ex, elUsuario.ClaveUsuario);
                    throw new CAppException(8010, ex.Message, ex);
                }
                return null;
            }

            public static List<DtoIncidencia> GraficaIncidencias(DateTime fechaIni, DateTime fechaFin)
            {
                fechaIni = new DateTime(fechaIni.Year, fechaIni.Month, fechaIni.Day);
                fechaFin = fechaFin.AddHours((23 - fechaFin.Hour));
                try
                {
                    SqlDatabase database = new SqlDatabase(
                     BDAutorizador.strBDLectura
                        //ConfigurationManager.AppSettings["BaseDatabase"]
                        //BDAutorizador.strBDLectura

                 );
                    //SqlDatabase database = new SqlDatabase(BdEcommerce.StrBdEcomerce);
                    DbCommand command = database.GetStoredProcCommand("web_CA_GraficaIncidencias");

                    database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                    database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);


                    var dataset = database.ExecuteDataSet(command);
                    var dataList = dataset.Tables[0].DataTableToList<DtoIncidencia>();
                    //vat test = (DataTable)dataset;
                    return dataList;
                }

                catch (Exception)
                {
                    //Loguear.Error(ex, elUsuario.ClaveUsuario);
                    //throw new CAppException(8010, ex.Message, ex);
                }
                return null;
            }


            public static List<DtoGrupoTarjeta> GraficaTarjetas(DateTime fechaIni, DateTime fechaFin)
            {
                fechaIni = new DateTime(fechaIni.Year, fechaIni.Month, fechaIni.Day);
                fechaFin = fechaFin.AddHours((23 - fechaFin.Hour));
                try
                {
                    SqlDatabase database = new SqlDatabase(
                     BDAutorizador.strBDLectura
                        //ConfigurationManager.AppSettings["BaseDatabase"]
                        //BDAutorizador.strBDLectura

                 );
                    //SqlDatabase database = new SqlDatabase(BdEcommerce.StrBdEcomerce);
                    DbCommand command = database.GetStoredProcCommand("web_CA_GraficaTarjetas");

                    database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                    database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);


                    var dataset = database.ExecuteDataSet(command);
                    var dataList = dataset.Tables[0].DataTableToList<DtoGrupoTarjeta>();
                    //vat test = (DataTable)dataset;
                    return dataList;
                }

                catch (Exception ex)
                {
                    //Loguear.Error(ex, elUsuario.ClaveUsuario);
                    throw new CAppException(8010, ex.Message, ex);
                }


                return null;
            }


        }
    }
}
