using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class GrupoMA
    {
        public Int32 ID_GrupoMA {get; set;}
        public string ClaveGrupo {get; set;}
        public string Descripcion {get; set;}
        public Int32 ID_Vigencia { get; set; }
    }
}
