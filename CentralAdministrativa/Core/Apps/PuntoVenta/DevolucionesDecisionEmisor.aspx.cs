using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.LogicaNegocio;
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

namespace TpvWeb
{
    public partial class DevolucionesDecisionEmisor : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Devoluciones por Decisión del Emisor
        private LogHeader LH_ParabDevsDecEmisor = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabDevsDecEmisor.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabDevsDecEmisor.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabDevsDecEmisor.User = this.Usuario.ClaveUsuario;
            LH_ParabDevsDecEmisor.Trace_ID = Guid.NewGuid();
            
            LogPCI log = new LogPCI(LH_ParabDevsDecEmisor);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA DevolucionesDecisionEmisor Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddMonths(-6);

                    this.dfFechaFin.MaxDate = DateTime.Today;

                    log.Info("INICIA ListaEstatusPostOperacion_NoVencidas()");
                    this.StoreEstatusPostOp.DataSource =
                        DAOCatalogos.ListaEstatusPostOperacion_NoVencidas(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabDevsDecEmisor);
                    log.Info("TERMINA ListaEstatusPostOperacion_NoVencidas()");
                    this.StoreEstatusPostOp.DataBind();

                    PagingDevsDecEmisor.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtOperacionesNoVencidas", null);
                }

                log.Info("TERMINA DevolucionesDecisionEmisor Page_Load()");
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
        /// Controla el evento Click al botón Limpiar del formulario, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.FormPanelDevsDecEmisor.Reset();

            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();

            this.txtTarjeta.Reset();
            cBoxEstatusPostOp.Clear();
           
            LimpiaGridDevsDecEmisor();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridDevsDecEmisor()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtOperacionesNoVencidas", null);
            StoreOpsNoVencidas.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            ReloadGridDevsDecEmisor();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones no vencidas, así como el ordenamiento
        /// y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridDevsDecEmisor(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            LogPCI logPCI = new LogPCI(LH_ParabDevsDecEmisor);

            try
            {
                DataTable dtOpsNoVencidas = new DataTable();

                dtOpsNoVencidas = HttpContext.Current.Session["DtOperacionesNoVencidas"] as DataTable;

                if (dtOpsNoVencidas == null)
                {
                    logPCI.Info("INICIA ObtieneOperacionesNoVencidas()");
                    dtOpsNoVencidas = DAOOperacion.ObtieneOperacionesNoVencidas(
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        txtTarjeta.Text, String.IsNullOrEmpty(cBoxEstatusPostOp.SelectedItem.Value) ?
                        -1 : int.Parse(cBoxEstatusPostOp.SelectedItem.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabDevsDecEmisor);
                    logPCI.Info("TERMINA ObtieneOperacionesNoVencidas()");

                    if (dtOpsNoVencidas.Rows.Count < 1)
                    {
                        X.Msg.Alert("Operaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtOpsNoVencidas = Tarjetas.EnmascaraTablaConTarjetas(dtOpsNoVencidas, "Tarjeta", "Enmascara", LH_ParabDevsDecEmisor);

                    HttpContext.Current.Session.Add("DtOperacionesNoVencidas", dtOpsNoVencidas);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtOpsNoVencidas.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Operaciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El resultado de la búsqueda se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "OpsNoVencidas.ClicDePaso()",
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
                    int TotalRegistros = dtOpsNoVencidas.Rows.Count;

                    (this.StoreOpsNoVencidas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsNoVencidas.Clone();
                    DataTable dtToGrid = dtOpsNoVencidas.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsNoVencidas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDevsDecEmisor.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDevsDecEmisor.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsNoVencidas.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreOpsNoVencidas.DataSource = dtToGrid;
                    this.StoreOpsNoVencidas.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Operaciones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un Error al Consultar las Operaciones").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones no vencidas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsNoVencidas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridDevsDecEmisor(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "OpsNoVencidas")]
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
        [DirectMethod(Namespace = "OpsNoVencidas")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "OperacionesNoVencidas";
                DataTable _dtOpsNoVencidas = HttpContext.Current.Session["DtOperacionesNoVencidas"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOpsNoVencidas, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabDevsDecEmisor);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un Error al Exportar a Excel").Show();
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
                ws.Column(4).Hide();
                ws.Column(8).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "OpsNoVencidas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla la modificación del estatus de las operaciones seleccionadas en base de datos
        /// </summary>
        [DirectMethod(Namespace = "OpsNoVencidas", Timeout = 120000)]
        public void CambiaEstatusAOperaciones()
        {
            LogPCI unLog = new LogPCI(LH_ParabDevsDecEmisor);

            try
            {
                RowSelectionModel lasOperaciones = this.GridDevsDecEmisor.SelectionModel.Primary as RowSelectionModel;

                foreach (SelectedRow operacion in lasOperaciones.SelectedRows)
                {
                    unLog.Info("INICIA ModificaEstatusOpNoVencida()");
                    LNOperaciones.ModificaEstatusOpNoVencida(Convert.ToInt32(this.cBoxEstatusPostOp.SelectedItem.Value),
                        Convert.ToInt64(operacion.RecordID), this.Usuario, LH_ParabDevsDecEmisor); ;
                    unLog.Info("TERMINA ModificaEstatusOpNoVencida()");
                }

                X.Msg.Notify("", "Estatus de las operaciones modificado <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                ReloadGridDevsDecEmisor();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Cambio de estatus", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Cambio de estatus", "Ocurrió un error al cambiar el estatus de la operación").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla la recarga de datos al grid tras haber modificado el estatus de una o varias operaciones
        /// </summary>
        protected void ReloadGridDevsDecEmisor()
        {
            LimpiaGridDevsDecEmisor();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }
    }
}
