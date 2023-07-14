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

namespace TpvWeb
{
    public partial class VentaTAE : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                  
                }
            }
            catch (Exception )
            {
               // Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }

        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {

        }
    }
}