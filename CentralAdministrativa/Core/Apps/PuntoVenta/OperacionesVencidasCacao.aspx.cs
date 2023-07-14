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
    public partial class OperacionesVencidasCacao : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Operaciones Vencidas
        private LogHeader LH_ParabOpsVencidas = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabOpsVencidas.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabOpsVencidas.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabOpsVencidas.User = this.Usuario.ClaveUsuario;
            LH_ParabOpsVencidas.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabOpsVencidas);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA OperacionesVencidasCacao Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.SetValue(DateTime.Now);
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddDays(-180);

                    this.dfFechaFin.SetValue(DateTime.Now);
                    this.dfFechaFin.MaxDate = DateTime.Today;
                    
                    LlenaCombos();

                    PagingOpsVenc.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtOperacionesVencidas", null);
                }

                log.Info("TERMINA OperacionesVencidasCacao Page_Load()");
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
        /// Obtiene los catálogos de estatus de operación, motivo de rechazo y estatus de
        /// compensación para establecerlos en los combos correspondientes
        /// </summary>
        protected void LlenaCombos()
        {
            LogPCI pCI = new LogPCI(LH_ParabOpsVencidas);

            try
            {
                pCI.Info("INICIA ListaTiposColectivaSubemisor()");
                DataSet dsTiposColectiva = DAOColectiva.ListaTiposColectivaSubemisor
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabOpsVencidas);
                pCI.Info("TERMINA ListaTiposColectivaSubemisor()");

                this.StoreTipoColectiva.DataSource = dsTiposColectiva;
                this.StoreTipoColectiva.DataBind();

                pCI.Info("INICIA ListaEstatusPostOperacion_Vencidos()");
                this.StoreEstatusPostOp.DataSource = 
                    DAOCatalogos.ListaEstatusPostOperacion_Vencidos(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                pCI.Info("TERMINA ListaEstatusPostOperacion_Vencidos()");
                this.StoreEstatusPostOp.DataBind();
            }
            catch (CAppException caEx)
            {
                pCI.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
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
            this.FormPanelOpsVencidas.Reset();

            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();

            this.txtTarjeta.Reset();
            cBoxEstatusPostOp.Clear();
           
            LimpiaGridOpsVenc();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridOpsVenc()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtOperacionesVencidas", null);
            StoreOpsVenc.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            ReloadGridOpsVenc();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones vencisas, así como el ordenamiento
        /// y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelOpsVencidas(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            LogPCI logPCI = new LogPCI(LH_ParabOpsVencidas);

            try
            {
                DataTable dtOpsVencidas = new DataTable();

                dtOpsVencidas = HttpContext.Current.Session["DtOperacionesVencidas"] as DataTable;

                if (dtOpsVencidas == null)
                {
                    logPCI.Info("INICIA ObtieneOperacionesVencidas()");
                    dtOpsVencidas = DAOOperacion.ObtieneOperacionesVencidas(
                        Convert.ToInt64(this.cBoxGpoComercial.SelectedItem.Value),
                        Convert.ToDateTime(this.dfFechaInicio.SelectedDate), 
                        Convert.ToDateTime(this.dfFechaFin.SelectedDate),
                        txtTarjeta.Text, String.IsNullOrEmpty(cBoxEstatusPostOp.SelectedItem.Value) ? 
                        -1 : int.Parse(cBoxEstatusPostOp.SelectedItem.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabOpsVencidas);
                    logPCI.Info("TERMINA ObtieneOperacionesVencidas()");

                    if (dtOpsVencidas.Rows.Count < 1)
                    {
                        X.Msg.Alert("Operaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtOpsVencidas = Tarjetas.EnmascaraTablaConTarjetas(dtOpsVencidas, "Tarjeta", "Enmascara", LH_ParabOpsVencidas);

                    HttpContext.Current.Session.Add("DtOperacionesVencidas", dtOpsVencidas);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtOpsVencidas.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Operaciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El resultado de la búsqueda se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "OpsVencidas.ClicDePaso()",
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
                    int TotalRegistros = dtOpsVencidas.Rows.Count;

                    (this.StoreOpsVenc.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsVencidas.Clone();
                    DataTable dtToGrid = dtOpsVencidas.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsVencidas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingOpsVenc.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingOpsVenc.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsVencidas.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreOpsVenc.DataSource = dtToGrid;
                    StoreOpsVenc.DataBind();

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
        /// Controla el evento onRefresh del grid de operaciones vencidas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsVenc_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelOpsVencidas(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "OpsVencidas")]
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
        [DirectMethod(Namespace = "OpsVencidas")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "OperacionesVencidas";
                DataTable _dtOpsVencidas = HttpContext.Current.Session["DtOperacionesVencidas"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOpsVencidas, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabOpsVencidas);
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
        [DirectMethod(Namespace = "OpsVencidas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla la modificación del estatus de las operaciones seleccionadas en base de datos
        /// </summary>
        [DirectMethod(Namespace = "OpsVencidas", Timeout = 120000)]
        public void CambiaEstatusAOperaciones()
        {
            LogPCI unLog = new LogPCI(LH_ParabOpsVencidas);

            try
            {
                RowSelectionModel lasOperaciones = this.GridOpsVenc.SelectionModel.Primary as RowSelectionModel;

                foreach (SelectedRow operacion in lasOperaciones.SelectedRows)
                {
                    unLog.Info("INICIA ModificaEstatusOpVencida()");
                    LNOperaciones.ModificaEstatusOpVencida(Convert.ToInt64(operacion.RecordID), LH_ParabOpsVencidas);
                    unLog.Info("TERMINA ModificaEstatusOpVencida()");
                }

                X.Msg.Notify("", "Estatus de las operaciones modificado <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                ReloadGridOpsVenc();
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
        protected void ReloadGridOpsVenc()
        {
            LimpiaGridOpsVenc();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Tipo de Colectiva, estableciendo las colectivas correspondientes al tipo elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceColectivas(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabOpsVencidas);

            try
            {
                unLog.Info("INICIA ListaColectivasPorTipo()");
                StoreGpoComercial.DataSource = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColectiva.SelectedItem.Value), "", this.Usuario, Guid.Parse(
                    ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabOpsVencidas);
                unLog.Info("TERMINA ListaColectivasPorTipo()");
                StoreGpoComercial.DataBind();
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
