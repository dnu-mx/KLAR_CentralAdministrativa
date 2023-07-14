using DALCentralAplicaciones.Utilidades;
using Interfases;
using System;
using System.Data;
using System.IO;
using System.Xml;

namespace DALCajero.LogicaNegocio
{
    public class LNCatalogos
    {
        public static DataSet ListaTiposCuentaAutorizador(Guid AppID, IUsuario elUser)
        {
            DataSet ds = new DataSet();
            try
            {
               

                String xmlString = "";
                ws_CatalogosAut.WS_CatalogosAUTClient wsCatalogos = new ws_CatalogosAut.WS_CatalogosAUTClient();
               
                //ws_EstadosCuenta.WS_EstadoCuentaService edoCta = new ws_EstadosCuenta.WS_EstadoCuentaService();
                String usrWSTiposCadena =Configuracion.Get(AppID,"usrWSTiposCadena").Valor;
                String pswWSTiposCadena =Configuracion.Get(AppID,"pswWSTiposCadena").Valor;
                String eventoWSTiposCadena =Configuracion.Get(AppID,"eventoWSTiposCadena").Valor;

                xmlString = wsCatalogos.ObtieneCuentasUsuario(usrWSTiposCadena, pswWSTiposCadena, eventoWSTiposCadena, elUser.UsuarioTemp.ToString(),AppID.ToString());
                //xmlString = wsCatalogos.ObtieneCuentasUsuario(usrWSTiposCadena, pswWSTiposCadena, eventoWSTiposCadena, "faf4d443-40d7-41b6-9320-d705247fdf97", "5630846e-2d27-4347-b656-5722dd0d32cd");

                ds.ReadXml(new XmlTextReader(new StringReader(xmlString)));

                
            }
            catch (Exception err)
            {
                Loguear.Error(err, AppID.ToString());
                //throw err;
            }

            return ds;

        }

    }
}
