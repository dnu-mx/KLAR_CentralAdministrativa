using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
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
    public partial class Reporte_ClabePendientes1 : DALCentralAplicaciones.PaginaBaseCAPP
    {
        private LogHeader LH_Reporte = new LogHeader();

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_Reporte.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_Reporte.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_Reporte.User = this.Usuario.ClaveUsuario;
            LH_Reporte.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_Reporte);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_ClabePendientes Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaColectivasSubEmisor()");
                    DataTable dtColectivas =
                        DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_Reporte);
                    log.Info("TERMINA ListaColectivasSubEmisor()");

                    List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow drCol in dtColectivas.Rows)
                    {
                        var colectivaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt32(drCol["ID_Colectiva"]),
                            ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                            NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                        };
                        ComboList.Add(colectivaCombo);
                    }

                    this.StoreCC.DataSource = ComboList;
                    this.StoreCC.DataBind();

                    _PagingCLABEs.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtCuentasCLABE", null);
                }

                log.Info("TERMINA Reporte_ClabePendientes Page_Load()");
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
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
        /// Controla el evento onRefresh del grid de operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreCLABEs_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridCLABEs(inicio, columna, orden);
        }


        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            this.StoreCLABEs.RemoveAll();
            HttpContext.Current.Session.Add("DtCuentasCLABE", null);

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }


        protected void LlenaGridCLABEs(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_Reporte);

            try
            {
                int idColectiva = Convert.ToInt32(this.cBoxCC.SelectedItem.Value);
                DateTime fechaInicial = this.dfFecha.SelectedDate;
                string estatus = this.cBoxEstatus.SelectedItem.Value;
                string tarjeta = this.txtTarjeta.Text;

                log.Info("INICIA ListaClabesPendientes()");
                DataSet ds = DAOReportes.ListaClabesPendientes(idColectiva, fechaInicial, estatus, tarjeta,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_Reporte);
                log.Info("TERMINA ListaClabesPendientes()");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    this.StoreCLABEs.DataSource = "";
                    this.StoreCLABEs.DataBind();
                    this.btnEnviar.Disabled = true;
                    btnExportExcel.Disabled = true;
                    X.Msg.Alert("Cuentas CLABE", "No existen coincidencias con la búsqueda solicitada").Show();
                    return;
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_MaxRegsPagina", LH_Reporte).Valor);

                DataTable dtEnmascara = Tarjetas.EnmascaraTablaConTarjetas(ds.Tables[0], "NumeroTarjeta", "Enmascara", LH_Reporte);

                DataTable dtCLABEs = CreaIndicesATabla(dtEnmascara);

                HttpContext.Current.Session.Add("DtCuentasCLABE", dtCLABEs);

                if (dtCLABEs.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Cuentas CLABE", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "RepClabe.ClicDePaso()",
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
                    int TotalRegistros = dtCLABEs.Rows.Count;

                    (this.StoreCLABEs.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtCLABEs.Clone();
                    DataTable dtToGrid = dtCLABEs.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCLABEs.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + _PagingCLABEs.PageSize) < TotalRegistros ?
                        (RegistroInicial + _PagingCLABEs.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtCLABEs.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    List<PayCard> ComboList = new List<PayCard>();

                    foreach (DataRow drCol in dtToGrid.Rows)
                    {
                        var Combo = new PayCard()
                        {
                            IdReporte = Convert.ToInt32(drCol["IdReporte"]),
                            NumeroTarjeta = drCol["NumeroTarjeta"].ToString(),
                            CuentaInterna = drCol["CuentaCACAO"].ToString(),
                            ID = drCol["ID"].ToString(),
                            FechaOperacion = Convert.ToDateTime(drCol["FechaOperacion"]),
                            EstatusEnvio = drCol["EstatusEnvio"].ToString(),
                            Mensaje = drCol["Mensaje"].ToString()
                        };
                        ComboList.Add(Combo);
                    }

                    this.StoreCLABEs.DataSource = ComboList;
                    this.StoreCLABEs.DataBind();

                    this.btnEnviar.Disabled = false;
                    this.btnExportExcel.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cuentas CLABE", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                X.Msg.Alert("Cuentas CLABE", "Ocurrió un error al obtener las cuentas CLABE no generadas").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Añade una columna índice a la tabla de resultados recibida desde base de datos
        /// </summary>
        /// <param name="elOriginal">Tabla con los datos originales de la consulta</param>
        /// <returns>DataTable con la columna índice añadida</returns>
        protected DataTable CreaIndicesATabla(DataTable elOriginal)
        {
            LogPCI log = new LogPCI(LH_Reporte);

            try
            {
                DataTable dt = new DataTable();
                int fila = 0;

                dt.Columns.Add("IdReporte");
                dt.Columns.Add("IdLog");
                dt.Columns.Add("NumeroTarjeta");
                dt.Columns.Add("CuentaInterna");
                dt.Columns.Add("ID");
                dt.Columns.Add("FechaOperacion");
                dt.Columns.Add("EstatusEnvio");
                dt.Columns.Add("Mensaje");
                dt.Columns.Add("Enmascara");

                foreach (DataRow unaFila in elOriginal.Rows)
                {
                    dt.Rows.Add();

                    dt.Rows[fila]["IdReporte"] = fila + 1;
                    dt.Rows[fila]["IdLog"] = unaFila["IdLog"];
                    dt.Rows[fila]["NumeroTarjeta"] = unaFila["NumeroTarjeta"].ToString();
                    dt.Rows[fila]["CuentaInterna"] = unaFila["CuentaCACAO"].ToString();
                    dt.Rows[fila]["ID"] = unaFila["ID"].ToString();
                    dt.Rows[fila]["FechaOperacion"] = unaFila["FechaOperacion"].ToString();
                    dt.Rows[fila]["EstatusEnvio"] = unaFila["EstatusEnvio"].ToString();
                    dt.Rows[fila]["Mensaje"] = unaFila["Mensaje"].ToString();
                    dt.Rows[fila]["Enmascara"] = unaFila["Enmascara"].ToString();

                    fila++;
                }

                return dt;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8012, "Ocurrió un error al generar los índices del reporte");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.cBoxCC.Reset();
            this.dfFecha.Reset();
            this.cBoxEstatus.Reset();
            this.txtTarjeta.Reset();

            this.StoreCLABEs.RemoveAll();
        }

        [DirectMethod(Namespace = "RepClabe")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        [DirectMethod(Namespace = "RepClabe")]
        public void ExportDTDetalleToExcel()
        {
            LogPCI unLog = new LogPCI(LH_Reporte);
            try
            {
                string reportName = "ReporteCuentasCLABE";
                DataTable _dsResultados = HttpContext.Current.Session["DtCuentasCLABE"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dsResultados, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWSClabePendientes(ws, ws.Column(1).CellsUsed().Count());

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
                unLog.Error("Error al exportar el reporte a Excel");
                unLog.ErrorException(ex); ;
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

        protected IXLWorksheet FormatWSClabePendientes(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();
                ws.Column(9).Hide();

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_Reporte);
            int resp = 0;
            try
            {
                RowSelectionModel losWebhook = this.GridCLABEs.SelectionModel.Primary as RowSelectionModel;

                int contador = losWebhook.SelectedRows.Count;

                if (contador == 0)
                {
                    X.Msg.Alert("Reporte-PayCard", "Por favor, selecciona al menos un Webhook.").Show();
                    return;
                }


                foreach (var item in losWebhook.SelectedRows)
                {
                    string IdLog = item.RecordID;
                    log.Info("INICIA ActualizaEstatusClabePendiente()");
                    resp = LNClabePendiente.ActualizaEstatusClabePendiente(IdLog, LH_Reporte );
                    log.Info("TERMINA ActualizaEstatusClabePendiente()");
                }
              
                if(resp == 1)
                {
                    this.btnEnviar.Disabled = true;
                    this.btnExportExcel.Disabled = true;
                    losWebhook.SelectedRows.Clear();
                    StoreCLABEs.RemoveAll();
                    X.Msg.Alert("Reporte-PayCard", "El envío se realizó correctamente").Show();
                }
                else
                {
                    X.Msg.Alert("Reporte-PayCard", "El envío no se realizó correctamente").Show();
                }
                
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reenviar", err.Mensaje());
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Reenviar", "Ocurrió un error al reenviar los Webhook");
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "RepClabe")]
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
            ExportDTDetalleToExcel();
        }
    }
}