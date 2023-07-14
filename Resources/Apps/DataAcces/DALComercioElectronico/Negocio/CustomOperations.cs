using DALComercioElectronico.Entidades;
using DALComercioElectronico.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Negocio
{
    public static class OperacionesCustomizadas
    {


        public static List<WorkDay> SetTimeSpans(this SucursalHorario ss)
        {
            var list = new List<WorkDay>();

            ss.WorkDays = list;

            var str = ss.horarios;
            var culture = new System.Globalization.CultureInfo("es-MX");


            ss.DaysOfWeek = CustomDaysOfWeek.GetDaysOfWeek();
                //CoreExtensions.EnumToDtoList<DayOfWeek>();

            

            if(!string.IsNullOrEmpty(str))
            {

                var datas = str.Split(',');
                

                foreach (var pair in datas)
                {
                    try
                    {
                        var dataPair = pair.Split('-');

                        var split1 = dataPair[0].Split(':');
                        var split2 = dataPair[1].Split(':');
                        
                        var dia =  int.Parse(split1[0]);

                        //var date1 = DateTime.Now;
                        //var date2 = DateTime.Now;

                         
                        //var span1 = new TimeSpan( int.Parse( split1[1]) ,int.Parse( split1[2]), 0);
                        //var span2 = new TimeSpan(int.Parse(split2[0]), int.Parse(split2[1]), 0);

                        //date1.Add(span1);
                        //date2.Add(span2);

                        var dateNow = DateTime.Now;

                        var hour1 = int.Parse(split1[1]) !=24? int.Parse(split1[1]):0 ;
                        var hour2 = int.Parse(split2[0]) !=24? int.Parse(split2[0]):0 ;


                        var date1 = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,  hour1, int.Parse(split1[2]), 0);
                        var date2 = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, hour2, int.Parse(split2[1]), 0);
//                        date1 = DateTime.Now;

                        //var day = ();
                        var diaStr = CustomDaysOfWeek.GetDaysOfWeek().First(m => m.Id == dia).Name;
                            //culture.DateTimeFormat.GetDayName((DayOfWeek)dia - 1).ToUpper(); ;

                        var tiempoSucursal = new WorkDay()
                        {
                            IdDay=dia,
                            DayStr=diaStr,
                            Range = new List<DateTime> { },
                           
                        };

                        tiempoSucursal.Range.Add(date1);
                        tiempoSucursal.Range.Add(date2);

                        list.Add(tiempoSucursal );

                        ss.WorkDays = list;


                    }
                    catch(Exception)
                    {
                        return list;
                    }
                }


            }


            return list;
        }

        public static string GenerateHorariosString(this SucursalHorario ss)
        {
            var list = new List<string>();
            //ss.WorkDays.Select(m=>m.IdDay).ToList().

            //if (ss.WorkDays.GroupBy(m => m.IdDay).Any(m => m.Count() > 1))
                //throw new Exception("No se puede registrar mas de una vez el mismo dia " + CustomDaysOfWeek.GetDaysOfWeek().First(z => z.Id== ss.WorkDays.GroupBy(m => m.IdDay).First(m => m.Count() > 1).First().IdDay).Name);

            foreach (var item in ss.WorkDays.OrderBy(m=>m.IdDay))
            {
                var str = "";
                str += item.IdDay.ToString() + ":";

                if (item.Range.Count < 2)
                    throw new Exception("Se requieren ambos horarios para el dia " + CustomDaysOfWeek.GetDaysOfWeek().First(m => m.Id == item.IdDay).Name);

                var time1= int.Parse(item.Range[0].ToString("HH"));
                var time2 = int.Parse(item.Range[1].ToString("HH"));

                var val1 = time1 == 0 ? 24 : time1;
                var val2 = time2 == 0 ? 24 : time2;

                str += val1 + item.Range[0].ToString(":mm") + "-";
                str += val2 + item.Range[1].ToString(":mm") + "";

                list.Add(str);
            }

            var data = string.Join(", ", list.ToArray());
            


            return data;
        }

    }
}
