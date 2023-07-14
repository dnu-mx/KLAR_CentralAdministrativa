using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALOperacionesEvertec.BaseDatos;
using DALOperacionesEvertec.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace OperacionesEvertec
{
    public partial class Reporte_OperacionesApiCacao : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Repote Operaciones API Cacao
        private LogHeader LH_RepOpsApiCacao = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_RepOpsApiCacao.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_RepOpsApiCacao.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_RepOpsApiCacao.User = this.Usuario.ClaveUsuario;
            LH_RepOpsApiCacao.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_RepOpsApiCacao);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_OperacionesApiCacao Page_Load()");

                if (!IsPostBack)
                {
                    dfFIniApiCacao.SetValue(DateTime.Now);
                    dfFIniApiCacao.MaxDate = DateTime.Today;

                    dfFFinApiCacao.SetValue(DateTime.Now);
                    dfFFinApiCacao.MaxDate = DateTime.Today;

                    dfFIniPresen.MaxDate = DateTime.Today;
                    dfFFinPresen.MaxDate = DateTime.Today;

                    dfFIniProc.MaxDate = DateTime.Today;
                    dfFFinProc.MaxDate = DateTime.Today;

                    LlenaCombos();

                    PagingOpsApiCacao.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_RepOpsApiCacao).Valor);

                    HttpContext.Current.Session.Add("DtOperacionesApiCacao", null);
                }

                log.Info("TERMINA Reporte_OperacionesApiCacao Page_Load()");
            }

            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
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
        /// Obtiene los catálogos de clientes, estatus de autorización y estatus de
        /// compensación para establecerlos en los combos correspondientes
        /// </summary>
        protected void LlenaCombos()
        {
            LogPCI unLog = new LogPCI(LH_RepOpsApiCacao);

            try
            {
                unLog.Info("INICIA ListaClientesCacao()");
                DataTable dtClientes = DAOReportesOpsEvertec.ListaSubEmisores(
                   this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_RepOpsApiCacao);
                unLog.Info("TERMINA ListaClientesCacao()");

                List<Cliente> listaClientes = new List<Cliente>();

                foreach (DataRow cliente in dtClientes.Rows)
                {
                    var clienteCombo = new Cliente()
                    {
                        ID_ClienteCacao = Convert.ToInt32(cliente["ID_ClienteCacao"].ToString()),
                        Clave = cliente["Clave"].ToString(),
                        NombreCliente = cliente["NombreCliente"].ToString()
                    };
                    listaClientes.Add(clienteCombo);
                }

                this.StoreClientes.DataSource = listaClientes;
                this.StoreClientes.DataBind();

                unLog.Info("INICIA ListaEstatusAutorizacion()");
                this.StoreEstatusAut.DataSource = DAOReportesOpsEvertec.ListaEstatusAutorizacion(LH_RepOpsApiCacao);
                unLog.Info("TERMINA ListaEstatusAutorizacion()");
                this.StoreEstatusAut.DataBind();

                unLog.Info("INICIA ListaEstatusPostOperacion()");
                this.StoreEstatusComp.DataSource = DAOReportesOpsEvertec.ListaEstatusPostOperacion(LH_RepOpsApiCacao);
                unLog.Info("TERMINA ListaEstatusPostOperacion()");
                this.StoreEstatusComp.DataBind();
            }
            catch (CAppException err)
            {
                unLog.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
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
            this.cBoxClienteCacao.Reset();

            this.dfFIniApiCacao.Reset();
            this.dfFFinApiCacao.Reset();

            this.dfFIniPresen.Reset();
            this.dfFFinPresen.Reset();

            this.dfFIniProc.Reset();
            this.dfFFinProc.Reset();

            this.txtTarjeta.Reset();
            this.cBoxEstatusOp.Clear();
            this.cBoxEstatusComp.Reset();

            LimpiaGridOpsApiCacao();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridOpsApiCacao()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtOperacionesApiCacao", null);
            StoreOpsApiCacao.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridOpsApiCacao();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsApiCacao(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            LogPCI pCI = new LogPCI(LH_RepOpsApiCacao);

            try
            {
                DataTable dtOpsApiCacao = new DataTable();

                dtOpsApiCacao = HttpContext.Current.Session["DtOperacionesApiCacao"] as DataTable;
                
                if (dtOpsApiCacao == null)
                {
                    pCI.Info("INICIA ObtieneReporteOperacionesApiCacao()");
                    dtOpsApiCacao = DAOReportesOpsEvertec.ObtieneReporteOperacionesApiCacao(
                        Convert.ToInt32(cBoxClienteCacao.SelectedItem.Value),
                        Convert.ToDateTime(dfFIniApiCacao.SelectedDate), Convert.ToDateTime(dfFFinApiCacao.SelectedDate),
                        Convert.ToDateTime(dfFIniPresen.SelectedDate), Convert.ToDateTime(dfFFinPresen.SelectedDate),
                        Convert.ToDateTime(dfFIniProc.SelectedDate), Convert.ToDateTime(dfFFinProc.SelectedDate),
                        txtTarjeta.Text,
                        String.IsNullOrEmpty(cBoxEstatusOp.SelectedItem.Value) ? -1 : int.Parse(cBoxEstatusOp.SelectedItem.Value),
                        String.IsNullOrEmpty(cBoxEstatusComp.SelectedItem.Value) ? -1 : int.Parse(cBoxEstatusComp.SelectedItem.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_RepOpsApiCacao);
                    pCI.Info("TERMINA ObtieneReporteOperacionesApiCacao()");

                    HttpContext.Current.Session.Add("DtOperacionesApiCacao", dtOpsApiCacao);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_RepOpsApiCacao).Valor);

                if (dtOpsApiCacao.Rows.Count < 1)
                {
                    X.Msg.Alert("Operaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtOpsApiCacao.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Operaciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepOpApiCacao.ClicDePaso()",
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
                    int TotalRegistros = dtOpsApiCacao.Rows.Count;

                    (this.StoreOpsApiCacao.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsApiCacao.Clone();
                    DataTable dtToGrid = dtOpsApiCacao.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsApiCacao.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingOpsApiCacao.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingOpsApiCacao.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsApiCacao.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreOpsApiCacao.DataSource = dtToGrid;
                    StoreOpsApiCacao.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Operaciones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un error al obtener el Reporte de Operaciones").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsApiCacao_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsApiCacao(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepOpApiCacao")]
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
        [DirectMethod(Namespace = "RepOpApiCacao")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteOperaciones";
                DataTable _dtOpsApiCacao = HttpContext.Current.Session["DtOperacionesApiCacao"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOpsApiCacao, reportName);

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
                LogPCI pCI = new LogPCI(LH_RepOpsApiCacao);
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
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 14).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 15).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 16).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 21).SetDataType(XLCellValues.DateTime);
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
        [DirectMethod(Namespace = "RepOpApiCacao")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
