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
using System.Web.Services.Description;

namespace TpvWeb
{
    public partial class AdministrarReportes : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA Administrar Reportes 
        private LogHeader LH_ParabAdminReportes = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Colectivas para Parabilia
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminReportes.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminReportes.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminReportes.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminReportes.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminReportes);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarReportes Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaTiposGeneracionReportes()");
                    this.StoreTiposGenRep.DataSource = DAOReportes.ListaTiposGeneracionReportes(LH_ParabAdminReportes);
                    log.Info("TERMINA ListaTiposGeneracionReportes()");
                    this.StoreTiposGenRep.DataBind();

                    LlenaGridReportes();
                }

                log.Info("TERMINA AdministrarReportes Page_Load()");
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
        /// Solicita el listado de reportes 
        /// para llenar el grid correspondiente
        /// </summary>
        protected void LlenaGridReportes()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminReportes);

            try
            {
                unLog.Info("INICIA ObtieneListadoReportes()");
                StoreReportes.DataSource = LNReportes.ObtieneListadoReportes(LH_ParabAdminReportes);
                unLog.Info("TERMINA ObtieneListadoReportes()");

                StoreReportes.DataBind();
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Nuevo Reporte
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnCrear_Click(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminReportes);
          
            try
            {
                if (txtClave.Text != "")
                {
                    if (txtClave.Text.Length > 10)
                    {
                        X.Msg.Alert("Nuevo Reporte", "La clave del reporte evento supera los 10 caracteres permitidos a la clave.").Show();
                        return;
                    }
                }

                if (LNReportes.VerificaExisteReporte(txtClave.Text, LH_ParabAdminReportes))
                {
                    X.Msg.Alert("Nuevo Reporte", "La clave del reporte ya Existe.").Show();
                    return;
                }

                logPCI.Info("INICIA AgregarNuevoReporte()");
                LNReportes.AgregarNuevoReporte(Convert.ToInt32(this.cBoxTipoGenRep.SelectedItem.Value), this.txtClave.Text,
                    this.txtNombre.Text, this.txtSP.Text, this.Usuario, LH_ParabAdminReportes);
                logPCI.Info("TERMINA AgregarNuevoReporte()");

                X.Msg.Notify("", "Reporte registrado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridReportes();
                
                RestableceFormulario();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Reporte", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Nuevo Reporte", "Ocurrió un error al crear el registro reporte").Show();
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
        /// Controla el evento de ejecución de comandos en el grid de reportes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminReportes);

            try
            {
                int IdReporte = 0, tipoGen = 0;
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
                        case "ID_Reporte": IdReporte = int.Parse(column.Value); break;
                        case "ClaveReporte": this.txtEditClave.Text = column.Value; break;
                        case "ID_TipoGeneracionReporte":
                            if (int.TryParse(column.Value, out tipoGen)) { this.cBoxEditTipoGenRep.Value = tipoGen; }
                            else { this.cBoxEditTipoGenRep.Value = 0; }
                            break;                            
                               // int.Parse(column.Value); break;
                        case "Nombre": txtEditNombre.Text = column.Value; break;
                        case "Sp": txtEditSp.Text = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = (String)e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Edit":
                        hdnIdReporte.Value = IdReporte;
                        WdwEditar.Show();
                        break;

                    case "Lock":
                    case "Unlock":
                        int estatus = comando == "Lock" ? 0 : 1;
                        string msj = comando == "Lock" ? "Desactivado" : "Activado";

                        log.Info("INICIA ActualizaEstatusReporte()");
                        LNReportes.ActualizaEstatusReporte(IdReporte, Convert.ToBoolean(estatus), this.Usuario, LH_ParabAdminReportes);
                        log.Info("TERMINA ActualizaEstatusReporte()");

                        X.Msg.Notify("", "Reporte " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        LlenaGridReportes();

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
            LogPCI unLog = new LogPCI(LH_ParabAdminReportes);

            try
            {
                unLog.Info("INICIA ModificaDatosReporte()");
                LNReportes.ModificaDatosReporte(Convert.ToInt32(this.hdnIdReporte.Value),
                    Convert.ToInt32(this.cBoxEditTipoGenRep.SelectedItem.Value), this.txtEditNombre.Text, 
                    this.txtEditSp.Text, this.Usuario, LH_ParabAdminReportes);
                unLog.Info("TERMINA ModificaDatosReporte()");

                X.Msg.Notify("Edición de Reporte", "Cambios guardados <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridReportes();

                WdwEditar.Hide();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Edición de Reporte", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Edición de Reporte", ex.Message).Show();
            }
        }




    }
}