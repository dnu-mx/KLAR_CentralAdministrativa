using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCortador.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Evento Manual Ajuste Saldo Cupón
    /// </summary>
    public class EvManAjusteSaldoCupon
    {
        public int      IdEvento            { get; set; }
        public string   ClaveEvento         { get; set; }
        public long     IdColectiva         { get; set; }
        public int      IdTipoColectiva     { get; set; }
        public string   ClaveColectivaCCM   { get; set; }
        public string   Importe             { get; set; }
        public string   Concepto            { get; set; }
        public string   Observaciones       { get; set; }
        public string   MedioAcceso         { get; set; }
    }
}
