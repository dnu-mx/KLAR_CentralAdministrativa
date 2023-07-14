using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Evento
    /// </summary>
    public class Evento
    {
        public int      ID_Evento       { get; set; }
        public int      ID_TipoEvento   { get; set; }
        public string   Clave           { get; set; }
        public string   Descripcion     { get; set; }
        public string   DescEdoCta      { get; set; }
        public bool     Activo          { get; set; }
        public bool     Reversable      { get; set; }
        public bool     Cancelable      { get; set; }
        public bool     Transaccional   { get; set; }
        public bool     GeneraPoliza    { get; set; }
        public bool     PreValidaciones { get; set; }
        public bool     PostValidaciones{ get; set; }
    }
}
