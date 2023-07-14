using System;

namespace DALPuntoVentaWeb.Entidades
{
    public class PersonaMoral
    {
        public Int64    ID_PersonaMoral             { get; set; }
        public String   ClaveEmpresa                { get; set; }
        public String   RazonSocial                 { get; set; }
        public String   RFC                         { get; set; }
        public String   GiroEmpresa                 { get; set; }
        public String   RepresentanteLegal          { get; set; }
        public String   Telefono                    { get; set; }
        public String   CorreoElectronico           { get; set; }
        public String   Calle                       { get; set; }
        public String   NumeroExterior              { get; set; }
        public String   NumeroInterior              { get; set; }
        public String   EntreCalles                 { get; set; }
        public String   Referencias                 { get; set; }
        public String   CodigoPostal                { get; set; }
        public String   Municipio                   { get; set; }
        public String   Ciudad                      { get; set; }
        public String   Estado                      { get; set; }
        public String   Pais                        { get; set; }
        public String   Latitud                     { get; set; }
        public String   Longitud                    { get; set; }
        public String   OrigenRecursos              { get; set; }
        public String   DestinoRecursos             { get; set; }
        public String   TipoRecursos                { get; set; }
        public String   NaturalezaRecursos          { get; set; }
        public String   DesempenaFuncionesPublicas  { get; set; }
        public String   ParentescoPPE               { get; set; }
        public String   SocioDePPE                  { get; set; }
        public String   DineroIngresoPorMes         { get; set; }
        public String   NumVecesDeIngresoPorMes     { get; set; }
        public String   CuandoIngresaDineroPorMes   { get; set; }
        public String   CentroCostos                { get; set; }
        public String   Colonia                     { get; set; }
        public String   CLABE                       { get; set; }
        public String   Estatus                     { get; set; }
        public String   NumeroTarjeta               { get; set; }
        public String   TipoManufactura             { get; set; }
    }
}
