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

namespace Cajero
{
    public partial class EdoCtaBatch : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                    //if (!LNPermisos.EsPaginaPermitida((FormsIdentity)Context.User.Identity, Request))
                    //{
                    //    Response.Redirect("../AccesoRestringido.aspx");
                    //}

                    ////Obtiene el Objeto Usuario para los datos necesarios de la Aplicacion.
                    //if ( == null)
                    //{
                    //    Usuario elUsuario = DALCentralAplicaciones.BaseDatos.DAOUsuario.ObtieneCaracteristicasUsuario(Context.User.Identity.Name);
                    //    HttpContext.Current.Session.Add("usuario", elUsuario);
                    //}
                }
            }
            catch (Exception err)
            {
                DALCajero.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }

        }


        protected void RowSelect(object sender, DirectEventArgs e)
        {


        }
    }
}