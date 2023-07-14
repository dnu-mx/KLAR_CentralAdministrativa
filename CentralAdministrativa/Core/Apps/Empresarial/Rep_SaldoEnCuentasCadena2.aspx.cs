using ClosedXML.Excel;
using CrystalDecisions.CrystalReports.Engine;
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
    public partial class  Rep_SaldoEnCuentasCadena2 : PaginaBaseCAPP
    {
      

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!base.IsPostBack)
                {
                    PreparaGridDetalles();
                    PreparaGridSaldos();
                    BindDataSaldos();
                }
            }
            catch (Exception error)
            {
                DALAutorizador.Utilidades.Loguear.Error(error, "");
            }
        }

        protected void RefreshGridSaldos(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGridSaldos();
                BindDataSaldos();
            }
            catch (Exception)
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
            Store2.AddField(new RecordField("IDReporte"));
            Store2.AddField(new RecordField("ID_Poliza"));
            Store2.AddField(new RecordField("FechaValor", RecordFieldType.Date));
            Store2.AddField(new RecordField("FechaPoliza"));
            Store2.AddField(new RecordField("ConceptoPoliza"));
            Store2.AddField(new RecordField("Concepto"));
            Store2.AddField(new RecordField("Cargo"));
            Store2.AddField(new RecordField("Abono"));
            Store2.AddField(new RecordField("SaldoAntes"));
            Store2.AddField(new RecordField("SaldoDespues"));
            Store2.AddField(new RecordField("Autorizacion"));
            Store2.AddField(new RecordField("ReferenciaPagoServicio"));
            Store2.AddField(new RecordField("ReferenciaNumerica"));
            Store2.AddField(new RecordField("Observaciones"));
            Store2.AddField(new RecordField("SaldoActual"));
            Store2.AddField(new RecordField("LineaCredito"));
            Store2.AddField(new RecordField("FechaInicio", RecordFieldType.Date));
            Store2.AddField(new RecordField("FechaFinal", RecordFieldType.Date));
            Store2.AddField(new RecordField("SaldoInicialPeriodo"));
            Store2.AddField(new RecordField("SaldoFinalPeriodo"));
            Store2.AddField(new RecordField("DescripcionMoneda"));
            Store2.AddField(new RecordField("Sucursal"));
        }

        protected void PreparaGridDetalles()
        {
            AgregaRecordFilesDetalleCuentas();
            Column column = new Column();
            column.DataIndex = "IDReporte";
            column.Header = "ID";
            column.Sortable = true;
            Column column2 = new Column();
            column2.DataIndex = "ID_Poliza";
            column2.Header = "Póliza";
            column2.Sortable = true;
            column2.Renderer.Fn = "link";
            DateColumn dateColumn = new DateColumn();
            dateColumn.DataIndex = "FechaValor";
            dateColumn.Header = "Fecha Valor";
            dateColumn.Format = "yyyy-MM-dd HH:mm:ss";
            dateColumn.Sortable = true;
            Column column3 = new Column();
            column3.DataIndex = "FechaPoliza";
            column3.Header = "Fecha Póliza";
            column3.Sortable = true;
            Column column4 = new Column();
            column4.DataIndex = "ConceptoPoliza";
            column4.Header = "Concepto Póliza";
            column4.Sortable = true;
            Column column5 = new Column();
            column5.DataIndex = "Concepto";
            column5.Header = "ConceptoEdoCta";
            column5.Sortable = true;
            NumberColumn numberColumn = new NumberColumn();
            numberColumn.DataIndex = "Cargo";
            numberColumn.Header = "Cargo";
            numberColumn.Sortable = true;
            numberColumn.Renderer.Format = RendererFormat.UsMoney;
            numberColumn.Format = "$0,0.00";
            numberColumn.Align = Alignment.Right;
            NumberColumn numberColumn2 = new NumberColumn();
            numberColumn2.DataIndex = "Abono";
            numberColumn2.Header = "Abono";
            numberColumn2.Sortable = true;
            numberColumn2.Align = Alignment.Right;
            numberColumn2.Renderer.Format = RendererFormat.UsMoney;
            numberColumn2.Format = "$0,0.00";
            Column column6 = new Column();
            column6.DataIndex = "SaldoAntes";
            column6.Header = "Saldo Antes";
            column6.Sortable = true;
            column6.Renderer.Format = RendererFormat.UsMoney;
            column6.Align = Alignment.Right;
            Column column7 = new Column();
            column7.DataIndex = "SaldoDespues";
            column7.Header = "Saldo Después";
            column7.Sortable = true;
            column7.Renderer.Format = RendererFormat.UsMoney;
            column7.Align = Alignment.Right;
            Column column8 = new Column();
            column8.DataIndex = "Autorizacion";
            column8.Header = "Autorización";
            column8.Sortable = true;
            Column column9 = new Column();
            column9.DataIndex = "ReferenciaPagoServicio";
            column9.Header = "Referencia Tx";
            column9.Sortable = true;
            Column column10 = new Column();
            column10.DataIndex = "ReferenciaNumerica";
            column10.Header = "Referencia Num.";
            column10.Sortable = true;
            Column column11 = new Column();
            column11.DataIndex = "Observaciones";
            column11.Header = "Observaciones";
            column11.Sortable = true;
            Column column12 = new Column();
            column12.DataIndex = "SaldoActual";
            column12.Header = "Saldo Actual Cta";
            column12.Sortable = true;
            column12.Renderer.Format = RendererFormat.UsMoney;
            column12.Align = Alignment.Right;
            Column column13 = new Column();
            column13.DataIndex = "LineaCredito";
            column13.Header = "Línea Crédito";
            column13.Sortable = true;
            column13.Renderer.Format = RendererFormat.UsMoney;
            column13.Align = Alignment.Right;
            DateColumn dateColumn2 = new DateColumn();
            dateColumn2.DataIndex = "FechaInicio";
            dateColumn2.Header = "Fecha Inicial";
            dateColumn2.Format = "yyyy-MM-dd";
            dateColumn2.Sortable = true;
            DateColumn dateColumn3 = new DateColumn();
            dateColumn3.DataIndex = "FechaFinal";
            dateColumn3.Header = "Fecha Final";
            dateColumn3.Format = "yyyy-MM-dd";
            dateColumn3.Sortable = true;
            Column column14 = new Column();
            column14.DataIndex = "SaldoInicialPeriodo";
            column14.Header = "Saldo Inicial Periodo";
            column14.Sortable = true;
            column14.Renderer.Format = RendererFormat.UsMoney;
            column14.Align = Alignment.Right;
            Column column15 = new Column();
            column15.DataIndex = "SaldoFinalPeriodo";
            column15.Header = "Saldo Final Periodo";
            column15.Sortable = true;
            column15.Renderer.Format = RendererFormat.UsMoney;
            column15.Align = Alignment.Right;
            Column column16 = new Column();
            column16.DataIndex = "DescripcionMoneda";
            column16.Header = "Moneda";
            column16.Sortable = true;
            Column column17 = new Column();
            column17.DataIndex = "Sucursal";
            column17.Header = "Sucursal";
            column17.Sortable = true;
            GridDetalle.ColumnModel.Columns.Add(column);
            GridDetalle.ColumnModel.Columns.Add(column2);
            GridDetalle.ColumnModel.Columns.Add(dateColumn);
            GridDetalle.ColumnModel.Columns.Add(column3);
            GridDetalle.ColumnModel.Columns.Add(column4);
            GridDetalle.ColumnModel.Columns.Add(column5);
            GridDetalle.ColumnModel.Columns.Add(numberColumn);
            GridDetalle.ColumnModel.Columns.Add(numberColumn2);
            GridDetalle.ColumnModel.Columns.Add(column6);
            GridDetalle.ColumnModel.Columns.Add(column7);
            GridDetalle.ColumnModel.Columns.Add(column8);
            GridDetalle.ColumnModel.Columns.Add(column9);
            GridDetalle.ColumnModel.Columns.Add(column10);
            GridDetalle.ColumnModel.Columns.Add(column11);
            GridDetalle.ColumnModel.Columns.Add(column12);
            GridDetalle.ColumnModel.Columns.Add(column13);
            GridDetalle.ColumnModel.Columns.Add(dateColumn2);
            GridDetalle.ColumnModel.Columns.Add(dateColumn3);
            GridDetalle.ColumnModel.Columns.Add(column14);
            GridDetalle.ColumnModel.Columns.Add(column15);
            GridDetalle.ColumnModel.Columns.Add(column16);
            GridDetalle.ColumnModel.Columns.Add(column17);
        }

        protected void PreparaGridSaldos()
        {
            AgregaRecordFilesCuentas();
            ColumnModel columnModel = new ColumnModel();
            ImageCommandColumn imageCommandColumn = new ImageCommandColumn();
            imageCommandColumn.Width = 10;
            ImageCommand imageCommand = new ImageCommand();
            imageCommand.Icon = Icon.Eyes;
            imageCommand.CommandName = "verDetalles";
            imageCommandColumn.Commands.Add(imageCommand);
            columnModel.Columns.Add(imageCommandColumn);
            GroupingSummaryColumn groupingSummaryColumn = new GroupingSummaryColumn();
            groupingSummaryColumn.DataIndex = "ID_Cuenta";
            groupingSummaryColumn.Header = "ID Cuenta";
            groupingSummaryColumn.Width = 50;
            groupingSummaryColumn.Sortable = true;
            groupingSummaryColumn.Hidden = false;
            columnModel.Columns.Add(groupingSummaryColumn);
            GroupingSummaryColumn groupingSummaryColumn2 = new GroupingSummaryColumn();
            groupingSummaryColumn2.DataIndex = "Afiliacion";
            groupingSummaryColumn2.Header = "ID Afiliacion";
            groupingSummaryColumn2.Width = 50;
            groupingSummaryColumn2.Sortable = true;
            groupingSummaryColumn2.Hidden = true;
            columnModel.Columns.Add(groupingSummaryColumn2);
            Column column = new Column();
            column.DataIndex = "CadenaComercial";
            column.Header = "CadenaComercial";
            column.Width = 50;
            column.Sortable = true;
            column.Hidden = true;
            columnModel.Columns.Add(column);
            GroupingSummaryColumn groupingSummaryColumn3 = new GroupingSummaryColumn();
            groupingSummaryColumn3.DataIndex = "ID_CCM";
            groupingSummaryColumn3.Header = "ID_CCM";
            groupingSummaryColumn3.Width = 50;
            groupingSummaryColumn3.Sortable = true;
            groupingSummaryColumn3.Hidden = true;
            columnModel.Columns.Add(groupingSummaryColumn3);
            GroupingSummaryColumn groupingSummaryColumn4 = new GroupingSummaryColumn();
            groupingSummaryColumn4.DataIndex = "EditarSaldoGrid";
            groupingSummaryColumn4.Header = "EditarSaldoGrid";
            groupingSummaryColumn4.Width = 50;
            groupingSummaryColumn4.Sortable = true;
            groupingSummaryColumn4.Hidden = true;
            columnModel.Columns.Add(groupingSummaryColumn4);
            GroupingSummaryColumn groupingSummaryColumn5 = new GroupingSummaryColumn();
            groupingSummaryColumn5.DataIndex = "DescEstatus";
            groupingSummaryColumn5.Header = "Estatus";
            groupingSummaryColumn5.Width = 50;
            groupingSummaryColumn5.Sortable = true;
            columnModel.Columns.Add(groupingSummaryColumn5);
            GroupingSummaryColumn groupingSummaryColumn6 = new GroupingSummaryColumn();
            groupingSummaryColumn6.DataIndex = "CodTipoCuentaISO";
            groupingSummaryColumn6.Header = "TipoCuentaISO";
            groupingSummaryColumn6.Width = 50;
            groupingSummaryColumn6.Sortable = true;
            groupingSummaryColumn6.Hidden = true;
            columnModel.Columns.Add(groupingSummaryColumn6);
            GroupingSummaryColumn groupingSummaryColumn7 = new GroupingSummaryColumn();
            groupingSummaryColumn7.DataIndex = "ID_Cuenta";
            groupingSummaryColumn7.Header = "ID Cuenta";
            groupingSummaryColumn7.Width = 50;
            groupingSummaryColumn7.Sortable = true;
            groupingSummaryColumn7.Hidden = false;
            columnModel.Columns.Add(groupingSummaryColumn7);
            GroupingSummaryColumn groupingSummaryColumn8 = new GroupingSummaryColumn();
            groupingSummaryColumn8.DataIndex = "CadenaComercial";
            groupingSummaryColumn8.Header = "Cadena Comercial";
            groupingSummaryColumn8.Hidden = true;
            groupingSummaryColumn8.Width = 50;
            groupingSummaryColumn8.Sortable = true;
            columnModel.Columns.Add(groupingSummaryColumn8);
            GroupingSummaryColumn groupingSummaryColumn9 = new GroupingSummaryColumn();
            groupingSummaryColumn9.DataIndex = "DescripcionInstanciaCuenta";
            groupingSummaryColumn9.Header = "Nombre de Cuenta";
            groupingSummaryColumn9.Sortable = true;
            columnModel.Columns.Add(groupingSummaryColumn9);
            GroupingSummaryColumn groupingSummaryColumn10 = new GroupingSummaryColumn();
            groupingSummaryColumn10.DataIndex = "LimiteCredito";
            groupingSummaryColumn10.Header = "Limite de Credito";
            groupingSummaryColumn10.Align = Alignment.Right;
            groupingSummaryColumn10.Sortable = true;
            groupingSummaryColumn10.Renderer.Format = RendererFormat.UsMoney;
            columnModel.Columns.Add(groupingSummaryColumn10);
            GroupingSummaryColumn groupingSummaryColumn11 = new GroupingSummaryColumn();
            groupingSummaryColumn11.DataIndex = "Consumos";
            groupingSummaryColumn11.Header = "Consumos";
            groupingSummaryColumn11.Sortable = true;
            groupingSummaryColumn11.Renderer.Format = RendererFormat.UsMoney;
            groupingSummaryColumn11.Align = Alignment.Right;
            columnModel.Columns.Add(groupingSummaryColumn11);
            NumberColumn numberColumn = new NumberColumn();
            numberColumn.DataIndex = "SaldoDisponible";
            numberColumn.Header = "Saldo Disponible";
            numberColumn.Align = Alignment.Right;
            numberColumn.Sortable = true;
            TextField textField = new TextField();
            textField.AllowBlank = false;
            numberColumn.Renderer.Format = RendererFormat.UsMoney;
            numberColumn.Format = "$0,0.00";
            numberColumn.Editable = true;
            columnModel.Columns.Add(numberColumn);
            GroupingSummaryColumn groupingSummaryColumn12 = new GroupingSummaryColumn();
            groupingSummaryColumn12.DataIndex = "CodigoMoneda";
            groupingSummaryColumn12.Header = "Moneda";
            groupingSummaryColumn12.Width = 80;
            groupingSummaryColumn12.Sortable = true;
            columnModel.Columns.Add(groupingSummaryColumn12);
            CommandColumn commandColumn = new CommandColumn();
            commandColumn.Header = "Acciones";
            commandColumn.Width = 30;
            GridCommand gridCommand = new GridCommand();
            gridCommand.Icon = Icon.Overlays;
            gridCommand.CommandName = "Detalle";
            gridCommand.ToolTip.Text = "Consultar Detalle de Movimientos";
            commandColumn.Commands.Add(gridCommand);
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Parameter("ID_Cuenta", "this.getRowsValues({ selectedOnly: true })[0].ID_Cuenta", ParameterMode.Raw));
            GridDetalle.DirectEvents.Command.ExtraParams.Add(new Parameter("Comando", "command", ParameterMode.Raw));
            GridPanel1.ColumnModel.Columns.Add(imageCommandColumn);
            GridPanel1.ColumnModel.Columns.Add(groupingSummaryColumn5);
            GridPanel1.ColumnModel.Columns.Add(groupingSummaryColumn2);
            GridPanel1.ColumnModel.Columns.Add(groupingSummaryColumn8);
            GridPanel1.ColumnModel.Columns.Add(groupingSummaryColumn9);
            GridPanel1.ColumnModel.Columns.Add(numberColumn);
            GridPanel1.ColumnModel.Columns.Add(groupingSummaryColumn12);
            GridPanel1.ColumnModel.Columns.Add(groupingSummaryColumn7);
        }

        private void BindDataSaldos()
        {
            GridPanel1.GetStore().DataSource = DAOColectiva.ReporteCuentasConSaldos(base.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel1.GetStore().DataBind();
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
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        protected void Store1_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string text = FormatType.Value.ToString();
            XmlNode xml = e.Xml;
            base.Response.Clear();
            switch (text)
            {
                case "xml":
                    {
                        string outerXml = xml.OuterXml;
                        base.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xml");
                        base.Response.AddHeader("Content-Length", outerXml.Length.ToString());
                        base.Response.ContentType = "application/xml";
                        base.Response.Write(outerXml);
                        break;
                    }
                case "xls":
                    {
                        base.Response.ContentType = "application/vnd.ms-excel";
                        base.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xls");
                        XslCompiledTransform xslCompiledTransform2 = new XslCompiledTransform();
                        xslCompiledTransform2.Load(base.Server.MapPath("xslFiles/Excel.xsl"));
                        xslCompiledTransform2.Transform(xml, null, base.Response.OutputStream);
                        break;
                    }
                case "csv":
                    {
                        base.Response.ContentType = "application/octet-stream";
                        base.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.csv");
                        XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
                        xslCompiledTransform.Load(base.Server.MapPath("xslFiles/Csv.xsl"));
                        xslCompiledTransform.Transform(xml, null, base.Response.OutputStream);
                        break;
                    }
            }
            base.Response.End();
        }

        protected void GetEstadoCuenta(object sender, DirectEventArgs e)
        {
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_0008: Expected O, but got Unknown
            try
            {
                ReportDocument val = new ReportDocument();
                val.Load(base.Server.MapPath("SaldoCuentasConcentrado.rpt"));
                val.SetDataSource(DAOColectiva.ReporteCuentasConSaldos(base.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Tables[0]);
                string str = "SaldoCta_" + DateTime.Now.ToString("HHss") + base.Usuario.UsuarioTemp.ToString() + ".pdf";
                string valor = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "URLReportes").Valor;
                val.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, valor + str);
                val.Close();
                base.Response.Clear();
                base.Response.ClearContent();
                base.Response.ClearHeaders();
                base.Response.ContentType = "Application/PDF";
                base.Response.AddHeader("Content-Disposition", "attachment; filename=" + str);
                base.Response.TransmitFile(valor + str);
                base.Response.Flush();
                base.Response.End();
            }
            catch (Exception error)
            {
                DALAutorizador.Utilidades.Loguear.Error(error, "");
            }
        }

        protected void Seleccionar(object sender, DirectEventArgs e)
        {
            try
            {
                string text = e.ExtraParams["ID_Cuenta"];
                pnlDetalles.Title = "Consulta de Detalle de Movimientos  Cuenta: [" + text + "]";
                HttpContext.Current.Session.Add("ID_CuentaSaldo", text);
                datInicio.SetValue(DateTime.Now);
                datFinal.SetValue(DateTime.Now);
                long iD_Cuenta = long.Parse(text);

                PreparaGridDetalles();
                GridDetalle.GetStore().DataSource = DAOColectiva.ListaPolizaDetalleCuenta(
                    iD_Cuenta, datInicio.SelectedDate, datFinal.SelectedDate, base.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridDetalle.GetStore().DataBind();

                PanelDetallePoliza.Title = "Detalle Póliza";
                StoreDetallePoliza.RemoveAll();
            }
            catch (Exception)
            {
            }
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

                long iD_Cuenta = long.Parse((string)HttpContext.Current.Session["ID_CuentaSaldo"]);
                PreparaGridDetalles();

                DataSet dsDetalle = DAOColectiva.ListaPolizaDetalleCuenta(iD_Cuenta, 
                    datInicio.SelectedDate, datFinal.SelectedDate, base.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsDetalle.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }

                GridDetalle.GetStore().DataSource = dsDetalle;
                GridDetalle.GetStore().DataBind();
            }
            catch (Exception error)
            {
                DALAutorizador.Utilidades.Loguear.Error(error, "");
            }
        }

        public void ExportarPDF(object sender, DirectEventArgs e)
        {
            //IL_0021: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Expected O, but got Unknown
            try
            {
                long num = long.Parse((string)HttpContext.Current.Session["ID_CuentaSaldo"]);
                ReportDocument val = new ReportDocument();
                val.Load(base.Server.MapPath("DetallePolizaCuenta.rpt"));
                val.SetDataSource(DAOColectiva.ListaPolizaDetalleCuenta(num, datInicio.SelectedDate, datFinal.SelectedDate, base.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())).Tables[0]);
                string str = "PolizaDetalleCta_" + DateTime.Now.ToString("HHss") + base.Usuario.UsuarioTemp.ToString() + ".pdf";
                string valor = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "URLReportes").Valor;
                val.SetParameterValue("@FechaInicial", (object)datInicio.SelectedDate);
                val.SetParameterValue("@FechaFinal", (object)datFinal.SelectedDate);
                val.SetParameterValue("@ID_Cuenta", (object)num);
                val.SetParameterValue("@UserTemp", (object)base.Usuario.UsuarioTemp.ToString());
                val.SetParameterValue("@AppId", (object)ConfigurationManager.AppSettings["IdApplication"].ToString());
                val.ExportToDisk( CrystalDecisions.Shared.ExportFormatType.Excel, valor + str);
                val.Close();
                base.Response.Clear();
                base.Response.ClearContent();
                base.Response.ClearHeaders();
                base.Response.ContentType = "Application/PDF";
                base.Response.AddHeader("Content-Disposition", "attachment; filename=" + str);
                base.Response.TransmitFile(valor + str);
                base.Response.Flush();
                base.Response.End();
            }
            catch (Exception error)
            {
                DALAutorizador.Utilidades.Loguear.Error(error, "");
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