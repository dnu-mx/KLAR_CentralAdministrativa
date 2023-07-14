using DALAutorizador.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using DALLealtad.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;

namespace Lealtad
{
    public partial class Beneficios : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// 
        /// </summary>
        protected class CadenaComboPredictivo
        {
            public Int64 ID_Cadena { get; set; }
            public String ClaveCadena { get; set; }
            public String NombreComercial { get; set; }
        }

        /// <summary>
        /// Realiza y controla la carga de la página Beneficios
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                    DataSet dsCadenas = DAOEcommerce.ListaCadenas(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    

                    List<CadenaComboPredictivo> ListaCadenas = new List<CadenaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenas.Tables[0].Rows)
                    {
                        var ComboCadenas = new CadenaComboPredictivo()
                        {
                            ID_Cadena = Convert.ToInt64(cadena["ID_Cadena"].ToString()),
                            ClaveCadena = cadena["ClaveCadena"].ToString(),
                            NombreComercial = cadena["NombreComercial"].ToString()
                        };
                        ListaCadenas.Add(ComboCadenas);
                    }

                    StoreCadena.DataSource = ListaCadenas;
                    StoreCadena.DataBind();

                    PreLlenaFormPanelConfiguracion();

                    PreLlenaFormPanelGenerarCupones();                    

                    FormPanelConfiguracion.Show();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void PreLlenaFormPanelConfiguracion()
        {
            StoreTipoCupon.DataSource = DAOEcommerce.ListaTiposCupon(
                       this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoCupon.DataBind();
        }

        protected void PreLlenaFormPanelGenerarCupones()
        {
            StoreTipoEmision.DataSource = DAOEcommerce.ListaTiposEmision(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreTipoEmision.DataBind();

            StoreAlgoritmos.DataSource = DAOEcommerce.ListaAlgoritmos(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            StoreAlgoritmos.DataBind();

            dfExpiracionCupon.MinDate = DateTime.Today.AddDays(1);
            dfExpiracionCupon.SetValue(DateTime.Today.AddDays(1));
        }

        /// <summary>
        /// Llena el grid de resultado de búsqueda de promociones
        /// </summary>
        protected void LlenarGridResultados()
        {
            try
            {
                if (String.IsNullOrEmpty(this.cBoxCadena.SelectedItem.Value) &&
                    String.IsNullOrEmpty(this.txtClave.Text))
                {
                    X.Msg.Alert("Búsqueda de Promociones", "Ingrese al menos un criterio de búsqueda").Show();
                    return;
                }

                DataSet dsResultados = DAOEcommerce.ListaPromociones(
                    String.IsNullOrEmpty(this.cBoxCadena.SelectedItem.Value) ? -1 : 
                    int.Parse(this.cBoxCadena.SelectedItem.Value), this.txtClave.Text,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords >= 100)
                {
                    X.Msg.Alert("Búsqueda de Promociones", "Demasiadas coincidencias, por favor afine su búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Búsqueda de Promociones", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                GridResultados.GetStore().DataSource = dsResultados;
                GridResultados.GetStore().DataBind();
            }

            catch (CAppException caEx)
            {
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Promociones", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Promociones", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de búsqueda, invocando la búsqueda
        /// de promociones a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Controla el evento Refresh en el grid de resultados, invocando nuevamente
        /// la búsqueda de promociones a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento Refresh Data del Store Clientes</param>
        protected void StorePromociones_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de un cliente en
        /// el Grid de Resultados Clientes
        /// </summary>
        protected void limpiaSeleccionPrevia()
        {
            FormPanelConfiguracion.Reset();

            FormPanelGenerarCupones.Reset();
            dfExpiracionCupon.MinDate = DateTime.Today.AddDays(1);
            dfExpiracionCupon.SetValue(DateTime.Today.AddDays(1));

            FormPanelConfiguracion.Show();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de promociones dentro
        /// del Grid de Resultados Promociones
        /// </summary>
        /// <param name="esBtnLimpiar">Indica si el método de origen fue el clic al botón Limpiar</param>
        protected void limpiaBusquedaPrevia(bool esBtnLimpiar)
        {
            if (esBtnLimpiar)
            {
                this.cBoxCadena.Reset();
                this.txtClave.Reset();
            }

            StorePromociones.RemoveAll();

            FormPanelConfiguracion.Reset();

            FormPanelGenerarCupones.Reset();
            this.txtClavePromocion.Text = "";

            FormPanelConfiguracion.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de búsqueda, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia(true);
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdPromocion = 0;
                string ClavePromo = "", Promo = "", Cadena = "", ClaveCadena = "", TipoCupon = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] promoSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in promoSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Promocion": IdPromocion = int.Parse(column.Value); break;
                        case "ClavePromocion": ClavePromo = column.Value; break;
                        case "Cadena": Cadena = column.Value; break;
                        case "ClaveCadena": ClaveCadena = column.Value; break;
                        case "Descripcion": Promo = column.Value; break;
                        case "TipoCupon": TipoCupon = column.Value; break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();

                HttpContext.Current.Session.Add("ID_Promocion", IdPromocion);
                HttpContext.Current.Session.Add("ClaveCadenaPromo", ClaveCadena);

                LlenaDatosPaneles(Promo, ClavePromo, Cadena, TipoCupon);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Resultados", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Resultados", "Ocurrió un Error al Seleccionar la Promoción").Show();
            }
        }

        /// <summary>
        /// Llena el FieldSet de Configuración con los datos seleccionados del Grid de Resultados
        /// </summary>
        protected void LlenaDatosPaneles(string promo, string clavePromo, string cadena, string tipocupon)
        {
            FormPanelConfiguracion.Reset();

            this.txtCadena.Text = cadena;
            this.txtClavePromo.Text = this.txtClavePromocion.Text = clavePromo;
            this.txtPromocion.Text = promo;
            this.txtTipoCupon.Text = tipocupon;
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de Configuración,
        /// llamando a la actualización de la promoción en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int IdPromocion = int.Parse(HttpContext.Current.Session["ID_Promocion"].ToString());
                string claveCadena = HttpContext.Current.Session["ClaveCadenaPromo"].ToString();

                LNEcommerce.ModificaTipoCuponPromo(IdPromocion,
                    int.Parse(this.cBoxTipoCupon.SelectedItem.Value), this.Usuario);

                LNEcommerce.ModificaCadenaEnAutorizador(claveCadena, this.txtCadena.Text, this.Usuario);

                DAOEcommerce.ActualizaPromocionAut(this.txtPromocion.Text, this.txtClavePromo.Text, 
                    claveCadena, this.Usuario);
                
                X.Msg.Notify("", "Promoción Configurada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Configuración", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Configuración", "Ocurrió un Error al Guardar los Cambios de la Promoción").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Generar Cupones del formulario Generar Cupones,
        /// llamando a la generación de cupones en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGenerarCupones_Click(object sender, EventArgs e)
        {
            try
            {
                Cupon cupon = new Cupon();

                //cupon.ClaveEvento = this.cBoxTipoCuentaCupon.SelectedItem.Value;
                //cupon.CantidadCupones = int.Parse(this.nfCantCupones.Text);

                string claveCadena = HttpContext.Current.Session["ClaveCadenaPromo"].ToString();

                //DAOEcommerce.ActualizaPromocionAut(this.txtPromocion.Text, this.txtClavePromo.Text,
                //    claveCadena, this.Usuario);

                X.Msg.Notify("", "Promoción Configurada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Configuración", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Configuración", "Ocurrió un Error al Guardar los Cambios de la Promoción").Show();
            }
        }
    }
}