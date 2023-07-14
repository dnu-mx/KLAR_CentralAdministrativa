using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
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
    public partial class MonitoreoCompensacion : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Compensación Manual
        private LogHeader LH_MonitoreoComp = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_MonitoreoComp.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_MonitoreoComp.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_MonitoreoComp.User = this.Usuario.ClaveUsuario;
            LH_MonitoreoComp.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_MonitoreoComp);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA MonitoreoCompensacion Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaCompensacion.SetValue(DateTime.Now);
                    this.dfFechaCompensacion.MinDate = DateTime.Today.AddMonths(-6);

                    this.PagingCargaArchivos.PageSize = 10;
                    HttpContext.Current.Session.Add("DtCargaArch", null);

                    this.PagingHomologacion.PageSize = 10;
                    HttpContext.Current.Session.Add("DtHomolRegs", null);

                    this.PagingCompRegistros.PageSize = 10;
                    HttpContext.Current.Session.Add("DtCompRegs", null);

                    this.PagingDetalle.PageSize = 10;
                    HttpContext.Current.Session.Add("DtDetalleRegs", null);
                }

                log.Info("TERMINA MonitoreoCompensacion Page_Load()");
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
        /// Controla el evento Click al botón Limpiar del formulario, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.chkBoxArchPendientes.Reset();
            
            this.dfFechaCompensacion.Reset();
            this.dfFechaCompensacion.Disabled = false;

            this.cBoxMarca.Reset();
            this.cBoxMarca.Disabled = false;

            LimpiaGrids();
            LimpiaVentanaDetalles();
        }

        /// <summary>
        /// Establece los controles de los grids a su estado de carga inicial
        /// </summary>
        protected void LimpiaGrids()
        {
            this.StoreCargaArchivos.RemoveAll();
            HttpContext.Current.Session.Add("DtCargaArch", null);
            this.PanelCargaArchivos.Collapsed = true;
            this.btnExcelCarga.Disabled = true;

            this.StoreHomologacion.RemoveAll();
            HttpContext.Current.Session.Add("DtHomolRegs", null);
            this.PanelHomologacion.Collapsed = true;
            this.btnExcelHomol.Disabled = true;

            this.StoreCompReg.RemoveAll();
            HttpContext.Current.Session.Add("DtCompRegs", null);
            this.PanelCompRegistros.Collapsed = true;
            this.btnExcelComp.Disabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void LimpiaVentanaDetalles()
        {
            this.StoreDetalle.RemoveAll();
            HttpContext.Current.Session.Add("DtDetalleRegs", null);
            this.btnExcelDetalle.Disabled = true;
            this.WdwDetalleRegistros.Hide();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
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
        /// Controla el evento onRefresh del grid de carga de archivos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreCargaArchivos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridCargaArchivos(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de carga de archivos,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridCargaArchivos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_MonitoreoComp);

            try
            {
                DataTable dtCargaArchivos = new DataTable();

                dtCargaArchivos = HttpContext.Current.Session["DtCargaArch"] as DataTable;

                if (dtCargaArchivos == null)
                {
                    log.Info("INICIA ObtieneCargaArchivosT112()");
                    dtCargaArchivos = DAOCompensaciones.ObtieneCargaArchivosT112(
                        this.chkBoxArchPendientes.Checked,
                        Convert.ToDateTime(this.dfFechaCompensacion.Value),
                        this.cBoxMarca.SelectedItem.Value, LH_MonitoreoComp);
                    log.Info("TERMINA ObtieneCargaArchivosT112()");

                    HttpContext.Current.Session.Add("DtCargaArch", dtCargaArchivos);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtCargaArchivos.Rows.Count < 1)
                {
                    X.Msg.Alert("Carga de Archivos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtCargaArchivos.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Carga de Archivos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                       "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                       {
                           Yes = new MessageBoxButtonConfig
                           {
                               Handler = "MonitComp.ClicDePaso_CA()",
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
                    int TotalRegistros = dtCargaArchivos.Rows.Count;

                    (this.StoreCargaArchivos.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT_CA = dtCargaArchivos.Clone();
                    DataTable dtToGrid_CA = dtCargaArchivos.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCargaArchivos.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT_CA = dv.ToTable();
                        sortedDT_CA.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingCargaArchivos.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingCargaArchivos.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid_CA.ImportRow(String.IsNullOrEmpty(Columna) ? dtCargaArchivos.Rows[row] : sortedDT_CA.Rows[row]);
                    }

                    dtToGrid_CA.AcceptChanges();

                    this.StoreCargaArchivos.DataSource = dtToGrid_CA;
                    this.StoreCargaArchivos.DataBind();

                    this.PanelCargaArchivos.Collapsed = false;
                    this.btnExcelCarga.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Carga de Archivos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Carga de Archivos", "Ocurrió un error al obtener la carga de archivos").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de homologación de registros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreHomologacion_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridHomologacion(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de homologación de registros,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridHomologacion(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_MonitoreoComp);

            try
            {
                DataTable dtHomolRegistros = new DataTable();

                dtHomolRegistros = HttpContext.Current.Session["DtHomolRegs"] as DataTable;

                if (dtHomolRegistros == null)
                {
                    log.Info("INICIA ObtieneHomologacionRegistrosT112()");
                    dtHomolRegistros = DAOCompensaciones.ObtieneHomologacionRegistrosT112(
                        this.chkBoxArchPendientes.Checked,
                        Convert.ToDateTime(this.dfFechaCompensacion.Value),
                        this.cBoxMarca.SelectedItem.Value, LH_MonitoreoComp);
                    log.Info("TERMINA ObtieneHomologacionRegistrosT112()");

                    HttpContext.Current.Session.Add("DtHomolRegs", dtHomolRegistros);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtHomolRegistros.Rows.Count < 1)
                {
                    X.Msg.Alert("Homologación de Registros", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtHomolRegistros.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Homologación de Registros", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "MonitComp.ClicDePaso_HR()",
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
                    int TotalRegistros = dtHomolRegistros.Rows.Count;

                    (this.StoreHomologacion.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT_HR = dtHomolRegistros.Clone();
                    DataTable dtToGrid_HR = dtHomolRegistros.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtHomolRegistros.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT_HR = dv.ToTable();
                        sortedDT_HR.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingHomologacion.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingHomologacion.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid_HR.ImportRow(String.IsNullOrEmpty(Columna) ? dtHomolRegistros.Rows[row] : sortedDT_HR.Rows[row]);
                    }

                    dtToGrid_HR.AcceptChanges();

                    this.StoreHomologacion.DataSource = dtToGrid_HR;
                    this.StoreHomologacion.DataBind();

                    this.PanelHomologacion.Collapsed = false;
                    this.btnExcelHomol.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Homologación de Registros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Homologación de Registros", "Ocurrió un error al obtener la homologación de registros").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de Operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellGridHomologacion_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_MonitoreoComp);

            try
            {

                CellSelectionModel sm = GridHomologacion.SelectionModel.Primary as CellSelectionModel;

                if ((sm.SelectedCell.Name != "RegistrosInvalidos") || (sm.SelectedCell.Value == "0"))
                {
                    return;
                }

                this.hdnIdFichero.Value = string.Empty;
                this.hdnIdFichTemp.Value = sm.SelectedCell.RecordID;
                LimpiaVentanaDetalles();

                Thread.Sleep(100);

                this.btnDetallesHide.FireEvent("click");
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de detalle de registros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreDetalle_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridDetalleRegistros(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle de registros,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridDetalleRegistros(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_MonitoreoComp);

            try
            {
                DataTable dtDetalleRegistros = new DataTable();

                dtDetalleRegistros = HttpContext.Current.Session["DtDetalleRegs"] as DataTable;
                long _IdHom = Convert.ToInt64(string.IsNullOrEmpty(this.hdnIdFichTemp.Value.ToString()) ? "0" : 
                    this.hdnIdFichTemp.Value);
                long _IdComp = Convert.ToInt64(string.IsNullOrEmpty(this.hdnIdFichero.Value.ToString()) ? "0" : 
                    this.hdnIdFichero.Value);

                if (dtDetalleRegistros == null)
                {
                    if (_IdHom > 0)
                    {
                        log.Info("INICIA ObtieneDetalleRegistrosInvalidos()");
                        dtDetalleRegistros = DAOCompensaciones.ObtieneDetalleRegistrosInvalidos(
                            _IdHom, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                            LH_MonitoreoComp);
                        log.Info("TERMINA ObtieneDetalleRegistrosInvalidos()");
                    }
                    else
                    {
                        log.Info("INICIA ObtieneDetalleRegistrosErroneos()");
                        dtDetalleRegistros = DAOCompensaciones.ObtieneDetalleRegistrosErroneos(
                            _IdComp, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                            LH_MonitoreoComp);
                        log.Info("TERMINA ObtieneDetalleRegistrosErroneos()");
                    }

                    dtDetalleRegistros = Tarjetas.EnmascaraTablaConTarjetas(dtDetalleRegistros, "Tarjeta", "Enmascara", LH_MonitoreoComp);

                    HttpContext.Current.Session.Add("DtDetalleRegs", dtDetalleRegistros);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtDetalleRegistros.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Detalle de Registros", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "MonitComp.ClicDePaso_DR()",
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
                    int TotalRegistros = dtDetalleRegistros.Rows.Count;

                    (this.StoreDetalle.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT_DR = dtDetalleRegistros.Clone();
                    DataTable dtToGrid_DR = dtDetalleRegistros.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtDetalleRegistros.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT_DR = dv.ToTable();
                        sortedDT_DR.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingDetalle.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingDetalle.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid_DR.ImportRow(String.IsNullOrEmpty(Columna) ? dtDetalleRegistros.Rows[row] : sortedDT_DR.Rows[row]);
                    }

                    dtToGrid_DR.AcceptChanges();

                    this.StoreDetalle.DataSource = dtToGrid_DR;
                    this.StoreDetalle.DataBind();

                    this.btnExcelDetalle.Disabled = false;
                    this.WdwDetalleRegistros.Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Detalle de Registros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Detalle de Registros", "Ocurrió un error al obtener el detalle de los registros").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de compensación de registros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreCompReg_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridCompRegistros(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de compensación de registros,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridCompRegistros(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_MonitoreoComp);

            try
            {
                DataTable dtCompRegistros = new DataTable();

                dtCompRegistros = HttpContext.Current.Session["DtCompRegs"] as DataTable;

                if (dtCompRegistros == null)
                {
                    log.Info("INICIA ObtieneCompensacionRegistrosT112()");
                    dtCompRegistros = DAOCompensaciones.ObtieneCompensacionRegistrosT112(
                        this.chkBoxArchPendientes.Checked,
                        Convert.ToDateTime(this.dfFechaCompensacion.Value),
                        this.cBoxMarca.SelectedItem.Value, LH_MonitoreoComp);
                    log.Info("TERMINA ObtieneCompensacionRegistrosT112()");

                    HttpContext.Current.Session.Add("DtCompRegs", dtCompRegistros);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtCompRegistros.Rows.Count < 1)
                {
                    X.Msg.Alert("Compensación de Registros", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtCompRegistros.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Compensación de Registros", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "MonitComp.ClicDePaso()",
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
                    int TotalRegistros = dtCompRegistros.Rows.Count;

                    (this.StoreCompReg.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT_CR = dtCompRegistros.Clone();
                    DataTable dtToGrid_CR = dtCompRegistros.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtCompRegistros.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT_CR = dv.ToTable();
                        sortedDT_CR.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingCompRegistros.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingCompRegistros.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid_CR.ImportRow(String.IsNullOrEmpty(Columna) ? dtCompRegistros.Rows[row] : sortedDT_CR.Rows[row]);
                    }

                    dtToGrid_CR.AcceptChanges();

                    this.StoreCompReg.DataSource = dtToGrid_CR;
                    this.StoreCompReg.DataBind();

                    this.PanelCompRegistros.Collapsed = false;
                    this.btnExcelComp.Disabled = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Compensación de Registros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Compensación de Registros", "Ocurrió un error al obtener la compensación de registros").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de compensación de registros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellGridCompRegistros_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_MonitoreoComp);

            try
            {
                CellSelectionModel sm = GridCompRegistros.SelectionModel.Primary as CellSelectionModel;

                if ((sm.SelectedCell.Name != "RegistrosError")  || (sm.SelectedCell.Value == "0"))
                {
                    return;
                }

                this.hdnIdFichTemp.Value = string.Empty;
                this.hdnIdFichero.Value = sm.SelectedCell.RecordID;
                LimpiaVentanaDetalles();

                Thread.Sleep(100);

                this.btnDetallesHide.FireEvent("click");
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ClicDePaso_CA()
        {
            this.btnDownload_CA.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download_CA(object sender, DirectEventArgs e)
        {
            ExportDTToExcel_CA();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ExportDTToExcel_CA()
        {
            try
            {
                string reportName = "CargaArchivos";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtCargaArch"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOperaciones, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns_CA(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_MonitoreoComp);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Carga de Archivos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
        protected IXLWorksheet FormatWsColumns_CA(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ClicDePaso_HR()
        {
            this.btnDownload_HR.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download_HR(object sender, DirectEventArgs e)
        {
            ExportDTToExcel_HR();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ExportDTToExcel_HR()
        {
            try
            {
                string reportName = "HomologRegistros";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtHomolRegs"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOperaciones, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns_HR(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_MonitoreoComp);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Homologación de Registros", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
        protected IXLWorksheet FormatWsColumns_HR(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ClicDePaso_DR()
        {
            this.btnDownload_DR.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download_DR(object sender, DirectEventArgs e)
        {
            ExportDTToExcel_DR();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ExportDTToExcel_DR()
        {
            try
            {
                string reportName = "DetalleRegistros";
                DataTable _dtDetalles = HttpContext.Current.Session["DtDetalleRegs"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtDetalles, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns_DR(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_MonitoreoComp);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Detalle de Registros", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
        protected IXLWorksheet FormatWsColumns_DR(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 1).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ClicDePaso_CR()
        {
            this.btnDownload_CR.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void Download_CR(object sender, DirectEventArgs e)
        {
            ExportDTToExcel_CR();
        }

        /// <summary>
        /// Exporta el reporte previamento consultado a un archivo Excel cuando éste
        /// excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void ExportDTToExcel_CR()
        {
            try
            {
                string reportName = "CompRegistros";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtCompRegs"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOperaciones, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns_HR(ws, ws.Column(1).CellsUsed().Count());

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
                LogPCI pCI = new LogPCI(LH_MonitoreoComp);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Compensación de Registros", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
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
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "MonitComp")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}

