using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using Ext.Net;
using DALCentralAplicaciones.LogicaNegocio;
using DALAutorizador.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Interfases;
using System.Configuration;
using DALCentralAplicaciones.Utilidades;
using Log_PCI.Entidades;

namespace Empresarial
{
    public partial class SaldosSucursal : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                  

                    PreparaGridEmpleados();
                    PreparaGridSaldos();
                }

                if (!X.IsAjaxRequest)
                {
                    this.BindDataEmpleados();
                    //this.BindDataSaldos(0,"");
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        private void AgregaRecordFilesCuentas()
        {
            Store1.AddField(new RecordField("ID_Cuenta"));
            Store1.AddField(new RecordField("ID_Colectiva"));
            Store1.AddField(new RecordField("CuentaHabiente"));
            Store1.AddField(new RecordField("ID_EstatusCuenta"));
            Store1.AddField(new RecordField("DescEstatus"));
            Store1.AddField(new RecordField("IconoEstatus"));
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
            Store1.AddField(new RecordField("CodTipoCuentaISO"));
            Store1.AddField(new RecordField("BreveDescripcion"));
            Store1.AddField(new RecordField("EditarSaldoGrid"));
            Store1.AddField(new RecordField("Afiliacion"));
            Store1.AddField(new RecordField("CadenaComercial"));
            Store1.AddField(new RecordField("ID_CCM"));

        }

        private void AgregaRecordFilesEmpleados()
        {
            Store2.AddField(new RecordField("ID_Colectiva"));
            Store2.AddField(new RecordField("ClaveColectiva"));
            Store2.AddField(new RecordField("NombreORazonSocial"));
            Store2.AddField(new RecordField("APaterno"));
            Store2.AddField(new RecordField("AMaterno"));
            Store2.AddField(new RecordField("RFC"));
            Store2.AddField(new RecordField("Email"));
            Store2.AddField(new RecordField("Estatus"));
            Store2.AddField(new RecordField("Clave"));
            Store2.AddField(new RecordField("Descripcion"));
            Store2.AddField(new RecordField("ID_CadenaComercial"));
            Store2.AddField(new RecordField("ClaveSucursalAdmin"));
            Store2.AddField(new RecordField("AfiliacionAdmin"));


        }

        [DirectMethod(Namespace = "Empresarial")]
        public void AfterEdit(int ID_Cuenta, string newValue, Int64 ID_Colectiva, string TipoCuentaISO, string codigoMoneda, bool AceptaEditar , string afiliacion)
        {
            RespuestaTransaccional resp = new RespuestaTransaccional();

            Guid AppId = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            try
            {
                LogHeader logTEMP = new LogHeader();
                resp = LNSaldos.ModificarSaldo(float.Parse(newValue), ID_Cuenta, ID_Colectiva, TipoCuentaISO, codigoMoneda,
                    AceptaEditar, AppId, this.Usuario, afiliacion, logTEMP);

                // Send Message...
                if (resp.IsAutorizada())
                {
                    X.Msg.Notify("Respuesta Transaccional", "Resultados de la Operación <br />  Codigo Respuesta: <b>" + resp.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + resp.Autorizacion + "</b> <br />").Show();
                    X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();
                    
                }
                else
                {
                    if (resp.IsTimeOut)
                    {
                        X.Msg.Notify("Respuesta Transaccional","Mensaje: <b>" + resp.ResultadoOperacion + "</b> <br />").Show();
                        X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br /><br />  <b> T I M E    O U T </b> <br />  <br /> ").Show();
                    }
                    else
                    {
                        if ((resp.DescripcionRespuesta == null) || (resp.DescripcionRespuesta == ""))
                        {
                            X.Msg.Notify("Respuesta Transaccional", "Mensaje: <b>" + resp.ResultadoOperacion + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Respuesta Transaccional", "<br />  Codigo Respuesta: <b>" + resp.CodigoRespuesta + "</b> <br /> Numero Autorización: <b>" + resp.Autorizacion + "</b> <br />" + "Mensaje: <b>" + resp.DescripcionRespuesta + "</b> <br />").Show();
                            X.Msg.Notify("Respuesta Transaccional", "Modificacion de Saldo <br /><br />  <b> D E C L I N A D A </b> <br />  <br /> ").Show();
                        }
                    }
                }

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Modificaciones de Saldos", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Modificaciones de Saldos", "No es Posible Modificar el Saldo.").Show();
            }
            finally
            {
                //Actualizar el grid de los Saldos.
                PreparaGridSaldos();
                BindDataSaldos(ID_Colectiva);
            }
          

            //this.GridPanel1.Store.Primary.CommitChanges();
        }

        protected void PreparaGridEmpleados()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesEmpleados();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Colectiva = new Column();
            colID_Colectiva.DataIndex = "ID_Colectiva";
            colID_Colectiva.Header = "ID";
            colID_Colectiva.Sortable = true;
            lasColumnas.Columns.Add(colID_Colectiva);

            Column colClaveColectiva = new Column();
            colClaveColectiva.DataIndex = "Estatus";
            colClaveColectiva.Header = "Estatus ";
            colClaveColectiva.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectiva);

            Column colNombreORazonSocial = new Column();
            colNombreORazonSocial.DataIndex = "NombreORazonSocial";
            colNombreORazonSocial.Header = "Nombre";
            colNombreORazonSocial.Sortable = true;
            lasColumnas.Columns.Add(colNombreORazonSocial);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "APaterno";
            colAPaterno.Header = "Ubicación 1";
            colAPaterno.Sortable = true;
              lasColumnas.Columns.Add(colAPaterno);

            Column colAMaterno = new Column();
            colAMaterno.DataIndex = "AMaterno";
            colAMaterno.Header = "Ubicación 2";
            colAMaterno.Sortable = true;
             lasColumnas.Columns.Add(colAMaterno);

            Column colRFC = new Column();
            colRFC.DataIndex = "RFC";
            colRFC.Header = "RFC";
            colRFC.Sortable = true;
            lasColumnas.Columns.Add(colRFC);

            Column colID_CadenaComercial = new Column();
            colID_CadenaComercial.DataIndex = "ID_CadenaComercial";
            colID_CadenaComercial.Header = "ID_CadenaComercial";
            colID_CadenaComercial.Sortable = true;
            colID_CadenaComercial.Hidden = true;
            lasColumnas.Columns.Add(colID_CadenaComercial);

            Column colClaveSucursalAdmin = new Column();
            colClaveSucursalAdmin.DataIndex = "ClaveSucursalAdmin";
            colClaveSucursalAdmin.Header = "ClaveSucursalAdmin";
            colClaveSucursalAdmin.Sortable = true;
            colClaveSucursalAdmin.Hidden = true;
            lasColumnas.Columns.Add(colClaveSucursalAdmin);

            //Column colAfiliacionAdmin = new Column();
            //colAfiliacionAdmin.DataIndex = "AfiliacionAdmin";
            //colAfiliacionAdmin.Header = "AfiliacionAdmin";
            //colAfiliacionAdmin.Sortable = true;
            //lasColumnas.Columns.Add(colAfiliacionAdmin);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            //ImageCommandColumn acciones = new ImageCommandColumn();
            //acciones.Header = "Acciones";
            //acciones.Width = 55;
            //ImageCommand edit = new ImageCommand();
            //edit.Icon = Icon.NoteEdit;
            //edit.CommandName = "Estatus";
            //edit.ToolTip.Text = "Consultar Saldo";
            //acciones.Commands.Add(edit);



            //AGREGAR COLUMNAS
            //GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel2.ColumnModel.Columns.Add(colID_Colectiva);

            GridPanel2.ColumnModel.Columns.Add(colClaveColectiva);
            GridPanel2.ColumnModel.Columns.Add(colNombreORazonSocial);
            GridPanel2.ColumnModel.Columns.Add(colAPaterno);
            GridPanel2.ColumnModel.Columns.Add(colAMaterno);
            GridPanel2.ColumnModel.Columns.Add(colRFC);
            GridPanel2.ColumnModel.Columns.Add(colEmail);
            GridPanel2.ColumnModel.Columns.Add(colID_CadenaComercial);
            GridPanel2.ColumnModel.Columns.Add(colClaveSucursalAdmin);
           // GridPanel2.ColumnModel.Columns.Add(colAfiliacionAdmin);


            //AGREGAR EVENTOS
            GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));
            GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Afiliacion", "this.getRowsValues({ selectedOnly: true })[0].AfiliacionAdmin", ParameterMode.Raw));
            




        }

        protected void btnCrearCta_Click(object sender, DirectEventArgs e)
        {
            Int64 ID_colectiva = 0; ;
            
            try
            {
                if (HttpContext.Current.Session.Contents["ID_ColectivaSaldo"] == null)
                {
                    throw new CAppException(8011, "No hay una Colectiva Seleccionada, Selecciona un registro e Intentalo nuevamente");
                }

                //if (HttpContext.Current.Session.Contents["AfiliacionSaldo"] == null)
                //{
                //    throw new CAppException(8011, "No hay una Colectiva Seleccionada, Selecciona un registro e Intentalo nuevamente");
                //}

                String unValor = (String)HttpContext.Current.Session.Contents["ID_ColectivaSaldo"];
                //unaAfil = (String)HttpContext.Current.Session.Contents["AfiliacionSaldo"];

                ID_colectiva = Int64.Parse(unValor);
                LogHeader logTEMP = new LogHeader();

                if (!LNCuenta.AgregarCuentasAColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), logTEMP))
                {
                    throw new CAppException(8011, "No fue Posible Crear las cuentas a la Colectiva Seleccionada");
                }

                X.Msg.Notify("Creación de Cuentas", "Se han Creado las Cuentas <br />  <br /> <b> E X I T O S A M E N T E  </b> <br />  <br /> ").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Creacion de Cuentas", err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                X.Msg.Alert("Creacion de Cuentas", err.Message).Show();
            }
            finally
            {
                PreparaGridSaldos();
                BindDataSaldos(ID_colectiva);
            }
        }

        protected void PreparaGridSaldos()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesCuentas();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            GroupingSummaryColumn colID_Colectiva = new GroupingSummaryColumn();
            colID_Colectiva.DataIndex = "ID_Cuenta";
            colID_Colectiva.Header = "ID Cuenta";
            colID_Colectiva.Width = 50;
            colID_Colectiva.Sortable = true;
            colID_Colectiva.Hidden = true;
            lasColumnas.Columns.Add(colID_Colectiva);

            GroupingSummaryColumn colAfiliacion = new GroupingSummaryColumn();
            colAfiliacion.DataIndex = "Afiliacion";
            colAfiliacion.Header = "ID Afiliacion";
            colAfiliacion.Width = 50;
            colAfiliacion.Sortable = true;
            colAfiliacion.Hidden = true;
            lasColumnas.Columns.Add(colAfiliacion);


            Column colCadenaComercial = new Column();
            colCadenaComercial.DataIndex = "CadenaComercial";
            colCadenaComercial.Header = "CadenaComercial";
            colCadenaComercial.Width = 50;
            colCadenaComercial.Sortable = true;
            colCadenaComercial.Hidden = true;
            lasColumnas.Columns.Add(colCadenaComercial);

            GroupingSummaryColumn colID_CCM = new GroupingSummaryColumn();
            colID_CCM.DataIndex = "ID_CCM";
            colID_CCM.Header = "ID_CCM";
            colID_CCM.Width = 50;
            colID_CCM.Sortable = true;
            colID_CCM.Hidden = true;
            lasColumnas.Columns.Add(colID_CCM);

            GroupingSummaryColumn colEditarSaldoGrid = new GroupingSummaryColumn();
            colEditarSaldoGrid.DataIndex = "EditarSaldoGrid";
            colEditarSaldoGrid.Header = "EditarSaldoGrid";
            colEditarSaldoGrid.Width = 50;
            colEditarSaldoGrid.Sortable = true;
            colEditarSaldoGrid.Hidden = true;
            lasColumnas.Columns.Add(colEditarSaldoGrid);

            GroupingSummaryColumn colDescEstatus = new GroupingSummaryColumn();
            colDescEstatus.DataIndex = "DescEstatus";
            colDescEstatus.Header = "Estatus";
            colDescEstatus.Width = 50;
            colDescEstatus.Sortable = true;
            lasColumnas.Columns.Add(colDescEstatus);

            GroupingSummaryColumn colCodTipoCuentaISO = new GroupingSummaryColumn();
            colCodTipoCuentaISO.DataIndex = "CodTipoCuentaISO";
            colCodTipoCuentaISO.Header = "TipoCuentaISO";
            colCodTipoCuentaISO.Width = 50;
            colCodTipoCuentaISO.Sortable = true;
            colCodTipoCuentaISO.Hidden = true;
            lasColumnas.Columns.Add(colCodTipoCuentaISO);

            GroupingSummaryColumn colID_Cuenta = new GroupingSummaryColumn();
            colID_Cuenta.DataIndex = "ID_Cuenta";
            colID_Cuenta.Header = "ID Cuenta";
            colID_Cuenta.Width = 50;
            colID_Cuenta.Sortable = true;
            colID_Cuenta.Hidden = true;
            lasColumnas.Columns.Add(colID_Cuenta);

            GroupingSummaryColumn colCuentaHabiente = new GroupingSummaryColumn();
            colCuentaHabiente.DataIndex = "CuentaHabiente";
            colCuentaHabiente.Header = "Cuenta Habiente";
            colCuentaHabiente.Hidden = true;
            colCuentaHabiente.Width = 50;
            colCuentaHabiente.Sortable = true;
            lasColumnas.Columns.Add(colCuentaHabiente);


            GroupingSummaryColumn colDescripcionCuenta = new GroupingSummaryColumn();
            colDescripcionCuenta.DataIndex = "DescripcionCuenta";
            colDescripcionCuenta.Header = "Tipo Cuenta";
            colDescripcionCuenta.Hidden = true;
            colDescripcionCuenta.Width = 200;
            colDescripcionCuenta.Sortable = true;
            lasColumnas.Columns.Add(colDescripcionCuenta);

            GroupingSummaryColumn colLimiteCredito = new GroupingSummaryColumn();
            colLimiteCredito.DataIndex = "LimiteCredito";
            colLimiteCredito.Header = "Limite de Credito";
            colLimiteCredito.Sortable = true;
            colLimiteCredito.Renderer.Format = RendererFormat.UsMoney;
           
            
             lasColumnas.Columns.Add(colLimiteCredito);

             GroupingSummaryColumn colConsumos = new GroupingSummaryColumn();
            colConsumos.DataIndex = "Consumos";
            colConsumos.Header = "Consumos";
            colConsumos.Sortable = true;
            colConsumos.Renderer.Format = RendererFormat.UsMoney;
            lasColumnas.Columns.Add(colConsumos);

            NumberColumn colSaldoActual = new NumberColumn();
            colSaldoActual.DataIndex = "SaldoDisponible";
            colSaldoActual.Header = "Saldo Disponible";
            colSaldoActual.Sortable = true;
            TextField elEditorNumero = new TextField();
            elEditorNumero.AllowBlank = false;
            colSaldoActual.Editor.Add(elEditorNumero);
            colSaldoActual.Renderer.Format = RendererFormat.UsMoney;
            colSaldoActual.Format = "$0,0.00";
            colSaldoActual.Editable = true;

            lasColumnas.Columns.Add(colSaldoActual);


            GroupingSummaryColumn colCodigoMoneda = new GroupingSummaryColumn();
            colCodigoMoneda.DataIndex = "CodigoMoneda";
            colCodigoMoneda.Header = "$";
            colCodigoMoneda.Width = 50;
            colCodigoMoneda.Sortable = true;
            lasColumnas.Columns.Add(colCodigoMoneda);

            GroupingSummaryColumn colDescripcionMoneda = new GroupingSummaryColumn();
            colDescripcionMoneda.DataIndex = "DescripcionMoneda";
            colDescripcionMoneda.Header = "Moneda";
            colDescripcionMoneda.Sortable = true;
            lasColumnas.Columns.Add(colDescripcionMoneda);

            GroupingSummaryColumn ColBreveDescripcion = new GroupingSummaryColumn();
            ColBreveDescripcion.DataIndex = "BreveDescripcion";
            ColBreveDescripcion.Header = "Breve Descripción";
            ColBreveDescripcion.Sortable = true;
            lasColumnas.Columns.Add(ColBreveDescripcion);



            CommandColumn acciones = new CommandColumn();
            acciones.Header = "Editable";
            acciones.Width = 30;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand edit = new GridCommand();
            edit.Icon = Icon.Overlays;
            edit.CommandName = "Editable";
            edit.ToolTip.Text = "La Cuenta Permite el Modificacion de Saldo por el Grid";
            acciones.Commands.Add(edit);

            GridCommand NOedit = new GridCommand();
            NOedit.Icon = Icon.Lock;
            NOedit.CommandName = "NoEditable";
            NOedit.ToolTip.Text = "La Cuenta No Permite el Modificacion de Saldo por el Grid";
            acciones.Commands.Add(NOedit);



            //AGREGAR COLUMNAS
            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colDescEstatus);
            GridPanel1.ColumnModel.Columns.Add(colAfiliacion);
            GridPanel1.ColumnModel.Columns.Add(colID_Cuenta);
            GridPanel1.ColumnModel.Columns.Add(colEditarSaldoGrid);
            GridPanel1.ColumnModel.Columns.Add(colCuentaHabiente);
            GridPanel1.ColumnModel.Columns.Add(ColBreveDescripcion);
            GridPanel1.ColumnModel.Columns.Add(colDescripcionCuenta);
            GridPanel1.ColumnModel.Columns.Add(colLimiteCredito);
            GridPanel1.ColumnModel.Columns.Add(colConsumos);
            GridPanel1.ColumnModel.Columns.Add(colSaldoActual);
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
        
        protected void RefreshGridSaldos(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                Int64 idCol = Int64.Parse(HttpContext.Current.Session["ID_ColectivaSaldo"].ToString());
                //string afil = HttpContext.Current.Session["AfiliacionSaldo"].ToString();
             
                PreparaGridSaldos();
                this.BindDataSaldos(idCol);
            }
            catch (Exception )
            {
            }

        }

        protected void RefreshGridEmpleados(object sender, StoreRefreshDataEventArgs e)
        {
            PreparaGridEmpleados();
            this.BindDataEmpleados();

        }

        [DirectMethod(Namespace = "Empresarial")]
        public void ActualizaGridSaldos()
        {
            try
            {
                Int64 idCol = Int64.Parse(HttpContext.Current.Session["ID_ColectivaSaldo"].ToString());
               // string afil = HttpContext.Current.Session["AfiliacionSaldo"].ToString();

                PreparaGridSaldos();
                this.BindDataSaldos(idCol);
            }
            catch (Exception )
            {
            }
        }

        private void BindDataSaldos( Int64 ID_colectiva)
        {
            LogHeader logTEMP = new LogHeader();
            GridPanel1.GetStore().DataSource = DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), logTEMP);
            GridPanel1.GetStore().DataBind();
            //store.DataSource = DAOFichaDeposito.ListaFichasDeposito();
            //store.DataBind();
            // PreparaGrid();
            // btnGuardar.Click +=new EventHandler(btnGuardar_Click);

        }

        private void BindDataEmpleados()
        {


            Guid IDApp = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());

            //GridPanel2.GetStore().DataSource = DAOColectiva.ListaSaldosColectivaIDTipoColectiva(-1, Configuracion.Get(IDApp, "ClaveTipoColectivaEmpleado").Valor);
            //GridPanel2.GetStore().DataBind();
            LogHeader logTEMP = new LogHeader();
            GridPanel2.GetStore().DataSource = DAOCatalogos.ListaEmpleadosMiembrosClubGrupoComercial(-1,
                Configuracion.Get(IDApp, "ClaveTipoColectivaSucursal").Valor, this.Usuario, 
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                logTEMP);
            GridPanel2.GetStore().DataBind();
        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            Panel5.Collapsed = true;
            GridPanel1.GetStore().RemoveAll();
        }

        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];
                //String unaAfil = e.ExtraParams["Afiliacion"];

                Panel5.Title = "Saldos en Cuentas de la Sucursal No. [" + unvalor + "]";

                HttpContext.Current.Session.Add("ID_ColectivaSaldo", unvalor);
                //HttpContext.Current.Session.Add("AfiliacionSaldo", unaAfil);

                Int64 ID_colectiva = Int64.Parse(unvalor);

                PreparaGridSaldos();
                BindDataSaldos(ID_colectiva);
            }
            catch (Exception )
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            Panel5.Collapsed = false;
        }

    }
}