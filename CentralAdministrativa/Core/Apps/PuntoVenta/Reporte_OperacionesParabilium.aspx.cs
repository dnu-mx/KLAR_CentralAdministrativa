using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
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
using Utilerias;

namespace TpvWeb
{
    public partial class Reporte_OperacionesParabilium : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte de Operaciones
        private LogHeader LH_ParabRepOperaciones = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepOperaciones.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepOperaciones.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepOperaciones.User = this.Usuario.ClaveUsuario;
            LH_ParabRepOperaciones.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepOperaciones);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_OperacionesParabilium Page_Load()");

                if (!IsPostBack)
                {
                    dfFechaInicio.SetValue(DateTime.Now);
                    dfFechaInicio.MaxDate = DateTime.Today;

                    dfFechaFin.SetValue(DateTime.Now);
                    dfFechaFin.MaxDate = DateTime.Today;

                    dfFechaIniPresen.MaxDate = DateTime.Today;
                    dfFechaFinPresen.MaxDate = DateTime.Today;
                    
                    LlenaCombos();

                    PagingRepOpParab.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtRepOpParabilium", null);
                }

                log.Info("TERMINA Reporte_OperacionesParabilium Page_Load()");
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
            LogPCI pCI = new LogPCI(LH_ParabRepOperaciones);

            try
            {
                pCI.Info("INICIA ListaTiposColectivaSubemisor()");
                DataSet dsTiposColectiva = DAOColectiva.ListaTiposColectivaSubemisor
                    (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabRepOperaciones);
                pCI.Info("TERMINA ListaTiposColectivaSubemisor()");

                this.StoreTipoColectiva.DataSource = dsTiposColectiva;
                this.StoreTipoColectiva.DataBind();

                pCI.Info("INICIA ListaEstatusOperacion()");
                StoreEstatusOp.DataSource = DAOCatalogos.ListaEstatusOperacion(LH_ParabRepOperaciones);
                pCI.Info("TERMINA ListaEstatusOperacion()");
                StoreEstatusOp.DataBind();

                pCI.Info("INICIA ListaCodigosRespuestaExternos()");
                StoreMotivoRechazo.DataSource = 
                    DAOReportes.ListaCodigosRespuestaExternos(this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    LH_ParabRepOperaciones);
                pCI.Info("TERMINA ListaCodigosRespuestaExternos()");
                StoreMotivoRechazo.DataBind();

                pCI.Info("INICIA ListaEstatusPostOperacion()");
                StoreEstatusComp.DataSource =
                    DAOReportes.ListaEstatusPostOperacion(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    LH_ParabRepOperaciones);
                pCI.Info("TERMINA ListaEstatusPostOperacion()");
                StoreEstatusComp.DataBind();
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
        /// Controla el evento Seleccionar del combo Tipo de Colectiva del panel izquierdo,
        /// estableciendo las colectivas correspondientes al tipo elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceColectivas(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepOperaciones);

            try
            {
                unLog.Info("INICIA ListaColectivasPorTipo()");
                DataSet dsColectivas = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColectiva.SelectedItem.Value), string.Empty, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabRepOperaciones);
                unLog.Info("TERMINA ListaColectivasPorTipo()");

                List<ColectivaComboPredictivo> grupoList = new List<ColectivaComboPredictivo>();

                foreach (DataRow grupo in dsColectivas.Tables[0].Rows)
                {
                    var grupoCombo = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(grupo["ID_Colectiva"].ToString()),
                        ClaveColectiva = grupo["ClaveColectiva"].ToString(),
                        NombreORazonSocial = grupo["NombreORazonSocial"].ToString()
                    };
                    grupoList.Add(grupoCombo);
                }

                this.StoreGpoComercial.DataSource = grupoList;
                this.StoreGpoComercial.DataBind();
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

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.cBoxTipoColectiva.Reset();
            this.cBoxGpoComercial.Reset();

            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();

            this.dfFechaIniPresen.Reset();
            this.dfFechaFinPresen.Reset();

            this.tfHoraInicio.Reset();
            this.tfHoraFin.Reset();

            this.txtTarjeta.Reset();
            this.cBoxEstatusOp.Clear();
            this.cBoxMotivoRechazo.Reset();
            this.cBoxMotivoRechazo.Disabled = true;
            this.cBoxEstatusComp.Reset();

            LimpiaGridOperaciones();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridOperaciones()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtRepOpParabilium", null);
            StoreOperaciones.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridOperaciones();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Establece la fecha y hora inicial del periodo de consulta para la
        /// fecha de operación, concatenando los controles y sus valores
        /// </summary>
        /// <returns>DateTime con la fecha/hora inicial</returns>
        protected DateTime EstableceFechaHoraInicialOperacion()
        {
            TimeSpan ts = new TimeSpan();

            if (!TimeSpan.TryParse(this.tfHoraInicio.SelectedTime.ToString(), out ts))
            {
                ts = new TimeSpan(0, 0, 0);
            }
            else if (ts.TotalHours < 0 )
            {
                ts = new TimeSpan(0, 0, 0);
            }
            
            DateTime dt = new DateTime(this.dfFechaInicio.SelectedDate.Year,
                this.dfFechaInicio.SelectedDate.Month, this.dfFechaInicio.SelectedDate.Day,
                ts.Hours, ts.Minutes, ts.Seconds);

            return dt;
        }

        /// <summary>
        /// Establece la fecha y hora final del periodo de consulta para la
        /// fecha de operación, concatenando los controles y sus valores
        /// </summary>
        /// <returns>DateTime con la fecha/hora final</returns>
        protected DateTime EstableceFechaHoraFinalOperacion()
        {
            TimeSpan _ts = new TimeSpan();

            if (!TimeSpan.TryParse(this.tfHoraFin.SelectedTime.ToString(), out _ts))
            {
                _ts = new TimeSpan(0, 0, 0);
            }
            else if (_ts.TotalHours < 0)
            {
                _ts = new TimeSpan(0, 0, 0);
            }

            DateTime dt = new DateTime(this.dfFechaFin.SelectedDate.Year,
                this.dfFechaFin.SelectedDate.Month, this.dfFechaFin.SelectedDate.Day,
                _ts.Hours, _ts.Minutes, _ts.Seconds);

            return dt;
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelOperaciones(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            LogPCI unLog = new LogPCI(LH_ParabRepOperaciones);

            try
            {
                DataTable dtOpsParabilium = new DataTable();

                dtOpsParabilium = HttpContext.Current.Session["DtRepOpParabilium"] as DataTable;

                if (dtOpsParabilium == null)
                {
                    unLog.Info("INICIA ListarOperacionesParabilium()");
                    dtOpsParabilium = DAOReportes.ListarOperacionesParabilium(
                        Convert.ToInt64(this.cBoxGpoComercial.SelectedItem.Value),
                        EstableceFechaHoraInicialOperacion(), EstableceFechaHoraFinalOperacion(),
                        Convert.ToDateTime(this.dfFechaIniPresen.Value), Convert.ToDateTime(this.dfFechaFinPresen.Value),
                        txtTarjeta.Text, String.IsNullOrEmpty(cBoxEstatusOp.SelectedItem.Value) ? "" : this.cBoxEstatusOp.SelectedItem.Value,
                        String.IsNullOrEmpty(this.cBoxMotivoRechazo.SelectedItem.Value) ? -1 : int.Parse(this.cBoxMotivoRechazo.SelectedItem.Value),
                        String.IsNullOrEmpty(this.cBoxEstatusComp.SelectedItem.Value) ? -1 : int.Parse(this.cBoxEstatusComp.SelectedItem.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabRepOperaciones);
                    unLog.Info("TERMINA ListarOperacionesParabilium()");

                    if (dtOpsParabilium.Rows.Count < 1)
                    {
                        X.Msg.Alert("Operaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    DataTable dtOperacionesMask = Tarjetas.EnmascaraTablaConTarjetas(dtOpsParabilium, "Tarjeta", "Enmascara", LH_ParabRepOperaciones);

                    HttpContext.Current.Session.Add("DtRepOpParabilium", dtOperacionesMask);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtOpsParabilium.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Operaciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepOpParabilium.ClicDePaso()",
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
                    int TotalRegistros = dtOpsParabilium.Rows.Count;

                    (this.StoreOperaciones.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsParabilium.Clone();
                    DataTable dtToGrid = dtOpsParabilium.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsParabilium.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingRepOpParab.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingRepOpParab.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsParabilium.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreOperaciones.DataSource = dtToGrid;
                    this.StoreOperaciones.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Operaciones", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un error al consultar el Reporte de Operaciones").Show();
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
        protected void StoreOperaciones_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelOperaciones(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepOpParabilium")]
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
        [DirectMethod(Namespace = "RepOpParabilium")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteOperaciones";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtRepOpParabilium"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabRepOperaciones);
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
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 9).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 19).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 20).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 21).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 25).SetDataType(XLCellValues.DateTime);
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
        [DirectMethod(Namespace = "RepOpParabilium")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
