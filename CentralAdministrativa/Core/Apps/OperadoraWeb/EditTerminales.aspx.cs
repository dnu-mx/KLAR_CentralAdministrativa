using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Web;

namespace OperadoraWeb
{
    public partial class EditTerminales : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                PreparaGridColectiva();
                // BindDataColectiva();
                LlenaAfiliaciones();


                if (!X.IsAjaxRequest)
                {
                    this.BindDataColectiva();
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
                PreparaGridColectiva();
                BindDataColectiva();
            }
            catch (Exception)
            {
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

        protected void LlenaAfiliaciones()
        {
            try
            {
                SAfiliaciones.DataSource = DAOCatalogos.ListaAfiliaciones(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                SAfiliaciones.DataBind();
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


            //AGREGAR COLUMNAS
            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 80;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand AddCuenta = new GridCommand();
            AddCuenta.Icon = Icon.MoneyAdd;
            AddCuenta.CommandName = "AddCuenta";
            AddCuenta.ToolTip.Text = "Crear Cuenta Individual";
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
            Bloquear.Icon = Icon.Lightbulb;
            Bloquear.CommandName = "Bloquear";
            Bloquear.ToolTip.Text = "Inactivar Terminal";
            acciones.Commands.Add(Bloquear);


            GridCommand play = new GridCommand();
            play.Icon = Icon.LightbulbOff;
            play.CommandName = "Activar";
            play.ToolTip.Text = "Activar Terminal";
            acciones.Commands.Add(play);

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

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 ID_Colectiva = Int64.Parse(e.ExtraParams["ID_Colectiva"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];
                String ClaveUsuario = (String)e.ExtraParams["ClaveColectiva"];

                if (ID_Colectiva == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;


                switch (EjecutarComando)
                {
                    case "CambioPass":
                        if (LNColectiva.MarcarCambioPassWord(ClaveUsuario, this.Usuario) != 0)
                        {
                            throw new Exception("No se Cambio el Estatus del Operador");
                        }
                        break;
                    case "Bloquear":
                        if (LNColectiva.DesactivaColectiva(ID_Colectiva, this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo la Afiliación");
                        }
                        break;
                    case "Activar":
                        if (LNColectiva.ActivaColectiva(ID_Colectiva, this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo la Afiliación");
                        }

                        break;
                }

                PreparaGridColectiva();
                this.BindDataColectiva();

                X.Msg.Notify("Operadora Web", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
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





        private void BindDataColectiva()
        {
            //var store = this.GridPanel1.GetStore();
            try
            {
                GridPanel1.GetStore().DataSource = DAOCatalogos.ListaTiposColectivasPorTipoColectiva(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaTerminal").Valor);
                GridPanel1.GetStore().DataBind();
            }
            catch (Exception)
            {
            }

        }


        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            FormPanel1.Collapsed = true;
        }

        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];

                // Panel5.Title = "Colectiva  No. [" + unvalor + "]";

                LlenaTiposColectiva();

                Int64 ID_colectiva = Int64.Parse(unvalor);
                DespliegaDatosColectiva(ID_colectiva);



            }
            catch (Exception)
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            FormPanel1.Collapsed = false;
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;


        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                if (0 == DAOColectiva.Modificar(AsignarNuevosDatos(new Colectiva()), this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())))
                {
                    FormPanel1.Reset();
                    BindDataColectiva();
                    FormPanel1.Collapsed = true;
                }
                else
                {
                    throw new Exception("Ocurrio un Error al Modificar, Intentalo nuevamente");
                }

            }
            catch (Exception err)
            {
                X.Msg.Alert("Actualizar Colectiva", "Ocurrio un Error:" + err.Message).Show();
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FormPanel1.Collapsed = true;
        }

        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            try
            {
                cmbPadreTipoColectiva.Clear();
                LlenaColectivasPosiblePadre(2, cmbTipoUsuario.Value.ToString().Trim());

            }
            catch (Exception)
            {
            }
        }

        protected void LlenaTiposColectiva()
        {
            try
            {
                STipoColectiva.DataSource = DAOCatalogos.ListaTiposColectiva((this.Usuario).ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                STipoColectiva.DataBind();

            }
            catch (Exception)
            {
            }
        }

        protected void LlenaColectivasPosiblePadre(Int64 ID_Padre, String ClaveTipoColectivaHijo)
        {
            try
            {
                SCPadre.DataSource = DAOCatalogos.ListaColectivaPosiblePadre(ID_Padre, ClaveTipoColectivaHijo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                SCPadre.DataBind();

            }
            catch (Exception)
            {
            }
        }

        protected void DespliegaDatosColectiva(Int64 ID_Colectiva)
        {
            Colectiva laColectivaEditar = DAOColectiva.ObtenerColectiva(ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            FormPanel1.Title = "Editando la Terminal Número [" + laColectivaEditar.ID_Colectiva + "]";
            TabPanel1.SetActiveTab(Panel1);

            HttpContext.Current.Session.Add("Colectiva", laColectivaEditar);

            txtUserName.Text = laColectivaEditar.ClaveColectiva;

            cmbTipoUsuario.SetValue(laColectivaEditar.TipoColectiva.Clave);
            //cmbTipoUsuario.Text = laColectivaEditar.TipoColectiva.Descripcion;

            //TODO: CAmbiar el fijo por el Padre definido al usuario Logueado
            LlenaColectivasPosiblePadre(2, laColectivaEditar.TipoColectiva.Clave);

            cmbPadreTipoColectiva.SetValue(laColectivaEditar.ID_ColectivaPadre);

            txtCURP.Text = laColectivaEditar.CURP;
            txtemail2.Text = laColectivaEditar.Email;
            txtFijo.Text = laColectivaEditar.Telefono;
            txtMovil.Text = laColectivaEditar.Movil;
            txtNombre.Text = laColectivaEditar.NombreORazonSocial;
            txtRFC.Text = laColectivaEditar.RFC;

            txtAmaterno.Text = laColectivaEditar.AMaterno;
            txtApaterno.Text = laColectivaEditar.APaterno;


        }

        protected Colectiva AsignarNuevosDatos(Colectiva laColectiva)
        {
            Colectiva ColectivaOriginal = (Colectiva)HttpContext.Current.Session.Contents["Colectiva"];

            laColectiva.ID_Colectiva = ColectivaOriginal.ID_Colectiva;
            laColectiva.ID_ColectivaPadre =  Int64.Parse(cmbPadreTipoColectiva.Value.ToString());
            laColectiva.ID_ColectivaCCM = ColectivaOriginal.ID_ColectivaCCM;// Int64.Parse(cmbPadreTipoColectiva.Value.ToString());


            laColectiva.ClaveColectiva = txtUserName.Text;
            laColectiva.TipoColectiva = ColectivaOriginal.TipoColectiva;
            laColectiva.CURP = ColectivaOriginal.CURP;
            laColectiva.Email = ColectivaOriginal.Email;
            laColectiva.Telefono = ColectivaOriginal.Telefono;
            laColectiva.Movil = ColectivaOriginal.Movil;
            laColectiva.NombreORazonSocial = txtNombre.Text;
            laColectiva.APaterno = txtApaterno.Text;
            laColectiva.AMaterno = txtAmaterno.Text;
            laColectiva.RFC = ColectivaOriginal.RFC;

            return laColectiva;


        }

    }
}