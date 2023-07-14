using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALPuntoVentaWeb.Entidades
{
    public class WsTokenOkResponse
    {
        public string expiresIn { get; set; }
        public string tokenType { get; set; }
        public string jwt { get; set; }
        public string refresh_token { get; set; }
    }

}
