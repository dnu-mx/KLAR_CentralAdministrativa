using Administracion.Componentes;
using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALCentralAplicaciones.Utilidades;
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
    public partial class NuevoTipoCuenta : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    initComonets();
                }
            }
            catch(Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        private void initComonets() 
        {

            /* Tipos de cuenta */
            CrearGridTiposCuenta();
            LlenarGridTiposCuenta();

            CrearDialogNuevoTipoCuenta();
        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los productos
         **************************************************************************************/
        private void CrearGridTiposCuenta()
        {
            GridTiposCuenta = TiposCuentaComponent.CrearGridPanelBase(GridTiposCuenta, false);
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
        private void CrearDialogNuevoTipoCuenta()
        {
            Dialog_NuevoTipoCuenta.Title = "Nuevo Tipo de Cuenta"; 
            Dialog_NuevoTipoCuenta.Hidden = true;
            Dialog_NuevoTipoCuenta.Modal = true;
            Dialog_NuevoTipoCuenta.Width = 500;
            Dialog_NuevoTipoCuenta.Height = 350;

            TxtID_TipoCuenta.Hidden = true;

            SelectCodTipoCuentaISO.FieldLabel = "Código ISO";
            SelectCodTipoCuentaISO.AnchorHorizontal = "100%";
            SelectCodTipoCuentaISO.AllowBlank = false;
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

        private void LlenarComboDivisas()
        {
            this.StoreDivisas.DataSource = DAOTipoCuenta.ListarDivisas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreDivisas.DataBind();
        }

        private void LlenarComboPeriodo()
        {
            this.StorePeriodos.DataSource = DAOGruposMA.ListaPeriodos(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StorePeriodos.DataBind();
        }

        private void LlenarComboCodTipoCuentaISO() 
        {
            this.StoreCodTipoCuentaISO.DataSource = DAOTipoCuenta.ListarCodTipoCuentaISO(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreCodTipoCuentaISO.DataBind();
        }

        protected void NuevoTipoCuenta_Event(object sender, EventArgs e)
        {
            LlenarComboPeriodo();
            LlenarComboDivisas();
            LlenarComboCodTipoCuentaISO();

            this.Dialog_NuevoTipoCuenta.Show();
        }

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

            DAOTipoCuenta.insertar(elTipocuenta,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.Dialog_NuevoTipoCuenta.Hidden = true;
            LlenarGridTiposCuenta();
        }

    }

}