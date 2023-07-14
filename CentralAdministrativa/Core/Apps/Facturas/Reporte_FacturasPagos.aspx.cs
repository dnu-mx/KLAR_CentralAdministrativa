using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace Facturas
{
    public partial class Reporte_FacturasPagos : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Facturas y Pagos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataSet dsCadenas = DAOCatalogos.ObtieneColectivasParaFiltrosReportes(this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "ClaveTipoColectivaCadenaComercial").Valor, "", -1);

                    List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var cadenaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                            ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                            NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                        };
                        cadenaList.Add(cadenaCombo);
                    }

                    StoreCadenaComercial.DataSource = cadenaList;
                    StoreCadenaComercial.DataBind();

                    /////FACTURAS
                    //Se prestablece el periodo de búsqueda y la fecha máxima de selección
                    // al día de hoy
                    datInicioFactura.MaxDate = DateTime.Today;
                    datInicioFactura.SetValue(DateTime.Now);

                    datFinFactura.MaxDate = DateTime.Today;
                    datFinFactura.SetValue(DateTime.Now);

                    //Se consulta el catálogo de estatus
                    StoreEstatusFactura.DataSource = DAOFactura.ListaEstatusFacturas(this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreEstatusFactura.DataBind();

                    /////PAGOS
                    //Se prestablece el periodo de búsqueda y la fecha máxima de selección
                    // al día de hoy
                    datInicioPago.MaxDate = DateTime.Today;
                    datInicioPago.SetValue(DateTime.Now);

                    datFinPago.MaxDate = DateTime.Today;
                    datFinPago.SetValue(DateTime.Now);

                    //Se consulta el catálogo de estatus
                    StoreEstatusPago.DataSource = DAOFactura.ListaEstatusPagos(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                    StoreEstatusPago.DataBind();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "FACTURACION");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, invocando a la 
        /// búsqueda de los datos del reporte en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Store1.RemoveAll();

                ObtieneReporte();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);    
                X.Msg.Alert("Búsqueda de Facturas y Pagos", err.Message).Show();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void ObtieneReporte()
        {
            try
            {
                int IdCadenaComercial, IdEstatusFactura, IdEstatusPago;
                DateTime FechaInicial_Factura, FechaFinal_Factura;
                DateTime FechaInicial_Pago, FechaFinal_Pago;
                float MontoInferior = 0, MontoSuperior = 0;

                if (datInicioFactura.Value == null || datFinFactura.Value == null)
                {
                    return;
                }
                
                //Se validan las fechas de las facturas
                FechaInicial_Factura = datInicioFactura.SelectedDate;
                FechaFinal_Factura = datFinFactura.SelectedDate;
                 
                if (DateTime.Compare(FechaInicial_Factura, FechaFinal_Factura) > 0)
                {
                    throw new Exception("La fecha inicial de la factura debe ser menor o igual a la fecha final");
                }

                //Validación de la Cadena Comercial
                IdCadenaComercial = cmbCadenaComercial.Value == null ? -1 : int.Parse(cmbCadenaComercial.Value.ToString());

                //Se validan las fechas de los pagos
                FechaInicial_Pago = datInicioPago.SelectedDate == DateTime.MinValue ? DateTime.Now.AddMonths(-6) : datInicioPago.SelectedDate;
                FechaFinal_Pago = datFinPago.SelectedDate == DateTime.MinValue ? DateTime.Now : datFinPago.SelectedDate;

                if (DateTime.Compare(FechaInicial_Pago, FechaFinal_Pago) > 0)
                {
                    throw new Exception("La fecha inicial del pago debe ser menor o igual a la fecha final");
                }

                //Validación del estatus de la factura
                IdEstatusFactura = cmbEstatusFactura.Value == null ? -1 : int.Parse(cmbEstatusFactura.Value.ToString());

                //Validación del rango de montos del pago
                MontoInferior = String.IsNullOrEmpty(txtImporteInferior.Text.Trim()) ? 0 : float.Parse(txtImporteInferior.Text.Trim());
                MontoSuperior = String.IsNullOrEmpty(txtImporteSuperior.Text.Trim()) ? 99999 : float.Parse(txtImporteSuperior.Text.Trim());

                //Validación del estatus del pago
                IdEstatusPago = cmbEstatusPago.Value == null ? -1 : int.Parse(cmbEstatusPago.Value.ToString());

                DataSet dsReporte = DAOFactura.ObtieneReporteFacturasPagos(
                    FechaInicial_Factura, FechaFinal_Factura, IdEstatusFactura,
                    FechaInicial_Pago, FechaFinal_Pago, MontoInferior, MontoSuperior, IdEstatusPago,
                    IdCadenaComercial, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsReporte.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Reporte", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    GridPanelReporte.GetStore().DataSource = dsReporte;
                    GridPanelReporte.GetStore().DataBind();
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Facturas y Pagos", "Ocurrió un Error en la Consulta del Reporte").Show();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                throw err;
            }
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

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                GridPanelReporte.GetStore().RemoveAll();
                FormPanel1.Reset();
            }

            catch (Exception err)
            {
                Loguear.Error(err, "FACTURACION");
                X.Msg.Alert("Búsqueda de Operaciones", err.Message).Show();
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
            string reportName = "Polizas";

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
            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 11).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 13).SetDataType(XLCellValues.Number);
            }

            return ws;
        }
    }
}