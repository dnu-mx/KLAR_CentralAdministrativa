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
    /// Realiza y controla la carga de la página Consulta de Relación de Compensaciones sin Operación sin Operación
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class ConsultaCompSinOper : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Consulta de Relación de Compensaciones sin Operación sin Operación
        private LogHeader LH_ConsultaCompSinOper = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ConsultaCompSinOper.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ConsultaCompSinOper.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ConsultaCompSinOper.User = this.Usuario.ClaveUsuario;
            LH_ConsultaCompSinOper.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ConsultaCompSinOper);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConsultaCompSinOper Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaIniPres.MaxDate = DateTime.Today;
                    this.dfFechaIniPres.MinDate = DateTime.Today.AddYears(-1);

                    this.dfFechaFinPres.MaxDate = DateTime.Today;

                    log.Info("INICIA ObtieneEstatusRelacionCSO()");
                    this.StoreEstatusRelCSO.DataSource =
                        DAOCompensaciones.ObtieneEstatusRelacionCSO(LH_ConsultaCompSinOper);
                    log.Info("TERMINA ObtieneEstatusRelacionCSO()");

                    this.StoreEstatusRelCSO.DataBind();
                    this.cBoxEstatusRelCSO.SetValue(-1);

                    this.PagingConsultaRelCSO.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ConsultaCompSinOper).Valor);

                    HttpContext.Current.Session.Add("DtRelacionCSOs", null);
                }

                log.Info("TERMINA ConsultaCompSinOper Page_Load()");
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
            this.dfFechaIniPres.Reset();
            this.dfFechaFinPres.Reset();
            this.txtNoTarjeta.Reset();
            this.txtRefer.Reset();

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
            HttpContext.Current.Session.Add("DtRelacionCSOs", null);
            this.StoreConsultaRelCSO.RemoveAll();

            this.GridOperacionRel.Title = "Detalle de la Operación";
            this.GridOperacionRel.Disabled = true;
            this.StoreOperacionRel.RemoveAll();
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
        protected void LlenaGridConsultaRelCSO(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ConsultaCompSinOper);

            try
            {
                DataTable dtRelacionCSOs = new DataTable();

                dtRelacionCSOs = HttpContext.Current.Session["DtRelacionCSOs"] as DataTable;

                if (dtRelacionCSOs == null)
                {
                    unLog.Info("INICIA ObtieneRelacionesCompensacionSinOperacion()");
                    dtRelacionCSOs = DAOCompensaciones.ObtieneRelacionesCompensacionSinOperacion(
                        Convert.ToDateTime(this.dfFechaIniPres.Value), Convert.ToDateTime(this.dfFechaFinPres.Value),
                        this.txtNoTarjeta.Text, this.txtRefer.Text, Convert.ToInt32(string.IsNullOrEmpty(
                        this.cBoxEstatusRelCSO.SelectedItem.Value) ? "-1" : this.cBoxEstatusRelCSO.SelectedItem.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ConsultaCompSinOper);
                    unLog.Info("TERMINA ObtieneRelacionesCompensacionSinOperacion()");

                    if (dtRelacionCSOs.Rows.Count < 1)
                    {
                        X.Msg.Alert("Relación de Compensaciones sin Operación", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtRelacionCSOs = Tarjetas.EnmascaraTablaConTarjetas(dtRelacionCSOs, "ClaveMA", "Enmascara", LH_ConsultaCompSinOper);

                    HttpContext.Current.Session.Add("DtRelacionCSOs", dtRelacionCSOs);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtRelacionCSOs.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Relación de Compensaciones sin Operación", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RelacionCSOs.ClicDePaso()",
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
                    int TotalRegistros = dtRelacionCSOs.Rows.Count;

                    (this.StoreConsultaRelCSO.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtRelacionCSOs.Clone();
                    DataTable dtToGrid = dtRelacionCSOs.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtRelacionCSOs.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingConsultaRelCSO.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingConsultaRelCSO.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtRelacionCSOs.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreConsultaRelCSO.DataSource = dtToGrid;
                    this.StoreConsultaRelCSO.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Relación de Compensaciones sin Operación", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Relación de Compensaciones sin Operación", "Ocurrió un error al consultar la Relación de Compensaciones").Show();
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
        protected void StoreConsultaRelCSO_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridConsultaRelCSO(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RelacionCSOs")]
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
                string reportName = "Relación de Compensaciones sin Operación";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtRelacionCSOs"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ConsultaCompSinOper);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Relación de Compensaciones sin Operación", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                string motivo = string.Empty;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] registroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in registroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_RelacionCSO": this.hdnIdRelacionCSO.Value = column.Value; break;
                        case "ID_Operacion": this.hdnIdOpRelacionada.Value = column.Value; break;
                        case "ClaveMA": tarjetaMask = column.Value; break;
                        case "Tarjeta": numTarjeta = column.Value; break;
                        case "Autorizacion": autorizacion = column.Value; break;
                        case "ImporteOperacion": importe = string.Format("{0:C2}", decimal.Parse(column.Value)); break;
                        case "MonedaOperacion": moneda = column.Value; break;
                        case "Motivo": motivo = column.Value; break;
                        default:
                            break;
                    }
                }

                this.hdnCardMsk.Value = tarjetaMask;
                LlenaGridOperacionRel();

                this.GridOperacionRel.Title += "  -  Tarjeta: " + tarjetaMask + ". Autorización: " + autorizacion +
                    ". Importe: " + importe + ". Moneda: " + moneda + ". Motivo: " + motivo;
                this.GridOperacionRel.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Operación", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ConsultaCompSinOper);
                unLog.ErrorException(ex);
                X.Msg.Alert("Detalle de Operación", "Ocurrió un error al obtener los detalles de la Operación.").Show();
            }
        }

        /// <summary>
        /// Llena el grid de operaciones con los datos consultados en base de datos
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta seleccionado</param>
        protected void LlenaGridOperacionRel()
        {
            LogPCI pCI = new LogPCI(LH_ConsultaCompSinOper);

            try
            {
                this.StoreOperacionRel.RemoveAll();

                pCI.Info("INICIA ObtieneOperacionRelacionCSO()");
                DataTable dtOperacion = DAOCompensaciones.ObtieneOperacionRelacionCSO(
                    Convert.ToInt64(this.hdnIdOpRelacionada.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ConsultaCompSinOper);
                pCI.Info("TERMINA ObtieneOperacionRelacionCSO()");

                if (dtOperacion.Rows.Count < 1)
                {
                    throw new CAppException(8011, "No existen coincidencias con la relación seleccionada");
                }

                dtOperacion = Tarjetas.EnmascaraTablaConTarjetas(dtOperacion, "NumTarjeta", "Enmascara", LH_ConsultaCompSinOper);

                this.StoreOperacionRel.DataSource = dtOperacion;
                this.StoreOperacionRel.DataBind();
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un error al obtener el Detalle de la Operación").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución del comando en el grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            this.txtMotivo.Clear();

            this.WdwMotivoCancelacion.Title = "Cancelar Relación";
            this.WdwMotivoCancelacion.Title += " - Tarjeta: " + this.hdnCardMsk.Value.ToString() +
                ", ID Operación: " + this.hdnIdOpRelacionada.Value.ToString();

            this.WdwMotivoCancelacion.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana de motivo de cancelación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnOkMotivo_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ConsultaCompSinOper);

            try
            {
                log.Info("INICIA CancelaRelacionCompensacionSinOperacion()");
                LNCompensaciones.CancelaRelacionCompensacionSinOperacion(
                    Convert.ToInt64(this.hdnIdRelacionCSO.Value), this.txtMotivo.Text,
                    this.Usuario, LH_ConsultaCompSinOper);
                log.Info("TERMINA CancelaRelacionCompensacionSinOperacion()");

                this.WdwMotivoCancelacion.Hide();

                X.Msg.Notify("", "Relación cancelada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                limpiaBusquedaPrevia();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cancelación de Relación", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Cancelación de Relación", "Ocurrió un error al cancelar la Relación de la Compensación y la Operación").Show();
            }
        }
    }
}