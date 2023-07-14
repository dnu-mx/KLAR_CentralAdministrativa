using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
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


namespace TpvWeb
{
    public partial class Reporte_ProcesosBatch : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Procesos Batch
        private LogHeader LH_ParabRepProcBatch = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Procesos Batch
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepProcBatch.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepProcBatch.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepProcBatch.User = this.Usuario.ClaveUsuario;
            LH_ParabRepProcBatch.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepProcBatch);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_ProcesosBatch Page_Load()");

                if (!IsPostBack)
                {
                    dfFechaInicio.MaxDate = DateTime.Today;
                    dfFechaFin.MaxDate = DateTime.Today;


                    HttpContext.Current.Session.Add("DtCatalogoProcesos", null);

                    log.Info("INICIA ListaProcesos()");
                    DataTable dtCatProcesos = DAOReportes.ListaProcesos(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepProcBatch);
                    log.Info("TERMINA ListaProcesos()");
                    StoreProcesos.DataSource = dtCatProcesos;
                    StoreProcesos.DataBind();

                    HttpContext.Current.Session.Add("DtCatalogoProcesos", dtCatProcesos);

                    log.Info("INICIA ListaEstatusFicheros()");
                    StoreEstatus.DataSource = DAOReportes.ListaEstatusFicheros(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepProcBatch);
                    log.Info("TERMINA ListaEstatusFicheros()");
                    StoreEstatus.DataBind();

                    PagingProcesosBatch.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtProcesosBatch", null);
                }

                log.Info("TERMINA Reporte_ProcesosBatch Page_Load()");
            }

            catch(CAppException)
            {
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
        /// Controla la alimentación de datos del grid de procesos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelProcesosBatch(int RegistroInicial, string Columna, SortDirection Orden)
        {
            btnExportExcel.Disabled = true;
            LogPCI logPCI = new LogPCI(LH_ParabRepProcBatch);

            try
            {
                DataTable dtProcesosBatch = new DataTable();

                dtProcesosBatch = HttpContext.Current.Session["DtProcesosBatch"] as DataTable;

                DataTable dtCatalogoProcesos = HttpContext.Current.Session["DtCatalogoProcesos"] as DataTable;

                if (dtProcesosBatch == null)
                {
                    logPCI.Info("INICIA ReporteProcesosBatch()");
                    dtProcesosBatch = DAOReportes.ObtieneReporteProcesoBatch(
                        Convert.ToDateTime(dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(dfFechaFin.SelectedDate),
                        dtCatalogoProcesos,
                        String.IsNullOrEmpty(cBoxProceso.SelectedItem.Value) ? -1 :
                        Convert.ToInt32(cBoxProceso.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxEstatus.SelectedItem.Value) ? -1 :
                        Convert.ToInt32(cBoxEstatus.SelectedItem.Value), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepProcBatch);
                    logPCI.Info("TERMINA ReporteProcesosBatch()");

                    HttpContext.Current.Session.Add("DtProcesosBatch", dtProcesosBatch);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtProcesosBatch.Rows.Count < 1)
                {
                    X.Msg.Alert("Procesos Batch", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtProcesosBatch.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Procesos Batch", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepProcBatch.ClicDePaso()",
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
                    int TotalRegistros = dtProcesosBatch.Rows.Count;

                    (this.StoreProcesosBatch.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtProcesosBatch.Clone();
                    DataTable dtToGrid = dtProcesosBatch.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtProcesosBatch.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingProcesosBatch.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingProcesosBatch.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtProcesosBatch.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreProcesosBatch.DataSource = dtToGrid;
                    StoreProcesosBatch.DataBind();

                    btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Procesos Batch", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Procesos Batch", "Error al obtener el Reporte de Procesos Batch").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de procesos batch
        /// </summary>
        protected void LimpiaGridProcesosBatch()
        {
            btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtProcesosBatch", null);
            StoreProcesosBatch.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            dfFechaInicio.Reset();
            dfFechaFin.Reset();
            cBoxProceso.Reset();
            cBoxEstatus.Reset();

            LimpiaGridProcesosBatch();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridProcesosBatch();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepProcBatch")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ProcesosBatch";
                DataTable _dtProcesosBatch = HttpContext.Current.Session["DtProcesosBatch"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtProcesosBatch, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepProcBatch);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Procesos Batch", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de procesos batch
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreProcesosBatch_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelProcesosBatch(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepProcBatch")]
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
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepProcBatch")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}