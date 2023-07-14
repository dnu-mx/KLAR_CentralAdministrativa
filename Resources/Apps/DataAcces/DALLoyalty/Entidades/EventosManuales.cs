using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALCentroContacto.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Eventos Manuales
    /// </summary>
    public class EventosManuales
    {
        public int      IdEvento                { get; set; }
        public string   ClaveEvento             { get; set; }
        public long     IdColectiva             { get; set; }
        public int      IdTipoColectiva         { get; set; }
        public string   ClaveCadenaComercial    { get; set; }
        public string   Importe                 { get; set; }
        public string   Concepto                { get; set; }
        public string   Observaciones           { get; set; }
        public long     Referencia              { get; set; }
    }
}
