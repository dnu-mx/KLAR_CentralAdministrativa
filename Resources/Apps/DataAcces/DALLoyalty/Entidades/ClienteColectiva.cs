using System;

namespace DALCentroContacto.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Cliente
    /// </summary>
    public class ClienteColectiva
    {
        public int      ID_Cliente      { get; set; }
        public int      ID_Colectiva    { get; set; }
        public string   Nombre          { get; set; }
        public string   ApellidoPaterno { get; set; }
        public string   ApellidoMaterno { get; set; }
        public string   Email           { get; set; }
        public string   Telefono        { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string   CorreoEmpresa   { get; set; }
        public string   ClaveEmpresa    { get; set; }
        public string   LugarTrabajo    { get; set; }
        public string   CodigoPostal    { get; set; }
        public string   RFC             { get; set; }
        public int      ID_MA           { get; set; }
        public string   Tarjeta         { get; set; }
        public string   Membresia       { get; set; }
    }
}
