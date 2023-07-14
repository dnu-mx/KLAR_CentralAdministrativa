using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace Administracion
{
    public partial class ConfiguradorTarjetas : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Configurador de Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    if (!X.IsAjaxRequest)
                    {
                        this.StoreCadenaComercial.DataSource =
                            DAOConfiguradorTarjetas.ListarCadenasComercialesAutMC(this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                        );

                        this.StoreCadenaComercial.DataBind();

                        EstableceRolesAuditables();
                    }
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Llena el grid de productos relacionados con la cadena comercial con los 
        /// datos obtenidos de base de datos
        /// </summary>
        protected void LlenaGridProductos()
        {
            try
            {
                if (String.IsNullOrEmpty(this.cmbCadenaComercial.SelectedItem.Value))
                {
                    return;
                }

                this.StoreProductos.DataSource =
                    DAOConfiguradorTarjetas.ConsultarProductosPorNombre(
                    this.txtProducto.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.StoreProductos.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Productos", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Consulta de Productos", "Ocurrió un Error al Consultar los Productos").Show();
            }

        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, invocando la consulta de productos
        /// en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarProductos_Click(object sender, EventArgs e)
        {
            LlenaGridProductos();

            //this.frmGrafica.Show();
        }

        /// <summary>
        /// Controla el evento Refresh al botón correspondiente del grid de productos, 
        /// invocando la consulta de productos en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento refresh data del store que se ejecutó</param>
        protected void RefreshGridProductos(object sender, StoreRefreshDataEventArgs e)
        {
            LlenaGridProductos();
        }

        /// <summary>
        /// Controla el evento selección/deselección a una fila del Grid de Productos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {
            PanelParametros.Collapsed = true;
            Panel1.Collapsed = true;
        }

        /// <summary>
        /// Controla el evento Doble Clic al Grid de Productos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void GridProductos_DblClik(object sender, DirectEventArgs e)
        {
            PanelParametros.Collapsed = true;
            Panel1.Collapsed = true;

            try
            {
                int idProducto = int.Parse(e.ExtraParams["ID_GrupoMA"].ToString());
                string producto = e.ExtraParams["Descripcion"].ToString();

                PanelParametros.Title = this.PanelParametros.Title + producto;
                FormPanelRangos.Title = this.FormPanelRangos.Title + producto;

                HttpContext.Current.Session.Add("ID_GrupoMA", idProducto);

                LlenaGridParametros(idProducto, false);
                LlenaGridRangos(idProducto);
            }

            catch (Exception)
            {
            }

            PanelParametros.Collapsed = false;
            Panel1.Collapsed = false;
        }

        /// <summary>
        /// Llena el grid de parámetros del producto con los datos obtenidos de la consulta a base de datos
        /// </summary>
        /// <param name="Id_Producto">Identificador del producto seleccionado</param>
        protected void LlenaGridParametros(Int64 Id_Producto, Boolean esVistaPorBoton)
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                this.GridPropiedades.SetSource(source);

                this.btnValoresActuales.Hidden = true;
                this.btnValoresPendientes.Hidden = true;

                Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());


                //Validaciones de despliegue y modo sólo lectura del grid y del botón Guardar
                if (esEjecutor)
                {
                    if (esVistaPorBoton)
                    {
                        GridPropiedades.AddListener("BeforeEdit", "removePPGridEdition");
                    }
                    else
                    {
                        GridPropiedades.RemoveListener("BeforeEdit", "removePPGridEdition");
                    }
                }
                else
                {
                    GridPropiedades.AddListener("BeforeEdit", "removePPGridEdition");
                    this.btnGuardarParams.Hidden = true;
                }


                foreach (ParametroGMA_V2 parametroGMA in 
                            DAOConfiguradorTarjetas.ListarParametrosProductoTDC(
                                Id_Producto, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())))
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(parametroGMA.Nombre, parametroGMA.Valor);
                    GridProp.DisplayName = parametroGMA.Nombre + " - " + parametroGMA.Descripcion;
                    GridProp.Value = parametroGMA.ValorActual;
                    
                    GridProp.Renderer.Fn = "restoreValueRenderer";

                    if (esEjecutor)
                    {
                        EstableceVistaValor(parametroGMA, GridProp, esVistaPorBoton);
                    }
                    else if (esAutorizador)
                    {
                        EstableceVistaValor(parametroGMA, GridProp, esVistaPorBoton);
                        this.btnAutorizar.Hidden = false;
                    }
                      
                    GridPropiedades.AddProperty(GridProp);
                    GridParamsHidden.AddProperty(GridProp);
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros del Producto", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Parámetros del Producto", "Ocurrió un Error al Consultar los Parámetros del Producto").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elParametro"></param>
        /// <param name="pgParameter"></param>
        /// <param name="vistaPorBoton"></param>
        /// <returns></returns>
        protected void EstableceVistaValor(ParametroGMA_V2 elParametro, PropertyGridParameter pgParameter, 
            Boolean vistaPorBoton)
        {
            if (!String.IsNullOrEmpty(elParametro.ValorPendiente) &&
                    elParametro.ValorActual != elParametro.ValorPendiente)
            {
                pgParameter.Renderer.Fn = vistaPorBoton ? "currentValueRenderer" : "pendingValueRenderer";
                pgParameter.Value = vistaPorBoton ? elParametro.ValorActual: elParametro.ValorPendiente;

                btnValoresActuales.Hidden = vistaPorBoton;
            }
        }


        /// <summary>
        /// Establece en variables de sesión los roles auditables para el control de
        /// flujos en la página
        /// </summary>
        protected void EstableceRolesAuditables()
        {
            this.Usuario.Roles.Sort();
            HttpContext.Current.Session.Add("EsAutorizador", false);
            HttpContext.Current.Session.Add("EsEjecutor", false);

            foreach (string rol in this.Usuario.Roles)
            {
                if (rol.Contains("Autor"))
                {
                    HttpContext.Current.Session.Add("EsAutorizador", true);
                }
                else if (rol.Contains("Ejec"))
                {
                    HttpContext.Current.Session.Add("EsEjecutor", true);
                }
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel Parámetros del Producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnGuardarParams_Click(object sender, DirectEventArgs e)
        {
            try
            {
                Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());
                
                List<ParametroGMA_V2> cambios = new List<ParametroGMA_V2>();

                //Obtiene los parámetros que cambiaron
                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    foreach (PropertyGridParameter param2 in this.GridParamsHidden.Source)
                    {
                        if (param2.Name == param.Name)
                        {
                            if (param.Value != param2.Value)
                            {
                                ParametroGMA_V2 parametro = new ParametroGMA_V2() { Nombre = param.Name, Valor = param.Value.ToString() };
                                cambios.Add(parametro);
                                break;
                            }
                            else break;
                        }
                    }
                }

                if (cambios.Count > 0)
                {
                    //Si el usuario es Ejecutor Y Autorizador, 
                    //actualiza directamente los valores en base de datos
                    if (esEjecutor && esAutorizador)
                    {
                        LNConfiguradorTarjetas.ActualizaParametrosProducto(cambios,
                            Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()),
                            this.Usuario);

                    }
                    //Si sólo tiene el rol de Ejecutor, se lleva el control de cambios
                    else
                    {
                        LNConfiguradorTarjetas.ControlaCambiosParametrosProducto(cambios,
                            Int64.Parse(this.cmbCadenaComercial.SelectedItem.Value),
                            Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()),
                            Path.GetFileName(Request.Url.AbsolutePath), this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    }
       
                    LlenaGridParametros(Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()), false);

                    X.Msg.Notify("Parámetros del Producto", "Modificación de Parámetros <br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();
                }
            }

            catch (Exception)
            {
                X.Msg.Notify("Parámetros del Producto", "Ocurrió un Error en la Modificación de Parámetros del Producto").Show();
            }
        }


        /// <summary>
        /// Controla el evento Click al botón Autorizar del panel Parámetros del Producto
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnAutorizar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                //Guarda los valores en BD
                LNConfiguradorTarjetas.AutorizaCambiosParametrosProducto
                    (Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()), this.Usuario);

                LlenaGridParametros(Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()), false);

                X.Msg.Notify("Parámetros del Producto", "Autorización de Cambios <br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();
            }

            catch (Exception)
            {
                X.Msg.Notify("Parámetros del Producto", "Ocurrió un Error en la Autorización de Cambios de Parámetros del Producto").Show();
            }
        }

        /// <summary>
        /// Llena el GridPanel Rangos con los datos del producto seleccionado
        /// </summary>
        /// <param name="Id_Producto">Identificador del producto</param>
        protected void LlenaGridRangos(Int64 Id_Producto)
        {
            try
            {
                GridRangos.GetStore().RemoveAll();

                GridRangos.GetStore().DataSource =
                    DAOConfiguradorTarjetas.ObtenerRangosProducto(Id_Producto, this.Usuario,
                                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridRangos.GetStore().DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Rangos de Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Rangos de Tarjetas", "Ocurrió un Error al Consultar los Rangos de las Tarjetas").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento de Click al botón de Guardar Cambios del grid de Rangos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del eventó que se ejecutó</param>
        [DirectMethod(Namespace = "Administracion")]
        public void GuardarRangos(int idRango, string inicial, string final)
        {
            try
            {
                LNConfiguradorTarjetas.ActualizaRangosProducto(idRango, inicial, final, this.Usuario);
                X.Msg.Notify("Rangos de Tarjetas", "Modificación de Rangos <br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Rangos de Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Rangos de Tarjetas", "Ocurrió un Error al Guardar los Cambios de los Rangos de Tarjetas").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Valores Actuales del panel de parámetros, 
        /// invocando la visualización de dichos valores de los datos en el el Grid Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValoresActuales_Click(object sender, EventArgs e)
        {
            LlenaGridParametros(Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()), true);

            this.btnValoresActuales.Hidden = true;
            this.btnValoresPendientes.Hidden = false;
        }

        /// <summary>
        /// Controla el evento Click al botón Valores Pendientes de Autorización del panel de parámetros,
        /// invocando la visualización de dichos valores de los datos en el el Grid Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValoresPendientes_Click(object sender, EventArgs e)
        {
            LlenaGridParametros(Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()), false);

            this.btnValoresActuales.Hidden = false;
            this.btnValoresPendientes.Hidden = true;
        }
    }
}