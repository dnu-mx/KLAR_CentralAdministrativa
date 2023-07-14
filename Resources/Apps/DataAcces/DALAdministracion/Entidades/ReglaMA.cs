using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Regla Multiasignación
    /// </summary>
    public class ReglaMA
    {
        public int ID_Regla { get; set; }
        public int ID_ReglaMultiasignacion { get; set; }
        public int ID_Entidad { get; set; }
        public int ID_CadenaComercial { get; set; }
        public int ID_Vigencia { get; set; }
        public int ID_Producto { get; set; }
        public int Prioridad { get; set; }
        public int OrdenEjecucionRMA { get; set; }
        public string NombreRMA { get; set; }
        public string VigenciaRMA { get; set; }
    }
}
