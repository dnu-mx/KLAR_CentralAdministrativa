using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;

namespace Administracion
{
    /// <summary>
    /// Realiza y controla la carga de la página Cuentas Bancarias
    /// </summary>
    /// <param name="sender">Objeto que envía el control</param>
    /// <param name="e">Argumentos del evento que se ejecutó</param>
    public partial class CuentasBancarias : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    EstableceRolesAuditables();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
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
        /// Llena el grid de resultado de búsqueda de cuentas
        /// </summary>
        protected void LlenarGridResultados()
        {
            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.ID_Cuenta = String.IsNullOrEmpty(this.txtNumCuenta.Text) ? 0 : int.Parse(this.txtNumCuenta.Text);
                datosPersonales.TarjetaTitular = this.txtNumTarjeta.Text;
                datosPersonales.TarjetaAdicional = this.txtNumTarjetaAdicional.Text;
                datosPersonales.Nombre = this.txtNombre.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaterno.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaterno.Text;

                DataSet dsResultados = LNCuentasBancarias_V2.ConsultaCuentas(datosPersonales, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 100)
                {
                    X.Msg.Alert("Consulta de Cuentas", "Demasiadas coincidencias, por favor afine su búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Cuentas", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                GridResultados.GetStore().DataSource = dsResultados;
                GridResultados.GetStore().DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Consulta de Cuentas", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Consulta de Cuentas", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
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
        /// Controla el evento Refresh en el grid de resultados, invocando nuevamente
        /// la búsqueda de cuentas a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento Refresh Data del Store Clientes</param>
        protected void StoreCuentas_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de una cuenta en
        /// el Grid de Resultados Cuentas
        /// </summary>
        protected void limpiaSeleccionPrevia()
        {
            FormPanelTitular.Reset();

            FormPanelCuenta.Reset();

            FormPanelAdicionales.Reset();
            StoreTarjetas.RemoveAll();
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
            }

            StoreCuentas.RemoveAll();

            FormPanelTitular.Reset();

            this.nfFechaCorte.Value = "";
            this.txtHDFechaCorte.Text = "";
            this.nfFechaLimitePago.Value = "";
            this.txtHDFechaLimitePago.Text = "";
            this.nfLimiteCredito.Value = "";
            this.txtHDLimiteCredito.Value = "";
            this.txtNombreEmbozadoTitular.Text = "";
            this.txtHDEmbozado.Text = "";
            this.dfVigenciaTarjeta.Value = "";
            this.dfHDVigenciaTarjeta.Value = "";

            FormPanelAdicionales.Reset();
            StoreTarjetas.RemoveAll();
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
                int IdCuentaCCLC = 0;
                int IdMA = 0;
                int IdColectiva = 0;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "ID_CuentaCLDC": IdCuentaCLDC = int.Parse(column.Value); break;
                        case "ID_CuentaCCLC": IdCuentaCCLC = int.Parse(column.Value); break;
                        case "ID_MA": IdMA = int.Parse(column.Value); break;
                        case "ID_Colectiva": IdColectiva = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();

                LlenaFieldSetDatosTitular(IdCuentaCLDC, IdMA, IdColectiva);
                LlenaFormPanelTarjetas(IdCuentaCLDC, IdMA);
                LlenaGridTarjetas(IdCuentaCLDC);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Resultados", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Resultados", "Ocurrió un Error al Seleccionar la Cuenta").Show();
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
            try
            {
                FormPanelTitular.Reset();

                 DataSet dsDatos = DAOCuentasBancarias_V2.ObtieneDatosPersonalesCuenta(
                    idTarjeta, idColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                this.txtID_Cuenta.Text = idCuenta.ToString();
                this.txtID_MA.Text = idTarjeta.ToString();
                this.txtID_Colectiva.Text = idColectiva.ToString();

                this.txtTarjetaTitular.Text = dsDatos.Tables[0].Rows[0]["NumeroTarjeta"].ToString().Trim();

                this.txtNombreClienteTitular.Text = dsDatos.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.txtApPaternoTitular.Text = dsDatos.Tables[0].Rows[0]["ApPaterno"].ToString().Trim();
                this.txtApMaternoTitular.Text = dsDatos.Tables[0].Rows[0]["ApMaterno"].ToString().Trim();
                this.txtRFCTitular.Text = dsDatos.Tables[0].Rows[0]["RFC"].ToString().Trim();

                this.txtID_Direccion.Text = dsDatos.Tables[0].Rows[0]["ID_Direccion"].ToString().Trim();
                this.txtCalle.Text = dsDatos.Tables[0].Rows[0]["Calle"].ToString().Trim();
                this.txtNumExterior.Text = dsDatos.Tables[0].Rows[0]["NumExterior"].ToString().Trim();
                this.txtNumInterior.Text = dsDatos.Tables[0].Rows[0]["NumInterior"].ToString().Trim();
                this.txtEntreCalles.Text = dsDatos.Tables[0].Rows[0]["EntreCalles"].ToString().Trim();
                this.txtReferencias.Text = dsDatos.Tables[0].Rows[0]["Referencias"].ToString().Trim();
                this.txtCPCliente.Text = dsDatos.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim();
                this.txtIDColonia.Text = dsDatos.Tables[0].Rows[0]["ID_Colonia"].ToString().Trim();
                this.cBoxColonia.SelectedItem.Text = dsDatos.Tables[0].Rows[0]["Colonia"].ToString().Trim();
                this.txtClaveMunicipio.Text = dsDatos.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                this.txtMunicipioTitular.Text = dsDatos.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                this.txtClaveEstado.Text = dsDatos.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                this.txtEstadoTitular.Text = dsDatos.Tables[0].Rows[0]["Estado"].ToString().Trim();

                this.nfTelParticular.Text = dsDatos.Tables[0].Rows[0]["TelParticular"].ToString().Trim();
                if (String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["TelParticular"].ToString().Trim()))
                {
                    this.nfTelParticular.Value = "";
                }
                
                this.nfTelCelular.Text = dsDatos.Tables[0].Rows[0]["TelCelular"].ToString().Trim();
                if (String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["TelCelular"].ToString().Trim()))
                {
                    this.nfTelCelular.Value = "";
                }

                this.nfTelTrabajo.Text = dsDatos.Tables[0].Rows[0]["TelTrabajo"].ToString().Trim();
                if (String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["TelTrabajo"].ToString().Trim()))
                {
                    this.nfTelTrabajo.Value = "";
                }

                this.txtCorreo.Text = dsDatos.Tables[0].Rows[0]["Email"].ToString().Trim();

                LlenaComboColonias();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Consultar los Datos del Cliente").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Llena el FormPanel Tarjetas con los datos de la cuenta seleccionada
        /// </summary>
        /// <param name="idCuenta">ID de la cuenta</param>
        /// <param name="idTarjeta">ID de la tarjeta</param>
        protected void LlenaFormPanelTarjetas(int idCuenta, int idTarjeta)
        {
            try
            {
                DataSet dsDatosCuenta = new DataSet();

                dsDatosCuenta = DAOCuentasBancarias_V2.ObtieneDatosCuenta(
                                   idCuenta, idTarjeta, this.Usuario,
                                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsDatosCuenta.Tables[0].Rows.Count > 0)
                {
                    LlenaFieldSetCuenta(dsDatosCuenta);
                }
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros de la Cuenta", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Parámetros de la Cuenta", "Ocurrió un Error al Consultar los Parámetros de la Cuenta").Show();
            }
        }

        /// <summary>
        /// Llena los campos del FieldSet Cuenta con sus correspondientes valores
        /// </summary>
        /// <param name="dsAccountData">DataSet origen de los valores</param>
        protected void LlenaFieldSetCuenta(DataSet dsDatosCuenta)
        {
            try
            {
                //Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                //Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                String ldc, ldcPendiente;

                this.btnValorActual.Hidden = true;
                this.btnValorPendiente.Hidden = true;

                this.nfFechaCorte.Value = dsDatosCuenta.Tables[0].Rows[0]["FechaCorte"].ToString().Trim();
                this.txtHDFechaCorte.Text = dsDatosCuenta.Tables[0].Rows[0]["FechaCorte"].ToString().Trim();

                this.nfFechaLimitePago.Value = dsDatosCuenta.Tables[0].Rows[0]["FechaLimitePago"].ToString().Trim();
                this.txtHDFechaLimitePago.Text = dsDatosCuenta.Tables[0].Rows[0]["FechaLimitePago"].ToString().Trim();


                /////////CAMPO AUTORIZABLE LIMITE DE CREDITO
                ldc = dsDatosCuenta.Tables[0].Rows[0]["LimiteCreditoActual"].ToString().Trim();
                ldcPendiente = dsDatosCuenta.Tables[0].Rows[0]["LimiteCreditoPendiente"].ToString().Trim();

                ControlaCambiosLDC(ldc, ldcPendiente, false);
                

                //if (String.IsNullOrEmpty(ldc))
                //{
                //    this.nfLimiteCredito.Value = "";
                //}
                //else
                //{
                //    this.nfLimiteCredito.Value = String.Format("{0:F2}", float.Parse(ldc));
                //    this.txtHDLimiteCredito.Value = String.Format("{0:F2}", float.Parse(ldc));
                //    this.txtHDLimiteCreditoPendiente.Value = String.Format("{0:F2}", float.Parse(ldc));
                //}

                ////Se ocultan los indicadores visuales de cambios pendientes en el control
                //this.nfLimiteCredito.RemoveClass("chng-red-nf-css");
                ////this.nfLimiteCredito.HideIndicator();

                ////Validaciones por rol
                //if (esEjecutor)
                //{
                //    LlenaLimiteCredito(ldc, ldcPendiente);
                //}
                //else if (esAutorizador)
                //{
                //    this.btnGuardarCuenta.Hidden = true;

                //    LlenaLimiteCredito(ldc, ldcPendiente);

                //    this.btnAutorizarCambios.Hidden = ldc != ldcPendiente ? false : true;

                //    FieldSetCuentasSoloLectura();
                //}
                //else
                //{
                //    this.nfLimiteCredito.ReadOnly = true;
                //}
                /////////


                this.txtNombreEmbozadoTitular.Text = dsDatosCuenta.Tables[0].Rows[0]["NombreEmbozo"].ToString().Trim();
                this.txtHDEmbozado.Text = dsDatosCuenta.Tables[0].Rows[0]["NombreEmbozo"].ToString().Trim();

                this.dfVigenciaTarjeta.Value = Convert.ToDateTime(dsDatosCuenta.Tables[0].Rows[0]["VigenciaTarjeta"].ToString().Trim());
                this.dfHDVigenciaTarjeta.Value = Convert.ToDateTime(dsDatosCuenta.Tables[0].Rows[0]["VigenciaTarjeta"].ToString().Trim());
            }

            catch (Exception ex)
            {
                throw new Exception("LlenaFieldSetCuenta() " + ex.Message);
            }
        }

        /// <summary>
        /// Controla los cambios en el valor al límite de crédito de la cuenta, según el rol en sesión y si
        /// tiene algún cambio pendiente de autorización, para mostrarlo en el control correspondiente
        /// </summary>
        /// <param name="LimiteCredito">Límite de crédito actual</param>
        /// <param name="LimiteCreditoPendiente">Límite de crédito pendiente de autorización</param>
        protected void ControlaCambiosLDC(String LimiteCredito, String LimiteCreditoPendiente, Boolean esVistaPorBoton)
        {
            try
            {
                Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());


                if (String.IsNullOrEmpty(LimiteCredito))
                {
                    this.nfLimiteCredito.Value = "";
                }
                else
                {
                    this.nfLimiteCredito.Value = String.Format("{0:F2}", float.Parse(LimiteCredito));
                    this.txtHDLimiteCredito.Value = String.Format("{0:F2}", float.Parse(LimiteCredito));
                    this.txtHDLimiteCreditoPendiente.Value = String.Format("{0:F2}", 
                        float.Parse(String.IsNullOrEmpty(LimiteCreditoPendiente) ? "0" : LimiteCreditoPendiente));
                }

                //Se ocultan los indicadores visuales de cambio pendiente en el control
                this.nfLimiteCredito.RemoveClass("chng-red-nf-css");
                this.nfLimiteCredito.RemoveClass("chng-blue-nf-css");


                //Se realizan las validaciones por rol
                if (esEjecutor)
                {
                    LlenaLimiteCredito(LimiteCredito, LimiteCreditoPendiente, esVistaPorBoton);
                }
                else if (esAutorizador)
                {
                    LlenaLimiteCredito(LimiteCredito, LimiteCreditoPendiente, esVistaPorBoton);

                    this.btnGuardarCuenta.Hidden = true;

                    this.btnAutorizarCambios.Hidden = LimiteCredito != LimiteCreditoPendiente ? false : true;

                    FieldSetCuentasSoloLectura(true);
                }
                else
                {
                    this.nfLimiteCredito.ReadOnly = true;
                }

                //if (!String.IsNullOrEmpty(LimiteCreditoPendiente) && !String.IsNullOrEmpty(LimiteCredito) 
                //    && LimiteCredito != LimiteCreditoPendiente)
                //{
                //    this.nfLimiteCredito.Cls = "chng-red-nf-css";

                //    this.nfLimiteCredito.Value = String.Format("{0:F2}", float.Parse(LimiteCreditoPendiente));
                //    this.txtHDLimiteCredito.Value = String.Format("{0:F2}", float.Parse(LimiteCreditoPendiente));

                //    //this.nfLimiteCredito.IndicatorIcon = Icon.Help;
                //    //this.nfLimiteCredito.IndicatorTip = "Este dato tiene un cambio pendiente de aprobación. El valor actual es de: " + String.Format("{0:F2}", float.Parse(LimiteCredito));
                //    //this.nfLimiteCredito.ShowIndicator();
   
                //    //this.btnHelp.Hidden = false;

                //    //this.btnHelp.SetTooltip(new QTipCfg()
                //    //{
                //    //    Title = "PENDIENTE DE AUTORIZACIÓN",
                //    //    Text = "Valor actual: " + String.Format("{0:F2}", float.Parse(LimiteCredito))
                //    //});
                //}
            }

            catch (Exception ex)
            {
                throw new Exception("LlenaLimiteCredito() " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LimiteCredito"></param>
        /// <param name="LimiteCreditoPendiente"></param>
        /// <param name="esVistaPorBoton"></param>
        protected void LlenaLimiteCredito(String LimiteCredito, String LimiteCreditoPendiente, Boolean esVistaPorBoton)
        {
            //Se realiza la validación para activar o no los indicadores visuales
            if (!String.IsNullOrEmpty(LimiteCreditoPendiente) && !String.IsNullOrEmpty(LimiteCredito)
           && LimiteCredito != LimiteCreditoPendiente)
            {
                this.nfLimiteCredito.Cls = esVistaPorBoton ? "chng-blue-nf-css" : "chng-red-nf-css";

                this.nfLimiteCredito.Value = esVistaPorBoton ?
                    String.Format("{0:F2}", float.Parse(LimiteCredito)) :
                    String.Format("{0:F2}", float.Parse(LimiteCreditoPendiente));

                FieldSetCuentasSoloLectura(esVistaPorBoton);

                btnValorActual.Hidden = false;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Valor Actual del panel de parámetros de la cuenta, 
        /// invocando la visualización de dicho valor de los datos en el el Grid Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValorActual_Click(object sender, EventArgs e)
        {
            ControlaCambiosLDC(this.txtHDLimiteCredito.Text, this.txtHDLimiteCreditoPendiente.Text, true);

            this.btnValorActual.Hidden = true;
            this.btnValorPendiente.Hidden = false;
        }

        /// <summary>
        /// Controla el evento Click al botón Valor Pendiente de Autorización del panel de parámetros de la cuenta,
        /// invocando la visualización de dicho valor del Límite de Crédito
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValorPendiente_Click(object sender, EventArgs e)
        {
            ControlaCambiosLDC(this.txtHDLimiteCredito.Text, this.txtHDLimiteCreditoPendiente.Text, false);

            this.btnValorActual.Hidden = false;
            this.btnValorPendiente.Hidden = true;
        }

        /// <summary>
        /// Establece los campos del FieldSet Cuentas como sólo lectura
        /// si es vista para valor actual del Límite de Crédito
        /// </summary>
        protected void FieldSetCuentasSoloLectura(Boolean esVistaPorBoton)
        {
            this.nfFechaCorte.ReadOnly = esVistaPorBoton ? true : false;

            this.nfFechaLimitePago.ReadOnly = esVistaPorBoton ? true : false;

            this.nfLimiteCredito.ReadOnly = esVistaPorBoton ? true : false;

            this.txtNombreEmbozadoTitular.ReadOnly = esVistaPorBoton ? true : false;

            this.dfVigenciaTarjeta.ReadOnly = esVistaPorBoton ? true : false;
        }


        /// <summary>
        /// Llena el GridPanel Tarjetas con los datos de la cuenta seleccionada
        /// </summary>
        /// <param name="idCuenta">ID de la cuenta</param>
        protected void LlenaGridTarjetas(int idCuenta)
        {
            try
            {
                GridTarjetas.GetStore().RemoveAll();

                GridTarjetas.GetStore().DataSource = 
                    DAOCuentasBancarias_V2.ObtieneTarjetasAdicionales(idCuenta, this.Usuario,
                                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                GridTarjetas.GetStore().DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas Adicionales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Tarjetas Adicionales", "Ocurrió un Error al Consultar las Tarjetas Adicionales").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
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
                try
                {
                    DataSet dsColonias = DAOCuentasBancarias_V2.ListaDatosPorCodigoPostal(
                                this.txtCPCliente.Text,
                                this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreColonias.DataSource = dsColonias;
                    this.StoreColonias.DataBind();

                    this.txtClaveMunicipio.Text = dsColonias.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                    this.txtMunicipioTitular.Text = dsColonias.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                    this.txtClaveEstado.Text = dsColonias.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                    this.txtEstadoTitular.Text = dsColonias.Tables[0].Rows[0]["Estado"].ToString().Trim();
                }

                catch (CAppException err)
                {
                    X.Msg.Alert("Datos del Titular", err.Mensaje()).Show();
                }

                catch (Exception)
                {
                    X.Msg.Alert("Datos del Titular", "Ocurrió un Error al Consultar los Códigos Postales").Show();
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
            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.ID_Cuenta = int.Parse(this.txtID_Cuenta.Text);
                datosPersonales.ID_MA = int.Parse(this.txtID_MA.Text);
                datosPersonales.ID_Colectiva = int.Parse(this.txtID_Colectiva.Text);

                datosPersonales.Nombre = this.txtNombreClienteTitular.Text;
                datosPersonales.ApellidoPaterno = this.txtApPaternoTitular.Text;
                datosPersonales.ApellidoMaterno = this.txtApMaternoTitular.Text;
                datosPersonales.RFC = this.txtRFCTitular.Text;

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

                LNCuentasBancarias_V2.ActualizaDatosPersonalesCuenta(datosPersonales, this.Usuario);

                X.Msg.Notify("", "Datos Personales de la Cuenta Actualizados Exitosamente").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Datos Personales de la Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Datos Personales de la Cuenta", "Ocurrió un Error al Actualizar los Datos Personales de la Cuenta").Show();
            }
        }

        // <summary>
        /// Controla el evento Click al botón Guardar del formulario de Datos, invocando la actualización
        /// de los datos del cliente en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<String, String> huboCambios = ValidaCambios();

                Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                if (huboCambios.Count > 0)
                {
                    foreach (KeyValuePair<string, string> cambio in huboCambios)
                    {
                        switch (cambio.Key)
                        {
                            case "Corte":
                                LNCuentasBancarias_V2.ActualizaParametrosCuenta(int.Parse(this.txtID_Cuenta.Text),
                                    int.Parse(this.txtID_MA.Text), "@DiaFechaCorte", this.nfFechaCorte.Text,
                                    this.Usuario);
                                break;
                            case "LimitePago":
                                LNCuentasBancarias_V2.ActualizaParametrosCuenta(int.Parse(this.txtID_Cuenta.Text),
                                    int.Parse(this.txtID_MA.Text), "@DiaFechaLimPago", this.nfFechaLimitePago.Text, 
                                    this.Usuario);
                                break;
                            case "LimiteCredito":
                                //Si el usuario es Ejecutor Y Autorizador, 
                                //realiza el ajuste directamente
                                if (esEjecutor && esAutorizador)
                                {
                                    AutorizaAjusteLimiteCredito();
                                }
                                else
                                {
                                    LNCuentasBancarias_V2.SolicitaCambioLimiteCredito(int.Parse(this.txtID_Cuenta.Text),
                                        int.Parse(this.txtID_MA.Text), this.nfLimiteCredito.Text,
                                        Path.GetFileName(Request.Url.AbsolutePath), this.Usuario,
                                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                                }
                                break;
                            case "NombreEmbozo":
                                LNCuentasBancarias_V2.ActualizaParametrosMA(int.Parse(this.txtID_MA.Text), cambio.Key,
                                    this.txtNombreEmbozadoTitular.Text, this.Usuario);
                                break;
                            case "VigenciaTarjeta":
                                LNCuentasBancarias_V2.ActualizaParametrosMA(int.Parse(this.txtID_MA.Text), cambio.Key,
                                    this.dfVigenciaTarjeta.Text, this.Usuario);
                                break;
                            default:
                                break;
                        }
                    }

                    LlenaFormPanelTarjetas(int.Parse(this.txtID_Cuenta.Text), int.Parse(this.txtID_MA.Text));

                    X.Msg.Notify("", "Parámetros de la Cuenta Actualizados<br /><br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros de la Cuenta", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Parámetros de la Cuenta", "Ocurrió un Error al Actualizar los Parámetros de la Cuenta").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<String, String> ValidaCambios()
        {
            try
            {
                CuentaTDC laCuentaAntes = new CuentaTDC();
                CuentaTDC laCuentaDespues = new CuentaTDC();

                laCuentaAntes.FechaCorte = int.Parse(
                    String.IsNullOrEmpty(txtHDFechaCorte.Text) ? "0" : txtHDFechaCorte.Text);
                laCuentaAntes.FechaLimitePago = int.Parse(
                    String.IsNullOrEmpty(txtHDFechaLimitePago.Text) ? "0" : txtHDFechaLimitePago.Text);
                laCuentaAntes.LimiteCredito = Convert.ToDecimal(
                    String.IsNullOrEmpty(txtHDLimiteCredito.Text) ? "0" : txtHDLimiteCredito.Text);
                laCuentaAntes.NombreEmbozado = txtHDEmbozado.Text;
                laCuentaAntes.VigenciaTarjeta = Convert.ToDateTime(this.dfHDVigenciaTarjeta.Text);

                laCuentaDespues.ID_Cuenta = int.Parse(this.txtID_Cuenta.Text);
                laCuentaDespues.ID_MA = int.Parse(this.txtID_MA.Text);
                laCuentaDespues.FechaCorte = int.Parse(
                    String.IsNullOrEmpty(nfFechaCorte.Text) ? "0" : nfFechaCorte.Text);
                laCuentaDespues.FechaLimitePago = int.Parse(
                    String.IsNullOrEmpty(nfFechaLimitePago.Text) ? "0" : nfFechaLimitePago.Text);
                laCuentaDespues.LimiteCredito = Convert.ToDecimal(
                    String.IsNullOrEmpty(nfLimiteCredito.Text) ? "0" : nfLimiteCredito.Text);
                laCuentaDespues.NombreEmbozado = this.txtNombreEmbozadoTitular.Text;
                laCuentaDespues.VigenciaTarjeta = Convert.ToDateTime(this.dfVigenciaTarjeta.Text);

                return LNCuentasBancarias_V2.ValidaDatosCuenta (laCuentaAntes, laCuentaDespues, this.Usuario);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Autorizar Cambio panel Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento directo que se ejecutó</param>
        protected void btnAutorizarCambios_Click(object sender, DirectEventArgs e)
        {
            try
            {
                AutorizaAjusteLimiteCredito();

                X.Msg.Notify("Tarjetas", "Autorización de Cambio de Límite de Crédito<br /><br />  <b> E X I T O S A </b> <br />  <br /> ").Show();
            }

            catch (Exception ex)
            {
                DALAdministracion.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Tarjetas", "Ocurrió un Error en la Autorización del Cambio en el Límite de Crédito").Show();
            }
        }

        /// <summary>
        /// Ejecuta las llamadas a los métodos para el ajuste al límite de crédito y llena nuevamente
        /// el panel de tarjetas con la información ajustada
        /// </summary>
        protected void AutorizaAjusteLimiteCredito()
        {
            try
            {
                //Ejecuta el evento manual para el cambio en el límite de crédito
                LNCuentasBancarias_V2.RegistraEvManual_AjustaLimiteCredito(
                    int.Parse(this.txtID_Colectiva.Text), this.txtTarjetaTitular.Text,
                    this.txtHDLimiteCredito.Text, this.nfLimiteCredito.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                //Registra el cambio del límite de crédito en base de datos
                LNCuentasBancarias_V2.RegistraCambioLimiteCredito(
                    Int64.Parse(this.txtID_Cuenta.Text), this.nfLimiteCredito.Text, this.Usuario);

                LlenaFormPanelTarjetas(int.Parse(this.txtID_Cuenta.Text), int.Parse(this.txtID_MA.Text));
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}