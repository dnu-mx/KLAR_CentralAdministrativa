using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
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
    public partial class CuentasVIP : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Cuentas VIP
        private LogHeader LH_ParabCuentasVIP = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Parámetros Open Pay
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabCuentasVIP.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabCuentasVIP.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabCuentasVIP.User = this.Usuario.ClaveUsuario;
            LH_ParabCuentasVIP.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabCuentasVIP);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CuentasVIP Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("EsAutorizador", false);
                    HttpContext.Current.Session.Add("EsEjecutor", false);

                    EstablecePermisosPorRol();

                    EstableceControlesPorRol();

                    EstableceSubEmisores();

                    HttpContext.Current.Session.Add("ElParametro", null);
                }

                log.Info("TERMINA CuentasVIP Page_Load()");
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
        /// Oculta el estatus habilitado/deshabilitado u oculto/visible de los
        /// controles por rol Ejecutor/Autorizador
        /// </summary>
        protected void EstableceControlesPorRol()
        {
            try
            {
                bool esAutorizador = bool.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                bool esEjecutor = bool.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                this.cBoxClienteSolic.Selectable = false;
                this.cBoxProductoSolic.Selectable = false;
                this.txtTarjeta.Disabled = true;
                this.btnValidar.Disabled = true;

                this.txtMontoSolicitado.Selectable = false;
                this.txtMontoAcumulado.Selectable = false;
                this.txtRazon.Selectable = false;
                this.btnSolic.Disabled = true;

                this.GridPanelAutorizaciones.Disabled = true;

                if (esAutorizador)
                {
                    this.GridPanelAutorizaciones.Disabled = false;
                }

                if (esEjecutor)
                {
                    this.cBoxClienteSolic.Selectable = true;
                    this.cBoxProductoSolic.Selectable = true;
                    this.txtTarjeta.Disabled = false;
                    this.btnValidar.Disabled = false;
                }
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabCuentasVIP);
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Establece los valores del combo SubEmisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI logPCI = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabCuentasVIP);
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
        protected void EstableceProductosSolic(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                this.StoreProductosSolic.RemoveAll();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductosSolic.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxClienteSolic.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabCuentasVIP);
                log.Info("TERMINA ObtieneProductosDeColectiva()");
                this.StoreProductosSolic.DataBind();
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
        /// Controla el evento Click al botón Validar, solicitando la verificación de la cuenta en
        /// base de datos y habilitando los controles para la solicitud
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValidar_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                long IdProducto = Convert.ToInt64(this.cBoxProductoSolic.SelectedItem.Value);

                unLog.Info("INICIA ValidaNivelCumplimientoCuenta()");
                DataTable dtResp = LNCuentas.ValidaNivelCumplimientoCuenta(
                    IdProducto, this.txtTarjeta.Text, LH_ParabCuentasVIP);
                unLog.Info("TERMINA ValidaNivelCumplimientoCuenta()");
                
                this.hdnIdPlantilla.Value = dtResp.Rows[0]["ID_Plantilla"];
                this.hdnTarjetaHabiente.Value = dtResp.Rows[0]["Tarjetahabiente"];

                this.txtMontoSolicitado.Selectable = true;
                this.txtMontoAcumulado.Selectable = true;
                this.txtRazon.Selectable = true;
                this.btnSolic.Disabled = false;
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Validación", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Validación", "Ocurrió un error al solicitar la validación del nivel de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar restableciendo todos los controles
        /// a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarSolic_Click(object sender, EventArgs e)
        {
            this.cBoxClienteSolic.Reset();

            this.cBoxProductoSolic.Reset();
            this.StoreProductosSolic.RemoveAll();
            this.txtTarjeta.Reset();

            this.txtMontoSolicitado.Reset();
            this.txtMontoAcumulado.Reset();
            this.txtRazon.Reset();

            EstableceControlesPorRol();
        }

        /// <summary>
        /// Controla el evento Click al botón Solicitar Cambio, solicitando la verificación de la cuenta en
        /// base de datos y habilitando los controles para la solicitud
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnSolic_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                string blResponse = VerificaBlackList();

                //registra en EJECUTOR AUTORIZADOR
                unLog.Info("INICIA CreaModificaSolicitudCambioNivelCuenta()");
                LNCuentas.CreaModificaSolicitudCambioNivelCuenta(Convert.ToInt64(this.hdnIdPlantilla.Value),
                    this.txtMontoSolicitado.Text.Replace("$", "").Replace(",", ""),
                    this.txtMontoAcumulado.Text.Replace("$", "").Replace(",", ""), this.txtRazon.Text,
                    blResponse, Path.GetFileName(Request.Url.AbsolutePath), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabCuentasVIP, this.txtTarjeta.Text);
                unLog.Info("TERMINA CreaModificaSolicitudCambioNivelCuenta()");

                X.Msg.Notify("Solicitud de Cambio", "Solicitud registrada <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnLimpiarSolic.FireEvent("click");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Solicitud de Cambio", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Solicitud de Cambio", "Ocurrió un error al realizar la solicitud de cambio de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Solicita la validación en lista negra del tarjetahabiente al servicio web y procesa la respuesta
        /// </summary>
        /// <returns>Respuesta del servicio web</returns>
        protected string VerificaBlackList()
        {
            LogPCI log = new LogPCI(LH_ParabCuentasVIP);
            //string wsLoginResp, wsBlResp = null;
            string wsBlResp = "90";
            log.Info("VerificaBlackList()");

            try
            {
                //Parametros.Headers _headers = new Parametros.Headers();
                //Parametros.LoginBodyENG _body = new Parametros.LoginBodyENG();

                //_headers.URL = Configuracion.Get(Guid.Parse(
                //    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                //    "WsParabBL_URL").Valor;

                //_body.user = Configuracion.Get(Guid.Parse(
                //    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                //    "WsParabBL_Usr").Valor;
                //_body.password = Configuracion.Get(Guid.Parse(
                //    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                //    "WsParabBL_Pwd").Valor;

                //log.Info("INICIA Parabilium.LoginENG()");
                //wsLoginResp = Parabilium.LoginENG(_headers, _body, LH_ParabCuentasVIP);
                //log.Info("TERMINA Parabilium.LoginENG()");

                //if (wsLoginResp.ToUpper().Contains("ERROR"))
                //{
                //    throw new CAppException(8006, wsLoginResp);
                //}

                ////BlackListBody
                //_headers.Token = wsLoginResp;
                //_headers.Credenciales = Cifrado.Base64Encode(_body.user + ":" + _body.password);

                //Parametros.BlackListBody blList = new Parametros.BlackListBody();
                //blList.full_name = this.hdnTarjetaHabiente.Value.ToString();

                //log.Info("INICIA Parabilium.BlackList()");
                //wsBlResp = Parabilium.BlackList(_headers, blList, LH_ParabCuentasVIP);
                //log.Info("TERMINA Parabilium.BlackList()");

                return wsBlResp;
            }

            catch (CAppException err)
            {
                log.Error("Verifica Black List: " + err.Mensaje());
                throw err;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo SubEmisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceProductosAut(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                this.StoreProductosAut.RemoveAll();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductosAut.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxClienteAut.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabCuentasVIP);
                log.Info("TERMINA ObtieneProductosDeColectiva()");
                this.StoreProductosAut.DataBind();
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
        /// Controla el evento Click al botón Buscar del panel de Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarAut_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                pCI.Info("INICIA ConsultaSolicitudesCambioCuentaVIP()");
                this.StoreParamsPorAutorizar.DataSource = LNCuentas.ConsultaSolicitudesCambioCuentaVIP(
                    Convert.ToInt64(this.cBoxProductoAut.SelectedItem.Value),
                    this.txtTarjetaAut.Text, LH_ParabCuentasVIP);
                pCI.Info("TERMINA ConsultaSolicitudesCambioCuentaVIP()");

                this.StoreParamsPorAutorizar.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Solicitudes", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Solicitudes", "Ocurrió un error al obtener las Solicitudes de Cambio de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel de Solicitudes de Cambio, restableciendo los controles
        /// de dicho panel a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarAut_Click(object sender, EventArgs e)
        {
            this.cBoxClienteAut.Reset();

            this.cBoxProductoAut.Reset();
            this.StoreProductosAut.RemoveAll();
            this.txtTarjetaAut.Reset();

            this.StoreParamsPorAutorizar.RemoveAll();

            EstableceControlesPorRol();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de solicitudes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoSolicitudes(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabCuentasVIP);
            pCI.Info("EjecutarComandoSolicitudes()");

            try
            {
                bool esAutorizador = bool.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                bool esEjecutor = bool.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                PlantillaCuentaVIP plantilla = new PlantillaCuentaVIP();
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
                        case "ID_Plantilla": plantilla.IdPlantilla = long.Parse(column.Value); break;
                        case "ID_EA_NivelCtaCumpl": plantilla.IdEA_NivelCtaCumpl = int.Parse(column.Value); break;
                        case "ID_VPMA_NivelCtaCumpl": plantilla.IdVPMA_NivelCtaCumpl = long.Parse(column.Value); break;
                        case "ID_EA_NivelCtaCumplPers": plantilla.IdEA_NivelCtaCumplPers = int.Parse(column.Value); break;
                        case "ID_PMA_NivelCtaCumplPers": plantilla.IdPMA_NivelCtaCumplPers = long.Parse(column.Value); break;
                        case "ID_EA_SaldoMaxPers": plantilla.IdEA_SaldoMaxPers = int.Parse(column.Value); break;
                        case "ID_PMA_SaldoMaxPers": plantilla.IdPMA_SaldoMaxPers = long.Parse(column.Value); break;
                        case "ID_EA_MaxAbonoPers": plantilla.IdEA_MaxAbonoPers = int.Parse(column.Value); break;
                        case "ID_PMA_MaxAbonoPers": plantilla.IdPMA_MaxAbonoPers = long.Parse(column.Value); break;
                        case "Motivo": plantilla.Motivo = column.Value; break;
                        case "RespListaNegra": plantilla.RespListaNegra = column.Value; break;
                        default:
                            break;
                    }
                }

                string comando = e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Autorizar":
                        AceptaSolicitud(plantilla);
                        break;

                    case "Rechazar":
                        RechazaSolicitud(plantilla);
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Solicitudes", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Solicita la autorización y modificación de los parámetros relacionados con el nivel de cumplimiento
        /// de la cuenta a base de datos
        /// </summary>
        /// <param name="laPlantilla">Entidad con los datos del parámetro por autorizar/modificar</param>
        protected void AceptaSolicitud(PlantillaCuentaVIP laPlantilla)
        {
            LogPCI logPCI = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                logPCI.Info("INICIA ModificaYAutorizaCambioCuentaVIP()");
                LNCuentas.ModificaYAutorizaCambioCuentaVIP(laPlantilla, this.Usuario, LH_ParabCuentasVIP);
                logPCI.Info("TERMINA ModificaYAutorizaCambioCuentaVIP()");

                X.Msg.Notify("Autorizar Solicitud", "Solicitud autorizada <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnBuscarAut.FireEvent("click");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Autorizar Solicitud", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Autorizar Solicitud", "Ocurrió un error al autorizar la Solicitud de Cambio de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Solicita el rechazo a la modificación de los parámetros relacionados con el nivel de cumplimiento
        /// de la cuenta a base de datos
        /// </summary>
        protected void RechazaSolicitud(PlantillaCuentaVIP plantillaCuentaVIP)
        {
            LogPCI logPCI = new LogPCI(LH_ParabCuentasVIP);

            try
            {
                logPCI.Info("INICIA RechazaCambioCuentaVIP()");
                LNCuentas.RechazaCambioCuentaVIP(plantillaCuentaVIP, this.Usuario, LH_ParabCuentasVIP);
                logPCI.Info("TERMINA RechazaCambioCuentaVIP()");

                X.Msg.Notify("Rechazar Solicitud", "Solicitud rechazada <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnBuscarAut.FireEvent("click");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Rechazar Solicitud", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Rechazar Solicitud", "Ocurrió un error al rechazar la Solicitud de Cambio de la Cuenta").Show();
            }
        }
    }
}
