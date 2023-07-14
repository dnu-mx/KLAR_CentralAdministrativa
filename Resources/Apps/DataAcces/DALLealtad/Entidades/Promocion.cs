using System;
using System.Collections.Generic;

namespace DALLealtad.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Promoción
    /// </summary>
    public class Promocion
    {
        public int Id_Promocion { get; set; }
        public String ClavePromocion { get; set; }
        public String Descripcion { get; set; }
        public String ClaveCadena { get; set; }
        public String Cadena { get; set; }
        public String Giro { get; set; }
        public String TituloPromocion { get; set; }
        public String TipoDescuento { get; set; }
        public String Restricciones { get; set; }
        public int? EsHotDeal { get; set; }
        public int? CarruselHome { get; set; }
        public int? PromoHome { get; set; }
        public int? Orden { get; set; }
        public DateTime? VigenciaInicio { get; set; }
        public DateTime? VigenciaFin { get; set; }
        public int Activa { get; set; }
        public int? ID_Clasificacion { get; set; }
        public String PalabrasClave { get; set; }

        public String URLCupon { get; set; }
        public int? ID_Genero { get; set; }
        public int? ID_TipoRedencion { get; set; }
        public int? ID_PromoPlus { get; set; }
        public List<int> RangosEdad { get; set; }
    }
}
