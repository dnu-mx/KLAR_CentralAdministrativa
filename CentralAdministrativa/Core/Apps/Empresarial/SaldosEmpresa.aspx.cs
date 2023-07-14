using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using System.Globalization;
using DALCentralAplicaciones.Utilidades;
using System.Configuration;

namespace Empresarial
{
    public partial class Saldos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               

                if (!IsPostBack)
                {
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

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        //Valida que el Usuario tenga permisos de Ver la pagina
        //        if (!LNPermisos.EsPaginaPermitida((FormsIdentity)Context.User.Identity, Request))
        //        {
        //            Response.Redirect("../AccesoRestringido.aspx");
        //        }

        //        //Obtiene el Objeto Usuario para los datos necesarios de la Aplicacion.
        //        if ( == null)
        //        {
        //            Usuario elUsuario = DALCentralAplicaciones.BaseDatos.DAOUsuario.ObtieneCaracteristicasUsuario(Context.User.Identity.Name);
        //            HttpContext.Current.Session.Add("usuario", elUsuario);
        //        }
        //    }

        //    if (!IsPostBack)
        //    {
        //        PreparaGrid();

        //    }

        //    if (!X.IsAjaxRequest)
        //    {
        //        this.BindData();
        //    }

        //}

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;

            
        }

        private void AgregaRecordFiles()
        {
            Store1.AddField(new RecordField("ID_Cuenta"));
            Store1.AddField(new RecordField("CuentaHabiente"));
            Store1.AddField(new RecordField("ClaveTipoCuenta"));
            Store1.AddField(new RecordField("DescripcionCuenta"));
            Store1.AddField(new RecordField("ClaveGrupoCuenta"));
            Store1.AddField(new RecordField("Descripcion"));
            Store1.AddField(new RecordField("ID_CuentaCorriente"));
            Store1.AddField(new RecordField("ID_CuentaLimiteCredito"));
            Store1.AddField(new RecordField("LimiteCredito"));
            Store1.AddField(new RecordField("Consumos"));
            Store1.AddField(new RecordField("SaldoDisponible"));
            Store1.AddField(new RecordField("CodigoMoneda"));
            Store1.AddField(new RecordField("DescripcionMoneda"));
            Store1.AddField(new RecordField("BreveDescripcion"));
            
        }


        protected void PreparaGrid()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFiles();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            GroupingSummaryColumn colID_Cuenta = new GroupingSummaryColumn();
            colID_Cuenta.DataIndex = "ID_Cuenta";
            colID_Cuenta.Header = "ID Cuenta";
            colID_Cuenta.Width = 50;
            colID_Cuenta.Sortable = true;
            colID_Cuenta.Hidden = true;
            lasColumnas.Columns.Add(colID_Cuenta);

            Column colCuentaHabiente = new Column();
            colCuentaHabiente.DataIndex = "CuentaHabiente";
            colCuentaHabiente.Header = "Cuenta Habiente";
            colCuentaHabiente.Hidden = true;
            colCuentaHabiente.Width = 50;
            colCuentaHabiente.Sortable = true;
            lasColumnas.Columns.Add(colCuentaHabiente);

            GroupingSummaryColumn colDescripcionCuenta = new GroupingSummaryColumn();
            colDescripcionCuenta.DataIndex = "DescripcionCuenta";
            colDescripcionCuenta.Header = "Tipo Cuenta";
            colDescripcionCuenta.Width = 120;
            colDescripcionCuenta.Sortable = true;
            lasColumnas.Columns.Add(colDescripcionCuenta);

            GroupingSummaryColumn colLimiteCredito = new GroupingSummaryColumn();
            colLimiteCredito.DataIndex = "LimiteCredito";
            colLimiteCredito.Header = "Limite de Credito";
            colLimiteCredito.Sortable = true;
            colLimiteCredito.Width = 100;
            colLimiteCredito.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colLimiteCredito);

            GroupingSummaryColumn colSaldoActual = new GroupingSummaryColumn();
            colSaldoActual.DataIndex = "Consumos";
            colSaldoActual.Header = "Consumos";
            colSaldoActual.Sortable = true;
            colSaldoActual.Width = 100;
            colSaldoActual.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colSaldoActual);

            GroupingSummaryColumn colSaldoDisponible = new GroupingSummaryColumn();
            colSaldoDisponible.DataIndex = "SaldoDisponible";
            colSaldoDisponible.Header = "Saldo Disponible";
            colSaldoDisponible.Sortable = true;
            colSaldoDisponible.Width = 100;
            colSaldoDisponible.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colSaldoDisponible);

            GroupingSummaryColumn colCodigoMoneda = new GroupingSummaryColumn();
            colCodigoMoneda.DataIndex = "CodigoMoneda";
            colCodigoMoneda.Header = "Moneda";
            colCodigoMoneda.Width = 60;
            colCodigoMoneda.Sortable = true;
            lasColumnas.Columns.Add(colCodigoMoneda);

            GroupingSummaryColumn colDescripcionMoneda = new GroupingSummaryColumn();
            colDescripcionMoneda.DataIndex = "DescripcionMoneda";
            colDescripcionMoneda.Header = "Moneda";
            colDescripcionMoneda.Width = 60;
            colDescripcionMoneda.Sortable = true;
            lasColumnas.Columns.Add(colDescripcionMoneda);

            GroupingSummaryColumn ColBreveDescripcion = new GroupingSummaryColumn();
            ColBreveDescripcion.DataIndex = "BreveDescripcion";
            ColBreveDescripcion.Header = "Breve Descripción";
            ColBreveDescripcion.Width = 120;
            ColBreveDescripcion.Sortable = true;
            lasColumnas.Columns.Add(ColBreveDescripcion);


            //ImageCommandColumn acciones = new ImageCommandColumn();
            //acciones.Header = "Acciones";
            //acciones.Width = 55;
            //ImageCommand edit = new ImageCommand();
            //edit.Icon = Icon.NoteEdit;
            //edit.CommandName = "Consultar";
            //edit.ToolTip.Text = "Consultar Saldo";
            //acciones.Commands.Add(edit);

        

            //AGREGAR COLUMNAS
            //GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colID_Cuenta);

            GridPanel1.ColumnModel.Columns.Add(colCuentaHabiente);
            GridPanel1.ColumnModel.Columns.Add(ColBreveDescripcion);
            GridPanel1.ColumnModel.Columns.Add(colDescripcionCuenta);
            GridPanel1.ColumnModel.Columns.Add(colLimiteCredito);
            GridPanel1.ColumnModel.Columns.Add(colSaldoActual);
            GridPanel1.ColumnModel.Columns.Add(colSaldoDisponible);
            GridPanel1.ColumnModel.Columns.Add(colCodigoMoneda);
            GridPanel1.ColumnModel.Columns.Add(colDescripcionMoneda);
            


            //AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Cuenta", "this.getRowsValues({ selectedOnly: true })[0].ID_Cuenta", ParameterMode.Raw));
            

            ////PLUGINS DE FILTRADO
            //GridFilters losFiltros = new GridFilters();


            //NumericFilter filID_FichaDeposito = new NumericFilter();
            //filID_FichaDeposito.DataIndex = "ID_Cuenta";
            //losFiltros.Filters.Add(filID_FichaDeposito);

            //StringFilter filEstatus = new StringFilter();
            //filEstatus.DataIndex = "EstatusDescripcion";
            //losFiltros.Filters.Add(filEstatus);

            //NumericFilter filID_EstatusFichaDeposito = new NumericFilter();
            //filID_EstatusFichaDeposito.DataIndex = "ID_EstatusFichaDeposito";
            //losFiltros.Filters.Add(filID_EstatusFichaDeposito);

            //StringFilter filClaveColectivaBanco = new StringFilter();
            //filClaveColectivaBanco.DataIndex = "Descripcion";
            //losFiltros.Filters.Add(filClaveColectivaBanco);

            //StringFilter filClaveSucursalBancaria = new StringFilter();
            //filClaveSucursalBancaria.DataIndex = "ClaveSucursalBancaria";
            //losFiltros.Filters.Add(filClaveSucursalBancaria);

            //StringFilter filClaveCajaBancaria = new StringFilter();
            //filClaveCajaBancaria.DataIndex = "ClaveCajaBancaria";
            //losFiltros.Filters.Add(filClaveCajaBancaria);

            //StringFilter filClaveOperador = new StringFilter();
            //filClaveOperador.DataIndex = "ClaveOperador";
            //losFiltros.Filters.Add(filClaveOperador);

       
            //NumericFilter filConsecutivo = new NumericFilter();
            //filConsecutivo.DataIndex = "Consecutivo";
            //losFiltros.Filters.Add(filConsecutivo);

            //StringFilter filClaveReferencia = new StringFilter();
            //filClaveReferencia.DataIndex = "Referencia";
            //losFiltros.Filters.Add(filClaveReferencia);

            //NumericFilter filImporte = new NumericFilter();
            //filImporte.DataIndex = "Importe";
            //losFiltros.Filters.Add(filImporte);

            //StringFilter filClaveTipoMA = new StringFilter();
            //filClaveTipoMA.DataIndex = "ClaveTipoMA";
            //losFiltros.Filters.Add(filClaveTipoMA);

            //StringFilter filClaveMedioAcceso = new StringFilter();
            //filClaveMedioAcceso.DataIndex = "ClaveMedioAcceso";
            //losFiltros.Filters.Add(filClaveMedioAcceso);

            //StringFilter filAfiliacion = new StringFilter();
            //filAfiliacion.DataIndex = "Afiliacion";
            //losFiltros.Filters.Add(filAfiliacion);

            //losFiltros.Filters.Add(filFechaRegistro);

            //GridPanel1.Plugins.Add(losFiltros);




        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            PreparaGrid();
            this.BindData();

            // AgregaRecordFiles();

        }

        private void BindData()
        {

            Guid IDApp = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            GridPanel1.GetStore().DataSource = DAOColectiva.ListaSaldosColectivaIDTipoColectiva(-1, Configuracion.Get(IDApp, "ClaveColectivaEmpresarial").Valor, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();


        }

        protected void Seleccionar(object sender, DirectEventArgs e)
        {
            try
            {
                String unvalor = e.ExtraParams["ID_Cuenta"];

                Int64 IdFicha = Int64.Parse(e.ExtraParams["ID_Cuenta"]);

                //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

                //DAOFichaDeposito.ConsultaFichaDeposito(e.
            }
            catch (Exception )
            {

            }
        }


        protected void DespliegaCuenta(Cuenta laCuenta)
        {
            txtcodigoMoneda.Text = laCuenta.CodDivisa;
            txtCuentahabiete.Text = laCuenta.CuentaHabiente;
            txtDescripcion.Text = laCuenta.DesTipoCuenta;
            txtGrupoCuenta.Text = laCuenta.DesGrupoCuenta;
            txtEstatus.Text =  laCuenta.DescEstatus;
            txtSaldo.Text = String.Format("{0:C}", laCuenta.SaldoActual);
            txtSaldoDisponible.Text = String.Format("{0:C}", laCuenta.SaldoDisponible);
            txtTipoColectiva.Text = laCuenta.desTipoColectiva;
            txtCtaISO.Text = laCuenta.DesTipoCuentaISO;


        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            Panel1.Collapsed = true;
            GridPanel1.GetStore().RemoveAll();
        }



        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {
                Cuenta unaCta = new Cuenta();

                String unvalor = e.ExtraParams["ID_Cuenta"];

                Panel1.Title = "Cuenta No. [" + unvalor + "]";

                Int64 ID_Cta = Int64.Parse(unvalor);

                unaCta = LNCuenta.ObtieneCuenta(ID_Cta, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                DespliegaCuenta(unaCta);

            }
            catch (Exception )
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            Panel1.Collapsed = false;
        }

    }
}