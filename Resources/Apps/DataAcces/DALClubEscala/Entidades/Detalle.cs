using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALClubEscala.Entidades
{
    public class Detalle
    {
        public Int64  ID_Detalle  { get; set; }
        public Int64 ID_Archivo { get; set; }
        public String FilaCompleta { get; set; }
        public bool EsProcesado { get; set; }

    }
}
