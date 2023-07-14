using DALCentralAplicaciones.Utilidades;
using DALCertificados.BaseDatos;
using DALCertificados.LogicaNegocio;
using Ext.Net;
using System;
using System.Configuration;

namespace Certificados
{
    public partial class AddCertificado : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {


                    cmbColectiva.Clear();
                    SCColectiva.DataSource = DALAutorizador.BaseDatos.DAOCatalogos.ListaColectivaCadenasComercial((String)"CCM", this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    SCColectiva.DataBind();
                }

                if (!IsPostBack)
                {

                    PreparaGrid();
                    BindDataCertificados();

                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void limpiarForm()
        {
            try
            {
              
                FormPanel1.Reset();
            }
            catch (Exception)
            {
            }
        }

        private void BindDataCertificados()
        {
            GridPanel1.GetStore().DataSource = DAOCertificado.ListaCertificados((this.Usuario).UsuarioTemp, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (LNCertificado.Crear(Int64.Parse(cmbColectiva.SelectedItem.Value), this.Usuario, Int32.Parse(cmbExpiracion.SelectedItem.Value), Int32.Parse(cmbTotalCertificados.SelectedItem.Value)))
                {
                    X.Msg.Notify("Creación Certificados", "Se crearon <br />  <b>" + cmbTotalCertificados.SelectedItem.Value + "</b> Certificados <br /> Expirarán el dia: <b>" + DateTime.Now.AddDays(Int32.Parse(cmbExpiracion.SelectedItem.Value)).ToShortDateString() + "</b> <br />").Show();
                    X.Msg.Notify("Creación Certificados", " <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();

                    PreparaGrid();
                    BindDataCertificados();
                }
                else
                {
                    X.Msg.Notify("Creación Certificados", "NO SE CREARON LOS CERTIFICADOS").Show();
                    X.Msg.Notify("Creación Certificados", " <br />  <br /> <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                }

                limpiarForm();
            }
            catch (Exception)
            {
                X.Msg.Alert("Status", "Ocurrio Un Error al crear el Certificado, Intentalo más tarde").Show();
                return;
            }

        }

    
        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            try
            {
               

            }
            catch (Exception)
            {
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Panel5.Collapsed = true;
        }

        private void AgregaRecordFiles()
        {
            storeCertificados.AddField(new RecordField("ID_Certificado"));
            storeCertificados.AddField(new RecordField("ID_CadenaComercial"));
            storeCertificados.AddField(new RecordField("ID_Estatus"));
            storeCertificados.AddField(new RecordField("Clave"));
            storeCertificados.AddField(new RecordField("FechaCreacion"));
            storeCertificados.AddField(new RecordField("UsuarioCreacion"));
            storeCertificados.AddField(new RecordField("FechaCaducidad"));
            storeCertificados.AddField(new RecordField("ID_Activacion"));
            storeCertificados.AddField(new RecordField("Sucursal"));
            storeCertificados.AddField(new RecordField("Afilacion"));
            storeCertificados.AddField(new RecordField("Terminal"));
            storeCertificados.AddField(new RecordField("UsuarioActivacion"));
            storeCertificados.AddField(new RecordField("FechaActivacion"));
            storeCertificados.AddField(new RecordField("MAC"));
            storeCertificados.AddField(new RecordField("IDPROC"));
            storeCertificados.AddField(new RecordField("IDMB"));
            storeCertificados.AddField(new RecordField("IDWIN"));

        }

        protected void PreparaGrid()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFiles();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            CommandColumn acciones = new CommandColumn();
            acciones.Header = "Acciones";
            acciones.Width = 55;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand deshabilitar = new GridCommand();
            deshabilitar.Icon = Icon.ShieldStop;
            deshabilitar.CommandName = "Deshabilitar";
            deshabilitar.ToolTip.Text = "Deshabilitar Certificado";

            GridCommand estatusPorActivar = new GridCommand();
            estatusPorActivar.Icon = Icon.ShieldGo;
            estatusPorActivar.CommandName = "Inactivo";
            estatusPorActivar.ToolTip.Text = "El Certificado está esperando ser Activado";

            GridCommand Caducado = new GridCommand();
            Caducado.Icon = Icon.ShieldError;
            Caducado.CommandName = "Caducado";
            Caducado.ToolTip.Text = "El Certificado está Caducado";

            GridCommand indeterminado = new GridCommand();
            indeterminado.Icon = Icon.ShieldSilver;
            indeterminado.CommandName = "indeterminado";
            indeterminado.ToolTip.Text = "El Certificado está en estatus no Valido para ser usado";

            acciones.Commands.Add(estatusPorActivar);
            
            acciones.Commands.Add(deshabilitar);
            acciones.Commands.Add(Caducado);
            acciones.Commands.Add(indeterminado);
            
            
           

            Column col1 = new Column();
            col1.DataIndex = "ID_Certificado";
            col1.Header = "ID";
            col1.Sortable = true;
            col1.Hidden = true;


            Column col2 = new Column();
            col2.DataIndex = "ID_CadenaComercial";
            col2.Header = "ID_CadenaComercial";
            col2.Sortable = true;

            Column col3 = new Column();
            col3.DataIndex = "Clave";
            col3.Header = "Clave";
            col3.Sortable = true;

            Column col4 = new Column();
            col4.DataIndex = "ID_Estatus";
            col4.Header = "ID_Estatus";
            col4.Sortable = true;

            Column col5 = new Column();
            col5.DataIndex = "Sucursal";
            col5.Header = "Sucursal";
            col5.Sortable = true;

            Column col6 = new Column();
            col6.DataIndex = "Afilacion";
            col6.Header = "Afilacion";
            col6.Sortable = true;

            Column col7 = new Column();
            col7.DataIndex = "Terminal";
            col7.Header = "Terminal";
            col7.Sortable = true;

            Column col8 = new Column();
            col8.DataIndex = "FechaActivacion";
            col8.Header = "FechaActivacion";
            col8.Sortable = true;

            Column col9 = new Column();
            col9.DataIndex = "MAC";
            col9.Header = "MAC";
            col9.Sortable = true;

            Column col10 = new Column();
            col10.DataIndex = "IDPROC";
            col10.Header = "IDPROC";
            col10.Sortable = true;

            Column col11 = new Column();
            col11.DataIndex = "IDMB";
            col11.Header = "IDMB";
            col11.Sortable = true;

            Column col12 = new Column();
            col12.DataIndex = "IDWIN";
            col12.Header = "IDWIN";
            col12.Sortable = true;


            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(col1);
            GridPanel1.ColumnModel.Columns.Add(col2);
            GridPanel1.ColumnModel.Columns.Add(col3);
            GridPanel1.ColumnModel.Columns.Add(col4);
            GridPanel1.ColumnModel.Columns.Add(col5);
            GridPanel1.ColumnModel.Columns.Add(col6);
            GridPanel1.ColumnModel.Columns.Add(col7);
            GridPanel1.ColumnModel.Columns.Add(col8);
            GridPanel1.ColumnModel.Columns.Add(col9);
            GridPanel1.ColumnModel.Columns.Add(col10);
            GridPanel1.ColumnModel.Columns.Add(col11);
            GridPanel1.ColumnModel.Columns.Add(col12);
            


            ////AGREGAR EVENTOS
            //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Certificado", "this.getRowsValues({ selectedOnly: true })[0].ID_Certificado", ParameterMode.Raw));
            //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Clave", "this.getRowsValues({ selectedOnly: true })[0].Clave", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();


            StringFilter fil1 = new StringFilter();
            fil1.DataIndex = "Clave";
            losFiltros.Filters.Add(fil1);

            StringFilter fil2 = new StringFilter();
            fil2.DataIndex = "Sucursal";
            losFiltros.Filters.Add(fil2);

            StringFilter fil3 = new StringFilter();
            fil3.DataIndex = "Afiliacion";
            losFiltros.Filters.Add(fil3);

            StringFilter fil4 = new StringFilter();
            fil4.DataIndex = "Terminal";
            losFiltros.Filters.Add(fil4);

            GridPanel1.Plugins.Add(losFiltros);


        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Guid ID_Certificado = new Guid(e.ExtraParams["ID_Certificado"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];
            
                switch (EjecutarComando)
                {
                    case "Deshabilitar":
                        LNCertificado.Desactivar(ID_Certificado, this.Usuario);
                        X.Msg.Notify("Los Certificados", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();

                        PreparaGrid();
                        BindDataCertificados(); 
                        break;
                    case "Activar":
                        break;
                }

                
                
            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Los Certificados", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Los Certificados", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }
   

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGrid();
                // LlenaTiposColectiva();
                BindDataCertificados();
            }
            catch (Exception)
            {
            }

        }
    }
}
