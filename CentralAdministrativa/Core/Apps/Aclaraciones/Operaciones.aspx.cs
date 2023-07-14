using DALAclaraciones.BaseDatos;
using DALAclaraciones.Entidades;
using DALAclaraciones.LogicaNegocio;
using DALAclaraciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Web;

namespace Aclaraciones
{
    public partial class Operaciones : DALCentralAplicaciones.PaginaBaseCAPP
    {
        static Operacion OpPorAclarar = new Operacion();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {                   
                    PreparaGrid();
                    PreparaGridHistorial();
                    StoreGpoTarjeta.DataSource = DAOCatalogos.ListaGruposMedioAcceso();
                    //StoreGpoTarjeta.DataSource = DAOCatalogos.ListaGruposMedioAcceso(this.Usuario, ConfigurationManager.AppSettings["IdApplication"].ToString());
                    StoreGpoTarjeta.DataBind();

                    datFechaInicial.SetValue(DateTime.Today.AddDays(-7));
                    datFechaInicial.MaxDate = DateTime.Today;

                    datFechaFinal.SetValue(DateTime.Today);
                    datFechaFinal.MaxDate = DateTime.Today;
                }

                if (!X.IsAjaxRequest)
                {
                    this.BindData();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                ActualizaDatos();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Aclaraciones", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Aclaraciones", "Ocurrió un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        protected void ActualizaDatos()
        {
            try
            {
                PreparaGrid();
                PreparaGridHistorial();                
                this.BindData();
            }
            catch
            {
                throw new CAppException(8009, "Ocurrio una Falla al Actualizar los Datos del Grid");
            }
        }

        private void AgregaRecordFiles()
        {
            StoreBusqueda.AddField(new RecordField("ID_Operacion"));
            StoreBusqueda.AddField(new RecordField("FechaOperacion", RecordFieldType.Date));
            StoreBusqueda.AddField(new RecordField("NumeroTarjeta"));
            StoreBusqueda.AddField(new RecordField("Importe", RecordFieldType.Float));
            StoreBusqueda.AddField(new RecordField("NumComercio"));
            StoreBusqueda.AddField(new RecordField("DescripcionComercio"));
            StoreBusqueda.AddField(new RecordField("EstatusOp"));
        }

        protected void PreparaGrid()
        {
            AgregaRecordFiles();

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colId_Operacion = new Column();
            colId_Operacion.DataIndex = "ID_Operacion";
            colId_Operacion.Header = "ID ";
            colId_Operacion.Width = 60;
            colId_Operacion.Hidden = true;
            colId_Operacion.Sortable = true;
            lasColumnas.Columns.Add(colId_Operacion);

            DateColumn colFechaOperacion = new DateColumn();
            colFechaOperacion.Format = "dd-MMM-yyyy";
            colFechaOperacion.DataIndex = "FechaOperacion";
            colFechaOperacion.Header = "Fecha";
            colFechaOperacion.Sortable = true;
            lasColumnas.Columns.Add(colFechaOperacion);

            Column colNumeroTarjeta = new Column();
            colNumeroTarjeta.DataIndex = "NumeroTarjeta";
            colNumeroTarjeta.Header = "Tarjeta";
            colNumeroTarjeta.Width = 100;
            colNumeroTarjeta.Sortable = true;
            lasColumnas.Columns.Add(colNumeroTarjeta);

            Column colImporte = new Column();
            colImporte.DataIndex = "Importe";
            colImporte.Header = "Importe";
            colImporte.Width = 100;
            colImporte.Sortable = true;
            colImporte.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colImporte);

            Column colNumeroComercio = new Column();
            colNumeroComercio.DataIndex = "NumComercio";
            colNumeroComercio.Header = "Afiliación";
            colNumeroComercio.Width = 100;
            colNumeroComercio.Sortable = true;
            lasColumnas.Columns.Add(colNumeroComercio);

            Column colDescripcionComercio = new Column();
            colDescripcionComercio.DataIndex = "DescripcionComercio";
            colDescripcionComercio.Header = "Descripción";
            colDescripcionComercio.Width = 150;
            colDescripcionComercio.Sortable = true;
            lasColumnas.Columns.Add(colDescripcionComercio);

            Column colEstatusOperacion = new Column();
            colEstatusOperacion.DataIndex = "EstatusOp";
            colEstatusOperacion.Header = "Estatus de la Operación";
            colEstatusOperacion.Width = 150;
            colEstatusOperacion.Sortable = true;
            lasColumnas.Columns.Add(colEstatusOperacion);

            
            ImageCommandColumn acciones = new ImageCommandColumn();
            acciones.Header = "Acciones";
            acciones.Width = 100;
            //ACCIONES:
            //Historial de envíos
            ImageCommand historial = new ImageCommand();
            historial.Icon = Icon.Time;
            historial.CommandName = "ConsultarHistorialEnviosContracargos";
            historial.ToolTip.Text = "Consultar Histoial de Envíos a Contracargos";
            acciones.Commands.Add(historial);
            //Enviar aclaración
            ImageCommand envio = new ImageCommand();
            envio.Icon = Icon.ScriptGo;
            envio.CommandName = "EnviarAclaracion";
            envio.ToolTip.Text = "Envío a Aclaración";
            acciones.Commands.Add(envio);
            //Eliminar aclaración
            ImageCommand borrar = new ImageCommand();
            borrar.Icon = Icon.ScriptDelete;
            borrar.CommandName = "EliminarOperacionArchivoAclaraciones";
            borrar.ToolTip.Text = "Eliminar Operación del Archivo de Aclaraciones";
            acciones.Commands.Add(borrar);



            ////AGREGAR COLUMNAS
            GridBusqueda.ColumnModel.Columns.Add(colId_Operacion);
            GridBusqueda.ColumnModel.Columns.Add(acciones);
            GridBusqueda.ColumnModel.Columns.Add(colFechaOperacion);
            GridBusqueda.ColumnModel.Columns.Add(colNumeroTarjeta);
            GridBusqueda.ColumnModel.Columns.Add(colImporte);
            GridBusqueda.ColumnModel.Columns.Add(colNumeroComercio);
            GridBusqueda.ColumnModel.Columns.Add(colDescripcionComercio);
            GridBusqueda.ColumnModel.Columns.Add(colEstatusOperacion);


            //AGREGAR EVENTOS
            GridBusqueda.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Operacion", "record.data.ID_Operacion", ParameterMode.Raw));
            GridBusqueda.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Importe", "record.data.Importe", ParameterMode.Raw));
            GridBusqueda.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

        }

        /// <summary>
        /// 
        /// </summary>
        private void AgregaHistorialRecordFiles()
        {
            Store2.AddField(new RecordField("IdArchivo"));
            Store2.AddField(new RecordField("FechaEnvio", RecordFieldType.Date));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void PreparaGridHistorial()
        {
            AgregaHistorialRecordFiles();

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colId_Archivo = new Column();
            colId_Archivo.DataIndex = "IdArchivo";
            colId_Archivo.Header = "ID ";
            colId_Archivo.Width = 60;
            colId_Archivo.Sortable = true;
            lasColumnas.Columns.Add(colId_Archivo);

            DateColumn colFechaEnvio = new DateColumn();
            colFechaEnvio.Format = "dd-MMM-yyyy";
            colFechaEnvio.DataIndex = "FechaEnvio";
            colFechaEnvio.Header = "Fecha de Envío";
            colFechaEnvio.Width = 240;
            colFechaEnvio.Sortable = true;
            lasColumnas.Columns.Add(colFechaEnvio);

            ////AGREGAR COLUMNAS
            GridPanel2.ColumnModel.Columns.Add(colId_Archivo);
            GridPanel2.ColumnModel.Columns.Add(colFechaEnvio);

            //AGREGAR EVENTOS
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("IdArchivo", "record.data.IdArchivo", ParameterMode.Raw));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 IdOperacion = Int64.Parse(e.ExtraParams["ID_Operacion"]);
                float importe = float.Parse(e.ExtraParams["Importe"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (IdOperacion == 0)
                {
                    throw new CAppException(8009, "Por Favor, Selecciona el Registro al que Deseas Aplicar el Comando <b> " + EjecutarComando + "</b>");
                }

                HttpContext.Current.Session.Add("ID_Operacion", IdOperacion);

                switch (EjecutarComando)
                {
                    case "ConsultarHistorialEnviosContracargos":
                        ConsultarHistorial(IdOperacion);
                        break;

                    case "EnviarAclaracion":
                        OpPorAclarar.Id_Operacion = IdOperacion;
                        OpPorAclarar.ImporteOper = importe;

                        StoreReasonCode.DataSource = DAOCatalogos.ListaReasonCodes();
                        StoreReasonCode.DataBind();
                        StoreDI.DataSource = DAOCatalogos.ListaDocumentIndicator();
                        StoreDI.DataBind();

                        this.frmEnvioAcl.Visible = true;
                        this.frmEnvioAcl.Hidden = false;
                        break;

                    case "EliminarOperacionArchivoAclaraciones":
                        EliminarOperacionDeArchivo(IdOperacion);
                        break;
                }
                ActualizaDatos();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Aclaraciones", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Aclaraciones", "Por Favor, Selecciona el Registro al que Deseas Aplicar la Acción").Show();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID_Operacion"></param>

        [DirectMethod(Namespace = "Aclaraciones")]
        public void EliminarOperacionDeArchivo(Int64 ID_Operacion)
        {
            try
            {
                LNOperaciones.EliminarOperacionDeContracargos(ID_Operacion, this.Usuario);
                X.Msg.Notify("Eliminar Operación", "Operación Eliminada Exitosamente").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Eliminar Operación", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Eliminar Operación ", "Ocurrió un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Operacion oper = new Operacion();

                oper.Tarjeta = Convert.ToInt64(txtTarjeta.Value.ToString() == "" ? "0" : txtTarjeta.Value).ToString();
                oper.ID_GrupoMA = short.Parse(cmbGpoTarjeta.Text == "" ? "0" : cmbGpoTarjeta.Text);
                oper.FechaInicial = (DateTime)datFechaInicial.Value == null ? DateTime.Now : (DateTime)datFechaInicial.Value;
                oper.FechaFinal = (DateTime)datFechaFinal.Value == null ? DateTime.Now : (DateTime)datFechaFinal.Value;
                oper.Importe = float.Parse(txtImporte.Text == "" ? "0" : txtImporte.Text);

                PreparaGrid();
                GridBusqueda.GetStore().DataSource = LNOperaciones.BuscarOperacion(oper, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridBusqueda.GetStore().DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Aclaraciones", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Aclaraciones", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdOperacion"></param>
        protected void ConsultarHistorial(Int64 IdOperacion)
        {
            try
            {
                PreparaGridHistorial();
                GridPanel2.GetStore().DataSource = LNOperaciones.ConsultarHistorialContracargos(IdOperacion, this.Usuario);
                GridPanel2.GetStore().DataBind();
                this.frmHistorial.Visible = true;
                this.frmHistorial.Hidden = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Aclaraciones", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Aclaraciones", "Ocurrió un Error al Consultar el Historial de Envíos con los Datos Proporcionados").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarAclr_Click(object sender, EventArgs e)
        {
            try
            {
                OpPorAclarar.ImporteAcl = float.Parse(nfImporte.Text == "" ? "0" : nfImporte.Text);
                OpPorAclarar.Id_RC = int.Parse(ComboBoxRC.Text == "" ? "0" : ComboBoxRC.Text.ToString());
                OpPorAclarar.Id_DI = int.Parse(ComboBoxDI.Text == "" ? "0" : ComboBoxDI.Text.ToString());
                OpPorAclarar.Observaciones = txtObservaciones.Text;

                LNOperaciones.EnviarOperacionAclaracion(OpPorAclarar, this.Usuario);

                PanelEnvio.Reset();
                X.Js.AddScript("#{frmEnvioAcl}.setVisible(false);");
                X.Msg.Notify("Enviar Operación", "La Operación se Envío a Aclaración Exitosamente").Show();
            }
            catch (CAppException err)
            {
                PanelEnvio.Reset();
                X.Js.AddScript("#{frmEnvioAcl}.setVisible(false);");
                X.Msg.Alert("Aclaraciones", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                PanelEnvio.Reset();
                X.Js.AddScript("#{frmEnvioAcl}.setVisible(false);");
                X.Msg.Alert("Aclaraciones", "Ocurrió un Error al Enviar la Operación a Aclaración con los Datos Proporcionados").Show();
            }
        }

        protected void btnBuscarNuevo_Click(object sender, EventArgs e)
        {
            frmBusqueda.Reset();
            StoreBusqueda.RemoveAll();
            frmGridBusqueda.Reset();
        }

        private void BindData() { }

    }
}