using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace Administracion
{
    public partial class ConsultarProducto : DALCentralAplicaciones.PaginaBaseCAPP
    {
        List<Producto> productos = new List<Producto>();
        AccordionLayout acc = new AccordionLayout();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    initComponents();

                    if (!X.IsAjaxRequest)
                    {
                        acc.Animate = true;
                        CrearMenuGMA();
                    }
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Crea el menú de árbol para los Grupos de Medios de Acceso
        /// </summary>
        protected void CrearMenuGMA()
        {
            string fn = "e.stopEvent(); checkNode(node)";
            int unNumero = 0;

            foreach (Producto elProducto in GeneraArbolMenu())
            {
                Ext.Net.TreePanel productos = new Ext.Net.TreePanel();

                //(TrP) = Tree Panel
                productos.ID = "TrP" + unNumero++.ToString() + "_" + elProducto.ID_GrupoMA.ToString();
                productos.Title = elProducto.ClaveGrupo + " " + elProducto.Descripcion;
                productos.AutoScroll = false;
                productos.RootVisible = false;
                productos.BodyBorder = false;
                productos.Lines = true;

                Ext.Net.TreeNode root = new Ext.Net.TreeNode();
                root.Expanded = true;
                productos.Root.Add(root);

                foreach (ConfiguracionGMA config in elProducto.Menus.Values)
                {
                    Ext.Net.TreeNode Raiz = new Ext.Net.TreeNode(config.Nombre);
                    //(TrR) = Tree Root
                    Raiz.NodeID = "TrR" + unNumero++.ToString() + "_" + elProducto.ID_GrupoMA.ToString();
                    Raiz.Listeners.Click.Handler = fn;
                    Raiz.Icon = config.Nombre == "Propiedades" ? Icon.Accept : Icon.Wrench;
                    root.Nodes.Add(Raiz);
                    Raiz.Expanded = true;

                    foreach (ConfiguracionGMA menu in config.Ramas.Values)
                    {
                        Ext.Net.TreeNode unMenu = new Ext.Net.TreeNode(menu.Nombre);
                        //(TrM) = Tree Menu
                        unMenu.NodeID = "TrM" + unNumero++.ToString() + "_" + elProducto.ID_GrupoMA.ToString();
                        unMenu.Listeners.Click.Handler = fn;
                        unMenu.Icon = Icon.SectionCollapsed;
                        Raiz.Nodes.Add(unMenu);
                        unMenu.Expanded = true;

                        foreach (ConfiguracionGMA subMenu in menu.Ramas.Values)
                        {
                            Ext.Net.TreeNode unSubMenu = new Ext.Net.TreeNode(subMenu.Nombre);
                            //(TrN) = Tree Node
                            unSubMenu.NodeID = "TrN" + unNumero++.ToString() + "_" + elProducto.ID_GrupoMA.ToString();
                            unSubMenu.Listeners.Click.Handler = fn;
                            unSubMenu.Icon = Icon.Tag;
                            unMenu.Nodes.Add(unSubMenu);
                            unSubMenu.Expanded = true;
                        }
                    }
                }

                acc.Items.Add(productos);
            }

            pArbolMenu.Items.Add(acc);
        }

        /// <summary>
        /// Obtiene la lista de datos de base de datos necesarios para el árbol del menú de Grupos de Medios de Acceso
        /// </summary>
        /// <returns></returns>
        private List<Producto> GeneraArbolMenu()
        {
            List<Producto> productos = new List<DALAdministracion.Entidades.Producto>();

            Dictionary<Int64, Producto> prods = DAOGruposMA.ListaGruposMA(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            productos = prods.Values.ToList();

            return productos;
        }


        /***************************************************************************************
         * Realiza la creación y la inicialización de los componentes de la página
         **************************************************************************************/
        private void initComponents()
        {
            /* Llena el combo para elegir Cadena Comercial */
            LlenarComboCadenaComercial();

            /* Productos */
            CrearGridProductos();

            /* Rangos */
            CrearGridRangos();
        }

        /// <summary>
        /// Llena la lista de Cadenas Comerciales para el cambio de Parámetros Multiasignación
        /// </summary>
        private void LlenarComboCadenaComercial()
        {
            this.StoreCadenaComercial.DataSource = DAOParametroMA.ListaCadenasComerciales(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreCadenaComercial.DataBind();
        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los productos
         **************************************************************************************/
        private void CrearGridProductos()
        {
            GroupingSummaryColumn IDGrupoMA = new GroupingSummaryColumn();
            IDGrupoMA.DataIndex = "ID_GrupoMA";
            IDGrupoMA.Hidden = true;
            this.GridProductos.ColumnModel.Columns.Add(IDGrupoMA);

            GroupingSummaryColumn ClaveGrupoMA = new GroupingSummaryColumn();
            ClaveGrupoMA.DataIndex = "Clave";
            ClaveGrupoMA.Header = "Clave";
            ClaveGrupoMA.Width = 120;
            this.GridProductos.ColumnModel.Columns.Add(ClaveGrupoMA);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Descripción";
            Descripcion.Width = 220;
            this.GridProductos.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn IDVigencia = new GroupingSummaryColumn();
            IDVigencia.DataIndex = "ID_Vigencia";
            IDVigencia.Hidden = true;
            this.GridProductos.ColumnModel.Columns.Add(IDVigencia);

            GroupingSummaryColumn Vigencia = new GroupingSummaryColumn();
            Vigencia.DataIndex = "Vigencia";
            Vigencia.Header = "Vigencia";
            Vigencia.Width = 220;
            this.GridProductos.ColumnModel.Columns.Add(Vigencia);
        }

        /// <summary>
        /// Llena el grid de productos con la información de la base de datos
        /// </summary>
        /// <param name="nodeId">Nombre del nodo del menú árbol seleccionado</param>
        /// <param name="idGMA">Identificador del Grupo del Grupo de Medios de Acceso</param>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarGridProductos(string nodeId, int idGMA)
        {
            if (idGMA == 0)
            {
                String[] subNodes = nodeId.Split('_');
                idGMA = Convert.ToInt16(subNodes[1]);
            }

            this.StoreProductos.DataSource = DAOGruposMA.ConsultaProducto(
                    idGMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );
            this.StoreProductos.DataBind();

            this.GridProductos.Hidden = false;
            this.GridProductos.Show();
        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los rangos
         **************************************************************************************/
        private void CrearGridRangos()
        {
            GroupingSummaryColumn ID_Rango = new GroupingSummaryColumn();
            ID_Rango.DataIndex = "ID_Rango";
            ID_Rango.Hidden = true;
            this.GridRangos.ColumnModel.Columns.Add(ID_Rango);

            GroupingSummaryColumn ID_GrupoMA = new GroupingSummaryColumn();
            ID_GrupoMA.DataIndex = "ID_GrupoMA";
            ID_GrupoMA.Hidden = true;
            this.GridRangos.ColumnModel.Columns.Add(ID_GrupoMA);

            GroupingSummaryColumn Clave = new GroupingSummaryColumn();
            Clave.DataIndex = "Clave";
            Clave.Header = "Clave";
            Clave.Width = 80;
            this.GridRangos.ColumnModel.Columns.Add(Clave);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Descripción";
            this.GridRangos.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn Inicio = new GroupingSummaryColumn();
            Inicio.DataIndex = "Inicio";
            Inicio.Header = "Inicio";
            Inicio.Width = 150;
            this.GridRangos.ColumnModel.Columns.Add(Inicio);

            GroupingSummaryColumn Fin = new GroupingSummaryColumn();
            Fin.DataIndex = "Fin";
            Fin.Header = "Fin";
            Fin.Width = 150;
            this.GridRangos.ColumnModel.Columns.Add(Fin);

            GroupingSummaryColumn esActivo = new GroupingSummaryColumn();
            esActivo.DataIndex = "esActivo";
            esActivo.Header = "Es Activo";
            esActivo.Width = 80;
            this.GridRangos.ColumnModel.Columns.Add(esActivo);

            ImageCommand EditarRango = new ImageCommand();
            EditarRango.Icon = Icon.NoteEdit;
            EditarRango.CommandName = "EditRango";

            this.GridRangos.AutoExpandColumn = "Descripcion";
        }


        /***************************************************************************************
         * Llena la lista de periodos para el registro de una nueva vigencia
         **************************************************************************************/
        private void LlenarComboPeriodos()
        {
            this.StorePeriodos.DataSource = DAOGruposMA.ListaPeriodos(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StorePeriodos.DataBind();
        }

        /***************************************************************************************
         * Llena la lista de tipos de vigencia para el registro de una nueva vigencia
         **************************************************************************************/
        private void LlenarComboTipoVigencias()
        {
            this.StoreTipoVigencia.DataSource = DAOGruposMA.ListaTiposVigencia(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreTipoVigencia.DataBind();
        }

        /// <summary>
        /// Llena el grid de rangos con la información de la base de datos
        /// </summary>
        /// <param name="nodeId">Nombre del nodo del menú árbol seleccionado</param>
        /// <param name="IDGrupoMA">Identificador del Grupo del Grupo de Medios de Acceso</param>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarGridRangos(string nodeId, int IDGrupoMA)
        {
            if (IDGrupoMA == 0)
            {
                String[] subNodes = nodeId.Split('_');
                IDGrupoMA = Convert.ToInt16(subNodes[1]);
            }

            this.StoreRangos.DataSource = DAOGruposMA.ListarRangos(IDGrupoMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreRangos.DataBind();

            this.GridRangos.Hidden = false;
            this.GridRangos.Show();
        }

        /// <summary>
        /// Llena el grid de las reglas de reglas multiasingación
        /// </summary>
        /// <param name="nodeId">Nombre del nodo del menú árbol seleccionado
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarGridReglasMA(string nodeId, int IdGrupoMA)
        {
            DataSet ds = new DataSet();

            if (IdGrupoMA == 0)
            {
                String[] subNodes = nodeId.Split('_');
                IdGrupoMA = Convert.ToInt32(subNodes[1]);
            }

            this.StoreReglasMA.DataSource = ds = DAOReglaMA.ConsultaReglasMA(
                   Convert.ToInt32(cmbCadenaComercial.Value),
                   IdGrupoMA,
                   this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
               );
            this.StoreReglasMA.DataBind();

            this.panelReglasMA.Hidden = false;
            this.panelReglasMA.Show();
        }

        /// <summary>
        /// Limpia el formulario de búsqueda y los stores asociados, y muestra el panel de parámetros multiasignación (PMA)
        /// para la entidad Regla.
        /// </summary>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarFieldSetReglasPMA()
        {
            this.panelReglaPMA.Reset();
            this.txtReglasPMA.Clear();

            this.StoreReglas.RemoveAll();
            this.StorePMA.RemoveAll();

            this.panelReglaPMA.Hidden = false;
            this.panelReglaPMA.Show();
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de reglas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnReglasPMA_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtReglasPMA.Text))
                {
                    StoreReglas.DataSource = DAOReglaMA.ListaCatalogoReglas(
                            txtReglasPMA.Text,
                            this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreReglas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Regla", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Regla", "Ocurrió un Error al Buscar las Reglas").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de reglas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void selectRegla_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] reglaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                int idRegla = 0;

                foreach (KeyValuePair<string, string> regla in reglaSeleccionada[0])
                {
                    switch (regla.Key)
                    {
                        case "ID_Regla": idRegla = int.Parse(regla.Value); break;
                        default:
                            break;
                    }
                }
                String[] subNodes = e.ExtraParams["IdProducto"].Split('_');
                int IdP = int.Parse(subNodes[1]);

                LlenarGridReglasPMA(idRegla, IdP);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Regla", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Regla", "Ocurrió un Error al Seleccionar las Reglas").Show();
            }
        }

        /// <summary>
        /// Llena el grid del nivel regla en parámetros multiasingación
        /// </summary>
        /// <param name="IdRegla">Identificador de la regla seleccionada</param>
        /// <param name="Id_GMA">Identificador del grupo de Medios de Acceso</param>
        public void LlenarGridReglasPMA(int IdRegla, int IdGMA)
        {
            this.StorePMA.DataSource = DAOParametroMA.ConsultaReglasPMA(
                    Convert.ToInt16(cmbCadenaComercial.Value),
                    IdRegla,
                    IdGMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StorePMA.DataBind();
        }

        /// <summary>
        /// Limpia el formulario de búsqueda y los stores asociados, y muestra el panel de parámetros multiasignación (PMA)
        /// para la entidad Tipo de Cuenta.
        /// </summary>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarFieldSetTiposCtaPMA()
        {
            this.panelTipoCuentaPMA.Reset();
            this.txtTiposCtaPMA.Clear();

            this.StoreTiposCta.RemoveAll();
            this.StorePMA.RemoveAll();

            this.panelTipoCuentaPMA.Hidden = false;
            this.panelTipoCuentaPMA.Show();
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de tipos de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnTiposCtaPMA_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTiposCtaPMA.Text))
                {
                    StoreTiposCta.DataSource = DAOTipoCuenta.ListaCatalogoTiposCuenta(
                            txtTiposCtaPMA.Text,
                            this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreTiposCta.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tipos de Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Tipos de Cuenta", "Ocurrió un Error al Buscar los Tipos de Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del Grid de Tipos de Cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void selectTipoCta_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] tipoCtaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                int tipoCuenta = 0;

                foreach (KeyValuePair<string, string> tipoCta in tipoCtaSeleccionada[0])
                {
                    switch (tipoCta.Key)
                    {
                        case "ID_TipoCuenta": tipoCuenta = int.Parse(tipoCta.Value); break;
                        default:
                            break;
                    }
                }
                String[] subNodes = e.ExtraParams["IdProducto"].Split('_');
                int IdP = int.Parse(subNodes[1]);

                LlenarGridTipoCuentaPMA(tipoCuenta, IdP);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tipos de Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Tipos de Cuenta", "Ocurrió un Error al Seleccionar los Tipos de Cuenta").Show();
            }
        }

        /// <summary>
        /// Llena el grid del nivel tipo de cuenta en parámetros multiasingación
        /// </summary>
        /// <param name="IdTipoCta">Identificador del tipo de cuenta</param>
        /// <param name="Id_GMA">Identificador del grupo de Medios de Acceso</param>
        protected void LlenarGridTipoCuentaPMA(int IdTipoCta, int IdGMA)
        {
            this.StorePMA.DataSource = DAOParametroMA.ConsultaTiposCuentaPMA(
                    Convert.ToInt32(cmbCadenaComercial.Value),
                    IdTipoCta,
                    IdGMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );
            this.StorePMA.DataBind();
        }

        /// <summary>
        /// Limpia el formulario de búsqueda y los stores asociados, y muestra el panel de parámetros multiasignación (PMA)
        /// para la entidad Grupo de Cuenta.
        /// </summary>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarFieldSetGposCuentaPMA()
        {
            this.panelGpoCuentaPMA.Reset();
            this.txtGrupoCuenta.Clear();

            this.StoreGpoCuenta.RemoveAll();
            this.StorePMA.RemoveAll();

            this.panelGpoCuentaPMA.Hidden = false;
            this.panelGpoCuentaPMA.Show();
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de grupo de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnGpoCuentaPMA_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtGrupoCuenta.Text))
                {
                    StoreGpoCuenta.DataSource = DAOGrupoCuenta.ListaCatalogoGruposCuenta(
                            txtGrupoCuenta.Text,
                            this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreGpoCuenta.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupo de Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Grupo de Cuenta", "Ocurrió un Error al Buscar los Grupos de Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid grupo de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void selectGpoCuenta_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] gpoCuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                int idGrupoCuenta = 0;

                foreach (KeyValuePair<string, string> gpoCuenta in gpoCuentaSeleccionada[0])
                {
                    switch (gpoCuenta.Key)
                    {
                        case "ID_GrupoCuenta": idGrupoCuenta = int.Parse(gpoCuenta.Value); break;
                        default:
                            break;
                    }
                }
                String[] subNodes = e.ExtraParams["IdProducto"].Split('_');
                int IdP = int.Parse(subNodes[1]);

                LlenarGridGpoCuentaPMA(idGrupoCuenta, IdP);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Grupo de Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Grupo de Cuenta", "Ocurrió un Error al Seleccionar los Grupos de Cuenta").Show();
            }
        }

        /// <summary>
        /// Llena el grid del nivel grupo de cuenta en parámetros multiasingación
        /// </summary>
        /// <param name="IdGpoCuenta">Identificador del grupo de cuenta</param>
        /// <param name="Id_GMA">Identificador del grupo de Medios de Acceso</param>
        public void LlenarGridGpoCuentaPMA(int IdGpoCuenta, int IdGMA)
        {
            this.StorePMA.DataSource = DAOParametroMA.ConsultaGrupoCuentaPMA(
                    Convert.ToInt32(cmbCadenaComercial.Value),
                    IdGpoCuenta,
                    IdGMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );
            this.StorePMA.DataBind();
        }

        /// <summary>
        /// Llena el grid del nivel grupo de tarjeta en parámetros multiasingación
        /// </summary>
        /// <param name="nodeId">Nombre del nodo del menú árbol seleccionado
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarGridGpoTarjetaPMA(string nodeId)
        {
            String[] subNodes = nodeId.Split('_');

            Int16 IdGrupoMA = Convert.ToInt16(subNodes[1]);

            this.StorePMA.DataSource = DAOParametroMA.ConsultaGrupoTarjetaPMA(
                    Convert.ToInt16(cmbCadenaComercial.Value),
                    IdGrupoMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StorePMA.DataBind();

            this.panelGpoTarjetaPMA.Hidden = false;
            this.panelGpoTarjetaPMA.Show();
        }

        /// <summary>
        /// Limpia el formulario de búsqueda y los stores asociados, y muestra el panel de parámetros multiasignación (PMA)
        /// para la entidad Tarjeta/Cuenta.
        /// </summary>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarFieldSetTarjetaCuentaPMA()
        {
            this.panelTarjetaCtaPMA.Reset();
            this.nfNumTarjeta.Clear();
            this.txtApPaterno.Clear();
            this.txtApMaterno.Clear();
            this.txtNombre.Clear();

            this.StoreTarjetaCta.RemoveAll();
            this.StorePMA.RemoveAll();

            this.panelTarjetaCtaPMA.Hidden = false;
            this.panelTarjetaCtaPMA.Show();
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de tarjeta/cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnTarjetaCta_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(nfNumTarjeta.Text) ||
                    !String.IsNullOrEmpty(txtApPaterno.Text) ||
                    !String.IsNullOrEmpty(txtApMaterno.Text) ||
                    !String.IsNullOrEmpty(txtNombre.Text))
                {
                    StoreTarjetaCta.DataSource = DAOTarjetaCuenta.ListaCatalogoTarjetaCuenta(
                            nfNumTarjeta.Text, txtNombre.Text,
                            txtApPaterno.Text, txtApMaterno.Text,
                            this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreTarjetaCta.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjeta/Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Tarjeta/Cuenta", "Ocurrió un Error al Buscar las Tarjetas/Cuentas").Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid tarjeta/cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void selectTarjetaCta_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] tarjetaCtaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                int idCuenta = 0;

                foreach (KeyValuePair<string, string> tarjetaCta in tarjetaCtaSeleccionada[0])
                {
                    switch (tarjetaCta.Key)
                    {
                        case "ID_Cuenta": idCuenta = int.Parse(tarjetaCta.Value); break;
                        default:
                            break;
                    }
                }
                String[] subNodes = e.ExtraParams["IdProducto"].Split('_');
                int IdP = int.Parse(subNodes[1]);

                LlenarGridTarjetaCtaPMA(idCuenta, IdP);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjeta/Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Tarjeta/Cuenta", "Ocurrió un Error al Seleccionar las Tarjetas/Cuentas").Show();
            }
        }

        /// <summary>
        /// Llena el grid del nivel tarjeta/cuenta en parámetros multiasingación
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="Id_GMA">Identificador del grupo de Medios de Acceso</param>
        public void LlenarGridTarjetaCtaPMA(int IdCuenta, int IdGMA)
        {
            this.StorePMA.DataSource = DAOParametroMA.ConsultaTarjetaCuentaPMA(
                    Convert.ToInt16(cmbCadenaComercial.Value),
                    IdCuenta,
                    IdGMA,
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );
            this.StorePMA.DataBind();
        }

        /***********************************************************************************************************************
         * Obtiene el catálogo de tipos de elementos que se pueden comparar
         * 
         **********************************************************************************************************************/
        private void obtenerTiposDeElementosComparar()
        {
            this.StoreGMATipoElementoComparar.DataSource = DAOCatalogos.ListaTipoElementoComparar(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreGMATipoElementoComparar.DataBind();
        }

        /// <summary>
        /// Llena el grid de Validaciones Multiasignación
        /// </summary>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenaGridVMA(string IdCadena)
        {
            this.cmbCadenaComercial.Value = IdCadena;
            int ordenValidacion = 0;

            DataSet validaciones = LNValidaciones.ObtenerVMA(IdCadena, this.Usuario);

            Ext.Net.TreeNode root = this.GridVMA.Root.Primary as Ext.Net.TreeNode;

            Ext.Net.TreeNodeCollection nodes = root.Nodes;
            nodes.Clear();

            IDictionary<long, NodosTrueFalse> listasubNodos = new Dictionary<long, NodosTrueFalse>();

            foreach (DataRow validacionRow in validaciones.Tables[0].Rows)
            {
                ValidacionMA laValidacion = new ValidacionMA();
                laValidacion.ID_ValidadorMultiasignacion = Convert.ToInt64(validacionRow["ID_ValidadorMultiasignacion"].ToString());
                laValidacion.esValidacionBase = Convert.ToBoolean(validacionRow["esValidacionBase"].ToString());
                laValidacion.Nombre = validacionRow["Nombre"].ToString();
                laValidacion.Formula = validacionRow["Formula"].ToString();
                laValidacion.ClaveTipoElemento = validacionRow["TipoValidacion"].ToString();
                laValidacion.Declinar = Convert.ToBoolean(validacionRow["Declinar"].ToString());
                laValidacion.Estatus = Convert.ToBoolean(validacionRow["EsActiva"].ToString());
                laValidacion.PreRegla = Convert.ToBoolean(validacionRow["PreRegla"].ToString());
                laValidacion.PostRegla = Convert.ToBoolean(validacionRow["PostRegla"].ToString());
                laValidacion.Vigencia = validacionRow["Vigencia"].ToString();
                laValidacion.Prioridad = Convert.ToInt32(validacionRow["Prioridad"].ToString());

                string strOrdenValidacion = validacionRow["OrdenValidacion"].ToString();
                laValidacion.OrdenValidacion = String.IsNullOrEmpty(strOrdenValidacion) ? 0 : Convert.ToInt32(strOrdenValidacion);

                string idValidacion = validacionRow["ID_ValidacionTrue"].ToString().ToLower();
                laValidacion.ID_ValidacionTrue = String.IsNullOrEmpty(idValidacion) ? 0L : Convert.ToInt64(idValidacion);

                idValidacion = validacionRow["ID_ValidacionFalse"].ToString().ToLower();
                laValidacion.ID_ValidacionFalse = String.IsNullOrEmpty(idValidacion) ? 0L : Convert.ToInt64(idValidacion);

                Ext.Net.TreeNode validacionBase = crearValidacion(laValidacion);

                if (laValidacion.ID_ValidacionTrue != 0)
                {
                    NodosTrueFalse subnodo = new NodosTrueFalse();
                    subnodo.Base = validacionBase;
                    subnodo.tipo = true;

                    listasubNodos.Add(laValidacion.ID_ValidacionTrue, subnodo);
                }

                if (laValidacion.ID_ValidacionFalse != 0)
                {
                    NodosTrueFalse subnodo = new NodosTrueFalse();
                    subnodo.Base = validacionBase;
                    subnodo.tipo = false;

                    listasubNodos.Add(laValidacion.ID_ValidacionFalse, subnodo);
                }

                if (laValidacion.esValidacionBase)
                {
                    validacionBase.Icon = Icon.FolderGo;
                    validacionBase.Leaf = false;
                    nodes.Add(validacionBase);
                    ordenValidacion++;
                }
                else
                {
                    NodosTrueFalse subnodo;
                    validacionBase.Leaf = false;

                    listasubNodos.TryGetValue(laValidacion.ID_ValidadorMultiasignacion, out subnodo);
                    if (subnodo != null)
                    {
                        if (subnodo.tipo)
                            validacionBase.Icon = Icon.Accept;
                        else
                            validacionBase.Icon = Icon.Cancel;

                        subnodo.Base.Nodes.Add(validacionBase);
                        listasubNodos.Remove(laValidacion.ID_ValidadorMultiasignacion);
                    }
                }
            }

            generarOrdenValidacion(ordenValidacion);

            this.panelValidaciones.Hidden = false;
            this.panelValidaciones.Show();

            X.Call("refreshTree", new JRawValue(this.GridVMA.ClientID), this.GridVMA.Root.ToJson().ToString());
            this.GridVMA.Hidden = false;
            this.GridVMA.Show();
        }

        /***********************************************************************************************************************
        * Crea un nodo genérico de la lista de validaciones
        * 
        * @param   idEvento        ID del evento al que pertenece la validación
        * @param   datosValidacion Objeto validación con los datos por almacenar
        * 
        * @return  nodo genérico para lista de valdaciones
        * 
        **********************************************************************************************************************/
        private Ext.Net.TreeNode crearValidacion(ValidacionMA datosValidacion)
        {
            Ext.Net.TreeNode node = new Ext.Net.TreeNode();
            node.Text = datosValidacion.Nombre;
            node.NodeID = Convert.ToString(datosValidacion.ID_ValidadorMultiasignacion);
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("ID_ValidadorMultiasignacion", node.NodeID, Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("Validacion", node.Text, Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("Formula", datosValidacion.Formula, Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("TipoValidacion", datosValidacion.ClaveTipoElemento, Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("NodosTrue", datosValidacion.ID_ValidacionTrue.ToString(), Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("NodosFalse", datosValidacion.ID_ValidacionFalse.ToString(), Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("Declinar", datosValidacion.Declinar ? "Declinar" : "No Declinar", Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("Estatus", datosValidacion.Estatus ? "Activa" : "Inactiva", Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("OrdenValidacion", datosValidacion.OrdenValidacion.ToString(), Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("PreReglas", datosValidacion.PreRegla.ToString(), Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("PostReglas", datosValidacion.PostRegla.ToString(), Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("Vigencia", datosValidacion.Vigencia.ToString(), Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("Prioridad", datosValidacion.Prioridad.ToString(), Ext.Net.ParameterMode.Value));

            node.Expanded = true;
            return node;
        }

        /***********************************************************************************************************************
        * Genera las opciones válidas para el campo orden de validación
        * 
        * @param nValidaciones Número de validaciones existentes
        * 
        **********************************************************************************************************************/
        private void generarOrdenValidacion(int nValidaciones)
        {
            nValidaciones++;

            List<object> list = new List<object>();
            for (int i = 1; i <= nValidaciones; i++)
            {
                list.Add(new { Text = i.ToString(), Value = i });
            }

            this.StoreGMAOrdenValidacion.DataSource = list;
            this.StoreGMAOrdenValidacion.DataBind();
            //this.f_orden.SelectedItem.Value = nValidaciones.ToString();
        }

        /***********************************************************************************************************************
        * Representación lógica de una validación TRUE/FALSE utilizada durante la recontrucción del árbol
        * 
        **********************************************************************************************************************/
        class NodosTrueFalse
        {
            public Ext.Net.TreeNode Base { get; set; }
            public bool tipo { get; set; }
        }
    }
}