using System;

namespace DALLealtad.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Campana
    /// </summary>
    public class ObjetoPrograma
    {
        public int ID_ProgramaObjeto { get; set; }
        public int ?ID_Entidad { get; set; }
        public int ID_TipoObjeto { get; set; }
        public String Orden { get; set; }
        public int ID_TipoEntidad { get; set; }
        public String URL { get; set; }
        public String PathImagen { get; set; }
        public int ID_Programa { get; set; }
        public int Activo { get; set; }
    }
}
