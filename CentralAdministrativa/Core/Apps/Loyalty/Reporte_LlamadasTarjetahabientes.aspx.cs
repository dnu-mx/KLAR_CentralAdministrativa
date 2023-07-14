using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
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

namespace CentroContacto
{
    public partial class Reporte_LlamadasTarjetahabientes : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Llamadas Tarjetahabientes
        private LogHeader LH_ParabRepLlamTH = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepLlamTH.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepLlamTH.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepLlamTH.User = this.Usuario.ClaveUsuario;
            LH_ParabRepLlamTH.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepLlamTH);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_LlamadasTarjetahabientes Page_Load()");

                if (!IsPostBack)
                {
                    dfFechaInicio.SetValue(DateTime.Now);
                    dfFechaInicio.MaxDate = DateTime.Today;

                    dfFechaFin.SetValue(DateTime.Now);
                    dfFechaFin.MaxDate = DateTime.Today;

                    LlenaComboSubemisor();

                    PagingRepLlamTH.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtRelLlamTH", null);
                }

                log.Info("TERMINA Reporte_LlamadasTarjetahabientes Page_Load()");
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
        protected void LlenaComboSubemisor()
        {
            LogPCI unLog = new LogPCI(LH_ParabRepLlamTH);

            try
            {
                unLog.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtColectivas = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabRepLlamTH);
                unLog.Info("TERMINA ListaColectivasSubEmisor()");

                List<ColectivaComboPredictivo> grupoList = new List<ColectivaComboPredictivo>();

                foreach (DataRow grupo in dtColectivas.Rows)
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
            catch (CAppException caEx)
            {
                unLog.Error(caEx.Mensaje());
                throw caEx;
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
            this.cBoxGpoComercial.Reset();

            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();

            LimpiaGridLlamadasTH();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones
        /// </summary>
        protected void LimpiaGridLlamadasTH()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtRelLlamTH", null);
            StoreLlamadasTH.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridLlamadasTH();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridLlamadasTH(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            LogPCI unLog = new LogPCI(LH_ParabRepLlamTH);

            try
            {
                DataTable dtLlamadasTH = new DataTable();

                dtLlamadasTH = HttpContext.Current.Session["DtRelLlamTH"] as DataTable;

                if (dtLlamadasTH == null)
                {
                    unLog.Info("INICIA ObtieneLlamadasSubemisor()");
                    dtLlamadasTH = DAOReportes.ObtieneLlamadasSubemisor(
                        Convert.ToInt32(this.cBoxGpoComercial.SelectedItem.Value), Convert.ToDateTime(this.dfFechaInicio.Value),
                        Convert.ToDateTime(this.dfFechaFin.Value), LH_ParabRepLlamTH);
                    unLog.Info("TERMINA ObtieneLlamadasSubemisor()");

                    if (dtLlamadasTH.Rows.Count < 1)
                    {
                        X.Msg.Alert("Llamadas", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    DataTable dtOperacionesMask = Tarjetas.EnmascaraTablaConTarjetas(dtLlamadasTH, "Tarjeta", "Enmascara", LH_ParabRepLlamTH);

                    HttpContext.Current.Session.Add("DtRelLlamTH", dtLlamadasTH);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtLlamadasTH.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Llamadas", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepLlamadasTH.ClicDePaso()",
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
                    int TotalRegistros = dtLlamadasTH.Rows.Count;

                    (this.StoreLlamadasTH.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtLlamadasTH.Clone();
                    DataTable dtToGrid = dtLlamadasTH.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtLlamadasTH.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingRepLlamTH.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingRepLlamTH.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtLlamadasTH.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreLlamadasTH.DataSource = dtToGrid;
                    this.StoreLlamadasTH.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Llamadas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Llamadas", "Ocurrió un error al consultar el Reporte de Llamadas").Show();
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
        protected void StoreLlamadasTH_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridLlamadasTH(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepLlamadasTH")]
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
        [DirectMethod(Namespace = "RepLlamadasTH")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteLlamadas";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtRelLlamTH"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabRepLlamTH);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Llamadas", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
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
        [DirectMethod(Namespace = "RepLlamadasTH")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
