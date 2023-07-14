using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace TpvWeb
{
    /// <summary>
    /// Realiza y controla la carga de la página Compensaciones sin Operación 
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class Reporte_EjecucionEdosCtaExternos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilium Reporte de Ejecución de Estados de Cuenta Exernos
        private LogHeader LH_LH_ParabRepEjecEdosCtaExt = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_LH_ParabRepEjecEdosCtaExt.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_LH_ParabRepEjecEdosCtaExt.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_LH_ParabRepEjecEdosCtaExt.User = this.Usuario.ClaveUsuario;
            LH_LH_ParabRepEjecEdosCtaExt.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_LH_ParabRepEjecEdosCtaExt);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_EjecucionEdosCtaExternos Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddMonths(-6);

                    this.dfFechaFin.MaxDate = DateTime.Today;

                    this.PagingBusqueda.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_LH_ParabRepEjecEdosCtaExt).Valor);
                    this.PagingDetalleEdoCta.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_LH_ParabRepEjecEdosCtaExt).Valor);

                    HttpContext.Current.Session.Add("DtEjecEdoCtaExt", null);
                    HttpContext.Current.Session.Add("DtDetalleEdoCtaExt", null);
                }

                log.Info("TERMINA Reporte_EjecucionEdosCtaExternos Page_Load()");
            }

            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                errRedirect = "../ErrorInicializarPagina.aspx";
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
        /// Limpia los controles asociados a la búsqueda previa
        /// </summary>
        protected void limpiaBusquedaPrevia()
        {
            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();

            LimpiaGridBusqueda();
            LimpiaGridDetalles();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de búsqueda
        /// </summary>
        protected void LimpiaGridBusqueda()
        {
            HttpContext.Current.Session.Add("DtEjecEdoCtaExt", null);
            this.StoreBusqueda.RemoveAll();
            this.btnExportExcel.Disabled = true;
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de detalles
        /// </summary>
        protected void LimpiaGridDetalles()
        {
            HttpContext.Current.Session.Add("DtDetalleEdoCtaExt", null);
            this.GridDetalleEdoCta.Title = "Detalle";
            this.GridDetalleEdoCta.Disabled = true;
            this.StoreDetalleEdoCta.RemoveAll();
            this.btnExcelDetalles.Disabled = true;
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridBusqueda();
            LimpiaGridDetalles();

            Thread.Sleep(100);

            this.btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de ejecuciones de solicitud, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelBusqueda(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_LH_ParabRepEjecEdosCtaExt);

            try
            {
                DataTable dtSolicitudes = new DataTable();
                this.btnExportExcel.Disabled = true;

                dtSolicitudes = HttpContext.Current.Session["DtEjecEdoCtaExt"] as DataTable;

                if (dtSolicitudes == null)
                {
                    unLog.Info("INICIA ObtieneReporteSolicitudesEdoCtaExternas()");
                    dtSolicitudes = DAOReportes.ObtieneReporteSolicitudesEdoCtaExternas(
                        Convert.ToDateTime(this.dfFechaInicio.Value), Convert.ToDateTime(this.dfFechaFin.Value),
                        LH_LH_ParabRepEjecEdosCtaExt);
                    unLog.Info("TERMINA ObtieneReporteSolicitudesEdoCtaExternas()");

                    if (dtSolicitudes.Rows.Count < 1)
                    {
                        X.Msg.Alert("Ejecución de Solicitudes", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    HttpContext.Current.Session.Add("DtEjecEdoCtaExt", dtSolicitudes);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtSolicitudes.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Ejecución de Solicitudes", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepEjecEdoCtaExt.ClicDePaso()",
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
                    int TotalRegistros = dtSolicitudes.Rows.Count;

                    (this.StoreBusqueda.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtSolicitudes.Clone();
                    DataTable dtToGrid = dtSolicitudes.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtSolicitudes.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingBusqueda.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingBusqueda.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtSolicitudes.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreBusqueda.DataSource = dtToGrid;
                    this.StoreBusqueda.DataBind();
                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ejecución de Solicitudes", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Ejecución de Solicitudes", "Ocurrió un error al consultar las Solicitudes").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de búsqueda
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreBusqueda_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelBusqueda(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepEjecEdoCtaExt")]
        public void ClicDePaso()
        {
            this.btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "Solicitudes";
                DataTable _dtSolicitudes = HttpContext.Current.Session["DtEjecEdoCtaExt"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSolicitudes, reportName);

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
                LogPCI pCI = new LogPCI(LH_LH_ParabRepEjecEdosCtaExt);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Ejecución de Solicitudes", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de ejecuciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                this.hdnIdArchivo.Value = e.ExtraParams["ID_Archivo"];
                this.btnBuscaDetalles.FireEvent("click");
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_LH_ParabRepEjecEdosCtaExt);
                unLog.ErrorException(ex);
                X.Msg.Alert("Ejecución de Solicitudes", "Ocurrió un error al obtener en la selección del Archivo.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscaDetalles_Click(object sender, EventArgs e)
        {
            LimpiaGridDetalles();

            Thread.Sleep(100);

            this.btnSelectDetHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalles de estados de cuenta, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridDetalleEdoCta(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_LH_ParabRepEjecEdosCtaExt);

            try
            {
                DataTable dtDetalles = new DataTable();
                this.btnExcelDetalles.Disabled = true;

                dtDetalles = HttpContext.Current.Session["DtDetalleEdoCtaExt"] as DataTable;

                if (dtDetalles == null)
                {
                    unLog.Info("INICIA ObtieneDetalleSolicitudEdoCtaExterno()");
                    dtDetalles = DAOReportes.ObtieneDetalleSolicitudEdoCtaExterno(
                        Convert.ToInt32(this.hdnIdArchivo.Value), LH_LH_ParabRepEjecEdosCtaExt);
                    unLog.Info("TERMINA ObtieneDetalleSolicitudEdoCtaExterno()");

                    if (dtDetalles.Rows.Count < 1)
                    {
                        X.Msg.Alert("Detalle de Solicitud", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    HttpContext.Current.Session.Add("DtDetalleEdoCtaExt", dtDetalles);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtDetalles.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Detalle de Solicitud", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepEjecEdoCtaExt.ClicDePaso()",
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
                    int TotalRegistros = dtDetalles.Rows.Count;

                    (this.StoreDetalleEdoCta.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtDetalles.Clone();
                    DataTable dtToGrid = dtDetalles.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDetalles.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDetalleEdoCta.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDetalleEdoCta.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtDetalles.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreDetalleEdoCta.DataSource = dtToGrid;
                    this.StoreDetalleEdoCta.DataBind();

                    this.GridDetalleEdoCta.Title += " Archivo: " + this.hdnIdArchivo.Value;
                    this.GridDetalleEdoCta.Disabled = false;
                    this.btnExcelDetalles.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Solicitud", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Detalle de Solicitud", "Ocurrió un error al consultar los Detalles de la Solicitud de Estado de Cuenta").Show();
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
        protected void StoreDetalleEdoCta_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridDetalleEdoCta(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepEjecEdoCtaExt")]
        public void ClicDetalleDePaso()
        {
            this.btnDownloadDetHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadDetalles(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "DetallesSolicitud";
                DataTable _dtDetalles = HttpContext.Current.Session["DtDetalleEdoCtaExt"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDetalles, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatDetWsColumns(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_LH_ParabRepEjecEdosCtaExt);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Solicitud", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
        protected IXLWorksheet FormatDetWsColumns(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                { 
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 20).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 27).SetDataType(XLCellValues.DateTime);
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
        [DirectMethod(Namespace = "RepEjecEdoCtaExt")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}