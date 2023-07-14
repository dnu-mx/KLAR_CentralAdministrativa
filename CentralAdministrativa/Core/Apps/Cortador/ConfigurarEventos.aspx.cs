using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Cortador
{
    public partial class ConfigurarEventos : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    obtenerTiposDeElementosComparar();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

        }

        /***********************************************************************************************************************
         * Obtiene el catálogo de tipos de elementos que se pueden comparar
         * 
         **********************************************************************************************************************/
        private void obtenerTiposDeElementosComparar() 
        {
            this.stTipoElementoComparar.DataSource = DAOCatalogos.ListaTipoElementoComparar(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.stTipoElementoComparar.DataBind();
        }

        /***********************************************************************************************************************
         * Obtiene los eventos en función del nombre proporcionado
         * 
         * @param tipoBusqueda      Tipo de filtropor aplicar (0) Clave / (1) Nombre evento
         * @param criterio          Cadena utilizada para aplicar el filtro
         * 
         **********************************************************************************************************************/
        private void obtenerEventos(int tipoBusqueda, string criterio) 
        {
            DataSet dsEventos = DAOCatalogos.ListaEventos(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                tipoBusqueda, criterio);

            if (dsEventos.Tables[0].Rows.Count < 1)
            {
                X.Msg.Alert("Búsqueda de Eventos", "No existen coincidencias con la búsqueda solicitada.").Show();
                return;
            }
            else
            {
                this.stEventos.DataSource = dsEventos;
                this.stEventos.DataBind();
            }
        }

        /***********************************************************************************************************************
         * Evalua si debe actualizarse el panel, con lo cual se realizará de forma posterior la llamada a 
         * la función stEventos_Refresh
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void llenarPanelEventos(object sender, EventArgs e)
        {
            this.ID_EventoSeleccionado.Value = "";
            this.panel_propiedades.Title = "Configuración";
            X.Call("refreshTree", new JRawValue(this.panel_validacion.ClientID), "");

            if ( !String.IsNullOrEmpty(this.txtCriterioBusqueada.Text) )
                this.panel_eventos.Reload();
        }

        /***********************************************************************************************************************
         * Recupera los eventos, en base al criterio y actualiza el panel
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void stEventos_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            string criterioBusqueda = this.txtCriterioBusqueada.Text;
            
            if (!String.IsNullOrEmpty(criterioBusqueda.Trim()))
            {
                int tipoBusqueda = Convert.ToInt32( this.cbTipoBusqueda.SelectedItem.Value );
                obtenerEventos(tipoBusqueda, criterioBusqueda);
                this.txtCriterioBusqueada.Text = "";
            }
            else 
            {
                this.stEventos.DataBind();
            }
        }

        /***********************************************************************************************************************
         * Recupera los datos del registro seleccionado y actualiza el panel de configuración cuando se da
         * doble click sobre el registro
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void evento_Event(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["Values"];
            IDictionary<string, string>[] eventosSeleccionados = JSON.Deserialize<Dictionary<string, string>[]>(json);

            if (eventosSeleccionados != null && eventosSeleccionados.Length > 0) 
            {
                string nombreEvento = "";
                string idEvento = "";
                eventosSeleccionados[0].TryGetValue("Nombre", out nombreEvento);
                eventosSeleccionados[0].TryGetValue("ID_Evento", out idEvento);
                this.panel_propiedades.Title = String.Format("Configuración del evento \"{0}\"", nombreEvento);

                llenarPanelConfiguraciones(idEvento);
                this.panel_propiedades.Expand(true);

            }
            
        }


        /***********************************************************************************************************************
         * Inicializa el panel de validaciones, al momento de seleccionar un elemento de la
         * lista de eventos
         * 
         * @param   strIDEvento     ID del evento que fue seleccionado
         * 
         **********************************************************************************************************************/
        protected void llenarPanelConfiguraciones(string strIDEvento)
        {
            if (String.IsNullOrEmpty(strIDEvento.Trim()))
                return;

            this.ID_EventoSeleccionado.Value = strIDEvento;
            int idEvento = Convert.ToInt32(strIDEvento);
            int ordenValidacion = 0;

            DataSet validaciones = DAOCatalogos.ListaValidacionesEvento(
                this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                idEvento);

            Ext.Net.TreeNode root = this.panel_validacion.Root.Primary as Ext.Net.TreeNode;
            Ext.Net.TreeNodeCollection nodes = root.Nodes;
            nodes.Clear();

            IDictionary<long, NodosTrueFalse> listasubNodos = new Dictionary<long, NodosTrueFalse>();

            foreach (DataRow validacionRow in validaciones.Tables[0].Rows)
            {
                Validacion nuevaValidacion = new Validacion();
                nuevaValidacion.ID_Validacion = Convert.ToInt64(validacionRow["ID_Validacion"].ToString());
                nuevaValidacion.esValidacionBase = Convert.ToBoolean(validacionRow["esValidacionBase"].ToString());
                nuevaValidacion.Nombre = validacionRow["Nombre"].ToString();
                nuevaValidacion.Formula = validacionRow["Formula"].ToString();
                nuevaValidacion.ClaveTipoElemento = validacionRow["TipoValidacion"].ToString();
                nuevaValidacion.Declinar = Convert.ToBoolean(validacionRow["Declinar"].ToString());
                nuevaValidacion.Estatus = Convert.ToBoolean(validacionRow["EsActiva"].ToString());
                nuevaValidacion.PreRegla = Convert.ToBoolean(validacionRow["PreRegla"].ToString());
                nuevaValidacion.PostRegla = Convert.ToBoolean(validacionRow["PostRegla"].ToString());

                string strOrdenValidacion = validacionRow["OrdenValidacion"].ToString();
                nuevaValidacion.OrdenValidacion = String.IsNullOrEmpty(strOrdenValidacion) ? 0 : Convert.ToInt32(strOrdenValidacion);

                string idValidacion = validacionRow["ID_ValidacionTrue"].ToString().ToLower();
                nuevaValidacion.ID_ValidacionTrue = String.IsNullOrEmpty(idValidacion) ? 0L : Convert.ToInt64(idValidacion);

                idValidacion = validacionRow["ID_ValidacionFalse"].ToString().ToLower();
                nuevaValidacion.ID_ValidacionFalse = String.IsNullOrEmpty(idValidacion) ? 0L : Convert.ToInt64(idValidacion);

                Ext.Net.TreeNode validacionBase = crearValidacion(Convert.ToString(idEvento), nuevaValidacion);

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

                    listasubNodos.TryGetValue(nuevaValidacion.ID_Validacion, out subnodo);
                    if (subnodo != null)
                    {
                        if (subnodo.tipo)
                            validacionBase.Icon = Icon.Accept;
                        else
                            validacionBase.Icon = Icon.Cancel;

                        subnodo.Base.Nodes.Add(validacionBase);
                        listasubNodos.Remove(nuevaValidacion.ID_Validacion);
                    }

                }

            }

            X.Call("refreshTree", new JRawValue(this.panel_validacion.ClientID), this.panel_validacion.Root.ToJson().ToString());
            generarOrdenValidacion(ordenValidacion);
        }

        /***********************************************************************************************************************
         * Genera las opciones válidad para el campo orden de validación
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

            this.stOrdenValidacion.DataSource = list;
            this.stOrdenValidacion.DataBind();
            this.f_orden.SelectedItem.Value = nValidaciones.ToString();
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
        private Ext.Net.TreeNode crearValidacion(string idEvento, Validacion datosValidacion)
        {
            Ext.Net.TreeNode node = new Ext.Net.TreeNode();
            node.Text = datosValidacion.Nombre;
            node.NodeID = Convert.ToString(datosValidacion.ID_Validacion);
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("ID_Evento", idEvento, Ext.Net.ParameterMode.Value));
            node.CustomAttributes.Add(new Ext.Net.ConfigItem("ID_Validacion", node.NodeID, Ext.Net.ParameterMode.Value));
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

            node.Expanded = true;
            return node;
        }

        /***********************************************************************************************************************
         * Agrega una nueva validación al evento seleccionado
         * 
         * @param name="sender"
         * @param name="e"
         * 
         **********************************************************************************************************************/
        protected void crearValidacion_Event(object sender, DirectEventArgs e)
        {
            string idNodeBase = e.ExtraParams["idNodo"];
            string tipo = e.ExtraParams["TipoValidacion"];
            string id_evento = this.ID_EventoSeleccionado.Value as string;

            if (!string.IsNullOrEmpty(tipo.Trim()) && !string.IsNullOrEmpty(id_evento.Trim()))
            {
                tipo = (String.Equals(tipo, "0") || String.Equals(tipo, "1")) ? "1" : tipo;

                Validacion validacionNueva = new Validacion();
                validacionNueva.ID_Evento = Convert.ToInt32(id_evento);
                validacionNueva.Nombre = this.f_validacion.Text;
                validacionNueva.Campo = this.f_campo.Text;
                validacionNueva.ClaveTipoElemento = this.f_tipo_elemento.Value.ToString();
                validacionNueva.Formula = this.f_formula.Text;
                validacionNueva.CodigoError = this.f_error.Text;
                validacionNueva.Declinar = Convert.ToBoolean(this.f_declinar.SelectedItem.Value);
                validacionNueva.PreRegla = Convert.ToBoolean(this.f_prereglas.SelectedItem.Value);
                validacionNueva.PostRegla = Convert.ToBoolean(this.f_postreglas.SelectedItem.Value);
                validacionNueva.OrdenValidacion = Convert.ToInt32(this.f_orden.SelectedItem.Value);

                DAOValidacion.insertar(
                    validacionNueva,
                    Convert.ToInt64(idNodeBase),
                    Convert.ToInt32(tipo),
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                    );

                this.nueva_validacion.Reset();
                llenarPanelConfiguraciones(id_evento);

            }

            this.dialog_validacion.Hide();
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
            string id_evento = this.ID_EventoSeleccionado.Value as string;

            if (String.IsNullOrEmpty(idValidacion))
                return;

            Validacion validacion = new Validacion();
            validacion.ID_Validacion = Convert.ToInt64(idValidacion);

            DAOValidacion.inactivar(
                validacion,
                false,
                this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            llenarPanelConfiguraciones(id_evento);
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
            string id_evento = this.ID_EventoSeleccionado.Value as string;

            if (String.IsNullOrEmpty(idValidacion))
                return;

            Validacion validacion = new Validacion();
            validacion.ID_Validacion = Convert.ToInt64(idValidacion);

            DAOValidacion.inactivar(
                validacion,
                true,
                this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            llenarPanelConfiguraciones(id_evento);
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
            string id_evento = this.ID_EventoSeleccionado.Value as string;

            if (String.IsNullOrEmpty(idValidacion))
                return;

            Validacion validacion = new Validacion();
            validacion.ID_Validacion = Convert.ToInt64(idValidacion);

            DAOValidacion.eliminar(
                validacion,
                this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            llenarPanelConfiguraciones(id_evento);
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
            string id_evento = this.ID_EventoSeleccionado.Value as string;

            if (String.IsNullOrEmpty(idValidacion))
                return;

            Validacion validacion = new Validacion();
            validacion.ID_Validacion = Convert.ToInt64(idValidacion);

            DAOValidacion.PreReglas(
                validacion,
                activar,
                this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            llenarPanelConfiguraciones(id_evento);
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
            string id_evento = this.ID_EventoSeleccionado.Value as string;

            if (String.IsNullOrEmpty(idValidacion))
                return;

            Validacion validacion = new Validacion();
            validacion.ID_Validacion = Convert.ToInt64(idValidacion);

            DAOValidacion.PostReglas(
                validacion,
                activar,
                this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            llenarPanelConfiguraciones(id_evento);
        }

        /***********************************************************************************************************************
        * Muestra el dialog para el registro de una nueva validación
        * 
        **********************************************************************************************************************/
        [DirectMethod]
        public void showFormNuevaValidacion(int numNodos, String nodo, int tipo) 
        {
            if (String.IsNullOrEmpty(this.ID_EventoSeleccionado.Value.ToString()))
                return;

            if (numNodos > 0 && tipo == 0)
                return;

            bool esValidacionBase = !(tipo == 2 || tipo == 3);
            this.dialog_validacion.Title = "Nueva Validación Base";
            this.dialog_validacion.Icon = Icon.Add;

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

                this.dialog_validacion.Icon = tipo == 2 ? Icon.Accept : Icon.Cancel;
                this.dialog_validacion.Title = tipo == 2 ? "Nueva Validación True" : "Nueva Validación False";

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

            this.dialog_validacion.Height = esValidacionBase ? 370 : 300;
            this.dialog_validacion.Show();
        }

        /***********************************************************************************************************************
         * Representación lógica de una validación TRUE/FALSE utilizada durante la recontrucción del árbol
         * 
         **********************************************************************************************************************/
        class NodosTrueFalse {
            public Ext.Net.TreeNode Base { get; set; }
            public bool tipo { get; set; }
        }

    }
}