using DALAdministracion.BaseDatos;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.LogicaNegocio;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using DALPuntoVentaWeb.LogicaNegocio;
using DALPuntoVentaWeb.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace TpvWeb
{
    public partial class AdministrarColectivasTAE : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administrar Colectivas para TAE
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataSet dsTiposColectiva = DAOColectiva.ListaTiposDeColectivaTAE
                        (this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                    this.StoreTipoColectiva.DataSource = dsTiposColectiva;
                    this.StoreTipoColectiva.DataBind();

                    this.StoreEstatus.DataSource =
                        DAOAdministrarColectivas.ListaEstatusColectivas(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreEstatus.DataBind();
                    
                    this.StoreTipoCuenta.DataSource = DAOAdministrarColectivas.ListaTiposCuenta
                        (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreTipoCuenta.DataBind();

                    this.StoreGrupoCuentas.DataSource = DAOProducto.ListaGruposCuenta
                        (this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreGrupoCuentas.DataBind();
                    
                    ValidaPermisosCadenas(dsTiposColectiva);

                    dfFechaNac.MaxDate = DateTime.Today;
                    cBoxTipoDomicilio.SetValue(1);

                    dfVigenciaCuenta.MinDate = DateTime.Today;
                    dfVigCuenta.MinDate = DateTime.Today;
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Valida que entre los tipos de colectiva permitidos al usuario 
        /// esté el de Cadena Comercial
        /// </summary>
        /// <param name="ds">DataSet con los tipos de colectiva permitidos para el usuario</param>
        protected void ValidaPermisosCadenas(DataSet ds)
        {
            DataRow[] dr = ds.Tables[0].Select(string.Format("{0} LIKE '%{1}%'", "Clave", "CCM"));

            //Sí tiene permisos
            if (dr.Length == 1)
            {
                DataSet dsCadenaComercial = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(dr[0].ItemArray[0].ToString()), "", this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

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
                this.cBoxCadComerCuentas.Disabled = true;
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
            try
            {
                DataTable dt = LNAdministrarColectivas.CreaNuevaColectivaEnAutorizador(
                    int.Parse(this.cBoxTipoColectiva.SelectedItem.Value),
                    Convert.ToInt32(this.cBoxColecPadre.SelectedItem.Value),
                    this.txtClaveColectiva.Text, this.txtRazonSocial.Text,
                    this.txtNombreComercial.Text, this.Usuario);

                string msj = dt.Rows[0]["Mensaje"].ToString();
                string idColectivaNueva = dt.Rows[0]["ID_NuevaColectiva"].ToString();

                if (idColectivaNueva == "-1")
                {
                    X.Msg.Alert("Nueva Colectiva", msj).Show();
                }
                else
                {   
                    PermisosNuevaColectiva(idColectivaNueva);

                    WdwNuevaColectiva.Hide();

                    cBoxTipoColec.SetValue(cBoxTipoColectiva.SelectedItem.Value);
                    cBoxTipoColec.SelectedItem.Text = cBoxTipoColectiva.SelectedItem.Text;
                    txtColectiva.Text = txtClaveColectiva.Text;                
                    LlenaGridResultados();

                    X.Msg.Alert("Nueva Colectiva", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
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
                X.Msg.Alert("Nueva Colectiva", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Nueva Colectiva", ex.Message).Show();
            }
        }

        /// <summary>
        /// Valida si el tipo de colectiva recién creado requiere de permisos para el usuario
        /// en sesión, y de ser así, solicita los permisos
        /// </summary>
        /// <param name="IdNuevaColectiva">Identificador de la nueva colectiva</param>
        protected void PermisosNuevaColectiva(string IdNuevaColectiva)
        {
            try
            {
                Guid TableIdColectiva =
                    DALCentralAplicaciones.BaseDatos.DAOCatalogos.ObtieneFiltrosParaAltaColectiva(
                    hdnClaveTipoColectiva.Value.ToString().Trim(),
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                //Si el tipo de colectiva recién creado requiere permisos
                if (TableIdColectiva != new Guid())
                {
                    //Se crean los permisos para el usuario
                    LNUsuarios.CreaPermisoTableValue(this.Usuario.UsuarioId, TableIdColectiva,
                        IdNuevaColectiva, true, this.Usuario);

                    //Se otrogan el permiso inmediato al usuario en sesión
                    LNFiltro.AgregarFiltrosEnSesion(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                }
            }

            catch (CAppException err)
            {
                throw err;
            }

            catch (Exception ex)
            {
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
            //this.cBoxTColecAsc.Disabled = Activa;
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
            try
            {
                int IdTipoColectivaPadre;
                DataSet dsTiposColectivaPadre = new DataSet();
                DataSet dsColectivasPadre = new DataSet();

                dsTiposColectivaPadre = DAOAdministrarColectivas.ListaTiposColectivaPadre(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                
                ActivaDesactivaAscendencia(true);

                foreach (DataRow dr in dsTiposColectivaPadre.Tables[0].Rows)
                {
                    if (this.cBoxTipoColectiva.SelectedItem.Value == dr["IdTipoColectiva"].ToString())
                    {
                        txtTColecAsc.Text = dr["ColectivaPadre"].ToString();
                        IdTipoColectivaPadre = int.Parse(dr["ID_TipoColectivaPadre"].ToString());

                        dsColectivasPadre = DAOColectiva.ListaColectivasPorTipo(
                            IdTipoColectivaPadre, "", this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

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
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Ascendencia", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Ascendencia", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el grid de resultados de colectivas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            try
            {
                LimpiaSeleccionPrevia();

                DataSet dsColectivas = DAOColectiva.ListaColectivasPorTipo(
                    int.Parse(this.cBoxTipoColec.SelectedItem.Value), this.txtColectiva.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

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
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Colectivas", ex.Message).Show();
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
            FormPanelInfoAd.Reset();
            btnGuardarInfoAd.Disabled = true;

            LimpiaFormPanelDirecciones();

            FormPanelNuevaCuenta.Reset();
            StoreCuentas.RemoveAll();
            btnNuevaCuenta.Disabled = true;

            cBoxColectivaOrigen.Reset();
            cBoxValoresContrato.Reset();
            StoreValoresContrato.RemoveAll();
            StoreParamsContrato.RemoveAll();

            cBoxParamsExtra.Reset();
            StoreParamsExtra.RemoveAll();
            StoreGridParamsExtra.RemoveAll();
            
            FormPanelInfoAd.Show();
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
                LlenaFormPanelContrato(IdColectiva, ClaveTipoColectiva);
                LlenaFormPanelParamsExtra(IdColectiva);
                LlenaFormPanelPtsBancarios(IdColectiva);

                PanelCentral.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Colectivas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Colectivas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="ClaveColectiva">Clave de la colectiva</param>
        protected void LlenaFormPanelInfoAd(int IdColectiva, string ClaveColectiva)
        {
            try
            {
                DataSet dsColectiva = DAOAdministrarColectivas.ListaInfoColectiva(IdColectiva, this.Usuario);

                txtClaveCol.Text    =   ClaveColectiva;
                txtRazonSoc.Text    =   dsColectiva.Tables[0].Rows[0]["NombreORazonSocial"].ToString().Trim();
                txtNombreCom.Text   =   dsColectiva.Tables[0].Rows[0]["NombreComercial"].ToString().Trim();
                txtApPaterno.Text   =   dsColectiva.Tables[0].Rows[0]["APaterno"].ToString().Trim();
                txtApMaterno.Text   =   dsColectiva.Tables[0].Rows[0]["AMaterno"].ToString().Trim();
                dfFechaNac.Value    =   String.IsNullOrEmpty(dsColectiva.Tables[0].Rows[0]["FechaNacimiento"].ToString().Trim()) ?
                                                    "" : dsColectiva.Tables[0].Rows[0]["FechaNacimiento"].ToString().Trim();

                txtCURP.Text   =   dsColectiva.Tables[0].Rows[0]["CURP"].ToString().Trim();
                txtRFC.Text    =   dsColectiva.Tables[0].Rows[0]["RFC"].ToString().Trim();
                txtEmail.Text  =   dsColectiva.Tables[0].Rows[0]["Email"].ToString().Trim();

                nfTelefonoFijo.Text    =   dsColectiva.Tables[0].Rows[0]["Telefono"].ToString().Trim();
                nfTelefonoFijo.Value   =   String.IsNullOrEmpty(dsColectiva.Tables[0].Rows[0]["Telefono"].ToString().Trim())
                                                    ? "0" : dsColectiva.Tables[0].Rows[0]["Telefono"].ToString().Trim();

                nfTelefonoCel.Text     =   dsColectiva.Tables[0].Rows[0]["Movil"].ToString().Trim();
                nfTelefonoCel.Value    =   String.IsNullOrEmpty(dsColectiva.Tables[0].Rows[0]["Movil"].ToString().Trim())
                                                    ? "0" : dsColectiva.Tables[0].Rows[0]["Movil"].ToString().Trim();

                cBoxCadena.SelectedItem.Text   =   dsColectiva.Tables[0].Rows[0]["CadenaRelacionada"].ToString().Trim();
                cBoxCadena.SelectedItem.Value  =   dsColectiva.Tables[0].Rows[0]["ID_CadenaComercial"].ToString().Trim();

                cBoxEstatus.SelectedItem.Text  =   dsColectiva.Tables[0].Rows[0]["Estatus"].ToString().Trim();
                cBoxEstatus.Value              =   dsColectiva.Tables[0].Rows[0]["ID_EstatusColectiva"].ToString().Trim();

                btnGuardarInfoAd.Disabled = false;
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Información Adicional", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Información Adicional", ex.Message).Show();
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
            try
            {
                DALPuntoVentaWeb.Entidades.Colectiva colectiva = new DALPuntoVentaWeb.Entidades.Colectiva();

                colectiva.ID_Colectiva          = int.Parse(this.hdnIdColectiva.Text);
                colectiva.ClaveColectiva        = this.txtClaveCol.Text;
                colectiva.NombreORazonSocial    = this.txtRazonSoc.Text;
                colectiva.APaterno              = this.txtApPaterno.Text;
                colectiva.AMaterno              = this.txtApMaterno.Text;
                colectiva.FechaNacimiento       = dfFechaNac.SelectedDate.Equals(DateTime.MinValue) ? 
                    DateTime.Today.AddDays(1) : Convert.ToDateTime(dfFechaNac.SelectedDate);
                colectiva.CURP                  = this.txtCURP.Text;
                colectiva.RFC                   = this.txtRFC.Text;
                colectiva.Email                 = this.txtEmail.Text;
                colectiva.Telefono              = this.nfTelefonoFijo.Text;
                colectiva.Movil                 = this.nfTelefonoCel.Text;
                colectiva.IdCadenaRelacionada   =   String.IsNullOrEmpty(this.cBoxCadena.SelectedItem.Value) ? 0 :
                                                        Convert.ToInt64(this.cBoxCadena.SelectedItem.Value);
                colectiva.IdEstatus             = Convert.ToInt32(this.cBoxEstatus.SelectedItem.Value);
                colectiva.Password              = this.txtPassword.Text;
                colectiva.RePassword            = this.txtRePassword.Text;

                LNAdministrarColectivas.ModificaColectivaEnAutorizador(colectiva, this.Usuario);

                X.Msg.Notify("", "Colectiva Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Colectiva", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Colectiva", ex.Message).Show();
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
            try
            {
                DataSet dsColonias = DAOAdministrarColectivas.ListaDatosPorCodigoPostal(
                    this.txtCP.Text, this.Usuario);
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
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Domicilios", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Domicilios", ex.Message).Show();
            }
        }
        /// <summary>
        /// Llena el combo de colonias y los campos de municipio y estado, con información de base de datos
        /// </summary>
        [DirectMethod(Namespace = "TpvWeb")]
        public void LlenaComboColonias()
        {
            try
            {
                LimpiaDatosRelacionadosCP();

                if (!String.IsNullOrEmpty(txtCP.Text))
                {
                    string resp = DAOAdministrarColectivas.ValidaCodigoPostal(txtCP.Text, this.Usuario);

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
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Domicilios", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Domicilios", ex.Message).Show();
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
            try
            {
                DataSet dsDireccion = DAOAdministrarColectivas.ConsultaDireccionColectiva(IdColectiva,
                    String.IsNullOrEmpty(cBoxTipoDomicilio.SelectedItem.Value) ? 1 :
                    int.Parse(cBoxTipoDomicilio.SelectedItem.Value), this.Usuario);

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
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Domicilios", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Domicilios", ex.Message).Show();
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

                LNAdministrarColectivas.ModificaDireccionColectiva(laDireccion, this.Usuario);

                X.Msg.Notify("", "Domicilio Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                ActualizaComboColonias();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Domicilio", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Domicilio", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Cuentas con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        protected void LlenaFormPanelCuentas(int IdColectiva)
        {
            try
            {
                FormPanelNuevaCuenta.Reset();

                StoreCuentas.DataSource = DAOAdministrarColectivas.ConsultaCuentasColectiva(IdColectiva,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreCuentas.DataBind();

                btnNuevaCuenta.Disabled = false;
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cuentas", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cuentas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevaCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 idCadena;
                DALPuntoVentaWeb.Entidades.Cuenta laCuenta = new DALPuntoVentaWeb.Entidades.Cuenta();

                laCuenta.ID_Colectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                laCuenta.ID_TipoCuenta = int.Parse(this.cBoxTipoCuenta.SelectedItem.Value);
                laCuenta.ID_GrupoCuentas = int.Parse(this.cBoxGpoCuentas.SelectedItem.Value);
                laCuenta.Descripcion = this.txtDescCuenta.Text;
                laCuenta.Vigencia = Convert.ToDateTime(this.dfVigenciaCuenta.SelectedDate);
                laCuenta.ID_ColectivaCadenaComercial = 
                    Int64.TryParse(this.cBoxCadComerCuentas.SelectedItem.Value, out idCadena) ?
                    idCadena : -1;

                LNAdministrarColectivas.CreaNuevaCuentaColectiva(laCuenta, this.Usuario);

                X.Msg.Notify("", "Cuenta Añadida" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaFormPanelCuentas(int.Parse(this.hdnIdColectiva.Text));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Añadir Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Añadir Cuenta", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                Int64 IdCuenta = Convert.ToInt64(e.ExtraParams["ID_Cuenta"]);
                char[] charsToTrim = { '*', '"', ' ' };

                switch (comando)
                {
                    case "Edit":
                        FormPanelCuenta.Reset();

                        this.StorePeriodo.DataSource = DAOAdministrarColectivas.ListaPeriodos(
                                this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                        this.StorePeriodo.DataBind();

                        DataSet dsInfoCuenta = DAOAdministrarColectivas.ConsultaDatosExtraCuenta(IdCuenta, this.Usuario);

                        this.hdnIdCuenta.Text       =   IdCuenta.ToString();
                        this.cBoxTipoCta.Value      =   e.ExtraParams["ID_TipoCuenta"];
                        this.cBoxGrupoCuentas.Value =   dsInfoCuenta.Tables[0].Rows[0]["ID_GrupoCuenta"].ToString();

                        this.txtCuenta.Text = e.ExtraParams["Descripcion"].Trim(charsToTrim);

                        if (!String.IsNullOrEmpty(dsInfoCuenta.Tables[0].Rows[0]["Vigencia"].ToString()))
                        {
                            this.dfVigCuenta.SetValue(Convert.ToDateTime(dsInfoCuenta.Tables[0].Rows[0]["Vigencia"]));
                        }

                        if (!String.IsNullOrEmpty(e.ExtraParams["ID_ColectivaCadenaComercial"]))
                        {
                            this.cBoxCCRCuenta.Value = e.ExtraParams["ID_ColectivaCadenaComercial"];
                        }

                        if (!String.IsNullOrEmpty(dsInfoCuenta.Tables[0].Rows[0]["Nivel"].ToString()))
                        {
                            this.nmbNivel.Text = dsInfoCuenta.Tables[0].Rows[0]["Nivel"].ToString();
                        }

                        this.cBoxPeriodo.Value = dsInfoCuenta.Tables[0].Rows[0]["ID_Periodo"].ToString();

                        if (!String.IsNullOrEmpty(dsInfoCuenta.Tables[0].Rows[0]["HeredaSaldo"].ToString()))
                        {
                            Boolean hereda = Convert.ToBoolean(dsInfoCuenta.Tables[0].Rows[0]["HeredaSaldo"]);
                            this.chkBoxSi.Checked = hereda ? true : false;
                            this.chkBoxNo.Checked = !hereda ? true : false;
                        }

                        WdwCuenta.Show();
                        break;

                    case "Lock":
                    case "Unlock":
                        String claveEstatus = e.ExtraParams["ClaveEstatus"].Trim(charsToTrim);
                        String msj = claveEstatus == "ACT" ? "Bloqueada" : "Desbloqueada";

                        LNAdministrarColectivas.BloqueaDesbloqueaCuenta(IdCuenta, claveEstatus, this.Usuario);

                        X.Msg.Notify("", "Cuenta " + msj + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaFormPanelCuentas(int.Parse(this.hdnIdColectiva.Text));
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Acción", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambios de la ventana de edición de cuenta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaCuenta_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 idCadena;
                Int32 nivel, periodo;
                DALPuntoVentaWeb.Entidades.Cuenta laCuenta = new DALPuntoVentaWeb.Entidades.Cuenta();

                laCuenta.ID_Cuenta = Convert.ToInt64(this.hdnIdCuenta.Text);
                laCuenta.ID_Colectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                laCuenta.ID_TipoCuenta = int.Parse(this.cBoxTipoCta.SelectedItem.Value);
                laCuenta.ID_GrupoCuentas = int.Parse(this.cBoxGrupoCuentas.SelectedItem.Value);
                laCuenta.Descripcion = this.txtCuenta.Text;
                laCuenta.Vigencia = Convert.ToDateTime(this.dfVigCuenta.SelectedDate);
                laCuenta.ID_ColectivaCadenaComercial = 
                    Int64.TryParse(this.cBoxCCRCuenta.SelectedItem.Value, out idCadena) ?
                    idCadena : -1;
                laCuenta.Nivel = Int32.TryParse(this.nmbNivel.Text, out nivel) ? nivel : -1;
                laCuenta.ID_Periodo = Int32.TryParse(this.cBoxPeriodo.SelectedItem.Value, out periodo) 
                    ? periodo : -1;
                laCuenta.HeredaSaldo = chkBoxSi.Checked ? 1 : chkBoxNo.Checked ? 0 : -1;

                LNAdministrarColectivas.ModificaCuentaColectiva(laCuenta, this.Usuario);

                X.Msg.Notify("", "Cuenta Modificada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                WdwCuenta.Hide();
                LlenaFormPanelCuentas(int.Parse(this.hdnIdColectiva.Text));
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Añadir Cuenta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Añadir Cuenta", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Contrato con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        /// <param name="ClaveColectiva">Clave de la colectiva</param>
        protected void LlenaFormPanelContrato(Int64 IdColectiva, String ClaveTipoColectiva)
        {
            try
            {
                if (ClaveTipoColectiva == "CCM")
                {
                    FormPanelContrato.Disabled = false;

                    StoreValoresContrato.DataSource = DAOAdministrarColectivas.ListaValoresContratoSinAsignar(
                        IdColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    StoreValoresContrato.DataBind();

                    LlenaGridParamsContrato(IdColectiva);
                }
                else
                {
                    FormPanelContrato.Disabled = true;
                }
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Contrato", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Contrato", ex.Message).Show();
            }
        }
        
        /// <summary>
        /// Establece los valores del Grid ParamsContrato
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        protected void LlenaGridParamsContrato(Int64 ID_Colectiva)
        {
            try
            {
                StoreParamsContrato.RemoveAll();

                StoreParamsContrato.DataSource =
                    DAOAdministrarColectivas.ConsultaValoresContratoColectiva(ID_Colectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreParamsContrato.DataBind();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw new Exception("PreparaPropertyGridContrato()", ex);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Clonar Parámetros de la pestaña de Contrato
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnClonarParams_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                LNAdministrarColectivas.ClonaParametrosContrato(
                    Convert.ToInt64(cBoxColectivaOrigen.SelectedItem.Value), IdColectiva, this.Usuario);

                X.Msg.Notify("", "Contrato Clonado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaFormPanelContrato(IdColectiva, "CCM");
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Clonar Parámetros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Clonar Parámetros", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Valor de la pestaña de Contrato
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddValor_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                string resp = LNAdministrarColectivas.AgregaParametroAContrato(
                    int.Parse(cBoxValoresContrato.SelectedItem.Value), IdColectiva, this.Usuario);

                cBoxValoresContrato.ClearValue();

                if (resp.ToUpper().Contains("ERROR"))
                {
                    X.Msg.Notify("Añadir Valor", "<b>" + resp + "</b> <br /> <br /> ").Show();
                }
                else
                {
                    LlenaFormPanelContrato(IdColectiva, "CCM");

                    X.Msg.Notify("", "Valor Añadido" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Añadir Valor", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Añadir Valor", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros de contrato
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParametros(object sender, DirectEventArgs e)
        {
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
                        case "ID_ValorContrato": unParametro.ID_ValorContrato = int.Parse(column.Value); break;
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
                hdnIdParametro.Value = unParametro.ID_ValorContrato;

                switch (comando)
                {
                    case "Edit":
                        if (unParametro.Preestablecido)
                        {
                            
                            StoreColectivasContrato.RemoveAll();

                            StoreColectivasContrato.DataSource = DAOColectiva.ListaColectivasPorTipo(
                                unParametro.ID_ParametroPrestablecido, "", this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                            StoreColectivasContrato.DataBind();

                            cBoxValorParametro.Value = unParametro.Valor;
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

                        LNAdministrarColectivas.BorraParametroAColectiva(unParametro.ID_ValorContrato,
                            IdColectiva, this.Usuario);

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridParamsContrato(IdColectiva);
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Acción", ex.Message).Show();
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
            try
            {
                ActualizaValorParametroContrato();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualización de Parámetros", ex.Message).Show();
            }
        }

        /// <summary>
        /// Solicita la actualización del valor del parámetro de contrato en el Autorizador
        /// </summary>
        protected void ActualizaValorParametroContrato()
        {
            Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
            Parametro elParametro = new Parametro();
            String Origen = hdnOrigen.Value.ToString();

            elParametro.ID_Colectiva = IdColectiva;
            elParametro.ID_Parametro = int.Parse(hdnIdParametro.Value.ToString());
            elParametro.Valor = Origen == "CMB" ? cBoxValorParametro.SelectedItem.Value : txtValorParametro.Text;
            elParametro.ID_ParametroPrestablecido = Origen == "CMB" ?
                int.Parse(cBoxValorParametro.SelectedItem.Value) : -1;

            LNAdministrarColectivas.ModificaValorParametro(elParametro, this.Usuario);

            WdwValorParametroTexto.Hide();
            WdwValorParametroCombo.Hide();

            X.Msg.Notify("Parámetros", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

            LlenaGridParamsContrato(IdColectiva);
        }

        /// <summary>
        /// Llena el panel de Parámetros Extra con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        protected void LlenaFormPanelParamsExtra(Int64 IdColectiva)
        {
            try
            {
                cBoxParamsExtra.Clear();
                StoreParamsExtra.RemoveAll();

                StoreParamsExtra.DataSource = DAOAdministrarColectivas.ListaParamsExtraSinAsignar(
                    IdColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreParamsExtra.DataBind();

                LlenaGridParamsExtra(IdColectiva);
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Parámetros Extra", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Parámetros Extra", ex.Message).Show();
            }
        }

        /// <summary>
        /// Establece los valores del Grid ParamsExtra (parámetros asignados a la colectiva)
        /// </summary>
        /// <param name="ID_Colectiva">Identificador de la colectiva</param>
        protected void LlenaGridParamsExtra(Int64 ID_Colectiva)
        {
            try
            {
                StoreGridParamsExtra.RemoveAll();

                StoreGridParamsExtra.DataSource = DAOAdministrarColectivas.ConsultaParamsExtraColectiva(ID_Colectiva, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                StoreGridParamsExtra.DataBind();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw new Exception("LlenaGridParamsExtra()", ex);
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros extra
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoParamExtra(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                hdnIdParamExtra.Text = e.ExtraParams["ID_Parametro"];
                string NombreParam = e.ExtraParams["Nombre"];
                string Descripcion = e.ExtraParams["Descripcion"];
                string Valor = e.ExtraParams["Valor"];
                bool Preestablecido = bool.Parse(e.ExtraParams["ValorPrestablecido"]);
                char[] charsToTrim = { '*', '"', '/' };

                switch (comando)
                {
                    case "Edit":
                        txtValorParamExtra.Clear();

                        if (Preestablecido)
                        {
                            StoreValoresParamExtra.RemoveAll();

                            StoreValoresParamExtra.DataSource =
                                DAOAdministrarColectivas.ListaCatalogoValoresParamsExtraColectiva(
                                int.Parse(hdnIdParamExtra.Text), this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                            StoreValoresParamExtra.DataBind();

                            cBoxValorParamExtra.Value = Valor.Trim(charsToTrim);
                            txtParamExtra_1.Text = Descripcion.Trim(charsToTrim);
                            WdwValorParamExtra_1.Title += " - " + NombreParam.Trim(charsToTrim);
                            WdwValorParamExtra_1.Show();
                        }
                        else
                        {
                            txtValorParamExtra.Text = Valor.Trim(charsToTrim);
                            txtParamExtra.Text = Descripcion.Trim(charsToTrim);
                            WdwValorParamExtra.Title += " - " + NombreParam.Trim(charsToTrim);
                            WdwValorParamExtra.Show();
                        }

                        break;

                    case "Delete":
                        Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                        LNAdministrarColectivas.BorraParametroExtraColectiva(
                            int.Parse(hdnIdParamExtra.Text), IdColectiva, this.Usuario);

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaFormPanelParamsExtra(IdColectiva);
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Acción", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir Parámetro
        /// de la pestaña de Parámetros Extra
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParam_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                LNAdministrarColectivas.AgregaParametroExtraColectiva(
                    int.Parse(cBoxParamsExtra.SelectedItem.Value), IdColectiva, this.Usuario);

                X.Msg.Notify("", "Parámetro Añadido" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaFormPanelParamsExtra(IdColectiva);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Añadir Parámetro", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Añadir Parámetro", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar Cambio de la ventana de edición de valor del parámetro extra,
        /// llamando a la actualización del parámetro en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorParamExtra_Click(object sender, EventArgs e)
        {
            try
            {
                ActualizaValorParamExtra();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualización de Parámetros", ex.Message).Show();
            }
        }

        /// <summary>
        /// Solicita la actualización del valor del parámetro al Autorizador
        /// </summary>
        protected void ActualizaValorParamExtra()
        {
            Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
            ParametroExtra elParametro = new ParametroExtra();

            elParametro.ID_Parametro = int.Parse(hdnIdParamExtra.Text);
            elParametro.Valor = String.IsNullOrEmpty(txtValorParamExtra.Text) ?
                cBoxValorParamExtra.SelectedItem.Value : txtValorParamExtra.Text;
            elParametro.ID_ParametroPrestablecido = String.IsNullOrEmpty(txtValorParamExtra.Text) ?
                int.Parse(cBoxValorParamExtra.SelectedItem.Value) : -1;

            LNAdministrarColectivas.ModificaParametroExtra(elParametro, IdColectiva, this.Usuario);

            WdwValorParamExtra.Hide();
            WdwValorParamExtra_1.Hide();

            X.Msg.Notify("Parámetros Extra", "Modificación de Parámetro <br />  <br /> <b>  E X I T O S A  </b> <br />  <br /> ").Show();

            LlenaGridParamsExtra(IdColectiva);
        }

        /// <summary>
        /// Llena el panel de Puntos Bancarios con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva</param>
        protected void LlenaFormPanelPtsBancarios(Int64 IdColectiva)
        {
            try
            {
                this.StorePtsBancarios.RemoveAll();

                this.StorePtsBancarios.DataSource = DAOAdministrarColectivas.ObtieneProductosPluginPuntosBancarios(
                    IdColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StorePtsBancarios.DataBind();

                LlenaGridParamsExtra(IdColectiva);
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Puntos Bancarios", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Puntos Bancarios", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros extra
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComandoPtsBancarios(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                int idProductoPlugin = Convert.ToInt32(e.ExtraParams["ID_ProductoPlugIn"]);

                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);
                int Activar = comando == "Activar" ? 1 : 0;
                string mensaje = Activar == 1 ? "Activado" : "Desactivado";

                LNAdministrarColectivas.ModificaEstatusProductoPluginColectiva(
                    idProductoPlugin, IdColectiva, Activar, this.Usuario);

                X.Msg.Notify("", "BIN " + mensaje + " <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaFormPanelPtsBancarios(IdColectiva);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Puntos Bancarios", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Añadir BIN de la ventana de nuevo BIN,
        /// llamando a la inserción del producto plugin en el Autorizador
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnNuevoBin_Click(object sender, EventArgs e)
        {
            try
            {
                Int64 IdColectiva = Convert.ToInt64(this.hdnIdColectiva.Text);

                LNAdministrarColectivas.CreaProductoPluginColectiva(this.txtSKU.Text, this.txtBIN.Text,
                    this.txtClave.Text, this.txtDescripcion.Text, IdColectiva, this.Usuario);

                X.Msg.Notify("", "BIN Añadido <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                this.WdwNuevoBin.Hide();
                LlenaFormPanelPtsBancarios(IdColectiva);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Añadir BIN", ex.Message).Show();
            }
        }
    }
}
