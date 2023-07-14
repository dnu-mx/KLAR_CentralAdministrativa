using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;
using System.Web.Security;
using Ext.Net;
using DALCentralAplicaciones.Utilidades;

namespace Usuarios
{
    public partial class CentralUsuariosEliminar : DALCentralAplicaciones.PaginaBaseCAPP
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
    }
}