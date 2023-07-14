using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Utilerias;

namespace Administracion
{
    public partial class AdminCuentasOnBoarding : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Administrar Cuentas OnBoarding
        private LogHeader LH_ParabAdminCtasOnB = new LogHeader();

        #endregion

        /// <summary>
        /// Clase entidad para el combo predictivo de País
        /// </summary>
        public class PaisComboPredictivo
        {
            public int      ID_Pais     { get; set; }
            public String   Clave       { get; set; }
            public String   Descripcion { get; set; }
        }

        /// <summary>
        /// Realiza y controla la carga de la página Administración de Cuentas OnBoarding
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminCtasOnB.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminCtasOnB.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminCtasOnB.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminCtasOnB.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminCtasOnB);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdminCuentasOnBoarding Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaNac.MaxDate = DateTime.Today;

                    log.Info("INICIA ListaTiposParametrosMA()");
                    this.StoreTipoParametroMA.DataSource =
                        DAOProducto.ListaTiposParametrosMA(true, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminCtasOnB);
                    log.Info("TERMINA ListaTiposParametrosMA()");
                    this.StoreTipoParametroMA.DataBind();

                    //log.Info("INICIA ListaCatalogoGenero()");
                    //this.StoreGenero.DataSource = 
                    //    DAOTarjetaCuenta.ListaCatalogoGenero(LH_ParabAdminCtasOnB);
                    //log.Info("TERMINA ListaCatalogoGenero()");
                    //this.StoreGenero.DataBind();

                    //log.Info("INICIA ListaCatalogoEstadosMexico()");
                    //this.StoreEstadosRepMex.DataSource =
                    //    DAOTarjetaCuenta.ListaCatalogoEstadosMexico(LH_ParabAdminCtasOnB);
                    //log.Info("TERMINA ListaCatalogoEstadosMexico()");
                    //this.StoreEstadosRepMex.DataBind();

                    EstableceComboPaises();
                }

                log.Info("TERMINA AdminCuentasOnBoarding Page_Load()");
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
        /// Establece los valores del combo Países con la información de base de datos
        /// </summary>
        protected void EstableceComboPaises()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasOnB);

            unLog.Info("INICIA ListaCatalogoPaises()");
            DataTable dtPaises = DAOTarjetaCuenta.ListaCatalogoPaises(LH_ParabAdminCtasOnB);
            unLog.Info("TERMINA ListaCatalogoPaises()");

            List<PaisComboPredictivo> ComboList = new List<PaisComboPredictivo>();

            foreach (DataRow drCol in dtPaises.Rows)
            {
                var paisCombo = new PaisComboPredictivo()
                {
                    ID_Pais = int.Parse(drCol["ID_Pais"].ToString()),
                    Clave = drCol["Clave"].ToString(),
                    Descripcion = drCol["Descripcion"].ToString()
                };
                ComboList.Add(paisCombo);
            }

            this.StorePaises.DataSource = ComboList;
            this.StorePaises.DataBind();
        }

        /// <summary>
        /// Llena el grid de resultado de búsqueda de cuentas
        /// </summary>
        protected void LlenarGridResultados()
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.CuentaInterna = this.txtNumCuenta.Text;
                datosPersonales.TarjetaTitular = this.txtNumTarjeta.Text;
                datosPersonales.SoloAdicionales = this.chkBoxSoloAdicionales.Checked;
                datosPersonales.Nombre = this.txtNombre.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaterno.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaterno.Text;

                pCI.Info("INICIA ObtieneCuentasPorFiltroOnB()");
                DataSet dsResultados = DAOTarjetaCuenta.ObtieneCuentasPorFiltroOnB(datosPersonales, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminCtasOnB);
                pCI.Info("TERMINA ObtieneCuentasPorFiltroOnB()");

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

                DataTable dt = Tarjetas.EnmascaraTablaConTarjetas(dsResultados.Tables[0], "Tarjeta", "Enmascara", LH_ParabAdminCtasOnB);

                this.StoreCuentas.DataSource = dt;
                this.StoreCuentas.DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Consulta de Cuentas", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Consulta de Cuentas", "Ocurrió un error al realiza la búsqueda").Show();
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
                this.EastPanel.Title = "_";
            }

            this.FormPanelTitular.Reset();

            StoreTarjetas.RemoveAll();
            StoreTiposMA.RemoveAll();

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
                string numTarjeta = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "CLDC": IdCuentaCLDC = int.Parse(column.Value); break;
                        case "CCLC": this.hdnIdCuentaCC.Value = column.Value; break;
                        case "IdTarjeta": IdMA = int.Parse(column.Value); break;
                        case "IdColectivaCuentahabiente": IdColectiva = int.Parse(column.Value); break;
                        case "Tarjeta": numTarjeta = column.Value; break;
                        case "IdCadenaComercial": IdCadena = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                limpiaBusquedaPrevia(false);

                this.hdnIdCuentaLDC.Value = IdCuentaCLDC;
                this.hdnIdMA.Value = IdMA;
                this.hdnIdColectiva.Value = IdColectiva;
                this.hdnIdCadena.Value = IdCadena;

                LlenaFieldSetDatosTitular(IdCuentaCLDC, IdMA, IdColectiva);
                LlenaGridTarjetas(IdMA);
                LlenaGridTiposMA(IdMA);

                this.EastPanel.Title = "Tarjeta " + numTarjeta;
                this.EastPanel.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminCtasOnB);
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
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                FormPanelTitular.Reset();

                unLog.Info("INICIA ObtieneDatosPersonalesCtaOnB()");
                DatosPersonalesCuenta losDatos = DAOTarjetaCuenta.ObtieneDatosPersonalesCtaOnB(
                   idTarjeta, idColectiva, this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_ParabAdminCtasOnB);
                unLog.Info("TERMINA ObtieneDatosPersonalesCtaOnB()");

                this.txtNombreClienteTitular.Text = losDatos.Nombre;
                this.txtApPaternoTitular.Text = losDatos.ApellidoPaterno;
                this.txtApMaternoTitular.Text = losDatos.ApellidoMaterno;
                this.dfFechaNac.Value = losDatos.FechaNacimiento;
                this.cBoxEdoNac.Value = losDatos.ClaveEdoNacimiento;
                this.cBoxEdoNac.Text = losDatos.EstadoNacimiento;
                this.cBoxGenero.Value = losDatos.Genero;
                this.txtNumID.Text = losDatos.NumeroIdentificacion;
                this.txtRFCTitular.Text = losDatos.RFC;
                this.txtCURP.Text = losDatos.CURP;
                this.txtOcupacion.Text = losDatos.Ocupacion;
                this.txtProfesion.Text = losDatos.Profesion;
                this.txtNacionalidad.Text = losDatos.Nacionalidad;

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
                this.hdnClaveCiudad.Value = losDatos.ClaveCiudad;
                this.txtCiudad.Text = losDatos.Ciudad;
                this.txtClaveEstado.Text = losDatos.ClaveEstado;
                this.txtEstadoTitular.Text = losDatos.Estado;
                this.cBoxPais.Value = losDatos.IdPais;
                this.txtGiroNegocio.Text = losDatos.GiroNegocio;
                this.txtLatitud.Text = losDatos.Latitud;
                this.txtLongitud.Text = losDatos.Longitud;

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
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Datos del Cliente", "Error al obtener los datos personales del Cliente").Show();
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
                LogPCI logPCI = new LogPCI(LH_ParabAdminCtasOnB);

                try
                {
                    logPCI.Info("INICIA ListaDatosPorCodigoPostal_V2()");
                    DataTable dtColonias = 
                        DAOTarjetaCuenta.ListaDatosPorCodigoPostal_V2(this.txtCPCliente.Text, LH_ParabAdminCtasOnB);
                    logPCI.Info("TERMINA ListaDatosPorCodigoPostal_V2()");

                    if (dtColonias.Rows.Count < 1)
                    {
                        X.Msg.Alert("Código Postal", "No existen coincidencias con el Código Postal ingresado").Show();
                    }
                    else
                    {
                        this.StoreColonias.DataSource = dtColonias;
                        this.StoreColonias.DataBind();

                        this.txtClaveMunicipio.Text = dtColonias.Rows[0]["ClaveMunicipio"].ToString().Trim();
                        this.txtMunicipioTitular.Text = dtColonias.Rows[0]["Municipio"].ToString().Trim();
                        this.hdnClaveCiudad.Value = dtColonias.Rows[0]["ClaveCiudad"].ToString().Trim();
                        this.txtCiudad.Text = dtColonias.Rows[0]["Ciudad"].ToString().Trim();
                        this.txtClaveEstado.Text = dtColonias.Rows[0]["ClaveEstado"].ToString().Trim();
                        this.txtEstadoTitular.Text = dtColonias.Rows[0]["Estado"].ToString().Trim();
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
        /// Controla el evento Click al botón Guardar del formulario de Datos, invocando la actualización
        /// de los datos del cliente en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaDatos_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.ID_Cuenta = Convert.ToInt32(this.hdnIdCuentaLDC.Value);
                datosPersonales.ID_MA = Convert.ToInt32(this.hdnIdMA.Value);
                datosPersonales.ID_Colectiva = Convert.ToInt32(this.hdnIdColectiva.Value);

                datosPersonales.Nombre = this.txtNombreClienteTitular.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaternoTitular.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaternoTitular.Text;
                datosPersonales.FechaNacimiento = this.dfFechaNac.SelectedDate;
                datosPersonales.ClaveEdoNacimiento = this.cBoxEdoNac.SelectedItem.Value;
                datosPersonales.Genero = this.cBoxGenero.SelectedItem.Value;
                datosPersonales.NumeroIdentificacion = this.txtNumID.Text;
                datosPersonales.RFC = this.txtRFCTitular.Text;
                datosPersonales.CURP = this.txtCURP.Text;
                datosPersonales.Ocupacion = this.txtOcupacion.Text;
                datosPersonales.Profesion = this.txtProfesion.Text;
                datosPersonales.Nacionalidad = this.txtNacionalidad.Text;

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
                datosPersonales.IdPais = Convert.ToInt32(this.cBoxPais.SelectedItem.Value);
                datosPersonales.GiroNegocio = this.txtGiroNegocio.Text;
                datosPersonales.Latitud = this.txtLatitud.Text;
                datosPersonales.Longitud = this.txtLongitud.Text;

                datosPersonales.NumTelParticular = this.nfTelParticular.Text;
                datosPersonales.NumTelCelular = this.nfTelCelular.Text;
                datosPersonales.NumTelTrabajo = this.nfTelTrabajo.Text;
                datosPersonales.Email = this.txtCorreo.Text;

                pCI.Info("INICIA ActualizaDatosPersonalesCuentaOnB()");
                LNCuentas.ActualizaDatosPersonalesCuentaOnB(datosPersonales, this.Usuario, LH_ParabAdminCtasOnB);
                pCI.Info("INICIA ActualizaDatosPersonalesCuentaOnB()");

                X.Msg.Notify("", "Datos Personales del Cuentahabiente actualizados <br /> <br /> <b> E X I T O S A M E N T E </b> <br /> <br />").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos Personales", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Datos Personales", "Ocurrió un error al modificar la información del Cuentahabiente").Show();
            }
        }

        /// <summary>
        /// Llena el GridPanel Tarjetas con los datos de la familia de la tarjeta seleccionada
        /// </summary>
        /// <param name="IDTarjeta">Identificador de la tarjeta</param>
        protected void LlenaGridTarjetas(int IDTarjeta)
        {
            LogPCI log = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                this.StoreTarjetas.RemoveAll();
                log.Info("INICIA ObtieneFamiliaTarjetas()");
                DataSet dsFamTarjetas =
                    DAOTarjetaCuenta.ObtieneFamiliaTarjetas(IDTarjeta, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminCtasOnB);
                log.Info("TERMINA ObtieneFamiliaTarjetas()");

                
                DataTable dtEnmascara = Tarjetas.EnmascaraTablaConTarjetas(dsFamTarjetas.Tables[0], "Tarjeta", "Enmascara", LH_ParabAdminCtasOnB);
                

                this.StoreTarjetas.DataSource = dtEnmascara;
                this.StoreTarjetas.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Tarjetas", "Ocurrió un error al obtener las Tarjetas Adicionales").Show();
            }
        }

        /// <summary>
        /// Llena el GridPanel TiposMA con los datos de base de datos
        /// </summary>
        /// <param name="IDTarjeta">Identificador de la tarjeta</param>
        protected void LlenaGridTiposMA(int IDTarjeta)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                this.StoreTiposMA.RemoveAll();

                unLog.Info("INICIA ObtieneTiposMediosAcceso()");
                this.StoreTiposMA.DataSource =
                    DAOTarjetaCuenta.ObtieneTiposMediosAcceso(IDTarjeta, LH_ParabAdminCtasOnB);
                unLog.Info("TERMINA ObtieneTiposMediosAcceso()");
                this.StoreTiposMA.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Medios de Acceso", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Medios de Acceso", "Ocurrió un error al obtener los Medios de Acceso").Show();
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
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                int IdCuenta = Convert.ToInt32(this.hdnIdCuentaLDC.Value);

                pCI.Info("INICIA ObtienePMAPorTipoSinAsignarCuenta()");
                StoreParametros.DataSource = DAOTarjetaCuenta.ObtienePMAPorTipoSinAsignarCuenta(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdCuenta, LH_ParabAdminCtasOnB);
                pCI.Info("TERMINA ObtienePMAPorTipoSinAsignarCuenta()");
                StoreParametros.DataBind();

                pCI.Info("INICIA ObtieneParametrosMACuenta()");
                StoreValoresParametros.DataSource = DAOTarjetaCuenta.ObtieneParametrosMACuenta(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdCuenta, LH_ParabAdminCtasOnB);
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
            LogPCI log = new LogPCI(LH_ParabAdminCtasOnB);

            try
            {
                int IdTarjeta = Convert.ToInt32(this.hdnIdMA.Value);
                Int64 IdPlantilla = Convert.ToInt64(this.hdnIdPlantilla.Value);

                log.Info("INICIA AgregaParametroMAACuenta()");
                LNCuentas.AgregaParametroMAACuenta(Convert.ToInt32(cBoxParametros.SelectedItem.Value),
                    IdTarjeta, IdPlantilla, this.Usuario, LH_ParabAdminCtasOnB);
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
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminCtasOnB);
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
                                LH_ParabAdminCtasOnB);
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

                    case "Delete":
                        pCI.Info("INICIA BorraValorParametro()");
                        LNProducto.BorraValorParametro((int)unParametro.ID_ValordelParametro, this.Usuario, LH_ParabAdminCtasOnB, "_CuentasCredito");
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
            LogPCI logPCI = new LogPCI(LH_ParabAdminCtasOnB);

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
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminCtasOnB, "_CuentasDebitoPrepago");
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
    }
}