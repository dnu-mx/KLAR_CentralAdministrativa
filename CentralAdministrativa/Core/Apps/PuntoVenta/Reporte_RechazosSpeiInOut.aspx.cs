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
    public partial class Reporte_RechazosSpeiInOut : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Procesos Batch
        private LogHeader LH_ParabRepRechazosSpei = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Rechazos SPEI IN/OUT
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepRechazosSpei.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepRechazosSpei.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepRechazosSpei.User = this.Usuario.ClaveUsuario;
            LH_ParabRepRechazosSpei.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepRechazosSpei);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_RechazosSpeiInOut Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.SetValue(DateTime.Now);
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddMonths(-3);

                    this.dfFechaFin.SetValue(DateTime.Now);
                    this.dfFechaFin.MaxDate = DateTime.Today;

                    this.PagingRechazoSpeiInOut.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtRechazosSpeiInOut", null);
                }

                log.Info("TERMINA Reporte_ReporteSpeiInOut Page_Load()");
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
        /// Controla la alimentación de datos del grid de rechazos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelRechazoSpeiInOut(int RegistroInicial, string Columna, SortDirection Orden)
        {
            btnExportExcel.Disabled = true;
            LogPCI logPCI = new LogPCI(LH_ParabRepRechazosSpei);

            try
            {
                DataTable dtRechazosSpeiInOut = new DataTable();

                dtRechazosSpeiInOut = HttpContext.Current.Session["DtRechazosSpeiInOut"] as DataTable;

                if (dtRechazosSpeiInOut == null)
                {
                    logPCI.Info("INICIA ObtieneRechazosSpeiInOut()");
                    dtRechazosSpeiInOut = DAOReportes.ObtieneRechazosSpeiInOut(
                        Convert.ToDateTime(dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(dfFechaFin.SelectedDate),
                        Convert.ToInt32(this.cBoxSpei.SelectedItem.Value),
                        this.txtClaveRastreo.Text,
                        LH_ParabRepRechazosSpei);
                    logPCI.Info("TERMINA ObtieneRechazosSpeiInOut()");

                    HttpContext.Current.Session.Add("DtRechazosSpeiInOut", dtRechazosSpeiInOut);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtRechazosSpeiInOut.Rows.Count < 1)
                {
                    X.Msg.Alert("Rechazos SPEI", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtRechazosSpeiInOut.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Rechazos SPEI", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepRechSpei.ClicDePaso()",
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
                    int TotalRegistros = dtRechazosSpeiInOut.Rows.Count;

                    (this.StoreRechazosSpeiInOut.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtRechazosSpeiInOut.Clone();
                    DataTable dtToGrid = dtRechazosSpeiInOut.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRechazosSpeiInOut.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingRechazoSpeiInOut.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingRechazoSpeiInOut.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRechazosSpeiInOut.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreRechazosSpeiInOut.DataSource = dtToGrid;
                    this.StoreRechazosSpeiInOut.DataBind();

                    btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Rechazos SPEI", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Rechazos SPEI", "Error al obtener el Reporte de Rechazos SPEI").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de procesos batch
        /// </summary>
        protected void LimpiaGridRechazosSpeiInOut()
        {
            btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtRechazosSpeiInOut", null);
            StoreRechazosSpeiInOut.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();
            this.cBoxSpei.Reset();
            this.txtClaveRastreo.Reset();

            LimpiaGridRechazosSpeiInOut();
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridRechazosSpeiInOut();

            Thread.Sleep(100);

            this.btnBuscarHide.FireEvent("click");
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
                string reportName = "RechazosSpeiInOut";
                DataTable _dtRechazosSpeiInOut = HttpContext.Current.Session["DtRechazosSpeiInOut"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtRechazosSpeiInOut, reportName);

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
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabRepRechazosSpei);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Rechazos SPEI", "Ocurrió un error al exportar el reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de rechazos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreRechazosSpeiInOut_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelRechazoSpeiInOut(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepRechSpei")]
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
        [DirectMethod(Namespace = "RepRechSpei")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}