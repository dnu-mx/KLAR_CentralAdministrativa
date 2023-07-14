using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCortador.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Depósito de Recolector (evento manual)
    /// </summary>
    public class EventoDepositoRecolector
    {
        public int      IdEvento                { get; set; }
        public string   ClaveEvento             { get; set; }
        public long     IdColectivaOrigen       { get; set; }
        public int      IdTipoColectivaOrigen   { get; set; }
        public string   ClaveColectiva          { get; set; }
        public string   Importe                 { get; set; }
        public string   Concepto                { get; set; }
        public string   Observaciones           { get; set; }
        public long     Referencia              { get; set; }
    }
}
