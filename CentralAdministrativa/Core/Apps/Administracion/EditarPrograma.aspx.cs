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
    public partial class EditarPrograma : DALCentralAplicaciones.PaginaBaseCAPP
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
            GridProgramas = ProgramaComponente.CrearGridPanelBase(GridProgramas, true);
        }

        /***********************************************************************************
         * Realiza la creación del formulario para registro de nuevos programas
         **********************************************************************************/
        private void CrearFormNuevoPrograma()
        {
            StoreVigencias = ProgramaComponente.CrearStoreVigencias(StoreVigencias);
            StoreColectivas = ProgramaComponente.CrearStoreColectivas(StoreColectivas);
            DialogEditarPrograma.Title = "Nuevo Programa";
            DialogEditarPrograma.Hidden = true;
            DialogEditarPrograma.Modal = true;
            DialogEditarPrograma.Height = 200;
            DialogEditarPrograma.Width = 400;

            FormEditarPrograma.Padding = 10;
            FormEditarPrograma.MonitorValid = true;

            TxtID_Programa.Hidden = false;

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

        /**********************************************************************************************
         *  Analiza el comando seleccionado
         *********************************************************************************************/
        protected void EditarPrograma_Click(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];
            if (!CommandName.Equals(ProgramaComponente.COMMAND_NAME))
            {
                return;
            }

            FormEditarPrograma.Reset();
            LlenarComboColectivas();
            LlenarComboVigencias();
            TxtID_Programa.Text = e.ExtraParams["ID_GrupoCuenta"];
            IDictionary<string, string> programaSeleccionado = Util.getDirectEventArgsValues(e);

            if (programaSeleccionado == null)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in programaSeleccionado)
            {
                switch (column.Key)
                {
		            case "ID_ColectivaEmisor":
                        SProgramaColectivaEmisor.SelectedItem.Value = column.Value;
                        break;

                    case "ClaveGrupoCuenta":
                        TxtProgramaClave.Text = column.Value;
                        break;

                    case "Descripcion":
                        TxtProgramaDescripcion.Text = column.Value;
                        break;

                    case "ID_Vigencia":
                        SProgramaVigencia.SelectedItem.Value = column.Value;
                        break;

                    default:
                        break;
                }
            }

            DialogEditarPrograma.Show();

        }

        /***********************************************************************************
         * Guarda los datos del formulario en la base de datos
         **********************************************************************************/
        protected void GuardarPrograma_Click(object sender, EventArgs e)
        {

            GrupoCuenta unGrupoCuenta = new GrupoCuenta();
            int id = String.IsNullOrEmpty(TxtID_Programa.Text) ? 0 : Int32.Parse(TxtID_Programa.Text);
            unGrupoCuenta.ID_GrupoCuenta = id;
            if (id == 0) {
                X.MessageBox.Alert("Error", "Programa no seleccionado").Show();
                DialogEditarPrograma.Hide();
                return;
            }
 
            unGrupoCuenta.ClaveGrupoCuenta = TxtProgramaClave.Text;
            unGrupoCuenta.Descripcion = TxtProgramaDescripcion.Text;

            string vigencia = SProgramaVigencia.SelectedItem.Value;
            id = String.IsNullOrEmpty(vigencia) ? 0 : Convert.ToInt32(vigencia);
            unGrupoCuenta.ID_Vigencia = id;

            string colectiva = SProgramaColectivaEmisor.SelectedItem.Value;
            id = String.IsNullOrEmpty(colectiva) ? 0 : Convert.ToInt32(colectiva);
            unGrupoCuenta.ID_ColectivaEmisor = id;

            DAOProgramas.actualizar(unGrupoCuenta, this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            this.DialogEditarPrograma.Hide();
            LlenarGridProgramas();
        }
    }
}