using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.LogicaNegocio.Dtos;


namespace ValidacionBatch.Controllers.Business
{
    public class QueriesController : ApiController
    {
        public string get()
        {

            return "alive";

        }
        [HttpGet]
        public DtoChartRequest GetGraficaTransacciones([FromUri]DtoChartRequest dtoChartRequest)
        {
            var queryData = DaoEfectivale.DataQuery.GraficaTransacciones(dtoChartRequest.DateStart, dtoChartRequest.DateEnd);
            
            dtoChartRequest.Labels = new List<string>();

            dtoChartRequest.Data = new List<List<int>>();
            dtoChartRequest.DataPercent = new List<List<int>>();
            dtoChartRequest.DataAmount = new List<List<decimal>>();
            dtoChartRequest.DataAmountPercent = new List<List<int>>();


            dtoChartRequest.Series = queryData.Select(m => m.Movimiento).Distinct().ToList();

            var months = queryData.Select(m => m.Mes).Distinct();
            months = months.OrderBy(m => m);


            //var countDays = new List<List<DtoTransaccion>>();

            List<KeyValuePair<int, int>> pairMonthDays= new List<KeyValuePair<int, int>>();

            foreach (var month in months)
            {
                var month1 = month;
                var days = queryData.Where(m => m.Mes == month1).OrderBy(m => m.Dia).ToList();
                
                //countDays.Add(days);


                foreach (var transaction in days)
                {
                    dtoChartRequest.Labels.Add(month + "/" + transaction.Dia);

                    if(!pairMonthDays.Exists(m=>m.Key==month&&m.Value==transaction.Dia))
                        pairMonthDays.Add(new KeyValuePair<int, int>(month,transaction.Dia));
                    
                    //var transaction1 = transaction;
                    //var operations = queryData.Where(m => m.Dia == transaction1.Dia && m.Mes == month1);
                    //dtoChartRequest.Data.Add();
                }

                dtoChartRequest.Labels = dtoChartRequest.Labels.Distinct().ToList();



            }

            foreach (var serieName in dtoChartRequest.Series)
            {
                
                var serie1 = serieName;
                var trans = queryData.Where(m => m.Movimiento == serie1);

                //var serie = new IEnumerable<int>();

                var serie = new List<int>();

                var serie2 = new List<decimal>();

                foreach (var pairMonthDay in pairMonthDays)
                {

                    var  dayAmount = queryData.FirstOrDefault(m => m.Movimiento == serie1&&m.Mes==pairMonthDay.Key&&m.Dia==pairMonthDay.Value);

                    if (dayAmount == null)
                        serie.Add(0);
                    else
                        serie.Add(dayAmount.Operaciones);
                    //dayAmount

                }

                //var serie = trans.Select(m => m.Operaciones);


                dtoChartRequest.Data.Add(serie.ToList());

                dtoChartRequest.DataPercent.Add(serie.ToList());
                dtoChartRequest.DataAmountPercent.Add(serie.ToList());




                foreach (var pairMonthDay in pairMonthDays)
                {

                    var dayAmount = queryData.FirstOrDefault(m => m.Movimiento == serie1 && m.Mes == pairMonthDay.Key && m.Dia == pairMonthDay.Value);

                    if (dayAmount == null)
                        serie2.Add(0);
                    else
                        serie2.Add(dayAmount.Monto);
                    //dayAmount

                }

                //var serie2 = trans.Select(m => m.Monto).ToList();
                dtoChartRequest.DataAmount.Add(serie2);
            }


            //dtoChartRequest.DataPercent=new List<List<int>>(dtoChartRequest.Data);
            //dtoChartRequest.DataAmountPercent= new List<List<int>>(dtoChartRequest.Data);

            if (dtoChartRequest.Data.Any())
                if (dtoChartRequest.Data[0].Any())
                    for (int z = 0; z < dtoChartRequest.Data[0].Count; z++)
                    {
                        var total = (decimal)(dtoChartRequest.Data[0][z] + dtoChartRequest.Data[1][z]);
                        var val1 = (decimal)dtoChartRequest.Data[0][z];
                        var val2 = (decimal)dtoChartRequest.Data[1][z];
                        dtoChartRequest.DataPercent[0][z] = (int)(val1 / total * 100);
                        dtoChartRequest.DataPercent[1][z] = (int)(val2 / total * 100);


                        
                        var totalB = (decimal)(dtoChartRequest.DataAmount[0][z] + dtoChartRequest.DataAmount[1][z]);
                        var val1b = (decimal)dtoChartRequest.DataAmount[0][z];
                        var val2b = (decimal)dtoChartRequest.DataAmount[1][z];
                        dtoChartRequest.DataAmountPercent[0][z] = (int)(val1b / totalB * 100);
                        dtoChartRequest.DataAmountPercent[1][z] = (int)(val2b / totalB * 100);


                    }


            dtoChartRequest.Elements = queryData.ConvertAll(m => (object)m);

            return dtoChartRequest;


        }


        [HttpGet]
        public DtoChartRequest GetGraficaTarjetas([FromUri] DtoChartRequest dtoChartRequest)
        {
            var queryData = DaoEfectivale.DataQuery.GraficaTarjetas(dtoChartRequest.DateStart, dtoChartRequest.DateEnd);




            dtoChartRequest.Labels = new List<string>();

            dtoChartRequest.Data = new List<List<int>>();

            dtoChartRequest.Series = new List<string>() { "Rechazadas", "Autorizadas" };


            List< int> autorizadas=new List<int>();
            List<int> rechazadas= new List<int>();
            var months = queryData.Select(m => m.Mes).Distinct();
            months = months.OrderBy(m => m);


            //var countDays = new List<List<DtoTransaccion>>();

            List<KeyValuePair<int, int>> pairMonthDays = new List<KeyValuePair<int, int>>();

            foreach (var month in months)
            {
                var month1 = month;
                var days = queryData.Where(m => m.Mes == month1).OrderBy(m => m.Dia).ToList();

                


                foreach (var transaction in days)
                {
                    dtoChartRequest.Labels.Add(month + "/" + transaction.Dia);

                    
                    rechazadas.Add(transaction.TarjetasRechazadas);
                    autorizadas.Add(transaction.TarjetasAutorizadas);

                    //if (!pairMonthDays.Exists(m => m.Key == month && m.Value == transaction.Dia)) 
                        //pairMonthDays.Add(new KeyValuePair<int, int>(month, transaction.Dia));

                    //var transaction1 = transaction;
                    //var operations = queryData.Where(m => m.Dia == transaction1.Dia && m.Mes == month1);
                    //dtoChartRequest.Data.Add();
                }

                dtoChartRequest.Labels = dtoChartRequest.Labels.Distinct().ToList();

            }

            
            dtoChartRequest.Data.Add(rechazadas);
            dtoChartRequest.Data.Add(autorizadas);




            //dtoChartRequest.DataAmount = new List<List<decimal>>();







            return dtoChartRequest;

        }


        [HttpGet]
        public DtoChartRequest GetGraficaIndicencias([FromUri]DtoChartRequest dtoChartRequest)
        {
            var queryData = DaoEfectivale.DataQuery.GraficaIncidencias(dtoChartRequest.DateStart, dtoChartRequest.DateEnd);



            dtoChartRequest.Series = new List<string>() { "Incidencias" };

            dtoChartRequest.Data = new List<List<int>>();
            dtoChartRequest.Data.Add(new List<int>());

            dtoChartRequest.Labels = new List<string>();


            var months = queryData.Select(m => m.Mes).Distinct();
            months = months.OrderBy(m => m);

            foreach (var month in months)
            {
                var month1 = month;
                var days = queryData.Where(m => m.Mes == month1).OrderBy(m => m.Dia);

                foreach (var transaction in days)
                {
                    dtoChartRequest.Labels.Add(month + "/" + transaction.Dia);


                    var transaction1 = transaction;
                    var opreation = queryData.First(m => m.Dia == transaction1.Dia && m.Mes == month1);

                    dtoChartRequest.Data[0].Add(opreation.Incidencias);

                }

                dtoChartRequest.Labels = dtoChartRequest.Labels.Distinct().ToList();




            }

            //foreach (var label in dtoChartRequest.Labels)
            //{
            //    //var transaction1 = transaction;
            //    var opreation = queryData.First(m => m.Dia == transaction1.Dia && m.Mes == month1);

            //    dtoChartRequest.Data[0].Add(opreation.Incidencias);
            //}


            dtoChartRequest.Elements = queryData.ConvertAll(m => (object)m);


            return dtoChartRequest;
        }


        public string Get()
        {
            return "Alive";
            //retreturn dtoChartRequest;
        }
    }
}