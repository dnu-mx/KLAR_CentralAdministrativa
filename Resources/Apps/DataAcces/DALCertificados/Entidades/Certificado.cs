using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALCertificados.Entidades
{
    public class Certificado
    {
             public Guid ID_Certificado  { get; set; }
             public Int64 ID_CadenaComercial { get; set; }
             public Int64 ID_ColectivaTerminal { get; set; }

             public Int64 ID_GrupoComercial { get; set; }
             public String Sucursal { get; set; }
             public String Afiliacion { get; set; }
             public String Terminal { get; set; }
             public int ID_Estatus { get; set; }
             public String Clave { get; set; }
             public DateTime FechaCreacion { get; set; }
             public String UsuarioCreacion { get; set; }
             public DateTime FechaCaducidad{ get; set; }
             public int ID_Activacion { get; set; }
             public int DiasExpiracion { get; set; }
             public String UsuarioActivacion { get; set; }
             public DateTime FechaActivacion { get; set; }
             public String MAC { get; set; }
             public String IDPROC { get; set; }
             public String IDMB { get; set; } 
             public String IDWIN { get; set; }
             public String ClaveCadena { get; set; }

             public Certificado()
             {
                 this.Sucursal = "";
                 this.Afiliacion = "";
                 this.Terminal = "";
                 this.ClaveCadena = "";
             }
    }
}
