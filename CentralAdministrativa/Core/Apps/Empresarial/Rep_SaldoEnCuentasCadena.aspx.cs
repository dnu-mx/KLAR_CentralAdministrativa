using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Xsl;


namespace Empresarial
{
    public partial class Rep_SaldoEnCuentasCadena : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PreparaGridDetalles();

                    PreparaGridSaldos();

                    BindDataSaldos();
                }
            }
            catch (Exception err)
            {
                DALAutorizador.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void RefreshGridSaldos(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGridSaldos();
                BindDataSaldos();
            }
            catch (Exception )
            {
            }

        }


     
        private void AgregaRecordFilesCuentas()
        {
            Store1.AddField(new RecordField("ID_Cuenta"));
            Store1.AddField(new RecordField("ID_Colectiva"));
            Store1.AddField(new RecordField("CadenaComercial"));
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
            Store1.AddField(new RecordField("DescripcionInstanciaCuenta"));
            Store1.AddField(new RecordField("ID_CCM"));

        }

        private void AgregaRecordFilesDetalleCuentas()
        {
            //Store2.AddField(new RecordField("ID_Poliza"));
            //Store2.AddField(new RecordField("FechaPoliza",RecordFieldType.Date));
            //Store2.AddField(new RecordField("Concepto"));
            //Store2.AddField(new RecordField("Referencia"));
            //Store2.AddField(new RecordField("Autorizacion"));
            //Store2.AddField(new RecordField("SaldoInicial"));
            //Store2.AddField(new RecordField("Cargo"));
            //Store2.AddField(new RecordField("Abono"));
            //Store2.AddField(new RecordField("SaldoFinal"));
            //Store2.AddField(new RecordField("SaldoFinalDetalle"));
            //Store2.AddField(new RecordField("DescripcionInstanciaCuenta"));
            //Store2.AddField(new RecordField("Sucursal"));
            //Store2.AddField(new RecordField("ReferenciaPagoServicio"));

            Store2.AddField(new RecordField("IDReporte"));
            Store2.AddField(new RecordField("ID_Poliza"));
            Store2.AddField(new RecordField("FechaValor", RecordFieldType.Date));
            Store2.AddField(new RecordField("FechaPoliza", RecordFieldType.Date));
            Store2.AddField(new RecordField("ConceptoPoliza"));
            Store2.AddField(new RecordField("Concepto"));
            Store2.AddField(new RecordField("Cargo"));
            Store2.AddField(new RecordField("Abono"));
            Store2.AddField(new RecordField("Autorizacion"));
            Store2.AddField(new RecordField("ReferenciaPagoServicio"));
            Store2.AddField(new RecordField("Sucursal"));
            Store2.AddField(new RecordField("ReferenciaNumerica"));
            Store2.AddField(new RecordField("Observaciones"));
            Store2.AddField(new RecordField("SaldoActual"));
            Store2.AddField(new RecordField("LineaCredito"));
            Store2.AddField(new RecordField("DescripcionMoneda"));
            Store2.AddField(new RecordField("FechaInicio", RecordFieldType.Date));
            Store2.AddField(new RecordField("FechaFinal", RecordFieldType.Date));
            Store2.AddField(new RecordField("SaldoInicialPeriodo"));
            Store2.AddField(new RecordField("SaldoFinalPeriodo"));
            Store2.AddField(new RecordField("SaldoAntes"));
            Store2.AddField(new RecordField("SaldoDespues"));
        }

        protected void PreparaGridDetalles()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesDetalleCuentas();


            //AGREGADO DE COLUMNAS
            //AGREGADO DE COLUMNAS
          //  ColumnModel lasColumnas = new ColumnModel();

            Column ColID = new Column();
            ColID.DataIndex = "IDReporte";
            ColID.Header = "ID";
            ColID.Width = 60;
            ColID.Sortable = true;
           // lasColumnas.Columns.Add(ColID);

            Column ColPoliza = new Column();
            ColPoliza.DataIndex = "ID_Poliza";
            ColPoliza.Header = "Poliza";
            ColPoliza.Width = 80;
            ColPoliza.Sortable = true;
            ColPoliza.Renderer.Fn = "link";
          //  lasColumnas.Columns.Add(ColPoliza);

            DateColumn colFecha = new DateColumn();
            colFecha.DataIndex = "FechaValor";
            colFecha.Header = "Fecha";
            colFecha.Width = 110;
            colFecha.Format = "yyyy-MM-dd HH:mm:ss";
            colFecha.Sortable = true;
           // lasColumnas.Columns.Add(colFecha);

            Column colConcepto = new Column();
            colConcepto.DataIndex = "Concepto";
            colConcepto.Header = "Concepto";
            colConcepto.Width = 200;
            colConcepto.Sortable = true;
          //  lasColumnas.Columns.Add(colConcepto);


            NumberColumn colCargo = new NumberColumn();
            colCargo.DataIndex = "Cargo";
            colCargo.Header = "Cargo";
            colCargo.Sortable = true;
            colCargo.Renderer.Format = RendererFormat.UsMoney;
            colCargo.Format = "$0,0.00";
            colCargo.Width = 90;
            colCargo.Align = Ext.Net.Alignment.Right;
           // lasColumnas.Columns.Add(colCargo);

            NumberColumn colAbono = new NumberColumn();
            colAbono.DataIndex = "Abono";
            colAbono.Header = "Abono";
            colAbono.Sortable = true;
            colAbono.Width = 90;
            colAbono.Align = Ext.Net.Alignment.Right;
            colAbono.Renderer.Format = RendererFormat.UsMoney;
            colAbono.Format = "$0,0.00";
         //   lasColumnas.Columns.Add(colAbono);

            Column colSaldoFinal = new Column();
            colSaldoFinal.DataIndex = "SaldoDespues";
            colSaldoFinal.Header = "Saldo Después";
            colSaldoFinal.Width = 100;
            //colSaldoFinal.Sortable = true;
            colSaldoFinal.Renderer.Format = RendererFormat.UsMoney;
            //colSaldoFinal.Format = "$0,0.00";
            colSaldoFinal.Align = Ext.Net.Alignment.Right;
          //  lasColumnas.Columns.Add(colSaldoFinal);

            Column colReferenciaPDS = new Column();
            colReferenciaPDS.DataIndex = "ReferenciaPagoServicio";
            colReferenciaPDS.Header = "Referencia";
            colReferenciaPDS.Width = 100;
            colReferenciaPDS.Sortable = true;
            colReferenciaPDS.Align = Ext.Net.Alignment.Right;
           // lasColumnas.Columns.Add(colReferenciaPDS);

            Column colAutorizacion = new Column();
            colAutorizacion.DataIndex = "Autorizacion";
            colAutorizacion.Header = "Autorización";
            colAutorizacion.Width = 90;
            colAutorizacion.Sortable = true;
            colAutorizacion.Align = Ext.Net.Alignment.Right;
           // lasColumnas.Columns.Add(colAutorizacion);

            Column colReferencia = new Column();
            colReferencia.DataIndex = "ReferenciaNumerica";
            colReferencia.Header = "Ref. Num.";
            colReferencia.Width = 80;
            colReferencia.Sortable = true;
           // lasColumnas.Columns.Add(colReferencia);

            Column colObservaciones = new Column();
            colObservaciones.DataIndex = "Observaciones";
            colObservaciones.Header = "Observaciones";
            colObservaciones.Sortable = true;
            //lasColumnas.Columns.Add(colObservaciones);


            //AGREGAR COLUMNAS
            GridDetalle.ColumnModel.Columns.Add(ColID);
            GridDetalle.ColumnModel.Columns.Add(ColPoliza);
            GridDetalle.ColumnModel.Columns.Add(colFecha);
            GridDetalle.ColumnModel.Columns.Add(colConcepto);
            GridDetalle.ColumnModel.Columns.Add(colCargo);
            GridDetalle.ColumnModel.Columns.Add(colAbono);
            GridDetalle.ColumnModel.Columns.Add(colSaldoFinal);
            GridDetalle.ColumnModel.Columns.Add(colReferenciaPDS);
            GridDetalle.ColumnModel.Columns.Add(colAutorizacion);
            GridDetalle.ColumnModel.Columns.Add(colReferencia);
            GridDetalle.ColumnModel.Columns.Add(colObservaciones);



        }
        


        protected void PreparaGridSaldos()
        {
            //LIMPIA GRID

            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesCuentas();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            ImageCommandColumn colDetalleMovimientos = new ImageCommandColumn();
            colDetalleMovimientos.Width = 10;

            ImageCommand commandVerDetalle = new ImageCommand();
            commandVerDetalle.Icon = Icon.Eyes;
            commandVerDetalle.CommandName = "verDetalles";
            colDetalleMovimientos.Commands.Add(commandVerDetalle);
            lasColumnas.Columns.Add(colDetalleMovimientos);

            GroupingSummaryColumn colID_Colectiva = new GroupingSummaryColumn();
            colID_Colectiva.DataIndex = "ID_Cuenta";
            colID_Colectiva.Header = "ID Cuenta";
            colID_Colectiva.Width = 50;
            colID_Colectiva.Sortable = true;
            colID_Colectiva.Hidden = false;
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
            colID_Cuenta.Hidden = false;
            lasColumnas.Columns.Add(colID_Cuenta);

            GroupingSummaryColumn colCuentaHabiente = new GroupingSummaryColumn();
            colCuentaHabiente.DataIndex = "CadenaComercial";
            colCuentaHabiente.Header = "Cadena Comercial";
            colCuentaHabiente.Hidden = true;
            colCuentaHabiente.Width = 50;
            colCuentaHabiente.Sortable = true;
            lasColumnas.Columns.Add(colCuentaHabiente);


            GroupingSummaryColumn colDescripcionCuenta = new GroupingSummaryColumn();
            colDescripcionCuenta.DataIndex = "DescripcionInstanciaCuenta";
            colDescripcionCuenta.Header = "Nombre de Cuenta";
            colDescripcionCuenta.Sortable = true;
            lasColumnas.Columns.Add(colDescripcionCuenta);

            GroupingSummaryColumn colLimiteCredito = new GroupingSummaryColumn();
            colLimiteCredito.DataIndex = "LimiteCredito";
            colLimiteCredito.Header = "Limite de Credito";
            colLimiteCredito.Align = Ext.Net.Alignment.Right; 
            colLimiteCredito.Sortable = true;
            colLimiteCredito.Renderer.Format = RendererFormat.UsMoney;


            lasColumnas.Columns.Add(colLimiteCredito);

            GroupingSummaryColumn colConsumos = new GroupingSummaryColumn();
            colConsumos.DataIndex = "Consumos";
            colConsumos.Header = "Consumos";
            colConsumos.Sortable = true;
            colConsumos.Renderer.Format = RendererFormat.UsMoney;
            colConsumos.Align = Ext.Net.Alignment.Right; 
            lasColumnas.Columns.Add(colConsumos);

            NumberColumn colSaldoActual = new NumberColumn();
            colSaldoActual.DataIndex = "SaldoDisponible";
            colSaldoActual.Header = "Saldo Disponible";
            colSaldoActual.Align = Ext.Net.Alignment.Right; 
            colSaldoActual.Sortable = true;

            TextField elEditorNumero = new TextField();
            elEditorNumero.AllowBlank = false;
            
            colSaldoActual.Renderer.Format = RendererFormat.UsMoney;
            colSaldoActual.Format = "$0,0.00";
            colSaldoActual.Editable = true;

            lasColumnas.Columns.Add(colSaldoActual);


            GroupingSummaryColumn colCodigoMoneda = new GroupingSummaryColumn();
            colCodigoMoneda.DataIndex = "CodigoMoneda";
            colCodigoMoneda.Header = "Moneda";
            colCodigoMoneda.Width = 80;
            colCodigoMoneda.Sortable = true;
            lasColumnas.Columns.Add(colCodigoMoneda);

            //GroupingSummaryColumn colDescripcionMoneda = new GroupingSummaryColumn();
            //colDescripcionMoneda.DataIndex = "DescripcionMoneda";
            //colDescripcionMoneda.Header = "Moneda";
            //colDescripcionMoneda.Sortable = true;
            //lasColumnas.Columns.Add(colDescripcionMoneda);

            //GroupingSummaryColumn ColBreveDescripcion = new GroupingSummaryColumn();
            //ColBreveDescripcion.DataIndex = "BreveDescripcion";
            //ColBreveDescripcion.Header = "Breve Descripción";
            //ColBreveDescripcion.Sortable = true;
            //lasColumnas.Columns.Add(ColBreveDescripcion);

            CommandColumn acciones = new CommandColumn();
            acciones.Header = "Acciones";
            acciones.Width = 30;
            //acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand Detalle = new GridCommand();
            Detalle.Icon = Icon.Overlays;
            Detalle.CommandName = "Detalle";
            Detalle.ToolTip.Text = "Consultar Detalle de Movimientos";
            acciones.Commands.Add(Detalle);

            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Cuenta", "this.getRowsValues({ selectedOnly: true })[0].ID_Cuenta", ParameterMode.Raw));
            GridDetalle.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            //AGREGAR COLUMNAS
            GridPanel1.ColumnModel.Columns.Add(colDetalleMovimientos);
            //GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colDescEstatus);
            GridPanel1.ColumnModel.Columns.Add(colAfiliacion);
            //GridPanel1.ColumnModel.Columns.Add(colEditarSaldoGrid);
            GridPanel1.ColumnModel.Columns.Add(colCuentaHabiente);
            //GridPanel1.ColumnModel.Columns.Add(ColBreveDescripcion);
            GridPanel1.ColumnModel.Columns.Add(colDescripcionCuenta);
            //GridPanel1.ColumnModel.Columns.Add(colLimiteCredito);
            //GridPanel1.ColumnModel.Columns.Add(colConsumos);
            GridPanel1.ColumnModel.Columns.Add(colSaldoActual);
            GridPanel1.ColumnModel.Columns.Add(colCodigoMoneda);
            ////GridPanel1.ColumnModel.Columns.Add(colDescripcionMoneda);
            GridPanel1.ColumnModel.Columns.Add(colID_Cuenta);
            



        }
        

        private void BindDataSaldos()
        {
            GridPanel1.GetStore().DataSource = DAOColectiva.ReporteCuentasConSaldos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));// DAOColectiva.ListaSaldosColectiva(ID_colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
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



        protected void GetEstadoCuenta(object sender, DirectEventArgs e)
        {
            try
            {

                CrystalDecisions.CrystalReports.Engine.ReportDocument report = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
         
                report.Load(Server.MapPath("SaldoCuentasConcentrado.rpt"));


                report.SetDataSource(DALAutorizador.BaseDatos.DAOColectiva.ReporteCuentasConSaldos(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Tables[0]);

                String NombreArchivo = "SaldoCta_" + DateTime.Now.ToString("HHss") + this.Usuario.UsuarioTemp.ToString() + ".pdf";
                String Path = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "URLReportes").Valor;


                report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path + NombreArchivo);


                report.Close();
                Response.Clear();
                Response.ClearContent(); 
                Response.ClearHeaders();
                Response.ContentType = "Application/PDF";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo);
                Response.TransmitFile(Path + NombreArchivo);
                Response.Flush();
                Response.End();

            }

            catch (Exception err)
            {
                DALAutorizador.Utilidades.Loguear.Error(err, "");
            }
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "Saldos";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

            //Se da el formato deseado a las columnas
            ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

            //Se prepara la respuesta
            this.Response.Clear();
            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Buffer = false;

            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte_" + reportName + ".xlsx");

            //Se envía el reporte como respuesta
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(this.Response.OutputStream);
                memoryStream.Close();
            }

            this.Response.End();
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            ws.Column(1).Hide();

            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.MainFormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);
                    break;
            }
            this.Response.End();
        }


        protected void Seleccionar(object sender, DirectEventArgs e)
        {
            try
            {
                PanelDetallePoliza.Title = "Detalle Póliza";
                PanelDetallePoliza.Collapsed = true;
                StoreDetallePoliza.RemoveAll();

                pnlDetalles.Collapse();
                GridDetalle.GetStore().RemoveAll();



                String unvalor = e.ExtraParams["ID_Cuenta"];
                //String unaAfil = e.ExtraParams["Afiliacion"];

                pnlDetalles.Title = "Consulta de Detalle de Movimientos Cuenta: [" + unvalor + "]";

                HttpContext.Current.Session.Add("ID_CuentaSaldo", unvalor);
                //HttpContext.Current.Session.Add("AfiliacionSaldo", unaAfil);

                datInicio.SetValue(DateTime.Now);

                datFinal.SetValue(DateTime.Now);

                Int64 laCuenta = Int64.Parse(unvalor);

                PreparaGridDetalles();

                GridDetalle.GetStore().DataSource = DAOColectiva.ListaPolizaDetalleCuenta(laCuenta, datInicio.SelectedDate, datFinal.SelectedDate, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridDetalle.GetStore().DataBind();
                
            }
            catch (Exception )
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            pnlDetalles.Collapsed = false;
        }

        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            pnlDetalles.Collapsed = true;
            GridDetalle.GetStore().RemoveAll();
        }

        public void ObtenerDatos(object sender, DirectEventArgs e)
        {
            try
            {
                GridDetalle.GetStore().RemoveAll();
                
                Int64 laCuenta = Int64.Parse((String)HttpContext.Current.Session["ID_CuentaSaldo"]);

                PreparaGridDetalles();

                DataSet dsDetalles = DAOColectiva.ListaPolizaDetalleCuenta(laCuenta, datInicio.SelectedDate, datFinal.SelectedDate, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsDetalles.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    GridDetalle.GetStore().DataSource = dsDetalles;
                    GridDetalle.GetStore().DataBind();
                }
            }
            catch (Exception err)
            {
                DALAutorizador.Utilidades.Loguear.Error(err, "");
            }
        }

        public void ExportarPDF(object sender, DirectEventArgs e)
        {
            try
            {

                Int64 laCuenta = Int64.Parse((String)HttpContext.Current.Session["ID_CuentaSaldo"]);

                CrystalDecisions.CrystalReports.Engine.ReportDocument report = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

                report.Load(Server.MapPath("DetallePolizaCuenta.rpt"));


                report.SetDataSource(DALAutorizador.BaseDatos.DAOColectiva.ListaPolizaDetalleCuenta(laCuenta,datInicio.SelectedDate,datFinal.SelectedDate,this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Tables[0]);
               

                String NombreArchivo = "PolizaDetalleCta_" + DateTime.Now.ToString("HHss") + this.Usuario.UsuarioTemp.ToString() + ".pdf";
                String Path = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "URLReportes").Valor;

                report.SetParameterValue("@FechaInicial", datInicio.SelectedDate);
                report.SetParameterValue("@FechaFinal", datFinal.SelectedDate);
                report.SetParameterValue("@ID_Cuenta", laCuenta);
                report.SetParameterValue("@UserTemp", this.Usuario.UsuarioTemp.ToString());
                report.SetParameterValue("@AppId", ConfigurationManager.AppSettings["IdApplication"].ToString());

                report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path + NombreArchivo);


                report.Close();
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "Application/PDF";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo);
                Response.TransmitFile(Path + NombreArchivo);
                Response.Flush();
                Response.End();

            }

            catch (Exception err)
            {
                DALAutorizador.Utilidades.Loguear.Error(err, "");
            }
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de detalle de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Cell_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = this.GridDetalle.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "ID_Poliza") != 0)
            {
                return;
            }

            try
            {
                if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                {
                    Int64 id_Poliza = Convert.ToInt64(sm.SelectedCell.Value);

                    LogHeader logTEMP = new LogHeader();
                    StoreDetallePoliza.DataSource = 
                        DAOColectiva.ListaDetallePoliza(id_Poliza, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        logTEMP);
                    StoreDetallePoliza.DataBind();

                    PanelDetallePoliza.Title += " no. " + id_Poliza.ToString();
                    PanelDetallePoliza.Collapsed = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}