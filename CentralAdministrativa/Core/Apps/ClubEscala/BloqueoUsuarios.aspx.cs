using Mensajeria;
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
using FreezeCode.IceCubes.Implement.Generic;
using FreezeCode.IceCubes;
using FreezeCode.IceCubes.Security.Encryption;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.ApplicationBlocks.Data;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;

namespace SitioInterno
{
    public partial class BloqueoUsuarios : DALCentralAplicaciones.PaginaBaseCAPP
    {
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

                Clientes misClientes = new Clientes();
                misClientes.Get();

                objPage.AllowPaging = true;

                List<Cliente> losClientes =  BuscarClientes(txtNombre.Text,txtApellido.Text,txtEmail.Text,misClientes);
                objPage.DataSource = losClientes;
                //Assigning the datasource to the 'objPage' object.
                //objPage.DataSource = misClientes;
         


                //Setting the Pagesize
                try
                {
                    objPage.PageSize = Int16.Parse((string)Configuracion.GetConfiguracion("Paginacion"));
                }
                catch (Exception)
                {
                    objPage.PageSize = 10;
                }

                if (losClientes.Count < objPage.PageSize)
                {
                    HttpContext.Current.Session.Add("CurrentPageUsuarios", 0);
                }


                //"CurrentPage" is public static variable to hold the current page index value declared in the global section.
                try
                {
                    if ((int)HttpContext.Current.Session.Contents["CurrentPageUsuarios"] < 0)
                    {
                        objPage.CurrentPageIndex = 0;
                    }
                    else
                    {
                        objPage.CurrentPageIndex = (int)HttpContext.Current.Session.Contents["CurrentPageUsuarios"];
                    }
                }
                catch (Exception)
                {
                    HttpContext.Current.Session.Add("CurrentPageUsuarios", 0);
                    objPage.CurrentPageIndex = 0;
                }


                if (losClientes.Count < objPage.PageSize)
                {
                    back.Visible = false;
                    next.Visible = false;
                    HttpContext.Current.Session.Add("CurrentPageUsuarios", 0);
                }
                else
                {
                    back.Visible = true;
                    next.Visible = true;
                }

                //Assigning Datasource to the DataList.
                RepeaterUsuarios.DataSource = objPage;
                //dlGallery.DataKeyField = "Image_ID";
                RepeaterUsuarios.DataBind();

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
            int pagina = (int)HttpContext.Current.Session.Contents["CurrentPageUsuarios"] - 1;
            HttpContext.Current.Session.Add("CurrentPageUsuarios", pagina);
            Bind();
        }

        protected void lbtnNext_Click(object sender, EventArgs e)
        {

            int pagina = (int)HttpContext.Current.Session.Contents["CurrentPageUsuarios"] + 1;
            HttpContext.Current.Session.Add("CurrentPageUsuarios", pagina);
            Bind();

        }

        protected List<Cliente> BuscarClientes(String nombre, String apellido, String email, List<Cliente> losClientes)
        {
            List<Cliente> resultado;


            resultado = (from cli in losClientes
                         where cli.Apellido1.ToUpper().Contains(apellido.ToUpper()) && cli.Nombre1.ToUpper().Contains(nombre.ToUpper()) && cli.Mail1.ToUpper().Contains(email.ToUpper())
                         orderby cli.Apellido1, cli.Nombre1, cli.Mail1
                         select cli).ToList();

            return resultado;

        }



        protected void SetStatus(object sender, CommandEventArgs e)
        {
            try
            {

                Cliente.SetEstatusCliente(e.CommandArgument.ToString());
                Bind();
            }
            catch (Exception)
            {
            }

        }
    }
}