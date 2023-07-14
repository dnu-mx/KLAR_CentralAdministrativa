using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace CentralAplicaciones
{
    public partial class Principal : System.Web.UI.Page
    {
        Ext.Net.Panel pnlEmpresa = new Ext.Net.Panel();
        Ext.Net.Panel pnlEmpleados = new Ext.Net.Panel();
        Ext.Net.Panel pnlCajero = new Ext.Net.Panel();
        Ext.Net.Panel pnlSettings = new Ext.Net.Panel();
        List<Aplicacion> aplicaciones = new List<Aplicacion>();
        AccordionLayout acc = new AccordionLayout();
        Usuario eluser = new Usuario();


        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LogHeader logHeader = new LogHeader();
            logHeader.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            logHeader.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            logHeader.User = this.User.Identity.Name;
            logHeader.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(logHeader);
            string redirect = string.Empty;

            try
            {
                log.Info("INICIA Principal Page_Load()");

                if (!IsPostBack)
                {
                    if (!IsPostBack && !X.IsAjaxRequest)
                    {
                        txtMinTimeOut.Value = Session.Timeout;
                    }

                    if (Context == null)
                    {
                        redirect = "../Account/login.aspx";
                        return;
                    }

                    if (Context.User == null)
                    {
                        redirect = "../Account/login.aspx";
                        return;
                    }

                    eluser = (Usuario)HttpContext.Current.Session.Contents["usuario"];

                    lblEmail.Text = eluser.Email;
                    lblPerfiles.Text = eluser.RolesToString();


                    if (!X.IsAjaxRequest)
                    {
                        acc.Animate = true;

                        CrearAcordeon(logHeader);
                    }
                }

                log.Info("TERMINA Principal Page_Load()");
            }
            catch (CAppException err)
            {
                log.Warn(err.Mensaje());
                redirect = "ErrorInicializarPagina.aspx";
            }
            catch (Exception ex)
            {
                log.WarnException(ex);
                redirect = "ErrorInicializarPagina.aspx";
            }
            finally
            {
                if (!string.IsNullOrEmpty(redirect))
                {
                    Response.Redirect(redirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        protected void CrearAcordeon(ILogHeader elLog)
        {
            List<Ext.Net.Panel> Paneles = new List<Ext.Net.Panel>();
            int i = 0;
            string fn = "if (node.attributes.href) { e.stopEvent();  loadPage(#{Pages}, node); }";

            //menus combinados by mauricio perez
            var menus= GeneraMenus(elLog);

            menus = this.MergeMenus(menus);

            foreach (Aplicacion unaAplciacion in menus)
            {
                //CREA EL ACORDEON
                Ext.Net.Panel pnlEmpresa = new Ext.Net.Panel();

                pnlEmpresa.ID = "ID_Uno" + String.Format("{0}", i++);
                pnlEmpresa.Title =  unaAplciacion.NombreAplicacion;
                pnlEmpresa.Border = false;
                pnlEmpresa.BodyStyle = "padding:6px;";
                pnlEmpresa.Icon = (Icon)unaAplciacion.Icono;
                pnlEmpresa.AutoScroll = true;
                Paneles.Add(pnlEmpresa);

                TreePanel tree = new TreePanel();

                tree.ID = "ID_dos" + String.Format("{0}", i++);
                tree.Icon = (Icon)unaAplciacion.Icono;
                tree.AutoScroll = false;
                tree.Lines = false;
                tree.RootVisible = false;
                tree.BodyBorder = false;
                tree.Lines = true;
                tree.Listeners.Click.Handler = fn;
                Ext.Net.TreeNode root = new Ext.Net.TreeNode("Menu");
                root.Expanded = true;
                tree.Root.Add(root);

                foreach (Pagina Menu in unaAplciacion.Menus.Values)
                {
                    Ext.Net.TreeNode unMenu = new Ext.Net.TreeNode(Menu.Nombre,(Icon) Menu.Icono);
                    unMenu.Href = Menu.URL;
                    root.Nodes.Add(unMenu);
                    unMenu.Expanded = true;

                    foreach (Pagina subMenu in Menu.Hijos.Values)
                    {
                        Ext.Net.TreeNode unSubMenu = new Ext.Net.TreeNode(subMenu.Nombre, (Icon)subMenu.Icono);
                        unSubMenu.Href = subMenu.URL;
                        unMenu.Nodes.Add(unSubMenu);
                        unSubMenu.Expanded = true;

                        foreach (Pagina subsubMenu in subMenu.Hijos.Values)
                        {
                            Ext.Net.TreeNode unsubsubMenu = new Ext.Net.TreeNode(subsubMenu.Nombre, (Icon)subsubMenu.Icono);
                            unsubsubMenu.Href = subsubMenu.URL;
                            unSubMenu.Nodes.Add(unsubsubMenu);
                            unsubsubMenu.Expanded = true;
                        }
                    }
                }
                pnlEmpresa.Add(tree);
                acc.Items.Add(pnlEmpresa);
            }

            PnlMenu.Items.Add(acc);
        }

        private List<Aplicacion> MergeMenus(List<Aplicacion> menus)
        {
            var groups = menus.GroupBy(m => new {m.NombreAplicacion,m.Icono});

            var nMenus= new List<Aplicacion>();

            foreach (var apps in groups)
            {
                if(nMenus.Count>1)
                foreach (var app in apps)
                { 
                    
                    if(app.Equals(apps.First()))
                        continue;
                    
                    foreach (var menu in app.Menus)
                    {
                        apps.First().Menus.Add(menu.Key,menu.Value);    

                    }

                    var val= apps.First().Menus.OrderBy(m => m.Value.OrdenDespliegue);

                    apps.First().Menus = val.ToDictionary(m => m.Key, m => m.Value);
                }

                nMenus.Add(apps.First());
            }

            return nMenus;
        }


        private List<Aplicacion> GeneraMenus(ILogHeader elLog)
        {
            LogPCI log = new LogPCI(elLog);

            try
            {
                List<Aplicacion> lasapps = new List<Aplicacion>();

                log.Info("INICIA DAOAplicacion.ObtieneMenusAplicaciones()");
                Dictionary<Guid, Aplicacion> apps = DAOAplicacion.ObtieneMenusAplicaciones(
                    (Usuario)HttpContext.Current.Session.Contents["usuario"], elLog);
                log.Info("TERMINA DAOAplicacion.ObtieneMenusAplicaciones()");

                lasapps = apps.Values.ToList();

                return lasapps;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }
    }
}