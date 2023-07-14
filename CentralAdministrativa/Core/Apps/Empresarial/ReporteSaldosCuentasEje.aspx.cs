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
    public partial class ReporteSaldosCuentasEje : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Saldos Cuentas Eje
        private LogHeader LH_ParabRepSaldosCtasEje = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepSaldosCtasEje.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepSaldosCtasEje.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepSaldosCtasEje.User = this.Usuario.ClaveUsuario;
            LH_ParabRepSaldosCtasEje.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepSaldosCtasEje);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteSaldosCuentasEje Page_Load()");

                if (!IsPostBack)
                {
                    PagingSaldosCuentasEje.PageSize =
                       Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_RegsPorPagina", LH_ParabRepSaldosCtasEje).Valor);

                    PagingDetSaldosCE.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepSaldosCtasEje).Valor);

                    HttpContext.Current.Session.Add("DtCuentasEje", null);
                    HttpContext.Current.Session.Add("DtDetalleCuentasEje", null);
                }

                log.Info("TERMINA ReporteSaldosCuentasEje Page_Load()");
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
        protected void StoreCuentasEje_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelCuentasEje(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de cuentas eje, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelCuentasEje(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepSaldosCtasEje);

            try
            {
                DataTable dtCuentasEje = new DataTable();

                dtCuentasEje = HttpContext.Current.Session["DtCuentasEje"] as DataTable;

                if (dtCuentasEje == null)
                {
                    unLog.Info("INICIA ReporteSaldosCuentasEje()");
                    dtCuentasEje = DAOCuenta.ReporteSaldosCuentasEje(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosCtasEje);
                    unLog.Info("TERMINA ReporteSaldosCuentasEje()");

                    HttpContext.Current.Session.Add("DtCuentasEje", dtCuentasEje);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSaldosCtasEje).Valor);

                if (dtCuentasEje.Rows.Count < 1)
                {
                    X.Msg.Alert("Saldos", "No existen Cuentas Eje o no tienes permiso para verlas.").Show();
                }
                else if (dtCuentasEje.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Saldos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosCuentasEje.ClicDePaso()",
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
                    int TotalRegistros = dtCuentasEje.Rows.Count;

                    (this.StoreCuentasEje.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCuentasEje.Clone();
                    DataTable dtToGrid = dtCuentasEje.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCuentasEje.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingSaldosCuentasEje.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingSaldosCuentasEje.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCuentasEje.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreCuentasEje.DataSource = dtToGrid;
                    StoreCuentasEje.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Saldos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Saldos", "Ocurrió un error al obtener el reporte").Show();
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
        [DirectMethod(Namespace = "RepSaldosCuentasEje")]
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
        [DirectMethod(Namespace = "RepSaldosCuentasEje")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteSaldosCuentasEje";
                DataTable _dtCuentasEje = HttpContext.Current.Session["DtCuentasEje"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCuentasEje, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepSaldosCtasEje);
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

            btnBuscarDetalleCEHide.FireEvent("click");
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de detalle de la cuenta
        /// </summary>
        protected void LimpiaGridDetalle()
        {
            btnExportDetCuentaEjeExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtDetalleCuentasEje", null);
            StoreDetalleCuentaEje.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridDetalleCuentasEje(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI pCI = new LogPCI(LH_ParabRepSaldosCtasEje);

            try
            {
                DataTable dtDetalleCuentasEje = new DataTable();
                Int64 idCuenta = Int64.Parse(hdnIdCuentaEje.Value.ToString());

                dtDetalleCuentasEje = HttpContext.Current.Session["DtDetalleCuentasEje"] as DataTable;

                if (dtDetalleCuentasEje == null)
                {
                    pCI.Info("INICIA ReporteDetallePolizaCuentaEje()");
                    dtDetalleCuentasEje = DAOCuenta.ReporteDetallePolizaCuentaEje(idCuenta,
                        datInicio.SelectedDate, datFinal.SelectedDate, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosCtasEje);
                    pCI.Info("TERMINA ReporteDetallePolizaCuentaEje()");

                    HttpContext.Current.Session.Add("DtDetalleCuentasEje", dtDetalleCuentasEje);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSaldosCtasEje).Valor);

                if (dtDetalleCuentasEje.Rows.Count < 1)
                {
                    X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtDetalleCuentasEje.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Detalle de Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosCuentasEje.ClicDetalleCuentaEjeDePaso()",
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
                    int TotalRegistros = dtDetalleCuentasEje.Rows.Count;

                    (this.StoreDetalleCuentaEje.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtDetalleCuentasEje.Clone();
                    DataTable dtToGrid = dtDetalleCuentasEje.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDetalleCuentasEje.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDetSaldosCE.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDetSaldosCE.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtDetalleCuentasEje.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreDetalleCuentaEje.DataSource = dtToGrid;
                    StoreDetalleCuentaEje.DataBind();

                    btnExportDetCuentaEjeExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Movimientos", "Ocurrió un error al obtener el reporte").Show();
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
        protected void StoreDetalleCuentaEje_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridDetalleCuentasEje(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte de detalle a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasEje")]
        public void ClicDetalleCuentaEjeDePaso()
        {
            btnDownloadDetCE.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto DownloadDetalle, sólo para poder llamar
        /// a la exportación del reporte a Excel del detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadDetalleCuentaEje(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        /// <summary>
        /// Exporta el reporte de detalle de movimientos, previamente consultado, a un archivo Excel
        /// cuando dicho reporte excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasEje")]
        public void ExportDTDetalleToExcel()
        {
            try
            {
                string reportName = "ReporteDetalleSaldosCuentasEje";
                DataTable _dtDetalleCuentasEje = HttpContext.Current.Session["DtDetalleCuentasEje"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDetalleCuentasEje, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsDetailCEColumns(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_ParabRepSaldosCtasEje);
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
        protected IXLWorksheet FormatWsDetailCEColumns(IXLWorksheet ws, int rowsNum)
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
        /// Controla el evento de selección de una celda del grid de detalle de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Cell_Click(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabRepSaldosCtasEje);

            CellSelectionModel sm = this.GridDetalleCuentasEje.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "ID_Poliza") != 0)
            {
                return;
            }

            try
            {
                if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
                {
                    Int64 id_Poliza = Convert.ToInt64(sm.SelectedCell.Value);

                    logPCI.Info("INCIA ListaDetallePoliza()");
                    StoreDetallePoliza.DataSource =
                        DAOColectiva.ListaDetallePoliza(id_Poliza, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosCtasEje);
                    logPCI.Info("TERMINA ListaDetallePoliza()");
                    StoreDetallePoliza.DataBind();

                    PanelDetallePoliza.Title += " no. " + id_Poliza.ToString();
                    PanelDetallePoliza.Collapsed = false;
                }

            }
            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Póliza", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Póliza", "Ocurrió un error al obtener el reporte").Show();
            }
        }

        /// <summary>
        /// Controla el evento de seleccionar un item del grid de saldos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void Seleccionar(object sender, DirectEventArgs e)
        {
            hdnIdCuentaEje.Value = e.ExtraParams["ID_Cuenta"];
            pnlDetalleCuentaEje.Title = "Consulta de Detalle de Movimientos Cuenta: [" + hdnIdCuentaEje.Value + "]";

            StoreDetalleCuentaEje.RemoveAll();

            datInicio.SetValue(DateTime.Now);
            datFinal.SetValue(DateTime.Now);

            pnlDetalleCuentaEje.Collapsed = false;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCuentasEje")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}