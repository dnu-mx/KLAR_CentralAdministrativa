using DALAdministracion.BaseDatos;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.LogicaNegocio;
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
using Utilerias;

namespace TpvWeb
{
    public partial class CuentaCLABE_MakerChecer : PaginaBaseCAPP
    {
        #region Variables

        //LOG HEADER Cuenta CLABE
        private LogHeader LH_CuentaClabe = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Cuenta CLABE
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        /// 
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_CuentaClabe.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_CuentaClabe.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_CuentaClabe.User = this.Usuario.ClaveUsuario;
            LH_CuentaClabe.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_CuentaClabe);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CuentaCLABE_MakerChecer Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("EsAutorizador", false);
                    HttpContext.Current.Session.Add("EsEjecutor", false);

                    EstablecePermisosPorRol();

                    EstableceControlesPorRol();

                    EstableceSubEmisores();

                    HttpContext.Current.Session.Add("ElParametro", null);
                }

                log.Info("TERMINA CuentaCLABE_MakerChecer Page_Load()");
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

                this.PanelEjecutor.Disabled = true;
                this.GridPanelAutorizaciones.Disabled = true;
                this.btnRegistrar.Hide();
                this.btnRegAut.Hide();

                if (esAutorizador)
                {
                    this.GridPanelAutorizaciones.Disabled = false;
                    this.btnRegAut.Show();
                }

                if (esEjecutor)
                {
                    this.PanelEjecutor.Disabled = false;
                    if (!esAutorizador)
                        this.btnRegistrar.Show();
                }
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_CuentaClabe);
                unLog.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Establece los valores del combo subemisor con la información de base de datos
        /// </summary>
        protected void EstableceSubEmisores()
        {
            LogPCI logPCI = new LogPCI(LH_CuentaClabe);

            try
            {
                logPCI.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_CuentaClabe);
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

                this.StoreSubemisores.DataSource = ComboList;
                this.StoreSubemisores.DataBind();
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
            LogPCI log = new LogPCI(LH_CuentaClabe);

            try
            {
                this.StoreProductosSolic.RemoveAll();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductosSolic.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxSubEmisor.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_CuentaClabe);
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
        /// Controla el evento Click al botón Registrar, solicitando la verificación de la cuenta CLABE
        /// en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_CuentaClabe);

            try
            {  
                //registra en EJECUTOR AUTORIZADOR
                unLog.Info("INICIA CreaModificaSolicitudCambioCuentaCLABE()");
                LNClabePendiente.CreaModificaSolicitudCambioCuentaCLABE(
                    Convert.ToInt32(this.cBoxProductoEjec.SelectedItem.Value), this.txtTarjeta.Text, 
                    this.txtCLABE.Text, Path.GetFileName(Request.Url.AbsolutePath), false, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_CuentaClabe);
                unLog.Info("TERMINA CreaModificaSolicitudCambioCuentaCLABE()");

                X.Msg.Notify("Registro de Solicitud", "Solicitud registrada <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnLimpiar.FireEvent("click");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Registro de Solicitud", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Registro de Solicitud", "Ocurrió un error al realizar el registro de la solicitud de cuenta CLABE").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Registrar, solicitando la verificación de la cuenta CLABE
        /// en base de datos
        /// </summary>
        [DirectMethod(Namespace = "CtaCLABE_PM")]
        public void RegistraYAutorizaSolicitud()
        {
            LogPCI unLog = new LogPCI(LH_CuentaClabe);

            try
            {
                //registra en EJECUTOR AUTORIZADOR
                unLog.Info("INICIA CreaModificaSolicitudCambioCuentaCLABE()");
                LNClabePendiente.CreaModificaSolicitudCambioCuentaCLABE(
                    Convert.ToInt32(this.cBoxProductoEjec.SelectedItem.Value), this.txtTarjeta.Text,
                    this.txtCLABE.Text, Path.GetFileName(Request.Url.AbsolutePath), true, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_CuentaClabe);
                unLog.Info("TERMINA CreaModificaSolicitudCambioCuentaCLABE()");

                X.Msg.Notify("Registro y Autorización de Solicitud", "Cuenta CLABE creada <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnLimpiar.FireEvent("click");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Registro y Autorización de Solicitud", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Registro y Autorización de Solicitud", "Ocurrió un error al realizar el registro y autorización de la cuenta CLABE").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel de Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarAut_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_CuentaClabe);

            try
            {
                pCI.Info("INICIA ObtieneSolicitudesAutorizarCuentaCLABE()");
                DataTable dtSolicitudes = DAOClabePendiente.ObtieneSolicitudesAutorizarCuentaCLABE(
                        Convert.ToInt64(this.cBoxProductoAut.SelectedItem.Value), this.txtTarjetaAut.Text, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_CuentaClabe);
                pCI.Info("TERMINA ObtieneSolicitudesAutorizarCuentaCLABE()");

                if (dtSolicitudes.Rows.Count == 0)
                {
                    X.Msg.Alert("Solicitudes", "No existen coincidencias con la búsqueda solicitada").Show();
                    return;
                }
                else
                {
                    DataTable dt = Tarjetas.EnmascaraTablaConTarjetas(dtSolicitudes, "NumeroTarjeta", "Enmascara", LH_CuentaClabe);

                    this.StoreClabes.DataSource = dtSolicitudes;
                    this.StoreClabes.DataBind();
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Solicitudes", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Solicitudes", "Ocurrió un error al obtener las Solicitudes de Cuenta CLABE").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar restableciendo todos los controles
        /// a su estado de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.cBoxSubEmisor.Reset();

            this.cBoxProductoEjec.Reset();
            this.StoreProductosSolic.RemoveAll();
            this.txtTarjeta.Reset();
            this.txtCLABE.Reset();

            EstableceControlesPorRol();
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo SubEmisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceProductosAut(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_CuentaClabe);

            try
            {
                this.StoreProductosAut.RemoveAll();

                log.Info("INICIA ObtieneProductosDeColectiva()");
                this.StoreProductosAut.DataSource = DAOProducto.ObtieneProductosDeColectiva(
                    Convert.ToInt32(this.cBoxClienteAut.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_CuentaClabe);
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
        /// Controla el evento Click al botón Limpiar del panel de Solicitudes, restableciendo los controles
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

            this.StoreClabes.RemoveAll();

            EstableceControlesPorRol();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de solicitudes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoSolicitudes(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_CuentaClabe);
            pCI.Info("EjecutarComandoSolicitudes()");

            try
            {
                int Id_Tarjeta = 0, Id_EjecAut = 0;
                string ctaClabe = string.Empty;

                bool esAutorizador = bool.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                bool esEjecutor = bool.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

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
                        case "IdTarjeta": Id_Tarjeta = int.Parse(column.Value); break;
                        case "ID_EjecutorAutorizador": Id_EjecAut = int.Parse(column.Value); break;
                        case "NuevoValor": ctaClabe = column.Value; break;

                        default:
                            break;
                    }
                }

                string comando = e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Autorizar":
                        AceptaSolicitud(Id_Tarjeta, Id_EjecAut, ctaClabe);
                        break;

                    case "Rechazar":
                        RechazaSolicitud(Id_EjecAut);
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
        /// Solicita la autorización y modificación de la solicitud de cuenta CLABE a base de datos
        /// </summary>
        /// <param name="id_Tarjeta">Identificador de la tarjeta</param>
        /// <param name="id_EjecAut">Identificador del registro en la tabla de control</param>
        /// <param name="cuentaClabe">Valor de la nueva cuenta CLABE</param>
        protected void AceptaSolicitud(int id_Tarjeta, int id_EjecAut, string cuentaClabe)
        {
            LogPCI logPCI = new LogPCI(LH_CuentaClabe);

            try
            {
                logPCI.Info("INICIA AceptaSolicitud()");
                LNClabePendiente.ModificaYAutorizaSolicitudCuentaCLABE(id_Tarjeta, id_EjecAut, cuentaClabe,
                    this.Usuario, LH_CuentaClabe);
                logPCI.Info("TERMINA AceptaSolicitud()");

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
                X.Msg.Alert("Autorizar Solicitud", "Ocurrió un error al autorizar la Solicitud de la Cuenta CLABE").Show();
            }
        }

        /// <summary>
        /// Solicita el rechazo de la solicitud de cuenta CLABE a base de datos
        /// </summary>
        /// <param name="id_EjecAut">Identificador del registro en la tabla de control</param>
        protected void RechazaSolicitud(int id_EjecAut)
        {
            LogPCI logPCI = new LogPCI(LH_CuentaClabe);

            try
            {

                logPCI.Info("INICIA RechazaSolicitudCuentaCLABE()");
                LNClabePendiente.RechazaSolicitudCuentaCLABE(id_EjecAut, this.Usuario, LH_CuentaClabe);
                logPCI.Info("TERMINA RechazaSolicitudCuentaCLABE()");

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
                X.Msg.Alert("Rechazar Solicitud", "Ocurrió un error al rechazar la Solicitud de la Cuenta CLABE").Show();
            }
        }
    }
}
