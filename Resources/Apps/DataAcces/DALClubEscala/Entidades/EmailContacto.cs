using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALClubEscala.Entidades
{
    /// <summary>
    /// Clase que establece las propiedades del objeto Contacto
    /// </summary>
    public class EmailContacto
    {
        public String Nombre { get; set; }
        public String Email { get; set; }
        public String Asunto { get; set; }
        public String Mensaje { get; set; }
    }
}
