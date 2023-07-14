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

namespace Administracion
{
    public partial class CuentasBancarias_V1 : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
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

                DataSet dsResultados = LNCuentasBancarias.ConsultaCuentas(datosPersonales, this.Usuario,
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
                X.Msg.Alert("Consulta de Clientes", err.Mensaje()).Show();
            }

            catch (Exception)
            {
                X.Msg.Alert("Consulta de Clientes", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
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

                 DataSet dsDatos = DAOCuentasBancarias.ObtieneDatosPersonalesCuenta(
                    idCuenta, idTarjeta, idColectiva, this.Usuario,
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
                FormPanelCuenta.Reset();

                DataSet dsDatosCuenta = DAOCuentasBancarias.ObtieneDatosCuenta(
                                   idCuenta, idTarjeta, this.Usuario,
                                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsDatosCuenta.Tables[0].Rows.Count > 0)
                {
                    this.nfFechaCorte.Value = dsDatosCuenta.Tables[0].Rows[0]["FechaCorte"].ToString().Trim();
                    this.txtHDFechaCorte.Text = dsDatosCuenta.Tables[0].Rows[0]["FechaCorte"].ToString().Trim();

                    this.nfFechaLimitePago.Value = dsDatosCuenta.Tables[0].Rows[0]["FechaLimitePago"].ToString().Trim();
                    this.txtHDFechaLimitePago.Text = dsDatosCuenta.Tables[0].Rows[0]["FechaLimitePago"].ToString().Trim();

                    if (String.IsNullOrEmpty(dsDatosCuenta.Tables[0].Rows[0]["LimiteCredito"].ToString().Trim()))
                    {
                        this.nfLimiteCredito.Value = "";
                    }
                    this.nfLimiteCredito.Value = String.Format("{0:F2}", float.Parse(
                            dsDatosCuenta.Tables[0].Rows[0]["LimiteCredito"].ToString().Trim()));
                    this.txtHDLimiteCredito.Value = String.Format("{0:F2}", float.Parse(
                            dsDatosCuenta.Tables[0].Rows[0]["LimiteCredito"].ToString().Trim()));
                    
                    this.txtNombreEmbozadoTitular.Text = dsDatosCuenta.Tables[0].Rows[0]["NombreEmbozo"].ToString().Trim();
                    this.txtHDEmbozado.Text = dsDatosCuenta.Tables[0].Rows[0]["NombreEmbozo"].ToString().Trim();

                    this.dfVigenciaTarjeta.Value = Convert.ToDateTime(dsDatosCuenta.Tables[0].Rows[0]["VigenciaTarjeta"].ToString().Trim());
                    this.dfHDVigenciaTarjeta.Value = Convert.ToDateTime(dsDatosCuenta.Tables[0].Rows[0]["VigenciaTarjeta"].ToString().Trim());
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros de la Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Parámetros de la Cuenta", "Ocurrió un Error al Consultar los Parámetros de la Cuenta").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
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
                    DAOCuentasBancarias.ObtieneTarjetasAdicionales(idCuenta, this.Usuario,
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
                    DataSet dsColonias = DAOCuentasBancarias.ListaDatosPorCodigoPostal(
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

                LNCuentasBancarias.ActualizaDatosPersonalesCuenta(datosPersonales, this.Usuario);

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
                CuentaTDC laCuentaAntes = new CuentaTDC();
                CuentaTDC laCuentaDespues = new CuentaTDC();

                laCuentaAntes.FechaCorte = int.Parse(this.txtHDFechaCorte.Text);
                laCuentaAntes.FechaLimitePago = int.Parse(this.txtHDFechaLimitePago.Text);
                laCuentaAntes.LimiteCredito = Convert.ToDecimal(this.txtHDLimiteCredito.Text);
                laCuentaAntes.NombreEmbozado = this.txtHDEmbozado.Text;
                laCuentaAntes.VigenciaTarjeta = Convert.ToDateTime(this.dfHDVigenciaTarjeta.Text);

                laCuentaDespues.ID_Cuenta = int.Parse(this.txtID_Cuenta.Text);
                laCuentaDespues.ID_MA = int.Parse(this.txtID_MA.Text);
                laCuentaDespues.FechaCorte = int.Parse(this.nfFechaCorte.Text);
                laCuentaDespues.FechaLimitePago = int.Parse(this.nfFechaLimitePago.Text);
                laCuentaDespues.LimiteCredito = Convert.ToDecimal(this.nfLimiteCredito.Text);
                laCuentaDespues.NombreEmbozado = this.txtNombreEmbozadoTitular.Text;
                laCuentaDespues.VigenciaTarjeta = Convert.ToDateTime(this.dfVigenciaTarjeta.Text);

                Dictionary<String, String> huboCambios = LNCuentasBancarias.ValidaDatosCuenta
                                                            (laCuentaAntes, laCuentaDespues, this.Usuario);
                if (huboCambios.Count > 0)
                {
                    foreach (KeyValuePair<string, string> cambio in huboCambios)
                    {
                        switch (cambio.Key)
                        {
                            case "Corte":
                                LNCuentasBancarias.ActualizaParametrosCuenta(laCuentaDespues.ID_Cuenta,
                                    laCuentaDespues.ID_MA, "@DiaFechaCorte", laCuentaDespues.FechaCorte.ToString(), 
                                    this.Usuario);
                                break;
                            case "LimitePago":
                                LNCuentasBancarias.ActualizaParametrosCuenta(laCuentaDespues.ID_Cuenta,
                                    laCuentaDespues.ID_MA, "@DiaFechaLimPago", laCuentaDespues.FechaLimitePago.ToString(), 
                                    this.Usuario);
                                break;
                            case "LimiteCredito":
                                LNCuentasBancarias.RegistraEvManual_AjustaLimiteCredito(
                                    int.Parse(this.txtID_Colectiva.Text), this.txtTarjetaTitular.Text,
                                    this.txtHDLimiteCredito.Text, this.nfLimiteCredito.Text, this.Usuario,
                                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                                break;
                            case "NombreEmbozo":
                                LNCuentasBancarias.ActualizaParametrosProducto(laCuentaDespues.ID_MA, cambio.Key,
                                    laCuentaDespues.NombreEmbozado.ToString(), this.Usuario);
                                break;
                            case "VigenciaTarjeta":
                                LNCuentasBancarias.ActualizaParametrosProducto(laCuentaDespues.ID_MA, cambio.Key,
                                    laCuentaDespues.VigenciaTarjeta.ToString(), this.Usuario);
                                break;
                            default:
                                break;
                        }
                    }

                    X.Msg.Notify("", "Parámetros de la Cuenta Actualizados Exitosamente").Show();
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
    }
}