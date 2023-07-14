using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using DALCortador.Entidades;
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
using System.Threading;
using System.Web;
using Utilerias;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace Administracion
{
    /// <summary>
    /// Realiza y controla la carga de la página Administración de Cuentas
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class AdminCuentasParabilia : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Administrar Cuentas Tarjeta de Crédito
        private LogHeader LH_ParabAdminCtasTDC = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminCtasTDC.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminCtasTDC.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminCtasTDC.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminCtasTDC.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdminCuentasParabilia Page_Load()");

                if (!IsPostBack)
                {
                    EstableceRolesAuditables();

                    EstableceCatalogos();

                    this.dfInicioSYM.MaxDate = DateTime.Today;
                    this.dfInicioSYM.MinDate = DateTime.Today.AddMonths(-3);

                    this.dfFinSYM.MaxDate = DateTime.Today;

                    PagingTBSaldosYMovs.PageSize =
                        Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_RegsPorPagina", LH_ParabAdminCtasTDC).Valor);

                    HttpContext.Current.Session.Add("DtSaldosYMovs", null);
                    HttpContext.Current.Session.Add("DtOpsPendientes", null);
                }

                log.Info("TERMINA AdminCuentasParabilia Page_Load()");
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
        /// Establece los valores de catálogos en los controles correspondientes
        /// </summary>
        protected void EstableceCatalogos()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasTDC);

            unLog.Info("INICIA ListaTiposParametrosMA()");
            this.StoreTipoParametroMA.DataSource =
                DAOProducto.ListaTiposParametrosMA(true, this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminCtasTDC);
            unLog.Info("TERMINA ListaTiposParametrosMA()");
            this.StoreTipoParametroMA.DataBind();

            unLog.Info("INICIA ObtienePertenenciasManuales()");
            this.StorePertenenciasManuales.DataSource =
               DAOProducto.ObtienePertenenciasManuales(this.Usuario,
               Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
               LH_ParabAdminCtasTDC);
            unLog.Info("TERMINA ObtienePertenenciasManuales()");
            this.StorePertenenciasManuales.DataBind();

            unLog.Info("INICIA ObtieneValorIVA()");
            this.hdnValorIVA.Value = DAOProducto.ObtieneValorIVA(LH_ParabAdminCtasTDC);
            unLog.Info("TERMINA ObtieneValorIVA()");

            unLog.Info("INICIA ListaCatalogoValoresParametroExtra()_RegimenFiscal");
            this.StoreRegFiscal.DataSource = DAOColectivas.ListaCatalogoValoresParametroExtra("@RegimenFiscal", LH_ParabAdminCtasTDC);
            unLog.Info("TERMINA ListaCatalogoValoresParametroExtra()_RegimenFiscal");
            this.StoreRegFiscal.DataBind();

            unLog.Info("INICIA ListaCatalogoValoresParametroExtra()_UsoCFDI");
            this.StoreUsoCFDI.DataSource = DAOColectivas.ListaCatalogoValoresParametroExtra("@UsoCFDI", LH_ParabAdminCtasTDC);
            unLog.Info("TERMINA ListaCatalogoValoresParametroExtra()_UsoCFDI");
            this.StoreUsoCFDI.DataBind();
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
            LogPCI logPCI = new LogPCI(LH_ParabAdminCtasTDC);

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
                        LH_ParabAdminCtasTDC);
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

                DataTable dt = Tarjetas.EnmascaraTablaConTarjetas(dsResultados.Tables[0], "Tarjeta", "Enmascara", LH_ParabAdminCtasTDC);

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

            this.StoreCatalogoPMA.RemoveAll();
            this.cBoxCatalogoPMA.Reset();
            this.cBoxCatalogoPMA.Hidden = true;
        }

        /// <summary>
        /// Restablece los controles de las ventanas emergentes de la página
        /// </summary>
        protected void LimpiaFormPanelLimiteCredito(bool limpiezaTotal)
        {
            this.txtImporteLDC.Reset();
            this.txtReferencia.Reset();
            this.txtObservaciones.Reset();
            this.txtObservaciones.ReadOnly = false;

            if (limpiezaTotal)
            {
                this.FormPanelLimiteCredito.Reset();
                this.txtLDCActual.Reset();
                this.txtLDCPendiente.Reset();
                this.hdnLDCActual.Reset();
                this.hdnLDCPendiente.Reset();
            }
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

            this.FormPanelTitular.Reset();

            StoreTarjetas.RemoveAll();

            LimpiaFormPanelLimiteCredito(true);

            this.FormPanelParams.Reset();
            this.cBoxTipoParametroMA.Reset();
            this.cBoxParametros.Reset();
            this.StoreParametros.RemoveAll();            
            this.StoreValoresParametros.RemoveAll();
            this.btnAddParametros.Disabled = true;

            this.FormPanelMovsManuales.Reset();
            this.cBoxPertenencias.Reset();
            this.txtImporte.Reset();
            this.txtRefAjustesManuales.Reset();
            this.txtObsAjustesManuales.Reset();

            this.FormPanelSaldosYMovs.Reset();
            this.dfInicioSYM.Reset();
            this.dfFinSYM.Reset();
            this.FormPanelResumenMovs.Reset();
            this.FieldSetResMovs.Collapse();
            this.FormPanelResumenMovsCorte.Reset();
            this.FieldSetResMovsCorte.Collapse();
            this.btnMovimientos.Disabled = true;

            this.EastPanel.Disabled = true;

            this.FormPanelTitular.Show();
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
                string Tarjeta = string.Empty, numTarjeta = string.Empty;
                int IdCuentaCDC = 0;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "CLDC": IdCuentaCLDC = int.Parse(column.Value); break;
                        case "CDC": IdCuentaCDC = int.Parse(column.Value); break;
                        case "CCLC": this.hdnIdCuentaCC.Value = column.Value; break;
                        case "IdTarjeta": IdMA = int.Parse(column.Value); break;
                        case "IdColectivaCuentahabiente": IdColectiva = int.Parse(column.Value); break;
                        case "Tarjeta": Tarjeta = column.Value; break;
                        case "NumTarjeta": numTarjeta = column.Value; break;
                        case "IdCadenaComercial": IdCadena = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                limpiaBusquedaPrevia(false);

                this.hdnIdCuentaLDC.Value = IdCuentaCLDC;
                this.hdnIdMA.Value = IdMA;
                this.hdnMaskCard.Value = Tarjeta;
                this.hdnTarjeta.Value = numTarjeta;
                this.hdnIdColectiva.Value = IdColectiva;
                this.hdnIdCadena.Value = IdCadena;
                this.hdnIdCuentaCDC.Value = IdCuentaCDC;

                LlenaFieldSetDatosTitular(IdCuentaCLDC, IdMA, IdColectiva);
                LlenaGridTarjetas(IdMA);
                LlenaFormPanelLimiteCredito(IdCuentaCLDC, IdMA);

                this.EastPanel.Title = "Tarjeta " + Tarjeta;
                this.EastPanel.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminCtasTDC);
                unLog.ErrorException(ex);
                X.Msg.Alert("Cuenta", "Ocurrió un error al obtener la información de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Llena el FieldSet con los datos personales de la cuenta seleccionada
        /// </summary>
        /// <param name="idCuenta">ID de la cuenta</param>
        /// <param name="idTarjeta">ID de la tarjeta</param>
        /// <param name="idColectiva">ID de la colectiva</param>
        protected void LlenaFieldSetDatosTitular(int idCuenta, int idTarjeta, int idColectiva)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                FormPanelTitular.Reset();

                string nombreEmbozo = string.Empty;

                pCI.Info("INICIA ObtieneDatosPersonalesCuenta()");
                DatosPersonalesCuenta losDatos = DAOTarjetaCuenta.ObtieneDatosPersonalesCuenta(
                   idTarjeta, idColectiva, ref nombreEmbozo, this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_ParabAdminCtasTDC);
                pCI.Info("TERMINA ObtieneDatosPersonalesCuenta()");
                
                if (losDatos.Enmascara == "TARJ")
                {
                    this.txtTarjetaTitular.Text = this.hdnMaskCard.Value.ToString();
                }
                else
                {
                    this.txtTarjetaTitular.Text = this.hdnTarjeta.Value.ToString();
                }

                this.txtNombreClienteTitular.Text = losDatos.Nombre;
                this.txtApPaternoTitular.Text = losDatos.ApellidoPaterno;
                this.txtApMaternoTitular.Text = losDatos.ApellidoMaterno;
                this.txtRFCTitular.Text = losDatos.RFC;
                this.dfFechaNac.Value = losDatos.FechaNacimiento;
                this.txtNombreEmbozo.Text = nombreEmbozo;
                this.txtCURP.Text = losDatos.CURP;

                this.txtID_Direccion.Text = losDatos.IdDireccion.ToString();
                this.txtCalle.Text = losDatos.Calle;
                this.txtNumExterior.Text = losDatos.NumExterior;
                this.txtNumInterior.Text = losDatos.NumInterior;
                this.txtEntreCalles.Text = losDatos.EntreCalles;
                this.txtReferencias.Text = losDatos.Referencias;
                this.txtCPCliente.Text = losDatos.CodigoPostal;
                this.txtIDColonia.Text = losDatos.IdColonia.ToString();
                this.cBoxColonia.SelectedItem.Text = losDatos.Colonia;
                this.txtClaveMunicipio.Text = losDatos.ClaveMunicipio;
                this.txtMunicipioTitular.Text = losDatos.Municipio;
                this.txtClaveEstado.Text = losDatos.ClaveEstado;
                this.txtEstadoTitular.Text = losDatos.Estado;

                this.nfTelParticular.Text = losDatos.NumTelParticular;
                if (String.IsNullOrEmpty(losDatos.NumTelParticular))
                {
                    this.nfTelParticular.Value = "";
                }
                
                this.nfTelCelular.Text = losDatos.NumTelCelular;
                if (String.IsNullOrEmpty(losDatos.NumTelCelular))
                {
                    this.nfTelCelular.Value = "";
                }

                this.nfTelTrabajo.Text = losDatos.NumTelTrabajo;
                if (String.IsNullOrEmpty(losDatos.NumTelTrabajo))
                {
                    this.nfTelTrabajo.Value = "";
                }

                this.txtCorreo.Text = losDatos.Email;

                this.txtIDDirFiscal.Text = losDatos.IdDireccionFiscal.ToString();
                this.txtCalleFiscal.Text = losDatos.CalleFiscal;
                this.txtNumExtFiscal.Text = losDatos.NumExteriorFiscal;
                this.txtNumIntFiscal.Text = losDatos.NumInteriorFiscal;
                this.txtCPFiscal.Text = losDatos.CodigoPostalFiscal;
                this.txtIDColFiscal.Text = losDatos.IdColoniaFiscal.ToString();
                this.cBoxColFiscal.SelectedItem.Text = losDatos.ColoniaFiscal;
                this.txtCveMunFiscal.Text = losDatos.ClaveMunicipioFiscal;
                this.txtDelMunFiscal.Text = losDatos.MunicipioFiscal;
                this.txtCveEdoFiscal.Text = losDatos.ClaveEstadoFiscal;
                this.txtEstadoFiscal.Text = losDatos.EstadoFiscal;
                this.cBoxRegimenFiscal.SelectedItem.Value = losDatos.IdRegimenFiscal == 0 ? "" : losDatos.IdRegimenFiscal.ToString();
                this.hdnCveRegimenFiscal.Value = losDatos.ClaveRegimenFiscal;
                this.cBoxUsoCFDI.SelectedItem.Value = losDatos.IdUsoCFDI == 0 ? "" : losDatos.IdUsoCFDI.ToString();
                this.hdnCveUsoCFDI.Value = losDatos.ClaveUsoCFDI;

                if (losDatos.IdColonia > 0)
                    LlenaComboColonias();

                if (losDatos.IdColoniaFiscal > 0)
                    LlenaColoniasFiscales();
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
        /// Llena el combo de colonias y los campos de municipio y estado, con información de base de datos
        /// </summary>
        [DirectMethod(Namespace = "CuentasBancarias")]
        public void LlenaComboColonias()
        {
            if (!String.IsNullOrEmpty(this.txtCPCliente.Text) &&
                this.txtCPCliente.Text.Length >= 5)
            {
                LogPCI logPCI = new LogPCI(LH_ParabAdminCtasTDC);

                try
                {
                    logPCI.Info("INICIA ListaDatosPorCodigoPostal()");
                    DataSet dsColonias = DAOTarjetaCuenta.ListaDatosPorCodigoPostal(
                                this.txtCPCliente.Text,
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabAdminCtasTDC);
                    logPCI.Info("TERMINA ListaDatosPorCodigoPostal()");

                    if (dsColonias.Tables[0].Rows.Count < 1)
                    {
                        X.Msg.Alert("Código Postal", "No existen coincidencias con el Código Postal ingresado").Show();
                    }
                    else
                    {

                        this.StoreColonias.DataSource = dsColonias;
                        this.StoreColonias.DataBind();

                        this.txtClaveMunicipio.Text = dsColonias.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                        this.txtMunicipioTitular.Text = dsColonias.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                        this.txtClaveEstado.Text = dsColonias.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                        this.txtEstadoTitular.Text = dsColonias.Tables[0].Rows[0]["Estado"].ToString().Trim();
                    }
                }

                catch (CAppException err)
                {
                    X.Msg.Alert("Datos del Titular", err.Mensaje()).Show();
                }

                catch (Exception ex)
                {
                    logPCI.ErrorException(ex);
                    X.Msg.Alert("Datos del Titular", "Ocurrió un error al obtener los datos del Código Postal");
                }
            }
            else
            {
                this.txtClaveMunicipio.Clear();
                this.txtMunicipioTitular.Clear();
                this.txtClaveEstado.Clear();
                this.txtEstadoTitular.Clear();
            }
        }

        /// <summary>
        /// Llena el combo de colonias y los campos de municipio y estado de la sección de datos fiscales,
        /// con información de base de datos
        /// </summary>
        [DirectMethod(Namespace = "CuentasBancarias")]
        public void LlenaColoniasFiscales()
        {
            if (!String.IsNullOrEmpty(this.txtCPFiscal.Text) &&
                this.txtCPFiscal.Text.Length >= 5)
            {
                LogPCI logPCI = new LogPCI(LH_ParabAdminCtasTDC);

                try
                {
                    logPCI.Info("INICIA ListaDatosPorCodigoPostal()");
                    DataSet dsColFiscales = DAOTarjetaCuenta.ListaDatosPorCodigoPostal(
                                this.txtCPFiscal.Text,
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabAdminCtasTDC);
                    logPCI.Info("TERMINA ListaDatosPorCodigoPostal()");

                    if (dsColFiscales.Tables[0].Rows.Count < 1)
                    {
                        X.Msg.Alert("Código Postal", "No existen coincidencias con el Código Postal ingresado").Show();
                    }
                    else
                    {
                        this.StoreColFiscal.DataSource = dsColFiscales;
                        this.StoreColFiscal.DataBind();

                        this.txtCveMunFiscal.Text = dsColFiscales.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                        this.txtDelMunFiscal.Text = dsColFiscales.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                        this.txtCveEdoFiscal.Text = dsColFiscales.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                        this.txtEstadoFiscal.Text = dsColFiscales.Tables[0].Rows[0]["Estado"].ToString().Trim();
                    }
                }

                catch (CAppException err)
                {
                    X.Msg.Alert("Datos Fiscales", err.Mensaje()).Show();
                }

                catch (Exception ex)
                {
                    logPCI.ErrorException(ex);
                    X.Msg.Alert("Datos Fiscales", "Ocurrió un error al obtener los datos del Código Postal");
                }
            }
            else
            {
                this.txtCveMunFiscal.Clear();
                this.txtDelMunFiscal.Clear();
                this.txtCveEdoFiscal.Clear();
                this.txtEstadoFiscal.Clear();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de Datos, invocando la actualización
        /// de los datos del cliente en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaDatos_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();
                Tarjeta unaTarjeta = new Tarjeta();

                datosPersonales.ID_Cuenta = Convert.ToInt32(this.hdnIdCuentaLDC.Value);
                datosPersonales.ID_Colectiva = Convert.ToInt32(this.hdnIdColectiva.Value);

                datosPersonales.Nombre = this.txtNombreClienteTitular.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaternoTitular.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaternoTitular.Text;
                datosPersonales.RFC = this.txtRFCTitular.Text;
                datosPersonales.FechaNacimiento = this.dfFechaNac.SelectedDate;
                datosPersonales.CURP = this.txtCURP.Text;

                datosPersonales.IdDireccion = int.Parse(this.txtID_Direccion.Text);
                datosPersonales.Calle = this.txtCalle.Text;
                datosPersonales.NumExterior = this.txtNumExterior.Text;
                datosPersonales.NumInterior = this.txtNumInterior.Text;
                datosPersonales.EntreCalles = this.txtEntreCalles.Text;
                datosPersonales.Referencias = this.txtReferencias.Text;
                datosPersonales.CodigoPostal = this.txtCPCliente.Text;
                if (this.cBoxColonia.SelectedItem.Value != null)
                {
                    datosPersonales.IdColonia = this.cBoxColonia.SelectedItem.Value == this.cBoxColonia.SelectedItem.Text ? int.Parse(this.txtIDColonia.Text) : int.Parse(this.cBoxColonia.SelectedItem.Value);
                }
                datosPersonales.Colonia = this.cBoxColonia.SelectedItem.Text;
                datosPersonales.ClaveMunicipio = this.txtClaveMunicipio.Text;
                datosPersonales.ClaveEstado = this.txtClaveEstado.Text;

                datosPersonales.NumTelParticular = this.nfTelParticular.Text;
                datosPersonales.NumTelCelular = this.nfTelCelular.Text;
                datosPersonales.NumTelTrabajo = this.nfTelTrabajo.Text;
                datosPersonales.Email = this.txtCorreo.Text;

                datosPersonales.IdDireccionFiscal = int.Parse(this.txtIDDirFiscal.Text);
                datosPersonales.CalleFiscal = this.txtCalleFiscal.Text;
                datosPersonales.NumExteriorFiscal = this.txtNumExtFiscal.Text;
                datosPersonales.NumInteriorFiscal = this.txtNumIntFiscal.Text;
                datosPersonales.CodigoPostalFiscal = this.txtCPFiscal.Text;
                if (this.cBoxColFiscal.SelectedItem.Value != null)
                {
                    datosPersonales.IdColoniaFiscal = this.cBoxColFiscal.SelectedItem.Value == this.cBoxColFiscal.SelectedItem.Text ? int.Parse(this.txtIDColFiscal.Text) : int.Parse(this.cBoxColFiscal.SelectedItem.Value);
                }
                datosPersonales.ColoniaFiscal = this.cBoxColFiscal.SelectedItem.Text;
                datosPersonales.ClaveMunicipioFiscal = this.txtCveMunFiscal.Text;
                datosPersonales.ClaveEstadoFiscal = this.txtCveEdoFiscal.Text;
                datosPersonales.IdRegimenFiscal = int.Parse(this.cBoxRegimenFiscal.SelectedItem.Value);
                datosPersonales.ClaveRegimenFiscal = this.hdnCveRegimenFiscal.Value.ToString();
                datosPersonales.IdUsoCFDI = int.Parse(this.cBoxUsoCFDI.SelectedItem.Value);
                datosPersonales.ClaveUsoCFDI = this.hdnCveUsoCFDI.Value.ToString();

                unaTarjeta.ID_MA = Convert.ToInt32(this.hdnIdMA.Value);
                unaTarjeta.NombreEmbozo = this.txtNombreEmbozo.Text;

                log.Info("INICIA ActualizaDatosTitularCuenta()");
                LNCuentas.ActualizaDatosTitularCuenta(datosPersonales, unaTarjeta, this.Usuario, LH_ParabAdminCtasTDC);
                log.Info("TERMINA ActualizaDatosTitularCuenta()");

                X.Msg.Notify("", "Datos Personales del Cuentahabiente actualizados <br /> <br /> <b> E X I T O S A M E N T E </b> <br /> <br />").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos Personales", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Datos Personales", "Ocurrió un error al modificar la información del Cuentahabiente").Show();
            }
        }

        /// <summary>
        /// Llena el GridPanel Tarjetas con los datos de la familia de la tarjeta seleccionada
        /// </summary>
        /// <param name="IdTarjeta">Identificador de tarjeta</param>
        protected void LlenaGridTarjetas(int IdTarjeta)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                this.StoreTarjetas.RemoveAll();

                pCI.Info("INICIA ObtieneFamiliaTarjetas()");
                DataSet dsFamTarjetas =
                     DAOTarjetaCuenta.ObtieneFamiliaTarjetas(IdTarjeta, this.Usuario,
                         Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminCtasTDC);
                pCI.Info("TERMINA ObtieneFamiliaTarjetas()");

                DataTable dtEnmascara = Tarjetas.EnmascaraTablaConTarjetas(dsFamTarjetas.Tables[0], "Tarjeta", "Enmascara", LH_ParabAdminCtasTDC);

                this.StoreTarjetas.DataSource = dtEnmascara;
                this.StoreTarjetas.DataBind();
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
            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);

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
                        Parametros.Headers _headers = LNCuentas.LoginWebServiceParabilium(LH_ParabAdminCtasTDC);
                        log.Info("TERMINA LoginWebServiceParabilium()");

                        log.Info("INICIA ActivaTarjetaWebServiceParabilium()");
                        wsActResp = LNCuentas.ActivaTarjetaWebServiceParabilium(_headers, Tarjeta,
                            MaskSensitiveData.cardNumber(Tarjeta), LH_ParabAdminCtasTDC);
                        log.Info("TERMINA ActivaTarjetaWebServiceParabilium()");

                        log.Info("INICIA RegistraEnBitacora()");
                        LNInfoOnBoarding.RegistraEnBitacora("pantalla_AdminCuentas_ActivarTarjeta", "MedioAcceso", 
                            "ID_EstatusMA", idMA, "1", "Activación Tarjeta: " + MaskSensitiveData.cardNumber(Tarjeta),
                            this.Usuario, LH_ParabAdminCtasTDC);
                        log.Info("TERMINA RegistraEnBitacora()");

                        X.Msg.Notify("", "Tarjeta Activada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridTarjetas(Convert.ToInt32(this.hdnIdMA.Value));
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
        /// Llena el FormPanel Tarjetas con los datos de la cuenta seleccionada
        /// </summary>
        /// <param name="idCuenta">ID de la cuenta</param>
        /// <param name="idTarjeta">ID de la tarjeta</param>
        protected void LlenaFormPanelLimiteCredito(int idCuenta, int idTarjeta)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                logPCI.Info("INICIA ObtieneLimiteCreditoCuenta()");
                DataSet dsLimites =
                    DAOTarjetaCuenta.ObtieneLimiteCreditoCuenta(idTarjeta, idCuenta, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminCtasTDC);
                logPCI.Info("TERMINA ObtieneLimiteCreditoCuenta()");

                /////////CAMPO AUTORIZABLE LIMITE DE CREDITO
                this.hdnLDCActual.Value = String.Format("{0:F2}", float.Parse(
                    dsLimites.Tables[0].Rows[0]["LimiteCreditoActual"].ToString().Trim()));

                string ldcPendiente = dsLimites.Tables[0].Rows[0]["LimiteCreditoPendiente"].ToString().Trim();
                this.hdnLDCPendiente.Value = String.Format("{0:F2}", 
                    float.Parse(String.IsNullOrEmpty(ldcPendiente) ? "0" : ldcPendiente));
                this.txtObservaciones.Reset();
                this.txtObservaciones.Text = dsLimites.Tables[0].Rows[0]["Observaciones"].ToString().Trim();

                ControlaCambiosLDC(this.hdnLDCActual.Value.ToString(), this.hdnLDCPendiente.Value.ToString());
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Límite de Crédito", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Límite de Crédito", "Ocurrió un error al establecer el Límite de Crédito").Show();
            }
        }

        /// <summary>
        /// Controla los cambios en el valor al límite de crédito de la cuenta, según el rol en sesión y si
        /// tiene algún cambio pendiente de autorización, para mostrarlo en el control correspondiente
        /// </summary>
        /// <param name="LimiteCredito">Límite de crédito actual</param>
        /// <param name="LimiteCreditoPendiente">Límite de crédito pendiente de autorización</param>
        protected void ControlaCambiosLDC(String LimiteCredito, String LimiteCreditoPendiente)
        {
            try
            {
                Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                this.txtLDCActual.Text = String.Format("{0:C2}", decimal.Parse(LimiteCredito));

                if (decimal.Parse(LimiteCreditoPendiente) == 0)
                {
                    this.txtLDCPendiente.Hide();
                }
                else
                {
                    this.txtLDCPendiente.Show();
                    this.txtLDCPendiente.Text = String.Format("{0:C2}", decimal.Parse(LimiteCreditoPendiente));
                }

                //Se realizan las validaciones por rol
                if (!esEjecutor)
                {
                    this.txtImporteLDC.Hide();
                    this.txtReferencia.Hide();
                    this.txtObservaciones.Hide();

                    this.btnModificarLDC.Hide();
                }
                if (esAutorizador)
                {
                    this.txtObservaciones.Show();
                    this.txtObservaciones.ReadOnly = true;

                    this.btnAutorizarCambios.Hidden =
                        (String.IsNullOrEmpty(LimiteCreditoPendiente) || float.Parse(LimiteCreditoPendiente) == 0) ? true :
                        (float.Parse(LimiteCreditoPendiente) > 0) && (LimiteCredito != LimiteCreditoPendiente) ? false :
                        true;

                    if (esEjecutor)
                    {
                        if (this.txtObservaciones.Hidden == true)
                        {
                            this.txtObservaciones.Text = "-o-";
                        }
                        this.txtObservaciones.ReadOnly = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);
                pCI.Error("ControlaCambiosLDC()");
                pCI.ErrorException(ex);
                throw ex;
            }
        }

        // <summary>
        /// Controla el evento Click al botón Guardar de la pestaña Límite de Crédito,
        /// invocando al registro del cambio de valor en base de datos
        /// </summary>
        [DirectMethod(Namespace = "AdminCtasParab")]
        public void ConfirmaModificacionLDC()
        {
            try
            {
                decimal Importe = 0;
                string sImp = this.txtImporteLDC.Text.Replace("$", "").Replace(",", "");

                if (!Decimal.TryParse(sImp, out Importe))
                {
                    X.Msg.Alert("Límite de Crédito", "El Importe [<b>" + this.txtImporteLDC.Text + "</b>] no es válido").Show();
                    return;
                }

                if (Importe == 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Límite de Crédito",
                        Message = "El Importe debe ser mayor a $0.00",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO
                    });
                }
                else
                {
                    string nuevoLDC = String.Format("{0:C}", Importe);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Confirmación",
                        Message = "La nueva Línea de Crédito será de <b>" + nuevoLDC + 
                            ".</b></br></br>¿Deseas solicitar la modificación?",
                        Buttons = MessageBox.Button.YESNO,
                        Icon = MessageBox.Icon.NONE,
                        Fn = new JFunction { Fn = "fnModificarLDC" }
                    });
                }
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);
                log.ErrorException(ex);
                X.Msg.Alert("Límite de Crédito", "Error al solicitar la confirmación en el Límite de Crédito").Show();
            }
        }

        // <summary>
        /// Controla el evento Click al botón SI del mensaje de confirmación al ajuste de Límite de Crédito,
        /// validando condiciones para el cambio
        /// </summary>
        [DirectMethod(Namespace = "AdminCtasParab")]
        public void ModificarLDC()
        {
            string msj = "";
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasTDC);
            Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
            Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

            try
            {
                decimal LDC_Antes = Convert.ToDecimal(
                    String.IsNullOrEmpty(this.hdnLDCActual.Value.ToString()) ? "0" : this.hdnLDCActual.Value.ToString());

                decimal Importe = Convert.ToDecimal(this.txtImporteLDC.Text.Replace("$", "").Replace(",", ""));

                //Si el ajuste solicitado restablece el LDC original, sólo se elimina el registro
                //en EjecutorAutorizador
                if (LDC_Antes == Importe)
                {
                    unLog.Info("INICIA CancelaCambioLimiteCredito()");
                    LNCuentas.CancelaCambioLimiteCredito(
                        Convert.ToInt64(this.hdnIdCuentaLDC.Value), this.Usuario, LH_ParabAdminCtasTDC);
                    unLog.Info("TERMINA CancelaCambioLimiteCredito()");

                    msj = "Se canceló la modificación al Límite de Crédito";
                }
                else
                {
                    //Si el usuario es Ejecutor y Autorizador, realiza el ajuste directamente
                    if (esEjecutor && esAutorizador)
                    {
                        this.hdnLDCPendiente.Value = String.Format("{0:F2}", Importe);
                        AutorizaAjusteLimiteCredito();
                        msj = "Límite de Crédito Modificado";
                    }
                    else
                    {
                        unLog.Info("INICIA SolicitaCambioLimiteCredito()");
                        LNCuentas.SolicitaCambioLimiteCredito(
                            Convert.ToInt32(this.hdnIdCuentaLDC.Value), Convert.ToInt32(this.hdnIdMA.Value),
                            Importe.ToString(), Path.GetFileName(Request.Url.AbsolutePath),
                            this.txtObservaciones.Text, this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                            LH_ParabAdminCtasTDC);
                        unLog.Info("TERMINA SolicitaCambioLimiteCredito()");

                        msj = "Límite de Crédito Modificado";
                    }
                }

                LlenaFormPanelLimiteCredito(Convert.ToInt32(this.hdnIdCuentaLDC.Value), Convert.ToInt32(this.hdnIdMA.Value));

                X.Msg.Notify("", msj + "<br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LimpiaFormPanelLimiteCredito(false);
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Límite de Crédito", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Límite de Crédito", "Ocurrió un error al establecer el ajuste al Límite de Crédito").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        // <summary>
        /// Controla el evento Click al botón NO del mensaje de autorización del ajuste de Límite de Crédito
        /// </summary>
        [DirectMethod(Namespace = "AdminCtasParab")]
        public void RechazaModificarLDC()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasTDC);
            try
            {
                unLog.Info("INICIA CancelaCambioLimiteCredito()");
                LNCuentas.CancelaCambioLimiteCredito(
                        Convert.ToInt64(this.hdnIdCuentaLDC.Value), this.Usuario, LH_ParabAdminCtasTDC);
                unLog.Info("TERMINA CancelaCambioLimiteCredito()");

                LlenaFormPanelLimiteCredito(Convert.ToInt32(this.hdnIdCuentaLDC.Value), Convert.ToInt32(this.hdnIdMA.Value));

                X.Msg.Notify("", "Se rechazó la modificación al Límite de Crédito<br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Límite de Crédito", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Límite de Crédito", "Ocurrió un error al rechazar la modificación al Límite de Crédito").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        // <summary>
        /// Controla el evento Click al botón SI del mensaje de autorización del ajuste de Límite de Crédito
        /// </summary>
        [DirectMethod(Namespace = "AdminCtasParab")]
        public void AutorizaModificarLDC()
        {
            try
            {
                AutorizaAjusteLimiteCredito();

                X.Msg.Notify("Límite de Crédito", "Autorización de ajuste a Límite de Crédito<br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();

                this.btnAutorizarCambios.Hidden = true;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Límite de Crédito", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);
                pCI.ErrorException(ex);
                X.Msg.Notify("Límite de Crédito", "Ocurrió un error al establecer el ajuste al Límite de Crédito").Show();
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        /// <summary>
        /// Ejecuta las llamadas a los métodos para el ajuste al límite de crédito y llena nuevamente
        /// el panel de tarjetas con la información ajustada
        /// </summary>
        protected void AutorizaAjusteLimiteCredito()
        {
            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                decimal ldcPendiente = Convert.ToDecimal(this.hdnLDCPendiente.Value);
                decimal ldcActual = Convert.ToDecimal(this.hdnLDCActual.Value);

                EventoManual evento = new EventoManual();

                evento.ClaveEvento = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "ClaveEvAjLimCred", LH_ParabAdminCtasTDC).Valor.Trim();
                evento.MedioAcceso = this.hdnTarjeta.Value.ToString();
                evento.Importe = ldcPendiente.ToString();
                evento.Referencia = String.IsNullOrEmpty(this.txtReferencia.Text) ? 0 :
                    Convert.ToInt64(this.txtReferencia.Text);
                evento.Observaciones = this.txtObservaciones.Text;
                evento.SaldoCuentaCLDC = ldcActual.ToString();

                log.Info("INICIA AjustaSaldoLimiteCredito()");
                LNCuentas.AjustaSaldoLimiteCredito(evento, Convert.ToInt64(this.hdnIdCuentaLDC.Value),
                    ldcPendiente.ToString(), this.Usuario, LH_ParabAdminCtasTDC);
                log.Info("TERMINA AjustaSaldoLimiteCredito()");

                LlenaFormPanelLimiteCredito(Convert.ToInt32(this.hdnIdCuentaLDC.Value), Convert.ToInt32(this.hdnIdMA.Value));
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
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
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                int IdCuenta = Convert.ToInt32(this.hdnIdCuentaLDC.Value);

                pCI.Info("INICIA ObtienePMAPorTipoSinAsignarCuenta()");
                StoreParametros.DataSource = DAOTarjetaCuenta.ObtienePMAPorTipoSinAsignarCuenta(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdCuenta, LH_ParabAdminCtasTDC);
                pCI.Info("TERMINA ObtienePMAPorTipoSinAsignarCuenta()");
                StoreParametros.DataBind();

                pCI.Info("INICIA ObtieneParametrosMACuenta()");
                StoreValoresParametros.DataSource = DAOTarjetaCuenta.ObtieneParametrosMACuenta(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdCuenta, LH_ParabAdminCtasTDC);
                pCI.Info("TERMINA ObtieneParametrosMACuenta()");
                StoreValoresParametros.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Parámetros de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametros_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                int IdTarjeta = Convert.ToInt32(this.hdnIdMA.Value);
                Int64 IdPlantilla = Convert.ToInt64(this.hdnIdPlantilla.Value);

                log.Info("INICIA AgregaParametroMAACuenta()");
                LNCuentas.AgregaParametroMAACuenta(Convert.ToInt32(cBoxParametros.SelectedItem.Value),
                    IdTarjeta, IdPlantilla, this.Usuario, LH_ParabAdminCtasTDC);
                log.Info("TERMINA AgregaParametroMAACuenta()");

                X.Msg.Notify("", "Parámetro Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.cBoxParametros.Reset();

                LlenaParametrosCuenta();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro", "Ocurrió un error al asignar el valor al Parámetro").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);
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
                        case "TipoValidacion": unParametro.TipoValidacion = column.Value; break;
                        case "ValorInicial": unParametro.ValorInicial = column.Value; break;
                        case "ValorFinal": unParametro.ValorFinal = column.Value; break;
                        case "ExpresionRegular": unParametro.ExpresionRegular = column.Value; break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                this.hdnIdParametroMA.Value = unParametro.ID_Parametro;
                this.hdnIdValorPMA.Value = unParametro.ID_ValordelParametro;
                this.hdnIdPlantilla.Value = unParametro.ID_Plantilla;

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
                                DAOProducto.ListaElementosCatalogoPMA(Convert.ToInt64(this.hdnIdColectiva.Value), 
                                Convert.ToInt64(unParametro.ID_Parametro), string.Empty, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabAdminCtasTDC);
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

                                default: break;
                            }
                        }

                        this.txtParametro.Text = unParametro.Descripcion;
                        this.WdwValorParametro.Title += " - " + unParametro.Nombre;
                        this.WdwValorParametro.Show();
                        break;

                    case "Delete":
                        pCI.Info("INICIA BorraValorParametro()");
                        LNProducto.BorraValorParametro((int)unParametro.ID_ValordelParametro, this.Usuario, LH_ParabAdminCtasTDC, "_CuentasCredito");
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
            LogPCI logPCI = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                ParametroValor elParametro = new ParametroValor();

                elParametro.ID_Parametro = Convert.ToInt32(this.hdnIdParametroMA.Value);
                elParametro.ID_Plantilla = Convert.ToInt32(this.hdnIdPlantilla.Value);
                elParametro.ID_ValordelParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);
                elParametro.Valor = String.IsNullOrEmpty(this.txtValorParFloat.Text) ?
                    string.IsNullOrEmpty(this.txtValorParInt.Text) ?
                    string.IsNullOrEmpty(this.txtValorParString.Text) ?
                    string.IsNullOrEmpty(this.cBoxValorPar.SelectedItem.Value) ?
                    this.cBoxCatalogoPMA.SelectedItem.Value : this.cBoxValorPar.SelectedItem.Value :
                    this.txtValorParString.Text : this.txtValorParInt.Text :
                    this.txtValorParFloat.Text;

                logPCI.Info("INICIA ModificaValorParametro()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminCtasTDC, "_CuentasCredito");
                logPCI.Info("TERMINA ModificaValorParametro()");

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
                logPCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al establecer el valor del Parámetro").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Aplicar de la pestaña de Movimientos Manuales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        public void btnAplicar_Click(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                log.Info("INICIA VerificaFechaCorte()");
                LNCuentas.VerificaFechaCorte(this.dfFechaAjusteManual.Value,
                    Convert.ToInt64(this.hdnIdCuentaCC.Value), LH_ParabAdminCtasTDC);
                log.Info("TERMINA VerificaFechaCorte()");

                EventoManual elEvento = new EventoManual();

                elEvento.MedioAcceso = this.hdnTarjeta.Value.ToString();
                elEvento.IdCadenaComercial = Convert.ToInt64(this.hdnIdCadena.Value);
                elEvento.IdColectivaOrigen = Convert.ToInt64(this.hdnIdColectiva.Value);
                elEvento.FechaAplicacion = Convert.ToDateTime(this.dfFechaAjusteManual.Value).ToString("yyyy-MM-dd");
                elEvento.Importe = this.txtImporte.Text.Replace("$","").Replace(",","");
                elEvento.IVA = this.hdnValorIVA.Value.ToString();
                elEvento.Referencia = Convert.ToInt64(String.IsNullOrEmpty(this.txtRefAjustesManuales.Text) ? null :
                    this.txtRefAjustesManuales.Text);
                elEvento.Observaciones = this.txtObsAjustesManuales.Text;

                log.Info("INICIA RegistraPertenenciaManual()");
                LNProducto.RegistraPertenenciaManual(elEvento, Convert.ToInt64(this.cBoxPertenencias.SelectedItem.Value),
                   this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_ParabAdminCtasTDC);
                log.Info("TERMINA RegistraPertenenciaManual()");

                X.Msg.Notify("Movimientos Manuales", "Movimiento aplicado <br /><br />  <b> E X I T O S A M E N T E</b> <br />  <br /> ").Show();

                this.FormPanelMovsManuales.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Movimientos Manuales", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Movimientos Manuales", "Ocurrió un error al aplicar el Movimiento Manual").Show();
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
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                DataTable dtOpsPendientes = new DataTable();

                dtOpsPendientes = HttpContext.Current.Session["DtSaldosYMovs"] as DataTable;

                if (dtOpsPendientes == null)
                {
                    pCI.Info("INICIA ObtieneDetalleMovimientos()");
                    dtOpsPendientes = DAOTarjetaCuenta.ObtieneDetalleMovimientos(
                        this.dfInicioSYM.SelectedDate, this.dfFinSYM.SelectedDate,
                        Convert.ToInt64(this.hdnIdCuentaCC.Value), Convert.ToInt64(this.hdnIdCuentaCDC.Value), LH_ParabAdminCtasTDC);
                    pCI.Info("TERMINA ObtieneDetalleMovimientos()");

                    HttpContext.Current.Session.Add("DtSaldosYMovs", dtOpsPendientes);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabAdminCtasTDC).Valor);

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
                                Handler = "AdminCtasParab.ClicDetSaldosYMovsDePaso()",
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
        [DirectMethod(Namespace = "AdminCtasParab")]
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
        [DirectMethod(Namespace = "AdminCtasParab")]
        public void ExportDTDetalleToExcel()
        {
            try
            {
                string reportName = "SaldosYMovimientos";
                DataTable _dtSaldosYMovs = HttpContext.Current.Session["DtSaldosYMovs"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);
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
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                DataTable dtOpsPendientes = new DataTable();

                dtOpsPendientes = HttpContext.Current.Session["DtOpsPendientes"] as DataTable;

                if (dtOpsPendientes == null)
                {
                    unLog.Info("INICIA ObtieneOperacionesEnTransito()");
                    dtOpsPendientes = DAOTarjetaCuenta.ObtieneOperacionesEnTransito(
                        this.dfInicioSYM.SelectedDate, this.dfFinSYM.SelectedDate,
                        Convert.ToInt64(this.hdnIdCuentaCC.Value), LH_ParabAdminCtasTDC);
                    unLog.Info("TERMINA ObtieneOperacionesEnTransito()");

                    HttpContext.Current.Session.Add("DtOpsPendientes", dtOpsPendientes);
                }

                int maxRegistros = Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "Reporte_MaxRegsPagina", LH_ParabAdminCtasTDC).Valor);

                if (dtOpsPendientes.Rows.Count > maxRegistros)
                {
                    X.Msg.Confirm("Movimientos Pendientes", "La búsqueda arrojó más de " + maxRegistros.ToString() + " registros.</br>" +
                        "El reporte se exportará directamente al archivo Excel.", new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "AdminCtasParab.ClicOpsPendientesDePaso()",
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
        [DirectMethod(Namespace = "AdminCtasParab")]
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
        [DirectMethod(Namespace = "AdminCtasParab")]
        public void ExportDTOpsPendientesToExcel()
        {
            try
            {
                string reportName = "OperacionesPendientes";
                DataTable _dtSaldosYMovs = HttpContext.Current.Session["DtOpsPendientes"] as DataTable;

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
                LogPCI pCI = new LogPCI(LH_ParabAdminCtasTDC);
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
        [DirectMethod(Namespace = "AdminCtasParab")]
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
            LogPCI logPCI = new LogPCI(LH_ParabAdminCtasTDC);
            
            try
            {
                logPCI.Info("INICIA ObtieneResumenMovimientos()");
                ResumenMovimientosCuenta elResumen = 
                    DAOTarjetaCuenta.ObtieneResumenMovimientos(this.dfInicioSYM.SelectedDate,
                    this.dfFinSYM.SelectedDate, Convert.ToInt64(this.hdnIdCuentaCDC.Value),
                    Convert.ToInt64(this.hdnIdCuentaLDC.Value), LH_ParabAdminCtasTDC);
                logPCI.Info("TERMINA ObtieneResumenMovimientos()");

                logPCI.Info("INICIA ObtieneResumenMovimientosAlCorte()");
                ResumenMovimientosCuenta resumen = DAOTarjetaCuenta.ObtieneResumenMovimientosAlCorte(
                    this.dfInicioSYM.SelectedDate, this.dfFinSYM.SelectedDate,
                    Convert.ToInt64(this.hdnIdCuentaCDC.Value), Convert.ToInt64(this.hdnIdCuentaLDC.Value),
                    Convert.ToInt64(this.hdnIdCuentaCC.Value),  LH_ParabAdminCtasTDC);
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

                /// Aplica funcionalidad de consumo del web service para consultar TDC
                char[] charsToTrim = { '*', '"', ' ' };
                string Tarjeta = this.hdnTarjeta.Value.ToString();
                string TarjetaEnmasc = this.hdnMaskCard.Value.ToString();
                string wsLoginResp = null;
                
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                _headers.URL = Configuracion.Get(Guid.Parse(
                            ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                            "WsParab_URL", LH_ParabAdminCtasTDC).Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Usr", LH_ParabAdminCtasTDC).Valor;
                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Pwd", LH_ParabAdminCtasTDC).Valor;

                logPCI.Info("INICIA Parabilium.Login()");
                wsLoginResp = Parabilium.Login(_headers, _body, LH_ParabAdminCtasTDC);
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

                logPCI.Info("INICIA Parabilium.ObtenerMovimientosTarjeta()");
                RespuestasJSON.ConsultarTarjetaCredito wsConsultaTDCResp = Parabilium.ObtenerMovimientosTarjeta(_headers, consultaTDC, maskedBody, LH_ParabAdminCtasTDC);
                logPCI.Info("TERMINA Parabilium.ObtenerMovimientosTarjeta()");

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
            HttpContext.Current.Session.Add("DtSaldosYMovs", null);
            this.StoreSaldosYMovs.RemoveAll();

            HttpContext.Current.Session.Add("DtOpsPendientes", null);
            this.StoreOpsPendientes.RemoveAll();

            Thread.Sleep(100);

            btnDetMovsHide.FireEvent("click");
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana emergente de Bloqueo de Tarjeta,
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBloquea_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminCtasTDC);

            try
            {
                string wsActResp = null;
                string laTarjeta = this.hdnSelectedCard.Value.ToString();

                log.Info("INICIA LoginWebServiceParabilium()");
                Parametros.Headers bloqHeaders = LNCuentas.LoginWebServiceParabilium(LH_ParabAdminCtasTDC);
                log.Info("TERMINA LoginWebServiceParabilium()");

                log.Info("INICIA BloqueaTarjetaWebServiceParabilium()");
                wsActResp = LNCuentas.BloqueaTarjetaWebServiceParabilium(bloqHeaders, laTarjeta,
                    MaskSensitiveData.cardNumber(laTarjeta), this.cBoxMotivo.SelectedItem.Value, LH_ParabAdminCtasTDC);
                log.Info("TERMINA BloqueaTarjetaWebServiceParabilium()");

                log.Info("INICIA RegistraEnBitacora()");
                LNInfoOnBoarding.RegistraEnBitacora("pantalla_AdminCuentas_BloquearTarjeta", "MedioAcceso",
                    "ID_EstatusMA", this.hdnSelectedIdMA.Value.ToString(), "8", 
                    "Bloqueo de Tarjeta: " + MaskSensitiveData.cardNumber(laTarjeta),
                    this.Usuario, LH_ParabAdminCtasTDC);
                log.Info("TERMINA RegistraEnBitacora()");

                X.Msg.Notify("", "Tarjeta Bloqueada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridTarjetas(Convert.ToInt32(this.hdnIdMA.Value));
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