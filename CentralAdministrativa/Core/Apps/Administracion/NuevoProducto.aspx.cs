using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Entidades;
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
    public partial class NuevoProducto : DALCentralAplicaciones.PaginaBaseCAPP
    {
        List<DALAdministracion.Entidades.Producto> productos = new List<DALAdministracion.Entidades.Producto>();
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

            foreach (DALAdministracion.Entidades.Producto elProducto in GeneraArbolMenu())
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

            //Ext.Net.ToolTip tooltip = new Ext.Net.ToolTip();
            //tooltip.Target = "={productos.body}";
            //tooltip.Html = "Pruebita";

            //pnlTooltip.ID = "TTApp" + i.ToString();
            //pnlTooltip.Target = "App" + i.ToString() + ".tabEl";


            //tooltip.Delegate = "div.x-tree-node-el";
            //tooltip.TrackMouse = true;
            //tooltip.Listeners.BeforeShow = Fnct

        }

        /// <summary>
        /// Obtiene la lista de datos de base de datos necesarios para el árbol del menú de Grupos de Medios de Acceso
        /// </summary>
        /// <returns></returns>
        private List<DALAdministracion.Entidades.Producto> GeneraArbolMenu()
        {
            List<DALAdministracion.Entidades.Producto> productos = new List<DALAdministracion.Entidades.Producto>();

            Dictionary<Int64, DALAdministracion.Entidades.Producto> prods = DAOGruposMA.ListaGruposMA(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

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
            //CrearFormRegistroRangos();
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

        protected void btnBuscar_Click(object sender, EventArgs e)
        {

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
            ClaveGrupoMA.Width = 100;
            this.GridProductos.ColumnModel.Columns.Add(ClaveGrupoMA);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Descripción";
            Descripcion.Width = 200;
            this.GridProductos.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn IDVigencia = new GroupingSummaryColumn();
            IDVigencia.DataIndex = "ID_Vigencia";
            IDVigencia.Hidden = true;
            this.GridProductos.ColumnModel.Columns.Add(IDVigencia);

            GroupingSummaryColumn Vigencia = new GroupingSummaryColumn();
            Vigencia.DataIndex = "Vigencia";
            Vigencia.Header = "Vigencia";
            Vigencia.Width = 200;
            this.GridProductos.ColumnModel.Columns.Add(Vigencia);

            this.GridProductos.Title = "Propiedades del DALAdministracion.Entidades.Producto";
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

            this.GridRangos.AutoExpandColumn = "Descripcion";
        }

        ///***************************************************************************************
        // * Realiza la creación del formulario para el registro de los rangos
        // **************************************************************************************/
        //private void CrearFormRegistroRangos()
        //{
        //    this.WNuevoRango.Title = "Nuevo Rango";
        //    this.WNuevoRango.Resizable = false;
        //    this.WNuevoRango.Width = 500;
        //    this.WNuevoRango.Height = 260;

        //    TxTRangoClave.FieldLabel = "Clave";
        //    TxTRangoClave.MaxLength = 20;
        //    TxTRangoClave.AnchorHorizontal = "100%";
        //    TxTRangoClave.AllowBlank = false;

        //    TxTRangoDescripcion.FieldLabel = "Descripción";
        //    TxTRangoDescripcion.MaxLength = 50;
        //    TxTRangoDescripcion.AnchorHorizontal = "100%";
        //    TxTRangoDescripcion.AllowBlank = false;

        //    NumRangoInicio.FieldLabel = "Inicio";
        //    NumRangoInicio.AnchorHorizontal = "100%";
        //    NumRangoInicio.MaxLength = 9;
        //    NumRangoInicio.AllowBlank = false;

        //    NumRangoFin.FieldLabel = "Fin";
        //    NumRangoFin.AnchorHorizontal = "100%";
        //    NumRangoFin.MaxLength = 9;
        //    NumRangoFin.AllowBlank = false;

        //    cmbServiceCode1.FieldLabel = "Código de Servicio - D1";
        //    cmbServiceCode1.AnchorHorizontal = "100%";
        //    cmbServiceCode1.AllowBlank = false;
        //    cmbServiceCode1.SelectedItem.Value = "1";
        //    cmbServiceCode1.Items.Add(new Ext.Net.ListItem("1", "1"));
        //    cmbServiceCode1.Items.Add(new Ext.Net.ListItem("2", "2"));
        //    cmbServiceCode1.Items.Add(new Ext.Net.ListItem("5", "5"));
        //    cmbServiceCode1.Items.Add(new Ext.Net.ListItem("6", "6"));
        //    cmbServiceCode1.Items.Add(new Ext.Net.ListItem("7", "7"));
        //    cmbServiceCode1.Items.Add(new Ext.Net.ListItem("9", "9"));

        //    cmbServiceCode2.FieldLabel = "Código de Servicio - D2";
        //    cmbServiceCode2.AnchorHorizontal = "100%";
        //    cmbServiceCode2.AllowBlank = false;
        //    cmbServiceCode2.SelectedItem.Value = "0";
        //    cmbServiceCode2.Items.Add(new Ext.Net.ListItem("0", "0"));
        //    cmbServiceCode2.Items.Add(new Ext.Net.ListItem("2", "2"));
        //    cmbServiceCode2.Items.Add(new Ext.Net.ListItem("4", "4"));

        //    cmbServiceCode3.FieldLabel = "Código de Servicio - D3";
        //    cmbServiceCode3.AnchorHorizontal = "100%";
        //    cmbServiceCode3.AllowBlank = false;
        //    cmbServiceCode3.SelectedItem.Value = "0";
        //    cmbServiceCode3.Items.Add(new Ext.Net.ListItem("1", "1"));
        //    cmbServiceCode3.Items.Add(new Ext.Net.ListItem("3", "3"));
        //    cmbServiceCode3.Items.Add(new Ext.Net.ListItem("4", "4"));
        //    cmbServiceCode3.Items.Add(new Ext.Net.ListItem("5", "5"));
        //    cmbServiceCode3.Items.Add(new Ext.Net.ListItem("6", "6"));
        //    cmbServiceCode3.Items.Add(new Ext.Net.ListItem("7", "7"));

        //    SelRangoEsActivo.FieldLabel = "Es Activo";
        //    SelRangoEsActivo.AnchorHorizontal = "100%";
        //    SelRangoEsActivo.AllowBlank = false;
        //    SelRangoEsActivo.SelectedItem.Value = "1";
        //    SelRangoEsActivo.Items.Add(new Ext.Net.ListItem("SI", "1"));
        //    SelRangoEsActivo.Items.Add(new Ext.Net.ListItem("NO", "0"));
        //}

        /***************************************************************************************
         * Llena la lista de vigencias para el registro de una GrupoMA
         **************************************************************************************/
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarComboVigencias()
        {
            try
            {
                this.StoreVigencias.DataSource = DAOGruposMA.ListaVigencias(this.Usuario);

                this.StoreVigencias.DataBind();
            }
            //Este catch se añadió con el único propósito de recuperar la validación de registros en el catálogo de vigencias
            catch (CAppException)
            {
                this.btnAddVigencia.Hidden = false;
            }
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

            this.panelRangos.Hidden = false;
            this.panelRangos.Show();
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
        /// Valida que se hayan capturado todos los campos de las Reglas Multiasignación (RMA) modificadas.
        /// </summary>
        /// <param name="ReglasMA">Tabla con los datos por validar</param>
        /// <returns>TRUE si se capturaron todos los campos requeridos</returns>
        public bool ValidaCamposRMA(DataTable ReglasMA)
        {
            for (int fila = 0; fila < ReglasMA.Rows.Count; fila++)
            {
                if (String.IsNullOrEmpty(ReglasMA.Rows[fila]["Prioridad"].ToString()))
                    return false;

                if (String.IsNullOrEmpty(ReglasMA.Rows[fila]["ID_Vigencia"].ToString()))
                    return false;

                if (String.IsNullOrEmpty(ReglasMA.Rows[fila]["OrdenEjecucion"].ToString()))
                    return false;
            }

            return true;
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

            this.StoreReglasMA.DataSource = ds = DAOReglaMA.ConsultaTotalReglasMA(
                   Convert.ToInt32(cmbCadenaComercial.Value),
                   IdGrupoMA,
                   this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
               );
            this.StoreReglasMA.DataBind();


            RowSelectionModel rsm = this.GridReglasMA.GetSelectionModel() as RowSelectionModel;

            foreach (DataRow ruleRow in ds.Tables[0].Rows)
            {
                if (Convert.ToInt32(ruleRow["ID_ReglaMultiasignacion"].ToString()) != 0)
                {
                    rsm.SelectedRows.Add(new SelectedRow(ds.Tables[0].Rows.IndexOf(ruleRow)));
                    rsm.UpdateSelection();
                }
            }

            this.panelReglasMA.Hidden = false;
            this.panelReglasMA.Show();
        }

        /// <summary>
        /// Controla el evento de Click al botón de Activar Reglas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnActivaReglas_Click(object sender, DirectEventArgs e)
        {
            try
            {
                String[] subNodes = e.ExtraParams["IdProducto"].Split('_');
                string IdP = subNodes[1];

                string json = e.ExtraParams["Values"];
                Dictionary<string, string>[] reglas = JSON.Deserialize<Dictionary<string, string>[]>(json);

                DataTable dt = new DataTable();
                int fila = 0;

                dt.Columns.Add("ID_Regla");
                dt.Columns.Add("ID_ReglaMultiasignacion");
                dt.Columns.Add("Prioridad");
                dt.Columns.Add("ID_Vigencia");
                dt.Columns.Add("ID_Producto");
                dt.Columns.Add("OrdenEjecucion");

                foreach (Dictionary<string, string> registro in reglas)
                {
                    dt.Rows.Add();

                    foreach (KeyValuePair<string, string> regla in registro)
                    {
                        switch (regla.Key)
                        {
                            case "ID_Regla": dt.Rows[fila]["ID_Regla"] = regla.Value; break;
                            case "ID_ReglaMultiasignacion": dt.Rows[fila]["ID_ReglaMultiasignacion"] = regla.Value; break;
                            case "Prioridad": dt.Rows[fila]["Prioridad"] = regla.Value; break;
                            case "ID_Vigencia": dt.Rows[fila]["ID_Vigencia"] = regla.Value; break;
                            case "OrdenEjecucion": dt.Rows[fila]["OrdenEjecucion"] = regla.Value; break;
                            default:
                                break;
                        }
                    }

                    dt.Rows[fila]["ID_Producto"] = IdP;
                    fila++;
                }

                if (!ValidaCamposRMA(dt))
                {
                    throw new CAppException(8009, "Por favor, ingresa todos los campos de las Reglas que deseas activar");
                }

                LNReglas.CreaModificaRMA(dt, Convert.ToInt32(cmbCadenaComercial.Value), this.Usuario);
                X.Msg.Notify("", "Regla(s) Activada(s) Exitosamente").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Activar Reglas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Activar Reglas", "Ocurrió un Error al Activar la Regla").Show();
            }
        }

        /// <summary>
        /// Controla el cambio de valor de un Parámetro Multiasignación (PMA), para hacer la llamada correspondiente a base de datos.
        /// </summary>
        /// <param name="IdVPMA">Identificador del Valor PMA</param>
        /// <param name="IdPMA">Identificador del PMA</param>
        /// <param name="e">Identificador de Entidad</param>
        /// <param name="tipo">"Tipo" de PMA: Regla, Tipo de Cuenta, etc. </param>
        /// <param name="o">Identificador del Origen o Entidad: Regla, Tipo de Cuenta, etc.</param>
        /// <param name="p">Identificador del DALAdministracion.Entidades.Producto o Grupo de Medios de Acceso</param>
        /// <param name="cc">Identificador de la Cadena Comercial</param>
        /// <param name="campo">Nombre del campo en el grid que se editó (Valor o Vigencia)</param>
        /// <param name="valorAnterior">Valor del PMA anterior al cambio</param>
        /// <param name="valorNuevo">Nuevo valor del PMA</param>
        [DirectMethod(Namespace = "Administracion")]
        public void NuevoValorPMA(int IdVPMA, int IdPMA, int e, string tipo, int o, int p, int cc, int vig, string valor)
        {
            try
            {
                if (vig == 0 || String.IsNullOrEmpty(valor))
                {
                    return;
                }

                ParametroMA nuevoPMA = new ParametroMA();
                nuevoPMA.ID_ValorParametroMultiasignacion = IdVPMA;
                nuevoPMA.ID_ParametroMultiasignacion = IdPMA;

                string gridPMAOrigen = tipo.Substring(16);

                if (e == 0)
                {
                    nuevoPMA.ID_Entidad = gridPMAOrigen == "ReglaPMA" ? 1 :
                        gridPMAOrigen == "TipoCuentaPMA" ? 2 :
                        gridPMAOrigen == "GpoCuentaPMA" ? 3 :
                        gridPMAOrigen == "GpoTarjetaPMA" ? 4 : 5;
                }

                nuevoPMA.ID_Origen = o;
                nuevoPMA.ID_Producto = p;
                nuevoPMA.ID_CadenaComercial = cc;
                nuevoPMA.ID_Vigencia = vig;
                nuevoPMA.ValorPMA = valor;

                LNParametros.CreaModificaPMA(nuevoPMA, this.Usuario);
                X.Msg.Notify("", "Parámetro Activado Exitosamente").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Activar Parámetro", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Activar Parámetro ", "Ocurrió un Error en la Activación").Show();
            }
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
                //string nombreRegla = "";

                foreach (KeyValuePair<string, string> regla in reglaSeleccionada[0])
                {
                    switch (regla.Key)
                    {
                        case "ID_Regla": idRegla = int.Parse(regla.Value); break;
                        //case "Nombre": nombreRegla = regla.Value; break;
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
            this.StorePMA.DataSource = DAOParametroMA.ConsultaTotalReglasPMA(
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
            this.StorePMA.DataSource = DAOParametroMA.ConsultaTotalTiposCuentaPMA(
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
            this.StorePMA.DataSource = DAOParametroMA.ConsultaTotalGrupoCuentaPMA(
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

            this.StorePMA.DataSource = DAOParametroMA.ConsultaTotalGrupoTarjetaPMA(
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
            this.StorePMA.DataSource = DAOParametroMA.ConsultaTotalTarjetaCuentaPMA(
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
        /// Muestra el grid correspondiente a cada entidad
        /// </summary>
        /// <param name="e">Nombre de la entidad</param>
        private void MuestraGridVMAPorEntidad(string e)
        {
            //if (e == "Regla")
            //{
            X.Call("refreshTree", new JRawValue(this.GridVMA.ClientID), this.GridVMA.Root.ToJson().ToString());
            this.GridVMA.Hidden = false;
            this.GridVMA.Show();
            //}
            //else if (e == "Tipo de Cuenta")
            //{
            //    X.Call("refreshTree", new JRawValue(this.GridTipoCtaVMA.ClientID), this.GridTipoCtaVMA.Root.ToJson().ToString());

            //    StoreTiposCta.DataSource = DAOTipoCuenta.ListaCatalogoTiposCuenta(
            //        this.Usuario,
            //        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            //    StoreTiposCta.DataBind();

            //    this.FieldSetTiposCtaVMA.Hidden = false;
            //    this.FieldSetTiposCtaVMA.Show();

            //    this.GridTipoCtaVMA.Hidden = false;
            //    this.GridTipoCtaVMA.Show();
            //}
            //else if (e == "Grupo de Cuenta")
            //{
            //    X.Call("refreshTree", new JRawValue(this.GridGpoCuentaVMA.ClientID), this.GridGpoCuentaVMA.Root.ToJson().ToString());

            //    StoreGruposCta.DataSource = DAOGrupoCuenta.ListaCatalogoGruposCuenta(
            //        this.Usuario,
            //        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            //    StoreGruposCta.DataBind();

            //    this.FieldSetGruposCtaVMA.Hidden = false;
            //    this.FieldSetGruposCtaVMA.Show();

            //    this.GridGpoCuentaVMA.Hidden = false;
            //    this.GridGpoCuentaVMA.Show();
            //}
            //else if (e == "Grupo de Tarjeta")
            //{
            //    X.Call("refreshTree", new JRawValue(this.GridGpoTarjetaVMA.ClientID), this.GridGpoTarjetaVMA.Root.ToJson().ToString());
            //    this.GridGpoTarjetaVMA.Hidden = false;
            //    this.GridGpoTarjetaVMA.Show();
            //}
            //else if (e == "Tarjeta/Cuenta")
            //{
            //    X.Call("refreshTree", new JRawValue(this.GridTarjetaCtaVMA.ClientID), this.GridTarjetaCtaVMA.Root.ToJson().ToString());
            //    this.GridTarjetaCtaVMA.Hidden = false;
            //    this.GridTarjetaCtaVMA.Show();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">Nombre de la entidad</param>
        /// <returns></returns>
        private Ext.Net.TreeNode LlenarGridPorEntidad(string e)
        {
            Ext.Net.TreeNode tree = new Ext.Net.TreeNode();

            //if (e == "Regla")
            //{
            tree = this.GridVMA.Root.Primary as Ext.Net.TreeNode;
            //}
            //else if (e == "Tipo de Cuenta")
            //{
            //    tree = this.GridTipoCtaVMA.Root.Primary as Ext.Net.TreeNode;
            //}
            //else if (e == "Grupo de Cuenta")
            //{
            //    tree = this.GridGpoCuentaVMA.Root.Primary as Ext.Net.TreeNode;
            //}
            //else if (e == "Grupo de Tarjeta")
            //{
            //    tree = this.GridGpoTarjetaVMA.Root.Primary as Ext.Net.TreeNode;
            //}
            //else //if (Entidad == "Tarjeta/Cuenta")
            //{
            //    tree = this.GridTarjetaCtaVMA.Root.Primary as Ext.Net.TreeNode;
            //}

            return tree;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void btnTiposCtaVMA_Click(object sender, EventArgs e)
        //{

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void btnGruposCtaVMA_Click(object sender, EventArgs e)
        //{

        //}

        /// <summary>
        /// Llena el grid correspondiente de Validaciones Multiasignación, según la
        /// entidad seleccionada
        /// </summary>
        [DirectMethod(Namespace = "Administracion")]
        public void LlenaGridVMA(string IdCadena, string Entidad)
        {
            this.cmbCadenaComercial.Value = IdCadena;
            int ordenValidacion = 0;

            DataSet validaciones = LNValidaciones.ObtenerVMA(IdCadena, this.Usuario);
            Ext.Net.TreeNode root = LlenarGridPorEntidad(Entidad);

            Ext.Net.TreeNodeCollection nodes = root.Nodes;
            nodes.Clear();

            IDictionary<long, NodosTrueFalse> listasubNodos = new Dictionary<long, NodosTrueFalse>();

            foreach (DataRow validacionRow in validaciones.Tables[0].Rows)
            {
                ValidacionMA nuevaValidacion = new ValidacionMA();
                nuevaValidacion.ID_ValidadorMultiasignacion = Convert.ToInt64(validacionRow["ID_ValidadorMultiasignacion"].ToString());
                nuevaValidacion.esValidacionBase = Convert.ToBoolean(validacionRow["esValidacionBase"].ToString());
                nuevaValidacion.Nombre = validacionRow["Nombre"].ToString();
                nuevaValidacion.Formula = validacionRow["Formula"].ToString();
                nuevaValidacion.ClaveTipoElemento = validacionRow["TipoValidacion"].ToString();
                nuevaValidacion.Declinar = Convert.ToBoolean(validacionRow["Declinar"].ToString());
                nuevaValidacion.Estatus = Convert.ToBoolean(validacionRow["EsActiva"].ToString());
                nuevaValidacion.PreRegla = Convert.ToBoolean(validacionRow["PreRegla"].ToString());
                nuevaValidacion.PostRegla = Convert.ToBoolean(validacionRow["PostRegla"].ToString());
                nuevaValidacion.Vigencia = validacionRow["Vigencia"].ToString();
                nuevaValidacion.Prioridad = Convert.ToInt32(validacionRow["Prioridad"].ToString());

                string strOrdenValidacion = validacionRow["OrdenValidacion"].ToString();
                nuevaValidacion.OrdenValidacion = String.IsNullOrEmpty(strOrdenValidacion) ? 0 : Convert.ToInt32(strOrdenValidacion);

                string idValidacion = validacionRow["ID_ValidacionTrue"].ToString().ToLower();
                nuevaValidacion.ID_ValidacionTrue = String.IsNullOrEmpty(idValidacion) ? 0L : Convert.ToInt64(idValidacion);

                idValidacion = validacionRow["ID_ValidacionFalse"].ToString().ToLower();
                nuevaValidacion.ID_ValidacionFalse = String.IsNullOrEmpty(idValidacion) ? 0L : Convert.ToInt64(idValidacion);

                Ext.Net.TreeNode validacionBase = crearValidacion(nuevaValidacion);

                if (nuevaValidacion.ID_ValidacionTrue != 0)
                {
                    NodosTrueFalse subnodo = new NodosTrueFalse();
                    subnodo.Base = validacionBase;
                    subnodo.tipo = true;

                    listasubNodos.Add(nuevaValidacion.ID_ValidacionTrue, subnodo);
                }

                if (nuevaValidacion.ID_ValidacionFalse != 0)
                {
                    NodosTrueFalse subnodo = new NodosTrueFalse();
                    subnodo.Base = validacionBase;
                    subnodo.tipo = false;

                    listasubNodos.Add(nuevaValidacion.ID_ValidacionFalse, subnodo);
                }

                if (nuevaValidacion.esValidacionBase)
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

                    listasubNodos.TryGetValue(nuevaValidacion.ID_ValidadorMultiasignacion, out subnodo);
                    if (subnodo != null)
                    {
                        if (subnodo.tipo)
                            validacionBase.Icon = Icon.Accept;
                        else
                            validacionBase.Icon = Icon.Cancel;

                        subnodo.Base.Nodes.Add(validacionBase);
                        listasubNodos.Remove(nuevaValidacion.ID_ValidadorMultiasignacion);
                    }
                }
            }

            generarOrdenValidacion(ordenValidacion);

            this.panelValidaciones.Hidden = false;
            this.panelValidaciones.Show();

            MuestraGridVMAPorEntidad(Entidad);
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
            this.f_orden.SelectedItem.Value = nValidaciones.ToString();
        }

        /***********************************************************************************************************************
         * Agrega una nueva validación al GMA seleccionado
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void crearValidacion_Event(object sender, DirectEventArgs e)
        {
            string idNodeBase = e.ExtraParams["idNodo"];
            string tipo = e.ExtraParams["TipoValidacion"];
            string id_cadena = this.cmbCadenaComercial.Value as string;
            String[] subNodes = e.ExtraParams["idProd"].ToString().Split('_');

            if (!string.IsNullOrEmpty(tipo.Trim()) && !string.IsNullOrEmpty(id_cadena.Trim()))
            {
                tipo = (String.Equals(tipo, "0") || String.Equals(tipo, "1")) ? "1" : tipo;

                ValidacionMA validacionNueva = new ValidacionMA();
                validacionNueva.ID_CadenaComercial = Convert.ToInt64(id_cadena);
                validacionNueva.Nombre = this.f_validacion.Text;
                validacionNueva.Campo = this.f_campo.Text;
                validacionNueva.ClaveTipoElemento = this.f_tipo_elemento.Value.ToString();
                validacionNueva.Formula = this.f_formula.Text;
                validacionNueva.CodigoError = this.f_error.Text;
                validacionNueva.Declinar = Convert.ToBoolean(this.f_declinar.SelectedItem.Value);
                validacionNueva.PreRegla = Convert.ToBoolean(this.f_prereglas.SelectedItem.Value);
                validacionNueva.PostRegla = Convert.ToBoolean(this.f_postreglas.SelectedItem.Value);
                validacionNueva.OrdenValidacion = Convert.ToInt32(this.f_orden.SelectedItem.Value);
                validacionNueva.ID_Vigencia = Convert.ToInt32(this.f_vigencia.SelectedItem.Value);
                validacionNueva.Prioridad = Convert.ToInt32(this.f_prioridad.SelectedItem.Value);
                validacionNueva.ID_Entidad = 0;
                validacionNueva.ID_Producto = Convert.ToInt64(subNodes[1]);

                LNValidaciones.InsertaVMA(validacionNueva, Convert.ToInt64(idNodeBase), Convert.ToInt32(tipo), this.Usuario);
                LlenaGridVMA(id_cadena, "");
            }

            this.nueva_validacion.Reset();
            this.DialogValidacion.Hide();
        }

        /***********************************************************************************************************************
         * Desactiva la validación seleccionada
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void InactivarValidacion_Event(object sender, DirectEventArgs e)
        {
            string idValidacion = e.ExtraParams["idNodo"];
            string id_cadena = this.cmbCadenaComercial.Value as string;
            string Entidad = e.ExtraParams["entidad"];

            if (String.IsNullOrEmpty(idValidacion))
                return;

            ValidacionMA validacion = new ValidacionMA();
            validacion.ID_ValidadorMultiasignacion = Convert.ToInt64(idValidacion);

            LNValidaciones.DesactivaVMA(validacion, this.Usuario);

            LlenaGridVMA(id_cadena, Entidad);
        }

        /***********************************************************************************************************************
         * Activa la validación seleccionada
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void ActivarValidacion_Event(object sender, DirectEventArgs e)
        {
            string idValidacion = e.ExtraParams["idNodo"];
            string id_cadena = this.cmbCadenaComercial.Value as string;
            string Entidad = e.ExtraParams["entidad"];

            if (String.IsNullOrEmpty(idValidacion))
                return;

            ValidacionMA validacion = new ValidacionMA();
            validacion.ID_ValidadorMultiasignacion = Convert.ToInt64(idValidacion);

            LNValidaciones.ActivaVMA(validacion, this.Usuario);

            LlenaGridVMA(id_cadena, Entidad);
        }

        /***********************************************************************************************************************
         * Elimina la validación seleccionada
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void EliminarValidacion_Event(object sender, DirectEventArgs e)
        {
            string idValidacion = e.ExtraParams["idNodo"];
            string id_cadena = this.cmbCadenaComercial.Value as string;
            string Entidad = e.ExtraParams["entidad"];

            if (String.IsNullOrEmpty(idValidacion))
                return;

            ValidacionMA validacion = new ValidacionMA();
            validacion.ID_ValidadorMultiasignacion = Convert.ToInt64(idValidacion);

            LNValidaciones.EliminaVMA(validacion, this.Usuario);

            LlenaGridVMA(id_cadena, Entidad);
        }

        /***********************************************************************************************************************
         * Caambia el estado de la ejecución prereglas de la validación seleccionada
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void CambiarEjecucionPrereglasValidacion_Event(object sender, DirectEventArgs e)
        {
            string idValidacion = e.ExtraParams["idNodo"];
            bool activar = Convert.ToBoolean(e.ExtraParams["activar"]);
            string id_cadena = this.cmbCadenaComercial.Value as string;
            string Entidad = e.ExtraParams["entidad"];

            if (String.IsNullOrEmpty(idValidacion))
                return;

            ValidacionMA validacion = new ValidacionMA();
            validacion.ID_ValidadorMultiasignacion = Convert.ToInt64(idValidacion);

            LNValidaciones.ModificaPreReglasVMA(validacion, activar, this.Usuario);

            LlenaGridVMA(id_cadena, Entidad);
        }

        /***********************************************************************************************************************
         * Caambia el estado de la ejecución postreglas de la validación seleccionada
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void CambiarEjecucionPostReglasValidacion_Event(object sender, DirectEventArgs e)
        {
            string idValidacion = e.ExtraParams["idNodo"];
            bool activar = Convert.ToBoolean(e.ExtraParams["activar"]);
            string id_cadena = this.cmbCadenaComercial.Value as string;
            string Entidad = e.ExtraParams["entidad"];

            if (String.IsNullOrEmpty(idValidacion))
                return;

            ValidacionMA validacion = new ValidacionMA();
            validacion.ID_ValidadorMultiasignacion = Convert.ToInt64(idValidacion);

            LNValidaciones.ModificaPostReglasVMA(validacion, activar, this.Usuario);

            LlenaGridVMA(id_cadena, Entidad);
        }

        /***********************************************************************************************************************
        * Muestra el dialog para el registro de una nueva validación
        * 
        **********************************************************************************************************************/
        [DirectMethod]
        public void showFormNuevaValidacion(int numNodos, String nodo, int tipo)
        {
            if (numNodos > 0 && tipo == 0)
                return;

            bool esValidacionBase = !(tipo == 2 || tipo == 3);
            this.DialogValidacion.Title = "Nueva Validación Base";
            this.DialogValidacion.Icon = Icon.Add;

            Newtonsoft.Json.Linq.JObject node = JSON.Deserialize<Newtonsoft.Json.Linq.JObject>(nodo);

            if (!esValidacionBase)
            {
                string nodosTrueFalse = node.SelectToken(tipo == 2 ? "attributes.NodosTrue" : "attributes.NodosFalse").ToString();
                nodosTrueFalse = nodosTrueFalse.Replace("\"", String.Empty);
                int tieneTrueFalse = Convert.ToInt32(nodosTrueFalse);
                if (tieneTrueFalse > 0)
                {
                    X.Msg.Alert("Acción no Permitida", String.Format("Ya Existe una Validación {0}", tipo == 2 ? "True" : "False")).Show();
                    return;
                }

                this.DialogValidacion.Icon = tipo == 2 ? Icon.Accept : Icon.Cancel;
                this.DialogValidacion.Title = tipo == 2 ? "Nueva Validación True" : "Nueva Validación False";

                this.f_orden.SelectedItem.Value = "1";
            }

            //Orden de validación
            this.f_orden.Disabled = !esValidacionBase;
            this.f_orden.Hidden = !esValidacionBase;

            //Prereglas
            this.f_prereglas.Disabled = !esValidacionBase;
            this.f_prereglas.Hidden = !esValidacionBase;

            //Prereglas
            this.f_postreglas.Disabled = !esValidacionBase;
            this.f_postreglas.Hidden = !esValidacionBase;

            obtenerTiposDeElementosComparar();

            this.DialogValidacion.Height = esValidacionBase ? 470 : 400;
            this.DialogValidacion.Show();
        }

        ///// <summary>
        ///// Llena el grid de los tipos de cuenta de las reglas multiasingación
        ///// </summary>
        ///// <param name="nodeId">Nombre del nodo del menú árbol seleccionado
        //[DirectMethod(Namespace = "Administracion")]
        //public void LlenarGridTipoCtaRMA(string nodeId)
        //{
        //    this.StoreTipoCtaRMA.DataSource = DAOReglaMA.ConsultaTipoCuentaRMA(
        //            Convert.ToInt16(cmbCadenaComercial.Value),
        //            this.Usuario,
        //            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
        //        );
        //    this.StoreTipoCtaRMA.DataBind();

        //    StoreTiposCta.DataSource = DAOTipoCuenta.ListaCatalogoTiposCuenta(
        //            this.Usuario,
        //            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
        //    StoreTiposCta.DataBind();

        //    this.panelTipoCtaRMA.Hidden = false;
        //    this.panelTipoCtaRMA.Show();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void btnTiposCtaRMA_Click(object sender, EventArgs e)
        //{

        //}

        ///// <summary>
        ///// Llena el grid de los grupos de cuenta de las reglas multiasingación
        ///// </summary>
        ///// <param name="nodeId">Nombre del nodo del menú árbol seleccionado
        //[DirectMethod(Namespace = "Administracion")]
        //public void LlenarGridGpoCuentaRMA(string nodeId)
        //{
        //    this.StoreGpoCuentaRMA.DataSource = DAOReglaMA.ConsultaGrupoCuentaRMA(
        //            Convert.ToInt16(cmbCadenaComercial.Value),
        //            this.Usuario,
        //            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
        //        );
        //    this.StoreGpoCuentaRMA.DataBind();

        //    StoreGruposCta.DataSource = DAOGrupoCuenta.ListaCatalogoGruposCuenta(
        //            this.Usuario,
        //            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
        //    StoreGruposCta.DataBind();

        //    this.panelGpoCuentaRMA.Hidden = false;
        //    this.panelGpoCuentaRMA.Show();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void btnGruposCtaRMA_Click(object sender, EventArgs e)
        //{

        //}

        ///// <summary>
        ///// Llena el grid de los grupos de tarjeta de las reglas multiasingación
        ///// </summary>
        ///// <param name="nodeId">Nombre del nodo del menú árbol seleccionado
        //[DirectMethod(Namespace = "Administracion")]
        //public void LlenarGridGpoTarjetaRMA(string nodeId)
        //{
        //    this.StoreGpoTarjetaRMA.DataSource = DAOReglaMA.ConsultaGrupoTarjetaRMA(
        //            Convert.ToInt16(cmbCadenaComercial.Value),
        //            this.Usuario,
        //            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
        //        );

        //    this.StoreGpoTarjetaRMA.DataBind();

        //    this.panelGpoTarjetaRMA.Hidden = false;
        //    this.panelGpoTarjetaRMA.Show();
        //}

        ///// <summary>
        ///// Llena el grid de tarjeta/cuenta de las reglas multiasingación
        ///// </summary>
        ///// <param name="nodeId">Nombre del nodo del menú árbol seleccionado
        //[DirectMethod(Namespace = "Administracion")]
        //public void LlenarGridTarjetaCtaRMA(string nodeId)
        //{
        //    this.StoreTarjetaCtaRMA.DataSource = DAOReglaMA.ConsultaTarjetaCuentaRMA(
        //            Convert.ToInt16(cmbCadenaComercial.Value),
        //            this.Usuario,
        //            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
        //        );

        //    this.StoreTarjetaCtaRMA.DataBind();

        //    this.panelTarjetaCtaRMA.Hidden = false;
        //    this.panelTarjetaCtaRMA.Show();
        //}
        /***************************************************************************************
         * Muestra el formulario para el registro de un nuevo producto
         **************************************************************************************/
        protected void Click_NuevoProducto(object sender, EventArgs e)
        {
            LlenarComboVigencias();
            this.FormNuevoProducto.Reset();
            this.DialogNuevoProducto.Show();
        }

        /***************************************************************************************
         * Guarda en la base de datos un nuevo producto
         **************************************************************************************/
        protected void Click_GuardarNuevoProducto(object sender, EventArgs e)
        {
            GrupoMA nuevoGrupoMedioAcceso = new GrupoMA();
            nuevoGrupoMedioAcceso.ClaveGrupo = this.TxtClaveGrupoMA.Text;
            nuevoGrupoMedioAcceso.Descripcion = this.TxtDescripcionGrupoMA.Text;
            nuevoGrupoMedioAcceso.ID_Vigencia = Convert.ToInt32(this.ComboVigenciaGrupoMA.SelectedItem.Value);

            LNProductos.ValidaNuevoProducto(nuevoGrupoMedioAcceso, this.Usuario);

            this.DialogNuevoProducto.Hide();
            this.FormNuevoProducto.Reset();

            //#WARNING: Verificar si esto funciona!!!
            //Response.Redirect("NuevoProducto.aspx");

        }

        /***************************************************************************************
         * Muestra el formulario para el registro de una nueva vigencia
         **************************************************************************************/
        protected void Click_NuevaVigencia(object sender, EventArgs e)
        {
            LlenarComboTipoVigencias();
            LlenarComboPeriodos();
            this.FrmNuevaVigencia.Reset();
            this.DialogNuevaVigencia.Show();
        }

        /***************************************************************************************
         * Guarda en la base de datos una nueva vigencia
         **************************************************************************************/
        protected void Click_GuardarNuevaVigencia(object sender, EventArgs e)
        {
            try
            {
                Vigencia nuevaVigencia = new Vigencia();
                nuevaVigencia.Clave = this.TxtVigenciaCalve.Text;
                nuevaVigencia.Descripcion = this.TxtVigenciaDescripcion.Text;
                nuevaVigencia.ID_TipoVigencia = Convert.ToInt32(this.ComboTipoVigencia.SelectedItem.Value);
                if (DateTime.Parse(this.DateFechaInicial.Value.ToString()) != DateTime.MinValue)
                {
                    nuevaVigencia.FechaInicial = (DateTime)this.DateFechaInicial.Value;
                }
                if (DateTime.Parse(this.DateFechaFinal.Value.ToString()) != DateTime.MinValue)
                {
                    nuevaVigencia.FechaFinal = (DateTime)this.DateFechaFinal.Value;
                }
                if (TimeSpan.Parse(this.TimeHoraInicial.Value.ToString()).TotalSeconds >= 0)
                {
                    nuevaVigencia.HoraInicial = (TimeSpan)this.TimeHoraInicial.Value;
                }
                if (TimeSpan.Parse(this.TimeHoraFinal.Value.ToString()).TotalSeconds >= 0)
                {
                    nuevaVigencia.HoraFinal = (TimeSpan)this.TimeHoraFinal.Value;
                }
                nuevaVigencia.ID_Periodo = Convert.ToInt32(this.ComboPeriodo.SelectedItem.Value);

                LNVigencias.ValidaNuevaVigencia(nuevaVigencia, this.Usuario);

                X.Msg.Notify("Vigencias", "La Nueva Vigencia se Creó Exitosamente").Show();
                this.DialogNuevaVigencia.Hidden = true;
                this.FrmNuevaVigencia.Reset();
                this.btnAddVigencia.Hidden = true;
                LlenarComboVigencias();
            }
            catch (CAppException ex)
            {
                X.Msg.Alert("Vigencias", ex.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Vigencias", "Ocurrió un Error al Crear la Nueva Vigencia con los Datos Proporcionados").Show();
            }
        }

        /***************************************************************************************
         * Muestra los datos de configurción del producto seleccionado
         **************************************************************************************/
        //        protected void SeleccionProducto_Event(object sender, DirectEventArgs e) {
        //            string json = e.ExtraParams["Values"];

        //            IDictionary<string, string>[] productosSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
        //            if (productosSeleccionados == null || productosSeleccionados.Length < 1)
        //            {
        //                return;
        //            }

        //            foreach (KeyValuePair<string, string> column in productosSeleccionados[0])
        //            {
        //                switch (column.Key)
        //                {
        //                    case "ID_GrupoMA": 
        //                        TxTRangoID_GrupoMA.Text = column.Value; 
        //                        break;
        /////###WARNING:
        //                    case "Descripcion":
        //                        this.PanelConfiguracion.Title = String.Format("Configuración del DALAdministracion.Entidades.Producto: \"{0}\"", column.Value);
        //                        break;

        //                    default:
        //                        break;
        //                }
        //            }

        //            this.BtnShowNuevoRango.Disabled = false;
        //            LlenarGridRangos();
        //            this.DialogNuevoProducto.Expand();
        //}

        /***************************************************************************************
         * Muestra el formulario para el registro de un nuevo rango
         **************************************************************************************/
        protected void ShowNuevoRango_Event(object sender, DirectEventArgs e)
        {
            this.FNuevoRango.Reset();
            this.WNuevoRango.Show();
        }

        /***************************************************************************************
         * Registra en la base de datos un nuevo rango
         **************************************************************************************/
        protected void GuardarNuevoRango_Click(object sender, EventArgs e)
        {
            try
            {
                Rango nuevoRangoGrupoMA = new Rango();
                //RowSelectionModel sm = this.GridProductos.SelectionModel.Primary as RowSelectionModel;
                //if (sm != null)
                //{
                //    nuevoRangoGrupoMA.ID_GrupoMA = String.IsNullOrEmpty(sm.SelectedRow.RecordID) ? 0 : Convert.ToInt32(sm.SelectedRow.RecordID);
                //}
                nuevoRangoGrupoMA.Clave = TxTRangoClave.Text;
                nuevoRangoGrupoMA.Descripcion = TxTRangoDescripcion.Text;
                nuevoRangoGrupoMA.Inicio = String.IsNullOrEmpty(NumRangoInicio.Text) ? 0 : Convert.ToDecimal(NumRangoInicio.Text);
                nuevoRangoGrupoMA.Fin = String.IsNullOrEmpty(NumRangoFin.Text) ? 0 : Convert.ToDecimal(NumRangoFin.Text);
                nuevoRangoGrupoMA.esActivo = !SelRangoEsActivo.SelectedItem.Value.Equals("0");

                LNRangos.ValidaNuevoRango(nuevoRangoGrupoMA, this.Usuario);
                X.Msg.Notify("", "Rango Creado Exitosamente").Show();

                if (nuevoRangoGrupoMA.DescTipoMA.ToUpperInvariant().Contains("TARJETA"))
                {
                    GeneraLoteTarjetas(nuevoRangoGrupoMA.ID_GrupoMA, nuevoRangoGrupoMA.Inicio,
                        nuevoRangoGrupoMA.Fin, this.Usuario);
                }

                this.WNuevoRango.Hide();
                this.FNuevoRango.Reset();
                LlenarGridRangos("", nuevoRangoGrupoMA.ID_GrupoMA);
            }
            catch (CAppException ex)
            {
                X.Msg.Alert("Rangos", ex.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Rangos", "Ocurrió un Error al Crear el Nuevo Rango con los Datos Proporcionados").Show();
            }
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


        private static void GeneraLoteTarjetas(int idgma, decimal ini, decimal fin, Usuario usuario)
        {
            try
            {
                LNTarjetas.CreaLoteDeTarjetas(idgma, ini, fin, usuario);
                X.Msg.Notify("", "Lote de Tarjetas Creado Exitosamente").Show();
            }

            catch (CAppException ex)
            {
                X.Msg.Alert("Tarjetas", ex.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Tarjetas", "Ocurrió un Error al Crear el Lote de Tarjetas del Nuevo Rango").Show();
            }
        }
    }
}


