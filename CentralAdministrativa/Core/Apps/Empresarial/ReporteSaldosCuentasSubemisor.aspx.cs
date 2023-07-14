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
    public partial class ReporteSaldosCuentasSubemisor : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Saldos Cuentas Subemisor
        private LogHeader LH_ParabRepSaldosCtasSubem = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepSaldosCtasSubem.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepSaldosCtasSubem.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepSaldosCtasSubem.User = this.Usuario.ClaveUsuario;
            LH_ParabRepSaldosCtasSubem.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepSaldosCtasSubem);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteSaldosCuentasSubemisor Page_Load()");

                if (!IsPostBack)
                {
                    PagingSaldosCtasSubem.PageSize =
                       Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_RegsPorPagina", LH_ParabRepSaldosCtasSubem).Valor);

                    PagingDetSaldosCS.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepSaldosCtasSubem).Valor);

                    HttpContext.Current.Session.Add("DtRepCtasSubem", null);
                    HttpContext.Current.Session.Add("DtDetalleCtasSubem", null);
                }

                log.Info("TERMINA ReporteSaldosCuentasSubemisor Page_Load()");
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
        protected void StoreCuentasSubemisor_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelCuentasSubem(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de cuentas, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelCuentasSubem(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI logPCI = new LogPCI(LH_ParabRepSaldosCtasSubem);

            try
            {
                DataTable dtCuentasSubemisor = new DataTable();

                dtCuentasSubemisor = HttpContext.Current.Session["DtRepCtasSubem"] as DataTable;

                if (dtCuentasSubemisor == null)
                {
                    logPCI.Info("INICIA ReporteSaldosCuentasSubemisor()");
                    dtCuentasSubemisor = DAOCuenta.ReporteSaldosCuentasSubemisor(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosCtasSubem);
                    logPCI.Info("TERMINA ReporteSaldosCuentasSubemisor()");

                    HttpContext.Current.Session.Add("DtRepCtasSubem", dtCuentasSubemisor);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSaldosCtasSubem).Valor);

                if (dtCuentasSubemisor.Rows.Count < 1)
                {
                    X.Msg.Alert("Saldos", "No existen Cuentas o no tienes permiso para verlas.").Show();
                }
                else if (dtCuentasSubemisor.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Saldos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosCtasSubem.ClicDePaso()",
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
                    int TotalRegistros = dtCuentasSubemisor.Rows.Count;

                    (this.StoreCuentasSubemisor.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCuentasSubemisor.Clone();
                    DataTable dtToGrid = dtCuentasSubemisor.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCuentasSubemisor.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingSaldosCtasSubem.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingSaldosCtasSubem.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCuentasSubemisor.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreCuentasSubemisor.DataSource = dtToGrid;
                    StoreCuentasSubemisor.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Saldos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Saldos", "Ocurrió un error al obtener el Reporte de Saldos").Show();
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
        [DirectMethod(Namespace = "RepSaldosCtasSubem")]
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
        [DirectMethod(Namespace = "RepSaldosCtasSubem")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteSaldosCtasSub";
                DataTable _dtCuentasSubemisor = HttpContext.Current.Session["DtRepCtasSubem"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCuentasSubemisor, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepSaldosCtasSubem);
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

            btnBuscarDetalleCCHide.FireEvent("click");
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de detalle de la cuenta
        /// </summary>
        protected void LimpiaGridDetalle()
        {
            btnExportDCS_Excel.Disabled = true;

            HttpContext.Current.Session.Add("DtDetalleCtasSubem", null);
            StoreDetalleCtaSubem.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridDetalleCtasSubem(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepSaldosCtasSubem);

            try
            {
                DataTable dtDetalleCtasSubem = new DataTable();
                Int64 idCuenta = Int64.Parse(hdnIdCuentaSubem.Value.ToString());

                dtDetalleCtasSubem = HttpContext.Current.Session["DtDetalleCtasSubem"] as DataTable;

                if (dtDetalleCtasSubem == null)
                {
                    unLog.Info("INICIA ReporteDetalleCuentasSubemisor()");
                    dtDetalleCtasSubem = DAOCuenta.ReporteDetalleCuentasSubemisor(idCuenta,
                        datInicioCC.SelectedDate, datFinalCC.SelectedDate, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosCtasSubem);
                    unLog.Info("TERMINA ReporteDetalleCuentasSubemisor()");

                    if (dtDetalleCtasSubem.Rows.Count < 1)
                    {
                        X.Msg.Alert("Detalle de Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtDetalleCtasSubem = Tarjetas.EnmascaraTablaConTarjetas(dtDetalleCtasSubem, "Tarjeta", "Enmascara", LH_ParabRepSaldosCtasSubem);

                    HttpContext.Current.Session.Add("DtDetalleCtasSubem", dtDetalleCtasSubem);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepSaldosCtasSubem).Valor);

                if (dtDetalleCtasSubem.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Detalle de Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSaldosCtasSubem.ClicDCS_DePaso()",
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
                    int TotalRegistros = dtDetalleCtasSubem.Rows.Count;

                    (this.StoreDetalleCtaSubem.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtDetalleCtasSubem.Clone();
                    DataTable dtToGrid = dtDetalleCtasSubem.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDetalleCtasSubem.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDetSaldosCS.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDetSaldosCS.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtDetalleCtasSubem.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreDetalleCtaSubem.DataSource = dtToGrid;
                    StoreDetalleCtaSubem.DataBind();

                    btnExportDCS_Excel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
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
        protected void StoreDetalleCtaSubem_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridDetalleCtasSubem(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte de detalle a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCtasSubem")]
        public void ClicDCS_DePaso()
        {
            btnDownloadDetCS.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto DownloadDetalle, sólo para poder llamar
        /// a la exportación del reporte a Excel del detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadDetalleCtaSubem(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        /// <summary>
        /// Exporta el reporte de detalle de movimientos, previamente consultado, a un archivo Excel
        /// cuando dicho reporte excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCtasSubem")]
        public void ExportDTDetalleToExcel()
        {
            try
            {
                string reportName = "ReporteDetalleCtaSubem";
                DataTable _dtDetalleCuentaSubem = HttpContext.Current.Session["DtDetalleCtasSubem"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDetalleCuentaSubem, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsDetailCSColumns(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_ParabRepSaldosCtasSubem);
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
        protected IXLWorksheet FormatWsDetailCSColumns(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 11).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 13).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 14).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.Text);
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
            LogPCI logPCI = new LogPCI(LH_ParabRepSaldosCtasSubem);

            CellSelectionModel sm = this.GridDetalleCtasSubem.SelectionModel.Primary as CellSelectionModel;

            if (string.Compare(sm.SelectedCell.Name, "ID_Poliza") != 0)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(sm.SelectedCell.Value))
                {
                    Int64 id_Poliza = Convert.ToInt64(sm.SelectedCell.Value);

                    logPCI.Info("INCIA ListaDetallePoliza()");
                    StoreDetallePoliza.DataSource =
                        DAOColectiva.ListaDetallePoliza(id_Poliza, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSaldosCtasSubem);
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
            hdnIdCuentaSubem.Value = e.ExtraParams["ID_Cuenta"];
            pnlDetalleCuentaSubem.Title = "Consulta de Detalle de Movimientos Cuenta: [" + hdnIdCuentaSubem.Value + "]";

            StoreDetalleCtaSubem.RemoveAll();

            datInicioCC.SetValue(DateTime.Now);
            datFinalCC.SetValue(DateTime.Now);

            pnlDetalleCuentaSubem.Collapsed = false;
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSaldosCtasSubem")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}