using DALCentralAplicaciones;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using DALLealtad.LogicaNegocio;
using DALLealtad.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Data;

namespace Lealtad
{
    public partial class AdministrarCadSucPrana : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administrar Cadenas y Sucursales para Prana
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreGiros_Suc.DataSource = DAOEcommercePrana.ListaGiros(this.Usuario);
                    StoreGiros_Suc.DataBind();

                    StorePresencia_Suc.DataSource = DAOEcommercePrana.ListaPresencias(this.Usuario);
                    StorePresencia_Suc.DataBind();

                    StoreClasif_Suc.DataSource = DAOEcommercePrana.ListaClasificaciones(this.Usuario);
                    StoreClasif_Suc.DataBind();

                    StorePais.DataSource = DAOEcommercePrana.ListaPaises(this.Usuario);
                    StorePais.DataBind();

                    StoreEstado.DataSource = DAOEcommercePrana.ListaEstados("MXN", this.Usuario);
                    StoreEstado.DataBind();

                    StorePerfilNSE_Suc.DataSource = DAOEcommercePrana.ListaPerfilNSE(this.Usuario);
                    StorePerfilNSE_Suc.DataBind();

                    StoreTipoEstablecimiento.DataSource = DAOEcommercePrana.ListaTipoEstablecimiento(this.Usuario);
                    StoreTipoEstablecimiento.DataBind();
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        public void ObtieneSubGiroSucEvent(object sender, EventArgs e)
        {
            ObtieneSubGiro_Suc();
        }

        public void ObtieneSubGiroInfoEvent(object sender, EventArgs e) {
            ObtieneSubGiroInfo();
        }

        public void ObtieneSubGiroInfo()
        {
            this.StoreSubGiros_Suc.RemoveAll();
            cBoxSubGiroInfoAd.Reset();

            StoreSubGiros_Suc.DataSource = DAOEcommercePrana.ListaSubGiros(this.Usuario, Convert.ToInt32(cBoxGiroInfoAd.Value));
            StoreSubGiros_Suc.DataBind();
        }

        public void ObtieneSubGiro_Suc()
        {
            this.StoreSubGiros_Suc.RemoveAll();
            cBoxSubGiro_Suc.Reset();

            StoreSubGiros_Suc.DataSource = DAOEcommercePrana.ListaSubGiros(this.Usuario, Convert.ToInt32(cBoxGiro_Suc.Value));
            StoreSubGiros_Suc.DataBind();
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Cadena
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddCadena_SucClick(object sender, EventArgs e)
        {
            try
            {
                Cadena cad = new Cadena();

                cad.ClaveCadena = txtClaveCadena_Suc.Text;
                cad.NombreComercial = txtCadena_Suc.Text;
                cad.ID_Giro = Convert.ToInt32(cBoxGiro_Suc.SelectedItem.Value);
                cad.ID_Presencia = Convert.ToInt32(cBoxPresencia_Suc.SelectedItem.Value);
                cad.Facebook = txtFacebook_Suc.Text;
                cad.Web = txtWeb_Suc.Text;

                cad.CuentaCLABE = txtCuentaCLABE_Suc.Text;
                cad.Contacto = txtContacto_Suc.Text;
                cad.TelContacto = txtTelContacto_Suc.Text;
                cad.Cargo = txtCargo_Suc.Text;
                cad.CelContacto = txtCelContacto_Suc.Text;
                cad.Correo = txtCorreo_Suc.Text;
                cad.Extracto = txtExtracto_Suc.Text;

                if (cBoxSubGiro_Suc.SelectedItem.Value == null)
                {
                    cad.ID_SubGiro = null; 
                } else
                {
                    cad.ID_SubGiro = Convert.ToInt32(cBoxSubGiro_Suc.SelectedItem.Value);
                }

                if (cBoxPerfilNSE_Suc.SelectedItem.Value == null)
                {
                    cad.ID_PerfilNSE = null;
                }
                else
                {
                    cad.ID_PerfilNSE = Convert.ToInt32(cBoxPerfilNSE_Suc.SelectedItem.Value);
                }

                if (cBoxTipoEstablecimiento_Suc.SelectedItem.Value == null)
                {
                    cad.ID_TipoEstablecimiento = null;
                }
                else
                {
                    cad.ID_TipoEstablecimiento = Convert.ToInt32(cBoxTipoEstablecimiento_Suc.SelectedItem.Value);
                }

                cad.TicketPromedio = txtTicketPromedio_Suc.Text;

                DataTable _dtNuevaCad = LNEcommercePrana.CreaNuevaCadena(cad, this.Usuario);

                string msj = _dtNuevaCad.Rows[0]["Mensaje"].ToString();
                int idNuevaCadena = Convert.ToInt32(_dtNuevaCad.Rows[0]["IdNuevaCadena"]);

                if (idNuevaCadena == -1)
                {
                    X.Msg.Alert("Nueva Cadena", msj).Show();
                }
                else
                {
                    WdwNC_Suc.Hide();

                    this.txtClaveCad.Text = txtClaveCadena_Suc.Text;
                    this.txtNombreCom.Text = "";
                    LlenaGridResultados();

                    X.Msg.Alert("Nueva Cadena", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ",
                        new MessageBoxButtonsConfig
                        {
                            Yes = new MessageBoxButtonConfig
                            {
                                Handler = "AdminCadSucPrana.CargaNuevaCadena()",
                                Text = "Aceptar"
                            }
                        }).Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nueva Cadena", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Nueva Cadena", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento clic al botón Aceptar al mensaje informativo de creación
        /// de cadena exitosa
        /// </summary>
        [DirectMethod(Namespace = "AdminCadSucPrana")]
        public void CargaNuevaCadena()
        {
            RowSelectionModel rsm = GridResultados.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Add(new SelectedRow(0));
            rsm.UpdateSelection();

            GridResultados.FireEvent("RowClick");
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            LimpiaVentanasAltaCadena();

            this.txtClaveCad.Reset();
            this.txtNombreCom.Reset();
            StoreCadenas_Suc.RemoveAll();

            LimpiaSeleccionPrevia();

            PanelCentralCadSuc.Disabled = true;
        }

        /// <summary>
        /// Limpia los controles de las ventanas pop up para añadir nuevas promociones o cadenas
        /// </summary>
        protected void LimpiaVentanasAltaCadena()
        {
            FormPanelNC_Suc.Reset();
            this.txtClaveCadena_Suc.Reset();
            this.txtCadena_Suc.Reset();
            this.cBoxGiro_Suc.Reset();
            this.cBoxPresencia_Suc.Reset();
            this.txtFacebook_Suc.Reset();
            this.txtWeb_Suc.Reset();

            this.txtCuentaCLABE_Suc.Reset();
            this.txtContacto_Suc.Reset();
            this.txtTelContacto_Suc.Reset();
            this.txtCargo_Suc.Reset();
            this.txtCelContacto_Suc.Reset();
            this.txtCorreo_Suc.Reset();
            this.txtExtracto_Suc.Reset();

            this.cBoxSubGiro_Suc.Reset();
            this.txtTicketPromedio_Suc.Reset();
            this.cBoxPerfilNSE_Suc.Reset();
            this.cBoxTipoEstablecimiento_Suc.Reset();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una promoción en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            PanelCentralCadSuc.SetTitle("_");

            FormPanelInfoAdCadena.Reset();
            btnGuardaInfoAdCadena.Disabled = true;

            this.StoreSucursales.RemoveAll();
            this.txtClaveSuc.Reset();
            _txtNombreSuc.Reset();

            FormPanelDatosSuc.Reset();
            FormPanelDatosSuc.SetTitle("Sucursal");
            FormPanelDatosSuc.Disabled = true;
            btnGuardaInfoSuc.Disabled = true;            

            FormPanelInfoAdCadena.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarCad_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultados de cadenas con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            try
            {
                DataSet dsCadenas = DAOEcommercePrana.ObtieneCadenasPorClaveONombre(
                    this.txtClaveCad.Text, this.txtNombreCom.Text, this.Usuario);

                if (dsCadenas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Cadenas", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreCadenas_Suc.DataSource = dsCadenas;
                    StoreCadenas_Suc.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cadenas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cadenas", ex.Message).Show();
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
                int IdCadena = 0;
                string ClaveCadena = "", NombreComercial = "";

                LimpiaSeleccionPrevia();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cadena = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cadena[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Cadena": IdCadena = int.Parse(column.Value); break;
                        case "ClaveCadena": ClaveCadena = column.Value; break;
                        case "NombreComercial": NombreComercial = column.Value; break;
                        default:
                            break;
                    }
                }

                hdnIdCad_Suc.Value = IdCadena;

                LlenaFormPanelInfoAd(ClaveCadena, NombreComercial);
                LlenaFormPanelSucursales();

                PanelCentralCadSuc.SetTitle(ClaveCadena + " - " + NombreComercial);
                PanelCentralCadSuc.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Cadenas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Cadenas", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="ClaveColectiva">Clave de la cadena</param>
        /// <param name="NombreComercial">Nombre comercial de la cadena</param>
        protected void LlenaFormPanelInfoAd(string ClaveCadena, string NombreComercial)
        {
            try
            {
                Cadena laCadena = DAOEcommercePrana.ObtieneInfoAdicionalCadena(
                    Convert.ToInt32(hdnIdCad_Suc.Value), this.Usuario);

                txtClaveCadInfoAd.Text = ClaveCadena;
                txtNombreComInfoAd.Text = NombreComercial;
                cBoxGiroInfoAd.Value = laCadena.ID_Giro;
                cBoxPresenciaInfoAd.Value = laCadena.ID_Presencia;
                txtFacebookInfoAd.Text = laCadena.Facebook.ToString();
                txtWebInfoAd.Text = laCadena.Web.ToString();

                txtCuentaCLABEInfoAd.Text = laCadena.CuentaCLABE;
                txtContactoInfoAd.Text = laCadena.Contacto;
                txtTelContactoInfoAd.Text = laCadena.TelContacto;
                txtCargoInfoAd.Text = laCadena.Cargo;
                txtCelContactoInfoAd.Text = laCadena.CelContacto;
                txtCorreoInfoAd.Text = laCadena.Correo;
                txtExtractoInfoAd.Text = laCadena.Extracto;

                ObtieneSubGiroInfo();

                cBoxSubGiroInfoAd.Value = laCadena.ID_SubGiro;
                txtTicketPromedioInfoAd.Text = laCadena.TicketPromedio;
                cBoxPerfilNSEInfoAd.Value = laCadena.ID_PerfilNSE;
                cBoxTipoEstablecimientoInfoAd.Value = laCadena.ID_TipoEstablecimiento;

                btnGuardaInfoAdCadena.Disabled = false;
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
        /// a la actualización de datos de la cadena en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarInfoAd_Click(object sender, EventArgs e)
        {
            try
            {
                Cadena _cadena = new Cadena();

                _cadena.ID_Cadena = Convert.ToInt32(hdnIdCad_Suc.Value);
                _cadena.NombreComercial = this.txtNombreComInfoAd.Text;
                _cadena.ID_Giro = Convert.ToInt32(this.cBoxGiroInfoAd.SelectedItem.Value);
                _cadena.ID_Presencia = Convert.ToInt32(this.cBoxPresenciaInfoAd.SelectedItem.Value);
                _cadena.Facebook = this.txtFacebookInfoAd.Text;
                _cadena.Web = this.txtWebInfoAd.Text;

                _cadena.CuentaCLABE = this.txtCuentaCLABEInfoAd.Text;
                _cadena.Contacto = this.txtContactoInfoAd.Text;
                _cadena.TelContacto = this.txtTelContactoInfoAd.Text;
                _cadena.Cargo = this.txtCargoInfoAd.Text;
                _cadena.CelContacto = this.txtCelContactoInfoAd.Text;
                _cadena.Correo = this.txtCorreoInfoAd.Text;
                _cadena.Extracto = this.txtExtractoInfoAd.Text;

                if (this.cBoxSubGiroInfoAd.SelectedItem.Value == null)
                {
                    _cadena.ID_SubGiro = null;
                }
                else
                {
                    _cadena.ID_SubGiro = Convert.ToInt32(this.cBoxSubGiroInfoAd.SelectedItem.Value);
                }

                if (this.cBoxPerfilNSEInfoAd.SelectedItem.Value == null)
                {
                    _cadena.ID_PerfilNSE = null;
                }
                else
                {
                    _cadena.ID_PerfilNSE = Convert.ToInt32(this.cBoxPerfilNSEInfoAd.SelectedItem.Value);
                }

                if (this.cBoxTipoEstablecimientoInfoAd.SelectedItem.Value == null)
                {
                    _cadena.ID_TipoEstablecimiento = null;
                }
                else
                {
                    _cadena.ID_TipoEstablecimiento = Convert.ToInt32(this.cBoxTipoEstablecimientoInfoAd.SelectedItem.Value);
                }

                _cadena.TicketPromedio = this.txtTicketPromedioInfoAd.Text;

                LNEcommercePrana.ModificaCadena(_cadena, this.Usuario);

                X.Msg.Notify("", "Cadena Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Cadena", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Cadena", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de sucursales con las asociadas a la cadena seleccionada
        /// </summary>
        protected void LlenaFormPanelSucursales()
        {
            try
            {
                DataSet dsSucursales = DAOEcommercePrana.ObtieneSucursalesPorIDCadena(
                    Convert.ToInt32(hdnIdCad_Suc.Value), this.Usuario);

                StoreSucursales.DataSource = dsSucursales;
                StoreSucursales.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Sucursales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Sucursales", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel de Sucursales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarSuc_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet _dsSuc = DAOEcommercePrana.ObtieneSucursalesPorClaveONombre(
                    Convert.ToInt32(hdnIdCad_Suc.Value), this.txtClaveSuc.Text, 
                    this._txtNombreSuc.Text, this.Usuario);

                if (_dsSuc.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Sucursales", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreSucursales.DataSource = _dsSuc;
                    StoreSucursales.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Sucursales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Sucursales", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento de selección de una fila del grid de Sucursales
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowSucursal_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdSucursal = 0;
                string ClaveSucursal = "", NombreSucursal = "";

                string json = e.ExtraParams["SucValues"];
                IDictionary<string, string>[] sucursal = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in sucursal[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Sucursal": IdSucursal = int.Parse(column.Value); break;
                        case "Clave": ClaveSucursal = column.Value; break;
                        case "Nombre": NombreSucursal = column.Value; break;
                        default:
                            break;
                    }
                }

                hdnIdSucursal.Value = IdSucursal;

                LlenaFormPanelDatosSuc(ClaveSucursal, NombreSucursal);

                FormPanelDatosSuc.Title += " " + NombreSucursal;
                FormPanelDatosSuc.Disabled = false;
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Sucursales", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                X.Msg.Alert("Sucursales", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de información adicional de la sucursal con los datos
        /// consultados a base de datos
        /// </summary>
        /// <param name="Clave">Clave de la sucursal</param>
        /// <param name="Nombre">Nombre de la sucursal</param>
        protected void LlenaFormPanelDatosSuc(string Clave, string Nombre)
        {
            try
            {
                Sucursal laSucursal = DAOEcommercePrana.ObtieneInfoAdicionalSucursal(
                    Convert.ToInt32(hdnIdSucursal.Value), this.Usuario);

                txtClaveSucInfo.Text = Clave;
                txtNombreSucInfo.Text = Nombre;
                txtDireccion.Text = laSucursal.Direccion;
                txtColonia.Text = laSucursal.Colonia;
                txtCiudad.Text = laSucursal.Ciudad;
                txtCP.Text = laSucursal.CodigoPostal;
                cBoxPais.Value = laSucursal.ClavePais;

                StoreEstado.DataSource = DAOEcommercePrana.ListaEstados(laSucursal.ClavePais, this.Usuario);
                StoreEstado.DataBind();
                cBoxEstado.Value = laSucursal.ClaveEstado;

                txtTelefono.Text = laSucursal.Telefono;
                txtLatitud.Text = laSucursal.Latitud.ToString();
                txtLongitud.Text = laSucursal.Longitud.ToString();
                cBoxSucActiva.Value = laSucursal.Activa;
                cBoxClasificacionSuc.Value = laSucursal.ID_Clasificacion;

                btnGuardaInfoSuc.Disabled = false;
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos de la Sucursal", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos de la Sucursal", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Crear Sucursal
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddSucursal_Click(object sender, EventArgs e)
        {
            try
            {
                Decimal latitud, longitud = 0;

                if (!String.IsNullOrEmpty(this.txtLatNuevaSuc.Text))
                {
                    if (!Decimal.TryParse(this.txtLatNuevaSuc.Text, out latitud))
                    {
                        X.Msg.Alert("Nueva Sucursal", "El valor ingresado como Latitud  es inválido").Show();
                        return;
                    }
                    else if (latitud < -90 || latitud > 90)
                    {
                        X.Msg.Alert("Nueva Sucursal", "El valor ingresado como Latitud  es inválido").Show();
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(this.txtLongNuevaSuc.Text))
                {
                    if (!Decimal.TryParse(this.txtLongNuevaSuc.Text, out longitud))
                    {
                        X.Msg.Alert("Nueva Sucursal", "El valor ingresado como Longitud  es inválido").Show();
                        return;
                    }
                    else if (longitud < -15069 || longitud > 15069)
                    {
                        X.Msg.Alert("Nueva Sucursal", "El valor ingresado como Longitud  es inválido").Show();
                        return;
                    }
                }

                Sucursal suc = new Sucursal();

                suc.ID_Cadena = Convert.ToInt32(this.hdnIdCad_Suc.Value);
                suc.Clave = txtClaveNuevaSuc.Text;
                suc.Nombre = txtNombreNuevaSuc.Text;
                suc.Direccion = txtDirecNuevaSuc.Text;
                suc.Colonia = txtColNuevaSuc.Text;
                suc.Ciudad = txtCdNuevaSuc.Text;
                suc.CodigoPostal = txtCpNuevaSuc.Text;
                suc.ClavePais = cBoxPaisNuevaSuc.SelectedItem.Value;
                suc.ClaveEstado = cBoxEstadoNuevaSuc.SelectedItem.Value;
                suc.Telefono = txtTelNuevaSuc.Text;
                suc.Latitud = txtLatNuevaSuc.Text;
                suc.Longitud = txtLongNuevaSuc.Text;
                suc.ID_Clasificacion = Convert.ToInt32(cBoxClasifNuevaSuc.SelectedItem.Value);                

                DataTable _dtNuevaCad = LNEcommercePrana.CreaNuevaSucursal(suc, this.Usuario);

                string msj = _dtNuevaCad.Rows[0]["Mensaje"].ToString();
                int idNuevaCadena = Convert.ToInt32(_dtNuevaCad.Rows[0]["IdNuevaSucursal"]);

                if (idNuevaCadena == -1)
                {
                    X.Msg.Alert("Nueva Sucursal", msj).Show();
                }
                else
                {
                    WdwNuevaSucursal.Hide();

                    LlenaFormPanelSucursales();

                    X.Msg.Alert("Nueva Sucursal", "<br />" + msj + "<br /> <b> E X I T O S A M E N T E </b> <br /> ").Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nueva Sucursal", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Nueva Sucursal", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel de datos de la sucursal, llamando
        /// a la actualización de datos de la misma en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaInfoSuc_Click(object sender, EventArgs e)
        {
            try
            {
                Sucursal suc = new Sucursal();

                suc.ID_Sucursal = Convert.ToInt32(this.hdnIdSucursal.Value);
                suc.Nombre = this.txtNombreSucInfo.Text;
                suc.Direccion = this.txtDireccion.Text;
                suc.Colonia = this.txtColonia.Text;
                suc.Ciudad = this.txtCiudad.Text;
                suc.CodigoPostal = this.txtCP.Text;
                suc.ClavePais = this.cBoxPais.SelectedItem.Value;
                suc.ClaveEstado = this.cBoxEstado.SelectedItem.Value;
                suc.Telefono = this.txtTelefono.Text;
                suc.Latitud = this.txtLatitud.Text;
                suc.Longitud = this.txtLongitud.Text;
                suc.Activa = Convert.ToInt32(cBoxSucActiva.SelectedItem.Value);
                suc.ID_Clasificacion = Convert.ToInt32(cBoxClasificacionSuc.SelectedItem.Value);

                LNEcommercePrana.ModificaSucursal(suc, this.Usuario);

                X.Msg.Notify("", "Sucursal Actualizada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de Sucursal", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de Sucursal", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Pais, estableciendo los Estados correspondientes al país elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceEstados(object sender, EventArgs e)
        {
            try
            {
                StoreEstado.DataSource = DAOEcommercePrana.ListaEstados(this.cBoxPais.SelectedItem.Value, this.Usuario);
                StoreEstado.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Estado", err.Mensaje()).Show();
            }

            catch (Exception )
            {
                X.Msg.Alert("Estado", "Ocurrió un error al establecer los Estados").Show();
            }
        }

        /// <summary>
        /// Controla el evento Seleccionar del combo Pais, estableciendo los Estados correspondientes al país elegido
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void EstableceEstadosNuevo(object sender, EventArgs e)
        {
            try
            {
                StoreEstado.DataSource = DAOEcommercePrana.ListaEstados(this.cBoxPaisNuevaSuc.SelectedItem.Value, this.Usuario);
                StoreEstado.DataBind();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Estado", err.Mensaje()).Show();
            }

            catch (Exception )
            {
                X.Msg.Alert("Estado", "Ocurrió un error al establecer los Estados").Show();
            }
        }
    }
}
       