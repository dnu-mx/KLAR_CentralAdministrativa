using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Excel;
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

namespace PreAutorizador
{
    public partial class ParametrosGrupos : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Preautorizador Parámetros Grupo
        private LogHeader LH_PreautParamGrupo = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Parámetros de Grupos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_PreautParamGrupo.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_PreautParamGrupo.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_PreautParamGrupo.User = this.Usuario.ClaveUsuario;
            LH_PreautParamGrupo.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_PreautParamGrupo);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ParametrosGrupos Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaColectivasSubEmisor()");
                    DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_PreautParamGrupo);
                    log.Info("INICIA ListaColectivasSubEmisor()");

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


                    log.Info("INICIA ListaTipoPMAPreaut()");
                    this.StoreTipoParametroMA.DataSource = DAOProducto.ListaTipoPMAPreaut(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_PreautParamGrupo);
                    log.Info("TERMINA ListaTipoPMAPreaut()");

                    this.StoreTipoParametroMA.DataBind();


                    PagingTB_TarjetasGpo.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_PreautParamGrupo).Valor);

                    PagingTB_TarjetasPA.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_PreautParamGrupo).Valor);

                    HttpContext.Current.Session.Add("DtTarj_Gpo", null);
                    HttpContext.Current.Session.Add("DtTarj_PA", null);
                    HttpContext.Current.Session.Add("DtTarjMasivo", null);
                }

                log.Info("TERMINA ParametrosGrupos Page_Load()");
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
            ConsultaProductosDeColectiva(Convert.ToInt32(this.hdnIdColectiva.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idColectiva"></param>
        protected void ConsultaProductosDeColectiva(int idColectiva)
        {
            LogPCI unLog = new LogPCI(LH_PreautParamGrupo);

            try
            {
                unLog.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductos.DataSource = 
                    DAOProducto.ObtieneProductosDeColectiva(idColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_PreautParamGrupo);
                unLog.Info("TERMINA ObtieneProductosDeColectiva()");

                this.StoreProductos.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al obtener los Productos del Cliente").Show();
            }
        }

        /// <summary>
        /// Llena el grid de resultados de subproductos con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI pCI = new LogPCI(LH_PreautParamGrupo);

            try
            {
                LimpiaSeleccionPrevia();

                pCI.Info("INICIA ObtieneSubproductosPreAutPorColectivaProducto()");
                DataSet dsGpos = DAOProducto.ObtieneSubproductosPreAutPorColectivaProducto(
                    int.Parse(this.cBoxSubEmisor.SelectedItem.Value),
                    int.Parse(this.cBoxProducto.SelectedItem.Value),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_PreautParamGrupo);
                pCI.Info("TERMINA ObtieneSubproductosPreAutPorColectivaProducto()");

                if (dsGpos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Grupos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.StoreGrupos.DataSource = dsGpos;
                    this.StoreGrupos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Grupos", "Ocurrió un error al obtener los Grupos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarGrupo_Click(object sender, EventArgs e)
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
            this.StoreGrupos.RemoveAll();

            LimpiaVentanaParams();

            LimpiaSeleccionPrevia();

            PanelCentralGpo.Disabled = true;
        }
        
        /// <summary>
        /// Restablece los controles de la ventana de edición de parámetros
        /// </summary>
        protected void LimpiaVentanaParams()
        {
            this.FormPanelValorParamTxt.Reset();
            this.txtParametro.Reset();
            this.lblInstruc.Text = "";

            this.txtValorParFloat.Reset();
            this.txtValorParFloat.Hidden = true;

            this.txtValorParInt.Reset();
            this.txtValorParInt.Hidden = true;

            this.txtValorParString.Reset();
            this.txtValorParString.Hidden = true;

            this.cBoxValorPar.Reset();
            this.cBoxValorPar.Hidden = true;
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// un subproducto en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            this.FormPanelParametrosSP.Reset();
            this.cBoxTipoParametroMA.Reset();
            this.StoreParametros.RemoveAll();
            this.StoreValoresParametros.RemoveAll();
            this.btnAddParametros.Disabled = true;

            this.btnExportExcel.Disabled = true;
            this.txtFiltroTarjeta.Reset();
            this.StoreTarjetas.RemoveAll();

            this.btnAsignarTarjetas.Disabled = true;
            this.txtTarjetaPA.Reset();
            this.StoreTarjetasPA.RemoveAll();

            this.FormPanelParametrosSP.Show();

            LimpiaGridTarjetas();
            LimpiaGridTarjetas_PA();
            LimpiaGridTarjetasArchivo();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Ver Tarjetas
        /// </summary>
        protected void LimpiaGridTarjetas()
        {
            this.btnExportExcel.Disabled = true;

            HttpContext.Current.Session.Add("DtTarj_Gpo", null);
            StoreTarjetas.RemoveAll();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Asignación de Tarjetas
        /// </summary>
        protected void LimpiaGridTarjetas_PA()
        {
            this.btnAsignarTarjetas.Disabled = true;

            HttpContext.Current.Session.Add("DtTarj_PA", null);
            StoreTarjetasPA.RemoveAll();
        }

        /// <summary>
        /// Establece en nulos los controles relacionados al Grid de Asignación de Tarjetas
        /// </summary>
        protected void LimpiaGridTarjetasArchivo()
        {
            this.fufCargaMasiva.Reset();
            this.btnAsignarMasivo.Disabled = true;

            HttpContext.Current.Session.Add("DtTarjMasivo", null);
            StoreTarjArchivo.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de nuevo grupo, llamando a la inserción de la
        /// nueva plantilla en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevoGpo_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PreautParamGrupo);

            try
            {
                Plantilla laPlantilla = new Plantilla();
                int _idProd = Convert.ToInt32(this.cBoxProdGpo.SelectedItem.Value);

                laPlantilla.ID_Producto = _idProd;
                laPlantilla.Clave = this.txtClaveGpo.Text;
                laPlantilla.Descripcion = this.txtDescGpo.Text;

                unLog.Info("INICIA CreaPlantillaPreAutDelProducto()");
                DataTable _dt = LNProducto.CreaPlantillaPreAutDelProducto(laPlantilla, LH_PreautParamGrupo);
                unLog.Info("TERMINA CreaPlantillaPreAutDelProducto()");

                string msj = _dt.Rows[0]["Mensaje"].ToString();
                string idNuevoGpo = _dt.Rows[0]["ID_NuevoGrupo"].ToString();

                if (idNuevoGpo == "-1")
                {
                    X.Msg.Alert("Nuevo Grupo", msj).Show();
                }
                else
                {
                    ///Otorgar los permisos a la recién creada plantilla
                    PermisosNuevaPlantilla(idNuevoGpo, false);

                    this.WdwNuevaPlantillaGpo.Hide();
                    this.cBoxSubEmisor.Value = this.cBoxColectivaGpo.Value;
                    this.cBoxProducto.Value = this.cBoxProdGpo.Value;
                    LlenaGridResultados();

                    X.Msg.Alert("Nuevo Grupo", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "PreautGrupos.CargaNuevoGrupo()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Grupo", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Nuevo Grupo", "Ocurrió un error al crear el Grupo").Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Aceptar al mensaje informativo
        /// de creación exitosa del grupo
        /// </summary>
        [DirectMethod(Namespace = "PreautGrupos")]
        public void CargaNuevoGrupo()
        {
            RowSelectionModel rsm = GridResultGposParab.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultGposParab.FireEvent("RowClick");
        }

        /// <summary>
        /// Otorga los permisos a la nueva plantilla para el usuario en sesión
        /// </summary>
        /// <param name="nuevoGrupo">Identificador de la nueva plantilla</param>
        /// <param name="esProducto">Bandera de filtro entre producto o plantilla</param>
        protected void PermisosNuevaPlantilla(string nuevoGrupo, bool esProducto)
        {
            LogPCI log = new LogPCI(LH_PreautParamGrupo);

            try
            {
                log.Info("INICIA ObtieneIdFiltro()");
                Guid idTabla = DAOFiltro.ObtieneIdFiltro(Guid.Parse(
                    ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "SPFiltroPlantillas", LH_PreautParamGrupo).Valor.Trim(), LH_PreautParamGrupo);
                log.Info("TERMINA ObtieneIdFiltro()");

                //Se crean los permisos para el usuario
                log.Info("INICIA CreaPermisoTableValue()");
                LNUsuarios.CreaPermisoTableValue(this.Usuario.UsuarioId, idTabla, nuevoGrupo,
                    true, LH_PreautParamGrupo, this.Usuario);
                log.Info("TERMINA CreaPermisoTableValue()");

                //Se otrogan el permiso inmediato al usuario en sesión
                log.Info("INICIA AgregarFiltrosEnSesion()");
                LNFiltro.AgregarFiltrosEnSesion(this.Usuario, LH_PreautParamGrupo,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                log.Info("TERMINA AgregarFiltrosEnSesion()");
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Colectiva para un nuevo grupo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceProdsNuevoGpo(object sender, EventArgs e)
        {
            ConsultaProductosDeColectiva(
                Convert.ToInt32(this.cBoxColectivaGpo.SelectedItem.Value));
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
                int IdGrupo = 0;
                string ClaveGrupo = "", NombreGrupo = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] subproducto = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in subproducto[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Plantilla": IdGrupo = int.Parse(column.Value); break;
                        case "Clave": ClaveGrupo = column.Value; break;
                        case "Descripcion": NombreGrupo = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnIdGrupo.Value = IdGrupo;
                this.hdnGrupo.Value = NombreGrupo;

                PanelCentralGpo.Title = ClaveGrupo +  " - " + NombreGrupo;

                PanelCentralGpo.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI logPCI = new LogPCI(LH_PreautParamGrupo);
                logPCI.ErrorException(ex);
                X.Msg.Alert("Grupos", "Ocurrió un error al obtener la información del Grupo").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de tipos de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaTipoPMA(object sender, EventArgs e)
        {
            LlenaParametrosGrupo();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros del subproducto,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosGrupo()
        {
            LogPCI pCI = new LogPCI(LH_PreautParamGrupo);

            try
            {
                Int64 IdGrupo = Convert.ToInt64(this.hdnIdGrupo.Value);
                int _idGrupo = Convert.ToInt32(this.hdnIdGrupo.Value);

                pCI.Info("INICIA ObtienePMAPorTipoSinAsignarSubProd()");
                this.StoreParametros.DataSource = DAOProducto.ObtienePMAPorTipoSinAsignarSubProd(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), _idGrupo, LH_PreautParamGrupo);
                pCI.Info("TERMINA ObtienePMAPorTipoSinAsignarSubProd()");

                this.StoreParametros.DataBind();

                pCI.Info("INICIA ObtienePMAsPorPlantilla()");
                this.StoreValoresParametros.DataSource = DAOParametroMA.ObtienePMAsPorPlantilla(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdGrupo, LH_PreautParamGrupo);
                pCI.Info("TERMINA ObtienePMAsPorPlantilla()");

                this.StoreValoresParametros.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Parámetros del Grupo").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametros_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PreautParamGrupo);

            try
            {
                int IdGrupo = Convert.ToInt32(this.hdnIdGrupo.Value);

                unLog.Info("INICIA AgregaParametroMAASubproducto()");
                LNProducto.AgregaParametroMAASubproducto(
                    Convert.ToInt32(cBoxParametros.SelectedItem.Value), IdGrupo, this.Usuario, LH_PreautParamGrupo, "_ParamGrupo");
                unLog.Info("TERMINA AgregaParametroMAASubproducto()");

                X.Msg.Notify("", "Parámetro Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.cBoxParametros.Reset();

                LlenaParametrosGrupo();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro", "Ocurrió un error al asignar el Parámetro al Grupo").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_PreautParamGrupo);
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
                        case "Instrucciones": unParametro.Instrucciones = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                this.hdnIdParametroMA.Value = unParametro.ID_Parametro;
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;

                switch (comando)
                {
                    case "Edit":
                        LimpiaVentanaParams();

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

                        this.txtParametro.Text = unParametro.Descripcion;
                        this.lblInstruc.Text = unParametro.Instrucciones;
                        this.WdwValorParametro.Title += " - " + unParametro.Nombre;
                        this.WdwValorParametro.Show();
                        break;

                    case "Delete":
                        pCI.Info("INICIA BorraValorParametro()");
                        LNProducto.BorraValorParametro((int)unParametro.ID_ValordelParametro, this.Usuario,
                            LH_PreautParamGrupo, "_ParamGrupo");
                        pCI.Info("TERMINA BorraValorParametro()");

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaParametrosGrupo();
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
            LogPCI unLog = new LogPCI(LH_PreautParamGrupo);

            try
            {
                ParametroValor elParametro = new ParametroValor();

                elParametro.ID_Parametro = Convert.ToInt32(this.hdnIdParametroMA.Value);
                elParametro.ID_Plantilla = Convert.ToInt32(this.hdnIdGrupo.Value);
                elParametro.ID_ValordelParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);
                elParametro.Valor = String.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                    String.IsNullOrEmpty(this.txtValorParInt.Text) ?
                    String.IsNullOrEmpty(this.txtValorParString.Text) ?
                    this.cBoxValorPar.SelectedItem.Value : this.txtValorParString.Text :
                    this.txtValorParInt.Text : this.txtValorParFloat.Text;
                //Validación para permitir valores en blanco
                elParametro.Valor = String.IsNullOrEmpty(elParametro.Valor) ? "" : elParametro.Valor;

                unLog.Info("INICIA ModificaValorParametro()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_PreautParamGrupo, "_ParamGrupo");
                unLog.Info("TERMINA ModificaValorParametro()");

                WdwValorParametro.Hide();

                X.Msg.Notify("Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaParametrosGrupo();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al modificar el Parámetro").Show();
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
            int IdGrupo = Convert.ToInt32(this.hdnIdGrupo.Value);
            LogPCI log = new LogPCI(LH_PreautParamGrupo);

            try
            {
                DataTable dtTarjetasGpo = new DataTable();

                dtTarjetasGpo = HttpContext.Current.Session["DtTarj_Gpo"] as DataTable;

                if (dtTarjetasGpo == null)
                {
                    log.Info("INICIA ObtieneTarjetasPlantillaPreaut()");
                    dtTarjetasGpo = DAOProducto.ObtieneTarjetasPlantillaPreaut(
                        IdGrupo, this.txtFiltroTarjeta.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_PreautParamGrupo);
                    log.Info("TERMINA ObtieneTarjetasPlantillaPreaut()");

                    if (dtTarjetasGpo.Rows.Count < 1)
                    {
                        X.Msg.Alert("Tarjetas", "No existen tarjetas con el filtro solicitado").Show();
                        return;
                    }

                    dtTarjetasGpo = Tarjetas.EnmascaraTablaConTarjetas(dtTarjetasGpo, "ClaveMA", "Enmascara", LH_PreautParamGrupo);

                    HttpContext.Current.Session.Add("DtTarj_Gpo", dtTarjetasGpo);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_PreautParamGrupo).Valor);

                if (dtTarjetasGpo.Rows.Count > maxRegistros)
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
                    int TotalRegistros = dtTarjetasGpo.Rows.Count;

                    (this.StoreTarjetas.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtTarjetasGpo.Clone();
                    DataTable dtToGrid = dtTarjetasGpo.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtTarjetasGpo.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTB_TarjetasGpo.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTB_TarjetasGpo.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtTarjetasGpo.Rows[row] : sortedDT.Rows[row]);
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
                X.Msg.Alert("Tarjetas", "Ocurrió un error al obtener las Tarjetas").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
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
                string reportName = "TarjetasGrupo";
                DataTable _dtTarjetas = HttpContext.Current.Session["DtTarj_Gpo"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtTarjetas, reportName);

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
                LogPCI pCI = new LogPCI(LH_PreautParamGrupo);
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
        [DirectMethod(Namespace = "AdminGpo")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento Click al botón Filtrar de la pestaña de Asignar Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnFiltrarPA_Click(object sender, EventArgs e)
        {
            LimpiaGridTarjetas_PA();

            Thread.Sleep(100);

            this.btnFiltrarHidePA.FireEvent("click");
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de tarjetas por asignar , así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridTarjetasPA(int RegistroInicial, string Columna, SortDirection Orden)
        {
            this.btnAsignarTarjetas.Disabled = true;
            int IdGrupo = Convert.ToInt32(this.hdnIdGrupo.Value);
            LogPCI unLog = new LogPCI(LH_PreautParamGrupo);

            try
            {
                DataTable dtTarjetasPA = new DataTable();

                dtTarjetasPA = HttpContext.Current.Session["DtTarj_PA"] as DataTable;

                if (dtTarjetasPA == null)
                {
                    unLog.Info("INICIA ObtienePosiblesTarjetasPlantillaPreaut()");
                    dtTarjetasPA = DAOProducto.ObtienePosiblesTarjetasPlantillaPreaut(
                        IdGrupo, this.txtTarjetaPA.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_PreautParamGrupo);
                    unLog.Info("TERMINA ObtienePosiblesTarjetasPlantillaPreaut()");

                    if (dtTarjetasPA.Rows.Count < 1)
                    {
                        X.Msg.Alert("Tarjetas por Asignar", "No existen tarjetas con el filtro solicitado").Show();
                        return;
                    }
                    else
                    {
                        string msj = dtTarjetasPA.Rows[0]["Mensaje"].ToString();

                        if (msj != "OK")
                        {
                            X.Msg.Alert("Tarjetas por Asignar", msj).Show();
                            return;
                        }

                        dtTarjetasPA = Tarjetas.EnmascaraTablaConTarjetas(dtTarjetasPA, "Tarjeta", "Enmascara", LH_PreautParamGrupo);

                        HttpContext.Current.Session.Add("DtTarj_PA", dtTarjetasPA);
                    }
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_PreautParamGrupo).Valor);

                if (dtTarjetasPA.Rows.Count > maxRegistros)
                {
                    X.Msg.Alert("Tarjetas por Asignar", "Demasiadas coincidencias. Por favor, afina tu búsqueda").Show();
                }
                else
                {
                    int TotalRegistros = dtTarjetasPA.Rows.Count;

                    (this.StoreTarjetasPA.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtTarjetasPA.Clone();
                    DataTable dtToGrid = dtTarjetasPA.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtTarjetasPA.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTB_TarjetasPA.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTB_TarjetasPA.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtTarjetasPA.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreTarjetasPA.DataSource = dtToGrid;
                    StoreTarjetasPA.DataBind();

                    this.btnAsignarTarjetas.Disabled = false;
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Tarjetas por Asignar", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Tarjetas por Asignar", "Ocurrió un error al obtener las Tarjetas por Asignar").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de tarjetas por asignar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreTarjetasPA_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridTarjetasPA(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la asignación de tarjetas al grupo tras las validaciones en front
        /// </summary>
        [DirectMethod(Namespace = "ParamPreautPlantilla")]
        public void AsignarTarjetas()
        {
            bool estatus = true;
            LogPCI logPCI = new LogPCI(LH_PreautParamGrupo);

            Int64 idPlantilla = Convert.ToInt64(this.hdnIdGrupo.Value);
            RowSelectionModel lasTarjetas = this.GridTarjetasPA.SelectionModel.Primary as RowSelectionModel;

            foreach (SelectedRow tarjeta in lasTarjetas.SelectedRows)
            {
                try
                {
                    logPCI.Info("INICIA ModificaPlantillaDeTarjeta()");
                    LNProducto.ModificaPlantillaDeTarjeta(int.Parse(tarjeta.RecordID), idPlantilla,
                        this.Usuario, LH_PreautParamGrupo);
                    logPCI.Info("TERMINA ModificaPlantillaDeTarjeta()");
                }
                catch (CAppException caEX)
                {
                    logPCI.Error(caEX.Mensaje() + " Cod.(" + caEX.CodigoError().ToString() + ")");
                    estatus = false;
                }
                catch (Exception ex)
                {
                    logPCI.ErrorException(ex);
                    estatus = false;
                }
            }

            if (!estatus)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Asignacion de Tarjetas",
                    Message = "Algunas tarjetas no pudieron asignarse.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO
                });
            }
            else
            {
                X.Msg.Notify("", "Tarjetas asignadas" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            this.btnFiltrarPA.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Click al botón Cargar Archivo de la ventana flotante para carga masiva
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        public void btnCargarArchivo_Click(object sender, DirectEventArgs e)
        {
            try
            {
                //   Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
                HttpPostedFile file = this.fufCargaMasiva.PostedFile;

                string[] directorios = file.FileName.Split('\\');
                string fileName = directorios[directorios.Count() - 1];

                //Se valida que se haya seleccionado un archivo
                if (String.IsNullOrEmpty(fileName))
                {
                    X.Msg.Alert("Cargar Archivo", "Selecciona un archivo para cargarlo.").Show();
                    return;
                }

                //Se valida que se haya seleccionado un archivo Excel
                if (!fileName.Contains(".xlsx"))
                {
                    X.Msg.Alert("Cargar Archivo", "El archivo seleccionado no es del formato Excel soportado (*.xlsx). Verifica tu archivo.").Show();
                    return;
                }

                string tempPath = "C:\\TmpXlsxFiles\\";

                //Si no existe el directorio, lo crea
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                tempPath += fileName;

                //Se almacena archivo en directorio temporal del que es dueño el aplicativo
                //para no tener problemas de permisos de apertura
                file.SaveAs(tempPath);

                FileStream stream = File.Open(tempPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = null;

                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                if (fileName.Contains(".xlsx"))
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    //DataSet - Create column names from first row
                    excelReader.IsFirstRowAsColumnNames = false;
                }

                if (!String.IsNullOrEmpty(excelReader.ExceptionMessage))
                {
                    throw new Exception("IExcelDataReader exception: " + excelReader.ExceptionMessage);
                }

                DataSet dsArchivo = excelReader.AsDataSet();

                DataTable _laTabla = FormatoAArchivo(dsArchivo);

                //Se valida la longitud de las tarjetas
                if (!VerificaLongitudTarjetas(_laTabla))
                {
                    return;
                }

                //Se valida que no existan duplicados en el archivo
                if (VerificaTarjetasDuplicadas(_laTabla))
                {
                    return;
                }

                //Se valida la existencia de las tarjetas en base de datos
                if (!VerificaTarjetas(_laTabla))
                {
                    return;
                }

                //Se verifica que el producto del grupo sea el mismo para todas las tarjetas
                if (!VerificaProductos(_laTabla))
                {
                    return;
                }

                //Se verifica que ninguna tarjeta esté cancelada
                if(!VerificaTarjetasCanceladas(_laTabla))
                {
                    return;
                }

                HttpContext.Current.Session.Add("DtTarjMasivo", _laTabla);

                LlenaGridTarjetasArchivo(_laTabla);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cargar Archivo", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_PreautParamGrupo);
                pCI.ErrorException(ex);
                X.Msg.Alert("Cargar Archivo", "Ocurrió un error durante la carga del archivo").Show();
            }
        }

        /// <summary>
        /// Se da formato a la tabla de datos con la información del archivo por cargar
        /// </summary>
        /// <param name="dsArchivo">Objeto con la información del archivo</param>
        /// <returns>DataTable con formato</returns>
        protected DataTable FormatoAArchivo(DataSet dsArchivo)
        {
            DataTable dtCloned = dsArchivo.Tables[0].Clone();
            int NumColumnas = dsArchivo.Tables[0].Columns.Count;

            try
            {
                if (NumColumnas > 1)
                {
                    for (int iCounter = 1; iCounter < NumColumnas; iCounter++)
                    {
                        dtCloned.Columns.RemoveAt(iCounter);
                    }
                }

                NumColumnas = dtCloned.Columns.Count;
                int val = 0;

                for (int i = 0; i < NumColumnas; i++)
                {
                    dtCloned.Columns[i].DataType = typeof(string);
                }

                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    for (int i = 0; i < NumColumnas; i++)
                    {
                        if (string.IsNullOrEmpty(row.ItemArray[i].ToString()))
                        {
                            dtCloned.Columns[i].DataType = typeof(string);
                        }
                    }
                    val++;
                }

                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    //Se eliminan espacios en blanco de más, 
                    //al inicio o al final, de todos los datos
                    for (int column = 0; column < 1; column++)
                    {
                        row[column] = row[column].ToString().Trim();
                    }

                    dtCloned.ImportRow(row);
                }

                dtCloned.Columns[0].ColumnName = "NUMTARJETA";
                dtCloned.AcceptChanges();

                return dtCloned;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_PreautParamGrupo);
                pCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Establece los datos en el grid de tarjetas por asignar de forma masiva, completando la tabla de datos
        /// recibida con la información de base de datos
        /// </summary>
        /// <param name="dt">Tabla de datos con la información del archivo cargado</param>
        protected void LlenaGridTarjetasArchivo(DataTable dt)
        {
            Int64 IdPlantilla = Convert.ToInt64(this.hdnIdGrupo.Value);
            DataTable dtTarjetasMasivo = new DataTable();
            LogPCI unLog = new LogPCI(LH_PreautParamGrupo);

            try
            {
                unLog.Info("INICIA ObtienePlantillasTarjetasPreaut()");
                dtTarjetasMasivo = DAOProducto.ObtienePlantillasTarjetasPreaut(IdPlantilla, dt, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_PreautParamGrupo);
                unLog.Info("TERMINA ObtienePlantillasTarjetasPreaut()");

                this.StoreTarjArchivo.DataSource = dtTarjetasMasivo;
                this.StoreTarjArchivo.DataBind();

                this.btnAsignarMasivo.Disabled = false;
            }
            catch(CAppException caEx)
            {
                throw caEx;
            }
            catch(Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Valida si existen tarjetas duplicadas en el archivo, regresando TRUE en tal caso
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>FALSE si no hay tarjetas duplicadas</returns>
        protected bool VerificaTarjetasDuplicadas(DataTable _dtClonado)
        {
            bool hayDuplicadas = false;

            try
            {

                var tarjetasDuplicadas = _dtClonado.AsEnumerable()
               .Select(dr => dr.Field<string>("NUMTARJETA"))
               .GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();

                if (tarjetasDuplicadas.Count > 0)
                {
                    foreach (String tarjeta in tarjetasDuplicadas)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error de contenido en archivo excel",
                            Message = "El número de tarjeta <b>" + tarjeta + "</b> está duplicado en el archivo. Verifícalo.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.WARNING
                        });
                        hayDuplicadas = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_PreautParamGrupo);
                unLog.ErrorException(ex);
                X.Msg.Alert("Tarjetas Duplicadas", "Ocurrió un error durante la validación de tarjetas duplicadas").Show();
            }

            return hayDuplicadas;
        }

        /// <summary>
        /// Valida que la longitud de las tarjetas no exceda el tamaño máximo de 16
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>FALSE si se excede la longitud en algún dato</returns>
        protected bool VerificaLongitudTarjetas(DataTable _dt)
        {
            bool validacion = true;
            int rowIndex = 1;

            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    if (row[0].ToString().Length != 16)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error de contenido en archivo excel",
                            Message = "La longitud de la tarjeta en la fila no. <b>" + rowIndex.ToString() + "</b> es incorrecta." +
                                  "<br/>Por favor, verifica que sean 16 dígitos.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.WARNING
                        });
                        validacion = false;
                        break;
                    }

                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_PreautParamGrupo);
                unLog.ErrorException(ex);
                X.Msg.Alert("Longitud de Tarjetas", "Ocurrió un error durante la validación de longitudes de tarjetas").Show();
            }

            return validacion;
        }

        /// <summary>
        /// Solicita la validación en base de datos de las tarjetas, si están dadas de alta.
        /// Si alguna no existe, devuelve FALSE
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si todas las tarjetas cumplen la validación</returns>
        protected bool VerificaTarjetas(DataTable _dt)
        {
            bool validacion = true;
            string msjResp = String.Empty;
            int rowIndex = 1;

            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    msjResp = DAOProducto.ValidaExisteTarjeta(row[0].ToString(), this.Usuario);

                    if (msjResp.ToUpper().Contains("NO"))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error de contenido en archivo excel",
                            Message = "La tarjeta de la fila no. <b>" + rowIndex.ToString() + "</b> no existe." +
                                   "<br/>Verifica el número de tarjeta.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.WARNING
                        });
                        validacion = false;
                        break;
                    }

                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_PreautParamGrupo);
                unLog.ErrorException(ex);
                X.Msg.Alert("Existencia Tarjetas", "Ocurrió un error durante la validación de existencia de tarjetas").Show();
            }

            return validacion;
        }

        /// <summary>
        /// Solicita la validación en base de datos entre el producto del grupo seleccionado
        /// y el producto de la tarjeta en el archivo. Si alguno no corresponde, devuelve FALSE
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si todas las tarjetas cumplen la validación</returns>
        protected bool VerificaProductos(DataTable _dt)
        {
            bool validacion = true;
            string msjValidacion = String.Empty;
            int rowIndex = 1;
            Int64 IdPlantilla = Convert.ToInt64(this.hdnIdGrupo.Value);

            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    msjValidacion = DAOProducto.ValidaProductosTarjetaPlantilla(
                        row[0].ToString(), IdPlantilla, this.Usuario);

                    if (msjValidacion.ToUpper().Contains("NO"))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error de contenido en archivo excel",
                            Message = "La tarjeta de la fila no. <b>" + rowIndex.ToString() + "</b> no puede asignarse a este Grupo, " +
                                   "debido a que pertenece a otro Producto.<br/>Verifica el número de tarjeta.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.WARNING
                        });
                        validacion = false;
                        break;
                    }

                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_PreautParamGrupo);
                unLog.ErrorException(ex);
                X.Msg.Alert("Productos y Tarjetas", "Ocurrió un error durante la validación de productos/tarjetas").Show();
            }

            return validacion;
        }

        /// <summary>
        /// Solicita la validación a base de datos de las tarjetas, para indentificar canceladas.
        /// Si alguna lo está, devuelve FALSE
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si todas las tarjetas cumplen la validación</returns>
        protected bool VerificaTarjetasCanceladas(DataTable _dt)
        {
            bool validacion = true;
            string msj = String.Empty;
            int rowIndex = 1;

            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    msj = DAOProducto.ValidaTarjetaNoCancelada(row[0].ToString(), this.Usuario);

                    if (msj.ToUpper().Contains("NO"))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error de contenido en archivo excel",
                            Message = "La tarjeta de la fila no. <b>" + rowIndex.ToString() + "</b> no puede asignarse a este Grupo, " +
                                   "debido a que está cancelada. <br/>Verifica el número de tarjeta.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.WARNING
                        });
                        validacion = false;
                        break;
                    }

                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_PreautParamGrupo);
                unLog.ErrorException(ex);
                X.Msg.Alert("Tarjetas Canceladas", "Ocurrió un error durante la validación de tarjetas canceladas").Show();
            }

            return validacion;
        }

        /// <summary>
        /// Solicita la asignación de tarjetas al grupo a base de datos
        /// </summary>
        [DirectMethod(Namespace = "ParamPreautPlantilla")]
        public void AsignaTarjetasMasivo()
        {
            LogPCI logPCI = new LogPCI(LH_PreautParamGrupo);

            try
            {
                Int64 idPlantilla = Convert.ToInt64(this.hdnIdGrupo.Value);
                DataTable dtTarjetas = HttpContext.Current.Session["DtTarjMasivo"] as DataTable;

                logPCI.Info("INICIA ModificaPlantillaTarjetas()");
                LNProducto.ModificaPlantillaTarjetas(idPlantilla, dtTarjetas, this.Usuario, LH_PreautParamGrupo);
                logPCI.Info("TERMINA ModificaPlantillaTarjetas()");

                X.Msg.Notify("", "Tarjetas asignadas" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.WdwCargaMasiva.Hide();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Asignar Tarjetas por Archivo", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Asignar Tarjetas por Archivo", "Ocurrió un error al asignar las Tarjetas al Grupo").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }
    }
}
