using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCentralAplicaciones.Utilidades;

namespace Facturas
{
    public partial class LaGrafica : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!X.IsAjaxRequest)
                {
                    Int64 ID_Receptor = HttpContext.Current.Session["ID_Receptor"] == null ? 0 : (Int64)HttpContext.Current.Session["ID_Receptor"];

                    this.Store1.DataSource = DALAutorizador.BaseDatos.DAOFactura.ObtieneDataSetGraficarFactura(ID_Receptor);
                    this.Store1.DataBind();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "FACTURACION");
            }
        }
    }
}