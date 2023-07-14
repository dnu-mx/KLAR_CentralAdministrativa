using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALEventos.BaseDatos;
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
    public partial class ReporteDispersionesCuentasEje : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Dispersiones Cuentas Eje
        private LogHeader LH_ParabRepDispCtasEje = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Dispersiones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepDispCtasEje.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepDispCtasEje.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepDispCtasEje.User = this.Usuario.ClaveUsuario;
            LH_ParabRepDispCtasEje.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepDispCtasEje);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteDispersionesCuentasEje Page_Load()");

                if (!IsPostBack)
                {
                    dfFechaInicio.SetValue(DateTime.Now);
                    dfFechaInicio.MaxDate = DateTime.Today;

                    dfFechaFin.SetValue(DateTime.Now);
                    dfFechaFin.MaxDate = DateTime.Today;

                    log.Info("INICIA ListaClientesCuentasEjeCacao()");
                    StoreColectivasCuentasEje.DataSource = 
                        DAOEvento.ListaClientesCuentasEjeCacao(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabRepDispCtasEje);
                    log.Info("TERMINA ListaClientesCuentasEjeCacao()");
                    StoreColectivasCuentasEje.DataBind();


                    PagingToolBar1.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepDispCtasEje).Valor);

                    HttpContext.Current.Session.Add("DtDispersiones", null);
                }

                log.Info("TERMINA ReporteDispersionesCuentasEje Page_Load()");
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
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            FormPanelFiltros.Reset();

            LimpiaGridDispersiones();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Imágenes
        /// </summary>
        protected void LimpiaGridDispersiones()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtDispersiones", null);
            StoreDispersiones.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de dispersiones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelDispersiones(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_ParabRepDispCtasEje);
            this.btnExportExcel.Disabled = true;

            try
            {
                DataTable dtDispersiones = new DataTable();

                dtDispersiones = HttpContext.Current.Session["DtDispersiones"] as DataTable;

                if (dtDispersiones == null)
                {
                    log.Info("INICIA ListaHistoricoDispersionesCuentasEje()");
                    dtDispersiones = DAOEvento.ListaHistoricoDispersionesCuentasEje(
                        Convert.ToDateTime(dfFechaInicio.SelectedDate), Convert.ToDateTime(dfFechaFin.SelectedDate),
                        String.IsNullOrEmpty(cBoxTipoMovimiento.SelectedItem.Value) ? -1 : int.Parse(cBoxTipoMovimiento.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxCliente.SelectedItem.Value) ? -1 : Convert.ToInt64(cBoxCliente.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxEstatus.SelectedItem.Value) ? "-1" : cBoxEstatus.SelectedItem.Value,
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabRepDispCtasEje);
                    log.Info("TERMINA ListaHistoricoDispersionesCuentasEje()");

                    HttpContext.Current.Session.Add("DtDispersiones", dtDispersiones);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepDispCtasEje).Valor);

                if (dtDispersiones.Rows.Count < 1)
                {
                    X.Msg.Alert("Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtDispersiones.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepDispersiones.ClicDePaso()",
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
                    int TotalRegistros = dtDispersiones.Rows.Count;

                    (this.StoreDispersiones.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtDispersiones.Clone();
                    DataTable dtToGrid = dtDispersiones.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDispersiones.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingToolBar1.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtDispersiones.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreDispersiones.DataSource = dtToGrid;
                    StoreDispersiones.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Movimientos", "Ocurrió un error al obtener el Reporte de Fondeos y Retiros").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, DirectEventArgs e)
        {
            LimpiaGridDispersiones();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de beneficios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreDispersiones_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelDispersiones(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepDispersiones")]
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
        [DirectMethod(Namespace = "RepDispersiones")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteFondeosyRetiros";
                DataTable _dtDispersiones = HttpContext.Current.Session["DtDispersiones"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDispersiones, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepDispCtasEje);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Movimientos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 10).SetDataType(XLCellValues.DateTime);
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
        [DirectMethod(Namespace = "RepDispersiones")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}