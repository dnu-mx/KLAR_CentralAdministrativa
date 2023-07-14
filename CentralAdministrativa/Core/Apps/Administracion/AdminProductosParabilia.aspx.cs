using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.BaseDatos;
using DALCentralAplicaciones.LogicaNegocio;
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
using Producto = DALAutorizador.Entidades.Producto;

namespace Administracion
{
    public partial class AdminProductosParabilia : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Administrar Productos
        private LogHeader LH_ParabAdminProductos = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Productos para Parabilia
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminProductos.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminProductos.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminProductos.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminProductos.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            this.cBoxTipoProd.Disabled = true;
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdminProductosParabilia Page_Load()");

                if (!IsPostBack)
                {
                    EstableceSubEmisores();

                    EstableceCombos();

                    dfFIniCampanya.SetValue(DateTime.Now);
                    dfFIniCampanya.MinDate = DateTime.Today;

                    dfFFinCampanya.SetValue(DateTime.Today.AddDays(2));
                }

                log.Info("TERMINA AdminProductosParabilia Page_Load()");
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
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            log.Info("INICIA ListaColectivasSubEmisor()");
            DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminProductos);
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
        }

        /// <summary>
        /// Establece los valores de los combos prestablecidos con la información de base de datos
        /// </summary>
        protected void EstableceCombos()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            unLog.Info("INICIA ListaTiposIntegracion()");
            this.StoreTipoIntegracion.DataSource =
                DAOProducto.ListaTiposIntegracion(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminProductos);
            unLog.Info("TERMINA ListaTiposIntegracion()");
            this.StoreTipoIntegracion.DataBind();

            unLog.Info("INICIA ListaGruposMediosAcceso()");
            this.StoreGruposMA.DataSource =
                DAOProducto.ListaGruposMediosAcceso(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminProductos);
            unLog.Info("TERMINA ListaGruposMediosAcceso()");
            this.StoreGruposMA.DataBind();

            unLog.Info("INICIA ListaTiposParametrosMA()");
            this.StoreTipoParametroMA.DataSource =
                DAOProducto.ListaTiposParametrosMA(false, this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminProductos);
            unLog.Info("TERMINA ListaTiposParametrosMA()");
            this.StoreTipoParametroMA.DataBind();

            unLog.Info("INICIA ListaPromociones()");
            this.StorePromociones.DataSource =
                DAOProducto.ListaPromociones(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminProductos);
            unLog.Info("TERMINA ListaPromociones()");
            this.StorePromociones.DataBind();
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Tipo de Producto de la ventana de nuevo producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void TipoProducto_Select(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);
            unLog.Info("TipoProducto_Select()");

            try
            {
                EstableceStoreProductosTitulares(Convert.ToInt32(this.hdnNuevoTipoProd.Value));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Producto Titular", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Producto Titular", "Ocurrió un error al establecer los Productos Titulares").Show();
            }
        }

        /// <summary>
        /// Obtiene la lista de productos padre que corresponden al tipo de producto indicado,
        /// consultando y estableciendo la lista en el Store correspondiente
        /// </summary>
        protected void EstableceStoreProductosTitulares(int IdTipoProductoPadre)
        {
            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            try
            {
                log.Info("INICIA ObtieneProductosPadrePorTipo()");
                this.StoreProdPadre.DataSource =
                    DAOProducto.ObtieneProductosPadrePorTipo(IdTipoProductoPadre, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                log.Info("TERMINA ObtieneProductosPadrePorTipo()");

                this.StoreProdPadre.DataBind();
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
        /// Controla el evento Seleccionar del combo Emisor de la ventana de nuevo producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Emisor_Select(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);
            unLog.Info("Emisor_Select()");

            try
            {
                unLog.Info("INICIA ListaTiposProducto()");
                this.StoreTipoProducto.DataSource =
                    DAOProducto.ListaTiposProducto(Convert.ToInt32(this.cBoxColNuevoProd.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                unLog.Info("TERMINA ListaTiposProducto()");
                this.StoreTipoProducto.DataBind();

                string clavePMA = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClavePMACatDisenyoTarjFis").Valor;

                EstableceCatalogoPMA(Convert.ToInt64(this.cBoxColNuevoProd.Value), -1, clavePMA, true);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tipo de Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Tipo de Productos", "Ocurrió un error al establecer los Tipos de Productos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Emisor de la ventana de nuevo producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Emisor_Select_Edit(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);
            unLog.Info("Emisor_Select_Edit()");

            try
            {
                //EstableceStoreProductosTitulares(Convert.ToInt32(this.hdnNuevoTipoProd.Value));
                unLog.Info("INICIA ListaTiposProducto_Edit()");
                this.StoreTipoProducto.DataSource =
                    DAOProducto.ListaTiposProducto(Convert.ToInt32(this.cBoxColectiva.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                unLog.Info("TERMINA ListaTiposProducto_Edit()");
                this.StoreTipoProducto.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tipos Productos Edit", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Tipos Productos Edit", "Ocurrió un error al establecer los Tipos de Productos para editar").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana de Nuevo Producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                bool ctaInterna = false;
                this.hdnValidaTipoMAExt.Value = false;
                this.hdnValidaMACLABE.Value = false;

                pCI.Info("INICIA ListaTiposMAPorTipoProducto()");
                List<string> clavesTiposMA = DAOProducto.ListaTiposMAPorTipoProducto(
                    Convert.ToInt32(this.hdnNuevoTipoProd.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                pCI.Info("TERMINA ListaTiposMAPorTipoProducto()");

                foreach (string laClave in clavesTiposMA)
                {
                    if (laClave.Contains("CACAO") || laClave.Contains("CTAINT"))
                    {
                        ctaInterna = true;
                    }
                }

                if (!ctaInterna)
                {
                    X.Msg.Alert("Nuevo Producto", "El Tipo de Producto seleccionado no tiene relación con " +
                        "el Tipo de Medios de Acceso:<br/><b> CUENTA INTERNA.</b><br/><br/> Contacte al Administrador del Sistema.").Show();
                    return;
                }

                foreach (string clave in clavesTiposMA)
                {
                    switch (clave)
                    {
                        case "CTAEXT":
                            this.hdnValidaTipoMAExt.Value = true;
                            ConfirmaTipoMAExterno();
                            break;

                        case "CLABE":
                            this.hdnValidaMACLABE.Value = true;
                            ConfirmaTipoMACLABE();
                            break;

                        default:
                            break;
                    }
                }

                if (!Convert.ToBoolean(this.hdnValidaTipoMAExt.Value) &&
                    !Convert.ToBoolean(this.hdnValidaMACLABE.Value))
                {
                    CreaProductoDePaso();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Producto", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Nuevo Producto", "Ocurrió un error al generar el Nuevo Producto").Show();
            }
        }

        /// <summary>
        /// Solicita la confirmación en pantalla para generar MAs externos para el nuevo producto
        /// </summary>
        protected void ConfirmaTipoMAExterno()
        {
            X.Msg.Confirm("Tipos de Medios de Acceso", "¿El nuevo producto generará un Medio de Acceso Externo" +
            " al crear nuevas cuentas?", new MessageBoxButtonsConfig
            {
                Yes = new MessageBoxButtonConfig
                {
                    Handler = "AdminProdParabilia.CuentaEXTDePaso(true)",
                    Text = "Sí"
                },
                No = new MessageBoxButtonConfig
                {
                    Handler = "AdminProdParabilia.CuentaEXTDePaso(false)",
                    Text = "No"
                }
            }).Show();
        }

        /// <summary>
        /// Solicita la confirmación en pantalla para generar MA CLABE para el nuevo producto
        /// </summary>
        protected void ConfirmaTipoMACLABE()
        {
            X.Msg.Confirm("Tipos de Medios de Acceso", "¿El nuevo producto generará un Medio de Acceso" +
            " CLABE al crear nuevas cuentas?", new MessageBoxButtonsConfig
            {
                Yes = new MessageBoxButtonConfig
                {
                    Handler = "AdminProdParabilia.CuentaCLABEDePaso(true)",
                    Text = "Sí"
                },
                No = new MessageBoxButtonConfig
                {
                    Handler = "AdminProdParabilia.CuentaCLABEDePaso(false)",
                    Text = "No"
                }
            }).Show();
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic del botón de validación
        /// de tipo MA Externo
        /// </summary>
        /// <param name="GeneraTMAExterno">Bandera de confirmación para generar MAs externos</param>
        [DirectMethod(Namespace = "AdminProdParabilia")]
        public void CuentaEXTDePaso(bool GeneraMAExterno)
        {
            this.hdnGeneraMAExt.Value = GeneraMAExterno;

            LimpiaWdwTipoMAExterno();

            btnValidaCtaExt.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Clic del botón de validación de tipos MA externos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void TiposMAExternos(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                if (Convert.ToBoolean(this.hdnGeneraMAExt.Value))
                {
                    pCI.Info("INICIA ObtieneTiposMAExternos()");
                    this.StoreTipoMAExterno.DataSource = DAOProducto.ObtieneTiposMAExternos(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminProductos);
                    pCI.Info("TERMINA ObtieneTiposMAExternos()");

                    this.StoreTipoMAExterno.DataBind();

                    WdwTipoMAExterno.Show();
                }
                else
                {
                    if (String.IsNullOrEmpty(this.hdnGeneraMACLABE.Value.ToString()) && 
                        Convert.ToBoolean(this.hdnValidaMACLABE.Value))
                    {
                        ConfirmaTipoMACLABE();
                    }
                    else
                    {
                        CreaProductoDePaso();
                    }
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Producto", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Nuevo Producto", "Ocurrió un error al establecer los tipos MA externos").Show();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic del botón de validación
        /// de tipo MA CLABE
        /// </summary>
        [DirectMethod(Namespace = "AdminProdParabilia")]
        public void CuentaCLABEDePaso(bool GeneraMACLABE)
        {
            this.hdnGeneraMACLABE.Value = GeneraMACLABE;
            btnValidaCtaCLABE.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Clic del botón de validación de tipo MA CLABE
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceParametrosCLABE(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                this.ParametrosCLABE.SetSource(source);

                if (Convert.ToBoolean(this.hdnGeneraMACLABE.Value))
                {
                    log.Info("INICIA ListaParametrosCLABE()");
                    List<ParametroValor> losParametros = DAOParametroMA.ListaParametrosCLABE(
                        Int64.Parse(this.cBoxColNuevoProd.SelectedItem.Value), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminProductos);
                    log.Info("TERMINA ListaParametrosCLABE()");

                    foreach (ParametroValor propiedad in losParametros)
                    {
                        PropertyGridParameter GridProp = new PropertyGridParameter(propiedad.Nombre, propiedad.Valor);
                        GridProp.DisplayName = propiedad.Nombre;

                        TextField txtEditor = new TextField();
                        txtEditor.MaskRe = @"/[0-9]/";
                        GridProp.Editor.Add(txtEditor);

                        ParametrosCLABE.AddProperty(GridProp);
                    }

                    WdwTipoMACLABE.Show();
                }
                else
                {
                    if (String.IsNullOrEmpty(this.hdnGeneraMAExt.Value.ToString()) &&
                        Convert.ToBoolean(this.hdnValidaTipoMAExt.Value))
                    {
                        ConfirmaTipoMAExterno();
                    }
                    else
                    {
                        CreaProductoDePaso();
                    }
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Producto", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
               log.ErrorException(ex);
                X.Msg.Alert("Nuevo Producto", "Ocurrió un error al establecer los parámetros para el Tipo de Medio de Acceso CLABE").Show();
            }
        }

        /// <summary>
        /// Establece los valores de los parámetros de contrato para las cuentas CLABE
        /// </summary>
        [DirectMethod(Namespace = "AdminProdParabilia")]
        public void EstableceValoresParametrosCLABE()
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                List<ParametroValor> losCambios = new List<ParametroValor>();

                //Obtiene las propiedades que cambiaron
                foreach (PropertyGridParameter param in this.ParametrosCLABE.Source)
                {
                    if (param.IsChanged)
                    {
                        ParametroValor unaProp = new ParametroValor() { Nombre = param.Name, Valor = param.Value.ToString() };

                        losCambios.Add(unaProp);
                    }
                }

                //Validación pare evitar errores si el Grid está vacío
                if (losCambios.Count == 0)
                {
                    return;
                }

                //Guardar Valores
                Int64 IdColectiva = Convert.ToInt64(this.cBoxColNuevoProd.SelectedItem.Value);

                pCI.Info("INICIA ModificaValoresParametrosCLABE()");
                LNProducto.ModificaValoresParametrosCLABE(IdColectiva, losCambios, this.Usuario, LH_ParabAdminProductos);
                pCI.Info("TERMINA ModificaValoresParametrosCLABE()");

                CreaProductoDePaso();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros CLABE", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros CLABE", "Ocurrió un error al modificar los Parámetros CLABE").Show();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic del botón de crear producto
        /// </summary>
        [DirectMethod(Namespace = "AdminProdParabilia")]
        public void CreaProductoDePaso()
        {
            btnCreaProducto.FireEvent("click");
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Generación de Nuevo Producto
        /// </summary>
        [DirectMethod(Namespace = "AdminProdParabilia")]
        public void StopMask()
        {
            Thread.Sleep(200);
            X.Mask.Hide();
        }

        /// <summary>
        /// Solicita la creación de un nuevo producto en base de datos, controlando altas adicionales,
        /// de ser necesarias, y permisos a datos para el usuario en sesión
        /// </summary>
        protected void CreaNuevoProducto(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                Producto producto = new Producto();

                producto.Clave = this.txtClaveProd.Text;
                producto.Descripcion = this.txtDescProd.Text;
                producto.ID_TipoProducto = int.Parse(this.cBoxTipoProducto.SelectedItem.Value);
                producto.ID_Colectiva = int.Parse(this.cBoxColNuevoProd.SelectedItem.Value);
                producto.ID_ProductoPadre = string.IsNullOrEmpty(this.cBoxProdPadre.SelectedItem.Value) ?
                    -1 : int.Parse(this.cBoxProdPadre.SelectedItem.Value);
                producto.ID_TipoIntegracion = int.Parse(this.cBoxTipoIntegracion.SelectedItem.Value);
                producto.ID_CatDisTarjFisicas = string.IsNullOrEmpty(this.cBoxDisTarj.SelectedItem.Value) ?
                    -1 : int.Parse(this.cBoxDisTarj.SelectedItem.Value);

                int counter = 1;
                foreach (SelectedListItem grupo in this.mcGrupoMA.SelectedItems)
                {
                    producto.GruposMA += (counter < this.mcGrupoMA.SelectedItems.Count) ?
                        grupo.Value + ";" : grupo.Value;
                    counter++;
                }

                producto.ClaveTipoMAExterno = String.IsNullOrEmpty(this.cBoxTiposMAExternos.SelectedItem.Value) ?
                    this.txtClaveTMAExt.Text : this.cBoxTiposMAExternos.SelectedItem.Value;
                producto.DescripcionTipoMAExterno = this.txtDescTMAExt.Text;
                producto.GeneraTipoMACLABE = Convert.ToBoolean(
                    String.IsNullOrEmpty(this.hdnGeneraMACLABE.Value.ToString()) ? 0 :
                    this.hdnGeneraMACLABE.Value);

                pCI.Info("INICIA CreaNuevoProductoEnAutorizador()");
                DataTable dt = LNProducto.CreaNuevoProductoEnAutorizador(producto, this.Usuario, LH_ParabAdminProductos);
                pCI.Info("INICIA CreaNuevoProductoEnAutorizador()");

                string msj = dt.Rows[0]["Mensaje"].ToString();
                string idProductoNuevo = dt.Rows[0]["ID_NuevoProducto"].ToString();

                if (idProductoNuevo == "-1")
                {
                    X.Msg.Alert("Nuevo Producto", msj).Show();
                }
                else
                {
                    PermisosNuevoRegistro(idProductoNuevo, true);

                    this.WdwTipoMAExterno.Hide();
                    this.WdwTipoMACLABE.Hide();
                    this.WdwNuevoProducto.Hide();

                    this.cBoxSubEmisor.Value = this.cBoxColNuevoProd.Value;
                    this.txtProducto.Text = this.txtClaveProd.Text;
                    LlenaGridResultados();

                    X.Msg.Alert("Nuevo Producto", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "Productos.CargaNuevoProducto()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Producto", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Nuevo Producto", "Ocurrió un error al generar el Nuevo Producto").Show();
            }
        }
        
        /// <summary>
        /// Controla el evento clic al botón Aceptar al mensaje informativo
        /// de creación exitosa de producto
        /// </summary>
        [DirectMethod(Namespace = "Productos")]
        public void CargaNuevoProducto()
        {
            RowSelectionModel rsm = GridResultadosProdsParab.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultadosProdsParab.FireEvent("RowClick");
        }

        /// <summary>
        /// Llena el grid de resultados de productos con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            try
            {
                LimpiaSeleccionPrevia();

                log.Info("INICIA ObtieneProductosPorClaveDescOColectiva()");
                DataSet dsProductos = DAOProducto.ObtieneProductosPorClaveDescOColectiva(
                    String.IsNullOrEmpty(this.cBoxSubEmisor.SelectedItem.Value) ? -1 :
                    int.Parse(this.cBoxSubEmisor.SelectedItem.Value),
                    String.IsNullOrEmpty(this.txtProducto.Text) ? null : this.txtProducto.Text, 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                log.Info("TERMINA ObtieneProductosPorClaveDescOColectiva()");

                if (dsProductos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Productos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreProductos.DataSource = dsProductos;
                    StoreProductos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al obtener los Productos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
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
            this.txtProducto.Reset();
            this.StoreProductos.RemoveAll();

            LimpiaVentanasPopUp();

            LimpiaSeleccionPrevia();

            PanelCentralProds.Disabled = true;
        }

        /// <summary>
        /// Restablece los controles de las ventanas emergentes de la página
        /// </summary>
        protected void LimpiaVentanasPopUp()
        {
            LimpiaWdwNuevoProducto();

            LimpiaWdwTipoMAExterno();
        }

        /// <summary>
        /// Restablece los controles de las ventana de nuevo producto
        /// </summary>
        protected void LimpiaWdwNuevoProducto()
        {
            FormPanelNuevoProd.Reset();

            this.cBoxTipoProducto.Reset();
            this.txtClaveProd.Reset();
            this.txtDescProd.Reset();
            this.cBoxColNuevoProd.Reset();
            this.cBoxTipoIntegracion.Reset();
            this.chkAdicional.Reset();

            StoreProdPadre.RemoveAll();
            this.cBoxProdPadre.Reset();
        }

        /// <summary>
        /// Restablece los controles de las ventana de tipos de medios de acceso externos
        /// </summary>
        protected void LimpiaWdwTipoMAExterno()
        {
            this.cBoxTiposMAExternos.Reset();
            this.StoreTipoMAExterno.RemoveAll();

            this.txtClaveTMAExt.Reset();
            this.txtDescTMAExt.Reset();
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
        /// un producto en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            this.FormPanelInfoAd_Prod.Reset();
            this.StoreTiposCuenta.RemoveAll();

            this.StoreEventos.RemoveAll();
            this.StoreScripts.RemoveAll();

            this.FormPanelNuevoBin.Reset();
            this.StoreBinesProducto.RemoveAll();

            this.cBoxTipoParametroMA.Reset();
            this.StoreValoresParametros.RemoveAll();

            this.FormPanelNuevoSubproducto.Reset();
            this.StoreSubproductos.RemoveAll();

            this.StoreCampanyas.RemoveAll();
            this.StoreConfigCamp.RemoveAll();

            this.FormPanelInfoAd_Prod.Show();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultadosPP_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdProducto = 0;
                int ID_TipoProducto = 0;
                string ClaveProducto = "", NombreProducto = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] producto = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in producto[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Producto": IdProducto = int.Parse(column.Value); break;
                        case "Clave": ClaveProducto = column.Value; break;
                        case "Descripcion": NombreProducto = column.Value; break;
                        case "ID_TipoProducto": ID_TipoProducto = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnIdProducto.Value = IdProducto;

                PanelCentralProds.Disabled = true;
                PanelCentralProds.Title = ClaveProducto +  " - " + NombreProducto;
                PanelCentralProds.Disabled = false;

                EstableceStoreProductosTitulares(ID_TipoProducto);
                LlenaFormPanelInfoAd(IdProducto);
                LlenaGridTiposCuenta(IdProducto);

                LlenaGridEventos(IdProducto);
                LlenaGridBines(IdProducto);
                LlenaGridSubproductos(IdProducto);
                LlenaGridCampanyas(IdProducto);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminProductos);
                unLog.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al obtener la información del Producto").Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        protected void LlenaFormPanelInfoAd(int IdProducto)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                char[] separador = { ';' };

                logPCI.Info("INICIA ObtieneInfoAdicionalProducto()");
                Producto elProducto = DAOProducto.ObtieneInfoAdicionalProducto(IdProducto, LH_ParabAdminProductos);
                logPCI.Info("TERMINA ObtieneInfoAdicionalProducto()");

                logPCI.Info("INICIA ListaTiposProducto()");
                this.StoreTipoProducto.DataSource =
                    DAOProducto.ListaTiposProducto(Convert.ToInt32(elProducto.ID_Colectiva), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                logPCI.Info("TERMINA ListaTiposProducto()");
                this.StoreTipoProducto.DataBind();

                this.cBoxColectiva.Value = elProducto.ID_Colectiva;
                this.cBoxTipoProd.Value = elProducto.ID_TipoProducto;
                this.txtClaveProducto.Text = elProducto.Clave;
                this.txtDescripcionProd.Text = elProducto.Descripcion;
                this.cBoxTipoIntegProd.Value = elProducto.ID_TipoIntegracion;

                this.mcGrupoMAProd.SelectedItems.Clear();
                String[] grupos = elProducto.GruposMA.Split(separador);
                foreach (String grupo in grupos)
                {
                    this.mcGrupoMAProd.SelectedItems.Add(new SelectedListItem(grupo));
                    this.mcGrupoMAProd.UpdateSelection();
                }

                if (elProducto.ID_ProductoPadre != (int?)null)
                {
                    this.chkBoxEsAdicional.Checked = true;
                    this.cBoxProductoPadre.Disabled = false;
                    this.cBoxProductoPadre.Value = elProducto.ID_ProductoPadre;
                }
                else
                {
                    this.chkBoxEsAdicional.Checked = false;
                    this.cBoxProductoPadre.Disabled = true;
                    this.cBoxProductoPadre.Clear();
                }
                   
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Información Adicional", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Información Adicional", "Ocurrió un error al obtener la Información Adicional del Producto").Show();
            }
        }

        /// <summary>
        /// Llena el grid de tipos de cuenta con la información consultada a base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        protected void LlenaGridTiposCuenta(int IdProducto)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                logPCI.Info("INICIA ObtieneTiposCuentaProducto()");
                this.StoreTiposCuenta.DataSource = 
                    DAOProducto.ObtieneTiposCuentaProducto(IdProducto, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                logPCI.Info("INICIA ObtieneTiposCuentaProducto()");

                this.StoreTiposCuenta.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Tipos de Cuenta", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Tipos de Cuenta", "Ocurrió un error al establecer los Tipos de Cuenta del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Información Adicional, llamando
        /// a la actualización de datos del producto en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAd_Prod_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            try
            {
                Producto unProducto = new Producto();
                int idProdSeleccionado = int.Parse(this.hdnIdProducto.Text);
                int idProdPadre = String.IsNullOrEmpty(this.cBoxProductoPadre.SelectedItem.Value) ?
                    -1 : int.Parse(this.cBoxProductoPadre.SelectedItem.Value);

                if (idProdSeleccionado == idProdPadre)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Actualización de Producto",
                        Message = "El Producto Titular no puede ser el mismo Producto que estás editando.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR
                    });
                    return;
                }

                else
                {
                    unProducto.ID_Producto = idProdSeleccionado;
                    unProducto.Descripcion = this.txtDescripcionProd.Text;
                    unProducto.ID_TipoProducto = int.Parse(this.cBoxTipoProd.SelectedItem.Value);
                    unProducto.ID_Colectiva = int.Parse(this.cBoxColectiva.SelectedItem.Value);
                    unProducto.ID_ProductoPadre = idProdPadre;
                    unProducto.ID_TipoIntegracion = int.Parse(this.cBoxTipoIntegProd.SelectedItem.Value);

                    int counter = 1;
                    foreach (SelectedListItem grupo in this.mcGrupoMAProd.SelectedItems)
                    {
                        unProducto.GruposMA += (counter < this.mcGrupoMAProd.SelectedItems.Count) ?
                            grupo.Value + ";" : grupo.Value;
                        counter++;
                    }

                    log.Info("INICIA ModificaProducto()");
                    string msjResp = LNProducto.ModificaProducto(unProducto, this.Usuario, LH_ParabAdminProductos);
                    log.Info("TERMINA ModificaProducto()");

                    if (!msjResp.Contains("OK"))
                    {
                        X.Msg.Alert("Actualización de Producto", msjResp).Show();
                    }
                    else
                    {
                        X.Msg.Notify("", "Producto Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridResultados();
                    }
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Producto", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Actualización de Producto", "Ocurrió un error al modificar el Producto").Show();
            }
        }

        /// <summary>
        /// Llena el grid de eventos con la información consultada a base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        protected void LlenaGridEventos(int IdProducto)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                pCI.Info("INICIA ObtieneEventosDeProducto()");
                this.StoreEventos.DataSource =
                    DAOProducto.ObtieneEventosDeProducto(IdProducto, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                pCI.Info("TERMINA ObtieneEventosDeProducto()");

                this.StoreEventos.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Eventos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Eventos", "Ocurrió un eror al obtener los Eventos del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de Eventos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoEventos(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                int IdEvento = Convert.ToInt32(e.ExtraParams["ID_Evento"]);

                switch (comando)
                {
                    case "Select":
                        unLog.Info("INICIA ObtieneDatosEvento()");
                        DataTable dtEv = DAOProducto.ObtieneDatosEvento(IdEvento, LH_ParabAdminProductos);
                        unLog.Info("TERMINA ObtieneDatosEvento()");

                        this.hdnIdEvento.Value = IdEvento;
                        this.GridScripts.Title = "Detalle del Evento " + dtEv.Rows[0]["ClaveEvento"].ToString() +
                            " - " + dtEv.Rows[0]["TipoEvento"].ToString() + " - " + 
                            dtEv.Rows[0]["DescripcionEdoCta"].ToString() + " - " + 
                            dtEv.Rows[0]["Descripcion"].ToString();

                        unLog.Info("INICIA ObtieneDatosScriptsEvento()");
                        this.StoreScripts.DataSource = 
                            DAOProducto.ObtieneDatosScriptsEvento(IdEvento, LH_ParabAdminProductos);
                        unLog.Info("TERMINA ObtieneDatosScriptsEvento()");

                        this.StoreScripts.DataBind();
                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Eventos", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Eventos", "Ocurrió un error al establecer los detalles del Evento").Show();
            }
        }

        /// <summary>
        /// Llena el grid de bines con la información consultada a base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        protected void LlenaGridBines(int IdProducto)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                unLog.Info("INICIA ObtieneBinesDeProducto()");
                this.StoreBinesProducto.DataSource =
                    DAOProducto.ObtieneBinesDeProducto(IdProducto, LH_ParabAdminProductos);
                unLog.Info("TERMINA ObtieneBinesDeProducto()");

                this.StoreBinesProducto.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Bines", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Bines", "Ocurrió un error al establecer los Bines del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Bin de la pestaña de Bines,
        /// llamando a la inserción del nuevo bin en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddBin_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminProductos);

            try
            {
                int idProd = Convert.ToInt32(this.hdnIdProducto.Value);

                log.Info("INICIA CreaNuevoBinDelProducto()");
                DataTable dt = LNProducto.CreaNuevoBinDelProducto(idProd, this.txtClaveNuevoBin.Text,
                    this.txtDescNuevoBin.Text, this.Usuario, LH_ParabAdminProductos);
                log.Info("TERMINA CreaNuevoBinDelProducto()");

                string msj = dt.Rows[0]["Mensaje"].ToString();
                string idBINNuevo = dt.Rows[0]["ID_NuevoBin"].ToString();

                if (idBINNuevo == "-1")
                {
                    X.Msg.Alert("Nuevo Bin", msj).Show();
                }
                else
                {
                    X.Msg.Alert("Nuevo Bin", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ").Show();

                    FormPanelNuevoBin.Reset();

                    LlenaGridBines(idProd);
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Bin", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Nuevo Bin", "Ocurrió un error al generar el BIN del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de Bines
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoBines(object sender, DirectEventArgs e)
        {
            try
            {
                char[] charsToTrim = { '*', '"', ' ' };
                String comando = (String)e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Edit":
                        this.hdnIdBin.Value = Convert.ToInt32(e.ExtraParams["ID_BIN"]);
                        this.txtClaveBin.Value = e.ExtraParams["BIN"].Trim(charsToTrim);
                        this.txtDescBin.Text = e.ExtraParams["Descripcion"].Trim(charsToTrim);

                        WdwEditarBIN.Show();
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabAdminProductos);
                pCI.ErrorException(ex);
                X.Msg.Alert("Editar BIN", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar de la ventana de edición de bin,
        /// llamando a la actualización del bin en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarEdicionBin_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                unLog.Info("INICIA ModificaBIN()");
                string resp = LNProducto.ModificaBIN(Convert.ToInt32(this.hdnIdBin.Value),
                    this.txtClaveBin.Text, this.txtDescBin.Text, this.Usuario, LH_ParabAdminProductos);
                unLog.Info("TERMINA ModificaBIN()");

                if (!resp.Contains("OK"))
                {
                    X.Msg.Alert("Actualización de Bin", resp).Show();
                }
                else
                {
                    X.Msg.Notify("", "Bin Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    
                    LlenaGridBines(Convert.ToInt32(this.hdnIdProducto.Value));
                    WdwEditarBIN.Hide();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Bin", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Actualización de Bin", "Ocurrió un error al actualizar los datos del Bin").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de tipo parámetro multiasignación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaTipoParamMA(object sender, EventArgs e)
        {
            LlenaParametrosProducto();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros del producto,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosProducto()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                int IdProducto = Convert.ToInt32(this.hdnIdProducto.Value);

                unLog.Info("INICIA ListaParametrosMAPorTipo()");
                DataSet dsParams = DAOParametroMA.ListaParametrosMAPorTipo(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdProducto, LH_ParabAdminProductos);
                unLog.Info("TERMINA ListaParametrosMAPorTipo()");

                if (dsParams.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Parámetros", "No existen coincidencias con los datos solicitados").Show();
                    return;
                }

                this.StoreValoresParametros.DataSource = dsParams;
                this.StoreValoresParametros.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Parámetros del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);
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
                        case "ID_Plantilla": unParametro.ID_Plantilla = int.Parse(column.Value); break;
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
                this.hdnIdPlantilla.Value = unParametro.ID_Plantilla;
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;

                this.hdnValorIniPMA.Value = unParametro.ValorInicial;
                this.hdnValorFinPMA.Value = unParametro.ValorFinal;

                switch (comando)
                {
                    case "Edit":
                        LimpiaVentanaParams();

                        if (!string.IsNullOrEmpty(unParametro.TipoValidacion) && unParametro.TipoValidacion.Contains("CAT")) //Clave fija de tipo de validación CATALOGO
                        {
                            EstableceCatalogoPMA(Convert.ToInt64(this.cBoxSubEmisor.SelectedItem.Value),
                                unParametro.ID_Parametro, string.Empty, false);
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

                    default: break;
                }
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
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                ParametroValor elParametro = new ParametroValor();
                
                elParametro.ID_Parametro = Convert.ToInt32(this.hdnIdParametroMA.Value);
                elParametro.ID_Plantilla = Convert.ToInt32(this.hdnIdPlantilla.Value);
                elParametro.ID_ValordelParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);
                elParametro.Valor = string.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                    string.IsNullOrEmpty(this.txtValorParInt.Text) ?
                    string.IsNullOrEmpty(this.txtValorParString.Text) ?
                    string.IsNullOrEmpty(this.cBoxValorPar.SelectedItem.Value) ?
                    this.cBoxCatalogoPMA.SelectedItem.Value : this.cBoxValorPar.SelectedItem.Value :
                    this.txtValorParString.Text : this.txtValorParInt.Text :
                    this.txtValorParFloat.Text;

                pCI.Info("INICIA ModificaValorParametro()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminProductos);
                pCI.Info("TERMINA ModificaValorParametro()");

                this.WdwValorParametro.Hide();

                X.Msg.Notify("Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaParametrosProducto();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al modificar el Parámetro").Show();
            }
        }

        /// <summary>
        /// Llena el grid de subproductos con la información consultada a base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        protected void LlenaGridSubproductos(int IdProducto)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                pCI.Info("INICIA ObtieneSubproductosDeProducto()");
                this.StoreSubproductos.DataSource =
                    DAOProducto.ObtieneSubproductosDeProducto(IdProducto, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                pCI.Info("TERMINA ObtieneSubproductosDeProducto()");

                this.StoreSubproductos.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Subproductos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Subproductos", "Ocurrió un error al obtener los Subproductos del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Subproducto de la pestaña de Subproductos,
        /// llamando a la inserción del nuevo subproducto en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevoSubproducto_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                Plantilla unSubP = new Plantilla();
                int _idProd = Convert.ToInt32(this.hdnIdProducto.Value);

                unSubP.ID_Producto = _idProd;
                unSubP.Clave = this.txtClaveSubproducto.Text;
                unSubP.Descripcion = this.txtDescSubproducto.Text;

                unLog.Info("INICIA CreaNuevoSubproductoDelProducto()");
                DataTable _dt = LNProducto.CreaNuevoSubproductoDelProducto(unSubP, this.Usuario, LH_ParabAdminProductos);
                unLog.Info("TERMINA CreaNuevoSubproductoDelProducto()");

                string msj = _dt.Rows[0]["Mensaje"].ToString();
                string idNuevoSubP = _dt.Rows[0]["ID_NuevoSubproducto"].ToString();

                if (idNuevoSubP == "-1")
                {
                    X.Msg.Alert("Nuevo Subproducto", msj).Show();
                }
                else
                {
                    ///Otorgar los permisos a la recién creada plantilla
                    PermisosNuevoRegistro(idNuevoSubP, false);

                    X.Msg.Alert("Nuevo Subproducto", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ").Show();

                    FormPanelNuevoSubproducto.Reset();

                    LlenaGridSubproductos(_idProd);
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Subproducto", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Nuevo Subproducto", "Ocurrió un error al generar el Subproducto").Show();
            }
        }

        /// <summary>
        /// Otorga los permisos al producto o plantilla recién creado, para el usuario en sesión
        /// </summary>
        /// <param name="NuevoId">Identificador del nuevo registro</param>
        /// <param name="esProducto">Bandera de filtro entre producto o plantilla</param>
        protected void PermisosNuevoRegistro(string NuevoId, bool esProducto)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                unLog.Info("INICIA ObtieneIdFiltro()");
                Guid idTabla = DAOFiltro.ObtieneIdFiltro(Guid.Parse(
                    ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    esProducto ? "SPFiltroProductos" : "SPFiltroPlantillas", LH_ParabAdminProductos).Valor.Trim(),
                    LH_ParabAdminProductos);
                unLog.Info("TERMINA ObtieneIdFiltro()");

                //Se crean los permisos para el usuario
                unLog.Info("INICIA CreaPermisoTableValue()");
                LNUsuarios.CreaPermisoTableValue(this.Usuario.UsuarioId, idTabla, NuevoId,
                    true, LH_ParabAdminProductos, this.Usuario);
                unLog.Info("TERMINA CreaPermisoTableValue()");

                //Se otrogan el permiso inmediato al usuario en sesión
                unLog.Info("INICIA AgregarFiltrosEnSesion()");
                LNFiltro.AgregarFiltrosEnSesion(this.Usuario, LH_ParabAdminProductos,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                unLog.Info("TERMINA AgregarFiltrosEnSesion()");

                //Si se dio de alta un producto, se otorga el permiso al usuario de WS
                //para el grupo de cuentas
                if (esProducto)
                {
                    Guid idAppWS = Guid.Parse(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "WsAppId", LH_ParabAdminProductos).Valor.Trim());

                    unLog.Info("INICIA ObtieneUserIdDeApp()");
                    Guid idUser = DAOFiltro.ObtieneUserIdDeApp(idAppWS, Configuracion.Get(
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "WsParab_Usr", LH_ParabAdminProductos).Valor.Trim(), LH_ParabAdminProductos);
                    unLog.Info("TERMINA ObtieneUserIdDeApp()");

                    unLog.Info("INICIA ObtieneIdFiltro()");
                    Guid _IDTabla = DAOFiltro.ObtieneIdFiltro(idAppWS, Configuracion.Get(
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "SPFiltroGruposCuenta", LH_ParabAdminProductos).Valor.Trim(), LH_ParabAdminProductos);
                    unLog.Info("TERMINA ObtieneIdFiltro()");

                    unLog.Info("INICIA ObtieneIdGrupoCuentas()");
                    int idGpoCtas = DAOProducto.ObtieneIdGrupoCuentas(this.txtClaveProd.Text, LH_ParabAdminProductos);
                    unLog.Info("TERMINA ObtieneIdGrupoCuentas()");

                    //Se crean los permisos para el usuario
                    unLog.Info("INICIA CreaPermisoTableValue()");
                    LNUsuarios.CreaPermisoTableValue(idUser, _IDTabla, idGpoCtas.ToString(), true, LH_ParabAdminProductos, this.Usuario);
                    unLog.Info("TERMINA CreaPermisoTableValue()");
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de Subproductos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoSubProd(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);
            pCI.Info("EjecutarComandoSubProd()");

            try
            {
                char[] charsToTrim = { '*', '"', ' ' };
                String comando = (String)e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Edit":
                        this.hndIdSubP.Value = Convert.ToInt32(e.ExtraParams["ID_Plantilla"]);
                        this.txtClaveEdtSubP.Value = e.ExtraParams["Clave"].Trim(charsToTrim);
                        this.txtDescEdtSubP.Text = e.ExtraParams["Descripcion"].Trim(charsToTrim);

                        WdwEditarSubproducto.Show();
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Subproducto", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar de la ventana de edición de subproducto,
        /// llamando a la actualización del bin en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnEditSubProd_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                Plantilla subproducto = new Plantilla();
                int _idProd = Convert.ToInt32(this.hdnIdProducto.Value);

                subproducto.ID_Plantilla = Convert.ToInt32(this.hndIdSubP.Value);
                subproducto.Clave = this.txtClaveEdtSubP.Text;
                subproducto.Descripcion = this.txtDescEdtSubP.Text;

                pCI.Info("INICIA ModificaSubproducto()");
                string response = LNProducto.ModificaSubproducto(subproducto, this.Usuario, LH_ParabAdminProductos);
                pCI.Info("TERMINA ModificaSubproducto()");

                if (!response.Contains("OK"))
                {
                    X.Msg.Alert("Actualización de Subproducto", response).Show();
                }
                else
                {
                    X.Msg.Notify("", "Subproducto actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    LlenaGridSubproductos(Convert.ToInt32(this.hdnIdProducto.Value));
                    WdwEditarSubproducto.Hide();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Subproducto", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Subproducto", "Ocurrió un error al modificar el Subproducto").Show();
            }
        }

        /// <summary>
        /// Establece los elementos del catálogo de parámetros multiasignación en el control
        /// destinado a ello
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="IdParametro">Identificador del parámetro multiasignación</param>
        /// <param name="ClaveParametro">Clave del parámetro mutltiasignación</param> 
        /// <param name="esNuevoProducto">TRUE si el catálogo se establece para un nuevo producto</param>
        protected void EstableceCatalogoPMA(long IdColectiva, long IdParametro, string ClaveParametro, bool esNuevoProducto)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                unLog.Info("INICIA ListaElementosCatalogoPMA()");
                DataSet dsCatDis = DAOProducto.ListaElementosCatalogoPMA(IdColectiva, IdParametro, ClaveParametro,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminProductos);
                unLog.Info("TERMINA ListaElementosCatalogoPMA()");

                this.StoreCatalogoPMA.DataSource = dsCatDis;
                this.StoreCatalogoPMA.DataBind();

                if (esNuevoProducto)
                {
                    if (dsCatDis.Tables[0].Rows.Count > 0)
                    {
                        this.hdnCatFlag.Value = 1;
                        this.hdnClaveGpoMA.Value = Configuracion.Get(Guid.Parse(
                            ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveGrupoMATarjFis").Valor;
                    }
                    else
                    {
                        this.hdnCatFlag.Value = 0;
                        this.hdnClaveGpoMA.Value = "";
                    }
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Llena el grid de campañas con la información consultada a base de datos
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        protected void LlenaGridCampanyas(int IdProducto)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                pCI.Info("INICIA ObtieneCampanyasDeProducto()");
                this.StoreCampanyas.DataSource =
                    DAOProducto.ObtieneCampanyasDeProducto(IdProducto, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                pCI.Info("TERMINA ObtieneCampanyasDeProducto()");

                this.StoreCampanyas.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Campañas MSI", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Campañas MSI", "Ocurrió un error al obtener las Campañas del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de Campañas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoCampanyas(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);
            pCI.Info("EjecutarComandoCampanyas()");

            try
            {
                int idCampanya = 0;
                string claveCampanya = string.Empty;
                string descCampanya = string.Empty;
                string comando = e.ExtraParams["Comando"].ToString();
                string json = string.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] campanya = JSON.Deserialize<Dictionary<string, string>[]>(json);
                if (campanya == null || campanya.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in campanya[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Campania": idCampanya = int.Parse(column.Value); break;
                        case "Clave": claveCampanya = column.Value; break;
                        case "Descripcion": descCampanya = column.Value; break;
                        case "FechaInicio": this.dfFIniCampanya.Value = this.dfFIniCampanya.SelectedValue = Convert.ToDateTime(column.Value); break;
                        case "FechaFin": dfFFinCampanya.Value = Convert.ToDateTime(column.Value); break;
                        default:
                            break;
                    }
                }
                this.hdnIdCampanya.Value = idCampanya;
                switch (comando)
                {
                    case "Activar":
                    case "Desactivar":
                        pCI.Info("INICIA ModificaEstatusCampanya()");
                        LNProducto.ModificaEstatusCampanya(idCampanya, this.Usuario, LH_ParabAdminProductos);
                        pCI.Info("TERMINA ModificaEstatusCampanya()");

                        X.Msg.Notify("", "Estatus de campaña modificado " + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridCampanyas(int.Parse(this.hdnIdProducto.Text));

                        break;

                    case "Editar":
                        this.txtClaveCamp.Text = claveCampanya;
                        this.txtDescripcionCamp.Text = descCampanya;
                        WdwCampanya.Title = "Editar Campaña - [" + claveCampanya + "] " + descCampanya;
                        WdwCampanya.Icon = Icon.Pencil;

                        WdwCampanya.Show();
                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Campañas", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Campañas", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Nueva Campaña de la pestaña Campañas MSI
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaCampanya_Click(object sender, EventArgs e)
        {
            this.FormPanelEdNewCampanya.Reset();
            this.hdnIdCampanya.Value = 0;
            WdwCampanya.Title = "Nueva Campaña";
            WdwCampanya.Icon = Icon.Add;
            WdwCampanya.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar de la ventana de campañas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarCampanya_Click(object sender, EventArgs e)
        {
            SolicitaAltaCambioCampanya();
        }

        /// <summary>
        /// Establece la solicitud de creación o actualización de la campaña en base de datos
        /// </summary>
        protected void SolicitaAltaCambioCampanya()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                int Id_Producto = int.Parse(this.hdnIdProducto.Text);
                int Id_Campanya = Convert.ToInt32(this.hdnIdCampanya.Value);
                CampanyaMSI unaCampanya = new CampanyaMSI();

                unaCampanya.ID_Campanya = Id_Campanya;
                unaCampanya.Clave = txtClaveCamp.Text;
                unaCampanya.Descripcion = txtDescripcionCamp.Text;
                unaCampanya.FechaInicio = dfFIniCampanya.SelectedDate;
                unaCampanya.FechaFin = dfFFinCampanya.SelectedDate;

                unLog.Info("INICIA CreaOModificaCampanya()");
                LNProducto.CreaOModificaCampanya(Id_Producto, unaCampanya, this.Usuario, LH_ParabAdminProductos);
                unLog.Info("TERMINA CreaOModificaCampanya()");

                string accion = Id_Campanya == 0 ? "creada" : "modificada";
                X.Msg.Notify("", "Campaña " + accion + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridCampanyas(Id_Producto);
                WdwCampanya.Hide();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Campañas", err.Mensaje() + Environment.NewLine + "Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Campañas", "Ocurrió un error al actualizar la campaña").Show();
            }
        }

        /// <summary>
        /// Controla el evento seleccionar del row grid Campañas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaCampanya(object sender, DirectEventArgs e)
        {
            int IdCampanya = Convert.ToInt32(e.ExtraParams["ID_Campania"]);
            this.GridConfigCamp.Title += " - [" + e.ExtraParams["Clave"].ToString() + 
                "] " + e.ExtraParams["Descripcion"].ToString();
            this.hdnIdCampanya.Value = IdCampanya;
            this.StoreConfigCamp.RemoveAll();
            LlenaGridConfigCamp(IdCampanya);
        }

        /// <summary>
        /// Llena el grid de configuración de campaña con la información consultada a base de datos
        /// </summary>
        /// <param name="IdCampanya">Identificador de la campaña</param>
        protected void LlenaGridConfigCamp(int IdCampanya)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminProductos);

            try
            {
                logPCI.Info("INICIA ObtienePromocionesDeCampanya()");
                DataTable dtPromos = DAOProducto.ObtienePromocionesDeCampanya(IdCampanya, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminProductos);
                logPCI.Info("TERMINA ObtienePromocionesDeCampanya()");

                if (dtPromos.Rows.Count == 0)
                {
                    X.Msg.Alert("Configuración de Campaña", "No existen configuraciones para la campaña seleccionada").Show();
                }
                else
                {
                    
                    this.StoreConfigCamp.DataSource = dtPromos;
                    this.StoreConfigCamp.DataBind();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Configuración de Campaña", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Configuración de Campaña", "Ocurrió un error al seleccionar la Campaña").Show();
            }
        }

        /// <summary>
        /// Establece a su estado de carga inicial los controles de la ventana configuración de campaña
        /// </summary>
        protected void LimpiaVentanaCfgCamp()
        {
            this.cBoxPromociones.Reset();
            this.txtMeses.Reset();
            this.txtTasaInteres.Reset();
            this.txtDiferimiento.Reset();
        }

        /// <summary>
        /// Controla el evento Click al botón Nueva Configuración de la pestaña Campañas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaCfgCamp_Click(object sender, EventArgs e)
        {
            LimpiaVentanaCfgCamp();
            WdwCfgCampanya.Title = "Nueva Configuración de Campaña";
            WdwCfgCampanya.Icon = Icon.Add;
            WdwCfgCampanya.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar de la ventana de campañas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarCfgCamp_Click(object sender, EventArgs e)
        {
            try
            {
                string claveMSI = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveMSI").Valor;
                string claveCHPD = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveCHPD").Valor;
                string claveMCI = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveMCI").Valor;

                if (this.hdnClavePromo.Value.ToString() == claveMSI)
                {
                    if (this.txtTasaInteres.Text != "0")
                    {
                        X.Msg.Alert("Configuración de Campaña", "La Tasa de Interés debe ser igual a cero en este tipo de Promoción.").Show();
                        return;
                    }
                    if (this.txtDiferimiento.Text != "0")
                    {
                        X.Msg.Alert("Configuración de Campaña", "El Diferimiento debe ser igual a cero en este tipo de Promoción.").Show();
                        return;
                    }
                }

                if (this.hdnClavePromo.Value.ToString() == claveMCI)
                {
                    if (this.txtTasaInteres.Text == "0")
                    {
                        X.Msg.Alert("Configuración de Campaña", "La Tasa de Interés no puede ser cero en este tipo de Promoción.").Show();
                        return;
                    }
                    if (this.txtDiferimiento.Text != "0")
                    {
                        X.Msg.Alert("Configuración de Campaña", "El Diferimiento debe ser igual a cero en este tipo de Promoción.").Show();
                        return;
                    }
                }

                if (this.hdnClavePromo.Value.ToString() == claveCHPD)
                {
                    if (this.txtTasaInteres.Text != "0")
                    {
                        X.Msg.Alert("Configuración de Campaña", "La Tasa de Interés debe ser igual a cero en este tipo de Promoción.").Show();
                        return;
                    }
                    if (this.txtDiferimiento.Text == "0")
                    {
                        X.Msg.Alert("Configuración de Campaña", "El Diferimiento no puede ser cero en este tipo de Promoción.").Show();
                        return;
                    }
                }

                SolicitaAltaCambioConfigCamp();
            }
            
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(LH_ParabAdminProductos);
                log.Info("btnGuardarCfgCamp_Click()");
                log.ErrorException(ex);
                X.Msg.Alert("Configuración de Campaña", "Ocurrió un error en las validaciones de promoción.").Show();
            }
        }

        /// <summary>
        /// Establece la solicitud de creación o actualización de una configuración de campaña en base de datos
        /// </summary>
        protected void SolicitaAltaCambioConfigCamp()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminProductos);

            try
            {
                int Id_Campanya = Convert.ToInt32(this.hdnIdCampanya.Value);
                int Id_Promocion = Convert.ToInt32(this.cBoxPromociones.SelectedItem.Value);
                PromocionMSI unaPromocion = new PromocionMSI();

                unaPromocion.ID_CampanyaPromocion = int.Parse(this.hdnIdCampCfg.Text);
                unaPromocion.ID_Promocion = Id_Promocion;
                unaPromocion.Meses = Convert.ToInt32(this.txtMeses.Text);
                unaPromocion.TasaInteres = Convert.ToDecimal(this.txtTasaInteres.Text);
                unaPromocion.Diferimiento = Convert.ToInt32(this.txtDiferimiento.Text);

                unLog.Info("INICIA CreaOModificaConfiguracionDeCampanya()");
                LNProducto.CreaOModificaConfiguracionDeCampanya(Id_Campanya, unaPromocion, this.Usuario, LH_ParabAdminProductos);
                unLog.Info("TERMINA CreaOModificaConfiguracionDeCampanya()");

                string accion = Id_Promocion == 0 ? "creada" : "modificada";
                X.Msg.Notify("", "Configuración de Campaña " + accion + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridConfigCamp(Id_Campanya);
                LlenaGridCampanyas(Convert.ToInt32(this.hdnIdProducto.Value));
                this.WdwCfgCampanya.Hide();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Configuración de Campaña", err.Mensaje() + Environment.NewLine + "Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Configuración de Campaña", "Ocurrió un error al actualizar la configuración").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de configuración de campañas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoCfgCamp(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminProductos);
            pCI.Info("EjecutarComandoCfgCamp()");

            try
            {
                int idCfgCamp = 0, idPromocion = 0;
                string clavePromo = string.Empty;
                string descPromo = string.Empty;
                string comando = e.ExtraParams["Comando"].ToString();
                string json = string.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] configCamp = JSON.Deserialize<Dictionary<string, string>[]>(json);
                if (configCamp == null || configCamp.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in configCamp[0])
                {
                    switch (column.Key)
                    {
                        case "ID_CampaniaConfiguracion": idCfgCamp = int.Parse(column.Value); break;
                        case "ID_Promocion": idPromocion = int.Parse(column.Value); break;
                        case "Clave": clavePromo = column.Value; break;
                        case "Descripcion": descPromo = column.Value; break;
                        case "Meses": this.txtMeses.Value = Convert.ToInt32(column.Value); break;
                        case "TasaInteres": this.txtTasaInteres.Value = Convert.ToDecimal(column.Value); break;
                        case "Diferimiento": this.txtDiferimiento.Value = Convert.ToInt32(column.Value); break;
                        default:
                            break;
                    }
                }

                this.hdnIdCampCfg.Value = idCfgCamp;
                this.hdnIdPromocion.Value = idPromocion;

                switch (comando)
                {
                    case "Eliminar":
                        pCI.Info("INICIA DesactivaConfiguracionDeCampanya()");
                        LNProducto.DesactivaConfiguracionDeCampanya(idCfgCamp, this.Usuario, LH_ParabAdminProductos);
                        pCI.Info("TERMINA DesactivaConfiguracionDeCampanya()");

                        X.Msg.Notify("", "Configuración de campaña eliminada " + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        this.StoreConfigCamp.RemoveAll();
                        LlenaGridConfigCamp(int.Parse(this.hdnIdCampanya.Text));
                        LlenaGridCampanyas(Convert.ToInt32(this.hdnIdProducto.Value));

                        break;

                    case "Editar":
                        this.cBoxPromociones.Value = idPromocion;
                        this.hdnClavePromo.Value = clavePromo;
                        this.WdwCfgCampanya.Title = "Editar - Promoción [" + clavePromo + "] " + descPromo;
                        this.WdwCfgCampanya.Icon = Icon.Pencil;

                        this.WdwCfgCampanya.Show();
                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Configuración de Campaña", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Configuración de Campaña", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

    }
}
