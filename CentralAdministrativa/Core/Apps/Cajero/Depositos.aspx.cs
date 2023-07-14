using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCajero.Entidades;
using DALCajero.BaseDatos;
using DALCajero.LogicaNegocio;
using Interfases;
using System.Web.Security;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using System.Configuration;
using System.Text.RegularExpressions;
using DALCentralAplicaciones;



namespace Cajero
{
    public partial class Depositos : PaginaBaseCAPP
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                 
                    PreparaGrid();

                    StoreBanco.DataSource = DAOCatalogos.ListaBancos();
                    StoreBanco.DataBind();

                    //StoreTipoMA.DataSource = DAOCatalogos.ListaTiposMedioAcceso();
                    //StoreTipoMA.DataBind();

                    try
                    {
                        StoreOper.DataSource = LNCatalogos.ListaTiposCuentaAutorizador(Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), this.Usuario);// DAOCatalogos.ListaTiposOperacion();
                        StoreOper.DataBind();
                    }
                    catch (Exception err)
                    {
                        DALCajero.Utilidades.Loguear.Error(err, "");
                        cmbTipoCuentaAbono.EmptyText = "NO HAY CUENTAS";
                    }
                    //storeCCM.DataSource = DAOCatalogos.ListaCadenasComerciales();
                    //storeCCM.DataBind();
                }

                if (!X.IsAjaxRequest)
                {
                    this.BindData();
                }

            }
            catch (Exception err)
            {
                DALCajero.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }


        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 IdFicha = Int64.Parse(e.ExtraParams["ID_Ficha"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (IdFicha == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " +EjecutarComando+ "</b>");
                }
                Usuario elUser = this.Usuario;

                FichaDeposito laFichaSeleccionada = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, elUser, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();


                switch (EjecutarComando)
                {
                    case "Eliminar":
                        LNFichaDeposito.EliminarFichaDeposito(laFichaSeleccionada, elUser);
                        break;
                    case "Asignar":
                        LNAsignacion.AsignarFichaDepositoAMovimiento(laFichaSeleccionada, elUser, enumTipoAsignacion.AlPresionarGridFicha, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        break;
                }

                PreparaGrid();
                BindData();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Fichas de Depósito", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Fichas de Depósito", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        protected void Refreshcombo(object sender, StoreRefreshDataEventArgs e)
        {
                try
                {
                    StoreOper.DataSource = LNCatalogos.ListaTiposCuentaAutorizador(Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), this.Usuario);// DAOCatalogos.ListaTiposOperacion();
                    StoreOper.DataBind();
                }
                catch (Exception err)
                {
                    DALCajero.Utilidades.Loguear.Error(err, "");
                    cmbTipoCuentaAbono.EmptyText = "NO HAY CUENTAS";
                }
       

        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGrid();
                this.BindData();
            }
            catch (Exception)
            {
            }

        }

        private void AgregaRecordFiles()
        {
            Store1.AddField(new RecordField("ID_FichaDeposito"));
            Store1.AddField(new RecordField("ID_EstatusFichaDeposito"));
            Store1.AddField(new RecordField("EstatusDescripcion"));
            Store1.AddField(new RecordField("Descripcion"));
            Store1.AddField(new RecordField("ClaveSucursalBancaria"));
            Store1.AddField(new RecordField("ClaveCajaBancaria"));
            Store1.AddField(new RecordField("ClaveOperador"));
            Store1.AddField(new RecordField("FechaOperacion", RecordFieldType.Date));
            Store1.AddField(new RecordField("Consecutivo"));
            Store1.AddField(new RecordField("Referencia"));
            Store1.AddField(new RecordField("Importe", RecordFieldType.Float));
            Store1.AddField(new RecordField("ClaveTipoMA"));
            Store1.AddField(new RecordField("ClaveMedioAcceso"));
            Store1.AddField(new RecordField("Afiliacion"));
            Store1.AddField(new RecordField("FechaRegistro", RecordFieldType.Date));
        }

        protected void PreparaGrid()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFiles();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Estatus = new Column();
            colID_Estatus.DataIndex = "ID_EstatusFichaDeposito";
            colID_Estatus.Header = "ID_Estatus";
            colID_Estatus.Width = 50;
            colID_Estatus.Hidden = true;
            colID_Estatus.Sortable = true;
            lasColumnas.Columns.Add(colID_Estatus);

            Column colID_FichaDeposito = new Column();
            colID_FichaDeposito.DataIndex="ID_FichaDeposito";
            colID_FichaDeposito.Header = "ID Ficha";
            colID_FichaDeposito.Width=50;
            colID_FichaDeposito.Sortable=true;
            lasColumnas.Columns.Add(colID_FichaDeposito);

            Column colEstatus = new Column();
            colEstatus.DataIndex = "EstatusDescripcion";
            colEstatus.Header = "Estado";
            colEstatus.Width = 70;
            colEstatus.Sortable = true;
            lasColumnas.Columns.Add(colEstatus);

            DateColumn colFechaRegistro = new DateColumn();
            colFechaRegistro.Format = "dd-MMM-yyyy";
            colFechaRegistro.DataIndex = "FechaRegistro";
            colFechaRegistro.Header = "Fecha Registro";
            colFechaRegistro.Sortable = true;
            lasColumnas.Columns.Add(colFechaRegistro);

            Column colClaveColectivaBanco = new Column();
            colClaveColectivaBanco.DataIndex = "Descripcion";
            colClaveColectivaBanco.Header = "Banco";
            colClaveColectivaBanco.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectivaBanco);

            Column colConsecutivo= new Column();
            colConsecutivo.DataIndex = "Consecutivo";
            colConsecutivo.Header = "Consecutivo";
            colConsecutivo.Sortable = true;
            lasColumnas.Columns.Add(colConsecutivo);

            Column colClaveSucursalBancaria= new Column();
            colClaveSucursalBancaria.DataIndex = "ClaveSucursalBancaria";
            colClaveSucursalBancaria.Header = "Sucursal Banco";
            colClaveSucursalBancaria.Sortable = true;
            lasColumnas.Columns.Add(colClaveSucursalBancaria);

            DateColumn colFechaOperacion = new DateColumn();
            colFechaOperacion.DataIndex = "FechaOperacion";
            colFechaOperacion.Header = "Fecha Operacion";
            colFechaOperacion.Format = "dd-MMM-yyyy";
            colFechaOperacion.Sortable = true;
            lasColumnas.Columns.Add(colFechaOperacion);

            Column colReferencia= new Column();
            colReferencia.DataIndex = "Referencia";
            colReferencia.Header = "Referencia";
            colReferencia.Sortable = true;
            lasColumnas.Columns.Add(colReferencia);

            Column colAfiliacion= new Column();
            colAfiliacion.DataIndex = "Afiliacion";
            colAfiliacion.Header = "Afiliacion";
            colAfiliacion.Hidden = true ;

            colAfiliacion.Sortable = true;
            lasColumnas.Columns.Add(colAfiliacion);

            Column colImporte= new Column();
            colImporte.DataIndex = "Importe";
            colImporte.Header = "Importe";
            colImporte.Sortable = true;
            colImporte.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colImporte);


            CommandColumn acciones = new CommandColumn();
            
            acciones.Header = "Acciones";
            acciones.Width = 55;
            acciones.PrepareToolbar.Fn = "prepareToolbar";


            GridCommand Reintenta = new GridCommand();
            Reintenta.Icon = Icon.EmailStart;
            Reintenta.CommandName = "Asignar";
            Reintenta.ToolTip.Text = "Buscar Movimiento Bancario y Asignar";
            acciones.Commands.Add(Reintenta);


            CommandSeparator separa = new CommandSeparator();
            acciones.Commands.Add(separa);


            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.Delete;
            elimina.CommandName = "Eliminar";
            elimina.ToolTip.Text = "Eliminar Ficha de Depósito";
            acciones.Commands.Add(elimina);

           
          

            //AGREGAR COLUMNAS
            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colEstatus);

            GridPanel1.ColumnModel.Columns.Add(colID_FichaDeposito);
            GridPanel1.ColumnModel.Columns.Add(colFechaRegistro);
            GridPanel1.ColumnModel.Columns.Add(colClaveColectivaBanco);
            GridPanel1.ColumnModel.Columns.Add(colConsecutivo);
            GridPanel1.ColumnModel.Columns.Add(colClaveSucursalBancaria);
            GridPanel1.ColumnModel.Columns.Add(colFechaOperacion);
            GridPanel1.ColumnModel.Columns.Add(colReferencia);
            GridPanel1.ColumnModel.Columns.Add(colAfiliacion);
            GridPanel1.ColumnModel.Columns.Add(colImporte);

            //AGREGAR EVENTOS
              //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_FichaDeposito","this.getRowsValues({ selectedOnly: true })[0].ID_FichaDeposito",ParameterMode.Raw));
            //GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Ficha", "#{GridPanel1}.getSelectionModel().hasSelection() ?#{GridPanel1}.getRowsValues({selectedOnly:true})[0].ID_FichaDeposito : '0'", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Ficha", "record.data.ID_FichaDeposito", ParameterMode.Raw));
              GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));
              //GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Registro", "record", ParameterMode.Raw));

            //PLUGINS DE FILTRADO
              GridFilters losFiltros = new GridFilters();


                NumericFilter filID_FichaDeposito = new NumericFilter();
                filID_FichaDeposito.DataIndex="ID_FichaDeposito";
                losFiltros.Filters.Add(filID_FichaDeposito);

                StringFilter filEstatus = new StringFilter();
                filEstatus.DataIndex = "EstatusDescripcion";
                losFiltros.Filters.Add(filEstatus);

                NumericFilter filID_EstatusFichaDeposito = new NumericFilter();
                filID_EstatusFichaDeposito.DataIndex="ID_EstatusFichaDeposito";
                losFiltros.Filters.Add(filID_EstatusFichaDeposito);

                StringFilter filClaveColectivaBanco = new StringFilter();
                filClaveColectivaBanco.DataIndex="Descripcion";
                losFiltros.Filters.Add(filClaveColectivaBanco);

                StringFilter filClaveSucursalBancaria = new StringFilter();
                filClaveSucursalBancaria.DataIndex="ClaveSucursalBancaria";
                losFiltros.Filters.Add(filClaveSucursalBancaria);

                StringFilter filClaveCajaBancaria = new StringFilter();
                filClaveCajaBancaria.DataIndex="ClaveCajaBancaria";
                losFiltros.Filters.Add(filClaveCajaBancaria);

                StringFilter filClaveOperador = new StringFilter();
                filClaveOperador.DataIndex="ClaveOperador" ;
                losFiltros.Filters.Add(filClaveOperador);

                DateFilter filFechaOperacion = new DateFilter();
                filFechaOperacion.DataIndex="FechaOperacion";
                filFechaOperacion.OnText = "Con Fecha";
                filFechaOperacion.BeforeText = "Desde";
                filFechaOperacion.AfterText = "Hasta";
                filFechaOperacion.DatePickerOptions.TodayText = "Hoy";
                losFiltros.Filters.Add(filFechaOperacion);


                NumericFilter filConsecutivo = new NumericFilter();
                filConsecutivo.DataIndex="Consecutivo";
                losFiltros.Filters.Add(filConsecutivo);

                StringFilter filClaveReferencia = new StringFilter();
                filClaveReferencia.DataIndex="Referencia";
                losFiltros.Filters.Add(filClaveReferencia);

                NumericFilter filImporte = new NumericFilter();
                filImporte.DataIndex="Importe" ;
                losFiltros.Filters.Add(filImporte);

                StringFilter filClaveTipoMA = new StringFilter();
                filClaveTipoMA.DataIndex="ClaveTipoMA";
                losFiltros.Filters.Add(filClaveTipoMA);

                StringFilter filClaveMedioAcceso = new StringFilter();
                filClaveMedioAcceso.DataIndex="ClaveMedioAcceso";
                losFiltros.Filters.Add(filClaveMedioAcceso);

                StringFilter filAfiliacion = new StringFilter();
                filAfiliacion.DataIndex="Afiliacion";
                losFiltros.Filters.Add(filAfiliacion);

                DateFilter filFechaRegistro = new DateFilter();
                filFechaRegistro.OnText = "Con Fecha";
                filFechaRegistro.BeforeText = "Desde";
                filFechaRegistro.AfterText = "Hasta";
                filFechaRegistro.DataIndex="FechaRegistro";
                filFechaRegistro.DatePickerOptions.TodayText = "Hoy";
            
                losFiltros.Filters.Add(filFechaRegistro);
            
                GridPanel1.Plugins.Add(losFiltros);
                
            
            

        }

        private void BindData()
        {
            //var store = this.GridPanel1.GetStore();
            Usuario elUser = this.Usuario;

            GridPanel1.GetStore().DataSource = DAOFichaDeposito.ListaFichasDeposito(elUser, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
            //store.DataSource = DAOFichaDeposito.ListaFichasDeposito();
            //store.DataBind();
           // PreparaGrid();
           // btnGuardar.Click +=new EventHandler(btnGuardar_Click);
            
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                //Obtiene el usuario en linea
                Usuario elUser = this.Usuario;
                FichaDeposito nuevaFicha= new FichaDeposito();
            
                Int64  Conse;
                float monto;
                

              bool isnumericoConsecutivo=  Int64.TryParse(txtConsecutivo.Text,out Conse);
              bool isnumericMonto = float.TryParse(txtImporte.Text, out monto);

              if ((Conse == 0) || (!isnumericoConsecutivo))
                {
                    throw new CAppException(8009, "El Valor <b>'" + txtConsecutivo.Text + "'</b> no es Valido en el Campo <b>Consecutivo</b>");
                }
              else if ((monto == 0) || (!isnumericMonto))
                {
                    throw new CAppException(8009, "El Valor <b>'" + txtImporte.Text + "'</b> no un Importe Valido");
                }


                    nuevaFicha = new FichaDeposito
                    {
                        Afiliacion = Regex.Match(cmbTipoCuentaAbono.SelectedItem.Value, @"\{([^)]*)\}").Groups[1].Value, //cmbTipoCuentaAbono.Value.ToString(),
                        ClaveUsuario = elUser.ClaveUsuario,
                        ClaveColectivaBanco = cmbBanco.Value.ToString(),
                        ClaveSucursalBancaria = txtSucursal.Text,
                        ClaveCajaBancaria = txtCaja.Text,
                        ClaveOperador = txtOperador.Text,
                        FechaOperacion = datFecha.Value.ToString(), //datFecha.Text.Substring(0, 10) + " " + datHora.Text,
                        Consecutivo = Int64.Parse(txtConsecutivo.Text),
                        Referencia = txtReferencia.Text,
                        Importe = float.Parse(txtImporte.Text),
                        Operacion = new TipoOperacionTransaccional(0, cmbTipoCuentaAbono.SelectedItem.Value.Substring(0, 2), cmbTipoCuentaAbono.SelectedItem.Value.Substring(2)),
                        // ClaveTipoMA = cmbTipoID.Value.ToString(),
                        ClaveTipoMA = DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), "TipoMA").Valor,//"IDCTA",
                        ClaveMedioAcceso = Regex.Match(cmbTipoCuentaAbono.SelectedItem.Value, @"\[([^)]*)\]").Groups[1].Value,
                        // ClaveMedioAcceso = txtMedioAcceso.Text,
                        Observaciones = txtObservaciones.Text,
                        AFLDescripcion =cmbTipoCuentaAbono.SelectedItem.Value,
                        DataTransaccionales = cmbTipoCuentaAbono.SelectedItem.Value


                    };
               

                if (nuevaFicha.EsCorrectoParaAgregar())
                {
                    Int64 Respon = -1;

                    Respon=LNFichaDeposito.AgregarFichaDeposito(nuevaFicha, elUser);
                    nuevaFicha.ID_FichaDeposito = Respon;

                    if (Respon > 0)
                    {
                        //Busca Moviiento Bancario para asignar la Ficha de deposito.
                        try
                        {
                          RespuestaTransaccional laRespuesta=  LNAsignacion.AsignarFichaDepositoAMovimiento(nuevaFicha, elUser, enumTipoAsignacion.AlRegistrarFicha, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                          DespliegaRespuesta(laRespuesta, "Asignar Ficha de Depósito");
                        }
                        catch (CAppException err)
                        {
                            X.Msg.Alert("Fichas de Depósito", err.Mensaje()).Show();
                        }
                        catch (Exception)
                        {
                            X.Msg.Alert("Fichas de Depósito", "Ocurrio un Error al Asignar la ficha de deposito con el Movimiento").Show();
                        }
                    }


                }
                else
                {
                    X.Msg.Alert("Guardar Ficha de Depósito", "Datos Proporcionados son inválidos, intenta nuevamente").Show();
                }


                FormPanel1.Reset();
                PreparaGrid();
                BindData();
 
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Guardar Ficha de Depósito", "Datos Proporcionados son inválidos, intenta nuevamente: <br/><br/>" + err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                X.Msg.Alert("Guardar Ficha de Depósito", "Datos Proporcionados son inválidos, intenta nuevamente: <br/>" + err.Message).Show();
            }
        }

        protected void DespliegaRespuesta(RespuestaTransaccional laRespuesta, String EjecutarComando)
        {
            try
            {
                if (laRespuesta.IsAutorizada())
                {
                    X.Msg.Notify("Respuesta Transaccional", "Resultados de la Operación <br />  Codigo Respuesta: <b>" + laRespuesta.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + laRespuesta.Autorizacion + "</b> <br />").Show();
                    X.Msg.Notify("Respuesta Transaccional", EjecutarComando + " <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();

                }
                else
                {
                    if (laRespuesta.IsTimeOut)
                    {
                        X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + laRespuesta.ResultadoOperacion + "</b> <br />").Show();
                        X.Msg.Notify("Respuesta Transaccional", EjecutarComando + " <br /><br />  <b> T I M E    O U T </b> <br />  <br /> ").Show();
                    }
                    else
                    {
                        if ((laRespuesta.DescripcionRespuesta == null) || (laRespuesta.DescripcionRespuesta == ""))
                        {
                            X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + laRespuesta.ResultadoOperacion + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", EjecutarComando + "  <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Respuesta Transaccional", "<br />  Codigo Respuesta: <b>" + laRespuesta.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + laRespuesta.Autorizacion + "</b> <br />" + "Mensaje: <b>" + laRespuesta.DescripcionRespuesta + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", EjecutarComando + "  <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }




        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                FormPanel1.Reset();
                
            }
            catch (Exception)
            {
            }

        }

        private void LimpiaCampos2()
        {
            txtCaja.Text = "";
            txtConsecutivo.Text = "";
            txtImporte.Text = "";
            //txtMedioAcceso.Text = "";
            txtObservaciones.Text = "";
            txtOperador.Text = "";
            txtReferencia.Text = "";
            txtSucursal.Text = "";
            cmbBanco.SelectedIndex = -1;
            cmbBanco.Text = "";
            //cmbOperacion.SelectedIndex = -1;
            //cmbOperacion.Text = "";
            datFecha.Text = "";
            datFecha.SelectedValue = -1;
            datHora.SelectedIndex = -1;
        }

    }
}