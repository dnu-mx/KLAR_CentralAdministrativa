using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public class DtoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
 
    }

    public class DtoListFull : DtoList
    {
        public bool Active { get; set; }
        public bool Selected { get; set; }
    }
}
