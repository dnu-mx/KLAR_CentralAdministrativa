using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public class Sucursal
    {
        public int id_sucursal { get; set; }
        //public int ID_SUCURSAL { get; set; }
        public string clave { get; set; }
        public string nombre { get; set; }

        public int? id_sucursal_madre;

        public int? id_suc_sustituta { get; set; }


        

        public int? secuencia;


        public string path_imagen { get; set; }
        public string coordenadas { get; set; }

        public string responsable { get; set; }
     


        public string calle  { get; set; }
        public string colonia  { get; set; }
        public string ciudad { get; set; }
        public string estado  { get; set; }

        public int cp  { get; set; }
        public string telefono { get; set; }
        public bool activa { get; set; }

        public decimal? cargo_envio;
        public decimal? minimo_para_entrega;
        

public string modificado_por { get; set; }
public DateTime fecha_modificacion { get; set; }
public string Insertado_por { get; set; }
public DateTime fecha_Insertado{ get; set; }


public string URL_PuntoVenta;

        


    }

    public class DtoSucursal
    {
        public int id_sucursal { get; set; }

        public List<AreaServicio> AreasServicios { get; set; }

    }

}

