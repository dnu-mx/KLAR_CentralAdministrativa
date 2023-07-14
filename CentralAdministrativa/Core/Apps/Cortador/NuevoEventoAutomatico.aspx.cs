using DALCentralAplicaciones;
using DALEventos.BaseDatos;
using DALEventos.Entidades;
using DALEventos.LogicaNegocio;
using DALEventos.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Cortador
{
    public partial class NuevoEventoAutomatico : PaginaBaseCAPP
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Llenamos los combos para evento nuevo/edición
                    //Tipos de Cuenta
                    this.StoreTipoCuenta.DataSource = DAOEvento.ListaTiposCuenta(
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreTipoCuenta.DataBind();

                    //Estatus de configuración
                    this.StoreEstatusConfig.DataSource = DAOEvento.ListaEstatusConfiguracion(
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreEstatusConfig.DataBind();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Consulta a base de datos los eventos indicados y llena el grid correspondiente
        /// </summary>
        protected void LlenarGridConsultaEventos()
        {
            try
            {
                StoreConsulta.RemoveAll();

                DataSet dsEventos = DAOEvento.ConsultaEventosConfigurador(
                        txtClaveEvento.Text, txtDescrEvento.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsEventos.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Búsqueda de Eventos", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    StoreConsulta.DataSource = dsEventos;
                    StoreConsulta.DataBind();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Eventos", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Eventos", "Ocurrió un Error al Buscar los Eventos").Show();
            }
        }
        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnBuscarEvento_Click(object sender, DirectEventArgs e)
        {
            LlenarGridConsultaEventos();

            this.PanelSur.Collapsed = true;

            this.StoreEvAutomaticos.RemoveAll();

            this.StoreEvTX.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click en el botón de Nuevo Evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnNuevoEvento_Click(object sender, EventArgs e)
        {
            this.FormPanelNuevoEvento.Reset();
            this.WindowNuevoEvento.Show();
        }

        /// <summary>
        /// Controla los comandos del grid de consulta de eventos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void EventoComando(object sender, DirectEventArgs e)
        {
            try
            {
                RowSelectionModel rsm = this.GridPanelConsulta.GetSelectionModel() as RowSelectionModel;
                rsm.SelectedRows.Clear();

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
                        case "ID_ConfiguracionCorte": txtIdConfigCorte.Text = column.Value; break;
                        case "Clave": txtClave.Text = column.Value; break;
                        case "Descripcion": txtDescripcion.Text = column.Value; break;
                        case "Nombre": txtNombre.Text = column.Value; break;
                        case "ID_TipoCuenta": cmbTipoCuenta.SelectedItem.Value = column.Value; break;
                        case "ID_EstatusConfiguracion": cmbEstatusConfig.SelectedItem.Value = column.Value; break;
                        default:
                            break;
                    }
                }


                String command = (String)e.ExtraParams["Comando"];

                switch (command)
                {
                    case "Edit":
                        this.WindowNuevoEvento.Show();
                        break;

                    case "Select":
                        this.StoreEvAutomaticos.RemoveAll();
                        this.StoreEvTX.RemoveAll();

                        this.PanelSur.Collapsed = false;

                        this.PanelEventosEjecutar.Title += " - " + txtNombre.Text;
                        this.PanelEventosTX.Title += " - " + txtNombre.Text;

                        break;
                }
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
        /// Controla el evento Click al botón Guardar del formulario de Nuevo Evento,
        /// invocando la inserción o modificación del evento a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                EventoConfigurador elCorte = new EventoConfigurador();

                elCorte.Nombre = this.txtNombre.Text;
                elCorte.Clave = this.txtClave.Text;
                elCorte.Descripcion = this.txtDescripcion.Text;

                elCorte.ID_TipoCuenta = int.Parse(this.cmbTipoCuenta.Value.ToString());
                elCorte.ID_EstatusConfiguracion = int.Parse(this.cmbEstatusConfig.Value.ToString());

                LNEvento.CreaModificaEvento(elCorte, this.Usuario);
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

            finally
            {
                LlenarGridConsultaEventos();
                this.FormPanelNuevoEvento.Reset();
                this.WindowNuevoEvento.Hide();
            }
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
                IDictionary<string, string>[] configCorteSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (configCorteSeleccionado == null || configCorteSeleccionado.Length < 1)
                {
                    return;
                }

                string nombreEvento = "";

                foreach (KeyValuePair<string, string> configCorte in configCorteSeleccionado[0])
                {
                    switch (configCorte.Key)
                    {
                        case "ID_ConfiguracionCorte": this.txtIdConfigCorte.Text = configCorte.Value; break;
                        case "Nombre": nombreEvento = configCorte.Value; break;
                        default:
                            break;
                    }
                }

                this.StoreEvAutomaticos.RemoveAll();
                this.StoreEvTX.RemoveAll();

                this.PanelSur.Collapsed = false;

                this.PanelEventosEjecutar.Title += " - " + nombreEvento;
                this.PanelEventosTX.Title += " - " + nombreEvento;
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
        /// Consulta a los eventos automáticos a base de datos y llena el grid correspondiente
        /// </summary>
        protected void LlenaGridEventosAutomaticos()
        {
            try
            {
                this.StoreEvAutomaticos.RemoveAll();

                DataSet dsEvAutomaticos = new DataSet();

                StoreEvAutomaticos.DataSource = dsEvAutomaticos =
                    DAOEvento.ConsultaEventosAutomaticos(
                        int.Parse(txtIdConfigCorte.Text), txtClaveEvAutom.Text,
                        txtDescEvAutom.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                StoreEvAutomaticos.DataBind();

                RowSelectionModel rsm = this.GridEventosAutomaticos.GetSelectionModel() as RowSelectionModel;

                foreach (DataRow evAut in dsEvAutomaticos.Tables[0].Rows)
                {
                    if (Convert.ToInt32(evAut["Agrupado"].ToString()) != 0)
                    {
                        rsm.SelectedRows.Add(new SelectedRow(dsEvAutomaticos.Tables[0].Rows.IndexOf(evAut)));
                        rsm.UpdateSelection();
                    }
                }

            }

            catch (CAppException err)
            {
                X.Msg.Alert("Eventos a Ejecutar", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Eventos a Ejecutar", "Ocurrió un Error al Buscar los Eventos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de eventos a ejecutar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnBuscarEvAutom_Click(object sender, DirectEventArgs e)
        {
            if (String.IsNullOrEmpty(this.txtIdConfigCorte.Text))
            {
                return;
            }
                       
            LlenaGridEventosAutomaticos();
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de eventos por ejecutar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectEvAutomatico_Event(object sender, DirectEventArgs e)
        {
            try
            {
                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] evAutomSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (evAutomSeleccionado == null || evAutomSeleccionado.Length < 1)
                {
                    return;
                }

                string descEvAgrupado = "";

                foreach (KeyValuePair<string, string> eventoAutom in evAutomSeleccionado[0])
                {
                    switch (eventoAutom.Key)
                    {
                        case "ID_EventoAgrupado": this.txtIdEvAgrupado.Text = eventoAutom.Value; break;
                        case "Descripcion": descEvAgrupado = eventoAutom.Value; break;
                        default:
                            break;
                    }
                }

                LlenaGridEventosTX();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Eventos por Ejecutar", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Eventos por Ejecutar", "Ocurrió un Error al Seleccionar el Evento").Show();
            }
        }

        /// <summary>
        /// Controla la llamada del front al evento de seleccion/deselección de un evento automático
        /// </summary>
        /// <param name="idEvento">Identificador del evento automático selecionado/deseleccionado</param>
        [DirectMethod(Namespace = "Cortador")]
        public void AgruparEventosAutomaticos(int idEvento)
        {
            try
            {
                LNEvento.CreaModificaEventoAgrupado(int.Parse(this.txtIdConfigCorte.Text), idEvento, this.Usuario);

                LlenaGridEventosAutomaticos();
            }

            catch (Exception)
            {
                X.Msg.Alert("Eventos por Agrupar", "Ocurrió un Error al Agrupar el Evento").Show();
            }
        }

        /// <summary>
        /// Consulta a los eventos transaccionales a base de datos y llena el grid correspondiente
        /// </summary>
        protected void LlenaGridEventosTX()
        {
            try
            {
                this.StoreEvTX.RemoveAll();

                DataSet dsEventosTX = new DataSet();

                StoreEvTX.DataSource = dsEventosTX = 
                    DAOEvento.ConsultaEventosTransaccionales(
                        int.Parse(txtIdConfigCorte.Text), txtClaveEvTX.Text,
                        txtDescEvTX.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                StoreEvTX.DataBind();

                RowSelectionModel rsm = this.GridEventosTX.GetSelectionModel() as RowSelectionModel;

                foreach (DataRow eventoTX in dsEventosTX.Tables[0].Rows)
                {
                    if (Convert.ToInt32(eventoTX["Activo"].ToString()) != 0)
                    {
                        rsm.SelectedRows.Add(new SelectedRow(dsEventosTX.Tables[0].Rows.IndexOf(eventoTX)));
                        rsm.UpdateSelection();
                    }
                }

            }

            catch (CAppException err)
            {
                X.Msg.Alert("Eventos Transaccionales", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Eventos Transaccionales", "Ocurrió un Error al Buscar los Eventos").Show();
            }
        }


        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de eventos transaccionales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnBuscarEvTX_Click(object sender, DirectEventArgs e)
        {
            if (String.IsNullOrEmpty(txtIdConfigCorte.Text))
            {
                //X.Msg.Alert("", "Por favor, seleccione un corte").Show());
                return;
            }

            LlenaGridEventosTX();

        }

        /// <summary>
        /// Controla la llamada del front al evento de seleccion/deselección de un evento transaccional
        /// </summary>
        /// <param name="idEvAgrupado">Identificador del evento agrupado</param>
        /// <param name="idEvento">Identificador del evento transaccional selecionado/deseleccionado</param>
        [DirectMethod(Namespace = "Cortador")]
        public void AsociarEventosTX(int idEvAgrupado, int idEvento)
        {
            try
            {
                LNEvento.CreaModificaEventoTX(idEvAgrupado, int.Parse(this.txtIdConfigCorte.Text), idEvento, this.Usuario);

                LlenaGridEventosTX();
            }

            catch (Exception)
            {
                X.Msg.Alert("Eventos Transaccionales", "Ocurrió un Error al Asociar el Evento").Show();
            }
        }


    }
}