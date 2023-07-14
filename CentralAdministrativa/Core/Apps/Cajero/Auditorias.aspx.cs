using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCentralAplicaciones.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using DALCajero.Utilidades;

namespace Cajero
{
    public partial class Auditorias : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                                  }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {

        }
    }
}