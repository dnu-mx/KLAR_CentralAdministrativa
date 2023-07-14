using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALCentralAplicaciones.Utilidades;
using DALCentralAplicaciones.Entidades;
using System.Configuration;
using Log_PCI.Entidades;

namespace Empresarial
{
    public partial class PDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                    String NombreArchivo = "";
                    if (HttpContext.Current.Session.Contents["usuario"] == null)
                    {
                        LogHeader logTEMP = new LogHeader();
                        Usuario elUsuario = DALCentralAplicaciones.BaseDatos.DAOUsuario.ObtieneCaracteristicasUsuario(
                            Context.User.Identity.Name, logTEMP);
                        HttpContext.Current.Session.Add("usuario", elUsuario);
                    }

                    if (Request.QueryString["file"] != null)
                    {
                        NombreArchivo = Request.QueryString["file"].ToString();// "Estado_Cuenta_2012593015959740.pdf";
                    }

                    String Path = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "URLReportes").Valor;



                    Response.Clear();
                    Response.ContentType = "Application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo);
                    Response.TransmitFile(Path + NombreArchivo);
                    Response.End();
                
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
            }
        }
    }
}