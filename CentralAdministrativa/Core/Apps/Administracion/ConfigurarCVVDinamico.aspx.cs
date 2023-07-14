using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
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
using System.Threading;
using System.Web;

namespace TpvWeb
{
    public partial class ConfigurarCVVDinamico : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Reporte Procesos Batch
        private LogHeader LH_ConfigurarCVVDinamico = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ConfigurarCVVDinamico.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ConfigurarCVVDinamico.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ConfigurarCVVDinamico.User = this.Usuario.ClaveUsuario;
            LH_ConfigurarCVVDinamico.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ConfigurarCVVDinamico);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConfigurarCVVDinamico Page_Load()");

                if (!IsPostBack)
                {
                    EstableceSubEmisores();


                    this.PagStoreConfCVV.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("dtConfigurarCVVDinamico", null);
                }

                log.Info("TERMINA ConfigurarCVVDinamico Page_Load()");
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
        /// Controla el evento Seleccionar del combo SubEmisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceProductos(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ConfigurarCVVDinamico);

            try
            {
                LimpiaBusquedaPrevia();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductos.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxCliente.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ConfigurarCVVDinamico);
                log.Info("TERMINA ObtieneProductosDeColectiva()");
                this.StoreProductos.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Productos", "Error al obtener los Productos del SubEmisor").Show();
            }
        }


        /// <summary>
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI logPCI = new LogPCI(LH_ConfigurarCVVDinamico);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ConfigurarCVVDinamico);
                logPCI.Info("TERMINA ListaColectivasSubEmisor()");

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

                this.StoreSubEmisores.DataSource = ComboList;
                this.StoreSubEmisores.DataBind();
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
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de parámetros
        /// </summary>
        protected void LimpiaBusquedaPrevia()
        {
            //this.cBoxProducto.Reset();
            //this.StoreProductos.RemoveAll();

            HttpContext.Current.Session.Add("dtConfigurarCVVDinamico", null);
            this.StoreConfigurarCVVDinamico.RemoveAll();
        }



        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            

            LimpiaBusquedaPrevia();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
            
        }





        /// <summary>
        /// Controla el evento Click al botón Buscar del panel de Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaGridPanelCVVDinamico(int RegistroInicial, string Columna, SortDirection Orden)
        {
            

            LogPCI unLog = new LogPCI(LH_ConfigurarCVVDinamico);

            try
            {

                    DataTable dtConfigurarCVVDinamico = new DataTable();
                    int IdProducto = Convert.ToInt32(this.cBoxProducto.SelectedItem.Value);
                    

                    dtConfigurarCVVDinamico = HttpContext.Current.Session["dtConfigurarCVVDinamico"] as DataTable;

                    if (dtConfigurarCVVDinamico == null)
                    {
                        unLog.Info("INICIA ListaCVVDinamicos()");
                        dtConfigurarCVVDinamico = DAOTarjeta.ListaCVVDinamicos(IdProducto,
                                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                                LH_ConfigurarCVVDinamico);
                        unLog.Info("TERMINA ListaCVVDinamicos()");

                        HttpContext.Current.Session.Add("dtConfigurarCVVDinamico", dtConfigurarCVVDinamico);
                    }

                     int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);



                //maxRegistros = maxRegistros; 5000


                if (dtConfigurarCVVDinamico.Rows.Count < 1)
                {
                    X.Msg.Alert("Configuración de CVV Dinamico", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                //else if (dtConfigurarCVVDinamico.Rows.Count > maxRegistros)
                //{
                //    X.Msg.Confirm("Configuración de CVV Dinamico", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                //        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                //        {
                //            Yes = new MessageBoxButtonConfig
                //            {
                //                Handler = "RepConfigurarCVVDinamico.ClicDePaso()",                                
                //                Text = "Aceptar"
                //            },
                //            No = new MessageBoxButtonConfig
                //            {
                //                Text = "Cancelar"
                //            }
                //        }).Show();
                //}
                else
                {
                    int TotalRegistros = dtConfigurarCVVDinamico.Rows.Count;

                    (this.StoreConfigurarCVVDinamico.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtConfigurarCVVDinamico.Clone();
                    DataTable dtToGrid = dtConfigurarCVVDinamico.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtConfigurarCVVDinamico.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagStoreConfCVV.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagStoreConfCVV.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtConfigurarCVVDinamico.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreConfigurarCVVDinamico.DataSource = dtToGrid;
                    StoreConfigurarCVVDinamico.DataBind();

                } 
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Configuración de CVV Dinamico", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Configuración de CVV Dinamico", "Ocurrió un error al consultar el Reporte de Configuración de CVV Dinamico").Show();
            }

            finally
            {
                X.Mask.Hide();
            }

        }





        /// <summary>
        /// Controla el evento onRefresh del grid de procesos batch
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreConfigurarCVVDinamico_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridPanelCVVDinamico(inicio, columna, orden);
        }








        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ConfigurarCVVDinamico);


            try
            {

                String comando = (String)e.ExtraParams["Comando"];
                int IdProducto = Convert.ToInt32(this.cBoxProducto.SelectedItem.Value);
                

                switch (comando)
                {


                    case "Lock":
                    case "Unlock":
                        int estatus = comando == "Lock" ? 0 : 1;
                        string msj = comando == "Lock" ? "Desactivado" : "Activado";

                        log.Info("INICIA ModificaEstatusCVVDinamico()");
                        LNTarjetas.ModificaEstatusEventoManual(IdProducto, estatus
                             , this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), LH_ConfigurarCVVDinamico);
                        log.Info("TERMINA ModificaEstatusCVVDinamico()");




                        X.Msg.Notify("", "Evento Manual " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();






                        LimpiaBusquedaPrevia();
                        Thread.Sleep(100);
                        btnBuscarHide.FireEvent("click");

                        break;

                    default:
                        break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Evento Manual", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Evento Manual", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            cBoxCliente.Reset();
            cBoxProducto.Reset();



            LimpiaBusquedaPrevia();
        }
    }
}