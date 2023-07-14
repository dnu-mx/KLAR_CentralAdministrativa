using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Nodo
    /// </summary>
    public class Nodo
    {
        public int      ID_ScoreNodo        { get; set; }
        public string   NombreKey           { get; set; }
        public string   NombreNodoPadre     { get; set; }
        public bool     EsActivo            { get; set; }
    }
}
