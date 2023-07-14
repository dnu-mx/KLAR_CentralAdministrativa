using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALEventos.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Configuración Corte
    /// </summary>
    public class EventoConfigurador
    {
        public int      ID_ConfiguracionCorte   { get; set; }
        public int      ID_TipoCuenta           { get; set; }
        public int      ID_Evento               { get; set; }
        public int      ID_EstatusConfiguracion { get; set; }
        public int      ID_TipoContrato         { get; set; }
        public string   Nombre                  { get; set; }
        public string   Clave                   { get; set; }
        public string   Descripcion             { get; set; }
    }
}
