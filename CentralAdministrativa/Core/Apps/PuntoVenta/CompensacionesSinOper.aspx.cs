using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.LogicaNegocio;
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
    /// <summary>
    /// Realiza y controla la carga de la página Compensaciones sin Operación 
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class CompensacionesSinOper : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Compensación sin Operaciones
        private LogHeader LH_CompensSinOper = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_CompensSinOper.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_CompensSinOper.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_CompensSinOper.User = this.Usuario.ClaveUsuario;
            LH_CompensSinOper.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_CompensSinOper);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CompensacionesSinOper Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddYears(-1);

                    this.dfFechaFin.MaxDate = DateTime.Today;

                    this.PagingRelCSO.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_CompensSinOper).Valor);
                    this.PagingOpsTransitoCSO.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_CompensSinOper).Valor);

                    HttpContext.Current.Session.Add("DtRelCSO", null);
                    HttpContext.Current.Session.Add("DtOpsTransitoCSO", null);
                }

                log.Info("TERMINA CompensacionesSinOper Page_Load()");
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
        /// Limpia los controles asociados a la búsqueda previa
        /// </summary>
        protected void limpiaBusquedaPrevia()
        {
            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();
            this.txtNumTarjeta.Reset();
            this.txtReferencia.Reset();

            LimpiaGrids();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados a los grids en pantalla
        /// </summary>
        protected void LimpiaGrids()
        {
            HttpContext.Current.Session.Add("DtRelCSO", null);
            this.StoreRelCSO.RemoveAll();

            HttpContext.Current.Session.Add("DtOpsTransitoCSO", null);
            this.GridOpsTransito.Title = "Operaciones";
            this.GridOpsTransito.Disabled = true;
            this.StoreOpsTransito.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGrids();

            Thread.Sleep(100);

            this.btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de compensaciones, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridPanelRelCSO(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_CompensSinOper);

            try
            {
                DataTable dtCompensaciones = new DataTable();

                dtCompensaciones = HttpContext.Current.Session["DtRelCSO"] as DataTable;

                if (dtCompensaciones == null)
                {
                    unLog.Info("INICIA ObtieneCompensacionRegistrosAut()");
                    dtCompensaciones = DAOCompensaciones.ObtieneCompensacionRegistrosAut(
                        Convert.ToDateTime(this.dfFechaInicio.Value), Convert.ToDateTime(this.dfFechaFin.Value),
                        this.txtNumTarjeta.Text, this.txtReferencia.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_CompensSinOper);
                    unLog.Info("TERMINA ObtieneCompensacionRegistrosAut()");

                    if (dtCompensaciones.Rows.Count < 1)
                    {
                        X.Msg.Alert("Compensaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtCompensaciones = Tarjetas.EnmascaraTablaConTarjetas(dtCompensaciones, "ClaveMA", "Enmascara", LH_CompensSinOper);

                    HttpContext.Current.Session.Add("DtRelCSO", dtCompensaciones);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtCompensaciones.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Compensaciones", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RelCSO.ClicDePaso()",
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
                    int TotalRegistros = dtCompensaciones.Rows.Count;

                    (this.StoreRelCSO.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCompensaciones.Clone();
                    DataTable dtToGrid = dtCompensaciones.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCompensaciones.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingRelCSO.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingRelCSO.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCompensaciones.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreRelCSO.DataSource = dtToGrid;
                    this.StoreRelCSO.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Compensaciones", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Compensaciones", "Ocurrió un error al consultar las Compensaciones").Show();
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
        protected void StoreRelCSO_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelRelCSO(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RelCSO")]
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
            try
            {
                string reportName = "Compensaciones";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtRelCSO"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_CompensSinOper);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Compensaciones", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                //ws.Column(28).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    //ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    //ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    //ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                    //ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                    //ws.Cell(rowsCounter, 15).SetDataType(XLCellValues.DateTime);
                    //ws.Cell(rowsCounter, 16).SetDataType(XLCellValues.Number);
                    //ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.Number);
                    //ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.Number);
                    //ws.Cell(rowsCounter, 22).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de registros de compensación.
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string numTarjeta = string.Empty;
                string tarjetaMask = string.Empty;
                string autorizacion = string.Empty;
                string importe = string.Empty;
                string moneda = string.Empty;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] registroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in registroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_RegistroCompensacion": this.hdnIdRegComp.Value = column.Value; break;
                        case "ClaveMA": tarjetaMask = column.Value; break;
                        case "Tarjeta": numTarjeta = column.Value; break;
                        case "Autorizacion": autorizacion = column.Value; break;
                        case "ImporteOperacion": importe = string.Format("{0:C2}", decimal.Parse(column.Value)); break;
                        case "MonedaOperacion": moneda = column.Value; break;
                        default:
                            break;
                    }
                }

                this.hdnCardMsk.Value = tarjetaMask;
                LlenaGridOpsTransito(numTarjeta);

                this.GridOpsTransito.Title += "  -  Tarjeta: " + tarjetaMask + ". Autorización: " + autorizacion +
                    ". Importe: " + importe + ". Moneda: " + moneda;
                this.GridOpsTransito.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Relación de CSO", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_CompensSinOper);
                unLog.ErrorException(ex);
                X.Msg.Alert("Relación de CSO", "Ocurrió un error al obtener los detalles de la Compensación.").Show();
            }
        }

        /// <summary>
        /// Llena el grid de operaciones con los datos consultados en base de datos
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta seleccionado</param>
        protected void LlenaGridOpsTransito(string tarjeta)
        {
            LogPCI pCI = new LogPCI(LH_CompensSinOper);

            try
            {
                this.StoreOpsTransito.RemoveAll();

                pCI.Info("INICIA ObtieneOperacionesDeTarjeta()");
                DataTable dtOpers = DAOCompensaciones.ObtieneOperacionesDeTarjeta(
                    tarjeta, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_CompensSinOper);
                pCI.Info("TERMINA ObtieneOperacionesDeTarjeta()");

                if (dtOpers.Rows.Count < 1)
                {
                    throw new CAppException(8011, "No existen coincidencias con la tarjeta seleccionada");
                }

                dtOpers = Tarjetas.EnmascaraTablaConTarjetas(dtOpers, "NumTarjeta", "Enmascara", LH_CompensSinOper);

                this.StoreOpsTransito.DataSource = dtOpers;
                this.StoreOpsTransito.DataBind();
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un error al obtener las Operaciones").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        [DirectMethod(Namespace = "RelCSO", Timeout = 120000)]
        public void btnRelacionarClick()
        {
            LogPCI pCI = new LogPCI(LH_CompensSinOper);

            try
            {
                this.StoreOpsTransito.RemoveAll();

                pCI.Info("INICIA RelacionaCompensacionConOperacion()");
                LNCompensaciones.RelacionaCompensacionConOperacion(Convert.ToInt64(this.hdnIdRegComp.Value),
                    Convert.ToInt64(this.hdnIdOp.Value), this.Usuario, LH_CompensSinOper);
                pCI.Info("TERMINA RelacionaCompensacionConOperacion()");

                X.Msg.Notify("", "Relación de Compensación y Operación creada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                limpiaBusquedaPrevia();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Relacionar", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Relacionar", "Ocurrió un error al relacionar la Compensación con la Operación").Show();
            }

            finally
            {
                X.Mask.Hide();
            }

        }
    }
}