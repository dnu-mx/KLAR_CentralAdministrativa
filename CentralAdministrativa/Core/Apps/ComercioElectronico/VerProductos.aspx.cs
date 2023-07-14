using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALCentralAplicaciones;
using DALComercioElectronico.BaseDatos;
using DALComercioElectronico.Entidades;

namespace ComercioElectronico
{
    public partial class VerProductos : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Ext.Net.Theme"] = Ext.Net.Theme.Default;
        }

        public List<Producto> VerProductosFull()
        {
            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var values = DaoProductosCombos.GetProductos(user, idApp);

            return values;
        }


    }
}