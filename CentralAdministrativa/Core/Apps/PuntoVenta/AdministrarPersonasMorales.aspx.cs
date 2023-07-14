using DALAdministracion.BaseDatos;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace TpvWeb
{
    public partial class AdministrarPersonasMorales : PaginaBaseCAPP
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        //LOG HEADER Parabilia Administra Personas Morales
        private LogHeader LH_ParabAdminPersMor = new LogHeader();

        #endregion

        /// <summary>
        /// Clase entidad para el combo predictivo de País
        /// </summary>
        public class PaisComboPredictivo
        {
            public int ID_Pais { get; set; }
            public String Clave { get; set; }
            public String Descripcion { get; set; }
        }

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Personas Morales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminPersMor.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminPersMor.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminPersMor.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminPersMor.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminPersMor);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarPersonasMorales Page_Load()");

                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("EsAutorizador", false);
                    HttpContext.Current.Session.Add("EsEjecutor", false);

                    EstablecePermisosPorRol();

                    EstableceControlesPorRol();

                    LlenaCombos();
                }

                log.Info("TERMINA AdministrarPersonasMorales Page_Load()");
            }

            catch (CAppException)
            {
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
        /// Establece en variables de sesión los roles auditables para el control de
        /// flujos en la página
        /// </summary>
        protected void EstablecePermisosPorRol()
        {
            this.Usuario.Roles.Sort();

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
        /// Oculta el estatus habilitado/deshabilitado u oculto/visible de los
        /// controles por rol Ejecutor/Autorizador
        /// </summary>
        protected void EstableceControlesPorRol()
        {
            try
            {
                Boolean esAutorizador = Boolean.Parse(HttpContext.Current.Session["EsAutorizador"].ToString());
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                BotonesOcultos();

                string estatus = this.hdnEstatus.Value == null ? "" : this.hdnEstatus.Value.ToString();

                if (esAutorizador && esEjecutor)
                {
                    this.btnNuevaPM.Hidden = false;
                    this.cBoxClienteNPM.Hidden = false;

                    this.btnNuevaPM.Disabled = false;
                    this.cBoxClienteNPM.Disabled = false;

                    if (estatus == "EnProceso")
                    {
                        this.btnGenerarCta.Hidden = false;
                        this.btnGenerarCta.Disabled = false;

                        this.btnInfoGral.Show();
                        this.btnDomicilio.Show();
                        this.btnCuestionario.Show();
                        this.ToolbarDocs.Show();
                    }
                    else if (estatus == "EnRevision" || estatus == "Autorizado")
                    {
                        this.btnGenerarCta.Hidden = false;
                        this.btnGenerarCta.Disabled = false;

                        if (estatus == "EnRevision")
                        {
                            this.btnRechazar.Hidden = false;
                            this.btnRechazar.Disabled = false;
                        }
                    }
                }
                else if (esAutorizador)
                {
                    if (estatus == "EnRevision" || estatus == "Autorizado")
                    {
                        this.btnGenerarCta.Hidden = false;
                        this.btnGenerarCta.Disabled = false;

                        if (estatus == "EnRevision")
                        {
                            this.btnRechazar.Hidden = false;
                            this.btnRechazar.Disabled = false;
                        }
                    }
                }
                else if (esEjecutor)
                {
                    this.btnNuevaPM.Hidden = false;
                    this.cBoxClienteNPM.Hidden = false;

                    this.btnNuevaPM.Disabled = false;
                    this.cBoxClienteNPM.Disabled = false;

                    if (estatus == "EnProceso")
                    {
                        this.btnARevision.Hidden = false;

                        this.btnInfoGral.Show();
                        this.btnDomicilio.Show();
                        this.btnCuestionario.Show();
                        this.ToolbarDocs.Show();
                    }
                }
            }
            catch(Exception ex)
            {
                LogPCI log = new LogPCI(LH_ParabAdminPersMor);
                log.Error("EstableceControlesPorRol()");
                throw ex;
            }
        }

        /// <summary>
        /// Oculta los botones para control de Ejecutor/Autorizador
        /// </summary>
        protected void BotonesOcultos()
        {
            this.btnGenerarCta.Hidden = true;
            this.btnGenerarCta.Disabled = true;
            this.btnNuevaPM.Hidden = true;
            this.btnNuevaPM.Disabled = true;
            this.cBoxClienteNPM.Hidden = true;
            this.btnARevision.Hidden = true;
            this.btnRechazar.Hidden = true;
            this.btnRechazar.Disabled = true;

            this.btnInfoGral.Hide();
            this.btnDomicilio.Hide();
            this.btnCuestionario.Hide();
            this.ToolbarDocs.Hide();
        }

        /// <summary>
        /// Esteblece los valores de los controles tipo combo con la información de catálogos
        /// en base de datos
        /// </summary>
        protected void LlenaCombos()
        {
            LogPCI log = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                log.Info("INICIA ListaColectivasSubEmisor()");
                DataTable dtColectivas =
                           DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                           Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                           LH_ParabAdminPersMor);
                log.Info("TERMINA ListaColectivasSubEmisor()");

                List<ColectivaComboPredictivo> clientList = new List<ColectivaComboPredictivo>();

                foreach (DataRow cliente in dtColectivas.Rows)
                {
                    var grupoCombo = new ColectivaComboPredictivo()
                    {
                        ID_Colectiva = Convert.ToInt64(cliente["ID_Colectiva"].ToString()),
                        ClaveColectiva = cliente["ClaveColectiva"].ToString(),
                        NombreORazonSocial = cliente["NombreORazonSocial"].ToString()
                    };
                    clientList.Add(grupoCombo);
                }

                this.StoreColectivas.DataSource = clientList;
                this.StoreColectivas.DataBind();

                log.Info("INICIA ListaEstatusPersonaMoral()");
                this.StoreEstatusPM.DataSource =
                    DAOAdministrarPersonasMorales.ListaEstatusPersonaMoral(LH_ParabAdminPersMor);
                this.StoreEstatusPM.DataBind();
                log.Info("INICIA ListaEstatusPersonaMoral()");


                log.Info("INICIA ListaCatalogoPaises()");
                DataTable dtPaises = DAOTarjetaCuenta.ListaCatalogoPaises(LH_ParabAdminPersMor);
                log.Info("INICIA ListaCatalogoPaises()");

                List<PaisComboPredictivo> listaPaises = new List<PaisComboPredictivo>();

                foreach (DataRow drCol in dtPaises.Rows)
                {
                    var paisCombo = new PaisComboPredictivo()
                    {
                        ID_Pais = int.Parse(drCol["ID_Pais"].ToString()),
                        Clave = drCol["Clave"].ToString(),
                        Descripcion = drCol["Descripcion"].ToString()
                    };
                    listaPaises.Add(paisCombo);
                }

                this.StorePaises.DataSource = listaPaises;
                this.StorePaises.DataBind();
            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                LimpiaSeleccionPrevia(true);

                this.cBoxClientes.Reset();
                this.cBoxEstatus.Reset();
                this.txtRazSoc.Reset();
                this.txt_CLABE.Reset();

                this.StorePersonas.RemoveAll();

                this.hdnEstatus.SetValue("");
                this.hdnIdPM.SetValue("");

                EstableceControlesPorRol();
            }
            catch(Exception ex)
            {
                LogPCI logPCI = new LogPCI(LH_ParabAdminPersMor);
                logPCI.ErrorException(ex);
                X.Msg.Alert("Limpiar", "Ocurrió un error al restablecer los controles").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar de la pestaña Información General
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaPM_Click(object sender, EventArgs e)
        {
            try
            {
                LimpiaSeleccionPrevia(false);

                this.cBoxClientes.Reset();
                this.cBoxEstatus.Reset();
                this.txtRazSoc.Reset();
                this.txt_CLABE.Reset();

                this.StorePersonas.RemoveAll();

                this.hdnEstatus.Value = "EnProceso";

                EstableceControlesPorRol();
            }
            catch (Exception ex)
            {
                LogPCI logPCI = new LogPCI(LH_ParabAdminPersMor);
                logPCI.ErrorException(ex);
                X.Msg.Alert("Nueva Persona Moral", "Ocurrió un error al restablecer los controles").Show();
            }
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una persona moral en el grid de resultados
        /// </summary>
        /// <param name="deshabilita">Bandera para indicar si los páneles se deshabilitan</param>
        protected void LimpiaSeleccionPrevia(bool deshabilita)
        {
            this.PanelCentral.Title = "-";

            LimpiaFormPanelInfoGral(deshabilita);

            LimpiaFormPanelDirecciones(deshabilita);

            LimpiaFormPanelCuestionario(deshabilita);

            this.FormPanelDocumentos.Disabled = deshabilita;
            this.StoreDocumentos.RemoveAll();

            this.FormPanelInfoGral.Show();
        }

        /// <summary>
        /// Restablece los controles del panel de información general al estatus de carga inicial
        /// </summary>
        /// <param name="panelDeshabilitado">Bandera para indicar si el panel queda o no deshabilitado</param>
        protected void LimpiaFormPanelInfoGral(bool panelDeshabilitado)
        {
            this.FormPanelInfoGral.Disabled = panelDeshabilitado;

            if (panelDeshabilitado)
            {
                this.cBoxClienteNPM.Reset();
            }

            this.txtEstatus.Reset();
            this.txtRazonSocial.Reset();
            this.txtCLABE.Reset();
            this.txtRFC.Reset();
            this.txtTelefono.Reset();
            this.txtCorreo.Reset();
            this.txtGiro.Reset();
            this.txtRepLegal.Reset();
            this.txtCentroCostos.Reset();
        }

        /// <summary>
        /// Restablece los controles del panel de direcciones a su estatus de carga inicial
        /// </summary>
        /// <param name="panelDeshabilitado">Bandera para indicar si el panel queda o no deshabilitado</param>
        protected void LimpiaFormPanelDirecciones(bool panelDeshabilitado)
        {
            this.FormPanelDomicilio.Disabled = panelDeshabilitado;

            this.txtCalle.Clear();
            this.txtNumExterior.Clear();
            this.txtNumInterior.Clear();
            this.txtEntreCalles.Clear();
            this.txtReferencias.Clear();
            this.txtCodigoPostal.Clear();
            this.cBoxColonia.Clear();
            this.txtMunicipio.Clear();
            this.txtCiudad.Clear();
            this.cBoxPais.Clear();
            this.txtLatitud.Clear();
            this.txtLongitud.Clear();
        }

        /// <summary>
        /// Restablece los controles del panel de cuestionario al estatus de carga inicial
        /// </summary>
        /// <param name="panelDeshabilitado">Bandera para indicar si el panel queda o no deshabilitado</param>
        protected void LimpiaFormPanelCuestionario(bool panelDeshabilitado)
        {
            this.FormPanelCuestionario.Disabled = panelDeshabilitado;

            this.cBoxP1.Reset();
            this.cBoxP2.Reset();
            this.cBoxP3.Reset();
            this.txtP4.Reset();
            this.txtP5.Reset();
            this.txtP6.Reset();
            this.txtP7.Reset();
            this.txtP8.Reset();
            this.txtP9.Reset();
            this.txtP10.Reset();
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar de la pestaña Información General
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnInfoGral_Click(object sender, EventArgs e)
        {
            EstableceDatosPersonaMoral();
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar de la pestaña de Domicilio
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnDomicilio_Click(object sender, EventArgs e)
        {
            EstableceDatosPersonaMoral();
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar de la pestaña de Cuestionario
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnCuestionario_Click(object sender, EventArgs e)
        {
            EstableceDatosPersonaMoral();
        }

        /// <summary>
        /// Establece un objeto de tipo PersonaMoral con todos los datos capturados en los
        /// formularios, solicitando la actualización de los mismos en base de datos
        /// </summary>
        protected void EstableceDatosPersonaMoral()
        {
            LogPCI log = new LogPCI(LH_ParabAdminPersMor);
            log.Info("EstableceDatosPersonaMoral()");

            try
            {
                PersonaMoral laPersonaMoral = new PersonaMoral();

                laPersonaMoral.ID_PersonaMoral =
                    Convert.ToInt16(this.hdnNew.Value) == 1 ? -1 : Convert.ToInt64(this.hdnIdPM.Value);
                laPersonaMoral.ClaveEmpresa = this.hdnClaveCliente.Value.ToString();
                laPersonaMoral.RazonSocial = this.txtRazonSocial.Text;
                laPersonaMoral.CLABE = this.txtCLABE.Text;
                laPersonaMoral.RFC = this.txtRFC.Text;
                laPersonaMoral.Telefono = this.txtTelefono.Text;
                
                //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
                if (!String.IsNullOrEmpty(this.txtCorreo.Text))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(this.txtCorreo.Text);

                    if (!matchExpression.Success)
                    {
                        X.Msg.Alert("Actualización de Datos", "El Correo Electrónico que ingresaste no es una dirección válida.").Show();
                        return;
                    }
                }
                laPersonaMoral.CorreoElectronico = this.txtCorreo.Text;
                
                laPersonaMoral.GiroEmpresa = this.txtGiro.Text;
                laPersonaMoral.RepresentanteLegal = this.txtRepLegal.Text;
                laPersonaMoral.CentroCostos = this.txtCentroCostos.Text;

                laPersonaMoral.Calle = this.txtCalle.Text;
                laPersonaMoral.NumeroExterior = this.txtNumExterior.Text;
                laPersonaMoral.NumeroInterior = this.txtNumInterior.Text;
                laPersonaMoral.EntreCalles = this.txtEntreCalles.Text;
                laPersonaMoral.Referencias = this.txtReferencias.Text;
                laPersonaMoral.CodigoPostal = this.txtCodigoPostal.Text;
                laPersonaMoral.Colonia = this.cBoxColonia.SelectedItem.Value;
                laPersonaMoral.Municipio = this.txtMunicipio.Text;
                laPersonaMoral.Ciudad = this.txtCiudad.Text;
                laPersonaMoral.Estado = this.txtEstado.Text;
                laPersonaMoral.Pais = this.cBoxPais.SelectedItem.Value;
                laPersonaMoral.Latitud = this.txtLatitud.Text;
                laPersonaMoral.Longitud = this.txtLongitud.Text;

                laPersonaMoral.DesempenaFuncionesPublicas = this.cBoxP1.SelectedItem.Value;
                laPersonaMoral.ParentescoPPE = this.cBoxP2.SelectedItem.Value;
                laPersonaMoral.SocioDePPE = this.cBoxP3.SelectedItem.Value;
                laPersonaMoral.DineroIngresoPorMes = this.txtP4.Text;
                laPersonaMoral.NumVecesDeIngresoPorMes = this.txtP5.Text;
                laPersonaMoral.CuandoIngresaDineroPorMes = this.txtP6.Text;
                laPersonaMoral.OrigenRecursos = this.txtP7.Text;
                laPersonaMoral.DestinoRecursos = this.txtP8.Text;
                laPersonaMoral.TipoRecursos = this.txtP9.Text;
                laPersonaMoral.NaturalezaRecursos = this.txtP10.Text;

                GuardarCambios(laPersonaMoral);
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Actualización de Datos", "Ocurrió un error al guardar los datos de la Persona Moral").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Colectiva
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void GuardarCambios(PersonaMoral persona)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                pCI.Info("INICIA CreaOModificaPersonaMoral()");
                long Folio = LNAdministrarPersonasMorales.CreaOModificaPersonaMoral(persona,
                    Path.GetFileName(Request.Url.AbsolutePath), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminPersMor);
                pCI.Info("TERMINA CreaOModificaPersonaMoral()");

                this.hdnIdPM.Value = Folio;

                if (Convert.ToInt16(this.hdnNew.Value) == 1)
                {
                    X.Msg.Notify("Persona Moral", "Nueva Persona Moral creada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("Persona Moral", "Cambios guardados <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }
            
            catch (CAppException err)
            {
                X.Msg.Alert("Persona Moral", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultados de colectivas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                LimpiaSeleccionPrevia(true);

                unLog.Info("INICIA ObtienePersonasMorales()");
                DataSet dsPersonas = DAOAdministrarPersonasMorales.ObtienePersonasMorales(
                    this.cBoxClientes.SelectedItem.Value, this.txtRazSoc.Text,
                    Convert.ToInt32(this.cBoxEstatus.SelectedItem.Value),
                    this.txt_CLABE.Text, LH_ParabAdminPersMor);
                unLog.Info("TERMINA ObtienePersonasMorales()");

                if (dsPersonas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Personas Morales", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.StorePersonas.DataSource = dsPersonas;
                    this.StorePersonas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Personas Morales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Personas Morales", "Ocurrió un error al obtener las Personas Morales").Show();
            }
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
                Int64 IdPersona = 0;
                string razon = String.Empty;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] persona = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in persona[0])
                {
                    switch (column.Key)
                    {
                        case "IdPersonaMoral": IdPersona = Convert.ToInt64(column.Value); break;
                        case "RazonSocial": razon = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia(false);

                this.hdnIdPM.Text = IdPersona.ToString();
                PanelCentral.Title = razon;

                LlenaPanelesInformacion(IdPersona);
                LlenaGridDocs(IdPersona);
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(LH_ParabAdminPersMor);
                pCI.ErrorException(ex);
                X.Msg.Alert("Persona Moral", "Error al establecer la información de la Persona Moral seleccionada").Show();
            }
        }

        /// <summary>
        /// Llena los controles de los paneles de Información General, Domicilio y Cuestionario
        /// con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdPersonaMoral">Identificador de la persona moral</param>
        protected void LlenaPanelesInformacion(Int64 IdPersonaMoral)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                pCI.Info("INICIA ObtieneDatosPersonaMoral()");
                PersonaMoral unaPersona =
                    DAOAdministrarPersonasMorales.ObtieneDatosPersonaMoral(IdPersonaMoral, LH_ParabAdminPersMor);
                pCI.Info("TERMINA ObtieneDatosPersonaMoral()");

                this.hdnClaveCliente.Value = this.cBoxClientes.Value;

                this.txtEstatus.Text = unaPersona.Estatus;
                this.txtRazonSocial.Text = unaPersona.RazonSocial;
                this.txtCLABE.Text = unaPersona.CLABE;
                this.txtRFC.Text = unaPersona.RFC;
                this.txtTelefono.Text = unaPersona.Telefono;
                this.txtCorreo.Text = unaPersona.CorreoElectronico;
                this.txtGiro.Text = unaPersona.GiroEmpresa;
                this.txtRepLegal.Text = unaPersona.RepresentanteLegal;
                this.txtCentroCostos.Text = unaPersona.CentroCostos;
                this.txtTarjeta.Text = unaPersona.NumeroTarjeta;
                this.txtTipoMan.Text = unaPersona.TipoManufactura;

                this.txtCalle.Text = unaPersona.Calle;
                this.txtNumExterior.Text = unaPersona.NumeroExterior;
                this.txtNumInterior.Text = unaPersona.NumeroInterior;
                this.txtEntreCalles.Text = unaPersona.EntreCalles;
                this.txtReferencias.Text = unaPersona.Referencias;
                this.txtCodigoPostal.Text = unaPersona.CodigoPostal;
                this.cBoxColonia.Value = unaPersona.Colonia;
                this.txtMunicipio.Text = unaPersona.Municipio;
                this.txtCiudad.Text = unaPersona.Ciudad;
                this.txtEstado.Text = unaPersona.Estado;
                this.cBoxPais.Value = unaPersona.Pais;
                this.txtLatitud.Value = unaPersona.Latitud;
                this.txtLongitud.Value = unaPersona.Longitud;

                this.cBoxP1.Value = unaPersona.DesempenaFuncionesPublicas;
                this.cBoxP2.Value = unaPersona.ParentescoPPE;
                this.cBoxP3.Value = unaPersona.SocioDePPE;
                this.txtP4.Text = unaPersona.DineroIngresoPorMes;
                this.txtP5.Text = unaPersona.NumVecesDeIngresoPorMes;
                this.txtP6.Text = unaPersona.CuandoIngresaDineroPorMes;
                this.txtP7.Text = unaPersona.OrigenRecursos;
                this.txtP8.Text = unaPersona.DestinoRecursos;
                this.txtP9.Text = unaPersona.TipoRecursos;
                this.txtP10.Text = unaPersona.NaturalezaRecursos;

                this.FormPanelInfoGral.Disabled = false;
                this.FormPanelDomicilio.Disabled = false;
                this.FormPanelCuestionario.Disabled = false;
                this.FormPanelDocumentos.Disabled = false;
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Información", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Información", "Error al obtener la información de la Persona Moral").Show();
            }
        }

        /// <summary>
        /// Llena el panel de Documentps con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdPersonaMoral">Identificador de la persona moral</param>
        protected void LlenaGridDocs(Int64 IdPersonaMoral)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                Boolean esEjecutor = Boolean.Parse(HttpContext.Current.Session["EsEjecutor"].ToString());

                unLog.Info("INICIA ObtieneDocumentosPersonaMoral()");
                this.StoreDocumentos.DataSource = 
                    DAOAdministrarPersonasMorales.ObtieneDocumentosPersonaMoral(IdPersonaMoral, 
                    esEjecutor, LH_ParabAdminPersMor);
                unLog.Info("TERMINA ObtieneDocumentosPersonaMoral()");

                this.StoreDocumentos.DataBind();

                EstableceControlesPorRol();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Documentos", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Documentos", "Error al obtener los documentos de la Persona Moral").Show();
            }
        }

        /// <summary>
        /// Llena el combo de colonias y los campos de municipio y estado, con información de base de datos
        /// </summary>
        [DirectMethod(Namespace = "PersonasMorales")]
        public void LlenaComboColonias()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminPersMor);

            if (!String.IsNullOrEmpty(this.txtCodigoPostal.Text) &&
                this.txtCodigoPostal.Text.Length >= 5)
            {
                try
                {
                    unLog.Info("INICIA ListaDatosPorCodigoPostal()");
                    DataSet dsColonias = DAOTarjetaCuenta.ListaDatosPorCodigoPostal(
                        this.txtCodigoPostal.Text,
                        this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminPersMor);
                    unLog.Info("TERMINA ListaDatosPorCodigoPostal()");

                    if (dsColonias.Tables[0].Rows.Count < 1)
                    {
                        X.Msg.Alert("Código Postal", "No existen coincidencias con el Código Postal ingresado").Show();
                    }
                    else
                    {
                        this.StoreColonias.DataSource = dsColonias;
                        this.StoreColonias.DataBind();

                        this.txtMunicipio.Text = dsColonias.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                        this.txtEstado.Text = dsColonias.Tables[0].Rows[0]["Estado"].ToString().Trim();
                    }
                }

                catch (CAppException err)
                {
                    X.Msg.Alert("Domicilio", err.Mensaje()).Show();
                }

                catch (Exception ex)
                {
                    unLog.ErrorException(ex);
                    X.Msg.Alert("Domicilio", "Error al obtener la información relacionada con el Código Postal").Show();
                }
            }
            else
            {
                this.txtMunicipio.Clear();
                this.txtEstado.Clear();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Agregar Documentos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        public void btnAgregaDoc_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                Int64 IdPersonaMoral;

                if (!Int64.TryParse(this.hdnIdPM.Value.ToString(), out IdPersonaMoral))
                {
                    X.Msg.Alert("Añadir Documento", "Para añadir documentos, por favor captura la Información General " +
                        " de la Persona Moral.").Show();
                    return;
                }

                HttpPostedFile file = this.fufDocumentos.PostedFile;

                string[] directorios = file.FileName.Split('\\');
                string fileName = directorios[directorios.Count() - 1];

                //Se valida que se haya seleccionado un archivo
                if (String.IsNullOrEmpty(fileName))
                {
                    X.Msg.Alert("Añadir Documento", "Selecciona un documento para cargarlo.").Show();
                    return;
                }

                //Se valida que se haya seleccionado un documento con los formatos definidos
                if (!fileName.Contains(".pdf") && !fileName.Contains(".txt") &&
                    !fileName.Contains(".jpg") && !fileName.Contains(".jpeg") &&
                    !fileName.Contains(".png"))
                {
                    X.Msg.Alert("Añadir Documento", "El documento seleccionado no es de los formatos permitidos " +
                        "(*.pdf, *.txt, *.jpg, *.jpeg, *.png).").Show();
                    return;
                }

                string rutaArchivo = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "DirectorioPM").Valor + IdPersonaMoral.ToString() + "\\";

                //Si no existe el directorio, lo crea
                if (!Directory.Exists(rutaArchivo))
                {
                    Directory.CreateDirectory(rutaArchivo);
                }

                rutaArchivo += fileName;

                if (!File.Exists(rutaArchivo))
                {
                    //Se almacena archivo en directorio
                    file.SaveAs(rutaArchivo);
                }

                unLog.Info("INICIA CreaDocumentoPersonaMoral()");
                LNAdministrarPersonasMorales.CreaDocumentoPersonaMoral(IdPersonaMoral, fileName,
                    rutaArchivo, this.Usuario, LH_ParabAdminPersMor);
                unLog.Info("TERMINA CreaDocumentoPersonaMoral()");

                X.Msg.Notify("Añadir Documento", "Documento añadido <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.StoreDocumentos.RemoveAll();
                LlenaGridDocs(IdPersonaMoral);
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Añadir Documento", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Añadir Documento", "Error al añadir el documento a la Persona Moral").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos del grid de documentos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                Int64 IdPersonaMoral = Convert.ToInt64(this.hdnIdPM.Value);
                Int64 IdDocumento = Convert.ToInt64(e.ExtraParams["ID_DocumentoPM"]);

                char[] charsToTrim = { '*', '"', ' ' };
                string NombreArchivo = e.ExtraParams["Nombre"].Trim(charsToTrim);
                

                string tempPath = Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                        "DirectorioPM").Valor + IdPersonaMoral.ToString() + "\\";

                switch (comando)
                {
                    case "Descargar":
                        string contentType = NombreArchivo.Contains(".pdf") ? "application/pdf" :
                            NombreArchivo.Contains(".txt") ? "text/plain" :
                            NombreArchivo.Contains(".png") ? "image/png" : "image/jpeg";
                        
                        HttpResponse response = HttpContext.Current.Response;
                        response.ClearContent();
                        response.Clear();
                        response.ClearHeaders();
                        response.ContentType = contentType;
                        response.AddHeader("Content-Disposition", "attachment; filename=" + NombreArchivo + ";");
                        response.TransmitFile(tempPath + NombreArchivo);
                        response.Flush();
                        response.End();
                        break;

                    //case "Eliminar":
                    default:
                        tempPath += NombreArchivo;

                        if (File.Exists(tempPath))
                        {
                            File.Delete(tempPath);
                        }

                        log.Info("INICIA BorraDocumentoPersonaMoral()");
                        LNAdministrarPersonasMorales.BorraDocumentoPersonaMoral(IdDocumento, 
                            this.Usuario, LH_ParabAdminPersMor);
                        log.Info("TERMINA BorraDocumentoPersonaMoral()");

                        X.Msg.Notify("", "Documento eliminado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                        break;
                }

                LlenaGridDocs(Convert.ToInt64(this.hdnIdPM.Value));
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Documentos", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Documentos", "Error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Generar Cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGenerarCta_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                Int64 IdPersonaMoral = Convert.ToInt64(this.hdnIdPM.Value);
                string elEstatus = this.hdnEstatus.Value.ToString();

                string wsLoginResp;
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                if (elEstatus != "Autorizado")
                {
                    logPCI.Info("INICIA RegistraAutorizacionPersonaMoral()");
                    LNAdministrarPersonasMorales.RegistraAutorizacionPersonaMoral(IdPersonaMoral, this.Usuario, LH_ParabAdminPersMor);
                    logPCI.Info("TERMINA RegistraAutorizacionPersonaMoral()");
                }

                _headers.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_URL").Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Usr").Valor;
                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsParab_Pwd").Valor;

                logPCI.Info("INICIA Parabilium.Login()");
                wsLoginResp = Parabilium.Login(_headers, _body, LH_ParabAdminPersMor);
                logPCI.Info("TERMINA Parabilium.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    X.Msg.Alert("Generar Cuenta", wsLoginResp).Show();
                }
                else
                {
                    _headers.Token = wsLoginResp;
                    _headers.Credenciales = Cifrado.Base64Encode(_body.NombreUsuario + ":" + _body.Password);

                    Parametros.AltaPersonaMoralBody alta = new Parametros.AltaPersonaMoralBody();
                    alta.Folio = IdPersonaMoral.ToString();
                    alta.ClaveEmpresa = this.cBoxClientes.SelectedItem.Value;
                    alta.UsuarioAutorizador = this.Usuario.ClaveUsuario;

                    logPCI.Info("INICIA Parabilium.AltaPersonaMoral()");
                    string wsAltaResp = Parabilium.AltaPersonaMoral(_headers, alta, LH_ParabAdminPersMor);
                    logPCI.Info("TERMINA Parabilium.AltaPersonaMoral()");

                    if (wsAltaResp.ToUpper().Contains("ERROR"))
                    {
                        X.Msg.Alert("Generar Cuenta", wsAltaResp).Show();
                    }
                    else
                    {
                        X.Msg.Notify("", "Cuenta generada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        this.btnLimpiar.FireEvent("click");
                    }
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Generar Cuenta", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Generar Cuenta", "Ocurrió un error al generar la Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Dejar en Revisión
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnARevision_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                Int64 IdPersonaMoral = Convert.ToInt64(this.hdnIdPM.Value);

                log.Info("INICIA ModificaEstatusPM_EnRevision()");
                LNAdministrarPersonasMorales.ModificaEstatusPM_EnRevision(IdPersonaMoral, this.Usuario, LH_ParabAdminPersMor);
                log.Info("TERMINA ModificaEstatusPM_EnRevision()");

                X.Msg.Notify("", "Persona Moral dejada a revisión <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnLimpiar.FireEvent("click");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("En Revisión", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("En Revisión", "Ocurrió un error al establecer En Revisión a la Persona Moral").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Rechazar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnRechazar_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminPersMor);

            try
            {
                Int64 IdPersonaMoral = Convert.ToInt64(this.hdnIdPM.Value);

                pCI.Info("INICIA ModificaEstatusPM_EnProceso()");
                LNAdministrarPersonasMorales.ModificaEstatusPM_EnProceso(IdPersonaMoral, this.Usuario, LH_ParabAdminPersMor);
                pCI.Info("INICIA ModificaEstatusPM_EnProceso()");

                X.Msg.Notify("", "Persona Moral rechazada <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.btnLimpiar.FireEvent("click");
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Rechazar", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Rechazar", "Ocurrió un error al rechazar la creación de la Persona Moral").Show();
            }
        }
    }
}
