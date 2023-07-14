using DALMonitoreo.BaseDatos;
using DALMonitoreo.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;

namespace Monitoreo
{
    public partial class AlertamientoAplicativos : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA FONDEO
        private LogHeader LH_ParabAlertaApps = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAlertaApps.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAlertaApps.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAlertaApps.User = this.Usuario.ClaveUsuario;
            LH_ParabAlertaApps.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAlertaApps);
            string errRedirect = string.Empty;
           
            try
            {
                log.Info("INICIA AlertamientoAplicativos Page_Load()");

                HttpContext.Current.Session.Clear();

                if (!IsPostBack)
                {
                    this.cmbEstatus.SetValue(1);

                    EstableceAplicativos();

                    HttpContext.Current.Session.Add("DtAlertamientos", null);
                }

                log.Info("TERMINA AlertamientoAplicativos Page_Load()");
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
        /// Establece los valores del combo aplicativos con los datos de la base 
        /// </summary>
        protected void EstableceAplicativos()
        {
            LogPCI log = new LogPCI(LH_ParabAlertaApps);

            log.Info("INICIA ListaAplicativos()");
            this.StoreAplicacion.DataSource = DAOAlertamientoAplicativos.ListaAplicativos(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAlertaApps);
            log.Info("TERMINA ListaAplicativos()");

            this.StoreAplicacion.DataBind();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de alertamientos, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridAlertamientos(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabAlertaApps);

            try
            {
                DataTable dtAlertas = new DataTable();

                dtAlertas = HttpContext.Current.Session["DtAlertamientos"] as DataTable;

                if (dtAlertas == null)
                {
                    unLog.Info("INICIA ObtieneAlertamientosAplicativo()");
                    dtAlertas = DAOAlertamientoAplicativos.ObtieneAlertamientosAplicativo(
                        Convert.ToInt32(this.cmbEstatus.SelectedItem.Value),
                        this.dfFecha.SelectedDate,
                        Convert.ToInt32(this.cmbAplicacion.SelectedItem.Value), this.txtInstancia.Text,
                        LH_ParabAlertaApps);
                    unLog.Info("TERMINA ObtieneAlertamientosAplicativo()");

                    HttpContext.Current.Session.Add("DtAlertamientos", dtAlertas);
                }

                if (dtAlertas.Rows.Count < 1)
                {
                    X.Msg.Alert("Alertamientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    int TotalRegistros = dtAlertas.Rows.Count;

                    (this.StoreAlertas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtAlertas.Clone();
                    DataTable dtToGrid = dtAlertas.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtAlertas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingAlertamientos.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingAlertamientos.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtAlertas.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreAlertas.DataSource = dtToGrid;
                    this.StoreAlertas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Alertamiento de Aplicativos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Alertamiento de Aplicativos", "Ocurrió un error al obtener los alertamientos").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de Traspasos 
        /// </summary>
        protected void LimpiaGridAlertamientos()
        {
            HttpContext.Current.Session.Add("DtAlertamientos", null);
            this.StoreAlertas.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.dfFecha.Reset();
            this.cmbAplicacion.Reset();
            this.txtInstancia.Reset();

            LimpiaGridAlertamientos();
        }

        /// <summary>
        /// Controla el evento Click al botón de Buscar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridAlertamientos();

            Thread.Sleep(100);

            this.btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de traspasos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreAlertas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridAlertamientos(inicio, columna, orden);
        }

        /// <summary>
        /// Controla el evento Click al botón Cerrar Alertamiento, solicitando la inactivación en tablas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnComentarios_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAlertaApps);

            try
            {
                pCI.Info("INICIA CierraAlertamiento()");
                LNAlertamientoAplicativos.CierraAlertamiento(Convert.ToInt32(this.hdnIdAlerta.Value),
                    this.txtComentarios.Text, this.Usuario, LH_ParabAlertaApps);
                pCI.Info("TERMINA CierraAlertamiento()");

                this.WdwComentariosCierre.Hide();

                X.Msg.Notify("Cierre de Alertamiento", "Cierre realizado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                
                this.btnBuscar.FireEvent("click");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cierre de Alertamiento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Cierre de Alertamiento", "Ocurrio un error al cerrar el alertamiento").Show();
            }
        }
    }
}