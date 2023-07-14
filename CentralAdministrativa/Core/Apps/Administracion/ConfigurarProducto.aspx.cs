using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;

namespace Administracion
{
    public partial class ConfigurarProducto : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    if (!X.IsAjaxRequest)
                    {
                        this.StoreCadenaComercial.DataSource =
                            //DAOGruposMA.ListaCadenasComercialesAutMC(this.Usuario,
                            DAOParametroMA.ListaCadenasComerciales(this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                        );

                        this.StoreCadenaComercial.DataBind();
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
                    DAOGruposMA.ConsultaProductosPorNombre(
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

                LlenaGridParametros(idProducto);
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
        protected void LlenaGridParametros(Int64 Id_Producto)
        {
            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();

                this.GridPropiedades.SetSource(source);

                foreach (ParametroGMA parametroGMA in 
                            DAOGruposMA.ObtenerParametrosProductoTDC(
                                Id_Producto, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())))
                {
                    PropertyGridParameter GridProp = new PropertyGridParameter(parametroGMA.Nombre, parametroGMA.Valor);
                    GridProp.DisplayName = parametroGMA.Nombre + " - " + parametroGMA.Descripcion;

                    GridPropiedades.AddProperty(GridProp);

                    PropertyGridParameter GridParams = new PropertyGridParameter(parametroGMA.Nombre, parametroGMA.Valor);
                    GridParamsHidden.AddProperty(GridParams);
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
        /// Controla el evento Click al botón Guardar del panel Parámetros del Producto,
        /// invocando la actualización de ellos en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnGuardarParams_Click(object sender, DirectEventArgs e)
        {
            try
            {
                List<ParametroGMA> cambios = new List<ParametroGMA>();

                //Obtiene los parámetros que cambiaron
                foreach (PropertyGridParameter param in this.GridPropiedades.Source)
                {
                    foreach (PropertyGridParameter param2 in this.GridParamsHidden.Source)
                    {
                        if ((param2.Name == param.Name) && (param.Value != param2.Value))
                        {
                            ParametroGMA parametro = new ParametroGMA(param.Name, param.Value.ToString());
                            cambios.Add(parametro);
                        }
                    }
                }

                if (cambios.Count > 0)
                {
                    //Guarda los valores en BD
                    LNProductos.ActualizaParametrosProducto(cambios, Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()), this.Usuario);

                    LlenaGridParametros(Int64.Parse(HttpContext.Current.Session["ID_GrupoMA"].ToString()));

                    X.Msg.Notify("Parámetros del Producto", "Modificación de Parámetros <br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();
                }
            }

            catch (Exception)
            {
                X.Msg.Notify("Parámetros del Producto", "Ocurrió un Error en la Modificación de Parámetros del Producto").Show();
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
                    DAOGruposMA.ObtenerRangosProducto(Id_Producto, this.Usuario,
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
                LNProductos.ActualizaRangosProducto(idRango, inicial, final, this.Usuario);
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

    }
}