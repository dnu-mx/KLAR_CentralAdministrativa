using System;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Parámetro de Grupo de Medios de Acceso
    /// </summary>
    public class ParametroGMA_V2
    {
        public ParametroGMA_V2()
        {

        }

        public ParametroGMA_V2(Int32 idparametro, String nombre, String descripcion, String valoractual, String valorpendiente)
        {
            ID_Parametro = idparametro;
            Nombre = nombre;
            Descripcion = descripcion;
            ValorActual = valoractual;
            ValorPendiente = valorpendiente;
        }

        public ParametroGMA_V2(String nombre, String valor)
        {
            Nombre = nombre;
            Valor = valor;
        }

        public Int32    ID_Parametro        { get; set; }
        public String   Nombre              { get; set; }
        public String   Descripcion         { get; set; }
        public String   Valor               { get; set; }
        public String   ValorActual         { get; set; }
        public String   ValorPendiente      { get; set; }
    }
}

