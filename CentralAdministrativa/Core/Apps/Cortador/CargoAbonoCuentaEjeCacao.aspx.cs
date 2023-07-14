using DALCortador.Entidades;
using DALEventos.BaseDatos;
using DALEventos.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Configuration;
using System.Data;
using System.Web;

namespace Cortador
{
    public partial class CargoAbonoCuentaEjeCacao : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER 
        private LogHeader LH_ParabCargoAbonoCtaEje = new LogHeader();

        #endregion

        /// <summary>
        /// Controla el evento inicial de carga de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabCargoAbonoCtaEje.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabCargoAbonoCtaEje.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabCargoAbonoCtaEje.User = this.Usuario.ClaveUsuario;
            LH_ParabCargoAbonoCtaEje.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabCargoAbonoCtaEje);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA CargoAbonoCuentaEjeCacao Page_Load()");

                HttpContext.Current.Session.Clear();

                if (!IsPostBack)
                {
                    EstableceRolesAuditables();

                    EstableceControlesPorRol();
                }

                log.Info("TERMINA CargoAbonoCuentaEjeCacao Page_Load()");
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
        /// Controla la visibilidad de los controles del formulario con base en el(los)
        /// rol(es) del usuario en sesión
        /// </summary>
        protected void EstableceControlesPorRol()
        {
            Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
            Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

            if (esEjecutor)
            {
                LlenaComboClientes();

                FormPanelCaptura.Disabled = false;
                cBoxTipoMov.Disabled = false;
                txtImporte.Disabled = true;
            }

            if (esAutorizador)
            {
                GridMovimientos.ColumnModel.SetHidden(11, false);
            }

            LlenaGridMovimientos();
        }

        /// <summary>
        /// Solicita el catálogo de clientes a base de datos y lo establece en 
        /// el combo correspondiente
        /// </summary>
        protected void LlenaComboClientes()
        {
            LogPCI unLog = new LogPCI(LH_ParabCargoAbonoCtaEje);

            try
            {
                unLog.Info("INICIA ListaClientesCuentasEjeCacao");
                StoreColectivasCuentasEje.DataSource = DAOEvento.ListaClientesCuentasEjeCacao(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), LH_ParabCargoAbonoCtaEje);
                unLog.Info("TERMINA ListaClientesCuentasEjeCacao");

                StoreColectivasCuentasEje.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Clientes", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Consulta de Clientes", "Error al obtener los Clientes.").Show();
            }
        }

        /// <summary>
        /// Solicita el catálogo de clientes a base de datos y lo establece en 
        /// el combo correspondiente
        /// </summary>
        protected void LlenaGridMovimientos()
        {
            LogPCI pCI = new LogPCI(LH_ParabCargoAbonoCtaEje);

            try
            {
                pCI.Info("INICIA ListaMovimientosCuentasEjeCacao()");
                DataTable dtMovimientos = DAOEvento.ListaMovimientosCuentasEjeCacao(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), LH_ParabCargoAbonoCtaEje);
                pCI.Info("TERMINA ListaMovimientosCuentasEjeCacao()");

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
                pCI.ErrorException(ex);
                X.Msg.Alert("Consulta de Movimientos", "Error al obtener los Movimientos.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnGuardar_Click(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabCargoAbonoCtaEje);

            try
            {
                log.Info("INICIA NuevoMovimientoCuentaEje()");
                LNEvento.NuevoMovimientoCuentaEje(int.Parse(cBoxCliente.SelectedItem.Value),
                    hdnNuevoSaldo.Value.ToString(), txtImporte.Text, int.Parse(cBoxTipoMov.SelectedItem.Value),
                    txtObservaciones.Text, this.Usuario, LH_ParabCargoAbonoCtaEje);
                log.Info("TERMINA NuevoMovimientoCuentaEje()");

                X.Msg.Notify("Captura de Movimiento", "<br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                RestableceFormulario();

                LlenaGridMovimientos();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Captura de Movimiento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Captura de Movimiento", "Ocurrió un error al registrar el Movimiento").Show();
            }
        }

        /// <summary>
        /// Restablece el formulario a su estado de carga de la página
        /// </summary>
        protected void RestableceFormulario()
        {
            FormPanelCaptura.Reset();
            cBoxCliente.Reset();
            hdnSaldoActual.Clear();
            hdnNuevoSaldo.Clear();

            cBoxTipoMov.Reset();

            txtImporte.Reset();
            txtImporte.Disabled = true;

            txtObservaciones.Reset();
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabCargoAbonoCtaEje);

            try
            {
                char[] charsToTrim = { '*', '"', ' ' };
                String comando = (String)e.ExtraParams["Comando"];

                int IdMovimientoCuentaEje = Convert.ToInt32(e.ExtraParams["ID_MovimientoCuentaEje"]);
                int IdTipoMovimiento = Convert.ToInt32(e.ExtraParams["TipoMovimiento"]);
                string Importe = e.ExtraParams["Importe"].Trim(charsToTrim);
                string Observaciones = e.ExtraParams["Observaciones"].Trim(charsToTrim);
                Int64 IdCuenta = Convert.ToInt64(e.ExtraParams["ID_Cuenta"]);
                Int64 IdColectiva = Convert.ToInt64(e.ExtraParams["ID_Colectiva"]);
                string SaldoActual = e.ExtraParams["SaldoActual"].Trim(charsToTrim);
                string UsuarioEjecutor = e.ExtraParams["UsuarioEjecutor"].Trim(charsToTrim);

                switch (comando)
                {
                    case "Aceptar":
                        try
                        {
                            AutorizaMovimientoSaldo(IdMovimientoCuentaEje, IdTipoMovimiento, Importe,
                                Observaciones, IdCuenta, IdColectiva, SaldoActual, UsuarioEjecutor);

                            X.Msg.Notify("Autorización de Movimiento", "El Movimiento de Saldo de la Cuenta Eje se Realizó<br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }
                        catch (CAppException err)
                        {
                            X.Msg.Alert("Autorización de Movimiento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
                        }
                        catch (Exception ex)
                        {
                            unLog.ErrorException(ex);
                            X.Msg.Alert("Autorización de Movimiento", "Error al autorizar el movimiento.").Show();
                        }

                        break;

                    case "Rechazar":
                        try
                        {
                            RechazarMovimiento(IdMovimientoCuentaEje);

                            X.Msg.Notify("Rechazo de Movimiento", "El Movimiento al Saldo de la Cuenta Eje se Rechazó<br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        }
                        catch (CAppException err)
                        {
                            X.Msg.Alert("Rechazo de Movimiento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
                        }
                        catch (Exception ex)
                        {
                            unLog.ErrorException(ex);
                            X.Msg.Alert("Rechazo de Movimiento", "Error al rechazar el movimiento.").Show();
                        }

                        break;

                    default: break;
                }

                LlenaGridMovimientos();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Acción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }


        /// <summary>
        /// Solicita y controla la ejecución del evento manual correspondiente, así como
        /// el registro del movimiento de la cuenta en la tabla de control
        /// </summary>
        /// <param name="IdMovimientoCuentaEje">Identificador del movimiento en la tabla de control</param>
        /// <param name="IdTipoMovimiento">Identificador del tipo de movimiento (1 => Fondeo, 2 => Retiro) </param>
        /// <param name="Importe">Importe del movimiento</param>
        /// <param name="Observaciones">Observaciones del movimiento</param>
        /// <param name="IdCuenta">Identificador de la cuenta por afectar</param>
        /// <param name="SaldoActual">Saldo actual de la cuenta por afectar</param>
        /// <param name="Ejecutor">Nombre de usuario que capturó el movimiento</param>
        protected void AutorizaMovimientoSaldo(int IdMovimientoCuentaEje, int IdTipoMovimiento, string Importe,
            string Observaciones, Int64 IdCuenta, Int64 IdColectiva, string SaldoActual, string Ejecutor)
        {
            LogPCI pCI = new LogPCI(LH_ParabCargoAbonoCtaEje);

            try
            {
                float nuevoSaldo = 0;
                DataSet dsEvento = new DataSet();
                EventoCargoAbonoCuentaEjeCacao elEvento = new EventoCargoAbonoCuentaEjeCacao();
                                
                if (IdTipoMovimiento == 1) //Aumento
                {
                    pCI.Info("INICIA ConsultaEventoFondeoCliente()");
                    dsEvento = DAOEvento.ConsultaEventoFondeoCliente(IdColectiva, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabCargoAbonoCtaEje);
                    pCI.Info("TERMINA ConsultaEventoFondeoCliente()");

                    nuevoSaldo = float.Parse(SaldoActual) + float.Parse(Importe);
                }
                else //Decremento
                {
                    pCI.Info("INICIA ConsultaEventoDecrementoCliente()");
                    dsEvento = DAOEvento.ConsultaEventoDecrementoCliente(IdColectiva, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        LH_ParabCargoAbonoCtaEje);
                    pCI.Info("TERMINA ConsultaEventoDecrementoCliente()");

                    nuevoSaldo = float.Parse(SaldoActual) - float.Parse(Importe);
                }

                elEvento.IdCadenaComercial = Convert.ToInt64(dsEvento.Tables[0].Rows[0]["IdCadenaComercial"].ToString());
                elEvento.IdGrupoComercial = IdColectiva;
                elEvento.ClaveColectiva = dsEvento.Tables[0].Rows[0]["ClaveColectiva"].ToString().Trim();
                elEvento.IdEvento = Convert.ToInt32(dsEvento.Tables[0].Rows[0]["ID_Evento"].ToString());
                elEvento.ClaveEvento = dsEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();
                elEvento.Concepto = dsEvento.Tables[0].Rows[0]["Descripcion"].ToString();
                elEvento.IdCuentahabiente = IdColectiva;
                elEvento.Importe = Importe;
                elEvento.Observaciones = Observaciones;

                //Se consultan los parámetros del contrato
                pCI.Info("INICIA RegistraEventoAjusteSaldoCliente()");
                LNEvento.RegistraEventoAjusteSaldoCliente(elEvento, this.Usuario, LH_ParabCargoAbonoCtaEje);
                pCI.Info("INICIA RegistraEventoAjusteSaldoCliente()");

                //Registra el cambio del saldo de la cuenta en bitácora
                pCI.Info("INICIA RegistraNuevoSaldoCuentaEje()");
                LNEvento.RegistraNuevoSaldoCuentaEje(IdMovimientoCuentaEje, IdCuenta, nuevoSaldo.ToString(), 
                    Ejecutor, this.Usuario, LH_ParabCargoAbonoCtaEje);
                pCI.Info("TERMINA RegistraNuevoSaldoCuentaEje()");
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla la llamada a los objetos de datos para registrar un rechazo de 
        /// ajuste al saldo de una cuenta eje
        /// </summary>
        /// <param name="IdMovimientoCuentaEje">Identificador del movimiento en la tabla de control</param>
        protected void RechazarMovimiento(int IdMovimientoCuentaEje)
        {
            LogPCI unLog = new LogPCI(LH_ParabCargoAbonoCtaEje);

            try
            {
                unLog.Info("INICIA RechazaCambioSaldoCuentaEje()");
                LNEvento.RechazaCambioSaldoCuentaEje(IdMovimientoCuentaEje, this.Usuario, LH_ParabCargoAbonoCtaEje);
                unLog.Info("TERMINA RechazaCambioSaldoCuentaEje()");
            }
            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw ex;
            }
        }
    }
}