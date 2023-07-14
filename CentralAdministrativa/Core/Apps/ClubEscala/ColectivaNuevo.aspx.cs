using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALAutorizador.Utilidades;
using Ext.Net;
using System;
using System.Configuration;

namespace ClubEscala
{
    public partial class ColectivaNuevo : DALCentralAplicaciones.PaginaBaseCAPP
    {
      

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;


        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string AfiliacionClubEscala = "";

                AfiliacionClubEscala = cmbAfiliacion.Value == null ? "" : cmbAfiliacion.Value.ToString();


                if (LNColectiva.Agregar(ObtieneNuevaColectiva(), cmbAfiliacion.SelectedItem.Text, (bool)esClubEscala.Value,AfiliacionClubEscala ,
                     this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())) == 0)
                {

                    FormPanel1.Reset();
                    PreparaGridColectiva();
                    this.BindDataColectiva();
                }
            }
            catch (Exception err)
            {
                X.Msg.Alert("Guardar Colectiva", "Ocurrio un Error:"+ err.Message).Show();
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
                    
                    //BindDataColectiva();

                    LlenaTiposColectiva();
                    LlenaEstadosUbicacion();
                    LlenaEstadosFacturacion();
                    LlenaAfiliaciones();

                    LlenaCadenaComercial();

                }

                if (!X.IsAjaxRequest)
                {
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

        protected void LlenaCadenaComercial()
        {
            try
            {


                storeCadenaComercial.DataSource = DAOCatalogos.ListaColectivaPosiblePadre(0, DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaSucursal").Valor, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                storeCadenaComercial.DataBind();
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
            Store2.AddField(new RecordField("Clave"));
            Store2.AddField(new RecordField("Descripcion"));
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

        protected void LlenaEstadosUbicacion()
        {
            try
            {
                SEstado.DataSource = DAOCatalogos.ListaEstados();
                SEstado.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void LlenaMunicipiosUbicacion(String cveEstado)
        {
            try
            {
                SMunicipio.DataSource = DAOCatalogos.ListaMunicipios(cveEstado);
                SMunicipio.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void LlenaAsentamientoUbicacion(String CveEstado, String CodigoPostal, String CveMpio)
        {
            try
            {
                SAsentamiento.DataSource = DAOCatalogos.ListaAsentamientos(CveEstado, CodigoPostal, CveMpio);
                SAsentamiento.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void EstadosUbicacion_Select(object sender, DirectEventArgs e)
        {
            try
            {
                UcmbMunicipios.Clear();
                UcmbAsentamiento.Clear();
                LlenaMunicipiosUbicacion(UcmbEstado.Value.ToString());
            }
            catch (Exception)
            {
            }
        }

        protected void MunicipioUbicacion_Select(object sender, DirectEventArgs e)
        {
            try
            {
                UcmbAsentamiento.Clear();
                UtxtCodigoPostal.Text = "";
                LlenaAsentamientoUbicacion(UcmbEstado.Value.ToString(), UtxtCodigoPostal.Text, UcmbMunicipios.Value.ToString());
            }
            catch (Exception)
            {
            }
        }

        protected void AsentamientoUbicacion_Select(object sender, DirectEventArgs e)
        {
            try
            {
                Ext.Net.ComboBox elObjeto = (Ext.Net.ComboBox)sender;

                UtxtCodigoPostal.Text = elObjeto.SelectedItem.Text.Substring(0, 5);
                
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
                LlenaColectivasPosiblePadre(2 , cmbTipoUsuario.Value.ToString().Trim());
               
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
                SCPadre.DataSource = DAOCatalogos.ListaColectivaPosiblePadreUsr(ID_Padre, ClaveTipoColectivaHijo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                SCPadre.DataBind();
                
            }
            catch (Exception)
            {
            }
        }


        protected void LlenaEstadosFacturacion()
        {
            try
            {
                SFEdo.DataSource = DAOCatalogos.ListaEstados();
                SFEdo.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void LlenaMunicipiosFacturacion(String cveEstado)
        {
            try
            {
                SFMpio.DataSource = DAOCatalogos.ListaMunicipios(cveEstado);
                SFMpio.DataBind();
            }
            catch (Exception)
            {
            }
        }

        protected void LlenaAsentamientoFacturacion(String CveEstado, String CodigoPostal, String CveMpio)
        {
            try
            {
                
                SFAsen.DataSource = DAOCatalogos.ListaAsentamientos(CveEstado, CodigoPostal, CveMpio);
                SFAsen.DataBind();
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

        protected void EstadosFacturacion_Select(object sender, DirectEventArgs e)
        {
            try
            {
                FcmbMunicipio.Clear();
                FcmbAsentamiento.Clear();
                LlenaMunicipiosFacturacion(fcmbEstado.Value.ToString());
            }
            catch (Exception)
            {
            }
        }

        protected void MunicipioFacturacion_Select(object sender, DirectEventArgs e)
        {
            try
            {
                FcmbAsentamiento.Clear();
                FtxtCodigoPostal.Text = "";
                LlenaAsentamientoFacturacion(fcmbEstado.Value.ToString(), FtxtCodigoPostal.Text, FcmbMunicipio.Value.ToString());
                
            }
            catch (Exception)
            {
            }
        }

        protected void AsentamientoFacturacion_Select(object sender, DirectEventArgs e)
        {
            try
            {
                Ext.Net.ComboBox elObjeto = (Ext.Net.ComboBox)sender;

                FtxtCodigoPostal.Text = elObjeto.SelectedItem.Text.Substring(0, 5);

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
            lasColumnas.Columns.Add(colID_Colectiva);

            Column colEstatus = new Column();
            colEstatus.DataIndex = "Estatus";
            colEstatus.Header = "Estatus ";
            colEstatus.Sortable = true;
            lasColumnas.Columns.Add(colEstatus);

            Column colClaveColectiva = new Column();
            colClaveColectiva.DataIndex = "ClaveColectiva";
            colClaveColectiva.Header = "Clave";
            colClaveColectiva.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectiva);

            Column colNombreORazonSocial = new Column();
            colNombreORazonSocial.DataIndex = "NombreORazonSocial";
            colNombreORazonSocial.Header = "Nombre";
            colNombreORazonSocial.Sortable = true;
            lasColumnas.Columns.Add(colNombreORazonSocial);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "APaterno";
            colAPaterno.Header = "ApellidoPaterno";
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
            colRFC.Sortable = true;
            colRFC.Hidden = true;
            lasColumnas.Columns.Add(colRFC);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
            colEmail.Hidden = true;
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            Column colTipoColectiva = new Column();
            colTipoColectiva.DataIndex = "TipoColectiva";
            colTipoColectiva.Header = "TipoColectiva";
            colTipoColectiva.Sortable = true;
            lasColumnas.Columns.Add(colTipoColectiva);



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
            GridPanel1.ColumnModel.Columns.Add(colID_Colectiva);
            GridPanel1.ColumnModel.Columns.Add(colEstatus);
            GridPanel1.ColumnModel.Columns.Add(colTipoColectiva);
            GridPanel1.ColumnModel.Columns.Add(colClaveColectiva);
            GridPanel1.ColumnModel.Columns.Add(colNombreORazonSocial);
            GridPanel1.ColumnModel.Columns.Add(colAPaterno);
            GridPanel1.ColumnModel.Columns.Add(colAMaterno);
            GridPanel1.ColumnModel.Columns.Add(colRFC);
            GridPanel1.ColumnModel.Columns.Add(colEmail);


            //AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));

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
            GridPanel1.GetStore().DataSource = DAOCatalogos.ListaTiposColectivasDeGrupoComercial(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
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
            laColectiva.Password = txtPass1.Text;
            laColectiva.TipoColectiva = new TipoColectiva(cmbTipoUsuario.Value.ToString());
            laColectiva.FechaNacimiento = (DateTime)datFecha.Value;
            laColectiva.TipoColectiva.Clave = cmbTipoUsuario.Value.ToString();
            laColectiva.ID_ColectivaPadre = Int64.Parse(cmbPadreTipoColectiva.Value == null ? "0" : cmbPadreTipoColectiva.Value.ToString());
            laColectiva.CURP = txtCURP.Text;
            laColectiva.Email = txtemail2.Text;
            laColectiva.Telefono = txtFijo.Text;
            laColectiva.Movil = txtMovil.Text;
            laColectiva.Sexo = Int32.Parse(cmbSexo.Value == null ? "1" : cmbSexo.Value.ToString());
            laColectiva.NombreORazonSocial = txtNombre.Text;
            laColectiva.APaterno = txtApaterno.Text;
            laColectiva.AMaterno = txtAmaterno.Text;
            laColectiva.RFC = txtRFC.Text;

            laColectiva.DUbicacion.Calle = UtxtCalle.Text;
            laColectiva.DUbicacion.EntreCalles = UtxtEntreCalles.Text;
            laColectiva.DUbicacion.NumExterior = UtxtExterior.Text;
            laColectiva.DUbicacion.NumInterior = Utxtinterior.Text;
            laColectiva.DUbicacion.Referencias = UtxtReferencias.Text;
            laColectiva.DUbicacion.Asentamiento.ID_Asentamiento = (string)UcmbAsentamiento.Value;
            laColectiva.DUbicacion.Asentamiento.CodigoPostal = UtxtCodigoPostal.Text;


            laColectiva.DFacturacion.Calle = FtxtCalle.Text;
            laColectiva.DFacturacion.EntreCalles = FtxtEntreCalle.Text;
            laColectiva.DFacturacion.NumExterior = FtxtExterior.Text;
            laColectiva.DFacturacion.NumInterior = FtxtInterior.Text;
            laColectiva.DFacturacion.Referencias = FtxtReferencia.Text;
            laColectiva.DFacturacion.Asentamiento.ID_Asentamiento = (string)FcmbAsentamiento.Text;
            laColectiva.DFacturacion.Asentamiento.CodigoPostal = FtxtCodigoPostal.Text;

            return laColectiva;


        }

    }
}