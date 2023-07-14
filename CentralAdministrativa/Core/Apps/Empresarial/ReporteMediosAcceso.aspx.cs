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
using Utilerias;

namespace Empresarial
{
    public partial class ReporteMediosAcceso : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Dispersiones Cuentas Eje
        private LogHeader LH_ParabRepMediosAcceso = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Medios de Acceso
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepMediosAcceso.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepMediosAcceso.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepMediosAcceso.User = this.Usuario.ClaveUsuario;
            LH_ParabRepMediosAcceso.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepMediosAcceso);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteMediosAcceso Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaTiposColectivaSubemisor()");
                    DataSet dsTiposColectiva = DAOColectiva.ListaTiposColectivaSubemisor
                        (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepMediosAcceso);
                    log.Info("TERMINA ListaTiposColectivaSubemisor()");

                    this.StoreTipoColectiva.DataSource = dsTiposColectiva;
                    this.StoreTipoColectiva.DataBind();

                    PagingMAs.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepMediosAcceso).Valor);

                    HttpContext.Current.Session.Add("DtMAs", null);
                    HttpContext.Current.Session.Add("DTCtaHab", null);
                }

                log.Info("TERMINA ReporteMediosAcceso Page_Load()");
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
        protected void LlenaGridPanelMA(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_ParabRepMediosAcceso);
            this.btnExportExcel.Disabled = true;
            this.btnReporte.Disabled = true;

            try
            {
                DataTable dtMediosAcceso = new DataTable();

                dtMediosAcceso = HttpContext.Current.Session["DtMAs"] as DataTable;

                if (dtMediosAcceso == null)
                {
                    log.Info("INICIA ReporteMediosAcceso()");
                    dtMediosAcceso = DAOMediosAcceso.ReporteMediosAcceso(
                        Convert.ToInt64(cBoxCliente.SelectedItem.Value), txtTarjeta.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepMediosAcceso);
                    log.Info("TERMINA ReporteMediosAcceso()");

                    if (dtMediosAcceso.Rows.Count < 1)
                    {
                        X.Msg.Alert("Medios de Acceso", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtMediosAcceso = Tarjetas.EnmascaraTablaConTarjetas(dtMediosAcceso, "Tarjeta", "Enmascara", LH_ParabRepMediosAcceso);

                    HttpContext.Current.Session.Add("DtMAs", dtMediosAcceso);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepMediosAcceso).Valor);

                if (dtMediosAcceso.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Medios de Acceso", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepMediosAcceso.ClicDePaso()",
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
                    int TotalRegistros = dtMediosAcceso.Rows.Count;

                    (this.StoreMediosAcceso.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtMediosAcceso.Clone();
                    DataTable dtToGrid = dtMediosAcceso.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtMediosAcceso.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingMAs.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingMAs.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtMediosAcceso.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreMediosAcceso.DataSource = dtToGrid;
                    StoreMediosAcceso.DataBind();

                    btnExportExcel.Disabled = false;
                    btnReporte.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Medios de Acceso", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Medios de Acceso", "Ocurrió un error al obtener el Reporte de Medios de Acceso").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de medios de acceso
        /// </summary>
        protected void LimpiaGridMediosAcceso()
        {
            btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtMAs", null);
            StoreMediosAcceso.RemoveAll();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de medios de acceso
        /// </summary>
        protected void LimpiaDtReporteCtaHab()
        {
            this.btnReporte.Disabled = true;
            HttpContext.Current.Session.Add("DTCtaHab", null);
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
            this.cBoxCliente.Disabled = true;
            this.txtTarjeta.Reset();
            this.txtTarjeta.Disabled = true;

            LimpiaGridMediosAcceso();
            LimpiaDtReporteCtaHab();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridMediosAcceso();
            LimpiaDtReporteCtaHab();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepMediosAcceso")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteMediosAcceso";
                DataTable _dtMediosAcceso = HttpContext.Current.Session["DtMAs"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtMediosAcceso, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepMediosAcceso);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Medios de Acceso", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                ws.Column(2).Hide();
                ws.Column(9).Hide();
                ws.Column(10).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 11).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 12).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de medios de acceso
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreMediosAcceso_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelMA(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepMediosAcceso")]
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
        [DirectMethod(Namespace = "RepMediosAcceso")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento Click al botón Reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnReporte_click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabRepMediosAcceso);

            try
            {
                DataTable dtRepCtaHab = new DataTable();

                dtRepCtaHab = HttpContext.Current.Session["DTCtaHab"] as DataTable;

                if (dtRepCtaHab == null)
                {
                    logPCI.Info("INICIA ObtieneDatosCuentahabientesSubEmisor()");
                    dtRepCtaHab = DAOColectivas.ObtieneDatosCuentahabientesSubEmisor(
                        Convert.ToInt64(this.cBoxCliente.SelectedItem.Value), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabRepMediosAcceso);
                    logPCI.Info("TERMINA ObtieneDatosCuentahabientesSubEmisor()");
                }

                dtRepCtaHab = Tarjetas.EnmascaraTablaConTarjetas(dtRepCtaHab, "Tarjeta", "Enmascara", LH_ParabRepMediosAcceso);
                HttpContext.Current.Session.Add("DTCtaHab", dtRepCtaHab);

                Thread.Sleep(100);

                btnHide.FireEvent("click");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reporte", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Notify("Reporte", "Ocurrió un error al obtener el reporte").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón oculto de descarga del reporte de cuentahabientes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void ExportaReporte(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "RepCtaHab";
                DataTable _dtRepCtaHab = HttpContext.Current.Session["DTCtaHab"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtRepCtaHab, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepMediosAcceso);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Reporte", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
        /// Controla el evento Seleccionar del combo Tipo de Colectiva, estableciendo las colectivas correspondientes al tipo elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceColectivas(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepMediosAcceso);

            try
            {
                unLog.Info("INICIA ListaColectivasPorTipo()");
                StoreClientes.DataSource = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColectiva.SelectedItem.Value), "", this.Usuario, Guid.Parse(
                    ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabRepMediosAcceso);
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