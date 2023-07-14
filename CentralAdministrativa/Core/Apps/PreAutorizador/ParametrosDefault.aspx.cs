using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;

namespace PreAutorizador
{
    public partial class ParametrosDefault : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Preautorizador Parámetros Default
        private LogHeader LH_PreautParamDefault = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Parámetros Default o Base
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_PreautParamDefault.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_PreautParamDefault.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_PreautParamDefault.User = this.Usuario.ClaveUsuario;
            LH_PreautParamDefault.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_PreautParamDefault);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ParametrosDefault Page_Load()");

                if (!IsPostBack)
                {
                    EstableceSubEmisores();

                    EstableceCombos();
                }

                log.Info("TERMINA ParametrosDefault Page_Load()");
            }

            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        /// <summary>
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI logPCI = new LogPCI(LH_PreautParamDefault);

            logPCI.Info("INICIA ListaColectivasSubEmisor()");
            DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_PreautParamDefault);
            logPCI.Info("TERMINA ListaColectivasSubEmisor()");

            List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

            foreach (DataRow drCol in dtSubEmisores.Rows)
            {
                var colectivaCombo = new ColectivaComboPredictivo()
                {
                    ID_Colectiva = Convert.ToInt64(drCol["ID_Colectiva"].ToString()),
                    ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                    NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                };
                ComboList.Add(colectivaCombo);
            }

            this.StoreSubEmisores.DataSource = ComboList;
            this.StoreSubEmisores.DataBind();
        }

        /// <summary>
        /// Establece los valores de los combos prestablecidos con la información de base de datos
        /// </summary>
        protected void EstableceCombos()
        {
            LogPCI pCI = new LogPCI(LH_PreautParamDefault);

            pCI.Info("INICIA ListaTipoPMAPreaut()");
            this.StoreTipoParametroMA.DataSource = DAOProducto.ListaTipoPMAPreaut(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_PreautParamDefault);
            pCI.Info("TERMINA ListaTipoPMAPreaut()");

            this.StoreTipoParametroMA.DataBind();
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Generación de Nuevo Producto
        /// </summary>
        [DirectMethod(Namespace = "AdminProdParabilia")]
        public void StopMask()
        {
            Thread.Sleep(200);
            X.Mask.Hide();
        }

        /// <summary>
        /// Llena el grid de resultados de productos con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI unLog = new LogPCI(LH_PreautParamDefault);

            try
            {
                LimpiaSeleccionPrevia();

                unLog.Info("INICIA ObtieneProductosPorClaveDescOColectiva()");
                DataSet dsProductos = DAOProducto.ObtieneProductosPorClaveDescOColectiva(
                    String.IsNullOrEmpty(this.cBoxSubEmisor.SelectedItem.Value) ? -1 :
                    int.Parse(this.cBoxSubEmisor.SelectedItem.Value),
                    String.IsNullOrEmpty(this.txtProducto.Text) ? null : this.txtProducto.Text, 
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_PreautParamDefault);
                unLog.Info("TERMINA ObtieneProductosPorClaveDescOColectiva()");

                if (dsProductos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Productos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreProductos.DataSource = dsProductos;
                    StoreProductos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al obtener los Productos del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, restableciendo
        /// todos los controles, páneles y grids a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            this.cBoxSubEmisor.Reset();
            this.txtProducto.Reset();
            this.StoreProductos.RemoveAll();

            LimpiaSeleccionPrevia();

            PanelCentralProds.Disabled = true;
        }
   
        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// un producto en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            cBoxTipoParametroMA.Reset();
            StoreValoresParametros.RemoveAll();
        }

        /// <summary>
        /// Restablece los controles de la ventana de edición de parámetros
        /// </summary>
        protected void LimpiaVentanaParams()
        {
            this.FormPanelValorParamTxt.Reset();
            this.txtParametro.Reset();
            this.lblInstruc.Text = "";

            this.txtValorParFloat.Reset();
            this.txtValorParFloat.Hidden = true;

            this.txtValorParInt.Reset();
            this.txtValorParInt.Hidden = true;

            this.txtValorParString.Reset();
            this.txtValorParString.Hidden = true;

            this.cBoxValorPar.Reset();
            this.cBoxValorPar.Hidden = true;
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultadosPP_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdProducto = 0;
                int ID_TipoProducto = 0;
                string ClaveProducto = "", NombreProducto = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] producto = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in producto[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Producto": IdProducto = int.Parse(column.Value); break;
                        case "Clave": ClaveProducto = column.Value; break;
                        case "Descripcion": NombreProducto = column.Value; break;
                        case "ID_TipoProducto": ID_TipoProducto = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnIdProducto.Value = IdProducto;

                PanelCentralProds.Disabled = true;
                PanelCentralProds.Title = ClaveProducto +  " - " + NombreProducto;
                PanelCentralProds.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_PreautParamDefault);
                unLog.ErrorException(ex);
                X.Msg.Alert("Productos", "Ocurrió un error al obtener la información del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de tipo parámetro multiasignación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaTipoParamMA(object sender, EventArgs e)
        {
            LlenaParametrosProducto();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros del producto,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosProducto()
        {
            LogPCI log = new LogPCI(LH_PreautParamDefault);

            try
            {
                int IdProducto = Convert.ToInt32(this.hdnIdProducto.Value);

                log.Info("INICIA ListaPMAsPreautProdPorTipo()");
                this.StoreValoresParametros.DataSource = DAOParametroMA.ListaPMAsPreautProdPorTipo(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdProducto, LH_PreautParamDefault);
                log.Info("TERMINA ListaPMAsPreautProdPorTipo()");

                this.StoreValoresParametros.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Parámetros del Producto").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_PreautParamDefault);
            pCI.Info("EjecutarComandoParametros()");

            try
            {
                ParametroValor unParametro = new ParametroValor();
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] parametroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (parametroSeleccionado == null || parametroSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in parametroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_ParametroMultiasignacion": unParametro.ID_Parametro = int.Parse(column.Value); break;
                        case "ID_Plantilla": unParametro.ID_Plantilla = int.Parse(column.Value); break;
                        case "ID_ValorParametroMultiasignacion": unParametro.ID_ValordelParametro = int.Parse(column.Value); break;
                        case "Nombre": unParametro.Nombre = column.Value; break;
                        case "Descripcion": unParametro.Descripcion = column.Value; break;
                        case "Valor": unParametro.Valor = column.Value; break;
                        case "TipoDato": unParametro.TipoDato = column.Value; break;
                        case "Instrucciones": unParametro.Instrucciones = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                this.hdnIdParametroMA.Value = unParametro.ID_Parametro;
                this.hdnIdPlantilla.Value = unParametro.ID_Plantilla;
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;

                switch (comando)
                {
                    case "Edit":
                        LimpiaVentanaParams();

                        switch (unParametro.TipoDato.ToUpper())
                        {
                            case "BOOL":
                            case "BOOLEAN":
                                this.cBoxValorPar.Value = unParametro.Valor;
                                this.cBoxValorPar.Hidden = false;
                                break;

                            case "FLOAT":
                                this.txtValorParFloat.Value = unParametro.Valor;
                                this.txtValorParFloat.Hidden = false;
                                break;

                            case "INT":
                                this.txtValorParInt.Value = unParametro.Valor;
                                this.txtValorParInt.Hidden = false;
                                break;

                            case "STRING":
                                this.txtValorParString.Value = unParametro.Valor;
                                this.txtValorParString.Hidden = false;
                                break;
                        }

                        this.txtParametro.Text = unParametro.Descripcion;
                        this.lblInstruc.Text = unParametro.Instrucciones;
                        this.WdwValorParametro.Title += " - " + unParametro.Nombre;
                        this.WdwValorParametro.Show();
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambio de las ventanas
        /// de edición de valor del parámetro de contrato
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorParametro_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_PreautParamDefault);

            try
            {
                ParametroValor elParametro = new ParametroValor();

                elParametro.ID_Parametro = Convert.ToInt32(this.hdnIdParametroMA.Value);
                elParametro.ID_Plantilla = Convert.ToInt32(this.hdnIdPlantilla.Value);
                elParametro.ID_ValordelParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);
                elParametro.Valor = String.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                   String.IsNullOrEmpty(this.txtValorParInt.Text) ?
                   String.IsNullOrEmpty(this.txtValorParString.Text) ?
                   this.cBoxValorPar.SelectedItem.Value : this.txtValorParString.Text :
                   this.txtValorParInt.Text : this.txtValorParFloat.Text;
                //Validación para permitir valores en blanco
                elParametro.Valor = String.IsNullOrEmpty(elParametro.Valor) ? "" : elParametro.Valor;

                log.Info("INICIA ModificaValorParametro()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_PreautParamDefault, "_ParametrosDefault");
                log.Info("TERMINA ModificaValorParametro()");

                this.WdwValorParametro.Hide();

                X.Msg.Notify("Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaParametrosProducto();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al modificar el Parámetro").Show();
            }
        }
    }
}
