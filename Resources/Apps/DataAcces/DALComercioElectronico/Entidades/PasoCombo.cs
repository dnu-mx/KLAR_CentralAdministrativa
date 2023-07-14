using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public  class PasoCombo
    {

        public int id { get; set; }
        public int secuencia { get; set; }
        public int id_producto {get;set;}
        public string descripcion { get; set; }


        public int cantidad { get; set; }
        

        public string modificado_por {get;set;}
        public DateTime fecha_modificacion { get; set; }
        public string Insertado_por {get;set;}
        public DateTime fecha_Insertado { get; set; }
        
        //extra
        public List<ProductoCombo> ProductosCombos { get; set; }


    }


    public class ProductoCombo
    {
        public int id { get; set; }
        public int id_producto { get; set; }
        public int id_pasos_combos { get; set; }





        public string modificado_por { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public string Insertado_por { get; set; }
        public DateTime fecha_Insertado { get; set; }



        //extra
        public string NameProducto { get; set; }
        public string SkuProducto { get; set; }


    }

    public class DtoProduct : DtoList
    {
        public string Sku { get; set; }
    }

}
