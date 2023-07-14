using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.LogicaNegocio;
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
using Utilerias;

namespace TpvWeb
{
    public partial class AdministrarColectivasParabilia : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER PARABILIA Administrar Colectivas
        private LogHeader LH_ParabAdminColectivas = new LogHeader();

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Administrar Colectivas para Parabilia
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ParabAdminColectivas.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ParabAdminColectivas.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ParabAdminColectivas.User = this.Usuario.ClaveUsuario;
            LH_ParabAdminColectivas.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ParabAdminColectivas);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdministrarColectivasParabilia Page_Load()");

                if (!IsPostBack)
                {
                    EstableceCombos();

                    this.dfFechaNac.MaxDate = DateTime.Today;
                    this.cBoxTipoDomicilio.SetValue(1);

                    dfVigenciaCuenta.MinDate = DateTime.Today;
                    dfVigCuenta.MinDate = DateTime.Today;
                }

                log.Info("TERMINA AdministrarColectivasParabilia Page_Load()");
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
        /// Establece los valores de los controles combo con sus valores de catálogos
        /// </summary>
        protected void EstableceCombos()
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            log.Info("INICIA ListaTiposColectivaSubemisor()");
            DataSet dsTiposColectiva = DAOColectiva.ListaTiposColectivaSubemisor
                (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminColectivas);
            log.Info("TERMINA ListaTiposColectivaSubemisor()");

            ValidaPermisosCadenas(dsTiposColectiva);

            this.StoreTipoColectiva.DataSource = dsTiposColectiva;
            this.StoreTipoColectiva.DataBind();

            log.Info("INICIA ListaEstatusColectivas()");
            this.StoreEstatus.DataSource = DAOAdministrarColectivas.ListaEstatusColectivas(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminColectivas);
            log.Info("TERMINA ListaEstatusColectivas()");
            this.StoreEstatus.DataBind();

            log.Info("INICIA ListaClasificacionParametros()");
            this.StoreClasificacion.DataSource = DAOAdministrarColectivas.ListaClasificacionParametros(
                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminColectivas);
            log.Info("TERMINA ListaClasificacionParametros()");
            this.StoreClasificacion.DataBind();

            log.Info("INICIA ListaParamsCatalogo()");
            DataSet dsCatalogos = DAOAdministrarColectivas.ListaParamsCatalogo(this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminColectivas);
            log.Info("TERMINA ListaParamsCatalogo()");

            if (dsCatalogos.Tables[0].Rows.Count > 0)
            {
                this.StorePMAsCat.DataSource = dsCatalogos;
                this.StorePMAsCat.DataBind();
                this.hdnCatalogos.Value = 1;
            }

            log.Info("INICIA ListaTiposParametrosMA()");
            this.StoreTipoParametroMA.DataSource =
                DAOAdministrarColectivas.ListaTiposParametrosMA(false, this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                LH_ParabAdminColectivas);
            log.Info("TERMINA ListaTiposParametrosMA()");
            this.StoreTipoParametroMA.DataBind();
        }

        /// <summary>
        /// Valida que entre los tipos de colectiva permitidos al usuario 
        /// esté el de Cadena Comercial
        /// </summary>
        /// <param name="ds">DataSet con los tipos de colectiva permitidos para el usuario</param>
        protected void ValidaPermisosCadenas(DataSet ds)
        {
            DataRow[] dr = ds.Tables[0].Select(string.Format("{0} LIKE '%{1}%'", "Clave", "CCM"));
            LogPCI unLog = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                //Sí tiene permisos
                if (dr.Length == 1)
                {
                    unLog.Info("INICIA ListaColectivasPorTipo()");
                    DataSet dsCadenaComercial = DAOColectiva.ListaColectivasPorTipo(
                        int.Parse(dr[0].ItemArray[0].ToString()), "", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ParabAdminColectivas);
                    unLog.Info("TERMINA ListaColectivasPorTipo()");

                    List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow cadena in dsCadenaComercial.Tables[0].Rows)
                    {
                        var cadenaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                            ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                            NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                        };
                        cadenaList.Add(cadenaCombo);
                    }

                    StoreCCR.DataSource = cadenaList;
                    StoreCCR.DataBind();
                }
                else
                {
                    this.cBoxCadena.Disabled = true;
                }
            }
            catch(CAppException caEx)
            {
                unLog.Error(caEx.Mensaje());
                throw new Exception();
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
            FormPanelDatosBase.Reset();
            ActivaDesactivaAscendencia(true);

            this.txtTColecAsc.Reset();
            StoreColectivaPadre.RemoveAll();

            this.cBoxTipoColec.Reset();
            this.txtColectiva.Reset();
            StoreColectivas.RemoveAll();

            LimpiaSeleccionPrevia();
            PanelCentral.Disabled = true;
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Colectiva
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaColectiva_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                unLog.Info("INICIA CreaNuevaColectivaEnAutorizador()");
                DataTable dt = LNAdministrarColectivas.CreaNuevaColectivaEnAutorizador(
                    int.Parse(this.cBoxTipoColectiva.SelectedItem.Value),
                    Convert.ToInt32(this.cBoxColecPadre.SelectedItem.Value),
                    this.txtClaveColectiva.Text, this.txtRazonSocial.Text,
                    this.txtNombreComercial.Text, int.Parse(this.cBoxDivisa.SelectedItem.Value),
                    this.Usuario, LH_ParabAdminColectivas);
                unLog.Info("TERMINA CreaNuevaColectivaEnAutorizador()");

                string msj = dt.Rows[0]["Mensaje"].ToString();
                string idColectivaNueva = dt.Rows[0]["ID_NuevaColectiva"].ToString();

                if (idColectivaNueva == "-1")
                {
                    X.Msg.Alert("Nueva Colectiva", msj).Show();
                }
                else
                {
                    PermisosNuevaColectiva(idColectivaNueva);

                    this.WdwNuevaColectiva.Hide();

                    this.cBoxTipoColec.Value = this.cBoxTipoColectiva.SelectedItem.Value;
                    this.cBoxTipoColec.SelectedItem.Text = this.cBoxTipoColectiva.SelectedItem.Text;
                    this.txtColectiva.Text = txtClaveColectiva.Text;                
                    LlenaGridResultados();

                    X.Msg.Alert("Nueva Colectiva", "<br />" + this.cBoxTipoColectiva.SelectedItem.Text + 
                        " creado(a)<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "TpvWeb.CargaNuevaColectiva()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nueva Colectiva", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Nueva Colectiva", "Ocurrió un error al crear la Colectiva").Show();
            }
        }

        /// <summary>
        /// Valida si el tipo de colectiva recién creado requiere de permisos para el usuario
        /// en sesión, y de ser así, solicita los permisos
        /// </summary>
        /// <param name="IdNuevaColectiva">Identificador de la nueva colectiva</param>
        protected void PermisosNuevaColectiva(string IdNuevaColectiva)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                pCI.Info("INICIA ObtieneSPFiltroAltaColectiva()");
                string nombreSP_Filtro =
                    DALCentralAplicaciones.BaseDatos.DAOCatalogos.ObtieneSPFiltroAltaColectiva(
                    hdnClaveTipoColectiva.Value.ToString().Trim(),
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                pCI.Info("TERMINA ObtieneSPFiltroAltaColectiva()");

                pCI.Info("INICIA ObtieneFiltrosParaAltaColectiva()");
                Guid TableIdColectiva =
                    DALCentralAplicaciones.BaseDatos.DAOCatalogos.ObtieneFiltrosParaAltaColectiva(
                    nombreSP_Filtro, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                pCI.Info("TERMINA ObtieneFiltrosParaAltaColectiva()");

                //Si el tipo de colectiva recién creado requiere permisos
                if (TableIdColectiva != new Guid())
                {
                    //Se crean los permisos para el usuario
                    pCI.Info("INICIA CreaPermisoTableValue()");
                    LNUsuarios.CreaPermisoTableValue(this.Usuario.UsuarioId, TableIdColectiva,
                        IdNuevaColectiva, true, LH_ParabAdminColectivas, this.Usuario);
                    pCI.Info("TERMINA CreaPermisoTableValue()");

                    //Se otrogan el permiso inmediato al usuario en sesión
                    pCI.Info("INICIA AgregarFiltrosEnSesion()");
                    LNFiltro.AgregarFiltrosEnSesion(this.Usuario, LH_ParabAdminColectivas,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    pCI.Info("TERMINA AgregarFiltrosEnSesion()");
                }
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
        /// Controla el evento clic al botón Aceptar al mensaje informativo de creación
        /// de colectiva exitosa
        /// </summary>
        [DirectMethod(Namespace = "TpvWeb")]
        public void CargaNuevaColectiva()
        {
            RowSelectionModel rsm = GridResultados.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultados.FireEvent("RowClick");
        }

        /// <summary>
        /// Habilita o deshabilita los controles del panel de Ascendencia
        /// </summary>
        /// <param name="Activa">Bandera para establecer la propiedad Disabled</param>
        protected void ActivaDesactivaAscendencia(bool Activa)
        {
            this.cBoxColecPadre.Disabled = Activa;
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Tipo de Colectiva del panel izquierdo,
        /// prestableciendo la colectiva padre correspondiente y obteniendo la lista de ellas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void PrestableceTColecPadre(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                int IdTipoColectivaPadre;
                DataSet dsTiposColectivaPadre = new DataSet();
                DataSet dsColectivasPadre = new DataSet();

                unLog.Info("INICIA ListaTiposColectivaPadre()");
                dsTiposColectivaPadre = DAOAdministrarColectivas.ListaTiposColectivaPadre(
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                unLog.Info("TERMINA ListaTiposColectivaPadre()");

                ActivaDesactivaAscendencia(true);

                foreach (DataRow dr in dsTiposColectivaPadre.Tables[0].Rows)
                {
                    if (this.cBoxTipoColectiva.SelectedItem.Value == dr["IdTipoColectiva"].ToString())
                    {
                        txtTColecAsc.Text = dr["ColectivaPadre"].ToString();
                        IdTipoColectivaPadre = int.Parse(dr["ID_TipoColectivaPadre"].ToString());

                        unLog.Info("INICIA ListaColectivasPorTipo()");
                        dsColectivasPadre = DAOColectiva.ListaColectivasPorTipo(
                            IdTipoColectivaPadre, "", this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                            LH_ParabAdminColectivas);
                        unLog.Info("TERMINA ListaColectivasPorTipo()");

                        List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

                        foreach (DataRow drCol in dsColectivasPadre.Tables[0].Rows)
                        {
                            var colectivaCombo = new ColectivaComboPredictivo()
                            {
                                ID_Colectiva = Convert.ToInt64(drCol["ID_Colectiva"].ToString()),
                                ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                                NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                            };
                            ComboList.Add(colectivaCombo);
                        }

                        StoreColectivaPadre.DataSource = ComboList;
                        StoreColectivaPadre.DataBind();

                        ActivaDesactivaAscendencia(false);
                    }
                }

                unLog.Info("INICIA ListaDivisas()");
                DataSet dsDivisas = DAOAdministrarColectivas.ListaDivisasColectivas
                    (int.Parse(this.cBoxTipoColectiva.SelectedItem.Value),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                unLog.Info("TERMINA ListaDivisas()");

                this.StoreDivisa.DataSource = dsDivisas;
                this.StoreDivisa.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ascendencia", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Ascendencia", "Ocurrió un error al preestablecer las Colectivas Padre").Show();
            }
        }

        /// <summary>
        /// Llena el grid de resultados de colectivas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                LimpiaSeleccionPrevia();

                unLog.Info("INICIA ListaColectivasPorTipo()");
                DataSet dsColectivas = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColec.SelectedItem.Value), this.txtColectiva.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                unLog.Info("TERMINA ListaColectivasPorTipo()");

                if (dsColectivas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Colectivas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreColectivas.DataSource = dsColectivas;
                    StoreColectivas.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Colectivas", "Ocurrió un error al realizar la bÚsqueda.").Show();
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
        /// Restablece los controles del panel de direcciones a su estatus de carga inicial
        /// </summary>
        protected void LimpiaFormPanelDirecciones()
        {
            FormPanelDirecciones.Reset();

            cBoxTipoDomicilio.Disabled = true;
            cBoxTipoDomicilio.SetValue(1);

            txtCalle.Clear();
            txtNumExterior.Clear();
            txtNumInterior.Clear();
            txtEntreCalles.Clear();
            txtReferencias.Clear();
            txtCP.Clear();

            LimpiaDatosRelacionadosCP();

            txtColonia.Hide();
            btnGuardaDireccion.Disabled = true;
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una colectiva en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            this.FormPanelInfoAd.Reset();
            this.btnGuardarInfoAd.Disabled = true;

            LimpiaFormPanelDirecciones();

            this.FormPanelNuevaCuenta.Reset();
            this.StoreCuentas.RemoveAll();
            this.btnNuevaCuenta.Disabled = true;

            this.cBoxClasificacion.Reset();
            this.cBoxParametros.Disabled = true;
            this.btnAddParametros.Disabled = true;
            this.cBoxParametros.Reset();
            this.StoreParametros.RemoveAll();
            this.StoreValoresParametros.RemoveAll();

            this.FormPanelNuevoCat.Reset();
            this.cBoxCatGrid.Reset();
            this.StoreElemsCat.RemoveAll();

            this.FormPanelInfoAd.Show();

            this.cBoxTipoParametroMA.Reset();
            this.cBoxParametrosMA.Disabled = true;
            this.btnAddParametrosMA.Disabled = true;
            this.cBoxParametrosMA.Reset();
            this.StoreParametrosMA.RemoveAll();
            this.StoreValoresParametrosMA.RemoveAll();
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
                int IdColectiva = 0;
                string ClaveColectiva = "", ClaveTipoColectiva = "", NombreColectiva = "";

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] colectiva = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in colectiva[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Colectiva": IdColectiva = int.Parse(column.Value); break;
                        case "ClaveColectiva": ClaveColectiva = column.Value; break;
                        case "NombreORazonSocial": NombreColectiva = column.Value; break;
                        case "ClaveTipoColectiva": ClaveTipoColectiva = column.Value; break;
                        default:
                            break;
                    }
                }

                LimpiaSeleccionPrevia();

                this.hdnIdColectiva.Text = IdColectiva.ToString();

                PanelCentral.Title = cBoxTipoColec.SelectedItem.Text +  " - " + NombreColectiva;

                LlenaFormPanelInfoAd(IdColectiva, ClaveColectiva);
                LlenaFormPanelDirecciones(IdColectiva);
                LlenaFormPanelCuentas(IdColectiva);
                LlenaFormPanelCLABE(IdColectiva);

                PanelCentral.Disabled = false;
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(LH_ParabAdminColectivas);
                unLog.ErrorException(ex);
                X.Msg.Alert("Colectivas", "Ocurrió un error al obtener la información de la Colectiva.").Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="ClaveColectiva">Clave de la colectiva</param>
        protected void LlenaFormPanelInfoAd(int IdColectiva, string ClaveColectiva)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                logPCI.Info("INICIA ListaInfoColectiva()");
                DataSet dsColectiva = DAOAdministrarColectivas.ListaInfoColectiva(
                    IdColectiva, this.Usuario, LH_ParabAdminColectivas);
                logPCI.Info("TERMINA ListaInfoColectiva()");

                this.txtClaveCol.Text    =   ClaveColectiva;
                this.txtRazonSoc.Text    =   dsColectiva.Tables[0].Rows[0]["NombreORazonSocial"].ToString().Trim();
                this.txtNombreCom.Text   =   dsColectiva.Tables[0].Rows[0]["NombreComercial"].ToString().Trim();
                this.txtApPaterno.Text   =   dsColectiva.Tables[0].Rows[0]["APaterno"].ToString().Trim();
                this.txtApMaterno.Text   =   dsColectiva.Tables[0].Rows[0]["AMaterno"].ToString().Trim();
                this.dfFechaNac.Value    =   String.IsNullOrEmpty(dsColectiva.Tables[0].Rows[0]["FechaNacimiento"].ToString().Trim()) ?
                                                    "" : dsColectiva.Tables[0].Rows[0]["FechaNacimiento"].ToString().Trim();

                this.txtCURP.Text   =   dsColectiva.Tables[0].Rows[0]["CURP"].ToString().Trim();
                this.txtRFC.Text    =   dsColectiva.Tables[0].Rows[0]["RFC"].ToString().Trim();
                this.txtEmail.Text  =   dsColectiva.Tables[0].Rows[0]["Email"].ToString().Trim();

                this.txtTelefonoFijo.Text    =   dsColectiva.Tables[0].Rows[0]["Telefono"].ToString().Trim();
                this.txtTelefonoCel.Text     =   dsColectiva.Tables[0].Rows[0]["Movil"].ToString().Trim();

                this.cBoxCadena.SelectedItem.Text   =   dsColectiva.Tables[0].Rows[0]["CadenaRelacionada"].ToString().Trim();
                this.cBoxCadena.SelectedItem.Value  =   dsColectiva.Tables[0].Rows[0]["ID_CadenaComercial"].ToString().Trim();

                this.cBoxEstatus.SelectedItem.Text  =   dsColectiva.Tables[0].Rows[0]["Estatus"].ToString().Trim();
                this.cBoxEstatus.Value              =   dsColectiva.Tables[0].Rows[0]["ID_EstatusColectiva"].ToString().Trim();

                this.btnGuardarInfoAd.Disabled = false;
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Colectivas", "Ocurrió un error al obtener la Información Adicional de la Colectiva.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de Información Adicional, llamando
        /// a la actualización de datos de la colectiva en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAd_Click(object sender, EventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                DALPuntoVentaWeb.Entidades.Colectiva colectiva = new DALPuntoVentaWeb.Entidades.Colectiva();

                colectiva.ID_Colectiva          = int.Parse(this.hdnIdColectiva.Text);
                colectiva.ClaveColectiva        = this.txtClaveCol.Text;
                colectiva.NombreORazonSocial    = this.txtRazonSoc.Text;
                colectiva.NombreComercial       = this.txtNombreCom.Text;
                colectiva.APaterno              = this.txtApPaterno.Text;
                colectiva.AMaterno              = this.txtApMaterno.Text;
                colectiva.FechaNacimiento       = dfFechaNac.SelectedDate.Equals(DateTime.MinValue) ? 
                    DateTime.Today.AddDays(1) : Convert.ToDateTime(dfFechaNac.SelectedDate);
                colectiva.CURP                  = this.txtCURP.Text;
                colectiva.RFC                   = this.txtRFC.Text;
                colectiva.Email                 = this.txtEmail.Text;
                colectiva.Telefono              = this.txtTelefonoFijo.Text;
                colectiva.Movil                 = this.txtTelefonoCel.Text;
                colectiva.IdCadenaRelacionada   =   String.IsNullOrEmpty(this.cBoxCadena.SelectedItem.Value) ? 0 :
                                                        Convert.ToInt64(this.cBoxCadena.SelectedItem.Value);
                colectiva.IdEstatus             = Convert.ToInt32(this.cBoxEstatus.SelectedItem.Value);

                pCI.Info("INICIA ModificaColectivaEnAutorizador()");
                LNAdministrarColectivas.ModificaColectivaEnAutorizador(colectiva, LH_ParabAdminColectivas, this.Usuario);
                pCI.Info("TERMINA ModificaColectivaEnAutorizador()");

                X.Msg.Notify("", "Colectiva Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Colectiva", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Colectiva", "Ocurrió un error al guardar la Información Adicional").Show();
            }
        }

        /// <summary>
        /// Limpia el contenido de los controles que dependen del código postal
        /// </summary>
        protected void LimpiaDatosRelacionadosCP()
        {
            StoreColonias.RemoveAll();
            cBoxColonia.Clear();
            txtClaveMunicipio.Clear();
            txtMunicipio.Clear();
            txtClaveEstado.Clear();
            txtEstado.Clear();
        }

        /// <summary>
        /// Actualiza el combo con las colonias del código postal ingresado
        /// </summary>
        protected void ActualizaComboColonias()
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                logPCI.Info("INICIA ListaDatosPorCodigoPostal()");
                DataSet dsColonias = DAOAdministrarColectivas.ListaDatosPorCodigoPostal(
                    this.txtCP.Text, LH_ParabAdminColectivas);
                logPCI.Info("TERMINA ListaDatosPorCodigoPostal()");

                StoreColonias.DataSource = dsColonias;
                StoreColonias.DataBind();

                if (!String.IsNullOrEmpty(txtColonia.Text))
                {
                    foreach (DataRow row in dsColonias.Tables[0].Rows)
                    {
                        if (row["Colonia"].ToString().Equals(txtColonia.Text))
                        {
                            cBoxColonia.SetValue(row["ID_Colonia"]);
                            txtColonia.Clear();
                            txtColonia.Hide();
                            break;
                        }
                    }
                }

                txtClaveMunicipio.Text = dsColonias.Tables[0].Rows[0]["ClaveMunicipio"].ToString().Trim();
                txtMunicipio.Text = dsColonias.Tables[0].Rows[0]["Municipio"].ToString().Trim();
                txtClaveEstado.Text = dsColonias.Tables[0].Rows[0]["ClaveEstado"].ToString().Trim();
                txtEstado.Text = dsColonias.Tables[0].Rows[0]["Estado"].ToString().Trim();
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Domicilios", "Ocurrió un error al establecer las Colonias").Show();
            }
        }

        /// <summary>
        /// Llena el combo de colonias y los campos de municipio y estado, con información de base de datos
        /// </summary>
        [DirectMethod(Namespace = "TpvWeb")]
        public void LlenaComboColonias()
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                LimpiaDatosRelacionadosCP();

                if (!String.IsNullOrEmpty(txtCP.Text))
                {
                    pCI.Info("INICIA ValidaCodigoPostal()");
                    string resp = DAOAdministrarColectivas.ValidaCodigoPostal(txtCP.Text, LH_ParabAdminColectivas);
                    pCI.Info("TERMINA ValidaCodigoPostal()");

                    if (resp.Equals("OK"))
                    {
                        ActualizaComboColonias();
                    }
                    else
                    {
                        X.Msg.Alert("Domicilios", resp).Show();
                    }
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Domicilios", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Domicilios", "Ocurrió un error al establecer las Colonias.").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select al combo de tipos de domicilio
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void cBoxTipoDomicilio_Select(object sender, DirectEventArgs e)
        {
            LlenaFormPanelDirecciones(int.Parse(this.hdnIdColectiva.Text));
        }

        /// <summary>
        /// Llena el panel de Domicilios con los datos consultados en base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        protected void LlenaFormPanelDirecciones(int IdColectiva)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                log.Info("INICIA ConsultaDireccionColectiva()");
                DataSet dsDireccion = DAOAdministrarColectivas.ConsultaDireccionColectiva(IdColectiva,
                    String.IsNullOrEmpty(cBoxTipoDomicilio.SelectedItem.Value) ? 1 :
                    int.Parse(cBoxTipoDomicilio.SelectedItem.Value), LH_ParabAdminColectivas);
                log.Info("TERMINA ConsultaDireccionColectiva()");

                txtID_Direccion.Text = dsDireccion.Tables[0].Rows[0]["ID_Direccion"].ToString().Trim();

                txtCalle.Text           =   dsDireccion.Tables[0].Rows[0]["Calle"].ToString().Trim();
                txtNumExterior.Text     =   dsDireccion.Tables[0].Rows[0]["NumExterior"].ToString().Trim();
                txtNumInterior.Text     =   dsDireccion.Tables[0].Rows[0]["NumInterior"].ToString().Trim();

                txtEntreCalles.Text     =   dsDireccion.Tables[0].Rows[0]["EntreCalles"].ToString().Trim();
                txtReferencias.Text     =   dsDireccion.Tables[0].Rows[0]["Referencias"].ToString().Trim();
                txtCP.Text              =   dsDireccion.Tables[0].Rows[0]["CodigoPostal"].ToString().Trim();

                LlenaComboColonias();

                txtIDColonia.Text              =   dsDireccion.Tables[0].Rows[0]["ID_Asentamiento"].ToString().Trim();
                cBoxColonia.SelectedItem.Text  =   dsDireccion.Tables[0].Rows[0]["Colonia"].ToString().Trim();

                cBoxTipoDomicilio.Disabled = false;
                btnGuardaDireccion.Disabled = false;
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Domicilios", "Ocurrió un error al obtener el Domicilio de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Valida los campos del formulario de domicilio, si están todos vacíos
        /// </summary>
        /// <returns>TRUE en caso de que estén todos vacíos</returns>
        protected bool ValidaDatosDireccion()
        {
            if (String.IsNullOrEmpty(cBoxColonia.SelectedItem.Value) &&
                String.IsNullOrEmpty(txtCalle.Text) &&
                String.IsNullOrEmpty(txtNumExterior.Text) &&
                String.IsNullOrEmpty(txtNumInterior.Text) &&
                String.IsNullOrEmpty(txtEntreCalles.Text) &&
                String.IsNullOrEmpty(txtReferencias.Text) &&
                String.IsNullOrEmpty(txtCP.Text) &&
                String.IsNullOrEmpty(cBoxColonia.SelectedItem.Value) &&
                String.IsNullOrEmpty(txtClaveMunicipio.Text) &&
                String.IsNullOrEmpty(txtClaveEstado.Text) &&
                String.IsNullOrEmpty(txtCP.Text) &&
                String.IsNullOrEmpty(txtCP.Text))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de Datos, invocando la actualización
        /// de los datos del cliente en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaDireccion_Click(object sender, EventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                DireccionColectiva laDireccion = new DireccionColectiva();

                laDireccion.ID_Colectiva = int.Parse(hdnIdColectiva.Text);
                laDireccion.ID_Direccion = int.Parse(String.IsNullOrEmpty(txtID_Direccion.Text) ?
                                                    "0" : txtID_Direccion.Text);
                laDireccion.ID_TipoDireccion = int.Parse(cBoxTipoDomicilio.SelectedItem.Value);

                if (ValidaDatosDireccion())
                {
                    X.Msg.Alert("Domicilio", "Ingrese al menos la información de un campo").Show();
                    return;
                }

                laDireccion.ID_Asentamiento = String.IsNullOrEmpty(cBoxColonia.SelectedItem.Value) ? -1 :
                    cBoxColonia.SelectedItem.Value == cBoxColonia.SelectedItem.Text ?
                        int.Parse(txtIDColonia.Text) : int.Parse(cBoxColonia.SelectedItem.Value);

                laDireccion.Calle = txtCalle.Text;
                laDireccion.NumExterior = txtNumExterior.Text;
                laDireccion.NumInterior = txtNumInterior.Text;
                laDireccion.EntreCalles = txtEntreCalles.Text;
                laDireccion.Referencias = txtReferencias.Text;

                laDireccion.Colonia = cBoxColonia.SelectedItem.Text == "Otra" ?
                    txtColonia.Text : cBoxColonia.SelectedItem.Text;

                laDireccion.CodigoPostal = txtCP.Text;
                laDireccion.ClaveMunicipio = txtClaveMunicipio.Text;
                laDireccion.ClaveEstado = txtClaveEstado.Text;

                unLog.Info("INICIA ModificaDireccionColectiva()");
                LNAdministrarColectivas.ModificaDireccionColectiva(laDireccion, this.Usuario, LH_ParabAdminColectivas);
                unLog.Info("TERMINA ModificaDireccionColectiva()");

                X.Msg.Notify("", "Domicilio Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                ActualizaComboColonias();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Domicilio", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                X.Msg.Alert("Domicilio", "Ocurrió un error al guardar el Domicilio").Show();
            }
        }

        /// <summary>
        /// Llena el panel de Cuentas con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        protected void LlenaFormPanelCuentas(int IdColectiva)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                this.FormPanelNuevaCuenta.Reset();

                logPCI.Info("INICIA ListaTiposCuenta()");
                this.StoreTipoCuenta.DataSource = 
                    DAOAdministrarColectivas.ListaTiposCuenta(IdColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                logPCI.Info("TERMINA ListaTiposCuenta()");
                this.StoreTipoCuenta.DataBind();

                logPCI.Info("INICIA ConsultaCuentasColectiva()");
                StoreCuentas.DataSource = DAOAdministrarColectivas.ConsultaCuentasColectiva(IdColectiva,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                logPCI.Info("TERMINA ConsultaCuentasColectiva()");

                StoreCuentas.DataBind();

                btnNuevaCuenta.Disabled = false;
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Cuentas", "Ocurrió un error al obtener las Cuentas de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaCuenta_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                DALPuntoVentaWeb.Entidades.Cuenta laCuenta = new DALPuntoVentaWeb.Entidades.Cuenta();

                laCuenta.ID_Colectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                laCuenta.ID_TipoCuenta = int.Parse(this.cBoxTipoCuenta.SelectedItem.Value);
                laCuenta.Descripcion = this.txtDescCuenta.Text;
                laCuenta.Vigencia = Convert.ToDateTime(this.dfVigenciaCuenta.SelectedDate);

                log.Info("INICIA CreaNuevaCuentaColectiva()");
                LNAdministrarColectivas.CreaNuevaCuentaColectiva(laCuenta, this.Usuario, LH_ParabAdminColectivas);
                log.Info("TERMINA CreaNuevaCuentaColectiva()");

                X.Msg.Notify("", "Cuenta Añadida" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaFormPanelCuentas(int.Parse(this.hdnIdColectiva.Text));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Añadir Cuenta", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Añadir Cuenta", "Ocurrió un error al crear la Cuenta de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                Int64 IdCuenta = Convert.ToInt64(e.ExtraParams["ID_Cuenta"]);
                char[] charsToTrim = { '*', '"', ' ' };

                switch (comando)
                {
                    case "Edit":
                        FormPanelCuenta.Reset();

                        pCI.Info("INICIA ConsultaDatosExtraCuenta()");
                        DataSet dsInfoCuenta = DAOAdministrarColectivas.ConsultaDatosExtraCuenta(
                            IdCuenta, LH_ParabAdminColectivas);
                        pCI.Info("TERMINA ConsultaDatosExtraCuenta()");

                        this.hdnIdCuenta.Text       =   IdCuenta.ToString();
                        this.cBoxTipoCta.Value      =   e.ExtraParams["ID_TipoCuenta"];

                        this.txtCuenta.Text = e.ExtraParams["Descripcion"].Trim(charsToTrim);

                        if (!String.IsNullOrEmpty(dsInfoCuenta.Tables[0].Rows[0]["Vigencia"].ToString()))
                        {
                            this.dfVigCuenta.SetValue(Convert.ToDateTime(dsInfoCuenta.Tables[0].Rows[0]["Vigencia"]));
                        }

                        WdwCuenta.Show();
                        break;

                    case "Lock":
                    case "Unlock":
                        String claveEstatus = e.ExtraParams["ClaveEstatus"].Trim(charsToTrim);
                        String msj = claveEstatus == "ACT" ? "Bloqueada" : "Desbloqueada";

                        pCI.Info("INICIA BloqueaDesbloqueaCuenta()");
                        LNAdministrarColectivas.BloqueaDesbloqueaCuenta(IdCuenta, claveEstatus, this.Usuario, LH_ParabAdminColectivas);
                        pCI.Info("TERMINA BloqueaDesbloqueaCuenta()");

                        X.Msg.Notify("", "Cuenta " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaFormPanelCuentas(int.Parse(this.hdnIdColectiva.Text));
                        break;

                    default: break;
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Cuenta", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Cuenta", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambios de la ventana de edición de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaCuenta_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                DALPuntoVentaWeb.Entidades.Cuenta laCuenta = new DALPuntoVentaWeb.Entidades.Cuenta();

                laCuenta.ID_Cuenta = Convert.ToInt64(this.hdnIdCuenta.Text);
                laCuenta.ID_Colectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                laCuenta.Descripcion = this.txtCuenta.Text;
                laCuenta.Vigencia = Convert.ToDateTime(this.dfVigCuenta.SelectedDate);

                logPCI.Info("INICIA ModificaCuentaColectiva()");
                LNAdministrarColectivas.ModificaCuentaColectiva(laCuenta, this.Usuario, LH_ParabAdminColectivas);
                logPCI.Info("TERMINA ModificaCuentaColectiva()");

                X.Msg.Notify("", "Cuenta Modificada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                WdwCuenta.Hide();
                LlenaFormPanelCuentas(int.Parse(this.hdnIdColectiva.Text));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Editar Cuenta", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Editar Cuenta", "Ocurrió un error al guardar la Cuenta").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de clasificación
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaClasificacion(object sender, EventArgs e)
        {
            LlenaParametrosColectiva();
        }
        
        /// <summary>
        /// Establece los valores de los controles de parámetros de la colectiva,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosColectiva()
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                Int64 IdColectiva = Convert.ToInt64(hdnIdColectiva.Text);

                log.Info("INICIA ListaParametrosSinAsignar()");
                this.StoreParametros.DataSource = DAOAdministrarColectivas.ListaParametrosSinAsignar(
                    int.Parse(cBoxClasificacion.SelectedItem.Value), IdColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                log.Info("TERMINA ListaParametrosSinAsignar()");
                this.StoreParametros.DataBind();

                log.Info("INICIA ListaParametrosAsignados()");
                this.StoreValoresParametros.DataSource = DAOAdministrarColectivas.ListaParametrosAsignados(
                    int.Parse(cBoxClasificacion.SelectedItem.Value), IdColectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ParabAdminColectivas);
                log.Info("TERMINA ListaParametrosAsignados()");
                this.StoreValoresParametros.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Parámetros", "Ocurrió un error al obtener los Paámetros de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametros_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                long IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                log.Info("INICIA AgregaParametroAContrato()");
                string resp = LNAdministrarColectivas.AgregaParametroAContrato(
                    int.Parse(cBoxParametros.SelectedItem.Value), IdColectiva, this.Usuario,
                    LH_ParabAdminColectivas);
                log.Info("TERMINA AgregaParametroAContrato()");

                cBoxParametros.ClearValue();

                if (resp.ToUpper().Contains("ERROR"))
                {
                    X.Msg.Notify("Asignación de Parámetro", "<b>" + resp + "</b> <br /> <br /> ").Show();
                }
                else
                {
                    X.Msg.Notify("", "Parámetro Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    this.cBoxClasificacion.FireEvent("select");
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro", "Ocurrió un error al asignar el Parámetro a la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                Parametro unParametro = new Parametro();
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
                        case "ID_Parametro": unParametro.ID_Parametro = int.Parse(column.Value); break;
                        case "Nombre": unParametro.Nombre = column.Value; break;
                        case "Descripcion": unParametro.Descripcion = column.Value; break;
                        case "Valor": unParametro.Valor = column.Value; break;
                        case "ValorPrestablecido": unParametro.Preestablecido = bool.Parse(column.Value); break;
                        case "ID_ValorPrestablecido": unParametro.ID_ParametroPrestablecido = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                String comando = e.ExtraParams["Comando"];
                hdnIdParametro.Value = unParametro.ID_Parametro;
                hdnParametroNombre.Value = unParametro.Nombre;

                switch (comando)
                {
                    case "Edit":
                        if (unParametro.Preestablecido)
                        {
                            StoreValoresPrestablecidos.RemoveAll();

                            log.Info("INICIA ListaCatalogoValoresParametro()");
                            StoreValoresPrestablecidos.DataSource =
                                DAOAdministrarColectivas.ListaCatalogoValoresParametro(
                                unParametro.ID_Parametro, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_ParabAdminColectivas);
                            log.Info("TERMINA ListaCatalogoValoresParametro()");
                            StoreValoresPrestablecidos.DataBind();

                            cBoxValorParametro.Value = unParametro.ID_ParametroPrestablecido;
                            txtParametroCombo.Text = unParametro.Descripcion;
                            WdwValorParametroCombo.Title += " - " + unParametro.Nombre;
                            WdwValorParametroCombo.Show();
                        }
                        else
                        {
                            FormPanelValorParamTxt.Reset();

                            txtValorParametro.Text = unParametro.Valor;
                            txtParametro.Text = unParametro.Descripcion;
                            WdwValorParametroTexto.Title += " - " + unParametro.Nombre;
                            WdwValorParametroTexto.Show();
                        }

                        break;

                    case "Delete":
                        Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                        log.Info("INICIA BorraParametroAColectiva()");
                        LNAdministrarColectivas.BorraParametroAColectiva(unParametro.ID_Parametro,
                            IdColectiva, this.Usuario, LH_ParabAdminColectivas);
                        log.Info("TERMINA BorraParametroAColectiva()");

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        this.cBoxClasificacion.FireEvent("select");
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
                log.ErrorException(ex);
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
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                //ActualizaValorParametro();
                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                Parametro elParametro = new Parametro();
                String Origen = hdnOrigen.Value.ToString();

                elParametro.ID_Colectiva = IdColectiva;
                elParametro.ID_Parametro = int.Parse(this.hdnIdParametro.Value.ToString());
                elParametro.Valor = Origen == "CMB" ? this.cBoxValorParametro.SelectedItem.Value : this.txtValorParametro.Text;
                elParametro.ID_ParametroPrestablecido = Origen == "CMB" ?
                    int.Parse(this.cBoxValorParametro.SelectedItem.Value) : -1;

                pCI.Info("INICIA ModificaValorParametro()");
                LNAdministrarColectivas.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminColectivas);
                pCI.Info("TERMINA ModificaValorParametro()");

                this.WdwValorParametroTexto.Hide();
                this.WdwValorParametroCombo.Hide();

                this.cBoxClasificacion.FireEvent("select");

                X.Msg.Notify("Actualización de Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros", "Ocurrió un error al actualizar el valor del Parámetro").Show();
            }
        }

        /// <summary>
        /// Llena el panel de CLABE con los datos consultados en base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        protected void LlenaFormPanelCLABE(int IdColectiva)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                log.Info("INICIA ConsultaCLABEColectiva()");
                DataSet dsDireccion = DAOAdministrarColectivas.ConsultaMediosAccesoColectiva(IdColectiva, "CLABE",
                   this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminColectivas);
                log.Info("TERMINA ConsultaCLABEColectiva()");

                if (dsDireccion.Tables[0].Rows.Count > 0)
                {
                    txtCuentaCLABE.Text = dsDireccion.Tables[0].Rows[0]["ClaveMA"].ToString().Trim();
                }
                else
                {
                    txtCuentaCLABE.Clear();
                }
                
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("CLABE", "Ocurrió un error al obtener la CLABE de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Actualiza Cuenta CLABE
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnActualizaCuentaCLABE_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                log.Info("INICIA ActualizaCuentaCLABEColectiva()");
                LNAdministrarColectivas.ActualizarCuentaCLABEColectiva(
                    Convert.ToInt64(this.hdnIdColectiva.Text), txtCuentaCLABE.Text,
                    this.Usuario, LH_ParabAdminColectivas);
                log.Info("TERMINA ActualizaCuentaCLABEColectiva()");

                X.Msg.Notify("", "Cuenta CLABE Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualizar Cuenta CLABE", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Actualizar Cuenta CLABE", "Ocurrió un error al actualizar la Cuenta CLABE de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir al Catálogo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAgregarCat_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                log.Info("INICIA CreaNuevoItemCatalogoPMAColectiva()");
                LNAdministrarColectivas.CreaNuevoItemCatalogoPMAColectiva(
                    Convert.ToInt64(this.cBoxCatalogo.SelectedItem.Value), Convert.ToInt32(this.hdnIdColectiva.Value),
                    this.txtClaveCat.Text, this.txtDescCat.Text, this.Usuario, LH_ParabAdminColectivas);
                log.Info("TERMINA CreaNuevoItemCatalogoPMAColectiva()");

                X.Msg.Notify("", "Elemento Añadido" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.FormPanelNuevoCat.Reset();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Añadir al Catálogo", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Añadir al Catálogo", "Ocurrió un error al añadir el elemento al Catálogo de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Select a los ítems del combo de Emisor
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceItemsCat(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                logPCI.Info("INICIA ListaElementosCatalogo()");
                this.StoreElemsCat.DataSource = 
                    DAOAdministrarColectivas.ListaElementosCatalogo(Convert.ToInt32(this.hdnIdColectiva.Value),
                    Convert.ToInt64(this.cBoxCatGrid.SelectedItem.Value), LH_ParabAdminColectivas);
                logPCI.Info("TERMINA ListaElementosCatalogo()");

                this.StoreElemsCat.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Catálogo", err.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Catálogo", "Ocurrió un error al establecer los elementos del Catálogo.").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de catálogos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void ActDesacCat(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                string comando = e.ExtraParams["Comando"].ToString();
                int id_catalogo = Convert.ToInt32(e.ExtraParams["ID_ValorPreePMA"]);
                bool estatusActual = Convert.ToBoolean(e.ExtraParams["Activo"]);

                switch (comando)
                {
                    case "Act":
                    case "Desact":
                        string msj = estatusActual == true ? "Desactivado" : "Activado";

                        pCI.Info("INICIA ActivaDesactivaItemCatalogo()");
                        LNAdministrarColectivas.ActivaDesactivaItemCatalogo(id_catalogo, this.Usuario, LH_ParabAdminColectivas);
                        pCI.Info("TERMINA ActivaDesactivaItemCatalogo()");

                        X.Msg.Notify("", "Elemento del Catálogo " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        this.cBoxCatGrid.FireEvent("Select");
                        break;

                    default: break;
                }
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Catálogo", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Catálogo", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }


        /// <summary>
        /// Controla el evento Select a los ítems del combo de tipos de parámetros adicionales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void SeleccionaTipoPMA(object sender, EventArgs e)
        {
            LlenaParametrosAdicionales();
        }

        /// <summary>
        /// Establece los valores de los controles de parámetros Adicionales de la colectiva,
        /// llamando a los objetos de datos que obtienen los valores
        /// </summary>
        protected void LlenaParametrosAdicionales()
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                int IdColectiva = Convert.ToInt32(this.hdnIdColectiva.Value);

                pCI.Info("INICIA ObtienePMASinAsignar()");
                StoreParametrosMA.DataSource = DAOAdministrarColectivas.ObtienePMASinAsignar(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdColectiva, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminColectivas);
                pCI.Info("TERMINA ObtienePMASinAsignar()"); 
                StoreParametrosMA.DataBind();

                pCI.Info("INICIA ObtieneParametrosMA()");
                StoreValoresParametrosMA.DataSource = DAOAdministrarColectivas.ObtieneParametrosMA(
                    int.Parse(this.cBoxTipoParametroMA.SelectedItem.Value), IdColectiva, LH_ParabAdminColectivas);
                pCI.Info("TERMINA ObtieneParametrosMA()");
                StoreValoresParametrosMA.DataBind();
            }
            catch (CAppException caEx)
            {
                X.Msg.Alert("Parámetros Adicionales", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros Adicionales", "Ocurrió un error al obtener los Parámetros Adicionales de la Colectiva").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Asignar Parámetro de la pestaña de Parámetros Adicionales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParametrosMA_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                long IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                log.Info("INICIA AgregaParametroMAAColectiva Adicionales()");
                LNAdministrarColectivas.AgregaParametroAdicionalColectiva(Convert.ToInt32(cBoxParametrosMA.SelectedItem.Value),
                    IdColectiva, this.Usuario, LH_ParabAdminColectivas);
                log.Info("TERMINA AgregaParametroMAAColectiva Adicionales()");

                X.Msg.Notify("", "Parámetro Adicional Asignado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.cBoxParametrosMA.Reset();

                LlenaParametrosAdicionales();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Asignación de Parámetro Adicional", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.ErrorException(ex);
                X.Msg.Alert("Asignación de Parámetro Adicional", "Ocurrió un error al asignar el valor al Parámetro").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros adicionales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametrosMA(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ParabAdminColectivas);
            pCI.Info("EjecutarComandoParametrosMA Adicionales()");

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
                        LimpiaVentanaParamsMA();

                        if (!string.IsNullOrEmpty(unParametro.TipoValidacion) && unParametro.TipoValidacion.Contains("CAT")) //Clave fija de tipo de validación CATALOGO
                        {
                            pCI.Info("INICIA ListaElementosCatalogoPMA()");
                            this.StoreCatalogoPMA.DataSource = 
                                DAOProducto.ListaElementosCatalogoPMA(Convert.ToInt64(this.hdnIdColectiva.Value), 
                                Convert.ToInt64(unParametro.ID_Parametro), string.Empty, this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ParabAdminColectivas);
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
                                    this.cBoxValorParMA.Value = unParametro.Valor;
                                    this.cBoxValorParMA.Hidden = false;
                                    break;

                                case "FLOAT":
                                    this.txtValorParFloatMA.Value = unParametro.Valor;
                                    this.txtValorParFloatMA.Hidden = false;
                                    break;

                                case "INT":
                                    this.txtValorParIntMA.Value = unParametro.Valor;
                                    this.txtValorParIntMA.Hidden = false;
                                    break;

                                case "STRING":
                                    this.txtValorParStringMA.Value = unParametro.Valor;
                                    this.txtValorParStringMA.Hidden = false;
                                    break;
                            }
                        }

                        this.txtParametroMA.Text = unParametro.Descripcion;
                        this.WdwValorParametroMA.Title += " - " + unParametro.Nombre;
                        this.WdwValorParametroMA.Show();
                        break;

                    case "Delete":
                        pCI.Info("INICIA BorraValorParametro()");
                        LNProducto.BorraValorParametro((int)unParametro.ID_ValordelParametro, this.Usuario, LH_ParabAdminColectivas, "_ColectivasAdicionales");
                        pCI.Info("TERMINA BorraValorParametro()");

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaParametrosAdicionales();
                        break;

                    default: break;
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Parámetros Adicionales", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Parámetros Adicionales", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }

        protected void LimpiaVentanaParamsMA()
        {
            this.FormPanelValorParamTxtMA.Reset();
            this.txtParametroMA.Reset();

            this.txtValorParFloatMA.Reset();
            this.txtValorParFloatMA.Hidden = true;

            this.txtValorParIntMA.Reset();
            this.txtValorParIntMA.Hidden = true;

            this.txtValorParStringMA.Reset();
            this.txtValorParStringMA.Hidden = true;

            this.cBoxValorParMA.Reset();
            this.cBoxValorParMA.Hidden = true;

            this.StoreCatalogoPMA.RemoveAll();
            this.cBoxCatalogoPMA.Reset();
            this.cBoxCatalogoPMA.Hidden = true;
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambio de las ventanas
        /// de edición de valor del parámetro adicional de colectivas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorParametroMA_Click(object sender, EventArgs e)
        {
            LogPCI logPCI = new LogPCI(LH_ParabAdminColectivas);

            try
            {
                ParametroValor elParametro = new ParametroValor();

                elParametro.ID_Parametro = Convert.ToInt32(this.hdnIdParametroMA.Value);
                elParametro.ID_Plantilla = Convert.ToInt32(this.hdnIdPlantilla.Value);
                elParametro.ID_ValordelParametro = Convert.ToInt64(this.hdnIdValorPMA.Value);
                elParametro.Valor = String.IsNullOrEmpty(this.txtValorParFloatMA.Text) ?
                    string.IsNullOrEmpty(this.txtValorParIntMA.Text) ?
                    string.IsNullOrEmpty(this.txtValorParStringMA.Text) ?
                    string.IsNullOrEmpty(this.cBoxValorParMA.SelectedItem.Value) ?
                    this.cBoxCatalogoPMA.SelectedItem.Value : this.cBoxValorParMA.SelectedItem.Value :
                    this.txtValorParStringMA.Text : this.txtValorParIntMA.Text :
                    this.txtValorParFloatMA.Text;

                logPCI.Info("INICIA ModificaValorParametro Adicional()");
                LNProducto.ModificaValorParametro(elParametro, this.Usuario, LH_ParabAdminColectivas, "_ColectivasAdicionales");
                logPCI.Info("TERMINA ModificaValorParametro Adicional()");

                WdwValorParametroMA.Hide();

                X.Msg.Notify("Parámetros Adicionales", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

                LlenaParametrosAdicionales();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Parámetros Adicionales", err.Mensaje() + " Cod.(" + err.CodigoError().ToString() + ")").Show();
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                X.Msg.Alert("Actualización de Parámetros Adicionales", "Ocurrió un error al establecer el valor del Parámetro").Show();
            }
        }

    }
}
