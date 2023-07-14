using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALCentroContacto.Entidades
{
    public class ReporteCliente
    {
        public int idColectiva { get; set; }
        public string idMedioAcceso { get; set; }
        public string tipoMedioAcceso { get; set; }
        public string nombreCliente { get; set; }
        public string correoCliente { get; set; }
        public string fechaNacimiento { get; set; }
        public string saldoActual { get; set; }
        public string estatus { get; set; }
        public string motivoBloqueo { get; set; }
        public string cadenaActivacion { get; set; }
        public string numeroSucursalActivacion { get; set; }
        public string nombreSucursalActivacion { get; set; }
        public string fechaActivacion { get; set; }
        public string usuarioActivacion { get; set; }
        public string correoUsuarioActivacion { get; set; }
        //public string usuarioBAja { get; set; }
        //public string correoBaja { get; set; }
        //public string fechaBaja { get; set; }
    }
}
