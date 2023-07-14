using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Ext.Net;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;
using System.Configuration;
using System.Web.Security;
using Interfases;

namespace Cajero
{
    public partial class Configuraciones : DALCentralAplicaciones.PaginaBaseCAPP
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

                    PreparaGripPropiedades();
                }
            }
            catch (Exception err)
            {
                DALCentralAplicaciones.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
            
        }

        protected void PreparaGripPropiedades()
        {
            try
            {
                //int e = 0;

                foreach (Propiedad unaProp in LNPropiedad.ObtieneParametros(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())))
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, unaProp.Valor);
                    GridProp.DisplayName = unaProp.ValueDescription;

                    Propiedades.AddProperty(GridProp);
                    
                }

            }
            catch (Exception)
            {
            }
        }



        //protected void CrearAcordeon()
        //{
        //    List<Ext.Net.Panel> Paneles = new List<Ext.Net.Panel>();
        //    int i = 0;
        //    string fn = "if (node.attributes.href) { e.stopEvent(); loadPage(#{Pages}, node); }";

        //    foreach (Aplicacion unaAplciacion in GeneraMenus())
        //    {
        //        //CREA EL ACORDEON
        //        Ext.Net.Panel pnlEmpresa = new Ext.Net.Panel();

        //        pnlEmpresa.ID = "ID_Uno" + String.Format("{0}", i++);
        //        pnlEmpresa.Title = unaAplciacion.NombreAplicacion;
        //        pnlEmpresa.Border = false;
        //        pnlEmpresa.BodyStyle = "padding:6px;";
        //        pnlEmpresa.Icon = (Icon)unaAplciacion.Icono;
        //        pnlEmpresa.AutoScroll = true;
        //        Paneles.Add(pnlEmpresa);

        //        Ext.Net.TreePanel tree = new Ext.Net.TreePanel();

        //        tree.ID = "ID_dos" + String.Format("{0}", i++);
        //        tree.Icon = (Icon)unaAplciacion.Icono;
        //        tree.AutoScroll = false;
        //        tree.Lines = false;
        //        tree.RootVisible = false;
        //        tree.BodyBorder = false;
        //        tree.Lines = true;
        //        tree.Listeners.Click.Handler = fn;
        //        Ext.Net.TreeNode root = new Ext.Net.TreeNode("Menu");
        //        root.Expanded = true;
        //        tree.Root.Add(root);

        //        //Ext.Net.TreeNode composerNode = new Ext.Net.TreeNode("Hola");
        //        // composerNode.Href = "Afiliaciones.aspx";
        //        //root.Nodes.Add(composerNode);

        //        foreach (Pagina Menu in unaAplciacion.Menus.Values)
        //        {
        //            Ext.Net.TreeNode unMenu = new Ext.Net.TreeNode(Menu.Nombre, (Icon)Menu.Icono);
        //            unMenu.Href = Menu.URL;
        //            root.Nodes.Add(unMenu);
        //            unMenu.Expanded = true;

        //            foreach (Pagina subMenu in Menu.Hijos.Values)
        //            {
        //                Ext.Net.TreeNode unSubMenu = new Ext.Net.TreeNode(subMenu.Nombre, (Icon)subMenu.Icono);
        //                unSubMenu.Href = subMenu.URL;
        //                unMenu.Nodes.Add(unSubMenu);
        //                unSubMenu.Expanded = true;

        //                foreach (Pagina subsubMenu in subMenu.Hijos.Values)
        //                {
        //                    Ext.Net.TreeNode unsubsubMenu = new Ext.Net.TreeNode(subsubMenu.Nombre, (Icon)subsubMenu.Icono);
        //                    unsubsubMenu.Href = subsubMenu.URL;
        //                    unSubMenu.Nodes.Add(unsubsubMenu);
        //                    unsubsubMenu.Expanded = true;
        //                }
        //            }
        //        }
        //        pnlEmpresa.Add(tree);
        //        acc.Items.Add(pnlEmpresa);
        //    }

        //    PnlMenu.Items.Add(acc);
        //}


        protected void Button1_Click(object sender, DirectEventArgs e)
        {
            try
            {
                List<Propiedad> losCambios = new List<Propiedad>();


                //Obtiene las propiedades que cambiaron
                foreach (PropertyGridParameter param in this.Propiedades.Source)
                {
                    if (param.IsChanged)
                    {
                        Propiedad unaProp = new Propiedad(param.Name, param.Value.ToString(), "", new Guid(), Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "");
                        losCambios.Add(unaProp);
                    }
                }

                //Guardar Valores
                LNPropiedad.ModificaParametros(losCambios, this.Usuario);

                ValoresInicial.InicializarContexto();


                X.Msg.Notify("Configuración", "Modificación de Configuración <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();


            }
            catch (Exception)
            {
                X.Msg.Notify("Configuración", "Modificación de Configuración <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }

        }

       


    }
}