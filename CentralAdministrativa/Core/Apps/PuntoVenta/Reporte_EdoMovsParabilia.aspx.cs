using ClosedXML.Excel;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DNU_ParabiliaOperacionesProcesoCortes;
using DNU_ParabiliaOperacionesProcesoCortes.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Utilerias;

namespace TpvWeb
{
    public partial class Reporte_EdoMovsParabilia : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte de Estado de Movimientos
        private LogHeader LH_ParabRepEdoMovs = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Reporte de Estado de Movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabRepEdoMovs.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabRepEdoMovs.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabRepEdoMovs.User = this.Usuario.ClaveUsuario;
            LH_ParabRepEdoMovs.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabRepEdoMovs);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA Reporte_EdoMovsParabilia Page_Load()");

                if (!IsPostBack)
                {
                    LlenaClientes();

                    PagingEdoMovs.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtEdoMovs", null);
                }

                log.Info("TERMINA Reporte_EdoMovsParabilia Page_Load()");
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
        /// Establece el control de SubEmisores con la información de base de datos
        /// </summary>
        protected void LlenaClientes()
        {
            LogPCI pCI = new LogPCI(LH_ParabRepEdoMovs);

            try
            {
                pCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtColectivas = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabRepEdoMovs);
                pCI.Info("TERMINA ListaColectivasSubEmisor()");

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

                StoreClientes.DataSource = grupoList;
                StoreClientes.DataBind();
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
        /// Controla la alimentación de datos del grid de procesos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridMovimientos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_ParabRepEdoMovs);

            try
            {
                DataTable dtEdoMovs = new DataTable();

                dtEdoMovs = HttpContext.Current.Session["DtEdoMovs"] as DataTable;

                var dateToConvert = this.hdnFechaCorte.Value.ToString();
                var format = "ddd MMM dd yyyy HH:mm:ss 'GMT'";
                DateTime laFecha = DateTime.ParseExact(dateToConvert.Substring(0, 28), format, CultureInfo.InvariantCulture);

                if (laFecha > DateTime.Today)
                {
                    X.Msg.Alert("Estado de Movimientos", "La Fecha de Corte debe ser menor o igual a la fecha actual.").Show();
                    return;
                }

                if (dtEdoMovs == null)
                {
                    log.Info("INICIA ReporteEstadoMovimientos()");
                    dtEdoMovs = DAOReportes.ObtieneReporteEdosMovs(
                        Convert.ToInt64(this.cBoxSubEmisor.SelectedItem.Value),
                        laFecha.Month + "/" + laFecha.Year,
                        Convert.ToUInt16(this.cBoxEstatus.SelectedItem.Value),
                        this.txtTarjeta.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabRepEdoMovs);
                    log.Info("TERMINA ReporteEstadoMovimientos()");

                    if (dtEdoMovs.Rows.Count < 1)
                    {
                        X.Msg.Alert("Estado de Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                        return;
                    }

                    dtEdoMovs = Tarjetas.EnmascaraTablaConTarjetas(dtEdoMovs, "NumTarjeta", "Enmascara", LH_ParabRepEdoMovs);

                    HttpContext.Current.Session.Add("DtEdoMovs", dtEdoMovs);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                       ConfigurationManager.AppSettings["IDApplication"].ToString()),
                       "Reporte_MaxRegsPagina").Valor);

                if (dtEdoMovs.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Estado de Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "EdoMovs.ClicDePaso()",
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
                    int TotalRegistros = dtEdoMovs.Rows.Count;

                    (this.StoreMovimientos.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtEdoMovs.Clone();
                    DataTable dtToGrid = dtEdoMovs.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtEdoMovs.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingEdoMovs.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingEdoMovs.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtEdoMovs.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreMovimientos.DataSource = dtToGrid;
                    StoreMovimientos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Estado de Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Estado de Movimientos", "Ocurrió un error al obtener el Estado de Movimientos").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de estado de movimientos
        /// </summary>
        protected void LimpiaGridEdoMovs()
        {
            HttpContext.Current.Session.Add("DtEdoMovs", null);
            StoreMovimientos.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.cBoxSubEmisor.Reset();
            this.dfCorte.Reset();
            this.cBoxEstatus.Reset();
            this.txtTarjeta.Reset();

            LimpiaGridEdoMovs();
        }
        
        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridEdoMovs();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Exporta el reporte previamente consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "EdoMovs")]
        public void ExportDataTableToExcel()
        {
            try
            {
                string reportName = "Reporte";
                DataTable _dtEdoMovs = HttpContext.Current.Session["DtEdoMovs"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtEdoMovs, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabRepEdoMovs);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Estado de Movimientos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
            return ws;
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de estado de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreMovimientos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridMovimientos(inicio, columna, orden);
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "EdoMovs")]
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
        [DirectMethod(Namespace = "EdoMovs")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutaComando(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabRepEdoMovs);

            try
            {
                char[] charsToTrim = { '*', '"', ' ', '/' };
                String comando = e.ExtraParams["Comando"];
                Int64 idCorte = Convert.ToInt64(e.ExtraParams["ID_Corte"]);
                String rutaPDF = e.ExtraParams["Ruta"].ToString().Trim(charsToTrim);

                switch (comando)
                {
                    case "PDF":
                        DescargarArchivoPDF(rutaPDF);
                        break;

                    case "Enviar":
                    default:
                        ProcesoCortes proc = new ProcesoCortes();

                        unLog.Info("INICIA ProcesoCortes.EnviarEstadoDeCuenta()");
                        RespuestaSolicitud resp = proc.EnviarEstadoDeCuenta(idCorte);
                        unLog.Info("TERMINA ProcesoCortes.EnviarEstadoDeCuenta()");

                        if (resp.codigoRespuesta == "0000")
                        {
                            X.Msg.Notify("", "Correo Enviado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }
                        else
                        {
                            X.Msg.Alert("Enviar Correo", "Correo NO Enviado. " + resp.descripcionRespuesta).Show();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Movimientos", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Establece la ruta y archivo del estado de cuenta y lo descarga al cliente
        /// </summary>
        /// <param name="rutaEstado">Ruta en el directorio del servidor que contiene los estados de cuenta</param>
        protected void DescargarArchivoPDF(string rutaEstado)
        {
            try
            {
                String NombreArchivo = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "NombreEdoMovs").Valor;
                
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "Application/PDF";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo);
                Response.TransmitFile(rutaEstado + NombreArchivo);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(LH_ParabRepEdoMovs);
                log.Error("DescargarArchivoPDF()");
                log.ErrorException(ex);
                throw ex;
            }
        }
    }
}