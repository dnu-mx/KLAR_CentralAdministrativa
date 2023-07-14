using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALClubEscala.Entidades
{
    public class Propiedad
    {

        public Propiedad()
        {

        }

        public Propiedad(String nombre, String valor, String descripcion, Guid ID_systemvalue, Guid appID, String valueDescription)
        {
            Nombre = nombre;
            Valor = valor;
            Descripcion = descripcion;
            ID_SystemValue = ID_systemvalue;
            AppID = appID;
            ValueDescription = valueDescription;

        }

        
        public String Nombre { get; set; }
        public String Valor { get; set; }
        public String Descripcion { get; set; }
        public String ValueDescription { get; set; }
        public Guid ID_SystemValue { get; set; }
        public Guid AppID { get; set; }
        
    }
}
