using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administracion
{
    public class Util
    {
        public static IDictionary<string, string> getDirectEventArgsValues(DirectEventArgs dea) 
        {
            string json = String.Format("[{0}]", dea.ExtraParams["Values"]);

            IDictionary<string, string>[] seleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (seleccionados == null || seleccionados.Length < 1)
            {
                return null;
            }

            return seleccionados[0];
        }
    }
}