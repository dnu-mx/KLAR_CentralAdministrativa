using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class Vigencia
    {
      public Int32 ID_Vigencia {get; set;}
      public string Clave {get; set;}
      public string Descripcion {get; set;}
      public Int32 ID_TipoVigencia {get; set;}
      public DateTime FechaInicial {get; set;}
      public DateTime FechaFinal {get; set;}
      public TimeSpan HoraInicial {get; set;}
      public TimeSpan HoraFinal {get; set;}
      public Int32 ID_Periodo {get; set;}
    }
}
