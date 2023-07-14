using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public class SucursalHorario
    {
        public int id { get; set; }
        public int id_sucursal { get; set; }

        public string horarios { get; set; }

        public string modificado_por { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public string Insertado_por { get; set; }
        public DateTime fecha_Insertado { get; set; }

         public List<WorkDay> WorkDays {get;set;}

         public List<DtoListFull> DaysOfWeek { get; set; }



         
    }

    public class WorkDay 
    {
        public string DayStr { get; set; }
        public int IdDay { get; set; }
        public List<DateTime> Range { get; set; }
    }


    


}

