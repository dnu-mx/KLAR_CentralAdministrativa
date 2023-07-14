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


namespace Empresarial
{
    public partial class ReporteSaldosCuentaSTP : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte de Saldos Cuentas Emisor
        private LogHeader LH_ParabRepSalCtasSTP = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Saldos Cuentas STP
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepSalCtasSTP.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepSalCtasSTP.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepSalCtasSTP.User = this.Usuario.ClaveUsuario;
            LH_ParabRepSalCtasSTP.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepSalCtasSTP);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteSaldosCuentaSTP Page_Load()");

                if (!IsPostBack)
                {
                    PagingSaldosCuentaSTP.PageSize =
                       Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_RegsPorPagina", LH_ParabRepSalCtasSTP).Valor);

                    PagingDetSaldosSTP.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepSalCtasSTP).Valor);

                    HttpContext.Current.Session.Add("DtCuentasSTP", null);
                    HttpContext.Current.Session.Add("DtDetalleCuentasSTP", null);
                }

                log.Info("TERMINA ReporteSaldosCuentaSTP Page_Load()");
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
        /// Controla el evento onRefresh del grid de saldos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreCuentasSTP_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelCuentasSTP(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de cuentas STP, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelCuentasSTP(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepSalCtasSTP);

            try
            {
                DataTable dtCuentasSTP = new DataTable();

                dtCuentasSTP = HttpContext.Current.Session["DtCuentasSTP"] as DataTable;

                if (dtCuentasSTP == null)
                {
                    unLog.Info("INICIA ReporteSaldosCuentasSTP()");
                    dtCuentasSTP = DAOCuenta.ReporteSaldosCuentasSTP(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSalCtasSTP);
                    unLog.Info("TERMINA ReporteSaldosCuentasSTP()");

                    HttpContext.Current.Session.Add("DtCuentasSTP", dtCuentasSTP);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSalCtasSTP).Valor);

                if (dtCuentasSTP.Rows.Count < 1)
                {
                    X.Msg.Alert("Saldos", "No existen Cuentas STP o no tienes permiso para verlas.").Show();
                }
                else if (dtCuentasSTP.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Saldos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosCuentasSTP.ClicDePaso()",
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
                    int TotalRegistros = dtCuentasSTP.Rows.Count;

                    (this.StoreCuentasSTP.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCuentasSTP.Clone();
                    DataTable dtToGrid = dtCuentasSTP.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCuentasSTP.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingSaldosCuentaSTP.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingSaldosCuentaSTP.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCuentasSTP.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreCuentasSTP.DataSource = dtToGrid;
                    StoreCuentasSTP.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Saldos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Saldos", "Ocurrió un error al obtener el Reporte de Saldos Cuentas STP").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasSTP")]
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
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasSTP")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteSaldosCuentasSTP";
                DataTable _dtCuentasSTP = HttpContext.Current.Session["DtCuentasSTP"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCuentasSTP, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepSalCtasSTP);
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
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de detalle
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridDetalle();

            Thread.Sleep(100);

            btnBuscarDetalleSTPHide.FireEvent("click");
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de detalle de la cuenta
        /// </summary>
        protected void LimpiaGridDetalle()
        {
            btnExportDetCtaSTPExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtDetalleCuentasSTP", null);
            StoreDetalleCuentaSTP.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridDetalleCuentasSTP(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI pCI = new LogPCI(LH_ParabRepSalCtasSTP);

            try
            {
                DataTable dtDetalleCuentasSTP = new DataTable();
                Int64 idCuenta = Int64.Parse(hdnIdCuentaSTP.Value.ToString());

                dtDetalleCuentasSTP = HttpContext.Current.Session["DtDetalleCuentasSTP"] as DataTable;

                if (dtDetalleCuentasSTP == null)
                {
                    pCI.Info("INICIA ReporteDetallePolizaCuentaEje()");
                    dtDetalleCuentasSTP = DAOCuenta.ReporteDetallePolizaCuentaEje(idCuenta,
                        datInicio.SelectedDate, datFinal.SelectedDate, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSalCtasSTP);
                    pCI.Info("TERMINA ReporteDetallePolizaCuentaEje()");

                    HttpContext.Current.Session.Add("DtDetalleCuentasSTP", dtDetalleCuentasSTP);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSalCtasSTP).Valor);

                if (dtDetalleCuentasSTP.Rows.Count < 1)
                {
                    X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtDetalleCuentasSTP.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Detalle de Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosCuentasSTP.ClicDetalleCuentaSTPDePaso()",
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
                    int TotalRegistros = dtDetalleCuentasSTP.Rows.Count;

                    (this.StoreDetalleCuentaSTP.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtDetalleCuentasSTP.Clone();
                    DataTable dtToGrid = dtDetalleCuentasSTP.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDetalleCuentasSTP.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDetSaldosSTP.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDetSaldosSTP.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtDetalleCuentasSTP.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreDetalleCuentaSTP.DataSource = dtToGrid;
                    StoreDetalleCuentaSTP.DataBind();

                    btnExportDetCtaSTPExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Movimientos", "Ocurrió un error al obtener el Detalle de Movimientos").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de detalle
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreDetalleCuentaSTP_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridDetalleCuentasSTP(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte de detalle a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasSTP")]
        public void ClicDetalleCuentaSTPDePaso()
        {
            btnDownloadDetCE.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto DownloadDetalle, sólo para poder llamar
        /// a la exportación del reporte a Excel del detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadDetalleCuentaSTP(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        /// <summary>
        /// Exporta el reporte de detalle de movimientos, previamente consultado, a un archivo Excel
        /// cuando dicho reporte excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasSTP")]
        public void ExportDTDetalleToExcel()
        {
            try
            {
                string reportName = "ReporteDetalleSaldosCuentasSTP";
                DataTable _dtDetalleCuentasSTP = HttpContext.Current.Session["DtDetalleCuentasSTP"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDetalleCuentasSTP, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsDetailSTPColumns(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_ParabRepSalCtasSTP);
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
        protected IXLWorksheet FormatWsDetailSTPColumns(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
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
        /// Controla el evento de selección de una celda del grid de detalle de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Cell_Click(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabRepSalCtasSTP);

            CellSelectionModel sm = this.GridDetalleCuentasSTP.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "ID_Poliza") != 0)
            {
                return;
            }

            try
            {
                if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                {
                    Int64 id_Poliza = Convert.ToInt64(sm.SelectedCell.Value);

                    logPCI.Info("INICIA ListaDetallePoliza()");
                    StoreDetallePolizaSTP.DataSource =
                        DAOColectiva.ListaDetallePoliza(id_Poliza, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSalCtasSTP);
                    logPCI.Info("TERMINA ListaDetallePoliza()");
                    StoreDetallePolizaSTP.DataBind();

                    PanelDetallePoliza.Title += " no. " + id_Poliza.ToString();
                    PanelDetallePoliza.Collapsed = false;
                }

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Cuenta", "Ocurrió un error al obtener el reporte").Show();
            }
        }

        /// <summary>
        /// Controla el evento de seleccionar un item del grid de saldos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Seleccionar(object sender, DirectEventArgs e)
        {
            hdnIdCuentaSTP.Value = e.ExtraParams["ID_Cuenta"];
            pnlDetalleCuentaSTP.Title = "Consulta de Detalle de Movimientos Cuenta: [" + hdnIdCuentaSTP.Value + "]";

            StoreDetalleCuentaSTP.RemoveAll();

            datInicio.SetValue(DateTime.Now);
            datFinal.SetValue(DateTime.Now);

            pnlDetalleCuentaSTP.Collapsed = false;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasSTP")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}