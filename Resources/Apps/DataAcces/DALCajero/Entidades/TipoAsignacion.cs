using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCajero.Entidades
{
    public class TipoAsignacion
    {

        public int ID_TipoAsignacion { get; set; }
        public String Clave { get; set; }
        public String Descripcion  { get; set; }
        public DateTime HoraInicialPermitida { get; set; }
        public DateTime HoraFinalPermitida { get; set; }
        public Boolean Lunes { get; set; }
        public Boolean Martes { get; set; }
        public Boolean Miercoles { get; set; }
        public Boolean Jueves { get; set; }
        public Boolean Viernes { get; set; }
        public Boolean Sabado { get; set; }
        public Boolean Domingo { get; set; }
        public int MaximoDiasDespuesDelDeposito { get; set; }

            
    }
}
