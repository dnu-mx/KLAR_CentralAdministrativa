using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Parámetro Multiasignación
    /// </summary>
    public class ParametroMA
    {
        public int ID_ValorParametroMultiasignacion { get; set; }
        public int ID_ParametroMultiasignacion { get; set; }
        public string NombrePMA { get; set; }
        public string DescripcionPMA { get; set; }
        public string ValorPMA { get; set; }
        public int ID_Entidad { get; set; }
        public int ID_CadenaComercial { get; set; }
        public int ID_Origen { get; set; }
        public int ID_Producto { get; set; }
        public int ID_Vigencia { get; set; }
    }
}
