using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using Ext.Net;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;

namespace Usuarios
{
    public partial class MenuEditor : System.Web.UI.Page
    {
        #region Variables privadas

        //LOG HEADER MEDU EDITOR
        private LogHeader LH_MenuEditor = new LogHeader();

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_MenuEditor.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_MenuEditor.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_MenuEditor.User = this.User.Identity.Name;
            LH_MenuEditor.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_MenuEditor);

            try
            {
                log.Info("INICIA MenuEditor Page_Load()");

                if (!IsPostBack)
                {
                    Guid IdRol =  new Guid((String)Request["guid"]);
                    CrearAcordeon(IdRol);
                }

                log.Info("TERMINA MenuEditor Page_Load()");
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                X.Msg.Alert("Menú Rol", caEx.Mensaje()).Show();
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                X.Msg.Alert("Menú Rol", "Ocurrió un error al establecer el menú del rol.").Show();
            }
        }

        protected void CrearAcordeon(Guid RoleId)
        {
            LogPCI pCI = new LogPCI(LH_MenuEditor);
            pCI.Info("CrearAcordeon()");

            List<Aplicacion> aplicaciones = new List<Aplicacion>();
            AccordionLayout acc = new AccordionLayout();
            List<Ext.Net.Panel> Paneles = new List<Ext.Net.Panel>();

            int i = 0;
            string fn = "ActualizaNodo(#{Pages}, node, '" + RoleId.ToString() + "'); ";

            try
            {
                foreach (Aplicacion unaAplicacion in GeneraMenus(RoleId))
                {
                    //CREA EL ACORDEON
                    Ext.Net.Panel pnlEmpresa = new Ext.Net.Panel();

                    pnlEmpresa.ID = "ID_Uno" + String.Format("{0}", i++);
                    pnlEmpresa.Title = unaAplicacion.NombreAplicacion;
                    pnlEmpresa.Border = false;
                    pnlEmpresa.BodyStyle = "padding:6px;";
                    pnlEmpresa.Icon = (Icon)unaAplicacion.Icono;
                    pnlEmpresa.AutoScroll = true;
                    Paneles.Add(pnlEmpresa);

                    TreePanel tree = new TreePanel();

                    tree.ID = "ID_dos" + String.Format("{0}", i++);
                    tree.Icon = (Icon)unaAplicacion.Icono;
                    tree.AutoScroll = false;
                    tree.Lines = false;
                    tree.RootVisible = false;
                    tree.BodyBorder = false;
                    tree.Lines = true;
                    Ext.Net.TreeNode root = new Ext.Net.TreeNode("Menu");
                    root.Expanded = true;
                    tree.Root.Add(root);

                    foreach (Pagina Menu in unaAplicacion.Menus.Values)
                    {
                        Ext.Net.TreeNode unMenu = new Ext.Net.TreeNode(Menu.Nombre, (Icon)Menu.Icono);
                        unMenu.Listeners.CheckChange.Handler = fn;
                        unMenu.NodeID = Menu.MenuId.ToString();
                        unMenu.Checked = Menu.RolID.Equals("00000000-0000-0000-0000-000000000000") ? ThreeStateBool.False : ThreeStateBool.True;
                        root.Nodes.Add(unMenu);
                        unMenu.Expanded = true;

                        foreach (Pagina subMenu in Menu.Hijos.Values)
                        {
                            Ext.Net.TreeNode unSubMenu = new Ext.Net.TreeNode(subMenu.Nombre, (Icon)subMenu.Icono);
                            unSubMenu.Listeners.CheckChange.Handler = fn;
                            unSubMenu.NodeID = subMenu.MenuId.ToString();
                            unSubMenu.Checked = subMenu.RolID.Equals("00000000-0000-0000-0000-000000000000") ? ThreeStateBool.False : ThreeStateBool.True;
                            unMenu.Nodes.Add(unSubMenu);
                            unSubMenu.Expanded = true;

                            foreach (Pagina subsubMenu in subMenu.Hijos.Values)
                            {
                                Ext.Net.TreeNode unsubsubMenu = new Ext.Net.TreeNode(subsubMenu.Nombre, (Icon)subsubMenu.Icono);
                                unsubsubMenu.Listeners.CheckChange.Handler = fn;
                                unsubsubMenu.NodeID = subsubMenu.MenuId.ToString();
                                unsubsubMenu.Checked = subsubMenu.RolID.Equals("00000000-0000-0000-0000-000000000000") ? ThreeStateBool.False : ThreeStateBool.True;
                                unSubMenu.Nodes.Add(unsubsubMenu);
                                unsubsubMenu.Expanded = true;
                            }
                        }
                    }
                    pnlEmpresa.Add(tree);
                    acc.Items.Add(pnlEmpresa);
                }

                PnlMenu2.Items.Add(acc);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }


        private List<Aplicacion> GeneraMenus(Guid RoleId)
        {
            LogPCI pCI = new LogPCI(LH_MenuEditor);

            try
            {
                List<Aplicacion> lasapps = new List<Aplicacion>();

                pCI.Info("INICIA ObtieneMenusAplicaciones2()");
                Dictionary<Guid, Aplicacion> apps = DAOAplicacion.ObtieneMenusAplicaciones2(RoleId, LH_MenuEditor);
                pCI.Info("TERMINA ObtieneMenusAplicaciones2()");

                lasapps = apps.Values.ToList();

                return lasapps;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }


        [DirectMethod(Namespace = "Usuarios")]
        public void ActualizarMenu(string  ID_Nodo, string  ID_Rol, bool cheked)
        {
            LogPCI pCI = new LogPCI(LH_MenuEditor);

            try
            {
                pCI.Info("INICIA AsignarRolAMenu()");
                LNRol.AsignarRolAMenu(Guid.Parse(ID_Rol), Guid.Parse(ID_Nodo), cheked, LH_MenuEditor);
                pCI.Info("TERMINA AsignarRolAMenu()");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualizar Menú", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualizar Menú", "Ocurrió un error al actualizar el menú al rol.").Show();
            }
        }
    }
}