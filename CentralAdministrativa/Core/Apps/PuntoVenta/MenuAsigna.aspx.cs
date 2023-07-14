using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TpvWeb
{
    public partial class MenuAsigna : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    initComponents();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        private void initComponents()
        {
            //Grid para TPVs
            GroupingSummaryColumn IdColectiva = new GroupingSummaryColumn();
            IdColectiva.DataIndex = "ID_Colectiva";
            IdColectiva.Hidden = true;
            this.grid_tpvs.ColumnModel.Columns.Add(IdColectiva);

            GroupingSummaryColumn ClaveColectiva = new GroupingSummaryColumn();
            ClaveColectiva.DataIndex = "ClaveColectiva";
            ClaveColectiva.Header = "Clave";
            this.grid_tpvs.ColumnModel.Columns.Add(ClaveColectiva);

            GroupingSummaryColumn NombreORazonSocial = new GroupingSummaryColumn();
            NombreORazonSocial.DataIndex = "NombreORazonSocial";
            NombreORazonSocial.Header = "Nombre o Razón Social";
            this.grid_tpvs.ColumnModel.Columns.Add(NombreORazonSocial);

            GroupingSummaryColumn NombreTPV = new GroupingSummaryColumn();
            NombreTPV.DataIndex = "NombreTPV";
            NombreTPV.Header = "Nombre de la TPV";
            this.grid_tpvs.ColumnModel.Columns.Add(NombreTPV);

            GroupingSummaryColumn ClaveMenuActual = new GroupingSummaryColumn();
            ClaveMenuActual.DataIndex = "ClaveMenu";
            ClaveMenuActual.Header = "Clave Menu Actual";
            ClaveMenuActual.Width = 120;
            this.grid_tpvs.ColumnModel.Columns.Add(ClaveMenuActual);

            this.grid_tpvs.AutoExpandColumn = "NombreORazonSocial";
            llenarGridTPVs();


            //Grid para Menús
            GroupingSummaryColumn IDMenu = new GroupingSummaryColumn();
            IDMenu.DataIndex = "ID_Menu";
            IDMenu.Hidden = true;
            this.grid_menus.ColumnModel.Columns.Add(IDMenu);

            GroupingSummaryColumn Clave = new GroupingSummaryColumn();
            Clave.DataIndex = "Clave";
            Clave.Header = "Clave";
            this.grid_menus.ColumnModel.Columns.Add(Clave);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Nombre del Menú";
            this.grid_menus.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn Version = new GroupingSummaryColumn();
            Version.DataIndex = "Version";
            Version.Header = "Versión";
            this.grid_menus.ColumnModel.Columns.Add(Version);

            this.grid_menus.AutoExpandColumn = "Descripcion";
            llenarGridMenu();
        }

        private void llenarGridMenu()
        {
            this.stMenus.DataSource = DAOMenu.ListaMenus(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.stMenus.DataBind();
        }

        private void llenarGridTPVs()
        {
            this.stTPVs.DataSource = DAOTPV.ListaTPVs(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.stTPVs.DataBind();
        }

        protected void stTPVs_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            llenarGridTPVs();
        }

        protected void stMenus_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            llenarGridMenu();
        }

        protected void Click_AsignarMenuATPV(Object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["ValuesMenu"];
            IDictionary<string, string>[] menusSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (menusSeleccionados == null || menusSeleccionados.Length < 1)
            {
                return;
            }

            json = e.ExtraParams["ValuesTPV"];
            IDictionary<string, string>[] tpvsSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (tpvsSeleccionados == null || tpvsSeleccionados.Length < 1)
            {
                return;
            }

            string strIDMenu;
            menusSeleccionados[0].TryGetValue("ID_Menu", out strIDMenu);
            Int32 ID_Menu = Convert.ToInt32(strIDMenu);

            foreach (IDictionary<string, string> row in tpvsSeleccionados)
            {
                string strIDTPV;
                row.TryGetValue("ID_Colectiva", out strIDTPV);

                DAOTPV.asignarMenu(ID_Menu, Convert.ToInt32(strIDTPV),
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                    );
            }

            this.grid_tpvs.DeleteSelected();
            llenarGridTPVs();
        }

        protected void Click_DesasignarMenuATPV(Object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["Values"];
            IDictionary<string, string>[] tpvsSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (tpvsSeleccionados == null || tpvsSeleccionados.Length < 1)
            {
                return;
            }

            foreach (IDictionary<string, string> row in tpvsSeleccionados)
            {
                string strIDTPV;
                row.TryGetValue("ID_Colectiva", out strIDTPV);

                DAOTPV.desasignarMenu(Convert.ToInt32(strIDTPV),
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                    );
            }

            this.grid_tpvs.DeleteSelected();
            llenarGridTPVs();
        }
    }
}