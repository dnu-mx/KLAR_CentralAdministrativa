using System;

namespace DALLealtad.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Campana
    /// </summary>
    public class Campana
    {
        public int ID_Campana { get; set; }
        public String ClaveCampana { get; set; }
        public String NombreComercial { get; set; }
        public int ID_Programa { get; set; }
        public int Activo { get; set; }
        public string path { get; set; }
    }
}
