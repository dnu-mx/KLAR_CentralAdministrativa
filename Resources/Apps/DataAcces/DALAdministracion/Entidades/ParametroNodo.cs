using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Nodo
    /// </summary>
    public class ParametroNodo
    {
        public int      ID_ValorNodo        { get; set; }
        public int      ID_ScoreNodo        { get; set; }
        public string   ValorKey            { get; set; }
        public string   DescripcionValor    { get; set; }        
    }
}
