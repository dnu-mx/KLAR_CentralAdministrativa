using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
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
    public partial class ReporteAltaYAniversariosTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte de Alta y Aniversarios de Tarjetas
        private LogHeader LH_ParabRepAltAnivTarj = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Tarjetas Activadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepAltAnivTarj.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepAltAnivTarj.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepAltAnivTarj.User = this.Usuario.ClaveUsuario;
            LH_ParabRepAltAnivTarj.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepAltAnivTarj);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteAltaYAniversariosTarjetas Page_Load()");

                if (!IsPostBack)
                {
                    this.dfPeriodo.MaxDate = DateTime.Now;

                    log.Info("INICIA ListaColectivasSubEmisor()");
                    this.StoreColectivaSub.DataSource = DAOColectiva.ListaColectivasSubEmisor("GCM",
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepAltAnivTarj);
                    log.Info("TERMINA ListaColectivasSubEmisor()");
                    this.StoreColectivaSub.DataBind();

                    this.PagingTarjetas.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepAltAnivTarj).Valor);

                    HttpContext.Current.Session.Add("DtRepAATarjetas", null);
                }

                log.Info("TERMINA ReporteAltaYAniversariosTarjetas Page_Load()");
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
        /// Controla el evento Seleccionar del combo Tipo de SubEmisor, estableciendo los productos
        /// correspondientes al subemisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceProductos(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabRepAltAnivTarj);

            try
            {
                int idSubem = Convert.ToInt32(this.cBoxSubemisor.SelectedItem.Value);

                if (idSubem != -1)
                {
                    pCI.Info("INICIA ObtieneProductosDeColectiva()");
                    this.StoreProductosSubEm.DataSource = 
                        DAOProducto.ObtieneProductosDeColectiva(idSubem, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepAltAnivTarj);
                    pCI.Info("TERMINA ObtieneProductosDeColectiva()");

                    this.StoreProductosSubEm.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al establecer los Productos del SubEmisor").Show();
            }
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de saldos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelTarjetas(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI pCI = new LogPCI(LH_ParabRepAltAnivTarj);
            btnExportExcel.Disabled = true;

            try
            {
                DataTable dtAltAnivTarj = new DataTable();

                dtAltAnivTarj = HttpContext.Current.Session["DtRepAATarjetas"] as DataTable;

                if (dtAltAnivTarj == null)
                {
                    pCI.Info("INICIA ReporteAltaAniversariosTarjetas()");
                    dtAltAnivTarj = DAOMediosAcceso.ReporteAltaAniversariosTarjetas(
                        Convert.ToInt32(this.cBoxSubemisor.SelectedItem.Value),
                        Convert.ToInt32(this.cBoxProductosSubEm.SelectedItem.Value),
                        dfPeriodo.SelectedDate.Month, dfPeriodo.SelectedDate.Year,
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepAltAnivTarj);
                    pCI.Info("TERMINA ReporteAltaAniversariosTarjetas()");

                    if (dtAltAnivTarj.Rows.Count < 1)
                    {
                        X.Msg.Alert("Tarjetas", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    HttpContext.Current.Session.Add("DtRepAATarjetas", dtAltAnivTarj);
                }               

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepAltAnivTarj).Valor);

                if (dtAltAnivTarj.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Tarjetas", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepAltAnivTarjetas.ClicDePaso()",
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
                    int TotalRegistros = dtAltAnivTarj.Rows.Count;

                    (this.StoreTarjetas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtAltAnivTarj.Clone();
                    DataTable dtToGrid = dtAltAnivTarj.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtAltAnivTarj.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTarjetas.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTarjetas.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtAltAnivTarj.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreTarjetas.DataSource = dtToGrid;
                    this.StoreTarjetas.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Tarjetas", "Ocurrió un error al obtener el reporte").Show();
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

            HttpContext.Current.Session.Add("DtRepAATarjetas", null);
            this.StoreTarjetas.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.cBoxSubemisor.Reset();
            this.cBoxProductosSubEm.Reset();
            this.dfPeriodo.Reset();

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
        [DirectMethod(Namespace = "RepAltAnivTarjetas")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteAltaAnivTarjetas";
                DataTable _dtTarjetasAct = HttpContext.Current.Session["DtRepAATarjetas"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabRepAltAnivTarj);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Tarjetas", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
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
        protected void StoreTarjetas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelTarjetas(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepAltAnivTarjetas")]
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
        [DirectMethod(Namespace = "RepAltAnivTarjetas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}