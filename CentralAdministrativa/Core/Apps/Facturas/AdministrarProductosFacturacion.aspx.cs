using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace Facturas
{
    public partial class AdministrarProductosFacturacion : PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataTable dtTI = DAOProductoFacturacion.ObtieneTiposImpuesto(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                    this.StoreTipoImpuesto.DataSource = dtTI;
                    this.StoreTipoImpuesto.DataBind();

                    foreach (DataRow row in dtTI.Rows)
                    {
                        if (row["Descripcion"].ToString() == "IVA")
                        {
                            this.cBoxTipoImpuesto.SelectedItem.Value = row["ID_TipoImpuesto"].ToString();
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    DataTable dtClaves =
                        DAOProductoFacturacion.ListaUltimasClavesEventos(this.Usuario);

                    sb.Append("Últimas claves: ");
                    foreach (DataRow dr in dtClaves.Rows)
                    {
                        sb.AppendFormat("{0}, ", dr[0].ToString());
                    }

                    this.btnTip.ToolTip = sb.ToString();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Consulta a base de datos los productos de facturación y llena el grid correspondiente
        /// </summary>
        protected void LlenaGridProductosFact()
        {
            try
            {
                DataTable dtProductosFact = DAOProductoFacturacion.ObtieneProductosFacturacion(
                    txtProductoFact.Text, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dtProductosFact.Rows.Count < 1)
                {
                    X.Msg.Alert("Consulta de Conceptos", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    StoreProductosFact.DataSource = dtProductosFact;
                    StoreProductosFact.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Conceptos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Conceptos", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de conceptos de facturación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnBuscarProdFact_Click(object sender, DirectEventArgs e)
        {
            LimpiaPanelCentral(false);

            LlenaGridProductosFact();
        }

        /// <summary>
        /// Restablece a su estado de carga inicial todos los controles del Panel Lateral
        /// </summary>
        protected void LimpiaPanelLateral()
        {
            this.FormPanelNuevoPF.Reset();
            this.hdnEsUnidad.Reset();
            this.hdnEsProdServ.Reset();
            this.txtDescripcion.Reset();
            this.cBoxTipoImpuesto.Reset();
            this.txtUnidad.Reset();
            this.txtProdServ.Reset();

            this.StoreCatalogo.RemoveAll();
            this.GridCatalogos.Disabled = true;
            this.PanelLateral.Collapsed = true;
        }

        /// <summary>
        /// Restablece a su estado de carga inicial todos los controles del panel central
        /// </summary>
        protected void LimpiaPanelCentral(bool Total)
        {
            if (Total)
            {
                this.txtProductoFact.Clear();
            }

            this.hdnIdProductoFacturacion.Clear();
            this.StoreProductosFact.RemoveAll();

            this.GridEventos.Disabled = true;
            this.StoreEventos.RemoveAll();
        }

        /// <summary>
        /// Controla el evento Click al botón de Limpiar en la sección de ingreso de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, DirectEventArgs e)
        {
            LimpiaPanelCentral(true);

            LimpiaPanelLateral();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de productos de facturación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                char[] charsToTrim = { '*', '"', ' ', '/' };
                string comando = e.ExtraParams["Comando"];

                int ID_ProductoFacturacion = Int32.Parse(e.ExtraParams["ID_ProductoFacturacion"]);
                string Descripcion = e.ExtraParams["Descripcion"].ToString().Trim(charsToTrim);
                int ID_TipoImpuesto = Int32.Parse(e.ExtraParams["ID_TipoImpuesto"]);
                string TipoImpuesto = e.ExtraParams["TipoImpuesto"].ToString().Trim(charsToTrim);
                int ID_Unidad = Int32.Parse(e.ExtraParams["ID_Unidad"]);
                string Unidad = e.ExtraParams["Unidad"].ToString().Trim(charsToTrim);
                string ClaveProdServ = e.ExtraParams["ClaveProdServ"].ToString().Trim(charsToTrim);
                string ProductoServicio = e.ExtraParams["ProductoServicio"].ToString().Trim(charsToTrim);

                this.hdnIdProductoFacturacion.Value = ID_ProductoFacturacion;
                FormPanelNuevoPF.Reset();

                switch (comando)
                {
                    case "Eventos":
                        LlenaGridEventos(Convert.ToInt32(this.hdnIdProductoFacturacion.Value));
                        this.GridEventos.Title += " - " + Descripcion;
                        this.GridEventos.Disabled = false;
                        this.WdwNuevoEvento.Title = "Nuevo Evento - " + Descripcion;
                        break;

                    case "Edit":
                        this.txtDescripcion.Text = Descripcion;
                        this.cBoxTipoImpuesto.SetValue(ID_TipoImpuesto);
                        this.cBoxTipoImpuesto.SelectedItem.Text = TipoImpuesto;

                        this.hdnCatUnidad.Value = true;
                        this.hdnIdUnidad.Value = ID_Unidad;
                        this.txtUnidad.Text = Unidad;

                        this.hdnCatProdServ.Value = true;
                        this.hdnClaveProdServ.Value = ClaveProdServ;
                        this.txtProdServ.Text = ProductoServicio;

                        PanelLateral.Title = "Editar Concepto " + this.txtDescripcion.Text;
                        PanelLateral.Collapsed = false;
                        break;

                    //case "Delete":
                    default:
                        string msj = LNProductoFacturacion.BajaProductoFacturacion(
                                Convert.ToInt32(this.hdnIdProductoFacturacion.Value), this.Usuario);

                        if (msj.ToUpper().Contains("NO"))
                        {
                            X.Msg.Notify("Concepto de Facturación", "<br /><b>" + msj +"</b><br />").Show();
                        }
                        else
                        {
                            X.Msg.Notify("Concepto de Facturación", "Eliminado" +
                                "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                            LimpiaPanelCentral(false);
                            LlenaGridProductosFact();
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Concepto de Facturación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de búsqueda en el catálogo de unidades
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnUnidad_Click(object sender, DirectEventArgs e)
        {
            try
            {
                X.Mask.Hide();

                DataTable dtUnidades = DAOProductoFacturacion.ObtieneUnidadesPF(
                    this.txtUnidad.Text, this.Usuario);

                StoreCatalogo.DataSource = dtUnidades;
                StoreCatalogo.DataBind();

                this.GridCatalogos.Title = "Unidades de Facturación";
                this.GridCatalogos.Disabled = false;
            }
            catch (CAppException caEX)
            {
                X.Msg.Alert("Unidades de Facturación", caEX.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Unidades de Facturación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de búsqueda en el catálogo de productos o servicios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnProdServ_Click(object sender, DirectEventArgs e)
        {
            try
            {
                X.Mask.Hide();

                DataTable dtProdServ = DAOProductoFacturacion.ObtieneProductosServiciosPF(
                    this.txtProdServ.Text, this.Usuario);

                StoreCatalogo.DataSource = dtProdServ;
                StoreCatalogo.DataBind();

                this.GridCatalogos.Title = "Productos o Servicios de Facturación";
                this.GridCatalogos.Disabled = false;
            }
            catch (CAppException caEX)
            {
                X.Msg.Alert("Productos o Servicios de Facturación", caEX.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Productos o Servicios de Facturación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de conceptos de facturación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void SeleccionaItemCatalogo(object sender, DirectEventArgs e)
        {
            try
            {
                Int64 IdCatalogo = 0;
                string Clave = String.Empty;
                string Descripcion = String.Empty;
                string json = e.ExtraParams["Values"];

                IDictionary<string, string>[] itemSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (itemSeleccionado == null || itemSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> columna in itemSeleccionado[0])
                {
                    switch (columna.Key)
                    {
                        case "ID": IdCatalogo = Convert.ToInt64(columna.Value); break;
                        case "Clave": Clave = columna.Value; break;
                        case "Descripcion": Descripcion = columna.Value; break;
                        default:
                            break;
                    }
                }

                if (Convert.ToBoolean(this.hdnEsUnidad.Value))
                {
                    this.hdnIdUnidad.Value = IdCatalogo;
                    this.txtUnidad.Text = Descripcion;
                    this.hdnCatUnidad.Value = true;
                }
                else
                {
                    this.hdnClaveProdServ.Value = Clave;
                    this.txtProdServ.Text = Descripcion;
                    this.hdnCatProdServ.Value = true;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Eventos Asociados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Eventos Asociados", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de alta o edición de
        /// productos de facturación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LNProductoFacturacion.CreaModificaProductoFacturacion(
                    String.IsNullOrEmpty(this.hdnIdProductoFacturacion.Value.ToString()) ? -1 :
                    Convert.ToInt32(this.hdnIdProductoFacturacion.Value), this.txtDescripcion.Text,
                    Convert.ToInt32(this.hdnIdUnidad.Value),
                    Convert.ToInt32(this.cBoxTipoImpuesto.SelectedItem.Value),
                    this.hdnClaveProdServ.Value.ToString(), this.Usuario);

                X.Msg.Notify("Concepto de Facturación", "Concepto guardado" + 
                    "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LimpiaPanelLateral();

                this.txtProductoFact.Text = this.txtDescripcion.Text;
                LimpiaPanelCentral(false);
                LlenaGridProductosFact();
            }

            catch (CAppException caEX)
            {
                X.Msg.Alert("Concepto de Facturación", caEX.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Concepto de Facturación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el grid de eventos asociados al producto de facturación 
        /// con la información de base de datos
        /// </summary>
        /// <param name="idProductoFact">Identificador del producto de facturación</param>
        protected void LlenaGridEventos(int idProductoFact)
        {
            try
            {
                StoreEventos.RemoveAll();

                DataTable dtEventos = DAOProductoFacturacion.ObtieneEventosProductosFacturacion(
                    idProductoFact, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dtEventos.Rows.Count < 1)
                {
                    X.Msg.Alert("Eventos Asociados", "No existen coincidencias con la búsqueda solicitada.").Show();
                    return;
                }
                else
                {
                    StoreEventos.DataSource = dtEventos;
                    StoreEventos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Eventos Asociados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Eventos Asociados", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al link de asociación de más eventos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void AsociarEventos(object sender, DirectEventArgs e)
        {
            try
            {
                StoreCatEventos.RemoveAll();

                DataTable dtEventos = DAOProductoFacturacion.ListaEventosPorAsociar(
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                StoreCatEventos.DataSource = dtEventos;
                StoreCatEventos.DataBind();

                WdwEventos.Show();
            }
            catch (CAppException caEX)
            {
                X.Msg.Alert("Eventos", caEX.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Eventos", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click en el botón de búsqueda de conceptos de facturación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnAsociarEventos_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel losEventos = this.GridCatEventos.SelectionModel.Primary as RowSelectionModel;

            if (losEventos.SelectedRows.Count == 0)
            {
                X.Msg.Alert("Asociar Eventos", "Por favor, selecciona al menos un evento.").Show();
                return;
            }

            int idProductoFact = Convert.ToInt32(hdnIdProductoFacturacion.Value);

            foreach (SelectedRow evento in losEventos.SelectedRows)
            {
                try
                {
                    LNProductoFacturacion.AsociaEventoProductoFacturacion(
                        idProductoFact, int.Parse(evento.RecordID), this.Usuario);
                }
                catch (CAppException caEX)
                {
                    X.Msg.Alert("Asociar Eventos", caEX.Mensaje()).Show();
                }

                catch (Exception ex)
                {
                    Loguear.Error(ex, this.Usuario.ClaveUsuario);
                    X.Msg.Alert("Asociar Eventos", ex.Message).Show();
                }
            }

            X.Msg.Notify("", "Eventos asociados" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

            WdwEventos.Hide();
            LlenaGridEventos(idProductoFact);
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de eventos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void DesasociarEvento(object sender, DirectEventArgs e)
        {
            try
            {
                int IdEvento = Convert.ToInt32(e.ExtraParams["ID_Evento"]);
                int IdProdFact = Convert.ToInt32(hdnIdProductoFacturacion.Value);

                LNProductoFacturacion.DesasociaEventoProductoFacturacion(
                    IdProdFact, IdEvento, this.Usuario);

                X.Msg.Notify("", "Evento desasociado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridEventos(IdProdFact);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Desasociar Evento - Concepto de Facturación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Nuevo Evento
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void btnGuardaEvento_Click(object sender, DirectEventArgs e)
        {
            try
            {
                int idProductoFact = Convert.ToInt32(this.hdnIdProductoFacturacion.Value);
                EventoFacturacion elEvento = new EventoFacturacion();

                elEvento.ClaveEvento = txtClaveEvento.Text;
                elEvento.Descripcion = txtDescripcionEv.Text;
                elEvento.DescripcionEstadoCuenta = txtDescrEdoCta.Text;

                string msj = LNProductoFacturacion.CreaEventoYLigaProducto(idProductoFact, elEvento, this.Usuario);

                if (msj.ToUpper().Contains("OK"))
                {
                    X.Msg.Notify("", "Evento creado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    WdwNuevoEvento.Hide();

                    LlenaGridEventos(idProductoFact);
                }
                else
                {
                    X.Msg.Alert("Nuevo Evento", "<br /><b>" + msj + "</b><br />").Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Evento", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Nuevo Evento", ex.Message).Show();
            }
        }
    }
}