using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALValidacionesBatchPPF.LogicaNegocio.Dtos
{
    public class DtoChartRequest
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public List<object> Elements { get; set; }


        public List<string> Labels { get; set; }
        public List<string> Series { get; set; }
        public List<List<int>> Data { get; set; }
        public List<List<int>> DataPercent { get; set; }
        public List<List<decimal>> DataAmount { get; set; }

        public List<List<int>> DataAmountPercent { get; set; }


    }
}