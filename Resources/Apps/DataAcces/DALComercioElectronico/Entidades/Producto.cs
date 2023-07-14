using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public  class Producto
    {
        public int producto_id {get;set;}
        public string sku {get;set;}
        public string familia_id {get;set;}
        public string nombre {get;set;}


        public string descripcion {get;set;}
        public string path_imagen {get;set;}
        public bool activo {get;set;}
        public int secuencia {get;set;}

        
        public string modificado_por {get;set;}
        public DateTime fecha_modificacion { get; set; }
        public string Insertado_por {get;set;}
        public DateTime fecha_Insertado { get; set; }


        public int? IdCombo {get;set;}        



        //extra        
        public string NombreFamilia { get; set; }
    }
}
