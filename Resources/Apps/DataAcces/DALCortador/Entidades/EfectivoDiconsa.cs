using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCortador.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Efectivo Diconsa (recepción de efectivo)
    /// </summary>
    public class EfectivoDiconsa
    {
        public int      IdEvento                { get; set; }
        public long     IdColectivaOrigen       { get; set; }
        public int      IdTipoColectivaOrigen   { get; set; }
        public long     IdColectivaDestino      { get; set; }
        public int      IdTipoColectivaDestino  { get; set; }
        public string   Importe                 { get; set; }
        public string   Concepto                { get; set; }
        public string   Observaciones           { get; set; }
        public long     Referencia              { get; set; }

    }
}

