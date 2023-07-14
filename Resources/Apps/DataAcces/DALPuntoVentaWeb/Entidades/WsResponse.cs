using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALPuntoVentaWeb.Entidades
{
    public class WsResponse
    {
        public bool success { get; set; }
        public int code { get; set; }
        public List<String> Messages { get; set; }
    }
}
