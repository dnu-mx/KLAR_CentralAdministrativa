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

namespace OperadoraWeb
{
    public partial class AddTerminales : PaginaBaseCAPP
    {


        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;


        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                LNColectiva.AgregarOperativa(ObtieneNuevaColectiva(), this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())) == 0)
                {

                    FormPanel1.Reset();

                    PreparaGridColectiva();
                    BindDataColectiva();
                }
            }
            catch (Exception err)
            {
                X.Msg.Alert("Guardar Colectiva", "Ocurrio un Error:" + err.Message).Show();
            }

        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Panel5.Collapsed = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!IsPostBack)
                {
                    PreparaGridColectiva();
                    //BindDataColectiva();

                    LlenaTiposColectiva();
                    LlenaAfiliaciones();

                    try
                    {
                        cmbTipoUsuario.SetValueAndFireSelect(DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaTerminal").Valor);
                        cmbTipoUsuario.SetValue(DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaTerminal").Valor);
                        datFecha.SetValue(DateTime.Now);
                    }
                    catch (Exception)
                    {
                    }

                }

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
            Store2.AddField(new RecordField("Clave"));
            Store2.AddField(new RecordField("Descripcion"));
            Store2.AddField(new RecordField("ColectivaPadre"));
            Store2.AddField(new RecordField("TipoColectiva"));
            Store2.AddField(new RecordField("ID_EstatusColectiva"));



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



        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            try
            {
                cmbPadreTipoColectiva.Clear();
                LlenaColectivasPosiblePadre(0, cmbTipoUsuario.Value.ToString().Trim());


               // cmbPadreTipoColectiva.SelectedIndex = 0;


            }
            catch (Exception)
            {
            }
        }


        protected void LlenaTiposColectiva()
        {
            try
            {
                STipoColectiva.DataSource = DAOCatalogos.ListaTiposColectiva((this.Usuario).ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())); //TODO: Quitar este codigo Duro
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
                        if (LNColectiva.DesactivaColectiva(ID_Colectiva,  this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo la Terminal");
                        }
                        break;
                    case "Activar":
                        if (LNColectiva.ActivaColectiva(ID_Colectiva,  this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo la Terminal");
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






        //protected void PreparaGridColectiva()
        //{
        //    //LIMPIA GRID

        //    //PREPARAR CONEXION A DATOS

        //    AgregaRecordFilesColectiva();


        //    //AGREGADO DE COLUMNAS
        //    ColumnModel lasColumnas = new ColumnModel();

        //    Column colID_Colectiva = new Column();
        //    colID_Colectiva.DataIndex = "ID_Colectiva";
        //    colID_Colectiva.Header = "ID";
        //    colID_Colectiva.Sortable = true;
        //    colID_Colectiva.Hidden = true;
        //    lasColumnas.Columns.Add(colID_Colectiva);

        //    Column ColEstatus = new Column();
        //    ColEstatus.DataIndex = "Estatus";
        //    ColEstatus.Header = "Estatus ";
        //    ColEstatus.Sortable = true;
        //    lasColumnas.Columns.Add(ColEstatus);

        //    Column colClaveColectiva = new Column();
        //    colClaveColectiva.DataIndex = "ClaveColectiva";
        //    colClaveColectiva.Header = "Clave";
        //    colClaveColectiva.Sortable = true;
        //    lasColumnas.Columns.Add(colClaveColectiva);

        //    Column colTipoColectiva = new Column();
        //    colTipoColectiva.DataIndex = "Descripcion";
        //    colTipoColectiva.Header = "Tipo Colectiva";
        //    colTipoColectiva.Sortable = true;
        //    lasColumnas.Columns.Add(colTipoColectiva);

        //    Column colNombreORazonSocial = new Column();
        //    colNombreORazonSocial.DataIndex = "NombreORazonSocial";
        //    colNombreORazonSocial.Header = "Nombre";
        //    colNombreORazonSocial.Sortable = true;
        //    lasColumnas.Columns.Add(colNombreORazonSocial);

        //    Column colAPaterno = new Column();
        //    colAPaterno.DataIndex = "APaterno";
        //    colAPaterno.Header = "ApellidoPaterno";
        //    colAPaterno.Sortable = true;
        //    lasColumnas.Columns.Add(colAPaterno);

        //    Column colAMaterno = new Column();
        //    colAMaterno.DataIndex = "AMaterno";
        //    colAMaterno.Header = "Apellido Materno";
        //    colAMaterno.Sortable = true;
        //    lasColumnas.Columns.Add(colAMaterno);

        //    Column colRFC = new Column();
        //    colRFC.DataIndex = "RFC";
        //    colRFC.Header = "RFC";
        //    colRFC.Hidden = true;
        //    colRFC.Sortable = true;
        //    lasColumnas.Columns.Add(colRFC);

        //    Column colEmail = new Column();
        //    colEmail.DataIndex = "Email";
        //    colEmail.Header = "Email";
        //    colEmail.Hidden = true;
        //    colEmail.Sortable = true;
        //    lasColumnas.Columns.Add(colEmail);

        //    Column ColColectivaPadre = new Column();
        //    ColColectivaPadre.DataIndex = "ColectivaPadre";
        //    ColColectivaPadre.Header = "Colectiva Padre";
        //    ColColectivaPadre.Sortable = true;
        //    lasColumnas.Columns.Add(ColColectivaPadre);


        //    //ImageCommandColumn acciones = new ImageCommandColumn();
        //    //acciones.Header = "Acciones";
        //    //acciones.Width = 55;
        //    //ImageCommand edit = new ImageCommand();
        //    //edit.Icon = Icon.NoteEdit;
        //    //edit.CommandName = "Estatus";
        //    //edit.ToolTip.Text = "Consultar Saldo";
        //    //acciones.Commands.Add(edit);



        //    //AGREGAR COLUMNAS
        //    //GridPanel1.ColumnModel.Columns.Add(acciones);
        //    GridPanel1.ColumnModel.Columns.Add(colID_Colectiva);
        //    GridPanel1.ColumnModel.Columns.Add(ColEstatus);
        //    GridPanel1.ColumnModel.Columns.Add(colTipoColectiva);
        //    GridPanel1.ColumnModel.Columns.Add(colClaveColectiva);

        //    GridPanel1.ColumnModel.Columns.Add(colNombreORazonSocial);
        //    GridPanel1.ColumnModel.Columns.Add(colAPaterno);
        //    GridPanel1.ColumnModel.Columns.Add(colAMaterno);
        //    GridPanel1.ColumnModel.Columns.Add(colRFC);
        //    GridPanel1.ColumnModel.Columns.Add(colEmail);
        //    GridPanel1.ColumnModel.Columns.Add(ColColectivaPadre);

        //    //AGREGAR EVENTOS
        //    GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));

        //    GridFilters losFiltros = new GridFilters();



        //    StringFilter filClaveColectiva = new StringFilter();
        //    filClaveColectiva.DataIndex = "ClaveColectiva";
        //    losFiltros.Filters.Add(filClaveColectiva);

        //    StringFilter filNombreORazonSocial = new StringFilter();
        //    filNombreORazonSocial.DataIndex = "NombreORazonSocial";
        //    losFiltros.Filters.Add(filNombreORazonSocial);

        //    StringFilter filAPaterno = new StringFilter();
        //    filAPaterno.DataIndex = "APaterno";
        //    losFiltros.Filters.Add(filAPaterno);

        //    StringFilter filAMaterno = new StringFilter();
        //    filAMaterno.DataIndex = "AMaterno";
        //    losFiltros.Filters.Add(filAMaterno);

        //    StringFilter filRFC = new StringFilter();
        //    filRFC.DataIndex = "RFC";
        //    losFiltros.Filters.Add(filRFC);

        //    StringFilter filEmail = new StringFilter();
        //    filEmail.DataIndex = "Email";
        //    losFiltros.Filters.Add(filEmail);

        //    GridPanel1.Plugins.Add(losFiltros);



        //}

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
            //store.DataSource = DAOFichaDeposito.ListaFichasDeposito();
            //store.DataBind();
            // PreparaGrid();
            // btnGuardar.Click +=new EventHandler(btnGuardar_Click);

        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            Panel5.Collapsed = true;
        }


        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];

                Panel5.Title = "Colectiva  No. [" + unvalor + "]";

                Int64 ID_colectiva = Int64.Parse(unvalor);


            }
            catch (Exception)
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            Panel5.Collapsed = false;
        }

        protected Colectiva ObtieneNuevaColectiva()
        {
            Colectiva laColectiva = new Colectiva();

            laColectiva.ClaveColectiva = txtUserName.Text;
            laColectiva.Password = "";
            laColectiva.TipoColectiva = new TipoColectiva(cmbTipoUsuario.Value.ToString());
            laColectiva.FechaNacimiento = (DateTime)datFecha.Value;
            laColectiva.TipoColectiva.Clave = cmbTipoUsuario.Value.ToString();
            laColectiva.ID_ColectivaPadre = Int64.Parse(cmbPadreTipoColectiva.Value.ToString());
            laColectiva.CURP = "";
            laColectiva.Email = txtemail2.Text;
            laColectiva.Telefono = txtFijo.Text;
            laColectiva.Movil = txtMovil.Text;
            laColectiva.Sexo = 1;
            laColectiva.NombreORazonSocial = txtNombre.Text;
            laColectiva.APaterno = txtApaterno.Text;
            laColectiva.AMaterno = txtAmaterno.Text;
            laColectiva.RFC = "";

            laColectiva.DUbicacion.Calle = "";
            laColectiva.DUbicacion.EntreCalles = "";
            laColectiva.DUbicacion.NumExterior = "";
            laColectiva.DUbicacion.NumInterior = "";
            laColectiva.DUbicacion.Referencias = "";
            laColectiva.DUbicacion.Asentamiento.ID_Asentamiento = "0";
            laColectiva.DUbicacion.Asentamiento.CodigoPostal = "";

            laColectiva.DFacturacion.Calle = "";
            laColectiva.DFacturacion.EntreCalles = "";
            laColectiva.DFacturacion.NumExterior = "";
            laColectiva.DFacturacion.NumInterior = "";
            laColectiva.DFacturacion.Referencias = "";
            laColectiva.DFacturacion.Asentamiento.ID_Asentamiento = "0";
            laColectiva.DFacturacion.Asentamiento.CodigoPostal = "";

            return laColectiva;


        }

    }
}