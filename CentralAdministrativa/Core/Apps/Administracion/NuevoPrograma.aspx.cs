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
    public partial class NuevoPrograma : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try 
            { 
                if( !this.IsPostBack ) {
                    initComponents();
                }
            }
            catch(Exception err) {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        private void initComponents()
        {
            /*programas*/
            CrearGridProgramas();
            LlenarGridProgramas();

            CrearFormNuevoPrograma();
        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los programas
         **************************************************************************************/
        private void CrearGridProgramas()
        {
            GridProgramas = ProgramaComponente.CrearGridPanelBase(GridProgramas, false);
        }

        /***********************************************************************************
         * Realiza la creación del formulario para registro de nuevos programas
         **********************************************************************************/
        private void CrearFormNuevoPrograma()
        {
            StoreVigencias = ProgramaComponente.CrearStoreVigencias(StoreVigencias);
            StoreColectivas = ProgramaComponente.CrearStoreColectivas(StoreColectivas);
            WNuevoPrograma.Title = "Nuevo Programa";
            WNuevoPrograma.Hidden = true;
            WNuevoPrograma.Modal = true;
            WNuevoPrograma.Height = 200;
            WNuevoPrograma.Width = 400;

            FNuevoPrograma.Padding = 10;
            FNuevoPrograma.MonitorValid = true;

            TxtProgramaClave.FieldLabel = "Clave";
            TxtProgramaClave.MaxLength = 5;
            TxtProgramaClave.AllowBlank = false;
            TxtProgramaClave.AnchorHorizontal = "100%";

            TxtProgramaDescripcion.FieldLabel = "Descripción";
            TxtProgramaDescripcion.MaxLength = 50;
            TxtProgramaDescripcion.AllowBlank = false;
            TxtProgramaDescripcion.AnchorHorizontal = "100%";

            SProgramaColectivaEmisor.FieldLabel = "Emisor";
            SProgramaColectivaEmisor.AnchorHorizontal = "100%";
            SProgramaColectivaEmisor.Store.Add(StoreColectivas);
            SProgramaColectivaEmisor.DisplayField = "Descripcion";
            SProgramaColectivaEmisor.ValueField = "ID_Colectiva";
            SProgramaColectivaEmisor.EmptyText = "Selecciona una Colectiva...";
            SProgramaColectivaEmisor.AllowBlank = false;

            SProgramaVigencia.FieldLabel = "Vigencia";
            SProgramaVigencia.AnchorHorizontal = "100%";
            SProgramaVigencia.Store.Add(StoreVigencias);
            SProgramaVigencia.DisplayField = "Descripcion";
            SProgramaVigencia.ValueField = "ID_Vigencia";
            SProgramaVigencia.EmptyText = "Selecciona una Vigencia...";
            SProgramaVigencia.AllowBlank = false;
        }

        /***************************************************************************************
         * Llena el gris de productos con la información de la base de datos
         **************************************************************************************/
        private void LlenarGridProgramas()
        {
            StoreProgramas = ProgramaComponente.CrearStore(StoreProgramas);
            StoreProgramas.DataSource = DAOProgramas.ListarGrupoCuentas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            StoreProgramas.DataBind();
        }

        /***********************************************************************************
         * Actualiza el panel con los datos de la base de datos
         **********************************************************************************/
        protected void StoreProgramas_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridProgramas();
        }

        /***********************************************************************************
         * Llena el combo de vigencias del formulario para nuevos programas
         **********************************************************************************/
        private void LlenarComboVigencias()
        {
            StoreVigencias = ProgramaComponente.CrearStoreVigencias(StoreVigencias);
            StoreVigencias.DataSource = DAOGruposMA.ListaVigencias(this.Usuario);

            StoreVigencias.DataBind();
        }

        /***********************************************************************************
         * Llena el combo de colectivas del formulario para nuevos programas
         **********************************************************************************/
        private void LlenarComboColectivas()
        {
            StoreColectivas = ProgramaComponente.CrearStoreColectivas(StoreColectivas);
            StoreColectivas.DataSource = DAOProgramas.ListarColectivas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            StoreColectivas.DataBind();
        }

        /***********************************************************************************
         * Guarda los datos del formulario en la base de datos
         **********************************************************************************/
        protected void GuardarNuevoPrograma_Click(object sender, EventArgs e)
        {

            GrupoCuenta unGrupoCuenta = new GrupoCuenta();

            unGrupoCuenta.ClaveGrupoCuenta = TxtProgramaClave.Text;
            unGrupoCuenta.Descripcion = TxtProgramaDescripcion.Text;

            string vigencia = SProgramaVigencia.SelectedItem.Value;
            int id = String.IsNullOrEmpty(vigencia) ? 0 : Convert.ToInt32(vigencia);
            unGrupoCuenta.ID_Vigencia = id;

            string colectiva = SProgramaColectivaEmisor.SelectedItem.Value;
            id = String.IsNullOrEmpty(colectiva) ? 0 : Convert.ToInt32(colectiva);
            unGrupoCuenta.ID_ColectivaEmisor = id;

            DAOProgramas.insertar(unGrupoCuenta, this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            this.WNuevoPrograma.Hide();
            LlenarGridProgramas();
        }

        /***********************************************************************************
         * Despliega y configura el formulario para un nuevo programa
         **********************************************************************************/
        protected void NuevoPrograma_Click(object sender, EventArgs e)
        {
            LlenarComboColectivas();
            LlenarComboVigencias();
            this.FNuevoPrograma.Reset();
            this.WNuevoPrograma.Show();
        }
    }
}