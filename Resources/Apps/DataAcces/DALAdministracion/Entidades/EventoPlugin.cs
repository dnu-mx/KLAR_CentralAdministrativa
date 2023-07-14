using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Evento
    /// </summary>
    public class EventoPlugin
    {
        public int      ID_Evento               { get; set; }
        public int      ID_Plugin               { get; set; }
        public bool     Activo                  { get; set; }
        public int      OrdenEjecucion          { get; set; }
        public bool     RespuestaISO            { get; set; }
        public bool     ObligatorioEnReverso    { get; set; }
    }
}
