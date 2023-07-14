using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALPuntoVentaWeb.Entidades
{

    public class WsTokenErrorResponse
    {
        public DateTime timestamp { get; set; }
        public int status { get; set; }
        public string error { get; set; }
        public Error[] errors { get; set; }
        public string message { get; set; }
        public string path { get; set; }
    }

    public class Error
    {
        public string[] codes { get; set; }
        public object[] arguments { get; set; }
        public string defaultMessage { get; set; }
        public string objectName { get; set; }
        public string field { get; set; }
        public string rejectedValue { get; set; }
        public bool bindingFailure { get; set; }
        public string code { get; set; }
    }

}
