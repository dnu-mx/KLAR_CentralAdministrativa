using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALAdministracion.Entidades
{
    public class ConfiguracionGMA
    {
        public Dictionary<UInt16, ConfiguracionGMA> Ramas = new Dictionary<UInt16, ConfiguracionGMA>();
        public UInt16 IdConfiguracion { get; set; }
        public String Nombre { get; set; }


        public ConfiguracionGMA(String _Nombre, UInt16 _IdConfiguracion)
        {
            Nombre = _Nombre;
            IdConfiguracion = _IdConfiguracion;
        }

        public ConfiguracionGMA(String _Nombre)
        {
            Nombre = _Nombre;
        }
    }
}
