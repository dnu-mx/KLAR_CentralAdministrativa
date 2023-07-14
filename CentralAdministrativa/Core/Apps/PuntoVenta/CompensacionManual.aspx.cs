using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALCortador.Entidades;
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
using System.Threading;
using System.Web;

namespace TpvWeb
{
    public partial class CompensacionManual : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Compensación Manual
        private LogHeader LH_ParabCompManual = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabCompManual.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabCompManual.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabCompManual.User = this.Usuario.ClaveUsuario;
            LH_ParabCompManual.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabCompManual);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CompensacionManual Page_Load()");

                if (!IsPostBack)
                {
                    LlenaComboClientes();

                    PagingOpsComp.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina").Valor);

                    HttpContext.Current.Session.Add("DtOpsComp", null);
                    HttpContext.Current.Session.Add("DtOpsPorComp", null);
                }

                log.Info("TERMINA CompensacionManual Page_Load()");
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
        /// Obtiene los Subemisores permitidos para el usuario en sesión y los establece en el combo correspondiente
        /// </summary>
        protected void LlenaComboClientes()
        {
            LogPCI logPCI = new LogPCI(LH_ParabCompManual);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtColectivas =
                        DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabCompManual);
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
        /// Controla el evento Click al botón Limpiar del formulario, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.cBoxCC.Reset();
            this.dfFechaInicial.Reset();
            this.dfFechaFinal.Reset();
            this.txtTarjeta.Reset();

            LimpiaGridOpsComp();

            LimpiaGridOpsPorComp();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones compensadas
        /// </summary>
        protected void LimpiaGridOpsComp()
        {
            HttpContext.Current.Session.Add("DtOpsComp", null);
            this.StoreOpsComp.RemoveAll();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al grid de operaciones por compensar
        /// </summary>
        protected void LimpiaGridOpsPorComp()
        {
            HttpContext.Current.Session.Add("DtOpsPorComp", null);
            this.StoreOpsPorComp.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario, consultando el reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LimpiaGridOpsComp();
            LimpiaGridOpsPorComp();

            Thread.Sleep(100);

            btnBuscarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones compensadas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsComp_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsComp(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones compensadas,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsComp(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_ParabCompManual);

            try
            {
                DataTable dtOpsComp = new DataTable();
                Int64 idColectiva = Convert.ToInt64(cBoxCC.SelectedItem.Value);

                dtOpsComp = HttpContext.Current.Session["DtOpsComp"] as DataTable;

                if (dtOpsComp == null)
                {
                    log.Info("INICIA ObtieneBinesCliente()");
                    string losBines = DAOCompensaciones.ObtieneBinesCliente(
                        Convert.ToInt64(this.cBoxCC.SelectedItem.Value), LH_ParabCompManual);
                    log.Info("TERMINA ObtieneBinesCliente()");

                    log.Info("INICIA ObtieneTXsFicherosT112()");
                    dtOpsComp = DAOCompensaciones.ObtieneTXsFicherosT112(losBines,
                        Convert.ToDateTime(this.dfFechaInicial.Value),
                        Convert.ToDateTime(this.dfFechaFinal.Value),
                        this.txtTarjeta.Text, LH_ParabCompManual);
                    log.Info("TERMINA ObtieneTXsFicherosT112()");

                    dtOpsComp = AgregaContadorOps(dtOpsComp);

                    HttpContext.Current.Session.Add("DtOpsComp", dtOpsComp);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtOpsComp.Rows.Count < 1)
                {
                    X.Msg.Alert("Operaciones", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtOpsComp.Rows.Count > maxRegistros)
                {
                    X.Msg.Alert("Operaciones", "Demasiadas coincidencias. Por favor, afina tu búsqueda").Show();
                }
                else
                {
                    int TotalRegistros = dtOpsComp.Rows.Count;

                    (this.StoreOpsComp.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsComp.Clone();
                    DataTable dtToGrid = dtOpsComp.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsComp.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingOpsComp.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingOpsComp.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsComp.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreOpsComp.DataSource = dtToGrid;
                    this.StoreOpsComp.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Operaciones", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un error al obtener las operaciones").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elOriginal"></param>
        /// <returns></returns>
        protected DataTable AgregaContadorOps(DataTable elOriginal)
        {
            LogPCI log = new LogPCI(LH_ParabCompManual);

            try
            {
                DataTable elFinal = elOriginal.Clone();

                foreach (DataRow row in elOriginal.Rows)
                {
                    log.Info("INICIA ObtieneNumOperacionesT112PorCompensar()");
                    row["NumOperacionesCoinciden"] = DAOCompensaciones.ObtieneNumOperacionesT112PorCompensar(
                        row["Tarjeta"].ToString(), row["NumAutorizacion"].ToString(), LH_ParabCompManual);
                    log.Info("TERMINA ObtieneNumOperacionesT112PorCompensar()");

                    elFinal.ImportRow(row);
                }

                elFinal.AcceptChanges();

                return elFinal;
            }
            catch (CAppException caEx)
            {
                log.Warn(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8012, "Ocurrió un error al agregar el contador de operaciones.");
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowOperaciones_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] transaccion = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (transaccion == null || transaccion.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in transaccion[0])
                {
                    switch (column.Key)
                    {
                        case "ID_FicheroDetalle": this.hdnIdFicheroDetalle.Value = column.Value; break;
                        case "Tarjeta": this.hdnTarjeta.Value = column.Value; break;
                        case "CodigoTX": this.hdnCodigoOperacion.Value = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaGridOpsPorComp();

                this.btnOpsPorCompHide.FireEvent("click");
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(LH_ParabCompManual);
                log.ErrorException(ex);
                X.Msg.Alert("Operaciones", "Ocurrió un error con los datos de operaciones para compensación").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones por compensar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsPorComp_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsPorComp(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de operaciones compensadas,
        /// así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsPorComp(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI log = new LogPCI(LH_ParabCompManual);

            try
            {
                DataTable dtOpsPorComp = new DataTable();
                Int64 idColectiva = Convert.ToInt64(cBoxCC.SelectedItem.Value);

                dtOpsPorComp = HttpContext.Current.Session["DtOpsPorComp"] as DataTable;

                if (dtOpsPorComp == null)
                {
                    log.Info("INICIA ObtieneOperacionesT112PorCompensar()");
                    dtOpsPorComp = DAOCompensaciones.ObtieneOperacionesT112PorCompensar(
                        this.hdnTarjeta.Value.ToString(),
                        Convert.ToDateTime(this.dfFechaInicial.Value),
                        Convert.ToDateTime(this.dfFechaFinal.Value),
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabCompManual);
                    log.Info("TERMINA ObtieneOperacionesT112PorCompensar()");

                    HttpContext.Current.Session.Add("DtOpsPorComp", dtOpsPorComp);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina").Valor);

                if (dtOpsPorComp.Rows.Count < 1)
                {
                    X.Msg.Alert("Operaciones para Compensación", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtOpsPorComp.Rows.Count > maxRegistros)
                {
                    X.Msg.Alert("Operaciones para Compensación", "Demasiadas coincidencias. Por favor, afina tu búsqueda").Show();
                }
                else
                {
                    int TotalRegistros = dtOpsPorComp.Rows.Count;

                    (this.StoreOpsPorComp.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsPorComp.Clone();
                    DataTable dtToGrid = dtOpsPorComp.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsPorComp.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTB_OpsXComp.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTB_OpsXComp.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsPorComp.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreOpsPorComp.DataSource = dtToGrid;
                    this.StoreOpsPorComp.DataBind();

                    this.GridOpsPorComp.Collapsed = false;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Operaciones para Compensación", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Operaciones para Compensación", "Ocurrió un error al obtener las operaciones").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Compensar Operación, restableciendo los controles
        /// de filtros a la carga inicial de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnCompensar_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabCompManual);

            try
            {
                EventoManual elEvento = new EventoManual();
                Int64 idFicheroDetalle = Convert.ToInt64(this.hdnIdFicheroDetalle.Value);

                RowSelectionModel oper = this.GridOpsPorComp.SelectionModel.Primary as RowSelectionModel;

                Int64 idOperacion = Convert.ToInt64(oper.SelectedRow.RecordID);

                //Se establecen los parámetros del evento manual
                elEvento.ClaveEvento = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveEventoCompManual").Valor;
                elEvento.Observaciones = "Central Administrativa - " + this.Usuario.ClaveUsuario;
                elEvento.IdColectivaOrigen = Convert.ToInt64(this.hdnEmisor.Value);
                elEvento.Importe = this.hdnMonto.Value.ToString();
                elEvento.IdColectivaDestino = Convert.ToInt64(this.cBoxCC.SelectedItem.Value);

                //Se obtiene el resto de los campos del fichero detalle
                logPCI.Info("INICIA ObtieneDatosTX_T112()");
                Operacion laOperacion = DAOCompensaciones.ObtieneDatosTX_T112(idFicheroDetalle, LH_ParabCompManual);
                logPCI.Info("TERMINA ObtieneDatosTX_T112()");
                laOperacion.ID_Operacion = idOperacion;

                //Obtiene el nuevo estatus de compensación de la operación y lo registra en el fichero detalle
                logPCI.Info("INICIA ObtieneNuevoIdEstatusCompensacion()");
                int idNuevoEstatus = DAOCompensaciones.ObtieneNuevoIdEstatusCompensacion(
                    idOperacion, this.hdnCodigoOperacion.Value.ToString(), LH_ParabCompManual);
                logPCI.Info("TERMINA ObtieneNuevoIdEstatusCompensacion()");

                //Se realiza la compensación
                logPCI.Info("INICIA CompensaOperacionManual_T112()");
                LNCompensaciones.CompensaOperacionManual_T112(elEvento, laOperacion, 
                    idFicheroDetalle, idNuevoEstatus, this.Usuario, LH_ParabCompManual);
                logPCI.Info("TERMINA CompensaOperacionManual_T112()");

                X.Msg.Notify("Compensación", "Compensación manual <br />  <br /> <b> E X I T O S A </b> <br />  <br /> ").Show();

                btnBuscar.FireEvent("click");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Compensación", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Compensación", "Ocurrió un error al relizar la compensación").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "CompMan")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
