using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using Mensajeria;
using System.Web.Security;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;


namespace SitioInterno
{
    public partial class BloqueoTarjetas : DALCentralAplicaciones.PaginaBaseCAPP
    {
        //int CurrentPage =0;
        protected void Page_Load(object sender, EventArgs e)
        {


            try
            {

                if (!this.IsPostBack)
                {

                    


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

        protected void RepeaterTarjetas_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            
        }

        public void Bind()
        {
            PagedDataSource objPage = new PagedDataSource();

                try
                {
                    //Cliente clienteNuevo = Cliente.Get(Context.User.Identity.Name);
                    //clienteNuevo.TarjetasBanco.GetAll();
                        
                    objPage.AllowPaging = true;
     
                    //Assigning the datasource to the 'objPage' object.
                    //objPage.DataSource = clienteNuevo.TarjetasBanco;

                    List<Tarjeta> misTarjetas=BuscarTarjetas(txtitular.Text, txtMedioPago.Text, cardType.SelectedValue, Tarjetas.GetAll());
                    objPage.DataSource = misTarjetas;


                     //Setting the Pagesize
                      try
                      {
                          objPage.PageSize = Int16.Parse((string)Configuracion.GetConfiguracion("Paginacion"));
                      }
                      catch (Exception)
                      {
                          objPage.PageSize = 10;
                      }

                      if (misTarjetas.Count < objPage.PageSize)
                      {
                          HttpContext.Current.Session.Add("CurrentPageTarjetas", 0);
                      }
     
                     //"CurrentPage" is public static variable to hold the current page index value declared in the global section.
                     try
                     {
                         if ((int)HttpContext.Current.Session.Contents["CurrentPageTarjetas"] < 0)
                         {
                             objPage.CurrentPageIndex = 0;
                         }
                         else
                         {
                             objPage.CurrentPageIndex = (int)HttpContext.Current.Session.Contents["CurrentPageTarjetas"];
                         }
                     }
                     catch (Exception)
                     {
                         HttpContext.Current.Session.Add("CurrentPageTarjetas", 0);
                         objPage.CurrentPageIndex = 0;
                     }

                     if (misTarjetas.Count < objPage.PageSize)
                     {
                         back.Visible = false;
                         next.Visible = false;
                     }
                     else
                     {
                         back.Visible = true;
                         next.Visible = true;
                     }
   
                     //Assigning Datasource to the DataList.
                     RepeaterTarjetas.DataSource = objPage;
                     //dlGallery.DataKeyField = "Image_ID";
                     RepeaterTarjetas.DataBind();

                     next.Enabled = !objPage.IsLastPage;
                     back.Enabled = !objPage.IsFirstPage;

                }
                catch(Exception ex)
                {
                 throw ex;
                }

        }

        protected void lbtnPrev_Click(object sender, EventArgs e)
        {
            int pagina = (int)HttpContext.Current.Session.Contents["CurrentPageTarjetas"] - 1;
            HttpContext.Current.Session.Add("CurrentPageTarjetas", pagina);
            Bind();
        }

        protected void lbtnNext_Click(object sender, EventArgs e)
        {

            int pagina = (int)HttpContext.Current.Session.Contents["CurrentPageTarjetas"] + 1;
            HttpContext.Current.Session.Add("CurrentPageTarjetas", pagina);
            Bind();

        }

        protected List<Tarjeta> BuscarTarjetas(String Titular, String NumeroTarjeta, String TipoTarjeta, List<Tarjeta> lasTarjetas)
        {
            List<Tarjeta> resultado;

            if (TipoTarjeta == "0")
                TipoTarjeta = "";

            

                resultado = (from tarj in lasTarjetas
                             where tarj.Titular.ToUpper().Contains(Titular.ToUpper()) && tarj.Numero.ToUpper().Contains(NumeroTarjeta.ToUpper()) && tarj.Compania.ToUpper().Contains(TipoTarjeta.ToUpper())
                                              orderby tarj.Titular, tarj.Numero, tarj.Compania
                                              select tarj).ToList();
            
            return resultado;

        }

        protected void SetStatus(object sender, CommandEventArgs e)
        {
            try
            {
                Tarjeta.SetEstatusCard(e.CommandArgument.ToString());
                Bind();
            }
            catch (Exception)
            {
            }
        }
       

    }


}