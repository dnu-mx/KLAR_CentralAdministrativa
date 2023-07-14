using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public class AreaServicio
    {
        public int id { get; set; }
        public int id_sucursal { get; set; }

        //public string coordenadas { get; set; }
        public int clave_asentamiento { get; set; }
        public string descripcion_asentamiento { get; set; }
        public int codigo_postal { get; set; }

        public string modificado_por { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public string Insertado_por { get; set; }
        public DateTime fecha_Insertado { get; set; }
         
    }



    public class Asentamiento
    {
        public string CveAsentamiento { get; set; }
        public string DesAsentamiento { get; set; }
        public string CveMunicipio { get; set; }
        public string CveCiudad { get; set; }
        public string CodigoPostal { get; set; }

        
        public string CveEstado { get; set; }

        public int ID_Asentamiento { get; set; }
        
    }
    


}

