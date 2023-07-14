using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCajero.BaseDatos;
using DALCajero.Entidades;
using DALCajero.LogicaNegocio;
using Interfases.Exceptiones;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using System.Web.Security;
using Interfases;
using System.Configuration;
using DALCajero.Utilidades;

namespace Cajero
{
    public partial class BuscarMovimientos_EnResguardo : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    
                    //Valida que el Usuario tenga permisos de Ver la pagina
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

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //Int64 IdFicha = Int64.Parse(e.ExtraParams["ID_FichaDeposito"]);

        }

        protected void Seleccionar(object sender, DirectEventArgs e)
        {

            //String unvalor = e.ExtraParams["ID_FichaDeposito"];

            //Int64 IdFicha = Int64.Parse(e.ExtraParams["ID_FichaDeposito"]);

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new Usuario());

            ////DAOFichaDeposito.ConsultaFichaDeposito(e.
        }

        private void DespliegaValoresFichaDeposito(Int64 ID_FichaDeposito)
        {

        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                ActualizaDatos();
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

        protected void ActualizaDatos()
        {
            try
            {
                PreparaGrid();
                this.BindData();
            }
            catch
            {
                throw new CAppException(8009, "Ocurrio una Falla al Actualizar los Datos del Grid");
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
            Store1.AddField(new RecordField("DescripcionBanco"));
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

            Column colEstatus = new Column();
            colEstatus.DataIndex = "Descripcion";
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
            colClaveColectivaBanco.DataIndex = "DescripcionBanco";
            colClaveColectivaBanco.Header = "Banco";
            colClaveColectivaBanco.Width = 100;
            colClaveColectivaBanco.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectivaBanco);

            Column colConsecutivo = new Column();
            colConsecutivo.DataIndex = "ConsecutivoBancario";
            colConsecutivo.Header = "Consecutivo";
            colConsecutivo.Width = 100;
            colConsecutivo.Sortable = true;
            lasColumnas.Columns.Add(colConsecutivo);

            Column colClaveSucursalBancaria = new Column();
            colClaveSucursalBancaria.DataIndex = "ClaveSucursalBancaria";
            colClaveSucursalBancaria.Header = "Sucursal Banco";
            colClaveSucursalBancaria.Width = 150;
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
            colReferencia.Width = 150;
            colReferencia.Sortable = true;
            lasColumnas.Columns.Add(colReferencia);


            Column colImporte = new Column();
            colImporte.DataIndex = "Importe";
            colImporte.Header = "Importe";
            colImporte.Width = 100;
            colImporte.Sortable = true;
            colImporte.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colImporte);


            ImageCommandColumn acciones = new ImageCommandColumn();
            acciones.Header = "Acciones";
            acciones.Width = 55;
            ImageCommand edit = new ImageCommand();
            edit.Icon = Icon.LockKey;
            edit.CommandName = "AsignarFichaAMovimientoResguardo";
            edit.ToolTip.Text = "Asginar Movimiento de Resguardo a ID de Ficha";
            acciones.Commands.Add(edit);



            ////AGREGAR COLUMNAS
            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colEstatus);
            GridPanel1.ColumnModel.Columns.Add(colID_Movimiento);
            GridPanel1.ColumnModel.Columns.Add(colFechaRegistro);
            GridPanel1.ColumnModel.Columns.Add(colClaveColectivaBanco);
            GridPanel1.ColumnModel.Columns.Add(colConsecutivo);
            GridPanel1.ColumnModel.Columns.Add(colClaveSucursalBancaria);
            GridPanel1.ColumnModel.Columns.Add(colFechaOperacion);
            //GridPanel1.ColumnModel.Columns.Add(colReferencia);
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

            NumericFilter filConsecutivoBancario = new NumericFilter();
            filConsecutivoBancario.DataIndex = "ConsecutivoBancario";
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
            //GridPanel1.GetStore().DataSource = DAOMovimiento.ListaMovimientosResguardo();
            //GridPanel1.GetStore().DataBind();


        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 IdMov = Int64.Parse(e.ExtraParams["ID_Movimiento"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (IdMov == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;// this.Usuario;

                HttpContext.Current.Session.Add("ID_Movimiento", IdMov);

                //Obtener el Movimiento de la Base de datos
               // Movimiento elMovimiento = DAOMovimiento.ConsultaMovimiento(IdMov, elUser);

                switch (EjecutarComando)
                {
                    case "AsignarFichaAMovimientoResguardo":

                        X.Msg.Prompt("Asignación Forzada", "Inserta el Identificador de la Ficha de Depósito:", new JFunction { Fn = "fnConfirmar" }).Show();
                        break;
                }

                ActualizaDatos();

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

        [DirectMethod(Namespace = "Cajero")]
        public void ConfirmarOperacion(String Id_Ficha)
        {
            try
            {
                Int64 Id_Movimiento =(Int64)HttpContext.Current.Session.Contents["ID_Movimiento"];
                HttpContext.Current.Session.Add("ID_Ficha", Int64.Parse(Id_Ficha));

               // X.Msg.Alert("Asignación Forzada", "Se Asignarán el ID_Ficha: [" + Id_Ficha + "] con el ID_Movimiento: [" + Id_Movimiento + "]", new JFunction { Fn = "fnAsignar" }).Show();

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Asignación Forzada",
                    Message = "Se Asignarán el ID_Ficha: [" + Id_Ficha + "] con el ID_Movimiento: [" + Id_Movimiento + "] <br/><br/><b> ¿Autorizas la Asignación? </b>",
                    Buttons = MessageBox.Button.OKCANCEL,
                    Icon = MessageBox.Icon.QUESTION,
                    Fn = new JFunction { Fn = "fnAsignar" }
                });

              
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

        [DirectMethod(Namespace = "Cajero")]
        public void AsignarOperacion()
        {
            try
            {
                RespuestaTransaccional laRespuesta = new RespuestaTransaccional();
                Usuario elUser = this.Usuario;//  this.Usuario;

                Int64 Id_Movimiento = (Int64)HttpContext.Current.Session.Contents["ID_Movimiento"];
                Int64 Id_Ficha = (Int64)HttpContext.Current.Session.Contents["ID_Ficha"];

               laRespuesta= LNAsignacion.Asignar(Id_Movimiento, Id_Ficha, elUser, enumTipoAsignacion.OperadorSeleccionaAmbos, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (laRespuesta.IsAutorizada())
                {
                    X.Msg.Notify("Respuesta Transaccional", "Resultados de la Operación <br />  Codigo Respuesta: <b>" + laRespuesta.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + laRespuesta.Autorizacion + "</b> <br />").Show();
                    X.Msg.Notify("Respuesta Transaccional", "Asignación de Movimiento <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();

                }
                else
                {
                    if (laRespuesta.IsTimeOut)
                    {
                        X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + laRespuesta.ResultadoOperacion + "</b> <br />").Show();
                        X.Msg.Notify("Respuesta Transaccional", "Asignación de Movimiento  <br /><br />  <b> T I M E    O U T </b> <br />  <br /> ").Show();
                    }
                    else
                    {
                        if ((laRespuesta.DescripcionRespuesta == null) || (laRespuesta.DescripcionRespuesta == ""))
                        {
                            X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + laRespuesta.ResultadoOperacion + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", "Asignación de Movimiento   <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Respuesta Transaccional", "<br />  Codigo Respuesta: <b>" + laRespuesta.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + laRespuesta.Autorizacion + "</b> <br />" + "Mensaje: <b>" + laRespuesta.DescripcionRespuesta + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", "Asignación de Movimiento   <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                    }
                }

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

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Movimiento unPosibleMovimiento = new Movimiento();
                unPosibleMovimiento.ClaveColectivaBanco = cmbBanco.Value == null ? "" : (String)cmbBanco.Value;
                unPosibleMovimiento.ClaveSucursalBancaria = txtSucursal.Text == null ? "" : txtSucursal.Text;
                unPosibleMovimiento.ConsecutivoBancario = Int64.Parse(txtConsecutivo.Text == "" ? "0" : txtConsecutivo.Text);
                unPosibleMovimiento.FechaOperacion = (DateTime)datFecha.Value == null ? DateTime.Now : (DateTime)datFecha.Value; 
                unPosibleMovimiento.Importe = float.Parse(txtImporte.Text == "" ? "0" : txtImporte.Text);

                PreparaGrid();
                GridPanel1.GetStore().DataSource = LNMovimiento.BuscarMovimiento(unPosibleMovimiento, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridPanel1.GetStore().DataBind();

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Buscando Movimientos Bancarios", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Buscando Movimientos Bancarios", "Ocurrio un Error al Ejecutar la Búsqueda con los datos Proporcionados").Show();
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {


        }

    }
}