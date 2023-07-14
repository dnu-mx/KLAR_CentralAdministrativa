using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Framework;
using FreezeCode.IceCubes.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Mensajeria;
using System.Web.Security;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;


namespace SitioInterno
{
    public partial class BloqueoTelefonos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            


            try
            {

                if (!this.IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                 


                    Bind();
                }

            }
            catch (Exception)
            {

                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            Bind();
        }

        protected void SetStatus(Object sender, CommandEventArgs e)
        {

           try
            {
                Telefono.SetEstatusTelefono(e.CommandArgument.ToString());
                Bind();
            }
            catch (Exception)
            {
            }
        }

        protected void RepeaterTarjetas_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {

        }

        public void Bind()
        {
            PagedDataSource objPage = new PagedDataSource();

            try
            {

               // Cliente clienteNuevo = Cliente.Get(Context.User.Identity.Name);

  //              clienteNuevo.PhoneNumbers.GetAll();
  
                

                objPage.AllowPaging = true;
                List<Telefono> misTelefonos =  BuscarTelefono(txtEmail.Text, TxtTelefono.Text, Telefonos.GetAll());
                objPage.DataSource = misTelefonos;
                //Assigning the datasource to the 'objPage' object.


                //Setting the Pagesize
                try
                {
                    objPage.PageSize = Int16.Parse((string)Configuracion.GetConfiguracion("Paginacion"));
                }
                catch (Exception)
                {
                    objPage.PageSize = 10;
                }

                if (misTelefonos.Count < objPage.PageSize)
                {
                    HttpContext.Current.Session.Add("CurrentPageTelefonos", 0);
                }

                //"CurrentPage" is public static variable to hold the current page index value declared in the global section.
                try
                {
                    if ((int)HttpContext.Current.Session.Contents["CurrentPageTelefonos"] < 0)
                    {
                        objPage.CurrentPageIndex = 0;
                    }
                    else
                    {
                        objPage.CurrentPageIndex = (int)HttpContext.Current.Session.Contents["CurrentPageTelefonos"];
                    }
                }
                catch (Exception)
                {
                    HttpContext.Current.Session.Add("CurrentPageTelefonos", 0);
                    objPage.CurrentPageIndex = 0;
                }


                if (misTelefonos.Count < objPage.PageSize)
                {
                    back.Visible = false;
                    next.Visible = false;
                    HttpContext.Current.Session.Add("CurrentPageTelefonos", 0);
                }
                else
                {
                    back.Visible = true;
                    next.Visible = true;
                }

                //Assigning Datasource to the DataList.
                RepeaterTelefonos.DataSource = objPage;
                //dlGallery.DataKeyField = "Image_ID";
                RepeaterTelefonos.DataBind();

                next.Enabled = !objPage.IsLastPage;
                back.Enabled = !objPage.IsFirstPage;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected void lbtnPrev_Click(object sender, EventArgs e)
        {
            int pagina = (int)HttpContext.Current.Session.Contents["CurrentPageTelefonos"] - 1;
            HttpContext.Current.Session.Add("CurrentPageTelefonos", pagina);
            Bind();
        }

        protected void lbtnNext_Click(object sender, EventArgs e)
        {

            int pagina = (int)HttpContext.Current.Session.Contents["CurrentPageTelefonos"] + 1;
            HttpContext.Current.Session.Add("CurrentPageTelefonos", pagina);
            Bind();

        }

        protected List<Telefono> BuscarTelefono(String Email, String numero, List<Telefono> lasTelefonos)
        {
            List<Telefono> resultado;


            resultado = (from tel in lasTelefonos
                         where tel.Numero.ToUpper().Contains(numero.ToUpper()) && tel.IdPropietario.ToUpper().Contains(Email.ToUpper())
                         orderby tel.Numero, tel.IdPropietario
                         select tel).ToList();

            return resultado;

        }

    }
}