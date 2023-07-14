using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Datos Sr Pago
    /// </summary>
    public class DatosSrPago
    {
        public int      IdTienda                { get; set; }
        public string   ClaveAlmacen            { get; set; }
        public string   ClaveTienda             { get; set; }
        public string   NombreTienda            { get; set; }
        public string   Calle                   { get; set; }
        public string   Localidad               { get; set; }
        public string   Municipio               { get; set; }
        public string   Estado                  { get; set; }
        public string   Telefono                { get; set; }
        public string   Password                { get; set; }
        public string   Email                   { get; set; }
        public int      IdOperador              { get; set; }
        public string   NombreOperador          { get; set; }
        public string   ApellidoPaternoOperador { get; set; }
        public string   ApellidoMaternoOperador { get; set; }
        public string   FechaNacimientoOperador { get; set; }
        public string   CodigoPostal            { get; set; }
        public string   NumeroTarjeta           { get; set; }
        public string   URL_IFE_Anverso         { get; set; }
        public string   URL_IFE_Reverso         { get; set; }
        public string   URL_Firma               { get; set; }
        public string   URL_CompDomicilio       { get; set; }
        public string   TipoDocumentoURL        { get; set; }
        public string   URLDocumento            { get; set; }
    }
}
