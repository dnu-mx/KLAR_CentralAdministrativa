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
    public partial class ReporteSaldosDiariosTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte de Saldos Diarios Tarjetas
        private LogHeader LH_ParabRepSalDiaTarj = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Saldos Diarios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepSalDiaTarj.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepSalDiaTarj.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepSalDiaTarj.User = this.Usuario.ClaveUsuario;
            LH_ParabRepSalDiaTarj.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepSalDiaTarj);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteSaldosDiariosTarjetas Page_Load()");

                if (!IsPostBack)
                {
                    dfFecha.MaxDate = DateTime.Today;

                    log.Info("INICIA ListaTiposColectivaSubemisor()");
                    DataSet dsTiposColectiva = DAOColectiva.ListaTiposColectivaSubemisor
                        (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSalDiaTarj);
                    log.Info("TERMINA ListaTiposColectivaSubemisor()");

                    this.StoreTipoColectiva.DataSource = dsTiposColectiva;
                    this.StoreTipoColectiva.DataBind();

                    PagingSDTarjetas.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepSalDiaTarj).Valor);

                    HttpContext.Current.Session.Add("DtSaldosDiariosTarjetas", null);
                }

                log.Info("TERMINA ReporteSaldosDiariosTarjetas Page_Load()");
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
        /// Controla la alimentación de datos del grid de saldos diarios, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridSaldosDiariosTarjetas(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_ParabRepSalDiaTarj);
            btnExportExcel.Disabled = true;

            try
            {
                DataTable dtSaldosDiariosTarj = new DataTable();

                dtSaldosDiariosTarj = HttpContext.Current.Session["DtSaldosDiariosTarjetas"] as DataTable;

                if (dtSaldosDiariosTarj == null)
                {
                    log.Info("INICIA ReporteSaldosDiariosTarjetasCliente()");
                    dtSaldosDiariosTarj = DAOMediosAcceso.ReporteSaldosDiariosTarjetasCliente(
                        Convert.ToInt64(cBoxCliente.SelectedItem.Value),
                        Convert.ToDateTime(dfFecha.SelectedDate), txtTarjeta.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepSalDiaTarj);
                    log.Info("TERMINA ReporteSaldosDiariosTarjetasCliente()");

                    if (dtSaldosDiariosTarj.Rows.Count < 1)
                    {
                        X.Msg.Alert("Saldos Diarios", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtSaldosDiariosTarj = Tarjetas.EnmascaraTablaConTarjetas(dtSaldosDiariosTarj, "NumTarjeta", "Enmascara", LH_ParabRepSalDiaTarj);

                    HttpContext.Current.Session.Add("DtSaldosDiariosTarjetas", dtSaldosDiariosTarj);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_MaxRegsPagina", LH_ParabRepSalDiaTarj).Valor);
                
                if (dtSaldosDiariosTarj.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Saldos Diarios", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepSDTarjetas.ClicDePaso()",
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
                    int TotalRegistros = dtSaldosDiariosTarj.Rows.Count;

                    (this.StoreTarjetasSD.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtSaldosDiariosTarj.Clone();
                    DataTable dtToGrid = dtSaldosDiariosTarj.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtSaldosDiariosTarj.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingSDTarjetas.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingSDTarjetas.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtSaldosDiariosTarj.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreTarjetasSD.DataSource = dtToGrid;
                    StoreTarjetasSD.DataBind();

                    btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Saldos Diarios", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Saldos Diarios", "Ocurrió un error al obtener el Reporte de Saldos Diarios").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de saldos diarios
        /// </summary>
        protected void LimpiaGridSaldosDiariosTarjetas()
        {
            btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtSaldosDiariosTarjetas", null);
            StoreTarjetasSD.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.cBoxTipoColectiva.Reset();
            this.cBoxCliente.Reset();
            this.dfFecha.Reset();
            this.txtTarjeta.Reset();

            LimpiaGridSaldosDiariosTarjetas();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridSaldosDiariosTarjetas();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepSDTarjetas")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "SaldosDiariosTarjetas";
                DataTable _dtSDTarjetas = HttpContext.Current.Session["DtSaldosDiariosTarjetas"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSDTarjetas, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepSalDiaTarj);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Saldos Diarios", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Column(10).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.DateTime);
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
        /// Controla el evento onRefresh del grid de saldos diarios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreTarjetasSD_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridSaldosDiariosTarjetas(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepSDTarjetas")]
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
        [DirectMethod(Namespace = "RepSDTarjetas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Tipo de Colectiva, estableciendo las colectivas correspondientes al tipo elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceColectivas(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepSalDiaTarj);

            try
            {
                unLog.Info("INICIA ListaColectivasPorTipo()");
                StoreClientes.DataSource = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColectiva.SelectedItem.Value), "", this.Usuario, Guid.Parse(
                    ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabRepSalDiaTarj);
                unLog.Info("TERMINA ListaColectivasPorTipo()");
                StoreClientes.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectiva", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Colectiva", "Ocurrió un error al establecer las Colectivas").Show();
            }
        }
    }
}