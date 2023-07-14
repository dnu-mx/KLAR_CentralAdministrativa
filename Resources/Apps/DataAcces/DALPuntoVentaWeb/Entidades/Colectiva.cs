using System;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Colectiva
    /// </summary>
    public class Colectiva
    {
        public Int64 ID_Colectiva { get; set; }
        public String ClaveColectiva { get; set; }
        public String NombreORazonSocial { get; set; }
        public String NombreComercial { get; set; }
        public String APaterno { get; set; }
        public String AMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public String RFC { get; set; }
        public String CURP { get; set; }
        public String Telefono { get; set; }
        public String Movil { get; set; }
        public String Email { get; set; }
        public Int64 IdCadenaComercial { get; set; }
        public String CadenaRelacionada { get; set; }
        public Int64 IdCadenaRelacionada { get; set; }
        public Int32 IdEstatus { get; set; }
        public String Password { get; set; }
        public String RePassword { get; set; }
    }
}
