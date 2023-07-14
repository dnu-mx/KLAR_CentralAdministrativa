using DALBovedaTarjetas.Entidades;
using DALBovedaTarjetas.LogicaNegocio;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;


namespace BovedaTarjetas
{
    public partial class ConsTarjetasStock : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Bóveda Cpnsultar Tarjetas Stock
        private LogHeader LH_BovedaConsTarjStock = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Consultar Solicitudes Stock
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_BovedaConsTarjStock.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_BovedaConsTarjStock.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_BovedaConsTarjStock.User = this.Usuario.ClaveUsuario;
            LH_BovedaConsTarjStock.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_BovedaConsTarjStock);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConsTarjetasStock Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaInicio.MaxDate = DateTime.Today;
                    this.dfFechaInicio.MinDate = DateTime.Today.AddMonths(-3);

                    this.dfFechaFin.MaxDate = DateTime.Today;

                    this.PagingSolicitudesStock.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                    HttpContext.Current.Session.Add("ListSolicitudesStock", null);
                }

                log.Info("TERMINA ConsTarjetasStock Page_Load()");
            }

            catch(CAppException)
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
        /// Controla el evento Click al botón de Buscar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarSolics_Click(object sender, EventArgs e)
        {
            LimpiaGridSolicitudesStock();
            LlenaGridSolicitudesStock();
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de solicitudes, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        protected void LlenaGridSolicitudesStock()
        {
            LogPCI logPCI = new LogPCI(LH_BovedaConsTarjStock);

            try
            {
                List<PeticionAltaTarjetas> laLista = new List<PeticionAltaTarjetas>();

                laLista = HttpContext.Current.Session["ListSolicitudesStock"] as List<PeticionAltaTarjetas>;

                if (laLista == null)
                {
                    logPCI.Info("INICIA ReporteProcesosBatch()");
                    laLista = LNBoveda.ConsultaPeticionesStock(
                        Convert.ToDateTime(dfFechaInicio.SelectedDate),
                        Convert.ToDateTime(dfFechaFin.SelectedDate), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_BovedaConsTarjStock);
                    logPCI.Info("TERMINA ReporteProcesosBatch()");

                    HttpContext.Current.Session.Add("ListSolicitudesStock", laLista);
                }
                
                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (laLista.Count < 1)
                {
                    X.Msg.Alert("Solicitudes Stock", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (laLista.Count == maxRegistros)
                {
                    X.Msg.Alert("Solicitudes Stock", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</ br > " +
                        "Se muestran sólo los " + maxRegistros.ToString() + "archivos más recientes. Por favor, afina tu búsqueda").Show();
                }
                else
                {
                    this.StoreSolicitudesStock.DataSource = laLista;
                    this.StoreSolicitudesStock.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Solicitudes Stock", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Solicitudes Stock", "Error al obtener las Solicitudes Stock").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de procesos batch
        /// </summary>
        protected void LimpiaGridSolicitudesStock()
        {
            HttpContext.Current.Session.Add("ListSolicitudesStock", null);
            this.StoreSolicitudesStock.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            this.dfFechaInicio.Reset();
            this.dfFechaFin.Reset();

            LimpiaGridSolicitudesStock();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_BovedaConsTarjStock);

            try
            {
                X.Mask.Show(new MaskConfig { Msg = "Descargando Archivo..." });

                Guid _appID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
                string nombreArchivo = e.ExtraParams["NombreArchivo"].ToString();
                string rutaArchivo = e.ExtraParams["RutaArchivo"].ToString();
                string rutaTemporalArchivo = Configuracion.Get(_appID, "DirectorioSeguridad").Valor; ;

                log.Info("INICIA CifraArchivoStock()");
                LNBoveda.CifraArchivoSolicitud(rutaArchivo, nombreArchivo, rutaTemporalArchivo,
                    _appID, LH_BovedaConsTarjStock);
                log.Info("TERMINA CifraArchivoStock()");

                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + nombreArchivo);
                Response.TransmitFile(rutaTemporalArchivo + nombreArchivo);
                
                Response.Flush();
                Response.Close();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Descargar Archivo", err.Mensaje()).Show();
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                X.Msg.Alert("Descargar Archivo", "Ocurrio un error al ejecutar la acción seleccionada").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }
    }
}