using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALCentroContacto.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Cliente
    /// </summary>
    public class Cliente
    {
        public int      ID_Cliente      { get; set; }
        public int      ID_MA           { get; set; }
        public int      ID_Cuenta       { get; set; }
        public string   Nombre          { get; set; }
        public string   ApellidoPaterno { get; set; }
        public string   ApellidoMaterno { get; set; }
        public string   MedioAcceso     { get; set; }
        public int      ID_Cadena       { get; set; }
        public string   Email           { get; set; }
        public string   Telefono        { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int      IdDireccion     { get; set; }
        public string   CodigoPostal    { get; set; }
        public int      IdColonia       { get; set; }
        public string   Colonia         { get; set; }
        public string   ClaveMunicipio  { get; set; }
        public string   Municipio       { get; set; }
        public string   ClaveEstado     { get; set; }
        public string   Estado          { get; set; }
        public string   ClaveColectiva  { get; set; }
    }
}
