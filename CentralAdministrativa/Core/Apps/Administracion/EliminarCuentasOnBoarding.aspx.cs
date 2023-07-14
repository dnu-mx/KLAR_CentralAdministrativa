using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.LogicaNegocio;
using DALCentralAplicaciones.Utilidades;
using DALEventos.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using Log_PCI.Utilidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using Utilerias;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace Administracion
{
    public partial class EliminarCuentasOnBoarding : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER Parabilia Eliminar Cuentas OnBoarding
        private LogHeader LH_ParabElimCtasOnB = new LogHeader();
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

        public class TarjetaRow
        {
            public int ID_MA { get; set; }
            public String ClaveMA { get; set; }
            public String TipoTarjeta { get; set; }
        }

        /// <summary>
        /// Realiza y controla la carga de la página Administración de Cuentas OnBoarding
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabElimCtasOnB.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabElimCtasOnB.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabElimCtasOnB.User = this.Usuario.ClaveUsuario;
            LH_ParabElimCtasOnB.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabElimCtasOnB);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA EliminarCuentasOnBoarding Page_Load()");

                if (!IsPostBack)
                {
                    this.dfFechaNac.MaxDate = DateTime.Today;

                    log.Info("INICIA ListaCatalogoGenero()");
                    this.StoreGenero.DataSource = 
                        DAOTarjetaCuenta.ListaCatalogoGenero(LH_ParabElimCtasOnB);
                    log.Info("TERMINA ListaCatalogoGenero()");
                    this.StoreGenero.DataBind();

                    log.Info("INICIA ListaCatalogoEstadosMexico()");
                    this.StoreEstadosRepMex.DataSource =
                        DAOTarjetaCuenta.ListaCatalogoEstadosMexico(LH_ParabElimCtasOnB);
                    log.Info("TERMINA ListaCatalogoEstadosMexico()");
                    this.StoreEstadosRepMex.DataBind();

                    EstableceComboPaises();
                }

                log.Info("TERMINA EliminarCuentasOnBoarding Page_Load()");
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
            LogPCI unLog = new LogPCI(LH_ParabElimCtasOnB);

            unLog.Info("INICIA ListaCatalogoPaises()");
            DataTable dtPaises = DAOTarjetaCuenta.ListaCatalogoPaises(LH_ParabElimCtasOnB);
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
            LogPCI pCI = new LogPCI(LH_ParabElimCtasOnB);

            try
            {
                DatosPersonalesCuenta datosPersonales = new DatosPersonalesCuenta();

                datosPersonales.CuentaInterna = this.txtNumCuenta.Text;
                datosPersonales.TarjetaTitular = this.txtNumTarjeta.Text;

                pCI.Info("INICIA ObtieneCuentasPorFiltroOnB_2()");
                DataSet dsResultados = DAOTarjetaCuenta.ObtieneCuentasPorFiltroOnB_2(datosPersonales, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabElimCtasOnB);
                pCI.Info("TERMINA ObtieneCuentasPorFiltroOnB_2()");

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Cuentas", "No existen coincidencias con la búsqueda solicitada").Show();
                    return;
                }

                DataTable dt = Tarjetas.EnmascaraTablaConTarjetas(dsResultados.Tables[0], "Tarjeta", 
                    "Enmascara", LH_ParabElimCtasOnB);

                this.StoreResultados.DataSource = dsResultados;
                this.StoreResultados.DataBind();
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
                StoreResultados.RemoveAll();
                this.EastPanel.Title = "_";
            }

            this.FormPanelTitular.Reset();

            StoreTarjetas.RemoveAll();
            StoreMediosAcceso.RemoveAll();
            StoreCuentas.RemoveAll();

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
                string numTarjeta = string.Empty;
                string tarjetaTitulo = string.Empty;
                string ClaveColectivaPadre = "";

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
                        case "ClaveMA": numTarjeta = column.Value; break;
                        case "Tarjeta": tarjetaTitulo = column.Value; break;
                        case "IdCadenaComercial": IdCadena = int.Parse(column.Value); break;
                        case "ClaveColectivaPadre": ClaveColectivaPadre = column.Value; break;
                        default:
                            break;
                    }
                }

                limpiaBusquedaPrevia(false);

                this.hdnIdCuentaLDC.Value = IdCuentaCLDC;
                this.hdnIdMA.Value = IdMA;
                this.hdnTarjeta.Value = numTarjeta;
                this.hdnIdColectiva.Value = IdColectiva;
                this.hdnIdCadena.Value = IdCadena;
                this.hdnClaveColectivaPadre.Value = ClaveColectivaPadre;

                LlenaFieldSetDatosTitular(IdCuentaCLDC, IdMA, IdColectiva);
                LlenaGridTarjetasYMedios(numTarjeta);
                LlenaGridCuentas(numTarjeta);

                this.EastPanel.Title = "Tarjeta " + tarjetaTitulo;
                this.EastPanel.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabElimCtasOnB);
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
            LogPCI unLog = new LogPCI(LH_ParabElimCtasOnB);

            try
            {
                FormPanelTitular.Reset();

                unLog.Info("INICIA ObtieneDatosPersonalesCtaOnB()");
                DatosPersonalesCuenta losDatos = DAOTarjetaCuenta.ObtieneDatosPersonalesCtaOnB(
                   idTarjeta, idColectiva, this.Usuario,
                   Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                   LH_ParabElimCtasOnB);
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
                LogPCI logPCI = new LogPCI(LH_ParabElimCtasOnB);

                try
                {
                    logPCI.Info("INICIA ListaDatosPorCodigoPostal_V2()");
                    DataTable dtColonias = 
                        DAOTarjetaCuenta.ListaDatosPorCodigoPostal_V2(this.txtCPCliente.Text, LH_ParabElimCtasOnB);
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
        /// Llena el GridPanel Tarjetas con los datos de la familia de la tarjeta seleccionada
        /// </summary>
        /// <param name="NumeroTarjeta">Número de tarjeta</param>
        protected void LlenaGridTarjetasYMedios(string NumeroTarjeta)
        {
            LogPCI log = new LogPCI(LH_ParabElimCtasOnB);

            try
            {
                this.StoreTarjetas.RemoveAll();
                this.StoreMediosAcceso.RemoveAll();

                DataTable dt = new DataTable();

                log.Info("INICIA web_CA_ObtieneMediosAcceso_Tarjetas()");
                DataSet dsTarjetas = DAOTarjetaCuenta.web_CA_ObtieneMediosAcceso(NumeroTarjeta, 1, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabElimCtasOnB);
                log.Info("TERMINA web_CA_ObtieneMediosAcceso_Tarjetas()");

                if (dsTarjetas.Tables[0].Rows.Count > 0)
                {
                    dt = Tarjetas.EnmascaraTablaConTarjetas(dsTarjetas.Tables[0], "ClaveMA", "Enmascara", LH_ParabElimCtasOnB);
                }

                this.StoreTarjetas.DataSource = dt;
                this.StoreTarjetas.DataBind();

                log.Info("INICIA web_CA_ObtieneMediosAcceso()");
                this.StoreMediosAcceso.DataSource =
                    DAOTarjetaCuenta.web_CA_ObtieneMediosAcceso(NumeroTarjeta, 0, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabElimCtasOnB);
                log.Info("TERMINA web_CA_ObtieneMediosAcceso()");
                
                this.StoreMediosAcceso.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Tarjetas y Medios Acceso", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Tarjetas y Medios Acceso", "Ocurrió un error al obtener las Tarjetas y/o Medios de Acceso").Show();
            }
        }

        /// <summary>
        /// Llena el GridPanel TiposMA con los datos de base de datos
        /// </summary>
        /// <param name="NumeroTarjeta">Número de tarjeta</param>
        protected void LlenaGridCuentas(string NumeroTarjeta)
        {
            LogPCI unLog = new LogPCI(LH_ParabElimCtasOnB);

            try
            {
                this.StoreCuentas.RemoveAll();

                unLog.Info("INICIA ObtieneCuentas()");
                this.StoreCuentas.DataSource =
                    DAOTarjetaCuenta.ObtieneCuentas(NumeroTarjeta, LH_ParabElimCtasOnB);
                unLog.Info("TERMINA ObtieneCuentas()");
                this.StoreCuentas.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Medios de Acceso", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Cuentas", "Ocurrió un error al obtener las Cuentas").Show();
            }
        }

        [DirectMethod(Namespace = "AdminObjeto")]
        public void Elimina(string valor)
        {
            //Thread.Sleep(5000);
            //X.Mask.Show(new MaskConfig { Msg = "Procesando..." });

            TarjetaRow rowTarjeta = new TarjetaRow();
            rowTarjeta = JSON.Deserialize<TarjetaRow>(valor);

            LogPCI log = new LogPCI(LH_ParabElimCtasOnB);

            try
            {
                string wsLoginResp, wsActResp = null;
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                _headers.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_URL").Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Usr").Valor;

                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Pwd").Valor;

                log.Info("INICIA Parabilium.Login()");
                wsLoginResp = Parabilium.Login(_headers, _body, LH_ParabElimCtasOnB);
                log.Info("TERMINA Parabilium.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headers.Token = wsLoginResp;
                _headers.Credenciales = Cifrado.Base64Encode(_body.NombreUsuario + ":" + _body.Password);

                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.EliminarTarjetaBody eliminar = new Parametros.EliminarTarjetaBody();
                eliminar.IDSolicitud = "";
                eliminar.Tarjeta = rowTarjeta.ClaveMA;
                eliminar.MedioAcceso = "";
                eliminar.TipoMedioAcceso = "";
                eliminar.MotivoCancelacion = "03";

                Parametros.EliminarTarjetaBody maskedDelete = new Parametros.EliminarTarjetaBody();
                maskedDelete.IDSolicitud = "";
                maskedDelete.Tarjeta = MaskSensitiveData.cardNumber(rowTarjeta.ClaveMA);
                maskedDelete.MedioAcceso = "";
                maskedDelete.TipoMedioAcceso = "";
                maskedDelete.MotivoCancelacion = "03";
                //

                log.Info("INICIA Parabilium.EliminarTarjeta()");
                wsActResp = Parabilium.EliminarTarjeta(_headers, eliminar, maskedDelete, LH_ParabElimCtasOnB);
                log.Info("TERMINA Parabilium.EliminarTarjeta()");

                if (!wsActResp.Contains("OK"))
                {
                    throw new CAppException(8006, wsActResp);
                }

                LNInfoOnBoarding.RegistraEnBitacora("pantalla_EliminarCuentas_EliminarTarjeta", "MediosAcceso", "ID_EstatusMA", rowTarjeta.ID_MA.ToString(),
                    eliminar.MotivoCancelacion, "Eliminación Tarjeta: " + eliminar.Tarjeta, this.Usuario, LH_ParabElimCtasOnB);

                X.Msg.Notify("", "Tarjeta Eliminada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridTarjetasYMedios(rowTarjeta.ClaveMA);
                Thread.Sleep(200);
                X.Mask.Hide();
            }
            catch (CAppException err)
            {
                Thread.Sleep(200);
                X.Mask.Hide();
                X.Msg.Alert("Eliminación", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                X.Mask.Hide();
                log.ErrorException(ex);
                X.Msg.Alert("Eliminación", "Ocurrió un error al eliminar la Tarjeta").Show();
            }
        }

        [DirectMethod(Namespace = "AdminObjeto")]
        public void CancelaCuentas()
        {
            LogPCI log = new LogPCI(LH_ParabElimCtasOnB);

            try
            {
                string wsLoginResp, wsActResp = null;
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                _headers.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_URL").Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Usr").Valor;

                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Pwd").Valor;

                log.Info("INICIA Parabilium.Login()");
                wsLoginResp = Parabilium.Login(_headers, _body, LH_ParabElimCtasOnB);
                log.Info("TERMINA Parabilium.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headers.Token = wsLoginResp;
                _headers.Credenciales = Cifrado.Base64Encode(_body.NombreUsuario + ":" + _body.Password);

                //Se genera por duplicado el cuerpo del método, uno para log seguro
                Parametros.CancelarCuentasBody cancelar = new Parametros.CancelarCuentasBody();
                cancelar.Empresa = this.hdnClaveColectivaPadre.Text;
                cancelar.Tarjeta = this.hdnTarjeta.Text;
                cancelar.MedioAcceso = "";
                cancelar.TipoMedioAcceso = "";

                Parametros.CancelarCuentasBody maskedCancel = new Parametros.CancelarCuentasBody();
                maskedCancel.Empresa = this.hdnClaveColectivaPadre.Text;
                maskedCancel.Tarjeta = MaskSensitiveData.cardNumber(this.hdnTarjeta.Text);
                maskedCancel.MedioAcceso = "";
                maskedCancel.TipoMedioAcceso = "";
                //

                log.Info("INICIA Parabilium.CancelarCuentas()");
                wsActResp = Parabilium.CancelarCuentas(_headers, cancelar, maskedCancel, LH_ParabElimCtasOnB);
                log.Info("TERMINA Parabilium.CancelarCuentas()");

                if (!wsActResp.Contains("OK"))
                {
                    throw new CAppException(8006, wsActResp);
                }

                LNInfoOnBoarding.RegistraEnBitacora("pantalla_EliminarCuentas_CancelarCuentas", "Cuentas", "ID_EstatusCuenta", "0",
                    "CER", "Cancelación Cuentas de Tarjeta: " + cancelar.Tarjeta, this.Usuario, LH_ParabElimCtasOnB);

                X.Msg.Notify("", "Cuentas Canceladas" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridTarjetasYMedios(this.hdnTarjeta.Text);
                LlenaGridCuentas(this.hdnTarjeta.Text);

                Thread.Sleep(200);
                X.Mask.Hide();
            }
            catch (CAppException err)
            {
                Thread.Sleep(200);
                X.Mask.Hide();
                X.Msg.Alert("Cancelación", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                X.Mask.Hide();
                log.ErrorException(ex);
                X.Msg.Alert("Cancelación", "Ocurrió un error al cancelar las Cuentas").Show();
            }
        }
    }
}