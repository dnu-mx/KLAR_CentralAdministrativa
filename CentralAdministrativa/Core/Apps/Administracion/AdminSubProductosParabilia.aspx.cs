using ClosedXML.Excel;
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Utilerias;

namespace Administracion
{
    public partial class AdminSubProductosParabilia : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Administrar SubProductos
        private LogHeader LH_ParabAdminSubProductos = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Subproductos para Parabilia
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminSubProductos.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminSubProductos.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminSubProductos.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminSubProductos.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminSubProductos);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdminSubProductosParabilia Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaColectivasSubEmisor()");
                    DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminSubProductos);
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

                    this.StoreSubEmisores.DataSource = ComboList;
                    this.StoreSubEmisores.DataBind();

                    log.Info("INICIA ListaTiposParametrosMA()");
                    this.StoreTipoParametroMA.DataSource =
                        DAOProducto.ListaTiposParametrosMA(true, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminSubProductos);
                    log.Info("TERMINA ListaTiposParametrosMA()");
                    this.StoreTipoParametroMA.DataBind();

                    PagingTB_Tarjetas.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabAdminSubProductos).Valor);

                    HttpContext.Current.Session.Add("DtTarj_SubP", null);
                }

                log.Info("TERMINA AdminSubProductosParabilia Page_Load()");
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
        /// Controla el evento Seleccionar del combo Colectiva
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceProductos(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                pCI.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductos.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.hdnIdColectiva.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminSubProductos);
                pCI.Info("TERMINA ObtieneProductosDeColectiva()");

                this.StoreProductos.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al establecer los Productos del Cliente").Show();
            }
        }

        /// <summary>
        /// Llena el grid de resultados de subproductos con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI log = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                LimpiaSeleccionPrevia();

                log.Info("INICIA ObtieneSubproductosPorColectivaProducto()");
                DataSet dsSubprod = DAOProducto.ObtieneSubproductosPorColectivaProducto(
                    int.Parse(this.cBoxSubEmisor.SelectedItem.Value),
                    int.Parse(this.cBoxProducto.SelectedItem.Value),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminSubProductos);
                log.Info("TERMINA ObtieneSubproductosPorColectivaProducto()");

                if (dsSubprod.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Subproductos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreSubproductos.DataSource = dsSubprod;
                    StoreSubproductos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Subproductos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Subproductos", "Ocurrió un error al obtener los Subproductos").Show();
            }
        }


        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarSubProducto_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, restableciendo
        /// todos los controles, páneles y grids a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            this.cBoxSubEmisor.Reset();
            this.cBoxProducto.Reset();
            this.StoreSubproductos.RemoveAll();

            LimpiaVentanaParams();

            LimpiaSeleccionPrevia();

            PanelCentralSubProd.Disabled = true;
        }

        /// <summary>
        /// Restablece los controles de la ventana de edición de parámetros
        /// </summary>
        protected void LimpiaVentanaParams()
        {
            this.FormPanelValorParamTxt.Reset();
            this.txtParametro.Reset();

            this.txtValorParFloat.Reset();
            this.txtValorParFloat.Hidden = true;

            this.txtValorParInt.Reset();
            this.txtValorParInt.Hidden = true;

            this.txtValorParString.Reset();
            this.txtValorParString.Hidden = true;

            this.cBoxValorPar.Reset();
            this.cBoxValorPar.Hidden = true;

            this.StoreCatalogoPMA.RemoveAll();
            this.cBoxCatalogoPMA.Reset();
            this.cBoxCatalogoPMA.Hidden = true;
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// un subproducto en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            FormPanelInfoAdSP.Reset();

            this.FormPanelParametrosSP.Reset();
            this.cBoxTipoParametroMA.Reset();
            this.StoreParametros.RemoveAll();
            this.StoreValoresParametros.RemoveAll();
            this.btnAddParametros.Disabled = true;

            this.btnExportExcel.Disabled = true;
            this.txtFiltroTarjeta.Reset();
            this.StoreTarjetas.RemoveAll();

            FormPanelInfoAdSP.Show();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Tarjetas
        /// </summary>
        protected void LimpiaGridTarjetas()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtTarj_SubP", null);
            StoreTarjetas.RemoveAll();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultadosSP_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdSubproducto = 0;
                string ClaveSubproducto = "", NombreSubproducto = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] subproducto = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in subproducto[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Plantilla": IdSubproducto = int.Parse(column.Value); break;
                        case "Clave": ClaveSubproducto = column.Value; break;
                        case "Descripcion": NombreSubproducto = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnIdSubproducto.Value = IdSubproducto;

                PanelCentralSubProd.Title = ClaveSubproducto +  " - " + NombreSubproducto;

                LlenaFormPanelInfoAd(ClaveSubproducto, NombreSubproducto);

                PanelCentralSubProd.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Subproductos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminSubProductos);
                unLog.ErrorException(ex);
                X.Msg.Alert("Subproductos", "Ocurrió un error al obtener la información del Subproducto").Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="ClaveSubproducto">Clave del subproducto</param>
        /// <param name="SubProducto">Nombre o descrición del subproducto</param>
        protected void LlenaFormPanelInfoAd(string ClaveSubproducto, string SubProducto)
        {
            try
            {
                this.txtClaveSubProd.Text = ClaveSubproducto;
                this.txtDescSubProd.Text = SubProducto;
                this.txtProducto.Text = this.cBoxProducto.SelectedItem.Text;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminSubProductos);
                unLog.ErrorException(ex);
                X.Msg.Alert("Información Adicional", "Ocurrió un error al obtener la información adicional del Subproducto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Información Adicional, llamando
        /// a la actualización de datos del subproducto en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAdSP_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                Plantilla subProd = new Plantilla();

                subProd.ID_Plantilla = Convert.ToInt32(this.hdnIdSubproducto.Value); ;
                subProd.Clave = this.txtClaveSubProd.Text;
                subProd.Descripcion = this.txtDescSubProd.Text;

                logPCI.Info("INICIA ModificaSubproducto()");
                string response = LNProducto.ModificaSubproducto(subProd, this.Usuario, LH_ParabAdminSubProductos, "_SubProducto");
                logPCI.Info("TERMINA ModificaSubproducto()");

                if (!response.Contains("OK"))
                {
                    X.Msg.Alert("Actualización de Subproducto", response).Show();
                }
                else
                {
                    X.Msg.Notify("", "Subproducto actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Subproducto", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Subproducto", "Ocurrió un error al modificar el Subproducto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de tipos de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaTipoPMA(object sender, EventArgs e)
        {
            LlenaParametrosSubproducto();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros del subproducto,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosSubproducto()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                int IdSubProducto = Convert.ToInt32(this.hdnIdSubproducto.Value);

                unLog.Info("INICIA ObtienePMAPorTipoSinAsignarSubProd()");
                DataSet dsPMAsSubPA = DAOProducto.ObtienePMAPorTipoSinAsignarSubProd(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdSubProducto, LH_ParabAdminSubProductos);
                unLog.Info("TERMINA ObtienePMAPorTipoSinAsignarSubProd()");

                this.StoreParametros.DataSource = dsPMAsSubPA;
                this.StoreParametros.DataBind();

                unLog.Info("INICIA ObtieneParametrosMASubproducto()");
                DataSet dsPMAsSubA = DAOProducto.ObtieneParametrosMASubproducto(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdSubProducto, LH_ParabAdminSubProductos);
                unLog.Info("TERMINA ObtieneParametrosMASubproducto()");

                this.StoreValoresParametros.DataSource = dsPMAsSubA;
                this.StoreValoresParametros.DataBind();

                if (dsPMAsSubPA.Tables[0].Rows.Count == 0 && dsPMAsSubA.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Parámetros", "No existen coincidencias con los datos solicitados").Show();
                    return;
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Parámetros del Subproducto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametros_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                int IdSubProducto = Convert.ToInt32(this.hdnIdSubproducto.Value);

                pCI.Info("INICIA AgregaParametroMAASubproducto()");
                LNProducto.AgregaParametroMAASubproducto(Convert.ToInt32(cBoxParametros.SelectedItem.Value),
                    IdSubProducto, this.Usuario, LH_ParabAdminSubProductos);
                pCI.Info("TERMINA AgregaParametroMAASubproducto()");

                X.Msg.Notify("", "Parámetro Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.cBoxParametros.Reset();

                LlenaParametrosSubproducto();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminSubProductos);
            pCI.Info("EjecutarComandoParametros()");

            try
            {
                ParametroValor unParametro = new ParametroValor();
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] parametroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (parametroSeleccionado == null || parametroSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in parametroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_ParametroMultiasignacion": unParametro.ID_Parametro = int.Parse(column.Value); break;
                        case "ID_ValorParametroMultiasignacion": unParametro.ID_ValordelParametro = int.Parse(column.Value); break;
                        case "Nombre": unParametro.Nombre = column.Value; break;
                        case "Descripcion": unParametro.Descripcion = column.Value; break;
                        case "Valor": unParametro.Valor = column.Value; break;
                        case "TipoDato": unParametro.TipoDato = column.Value; break;
                        case "TipoValidacion": unParametro.TipoValidacion = column.Value; break;
                        case "ValorInicial": unParametro.ValorInicial = column.Value; break;
                        case "ValorFinal": unParametro.ValorFinal = column.Value; break;
                        case "ExpresionRegular": unParametro.ExpresionRegular = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                this.hdnIdParametroMA.Value = unParametro.ID_Parametro;
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;

                this.hdnValorIniPMA.Value = unParametro.ValorInicial;
                this.hdnValorFinPMA.Value = unParametro.ValorFinal;

                switch (comando)
                {
                    case "Edit":
                        LimpiaVentanaParams();

                        if (!string.IsNullOrEmpty(unParametro.TipoValidacion) && unParametro.TipoValidacion.Contains("CAT")) //Clave fija de tipo de validación CATALOGO
                        {
                            pCI.Info("INICIA ListaElementosCatalogoPMA()");
                            this.StoreCatalogoPMA.DataSource = 
                                DAOProducto.ListaElementosCatalogoPMA(Convert.ToInt64(this.hdnIdColectiva.Value),
                                Convert.ToInt64(unParametro.ID_Parametro), string.Empty, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabAdminSubProductos);
                            pCI.Info("TERMINA ListaElementosCatalogoPMA()");

                            this.StoreCatalogoPMA.DataBind();
                            this.cBoxCatalogoPMA.Value = unParametro.Valor;
                            this.cBoxCatalogoPMA.Hidden = false;
                        }
                        else
                        {
                            switch (unParametro.TipoDato.ToUpper())
                            {
                                case "BOOL":
                                case "BOOLEAN":
                                    this.cBoxValorPar.Value = unParametro.Valor;
                                    this.cBoxValorPar.Hidden = false;
                                    break;

                                case "FLOAT":
                                    this.txtValorParFloat.Value = unParametro.Valor;
                                    this.txtValorParFloat.Hidden = false;
                                    break;

                                case "INT":
                                    this.txtValorParInt.Value = unParametro.Valor;
                                    this.txtValorParInt.Hidden = false;
                                    break;

                                case "STRING":
                                    this.txtValorParString.Value = unParametro.Valor;
                                    this.txtValorParString.Hidden = false;
                                    break;
                            }
                        }

                        this.txtParametro.Text = unParametro.Descripcion;
                        this.WdwValorParametro.Title += " - " + unParametro.Nombre;
                        this.WdwValorParametro.Show();
                        break;

                    case "Delete":
                        pCI.Info("INICIA BorraValorParametro()");
                        LNProducto.BorraValorParametro((int)unParametro.ID_ValordelParametro, this.Usuario,
                            LH_ParabAdminSubProductos);
                        pCI.Info("TERMINA BorraValorParametro()");

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaParametrosSubproducto();
                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambio de las ventanas
        /// de edición de valor del parámetro de contrato
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorParametro_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                ParametroValor elParametro = new ParametroValor();

                elParametro.ID_Parametro = Convert.ToInt32(this.hdnIdParametroMA.Value);
                elParametro.ID_Plantilla = Convert.ToInt32(this.hdnIdSubproducto.Value);
                elParametro.ID_ValordelParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);
                elParametro.Valor = String.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                    string.IsNullOrEmpty(this.txtValorParInt.Text) ?
                    string.IsNullOrEmpty(this.txtValorParString.Text) ?
                    string.IsNullOrEmpty(this.cBoxValorPar.SelectedItem.Value) ?
                    this.cBoxCatalogoPMA.SelectedItem.Value : this.cBoxValorPar.SelectedItem.Value :
                    this.txtValorParString.Text : this.txtValorParInt.Text :
                    this.txtValorParFloat.Text;

                logPCI.Info("INICIA ModificaValorParametro()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminSubProductos, "_SubProducto");
                logPCI.Info("TERMINA ModificaValorParametro()");

                WdwValorParametro.Hide();

                X.Msg.Notify("Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaParametrosSubproducto();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al modificar el valor del Parámetro").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Filtrar de la pestaña de tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            LimpiaGridTarjetas();

            Thread.Sleep(100);

            btnFiltrarHide.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de tarjetas, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridTarjetas(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnExportExcel.Disabled = true;
            int IdSubProducto = Convert.ToInt32(this.hdnIdSubproducto.Value);
            LogPCI log = new LogPCI(LH_ParabAdminSubProductos);

            try
            {
                DataTable dtTarjetas = new DataTable();

                dtTarjetas = HttpContext.Current.Session["DtTarj_SubP"] as DataTable;

                if (dtTarjetas == null)
                {
                    log.Info("INICIA ObtieneTarjetasSubproducto()");
                    dtTarjetas = DAOProducto.ObtieneTarjetasSubproducto(
                        IdSubProducto, this.txtFiltroTarjeta.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminSubProductos);
                    log.Info("TERMINA ObtieneTarjetasSubproducto()");

                    if (dtTarjetas.Rows.Count < 1)
                    {
                        X.Msg.Alert("Tarjetas", "No existen tarjetas con el filtro solicitado").Show();
                        return;
                    }

                    dtTarjetas = Tarjetas.EnmascaraTablaConTarjetas(dtTarjetas, "ClaveMA", "Enmascara", LH_ParabAdminSubProductos);

                    HttpContext.Current.Session.Add("DtTarj_SubP", dtTarjetas);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabAdminSubProductos).Valor);

                if (dtTarjetas.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Tarjetas", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "AdminSubProd.ClicDePaso()",
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
                    int TotalRegistros = dtTarjetas.Rows.Count;

                    (this.StoreTarjetas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtTarjetas.Clone();
                    DataTable dtToGrid = dtTarjetas.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtTarjetas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTB_Tarjetas.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingToolBar1.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtTarjetas.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreTarjetas.DataSource = dtToGrid;
                    StoreTarjetas.DataBind();

                    this.btnExportExcel.Disabled = false;
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Tarjetas", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Tarjetas", "Ocurrió un error al obtener las Tajetas asociadas al Subproducto").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte a Excel
        /// </summary>
        [DirectMethod(Namespace = "AdminSubProd")]
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
            try
            {
                string reportName = "TarjetasProducto";
                DataTable _dtSucursales = HttpContext.Current.Session["DtTarj_SubP"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSucursales, reportName);

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
                LogPCI pCI = new LogPCI(LH_ParabAdminSubProductos);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Tarjetas", "Ocurrió un Error al Exportar el Detalle a Excel").Show();
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

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de sucursales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreTarjetas_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridTarjetas(inicio, columna, orden);
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "AdminSubProd")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
    }
}
