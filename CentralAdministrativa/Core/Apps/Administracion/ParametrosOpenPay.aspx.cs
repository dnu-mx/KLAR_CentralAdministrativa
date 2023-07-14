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
using System.IO;
using System.Web;

namespace Administracion
{
    public partial class ParametrosOpenPay : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Administra Personas Morales
        private LogHeader LH_ParabParamsOP = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Parámetros Open Pay
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabParamsOP.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabParamsOP.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabParamsOP.User = this.Usuario.ClaveUsuario;
            LH_ParabParamsOP.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabParamsOP);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ParametrosOpenPay Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("EsAutorizador", false);
                    HttpContext.Current.Session.Add("EsEjecutor", false);

                    EstablecePermisosPorRol();

                    EstableceSubEmisores();

                    HttpContext.Current.Session.Add("ElParametro", null);
                }

                log.Info("TERMINA ParametrosOpenPay Page_Load()");
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
        /// Establece en variables de sesión los roles auditables para el control de permisos
        /// en la página
        /// </summary>
        protected void EstablecePermisosPorRol()
        {
            this.Usuario.Roles.Sort();

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
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI logPCI = new LogPCI(LH_ParabParamsOP);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabParamsOP);
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
            catch (CAppException caEx)
            {
                logPCI.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw ex;
            }

        }

        /// <summary>
        /// Controla el evento Seleccionar del combo SubEmisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceProductos(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabParamsOP);

            try
            {
                LimpiaBusquedaPrevia();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductos.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxCliente.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabParamsOP);
                log.Info("TERMINA ObtieneProductosDeColectiva()");
                this.StoreProductos.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Productos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Productos", "Error al obtener los Productos del SubEmisor").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar, solicitando el llenado del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaParametrosProducto();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar restableciendo todos los controles
        /// a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.cBoxCliente.Reset();

            LimpiaBusquedaPrevia();

            LimpiaVentanaParams();
        }

        /// <summary>
        /// Restablece los controles de la ventana de edición de parámetros
        /// </summary>
        protected void LimpiaVentanaParams()
        {
            this.FormPanelValorParamTxt.Reset();
            this.txtParametro.Reset();

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
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de parámetros
        /// </summary>
        protected void LimpiaBusquedaPrevia()
        {
            this.cBoxProducto.Reset();
            this.StoreProductos.RemoveAll();

            HttpContext.Current.Session.Add("ElParametro", null);
            this.StoreValoresParametros.RemoveAll();
        }

        /// <summary>
        /// Establece los valores de parámetros del producto en el grid, llamando a los objetos de datos
        /// que obtienen los valores
        /// </summary>
        protected void LlenaParametrosProducto()
        {
            LogPCI unLog = new LogPCI(LH_ParabParamsOP);

            try
            {
                int IdProducto = Convert.ToInt32(this.cBoxProducto.SelectedItem.Value);
                bool esAutorizador = bool.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                bool esEjecutor = bool.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                unLog.Info("INICIA ListaParametrosMAOpenPay()");
                this.StoreValoresParametros.DataSource = 
                    DAOParametroMA.ListaParametrosMAOpenPay(IdProducto, esAutorizador, esEjecutor,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabParamsOP);
                unLog.Info("TERMINA ListaParametrosMAOpenPay()");

                this.StoreValoresParametros.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Error al establecer los Parámetros").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabParamsOP);
            pCI.Info("EjecutarComandoParametros()");

            try
            {
                bool esAutorizador = bool.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                bool esEjecutor = bool.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

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
                        case "ValorPorAutorizar": unParametro.ValorPorAutorizar = column.Value; break;
                        case "ValorAutorizado": unParametro.Valor = column.Value; break;
                        case "TipoDato": unParametro.TipoDato = column.Value; break;
                        case "TipoValidacion": unParametro.TipoValidacion = column.Value; break;
                        case "ValorInicial": unParametro.ValorInicial = column.Value; break;
                        case "ValorFinal": unParametro.ValorFinal = column.Value; break;
                        case "ExpresionRegular": unParametro.ExpresionRegular = column.Value; break;
                        default:
                            break;
                    }
                }

                string comando = e.ExtraParams["Comando"];
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;
                HttpContext.Current.Session.Add("ElParametro", unParametro);

                this.hdnValorIniPMA.Value = unParametro.ValorInicial;
                this.hdnValorFinPMA.Value = unParametro.ValorFinal;

                switch (comando)
                {
                    case "Edit":
                        LimpiaVentanaParams();

                        if (!string.IsNullOrEmpty(unParametro.TipoValidacion) && unParametro.TipoValidacion.Contains("CAT")) //Clave fija de tipo de validación CATALOGO
                        {
                            pCI.Info("INICIA ListaElementosCatalogoPMA()");
                            this.StoreCatalogoPMA.DataSource =
                                DAOProducto.ListaElementosCatalogoPMA(Convert.ToInt64(this.cBoxCliente.SelectedItem.Value),
                                Convert.ToInt64(unParametro.ID_Parametro), string.Empty, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabParamsOP);
                            pCI.Info("TERMINA ListaElementosCatalogoPMA()");

                            this.StoreCatalogoPMA.DataBind();
                            this.cBoxCatalogoPMA.Value = unParametro.Valor;
                            this.cBoxCatalogoPMA.Hidden = false;
                        }
                        else
                        {
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
                        }

                        this.txtParametro.Text = unParametro.Descripcion;
                        this.WdwValorParametro.Title += " - " + unParametro.Nombre;
                        this.WdwValorParametro.Show();
                        break;

                    case "Accept":
                        AceptaValorParametro(unParametro, "Autorizar Valor", esAutorizador, esEjecutor);
                        break;

                    case "Reject":
                        RechazarValorPorAutorizar();
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
        /// Controla el evento Click al botón Guardar de las ventana de valor del parámetro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorParametro_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabParamsOP);

            try
            {
                bool esAutorizador = bool.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                bool esEjecutor = bool.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                string nuevoValor = string.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                    string.IsNullOrEmpty(this.txtValorParInt.Text) ?
                    string.IsNullOrEmpty(this.txtValorParString.Text) ?
                    string.IsNullOrEmpty(this.cBoxValorPar.SelectedItem.Value) ?
                    this.cBoxCatalogoPMA.SelectedItem.Value : this.cBoxValorPar.SelectedItem.Value :
                    this.txtValorParString.Text : this.txtValorParInt.Text :
                    this.txtValorParFloat.Text;

                //Si el usuario autorizador y ejecutor, el cambio se realiza directo
                if (esAutorizador && esEjecutor)
                {
                    ParametroValor parametro = HttpContext.Current.Session["ElParametro"] as ParametroValor;
                    parametro.ValorPorAutorizar = nuevoValor;
                    AceptaValorParametro(parametro, "Parámetro", esAutorizador, esEjecutor);
                }
                else
                {
                    pCI.Info("INICIA CreaModificaValorPMAPorAutorizar()");
                    LNParametros.CreaModificaValorPMAPorAutorizar(Convert.ToInt64(this.hdnIdValorPMA.Value),
                        nuevoValor, Path.GetFileName(Request.Url.AbsolutePath), this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabParamsOP);
                    pCI.Info("TERMINA CreaModificaValorPMAPorAutorizar()");

                    X.Msg.Notify("Parámetro", "Valor del Parámetro por Autorizar registrado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }

                this.WdwValorParametro.Hide();

                LlenaParametrosProducto();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetro", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetro", "Error al registrar el Valor por Autorizar del Parámetro").Show();
            }
        }

        /// <summary>
        /// Solicita la autorización y modificación del valor del parámetro a base de datos
        /// </summary>
        /// <param name="elParametro">Entidad con los datos del parámetro por autorizar/modificar</param>
        /// <param name="Titulo">Título de los mensajes en pantalla</param>
        /// <param name="EsAutorizador">Bandera para el rol Autorizador del usuario en sesión</param>
        /// <param name="EsEjecutor">Bandera para el rol Ejecutor del usuario en sesión</param>
        protected void AceptaValorParametro(ParametroValor elParametro, string Titulo, bool EsAutorizador, bool EsEjecutor)
        {
            LogPCI logPCI = new LogPCI(LH_ParabParamsOP);

            try
            {
                bool EsAutorYEjecut = EsAutorizador && EsEjecutor ? true : false;

                logPCI.Info("INICIA ModificaYAutorizaValorParametro()");
                LNParametros.ModificaYAutorizaValorParametro(elParametro, EsAutorYEjecut, this.Usuario, LH_ParabParamsOP);
                logPCI.Info("TERMINA ModificaYAutorizaValorParametro()");

                X.Msg.Notify(Titulo, "Valor del Parámetro modificado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaParametrosProducto();
            }
            catch (CAppException err)
            {
                X.Msg.Alert(Titulo, err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert(Titulo, "Ocurrió un error al establecer el valor del Parámetro").Show();
            }
        }

        /// <summary>
        /// Solicita el rechazo a la modificación del valor del parámetro a base de datos
        /// </summary>
        protected void RechazarValorPorAutorizar()
        {
            LogPCI logPCI = new LogPCI(LH_ParabParamsOP);

            try
            {
                long IDValorParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);

                logPCI.Info("INICIA RechazaValorParametroPorAutorizar()");
                LNParametros.RechazaValorParametroPorAutorizar(IDValorParametro, LH_ParabParamsOP);
                logPCI.Info("TERMINA RechazaValorParametroPorAutorizar()");

                X.Msg.Notify("Rechazar Valor por Autorizar", "Cambio de valor del Parámetro rechazado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaParametrosProducto();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Rechazar Valor por Autorizar", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Rechazar Valor por Autorizar", "Ocurrió un error al rechazar el cambio de valor del Parámetro").Show();
            }
        }
    }
}
