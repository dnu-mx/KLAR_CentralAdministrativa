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
    public partial class ReporteResumenTransacciones : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Resumen de Transacciones
        private LogHeader LH_ParabRepResumenTXs = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Resumen de Transacciones Activadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepResumenTXs.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepResumenTXs.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepResumenTXs.User = this.Usuario.ClaveUsuario;
            LH_ParabRepResumenTXs.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepResumenTXs);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteResumenTransacciones Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaIniResTX.MaxDate = DateTime.Today;
                    this.dfFechaFinResTX.MaxDate = DateTime.Today;

                    this.PagingTXs.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepResumenTXs).Valor);

                    HttpContext.Current.Session.Add("DtRepResumenTXs", null);
                }

                log.Info("TERMINA ReporteResumenTransacciones Page_Load()");
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
        protected void LlenaGrinPanelTXs(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI pCI = new LogPCI(LH_ParabRepResumenTXs);
            this.btnExportExcel.Disabled = true;

            try
            {
                DataTable dtRepResumenTXs = new DataTable();

                dtRepResumenTXs = HttpContext.Current.Session["DtRepResumenTXs"] as DataTable;

                if (dtRepResumenTXs == null)
                {
                    pCI.Info("INICIA ReporteResumenTransacciones()");
                    dtRepResumenTXs = DAOPoliza.ReporteResumenTransacciones(
                        Convert.ToDateTime(dfFechaIniResTX.SelectedDate),
                        Convert.ToDateTime(dfFechaFinResTX.SelectedDate),
                        LH_ParabRepResumenTXs);
                    pCI.Info("TERMINA ReporteResumenTransacciones()");

                    if (dtRepResumenTXs.Rows.Count < 1)
                    {
                        X.Msg.Alert("Resumen de Transacciones", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    HttpContext.Current.Session.Add("DtRepResumenTXs", dtRepResumenTXs);
                }               

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepResumenTXs).Valor);

                if (dtRepResumenTXs.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Resumen de Transacciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepResumenTXs.ClicDePaso()",
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
                    int TotalRegistros = dtRepResumenTXs.Rows.Count;

                    (this.StoreResumenTXs.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtRepResumenTXs.Clone();
                    DataTable dtToGrid = dtRepResumenTXs.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRepResumenTXs.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTXs.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTXs.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRepResumenTXs.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreResumenTXs.DataSource = dtToGrid;
                    this.StoreResumenTXs.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Resumen de Transacciones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Resumen de Transacciones", "Ocurrió un error al obtener el reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de tarjetas
        /// </summary>
        protected void LimpiaGridTarjetas()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtRepResumenTXs", null);
            this.StoreResumenTXs.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.dfFechaIniResTX.Reset();
            this.dfFechaFinResTX.Reset();

            LimpiaGridTarjetas();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridTarjetas();

            Thread.Sleep(100);

            this.btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepResumenTXs")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteResumenTXs";
                DataTable _dtTarjetasAct = HttpContext.Current.Session["DtRepResumenTXs"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtTarjetasAct, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepResumenTXs);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Resumen de Transacciones", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
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
        /// Controla el evento onRefresh del grid de tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreResumenTXs_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGrinPanelTXs(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepResumenTXs")]
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
            ExportDataTableToExcel();
        }
        
        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepResumenTXs")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}