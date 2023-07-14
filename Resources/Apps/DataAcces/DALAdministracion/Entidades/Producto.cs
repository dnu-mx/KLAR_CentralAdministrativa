using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class Producto
    {
        public Dictionary<UInt16, ConfiguracionGMA> Menus = new Dictionary<UInt16, ConfiguracionGMA>();
        public Int64 ID_GrupoMA;
        public String ClaveGrupo;
        public String Descripcion;
        public Int64 ID_Vigencia;
        public String Vigencia;
    }
}
