using ClosedXML.Excel;
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
using Utilerias;

namespace TpvWeb
{
    public partial class Reporte_Timbrados : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilium Reporte de Timbrados
        private LogHeader LH_ParabRepTimbrados = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepTimbrados.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepTimbrados.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepTimbrados.User = this.Usuario.ClaveUsuario;
            LH_ParabRepTimbrados.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepTimbrados);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_Timbrados Page_Load()");

                if (!IsPostBack)
                {
                    PagingTimbrados.PageSize =
                         Convert.ToInt32(Configuracion.Get(Guid.Parse(
                         ConfigurationManager.AppSettings["IDApplication"].ToString()),
                         "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtRepTimbrados", null);

                }

                log.Info("TERMINA Reporte_Timbrados Page_Load()");
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
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
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarTimbrados_Click(object sender, EventArgs e)
        {
            LimpiaGridPanelTimbrados();

            Thread.Sleep(100);

            btnBuscarTimbradosHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarTimbrados_Click(object sender, EventArgs e)
        {
            dfFI_Timbrados.Reset();
            dfFF_Timbrados.Reset();

            LimpiaGridPanelTimbrados();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid
        /// </summary>
        protected void LimpiaGridPanelTimbrados()
        {
            btnExcelTimbrados.Disabled = true;

            HttpContext.Current.Session.Add("DtRepTimbrados", null);
            StoreTimbrados.RemoveAll();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelTimbrados(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExcelTimbrados.Disabled = true;
            LogPCI unLog = new LogPCI(LH_ParabRepTimbrados);

            try
            {
                DataTable dtTimbrados = new DataTable();

                dtTimbrados = HttpContext.Current.Session["DtRepTimbrados"] as DataTable;

                if (dtTimbrados == null)
                {
                    unLog.Info("INICIA ObtieneEstadosCuentaTimbrados()");
                    dtTimbrados = DAOReportes.ObtieneEstadosCuentaTimbrados(
                        Convert.ToDateTime(dfFI_Timbrados.Value), Convert.ToDateTime(dfFF_Timbrados.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabRepTimbrados);
                    unLog.Info("TERMINA ObtieneEstadosCuentaTimbrados()");

                    if (dtTimbrados.Rows.Count < 1)
                    {
                        X.Msg.Alert("Timbrados", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    DataTable dtTimbradosMask = Tarjetas.EnmascaraTablaConTarjetas(dtTimbrados, "ClaveMA", "Enmascara", LH_ParabRepTimbrados);

                    HttpContext.Current.Session.Add("DtRepTimbrados", dtTimbradosMask);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtTimbrados.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Timbrados", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepTimbrados.ClicDePaso()",
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
                    int TotalRegistros = dtTimbrados.Rows.Count;

                    (this.StoreTimbrados.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtTimbrados.Clone();
                    DataTable dtToGrid = dtTimbrados.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtTimbrados.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTimbrados.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTimbrados.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtTimbrados.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreTimbrados.DataSource = dtToGrid;
                    this.StoreTimbrados.DataBind();

                    this.btnExcelTimbrados.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Timbrados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Timbrados", "Ocurrió un error al consultar el Reporte de Timbrados").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreTimbrados_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelTimbrados(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepTimbrados")]
        public void ClicDePaso()
        {
            btnDownTimbHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepTimbrados")]
        public void ExportDTDetalleToExcel()
        {
            try
            {
                string reportName = "ReporteTimbrados";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtRepTimbrados"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabRepTimbrados);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Timbrados", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Column(21).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 11).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 14).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepTimbrados")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}