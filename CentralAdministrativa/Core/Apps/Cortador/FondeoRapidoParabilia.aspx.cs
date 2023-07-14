using DALCortador.Entidades;
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
    public partial class FondeoRapidoParabilia : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA FONDEO
        private LogHeader LH_ParabFondeo = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabFondeo.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabFondeo.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabFondeo.User = this.Usuario.ClaveUsuario;
            LH_ParabFondeo.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabFondeo);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA FondeoRapidoParabilia Page_Load()");

                HttpContext.Current.Session.Clear();

                if (!IsPostBack)
                {
                    EstableceRolesAuditables();

                    EstableceControlesPorRol();
                }

                log.Info("TERMINA FondeoRapidoParabilia Page_Load()");
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
            }
        }

        /// <summary>
        /// Controla la visibilidad de los controles del formulario con base en el(los)
        /// rol(es) del usuario en sesión
        /// </summary>
        protected void EstableceControlesPorRol()
        {
            Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());

            if (esAutorizador)
            {
                GridMovimientos.ColumnModel.SetHidden(10, false);
            }

            LlenaGridMovimientos();
        }


        /// <summary>
        /// Solicita el catálogo de clientes a base de datos y lo establece en 
        /// el combo correspondiente
        /// </summary>
        protected void LlenaGridMovimientos()
        {
            LogPCI unLog = new LogPCI(LH_ParabFondeo);

            try
            {
                unLog.Info("INICIA ListaMovimientosFondeoRapidoCacao()");
                DataTable dtMovimientos = DAOEvento.ListaMovimientosFondeoRapidoCacao(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), LH_ParabFondeo);
                unLog.Info("TERMINA ListaMovimientosFondeoRapidoCacao()");

                if (dtMovimientos.Rows.Count < 1)
                {
                    X.Msg.Alert("Movimientos", "No existen registros de movimientos pendientes de autorización").Show();
                }
                else
                {
                    StoreMovimientos.DataSource = dtMovimientos;
                    StoreMovimientos.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Consulta de Movimientos", "Error al consultar los movimientos.").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabFondeo);

            try
            {
                int IDMovFondeo = 0;
                Int64 IdCuenta = 0;
                decimal saldoActual = 0, nuevoSaldo = 0, importe = 0;
                string ejecutor = "", claveEventoAut = "", claveEventoRech = "";

                EventoManual elEvento = new EventoManual();
                string json = String.Format("[{0}]", e.ExtraParams["ValuesFR"]);

                IDictionary<string, string>[] parametroSeleccionado = JSON.Deserialize<Dictionary<string, string>[]>(json);

                if (parametroSeleccionado == null || parametroSeleccionado.Length < 1)
                {
                    return;
                }
                
                foreach (KeyValuePair<string, string> column in parametroSeleccionado[0])
                {
                    switch (column.Key)
                    {
                        case "ID_MovimientoFondeoRapido": IDMovFondeo = int.Parse(column.Value); break;
                        case "ID_Cuenta": IdCuenta = Convert.ToInt64(column.Value); break;
                        case "ID_Colectiva": elEvento.IdColectivaDestino = Convert.ToInt64(column.Value); break;
                        case "ClaveColectiva": elEvento.ClaveColectiva = column.Value; break;
                        case "SaldoActual": saldoActual = decimal.Parse(column.Value); break;
                        case "Importe": elEvento.Importe = column.Value; importe = decimal.Parse(column.Value); break;
                        case "Observaciones": elEvento.Observaciones = column.Value; break;
                        case "UsuarioEjecutor": ejecutor = column.Value; break;
                        case "ClaveEventoAutoriza": claveEventoAut = column.Value.Trim(); break;
                        case "ClaveEventoDeclina": claveEventoRech = column.Value.Trim(); break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];

                switch (comando)
                {
                    case "Autorizar":
                        try
                        {
                            elEvento.ClaveEvento = claveEventoAut;

                            //Ejecuta el Evento Manual
                            pCI.Info("INICIA Autorizar_RegistraEventoFondeoRapido()");
                            LNEvento.RegistraEventoFondeoRapido(elEvento, this.Usuario, LH_ParabFondeo);
                            pCI.Info("TERMINA Autorizar_RegistraEventoFondeoRapido()");

                            //Registra el cambio del saldo de la cuenta en bitácora
                            nuevoSaldo = saldoActual + importe;
                            pCI.Info("INICIA RegistraNuevoSaldoFondeoRapido()");
                            LNEvento.RegistraNuevoSaldoFondeoRapido(IDMovFondeo, IdCuenta, nuevoSaldo.ToString(),
                                ejecutor, this.Usuario, LH_ParabFondeo);
                            pCI.Info("INICIA RegistraNuevoSaldoFondeoRapido()");

                            X.Msg.Notify("Autorización de Movimiento", "El Fondeo Rápido se Realizó<br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }
                        catch (CAppException err)
                        {
                            X.Msg.Alert("Autorización de Movimiento", err.Mensaje()).Show();
                        }
                        catch (Exception ex)
                        {
                            pCI.ErrorException(ex);
                            X.Msg.Alert("Autorización de Movimiento", "Error al autorizar el movimiento.").Show();
                        }
                        break;

                    case "Rechazar":
                        try
                        {
                            elEvento.ClaveEvento = claveEventoRech;

                            //Ejecuta el Evento Manual
                            pCI.Info("INICIA Rechazar_RegistraEventoFondeoRapido()");
                            LNEvento.RegistraEventoFondeoRapido(elEvento, this.Usuario, LH_ParabFondeo);
                            pCI.Info("TERMINA Rechazar_RegistraEventoFondeoRapido()");

                            //Actualiza el movimiento en la tabla de control
                            pCI.Info("INICIA RechazaFondeoRapido()");
                            LNEvento.RechazaFondeoRapido(IDMovFondeo, this.Usuario, LH_ParabFondeo);
                            pCI.Info("TERMINA RechazaFondeoRapido()");

                            X.Msg.Notify("Rechazo de Movimiento", "El Fondeo Rápido se Rechazó<br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }
                        catch (CAppException err)
                        {
                            X.Msg.Alert("Rechazo de Movimiento", err.Mensaje()).Show();
                        }

                        catch (Exception ex)
                        {
                            pCI.ErrorException(ex);
                            X.Msg.Alert("Rechazo de Movimiento", "Error al rechazar el movimiento.").Show();
                        }

                        break;

                    default: break;
                }

                LlenaGridMovimientos();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Acción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreMovimientos_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            LlenaGridMovimientos();
        }
    }
}