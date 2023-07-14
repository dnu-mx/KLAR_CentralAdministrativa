using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Utilerias;

namespace Empresarial
{
    public partial class ReporteSaldosPorTarjeta : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Saldos por Tarjeta
        private LogHeader LH_ParabRepSaldosXTarjeta = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepSaldosXTarjeta.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepSaldosXTarjeta.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepSaldosXTarjeta.User = this.Usuario.ClaveUsuario;
            LH_ParabRepSaldosXTarjeta.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepSaldosXTarjeta);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteSaldosPorTarjeta Page_Load()");

                if (!IsPostBack)
                {
                    PagingSaldosTarjeta.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepSaldosXTarjeta).Valor);

                    PagingDetSaldosTarjeta.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepSaldosXTarjeta).Valor);

                    HttpContext.Current.Session.Add("DtSaldos", null);
                    HttpContext.Current.Session.Add("DtDetalleSaldos", null);
                }

                log.Info("TERMINA ReporteSaldosPorTarjeta Page_Load()");
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de saldos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelSaldos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepSaldosXTarjeta);
            btnExportExcel.Disabled = true;

            try
            {
                DataTable dtSaldos = new DataTable();

                dtSaldos = HttpContext.Current.Session["DtSaldos"] as DataTable;

                if (dtSaldos == null)
                {
                    unLog.Info("INICIA ReporteSaldosPorTarjeta()");
                    dtSaldos = DAOMediosAcceso.ReporteSaldosPorTarjeta(
                        this.txtNombre.Text, this.txtTarjeta.Text, this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosXTarjeta);
                    unLog.Info("TERMINA ReporteSaldosPorTarjeta()");

                    if (dtSaldos.Rows.Count < 1)
                    {
                        X.Msg.Alert("Cuentas", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }
                    else if (dtSaldos.Rows[0]["Mensaje"].ToString() != "OK")
                    {
                        X.Msg.Alert("Cuentas", dtSaldos.Rows[0]["Mensaje"].ToString()).Show();
                        return;
                    }

                    dtSaldos = Tarjetas.EnmascaraTablaConTarjetas(dtSaldos, "ClaveMA", "Enmascara", LH_ParabRepSaldosXTarjeta);
                    HttpContext.Current.Session.Add("DtSaldos", dtSaldos);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSaldosXTarjeta).Valor);

                if (dtSaldos.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Cuentas", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosPorTarjeta.ClicDePaso()",
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
                    int TotalRegistros = dtSaldos.Rows.Count;

                    (this.StoreSaldos.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtSaldos.Clone();
                    DataTable dtToGrid = dtSaldos.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtSaldos.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingSaldosTarjeta.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingSaldosTarjeta.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtSaldos.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreSaldos.DataSource = dtToGrid;
                    StoreSaldos.DataBind();

                    btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cuentas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Cuentas", "Ocurrió un error al consultar el reporte de Saldos por Tarjeta").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de saldos
        /// </summary>
        protected void LimpiaGridSaldos()
        {
            btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtSaldos", null);
            StoreSaldos.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            txtTarjeta.Reset();
            txtNombre.Clear();

            LimpiaGridSaldos();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridSaldos();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento de seleccionar un item del gri de saldos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Seleccionar(object sender, DirectEventArgs e)
        {
            hdnIdCuenta.Value = e.ExtraParams["ID_Cuenta"];
            pnlDetalles.Title += " Cuenta No. " + hdnIdCuenta.Value;

            StoreDetalle.RemoveAll();

            datInicio.SetValue(DateTime.Now);
            datFinal.SetValue(DateTime.Now);

            pnlDetalles.Collapsed = false;
        }
        
        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosPorTarjeta")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteSaldosPorTarjeta";
                DataTable _dtSaldos = HttpContext.Current.Session["DtSaldos"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSaldos, reportName);

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
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabRepSaldosXTarjeta);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Saldos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
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
            try
            {
                ws.Column(11).Hide();
                ws.Column(12).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de saldos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreSaldos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelSaldos(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosPorTarjeta")]
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
        /// Controla el evento onRefresh del grid de detalle
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreDetalle_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;
            
            LlenaGridPanelDetalle(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelDetalle(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepSaldosXTarjeta);
            btnExportaDetallesExcel.Disabled = true;

            try
            {
                DataTable dtDetalleSaldos = new DataTable();
                Int64 idCuenta = Int64.Parse(hdnIdCuenta.Value.ToString());

                dtDetalleSaldos = HttpContext.Current.Session["DtDetalleSaldos"] as DataTable;

                if (dtDetalleSaldos == null)
                {
                    unLog.Info("INICIA ReporteDetallePolizaCuenta()");
                    dtDetalleSaldos = DAOCuenta.ReporteDetallePolizaCuenta(idCuenta,
                        datInicio.SelectedDate, datFinal.SelectedDate, this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    unLog.Info("TERMINA ReporteDetallePolizaCuenta()");

                    HttpContext.Current.Session.Add("DtDetalleSaldos", dtDetalleSaldos);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSaldosXTarjeta).Valor);

                if (dtDetalleSaldos.Rows.Count < 1)
                {
                    X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtDetalleSaldos.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Detalle de Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosPorTarjeta.ClicDetalleDePaso()",
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
                    int TotalRegistros = dtDetalleSaldos.Rows.Count;

                    (this.StoreDetalle.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtDetalleSaldos.Clone();
                    DataTable dtToGrid = dtDetalleSaldos.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDetalleSaldos.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDetSaldosTarjeta.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDetSaldosTarjeta.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtDetalleSaldos.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreDetalle.DataSource = dtToGrid;
                    StoreDetalle.DataBind();

                    btnExportaDetallesExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Detalle de Movimientos", "Ocurrió un Error al Consultar el Reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de detalle
        /// </summary>
        protected void LimpiaGridDetalle()
        {
            btnExportaDetallesExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtDetalleSaldos", null);
            StoreDetalle.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en el detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarDetalle_Click(object sender, EventArgs e)
        {
            LimpiaGridDetalle();

            Thread.Sleep(100);

            btnBuscarDetalleHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en el detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarDetalle_Click(object sender, EventArgs e)
        {
            datInicio.SetValue(DateTime.Now);
            datFinal.SetValue(DateTime.Now);

            LimpiaGridDetalle();
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte de detalle a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosPorTarjeta")]
        public void ClicDetalleDePaso()
        {
            btnDownloadDetalleHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto DownloadDetalle, sólo para poder llamar
        /// a la exportación del reporte a Excel del detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadDetalle(object sender, DirectEventArgs e)
        {
            ExportDataTableDetalleToExcel();
        }

        /// <summary>
        /// Exporta el reporte de detalle de movimientos, previamente consultado, a un archivo Excel
        /// cuando dicho reporte excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosPorTarjeta")]
        public void ExportDataTableDetalleToExcel()
        {
            try
            {
                string reportName = "ReporteDetalleSaldosPorTarjeta";
                DataTable _dtDetalleSaldos = HttpContext.Current.Session["DtDetalleSaldos"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDetalleSaldos, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsDetailColumns(ws, ws.Column(1).CellsUsed().Count());

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
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabRepSaldosXTarjeta);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Movimientos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsDetailColumns(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosPorTarjeta")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}