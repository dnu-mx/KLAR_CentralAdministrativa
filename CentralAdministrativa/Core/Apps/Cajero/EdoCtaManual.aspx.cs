using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCajero.Entidades;
using DALCajero.BaseDatos;
using System.Web.Security;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using DALCajero.LogicaNegocio;
using Interfases;
using System.Configuration;
using DALCajero.Utilidades;


namespace Cajero
{
    public partial class EdoCtaManual : DALCentralAplicaciones.PaginaBaseCAPP
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                    //if (!LNPermisos.EsPaginaPermitida((FormsIdentity)Context.User.Identity, Request))
                    //{
                    //    Response.Redirect("../AccesoRestringido.aspx");
                    //}

                    ////Obtiene el Objeto Usuario para los datos necesarios de la Aplicacion.
                    //if ( == null)
                    //{
                    //    Usuario elUsuario = DALCentralAplicaciones.BaseDatos.DAOUsuario.ObtieneCaracteristicasUsuario(Context.User.Identity.Name);
                    //    HttpContext.Current.Session.Add("usuario", elUsuario);
                    //}



                    PreparaGrid();

                    StoreBanco.DataSource = DAOCatalogos.ListaBancos();
                    StoreBanco.DataBind();

                    StoreOper.DataSource = DAOCatalogos.ListaTiposOperacion();
                    StoreOper.DataBind();

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

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            

        }

        protected void Seleccionar(object sender, DirectEventArgs e)
        {

          
        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 IdMov = Int64.Parse(e.ExtraParams["ID_Movimiento"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];
                Guid AppID=Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

                if (IdMov == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;

                Movimiento elMovimiento = DAOMovimiento.ConsultaMovimiento(IdMov, elUser, AppID);

                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();
                RespuestaTransaccional laRespuesta = new RespuestaTransaccional();

                switch (EjecutarComando)
                {
                    case "Eliminar":
                       laRespuesta= LNMovimiento.EliminarMovimiento(elMovimiento, elUser);
                        break;
                    case "Asignar":
                        laRespuesta = LNAsignacion.AsignarMovimientoAFichaDeposito(elMovimiento, elUser, Interfases.enumTipoAsignacion.AlPresionarGridMovimiento, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        break;
                    case "Activar":
                        laRespuesta = LNMovimiento.ActivarMovimiento(elMovimiento, elUser, AppID);
                        break;
                    case "Resguardar":
                        laRespuesta = LNMovimiento.ResguardarMovimiento(elMovimiento, elUser, AppID);
                        break;
                    case "QuitarResguardo":
                        laRespuesta = LNMovimiento.QuitarResguardoMovimiento(elMovimiento, elUser, AppID);
                        break;
                }

                PreparaGrid();
                BindData();

                DespliegaRespuesta(laRespuesta, EjecutarComando);
               
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos Bancarios", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Movimientos Bancarios", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            PreparaGrid();
            this.BindData();
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

        private void AgregaRecordFiles()
        {
            Store1.AddField(new RecordField("ID_Movimiento"));
            Store1.AddField(new RecordField("ID_EstatusMovimiento"));
            Store1.AddField(new RecordField("Descripcion"));
            Store1.AddField(new RecordField("ClaveSucursalBancaria"));
            Store1.AddField(new RecordField("ClaveCajaBancaria"));
            Store1.AddField(new RecordField("ClaveOperador"));
            Store1.AddField(new RecordField("FechaOperacion", RecordFieldType.Date));
            Store1.AddField(new RecordField("ConsecutivoBancario"));
            Store1.AddField(new RecordField("Referencia"));
            Store1.AddField(new RecordField("EstatusDescripcion"));
            Store1.AddField(new RecordField("Importe", RecordFieldType.Float));
            Store1.AddField(new RecordField("FechaRegistro", RecordFieldType.Date));
        }

        protected void PreparaGrid()
        {
            ////LIMPIA GRID

            ////PREPARAR CONEXION A DATOS

            AgregaRecordFiles();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Movimiento = new Column();
            colID_Movimiento.DataIndex = "ID_Movimiento";
            colID_Movimiento.Header = "ID ";
            colID_Movimiento.Width = 60;
            colID_Movimiento.Sortable = true;
            lasColumnas.Columns.Add(colID_Movimiento);

            Column ID_EstatusMovimiento = new Column();
            ID_EstatusMovimiento.DataIndex = "ID_EstatusMovimiento";
            ID_EstatusMovimiento.Header = "ID ";
            ID_EstatusMovimiento.Width = 60;
            ID_EstatusMovimiento.Sortable = true;
            ID_EstatusMovimiento.Hidden = true;
            lasColumnas.Columns.Add(ID_EstatusMovimiento);

            Column colEstatus = new Column();
            colEstatus.DataIndex = "EstatusDescripcion";
            colEstatus.Header = "Estado";
            colEstatus.Width = 100;
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

            Column colConsecutivo = new Column();
            colConsecutivo.DataIndex = "ConsecutivoBancario";
            colConsecutivo.Header = "Consecutivo";
            colConsecutivo.Sortable = true;
            lasColumnas.Columns.Add(colConsecutivo);

            Column colClaveSucursalBancaria = new Column();
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

            Column colReferencia = new Column();
            colReferencia.DataIndex = "Referencia";
            colReferencia.Header = "Referencia";
            colReferencia.Sortable = true;
            lasColumnas.Columns.Add(colReferencia);

            //Column colAfiliacion = new Column();
            //colAfiliacion.DataIndex = "Afiliacion";
            //colAfiliacion.Header = "Afiliacion";
            //colAfiliacion.Width = 130;
            //colAfiliacion.Sortable = true;
            //lasColumnas.Columns.Add(colAfiliacion);

            Column colImporte = new Column();
            colImporte.DataIndex = "Importe";
            colImporte.Header = "Importe";
            colImporte.Sortable = true;
            colImporte.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colImporte);



            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 100;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand Activar = new GridCommand();
            Activar.Icon = Icon.Accept;
            Activar.CommandName = "Activar";
            Activar.ToolTip.Text = "Activar Transaccionalmente el Movimiento";
            acciones.Commands.Add(Activar);

            CommandSeparator separa = new CommandSeparator();
            acciones.Commands.Add(separa);

            
            GridCommand Reintenta = new GridCommand();
            Reintenta.Icon = Icon.EmailStart;
            Reintenta.CommandName = "Asignar";
            Reintenta.ToolTip.Text = "Buscar Ficha de Deposito y Asignar";
            acciones.Commands.Add(Reintenta);


            CommandSeparator separa2 = new CommandSeparator();
            acciones.Commands.Add(separa2);


            GridCommand SacarResguardo = new GridCommand();
            SacarResguardo.Icon = Icon.LockKey;
            SacarResguardo.CommandName = "QuitarResguardo";
            SacarResguardo.ToolTip.Text = "Quitar de Resguardo para Asignacion";
            acciones.Commands.Add(SacarResguardo);
            
            acciones.Commands.Add(separa);

            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.Delete;
            elimina.CommandName = "Eliminar";
            elimina.ToolTip.Text = "Eliminar Movimiento";
            acciones.Commands.Add(elimina);
            
            CommandSeparator separa5 = new CommandSeparator();
            acciones.Commands.Add(separa5);

            GridCommand Resguardar = new GridCommand();
            Resguardar.Icon = Icon.LockAdd;
            Resguardar.CommandName = "Resguardar";
            Resguardar.ToolTip.Text = "Movimiento Activo a Resguardo";
            acciones.Commands.Add(Resguardar);

          
          



            ////AGREGAR COLUMNAS
            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colEstatus);
            GridPanel1.ColumnModel.Columns.Add(colID_Movimiento);
            GridPanel1.ColumnModel.Columns.Add(colFechaRegistro);
            GridPanel1.ColumnModel.Columns.Add(colClaveColectivaBanco);
            GridPanel1.ColumnModel.Columns.Add(colConsecutivo);
            GridPanel1.ColumnModel.Columns.Add(colClaveSucursalBancaria);
            GridPanel1.ColumnModel.Columns.Add(colFechaOperacion);
            GridPanel1.ColumnModel.Columns.Add(colReferencia);
            GridPanel1.ColumnModel.Columns.Add(colImporte);

            //AGREGAR EVENTOS
            //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Movimiento", "this.getRowsValues({ selectedOnly: true })[0].ID_FichaDeposito", ParameterMode.Raw));
            //GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Movimiento", "#{GridPanel1}.getSelectionModel().hasSelection() ?#{GridPanel1}.getRowsValues({selectedOnly:true})[0].ID_Movimiento : '0'", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Movimiento", "record.data.ID_Movimiento", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            ////PLUGINS DE FILTRADO
            GridFilters losFiltros = new GridFilters();


            NumericFilter filID_Movimiento = new NumericFilter();
            filID_Movimiento.DataIndex = "ID_Movimiento";
            losFiltros.Filters.Add(filID_Movimiento);

            StringFilter filEstatus = new StringFilter();
            filEstatus.DataIndex = "EstatusDescripcion";
            losFiltros.Filters.Add(filEstatus);

            NumericFilter filID_EstatusMovimiento = new NumericFilter();
            filID_EstatusMovimiento.DataIndex = "ID_EstatusMovimiento";
            losFiltros.Filters.Add(filID_EstatusMovimiento);

            StringFilter filClaveColectivaBanco = new StringFilter();
            filClaveColectivaBanco.DataIndex = "Descripcion";
            losFiltros.Filters.Add(filClaveColectivaBanco);

            StringFilter filClaveSucursalBancaria = new StringFilter();
            filClaveSucursalBancaria.DataIndex = "ClaveSucursalBancaria";
            losFiltros.Filters.Add(filClaveSucursalBancaria);

            StringFilter filClaveCajaBancaria = new StringFilter();
            filClaveCajaBancaria.DataIndex = "ClaveCajaBancaria";
            losFiltros.Filters.Add(filClaveCajaBancaria);

            DateFilter filFechaOperacion = new DateFilter();
            filFechaOperacion.DataIndex = "FechaOperacion";
            filFechaOperacion.OnText = "Con Fecha";
            filFechaOperacion.BeforeText = "Desde";
            filFechaOperacion.AfterText = "Hasta";
            filFechaOperacion.DatePickerOptions.TodayText = "Hoy";
            losFiltros.Filters.Add(filFechaOperacion);


            NumericFilter filConsecutivo = new NumericFilter();
            filConsecutivo.DataIndex = "Consecutivo";
            losFiltros.Filters.Add(filConsecutivo);

            StringFilter filClaveReferencia = new StringFilter();
            filClaveReferencia.DataIndex = "Referencia";
            losFiltros.Filters.Add(filClaveReferencia);

            NumericFilter filImporte = new NumericFilter();
            filImporte.DataIndex = "Importe";
            losFiltros.Filters.Add(filImporte);


            DateFilter filFechaRegistro = new DateFilter();
            filFechaRegistro.OnText = "Con Fecha";
            filFechaRegistro.BeforeText = "Desde";
            filFechaRegistro.AfterText = "Hasta";
            filFechaRegistro.DataIndex = "FechaRegistro";
            filFechaRegistro.DatePickerOptions.TodayText = "Hoy";

            losFiltros.Filters.Add(filFechaRegistro);

            GridPanel1.Plugins.Add(losFiltros);




        }

        private void BindData()
        {
            //var store = this.GridPanel1.GetStore();
            GridPanel1.GetStore().DataSource = DAOMovimiento.ListaMovimientos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
           
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                //Obtiene el usuario en linea
                Usuario elUser = this.Usuario;
                Movimiento NuevoMovimiento = new Movimiento();
                Guid AppID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

                Int64 Conse;
                float monto;


                Int64.TryParse(txtConsecutivo.Text, out Conse);
                float.TryParse(txtImporte.Text, out monto);

                if (Conse == 0)
                {
                    throw new CAppException(8009, "El Valor <b>'" + txtConsecutivo.Text + "'</b> no es Valido en el Campo <b>Consecutivo</b>");
                }
                else if (monto == 0)
                {
                    throw new CAppException(8009, "El Valor <b>'" + txtImporte.Text + "'</b> no un Importe Valido");
                }

                try
                {
                    NuevoMovimiento = new Movimiento
                    {

                        ClaveUsuarioRegistro = elUser.ClaveUsuario,
                        ClaveColectivaBanco = cmbBanco.Value.ToString(),
                        elTipoRegistro = new TipoRegistro(0, "001", "MANUAL"),
                        //elTipoOperacion = new TipoOperacionTransaccional(0, "640000", ""),//TODO: Poner el PC en Configuracion a nivel Aplicativo
                        ClaveSucursalBancaria = txtSucursal.Text,
                        ClaveCajaBancaria = "",
                        ClaveOperador = "",
                        FechaOperacion = (DateTime)datFecha.Value,
                        FechaValor = (DateTime)datFechaValor.Value,
                        NumeroCheque = txtCheque.Text,
                        ConsecutivoBancario = Int64.Parse(txtConsecutivo.Text),
                        Referencia = "",
                        Importe = float.Parse(txtImporte.Text),
                        Observaciones = txtobservaciones.Text,
                        elEstatus = enumEstatusMovimiento.EnProcesoActivar
                    };
                }
                catch (Exception err)
                {
                    Loguear.Error(err, "");
                    throw new CAppException(8007, "No se puede crear un Movimiento Bancario con los datos proporcionados.", err);
                }


               RespuestaTransaccional laRespuesta= LNMovimiento.AgregarMovimiento(NuevoMovimiento, elUser, AppID);

                DespliegaRespuesta(laRespuesta, "Guardar Movimiento");

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Guardar Movimiento Bancario", "Ocurrió un Imprevisto:<br/>" + err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                X.Msg.Alert("Guardar Movimiento Bancario", "Datos Proporcionados son inválidos, intenta nuevamente: <br/>" + err.Message).Show();
            }
            finally
            {
                //LimpiaCampos();
                PreparaGrid();
                BindData();
                FormPanel1.Reset();
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            

        }

        private void LimpiaCampos()
        {
            txtCaja.Text = "";
            txtConsecutivo.Text = "";
            txtImporte.Text = "";
            //txtMedioAcceso.Text = "";
            txtobservaciones.Text = "";
            txtOperador.Text = "";
            txtReferencia.Text = "";
            txtSucursal.Text = "";
            cmbBanco.SelectedIndex = -1;
            cmbBanco.Text = "";
            datFecha.Text = "";
            datHora.SelectedIndex = -1;
            datFecha.SelectedValue = -1;
            datFechaValor.SelectedValue = -1;
            datHora.SelectedIndex = -1;
            txtCheque.Text = "";
                 
        }

    }
}