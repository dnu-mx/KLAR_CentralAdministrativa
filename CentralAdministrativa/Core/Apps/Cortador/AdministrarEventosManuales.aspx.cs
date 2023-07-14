using DALCentralAplicaciones.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using DALEventos.BaseDatos;
using DALEventos.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;

namespace Cortador
{
    public partial class AdministrarEventosManuales : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER ADMINISTRAR EVENTOS MANUALES
        private LogHeader LH_AdminEvMan = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_AdminEvMan.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_AdminEvMan.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_AdminEvMan.User = this.Usuario.ClaveUsuario;
            LH_AdminEvMan.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_AdminEvMan);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarEventosManuales Page_Load()");

                HttpContext.Current.Session.Clear();

                if (!IsPostBack)
                {
                    LlenaGridEventos();
                }

                log.Info("TERMINA AdministrarEventosManuales Page_Load()");
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
        /// Solicita el catálogo de clientes a base de datos y lo establece en 
        /// el combo correspondiente
        /// </summary>
        protected void LlenaGridEventos()
        {
            LogPCI unLog = new LogPCI(LH_AdminEvMan);

            try
            {
                unLog.Info("INICIA ListaEventosManualesParaAdministrar()");
                DataTable dtEventos = DAOEvento.ListaEventosManualesParaAdministrar(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), LH_AdminEvMan);
                unLog.Info("TERMINA ListaEventosManualesParaAdministrar()");

                if (dtEventos.Rows.Count < 1)
                {
                    X.Msg.Alert("Movimientos Existentes", "No existen Eventos Manuales o no tienes permiso para ellos.").Show();
                }
                else
                {
                    StoreEventos.DataSource = dtEventos;
                    StoreEventos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos Existentes", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Movimientos Existentes", "Error al obtener los eventos manuales.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Evento Manual
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnCrear_Click(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_AdminEvMan);

            try
            {
                logPCI.Info("INICIA CreaEventoManual()");
                DataTable dt = LNEvento.CreaEventoManual(txtClave.Text, txtDescInterna.Text, txtDescEdoCta.Text,
                    int.Parse(cBoxTipoMov.SelectedItem.Value), this.Usuario, LH_AdminEvMan);
                logPCI.Info("TERMINA CreaEventoManual()");

                int IdNuevoEvento = Convert.ToInt32(dt.Rows[0]["IdNuevoEvento"]);

                if (IdNuevoEvento == -1)
                {
                    string msj = dt.Rows[0]["Respuesta"].ToString();
                    X.Msg.Alert("Nuevo Evento Manual", msj).Show();
                }
                else
                {
                    string sp = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "SPFiltroAltaEventoManual", LH_AdminEvMan).Valor;

                    //Se crea el permiso para el usuario al recién creado evento manual
                    logPCI.Info("INICIA BuscaYCreaPermisoEventoManual()");
                    LNUsuarios.BuscaYCreaPermisoEventoManual(sp, this.Usuario.UsuarioId,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        IdNuevoEvento.ToString(), LH_AdminEvMan);
                    logPCI.Info("TERMINA BuscaYCreaPermisoEventoManual()");

                    //Se otrogan el permiso inmediato al usuario en sesión
                    logPCI.Info("INICIA AgregarFiltrosEnSesion()");
                    LNFiltro.AgregarFiltrosEnSesion(this.Usuario, LH_AdminEvMan,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    logPCI.Info("TERMINA AgregarFiltrosEnSesion()");

                    X.Msg.Notify("Nuevo Evento Manual", "El Evento Manual se Creó <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    LlenaGridEventos();
                }

                RestableceFormulario();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Evento Manual", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Nuevo Evento Manual", "Ocurrió un error al crear el Evento Manual").Show();
            }
        }

        /// <summary>
        /// Restablece el formulario a su estado de carga de la página
        /// </summary>
        protected void RestableceFormulario()
        {
            FormPanelCrear.Reset();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_AdminEvMan);

            try
            {
                int IdEvento = 0;
                string json = String.Format("[{0}]", e.ExtraParams["Values"]);

                IDictionary<string, string>[] eventoSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (eventoSeleccionado == null || eventoSeleccionado.Length < 1)
                {
                    return;
                }

                foreach (KeyValuePair<string, string> column in eventoSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Evento": IdEvento = int.Parse(column.Value); break;
                        case "ClaveEvento": txtEditClave.Text = column.Value; break;
                        case "Descripcion": txtEditDescr.Text = column.Value; break;
                        case "DescripcionEdoCta": txtEditDescrEdoCta.Text = column.Value; break;
                        case "TipoMovimiento": txtTipoMovimiento.Text = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = (String)e.ExtraParams["Comando"];
                
                switch (comando)
                {
                    case "Edit":
                        hdnIdEvento.Value = IdEvento;
                        WdwEditar.Show();
                        break;

                    case "Lock":
                    case "Unlock":
                        int estatus = comando == "Lock" ? 0 : 1;
                        string msj = comando == "Lock" ? "Desactivado" : "Activado";

                        log.Info("INICIA ModificaEstatusEventoManual()");
                        LNEvento.ModificaEstatusEventoManual(IdEvento, estatus, this.Usuario, LH_AdminEvMan);
                        log.Info("TERMINA ModificaEstatusEventoManual()");

                        X.Msg.Notify("", "Evento Manual " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridEventos();

                        break;

                    default:
                        break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Evento Manual", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Evento Manual", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar dentro de la ventana de edición
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnGuardar_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_AdminEvMan);

            try
            {
                unLog.Info("INICIA ModificaDescripcionesEventoManual()");
                LNEvento.ModificaDescripcionesEventoManual(Convert.ToInt32(hdnIdEvento.Value),
                    txtEditDescr.Text, txtEditDescrEdoCta.Text, this.Usuario, LH_AdminEvMan);
                unLog.Info("TERMINA ModificaDescripcionesEventoManual()");

                X.Msg.Notify("Edición de Evento Manual", "Cambios guardados <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridEventos();

                WdwEditar.Hide();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Edición de Evento Manual", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Edición de Evento Manual", ex.Message).Show();
            }
        }
    }
}