using DALCentralAplicaciones.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALPuntoVentaWeb.BaseDatos;
using System.Configuration;
using Ext.Net;
using DALPuntoVentaWeb.Entidades;
using System.Data;

namespace TpvWeb
{
    public partial class MenuAdmin : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    llenarMenuGrid();
                    llenarPromocionGrid();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        /**********************************************************************************************
         * 
         *********************************************************************************************/
        private void llenarMenuGrid() {
            this.stMenus.DataSource = DAOMenu.ListaMenus(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.stMenus.DataBind();
        }

        /**********************************************************************************************
         * 
         *********************************************************************************************/
        private void llenarPromocionGrid()
        {
            this.stPromociones.DataSource = DAOPromociones.ListaPromociones(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.stPromociones.DataBind();
        }

        /**********************************************************************************************
         *  Crea y muestra el formulario para registro del nuevo menú
         *********************************************************************************************/
        protected void Click_FormularioNuevoMenu(object sender, DirectEventArgs e)
        {
            this.dialog_Menu.Title = "Nuevo Menú";
            this.btnActualizar.Hidden = true;
            this.btnGuardar.Hidden = false;
            this.FormMenuPanel.Reset();
            this.FormMenuPanel.Expand(true);
            this.dialog_Menu.Show();
        }

        /**********************************************************************************************
         *  Guarda un nuevo menú en la base de datos
         *********************************************************************************************/
        protected void Click_GuardarNuevoMenu(object sender, DirectEventArgs e) 
        {
            if (String.IsNullOrEmpty(this.txtClave.Text) || 
                String.IsNullOrEmpty(this.txtDescripcion.Text) ||
                String.IsNullOrEmpty(this.txtVersion.Text))
                return;

            MenuTPV nuevoMenu = new MenuTPV();
            nuevoMenu.Clave = this.txtClave.Text;
            nuevoMenu.Descripcion = this.txtDescripcion.Text;
            nuevoMenu.Version = Convert.ToInt32(this.txtVersion.Text);

            DAOMenu.insertar(nuevoMenu,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.dialog_Menu.Hidden = true;
            this.FormMenuPanel.Reset();
            llenarMenuGrid();
        }

        /**********************************************************************************************
         *  Muestra los datos en el panel de edición
         *********************************************************************************************/
        protected void Click_VerDatosMenu(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["Values"];
            VerDatosMenu(json);

            if (String.IsNullOrEmpty(this.txtIDMenu.Text))
            {
                this.panel_nodos_menu.Title = "Opciones del Menú:";
                return;
            }

            this.panel_nodos_menu.Title = String.Format("Opciones del Menú: \"{0}\"", this.txtDescripcion.Text);
            llenarPromocionMenuGrid(Convert.ToInt32(this.txtIDMenu.Text));
        }

        private void VerDatosMenu(string json)
        {
            IDictionary<string, string>[] menusSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (menusSeleccionados == null || menusSeleccionados.Length < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in menusSeleccionados[0])
            {
                switch(column.Key) {
                    case "ID_Menu": txtIDMenu.Text = column.Value; break;
                    case "Clave": txtClave.Text = column.Value; break;
                    case "Descripcion": txtDescripcion.Text = column.Value; break;
                    case "Version": txtVersion.Text = column.Value; break;
                    default:
                        break;
                }
            }

            this.dialog_Menu.Title = String.Format("Menú: \"{0}\"", this.txtDescripcion.Text);
            this.btnActualizar.Hidden = false;
            this.btnGuardar.Hidden = true;
            this.FormMenuPanel.Expand(true);
        }

        /**********************************************************************************************
         *  Realia la actualización de un menú
         *********************************************************************************************/
        protected void Click_ActualizarMenu(object sender, DirectEventArgs e) 
        {
            MenuTPV menuActualizado = new MenuTPV();
            menuActualizado.ID_Menu = Convert.ToInt32(txtIDMenu.Text);
            menuActualizado.Clave = txtClave.Text;
            menuActualizado.Descripcion = txtDescripcion.Text;
            menuActualizado.Version = Convert.ToInt32(txtVersion.Text);

            DAOMenu.actualizar(menuActualizado,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.dialog_Menu.Hidden = true;
            this.FormMenuPanel.Reset();
            llenarMenuGrid();
        }

        /**********************************************************************************************
         *  Realiza la eliminación de un menú
         *********************************************************************************************/
        protected void Click_ElimiarMenu(object sender, DirectEventArgs e) 
        {
            string CommandName = e.ExtraParams["CommandName"];

            switch (CommandName)
            { 
                case "BorrarMenu":
                    EliminarMenu(Convert.ToInt32(e.ExtraParams["ID_Menu"]));
                    llenarMenuGrid();
                    this.panel_nodos_menu.Title = "Opciones del Menú:";
                    break;

                case "EditarMenu":
                    VerDatosMenu(String.Format("[{0}]", e.ExtraParams["Values"]));
                    this.dialog_Menu.Show();
                    break;
            }
        }

        private void EliminarMenu(Int32 ID_Menu) 
        {
            MenuTPV menuAEliminar = new MenuTPV();
            menuAEliminar.ID_Menu = ID_Menu;

            DAOMenu.eliminar(menuAEliminar,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );
        }

        /**********************************************************************************************
         *  Crea y muestra el formulario para registro de una nueva promoción
         *********************************************************************************************/
        protected void Click_FormularioNuevaPromocion(object sender, DirectEventArgs e)
        {
            this.dialog_promocion.Title = "Nueva Promoción";
            this.btnActualizarPromocion.Hidden = true;
            this.btnGuardarPromocion.Hidden = false;
            this.FormPromociones.Reset();
            this.FormPromociones.Expand(true);
            this.dialog_promocion.Show();
        }

        /**********************************************************************************************
         *  Guarda un nuevo menú en la base de datos
         *********************************************************************************************/
        protected void Click_GuardarNuevaPromocion(object sender, DirectEventArgs e)
        {

            if (String.IsNullOrEmpty(this.txtClavePromocion.Text) ||
                String.IsNullOrEmpty(txtDescripcionPromocion.Text) ||
                String.IsNullOrEmpty(txtMesesPromocion.Text) ||
                String.IsNullOrEmpty(txtEtiquePromocion.Text) ||
                String.IsNullOrEmpty(txtPrimerPagoPromocion.SelectedItem.Value))
                return;

            Promocion nuevaPromocion = new Promocion();
            nuevaPromocion.Clave = this.txtClavePromocion.Text;
            nuevaPromocion.Descripcion = txtDescripcionPromocion.Text;
            nuevaPromocion.Meses = Convert.ToInt32(txtMesesPromocion.Text);
            nuevaPromocion.Etiqueta = txtEtiquePromocion.Text;
            nuevaPromocion.PrimerPago = Convert.ToInt32(txtPrimerPagoPromocion.SelectedItem.Value);

            DAOPromociones.insertar(nuevaPromocion,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.dialog_promocion.Hidden = true;
            this.FormPromociones.Reset();
            llenarPromocionGrid();
        }

        /**********************************************************************************************
         *  Muestra los datos en el panel de edición
         *********************************************************************************************/
        private void VerDatosPromocion(string json)
        {
            IDictionary<string, string>[] promocionesSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (promocionesSeleccionados == null || promocionesSeleccionados.Length < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in promocionesSeleccionados[0])
            {
                switch (column.Key)
                {
                    case "ID_Promocion": txtIDPromocion.Text = column.Value; break;
                    case "Clave": txtClavePromocion.Text = column.Value; break;
                    case "Descripcion": txtDescripcionPromocion.Text = column.Value; break;
                    case "Meses": txtMesesPromocion.Text = column.Value; break;
                    case "Etiqueta": txtEtiquePromocion.Text = column.Value; break;
                    case "PrimerPago": txtPrimerPagoPromocion.SelectedItem.Value = column.Value; break;
                    default:
                        break;
                }
            }

            this.FormPromociones.Title = String.Format("Promoción: \"{0}\"", this.txtDescripcionPromocion.Text);
            this.btnActualizarPromocion.Hidden = false;
            this.btnGuardarPromocion.Hidden = true;
            this.FormPromociones.Expand(true);
        }

        /**********************************************************************************************
         *  Realia la actualización de un menú
         *********************************************************************************************/
        protected void Click_ActualizarPromocion(object sender, DirectEventArgs e)
        {
            Promocion promocionActualizada = new Promocion();
            promocionActualizada.ID_Promocion = Convert.ToInt32(txtIDPromocion.Text);
            promocionActualizada.Clave = txtClavePromocion.Text;
            promocionActualizada.Descripcion = txtDescripcionPromocion.Text;
            promocionActualizada.Meses = Convert.ToInt32(txtMesesPromocion.Text);
            promocionActualizada.Etiqueta = txtEtiquePromocion.Text;
            promocionActualizada.PrimerPago = Convert.ToInt32(txtPrimerPagoPromocion.SelectedItem.Value);

            DAOPromociones.actualizar(promocionActualizada,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.dialog_promocion.Hidden = true;
            this.FormPromociones.Reset();
            llenarPromocionGrid();
        }

        /**********************************************************************************************
         *  Realiza la eliminación de un menú
         *********************************************************************************************/
        protected void Click_ElimiarPromocion(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];

            switch (CommandName)
            { 
                case "BorrarPromocion":
                    EliminarPromocion(Convert.ToInt32(e.ExtraParams["ID_Promocion"]));
                    llenarPromocionGrid();
                    break;

                case "EditarPromocion":
                    VerDatosPromocion(String.Format("[{0}]", e.ExtraParams["Values"]));
                    this.dialog_promocion.Show();
                    break;
            }
        }

        private void EliminarPromocion(Int32 ID_Promocion)
        {
            Promocion promocionAEliminar = new Promocion();
            promocionAEliminar.ID_Promocion = ID_Promocion;

            DAOPromociones.eliminar(promocionAEliminar,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );
        }

        /**********************************************************************************************
         * 
         *********************************************************************************************/
        private void llenarPromocionMenuGrid(Int32 ID_Menu)
        {
            this.stPromocionMenu.DataSource = DAOMenu.ListaPromocionesMenus(ID_Menu,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.stPromocionMenu.DataBind();
        }

        /**********************************************************************************************
         * 
         *********************************************************************************************/
        protected void Click_AsignarPromocionAMenu(Object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["ValuesPromociones"];
            IDictionary<string, string>[] promocionesSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (promocionesSeleccionados == null || promocionesSeleccionados.Length < 1)
            {
                return;
            }

            json = e.ExtraParams["ValuesMenus"];
            IDictionary<string, string>[] menusSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (menusSeleccionados == null || menusSeleccionados.Length < 1)
            {
                return;
            }

            string strIDMenu;
            menusSeleccionados[0].TryGetValue("ID_Menu", out strIDMenu);
            Int32 ID_Menu = Convert.ToInt32(strIDMenu);

            foreach (IDictionary<string, string> row in promocionesSeleccionados)
            {
                string strIDPromocion;
                row.TryGetValue("ID_Promocion", out strIDPromocion);

                DAOMenu.asignarPromocion(ID_Menu, Convert.ToInt32(strIDPromocion),
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                    );
            }

            this.GridPanelNodos.DeleteSelected();
            llenarPromocionGrid();
            llenarPromocionMenuGrid(ID_Menu);
        }

        /**********************************************************************************************
         *  Identifica el comando invocado y lo ejecuta
         *********************************************************************************************/
        protected void Click_EjecutarComando(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];

            switch (CommandName)
            {
                case "DesasignarPromocion":
                    DesasignarPromocion(
                            Convert.ToInt32(e.ExtraParams["ID_Menu"]),
                            Convert.ToInt32(e.ExtraParams["ID_Promocion"])
                        );
                    llenarPromocionMenuGrid(Convert.ToInt32(e.ExtraParams["ID_Menu"]));
                    break;

                case "SubirPromocion":
                    SubirBjarPromocion(
                            Convert.ToInt32(e.ExtraParams["ID_Menu"]),
                            Convert.ToInt32(e.ExtraParams["ID_Promocion"]),
                            1
                        );
                    llenarPromocionMenuGrid(Convert.ToInt32(e.ExtraParams["ID_Menu"]));
                    break;

                case "BajarPromocion":
                    SubirBjarPromocion(
                            Convert.ToInt32(e.ExtraParams["ID_Menu"]),
                            Convert.ToInt32(e.ExtraParams["ID_Promocion"]),
                            2
                        );
                    llenarPromocionMenuGrid(Convert.ToInt32(e.ExtraParams["ID_Menu"]));
                    break;
            }
        }

        private void DesasignarPromocion(Int32 ID_Menu, Int32 ID_Promocion) 
        {
            DAOMenu.desasignarPromocion(ID_Menu, ID_Promocion,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                    );
        }

        private void SubirBjarPromocion(Int32 ID_Menu, Int32 ID_Promocion, Int32 Accion)
        {
            DAOMenu.subirBajarPromocion(ID_Menu, ID_Promocion, Accion,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                    );
        }

        protected void Click_CancelarDialog(object sender, DirectEventArgs e)
        {
            this.dialog_Menu.Hidden = true;
            this.dialog_promocion.Hidden = true;

            this.FormMenuPanel.Reset();
            this.FormPromociones.Reset();
        }

    }
}