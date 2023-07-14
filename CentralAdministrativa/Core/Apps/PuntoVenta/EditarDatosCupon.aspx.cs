using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using DALPuntoVentaWeb.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace TpvWeb
{
    public partial class EditarDatosCupon : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Editar Datos de Cupón
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataSet dsCadenas = DAOPromociones.ObtieneColectivasFiltrosDatosCupones(-1, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                    List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var cadenaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                            ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                            NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                        };
                        cadenaList.Add(cadenaCombo);
                    }

                    StoreCadena.DataSource = dsCadenas;
                    StoreCadena.DataBind();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento de selección de un ítem del combo de cadenas comerciales, llenando
        /// el combo de promociones con los datos de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void LlenaPromociones(object sender, EventArgs e)
        {
            try
            {
                StorePromocion.RemoveAll();
                cmbPromocion.Reset();

                StorePromocion.DataSource = DAOPromociones.ObtieneColectivasFiltrosDatosCupones(
                    int.Parse(this.cmbCadena.SelectedItem.Value), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StorePromocion.DataBind();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Editar Datos de Cupón", "Ocurrió un Error al Obtener la Lista de Promociones").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            StoreCupones.RemoveAll();
            FormPanelFiltros.Reset();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de filtros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscaDatosYLigaGrid();
        }

        /// <summary>
        /// Consulta los cupones con los datos del panel de filtro, validándolos y ligando la
        /// información obtenida al Grid correspondiente
        /// </summary>
        private void BuscaDatosYLigaGrid()
        {
            try
            {
                StoreCupones.RemoveAll();

                DataSet dsCupones = DAOPromociones.ConsultaCuponesPromocion(
                    int.Parse(this.cmbCadena.SelectedItem.Value),
                    String.IsNullOrEmpty(this.cmbPromocion.SelectedItem.Value) ? 0 : int.Parse(this.cmbPromocion.SelectedItem.Value),
                    this.txtClaveCupon.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsCupones.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Búsqueda de Cupones", "No Existen Cupones con los Filtros Indicados.").Show();
                }

                else
                {
                    GridPanelCupones.GetStore().DataSource = dsCupones;
                    GridPanelCupones.GetStore().DataBind();
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Cupones", ex.Message).Show();
            }
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
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] cuponSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (cuponSeleccionado == null || cuponSeleccionado.Length < 1)
                {
                    return;
                }

                this.FormPanelEditaOperacion.Reset();

                foreach (KeyValuePair<string, string> dato in cuponSeleccionado[0])
                {
                    switch (dato.Key)
                    {
                        case "ID_Operacion": txtIdOperacion.Text = dato.Value; break;
                        case "NumCupon": txtCupon.Text = dato.Value; break;
                        case "Fecha": dfFecha.Text = dato.Value; break;
                        case "Hora": dfFecha.Text += " " + dato.Value; break;
                        case "Promocion": txtPromocion.Text = dato.Value; break;
                        case "Vigencia": txtVigencia.Text = dato.Value; break;
                        case "OrigenEmision": txtOrigenEmision.Text = dato.Value; break;
                        case "Autorizacion": txtAutorizacion.Text = dato.Value; break;
                        case "Operador": txtOperador.Text = dato.Value; break;
                        case "Ticket": txtTicket.Text = dato.Value; break;
                        case "FormaPago": cmbFormaPago.SelectedItem.Text = dato.Value; break;
                        case "ClaveCadena": txtClaveCadena.Text = dato.Value; break;
                        case "NombreCadena": txtNombreCadena.Text = dato.Value; break;
                        case "Sucursal": txtSucursal.Text = dato.Value; break;
                        default:
                            break;
                    }
                }

                String command = (String)e.ExtraParams["Comando"];

                switch (command)
                {
                    case "Edit":
                        this.WindowEditaOperacion.Show();
                        break;

                    default:
                        break;
                }
            }

            catch (Exception)
            {
                X.Msg.Alert("Edición de Cupón", "Ocurrió un Error al Seleccionar el Cupón").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de Editar Cupón,
        /// invocando la modificación de los datos del cupón en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                LNPromociones.ModificaDatosCupon(int.Parse(txtIdOperacion.Text),
                    int.Parse(cmbCadena.SelectedItem.Value), txtCupon.Text,
                    txtTicket.Text, cmbFormaPago.SelectedItem.Value, this.Usuario);
                X.Msg.Notify("Edición de Cupón", "Datos del Cupón Modificados <br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Edición de Cupón", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Edición de Cupón", "Ocurrió un Error al Guardar los Datos del Cupón").Show();
            }

            finally
            {
                BuscaDatosYLigaGrid();
                this.FormPanelEditaOperacion.Reset();
                this.WindowEditaOperacion.Hide();
            }
        }
    }
}