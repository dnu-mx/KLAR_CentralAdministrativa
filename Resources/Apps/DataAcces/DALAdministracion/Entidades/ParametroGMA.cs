using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Parámetro Grupo de Medios de Acceso (Producto)
    /// </summary>
    public class ParametroGMA
    {
        public ParametroGMA()
        {

        }

        public ParametroGMA(String nombre, String valor, String descripcion, Int32 ID_systemvalue)
        {
            Nombre = nombre;
            Valor = valor;
            Descripcion = descripcion;
            ID_SystemValue = ID_systemvalue;
        }

        public ParametroGMA(String nombre, String valor)
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

