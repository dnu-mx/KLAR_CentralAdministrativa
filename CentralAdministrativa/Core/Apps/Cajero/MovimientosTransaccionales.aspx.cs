using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALCajero.BaseDatos;
using DALCajero.Entidades;
using DALCentralAplicaciones.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using DALCajero.Utilidades;

namespace Cajero
{
    public partial class MovimientosTransaccionales : DALCentralAplicaciones.PaginaBaseCAPP
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
            PreparaGrid();
            this.BindData();

            //// AgregaRecordFiles();

        }

        private void AgregaRecordFiles()
        {

            Store1.AddField(new RecordField("ID_Operacion"));
            Store1.AddField(new RecordField("ID_ColectivaEmisor"));
            Store1.AddField(new RecordField("ID_ColectivaCadComercial"));
            Store1.AddField(new RecordField("ID_ColectivaGrupoComercial"));
            Store1.AddField(new RecordField("CveCiaServicios"));
            Store1.AddField(new RecordField("ID_Estatus"));
            Store1.AddField(new RecordField("SKUTAE"));
            Store1.AddField(new RecordField("TipoMA"));
            Store1.AddField(new RecordField("ClaveMA"));
            Store1.AddField(new RecordField("ReferenciaPagoServicio"));
            Store1.AddField(new RecordField("CodigoProceso"));
            Store1.AddField(new RecordField("FechaRegistro", RecordFieldType.Date));
            Store1.AddField(new RecordField("FechaOperacion", RecordFieldType.Date));
            Store1.AddField(new RecordField("CodigoMoneda"));
            Store1.AddField(new RecordField("Importe", RecordFieldType.Float));
            Store1.AddField(new RecordField("NumeroAuditoria"));
            Store1.AddField(new RecordField("Sucursal"));
            Store1.AddField(new RecordField("Afiliacion"));
            Store1.AddField(new RecordField("Terminal"));
            Store1.AddField(new RecordField("Ticket"));
            Store1.AddField(new RecordField("Operador"));
            Store1.AddField(new RecordField("Cod_Respuesta"));
            Store1.AddField(new RecordField("Autorizacion"));
            Store1.AddField(new RecordField("DBUser"));
        }

        protected void PreparaGrid()
        {
            ////LIMPIA GRID

            ////PREPARAR CONEXION A DATOS

            AgregaRecordFiles();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colAfiliacion = new Column();
            colAfiliacion.DataIndex = "Afiliacion";
            colAfiliacion.Header = "Numero Afiliacion";
            colAfiliacion.Sortable = true;
            lasColumnas.Columns.Add(colAfiliacion);
            GridPanel1.ColumnModel.Columns.Add(colAfiliacion);

            Column colOperador = new Column();
            colOperador.DataIndex = "Operador";
            colOperador.Header = "Usuario";
            colOperador.Sortable = true;
            lasColumnas.Columns.Add(colOperador);
            GridPanel1.ColumnModel.Columns.Add(colOperador);

            Column colCodigoProceso = new Column();
            colCodigoProceso.DataIndex = "CodigoProceso";
            colCodigoProceso.Header = "Tipo Operacion";
            colCodigoProceso.Sortable = true;
            lasColumnas.Columns.Add(colCodigoProceso);
            GridPanel1.ColumnModel.Columns.Add(colCodigoProceso);

            Column colFechaRegistro = new Column();
            colFechaRegistro.DataIndex = "FechaRegistro";
            colFechaRegistro.Header = "Fecha Registro";
            colFechaRegistro.Sortable = true;
            lasColumnas.Columns.Add(colFechaRegistro);
            GridPanel1.ColumnModel.Columns.Add(colFechaRegistro);

            Column colFechaOperacion = new Column();
            colFechaOperacion.DataIndex = "FechaOperacion";
            colFechaOperacion.Header = "Fecha de Operacion";
            colFechaOperacion.Sortable = true;
            lasColumnas.Columns.Add(colFechaOperacion);
            GridPanel1.ColumnModel.Columns.Add(colFechaOperacion);

            Column colCodigoMoneda = new Column();
            colCodigoMoneda.DataIndex = "CodigoMoneda";
            colCodigoMoneda.Header = "Moneda";
            colCodigoMoneda.Sortable = true;
            lasColumnas.Columns.Add(colCodigoMoneda);
            GridPanel1.ColumnModel.Columns.Add(colCodigoMoneda);

            Column colImporte = new Column();
            colImporte.DataIndex = "Importe";
            colImporte.Header = "Importe";
            colImporte.Sortable = true;
            lasColumnas.Columns.Add(colImporte);
            GridPanel1.ColumnModel.Columns.Add(colImporte);

            Column colCod_Respuesta = new Column();
            colCod_Respuesta.DataIndex = "Cod_Respuesta";
            colCod_Respuesta.Header = "Codigo Respuesta";
            colCod_Respuesta.Sortable = true;
            lasColumnas.Columns.Add(colCod_Respuesta);
            GridPanel1.ColumnModel.Columns.Add(colCod_Respuesta);

            Column colAutorizacion = new Column();
            colAutorizacion.DataIndex = "Autorizacion";
            colAutorizacion.Header = "Autorizacion";
            colAutorizacion.Sortable = true;
            lasColumnas.Columns.Add(colAutorizacion);



            //////AGREGAR COLUMNAS
            //GridPanel1.ColumnModel.Columns.Add(acciones);
            //GridPanel1.ColumnModel.Columns.Add(colEstatus);
            //GridPanel1.ColumnModel.Columns.Add(colID_Movimiento);
            //GridPanel1.ColumnModel.Columns.Add(colFechaRegistro);
            //GridPanel1.ColumnModel.Columns.Add(colClaveColectivaBanco);
            //GridPanel1.ColumnModel.Columns.Add(colConsecutivo);
            //GridPanel1.ColumnModel.Columns.Add(colClaveSucursalBancaria);
            //GridPanel1.ColumnModel.Columns.Add(colFechaOperacion);
            //GridPanel1.ColumnModel.Columns.Add(colReferencia);
            //GridPanel1.ColumnModel.Columns.Add(colImporte);

            ////AGREGAR EVENTOS
            //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Movimiento", "this.getRowsValues({ selectedOnly: true })[0].ID_FichaDeposito", ParameterMode.Raw));

            //////PLUGINS DE FILTRADO
            //GridFilters losFiltros = new GridFilters();


            //NumericFilter filID_Movimiento = new NumericFilter();
            //filID_Movimiento.DataIndex = "ID_Movimiento";
            //losFiltros.Filters.Add(filID_Movimiento);

            //StringFilter filEstatus = new StringFilter();
            //filEstatus.DataIndex = "EstatusDescripcion";
            //losFiltros.Filters.Add(filEstatus);

            //NumericFilter filID_EstatusMovimiento = new NumericFilter();
            //filID_EstatusMovimiento.DataIndex = "ID_EstatusMovimiento";
            //losFiltros.Filters.Add(filID_EstatusMovimiento);

            //StringFilter filClaveColectivaBanco = new StringFilter();
            //filClaveColectivaBanco.DataIndex = "Descripcion";
            //losFiltros.Filters.Add(filClaveColectivaBanco);

            //StringFilter filClaveSucursalBancaria = new StringFilter();
            //filClaveSucursalBancaria.DataIndex = "ClaveSucursalBancaria";
            //losFiltros.Filters.Add(filClaveSucursalBancaria);

            //StringFilter filClaveCajaBancaria = new StringFilter();
            //filClaveCajaBancaria.DataIndex = "ClaveCajaBancaria";
            //losFiltros.Filters.Add(filClaveCajaBancaria);

            //DateFilter filFechaOperacion = new DateFilter();
            //filFechaOperacion.DataIndex = "FechaOperacion";
            //filFechaOperacion.OnText = "Con Fecha";
            //filFechaOperacion.BeforeText = "Desde";
            //filFechaOperacion.AfterText = "Hasta";
            //filFechaOperacion.DatePickerOptions.TodayText = "Hoy";
            //losFiltros.Filters.Add(filFechaOperacion);


            //NumericFilter filConsecutivo = new NumericFilter();
            //filConsecutivo.DataIndex = "Consecutivo";
            //losFiltros.Filters.Add(filConsecutivo);

            //StringFilter filClaveReferencia = new StringFilter();
            //filClaveReferencia.DataIndex = "Referencia";
            //losFiltros.Filters.Add(filClaveReferencia);

            //NumericFilter filImporte = new NumericFilter();
            //filImporte.DataIndex = "Importe";
            //losFiltros.Filters.Add(filImporte);


            //DateFilter filFechaRegistro = new DateFilter();
            //filFechaRegistro.OnText = "Con Fecha";
            //filFechaRegistro.BeforeText = "Desde";
            //filFechaRegistro.AfterText = "Hasta";
            //filFechaRegistro.DataIndex = "FechaRegistro";
            //filFechaRegistro.DatePickerOptions.TodayText = "Hoy";

            //losFiltros.Filters.Add(filFechaRegistro);

            //GridPanel1.Plugins.Add(losFiltros);




        }

        protected void Store1_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            int start = int.Parse(e.Parameters["startRemote"]);
            int limit = int.Parse(e.Parameters["limitRemote"]);

            List<object> data = new List<object>(limit);

            for (int i = start; i < start + limit; i++)
            {
                data.Add(new { field = "Value" + (i + 1) });
            }

            e.Total = 8000;

            var store = this.GridPanel1.GetStore();
            store.DataSource = data;
            store.DataBind();
        }


        private void BindData()
        {
            //var store = this.GridPanel1.GetStore();
            GridPanel1.GetStore().DataSource = DAOOperaciones.ListaOperaciones();
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


                //DAOMovimiento.Insertar(nuevoMovimiento, new Usuario());

            }
            catch (Exception)
            {
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {


        }

       
    }
}