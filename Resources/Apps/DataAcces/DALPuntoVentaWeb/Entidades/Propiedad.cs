using System;

namespace DALPuntoVentaWeb.Entidades
{
    public class Propiedad
    {

        public Propiedad()
        {

        }

        public Propiedad(String nombre, String valor, String descripcion, Int32 ID_systemvalue)
        {
            Nombre = nombre;
            Valor = valor;
            Descripcion = descripcion;
            ID_SystemValue = ID_systemvalue;


        }

        public Propiedad(String nombre, String valor)
        {
            Nombre = nombre;
            Valor = valor;
        }

        public String Nombre { get; set; }
        public String Valor { get; set; }
        public String Descripcion { get; set; }
        public Int32 ID_SystemValue { get; set; }

        
    }
}
