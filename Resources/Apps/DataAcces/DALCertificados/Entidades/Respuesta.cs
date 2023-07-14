using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCertificados.Entidades
{
    public class Respuesta
    {

        public Respuesta()
        {
        }

        public Respuesta(Int32 codigo, String Descripcion, String SufijoUsuario)
        {
            this.CodigoRespuesta = codigo;
            this.Descripcion = Descripcion;
            this.SufijoUser = SufijoUsuario;
        }

        public Int32 CodigoRespuesta { get; set; }
        public String Descripcion { get; set; }
        public String SufijoUser { get; set; }
    }
}
