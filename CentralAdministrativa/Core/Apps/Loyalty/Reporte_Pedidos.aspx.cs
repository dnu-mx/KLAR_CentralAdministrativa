using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace CentroContacto
{
    public partial class Reporte_Pedidos : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LlenaSucursales();
                    LLenaTiposServicio();

                    //Se prestablecen las fechas de consulta del reporte
                    datInicio.SetValue(DateTime.Today);
                    datInicio.MaxDate = DateTime.Today;

                    datFinal.SetValue(DateTime.Today);
                    datFinal.MaxDate = DateTime.Today;

                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtPedidos", null);
                }
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de pedidos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelPedidos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            DateTime fechaInicial, fechaFinal;

            fechaInicial = Convert.ToDateTime(datInicio.SelectedDate);
            fechaFinal = Convert.ToDateTime(datFinal.SelectedDate);

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }

            btnExportXML.Disabled = true;
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            try
            {
                DataTable dtPedidos = new DataTable();

                dtPedidos = HttpContext.Current.Session["DtPedidos"] as DataTable;

                if (dtPedidos == null)
                {
                    dtPedidos = LNReportes.ReportePedidosMoshi(fechaInicial, fechaFinal,
                        String.IsNullOrEmpty(this.cmbSucursal.SelectedItem.Value) ? 0 : int.Parse(this.cmbSucursal.SelectedItem.Value),
                        String.IsNullOrEmpty(this.cmbTipoServicio.SelectedItem.Value) ? 0 : int.Parse(this.cmbTipoServicio.SelectedItem.Value),
                        this.Usuario);

                    HttpContext.Current.Session.Add("DtPedidos", dtPedidos);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtPedidos == null)
                {
                    X.Msg.Alert("Reporte de Pedidos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtPedidos.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Reporte de Pedidos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepPedidosMoshi.ClicDePaso()",
                                Text = "Aceptar"
                            },
                            No = new MessageBoxButtonConfig
                            {
                                Text = "Cancelar"
                            }
                        }).Show();
                }

                else
                {
                    int TotalRegistros = dtPedidos.Rows.Count;

                    (this.StorePedidos.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtPedidos.Clone();
                    DataTable dtToGrid = dtPedidos.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtPedidos.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtPedidos.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StorePedidos.DataSource = dtToGrid;
                    StorePedidos.DataBind();

                    btnExportXML.Disabled = false;
                    btnExportExcel.Disabled = false;
                    btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Reporte de Pedidos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Pedidos", "Ocurrió un Error al Consultar el Reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
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


        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridPedidos();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            FormPanel1.Reset();

            LimpiaGridPedidos();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Pedidos
        /// </summary>
        protected void LimpiaGridPedidos()
        {
            btnExportXML.Disabled = true;
            btnExportExcel.Disabled = true;
            btnExportCSV.Disabled = true;

            HttpContext.Current.Session.Add("DtPedidos", null);
            StorePedidos.RemoveAll();
        }

        protected void LlenaSucursales()
        {
            try
            {
                StoreSucursal.RemoveAll();
               
                var usert = this.Usuario;
                var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

                StoreSucursal.DataSource = DAOReportes.ListaSucursalesMoshi(usert, id);
                StoreSucursal.DataBind();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Pedidos", "Ocurrió un Error al Obtener la Lista de Sucursales").Show();
            }
        }


        protected void LLenaTiposServicio()
        {
            try
            {
                StoreTipoServicio.RemoveAll();

                var usert = this.Usuario;
                var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

                StoreTipoServicio.DataSource = DAOReportes.ListaTiposServicio(usert, id);
                StoreTipoServicio.DataBind();
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Pedidos", "Ocurrió un Error al Obtener la Lista de Tipos de Servicio").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de pedidos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StorePedidos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelPedidos(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepPedidosMoshi")]
        public void ClicDePaso()
        {
            btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download(object sender, DirectEventArgs e)
        {
            ExportDataTableToExcel();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepPedidosMoshi")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReportePedidos";
                DataTable _dtPedidos = HttpContext.Current.Session["DtPedidos"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtPedidos, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }

                this.Response.End();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Reporte de Pedidos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }
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
                ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 11).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 13).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 14).SetDataType(XLCellValues.Number);
                ws.Cell(rowsCounter, 15).SetDataType(XLCellValues.Number);
            }

            return ws;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepPedidosMoshi")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}