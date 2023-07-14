using DALCentralAplicaciones.Utilidades;
using DALAutorizador.BaseDatos;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace ValidacionesBatch
{
    public partial class Configuraciones : DALCentralAplicaciones.PaginaBaseCAPP
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
            /* reglas */
            CrearGridReglas();
            LlenarGridReglas();

            /* configuración */
            CrearPanelConfiguracion();
        }

        /*******************************************************************************
         * Realiza la creación del componente grid para las reglas
         ******************************************************************************/
        private void CrearGridReglas() 
        {
            ImageCommand ConfiguracionRegla = new ImageCommand();
            ConfiguracionRegla.Icon = Icon.CogGo;
            ConfiguracionRegla.CommandName = "ConfigRegla";

            ImageCommandColumn ConfiguracionReglaColumn = new ImageCommandColumn();
            ConfiguracionReglaColumn.Width = 25;
            ConfiguracionReglaColumn.Commands.Add(ConfiguracionRegla);
            GridReglas.ColumnModel.Columns.Add(ConfiguracionReglaColumn);

            GroupingSummaryColumn ID_Regla = new GroupingSummaryColumn();
            ID_Regla.DataIndex = "ID_Regla";
            ID_Regla.Hidden = true;
            GridReglas.ColumnModel.Columns.Add(ID_Regla);

            GroupingSummaryColumn Nombre = new GroupingSummaryColumn();
            Nombre.DataIndex = "Nombre";
            Nombre.Header = "Nombre";
            GridReglas.ColumnModel.Columns.Add(Nombre);

            GroupingSummaryColumn DescripcionRegla = new GroupingSummaryColumn();
            DescripcionRegla.DataIndex = "DescripcionRegla";
            DescripcionRegla.Header = "Descripción";
            DescripcionRegla.Width = 250;
            GridReglas.ColumnModel.Columns.Add(DescripcionRegla);

            GroupingSummaryColumn StoreProcedure = new GroupingSummaryColumn();
            StoreProcedure.DataIndex = "StoreProcedure";
            StoreProcedure.Header = "Store Procedure";
            StoreProcedure.Width = 150;
            GridReglas.ColumnModel.Columns.Add(StoreProcedure);

            GroupingSummaryColumn OrdenEjecucion = new GroupingSummaryColumn();
            OrdenEjecucion.DataIndex = "OrdenEjecucion";
            OrdenEjecucion.Header = "Orden";
            OrdenEjecucion.Width = 80;
            GridReglas.ColumnModel.Columns.Add(OrdenEjecucion);

            GroupingSummaryColumn EsActiva = new GroupingSummaryColumn();
            EsActiva.DataIndex = "EsActiva";
            EsActiva.Header = "Activa";
            EsActiva.Width = 50;
            GridReglas.ColumnModel.Columns.Add(EsActiva);

            GroupingSummaryColumn EsAccion = new GroupingSummaryColumn();
            EsAccion.DataIndex = "EsAccion";
            EsAccion.Header = "Accion";
            EsAccion.Width = 50;
            GridReglas.ColumnModel.Columns.Add(EsAccion);

            GridReglas.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "ID_Regla", "record.data.ID_Regla", ParameterMode.Raw
                ));

            GridReglas.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "CommandName", "command", ParameterMode.Raw
                ));

            GridReglas.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values", "Ext.encode(record.data)", ParameterMode.Raw
                ));

            GridReglas.Title = "Configuración Reglas";
            GridReglas.AutoExpandColumn = "Nombre";
        }

        /*******************************************************************************
         * Panel de configuración
         ******************************************************************************/
        private void CrearPanelConfiguracion() 
        {
            PanelConfig.Title = "Configuración de Regla";
            DatosConfig.Title = "Configuración de Parámetros por Prioridades";
            DatosRegla.Height = 200;

            TxtDescripcion.FieldLabel = "Descripción";
            TxtDescripcion.AnchorHorizontal = "100%";
            TxtDescripcion.ReadOnly = true;

            TxtNombreRegla.FieldLabel = "Nombre Regla";
            TxtNombreRegla.AnchorHorizontal = "100%";
            TxtNombreRegla.ReadOnly = true;

            SelectCadena.FieldLabel = "Cadena";
            SelectCadena.AnchorHorizontal = "100%";

            PanelRegla.Title = "1. Cuenta";
            PanelTipoCuenta.Title = "2. Tipo Cuenta";
            PanelGrupoCuenta.Title = "3. Grupo Cuenta";
            PanelCuenta.Title = "4. Cuenta";
            PanelGrupoTarjeta.Title = "5. Grupo Tarjeta";
            PanelTarjeta.Title = "6. Tarjeta";
        }

        /*******************************************************************************
         * Consulta las reglas y llena el componente
         ******************************************************************************/
        private void LlenarGridReglas()
        {
            this.StoreReglas.DataSource = DAORegla.ListaDeReglas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreReglas.DataBind();
        }

        /*******************************************************************************
         * Actualiza los datos del grid reglas
         ******************************************************************************/
        protected void StoreReglas_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridReglas();
        }

        /*******************************************************************************
         * Acciones del comando para ver la pantalla de configuraciones
         ******************************************************************************/
        protected void Click_ConfigurarRegla(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];
            if (!CommandName.Equals("ConfigRegla"))
            {
                return;
            }

            string json = String.Format("[{0}]", e.ExtraParams["Values"]);

            IDictionary<string, string>[] reglasSeleccionadas = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (reglasSeleccionadas == null || reglasSeleccionadas.Length < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in reglasSeleccionadas[0])
            {
                switch (column.Key)
                {

                    case "ID_Regla":
                        break;

                    case "Nombre":
                        TxtNombreRegla.Text = column.Value;
                        break;

                    case "DescripcionRegla":
                        TxtDescripcion.Text = column.Value;
                        break;

                    default:
                        break;
                }
            }

            PanelConfig.Show();
        }
    }
}