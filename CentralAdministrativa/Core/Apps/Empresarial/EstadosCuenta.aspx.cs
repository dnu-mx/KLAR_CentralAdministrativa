using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.LogicaNegocio;
using System.Web.Security;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using System.Configuration;
using Interfases.Exceptiones;
using DALCentralAplicaciones.Utilidades;


namespace Empresarial
{
    public partial class EstadosCuenta : DALCentralAplicaciones.PaginaBaseCAPP
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                   
                }

                if (!IsPostBack)
                {
                    PreparaGridColectiva();
                    BindDataColectiva();
                    PreparaGridEstadosCuenta();

                }

                if (!X.IsAjaxRequest)
                {
                    this.BindDataColectiva();
                }
            }
            catch (Exception err)
            {
                DALCentralAplicaciones.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
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
            catch (Exception )
            {
            }

        }

        protected void RefreshGridCorte(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                Int64 idColectiva = (Int64)HttpContext.Current.Session.Contents["EM_ID_Colectiva"];
                PreparaGridEstadosCuenta();

                BindDataCortes(idColectiva);
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




        }


        private void AgregaRecordFilesCortes()
        {
            Store1.AddField(new RecordField("ID_Cuenta"));
            Store1.AddField(new RecordField("ID_Corte"));
            Store1.AddField(new RecordField("NombreCorte"));
            Store1.AddField(new RecordField("FechaCorte"));
            Store1.AddField(new RecordField("SaldoDespuesCorte"));
            Store1.AddField(new RecordField("SaldoActual"));
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


        protected void PreparaGridEstadosCuenta()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesCortes();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Cuenta = new Column();
            colID_Cuenta.DataIndex = "ID_Cuenta";
            colID_Cuenta.Header = "ID";
            colID_Cuenta.Sortable = true;
            lasColumnas.Columns.Add(colID_Cuenta);

            Column colNombreCorte = new Column();
            colNombreCorte.DataIndex = "NombreCorte";
            colNombreCorte.Header = "Nombre Proceso Recurrente ";
            colNombreCorte.Sortable = true;
            lasColumnas.Columns.Add(colNombreCorte);

            DateColumn colFechaCorte = new DateColumn();
            colFechaCorte.DataIndex = "FechaCorte";
            colFechaCorte.Header = "Fecha de Corte";
            colFechaCorte.Format = "yyyy-m-dd";
            //colFechaCorte.Renderer.Format = RendererFormat.DateRenderer;// "yyyy-mmm-dd";
            colFechaCorte.Sortable = true;
            lasColumnas.Columns.Add(colFechaCorte);

            Column colSaldoDespuesCorte = new Column();
            colSaldoDespuesCorte.DataIndex = "SaldoDespuesCorte";
            colSaldoDespuesCorte.Header = "Saldo Después";
            colSaldoDespuesCorte.Renderer.Format = RendererFormat.UsMoney;
            colSaldoDespuesCorte.Sortable = true;
            lasColumnas.Columns.Add(colSaldoDespuesCorte);

            Column colSaldoActual = new Column();
            colSaldoActual.DataIndex = "SaldoActual";
            colSaldoActual.Header = "Saldo Actual";
            colSaldoActual.Renderer.Format = RendererFormat.UsMoney;
            colSaldoActual.Sortable = true;
            lasColumnas.Columns.Add(colSaldoActual);

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 55;
            //acciones.PrepareToolbar.Fn = "prepareToolbar";


            GridCommand Reintenta = new GridCommand();
            Reintenta.Icon = Icon.PageWhiteAcrobat;
            Reintenta.CommandName = "ObtieneEstadoCuenta";
            Reintenta.ToolTip.Text = "Obtener Estado de Cuenta";
            acciones.Commands.Add(Reintenta);



            //AGREGAR COLUMNAS
            GridPanel2.ColumnModel.Columns.Add(acciones);
            GridPanel2.ColumnModel.Columns.Add(colID_Cuenta);

            GridPanel2.ColumnModel.Columns.Add(colNombreCorte);
            GridPanel2.ColumnModel.Columns.Add(colFechaCorte);
            GridPanel2.ColumnModel.Columns.Add(colSaldoDespuesCorte);
            GridPanel2.ColumnModel.Columns.Add(colSaldoActual);



            //AGREGAR EVENTOS
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Corte", "record.data.ID_Corte", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Cuenta", "record.data.ID_Cuenta", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            GridPanel2.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();


            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "NombreCorte";
            losFiltros.Filters.Add(filClaveColectiva);

            DateFilter filNombreORazonSocial = new DateFilter();
            filNombreORazonSocial.DataIndex = "FechaCorte";
            losFiltros.Filters.Add(filNombreORazonSocial);


            GridPanel2.Plugins.Add(losFiltros);


        }

    
        private void BindDataColectiva()
        {
            //var store = this.GridPanel1.GetStore();
            GridPanel1.GetStore().DataSource = DAOCatalogos.ListaTiposColectivasDeGrupoComercialEDOCTA(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
            //store.DataSource = DAOFichaDeposito.ListaFichasDeposito();
            //store.DataBind();
            // PreparaGrid();
            // btnGuardar.Click +=new EventHandler(btnGuardar_Click);

        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 ID_Corte = Int64.Parse(e.ExtraParams["ID_Corte"]);
                Int64 ID_Cuenta = Int64.Parse(e.ExtraParams["ID_Cuenta"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (ID_Cuenta == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;


                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();


                switch (EjecutarComando)
                {
                    case "ObtieneEstadoCuenta":
                        GetEstadoCuenta(ID_Corte, ID_Cuenta);
                        break;
                }


            }
            catch (CAppException err)
            {
                X.Msg.Alert("Estados de Cuenta", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Estados de Cuenta", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }

        private void BindDataCortes(Int64 ID_Colectiva)
        {
            //var store = this.GridPanel1.GetStore();
            GridPanel2.GetStore().DataSource = DALAutorizador.BaseDatos.DAOCortes.ObtieneEncabezadoCortes(ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel2.GetStore().DataBind();


        }


        public void GetEstadoCuenta(Int64 ID_Corte, Int64 ID_Cuenta)
        {
            try
            {

                CrystalDecisions.CrystalReports.Engine.ReportDocument report = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                //CrystalDecisions.CrystalReports.Engine.ReportDocument reporte = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

                //report.Load(@"EstadoCuenta.rpt");
                report.Load(Server.MapPath("EstadoCuenta.rpt"));
                

                report.SetDataSource(DALAutorizador.BaseDatos.DAOCortes.ObtieneDetalleCortes(ID_Cuenta, ID_Corte, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Tables[0]);
                //VisorCR.ReportSource = reporte;
                //VisorCR.BestFitPage = true;
                String NombreArchivo = "Estado_Cuenta_" + DateTime.Now.ToString("yyyyMdd") + ID_Corte + ID_Cuenta + ".pdf";
                String Path = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),"URLReportes").Valor;


                report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path + NombreArchivo);
                report.Close();


                Response.Clear();
                Response.ContentType = "Application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo);
                Response.TransmitFile(Path + NombreArchivo);
                Response.Flush();
                Response.End();
                //Response.Redirect("PDF.aspx?file=" + NombreArchivo,false);

                //Page.RegisterStartupScript("Key", "<script language=\"javascript\"> window.location='PDF.aspx?file=" + NombreArchivo + "';</script>");

                
            }
           
            catch (Exception err)
            {
                DALAutorizador.Utilidades.Loguear.Error(err, "");
            }
        }



        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            Panel3.Collapsed = true;
        }

        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];
                

                // Panel5.Title = "Colectiva  No. [" + unvalor + "]";

                Int64 ID_colectiva = Int64.Parse(unvalor);

                HttpContext.Current.Session.Add("EM_ID_Colectiva", ID_colectiva);
               // DespliegaDatosColectiva(ID_colectiva); 
                PreparaGridEstadosCuenta();
                BindDataCortes(ID_colectiva);



            }
            catch (Exception )
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            Panel3.Collapsed = false;
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;


        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {


               // DAOColectiva.Modificar(AsignarNuevosDatos(new Colectiva()), this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
            }
            catch (Exception )
            {
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Panel3.Collapsed = true;
        }


        #region NoUsuados
        //protected void LlenaEstadosUbicacion()
        //{
        //    try
        //    {
        //        SEstado.DataSource = DAOCatalogos.ListaEstados();
        //        SEstado.DataBind();
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void LlenaMunicipiosUbicacion(String cveEstado)
        //{
        //    try
        //    {
        //        SMunicipio.DataSource = DAOCatalogos.ListaMunicipios(cveEstado);
        //        SMunicipio.DataBind();
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void LlenaAsentamientoUbicacion(String CveEstado, String CodigoPostal, String CveMpio)
        //{
        //    try
        //    {
        //        SAsentamiento.DataSource = DAOCatalogos.ListaAsentamientos(CveEstado, CodigoPostal, CveMpio);
        //        SAsentamiento.DataBind();
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void EstadosUbicacion_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        UcmbMunicipios.Clear();
        //        UcmbAsentamiento.Clear();
        //        LlenaMunicipiosUbicacion(UcmbEstado.Value.ToString());
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void MunicipioUbicacion_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        UcmbAsentamiento.Clear();
        //        UtxtCodigoPostal.Text = "";
        //        LlenaAsentamientoUbicacion(UcmbEstado.Value.ToString(), UtxtCodigoPostal.Text, UcmbMunicipios.Value.ToString());
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void AsentamientoUbicacion_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        Ext.Net.ComboBox elObjeto = (Ext.Net.ComboBox)sender;

        //        UtxtCodigoPostal.Text = elObjeto.SelectedItem.Text.Substring(0, 5);

        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void LlenaEstadosFacturacion()
        //{
        //    try
        //    {
        //        SFEdo.DataSource = DAOCatalogos.ListaEstados();
        //        SFEdo.DataBind();
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void LlenaMunicipiosFacturacion(String cveEstado)
        //{
        //    try
        //    {
        //        SFMpio.DataSource = DAOCatalogos.ListaMunicipios(cveEstado);
        //        SFMpio.DataBind();
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void LlenaAsentamientoFacturacion(String CveEstado, String CodigoPostal, String CveMpio)
        //{
        //    try
        //    {

        //        SFAsen.DataSource = DAOCatalogos.ListaAsentamientos(CveEstado, CodigoPostal, CveMpio);
        //        SFAsen.DataBind();
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void EstadosFacturacion_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        FcmbMunicipio.Clear();
        //        FcmbAsentamiento.Clear();
        //        LlenaMunicipiosFacturacion(fcmbEstado.Value.ToString());
        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void MunicipioFacturacion_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        FcmbAsentamiento.Clear();
        //        FtxtCodigoPostal.Text = "";
        //        LlenaAsentamientoFacturacion(fcmbEstado.Value.ToString(), FtxtCodigoPostal.Text, FcmbMunicipio.Value.ToString());

        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void AsentamientoFacturacion_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        Ext.Net.ComboBox elObjeto = (Ext.Net.ComboBox)sender;

        //        FtxtCodigoPostal.Text = elObjeto.SelectedItem.Text.Substring(0, 5);

        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}


        //protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        //{
        //    try
        //    {
        //        cmbPadreTipoColectiva.Clear();
        //        LlenaColectivasPosiblePadre(2, cmbTipoUsuario.Value.ToString().Trim());

        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}


        //protected void LlenaTiposColectiva()
        //{
        //    try
        //    {
        //        STipoColectiva.DataSource = DAOCatalogos.ListaTiposColectiva((this.Usuario).ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
        //        STipoColectiva.DataBind();

        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //}

        //protected void LlenaColectivasPosiblePadre(Int64 ID_Padre, String ClaveTipoColectivaHijo)
        //{
        //    try
        //    {
        //        SCPadre.DataSource = DAOCatalogos.ListaColectivaPosiblePadre(ID_Padre, ClaveTipoColectivaHijo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
        //        SCPadre.DataBind();

        //    }
        //    catch (Exception Error)
        //    {
        //    }
        //} 
        #endregion



        protected void DespliegaEstadosCuenta(Int64 ID_Colectiva)
        {
            DALAutorizador.Entidades.Colectiva laColectivaEditar = DAOColectiva.ObtenerColectiva(ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            Panel3.Title = "Estados de Cuenta Disponibles de la Colectiva [" + laColectivaEditar.ID_Colectiva + "]";


            HttpContext.Current.Session.Add("Colectiva", laColectivaEditar);


        }

          }
}