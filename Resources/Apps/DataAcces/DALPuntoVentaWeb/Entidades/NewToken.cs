using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALPuntoVentaWeb.Entidades
{
    public class NewToken
    {
        public String email { get; set; }
        public String name { get; set; }
        public String password { get; set; }
        public String description { get; set; }
        public long colectivaId { get; set; }
    }
}
