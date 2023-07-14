using System;

namespace DALBovedaTarjetas.Entidades
{
    public class PeticionAltaTarjetas
    {
        public long         ID_Peticion             { get; set; }
        public int          ID_Producto             { get; set; }
        public string       ClaveTipoPeticion       { get; set; }
        public int          ID_ValorPrestablecido   { get; set; }
        public int          Cantidad                { get; set; }
        public string       NumLote                 { get; set; }
        public string       NombreArchivo           { get; set; }
        public int          ID_Colectiva            { get; set; }
        public string       RutaEntradaArchivos     { get; set; }
        public string       VigenciaPlasticos       { get; set; }
        public string       LlaveCifrado            { get; set; }
        public string       VectorCifrado           { get; set; }
        public string       NombreEmbozo            { get; set; }
        public DateTime?    FechaSolicitud          { get; set; }
        public string       EstatusPeticion         { get; set; }
        public string       EstatusProcesamiento    { get; set; }
        public string       Emisor                  { get; set; }
        public string       Producto                { get; set; }
        public string       TipoManufactura         { get; set; }
        public string       RutaArchivo             { get; set; }
        public string       ClaveEmisor             { get; set; }
    }
}
