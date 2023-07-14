using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class Rango
    {
        public int      ID_GrupoMA  { get; set; }
        public int      ID_Rango    { get; set; }
        public int      ID_TipoMA   { get; set; }
        public string   DescTipoMA  { get; set; }
        public string   Clave       { get; set; }
        public string   Descripcion { get; set; }
        public decimal  Inicio      { get; set; }
        public decimal  Fin         { get; set; }
        public bool     esActivo    { get; set; }
    }
}
