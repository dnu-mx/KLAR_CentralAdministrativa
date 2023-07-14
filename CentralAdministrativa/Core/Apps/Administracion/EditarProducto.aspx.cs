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
    public partial class EditarProducto : DALCentralAplicaciones.PaginaBaseCAPP
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
            CrearEditaproductoDialog();

            /* Rangos */
            CrearGridRangos();

            /* Vigencias */
            CrearGridVigencias();
            CrearDialogVigencias();
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

            ImageCommand EditarProducto = new ImageCommand();
            EditarProducto.Icon = Icon.NoteEdit;
            EditarProducto.CommandName = "EditProducto";

            ImageCommandColumn EditarProductoColumn = new ImageCommandColumn();
            EditarProductoColumn.Width = 25;
            EditarProductoColumn.Commands.Add(EditarProducto);
            this.GridProductos.ColumnModel.Columns.Add(EditarProductoColumn);

            this.GridProductos.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "ID_GrupoMA", "record.data.ID_GrupoMA", ParameterMode.Raw
                ));
            this.GridProductos.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "CommandName", "command", ParameterMode.Raw
                ));
            this.GridProductos.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values", "Ext.encode(record.data)", ParameterMode.Raw
                ));

            this.GridProductos.DirectEvents.RowClick.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values",
                    "Ext.encode(#{GridProductos}.getRowsValues({selectedOnly:true}))",
                    ParameterMode.Raw
                ));

            this.GridProductos.Title = "Propiedades del Producto";
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

        /**********************************************************************************************
         *  Crear edita producto
         *********************************************************************************************/
        private void CrearEditaproductoDialog() 
        {
            DialogEditarProducto.Title = "Editar Producto: ";
            DialogEditarProducto.Hidden = true;
            DialogEditarProducto.Width = 550;
            DialogEditarProducto.Height = 170;
            DialogEditarProducto.Modal = true;

            TxtIDGrupoMA.Hidden = true;

            TxtClaveGrupoMA.FieldLabel = "Clave";
            TxtClaveGrupoMA.AnchorHorizontal = "100%";
            TxtClaveGrupoMA.MaxLength = 10;
            TxtClaveGrupoMA.AllowBlank = false;
            
            TxtDescripcionGrupoMA.FieldLabel = "Descripción";
            TxtDescripcionGrupoMA.AnchorHorizontal = "100%";
            TxtDescripcionGrupoMA.MaxLength = 50;
            TxtDescripcionGrupoMA.AllowBlank = false;

            ComboVigenciaGrupoMA.FieldLabel = "Vigencia";
            ComboVigenciaGrupoMA.AnchorHorizontal = "100%";
            ComboVigenciaGrupoMA.AllowBlank = false;
            ComboVigenciaGrupoMA.EmptyText = "Selecciona una Vigencia";
            ComboVigenciaGrupoMA.DisplayField = "Descripcion";
            ComboVigenciaGrupoMA.ValueField = "ID_Vigencia";

            FormEditarProducto.Reset();
        }

        /**********************************************************************************************
         *  Analiza el comando seleccionado
         *********************************************************************************************/
        protected void Click_EditarProducto(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];
            if( !CommandName.Equals("EditProducto") ) {
                return;
            }

            LlenarComboVigencias();
            TxtIDGrupoMA.Text = e.ExtraParams["ID_GrupoMA"];
            string json = String.Format("[{0}]", e.ExtraParams["Values"]);

            IDictionary<string, string>[] productosSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (productosSeleccionados == null || productosSeleccionados.Length < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in productosSeleccionados[0])
            {
                switch (column.Key)
                {
                    case "Clave": TxtClaveGrupoMA.Text = column.Value; break;
                    case "Descripcion": TxtDescripcionGrupoMA.Text = column.Value; break;
                    case "ID_Vigencia": ComboVigenciaGrupoMA.SelectedItem.Value = column.Value; break;
                    default:
                        break;
                }
            }

            DialogEditarProducto.Show();

        }

        /***************************************************************************************
         * Guarda en la base de datos un nuevo producto
         **************************************************************************************/
        protected void Click_GuardarProducto(object sender, EventArgs e)
        {
            GrupoMA grupoMedioAcceso = new GrupoMA();
            grupoMedioAcceso.ID_GrupoMA = String.IsNullOrEmpty(TxtIDGrupoMA.Text) ? 0 : Int32.Parse(TxtIDGrupoMA.Text);
            if( grupoMedioAcceso.ID_GrupoMA <= 0 ) {
                X.MessageBox.Alert("Advertencia", "No se ha seleccionado un producto");
                return;
            }

            grupoMedioAcceso.ClaveGrupo = TxtClaveGrupoMA.Text;
            grupoMedioAcceso.Descripcion = TxtDescripcionGrupoMA.Text;
            grupoMedioAcceso.ID_Vigencia = String.IsNullOrEmpty(ComboVigenciaGrupoMA.SelectedItem.Value) ? 0 : Int32.Parse(ComboVigenciaGrupoMA.SelectedItem.Value);

            LNProductos.ActualizaProducto(grupoMedioAcceso, this.Usuario);

            this.DialogEditarProducto.Hide();
            this.FormEditarProducto.Reset();
            LlenarGridProductos("", grupoMedioAcceso.ID_GrupoMA);
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

            ImageCommandColumn EditarRangoColumn = new ImageCommandColumn();
            EditarRangoColumn.Width = 25;
            EditarRangoColumn.Commands.Add(EditarRango);
            this.GridRangos.ColumnModel.Columns.Add(EditarRangoColumn);

            this.GridRangos.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "CommandName", "command", ParameterMode.Raw
                ));
            this.GridRangos.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values", "Ext.encode(record.data)", ParameterMode.Raw
                ));

            this.GridRangos.DirectEvents.RowClick.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values",
                    "Ext.encode(#{GridRangos}.getRowsValues({selectedOnly:true}))",
                    ParameterMode.Raw
                ));

            this.GridRangos.AutoExpandColumn = "Descripcion";
        }

        /// <summary>
        /// Controla las acciones subsecuentes al evento CLICK del botón de edición de rango
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Click_EditarRango(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];
            if (!CommandName.Equals("EditRango"))
            {
                return;
            }

            txtIDRango.Text = e.ExtraParams["ID_Rango"];
            string json = String.Format("[{0}]", e.ExtraParams["Values"]);

            IDictionary<string, string>[] rangosSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (rangosSeleccionados == null || rangosSeleccionados.Length < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in rangosSeleccionados[0])
            {
                switch (column.Key)
                {
                    case "ID_Rango": txtIDRango.Text = column.Value; break;
                    case "ID_GrupoMA": txtRango_IDGrupoMA.Text = column.Value; break;
                    case "Clave": txtRango_Clave.Text = column.Value; break;
                    case "Descripcion": txtRango_Descripcion.Text = column.Value; break;
                    case "Inicio": NumRangoInicio.Text = column.Value; break;
                    case "Fin": NumRangoFin.Text = column.Value; break;
                    case "esActivo": SelRangoEsActivo.SelectedItem.Value = column.Value; break;
                    default:
                        break;
                }
            }

            DialogEditarRango.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void GuardarRangoEditado_Click(object sender, EventArgs e)
        {
            try
            {
                Rango elRangoEditado = new Rango();

                elRangoEditado.ID_Rango = String.IsNullOrEmpty(txtIDRango.Text) ? 0 : Int32.Parse(txtIDRango.Text);
                if (elRangoEditado.ID_Rango <= 0) 
                {
                    X.MessageBox.Alert("Advertencia", "No se ha seleccionado un rango");
                    return;
                }

                elRangoEditado.ID_GrupoMA = Int32.Parse(txtRango_IDGrupoMA.Text);
                elRangoEditado.Clave = txtRango_Clave.Text;
                elRangoEditado.Descripcion = txtRango_Descripcion.Text;
                elRangoEditado.Inicio = String.IsNullOrEmpty(NumRangoInicio.Text) ? 0 : Convert.ToDecimal(NumRangoInicio.Text);
                elRangoEditado.Fin = String.IsNullOrEmpty(NumRangoFin.Text) ? 0 : Convert.ToDecimal(NumRangoFin.Text);
                elRangoEditado.esActivo = !SelRangoEsActivo.SelectedItem.Value.Equals("0");

                LNRangos.ValidaRangoEditado(elRangoEditado, this.Usuario);
              
                this.DialogEditarRango.Hide();
                this.FPEditarRango.Reset();
                LlenarGridRangos("", elRangoEditado.ID_GrupoMA);
            }
            catch (CAppException ex)
            {
                X.Msg.Alert("Rangos", ex.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Rangos", "Ocurrió un Error al Modificar el Rango con los Datos Proporcionados").Show();
            }
        }
        
        /***************************************************************************************
         * Realiza la creación del componente grid para los productos
         **************************************************************************************/
        private void CrearGridVigencias()
        {

            GroupingSummaryColumn ID_Vigencia = new GroupingSummaryColumn();
            ID_Vigencia.DataIndex = "ID_Vigencia";
            ID_Vigencia.Hidden = true;
            GridPanelVigencias.ColumnModel.Columns.Add(ID_Vigencia);

            GroupingSummaryColumn Clave = new GroupingSummaryColumn();
            Clave.DataIndex = "Clave";
            Clave.Header = "Clave";
            Clave.Width = 80;
            GridPanelVigencias.ColumnModel.Columns.Add(Clave);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Descripción";
            GridPanelVigencias.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn ID_TipoVigencia = new GroupingSummaryColumn();
            ID_TipoVigencia.DataIndex = "ID_TipoVigencia";
            ID_TipoVigencia.Hidden = true;
            GridPanelVigencias.ColumnModel.Columns.Add(ID_TipoVigencia);

            GroupingSummaryColumn TipoVigencia = new GroupingSummaryColumn();
            TipoVigencia.DataIndex = "TipoVigencia";
            TipoVigencia.Header = "Tipo";
            TipoVigencia.Width = 100;
            GridPanelVigencias.ColumnModel.Columns.Add(TipoVigencia);

            GroupingSummaryColumn FechaIncial = new GroupingSummaryColumn();
            FechaIncial.DataIndex = "FechaIncial";
            FechaIncial.Header = "Fecha Incial";
            FechaIncial.Width = 80;
            GridPanelVigencias.ColumnModel.Columns.Add(FechaIncial);

            GroupingSummaryColumn FechaFinal = new GroupingSummaryColumn();
            FechaFinal.DataIndex = "FechaFinal";
            FechaFinal.Header = "Fecha Final";
            FechaFinal.Width = 80;
            GridPanelVigencias.ColumnModel.Columns.Add(FechaFinal);

            GroupingSummaryColumn HoraInicial = new GroupingSummaryColumn();
            HoraInicial.DataIndex = "HoraInicial";
            HoraInicial.Header = "Hora Inicial";
            HoraInicial.Width = 80;
            GridPanelVigencias.ColumnModel.Columns.Add(HoraInicial);

            GroupingSummaryColumn HoraFinal = new GroupingSummaryColumn();
            HoraFinal.DataIndex = "HoraFinal";
            HoraFinal.Header = "Hora Final";
            HoraFinal.Width = 80;
            GridPanelVigencias.ColumnModel.Columns.Add(HoraFinal);

            GroupingSummaryColumn ID_Periodo = new GroupingSummaryColumn();
            ID_Periodo.DataIndex = "ID_Periodo";
            ID_Periodo.Hidden = true;
            GridPanelVigencias.ColumnModel.Columns.Add(ID_Periodo);

            GroupingSummaryColumn Periodo = new GroupingSummaryColumn();
            Periodo.DataIndex = "Periodo";
            Periodo.Header = "Periodo";
            Periodo.Width = 110;
            GridPanelVigencias.ColumnModel.Columns.Add(Periodo);

            ImageCommand EditarVigencia = new ImageCommand();
            EditarVigencia.Icon = Icon.NoteEdit;
            EditarVigencia.CommandName = "EditVigencia";

            ImageCommandColumn EditarVigenciaColumn = new ImageCommandColumn();
            EditarVigenciaColumn.Width = 25;
            EditarVigenciaColumn.Commands.Add(EditarVigencia);
            GridPanelVigencias.ColumnModel.Columns.Add(EditarVigenciaColumn);

            this.GridPanelVigencias.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "ID_Vigencia", "record.data.ID_Vigencia", ParameterMode.Raw
                ));
            this.GridPanelVigencias.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "CommandName", "command", ParameterMode.Raw
                ));
            this.GridPanelVigencias.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values", "Ext.encode(record.data)", ParameterMode.Raw
                ));

            this.GridPanelVigencias.DirectEvents.RowClick.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values",
                    "Ext.encode(#{GridPanelVigencias}.getRowsValues({selectedOnly:true}))",
                    ParameterMode.Raw
                ));

            GridPanelVigencias.AutoExpandColumn = "Descripcion";
            TablaVigencias.Title = "Vigencias";
            TablaVigencias.Icon = Icon.ApplicationViewColumns;
            TablaVigencias.Hidden = true;
            TablaVigencias.Modal = true;
            TablaVigencias.Width = 800;
            TablaVigencias.Height = 300;
        }

        /***************************************************************************************
         * Realiza la creación del componente dialog para el registro y la edición 
         * de las vigencias
         **************************************************************************************/
        private void CrearDialogVigencias() 
        {
            DialogEditarVigencia.Title = "Vigencias";
            DialogEditarVigencia.Hidden = true;
            DialogEditarVigencia.Width = 500;
            DialogEditarVigencia.Height = 310;
            DialogEditarVigencia.Modal = true;

            TxtIDVigencia.Hidden = true;
            
            TxtVigenciaCalve.FieldLabel = "Clave";
            TxtVigenciaCalve.AnchorHorizontal = "100%";
            TxtVigenciaCalve.MaxLength = 50;
            TxtVigenciaCalve.AllowBlank = false;


            TxtVigenciaDescripcion.FieldLabel = "Descripción";
            TxtVigenciaDescripcion.AnchorHorizontal = "100%";
            TxtVigenciaDescripcion.MaxLength = 15;
            TxtVigenciaDescripcion.AllowBlank = false;

            ComboTipoVigencia.FieldLabel = "Tipo";
            ComboTipoVigencia.AnchorHorizontal = "100%";
            ComboTipoVigencia.EmptyText = "Selecciona un tipo de Vigencias";
            ComboTipoVigencia.DisplayField = "Descripcion";
            ComboTipoVigencia.ValueField = "ID_TipoVigencia";

            DateFechaInicial.FieldLabel = "Fecha Inicial";
            DateFechaInicial.AnchorHorizontal = "100%";
            DateFechaInicial.AllowBlank = false;
            
            DateFechaFinal.FieldLabel = "Fecha Terminación";
            DateFechaFinal.AnchorHorizontal = "100%";
            DateFechaFinal.AllowBlank = false;

            TimeHoraInicial.FieldLabel = "Hora Inicial";
            TimeHoraInicial.AnchorHorizontal = "100%";
            TimeHoraInicial.AllowBlank = false;
            
            TimeHoraFinal.FieldLabel = "Hora Terminación";
            TimeHoraFinal.AnchorHorizontal = "100%";
            TimeHoraFinal.AllowBlank = false;
            
            ComboPeriodo.FieldLabel = "Periodo";
            ComboPeriodo.AnchorHorizontal = "100%";
            ComboPeriodo.EmptyText = "Selecciona un Periodo";
            ComboPeriodo.DisplayField = "Descripcion";
            ComboPeriodo.ValueField = "ID_Periodo";
            ComboPeriodo.AllowBlank = false;
            
        }

        /***************************************************************************************
         * Llen la lista de vigencias para el registro de una GrupoMA
         **************************************************************************************/
        [DirectMethod(Namespace = "Administracion")]
        public void LlenarComboVigencias()
        {
            this.StoreVigencias.DataSource = DAOGruposMA.ListaVigencias(this.Usuario);

            this.StoreVigencias.DataBind();
        }

        /***************************************************************************************
         * Muestra el formulario para el registro de un nuevo producto
         **************************************************************************************/
        protected void VerVigencias_Click(object sender, EventArgs e)
        {
            LlenarComboVigencias();
            this.TablaVigencias.Show();
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

        /***************************************************************************************
         * Muestra el formulario para el registro de una nueva vigenia
         **************************************************************************************/
        protected void NuevaVigencia_Click(object sender, EventArgs e)
        {
            FormEditarVigencia.Reset();
            LlenarComboPeriodos();
            LlenarComboTipoVigencias();
            DialogEditarVigencia.Show();
        }

        /**********************************************************************************************
         *  Analiza el comando seleccionado
         *********************************************************************************************/
        protected void EditarVigencia_Click(object sender, DirectEventArgs e)
        {
            string CommandName = e.ExtraParams["CommandName"];
            if (!CommandName.Equals("EditVigencia"))
            {
                return;
            }

            FormEditarVigencia.Reset();
            LlenarComboPeriodos();
            LlenarComboTipoVigencias();
            TxtIDVigencia.Text = e.ExtraParams["ID_Vigencia"];

            string json = String.Format("[{0}]", e.ExtraParams["Values"]);

            IDictionary<string, string>[] vigenciasSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (vigenciasSeleccionados == null || vigenciasSeleccionados.Length < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> column in vigenciasSeleccionados[0])
            {
                switch (column.Key)
                {

                    case "Clave": 
                        TxtVigenciaCalve.Text = column.Value; 
                        break;

                    case "Descripcion": 
                        TxtVigenciaDescripcion.Text = column.Value; 
                        DialogEditarVigencia.Title = "Editar Vigencia: " + column.Value;
                        break;

                    case "ID_TipoVigencia": 
                        ComboTipoVigencia.SelectedItem.Value = column.Value; 
                        break;

                    case "FechaIncial": 
                        DateFechaInicial.SelectedValue = column.Value; 
                        break;

                    case "FechaFinal": 
                        DateFechaFinal.SelectedValue = column.Value; 
                        break;

                    case "HoraInicial": 
                        TimeHoraInicial.SelectedValue = column.Value; 
                        break;

                    case "HoraFinal": 
                        TimeHoraFinal.SelectedValue = column.Value; 
                        break;

                    case "ID_Periodo": 
                        ComboPeriodo.SelectedItem.Value = column.Value; 
                        break;

                    default:
                        break;
                }
            }

            DialogEditarVigencia.Show();

        }

        /***************************************************************************************
         * Guarda en la base de datos una nueva vigencia
         **************************************************************************************/
        protected void GuardarVigencia_Click(object sender, EventArgs e)
        {
            Vigencia vigencia = new Vigencia();

            vigencia.ID_Vigencia = String.IsNullOrEmpty(TxtIDVigencia.Text) ? 0 : Int32.Parse(TxtIDVigencia.Text);
            vigencia.Clave = this.TxtVigenciaCalve.Text;
            vigencia.Descripcion = this.TxtVigenciaDescripcion.Text;

            string selectedValue = ComboTipoVigencia.SelectedItem.Value;
            vigencia.ID_TipoVigencia = String.IsNullOrEmpty(selectedValue) ? 0 : Int32.Parse(selectedValue);

            vigencia.FechaInicial = this.DateFechaInicial.SelectedDate;
            vigencia.FechaFinal = this.DateFechaFinal.SelectedDate;
            vigencia.HoraInicial = this.TimeHoraInicial.SelectedTime;
            vigencia.HoraFinal = this.TimeHoraFinal.SelectedTime;

            selectedValue = ComboPeriodo.SelectedItem.Value;
            vigencia.ID_Periodo = String.IsNullOrEmpty(selectedValue) ? 0 : Int32.Parse(selectedValue);

            if (vigencia.ID_Vigencia == 0)
            {
                LNVigencias.ValidaNuevaVigencia(vigencia, this.Usuario);
            }
            else 
            {
                LNVigencias.ActualizaVigencia(vigencia, this.Usuario);
            }

            LlenarComboVigencias();
            DialogEditarVigencia.Hide();
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
        /// Controla el evento de Click al botón de Guardar Cambios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnEditarReglas_Click(object sender, DirectEventArgs e)
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
                    throw new CAppException(8009, "Por favor, ingresa todos los campos de las Reglas que deseas modificar");
                }

                LNReglas.CreaModificaRMA(dt, Convert.ToInt32(cmbCadenaComercial.Value), this.Usuario);
                X.Msg.Notify("", "Regla(s) Editada(s) Exitosamente").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Editar Reglas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Editar Reglas", "Ocurrió un Error al Editar la Regla").Show();
            }
        }

        /// <summary>
        /// Controla la edición de un Parámetro Multiasignación (PMA), para hacer la llamada 
        /// correspondiente a base de datos.
        /// </summary>
        /// <param name="IdVPMA">Identificador del Valor PMA</param>
        /// <param name="IdPMA">Identificador del PMA</param>
        /// <param name="e">Identificador de Entidad</param>
        /// <param name="tipo">"Tipo" de PMA: Regla, Tipo de Cuenta, etc. </param>
        /// <param name="o">Identificador del Origen o Entidad: Regla, Tipo de Cuenta, etc.</param>
        /// <param name="p">Identificador del Producto o Grupo de Medios de Acceso</param>
        /// <param name="cc">Identificador de la Cadena Comercial</param>
        /// <param name="vig">Identificador de la Vigencia</param>
        /// <param name="valor">Valor del PMA editado</param>
        [DirectMethod(Namespace = "Administracion")]
        public void EdicionPMA(int IdVPMA, int IdPMA, int e, string tipo, int o, int p, int cc, int vig, string valor)
        {
            try
            {
                if (vig == 0 || String.IsNullOrEmpty(valor))
                {
                    return;
                }

                ParametroMA PMAeditado = new ParametroMA();

                PMAeditado.ID_ValorParametroMultiasignacion = IdVPMA;
                PMAeditado.ID_ParametroMultiasignacion = IdPMA;
                PMAeditado.ID_Entidad = e;
                PMAeditado.ID_Origen = o;
                PMAeditado.ID_Producto = p;
                PMAeditado.ID_CadenaComercial = cc;
                PMAeditado.ID_Vigencia = vig;
                PMAeditado.ValorPMA = valor;

                LNParametros.CreaModificaPMA(PMAeditado, this.Usuario);
                X.Msg.Notify("", "Parámetro Actualizado Exitosamente").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualizar Parámetro Multiasignación", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                X.Msg.Alert("Ocurrió un Error al Actualizar el Parámetro", ex.Message).Show();
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
                    !String.IsNullOrEmpty(txtNombre.Text) )
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id_VMA"></param>
        /// <returns></returns>
        private DataSet consultaDatosParaEdicionVMA(int Id_VMA)
        {
            DataSet dsDatosEdicion = new DataSet();

            try
        {
                string id_cadena = this.cmbCadenaComercial.Value as string;

                dsDatosEdicion = DAOValidacionMA.ConsultaDatosPorEditarVMA(
                    int.Parse(id_cadena), Id_VMA, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Editar Validación", err.Mensaje()).Show();
        }

            catch (Exception)
        {
                X.Msg.Alert("Editar Validación", "Ocurrió un Error al Obtener los Datos de la Validación.").Show();
            }

            return dsDatosEdicion;
        }

        /// <summary>
        /// Edita una validación multiasignación del GMA seleccionado
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void muestraDialogValidacion(object sender, DirectEventArgs e)
        {
            string ID_VMA = e.ExtraParams["idVM"].ToString();
            this.f_validacion.Text = e.ExtraParams["validacion"].ToString();
            this.f_formula.Text = e.ExtraParams["formula"].ToString();
            this.f_tipo_elemento.SelectedItem.Text = e.ExtraParams["tipoVal"].ToString();
            this.f_declinar.Value = e.ExtraParams["declinar"].ToString().Contains("No") ? false : true;
            this.f_orden.SelectedItem.Value = e.ExtraParams["ordenVal"].ToString();
            this.f_prereglas.Value = e.ExtraParams["preReglas"].ToString() == "False" ? false : true;
            this.f_postreglas.Value = e.ExtraParams["postReglas"].ToString() == "False" ? false : true;
            this.f_vigencia.SelectedItem.Text = e.ExtraParams["vigencia"].ToString();
            this.f_prioridad.SelectedItem.Value = e.ExtraParams["prioridad"].ToString();

            DataSet dsData = consultaDatosParaEdicionVMA(int.Parse(ID_VMA));

            bool esValidacionBase = Convert.ToBoolean(dsData.Tables[0].Rows[0]["esValidacionBase"]);
            this.f_campo.Text = dsData.Tables[0].Rows[0]["Campo"].ToString();
            this.f_error.Text = dsData.Tables[0].Rows[0]["CodigoError"].ToString();


            //Se configura cuadro de diálogo con los campos y títulos que le corresponden según la VMA
            this.DialogValidacion.Title = esValidacionBase ? "Editar Validación Base" : "Editar Validación";
            this.DialogValidacion.Icon = Icon.NoteEdit;
           
            //Orden de validación
            this.f_orden.Disabled = esValidacionBase ? false : true;
            this.f_orden.Hidden = esValidacionBase ? false : true;

            //Prereglas
            this.f_prereglas.Disabled = esValidacionBase ? false : true;
            this.f_prereglas.Hidden = esValidacionBase ? false : true;

            //Prereglas
            this.f_postreglas.Disabled = esValidacionBase ? false : true;
            this.f_postreglas.Hidden = esValidacionBase ? false : true;

            obtenerTiposDeElementosComparar();

            this.DialogValidacion.Height = esValidacionBase ? 470 : 400;
            this.DialogValidacion.Show();
        }

        /// <summary>
        /// Edita una validación multiasignación del GMA seleccionado
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void editarValidacion_Event(object sender, DirectEventArgs e)
        {
            try
        {
                string idNodeBase = e.ExtraParams["idNodo"];
                String[] subNodes = e.ExtraParams["idProd"].ToString().Split('_');
                string id_cadena = this.cmbCadenaComercial.Value as string;
                
                ValidacionMA vmaEditada = new ValidacionMA();
                vmaEditada.ID_CadenaComercial = Convert.ToInt64(id_cadena);
                vmaEditada.ID_Producto = Convert.ToInt64(subNodes[1]);
                vmaEditada.Vigencia = this.f_vigencia.SelectedItem.Value.ToString();
                vmaEditada.Nombre = this.f_validacion.Text;
                vmaEditada.Campo = this.f_campo.Text;
                vmaEditada.ClaveTipoElemento = this.f_tipo_elemento.Value.ToString();
                vmaEditada.Formula = this.f_formula.Text;
                vmaEditada.CodigoError = this.f_error.Text;
                vmaEditada.Declinar = Convert.ToBoolean(this.f_declinar.SelectedItem.Value);
                vmaEditada.PreRegla = Convert.ToBoolean(this.f_prereglas.SelectedItem.Value);
                vmaEditada.PostRegla = Convert.ToBoolean(this.f_postreglas.SelectedItem.Value);
                vmaEditada.OrdenValidacion = Convert.ToInt32(this.f_orden.SelectedItem.Value);
                vmaEditada.Prioridad = Convert.ToInt32(this.f_prioridad.SelectedItem.Value);

                LNValidaciones.ModificaVMA(vmaEditada, Convert.ToInt64(idNodeBase), this.Usuario);
                LlenaGridVMA(id_cadena);

                this.edita_validacion.Reset();
                this.DialogValidacion.Hide();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Editar Validación", err.Mensaje()).Show();
        }

            catch (Exception)
        {
                X.Msg.Alert("Editar Validación", "Ocurrió un Error al Editar la Validación").Show();
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
    }
}