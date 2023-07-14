using DALAutorizador.BaseDatos;
using DALAutorizador.LogicaNegocio;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Web;

namespace OperadoraWeb
{
    public partial class AddMediosDePago : PaginaBaseCAPP
    {

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {

                String EjecutarComando = (String)e.ExtraParams["Comando"];
                String claveOperador = (String)e.ExtraParams["Operador"];
                String afiliacion = (String)e.ExtraParams["Afiliacion"];
                String descCadenaComercial = (String)e.ExtraParams["descCadenaComercial"];
                String ClaveMA = (String)e.ExtraParams["ClaveMA"];
                String TipoISO = (String)e.ExtraParams["TipoISO"];

                

                if (claveOperador.Length == 0)
                {
                    throw new Exception("El Registro seleccionado no tiene Clave de Operador definida");
                }

                if (afiliacion.Length == 0)
                {
                    throw new Exception("El Registro seleccionado no tiene Afiliacion Definida");
                }
                

                switch (EjecutarComando)
                {
                    case "Bloquear":

                        if (LNMediosAcceso.DesasignarMediodePagoOperadora(claveOperador, ClaveMA, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())) != 0)
                        {
                            throw new Exception("No se Bloqueo el Medio de Pago");
                        }
                        else
                        {
                            X.Msg.Notify("Operadora Web", "Desasignación de Medio de Pago a Operador <br />  <br /> <b> A U T O R I Z A D O </b> <br />  <br /> ").Show();
                        }
                        break;
                    case "Activar":
                        if (LNMediosAcceso.AsignarMediodePagoOperadora(claveOperador, afiliacion, descCadenaComercial, ClaveMA, TipoISO, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())) != 0)
                        {
                            throw new Exception("No se Activo el Medio de Pago");
                        }
                        else
                        {
                            X.Msg.Notify("Operadora Web", "Activación/Asignación Medio de Pago a Operador <br />  <br /> <b> A U T O R I Z A D A </b> <br />  <br /> ").Show();
                        }
                        break;
                }

                BindDataMedioAcceso(Int64.Parse(HttpContext.Current.Session["OPEWEB_ID_Operador"].ToString()),HttpContext.Current.Session["OPEWEB_ClaveOperador"].ToString());
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Operadores Web", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Operadores Web", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!IsPostBack)
                {
                    PreparaGridMediosAcceso();
                    PreparaGridColectiva();
                    this.BindDataColectiva();
                }

                
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        private void AgregaRecordFilesColectiva()
        {
            Store2.AddField(new RecordField("ID_Colectiva"));
            Store2.AddField(new RecordField("ClaveColectiva"));
            Store2.AddField(new RecordField("NombreORazonSocial"));
            Store2.AddField(new RecordField("APaterno"));
            Store2.AddField(new RecordField("AMaterno"));
            Store2.AddField(new RecordField("RFC"));
            Store2.AddField(new RecordField("Email"));
            Store2.AddField(new RecordField("Estatus"));
            Store2.AddField(new RecordField("ID_EstatusColectiva"));
            Store2.AddField(new RecordField("Clave"));
            Store2.AddField(new RecordField("Descripcion"));
            Store2.AddField(new RecordField("ColectivaPadre"));
            Store2.AddField(new RecordField("TipoColectiva"));
        }

        private void AgregaRecordFilesMediosPago()
        {

            storeMedioPago.AddField(new RecordField("descCuentaHabiente"));
            storeMedioPago.AddField(new RecordField("descTipoColectiva"));
            storeMedioPago.AddField(new RecordField("NombreCuentahabiente"));
            storeMedioPago.AddField(new RecordField("descCadenaComercial"));
            storeMedioPago.AddField(new RecordField("DescTipoMedioAcceso"));
            storeMedioPago.AddField(new RecordField("ClaveMA"));
            storeMedioPago.AddField(new RecordField("BreveDescripcion"));
            storeMedioPago.AddField(new RecordField("TipoISO"));
            storeMedioPago.AddField(new RecordField("Afiliacion"));
            storeMedioPago.AddField(new RecordField("ExisteEnOperadora"));
            storeMedioPago.AddField(new RecordField("Operador",RecordFieldType.String));
            storeMedioPago.AddField(new RecordField("ExisteEnOperadora"));
        }

        protected void PreparaGridMediosAcceso()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesMediosPago();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();
            Column colOperador = new Column();
            colOperador.DataIndex = "ExisteEnOperadora";
            colOperador.Header = "ExisteEnOperadora";
            colOperador.Hidden = true;
            colOperador.Sortable = true;
            lasColumnas.Columns.Add(colOperador);

            Column colID_Colectiva = new Column();
            colID_Colectiva.DataIndex = "descCuentaHabiente";
            colID_Colectiva.Header = "Cuentahabiente";
            colID_Colectiva.Sortable = true;
            
            lasColumnas.Columns.Add(colID_Colectiva);

            Column ColEstatus = new Column();
            ColEstatus.DataIndex = "descTipoColectiva";
            ColEstatus.Header = "Tipo Cuentahabiente";
            ColEstatus.Hidden = true;
            ColEstatus.Sortable = true;
            lasColumnas.Columns.Add(ColEstatus);

            Column colClaveColectiva = new Column();
            colClaveColectiva.DataIndex = "descCadenaComercial";
            colClaveColectiva.Header = "Cadena Dueña de Cuenta";
            colClaveColectiva.Sortable = true;
            colClaveColectiva.Hidden = true;
            lasColumnas.Columns.Add(colClaveColectiva);


            Column colTipoColectiva = new Column();
            colTipoColectiva.DataIndex = "DescTipoMedioAcceso";
            colTipoColectiva.Header = "Tipo de Medio de Pago";
            colTipoColectiva.Sortable = true;
            lasColumnas.Columns.Add(colTipoColectiva);

            Column colNombreORazonSocial = new Column();
            colNombreORazonSocial.DataIndex = "ClaveMA";
            colNombreORazonSocial.Header = "Medio de Pago";
            colNombreORazonSocial.Sortable = true;
            lasColumnas.Columns.Add(colNombreORazonSocial);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "BreveDescripcion";
            colAPaterno.Header = "Descripcion";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            Column colAMaterno = new Column();
            colAMaterno.DataIndex = "TipoISO";
            colAMaterno.Header = "Tipo Cuenta ISO";
            colAMaterno.Sortable = true;
            colAMaterno.Hidden = true;
            lasColumnas.Columns.Add(colAMaterno);

            Column colRFC = new Column();
            colRFC.DataIndex = "Afiliacion";
            colRFC.Header = "Afiliacion";
            colRFC.Hidden = true;
            colRFC.Sortable = true;
            lasColumnas.Columns.Add(colRFC);


            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 30;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

       
            GridCommand Bloquear = new GridCommand();
            Bloquear.Icon = Icon.Delete;
            Bloquear.CommandName = "Bloquear";
            Bloquear.ToolTip.Text = "Desasignar Medio de Pago";
            acciones.Commands.Add(Bloquear);


            GridCommand play = new GridCommand();
            play.Icon = Icon.Add;
            play.CommandName = "Activar";
            play.ToolTip.Text = "Asignar Medio de Pago";
            acciones.Commands.Add(play);


            //AGREGAR COLUMNAS
            GridPanel2.ColumnModel.Columns.Add(acciones);
            GridPanel2.ColumnModel.Columns.Add(colID_Colectiva);
            GridPanel2.ColumnModel.Columns.Add(ColEstatus);
            GridPanel2.ColumnModel.Columns.Add(colTipoColectiva);
            GridPanel2.ColumnModel.Columns.Add(colClaveColectiva);
            GridPanel2.ColumnModel.Columns.Add(colOperador);
            GridPanel2.ColumnModel.Columns.Add(colNombreORazonSocial);
            GridPanel2.ColumnModel.Columns.Add(colAPaterno);
            GridPanel2.ColumnModel.Columns.Add(colAMaterno);
            GridPanel2.ColumnModel.Columns.Add(colRFC);


            //AGREGAR EVENTOS
            GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ClaveMA", "record.data.ClaveMA", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Afiliacion", "record.data.Afiliacion", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Operador", "record.data.Operador", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("descCadenaComercial", "record.data.descCadenaComercial", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("TipoISO", "record.data.TipoISO", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));


            GridFilters losFiltros = new GridFilters();



            //StringFilter filClaveColectiva = new StringFilter();
            //filClaveColectiva.DataIndex = "ClaveColectiva";
            //losFiltros.Filters.Add(filClaveColectiva);

            //StringFilter filNombreORazonSocial = new StringFilter();
            //filNombreORazonSocial.DataIndex = "NombreORazonSocial";
            //losFiltros.Filters.Add(filNombreORazonSocial);

            //StringFilter filAPaterno = new StringFilter();
            //filAPaterno.DataIndex = "APaterno";
            //losFiltros.Filters.Add(filAPaterno);

            //StringFilter filAMaterno = new StringFilter();
            //filAMaterno.DataIndex = "AMaterno";
            //losFiltros.Filters.Add(filAMaterno);

            //StringFilter filRFC = new StringFilter();
            //filRFC.DataIndex = "RFC";
            //losFiltros.Filters.Add(filRFC);

            //StringFilter filEmail = new StringFilter();
            //filEmail.DataIndex = "Email";
            //losFiltros.Filters.Add(filEmail);

            GridPanel2.Plugins.Add(losFiltros);



        }

      
        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGridColectiva();
                // LlenaTiposColectiva();
                this.BindDataColectiva();
            }
            catch (Exception)
            {
            }

        }

        protected void PreparaGridColectiva()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesColectiva();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Colectiva = new Column();
            colID_Colectiva.DataIndex = "ID_Colectiva";
            colID_Colectiva.Header = "ID";
            colID_Colectiva.Sortable = true;
            colID_Colectiva.Hidden = true;
            lasColumnas.Columns.Add(colID_Colectiva);

            Column ColEstatus = new Column();
            ColEstatus.DataIndex = "Estatus";
            ColEstatus.Header = "Estatus ";
            ColEstatus.Sortable = true;
            ColEstatus.Hidden = true;
            lasColumnas.Columns.Add(ColEstatus);

            Column colClaveColectiva = new Column();
            colClaveColectiva.DataIndex = "ClaveColectiva";
            colClaveColectiva.Header = "Clave";
            colClaveColectiva.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectiva);

            Column colTipoColectiva = new Column();
            colTipoColectiva.DataIndex = "Descripcion";
            colTipoColectiva.Header = "Tipo Colectiva";
            colTipoColectiva.Sortable = true;
            lasColumnas.Columns.Add(colTipoColectiva);

            Column colNombreORazonSocial = new Column();
            colNombreORazonSocial.DataIndex = "NombreORazonSocial";
            colNombreORazonSocial.Header = "Nombre";
            colNombreORazonSocial.Sortable = true;
            lasColumnas.Columns.Add(colNombreORazonSocial);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "APaterno";
            colAPaterno.Header = "Apellido Paterno";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            Column colAMaterno = new Column();
            colAMaterno.DataIndex = "AMaterno";
            colAMaterno.Header = "Apellido Materno";
            colAMaterno.Sortable = true;
            lasColumnas.Columns.Add(colAMaterno);

            Column colRFC = new Column();
            colRFC.DataIndex = "RFC";
            colRFC.Header = "RFC";
            colRFC.Hidden = true;
            colRFC.Sortable = true;
            lasColumnas.Columns.Add(colRFC);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
            colEmail.Hidden = true;
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            Column ColColectivaPadre = new Column();
            ColColectivaPadre.DataIndex = "ColectivaPadre";
            ColColectivaPadre.Header = "Colectiva Padre";
            ColColectivaPadre.Sortable = true;
            lasColumnas.Columns.Add(ColColectivaPadre);


            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 80;
            acciones.PrepareToolbar.Fn = "prepareToolbar";
            acciones.Hidden = true;

            GridCommand AddCuenta = new GridCommand();
            AddCuenta.Icon = Icon.MoneyAdd;
            AddCuenta.CommandName = "AddCuenta";
            AddCuenta.ToolTip.Text = "Crear Cuenta de Individual";
            acciones.Commands.Add(AddCuenta);


            CommandSeparator separa = new CommandSeparator();
            acciones.Commands.Add(separa);


            GridCommand AddMP = new GridCommand();
            AddMP.Icon = Icon.Creditcards;
            AddMP.CommandName = "AddMedioPago";
            AddMP.ToolTip.Text = "Asignar Medios de Pago";
            acciones.Commands.Add(AddMP);

            CommandSeparator separa2 = new CommandSeparator();
            acciones.Commands.Add(separa);

            GridCommand Bloquear = new GridCommand();
            Bloquear.Icon = Icon.Stop;
            Bloquear.CommandName = "Bloquear";
            Bloquear.ToolTip.Text = "Bloquear Operador";
            acciones.Commands.Add(Bloquear);


            GridCommand play = new GridCommand();
            play.Icon = Icon.PlayBlue;
            play.CommandName = "Activar";
            play.ToolTip.Text = "Activar Operador";
            acciones.Commands.Add(play);


            //AGREGAR COLUMNAS
            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colID_Colectiva);
            GridPanel1.ColumnModel.Columns.Add(ColEstatus);
            GridPanel1.ColumnModel.Columns.Add(colTipoColectiva);
            GridPanel1.ColumnModel.Columns.Add(colClaveColectiva);

            GridPanel1.ColumnModel.Columns.Add(colNombreORazonSocial);
            GridPanel1.ColumnModel.Columns.Add(colAPaterno);
            GridPanel1.ColumnModel.Columns.Add(colAMaterno);
            GridPanel1.ColumnModel.Columns.Add(colRFC);
            GridPanel1.ColumnModel.Columns.Add(colEmail);
            GridPanel1.ColumnModel.Columns.Add(ColColectivaPadre);

            //AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ClaveColectiva", "this.getRowsValues({ selectedOnly: true })[0].ClaveColectiva", ParameterMode.Raw));

            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "record.data.ID_Colectiva", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ClaveColectiva", "record.data.ClaveColectiva", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();



            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "ClaveColectiva";
            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "NombreORazonSocial";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "APaterno";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "AMaterno";
            losFiltros.Filters.Add(filAMaterno);

            StringFilter filRFC = new StringFilter();
            filRFC.DataIndex = "RFC";
            losFiltros.Filters.Add(filRFC);

            StringFilter filEmail = new StringFilter();
            filEmail.DataIndex = "Email";
            losFiltros.Filters.Add(filEmail);

            GridPanel1.Plugins.Add(losFiltros);



        }

        private void BindDataColectiva()
        {
            //var store = this.GridPanel1.GetStore();
            try
            {
                GridPanel1.GetStore().DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor);
                GridPanel1.GetStore().DataBind();
            }
            catch (Exception)
            {
            }
            //store.DataSource = DAOFichaDeposito.ListaFichasDeposito();
            //store.DataBind();
            // PreparaGrid();
            // btnGuardar.Click +=new EventHandler(btnGuardar_Click);

        }

        private void BindDataMedioAcceso(Int64 ID_Operador, String ClaveOperador)
        {
            //var store = this.GridPanel1.GetStore();
            try
            {
                PreparaGridMediosAcceso();

                GridPanel2.GetStore().DataSource = LNMediosAcceso.ListarMediosAccesoOperador(ID_Operador, ClaveOperador, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridPanel2.GetStore().DataBind();
            }
            catch (Exception)
            {
            }
            //store.DataSource = DAOFichaDeposito.ListaFichasDeposito();
            //store.DataBind();
            // PreparaGrid();
            // btnGuardar.Click +=new EventHandler(btnGuardar_Click);

        }

        protected void RefreshGridMediosPago(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                BindDataMedioAcceso(Int64.Parse(HttpContext.Current.Session["OPEWEB_ID_Operador"].ToString()), HttpContext.Current.Session["OPEWEB_ClaveOperador"].ToString());
            }
            catch (Exception)
            {
            }

        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            try
            {
                Panel1.Collapsed = true;
                GridPanel2.GetStore().RemoveAll();
            }
            catch (Exception)
            {
            }
        }


        protected void GridOperadores_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];
                String unaClaveColectiva = e.ExtraParams["ClaveColectiva"];

                Panel1.Title = "Los Medios de Pago Posibles para el Operador  No. [" + unvalor + "]";

                HttpContext.Current.Session["OPEWEB_ID_Operador"] = unvalor;
                HttpContext.Current.Session["OPEWEB_ClaveOperador"] = unaClaveColectiva;

                Int64 ID_colectiva = Int64.Parse(unvalor);

                BindDataMedioAcceso(ID_colectiva, unaClaveColectiva);
            }
            catch (Exception)
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            Panel1.Collapsed = false;
        }

    
    }
}