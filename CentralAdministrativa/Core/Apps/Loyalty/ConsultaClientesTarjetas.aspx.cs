using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Utilerias;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace CentroContacto
{
    /// <summary>
    /// Realiza y controla la carga de la página Consulta de Clientes Tarjetas
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class ConsultaClientesTarjetas : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilium Consulta de Clientes Tarjetas
        private LogHeader LH_ParabConsCliTarjetas = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabConsCliTarjetas.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabConsCliTarjetas.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabConsCliTarjetas.User = this.Usuario.ClaveUsuario;
            LH_ParabConsCliTarjetas.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabConsCliTarjetas);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA ConsultaClientesTarjetas Page_Load()");

                if (!IsPostBack)
                {
                    EstableceCatalogos();

                    this.dfInicioSYM.MaxDate = DateTime.Today;
                    this.dfInicioSYM.MinDate = DateTime.Today.AddMonths(-3);

                    this.dfFinSYM.MaxDate = DateTime.Today;

                    PagingTBSaldosYMovs.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabConsCliTarjetas).Valor);

                    HttpContext.Current.Session.Add("DtSyMCtesTarj", null);
                    HttpContext.Current.Session.Add("DtOpsPendTarj", null);
                    HttpContext.Current.Session.Add("tknDNUUsrs", null);
                    HttpContext.Current.Session.Add("credDNUUsrs", null);


                    this.PagingHistorial.PageSize = 10;
                    HttpContext.Current.Session.Add("DtHistLlamadas", null);
                }

                log.Info("TERMINA ConsultaClientesTarjetas Page_Load()");
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
        /// Establece los valores del combo SubEmisores con la información de base de datos
        /// </summary>
        protected void EstableceCatalogos()
        {
            LogPCI log = new LogPCI(LH_ParabConsCliTarjetas);

            log.Info("INICIA ListaColectivasSubEmisor()");
            DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabConsCliTarjetas);
            log.Info("TERMINA ListaColectivasSubEmisor()");

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


            log.Info("INICIA ObtieneMotivosLlamada()");
            this.StoreMotivos.DataSource = DAOConsultaClientes.ObtieneMotivosLlamada(LH_ParabConsCliTarjetas);
            log.Info("TERMINA ObtieneMotivosLlamada()");
            this.StoreMotivos.DataBind();
        }

        /// <summary>
        /// Llena el grid de resultado de búsqueda de cuentas
        /// </summary>
        protected void LlenarGridResultados()
        {
            LogPCI logPCI = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                limpiaBusquedaPrevia(false);
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.ID_Cuenta = String.IsNullOrEmpty(this.txtNumCuenta.Text) ? -1 :
                    int.Parse(this.txtNumCuenta.Text);
                datosPersonales.TarjetaTitular = this.txtNumTarjeta.Text;
                datosPersonales.SoloAdicionales = this.chkBoxSoloAdicionales.Checked;
                datosPersonales.Nombre = this.txtNombre.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaterno.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaterno.Text;

                logPCI.Info("INICIA ObtieneCuentasTDCSubEmisor()");
                DataSet dsResultados = DAOTarjetaCuenta.ObtieneCuentasTDCSubEmisor(
                    this.cBoxSubEm.SelectedItem.Value == null ? -1 :
                    Convert.ToInt32(this.cBoxSubEm.SelectedItem.Value), datosPersonales, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabConsCliTarjetas);
                logPCI.Info("TERMINA ObtieneCuentasTDCSubEmisor()");

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

                DataTable dt = Tarjetas.EnmascaraTarjetasSolo4Dig(dsResultados.Tables[0], "NumTarjeta", LH_ParabConsCliTarjetas);

                this.StoreCuentas.DataSource = dt;
                this.StoreCuentas.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Consulta de Cuentas", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Consulta de Cuentas", "Ocurrió un error al obtener las Cuentas").Show();
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
                this.EastPanelCCT.Title = "_";
            }

            this.FormPanelTitular.Reset();

            this.StoreFamTarjetas.RemoveAll();

            this.FormPanelSaldosYMovs.Reset();
            this.dfInicioSYM.Reset();
            this.dfFinSYM.Reset();
            this.FormPanelResumenMovs.Reset();
            this.FieldSetResMovs.Collapse();
            this.FormPanelResumenMovsCorte.Reset();
            this.FieldSetResMovsCorte.Collapse();
            this.btnMovimientos.Disabled = true;

            this.FormPanelLlamadas.Reset();
            this.txtTokenSMS.Reset();
            LimpiaGridHistorial();
            DeshabilitaPaneles();

            this.EastPanelCCT.Disabled = true;

            this.FormPanelLlamadas.Show();
        }

        /// <summary>
        /// Mantiene deshabilitados los paneles con información del tarjetahabiente
        /// </summary>
        protected void DeshabilitaPaneles()
        {
            this.FormPanelTitular.Disabled = true;
            this.FormPanelTarjetas.Disabled = true;
            this.FormPanelSaldosYMovs.Disabled = true;
        }

        /// <summary>
        /// Restablece a su estado de carga inicial los controles y variables relacionados al grid historial
        /// </summary>
        protected void LimpiaGridHistorial()
        {
            this.btnExcelHistorial.Disabled = true;

            HttpContext.Current.Session.Add("DtHistLlamadas", null);

            this.StoreHistorial.RemoveAll();
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
                string numTarjeta = string.Empty;
                string numCel = string.Empty;
                StringBuilder sbMsj = new StringBuilder();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "CLDC": this.hdnIdCuentaLDC.Value = column.Value; break;
                        case "CDC": this.hdnIdCuentaCDC.Value = column.Value; break;
                        case "CCLC": this.hdnIdCuentaCC.Value = column.Value; break;
                        case "IdTarjeta": this.hdnIdMA.Value = column.Value; break;
                        case "IdColectivaCuentahabiente": this.hdnIdColectiva.Value = column.Value; break;
                        case "ClaveMA": this.hdnTarjeta.Value = column.Value; break;
                        case "NumTarjeta": numTarjeta = column.Value; break;
                        case "MaskCel": numCel = column.Value; break;
                        case "TelCelular": this.hdnNumCel.Value = column.Value; break;
                        case "NombreTarjetahabiente": this.hdnTarjetahabiente.Value = column.Value; break;
                        default:
                            break;
                    }
                }

                if (string.IsNullOrEmpty(numCel))
                {
                    throw new CAppException(8006, "El cliente no tiene un número telefónico celular registrado y no es posible su atenticación. Solicita que lo registre.");
                }

                limpiaBusquedaPrevia(false);

                this.hdnMaskCard.Value = MaskSensitiveData.cardNumber4Digits(numTarjeta);

                sbMsj.AppendFormat("Se enviará un mensaje SMS para autenticar a <b>{0}</b>, con No. de tarjeta " +
                    "<b>{1}</b>.<br /><br />Confirma con el cliente si el número telefónico celular registrado <b>{2}</b> sigue siendo su " +
                    "número, y si lo tiene disponible en este momento.<br />", this.hdnTarjetahabiente.Value.ToString(),
                    this.hdnMaskCard.Value.ToString(), numCel);

                DeshabilitaPaneles();

                X.Msg.Confirm("Cuenta", sbMsj.ToString(), new MessageBoxButtonsConfig
                {
                    Yes = new MessageBoxButtonConfig
                    {
                        Handler = "ConsultaClientesTarjetas.SolicitaEnvioSMS()",
                        Text = "Enviar SMS"
                    },
                    No = new MessageBoxButtonConfig
                    {
                        Handler = "ConsultaClientesTarjetas.CancelarSMS()",
                        Text = "Cancelar y Registrar Llamada"
                    }
                }).Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabConsCliTarjetas);
                unLog.ErrorException(ex);
                X.Msg.Alert("Cuenta", "Ocurrió un error al obtener la información de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Solicta el envío del mensaj SMS al clliente, desde el servicio web
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void SolicitaEnvioSMS()
        {
            LogPCI log = new LogPCI(LH_ParabConsCliTarjetas);
            log.Info("SolicitaEnvioSMS()");
            X.Mask.Show(new MaskConfig { Msg = "Enviando mensaje SMS..." });

            try
            {
                Parametros.Headers _headersSMS1 = new Parametros.Headers();
                Parametros.LoginDnuBody _bodyLogin = new Parametros.LoginDnuBody();
                Parametros.SmsBody _bodySMS1 = new Parametros.SmsBody();

                _headersSMS1.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsDnuUsrs_URL", LH_ParabConsCliTarjetas).Valor;

                _bodyLogin.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsDnuUsrs_Usr", LH_ParabConsCliTarjetas).Valor;
                _bodyLogin.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsDnuUsrs_Pwd", LH_ParabConsCliTarjetas).Valor;
                _bodyLogin.Aplicacion = Guid.Parse(Configuracion.Get(Guid.Parse(
                ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "WsDnuUsrs_AppId", LH_ParabConsCliTarjetas).Valor.Trim());
                _bodyLogin.Cifrado = "0";

                log.Info("INICIA DNUUsuarios.Login()");
                string wsLoginResp = DNUUsuarios.Login(_headersSMS1, _bodyLogin, LH_ParabConsCliTarjetas);
                log.Info("TERMINA DNUUsuarios.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headersSMS1.Token = wsLoginResp;
                _headersSMS1.Credenciales = Cifrado.Base64Encode(_bodyLogin.NombreUsuario + ":" + _bodyLogin.Password);
                HttpContext.Current.Session.Add("tknDNUUsrs", _headersSMS1.Token);
                HttpContext.Current.Session.Add("credDNUUsrs", _headersSMS1.Credenciales);

                _bodySMS1.UserID = this.Usuario.UsuarioId;
                _bodySMS1.NombreUsuario = this.Usuario.ClaveUsuario;
                _bodySMS1.Telefono = this.hdnNumCel.Value.ToString();

                log.Info("INICIA DNUUsuarios.PutSMS()");
                DNUUsuarios.PutSMS(_headersSMS1, _bodySMS1, LH_ParabConsCliTarjetas);
                log.Info("TERMINA DNUUsuarios.PutSMS()");

                X.Msg.Notify("", "Mensaje SMS enviado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                Thread.Sleep(2000);

                this.txtTokenSMS.Reset();
                this.WdwConfirmaTokenSMS.Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Envío de SMS", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Envío de SMS", "Ocurrió un error en el envío del mensaje SMS").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Establece como visibles los controles básicos necesarios para registrar una llamada
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void CancelarSMS()
        {
            this.txtUsuarioLlama.Text = this.hdnTarjetahabiente.Value.ToString();
            this.btnHistLlamadas.FireEvent("click");
            this.EastPanelCCT.Disabled = false;
        }

        /// <summary>
        /// Controla el evento Click al botón Validar Token, solicitando la validación al servicio web
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValidaToken_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                Parametros.Headers _headersSMS2 = new Parametros.Headers();
                Parametros.SmsBody _bodySMS2 = new Parametros.SmsBody();

                _headersSMS2.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsDnuUsrs_URL", LH_ParabConsCliTarjetas).Valor;
                _headersSMS2.Token = HttpContext.Current.Session["tknDNUUsrs"].ToString();
                _headersSMS2.Credenciales = HttpContext.Current.Session["credDNUUsrs"].ToString();
                _bodySMS2.UserID = this.Usuario.UsuarioId;
                _bodySMS2.NombreUsuario = this.Usuario.ClaveUsuario;
                _bodySMS2.TokenSMS = this.txtTokenSMS.Text;

                log.Info("INICIA DNUUsuarios.PostSMS()");
                DNUUsuarios.PostSMS(_headersSMS2, _bodySMS2, LH_ParabConsCliTarjetas);
                log.Info("TERMINA DNUUsuarios.PostSMS()");

                X.Msg.Notify("", "Validación de Token" + "<br />  <br /> <b> E X I T O S A </b> <br />  <br /> ").Show();

                this.btnHistLlamadas.FireEvent("click");

                LlenaFieldSetDatosTitular();
                this.FormPanelTitular.Disabled = false;

                LlenaGridTarjetas();
                this.FormPanelTarjetas.Disabled = false;

                this.FormPanelSaldosYMovs.Disabled = false;
                this.txtUsuarioLlama.Text = this.hdnTarjetahabiente.Value.ToString();

                this.WdwConfirmaTokenSMS.Hide();
                this.EastPanelCCT.Title = "Tarjeta " + this.hdnMaskCard.Value;
                this.EastPanelCCT.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Envío de SMS", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Envío de SMS", "Ocurrió un error en el envío del mensaje SMS").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreHistorial_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridHistorial(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid, así como el ordenamiento y la paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridHistorial(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabConsCliTarjetas);
            this.btnExcelHistorial.Disabled = true;

            try
            {
                DataTable dtLlamadas = new DataTable();

                dtLlamadas = HttpContext.Current.Session["DtHistLlamadas"] as DataTable;

                if (dtLlamadas == null)
                {
                    unLog.Info("INICIA ObtieneHistorialLlamadas()");
                    dtLlamadas = DAOReportes.ObtieneHistorialLlamadas(
                        Convert.ToInt32(this.hdnIdMA.Value), LH_ParabConsCliTarjetas);
                    unLog.Info("TERMINA ObtieneHistorialLlamadas()");

                    HttpContext.Current.Session.Add("DtHistLlamadas", dtLlamadas);
                }

                int TotalRegistros = dtLlamadas.Rows.Count;

                if (TotalRegistros > 0)
                {
                    (this.StoreHistorial.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtLlamadas.Clone();
                    DataTable dtToGrid = dtLlamadas.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtLlamadas.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingHistorial.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingHistorial.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtLlamadas.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    this.StoreHistorial.DataSource = dtToGrid;
                    this.StoreHistorial.DataBind();
                    this.btnExcelHistorial.Disabled = false;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Historial de Llamadas", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Historial de Llamadas", "Ocurrió un error al obtener los usuarios.").Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón oculto Download, sólo para poder llamar
        /// a la exportación del reporte a Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnExcelHistorial_Click(object sender, DirectEventArgs e)
        {
            try
            {
                string reportName = "HistorialLlamadas";
                DataTable _dtOperaciones = HttpContext.Current.Session["DtHistLlamadas"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtOperaciones, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabConsCliTarjetas);
                pCI.Error("Error al exportar el historial a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Historial de Llamadas", "Ocurrió un Error al Exportar el Reposrte a Excel").Show();
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                }
                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Llena el FieldSet con los datos personales de la cuenta seleccionada
        /// </summary>
        protected void LlenaFieldSetDatosTitular()
        {
            LogPCI pCI = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                int idCuenta = Convert.ToInt32(this.hdnIdCuentaLDC.Value);
                int idTarjeta = Convert.ToInt32(this.hdnIdMA.Value);
                int idColectiva = Convert.ToInt32(this.hdnIdColectiva.Value);
                string nombreEmbozo = string.Empty;

                FormPanelTitular.Reset();

                pCI.Info("INICIA ObtieneDatosPersonalesCuenta()");
                DatosPersonalesCuenta losDatos = DAOTarjetaCuenta.ObtieneDatosPersonalesCuenta(
                   idTarjeta, idColectiva, ref nombreEmbozo, this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_ParabConsCliTarjetas);
                pCI.Info("TERMINA ObtieneDatosPersonalesCuenta()");

                this.txtNombreClienteTitular.Text = losDatos.Nombre;
                this.txtApPaternoTitular.Text = losDatos.ApellidoPaterno;
                this.txtApMaternoTitular.Text = losDatos.ApellidoMaterno;
                this.txtRFCTitular.Text = losDatos.RFC;
                this.txtNombreEmbozo.Text = nombreEmbozo;
                this.dfFechaNac.Value = losDatos.FechaNacimiento;
                this.txtCURP.Text = losDatos.CURP;

                this.txtID_Direccion.Text = losDatos.IdDireccion.ToString();
                this.txtCalle.Text = losDatos.Calle;
                this.txtNumExterior.Text = losDatos.NumExterior;
                this.txtNumInterior.Text = losDatos.NumInterior;
                this.txtEntreCalles.Text = losDatos.EntreCalles;
                this.txtReferencias.Text = losDatos.Referencias;
                this.txtCPCliente.Text = losDatos.CodigoPostal;
                this.txtIDColonia.Text = losDatos.IdColonia.ToString();
                this.txtColonia.Text = losDatos.Colonia;
                this.txtClaveMunicipio.Text = losDatos.ClaveMunicipio;
                this.txtMunicipioTitular.Text = losDatos.Municipio;
                this.txtClaveEstado.Text = losDatos.ClaveEstado;
                this.txtEstadoTitular.Text = losDatos.Estado;

                this.txtTelParticular.Text = string.IsNullOrEmpty(losDatos.NumTelParticular) ? "" :
                    MaskSensitiveData.generalInfo(losDatos.NumTelParticular);
                this.txtTelCelular.Text = string.IsNullOrEmpty(losDatos.NumTelCelular) ? "" :
                    MaskSensitiveData.generalInfo(losDatos.NumTelCelular);
                this.txtTelTrabajo.Text = string.IsNullOrEmpty(losDatos.NumTelTrabajo) ? "" :
                    MaskSensitiveData.generalInfo(losDatos.NumTelTrabajo);
                this.txtCorreo.Text = string.IsNullOrEmpty(losDatos.Email) ? "" :
                    MaskSensitiveData.Email(losDatos.Email);

                this.txtIDDirFiscal.Text = losDatos.IdDireccionFiscal.ToString();
                this.txtCalleFiscal.Text = losDatos.CalleFiscal;
                this.txtNumExtFiscal.Text = losDatos.NumExteriorFiscal;
                this.txtNumIntFiscal.Text = losDatos.NumInteriorFiscal;
                this.txtCPFiscal.Text = losDatos.CodigoPostalFiscal;
                this.txtIDColFiscal.Text = losDatos.IdColoniaFiscal.ToString();
                this.txtColFiscal.Text = losDatos.ColoniaFiscal;
                this.txtCveMunFiscal.Text = losDatos.ClaveMunicipioFiscal;
                this.txtDelMunFiscal.Text = losDatos.MunicipioFiscal;
                this.txtCveEdoFiscal.Text = losDatos.ClaveEstadoFiscal;
                this.txtEstadoFiscal.Text = losDatos.EstadoFiscal;
                this.txtRegimenFiscal.Text = losDatos.RegimenFiscal;
                this.txtUsoCFDI.Text = losDatos.UsoCFDI;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Datos del Cliente", "Ocurrió un error al obtener los Datos del Cliente").Show();
            }
        }

        /// <summary>
        /// Llena el GridPanel Tarjetas con los datos de la familia de la tarjeta seleccionada
        /// </summary>
        protected void LlenaGridTarjetas()
        {
            LogPCI pCI = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                int IdTarjeta = Convert.ToInt32(this.hdnIdMA.Value);

                this.StoreFamTarjetas.RemoveAll();

                pCI.Info("INICIA ObtieneFamTarjetas()");
                DataSet dsFamTarjetas = DAOTarjetaCuenta.ObtieneFamTarjetas(IdTarjeta, LH_ParabConsCliTarjetas);
                pCI.Info("TERMINA ObtieneFamTarjetas()");

                DataTable dtEnmascara = Tarjetas.EnmascaraTarjetasSolo4Dig(dsFamTarjetas.Tables[0], "Tarjeta", LH_ParabConsCliTarjetas);

                this.StoreFamTarjetas.DataSource = dtEnmascara;
                this.StoreFamTarjetas.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Tarjetas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoTarjetas(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                string wsActResp = null;
                char[] charsToTrim = { '*', '"', ' ' };
                string comando = e.ExtraParams["Comando"];
                string idMA = e.ExtraParams["ID_MA"].Trim(charsToTrim);
                string Tarjeta = e.ExtraParams["ClaveMA"].Trim(charsToTrim);
                string TarjetaEnmasc = e.ExtraParams["Tarjeta"].Trim(charsToTrim);

                switch (comando)
                {
                    case "Activar":
                        log.Info("INICIA LoginWebServiceParabilium()");
                        Parametros.Headers _headers = LNCuentas.LoginWebServiceParabilium(LH_ParabConsCliTarjetas);
                        log.Info("TERMINA LoginWebServiceParabilium()");

                        log.Info("INICIA ActivaTarjetaWebServiceParabilium()");
                        wsActResp = LNCuentas.ActivaTarjetaWebServiceParabilium(_headers, Tarjeta,
                            MaskSensitiveData.cardNumber(Tarjeta), LH_ParabConsCliTarjetas);
                        log.Info("TERMINA ActivaTarjetaWebServiceParabilium()");

                        log.Info("INICIA RegistraEnBitacora()");
                        LNInfoOnBoarding.RegistraEnBitacora("pantalla_ConsultaClientesTarjetas_ActivarTarjeta",
                            "MedioAcceso", "ID_EstatusMA", idMA, "1", "Activación Tarjeta: " + MaskSensitiveData.cardNumber(Tarjeta),
                            this.Usuario, LH_ParabConsCliTarjetas);
                        log.Info("TERMINA RegistraEnBitacora()");

                        X.Msg.Notify("", "Tarjeta Activada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridTarjetas();
                        break;

                    case "Bloquear":
                        this.hdnSelectedCard.Value = Tarjeta;
                        this.hdnSelectedIdMA.Value = idMA;
                        this.cBoxMotivo.Reset();
                        this.WdwConfirmaBloqueo.Title += " " + TarjetaEnmasc;
                        this.WdwConfirmaBloqueo.Show();

                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Activación", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Acciones", "Ocurrió un error al ejecutar la acción seleccionada.").Show();
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de saldos y movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreSaldosYMovs_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridSaldosYMovs(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridSaldosYMovs(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI pCI = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                DataTable dtOpsPendientes = new DataTable();

                dtOpsPendientes = HttpContext.Current.Session["DtSyMCtesTarj"] as DataTable;

                if (dtOpsPendientes == null)
                {
                    pCI.Info("INICIA ObtieneDetalleMovimientos()");
                    dtOpsPendientes = DAOTarjetaCuenta.ObtieneDetalleMovimientos(
                        this.dfInicioSYM.SelectedDate, this.dfFinSYM.SelectedDate,
                        Convert.ToInt64(this.hdnIdCuentaCC.Value), Convert.ToInt64(this.hdnIdCuentaCDC.Value), LH_ParabConsCliTarjetas);
                    pCI.Info("TERMINA ObtieneDetalleMovimientos()");

                    HttpContext.Current.Session.Add("DtSyMCtesTarj", dtOpsPendientes);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabConsCliTarjetas).Valor);

                if (dtOpsPendientes.Rows.Count < 1)
                {
                    X.Msg.Alert("Saldos y Movimientos", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else if (dtOpsPendientes.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Saldos y Movimientos", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "ConsultaClientesTarjetas.ClicDetSaldosYMovsDePaso()",
                                Text = "Aceptar"
                            },
                            No = new MessageBoxButtonConfig
                            {
                                Text = "Cancelar"
                            }
                        }).Show();
                }
                else
                {
                    int TotalRegistros = dtOpsPendientes.Rows.Count;

                    (this.StoreSaldosYMovs.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsPendientes.Clone();
                    DataTable dtToGrid = dtOpsPendientes.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsPendientes.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTBSaldosYMovs.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTBSaldosYMovs.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsPendientes.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreSaldosYMovs.DataSource = dtToGrid;
                    StoreSaldosYMovs.DataBind();

                    this.btnDetMovsPendHide.FireEvent("click");
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Saldos y Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Saldos y Movimientos", "Ocurrió un error al obtener el Detalle de Movimientos").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte de detalle a Excel
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void ClicDetSaldosYMovsDePaso()
        {
            btnDownloadHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto DownloadDetalle, sólo para poder llamar
        /// a la exportación del reporte a Excel del detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadSaldosyMovs(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        /// <summary>
        /// Exporta el reporte de detalle de movimientos, previamente consultado, a un archivo Excel
        /// cuando dicho reporte excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void ExportDTDetalleToExcel()
        {
            try
            {
                string reportName = "SaldosYMovimientos";
                DataTable _dtSaldosYMovs = HttpContext.Current.Session["DtSyMCtesTarj"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSaldosYMovs, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWSSaldosYMovs(ws, ws.Column(1).CellsUsed().Count());

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabConsCliTarjetas);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Saldos y Movimientos", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWSSaldosYMovs(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 3).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 7).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 8).SetDataType(XLCellValues.Number);
                    ws.Cell(rowsCounter, 17).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 18).SetDataType(XLCellValues.DateTime);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento onRefresh del grid de operaciones pendientes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo (StoreRefresh) que se ejecutó</param>
        protected void StoreOpsPendientes_RefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int inicio = e.Start;
            string columna = e.Sort;
            SortDirection orden = e.Dir;

            LlenaGridOpsPendientes(inicio, columna, orden);
        }

        /// <summary>
        /// Controla la alimentación de datos del grid de detalle, así como el ordenamiento y la
        /// paginación del mismo
        /// </summary>
        /// <param name="RegistroInicial">Número de registro con que inicia la página</param>
        /// <param name="Columna">Nombre de la columna a la que se solicitó su ordenamiento</param>
        /// <param name="Orden">Dirección de orden de datos (ascendente o descendente)</param>
        protected void LlenaGridOpsPendientes(int RegistroInicial, string Columna, SortDirection Orden)
        {
            LogPCI unLog = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                DataTable dtOpsPendientes = new DataTable();

                dtOpsPendientes = HttpContext.Current.Session["DtOpsPendTarj"] as DataTable;

                if (dtOpsPendientes == null)
                {
                    unLog.Info("INICIA ObtieneOperacionesEnTransito()");
                    dtOpsPendientes = DAOTarjetaCuenta.ObtieneOperacionesEnTransito(
                        this.dfInicioSYM.SelectedDate, this.dfFinSYM.SelectedDate,
                        Convert.ToInt64(this.hdnIdCuentaCC.Value), LH_ParabConsCliTarjetas);
                    unLog.Info("TERMINA ObtieneOperacionesEnTransito()");

                    HttpContext.Current.Session.Add("DtOpsPendTarj", dtOpsPendientes);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabConsCliTarjetas).Valor);

                if (dtOpsPendientes.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Movimientos Pendientes", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "ConsultaClientesTarjetas.ClicOpsPendientesDePaso()",
                                Text = "Aceptar"
                            },
                            No = new MessageBoxButtonConfig
                            {
                                Text = "Cancelar"
                            }
                        }).Show();
                }
                else
                {
                    int TotalRegistros = dtOpsPendientes.Rows.Count;

                    (this.StoreOpsPendientes.Proxy[0] as PageProxy).Total = TotalRegistros;

                    DataTable sortedDT = dtOpsPendientes.Clone();
                    DataTable dtToGrid = dtOpsPendientes.Clone();

                    //Se ordenan los datos según la elección del usuario
                    if (!String.IsNullOrEmpty(Columna))
                    {
                        System.Data.DataView dv = dtOpsPendientes.DefaultView;

                        dv.Sort = Columna + " " + Orden.ToString();
                        sortedDT = dv.ToTable();
                        sortedDT.AcceptChanges();
                    }

                    int RegistroFinal = (RegistroInicial + PagingTBOpsPendientes.PageSize) < TotalRegistros ?
                        (RegistroInicial + PagingTBOpsPendientes.PageSize) : TotalRegistros;

                    //Se recorta el número de registros a los definidos por página
                    for (int row = RegistroInicial; row < RegistroFinal; row++)
                    {
                        dtToGrid.ImportRow(String.IsNullOrEmpty(Columna) ? dtOpsPendientes.Rows[row] : sortedDT.Rows[row]);
                    }

                    dtToGrid.AcceptChanges();

                    StoreOpsPendientes.DataSource = dtToGrid;
                    StoreOpsPendientes.DataBind();

                    this.WdwDetalleMovs.Title += " - Tarjeta No. " + this.hdnMaskCard.Value.ToString() +
                        " - Cuenta No. " + this.hdnIdCuentaCDC.Value.ToString() +
                        " - Periodo del " + this.dfInicioSYM.SelectedDate.ToShortDateString() +
                        " al " + this.dfFinSYM.SelectedDate.ToShortDateString();
                    this.WdwDetalleMovs.Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos Pendientes", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Movimientos Pendientes", "Ocurrió un error al obtener los Movimientos Pendientes").Show();
            }

            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Método directo de paso creado para disparar el evento clic
        /// del botón oculto de descarga del reporte de detalle a Excel
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void ClicOpsPendientesDePaso()
        {
            btnDownOpsPendientes.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento clic al botón oculto DownloadDetalle, sólo para poder llamar
        /// a la exportación del reporte a Excel del detalle de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void DownloadOpsPendientes(object sender, DirectEventArgs e)
        {
            ExportDTOpsPendientesToExcel();
        }

        /// <summary>
        /// Exporta el reporte de detalle de movimientos, previamente consultado, a un archivo Excel
        /// cuando dicho reporte excedió el número máximo de registros en pantalla
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void ExportDTOpsPendientesToExcel()
        {
            try
            {
                string reportName = "OperacionesPendientes";
                DataTable _dtSaldosYMovs = HttpContext.Current.Session["DtOpsPendTarj"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtSaldosYMovs, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWSOpsPendientes(ws, ws.Column(1).CellsUsed().Count());

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabConsCliTarjetas);
                pCI.Error("Error al exportar el reporte a Excel");
                pCI.ErrorException(ex);
                X.Msg.Alert("Movimientos Pendientes", "Ocurrió un Error al Exportar el Reporte a Excel").Show();
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWSOpsPendientes(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(1).Hide();

                for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
                {
                    ws.Cell(rowsCounter, 2).SetDataType(XLCellValues.DateTime);
                    ws.Cell(rowsCounter, 4).SetDataType(XLCellValues.Number);
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "ConsultaClientesTarjetas")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar en la pestaña de Saldos y Movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnBuscarSaldosYMovs_Click(object sender, DirectEventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabConsCliTarjetas);
            
            try
            {
                logPCI.Info("INICIA ObtieneResumenMovimientos()");
                ResumenMovimientosCuenta elResumen = 
                    DAOTarjetaCuenta.ObtieneResumenMovimientos(this.dfInicioSYM.SelectedDate,
                    this.dfFinSYM.SelectedDate, Convert.ToInt64(this.hdnIdCuentaCDC.Value),
                    Convert.ToInt64(this.hdnIdCuentaLDC.Value), LH_ParabConsCliTarjetas);
                logPCI.Info("TERMINA ObtieneResumenMovimientos()");

                logPCI.Info("INICIA ObtieneResumenMovimientosAlCorte()");
                ResumenMovimientosCuenta resumen = DAOTarjetaCuenta.ObtieneResumenMovimientosAlCorte(
                    this.dfInicioSYM.SelectedDate, this.dfFinSYM.SelectedDate,
                    Convert.ToInt64(this.hdnIdCuentaCDC.Value), Convert.ToInt64(this.hdnIdCuentaLDC.Value),
                    Convert.ToInt64(this.hdnIdCuentaCC.Value),  LH_ParabConsCliTarjetas);
                logPCI.Info("TERMINA ObtieneResumenMovimientosAlCorte()");

                this.lblSaldoInicial.Text = String.Format("{0:C}", elResumen.SaldoInicial);
                this.lblCargos.Text = String.Format("(+) {0:C}", elResumen.Cargos);
                this.lblAbonos.Text = String.Format("(-) {0:C}", elResumen.Abonos);
                this.lblSaldoFinal.Text = String.Format("{0:C}", elResumen.SaldoFinal);

                this.FieldSetResMovs.Title = "Resumen de Movimientos - Del " + this.dfInicioSYM.SelectedDate.ToShortDateString() +
                " al " + this.dfFinSYM.SelectedDate.ToShortDateString();

                this.FieldSetResMovs.Collapsed = false;
                this.FieldSetResMovsCorte.Collapsed = false;
                this.btnMovimientos.Disabled = false;

                /// Aplica funcionalidad de consumo del WEB SERVICE PIEK para consultar TDC
                char[] charsToTrim = { '*', '"', ' ' };
                string Tarjeta = this.hdnTarjeta.Value.ToString();
                string TarjetaEnmasc = this.hdnMaskCard.Value.ToString();
                string wsLoginResp = null;
                
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                _headers.URL = Configuracion.Get(Guid.Parse(
                            ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                            "WsParab_URL", LH_ParabConsCliTarjetas).Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Usr", LH_ParabConsCliTarjetas).Valor;
                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Pwd", LH_ParabConsCliTarjetas).Valor;

                logPCI.Info("INICIA Parabilium.Login()");
                wsLoginResp = Parabilium.Login(_headers, _body, LH_ParabConsCliTarjetas);
                logPCI.Info("TERMINA Parabilium.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headers.Token = wsLoginResp;
                _headers.Credenciales = Cifrado.Base64Encode(_body.NombreUsuario + ":" + _body.Password);

                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.ConsultarTarjetaCreditoBody consultaTDC = new Parametros.ConsultarTarjetaCreditoBody();
                consultaTDC.IDSolicitud = "";
                consultaTDC.Tarjeta = Tarjeta;
                consultaTDC.MedioAcceso = "";
                consultaTDC.TipoMedioAcceso = "";

                Parametros.ConsultarTarjetaCreditoBody maskedBody = new Parametros.ConsultarTarjetaCreditoBody();
                maskedBody.IDSolicitud = "";
                maskedBody.Tarjeta = TarjetaEnmasc;
                maskedBody.MedioAcceso = "";
                maskedBody.TipoMedioAcceso = "";

                logPCI.Info("INICIA Parabilium.ConsultarTarjetaCredito()");
                RespuestasJSON.ConsultarTarjetaCredito wsConsultaTDCResp = Parabilium.ObtenerMovimientosTarjeta(_headers, consultaTDC, maskedBody, LH_ParabConsCliTarjetas);
                logPCI.Info("TERMINA Parabilium.ConsultarTarjetaCredito()");

                if (wsConsultaTDCResp == null || wsConsultaTDCResp.CodRespuesta != 0)
                {
                    this.lblFechaUltimoCorte.Text = "-";
                    this.lblLimiteCredito.Text = "-";
                    this.lblSaldo.Text = "-";
                    this.lblMovsPendientes.Text = "-";
                    this.lblCreditoDisponible.Text = "-";
                    this.lblSaldoDisponible.Text = "-";
                    this.lblPagoMinimo.Text = "-";
                    this.lblPagoNoIntereses.Text = "-";
                    this.lblFechaLimitePago.Text = "-";
                    this.lblNumPagosVencidos.Text = "-";
                    throw new CAppException(8006, wsConsultaTDCResp.DescRespuesta);
                }

                this.lblFechaUltimoCorte.Text = (string.IsNullOrEmpty(wsConsultaTDCResp.FechaUltimoCorte)) ? "-" : wsConsultaTDCResp.FechaUltimoCorte;
                this.lblLimiteCredito.Text = String.Format("{0:C}", wsConsultaTDCResp.LimiteDeCredito);
                this.lblSaldo.Text = String.Format("{0:C}", wsConsultaTDCResp.SaldoActual);
                this.lblMovsPendientes.Text = String.Format("{0:C}", wsConsultaTDCResp.MovtosEnTransito);
                this.lblCreditoDisponible.Text = String.Format("{0:C}", wsConsultaTDCResp.CreditoDisponible);
                this.lblSaldoDisponible.Text = String.Format("{0:C}", wsConsultaTDCResp.SaldoDisponible);
                this.lblPagoMinimo.Text = String.Format("{0:C}", wsConsultaTDCResp.PagoMinimoActual);
                this.lblPagoNoIntereses.Text = String.Format("{0:C}", wsConsultaTDCResp.PagoParaNoGenerarInteresesActual);
                this.lblFechaLimitePago.Text = (string.IsNullOrEmpty(wsConsultaTDCResp.FechaLimitePago)) ? "-" : wsConsultaTDCResp.FechaLimitePago;
                this.lblNumPagosVencidos.Text = wsConsultaTDCResp.PagosVencidos.ToString();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Notify("Saldos y Movimientos", "Ocurrió un error al obtener el Resumen de Movimientos").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Ver Detalle de Movimientos, dentro de
        /// la pestaña de Saldos y Movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnMovimientos_Click(object sender, DirectEventArgs e)
        {
            HttpContext.Current.Session.Add("DtSyMCtesTarj", null);
            this.StoreSaldosYMovs.RemoveAll();

            HttpContext.Current.Session.Add("DtOpsPendTarj", null);
            this.StoreOpsPendientes.RemoveAll();

            Thread.Sleep(100);

            btnDetMovsHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de captura de llamada,
        /// invocando la inserción del registro correspondiente en la bitácora de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarLlamada_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                RegistroLlamada unRegistro = new RegistroLlamada();
                unRegistro.ID_MotivoLlamada = Convert.ToInt32(this.cBoxMotivoLlamada.SelectedItem.Value);
                unRegistro.ID_MedioAcceso = Convert.ToInt32(this.hdnIdMA.Value);
                unRegistro.ID_Colectiva = Convert.ToInt32(this.hdnIdColectiva.Value);
                unRegistro.UsuarioLlama = this.txtUsuarioLlama.Text;
                unRegistro.Comentarios = this.txtComentarios.Text;

                unLog.Info("INICIA RegistrarLlamada()");
                LNClientes.RegistrarLlamada(unRegistro, this.Usuario, LH_ParabConsCliTarjetas);
                unLog.Info("TERMINA RegistrarLlamada()");

                X.Msg.Notify("", "Llamada Registrada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.FormPanelLlamadas.Reset();

                LimpiaGridHistorial();
                this.btnHistLlamadas.FireEvent("click");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Captura de Llamada", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Captura de Llamada", "Ocurrió un error en el registro de la llamada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana emergente de Bloqueo de Tarjeta,
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBloquea_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabConsCliTarjetas);

            try
            {
                string wsActResp = null;
                string laTarjeta = this.hdnSelectedCard.Value.ToString();

                log.Info("INICIA LoginWebServiceParabilium()");
                Parametros.Headers bloqHeaders = LNCuentas.LoginWebServiceParabilium(LH_ParabConsCliTarjetas);
                log.Info("TERMINA LoginWebServiceParabilium()");

                log.Info("INICIA BloqueaTarjetaWebServiceParabilium()");
                wsActResp = LNCuentas.BloqueaTarjetaWebServiceParabilium(bloqHeaders, laTarjeta,
                    MaskSensitiveData.cardNumber(laTarjeta), this.cBoxMotivo.SelectedItem.Value, LH_ParabConsCliTarjetas);
                log.Info("TERMINA BloqueaTarjetaWebServiceParabilium()");

                log.Info("INICIA RegistraEnBitacora()");
                LNInfoOnBoarding.RegistraEnBitacora("pantalla_AdminCuentas_BloquearTarjeta", "MedioAcceso",
                    "ID_EstatusMA", this.hdnSelectedIdMA.Value.ToString(), "8",
                    "Bloqueo de Tarjeta: " + MaskSensitiveData.cardNumber(laTarjeta),
                    this.Usuario, LH_ParabConsCliTarjetas);
                log.Info("TERMINA RegistraEnBitacora()");

                X.Msg.Notify("", "Tarjeta Bloqueada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridTarjetas();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Bloqueo", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Bloqueo", "Ocurrió un error al bloquear la tarjeta").Show();
            }
            finally
            {
                this.WdwConfirmaBloqueo.Hide();
            }
        }
    }
}