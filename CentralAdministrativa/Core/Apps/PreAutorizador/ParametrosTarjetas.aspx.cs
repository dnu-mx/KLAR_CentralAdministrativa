using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using Utilerias;

namespace PreAutorizador
{
    /// <summary>
    /// Realiza y controla la carga de la página Administración de Cuentas
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class ParametrosTarjetas : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Preautorizador Parámetros Tarjetas
        private LogHeader LH_PreautParamTarj = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Parámetros de Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_PreautParamTarj.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_PreautParamTarj.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_PreautParamTarj.User = this.Usuario.ClaveUsuario;
            LH_PreautParamTarj.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_PreautParamTarj);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ParametrosTarjetas Page_Load()");

                if (!IsPostBack)
                {
                    EstableceRolesAuditables();

                    log.Info("INICIA ListaTipoPMAPreaut()");
                    this.StoreTipoParametroMA.DataSource = DAOProducto.ListaTipoPMAPreaut(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_PreautParamTarj);
                    log.Info("TERMINA ListaTipoPMAPreaut()");

                    this.StoreTipoParametroMA.DataBind();
                }

                log.Info("TERMINA ParametrosTarjetas Page_Load()");
            }

            catch (CAppException)
            {
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
        /// Llena el grid de resultado de búsqueda de cuentas
        /// </summary>
        protected void LlenarGridResultados()
        {
            LogPCI logPCI = new LogPCI(LH_PreautParamTarj);

            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.ID_Cuenta = String.IsNullOrEmpty(this.txtNumCuenta.Text) ? -1 : int.Parse(this.txtNumCuenta.Text);
                datosPersonales.TarjetaTitular = this.txtNumTarjeta.Text;
                datosPersonales.SoloAdicionales = this.chkBoxSoloAdicionales.Checked;
                datosPersonales.Nombre = this.txtNombre.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaterno.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaterno.Text;

                logPCI.Info("INICIA ObtieneCuentasPorFiltro()");
                DataSet dsResultados = DAOTarjetaCuenta.ObtieneCuentasPorFiltro(datosPersonales, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_PreautParamTarj);
                logPCI.Info("TERMINA ObtieneCuentasPorFiltro()");

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 100)
                {
                    X.Msg.Alert("Consulta de Cuentas", "Demasiadas coincidencias. Por favor, afina tu búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Cuentas", "No existen coincidencias con la búsqueda solicitada").Show();
                    return;
                }

                DataTable dtResultados = Tarjetas.EnmascaraTablaConTarjetas(dsResultados.Tables[0], "Tarjeta", "Enmascara", LH_PreautParamTarj);

                this.StoreCuentas.DataSource = dtResultados;
                this.StoreCuentas.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Consulta de Cuentas", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Consulta de Cuentas", "Ocurrió un error al establecer las Cuentas").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de búsqueda, invocando la búsqueda
        /// de cuentas a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarGridResultados();
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
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de cuentas dentro
        /// del Grid de Resultados Cuentas
        /// </summary>
        /// <param name="esBtnLimpiar">Indica si el método de origen fue el clic al botón Limpiar</param>
        protected void limpiaBusquedaPrevia(bool esBtnLimpiar)
        {
            if (esBtnLimpiar)
            {
                FormPanelBusqueda.Reset();
                StoreCuentas.RemoveAll();
                this.EastPanel.Title = "_";
            }

            LimpiaVentanaParams();

            this.FormPanelParams.Reset();
            this.cBoxTipoParametroMA.Reset();
            this.cBoxParametros.Reset();
            this.StoreParametros.RemoveAll();            
            this.StoreValoresParametros.RemoveAll();
            this.btnAddParametros.Disabled = true;

            this.StoreAllParams.RemoveAll();
            this.hdnIdCuentaLDC.Reset();
            this.hdnIdMA.Reset();
            this.hdnIdColectiva.Reset();
            this.hdnIdCadena.Reset();

            this.EastPanel.Disabled = true;

            this.FormPanelParams.Show();
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
                int IdCuentaCLDC = 0;
                int IdMA = 0;
                int IdColectiva = 0;
                int IdCadena = 0;
                string numTarjetaMask = string.Empty, numTarjeta = string.Empty;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "CLDC": IdCuentaCLDC = int.Parse(column.Value); break;
                        case "CCLC": this.hdnIdCuentaCC.Value = column.Value; break;
                        case "IdTarjeta": IdMA = int.Parse(column.Value); break;
                        case "IdColectivaCuentahabiente": IdColectiva = int.Parse(column.Value); break;
                        case "Tarjeta": numTarjetaMask = column.Value; break;
                        case "NumeroTarjeta": numTarjeta = column.Value; break;
                        case "IdCadenaComercial": IdCadena = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                limpiaBusquedaPrevia(false);

                this.hdnIdCuentaLDC.Value = IdCuentaCLDC;
                this.hdnIdMA.Value = IdMA;
                this.hdnIdColectiva.Value = IdColectiva;
                this.hdnIdCadena.Value = IdCadena;

                LlenaGridPanelTodos(numTarjeta);

                this.EastPanel.Title = "Tarjeta " + numTarjetaMask;
                this.EastPanel.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_PreautParamTarj);
                pCI.ErrorException(ex);
                X.Msg.Alert("Tarjetas", "Ocurrió un error al obtener la información de la Tarjeta").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de tipos de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaTipoPMA(object sender, EventArgs e)
        {
            LlenaParametrosCuenta();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros de la cuenta,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosCuenta()
        {
            LogPCI log = new LogPCI(LH_PreautParamTarj);

            try
            {
                int IdCuenta = Convert.ToInt32(this.hdnIdCuentaLDC.Value);

                log.Info("INICIA ObtienePMAPreAutPorTipoSinAsignarCuenta()");
                this.StoreParametros.DataSource = DAOTarjetaCuenta.ObtienePMAPreAutPorTipoSinAsignarCuenta(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdCuenta, LH_PreautParamTarj);
                log.Info("TERMINA ObtienePMAPreAutPorTipoSinAsignarCuenta()");

                this.StoreParametros.DataBind();

                log.Info("INICIA ObtienePMAPreAutCuenta()");
                this.StoreValoresParametros.DataSource = DAOTarjetaCuenta.ObtienePMAPreAutCuenta(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdCuenta, LH_PreautParamTarj);
                log.Info("TERMINA ObtienePMAPreAutCuenta()");

                this.StoreValoresParametros.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al establecer los Parámetros").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametros_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_PreautParamTarj);

            try
            {
                int IdTarjeta = Convert.ToInt32(this.hdnIdMA.Value);
                Int64 IdPlantilla = Convert.ToInt64(this.hdnIdPlantilla.Value);

                unLog.Info("INICIA AgregaPMAPreAutACuenta()");
                LNCuentas.AgregaPMAPreAutACuenta(Convert.ToInt32(cBoxParametros.SelectedItem.Value), IdTarjeta, IdPlantilla,
                    this.Usuario, LH_PreautParamTarj);
                unLog.Info("TERMINA AgregaPMAPreAutACuenta()");

                X.Msg.Notify("", "Parámetro Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.cBoxParametros.Reset();

                LlenaParametrosCuenta();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro", "Ocurrió un error al asignar el Parámetro").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_PreautParamTarj);
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
                        case "ID_ValorParametroMultiasignacion": unParametro.ID_ValordelParametro = int.Parse(column.Value); break;
                        case "Nombre": unParametro.Nombre = column.Value; break;
                        case "Descripcion": unParametro.Descripcion = column.Value; break;
                        case "Valor": unParametro.Valor = column.Value; break;
                        case "ID_Plantilla": unParametro.ID_Plantilla = int.Parse(column.Value); break;
                        case "TipoDato": unParametro.TipoDato = column.Value; break;
                        case "Instrucciones": unParametro.Instrucciones = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                this.hdnIdParametroMA.Value = unParametro.ID_Parametro;
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;
                this.hdnIdPlantilla.Value = unParametro.ID_Plantilla;

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

                    case "Delete":
                        pCI.Info("INICIA BorraValorParametro()");
                        LNProducto.BorraValorParametro((int)unParametro.ID_ValordelParametro, this.Usuario,
                            LH_PreautParamTarj, "_ParamTarjetas");
                        pCI.Info("TERMINA BorraValorParametro()");

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaParametrosCuenta();
                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
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
            LogPCI unLog = new LogPCI(LH_PreautParamTarj);

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

                unLog.Info("INICIA ModificaValorParametro()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_PreautParamTarj, "_ParamTarjetas");
                unLog.Info("TERMINA ModificaValorParametro()");

                WdwValorParametro.Hide();

                X.Msg.Notify("Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaParametrosCuenta();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al modificar el Parámetro").Show();
            }
        }

        /// <summary>
        /// Establece los datos en el grid de todos los parámetros, según la tarjeta seleccionada
        /// </summary>
        /// <param name="tarjeta">Número de tarjeta</param>
        protected void LlenaGridPanelTodos(string tarjeta)
        {
            LogPCI log = new LogPCI(LH_PreautParamTarj);

            try
            {
                log.Info("INICIA ObtienePMAPreAutTarjeta()");
                this.StoreAllParams.DataSource = DAOTarjetaCuenta.ObtienePMAPreAutTarjeta(tarjeta,
                    Configuracion.Get( Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveTipoPMA_Preaut", LH_PreautParamTarj).Valor, LH_PreautParamTarj);
                log.Info("TERMINA ObtienePMAPreAutTarjeta()");

                this.StoreAllParams.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Consultar Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Consultar Parámetros", "Ocurrió un error al establecer los Parámetros").Show();
            }
        }
    }
}