using DALCentralAplicaciones;
using DALCentralAplicaciones.BaseDatos;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Administracion
{
    public partial class ConsultaLogsAuditoria : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA Administrar Consulta de Logs
        private LogHeader LH_ConsultaLogs = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Consulta de Logs
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ConsultaLogs.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ConsultaLogs.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ConsultaLogs.User = this.Usuario.ClaveUsuario;
            LH_ConsultaLogs.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ConsultaLogs);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConsultaLogsAuditoria Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaAplicaciones()");
                    DataSet dsAplicaciones = DAOInfoLogs.ListaAplicaciones(this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ConsultaLogs);
                    log.Info("TERMINA ListaAplicaciones()");

                    this.StoreAplicaciones.DataSource = dsAplicaciones;
                    this.StoreAplicaciones.DataBind();

                    log.Info("INICIA ListaUsuarios()");
                    this.StoreUsuario.DataSource = DAOInfoLogs.ListaUsuarios(
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ConsultaLogs);
                    log.Info("TERMINA ListaUsuarios()");
                    this.StoreUsuario.DataBind();

                    DateTime d1 = new DateTime();
                    d1 = DateTime.Today;

                    datInicio.MinDate = d1.AddDays(-7);
                    datFinal.MinDate = DateTime.Today;

                    datInicio.MaxDate = DateTime.Today;
                    datFinal.MaxDate = DateTime.Today;

                    datInicio.SelectedDate = DateTime.Today;
                    datFinal.SelectedDate = DateTime.Today;
                }

                log.Info("TERMINA ConsultaLogsAuditoria Page_Load()");
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
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            this.cBoxAplicaciones.Reset();
            this.txtPantalla.Reset();
            StorePantallas.RemoveAll();

            LimpiaSeleccionPrevia();
            PanelCentral.Disabled = true;
        }

        /// <summary>
        /// Llena el grid de resultados de pantallas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI unLog = new LogPCI(LH_ConsultaLogs);

            try
            {
                LimpiaSeleccionPrevia();

                unLog.Info("INICIA ListaPantallas()");
                DataSet dsPantallas = DAOInfoLogs.ListaPantallas(this.txtPantalla.Text, this.Usuario, this.cBoxAplicaciones.SelectedItem.Value,
                    LH_ConsultaLogs);
                unLog.Info("TERMINA ListaPantallas()");

                if (dsPantallas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Pantallas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StorePantallas.DataSource = dsPantallas;
                    StorePantallas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta Logs", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Consulta Logs", "Ocurrió un error al realizar la Búsqueda.").Show();
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
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarLogs_Click(object sender, EventArgs e)
        {
            LlenaGridLogs();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una colectiva en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            this.FormPanelLogs.Reset();

            this.cBoxUsuarios.Reset();
            this.cBoxAccion.Reset();
            this.datInicio.Reset();
            this.datFinal.Reset();

            this.StoreBitacoraLogs.RemoveAll();
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
                string PantallaAspx = "", NombrePantalla = "", Application_ID = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] pantalla = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in pantalla[0])
                {
                    switch (column.Key)
                    {
                        case "PaginaAspx": PantallaAspx = column.Value; break;
                        case "NombrePantalla": NombrePantalla = column.Value; break;
                        case "Application_ID": Application_ID = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();
                this.hdnPantallaAspx.Text = PantallaAspx;
                this.hdnApplicationID.Text = Application_ID;

                PanelCentral.Title = cBoxAplicaciones.SelectedItem.Text +  " - " + NombrePantalla;
                LlenaAccionesPantalla(PantallaAspx);
                PanelCentral.Disabled = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Consulta Logs", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ConsultaLogs);
                unLog.ErrorException(ex);
                X.Msg.Alert("Consulta Logs", "Ocurrió un error al obtener la información de la Aplicación.").Show();
            }
        }

        /// <summary>
        /// Establece los valores de las acciones por pantalla
        /// </summary>
        protected void LlenaAccionesPantalla(string pantalla)
        {
            LogPCI log = new LogPCI(LH_ConsultaLogs);

            try
            {
                log.Info("INICIA ListaAcciones()");
                this.StoreAccion.DataSource = DAOInfoLogs.ListaAcciones(pantalla, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ConsultaLogs);
                log.Info("TERMINA ListaAcciones()");
                this.StoreAccion.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Consulta Logs", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Consulta Logs", "Ocurrió un error al obtener los Acciones de la pantalla").Show();
            }
        }

        /// <summary>
        /// Obtiene los Logs de la Bitacora con los filtros seleccionados
        /// </summary>
        protected void LlenaGridLogs()
        {
            LogPCI log = new LogPCI(LH_ConsultaLogs);
            string storeds = "";
            string nameSystemValue = "";

            try
            {
                log.Info("INICIA ListaAcciones()");

                DataSet acciones = DAOInfoLogs.ListaAcciones(this.hdnPantallaAspx.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ConsultaLogs);

                log.Info("TERMINA ListaAcciones()");

                foreach (DataRow item in acciones.Tables[0].Rows)
                {
                    if (this.cBoxAccion.SelectedItem.Value == null) // Todos
                    {
                        storeds = storeds + item[0] + ":" + item[1] + ",";
                    }
                    else
                    {
                        if (this.cBoxAccion.SelectedItem.Value == item[1].ToString())
                        {
                            storeds = storeds + item[0] + ":" + item[1] + ",";
                        }
                    }

                    nameSystemValue = item[2].ToString();
                }

                Guid app = new Guid(this.hdnApplicationID.Text);

                log.Info("INICIA ListaLogs()");
                DataSet dsLogs = DAOInfoLogs.ListaLogs(storeds, this.cBoxUsuarios.SelectedItem.Value, Convert.ToDateTime(this.datInicio.SelectedDate),
                    Convert.ToDateTime(this.datFinal.SelectedDate), nameSystemValue, this.Usuario, app, LH_ConsultaLogs);
                log.Info("TERMINA ListaLogs()");

                if (dsLogs.Tables[0].Rows.Count < 1)
                {
                    X.Msg.Alert("Consulta Logs", "No existen coincidencias con la búsqueda solicitada").Show();
                    return;
                }

                this.StoreBitacoraLogs.DataSource = dsLogs;
                this.StoreBitacoraLogs.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Consulta Logs", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Consulta Logs", "Ocurrió un error al obtener los Logs de la Bitacora").Show();
            }
        }
    }
}
