using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public class Empresa
    {
        public int ID_Empresa { get; set; }

        public string ClaveEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public bool AsociarCorreo { get; set; }
        public string DominiosCorreo { get; set; }
        public DateTime? FechaInsert { get; set; }
        public string UsuarioInsert { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
    }
}