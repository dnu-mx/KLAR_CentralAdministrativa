using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;

namespace Administracion
{
    public partial class NuevaOperacion : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Nueva Operación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Llenamos el combo de Tipo de Evento
                    this.StoreTipoEvento.DataSource = DAOEvento.ListaTiposDeEvento(
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                    this.StoreTipoEvento.DataBind();

                    PreparaGridScript();
                    LlenaTipoCuenta();
                    LlenaTipoColectiva();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de Nuevo Evento,
        /// invocando la inserción o modificación del evento a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Evento evento = new Evento();

                evento.Clave = this.txtClave.Text;
                evento.Descripcion = this.txtDescripcion.Text;
                evento.ID_TipoEvento = int.Parse(this.cBoxTipoEvento.Value.ToString());

                evento.Activo = !sBoxActivo.SelectedItem.Value.Equals("0");
                evento.Reversable = !sBoxReversable.SelectedItem.Value.Equals("0");
                evento.Cancelable = !sBoxCancelable.SelectedItem.Value.Equals("0");
                evento.Transaccional = !sBoxTransaccional.SelectedItem.Value.Equals("0");
                evento.GeneraPoliza = !sBoxPoliza.SelectedItem.Value.Equals("0");
                evento.PreValidaciones = !sBoxPreValidaciones.SelectedItem.Value.Equals("0");
                evento.PostValidaciones = !sBoxPostValidaciones.SelectedItem.Value.Equals("0");
                
                evento.DescEdoCta = this.txtDescripcionEdoCta.Text;


                LNOperaciones.CreaModificaEvento(evento, this.Usuario);
                X.Msg.Notify("", "Evento Almacenado Exitosamente").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Evento", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Evento", "Ocurrió un Error al Guardar los Datos del Evento").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnBuscarEvento_Click(object sender, DirectEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtClaveEvento.Text) ||
                    !String.IsNullOrEmpty(txtDescrEvento.Text))
                {
                    StoreConsulta.DataSource = DAOEvento.ListaCatalogoEventos(
                            txtClaveEvento.Text, txtDescrEvento.Text, this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreConsulta.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Evento", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Consulta de Evento", "Ocurrió un Error al Buscar los Eventos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de Nuevo Evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnNuevoEvento_Click(object sender, EventArgs e)
        {
            this.FormPanelNuevoEvento.Reset();
            this.WindowNuevoEvento.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditarEvento(object sender, DirectEventArgs e)
        {
            string json = String.Format("[{0}]", e.ExtraParams["Values"]);

            IDictionary<string, string>[] eventoSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);
            if (eventoSeleccionado == null || eventoSeleccionado.Length < 1)
            {
                return;
            }

            this.FormPanelNuevoEvento.Reset();

            foreach (KeyValuePair<string, string> column in eventoSeleccionado[0])
            {
                switch (column.Key)
                {
                    case "ClaveEvento": 
                        this.txtClave.Text = column.Value;
                        break;

                    case "Descripcion":
                        this.txtDescripcion.Text = column.Value;
                        break;

                    case "EsActivo":
                        sBoxActivo.SelectedItem.Value = column.Value;
                        break;

                    case "EsReversable":
                        sBoxReversable.SelectedItem.Value = column.Value;
                        break;

                    case "EsCancelable":
                        sBoxCancelable.SelectedItem.Value = column.Value;
                        break;

                    case "EsTransaccional":
                        sBoxTransaccional.SelectedItem.Value = column.Value;
                        break;

                    case "ID_TipoEvento":
                        cBoxTipoEvento.SelectedItem.Value = column.Value;
                        break;

                    case "DescripcionEdoCta":
                        this.txtDescripcionEdoCta.Text = column.Value;
                        break;

                    case "GeneraPoliza":
                        sBoxPoliza.SelectedItem.Value = column.Value;
                        break;

                    case "PreValidaciones":
                        sBoxPreValidaciones.SelectedItem.Value = column.Value;
                        break;

                    case "PostValidaciones":
                        sBoxPostValidaciones.SelectedItem.Value = column.Value;
                        break;

                    default:
                        break;
                }
            }

            this.WindowNuevoEvento.Show();
        }


        /// <summary>
        /// Controla el evento de selección de una fila del grid de eventos consultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectEvento_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] eventoSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (eventoSeleccionado == null || eventoSeleccionado.Length < 1)
                {
                    return;
                }

                int idEvento = 0;

                foreach (KeyValuePair<string, string> evento in eventoSeleccionado[0])
                {
                    switch (evento.Key)
                    {
                        case "ID_Evento": idEvento = int.Parse(evento.Value); break;
                        default:
                            break;
                    }
                }

                LlenarGridReglas(idEvento);
                BindDataScript(idEvento);
                LlenarGridPlugins(idEvento);
                LlenarGridAutorizador(idEvento);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Eventos", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Consulta de Eventos", "Ocurrió un Error al Seleccionar el Evento").Show();
            }
        }

        /// <summary>
        /// Llena el grid Reglas de la sección de configuraciones del evento
        /// </summary>
        /// <param name="idEvento">Identificador del evento seleccionado</param>
        public void LlenarGridReglas(int idEvento)
        {
            try
            {
                DataSet ds_Reglas = new DataSet();

                this.StoreReglas.DataSource = ds_Reglas = DAOEvento.ConsultaReglasEvento(
                       idEvento, this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                   );
                this.StoreReglas.DataBind();


                RowSelectionModel rsm_Plugins = this.GridReglas.GetSelectionModel() as RowSelectionModel;

                foreach (DataRow ruleRow in ds_Reglas.Tables[0].Rows)
                {
                    if ((bool)ruleRow["Activa"])
                    {
                        rsm_Plugins.SelectedRows.Add(new SelectedRow(ds_Reglas.Tables[0].Rows.IndexOf(ruleRow)));
                        rsm_Plugins.UpdateSelection();
                    }
                }                
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Reglas", err.Mensaje()).Show();
            }
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
                string json = e.ExtraParams["Values"];
                Dictionary<string, string>[] reglas = JSON.Deserialize<Dictionary<string, string>[]>(json);

                DataTable dt = new DataTable();
                int fila = 0;

                dt.Columns.Add("ID_Evento");
                dt.Columns.Add("ID_Regla");
                dt.Columns.Add("Activa");

                foreach (Dictionary<string, string> registro in reglas)
                {
                    dt.Rows.Add();

                    foreach (KeyValuePair<string, string> regla in registro)
                    {
                        switch (regla.Key)
                        {
                            case "ID_Evento": dt.Rows[fila]["ID_Evento"] = regla.Value; break;
                            case "ID_Regla": dt.Rows[fila]["ID_Regla"] = regla.Value; break;
                            default:
                                break;
                        }
                    }

                    dt.Rows[fila]["Activa"] = false;

                    RowSelectionModel rsm = this.GridReglas.SelectionModel.Primary as RowSelectionModel;
                    foreach (SelectedRow reglaSeleccionada in rsm.SelectedRows)
                    {
                        if (dt.Rows[fila]["ID_Regla"].ToString() == reglaSeleccionada.RecordID)
                        {
                            dt.Rows[fila]["Activa"] = true;
                        }
                    }

                    fila++;
                }

                LNOperaciones.CreaModificaEventoReglas(dt, this.Usuario);
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
        /// Llena el grid Plugins de la sección de configuraciones del evento
        /// </summary>
        /// <param name="idEvento">Identificador del evento seleccionado</param>
        public void LlenarGridPlugins(int idEvento)
        {
            try
            {
                DataSet ds_Plugins = new DataSet();

                this.StorePlugins.DataSource = ds_Plugins = DAOEvento.ConsultaPluginsEvento(
                       idEvento, this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                   );
                this.StorePlugins.DataBind();


                RowSelectionModel rsm_Plugins = this.GridPlugins.GetSelectionModel() as RowSelectionModel;

                foreach (DataRow pRow in ds_Plugins.Tables[0].Rows)
                {
                    if ((bool)pRow["Activo"])
                    {
                        rsm_Plugins.SelectedRows.Add(new SelectedRow(ds_Plugins.Tables[0].Rows.IndexOf(pRow)));
                        rsm_Plugins.UpdateSelection();
                    }
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Plugins", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Controla el evento de Click al botón de Activar PlugIns
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnActivaPlugins_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                Dictionary<string, string>[] plugins = JSON.Deserialize<Dictionary<string, string>[]>(json);

                DataTable dt = new DataTable();
                int fila = 0;

                dt.Columns.Add("ID_Evento");
                dt.Columns.Add("ID_Plugin");
                dt.Columns.Add("Activo");
                dt.Columns.Add("OrdenEjecucion");
                dt.Columns.Add("EsRespuestaISO");
                dt.Columns.Add("EsObligatorioParaReverso");

                foreach (Dictionary<string, string> registro in plugins)
                {
                    dt.Rows.Add();

                    foreach (KeyValuePair<string, string> plugin in registro)
                    {
                        switch (plugin.Key)
                        {
                            case "ID_Evento": dt.Rows[fila]["ID_Evento"] = plugin.Value; break;
                            case "ID_Plugin": dt.Rows[fila]["ID_Plugin"] = plugin.Value; break;
                            case "OrdenEjecucion": dt.Rows[fila]["OrdenEjecucion"] = plugin.Value; break;
                            case "EsRespuestaISO": dt.Rows[fila]["EsRespuestaISO"] = plugin.Value; break;
                            case "EsObligatorioParaReverso": dt.Rows[fila]["EsObligatorioParaReverso"] = plugin.Value; break;
                            default:
                                break;
                        }
                    }

                    dt.Rows[fila]["Activo"] = false;

                    RowSelectionModel rsm = this.GridPlugins.SelectionModel.Primary as RowSelectionModel;
                    foreach (SelectedRow pluginSeleccionado in rsm.SelectedRows)
                    {
                        if (dt.Rows[fila]["ID_Plugin"].ToString() == pluginSeleccionado.RecordID)
                        {
                            dt.Rows[fila]["Activo"] = true;
                        }
                    }

                    fila++;
                }

                LNOperaciones.CreaModificaEventoPlugins(dt, this.Usuario);
                X.Msg.Notify("", "PlugIn(s) Activado(s) Exitosamente").Show();
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
        /// Llena el grid Autorizador Externa de la sección de configuraciones del evento
        /// </summary>
        /// <param name="idEvento">Identificador del evento seleccionado</param>
        public void LlenarGridAutorizador(int idEvento)
        {
            try
            {
                DataSet ds = new DataSet();

                this.StoreAutorizador.DataSource = ds = DAOEvento.ConsultaPluginsAutorizadorEvento(
                       idEvento, this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                   );
                this.StoreAutorizador.DataBind();


                RowSelectionModel rsm_Aut = this.GridAutorizador.GetSelectionModel() as RowSelectionModel;

                foreach (DataRow autRow in ds.Tables[0].Rows)
                {
                    if ((bool)autRow["Activo"])
                    {
                        rsm_Aut.SelectedRows.Add(new SelectedRow(ds.Tables[0].Rows.IndexOf(autRow)));
                        rsm_Aut.UpdateSelection();
                    }
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Autorizador", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Controla el evento de Click al botón de Activar PlugIns de la pestaña Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        protected void btnActivaPlugins2_Click(object sender, DirectEventArgs e)
        {
            try
            {
                RowSelectionModel rsm_Autorizador = this.GridAutorizador.SelectionModel.Primary as RowSelectionModel;

                if ((rsm_Autorizador.SelectedRows.Count > 1))
                {
                    X.Msg.Alert("Autorizador Externo", "Sólo puede activarse un PlugIn al Evento.").Show();
                    return;
                }



                string json = e.ExtraParams["Values"];
                Dictionary<string, string>[] plugins = JSON.Deserialize<Dictionary<string, string>[]>(json);

                DataTable dt = new DataTable();
                int fila = 0;

                dt.Columns.Add("ID_Evento");
                dt.Columns.Add("ID_Plugin");
                dt.Columns.Add("Activo");
                dt.Columns.Add("OrdenEjecucion");
                dt.Columns.Add("EsRespuestaISO");
                dt.Columns.Add("EsObligatorioParaReverso");

                foreach (Dictionary<string, string> registro in plugins)
                {
                    dt.Rows.Add();

                    foreach (KeyValuePair<string, string> plugin in registro)
                    {
                        switch (plugin.Key)
                        {
                            case "ID_Evento": dt.Rows[fila]["ID_Evento"] = plugin.Value; break;
                            case "ID_Plugin": dt.Rows[fila]["ID_Plugin"] = plugin.Value; break;
                            case "OrdenEjecucion": dt.Rows[fila]["OrdenEjecucion"] = plugin.Value; break;
                            case "EsRespuestaISO": dt.Rows[fila]["EsRespuestaISO"] = plugin.Value; break;
                            case "EsObligatorioParaReverso": dt.Rows[fila]["EsObligatorioParaReverso"] = plugin.Value; break;
                            default:
                                break;
                        }
                    }

                    dt.Rows[fila]["Activo"] = false;

                    RowSelectionModel rsm = this.GridPlugins.SelectionModel.Primary as RowSelectionModel;
                    foreach (SelectedRow pluginSeleccionado in rsm.SelectedRows)
                    {
                        if (dt.Rows[fila]["ID_Plugin"].ToString() == pluginSeleccionado.RecordID)
                        {
                            dt.Rows[fila]["Activo"] = true;
                        }
                    }

                    fila++;
                }

                LNOperaciones.CreaModificaEventoPlugins(dt, this.Usuario);
                X.Msg.Notify("", "PlugIn(s) Activado(s) Exitosamente").Show();
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

        protected void PreparaGridScript()
        {

            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 55;
            //acciones.PrepareToolbar.Fn = "prepareToolbar";


            GridCommand elimina = new GridCommand();
            elimina.Icon = Icon.Delete;
            elimina.CommandName = "Eliminar";
            elimina.ToolTip.Text = "Eliminar Movimiento del Script";
            acciones.Commands.Add(elimina);


            Column colID = new Column();
            colID.DataIndex = "ID_Script";
            colID.Header = "ID";
            colID.Sortable = true;
            colID.Hidden = true;
            lasColumnas.Columns.Add(colID);

            Column colEstatus = new Column();
            colEstatus.DataIndex = "EsActiva";
            colEstatus.Header = "Estatus";
            colEstatus.Sortable = true;
            // colEstatus.Hidden = true;
            lasColumnas.Columns.Add(colEstatus);

            Column coldescTipoCuenta = new Column();
            coldescTipoCuenta.DataIndex = "descTipoCuenta";
            coldescTipoCuenta.Header = "Tipo Cuenta";
            coldescTipoCuenta.Sortable = true;
            coldescTipoCuenta.Width = 150;
            lasColumnas.Columns.Add(coldescTipoCuenta);

            Column coldescTipoColectiva = new Column();
            coldescTipoColectiva.DataIndex = "descTipoColectiva";
            coldescTipoColectiva.Header = "Tipo Entidad";
            coldescTipoColectiva.Sortable = true;
            coldescTipoColectiva.Width = 150;
            lasColumnas.Columns.Add(coldescTipoColectiva);

            Column colEsAbono = new Column();
            colEsAbono.DataIndex = "EsAbono";
            colEsAbono.Header = "EsAbono";
            colEsAbono.Sortable = true;
            lasColumnas.Columns.Add(colEsAbono);

            Column colFormula = new Column();
            colFormula.DataIndex = "Formula";
            colFormula.Header = "Formula de Aplicacion";
            colFormula.Sortable = true;
            colFormula.Width = 300;
            lasColumnas.Columns.Add(colFormula);


            Column colValidaSaldo = new Column();
            colValidaSaldo.DataIndex = "ValidaSaldo";
            colValidaSaldo.Header = "Valida Saldo";
            colValidaSaldo.Sortable = true;
            lasColumnas.Columns.Add(colValidaSaldo);


            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Script", "record.data.ID_Script", ParameterMode.Raw));
            GridPanel2.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));


            GridPanel2.ColumnModel.Columns.Add(acciones);
            GridPanel2.ColumnModel.Columns.Add(colEstatus);
            GridPanel2.ColumnModel.Columns.Add(colID);
            GridPanel2.ColumnModel.Columns.Add(coldescTipoCuenta);
            GridPanel2.ColumnModel.Columns.Add(coldescTipoColectiva);
            GridPanel2.ColumnModel.Columns.Add(colEsAbono);
            GridPanel2.ColumnModel.Columns.Add(colValidaSaldo);
            GridPanel2.ColumnModel.Columns.Add(colFormula);


            GridFilters losFiltros = new GridFilters();


            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "descTipoCuenta";
            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "descTipoColectiva";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "Formula";
            losFiltros.Filters.Add(filAPaterno);

            GridPanel2.Plugins.Add(losFiltros);

        }

        public void LlenaTipoCuenta()
        {

            StoreTipoCta.DataSource = DAOCuenta.ObtieneTiposCuentaEjecutor(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoCta.DataBind();

        }

        public void LlenaTipoColectiva()
        {

            StoreTipoColectiva.DataSource = DAOCatalogos.ListaTipoColectivaParaEjecutor(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoColectiva.DataBind();
        }

        private void BindDataScript(Int32 ID_Evento)
        {
            GridPanel2.GetStore().DataSource = DAOCortes.ObtieneScriptsContables(ID_Evento, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            GridPanel2.GetStore().DataBind();
        }

        protected ScriptContable NuevoScript()
        {
            try
            {
                ScriptContable unNuevoConfig = new ScriptContable();

                unNuevoConfig.ID_ClaveAplicacion = 0;
                unNuevoConfig.ID_Script = 0;
                unNuevoConfig.ID_TipoColectiva = int.Parse(cmbTipocolectiva.Value.ToString());
                unNuevoConfig.ID_TipoCuenta = int.Parse(cmbTipoCuenta2.Value.ToString());
                unNuevoConfig.ValidaSaldo = bool.Parse(cmbValidaSaldo.Value.ToString());
                unNuevoConfig.EsAbono = bool.Parse(cmbTipoMovimiento.Value.ToString());
                unNuevoConfig.Formula = txtFormula.Text;
                unNuevoConfig.ID_Evento = (int)HttpContext.Current.Session.Contents["ID_Evento"];

                return unNuevoConfig;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                return new ScriptContable();
            }
        }

        protected void btnVer_Parametros(object sender, EventArgs e)
        {
            try
            {

                Window1.Html = DAOCatalogos.ParametrosContrato(0);
                this.Window1.Show();

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Guardar Script", "Error al Guardar el Script Contable").Show();
                return;
            }
        }

        protected void btnGuardarScript_Click(object sender, EventArgs e)
        {
            try
            {

                //Agrega el Script al Evento
                DAOCortes.AgregarScript(NuevoScript(), this.Usuario);

                //  //Limpia el Formulario 
                FormPanel2.Reset();

                //actualiza el Store del Grid
                PreparaGridScript();
                BindDataScript((Int32)HttpContext.Current.Session.Contents["ID_Evento"]);


            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                X.Msg.Alert("Guardar Script", "Error al Guardar el Script Contable").Show();
                return;
            }

        }

        protected void btnCancelarScript_Click(object sender, EventArgs e)
        {
            FormPanel2.Reset();
        }

        protected void EjecutarComandoScript(object sender, DirectEventArgs e)
        {

            try
            {
                int ID_Script = Int32.Parse(e.ExtraParams["ID_Script"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];

                if (ID_Script == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;


                //Solicitar una Confirmacion
                //X.Msg.Confirm("Confirm", "¿Estas seguro de realizar la Accion: <b>" + EjecutarComando + "</b> a la Ficha Numero:<b>" + laFichaSeleccionada.ID_FichaDeposito + "</b> de un Importe de " + String.Format("{0:C}", laFichaSeleccionada.Importe) + "?").Show();


                switch (EjecutarComando)
                {
                    case "Eliminar":
                        DAOCortes.EliminarScript(ID_Script, elUser);
                        break;

                }

                PreparaGridScript();
                BindDataScript((Int32)HttpContext.Current.Session.Contents["ID_Evento"]);


            }
            catch (CAppException err)
            {
                X.Msg.Alert("Póliza", err.Mensaje()).Show();
            }
            catch (Exception)
            {
                X.Msg.Alert("Póliza", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }
    }
}