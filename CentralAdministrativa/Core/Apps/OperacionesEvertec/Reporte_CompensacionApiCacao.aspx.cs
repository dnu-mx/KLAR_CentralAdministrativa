using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALOperacionesEvertec.BaseDatos;
using DALOperacionesEvertec.LogicaNegocio;
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

namespace OperacionesEvertec
{
    public partial class Reporte_CompensacionApiCacao : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Repote Compensación Operaciones Evertec
        private LogHeader LH_RepCompOpsEvertec = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Compensación Evertec
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_RepCompOpsEvertec.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_RepCompOpsEvertec.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_RepCompOpsEvertec.User = this.Usuario.ClaveUsuario;
            LH_RepCompOpsEvertec.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_RepCompOpsEvertec);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_CompensacionApiCacao Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA EscribeControles_MB()");
                    LNT112.EscribeControles_MB(LH_RepCompOpsEvertec);
                    log.Info("TERMINA EscribeControles_MB()");

                    log.Info("INICIA EscribeControles_MI()");
                    LNT112.EscribeControles_MI(LH_RepCompOpsEvertec);
                    log.Info("TERMINA EscribeControles_MI()");

                    dfFechaInicio.SetValue(DateTime.Now.AddDays(-3));
                    dfFechaInicio.MaxDate = DateTime.Today;
                    dfFechaInicio.MinDate = DateTime.Today.AddDays(-90);

                    dfFechaFin.SetValue(DateTime.Now);
                    dfFechaFin.MaxDate = DateTime.Today;
                    
                    PagingFechasCompensacion.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_RepCompOpsEvertec).Valor);

                    HttpContext.Current.Session.Add("DtCompOpsEvtc", null);
                }

                log.Info("TERMINA Reporte_CompensacionApiCacao Page_Load()");
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
        /// Controla el evento Click al botón Limpiar del formulario, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            dfFechaInicio.Reset();
            dfFechaFin.Reset();

            LimpiaGridFechasCompensacion();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de fechas de compensación
        /// </summary>
        protected void LimpiaGridFechasCompensacion()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtCompOpsEvtc", null);
            StoreFicheros.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridFechasCompensacion();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridFechasCompensacion(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            LogPCI unLog = new LogPCI(LH_RepCompOpsEvertec);

            try
            {
                DataTable dtFechasComp = new DataTable();

                dtFechasComp = HttpContext.Current.Session["DtCompOpsEvtc"] as DataTable;

                if (dtFechasComp == null)
                {
                    unLog.Info("INICIA ObtieneReporteCompensaciones()");
                    dtFechasComp = DAOReportesOpsEvertec.ObtieneReporteCompensaciones(
                        this.dfFechaInicio.SelectedDate, this.dfFechaFin.SelectedDate, LH_RepCompOpsEvertec);
                    unLog.Info("TERMINA ObtieneReporteCompensaciones()");

                    HttpContext.Current.Session.Add("DtCompOpsEvtc", dtFechasComp);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_RepCompOpsEvertec).Valor);

                if (dtFechasComp.Rows.Count < 1)
                {
                    X.Msg.Alert("Compensación Evertec", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtFechasComp.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Compensación Evertec", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepProcBatchOpEvtc.ClicDePaso()",
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
                    int TotalRegistros = dtFechasComp.Rows.Count;

                    (this.StoreFicheros.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtFechasComp.Clone();
                    DataTable dtToGrid = dtFechasComp.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtFechasComp.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingFechasCompensacion.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingFechasCompensacion.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtFechasComp.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreFicheros.DataSource = dtToGrid;
                    StoreFicheros.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Compensación Evertec", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Compensación Evertec", "Ocurrió un error al obtener el Reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreFicheros_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridFechasCompensacion(inicio, columna, orden);
        }

        [DirectMethod(Namespace = "RepProcBatchOpEvtc")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteCompensaciones";
                DataTable _dtProcesosBatch = HttpContext.Current.Session["DtCompOpsEvtc"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_RepCompOpsEvertec);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Compensación Evertec", "Ocurrió un error al exportar el reporte a Excel").Show();
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
                ws.Column(3).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepProcBatchOpEvtc")]
        public void ClicDePaso()
        {
            btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepProcBatchOpEvtc")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}