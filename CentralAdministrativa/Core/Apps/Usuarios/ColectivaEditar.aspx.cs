using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.Utilidades;
using Ext.Net;
using System;
using System.Configuration;
using System.Web;

namespace Usuarios
{
    public partial class ColectivaEditar : DALCentralAplicaciones.PaginaBaseCAPP
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
            catch (Exception )
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
            catch (Exception )
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
            lasColumnas.Columns.Add(colRFC);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
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

            FormPanel1.Collapsed = true;
        }
        
        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];

               // Panel5.Title = "Colectiva  No. [" + unvalor + "]";

                LlenaTiposColectiva();
                LlenaEstadosUbicacion();
                LlenaEstadosFacturacion();

                Int64 ID_colectiva = Int64.Parse(unvalor);
                DespliegaDatosColectiva(ID_colectiva);

                

            }
            catch (Exception )
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

              int i=  DAOColectiva.Modificar(AsignarNuevosDatos(new Colectiva()),this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

              if (i == 0)
              {
                  X.Msg.Notify("Editar", "Se han guardado los cambios <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
              }
              else
              {
                  X.Msg.Notify("Editar", "No se guardaron los cambios " ).Show();
                  X.Msg.Notify("Editar", " <br /> <b> D E C L I N A D O</b> <br />  <br /> ").Show();
              }
               // FormPanel1.Reset();
            }
            catch (Exception err)
            {
                X.Msg.Alert("Guardar Colectiva", "Ocurrio un Error:" + err.Message).Show();
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FormPanel1.Collapsed = true;
        }
        
        protected void LlenaEstadosUbicacion()
        {
            try
            {
                SEstado.DataSource = DAOCatalogos.ListaEstados();
                SEstado.DataBind();
            }
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
            {
            }
        }

        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            try
            {
                cmbPadreTipoColectiva.Clear();
                LlenaColectivasPosiblePadre(2, cmbTipoUsuario.Value.ToString().Trim());

            }
            catch (Exception )
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
            catch (Exception )
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
            catch (Exception )
            {
            }
        }

        protected void DespliegaDatosColectiva(Int64 ID_Colectiva)
        {
            Colectiva laColectivaEditar = DAOColectiva.ObtenerColectiva(ID_Colectiva, this.Usuario , Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            FormPanel1.Title = "Editando al Usuario Número [" + laColectivaEditar.ID_Colectiva + "]";
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

            UcmbEstado.SetValue(laColectivaEditar.DUbicacion.Asentamiento.ElEstado.CveEstado.Trim());
           // UcmbEstado.Text = laColectivaEditar.DUbicacion.Asentamiento.ElEstado.Descripcion;
            UcmbMunicipios.Clear();
            UcmbAsentamiento.Clear();
            LlenaMunicipiosUbicacion(UcmbEstado.Value.ToString());

            
            fcmbEstado.SetValue(laColectivaEditar.DFacturacion.Asentamiento.ElEstado.CveEstado.Trim());
            //fcmbEstado.Text = laColectivaEditar.DFacturacion.Asentamiento.ElEstado.Descripcion;
            FcmbMunicipio.Clear();
            FcmbAsentamiento.Clear();
            LlenaMunicipiosFacturacion(fcmbEstado.Value.ToString());


            UtxtCalle.Text = laColectivaEditar.DUbicacion.Calle;
            UtxtEntreCalles.Text = laColectivaEditar.DUbicacion.EntreCalles;
            UtxtExterior.Text = laColectivaEditar.DUbicacion.NumExterior;
            Utxtinterior.Text = laColectivaEditar.DUbicacion.NumInterior;
            UtxtReferencias.Text = laColectivaEditar.DUbicacion.Referencias;

          


            UcmbMunicipios.SetValue(laColectivaEditar.DUbicacion.Asentamiento.ElMunicipio.CveMunicipio.Trim());
           // UcmbMunicipios.Text = laColectivaEditar.DUbicacion.Asentamiento.ElMunicipio.DesMunicipio.Trim();
            UcmbAsentamiento.Clear();
            UtxtCodigoPostal.Text = "";
            LlenaAsentamientoUbicacion(UcmbEstado.Value.ToString(), UtxtCodigoPostal.Text, UcmbMunicipios.Value.ToString());

            FcmbMunicipio.SetValue(laColectivaEditar.DFacturacion.Asentamiento.ElMunicipio.CveMunicipio.Trim());
           // FcmbMunicipio.Text = laColectivaEditar.DFacturacion.Asentamiento.ElMunicipio.DesMunicipio.Trim();
            UtxtCodigoPostal.Text = laColectivaEditar.DUbicacion.Asentamiento.CodigoPostal;

            FcmbAsentamiento.Clear();
            FtxtCodigoPostal.Text = "";
            LlenaAsentamientoFacturacion(fcmbEstado.Value.ToString(), FtxtCodigoPostal.Text, FcmbMunicipio.Value.ToString());

            FtxtCalle.Text = laColectivaEditar.DFacturacion.Calle;

            FtxtEntreCalle.Text = laColectivaEditar.DFacturacion.EntreCalles;
            FtxtExterior.Text = laColectivaEditar.DFacturacion.NumExterior;
            FtxtInterior.Text = laColectivaEditar.DFacturacion.NumInterior;
            FtxtReferencia.Text = laColectivaEditar.DFacturacion.Referencias;

            FtxtCodigoPostal.Text = laColectivaEditar.DFacturacion.Asentamiento.CodigoPostal;
            txtAmaterno.Text = laColectivaEditar.AMaterno;
            txtApaterno.Text = laColectivaEditar.APaterno;


            FcmbAsentamiento.SetValue(laColectivaEditar.DFacturacion.Asentamiento.ID_Asentamiento.Trim());
          //  FcmbAsentamiento.Text = laColectivaEditar.DFacturacion.Asentamiento.CodigoPostal + ' ' + laColectivaEditar.DFacturacion.Asentamiento.DesAsentamiento.Trim();

            UcmbAsentamiento.SetValue(laColectivaEditar.DUbicacion.Asentamiento.ID_Asentamiento.Trim());
           // UcmbAsentamiento.Text = laColectivaEditar.DUbicacion.Asentamiento.CodigoPostal + ' ' + laColectivaEditar.DUbicacion.Asentamiento.DesAsentamiento.Trim();

        }

        protected Colectiva AsignarNuevosDatos(Colectiva laColectiva)
        {
            Colectiva ColectivaOriginal = (Colectiva)HttpContext.Current.Session.Contents["Colectiva"];

            laColectiva.ID_Colectiva = ColectivaOriginal.ID_Colectiva;
            laColectiva.ID_ColectivaPadre = Int64.Parse(cmbPadreTipoColectiva.Value.ToString());

            laColectiva.ClaveColectiva = txtUserName.Text;
            laColectiva.TipoColectiva = new TipoColectiva(cmbTipoUsuario.Value.ToString());
            laColectiva.CURP=txtCURP.Text ;
            laColectiva.Email=txtemail2.Text;
            laColectiva.Telefono=txtFijo.Text ;
            laColectiva.Movil=txtMovil.Text;
            laColectiva.NombreORazonSocial=txtNombre.Text ;
            laColectiva.APaterno = txtApaterno.Text;
            laColectiva.AMaterno = txtAmaterno.Text;
            laColectiva.RFC=txtRFC.Text;

            laColectiva.DUbicacion.Calle=UtxtCalle.Text;
            laColectiva.DUbicacion.EntreCalles=UtxtEntreCalles.Text ;
            laColectiva.DUbicacion.NumExterior= UtxtExterior.Text;
            laColectiva.DUbicacion.NumInterior=Utxtinterior.Text ;
            laColectiva.DUbicacion.Referencias=UtxtReferencias.Text;
            laColectiva.DUbicacion.ID_Direccion = ColectivaOriginal.DUbicacion.ID_Direccion;
            laColectiva.DUbicacion.Asentamiento.ID_Asentamiento = UcmbAsentamiento.SelectedIndex == -1 ? ColectivaOriginal.DUbicacion.Asentamiento.ID_Asentamiento : UcmbAsentamiento.Value.ToString();
            laColectiva.DUbicacion.Asentamiento.CodigoPostal=UtxtCodigoPostal.Text ;
            

            laColectiva.DFacturacion.Calle=FtxtCalle.Text ;
            laColectiva.DFacturacion.ID_Direccion = ColectivaOriginal.DFacturacion.ID_Direccion;
            laColectiva.DFacturacion.EntreCalles=FtxtEntreCalle.Text ;
            laColectiva.DFacturacion.NumExterior= FtxtExterior.Text;
            laColectiva.DFacturacion.NumInterior=FtxtInterior.Text ;
            laColectiva.DFacturacion.Referencias=FtxtReferencia.Text;
            laColectiva.DFacturacion.Asentamiento.ID_Asentamiento = FcmbAsentamiento.SelectedIndex == -1 ? ColectivaOriginal.DFacturacion.Asentamiento.ID_Asentamiento : FcmbAsentamiento.Value.ToString();
            laColectiva.DFacturacion.Asentamiento.CodigoPostal=FtxtCodigoPostal.Text;

            return laColectiva;


        }
    }
}