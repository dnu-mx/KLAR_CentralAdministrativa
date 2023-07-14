using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using DNU.Cifrado.DES;
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
using static DALPuntoVentaWeb.BaseDatos.DAOReportes;

namespace TpvWeb
{
    public partial class ConsultaWebhookOnboarding : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Webhook Onboarding
        private LogHeader LH_ParabWbhkOnB = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabWbhkOnB.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabWbhkOnB.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabWbhkOnB.User = this.Usuario.ClaveUsuario;
            LH_ParabWbhkOnB.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabWbhkOnB);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConsultaWebhookOnboarding Page_Load()");

                if (!IsPostBack)
                {
                    LlenaCombos();

                    PagingWHOnboard.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtWHOnboard", null);
                }

                log.Info("TERMINA ConsultaWebhookOnboarding Page_Load()");
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
        /// Obtiene los catálogos de SubEmisores y tipos de operación para establecerlos
        /// en los combos correspondientes
        /// </summary>
        protected void LlenaCombos()
        {
            LogPCI logPCI = new LogPCI(LH_ParabWbhkOnB);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtColectivas =
                        DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabWbhkOnB);
                logPCI.Info("TERMINA ListaColectivasSubEmisor()");

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

                this.StoreCC.DataSource = grupoList;
                this.StoreCC.DataBind();


                logPCI.Info("INICIA ListaEstatusEnvioOnboarding()");
                this.StoreEstatusWH.DataSource = DAOWebhook.ListaEstatusEnvioOnboarding(LH_ParabWbhkOnB);
                logPCI.Info("TERMINA ListaEstatusEnvioOnboarding()");

                this.StoreEstatusWH.DataBind();
            }
            catch (CAppException caEx)
            {
                logPCI.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiaPantalla();
        }

        /// <summary>
        /// Restablece todos los controles a su estatus de carga inicial de la página
        /// </summary>
        protected void LimpiaPantalla()
        {
            this.cBoxCC.Reset();
            this.dfFecha.Reset();
            this.cBoxEstatus.Clear();
            this.txtIdMensaje.Reset();

            LimpiaGridWHOnboard();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de estatus de nuevos webhook
        /// </summary>
        protected void LimpiaGridWHOnboard()
        {
            this.btnExportExcel.Disabled = true;
            this.btnReenviar.Disabled = true;

            HttpContext.Current.Session.Add("DtWHOnboard", null);
            StoreWHOnboard.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridWHOnboard();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de webhook onboarding,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridWHOnboard(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            this.btnReenviar.Disabled = true;
            LogPCI unLog = new LogPCI(LH_ParabWbhkOnB);

            try
            {
                DataTable dtWHOnboard = new DataTable();

                dtWHOnboard = HttpContext.Current.Session["DtWHOnboard"] as DataTable;

                if (dtWHOnboard == null)
                {
                    unLog.Info("INICIA ObtieneWebhookOnboarding()");
                    dtWHOnboard = DAOWebhook.ObtieneWebhookOnboarding(
                        this.hdnCveColectiva.Value.ToString(),
                        Convert.ToDateTime(this.dfFecha.SelectedDate),
                        int.Parse(this.cBoxEstatus.SelectedItem.Value),
                        this.txtIdMensaje.Text,
                        LH_ParabWbhkOnB);
                    unLog.Info("TERMINA ObtieneWebhookOnboarding()");

                    HttpContext.Current.Session.Add("DtWHOnboard", dtWHOnboard);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                int TotalRegistros = dtWHOnboard.Rows.Count;

                if (TotalRegistros < 1)
                {
                    X.Msg.Alert("Webhook Onboarding", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (TotalRegistros > maxRegistros)
                {
                    X.Msg.Confirm("Webhook Onboarding", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "¿Deseas reenviar todos los <b>[" + TotalRegistros.ToString() + "]</b> Webhook?", 
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "#{btnReenvioMasivo}.fireEvent('click');",
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
                    (this.StoreWHOnboard.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtWHOnboard.Clone();
                    DataTable dtToGrid = dtWHOnboard.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtWHOnboard.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }
                    
                    int RegistroFinal = (RegistroInicial + PagingWHOnboard.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingWHOnboard.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtWHOnboard.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreWHOnboard.DataSource = dtToGrid;
                    this.StoreWHOnboard.DataBind();

                    this.btnExportExcel.Disabled = false;
                    this.btnReenviar.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Webhook Onboarding", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Webhook Onboarding", "Ocurrió un error al obtener los Webhook Onboarding").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de reenviar, solicitando la modificación
        /// de estatus de los mensajes seleccionados a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnReenviar_Click(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabWbhkOnB);
            pCI.Info("btnReenviar_Click()");

            try
            {
                DataTable _elDt = HttpContext.Current.Session["DtWHOnboard"] as DataTable;

                RowSelectionModel losWebhook = this.GridWHOnboard.SelectionModel.Primary as RowSelectionModel;
                int contador = losWebhook.SelectedRows.Count;                

                if (Convert.ToInt16(this.hdnFlag.Value) == 1)
                {
                    int totalRegistros = _elDt.Rows.Count;

                    X.Msg.Confirm("Webhook Onboarding", "¿Deseas reenviar todos los <b>[" + totalRegistros.ToString() + "]</b> Webhook, " +
                        "o únicamente los <b>[" + contador.ToString() + "]</b> Webhook seleccionados?",
                            new MessageBoxButtonsConfig
                            {
                                Yes = new MessageBoxButtonConfig
                                {
                                    Handler = "#{btnReenvioMasivo}.fireEvent('click');",
                                    Text = "Todos"
                                },
                                No = new MessageBoxButtonConfig
                                {
                                    Handler = "#{hdnFlag}.setValue(0); #{btnReenviar}.fireEvent('click');",
                                    Text = "Sólo seleccionados"
                                }
                            }).Show();
                }
                else
                {
                    ReenviaMensajes();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reenviar", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Reenviar", "Ocurrió un error al reenviar los Webhook Onboarding");
            }
        }
        
        /// <summary>
        /// Controla el evento onRefresh del grid de webhook onboarding
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreWHOnboard_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridWHOnboard(inicio, columna, orden);
        }

        /// <summary>
        /// Controla el evento Click del botón oculto para reenvío masivo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnReenvioMasivo_Click(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabWbhkOnB);
            pCI.Info("btnReenvioMasivo_Click()");

            try
            {
                ReenviaMensajes();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Reenviar", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Reenviar", "Ocurrió un error al reenviar los Webhook Onboarding");
            }
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnExportExcel_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "Reporte";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtWHOnboard"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabWbhkOnB);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Webhook Onboarding", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
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
        [DirectMethod(Namespace = "RepWHOnboard")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ReenviaMensajes()
        {
            LogPCI pCI = new LogPCI(LH_ParabWbhkOnB);
            pCI.Info("ReenviaMensajes()");

            try
            {
                long IdColectiva = Convert.ToInt64(cBoxCC.SelectedItem.Value);
                HttpWebhookOnB datosHTTP = new HttpWebhookOnB();

                pCI.Info("INICIA ObtieneParametrosContratoWebhookOnboarding()");
                List<Parametro> Parametros =
                    DAOWebhook.ObtieneParametrosContratoWebhookOnboarding(IdColectiva, LH_ParabWbhkOnB);
                pCI.Info("TERMINA ObtieneParametrosContratoWebhookOnboarding()");

                foreach (Parametro parametro in Parametros)
                {
                    switch (parametro.Nombre)
                    {
                        case "@URL_Onboarding": datosHTTP.URL = parametro.Valor; break;
                        case "@WH_X_Api_Key": datosHTTP.XApiKey = parametro.Valor; break;
                        case "@WH_X_Track_Id": datosHTTP.XTrackId = parametro.Valor; break;
                        case "@WH_AutBasic_User": datosHTTP.AutBasic_User = parametro.Valor; break;
                        case "@WH_AutBasic_Pwd": datosHTTP.AutBasic_Pwd = parametro.Valor; break;
                    }
                }

                if (string.IsNullOrEmpty(datosHTTP.URL))
                {
                    throw new CAppException(8012, "El SubEmisor no tiene configurada la URL de dirección Onboarding");
                }

                RowSelectionModel losWebhook = this.GridWHOnboard.SelectionModel.Primary as RowSelectionModel;
                DataTable _elDt = HttpContext.Current.Session["DtWHOnboard"] as DataTable;

                foreach (SelectedRow webhook in losWebhook.SelectedRows)
                {
                    foreach (DataRow row in _elDt.Rows)
                    {
                        if (row["IdMensaje"].ToString() == webhook.RecordID)
                        {
                            datosHTTP.JsonBody = row["DescripcionMensaje"].ToString();
                            break;
                        }
                    }

                    pCI.Info("INICIA ReenviaWebhookOnboarding()");
                    LNWebhook.ReenviaWebhookOnboarding(datosHTTP, LH_ParabWbhkOnB);
                    pCI.Info("TERMINA ReenviaWebhookOnboarding()");

                    pCI.Info("INICIA ModificaEstatusMsjWebhook()");
                    LNWebhook.ModificaEstatusMsjWebhookOnB(int.Parse(webhook.RecordID),
                        this.Usuario, LH_ParabWbhkOnB);
                    pCI.Info("TERMINA ModificaEstatusMsjWebhook()");


                    X.Msg.Notify("", "Webhook Onboarding reenviados" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    LimpiaPantalla();
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }
    }
}
