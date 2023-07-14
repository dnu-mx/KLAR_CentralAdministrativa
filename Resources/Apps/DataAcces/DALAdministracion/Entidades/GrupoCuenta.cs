using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class GrupoCuenta
    {
        public Int32 ID_GrupoCuenta;
        public Int32 ID_ColectivaEmisor;
        public string ColectivaDescripcion;
        public string ClaveGrupoCuenta;
        public string Descripcion;
        public Int32 ID_Vigencia;
        public string VigenciaDescripcion;
    }
}
