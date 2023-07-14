using System;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Parámetro Extra
    /// </summary>
    public class ParametroExtra
    {
        public ParametroExtra()
        {

        }

        public ParametroExtra(Int32 id_parametro, String nombre, String valor, String descripcion, 
            Int32 id_parametroprestablecido)
        {
            ID_Parametro = id_parametro;
            Nombre = nombre;
            Descripcion = descripcion;
            Valor = valor;
            ID_ParametroPrestablecido = id_parametroprestablecido;
        }
        
        public Int32    ID_Parametro                { get; set; }
        public String   Nombre                      { get; set; }
        public String   Descripcion                 { get; set; }
        public String   Valor                       { get; set; }
        public Int32    ID_ParametroPrestablecido   { get; set; }

    }
}
