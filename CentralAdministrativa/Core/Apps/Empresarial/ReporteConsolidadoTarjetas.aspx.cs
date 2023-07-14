using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
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

namespace Empresarial
{
    public partial class ReporteConsolidadoTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Consolidados
        private LogHeader LH_ParabRepConsTarj = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte Consolidado de Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepConsTarj.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepConsTarj.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepConsTarj.User = this.Usuario.ClaveUsuario;
            LH_ParabRepConsTarj.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepConsTarj);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ReporteConsolidadoTarjetas Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFI_RepConsTarj.MaxDate = DateTime.Today;
                    this.dfFI_RepConsTarj.MinDate = DateTime.Today.AddMonths(-6);

                    this.dfFF_RepConsTarj.MaxDate = DateTime.Today;

                    EstableceSubEmisores();

                    this.PagingConsTarj.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabRepConsTarj).Valor);

                    HttpContext.Current.Session.Add("DtRepConsTarj", null);
                }

                log.Info("TERMINA ReporteConsolidadoTarjetas Page_Load()");
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
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI log = new LogPCI(LH_ParabRepConsTarj);

            log.Info("INICIA ListaColectivasSubEmisor()");
            DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabRepConsTarj);
            log.Info("TERMINA ListaColectivasSubEmisor()");

            List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

            foreach (DataRow drCol in dtSubEmisores.Rows)
            {
                var colectivaCombo = new ColectivaComboPredictivo()
                {
                    ID_Colectiva = Convert.ToInt64(drCol["ID_Colectiva"].ToString()),
                    ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                    NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                };
                ComboList.Add(colectivaCombo);
            }

            this.StoreSubemisor.DataSource = ComboList;
            this.StoreSubemisor.DataBind();
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Subemisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceProductos(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepConsTarj);

            try
            {
                unLog.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProducto.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxSubemisor.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabRepConsTarj);
                unLog.Info("TERMINA ObtieneProductosDeColectiva()");

                this.StoreProducto.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al establecer los Productos del Subemisor").Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Tipo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceEstatus(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepConsTarj);

            try
            {
                this.StoreEstatus.RemoveAll();

                switch (this.cBoxTipo.SelectedItem.Value)
                {
                    case "Tarjetas":
                        this.StoreEstatus.DataSource = new object[]
                        {
                            new object[] { "TOD", "Todos" },
                            new object[] { "ACT", "Activadas" },
                            new object[] { "ACOP", "Activadas con Operacion" },
                            new object[] { "CAN", "Cancelada" },
                            new object[] { "INA", "Inactiva" }
                        };
                        break;

                    case "Cuentas":
                        this.StoreEstatus.DataSource = new object[]
                        {
                            new object[] { "TOD", "Todos" },
                            new object[] { "ACT", "Activa" },
                            new object[] { "CER", "Cerrada" }
                        };
                        break;

                    default: //TODOS
                        this.StoreEstatus.DataSource = new object[]
                        {
                            new object[] { "TOD", "Todos" }
                        };
                        break;
                }

                this.StoreEstatus.DataBind();

                this.cBoxEstatus.SelectedIndex = 0;
                this.cBoxEstatus.FireEvent("select");
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Estatus", "Ocurrió un error al establecer los Estatus").Show();
            }
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de saldos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridConsTarjetas(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI pCI = new LogPCI(LH_ParabRepConsTarj);
            this.btnExcelRepConsTarj.Disabled = true;

            try
            {
                DataTable dtConsTarjCtas = new DataTable();

                dtConsTarjCtas = HttpContext.Current.Session["DtRepConsTarj"] as DataTable;

                if (dtConsTarjCtas == null)
                {
                    pCI.Info("INICIA ReporteConsolidadoTarjetasCuentas()");
                    dtConsTarjCtas = DAOMediosAcceso.ReporteConsolidadoTarjetasCuentas(
                        Convert.ToInt64(this.cBoxSubemisor.SelectedItem.Value),
                        Convert.ToInt32(this.cBoxProducto.SelectedItem.Value), this.cBoxTipo.SelectedItem.Value,
                        this.cBoxEstatus.SelectedItem.Value, Convert.ToDateTime(this.dfFI_RepConsTarj.SelectedDate),
                        Convert.ToDateTime(this.dfFF_RepConsTarj.SelectedDate), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepConsTarj);
                    pCI.Info("TERMINA ReporteConsolidadoTarjetasCuentas()");

                    if (dtConsTarjCtas.Rows.Count < 1)
                    {
                        X.Msg.Alert("Consolidados", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    HttpContext.Current.Session.Add("DtRepConsTarj", dtConsTarjCtas);
                }               

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabRepConsTarj).Valor);

                if (dtConsTarjCtas.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Consolidados", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepConsTarjCtas.ClicDePaso()",
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
                    int TotalRegistros = dtConsTarjCtas.Rows.Count;

                    (this.StoreConsTarjetas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtConsTarjCtas.Clone();
                    DataTable dtToGrid = dtConsTarjCtas.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtConsTarjCtas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingConsTarj.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingConsTarj.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtConsTarjCtas.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreConsTarjetas.DataSource = dtToGrid;
                    StoreConsTarjetas.DataBind();

                    this.btnExcelRepConsTarj.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consolidados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Consolidados", "Ocurrió un error al obtener el reporte").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid
        /// </summary>
        protected void LimpiaGridConsTarjetas()
        {
            this.btnExcelRepConsTarj.Disabled = true;

            HttpContext.Current.Session.Add("DtRepConsTarj", null);

            this.StoreConsTarjetas.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.cBoxSubemisor.Reset();
            this.cBoxProducto.Reset();
            this.cBoxTipo.Reset();
            this.cBoxEstatus.Reset();
            this.dfFI_RepConsTarj.Reset();
            this.dfFF_RepConsTarj.Reset();
            
            LimpiaGridConsTarjetas();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridConsTarjetas();

            Thread.Sleep(100);

            this.btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "RepConsTarjCtas")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "ReporteConsolidados";
                DataTable _dtConsTarjCtas = HttpContext.Current.Session["DtRepConsTarj"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtConsTarjCtas, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepConsTarj);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Consolidados", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                //ws.Column(1).Hide();
                //ws.Column(6).Hide();

                //for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                //{
                //    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
                //}

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreConsTarjetas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridConsTarjetas(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepConsTarjCtas")]
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
        [DirectMethod(Namespace = "RepConsTarjCtas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}