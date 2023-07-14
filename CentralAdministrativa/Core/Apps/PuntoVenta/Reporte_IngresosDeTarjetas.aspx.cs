using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.LogicaNegocio;
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
using System.Xml;
using System.Xml.Xsl;

namespace TpvWeb
{
    public partial class Reporte_IngresosDeTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Procesos Batch
        private LogHeader LH_ParabRepIngresosDeTarjetas = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepIngresosDeTarjetas.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepIngresosDeTarjetas.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepIngresosDeTarjetas.User = this.Usuario.ClaveUsuario;
            LH_ParabRepIngresosDeTarjetas.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepIngresosDeTarjetas);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_IngresosDeTarjetas_CashIn_MontoPromedio Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.SetValue(DateTime.Now);
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddMonths(-3);

                    this.dfFechaFin.SetValue(DateTime.Now);
                    this.dfFechaFin.MaxDate = DateTime.Today;
                                        

                    this.PagingStoreIngTarj.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("dtIngresosDeTarjetas", null);
                }

                log.Info("TERMINA Reporte_IngresosDeTarjetas_CashIn_MontoPromedio Page_Load()");
            }

            catch (CAppException)
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
        /// Controla el evento Click al botón Buscar del panel de Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaGridPanelIngresosDeTarjetas(int RegistroInicial, string Columna, SortDirection Orden)
        {

            this.btnExportExcel.Disabled = true;
            LogPCI unLog = new LogPCI(LH_ParabRepIngresosDeTarjetas);

            try
            {
                DateTime olddate = Convert.ToDateTime(dfFechaInicio.SelectedDate);
                DateTime newdate = Convert.ToDateTime(dfFechaFin.SelectedDate);
                int months = Math.Abs((olddate.Month + (olddate.Year * 12)) - (newdate.Month + (newdate.Year * 12)));
                int limiteMesesProcesar = 3;

               
                if (months <= limiteMesesProcesar)
                {

                    DataTable dtIngresosDeTarjetas = new DataTable();

                    dtIngresosDeTarjetas = HttpContext.Current.Session["dtIngresosDeTarjetas"] as DataTable;

                    if (dtIngresosDeTarjetas == null)
                    {
                        unLog.Info("INICIA ListaIngresosDeTarjetasCashInYMontoPromedio()");
                        dtIngresosDeTarjetas = DAOReportes.ListaIngresosDeTarjetasCashInYMontoPromedio(
                            Convert.ToDateTime(dfFechaInicio.SelectedDate),
                            Convert.ToDateTime(dfFechaFin.SelectedDate),
                            this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                            LH_ParabRepIngresosDeTarjetas);
                        unLog.Info("TERMINA ListaIngresosDeTarjetasCashInYMontoPromedio()");

                        HttpContext.Current.Session.Add("dtIngresosDeTarjetas", dtIngresosDeTarjetas);
                    }

                    int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                            ConfigurationManager.AppSettings["IDApplication"].ToString()),
                            "Reporte_MaxRegsPagina").Valor);


                    if (dtIngresosDeTarjetas.Rows.Count < 1)
                    {
                        X.Msg.Alert("IngresosDeTarjetas", "No existen coincidencias con la búsqueda solicitada").Show();
                    }
                    else if (dtIngresosDeTarjetas.Rows.Count > maxRegistros)
                    {
                        X.Msg.Confirm("IngresosDeTarjetas", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                            "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                            {
                                Yes = new MessageBoxButtonConfig
                                {
                                    Handler = "RepIngresosTarjetas.ClicDePaso()",
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
                        int TotalRegistros = dtIngresosDeTarjetas.Rows.Count;

                        (this.StoreIngresosDeTarjetas.Proxy[0] as PageProxy).Total = TotalRegistros;

                        DataTable sortedDT = dtIngresosDeTarjetas.Clone();
                        DataTable dtToGrid = dtIngresosDeTarjetas.Clone();

                        //Se ordenan los datos según la elección del usuario
                        if (!String.IsNullOrEmpty(Columna))
                        {
                            System.Data.DataView dv = dtIngresosDeTarjetas.DefaultView;

                            dv.Sort = Columna + " " + Orden.ToString();
                            sortedDT = dv.ToTable();
                            sortedDT.AcceptChanges();
                        }

                        int RegistroFinal = (RegistroInicial + PagingStoreIngTarj.PageSize) < TotalRegistros ?
                            (RegistroInicial + PagingStoreIngTarj.PageSize) : TotalRegistros;

                        //Se recorta el número de registros a los definidos por página
                        for (int row = RegistroInicial; row < RegistroFinal; row++)
                        {
                            dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtIngresosDeTarjetas.Rows[row] : sortedDT.Rows[row]);
                        }

                        dtToGrid.AcceptChanges();

                        StoreIngresosDeTarjetas.DataSource = dtToGrid;
                        StoreIngresosDeTarjetas.DataBind();

                        this.btnExportExcel.Disabled = false;
                    }
                }
                else
                {
                    X.Msg.Alert("IngresosDeTarjetas", "El periodo de la FechaInicial y FechaFinal debe ser menor a 3 meses. Número de meses capturados: " + months).Show();

                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("IngresosDeTarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("IngresosDeTarjetas", "Ocurrió un error al consultar el Reporte de IngresosDeTarjetas").Show();
            }

            finally
            {
                X.Mask.Hide();
            }

        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridIngresosDeTarjetas()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("dtIngresosDeTarjetas", null);
            StoreIngresosDeTarjetas.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridIngresosDeTarjetas();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
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

            LimpiaGridIngresosDeTarjetas();
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
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
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
        protected void StoreIngresosDeTarjetas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelIngresosDeTarjetas(inicio, columna, orden);
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepIngresosTarjetas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepIngresosTarjetas")]
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
        [DirectMethod(Namespace = "RepIngresosTarjetas")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "Reporte_IngresosDeTarjetas";
                DataTable _dtOperaciones = HttpContext.Current.Session["dtIngresosDeTarjetas"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOperaciones, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepIngresosDeTarjetas);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
    }
}