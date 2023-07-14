using DALCentralAplicaciones.Utilidades;
using DALCertificados.BaseDatos;
using DALCertificados.Entidades;
using DALCertificados.LogicaNegocio;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Xml.Xsl;

namespace Certificados
{
    public partial class ConsultaCertificado : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                    PreparaGrid();
                    BindDataCertificados();

                }

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }



        private void BindDataCertificados()
        {

            DataSet Terminales = DALAutorizador.BaseDatos.DAOCatalogos.ListaTerminalCadenasComercial(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            DataSet Certificados = DAOCertificado.ListaCertificados(this.Usuario.UsuarioTemp, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            Dictionary<Int64, Terminal> TerminalesTabla = LNCertificado.ObtenerTerminales(Terminales.Tables[0]);
            Dictionary<Int64, Certificado> CertificadosTabla = LNCertificado.ObtenerCertificados(Certificados.Tables[0]);

            List<Certificado> ListaCertifcados = LNCertificado.Obtener(TerminalesTabla, CertificadosTabla);
            GridPanel1.GetStore().DataSource = ListaCertifcados;
       
            GridPanel1.GetStore().DataBind();
        }

        protected void Store1_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xml":
                    string strXml = xml.OuterXml;
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xml");
                    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
                    this.Response.ContentType = "application/xml";
                    this.Response.Write(strXml);
                    break;

                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);

                    break;

                case "csv":
                    this.Response.ContentType = "application/octet-stream";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.csv");
                    XslCompiledTransform xtCsv = new XslCompiledTransform();
                    xtCsv.Load(Server.MapPath("xslFiles/Csv.xsl"));
                    xtCsv.Transform(xml, null, Response.OutputStream);

                    break;
            }
            this.Response.End();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
              
               
                    X.Msg.Notify("Creación Certificados", "NO SE CREARON LOS CERTIFICADOS").Show();
                    X.Msg.Notify("Creación Certificados", " <br />  <br /> <b> D E C L I N A D A </b> <br />  <br /> ").Show();
              

              
            }
            catch (Exception)
            {
                X.Msg.Alert("Status", "Ocurrio Un Error al crear el Certificado, Intentalo más tarde").Show();
                return;
            }

        }


        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            try
            {


            }
            catch (Exception)
            {
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Panel5.Collapsed = true;
        }

        private void AgregaRecordFiles()
        {
             
            storeCertificados.AddField(new RecordField("Sucursal"));
            storeCertificados.AddField(new RecordField("Afiliacion"));
            storeCertificados.AddField(new RecordField("Terminal"));
            storeCertificados.AddField(new RecordField("Clave"));
            storeCertificados.AddField(new RecordField("ClaveTipoColectiva"));
            storeCertificados.AddField(new RecordField("Descripcion"));
            storeCertificados.AddField(new RecordField("Clave"));
            storeCertificados.AddField(new RecordField("ID_Estatus"));
            storeCertificados.AddField(new RecordField("ID_ColectivaTerminal"));
            storeCertificados.AddField(new RecordField("ID_CadenaComercial"));
            storeCertificados.AddField(new RecordField("ID_Activacion"));
            storeCertificados.AddField(new RecordField("ID_Certificado"));
            storeCertificados.AddField(new RecordField("MAC"));
            storeCertificados.AddField(new RecordField("IDPROC"));
            storeCertificados.AddField(new RecordField("IDMB"));
            storeCertificados.AddField(new RecordField("IDWIN"));
            storeCertificados.AddField(new RecordField("FechaActivacion"));
            storeCertificados.AddField(new RecordField("UsuarioActivacion"));
            storeCertificados.AddField(new RecordField("ClaveCadena"));

            
        }

        protected void PreparaGrid()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFiles();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            CommandColumn acciones = new CommandColumn();
            acciones.Header = "Acciones";
            acciones.Width = 55;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand CrearCertificado = new GridCommand();
            CrearCertificado.Icon = Icon.ShieldAdd;
            CrearCertificado.CommandName = "Crear";
            CrearCertificado.ToolTip.Text = "Crear Certificado";

            GridCommand BloquearCertificado = new GridCommand();
            BloquearCertificado.Icon = Icon.ShieldDelete;
            BloquearCertificado.CommandName = "Bloquear";
            BloquearCertificado.ToolTip.Text = "Bloquear Certificado";

            GridCommand Bloqueado = new GridCommand();
            Bloqueado.Icon = Icon.ShieldSilver;
            Bloqueado.CommandName = "Bloqueado";
            Bloqueado.ToolTip.Text = "Certificado Bloqueado";

            GridCommand CertificadoInactivo = new GridCommand();
            CertificadoInactivo.Icon = Icon.ShieldError;
            CertificadoInactivo.CommandName = "Por Activar";
            CertificadoInactivo.ToolTip.Text = "Certificado por Activar";

            acciones.Commands.Add(CrearCertificado);
            acciones.Commands.Add(BloquearCertificado);
            acciones.Commands.Add(CertificadoInactivo);
            acciones.Commands.Add(Bloqueado);
            

            //acciones.Commands.Add(Caducado);
            //acciones.Commands.Add(indeterminado);

             
            Column col1 = new Column();
            col1.DataIndex = "Sucursal";
            col1.Header = "Sucursal";
            col1.Sortable = true;
            //col1.Hidden = true;


            Column col2 = new Column();
            col2.DataIndex = "Afiliacion";
            col2.Header = "Afiliacion";
            col2.Sortable = true;

            Column col3 = new Column();
            col3.DataIndex = "Terminal";
            col3.Header = "Terminal";
            col3.Sortable = true;
   
            Column col4 = new Column();
            col4.DataIndex = "MAC";
            col4.Header = "MAC";
            col4.Sortable = true;
            col4.Hidden = true;

            Column col5 = new Column();
            col5.DataIndex = "IDPROC";
            col5.Header = "IDPROC";
            col5.Hidden = true;
            col5.Sortable = true;

            Column col51 = new Column();
            col51.DataIndex = "IDMB";
            col51.Header = "IDMB";
            col51.Hidden = true;
            col51.Sortable = true;

            Column col52 = new Column();
            col52.DataIndex = "FechaActivacion";
            col52.Header = "FechaActivacion";
           // col52.Hidden = true;
            col52.Sortable = true;

            Column col53 = new Column();
            col53.DataIndex = "UsuarioActivacion";
            col53.Header = "UsuarioActivacion";
            //col53.Hidden = true;
            col53.Sortable = true;

            Column col6 = new Column();
            col6.DataIndex = "Clave";
            col6.Header = "No. Certificado";
           // col6.Hidden = true;
            col6.Sortable = true;

            Column col7 = new Column();
            col7.DataIndex = "ID_Estatus";
            col7.Header = "Estatus";
            col7.Hidden = true;
            col7.Sortable = true;

            Column col8 = new Column();
            col8.DataIndex = "ID_ColectivaTerminal";
            col8.Header = "Identificador";
            col8.Sortable = true;
            col8.Hidden = true;
            
            Column col9 = new Column();
            col9.DataIndex = "ID_CadenaComercial";
            col9.Header = "ID_CadenaComercial";
            col9.Sortable = true;
            col9.Hidden = true;

            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(col6);
            GridPanel1.ColumnModel.Columns.Add(col1);
            GridPanel1.ColumnModel.Columns.Add(col2);
            GridPanel1.ColumnModel.Columns.Add(col3);
            GridPanel1.ColumnModel.Columns.Add(col52);
            GridPanel1.ColumnModel.Columns.Add(col4);
            GridPanel1.ColumnModel.Columns.Add(col5);
            GridPanel1.ColumnModel.Columns.Add(col51);
            
            GridPanel1.ColumnModel.Columns.Add(col53);
            
            GridPanel1.ColumnModel.Columns.Add(col7);
            GridPanel1.ColumnModel.Columns.Add(col8);
            GridPanel1.ColumnModel.Columns.Add(col9);



            ////AGREGAR EVENTOS
            //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Certificado", "this.getRowsValues({ selectedOnly: true })[0].ID_Certificado", ParameterMode.Raw));
            //GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("Clave", "this.getRowsValues({ selectedOnly: true })[0].Clave", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();


            StringFilter fil1 = new StringFilter();
            fil1.DataIndex = "Clave";
            losFiltros.Filters.Add(fil1);

            StringFilter fil2 = new StringFilter();
            fil2.DataIndex = "Sucursal";
            losFiltros.Filters.Add(fil2);

            StringFilter fil3 = new StringFilter();
            fil3.DataIndex = "Afiliacion";
            losFiltros.Filters.Add(fil3);

            StringFilter fil4 = new StringFilter();
            fil4.DataIndex = "Terminal";
            losFiltros.Filters.Add(fil4);

            GridPanel1.Plugins.Add(losFiltros);


        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Guid ID_Certificado = new Guid(e.ExtraParams["ID_Certificado"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                Certificado unNuevoCertificado = new Certificado();
                unNuevoCertificado.Sucursal = (String)e.ExtraParams["Sucursal"];
                unNuevoCertificado.Afiliacion = (String)e.ExtraParams["Afiliacion"];
                unNuevoCertificado.Terminal = (String)e.ExtraParams["Terminal"];
                unNuevoCertificado.ClaveCadena = (String)e.ExtraParams["ClaveCadena"];
                unNuevoCertificado.ID_ColectivaTerminal = (Int64.Parse(e.ExtraParams["ID_Colectiva"]));
                unNuevoCertificado.ID_CadenaComercial = (Int64.Parse(e.ExtraParams["ID_CadenaComercial"]));
                unNuevoCertificado.UsuarioCreacion = (String)this.Usuario.ClaveUsuario;


                switch (EjecutarComando)
                {
                    case "Bloquear":
                        LNCertificado.Desactivar(ID_Certificado, this.Usuario);
                        X.Msg.Notify("Los Certificados", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();

                        PreparaGrid();
                        BindDataCertificados();
                        break;
                    case "Crear":

                        LNCertificado.CrearYAsignarColectivas(unNuevoCertificado, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                          X.Msg.Notify("Los Certificados", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();

                        PreparaGrid();
                        BindDataCertificados();
                        break; 
                    case "Activar":
                        break;
                }



            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Los Certificados", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Los Certificados", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }





        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGrid();
                // LlenaTiposColectiva();
                BindDataCertificados();
            }
            catch (Exception)
            {
            }

        }
    
    }
}