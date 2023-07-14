using Administracion.Componentes;
using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.Utilidades;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Administracion
{
    public partial class EditarTipoCuenta : DALCentralAplicaciones.PaginaBaseCAPP
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
            /* Tipos de cuenta */
            CrearGridTiposCuenta();
            LlenarGridTiposCuenta();

            CrearDialogNuevoEditarTipoCuenta();
        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los productos
         **************************************************************************************/
        private void CrearGridTiposCuenta()
        {
            GridTiposCuenta = TiposCuentaComponent.CrearGridPanelBase(GridTiposCuenta, true);

            GridTiposCuenta.DirectEvents.RowClick.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values",
                    "Ext.encode(#{GridProductos}.getRowsValues({selectedOnly:true}))",
                    ParameterMode.Raw
                ));

        }

        /***************************************************************************************
         * Llena el gris de productos con la información de la base de datos
         **************************************************************************************/
        private void LlenarGridTiposCuenta()
        {
            this.StoreTiposCuenta.DataSource = DAOTipoCuenta.ListarTiposCuenta(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreTiposCuenta.DataBind();
        }

        /***************************************************************************************
         * Controla la acción de atualizar los datos del gri productos
         **************************************************************************************/
        protected void StoreTiposCuenta_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridTiposCuenta();
        }

        /***************************************************************************************
         * Crea el Dialog para el registro de un nuevo tipo de cuenta
         **************************************************************************************/
        private void CrearDialogNuevoEditarTipoCuenta()
        {
            DialogNuevoEditarTipoCueta.Title = "Nuevo Tipo de Cuenta";
            DialogNuevoEditarTipoCueta.Hidden = true;
            DialogNuevoEditarTipoCueta.Modal = true;
            DialogNuevoEditarTipoCueta.Width = 500;
            DialogNuevoEditarTipoCueta.Height = 350;

            FormNuevoEditarTipoCuenta.Padding = 10;
            FormNuevoEditarTipoCuenta.MonitorValid = true;

            TxtID_TipoCuenta.Hidden = true;

            SelectCodTipoCuentaISO.FieldLabel = "Código ISO";
            SelectCodTipoCuentaISO.AnchorHorizontal = "100%";
            SelectCodTipoCuentaISO.AllowBlank = true;
            SelectCodTipoCuentaISO.DisplayField = "Descripcion";
            SelectCodTipoCuentaISO.ValueField = "CodTipoCuentaISO";
            SelectCodTipoCuentaISO.EmptyText = "Selecciona un código ISO";

            TxtClaveTipoCuenta.FieldLabel = "Clave";
            TxtClaveTipoCuenta.AnchorHorizontal = "100%";
            TxtClaveTipoCuenta.AllowBlank = false;
            TxtClaveTipoCuenta.MaxLength = 10;

            TxtDescripcion.FieldLabel = "Descripción";
            TxtDescripcion.AnchorHorizontal = "100%";
            TxtDescripcion.AllowBlank = false;
            TxtDescripcion.MaxLength = 50;

            SelectGeneraDetalle.FieldLabel = "Genera Detalle";
            SelectGeneraDetalle.AnchorHorizontal = "100%";
            SelectGeneraDetalle.AllowBlank = false;

            SelectGeneraCorte.FieldLabel = "Genera Corte";
            SelectGeneraCorte.AnchorHorizontal = "100%";
            SelectGeneraCorte.AllowBlank = false;

            SelectDivisa.FieldLabel = "Divisa";
            SelectDivisa.AnchorHorizontal = "100%";
            SelectDivisa.AllowBlank = false;

            SelectPeriodo.FieldLabel = "Periodo";
            SelectPeriodo.AnchorHorizontal = "100%";
            SelectPeriodo.AllowBlank = false;

            TxtBreveDescripcion.FieldLabel = "Breve Descripción";
            TxtBreveDescripcion.AnchorHorizontal = "100%";
            TxtBreveDescripcion.AllowBlank = false;
            TxtBreveDescripcion.MaxLength = 200;

            SelectEditarSaldoGrid.FieldLabel = "Editar Saldo";
            SelectEditarSaldoGrid.AnchorHorizontal = "100%";
            SelectEditarSaldoGrid.AllowBlank = false;

            SelectInteractuaCajero.FieldLabel = "Interactua Cajero";
            SelectInteractuaCajero.AnchorHorizontal = "100%";
            SelectInteractuaCajero.AllowBlank = false;

            this.SelectGeneraDetalle.EmptyText = "Selecciona opción.";
            this.SelectGeneraDetalle.Items.Add(new Ext.Net.ListItem("SI", "true"));
            this.SelectGeneraDetalle.Items.Add(new Ext.Net.ListItem("NO", "false"));

            this.SelectGeneraCorte.EmptyText = "Selecciona Opción.";
            this.SelectGeneraCorte.Items.Add(new Ext.Net.ListItem("SI", "true"));
            this.SelectGeneraCorte.Items.Add(new Ext.Net.ListItem("NO", "false"));

            this.SelectEditarSaldoGrid.EmptyText = "Selecciona Opción.";
            this.SelectEditarSaldoGrid.Items.Add(new Ext.Net.ListItem("SI", "true"));
            this.SelectEditarSaldoGrid.Items.Add(new Ext.Net.ListItem("NO", "false"));

            this.SelectInteractuaCajero.EmptyText = "Selecciona Opción";
            this.SelectInteractuaCajero.Items.Add(new Ext.Net.ListItem("SI", "true"));
            this.SelectInteractuaCajero.Items.Add(new Ext.Net.ListItem("NO", "false"));

            this.SelectDivisa.EmptyText = "Selecciona Divisa.";
            this.SelectDivisa.DisplayField = "Descripcion";
            this.SelectDivisa.ValueField = "ID_Divisa";

            this.SelectPeriodo.EmptyText = "Selecciona Periodo.";
            this.SelectPeriodo.DisplayField = "Descripcion";
            this.SelectPeriodo.ValueField = "ID_Periodo";
        }

        /***************************************************************************************
         * Obtiene la lista de divisas para el combobox
         **************************************************************************************/
        private void LlenarComboDivisas()
        {
            this.StoreDivisas.DataSource = DAOTipoCuenta.ListarDivisas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreDivisas.DataBind();
        }

        /***************************************************************************************
         * Obtiene la lista de periodos para el combobox
         **************************************************************************************/
        private void LlenarComboPeriodo()
        {
            this.StorePeriodos.DataSource = DAOGruposMA.ListaPeriodos(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StorePeriodos.DataBind();
        }

        /***************************************************************************************
         * Obtiene la lista de Códigod de tipocuenta iso
         **************************************************************************************/
        private void LlenarComboCodTipoCuentaISO()
        {
            this.StoreCodTipoCuentaISO.DataSource = DAOTipoCuenta.ListarCodTipoCuentaISO(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreCodTipoCuentaISO.DataBind();
        }

        /***************************************************************************************
         * Configura y muestra el dialogo para la edición de un tipo de cuenta
         **************************************************************************************/
        protected void EditarTipoCuenta_Click(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];
            if (!CommandName.Equals(TiposCuentaComponent.COMMAND_NAME))
            {
                return;
            }

            LlenarComboPeriodo();
            LlenarComboDivisas();
            LlenarComboCodTipoCuentaISO();

            TxtID_TipoCuenta.Text = e.ExtraParams["ID_TipoCuenta"];
            IDictionary<string, string> tipoCuentaSeleccionada = Util.getDirectEventArgsValues(e);
            if (tipoCuentaSeleccionada == null) 
            {
                return;
            }

            foreach(KeyValuePair<string, string> column in tipoCuentaSeleccionada)
            {
                switch (column.Key)
                {
                    case "CodTipoCuentaISO":
                        SelectCodTipoCuentaISO.SelectedItem.Value = column.Value;
                        break;

                    case "ClaveTipoCuenta":
                        TxtClaveTipoCuenta.Text = column.Value;
                        break;

                    case "Descripcion":
                        TxtDescripcion.Text = column.Value;
                        break;

                    case "GeneraDetalle":
                        SelectGeneraDetalle.SelectedItem.Value = column.Value;
                        break;

                    case "GeneraCorte":
                        SelectGeneraCorte.SelectedItem.Value = column.Value;
                        break;

                    case "ID_Divisa":
                        SelectDivisa.SelectedItem.Value = column.Value;
                        break;

                    case "ID_Periodo":
                        SelectPeriodo.SelectedItem.Value = column.Value;
                        break;

                    case "BreveDescripcion":
                        TxtBreveDescripcion.Text = column.Value;
                        break;

                    case "EditarSaldoGrid":
                        SelectEditarSaldoGrid.SelectedItem.Value = column.Value;
                        break;

                    case "InteractuaCajero":
                        SelectInteractuaCajero.SelectedItem.Value = column.Value;
                        break;

                    case "ID_NaturalezaCuenta":
                        break;

                    default:
                        break;
                }
            }

            this.DialogNuevoEditarTipoCueta.Show();
        }

        /***************************************************************************************
         * Guarda los cambios echos a un tipo de cuenta
         **************************************************************************************/
        protected void GuardarTipoCuenta_Event(object sender, EventArgs e)
        {
            TipoCuenta elTipocuenta = new TipoCuenta();
            elTipocuenta.ID_TipoCuenta = String.IsNullOrEmpty(TxtID_TipoCuenta.Text) ? 0 : Convert.ToInt32(TxtID_TipoCuenta.Text);
            elTipocuenta.CodigoTipoCuentaISO = SelectCodTipoCuentaISO.SelectedItem.Value;
            elTipocuenta.ClaveTipoCuenta = TxtClaveTipoCuenta.Text;
            elTipocuenta.Descripcion = TxtDescripcion.Text;
            elTipocuenta.GeneraDetalle = Convert.ToBoolean(SelectGeneraDetalle.SelectedItem.Value);
            elTipocuenta.GeneraCorte = Convert.ToBoolean(SelectGeneraCorte.SelectedItem.Value);
            elTipocuenta.ID_Divisa = Convert.ToInt32(SelectDivisa.SelectedItem.Value);
            elTipocuenta.ID_Periodo = Convert.ToInt32(SelectPeriodo.SelectedItem.Value);
            elTipocuenta.BreveDescripcion = TxtBreveDescripcion.Text;
            elTipocuenta.EditarSaldoGrid = Convert.ToBoolean(SelectEditarSaldoGrid.SelectedItem.Value);
            elTipocuenta.InteractuaCajero = Convert.ToBoolean(SelectInteractuaCajero.SelectedItem.Value);
            elTipocuenta.ID_NaturalezaCuenta = 0;

            DAOTipoCuenta.actualizar(elTipocuenta,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.DialogNuevoEditarTipoCueta.Hidden = true;
            LlenarGridTiposCuenta();
        }
    }
}