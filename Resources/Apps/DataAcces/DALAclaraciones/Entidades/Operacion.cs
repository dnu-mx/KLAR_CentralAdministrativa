using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfases;

namespace DALAclaraciones.Entidades
{
    public class Operacion
    {
        public String Tarjeta { get; set; }
        public Int16 ID_GrupoMA { get; set; }
        public float Importe { get; set; }
        public DateTime FechaInicial  { get; set; }
        public DateTime FechaFinal { get; set; }

        public Int64 Id_Operacion { get; set; }
        public float ImporteOper { get; set; }
        public float ImporteAcl { get; set; }
        public int Id_RC { get; set; }
        public int Id_DI { get; set; }
        public String Observaciones { get; set; }
    }

}
